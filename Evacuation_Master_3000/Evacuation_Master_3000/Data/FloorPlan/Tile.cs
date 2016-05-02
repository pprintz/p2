﻿using System;
using System.Collections.Generic;
using static Evacuation_Master_3000.ImportExportSettings;

namespace Evacuation_Master_3000
{
    public class Tile
    {
        public enum Types
        {
            Free,
            Occupied,
            Furniture,
            Wall,
            Door,
            Exit,
            Person
        }

        public Types Type { get; set; }
        public int X { get; }
        public int Y { get; }
        public int Z { get; } // Translates into the floor level
        public HashSet<Tile> Neighbours { get; } = new HashSet<Tile>();
        // Needs to remember original Types for after a person leaves. <<--- Needs implementation
        public Tile(int x, int y, int z = 0, Types type = Types.Free)
        {
            X = x;
            Y = y;
            Z = z;
            Type = type;
        }

        public double DistanceTo(Tile other)
        {
            return Math.Abs(Math.Sqrt(Math.Pow(other.X - X, 2) + Math.Pow(other.Y - Y, 2)));
        }

        public override string ToString()
        {
            return Coordinate(this) + " : " + Type;
        }

        public override bool Equals(object obj) {
            Tile other = obj as Tile;
            return (Z == other.Z && Y == other.Y && X == other.X && Type == other.Type);
        }
    }
}