using System;

namespace Evacuation_Master_3000
{
    public class MovementStep
    {
        public MovementStep(Person person, Tile sourceTile, Tile destinationTile)
        {
            Person = person;
            SourceTile = sourceTile;
            DestinationTile = destinationTile;
            Distance = sourceTile.DistanceTo(destinationTile);
            DistanceInMeters = Distance * GlobalVariables.BlockWidthInMeters;
            Person.PersonInteractionStats.DistanceTraveled += DistanceInMeters;
        }

        public int TicksAtArrival { get; set; }
        private Person Person { get;  }
        public Tile SourceTile { get; }
        public Tile DestinationTile { get; }
        private double Distance { get; }

        private double _distanceMeters;
        public double DistanceInMeters
        {
            get { return Math.Round(_distanceMeters, 2); }
            private set { _distanceMeters = value; }
        }
    }
}
