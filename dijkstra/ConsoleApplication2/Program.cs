using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dijkstra {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("List of egdes");
            List<Vertex> vertices = new List<Vertex>();
            Vertex a = new Vertex(0, 0, "a");
            Vertex b = new Vertex(3, 0, "b");
            Vertex c = new Vertex(1, -1, "c");
            Vertex d = new Vertex(2, -6, "d");
            Vertex e = new Vertex(2, -8, "e");

            vertices.Add(a);
            vertices.Add(b);
            vertices.Add(c);
            vertices.Add(d);
            vertices.Add(e);
            
            c.addEgde(c, a, b, d);
            a.addEgde(a, e);
            b.addEgde(b, e);
            d.addEgde(d, e);

            Graph graf = new Graph(c, e, vertices);
            Graph graft= new Graph(a, e, vertices);

            foreach (Vertex vertex in vertices) {
                Egde.printEgdeCoordsAndLen(vertex);
            }
            graf.dijkstra();
            graf.printPathAndLength();
            Console.ReadKey();
        }
    }
}
