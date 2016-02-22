using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dijkstra {
    class Graph {
        public Vertex source;
        public Vertex current;
        public Vertex dest;
        public string path;
        List<Vertex> vertices = new List<Vertex>();
        List<Vertex> unvisitedVertices = new List<Vertex>();
        public Graph(Vertex source, Vertex dest, List<Vertex> vertices) {
            this.source = source;
            this.dest = dest;
            this.vertices = vertices;
        }
        public void dijkstra() {
            foreach (Vertex vertex in vertices) {
                unvisitedVertices.Add(vertex);
            }
            source.lenFromSource = 0;
            while(unvisitedVertices.Count != 0) {
                Vertex current = unvisitedVertices[0];
                unvisitedVertices.Remove(current);
                if (current == dest) {
                    break;
                }
                checkEgdes(current);
            }
        }
        public void checkEgdes(Vertex current) {
            foreach (Egde egde in current.egdeList) {
                 if (egde.length + current.lenFromSource < egde.destVertex.lenFromSource) {
                    egde.destVertex.lenFromSource = egde.length + current.lenFromSource;
                    egde.destVertex.parent = egde.sourceVertex;
                    }
                } 
        } 
        public void printPathAndLength() {
            Vertex parent = dest;
            List<string> path = new List<string>();
            Console.WriteLine();
            do {
                path.Add(parent.name);
                parent = parent.parent;
            } while (parent.parent != null);
            path.Reverse();
            Console.Write($"{source.name} --> ");
            foreach (string item in path) {
                if (item != path[path.Count-1]) {
                    Console.Write(item + " --> ");
                }
                else {
                    Console.Write(item);
                }
            }
            Console.WriteLine();
            Console.WriteLine($"Length from {source.name} to {dest.name} is:\n{dest.lenFromSource}");
        }
    }
}
