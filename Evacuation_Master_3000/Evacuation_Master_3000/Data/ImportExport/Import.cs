#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using static Evacuation_Master_3000.Settings;

#endregion

namespace Evacuation_Master_3000 {
    public static class Import {
        private static object BuildingInformation { get; set; }
        private static object Target { get; set; }
        private static List<PropertyInfo> TargetInformationList { get; set; }
        private static MethodInfo AttributeConversion { get; } = typeof(Import).GetMethod("AttributeConverter");
        private static int CurrentPerson { get; set; }
        private static int CurrentFloor { get; set; }
        private static Dictionary<Enum, Tuple<Action, Func<string, object[]>, Action>> AttributeMethods { get; } = new Dictionary<Enum, Tuple<Action, Func<string, object[]>, Action>>();
        private enum ParentTypes { None, Settings, People, Person, Floor }
        private  static ParentTypes NodeParent { get; set; }
        public static ImportFloorPlanFeedBack OnImportFeedBack;

        private static void SetupImport() {
            BuildingInformation = new BuildingInformationCollection();

            if (!AttributeMethods.ContainsKey(ParentTypes.Person))
                AttributeMethods.Add(ParentTypes.Person, Tuple.Create(
                    new Action(delegate { Target = new PersonInformation(); TargetInformationList = typeof(PersonInformation).GetProperties().ToList(); }),
                    new Func<string, object[]>(value => new object[] {value}),
                    new Action(delegate { ((BuildingInformationCollection)BuildingInformation).PeopleCollection[CurrentPerson++] = (PersonInformation)Target; })));

            if (!AttributeMethods.ContainsKey(ParentTypes.Settings))
                AttributeMethods.Add(ParentTypes.Settings, Tuple.Create(
                    new Action(delegate { Target = BuildingInformation; TargetInformationList = typeof(BuildingInformationCollection).GetProperties().ToList(); }),
                    new Func<string, object[]>(value => new object[] {value}),
                    new Action(delegate { })));

            if (!AttributeMethods.ContainsKey(ParentTypes.People))
                AttributeMethods.Add(ParentTypes.People, AttributeMethods[ParentTypes.Settings]);

            if (!AttributeMethods.ContainsKey(ParentTypes.Floor))
                AttributeMethods.Add(ParentTypes.Floor, Tuple.Create(
                    new Action(delegate { Target = new FloorInformation(); TargetInformationList = typeof(FloorInformation).GetProperties().ToList(); }),
                    new Func<string, object[]>(delegate (string value) { return value.Replace(" ", "").Split(','); }),
                    new Action(delegate { ((BuildingInformationCollection)BuildingInformation).FloorCollection[CurrentFloor++] = (FloorInformation)Target; })));
        }

        internal static BuildingInformationCollection ImportBuilding(string filePath) {
            /* BuildingInformation is what we're going to return when the building has been sucessfully imported. 
            Because of how the import mechanisms work, BuildingInformation has to be instantiated and visible for the entire class.*/
            if (BuildingInformation == null)
                SetupImport();

            using (XmlReader reader = XmlReader.Create(File.OpenRead(filePath))) {
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Whitespace || reader.Value == "\n")
                        continue;

                    if (reader.HasAttributes) {
                        if (reader.IsStartElement()) {
                            ParentTypes parentPlaceholder;
                            NodeParent = (Enum.TryParse(reader.Name, out parentPlaceholder) ? parentPlaceholder : ParentTypes.None);

                            if (AttributeMethods.ContainsKey(NodeParent))
                                AttributeMethods[NodeParent].Item1?.DynamicInvoke();
                            else
                                continue;
                        }

                        /* Looping through all attributes of node/element */
                        for (int i = 0; i < reader.AttributeCount; i++) {
                            reader.MoveToNextAttribute();

                            string name = reader.Name, value = reader.Value;

                            var currentMemberInfo = TargetInformationList.FirstOrDefault(p => p.Name == name);
                            if (currentMemberInfo == default(PropertyInfo)) continue; /* Like a null-reference check-up */

                            object[] values = AttributeMethods[NodeParent].Item2?.DynamicInvoke(value) as object[];

                            if (values == null) continue;

                            foreach (string item in values)
                                currentMemberInfo.SetValue(Target, AttributeConversion.MakeGenericMethod(currentMemberInfo.PropertyType).Invoke(null, new object[] { item }));
                        }
                        AttributeMethods[NodeParent].Item3?.DynamicInvoke();
                    }
                };
            }

            /* We're finished, return the results for further working */
            return (BuildingInformationCollection)BuildingInformation;
        }

        public static T AttributeConverter<T>(string input)
        {
            string value = input;
            var result = TypeDescriptor.GetConverter(typeof(T));
            if (typeof(T) == typeof(double) && value.Contains('.'))
                value = input.Replace('.', ',');
            return (T)result.ConvertFrom(value);
        }

        internal static void EffectuateFloorPlanSettings(BuildingInformationCollection buildingInformation, ref IFloorPlan floorPlan, ref Dictionary<int, Person> allPeople) {
            const int freeTypeIntRepresentation = (int)Tile.Types.Free;
            /* Step 1: Subject the types of each tile to the type of the tile in the BuildingInformationCollection */
            for (int z = 0; z < buildingInformation.Floors; z++) {
                for (int y = 0; y < buildingInformation.Height; y++) {
                    for (int x = 0; x < buildingInformation.Width; x++) {
                        int type = int.Parse(buildingInformation.FloorCollection[z].Rows[y][x].ToString());
                        /* If the point is already marked as free, continue - no need to force-convert it to free */
                        if (type == freeTypeIntRepresentation)
                            continue;

                        Tile.Types newType = (Tile.Types)type;

                        if (newType == Tile.Types.Person) {
                            PersonInformation personInfo = buildingInformation.PeopleCollection.FirstOrDefault(p => p.Position == Coordinate(x, y, z));
                            allPeople.Add(personInfo.ID, new Person(personInfo.ID, personInfo.MovementSpeed, floorPlan.Tiles[personInfo.Position] as BuildingBlock));
                        }

                        floorPlan.Tiles[Coordinate(x, y, z)].Type = newType;
                    }
                }
            }
        }
    }
}