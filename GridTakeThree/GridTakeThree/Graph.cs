using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridTakeThree {
    class Graph {
        List<Point> vertices = new List<Point>();
        public Point current;
        public Graph(List<Point> vertices) {
            this.vertices = vertices;
        }

        public List<Point> dijkstra(Point source, Point destination) {
            List<Point> unvisitedVertices = new List<Point>();
            foreach (Point vertex in vertices) {
                unvisitedVertices.Add(vertex);
            }
            unvisitedVertices.Remove(source);
            unvisitedVertices.Remove(destination);
            unvisitedVertices.Add(destination);
            unvisitedVertices.Insert(0, source);

            source.LenghtFromSource = 0;
            while(unvisitedVertices.Count != 0) {
                if (current == destination) {
                    break;
                }
                current = unvisitedVertices[0];
                unvisitedVertices.Remove(current);
                checkEgdes(current);
            }

            List<Point> path = new List<Point>();
            Point parent = destination;
            do {
                path.Add(parent);
                parent.Elevation = Point.ElevationTypes.Exit;
                parent.ColorizePoint();
                parent = parent.Parent;
            } while (parent != null);
            path.Reverse();
            return path;
        }

        public void checkEgdes(Point currentPoint) {
            foreach (Point neighbour in currentPoint.Neighbours) {
                 if (currentPoint.DistanceToPoint(neighbour) + currentPoint.LenghtFromSource < neighbour.LenghtFromSource) {
                    neighbour.LenghtFromSource = currentPoint.DistanceToPoint(neighbour) + currentPoint.LenghtFromSource;
                    neighbour.Parent = currentPoint;
                    }
                } 
        } 
        /*static public void printPathAndLength(List<Point> path) {
            Console.WriteLine();
            foreach (Point item in path) {
                if (item != path[path.Count-1]) {
                    Console.Write(item.name + " --> ");
                }
                else {
                    Console.Write(item.name);
                }
            }
            Console.WriteLine();
            Console.WriteLine($"Length from {path[0].name} to {path[path.Count-1].name} is:\n{path[path.Count-1].lenFromSource}");
        }*/
    }
}
