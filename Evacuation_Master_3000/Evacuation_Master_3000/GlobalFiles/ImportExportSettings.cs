#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

#endregion

namespace Evacuation_Master_3000
{
    public static class ImportExportSettings
    {
        public enum FileSettings
        {
            NA,
            Settings,
            Width,
            Height,
            FloorAmount,
            Header,
            Description,
            Grid,
            CurrentFloor,
            CurrentRow,
            Row
        }

        /* Grid file extension */
        public static string Extension => ".grid";

        public static string GridDirectoryPath
        {
            get
            {
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + @"Grids";
                if (!Directory.Exists(fullPath))
                    Directory.CreateDirectory(fullPath);
                return fullPath;
            }
        }

        /* Grid default values */
        public static int DefaultWidth => 100;
        public static int DefaultHeight => 100;
        public static int DefaultFloorAmount => 1;
        public static string DefaultDescription => string.Empty;

        /* Grid write file settings */
        private static string Prefix => "<";
        private static string Suffix => ">";
        private static string EndModifier => "/";
        private static string Newline => Environment.NewLine;

        private static FileSettings Setting { get; set; }
        private static string Pre => Prefix + Setting + Suffix;
        private static string Post => Prefix + EndModifier + Setting + Suffix + Newline;
        private static string GridKeyFormat => "({0}, {1}, {2})";

        /* Grid methods */

        /// <summary>
        ///     Wraps a single entry wihtin the corresponding Settings type.
        /// </summary>
        /// <param name="type">The Settings type that wraps the input</param>
        /// <param name="input">The value to save. Input is cast to string.</param>
        /// <returns>A single entry string containing the value stringed and wrapped in the corresponding Settings type.</returns>
        private static string WriteSingleItem(FileSettings type, object input)
        {
            Setting = type;
            return Pre + input + Post;
        }

        //public static string WriteSingleItem(Settings type, int input) {
        //    return WriteSingleItem(type, input.ToString());
        //}
        /// <summary>
        ///     Wraps multiple entries to a grid file in a <Settings></Settings> environment.
        /// </summary>
        /// <param name="container">
        ///     A dictionary containing the entries. Each entry is of type object with key of type Settings.
        /// </param>
        /// <returns>Returns each value as string concatinated into a single string.</returns>
        public static string WriteSettings(Dictionary<FileSettings, object> container)
        {
            string toFile = string.Empty;
            foreach (var item in container)
            {
                toFile += WriteSingleItem(item.Key, item.Value.ToString());
            }
            Setting = FileSettings.Settings;
            //container.First().Key < Settings.Grid ? Settings.Settings : Settings.Grid;
            return Pre + Newline + toFile + Post;
        }

        /// <summary>
        ///     Takes a list of rows as strings and concatinates them in a <Grid></Grid> environment
        /// </summary>
        /// <param name="container">
        ///     A list containing the rows as strings.
        /// </param>
        /// <returns>Returns each row wrapped in <Row></Row> in a concatinated string wrappin in <Grid></Grid>.</returns>
        public static string WriteRows(List<string> container)
        {
            string toFile = string.Empty;
            foreach (string row in container)
            {
                toFile += WriteSingleItem(FileSettings.Row, row);
            }
            Setting = FileSettings.Grid;
            return Pre + Newline + toFile + Post;
        }

        /// <summary>
        ///     Converts X, Y and Z coordinates to a string in the form of CoordinateKeyFormat.
        /// </summary>
        /// <param name="x">The X-coordinate</param>
        /// <param name="y">The Y-coordinate</param>
        /// <param name="z">The Z-coordinate, aka floorlevel</param>
        /// <returns>Returns a string with X, Y and Z values in the form of CoordinateKeyFormat.</returns>
        public static string Coordinate(int x, int y, int z)
        {
            return string.Format(GridKeyFormat, x, y, z);
        }
        /// <summary>
        /// Converts the given tile's X, Y and Z coordinates, to a string in the form of CoordinateKeyFormat.
        /// </summary>
        /// <param name="tile">The tile to get coordinates from</param>
        /// <returns></returns>
        public static string Coordinate(Tile tile)
        {
            return string.Format(GridKeyFormat, tile.X, tile.Y, tile.Z);
        }
    }
}