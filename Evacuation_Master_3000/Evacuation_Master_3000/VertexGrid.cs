namespace Evacuation_Master_3000
{
    class VertexGrid
    {
        private Grid _grid;
        private readonly VertexDatabase _vertexDatabase;

        public VertexGrid(Grid grid)
        {
            _grid = grid;
            Vertex verticeOne = new Vertex(grid, VertexGridSize, VertexGridSize);
            Vertex verticeTwo = new Vertex(grid, VertexGridSize, VertexGridSize*2);
            verticeOne.FillVertexGrid(verticeTwo, ref _vertexDatabase);
        }

        public double VertexGridSize { get; set; } = 3;
    }
}