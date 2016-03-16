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
using static GridTakeThree.ImportExportSettings;

namespace GridTakeThree {
    class Export {
        /// <summary>
        /// Exports/saves the grid from the canvas provided.
        /// <para>Speaking of saving, isn't it funny how Jesus and a floppy disc both are a symbol of saving?</para>
        /// </summary>
        public Export(Grid grid) {
            CurrentGrid = grid;
            exportWindow = new ExportWindow();
            exportWindow.SaveGridButton.Click += ExportGrid;
            exportWindow.GridWidth = CurrentGrid.PointsPerRow;
            exportWindow.GridHeight = CurrentGrid.PointsPerColumn;

            exportWindow.ShowDialog();
        }

        public ExportWindow exportWindow { get; }
        public Grid CurrentGrid { get; } 
        public string FullGridPath { get { return exportWindow.Path + "\\" + FullFileName; } }
        public string FullFileName { get { return exportWindow.FileName + Extension; } }
        private Dictionary<Settings, object> _settings = new Dictionary<Settings, object>();
        private Dictionary<Settings, object> GridMatrix = new Dictionary<Settings, object>();
        private Dictionary<Settings, object> Settings {
            get {
                if (_settings.Count == 0)
                    FillSettings();
                return _settings;
            }
        }
        private bool FileExistInFolder
        {
            get {
                foreach (string file in Directory.GetFiles(exportWindow.Path)) {
                    if (file == FullGridPath)
                        return true;
                }
                return false;
            }
        }
        private void FillSettings() {
            _settings.Add(ImportExportSettings.Settings.Width, exportWindow.GridWidth);
            _settings.Add(ImportExportSettings.Settings.Height, exportWindow.GridHeight);
            if(!string.IsNullOrWhiteSpace(exportWindow.Header))
                _settings.Add(ImportExportSettings.Settings.Header, exportWindow.Header);
            if (!string.IsNullOrWhiteSpace(exportWindow.Description))
                _settings.Add(ImportExportSettings.Settings.Description, exportWindow.Description);
        }

        private List<string> GridToFile() {
            string rowString;
            List<string> rows = new List<string>();
            for (int x = 1; x <= CurrentGrid.PointsPerRow; x++) {
                rowString = string.Empty;
                for (int y = 1; y <= CurrentGrid.PointsPerColumn; y++) {
                    rowString += (int)CurrentGrid.AllPoints[Coordinate(x, y)].Elevation;
                }
                rows.Add(rowString);
            }
            return rows;
        }

        private void ExportGrid(object sender, RoutedEventArgs e) {
            MessageBoxResult result = MessageBoxResult.None;
            string message, caption;
            /* First check: Are either or both path and/or file name missing? */
            if(string.IsNullOrWhiteSpace(exportWindow.Path) || string.IsNullOrWhiteSpace(exportWindow.FileName)) {
                message = "Error! Please correct the following before saving: \n";
                message += string.IsNullOrWhiteSpace(exportWindow.Path) ? "\t - Missing path (all whitespaces?)\n" : string.Empty;
                message += string.IsNullOrWhiteSpace(exportWindow.FileName) ? "\t - Missing file name (all whitespaces?)\n" : string.Empty;
                caption = "Missing file name and/or path";
                MessageBox.Show(message, caption);
                return;
            }

            /* Second check: Does the directory exist? 
               If not, the user will be prompted an error and asked if the directory should be created */
            if (!Directory.Exists(exportWindow.Path)) {
                message = $"Error! The directory\n{exportWindow.Path}\n does not exist.\nDo you want to create this folder?";
                caption = "Directory invalid";
                OnSaveError(message, caption, ref result);
                if (result == MessageBoxResult.Yes)
                    Directory.CreateDirectory(exportWindow.Path);
                else
                    return;
            }
            /* Third check: Does the directory already contain a file with the specified name? 
                If it does, the user will be prompted an error and asked if the existing file should be overridden (deleted) */
            if (FileExistInFolder) {
                message = $"Error! The file {FullFileName} already exist in the current path. Do you wish to override the file?\nWarning! This will permanently destroy the current file!";
                caption = "File already exist in directory";
                OnSaveError(message, caption, ref result);
                if (result == MessageBoxResult.Yes)
                    DeleteExistingFile();
                else
                    return;
            }

            FileSetup();
        }

        private void FileSetup() {
            try {
                using(StreamWriter sw = new StreamWriter(FullGridPath, false)) {
                    sw.Write(WriteSettings(Settings));
                    sw.Write(WriteRows(GridToFile()));

                    sw.Close();
                }

                OnSaveSucces();/* <- Success! If we reach this code, it *should* mean that the grid has been succesfully saved!*/
            } catch (Exception ex) {
                /* Whoops */
                MessageBox.Show($"Error! Unable to save the file {FullGridPath}!\n\nError message:\n{ex.Message}");

                /* Debug mode??
                if(!DebugMode){
                    DeleteExistingFile();
                }
                */
            }
        }

        private void DeleteExistingFile() {
            if (File.Exists(FullGridPath))
                File.Delete(FullGridPath);
        }

        private void OnSaveError(string message, string caption, ref MessageBoxResult result) {
            result = MessageBox.Show(message, caption, MessageBoxButton.YesNo);
        }

        private void OnSaveSucces() {
            exportWindow.Hide();
            MessageBox.Show("Succesfully saved the grid!");
        }

    }
}
