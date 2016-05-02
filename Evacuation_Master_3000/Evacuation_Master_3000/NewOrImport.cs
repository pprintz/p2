#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using static Evacuation_Master_3000.ImportExportSettings;

#endregion

namespace Evacuation_Master_3000
{
    internal class NewOrImport
    {
        private readonly List<string> _importedRows = new List<string>();
        private GridNewOrLoadWindow NewOrImportWindow { get; }
        private Canvas CurrentCanvas { get; }
        private Grid NewGrid { get; }
        private bool ImportedGrid { get; set; }
        private int ImportedGridWidth { get; set; }
        private int ImportedGridHeight { get; set; }
        private string ImportedGridHeader { get; set; }
        private string ImportedGridDescription { get; set; }

        /// <summary>
        /// Creates a new grid on a given canvas. It can load previously created grids or simply create a new blank one.
        /// </summary>
        /// <param name="canvas">Takes a WPF Canvas to draw on.</param>
        /// <param name="grid">Special Grid class which contains elements to be drawn.</param>
        /// <param name="window">Opens a new window with options concerning the creation of a new grid.</param>
        public NewOrImport(Canvas canvas, Grid grid, GridNewOrLoadWindow.NewOrImport window)
        {
            CurrentCanvas = canvas;
            NewGrid = grid;
            NewOrImportWindow = new GridNewOrLoadWindow(window);
            NewOrImportWindow.BrowseButton.Click += Browse;
            NewOrImportWindow.NewBrowse.Click += Browse;
            NewOrImportWindow.ImportGridButton.Click += ImportGrid;
            NewOrImportWindow.CreateGridButton.Click += CreateGrid;
            NewOrImportWindow.ShowDialog();
        }

        private void ImportGrid(object sender, RoutedEventArgs e)
        {
            // The purpose of this is to show some information about the grid before actually loading it.
        }

        private void CreateGrid(object sender = null, RoutedEventArgs e = null)
        {
            /* Was this method called through the press of a button ? */
            if (sender != null)
            {
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

        private void Browse(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog {Filter = "Grid files|*" + Extension};
            if (open.ShowDialog() == true)
            {
                ReadGridFile(open.FileName);
            }
        }

        private void ReadGridFile(string path)
        {
            /* Der bliver aktuelt ikke taget højde for corrupt files - 
            der skal tages forbehold for, hvis det ikke er lykkedes at læse width, height og tilsvarende Row-informationer*/
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        DeconstructSettingsFromFile(line);
                    }

                    ImportedGrid = true;
                    CreateGrid();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void DeconstructSettingsFromFile(string line)
        {
            FileSettings setting = ExtractSettingFromFile(line);
            if (setting == FileSettings.NA)
                return;

            string value = ExtractValueFromLine(line);
            switch (setting)
            {
                case FileSettings.Width:
                    ImportedGridWidth = int.Parse(value);
                    break;
                case FileSettings.Height:
                    ImportedGridHeight = int.Parse(value);
                    break;
                case FileSettings.Header:
                    ImportedGridHeader = value;
                    break;
                case FileSettings.Description:
                    ImportedGridDescription = value;
                    break;
                case FileSettings.Row:
                    _importedRows.Add(value);
                    break;
            }
        }

        private void ImportGridSettings()
        {
            /* The grid types is subjected as a two-dimensional int array - aka matrix */
            int[,] gridMatrix = new int[ImportedGridWidth, ImportedGridHeight];
            TransformGridMatrix(ref gridMatrix);

            const int freeAsInt = (int) BuildingBlock.ElevationTypes.Free;

            for (int x = 0; x < ImportedGridWidth; x++)
            {
                for (int y = 0; y < ImportedGridHeight; y++)
                {
                    int type = gridMatrix[x, y];
                    /* If the point is already marked as free, continue - no need to force-convert it to free */
                    if (type == freeAsInt)
                        continue;

                    BuildingBlock.ElevationTypes newType = (BuildingBlock.ElevationTypes) type;

                    NewGrid.AllPoints[Coordinate(x, y)].Elevation = newType;
                }
            }
        }

        private void TransformGridMatrix(ref int[,] matrix)
        {
            for (int x = 0; x < _importedRows.Count; x++)
            {
                int y = 0;
                foreach (char type in _importedRows[x])
                {
                    matrix[x, y] = int.Parse(type.ToString());
                    y++;
                }
            }
        }

        private void OnSuccess()
        {
            NewOrImportWindow.Close();
        }

        //private bool ValidateInputs()
        //{
        //    /* First check: Are either or both width and/or height <= 0? */
        //    if (NewOrImportWindow.GridWidth <= 0 || NewOrImportWindow.GridHeight <= 0)
        //    {
        //        string caption = "Fix grid width/height";
        //        string message = "Error! Could not create a grid with the specified width/height. Please resolve the following:\n";
        //        message += NewOrImportWindow.GridWidth <= 0 ? "\t - Width should be a positive integer\n" : string.Empty;
        //        message += NewOrImportWindow.GridHeight <= 0
        //            ? "\t - Height should be a positive integer\n"
        //            : string.Empty;
        //        MessageBox.Show(message, caption);
        //        return false;
        //    }

        //    /* Second check: ? */

        //    /* Third check: Profit! */

        //    return true;
        //}
    }
}