using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evacuation_Master_3000
{
    public class Person
    {
        public BuildingBlock Position { get; private set; }
        public List<BuildingBlock> PathList = new List<BuildingBlock>();
        public int AmountOfMoves = 0;
        public Person(BuildingBlock position)
        {
            this.Position = position;
        }

        public void Move()
        {
            if (this.PathList.Count > 0) {
                Position.Elevation = BuildingBlock.ElevationTypes.Free;
                Position.ColorizePoint();
                Position = PathList[0];
                Position.Elevation = BuildingBlock.ElevationTypes.Person;
                Position.HeatmapCounter++;
                Position.ColorizePoint();
                PathList.Remove(Position);
            }
        }
    }
}