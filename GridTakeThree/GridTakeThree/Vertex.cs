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

        /// <summary>
        /// Attempt to add two vertices "this" and "vertex" to eachothers neighbours.
        /// </summary>
        /// <param name="vertex">Other vector to add</param>
        /// <returns>Returns weather or not the attempt was successful.</returns>
        public bool SetNeighbour(Vertex vertex)
        {
            if (vertex.neighbours.Contains(this))
                return false;
            Point a, b;
            grid.AllPoints.TryGetValue($"({x}, {y})", out a);
            grid.AllPoints.TryGetValue($"({vertex.x}, {vertex.y})", out b);
            if (grid.CheckConnection(a,b))
            {
                neighbours.Add(vertex);
                vertex.neighbours.Add(this);
                return true;
            }
            return false;
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
