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
    class Grid_new // IFloorPlan
    {
        //private VertexDatabase _vertexDatabase;
        private int PointSize { get; } = 10;
        private Canvas TheCanvas { get; set; }

        public int GridSpacing { get; } = 10;
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
            for (int y = 0; y < PointsPerColumn; y++)
            {
                for (int x = 0; x < PointsPerRow; x++)
                {
                    //Ellipse figure = new Ellipse();
                    Rectangle figure = new Rectangle
                    {
                        Height = PointSize,
                        Width = PointSize,
                        Margin = new Thickness(x*GridSpacing, y*GridSpacing, 0, 0)
                    };

                    //string coordinate = Coordinate(x, y);
                    //AllPoints.Add(coordinate, new BuildingBlock(x, y, figure));

                    TheCanvas.Children.Add(figure);
                }
            }
            //_vertexDatabase = new VertexDatabase();
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
                if (pair.Value.Type == BuildingBlock.Types.Wall ||
                    pair.Value.Type == BuildingBlock.Types.Furniture)
                {
                    foreach (BuildingBlock neighbour in pair.Value.Neighbours)
                    {
                        //if (neighbour.DistanceTo(pair.Value) > 1) // Then it is a diagonal
                        //{
                        //    var illegalConnectedPointCoordinateSetOne = Coordinate(pair.Value.X,
                        //        neighbour.Y);
                        //    var illegalConnectedPointCoordinateSetTwo = Coordinate(neighbour.X,
                        //        pair.Value.Y);
                        //    if (!AllPoints.ContainsKey(illegalConnectedPointCoordinateSetOne) ||
                        //        !AllPoints.ContainsKey(illegalConnectedPointCoordinateSetTwo)) continue;
                        //    AllPoints[illegalConnectedPointCoordinateSetOne].Neighbours.Remove(
                        //        AllPoints[illegalConnectedPointCoordinateSetTwo]);
                        //    AllPoints[illegalConnectedPointCoordinateSetTwo].Neighbours.Remove(
                        //        AllPoints[illegalConnectedPointCoordinateSetOne]);
                        //}
                    }
                }
            }
        }
    }
}