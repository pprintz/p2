#region

using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static Evacuation_Master_3000.ImportExportSettings;
using static Evacuation_Master_3000.Settings;

#endregion

namespace Evacuation_Master_3000
{
    public class BuildingBlock : IComparable<BuildingBlock>
    {
        public enum ElevationTypes
        {
            Free,
            Occupied,
            Furniture,
            Wall,
            Door,
            Hall,
            Exit,
            Person
        }

        public static List<BuildingBlock> Path = new List<BuildingBlock>();

        private ElevationTypes _elevation;
        public bool IsChecked = false;

        public double LengthToDestination;

        public BuildingBlock(int x, int y, Rectangle visual) : this(x, y, visual, ElevationTypes.Free)
        {
        }

        public BuildingBlock(int x, int y, Rectangle visual, ElevationTypes elevation)
        {
            X = x;
            Y = y;

            Visual = visual;
            Visual.MouseLeftButtonDown += OnClick;
            Elevation = elevation;
        }

        public int X { get; }
        public int Y { get; }
        public int HeatmapCounter { get; set; }

        public ElevationTypes Elevation
        {
            get { return _elevation; }
            set
            {
                _elevation = value;
                ColorizePoint();
            }
        }

        public Rectangle Visual { get; }
        public double LengthFromSource { get; set; } = 100000;
        public BuildingBlock Parent { get; set; }
        public List<BuildingBlock> Neighbours { get; } = new List<BuildingBlock>();

        public static BuildingBlock SelectedPoint { get; private set; }

        public int CompareTo(BuildingBlock other)
        {
            if (LengthFromSource + LengthToDestination < other.LengthFromSource + other.LengthToDestination)
            {
                return -1;
            }
            if (LengthFromSource + LengthToDestination > other.LengthFromSource + other.LengthToDestination)
            {
                return 1;
            }
            return 0;
        }

        public void ColorizePoint()
        {
            SolidColorBrush newColor = new SolidColorBrush();

            if (SelectedPoint == this)
            {
                Visual.Fill = new SolidColorBrush(Colors.Red);
                return;
            }
            switch (Elevation)
            {
                case ElevationTypes.Free:
                    newColor = ShowHeatMap == false ? new SolidColorBrush(Colors.White) : HeatMapColor();
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
                case ElevationTypes.Person:
                    newColor = new SolidColorBrush(Colors.Crimson);
                    break;
            }

            Visual.Fill = newColor;
        }

        private SolidColorBrush HeatMapColor()
        {
            int red, green, blue;
            int multiplier = (int) Math.Round(765f/PersonCount);
            // Maybe change, so that not everyone needs to go through in order for (255,0,0) to appear.
            int count = multiplier*HeatmapCounter;
            if (count <= 255)
            {
                red = 255 - count;
                green = 255;
                blue = 255 - count;
            }
            else if (count <= 510)
            {
                red = count - 255;
                green = 255;
                blue = 0;
            }
            else
            {
                // if (count <= 765) {
                red = 255;
                green = Math.Min(765 - count, 255);
                blue = 0;
            }

            return new SolidColorBrush(Color.FromRgb((byte) red, (byte) green, (byte) blue));
        }

        private void RemoveNeighbours()
        {
            foreach (BuildingBlock neighbour in Neighbours)
            {
                neighbour.RemovePointFromNeighbours(this);
            }
        }

        public void RemovePointFromNeighbours(BuildingBlock point)
        {
            if (Neighbours.Contains(point))
                Neighbours.Remove(point);
        }

        public void AddPointToNeighbours(BuildingBlock point)
        {
            //if(GridSettings.MaxPointDistance <= DistanceToPoint(point) && !this.Neighbours.Contains(point))
            Neighbours.Add(point);
        }

        public double DistanceToPoint(BuildingBlock point)
        {
            return Math.Abs(Math.Sqrt(Math.Pow(point.X - X, 2) + Math.Pow(point.Y - Y, 2)));
        }

        public void CalculateNeighbours(Dictionary<string, BuildingBlock> allPoints)
        {
            int topLeftNeighbourX = X - 1;
            int topLeftNeighbourY = Y - 1;

            BuildingBlock currentPoint;
            for (int currentY = topLeftNeighbourY; currentY <= topLeftNeighbourY + 2; currentY++)
            {
                for (int currentX = topLeftNeighbourX; currentX <= topLeftNeighbourX + 2; currentX++)
                {
                    string coordinate = Coordinate(currentX, currentY);
                    if (allPoints.ContainsKey(coordinate) == false || allPoints[coordinate] == this)
                        continue;

                    currentPoint = allPoints[coordinate];

                    if (currentPoint.Elevation != ElevationTypes.Wall &&
                        currentPoint.Elevation != ElevationTypes.Furniture
                        // Everything else connects to everything else
                        || currentPoint.Elevation == ElevationTypes.Wall && currentPoint.Elevation == Elevation
                        // Wall connects to walls
                        || currentPoint.Elevation == ElevationTypes.Furniture && currentPoint.Elevation == Elevation)
                        // furniture connects to furniture
                        AddPointToNeighbours(currentPoint);
                }
            }
        }

        public void OnClick(object sender, MouseButtonEventArgs e)
        {
            if (MainWindow.lineTool && e != null)
                MainWindow.InputLineTool(this);
            else if (MainWindow.makeWall)
                Elevation = ElevationTypes.Wall;
            else if (MainWindow.makeDoor)
                Elevation = ElevationTypes.Door;
            else if (MainWindow.makePath)
            {
                Elevation = ElevationTypes.Hall;
                Path.Add(this);
            }
            else if (MainWindow.makePerson)
            {
                Elevation = ElevationTypes.Person;
                MainWindow.PList.Add(new Person(this));
            }
            else
                Elevation = ElevationTypes.Free;

            ColorizePoint();
            /*if (SelectedPoint != null)
                SelectedPoint.ColorizePoint();

            SelectedPoint = this;
            this.ColorizePoint();

            foreach (BuildingBlock neighbour in Neighbours) {
                neighbour.Elevation = ElevationTypes.Exit;
                neighbour.ColorizePoint();
            }*/
        }
    }
}