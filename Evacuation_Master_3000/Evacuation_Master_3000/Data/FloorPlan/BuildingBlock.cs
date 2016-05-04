using System;
using System.Collections.Generic;
using static Evacuation_Master_3000.ImportExportSettings;
namespace Evacuation_Master_3000
{
    public class BuildingBlock : Tile, IComparable<BuildingBlock>
    {
        public BuildingBlock(int x, int y, int z = 0, Types type = Types.Free) : base(x, y, z, type) { }
        public bool IsChecked { get; set; }
        public double LengthToDestination { get; set; }
        public int Room { get; set; }
        public int Priority { get; set; } = 100;
        public double LengthFromSource { get; set; } = double.MaxValue;
        public BuildingBlock Parent { get; set; }
        public HashSet<BuildingBlock> BNeighbours = new HashSet<BuildingBlock>();
        public int HeatmapCounter { get; set; }
        //Room info prop

        public int CompareTo(BuildingBlock other)
        {
            if (other.LengthFromSource + other.LengthToDestination > LengthFromSource + LengthToDestination)
                return -1;
            return other.LengthFromSource + other.LengthToDestination < LengthFromSource + LengthToDestination ? 1 : 0;
        }

    }
}
//public void CalculateNeighbours(Dictionary<string, BuildingBlock> tiles)
//{
//    int topLeftNeighbourX = X - 1;
//    int topLeftNeighbourY = Y - 1;

//    for (int currentY = topLeftNeighbourY; currentY <= topLeftNeighbourY + 2; currentY++)
//    {
//        for (int currentX = topLeftNeighbourX; currentX <= topLeftNeighbourX + 2; currentX++)
//        {
//            // Only looks for neighbours in own Z coordinate
//            // Something else needs to happens with staircases etc.
//            string coordinate = Coordinate(currentX, currentY, Z);
//            if (tiles.ContainsKey(coordinate) == false || tiles[coordinate] == this)
//                continue;

//            BuildingBlock currentTile = tiles[coordinate];

//            if (Type == Types.Wall && currentTile.Type == Types.Wall ||
//                // Walls connects to walls
//                Type == Types.Furniture && currentTile.Type == Types.Furniture ||
//                // Furniture connects to furniture

//                Type != Types.Wall && Type != Types.Furniture &&
//                // Everything else connects to everything else
//                currentTile.Type != Types.Wall &&
//                currentTile.Type != Types.Furniture)
//            {
//                Neighbours.Add(currentTile);
//            }

//        }
//    }
//}