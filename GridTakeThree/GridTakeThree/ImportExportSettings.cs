using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace GridTakeThree {
    public static class ImportExportSettings {
        /* Grid file extension */
        public static string Extension { get { return ".grid"; } }
        public static string FileName { get { return "Grid"; } }
        public static string GridDirectoryPath {
            get {
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + @"Grids";
                if (!Directory.Exists(fullPath))
                    Directory.CreateDirectory(fullPath);
                return fullPath;
            }
        }

        /* Grid default values */
        public static int DefaultWidth { get { return 100; } }
        public static int DefaultHeight { get { return 100; } }
        public static string DefaultImportHeader { get { return "Import grid"; } }
        public static string DefaultNewHeader { get { return string.Empty; } }
        public static string DefaultDescription { get { return string.Empty; } }
        public static string DefaultDescriptionMessage { get { return "This grid has no description."; } }

        /* Grid write file settings */
        public static string Prefix { get { return "<"; } }
        public static string Suffix { get { return ">"; } }   
        public static string EndModifier { get { return "/"; } }
        public static string Newline { get { return Environment.NewLine; } }
        public enum FileSettings { NA, Settings, Width, Height, Header, Description, Grid, Row}
        private static FileSettings Setting { get; set; }
        private static string Pre { get { return Prefix + Setting.ToString() + Suffix; } }
        private static string Post { get { return Prefix + EndModifier + Setting.ToString() + Suffix + Newline; } }
        public static string GridKeyFormat { get { return "({0}, {1})"; } }

        /* Grid methods */
        /// <summary>
        /// Wraps a single entry wihtin the corresponding Settings type.
        /// </summary>
        /// <param name="type">The Settings type that wraps the input</param>
        /// <param name="input">The value to save. Input is cast to string.</param>
        /// <returns>A single entry string containing the value stringed and wrapped in the corresponding Settings type.</returns>
        public static string WriteSingleItem(FileSettings type, object input) {
            Setting = type;
            return Pre + input.ToString() + Post;
        }
        //public static string WriteSingleItem(Settings type, int input) {
        //    return WriteSingleItem(type, input.ToString());
        //}
        /// <summary>
        /// Wraps multiple entries to a grid file in a <Settings></Settings> environment. 
        /// </summary>
        /// <param name="container">
        /// A dictionary containing the entries. Each entry is of type object with key of type Settings.
        /// </param>
        /// <returns>Returns each value as string concatinated into a single string.</returns>
        public static string WriteSettings(Dictionary<FileSettings, object> container) {
            string toFile = string.Empty;
            foreach (var item in container) {
                toFile += WriteSingleItem(item.Key, item.Value.ToString());
            }
            Setting = FileSettings.Settings; //container.First().Key < Settings.Grid ? Settings.Settings : Settings.Grid;
            return Pre + Newline + toFile + Post;
        }
        /// <summary>
        /// Takes a list of rows as strings and concatinates them in a <Grid></Grid> environment
        /// </summary>
        /// <param name="container">
        /// A list containing the rows as strings.
        /// </param>
        /// <returns>Returns each row wrapped in <Row></Row> in a concatinated string wrappin in <Grid></Grid>.</returns>
        public static string WriteRows(List<string> container) {
            string toFile = string.Empty;
            foreach (string row in container) {
                toFile += WriteSingleItem(FileSettings.Row, row);
            }
            Setting = FileSettings.Grid;
            return Pre + Newline + toFile + Post;
        }
        /// <summary>
        /// Extracts Settings info from a given line read from a grid-file
        /// </summary>
        /// <param name="line">The string read from a grid-file containing the Settings info</param>
        /// <returns>On succes returns the extracted Settings type. On failure returns Settings.NA</returns>
        public static FileSettings ExtractSettingFromFile(string line) {
            /* searchPattern returns the name of the setting encapsulated in the prefrix and suffix */
            string searchPattern = $@"{Prefix}([^<\s>/]*){Suffix}";   /* Captures <This> */
            //Alternate version: (?<={Prefix + EndModifier}).*(?={Suffix})  ---- This is the same version used in SubtractValue-method

            if (Regex.IsMatch(line, searchPattern)) {
                Match match = Regex.Match(line, searchPattern);
                if (match.Value == EncapsulateSetting(FileSettings.Settings) || match.Value == EncapsulateSetting(FileSettings.Grid))
                    return FileSettings.NA;

                foreach (FileSettings setting in Enum.GetValues(typeof(FileSettings))) {
                    if (EncapsulateSetting(setting) == match.Value)
                        return setting;
                }
            }

            return FileSettings.NA;
        }

        private static string EncapsulateSetting(FileSettings type) {
            return Prefix + type.ToString() + Suffix;
        }
        /// <summary>
        /// Extracts the value of a setting from a given line read from a grid-file
        /// </summary>
        /// <param name="line">The string read from a grid-file containing the value</param>
        /// <returns>On succes returns the extracted value. On failure returns string.Empty/returns>
        public static string ExtractValueFromLine(string line) {
            /* searchPattern returns the value between */
            string searchPattern = $@"(?<={Suffix}).*(?={Prefix})";  /* Captures <>This</>*/
            return Regex.IsMatch(line, searchPattern) ?
                Regex.Match(line, searchPattern).Value :
                string.Empty;
        }
        /// <summary>
        /// Converts X and Y coordinates to a string in the form of GridKeyFormat.
        /// </summary>
        /// <param name="X">The X-coordinate</param>
        /// <param name="Y">The Y-coordinate</param>
        /// <returns>Returns a string with X and Y values in the form of GridKeyFormat.</returns>
        public static string Coordinate(int X, int Y) {
            return string.Format(GridKeyFormat, X, Y);
        }
    }
}
