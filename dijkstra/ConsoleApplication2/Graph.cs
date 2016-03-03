using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dijkstra {
    class Graph {
        List<Vertex> vertices = new List<Vertex>();
        public Vertex current;
        public Graph(List<Vertex> vertices) {
            this.vertices = vertices;
        }
        public List<Vertex> dijkstra(Vertex source, Vertex destination) {
            List<Vertex> unvisitedVertices = new List<Vertex>();
            foreach (Vertex vertex in vertices) {
                unvisitedVertices.Add(vertex);
            }
            unvisitedVertices.Remove(source);
            unvisitedVertices.Remove(destination);
            unvisitedVertices.Add(destination);
            unvisitedVertices.Insert(0, source);

            source.lenFromSource = 0;
            while(unvisitedVertices.Count != 0) {
                if (current == destination) {
                    break;
                }
                current = unvisitedVertices[0];
                unvisitedVertices.Remove(current);
                checkEgdes(current);
            }

            List<Vertex> path = new List<Vertex>();
            Vertex parent = destination;
            do {
                path.Add(parent);
                parent = parent.parent;
            } while (parent != null);
            path.Reverse();
            return path;
        }
        public void checkEgdes(Vertex current) {
            foreach (Egde egde in current.egdeList) {
                 if (egde.length + current.lenFromSource < egde.destVertex.lenFromSource) {
                    egde.destVertex.lenFromSource = egde.length + current.lenFromSource;
                    egde.destVertex.parent = egde.sourceVertex;
                    }
                } 
        } 
        static public void printPathAndLength(List<Vertex> path) {
            Console.WriteLine();
            foreach (Vertex item in path) {
                if (item != path[path.Count-1]) {
                    Console.Write(item.name + " --> ");
                }
                else {
                    Console.Write(item.name);
                }
            }
            Console.WriteLine();
            Console.WriteLine($"Length from {path[0].name} to {path[path.Count-1].name} is:\n{path[path.Count-1].lenFromSource}");
        }
    }
}
