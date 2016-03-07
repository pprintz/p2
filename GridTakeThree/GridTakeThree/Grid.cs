﻿using System;
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

        public double WindowWidth { get; set; }
        public double WindowHeight { get; set; }

        public int PointsPerRow => (int)((WindowWidth - 200) / GridSpacing);
        public int PointsInHeight => (int)(WindowHeight / GridSpacing);


        public Grid(Canvas canvas, int windowWidth, int windowHeight) {
            TheCanvas = canvas;
            WindowWidth = windowWidth;
            WindowHeight = windowHeight;
        }

        public Canvas TheCanvas { get; }

        public List<Point> WalledPoints { get; private set; } = new List<Point>();
        public List<Point> DoorPoints { get; private set; } = new List<Point>();
        public List<Point> UnavailablePoints { get; private set; } = new List<Point>();

        public Dictionary<string, Point> AllPoints { get; private set; } = new Dictionary<string, Point>();

        public void CreateGrid() {
            for (int y = 0; y < PointsInHeight; y++) {
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
