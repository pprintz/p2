using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridTakeThree
{
    public class Person
    {
        public Point Position { get; private set; }
        public List<Point> PathList = new List<Point>();
        public int AmountOfMoves = 0;
        public Person(Point position)
        {
            this.Position = position;
        }

        public void Move()
        {
            if (this.PathList.Count > 0) {
                Position.Elevation = Point.ElevationTypes.Free;
                Position.ColorizePoint();
                Position = PathList[0];
                Position.Elevation = Point.ElevationTypes.Person;
                Position.ColorizePoint();
                PathList.Remove(Position);
            }
        }
    }
}