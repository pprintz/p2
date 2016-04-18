#region

using System;
using System.Collections.Generic;
using System.Linq;
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
        private int PointSize { get; } = 10;
        private Canvas TheCanvas { get; set; }
        public int PointsPerRow { get; private set; }
        public int PointsPerColumn { get; private set; }

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
                    Rectangle figure = new Rectangle
                    {
                        Height = PointSize,
                        Width = PointSize,
                        Margin = new Thickness(x*GridSpacing, y*GridSpacing, 0, 0)
                    };

                    string coordinate = Coordinate(x, y);
                    AllPoints.Add(coordinate, new BuildingBlock(x, y, figure));

                    TheCanvas.Children.Add(figure);
                }
            }
            _vertexDatabase = new VertexDatabase();
        }

        public void CalculateAllNeighbours()
        {
            //Vertex vertexOne = new Vertex(this, 10, 10);
            //_vertexDatabase.Add(vertexOne);
            //Vertex vertexTwo = new Vertex(this, 10, 13);
            //_vertexDatabase.Add(vertexTwo);
            //vertexOne.FillVertexGrid(vertexTwo, ref _vertexDatabase);
            //foreach (Vertex vertex in _vertexDatabase)
            //{
            //    TheCanvas.Children.Add(vertex.ToDrawableObject());
            //}
            foreach (BuildingBlock item in AllPoints.Values)
            {
                item.CalculateNeighbours(AllPoints);
            }
            CheckForConnectionsThroughDiagonalUnwalkableElements();
        }


        private void CheckForConnectionsThroughDiagonalUnwalkableElements()
        {
            foreach (KeyValuePair<string, BuildingBlock> pair in AllPoints)
            {
                if (pair.Value.Elevation == BuildingBlock.ElevationTypes.Wall ||
                    pair.Value.Elevation == BuildingBlock.ElevationTypes.Furniture)
                {
                    foreach (BuildingBlock neighbour in pair.Value.Neighbours)
                    {
                        if (neighbour.DistanceToPoint(pair.Value) > 1) // Then it is a diagonal
                        {
                            var illegalConnectedPointCoordinateSetOne = Coordinate(pair.Value.X,
                                neighbour.Y);
                            var illegalConnectedPointCoordinateSetTwo = Coordinate(neighbour.X,
                                pair.Value.Y);
                            if (!AllPoints.ContainsKey(illegalConnectedPointCoordinateSetOne) ||
                                !AllPoints.ContainsKey(illegalConnectedPointCoordinateSetTwo)) continue;
                            AllPoints[illegalConnectedPointCoordinateSetOne].Neighbours.Remove(
                                AllPoints[illegalConnectedPointCoordinateSetTwo]);
                            AllPoints[illegalConnectedPointCoordinateSetTwo].Neighbours.Remove(
                                AllPoints[illegalConnectedPointCoordinateSetOne]);
                        }
                    }
                }
            }
        }


        public bool CheckConnection(BuildingBlock a, BuildingBlock b)
        {
            BuildingBlock p;
            AllPoints.TryGetValue(Coordinate(a.X, b.Y), out p);
            bool result = ReturnLine(a, p).All(point => point.Elevation != BuildingBlock.ElevationTypes.Wall);
            if (result)
            {
                if (ReturnLine(p, b).Any(point => point.Elevation == BuildingBlock.ElevationTypes.Wall))
                {
                    return false;
                }
            }
            AllPoints.TryGetValue(Coordinate(b.X, a.Y), out p);
            result = ReturnLine(a, p).All(point => point.Elevation != BuildingBlock.ElevationTypes.Wall);
            if (!result) return false;
            {
                if (ReturnLine(p, b).Any(point => point.Elevation == BuildingBlock.ElevationTypes.Wall))
                {
                    return false;
                }
            }
            return true;
        }

        private IEnumerable<BuildingBlock> ReturnLine(BuildingBlock a, BuildingBlock b)
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