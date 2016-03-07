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
    class Point {
        public Point(int x, int y, Ellipse visual) : this (x, y, visual, ElevationTypes.Free) { }
        public Point(int x, int y, Ellipse visual, ElevationTypes elevation) {
            X = x;
            Y = y;

            Visual = visual;
            Visual.MouseLeftButtonDown += OnClick;
            Elevation = elevation;
        }

        public int X { get; }
        public int Y { get; }
        
        public enum ElevationTypes { Free, Occupied, Furniture, Wall, Door, Hall, Exit }
        private ElevationTypes _elevation;
        public ElevationTypes Elevation {
            get { return _elevation; }
            set {
                /*if (value == ElevationTypes.Wall)
                    //PointToWall();
                else if (value == ElevationTypes.Door)
                    //PointToDoor();
                else if (value == ElevationTypes.Furniture)
                    //PointToFurniture();
                else if (value == ElevationTypes.Free)
                    //FreePoint();*/

                _elevation = value;
                ColorizePoint();
            }
        } 
        
        public Ellipse Visual { get; }
        public double LengthFromSource { get; set; } = 1000000;
        public Point Parent { get; set; }
        public List<Point> Neighbours { get; private set; } = new List<Point>();

        public void ColorizePoint() {
            SolidColorBrush newColor = new SolidColorBrush();

            if (SelectedPoint == this) {
                Visual.Fill = new SolidColorBrush(Colors.Red);
                return;
            }

            switch (Elevation) {
                case ElevationTypes.Free:
                    newColor = new SolidColorBrush(Colors.Wheat);
                    break;
                case ElevationTypes.Occupied:
                    newColor = new SolidColorBrush(Colors.Yellow);
                    break;
                case ElevationTypes.Furniture:
                    newColor = new SolidColorBrush(Colors.Gray);
                    break;
                case ElevationTypes.Wall:
                    newColor = new SolidColorBrush(Colors.Black);
                    break;
                case ElevationTypes.Door:
                    newColor = new SolidColorBrush(Colors.Pink);
                    break;
                case ElevationTypes.Hall:
                    newColor = new SolidColorBrush(Colors.Blue);
                    break;
                case ElevationTypes.Exit:
                    newColor = new SolidColorBrush(Colors.Green);
                    break;
                default:
                    break;
            }

            Visual.Fill = newColor;
        }

        private void RemoveNeighbours() {
            foreach (Point neighbour in this.Neighbours) {
                neighbour.RemovePointFromNeighbours(this);
            }
        }

        public void RemovePointFromNeighbours(Point point) {
            if (Neighbours.Contains(point))
                Neighbours.Remove(point);
        }
        
        public void AddPointToNeighbours(Point point) {
            //if(GridSettings.MaxPointDistance <= DistanceToPoint(point) && !this.Neighbours.Contains(point))
                this.Neighbours.Add(point);
        } 
        
        public double DistanceToPoint(Point point) {
            return Math.Abs(Math.Sqrt(Math.Pow(point.X - this.X, 2) + Math.Pow(point.Y - this.Y, 2)));
        }

        public void CalculateNeighbours(Dictionary<string, Point> allPoints) {
            int topLeftNeighbourX = X - 1;
            int topLeftNeighbourY = Y - 1;

            Point currentPoint;
            for(int currentY = topLeftNeighbourY; currentY <= topLeftNeighbourY + 2; currentY++) 
            {
                for (int currentX = topLeftNeighbourX; currentX <= topLeftNeighbourX + 2; currentX++) 
                {
                    string coordinate = $"({currentX}, {currentY})";
                    if (allPoints.ContainsKey(coordinate) == false || allPoints[coordinate] == this) 
                        continue;
                        
                    currentPoint = allPoints[coordinate];

                    if (currentPoint.Elevation != ElevationTypes.Wall && currentPoint.Elevation != ElevationTypes.Furniture)
                        AddPointToNeighbours(currentPoint);
                 }
            }
        }

        public static Point SelectedPoint { get; private set; }
        public static List<Point> Path = new List<Point>();
        public void OnClick(object sender, MouseButtonEventArgs e) {
            if (MainWindow.makeWall)
                Elevation = ElevationTypes.Wall;
            else if (MainWindow.makeDoor)
                Elevation = ElevationTypes.Door;
            else if (MainWindow.makePath) {
                Elevation = ElevationTypes.Hall;
                Path.Add(this);
            }
            else
                Elevation = ElevationTypes.Free;

            ColorizePoint();
            /*if (SelectedPoint != null)
                SelectedPoint.ColorizePoint();

            SelectedPoint = this;
            this.ColorizePoint();

            foreach (Point neighbour in Neighbours) {
                neighbour.Elevation = ElevationTypes.Exit;
                neighbour.ColorizePoint();
            }*/
        }
        
    }
}
