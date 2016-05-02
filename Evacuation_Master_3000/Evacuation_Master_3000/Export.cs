#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using static Evacuation_Master_3000.ImportExportSettings;

#endregion

namespace Evacuation_Master_3000
{
    internal class Export
    {
        private readonly Dictionary<FileSettings, object> _settings = new Dictionary<FileSettings, object>();
        private old_ExportWindow ExportWindow { get; }
        private Grid_new CurrentGrid { get; }
        private string FullGridPath => ExportWindow.Path + "\\" + FullFileName;
        private string FullFileName => ExportWindow.FileName + Extension;
        private Dictionary<FileSettings, object> Settings
        {
            get
            {
                if (_settings.Count == 0)
                    FillSettings();
                return _settings;
            }
        }
        private bool FileExistInFolder
        {
            get
            { return Directory.GetFiles(ExportWindow.Path).Any(file => file == FullGridPath); }
        }

        /// <summary>
        /// Exports/saves the grid from the canvas provided.
        /// <para>Speaking of saving, isn't it funny how Jesus and a floppy disc both are a symbol of saving?</para>
        /// </summary>
        public Export(Grid_new grid) // , Dictionary<FileSettings, object> gridMatrix
        {
            CurrentGrid = grid;
            //GridMatrix = gridMatrix;
            ExportWindow = new old_ExportWindow();
            ExportWindow.SaveGridButton.Click += ExportGrid;
            ExportWindow.GridWidth = CurrentGrid.PointsPerRow;
            ExportWindow.GridHeight = CurrentGrid.PointsPerColumn;

            ExportWindow.Header = string.IsNullOrWhiteSpace(CurrentGrid.Header) ? string.Empty : CurrentGrid.Header;
            ExportWindow.Description = string.IsNullOrWhiteSpace(CurrentGrid.Description)
                ? string.Empty
                : CurrentGrid.Description;

            ExportWindow.ShowDialog();
        }

        private void FillSettings()
        {
            _settings.Add(FileSettings.Width, ExportWindow.GridWidth);
            _settings.Add(FileSettings.Height, ExportWindow.GridHeight);
            if (!string.IsNullOrWhiteSpace(ExportWindow.Header))
                _settings.Add(FileSettings.Header, ExportWindow.Header);
            if (!string.IsNullOrWhiteSpace(ExportWindow.Description))
                _settings.Add(FileSettings.Description, ExportWindow.Description);
        }

        private List<string> GridToFile()
        {
            List<string> rows = new List<string>();
            for (int x = 0; x < CurrentGrid.PointsPerRow; x++)
            {
                var rowString = string.Empty;
                for (int y = 0; y < CurrentGrid.PointsPerColumn; y++)
                {
                    ////////////////////rowString += (int) CurrentGrid.AllPoints[Coordinate(x, y)].Type;
                }
                rows.Add(rowString);
            }
            return rows;
        }

        private void ExportGrid(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBoxResult.None;
            string message, caption;
            /* First check: Are either or both path and/or file name missing? */
            if (string.IsNullOrWhiteSpace(ExportWindow.Path) || string.IsNullOrWhiteSpace(ExportWindow.FileName))
            {
                message = "Error! Please correct the following before saving: \n";
                message += string.IsNullOrWhiteSpace(ExportWindow.Path)
                    ? "\t - Missing path (all whitespaces?)\n"
                    : string.Empty;
                message += string.IsNullOrWhiteSpace(ExportWindow.FileName)
                    ? "\t - Missing file name (all whitespaces?)\n"
                    : string.Empty;
                caption = "Missing file name and/or path";
                MessageBox.Show(message, caption);
                return;
            }

            /* Second check: Does the directory exist? 
               If not, the user will be prompted an error and asked if the directory should be created */
            if (!Directory.Exists(ExportWindow.Path))
            {
                message =
                    $"Error! The directory\n{ExportWindow.Path}\n does not exist.\nDo you want to create this folder?";
                caption = "Directory invalid";
                OnSaveError(message, caption, ref result);
                if (result == MessageBoxResult.Yes)
                    Directory.CreateDirectory(ExportWindow.Path);
                else
                    return;
            }
            /* Third check: Does the directory already contain a file with the specified name? 
                If it does, the user will be prompted an error and asked if the existing file should be overridden (deleted) */
            if (FileExistInFolder)
            {
                message =
                    $"Error! The file {FullFileName} already exist in the current path. Do you wish to override the file?\nWarning! This will permanently destroy the current file!";
                caption = "File already exist in directory";
                OnSaveError(message, caption, ref result);
                if (result == MessageBoxResult.Yes)
                    DeleteExistingFile();
                else
                    return;
            }

            FileSetup();
        }

        private void FileSetup()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(FullGridPath, false))
                {
                    sw.Write(WriteSettings(Settings));
                    sw.Write(WriteRows(GridToFile()));

                    sw.Close();
                }

                OnSaveSucces();
                /* <- Success! If we reach this code, it *should* mean that the grid has been succesfully saved!*/
            }
            catch (Exception ex)
            {
                /* Whoops */
                MessageBox.Show($"Error! Unable to save the file {FullGridPath}!\n\nError message:\n{ex.Message}");

                /* Debug mode??
                if(!DebugMode){
                    DeleteExistingFile();
                }
                */
            }
        }

        private void DeleteExistingFile()
        {
            if (File.Exists(FullGridPath))
                File.Delete(FullGridPath);
        }

        private void OnSaveError(string message, string caption, ref MessageBoxResult result)
        {
            result = MessageBox.Show(message, caption, MessageBoxButton.YesNo);
        }

        private void OnSaveSucces()
        {
            ExportWindow.Hide();
            MessageBox.Show("Succesfully saved the grid!");
        }
    }
}