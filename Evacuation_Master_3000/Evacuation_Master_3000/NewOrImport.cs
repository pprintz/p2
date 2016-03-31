using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.IO;
using Microsoft.Win32;
using static Evacuation_Master_3000.ImportExportSettings;

namespace Evacuation_Master_3000 {
    class NewOrImport {
        public NewOrImport(Canvas canvas, Grid grid, GridNewOrLoadWindow.NewOrImport window) {
            CurrentCanvas = canvas;
            NewGrid = grid;
            NewOrImportWindow = new GridNewOrLoadWindow(window);
            NewOrImportWindow.BrowseButton.Click += Browse;
            NewOrImportWindow.newBrowse.Click += Browse;
            NewOrImportWindow.ImportGridButton.Click += ImportGrid;
            NewOrImportWindow.CreateGridButton.Click += CreateGrid;
            NewOrImportWindow.ShowDialog();
        }

        public GridNewOrLoadWindow NewOrImportWindow { get; }
        public Canvas CurrentCanvas { get; }
        private Grid NewGrid { get; set; }
        private bool ImportedGrid { get; set; }
        private int ImportedGridWidth { get; set; }
        private int ImportedGridHeight { get; set; }
        private string ImportedGridHeader { get; set; }
        private string ImportedGridDescription { get; set; }
        private List<string> ImportedRows = new List<string>();

        private bool ValidateInputs() {
            string message, caption;
            /* First check: Are either or both width and/or height <= 0? */
            if (NewOrImportWindow.GridWidth <= 0 || NewOrImportWindow.GridHeight <= 0) {
                caption = "Fix grid width/height";
                message = "Error! Could not create a grid with the specified width/height. Please resolve the following:\n";
                message += NewOrImportWindow.GridWidth <= 0 ? "\t - Width should be a positive integer\n" : string.Empty;
                message += NewOrImportWindow.GridHeight <= 0 ? "\t - Height should be a positive integer\n" : string.Empty;
                MessageBox.Show(message, caption);
                return false;
            }

            /* Second check: ? */

            /* Third check: Profit! */

            return true;
        }

        private void ImportGrid(object sender, RoutedEventArgs e) {
           
        }

        private void CreateGrid() { CreateGrid(null, null); }
        private void CreateGrid(object sender, RoutedEventArgs e) {
            /* Was this method called through the press of a button ? */
            if(sender != null) {
                /* Set all variables to the grid accordingly */
                ImportedGridWidth = NewOrImportWindow.GridWidth;
                ImportedGridHeight = NewOrImportWindow.GridHeight;
                ImportedGridHeader = NewOrImportWindow.NewHeader;
                ImportedGridDescription = NewOrImportWindow.Description;
            }

            NewGrid.Header = ImportedGridHeader;
            NewGrid.Description = ImportedGridDescription;
            NewGrid.CreateGrid(CurrentCanvas, ImportedGridWidth, ImportedGridHeight);

            if (ImportedGrid)
                ImportGridSettings();

            OnSuccess();
        }

        private void Browse(object sender, RoutedEventArgs e) {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Grid files|*" + Extension;
            if(open.ShowDialog() == true) {
                ReadGridFile(open.FileName);
            }
        }

        private void ReadGridFile(string path) {
            /* Der bliver aktuelt ikke taget højde for corrupt files - 
            der skal tages forbehold for, hvis det ikke er lykkedes at læse width, height og tilsvarende Row-informationer*/
            try {
                using(StreamReader sr = new StreamReader(path)){
                    string line;
                    while((line = sr.ReadLine()) != null) {
                        DeconstructSettingsFromFile(line);
                    }

                    ImportedGrid = true;
                    CreateGrid();
                }
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }

        }
        
        private void DeconstructSettingsFromFile(string line) {
            ImportExportSettings.FileSettings setting = ImportExportSettings.ExtractSettingFromFile(line);
            if (setting == FileSettings.NA)
                return;

            string value = ImportExportSettings.ExtractValueFromLine(line);
            switch (setting) {
                case ImportExportSettings.FileSettings.Width:
                    ImportedGridWidth = int.Parse(value);
                    break;
                case ImportExportSettings.FileSettings.Height:
                    ImportedGridHeight = int.Parse(value);
                    break;
                case ImportExportSettings.FileSettings.Header:
                    ImportedGridHeader = value;
                    break;
                case ImportExportSettings.FileSettings.Description:
                    ImportedGridDescription = value;
                    break;
                case ImportExportSettings.FileSettings.Row:
                    ImportedRows.Add(value);
                    break;
                default:
                    break;
            }

        }

        private void ImportGridSettings() {
            /* The grid types is subjected as a two-dimensional int array - aka matrix */
            int[,] gridMatrix = new int[ImportedGridWidth, ImportedGridHeight];
            TransformGridMatrix(ref gridMatrix);

            const int FreeAsInt = (int)BuildingBlock.ElevationTypes.Free;
            BuildingBlock.ElevationTypes newType;

            for (int x = 0; x < ImportedGridWidth; x++) {
                for (int y = 0; y < ImportedGridHeight; y++) {
                    int type = gridMatrix[x, y];
                    /* If the point is already marked as free, continue - no need to force-convert it to free */
                    if (type == FreeAsInt)
                        continue;
                    
                    newType = (BuildingBlock.ElevationTypes)type;

                    NewGrid.AllPoints[Coordinate(x, y)].Elevation = newType;
                }
            }

        }

        private void TransformGridMatrix(ref int[,] matrix) {
            for (int x = 0; x < ImportedRows.Count; x++) {
                int y = 0;
                foreach (char type in ImportedRows[x]) {
                    matrix[x, y] = int.Parse(type.ToString());
                    y++;
                }
            }
        }

        private void OnSuccess() {
            NewOrImportWindow.Close();
        }
    }
}
