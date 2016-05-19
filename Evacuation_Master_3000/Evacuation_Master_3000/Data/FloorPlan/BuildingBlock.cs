using System;
using System.Collections.Generic;
using System.Windows.Shapes;

namespace Evacuation_Master_3000
{
    public class BuildingBlock : Tile, IComparable<BuildingBlock>
    {
        public BuildingBlock(int x, int y, int z = 0, Types type = Types.Free) : base(x, y, z, type) { }
        public bool IsChecked { get; set; }
        public double LengthToDestination { private get; set; }
        public int Room { get; set; }
        public int Priority { get; set; } = int.MaxValue;
        public double LengthFromSource { get; set; } = double.MaxValue;
        public BuildingBlock Parent { get; set; }
        public readonly HashSet<BuildingBlock> BuildingBlockNeighbours = new HashSet<BuildingBlock>();
        public int HeatmapCounter { get; set; }
        public Rectangle Figure { get; set; }

        public int CompareTo(BuildingBlock other)
        {
            if (other.LengthFromSource + other.LengthToDestination > LengthFromSource + LengthToDestination)
                return -1;
            return other.LengthFromSource + other.LengthToDestination < LengthFromSource + LengthToDestination ? 1 : 0;
        }

    }
}