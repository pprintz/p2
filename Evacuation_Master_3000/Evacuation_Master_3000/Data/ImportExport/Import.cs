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
        /* AttributeConverter-method is a generic method, but calling this method with a type not known at compile-time requries re-instantiation of the method.
        This is achieved by getting a reference to the method as MethodInfo. */
        private static MethodInfo AttributeConversion { get; } = typeof(Import).GetMethod("AttributeConverter");
        private static int CurrentPerson { get; set; }
        private static int CurrentFloor { get; set; }
        /* AttributeMethods is a Dictionary containing 3 kinds of functionality for each key.
        The value of the dictionary is a Tuple with the 3 funcitonalities:
            - First functionality is a kind of "preperation"-step, where the PropertyInfo-list TargetInformationList is filled with info of a certain class/struct's properties 
            - Second functionality is a value conversion. This converts a value object to object[] and may do some working with the value before returning
            - Third functionality is for those attributes, where some extra code needs to be run at the end of a value entry. This can be incrementing a person/floor etc */
        private static Dictionary<Enum, Tuple<Action, Func<string, object[]>, Action>> AttributeMethods { get; } = new Dictionary<Enum, Tuple<Action, Func<string, object[]>, Action>>();
        private enum ParentTypes { None, Settings, People, Person, Floor }
        private  static ParentTypes NodeParent { get; set; }
        public static ImportFloorPlanFeedBack OnImportFeedBack;

        private static void SetupImport() {
            BuildingInformation = new BuildingInformationCollection();
            /* This is where behavior for each .grid-attribute is being inserted into the dictionary. */
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
                        /* A start element marks a new parent */
                        if (reader.IsStartElement()) {
                            ParentTypes parentPlaceholder;
                            NodeParent = (Enum.TryParse(reader.Name, out parentPlaceholder) ? parentPlaceholder : ParentTypes.None);

                            /* If whatever parent read is known, run the "preperation" for that parent */
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
                            
                            /* If necessary, do some work on the value before converting it to object[] for later use */
                            object[] values = AttributeMethods[NodeParent].Item2?.DynamicInvoke(value) as object[];

                            if (values == null) continue; 

                            /* Loop through values, setting the value to the correct property in the target class/struct. 
                            Type conversion is handled through AttributeConversion:
                                - As stated previously, AttributeConverter is a generic method, for which types are not known at compile time.
                                    To get around this, an instance of the method is created, in which the type is provided.
                                The return value from the method is the value for the set-method of the property, for which we are trying to assign a value. */
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

        /* AttributeConverter converts a string input to a given type */
        public static T AttributeConverter<T>(string input)
        {
            string value = input;
            var result = TypeDescriptor.GetConverter(typeof(T));
            if (typeof(T) == typeof(double)) {
                /* This is for conversion of doubles across computers with different languages (and thereby different number decimal seperator) 
                This basically forces whatever number decimal seperator is inputted to be converted to the number decimal seperator supported by the OS language */
                value = input.Replace(",", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                value = input.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            }
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