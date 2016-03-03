using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astar {
    class Program {
        static void Main(string[] args) {
            List<Vertex> vertices = new List<Vertex>();
            vertices.Add(new Vertex(1, 2, "a"));
            vertices.Add(new Vertex(9, 1, "b"));
            vertices.Add(new Vertex(-5, 5, "c"));
            vertices.Add(new Vertex(3, 2, "d"));
            vertices.Add(new Vertex(1, 6, "e"));
            vertices.Add(new Vertex(0, 0, "f"));
        }
        //F = G+h
    }
}
