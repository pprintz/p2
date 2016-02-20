using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2 {
    class Vertex {

        static public int vertexId = 0;
        public int id = 0;
        public int x { get; }
        public int y { get; }

        public Vertex(int x, int y) {
            this.x = x;
            this.y = y;
            id = vertexId++;
        }
        public List<Egde> egdeList = new List<Egde>();
        public void addEgde(Vertex sourceVertex, Vertex destVertex) {
            egdeList.Add(new Egde(sourceVertex, destVertex));
       }
        public string coords() {
            return $"{x},{y}";
        }
    }
    class Egde {
        Vertex sourceVertex, destVertex;
        public double length { get; set; }
        public Egde(Vertex from, Vertex to) {
            this.sourceVertex = from;
            this.destVertex = to;
            length = distEgde();
        }
        private double distEgde() {
            length = Math.Sqrt(Math.Pow(Math.Abs(sourceVertex.x - destVertex.x), 2) +
                     Math.Pow(Math.Abs(sourceVertex.y - destVertex.y), 2));
            return length;
        }
        public static void printEgde(Vertex a) {
            foreach (Egde egde in a.egdeList) {
                Console.WriteLine($"{egde.sourceVertex.id}:{egde.sourceVertex.coords()} --> {egde.destVertex.id}:{egde.destVertex.coords()} Length: {egde.length} ");
            }
        }
    }
    class Graph {
        Vertex source;
        Vertex dest;
        List<Vertex> vertices = new List<Vertex>();
        public Graph(Vertex source, Vertex dest, List<Vertex> vertices) {
            this.source = source;
            this.dest = dest;
            this.vertices = vertices;
        }
    }
}

