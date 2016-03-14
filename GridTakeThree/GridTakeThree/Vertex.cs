using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GridTakeThree
{
    class Vertex
    {
        public int x, y;
        private Grid grid;

        public List<Vertex> neighbours;

        public void SetNeighbour(Vertex vertex)
        {
            if (vertex.neighbours.Contains(this))
                return;
            Point a, b;
            grid.AllPoints.TryGetValue($"({x}, {y})", out a);
            grid.AllPoints.TryGetValue($"({vertex.x}, {vertex.y})", out b);
            if (grid.CheckConnection(a,b))
            {
                neighbours.Add(vertex);
                vertex.neighbours.Add(this);
            }
        }

        public Vertex(Grid grid, int x, int y)
        {
            this.grid = grid;
            this.x = x;
            this.y = y;
            neighbours = new List<Vertex>();
        }
    }
}
