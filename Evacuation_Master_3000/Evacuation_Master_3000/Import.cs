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
        static object BuildingInformation { get; set; }
        static object Target { get; set; }
        static List<PropertyInfo> TargetInformationList { get; set; }
        static MethodInfo AttributeConversion { get; } = typeof(Import).GetMethod("AttributeConverter");
        static int CurrentPerson { get; set; }
        static int CurrentFloor { get; set; }
        static Dictionary<Enum, Tuple<Action, Func<string, object[]>, Action>> AttributeMethods { get; } = new Dictionary<Enum, Tuple<Action, Func<string, object[]>, Action>>();
        enum ParentTypes { None, Settings, People, Person, Floor }
        static ParentTypes NodeParent { get; set; }
        public static ImportFloorPlanFeedBack OnImportFeedBack;

        private static void SetupImport() {
            BuildingInformation = new BuildingInformationCollection();

            if (!AttributeMethods.ContainsKey(ParentTypes.Person))
                AttributeMethods.Add(ParentTypes.Person, Tuple.Create(
                    new Action(delegate () { Target = new PersonInformation(); TargetInformationList = typeof(PersonInformation).GetProperties().ToList(); }),
                    new Func<string, object[]>(delegate (string value) { return new object[] { value }; }),
                    new Action(delegate () { ((BuildingInformationCollection)BuildingInformation).PeopleCollection[CurrentPerson++] = (PersonInformation)Target; })));

            if (!AttributeMethods.ContainsKey(ParentTypes.Settings))
                AttributeMethods.Add(ParentTypes.Settings, Tuple.Create(
                    new Action(delegate () { Target = BuildingInformation; TargetInformationList = typeof(BuildingInformationCollection).GetProperties().ToList(); }),
                    new Func<string, object[]>(delegate (string value) { return new object[] { value }; }),
                    new Action(delegate () { })));

            if (!AttributeMethods.ContainsKey(ParentTypes.People))
                AttributeMethods.Add(ParentTypes.People, AttributeMethods[ParentTypes.Settings]);

            if (!AttributeMethods.ContainsKey(ParentTypes.Floor))
                AttributeMethods.Add(ParentTypes.Floor, Tuple.Create(
                    new Action(delegate () { Target = new FloorInformation(); TargetInformationList = typeof(FloorInformation).GetProperties().ToList(); }),
                    new Func<string, object[]>(delegate (string value) { return value.Replace(" ", "").Split(','); }),
                    new Action(delegate () { ((BuildingInformationCollection)BuildingInformation).FloorCollection[CurrentFloor++] = (FloorInformation)Target; })));
        }

        internal static BuildingInformationCollection ImportBuilding(string filePath) {
            /* BuildingInformation is what we're going to return when the building has been sucessfully imported. 
            Because of how the import mechanisms work, BuildingInformation has to be instantiated and visible for the entire class.*/
            if (BuildingInformation == null)
                SetupImport();

            if (!File.Exists(filePath))
                throw new ImportExportException($"Error while importing building:\n\t-No file with path {filePath} exists!");

            using (XmlReader reader = XmlReader.Create(File.OpenRead(filePath))) {
                while (reader.Read()) {
                    if (reader.NodeType == XmlNodeType.Whitespace || reader.Value == "\n")
                        continue;

                    if (reader.HasAttributes) {
                        if (reader.IsStartElement()) {
                            ParentTypes parentPlaceholder;
                            NodeParent = (Enum.TryParse(reader.Name, out parentPlaceholder) == true ? parentPlaceholder : ParentTypes.None);

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
                            //((PropertyInfo)info).SetValue(target, AttributeConversion.MakeGenericMethod(PropertyType)?.Invoke(this, new object[] { value })); }),
                            foreach (string item in values)
                                currentMemberInfo.SetValue(Target, AttributeConversion.MakeGenericMethod(currentMemberInfo.PropertyType)?.Invoke(null, new object[] { item }));
                        }
                        AttributeMethods[NodeParent].Item3?.DynamicInvoke();
                    }
                };
            }

            OnImportFeedBack?.Invoke();
            /* We're finished, return the results for further working */
            return (BuildingInformationCollection)BuildingInformation;
        }

        public static T AttributeConverter<T>(string input) {
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


        //internal class NewOrImport
        //{
        //    private readonly List<string> _importedRows = new List<string>();
        //    private GridNewOrLoadWindow NewOrImportWindow { get; }
        //    private Canvas CurrentCanvas { get; }
        //    private Grid_new NewGrid { get; }
        //    private bool ImportedGrid { get; set; }
        //    private int ImportedGridWidth { get; set; }
        //    private int ImportedGridHeight { get; set; }
        //    private string ImportedGridHeader { get; set; }
        //    private string ImportedGridDescription { get; set; }

        //    /// <summary>
        //    /// Creates a new grid on a given canvas. It can load previously created grids or simply create a new blank one.
        //    /// </summary>
        //    /// <param name="canvas">Takes a WPF Canvas to draw on.</param>
        //    /// <param name="grid">Special Grid class which contains elements to be drawn.</param>
        //    /// <param name="window">Opens a new window with options concerning the creation of a new grid.</param>
        //    public NewOrImport(Canvas canvas, Grid_new grid, GridNewOrLoadWindow.NewOrImport window)
        //    {
        //        CurrentCanvas = canvas;
        //        NewGrid = grid;
        //        NewOrImportWindow = new GridNewOrLoadWindow(window);
        //        NewOrImportWindow.BrowseButton.Click += Browse;
        //        NewOrImportWindow.NewBrowse.Click += Browse;
        //        NewOrImportWindow.ImportGridButton.Click += ImportGrid;
        //        NewOrImportWindow.CreateGridButton.Click += CreateGrid;
        //        NewOrImportWindow.ShowDialog();
        //    }

        //    private void ImportGrid(object sender, RoutedEventArgs e)
        //    {
        //        // The purpose of this is to show some information about the grid before actually loading it.
        //    }

        //    private void CreateGrid(object sender = null, RoutedEventArgs e = null)
        //    {
        //        /* Was this method called through the press of a button ? */
        //        if (sender != null)
        //        {
        //            /* Set all variables to the grid accordingly */
        //            ImportedGridWidth = NewOrImportWindow.GridWidth;
        //            ImportedGridHeight = NewOrImportWindow.GridHeight;
        //            ImportedGridHeader = NewOrImportWindow.NewHeader;
        //            ImportedGridDescription = NewOrImportWindow.Description;
        //        }

        //        NewGrid.Header = ImportedGridHeader;
        //        NewGrid.Description = ImportedGridDescription;
        //        NewGrid.CreateGrid(CurrentCanvas, ImportedGridWidth, ImportedGridHeight);

        //        if (ImportedGrid)
        //            ImportGridSettings();

        //        OnSuccess();
        //    }

        //    private void Browse(object sender, RoutedEventArgs e)
        //    {
        //        OpenFileDialog open = new OpenFileDialog {Filter = "Grid files|*" + Extension};
        //        if (open.ShowDialog() == true)
        //        {
        //            ReadGridFile(open.FileName);
        //        }
        //    }

        //    private void ReadGridFile(string path)
        //    {
        //        /* Der bliver aktuelt ikke taget højde for corrupt files - 
        //        der skal tages forbehold for, hvis det ikke er lykkedes at læse width, height og tilsvarende Row-informationer*/
        //        try
        //        {
        //            using (StreamReader sr = new StreamReader(path))
        //            {
        //                string line;
        //                while ((line = sr.ReadLine()) != null)
        //                {
        //                    DeconstructSettingsFromFile(line);
        //                }

        //                ImportedGrid = true;
        //                CreateGrid();
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            MessageBox.Show(e.Message);
        //        }
        //    }

        //    private void DeconstructSettingsFromFile(string line)
        //    {
        //        FileSettings setting = ExtractSettingFromFile(line);
        //        if (setting == FileSettings.NA)
        //            return;

        //        string value = ExtractValueFromLine(line);
        //        switch (setting)
        //        {
        //            case FileSettings.Width:
        //                ImportedGridWidth = int.Parse(value);
        //                break;
        //            case FileSettings.Height:
        //                ImportedGridHeight = int.Parse(value);
        //                break;
        //            case FileSettings.Header:
        //                ImportedGridHeader = value;
        //                break;
        //            case FileSettings.Description:
        //                ImportedGridDescription = value;
        //                break;
        //            case FileSettings.Row:
        //                _importedRows.Add(value);
        //                break;
        //        }
        //    }

        //    private void ImportGridSettings()
        //    {
        //        /* The grid types is subjected as a two-dimensional int array - aka matrix */
        //        int[,] gridMatrix = new int[ImportedGridWidth, ImportedGridHeight];
        //        TransformGridMatrix(ref gridMatrix);

        //        const int freeAsInt = (int) BuildingBlock.Types.Free;

        //        for (int x = 0; x < ImportedGridWidth; x++)
        //        {
        //            for (int y = 0; y < ImportedGridHeight; y++)
        //            {
        //                int type = gridMatrix[x, y];
        //                /* If the point is already marked as free, continue - no need to force-convert it to free */
        //                if (type == freeAsInt)
        //                    continue;

        //                Tile.Types newType = (Tile.Types) type;

        //                NewGrid.AllPoints[Coordinate(x, y)].Type = newType;
        //            }
        //        }
        //    }

        //    private void TransformGridMatrix(ref int[,] matrix)
        //    {
        //        for (int x = 0; x < _importedRows.Count; x++)
        //        {
        //            int y = 0;
        //            foreach (char type in _importedRows[x])
        //            {
        //                matrix[x, y] = int.Parse(type.ToString());
        //                y++;
        //            }
        //        }
        //    }

        //    private void OnSuccess()
        //    {
        //        NewOrImportWindow.Close();
        //    }

        //    //private bool ValidateInputs()
        //    //{
        //    //    /* First check: Are either or both width and/or height <= 0? */
        //    //    if (NewOrImportWindow.GridWidth <= 0 || NewOrImportWindow.GridHeight <= 0)
        //    //    {
        //    //        string caption = "Fix grid width/height";
        //    //        string message = "Error! Could not create a grid with the specified width/height. Please resolve the following:\n";
        //    //        message += NewOrImportWindow.GridWidth <= 0 ? "\t - Width should be a positive integer\n" : string.Empty;
        //    //        message += NewOrImportWindow.GridHeight <= 0
        //    //            ? "\t - Height should be a positive integer\n"
        //    //            : string.Empty;
        //    //        MessageBox.Show(message, caption);
        //    //        return false;
        //    //    }

        //    //    /* Second check: ? */

        //    //    /* Third check: Profit! */

        //    //    return true;
        //    //}
        //}

        //internal class Import
        //{
        //    public Import(string filePath)
        //    {
        //        ReadFromFile(filePath);
        //    }

        //    public int Width { get; private set; }
        //    public int Height { get; private set; }
        //    public int FloorAmount { get; private set; }
        //    public string[] Headers { get; private set; }
        //    public string Description { get; private set; }
        //    private readonly List<string> floorplanMatrix = new List<string>();
        //    private int[,,] floorPlanMatrix;
        //    private int CurrentFloor;
        //    private int CurrentRow;

        //    private void ReadFromFile(string path)
        //    {
        //        /* Der bliver aktuelt ikke taget højde for corrupt files - 
        //        der skal tages forbehold for, hvis det ikke er lykkedes at læse width, height og tilsvarende Row-informationer*/

        //        using (StreamReader sr = new StreamReader(path))
        //        {
        //            string line;
        //            while ((line = sr.ReadLine()) != null)
        //            {
        //                DeconstructSettingsFromFile(line);
        //            }

        //            //////////////////////////////ImportedGrid = true;
        //            //////////////////////////////CreateGrid();
        //        }
        //    }
        //    private void DeconstructSettingsFromFile(string line)
        //    {
        //        FileSettings setting = ExtractSettingFromFile(line);
        //        if (setting == FileSettings.NA)
        //            return;

        //        string value = ExtractValueFromLine(line);
        //        switch (setting)
        //        {
        //            case FileSettings.Width:
        //                Width = int.Parse(value);
        //                break;
        //            case FileSettings.Height:
        //                Height = int.Parse(value);
        //                break;
        //            case FileSettings.FloorAmount:
        //                FloorAmount = int.Parse(value);
        //                break;
        //            case FileSettings.Header:
        //                /* Skal udvide Headers efter behov. Concat?? */
        //                //header = value;
        //                break;
        //            case FileSettings.Description:
        //                Description = value;
        //                break;
        //            case FileSettings.CurrentFloor:
        //                CurrentFloor = int.Parse(value);
        //                break;
        //            case FileSettings.CurrentRow:
        //                CurrentRow = int.Parse(value);
        //                break;
        //            case FileSettings.Row:
        //                if(floorPlanMatrix == null)
        //                    floorPlanMatrix = new int[FloorAmount, Width, Height];
        //                int width = 0;
        //                foreach (char character in value)
        //                {
        //                    floorPlanMatrix[CurrentFloor, width++, CurrentRow] = int.Parse(character.ToString());
        //                }
        //                break;
        //        }
        //    }

        //    public void ImportFloorPlanSettings(IFloorPlan floorPlan)
        //    {
        //        /* The grid types is subjected as a three-dimensional int array - aka matrix */

        //        const int freeAsInt = (int)BuildingBlock.Types.Free;
        //        for(int z = 0; z < FloorAmount; z++)
        //        {
        //            for (int x = 0; x < Width; x++)
        //            {
        //                for (int y = 0; y < Height; y++)
        //                {
        //                    int type = floorPlanMatrix[z, x, y];
        //                    /* If the point is already marked as free, continue - no need to force-convert it to free */
        //                    if (type == freeAsInt)
        //                        continue;

        //                    Tile.Types newType = (Tile.Types)type;

        //                    floorPlan.Tiles[Coordinate(x, y, z)].Type = newType;
        //                }
        //            }
        //        }
        //    }
        //}
    }
}