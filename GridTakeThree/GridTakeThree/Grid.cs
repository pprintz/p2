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

        public int PointsPerRow { get; private set; }
        public int PointsPerColumn { get; private set; }

        
        //public Grid(Canvas canvas, int pointsPerRow, int pointsPerColumn) {
        //    TheCanvas = canvas;
        //    PointsPerRow = pointsPerRow;
        //    PointsPerColumn = pointsPerColumn;
        //}

        //public Grid(Canvas canvas, int pointsPerRowAndColumn) : this(canvas, pointsPerRowAndColumn, pointsPerRowAndColumn) { }

        public Canvas TheCanvas { get; private set; }

        public Dictionary<string, Point> AllPoints { get; private set; } = new Dictionary<string, Point>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="pointsPerRow"></param>
        /// <param name="pointsPerColumn"></param>
        public void CreateGrid(Canvas canvas, int pointsPerRow, int pointsPerColumn) {
            TheCanvas = canvas;
            PointsPerRow = pointsPerRow;
            PointsPerColumn = pointsPerColumn;
            for (int y = 1; y < PointsPerColumn; y++) {
                for (int x = 1; x < PointsPerRow; x++) {
                    Ellipse figure = new Ellipse();
                    figure.Height = PointSize;
                    figure.Width = PointSize;
                    figure.Margin = new Thickness(x*GridSpacing, y* GridSpacing, 0, 0);

                    string coordinate = ImportExportSettings.Coordinate(x,y);
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
                            illegalConnectedPointCoordinateSetOne = ImportExportSettings.Coordinate(pair.Value.X,neighbour.Y);
                            illegalConnectedPointCoordinateSetTwo = ImportExportSettings.Coordinate(neighbour.X, pair.Value.Y);
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
