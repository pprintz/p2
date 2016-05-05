using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evacuation_Master_3000
{
    public class MovementStep
    {
        public MovementStep(Tile sourceTile, Tile destinationTile)
        {
            SourceTile = sourceTile;
            DestinationTile = destinationTile;
            Distance = sourceTile.DistanceTo(destinationTile);
        }
        public Tile SourceTile { get; set; }
        public Tile DestinationTile { get; set; }
        public double Distance { get; set; }
    }
}
