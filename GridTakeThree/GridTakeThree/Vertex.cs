using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using static GridTakeThree.ImportExportSettings;

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
            grid.AllPoints.TryGetValue(Coordinate((int)x, (int)y), out a);
            grid.AllPoints.TryGetValue(Coordinate((int)vertex.x, (int)vertex.y), out b);
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
                    deltaX = x1 - x0,
                    deltaY = y1 - y0,
                    dist = Math.Sqrt(Math.Abs(deltaX * deltaX - deltaY * deltaY)),
                    a = (dist * dist) / (2 * dist),
                    h = Math.Sqrt(dist * dist - a * a),
                    x2 = x0 + a * deltaX / dist,
                    y2 = y0 + a * deltaY / dist,
                    x3 = x2 + h * deltaY / dist,
                    y3 = y2 - h * deltaX / dist,
                    x4 = x2 - h * deltaY / dist,
                    y4 = y2 + h * deltaX / dist;

                Vertex vertexOne = null;
                Vertex vertexTwo = null;
                if (0 < (int)x3 && (int)x3 < grid.PointsPerRow && 0 < (int)y3 && (int)y3 < grid.PointsPerColumn)
                {
                    vertexOne = new Vertex(grid, x3, y3);
                    vertexOne = database.Add(vertexOne);
                    vertexOne.FillVertexGrid(this, ref database);
                }
                if (0 < (int)x4 && (int)x4 < grid.PointsPerRow && 0 < (int)y4 && (int)y4 < grid.PointsPerColumn)
                {
                    vertexTwo = new Vertex(grid, x4, y4);
                    vertexTwo = database.Add(vertexTwo);
                    vertexTwo.FillVertexGrid(this, ref database);
                }
                if (vertexOne != null && vertexTwo != null)
                {
                    vertexOne.FillVertexGrid(vertexTwo,ref database);
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
