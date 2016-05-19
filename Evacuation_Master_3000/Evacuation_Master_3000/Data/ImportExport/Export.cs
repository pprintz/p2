#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using static Evacuation_Master_3000.Settings;
#endregion

namespace Evacuation_Master_3000 {
    public static class Export {
        public static ExportFloorPlanFeedBack OnExportFeedBack;
        public enum ExportOutcomes { Succes, Failure }
        public static void ExportBuilding(string filePath, IFloorPlan floorPlan, Dictionary<int, Person> allPeople) {
            string exportFeedbackMessage = $"Successfully exported the grid to destination: {filePath}";
            ExportOutcomes outcome = ExportOutcomes.Succes;
            try {
                string xmlPath = Path.GetFileNameWithoutExtension(filePath) + ".xml";
                using (XmlWriter writer = XmlWriter.Create(xmlPath)) {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("BuildingPlan");

                    writer.WriteStartElement("Settings");
                    writer.WriteAttributeString("Width", floorPlan.Width.ToString());
                    writer.WriteAttributeString("Height", floorPlan.Height.ToString());
                    writer.WriteAttributeString("Floors", floorPlan.FloorAmount.ToString());
                    writer.WriteAttributeString("Description", floorPlan.Description);
                    writer.WriteEndElement();

                    writer.WriteStartElement("People");
                    if (allPeople.Count > 0) {
                        writer.WriteAttributeString("NumberOfPeople", allPeople.Count.ToString());
                        foreach (Person item in allPeople.Values) {
                            writer.WriteStartElement("Person");
                            writer.WriteAttributeString("ID", item.ID.ToString());
                            writer.WriteAttributeString("Position", Coordinate(item.Position));
                            writer.WriteAttributeString("MovementSpeed", item.MovementSpeed.ToString());
                            writer.WriteEndElement();
                        }
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("FloorPlan");
                    for (int currentFloor = 0; currentFloor < floorPlan.FloorAmount; currentFloor++) {
                        writer.WriteStartElement("Floor");
                        writer.WriteAttributeString("Level", currentFloor.ToString());
                        writer.WriteAttributeString("Header", floorPlan.Headers[currentFloor]); 

                        StringBuilder rowAttributeBuilder = new StringBuilder();
                        StringBuilder rowBuilder = new StringBuilder();

                        for (int currentY = 0; currentY < floorPlan.Height; currentY++) {
                            for (int currentX = 0; currentX < floorPlan.Width; currentX++) {
                                string typeString = ((int)floorPlan.Tiles[Coordinate(currentX, currentY, currentFloor)].Type).ToString();
                                rowBuilder.Append(typeString);
                            }
                            rowAttributeBuilder.Append(rowBuilder + ", ");
                            rowBuilder.Clear();
                        }

                        writer.WriteAttributeString("RowInformation", rowAttributeBuilder.ToString().TrimEnd(' ').TrimEnd(','));
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }

                File.Copy(xmlPath, filePath);
                File.Delete(xmlPath);
            }
            catch (Exception e) {
                outcome = ExportOutcomes.Failure;
                exportFeedbackMessage = $"Error while exporting! \n Error message: {e.Message}\n\nDestination: {filePath}";
            }
            finally {
                OnExportFeedBack?.Invoke(exportFeedbackMessage, outcome);
            }
        }
    }
}