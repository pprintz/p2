using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dijkstra {
    class Egde {
        public Vertex sourceVertex, destVertex;
        public double length;
        public Egde(Vertex from, Vertex to) {
            this.sourceVertex = from;
            this.destVertex = to;
            length = distEgde();
        }
        private double distEgde() {
            double length = Math.Sqrt(Math.Pow(Math.Abs(sourceVertex.x - destVertex.x), 2) +
                     Math.Pow(Math.Abs(sourceVertex.y - destVertex.y), 2));
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
