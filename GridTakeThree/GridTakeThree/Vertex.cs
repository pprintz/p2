using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace GridTakeThree
{
    class Vertex
    {
        public double x, y;
        private Grid grid;

        public List<Vertex> neighbours;

        /// <summary>
        /// Attempt to add two vertices "this" and "vertex" to eachothers neighbours.
        /// </summary>
        /// <param name="vertex">Other vector to add</param>
        /// <returns>Returns wether or not the attempt was successful.</returns>
        public bool SetNeighbour(Vertex vertex)
        {
            if (vertex.neighbours.Contains(this))
                return false;
            Point a, b;
            grid.AllPoints.TryGetValue($"({(int)x}, {(int)y})", out a);
            grid.AllPoints.TryGetValue($"({(int)vertex.x}, {(int)vertex.y})", out b);
            if (grid.CheckConnection(a, b))
            {
                neighbours.Add(vertex);
                vertex.neighbours.Add(this);
                return true;
            }
            return false;
        }

        public void FillVertexGrid(Vertex vertex, ref VertexDatabase database)
        {
            if (SetNeighbour(vertex))
            {
                double 
                    x0 = x,
                    y0 = y,
                    x1 = vertex.x,
                    y1 = vertex.y,
                    d = Math.Sqrt(Math.Pow(x1 - x0, 2) + Math.Pow(y1 - y0, 2)),
                    deltaX = x1 - x0,
                    deltaY = y1 - y0,
                    x2 = deltaX + x0,
                    y2 = deltaY + y0,
                    x3 = (x2 + deltaY) / d,
                    y3 = (y2 - deltaX) / d,
                    x4 = (x2 - deltaY) / d,
                    y4 = (y2 + deltaX) / d;
                if (0 < x3 && x3 < grid.PointsPerRow && 0 < y3 && y3 < grid.PointsPerColumn)
                {
                    Vertex vertexOne = new Vertex(grid, x3, y3);
                    if (database.Add(vertexOne))
                    { FillVertexGrid(vertexOne, ref database); }
                }
                if (0 < x4 && x4 < grid.PointsPerRow && 0 < y4 && y4 < grid.PointsPerColumn)
                {
                    Vertex vertexTwo = new Vertex(grid, x4, y4);
                    if (database.Add(vertexTwo))
                    { FillVertexGrid(vertexTwo, ref database); }
                }
            }
        }

        public Label ToDrawableObject()
        {
            Label label = new Label();
            label.Content = "x";
            label.Margin = new Thickness(x * grid.GridSpacing, y * grid.GridSpacing, 0, 0);
            return label;
        }

        public Vertex(Grid grid, double x, double y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
            neighbours = new List<Vertex>();
        }
    }
}
