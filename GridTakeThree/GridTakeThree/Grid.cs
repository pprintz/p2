using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GridTakeThree {
    class Grid {

        public int GridSpacing { get; } = 10;
        public int PointSize { get; private set; } = 10;

        /*public static double MaxPointDistance
        {
            get { return Math.Sqrt(Math.Pow(GridSpacing - 0, 2) + Math.Pow(GridSpacing - 0, 2)); }
        }*/

        public int PointsPerRow { get; }
        public int PointsPerColumn { get; }
        
        public Grid(Canvas canvas, int pointsPerRow, int pointsPerColumn) {
            TheCanvas = canvas;
            PointsPerRow = pointsPerRow;
            PointsPerColumn = pointsPerColumn;
        }

        public Grid(Canvas canvas, int pointsPerRowAndColumn) : this(canvas, pointsPerRowAndColumn, pointsPerRowAndColumn) { }

        public Canvas TheCanvas { get; }

        public List<Point> WalledPoints { get; private set; } = new List<Point>();
        public List<Point> DoorPoints { get; private set; } = new List<Point>();
        public List<Point> UnavailablePoints { get; private set; } = new List<Point>();

        public Dictionary<string, Point> AllPoints { get; private set; } = new Dictionary<string, Point>();

        public void CreateGrid() {
            for (int y = 0; y < PointsPerColumn; y++) {
                for (int x = 0; x < PointsPerRow; x++) {
                    Ellipse figure = new Ellipse();
                    figure.Height = PointSize;
                    figure.Width = PointSize;
                    figure.Margin = new Thickness(x*GridSpacing, y* GridSpacing, 0, 0);

                    string coordinate = $"({x}, {y})";
                    AllPoints.Add(coordinate, new Point(x, y, figure));

                    TheCanvas.Children.Add(figure);
                }
            }

            //CalculateAllNeighbours();
        }

        public void CalculateAllNeighbours()
        {
            foreach (Point item in AllPoints.Values) {
                item.CalculateNeighbours(AllPoints);
            }
            CheckForConnectionsThroughDiagonalUnwalkableElements();
            
        }

        
        public void CheckForConnectionsThroughDiagonalUnwalkableElements()
        {
            string illegalConnectedPointCoordinateSetOne;
            string illegalConnectedPointCoordinateSetTwo;

            foreach (KeyValuePair<string, Point> pair in AllPoints)
            {
                if (pair.Value.Elevation == Point.ElevationTypes.Wall ||
                    pair.Value.Elevation == Point.ElevationTypes.Furniture)
                {
                    foreach (Point neighbour in pair.Value.Neighbours)
                    {
                        if(neighbour.DistanceToPoint(pair.Value) > 1) // Then it is a diagonal
                        {
                            illegalConnectedPointCoordinateSetOne = $"({pair.Value.X}, {neighbour.Y})";
                            illegalConnectedPointCoordinateSetTwo = $"({neighbour.X}, {pair.Value.Y})";
                            if (AllPoints.ContainsKey(illegalConnectedPointCoordinateSetOne) && AllPoints.ContainsKey(illegalConnectedPointCoordinateSetTwo))
                            {
                                AllPoints[illegalConnectedPointCoordinateSetOne].Neighbours.Remove(AllPoints[illegalConnectedPointCoordinateSetTwo]);
                                AllPoints[illegalConnectedPointCoordinateSetTwo].Neighbours.Remove(AllPoints[illegalConnectedPointCoordinateSetOne]);
                            }
                        }
                    } 
                }   
            }   
        }


        public bool CheckConnection(Point a, Point b)
        {
            bool result = true;
            Point p;
            AllPoints.TryGetValue($"({a.X}, {b.Y})", out p);
            foreach (Point point in ReturnLine(a,p))
            {
                if (point.Elevation == Point.ElevationTypes.Wall)
                {
                    result = false;
                    break;
                }
            }
            if (result)
            {
                foreach (Point point in ReturnLine(p, b))
                {
                    if (point.Elevation == Point.ElevationTypes.Wall)
                    {
                        return false;
                    }
                }
            }
            result = true;
            AllPoints.TryGetValue($"({b.X}, {a.Y})", out p);
            foreach (Point point in ReturnLine(a, p))
            {
                if (point.Elevation == Point.ElevationTypes.Wall)
                {
                    result = false;
                    break;
                }
            }
            if (result)
            {
                foreach (Point point in ReturnLine(p, b))
                {
                    if (point.Elevation == Point.ElevationTypes.Wall)
                    {
                        return false;
                    }
                }
            }
            return result;
        }

        public IEnumerable<Point> ReturnLine(Point a, Point b)
        {
            if (a.X==b.X)
            {
                if (a.Y<b.Y)
                {
                    return ReturnLineX(a,b);
                }
                else
                {
                    return ReturnLineX(b,a);
                }
            }
            else if (a.Y==b.Y)
            {
                if (a.Y<b.Y)
                {
                    return ReturnLineY(a, b);
                }
                else
                {
                    return ReturnLineY(b, a);
                }
            }
            throw new ArgumentException("De to punkter er ikke på linje.");
        }

        private IEnumerable<Point> ReturnLineX(Point a, Point b)
        {
            List<Point> points = new List<Point>();
            for (int x = a.X; x < b.X; x++)
            {
                Point point;
                AllPoints.TryGetValue($"({x}, {a.Y}", out point);
                points.Add(point);
            }
            return points;
        }

        private IEnumerable<Point> ReturnLineY(Point a, Point b)
        {
            List<Point> points = new List<Point>();
            for (int y = a.Y; y < b.Y; y++)
            {
                Point point;
                AllPoints.TryGetValue($"({a.X}, {y}", out point);
                points.Add(point);
            }
            return points;
        }


        /*public void PointToWall() {
            if (!WalledPoints.Contains(this))
                WalledPoints.Add(this);
        }

        public void PointToDoor() {
            if (WalledPoints.Contains(this) && !DoorPoints.Contains(this)) {
                DoorPoints.Add(this);
                WalledPoints.Remove(this);
            }
        }

        public void PointToFurniture() {
            if (!UnavailablePoints.Contains(this))
                UnavailablePoints.Add(this);
        }

        public void FreePoint() {
            List<List<Point>> listOfPoints = new List<List<Point>>();

            if (WalledPoints != null)
                listOfPoints?.Add(WalledPoints);

            if (DoorPoints != null)
                listOfPoints.Add(DoorPoints);

            if (UnavailablePoints != null)
                listOfPoints.Add(UnavailablePoints);

            if (listOfPoints.Count > 0)
                foreach (List<Point> pointList in listOfPoints) {
                    if (pointList.Contains(this))
                        pointList.Remove(this);
                }
        }*/
    }
}
