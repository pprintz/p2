#region

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using static Evacuation_Master_3000.ImportExportSettings;

#endregion

namespace Evacuation_Master_3000
{
    class Vertex
    {
        private readonly Grid _grid;

        public List<Vertex> Neighbours;
        public double X, Y;

        public Vertex(Grid grid, double x, double y)
        {
            this._grid = grid;
            X = x;
            Y = y;
            Neighbours = new List<Vertex>();
        }

        /// <summary>
        ///     Attempt to add two vertices "this" and "vertex" to eachothers neighbours.
        /// </summary>
        /// <param name="vertex">Other vector to add</param>
        /// <returns>Returns wether or not the attempt was successful.</returns>
        public bool SetNeighbour(Vertex vertex)
        {
            if (vertex.Neighbours.Contains(this))
                return false;
            BuildingBlock a, b;
            _grid.AllPoints.TryGetValue(Coordinate((int) X, (int) Y), out a);
            _grid.AllPoints.TryGetValue(Coordinate((int) vertex.X, (int) vertex.Y), out b);
            if (_grid.CheckConnection(a, b))
            {
                Neighbours.Add(vertex);
                vertex.Neighbours.Add(this);
                return true;
            }
            return false;
        }

        public void FillVertexGrid(Vertex vertex, ref VertexDatabase database)
        {
            if (SetNeighbour(vertex))
            {
                double
                    x0 = X,
                    y0 = Y,
                    x1 = vertex.X,
                    y1 = vertex.Y,
                    deltaX = x1 - x0,
                    deltaY = y1 - y0,
                    dist = Math.Sqrt(Math.Abs(deltaX*deltaX - deltaY*deltaY)),
                    a = dist*dist/(2*dist),
                    h = Math.Sqrt(dist*dist - a*a),
                    x2 = x0 + a*deltaX/dist,
                    y2 = y0 + a*deltaY/dist,
                    x3 = x2 + h*deltaY/dist,
                    y3 = y2 - h*deltaX/dist,
                    x4 = x2 - h*deltaY/dist,
                    y4 = y2 + h*deltaX/dist;

                Vertex vertexOne = null;
                Vertex vertexTwo = null;
                if (0 < (int) x3 && (int) x3 < _grid.PointsPerRow && 0 < (int) y3 && (int) y3 < _grid.PointsPerColumn)
                {
                    vertexOne = new Vertex(_grid, x3, y3);
                    vertexOne = database.Add(vertexOne);
                    vertexOne.FillVertexGrid(this, ref database);
                }
                if (0 < (int) x4 && (int) x4 < _grid.PointsPerRow && 0 < (int) y4 && (int) y4 < _grid.PointsPerColumn)
                {
                    vertexTwo = new Vertex(_grid, x4, y4);
                    vertexTwo = database.Add(vertexTwo);
                    vertexTwo.FillVertexGrid(this, ref database);
                }
                if (vertexOne != null && vertexTwo != null)
                {
                    vertexOne.FillVertexGrid(vertexTwo, ref database);
                }
            }
        }

        public Label ToDrawableObject()
        {
            Label label = new Label();
            label.Content = "x";
            label.Margin = new Thickness(X*_grid.GridSpacing, Y*_grid.GridSpacing, 0, 0);
            return label;
        }
    }
}