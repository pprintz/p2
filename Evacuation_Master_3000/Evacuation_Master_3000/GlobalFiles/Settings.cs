#region
using System;
using System.Windows.Input;
using System.IO;
#endregion

namespace Evacuation_Master_3000
{
    internal static class Settings
    {   /* Key mapping settings */
        public static Key ZoomKey { get; } = Key.LeftCtrl;
        public static Key HorizontalScrollKey { get; } = Key.LeftShift;
        public static Key LineToolKey { get; } = Key.LeftAlt;

        /* Building file extension */
        public static string Extension => ".grid";

        public static string GridDirectoryPath {
            get {
                string fullPath = AppDomain.CurrentDomain.BaseDirectory + @"Grids";
                if (!Directory.Exists(fullPath))
                    Directory.CreateDirectory(fullPath);
                return fullPath;
            }
        }

        /* Building plan default values */
        public static int DefaultWidth => 50;
        public static int DefaultHeight => 50;
        public static int DefaultFloorAmount => 1;
        public static string DefaultDescription => string.Empty;

        /* Internal properties and methods */
        private static string CoordinateKeyFormat => "({0}, {1}, {2})";
        /// <summary>
        ///     Converts X, Y and Z coordinates to a string in the form of CoordinateKeyFormat.
        /// </summary>
        /// <param name="x">The X-coordinate</param>
        /// <param name="y">The Y-coordinate</param>
        /// <param name="z">The Z-coordinate, aka floorlevel</param>
        /// <returns>Returns a string with X, Y and Z values in the form of CoordinateKeyFormat.</returns>
        public static string Coordinate(int x, int y, int z) {
            return string.Format(CoordinateKeyFormat, x, y, z);
        }
        /// <summary>
        /// Converts the given tile's X, Y and Z coordinates, to a string in the form of CoordinateKeyFormat.
        /// </summary>
        /// <param name="tile">The tile to get coordinates from</param>
        /// <returns></returns>
        public static string Coordinate(Tile tile) {
            return string.Format(CoordinateKeyFormat, tile.X, tile.Y, tile.Z);
        }
    }
}