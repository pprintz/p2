using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astar {
    class Egde {
        public Vertex sourceVertex, destVertex;
        public double length;
        public Egde(Vertex from, Vertex to) {
            this.sourceVertex = from;
            this.destVertex = to;
            length = calculateDist(sourceVertex, destVertex);
        }
        public static double calculateDist(Vertex a, Vertex b) {
            double length = Math.Sqrt(Math.Pow(Math.Abs(a.x - b.x), 2) +
                     Math.Pow(Math.Abs(a.y - b.y), 2));
            return length;
        }
        public static void printEgdeCoordsAndLen(Vertex a) {
            foreach (Egde egde in a.egdeList) {
                Console.Write($"{egde.sourceVertex.name}({egde.sourceVertex.getCoords()})");
                Console.WriteLine($"--->{egde.destVertex.name}({egde.destVertex.getCoords()}) length:{egde.length}");
            }
        }
    }
}
