using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2 {
    class Graph {
        public Vertex initial;
        public Vertex current;
        public Vertex dest;
        public string path;
        List<Vertex> vertices = new List<Vertex>();
        List<Vertex> unvisitedVertices = new List<Vertex>();
        public Graph(Vertex source, Vertex dest, List<Vertex> vertices) {
            this.initial = source;
            this.dest = dest;
            this.vertices = vertices;
        }
        public void dijkstra() {
            foreach (Vertex vertex in vertices) {
                unvisitedVertices.Add(vertex);
            }
            initial.lenFromSource = 0;
            while(unvisitedVertices.Count != 0) {
                Vertex current = unvisitedVertices[0];
                unvisitedVertices.Remove(current);
                if (current == dest) {
                    break;
                }
                checkEgdes(current);
            }
        }
        public void checkEgdes(Vertex vertex) {
            foreach (Egde egde in vertex.egdeList) {
                 if (egde.length + vertex.lenFromSource < egde.destVertex.lenFromSource) {
                    egde.destVertex.lenFromSource = egde.length + vertex.lenFromSource;
                    egde.destVertex.parent = egde.sourceVertex;
                    }
                } 
        } 
        public void printPathAndLength() {
            Vertex parenthee = dest;
            List<string> path = new List<string>();
            Console.WriteLine();
            do {
                path.Add(parenthee.name);
                parenthee = parenthee.parent;
            } while (parenthee.parent != null);
            path.Reverse();
            Console.Write($"{initial.name} --> ");
            foreach (string item in path) {
                if (item != path[path.Count-1]) {
                    Console.Write(item + " --> ");
                }
                else {
                    Console.Write(item);
                }
            }
            Console.WriteLine();
            Console.WriteLine($"Length from {initial.name} to {dest.name} is:\n{dest.lenFromSource}");
        }
    }
}
