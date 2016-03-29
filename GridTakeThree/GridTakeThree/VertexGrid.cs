using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridTakeThree
{
    class VertexGrid
    {
        private Grid grid;

        public double vertexGridSize { get; set; } = 3;
        private VertexDatabase vertexDatabase;

        public VertexGrid(Grid grid)
        {
            this.grid = grid;
            Vertex verticeOne = new Vertex(grid, vertexGridSize, vertexGridSize);
            Vertex verticeTwo = new Vertex(grid, vertexGridSize, vertexGridSize*2);
            verticeOne.FillVertexGrid(verticeTwo, ref vertexDatabase);
        }
    }
}
