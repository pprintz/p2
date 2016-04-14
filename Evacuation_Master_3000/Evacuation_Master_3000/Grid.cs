#region

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using static Evacuation_Master_3000.ImportExportSettings;

#endregion

namespace Evacuation_Master_3000
{
    class Grid
    {
        private VertexDatabase _vertexDatabase;
        public int GridSpacing { get; } = 10;
        public int PointSize { get; } = 10;
        public Canvas TheCanvas { get; private set; }
        public int PointsPerRow { get; private set; }
        public int PointsPerColumn { get; private set; }


        //public Grid(Canvas canvas, int pointsPerRow, int pointsPerColumn) {
        //    TheCanvas = canvas;
        //    PointsPerRow = pointsPerRow;
        //    PointsPerColumn = pointsPerColumn;
        //}

        //public Grid(Canvas canvas, int pointsPerRowAndColumn) : this(canvas, pointsPerRowAndColumn, pointsPerRowAndColumn) { }

        public string Header { get; set; }
        public string Description { get; set; }

        public Dictionary<string, BuildingBlock> AllPoints { get; } =
            new Dictionary<string, BuildingBlock>();

        public void CreateGrid(Canvas canvas, int pointsPerRow, int pointsPerColumn)
        {
            TheCanvas = canvas;
            PointsPerRow = pointsPerRow;
            PointsPerColumn = pointsPerColumn;
            for (int y = 1; y <= PointsPerColumn; y++)
            {
                for (int x = 1; x <= PointsPerRow; x++)
                {
                    //Ellipse figure = new Ellipse();
                    Rectangle figure = new Rectangle();
                    figure.Height = PointSize;
                    figure.Width = PointSize;
                    figure.Margin = new Thickness(x*GridSpacing, y*GridSpacing, 0, 0);

                    string coordinate = Coordinate(x, y);
                    AllPoints.Add(coordinate, new BuildingBlock(x, y, figure));

                    TheCanvas.Children.Add(figure);
                }
            }
            _vertexDatabase = new VertexDatabase();
        }

        public void CalculateAllNeighbours()
        {
            //NUnit.Framework.Assert.AreEqual(1, 2);
            Vertex vertexOne = new Vertex(this, 10, 10);
            _vertexDatabase.Add(vertexOne);
            Vertex vertexTwo = new Vertex(this, 10, 13);
            _vertexDatabase.Add(vertexTwo);
            vertexOne.FillVertexGrid(vertexTwo, ref _vertexDatabase);
            foreach (Vertex vertex in _vertexDatabase)
            {
                TheCanvas.Children.Add(vertex.ToDrawableObject());
            }
            foreach (BuildingBlock item in AllPoints.Values)
            {
                item.CalculateNeighbours(AllPoints);
            }
            CheckForConnectionsThroughDiagonalUnwalkableElements();
        }


        public void CheckForConnectionsThroughDiagonalUnwalkableElements()
        {
            string illegalConnectedPointCoordinateSetOne;
            string illegalConnectedPointCoordinateSetTwo;

            foreach (KeyValuePair<string, BuildingBlock> pair in AllPoints)
            {
                if (pair.Value.Elevation == BuildingBlock.ElevationTypes.Wall ||
                    pair.Value.Elevation == BuildingBlock.ElevationTypes.Furniture)
                {
                    foreach (BuildingBlock neighbour in pair.Value.Neighbours)
                    {
                        if (neighbour.DistanceToPoint(pair.Value) > 1) // Then it is a diagonal
                        {
                            illegalConnectedPointCoordinateSetOne = Coordinate(pair.Value.X,
                                neighbour.Y);
                            illegalConnectedPointCoordinateSetTwo = Coordinate(neighbour.X,
                                pair.Value.Y);
                            if (AllPoints.ContainsKey(illegalConnectedPointCoordinateSetOne) &&
                                AllPoints.ContainsKey(illegalConnectedPointCoordinateSetTwo))
                            {
                                AllPoints[illegalConnectedPointCoordinateSetOne].Neighbours.Remove(
                                    AllPoints[illegalConnectedPointCoordinateSetTwo]);
                                AllPoints[illegalConnectedPointCoordinateSetTwo].Neighbours.Remove(
                                    AllPoints[illegalConnectedPointCoordinateSetOne]);
                            }
                        }
                    }
                }
            }
        }


        public bool CheckConnection(BuildingBlock a, BuildingBlock b)
        {
            bool result = true;
            BuildingBlock p;
            AllPoints.TryGetValue(Coordinate(a.X, b.Y), out p);
            foreach (BuildingBlock point in ReturnLine(a, p))
            {
                if (point.Elevation == BuildingBlock.ElevationTypes.Wall)
                {
                    result = false;
                    break;
                }
            }
            if (result)
            {
                foreach (BuildingBlock point in ReturnLine(p, b))
                {
                    if (point.Elevation == BuildingBlock.ElevationTypes.Wall)
                    {
                        return false;
                    }
                }
            }
            result = true;
            AllPoints.TryGetValue(Coordinate(b.X, a.Y), out p);
            foreach (BuildingBlock point in ReturnLine(a, p))
            {
                if (point.Elevation == BuildingBlock.ElevationTypes.Wall)
                {
                    result = false;
                    break;
                }
            }
            if (result)
            {
                foreach (BuildingBlock point in ReturnLine(p, b))
                {
                    if (point.Elevation == BuildingBlock.ElevationTypes.Wall)
                    {
                        return false;
                    }
                }
            }
            return result;
        }

        public IEnumerable<BuildingBlock> ReturnLine(BuildingBlock a, BuildingBlock b)
        {
            if (a.X == b.X)
            {
                if (a.Y < b.Y)
                {
                    return ReturnLineX(a, b);
                }
                return ReturnLineX(b, a);
            }
            if (a.Y == b.Y)
            {
                if (a.Y < b.Y)
                {
                    return ReturnLineY(a, b);
                }
                return ReturnLineY(b, a);
            }
            throw new ArgumentException("De to punkter er ikke på linje.");
        }

        private IEnumerable<BuildingBlock> ReturnLineX(BuildingBlock a, BuildingBlock b)
        {
            List<BuildingBlock> points = new List<BuildingBlock>();
            for (int x = a.X; x < b.X; x++)
            {
                BuildingBlock point;
                AllPoints.TryGetValue(Coordinate(x, a.Y), out point);
                points.Add(point);
            }
            return points;
        }

        private IEnumerable<BuildingBlock> ReturnLineY(BuildingBlock a, BuildingBlock b)
        {
            List<BuildingBlock> points = new List<BuildingBlock>();
            for (int y = a.Y; y < b.Y; y++)
            {
                BuildingBlock point;
                AllPoints.TryGetValue(Coordinate(a.X, y), out point);
                points.Add(point);
            }
            return points;
        }
    }
}