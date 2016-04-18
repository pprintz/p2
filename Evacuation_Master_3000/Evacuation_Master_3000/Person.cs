#region

using System.Collections.Generic;

#endregion

namespace Evacuation_Master_3000
{
    public class Person
    {
        public int AmountOfMoves = 0;
        public List<BuildingBlock> PathList = new List<BuildingBlock>();

        public Person(BuildingBlock position)
        {
            Position = position;
        }

        public BuildingBlock Position { get; private set; }

        public void Move()
        {
            if (PathList.Count > 0)
            {
                Position.Elevation = BuildingBlock.ElevationTypes.Free;
                Position = PathList[0];
                Position.Elevation = BuildingBlock.ElevationTypes.Person;
                Position.HeatmapCounter++;
                PathList.Remove(Position);
            }
        }
    }
}