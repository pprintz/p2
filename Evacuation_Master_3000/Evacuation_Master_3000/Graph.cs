using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;
using static Evacuation_Master_3000.ImportExportSettings;

namespace Evacuation_Master_3000 {
    class Graph {
        public List<BuildingBlock> vertices = new List<BuildingBlock>();
        private static bool firstRun = true;
        
        public Graph(List<BuildingBlock> vertices) {
            this.vertices = vertices;
        }

        public List<BuildingBlock> AStar(BuildingBlock source, BuildingBlock destination) {
            SortedSet<BuildingBlock> priorityQueue = new SortedSet<BuildingBlock>(Comparer<BuildingBlock>.Default);
            Dictionary<string, BuildingBlock> closedSet = new Dictionary<string, BuildingBlock>();
            List<BuildingBlock> unvisitedVertices = vertices.ToList();
            BuildingBlock current;

            foreach (BuildingBlock point in unvisitedVertices) {
                point.lengthToDestination = point.DistanceToPoint(destination);
                point.Parent = null;
                if (firstRun)
                    firstRun = false;
                else
                    point.LengthFromSource = 100000;
                point.isChecked = false;
            }

            unvisitedVertices.Remove(source);
            source.LengthFromSource = 0;
            unvisitedVertices.Insert(0, source);
            current = unvisitedVertices[0];

            while (current != destination) {
                foreach (BuildingBlock point in current.Neighbours) {
                    if (point.isChecked == false) {
                        if (!(closedSet.ContainsKey(Coordinate(point.X, point.Y)))) {
                            priorityQueue.Add(point);
                        }
                    }
                }
                CheckNeighbors(current, priorityQueue);
                if (priorityQueue.Count == 0) {
                    if (closedSet.ContainsKey(Coordinate(current.X, current.Y)) == false)
                        closedSet.Add(Coordinate(current.X, current.Y), current);

                    current = source;
                    foreach (BuildingBlock point in unvisitedVertices) {
                        point.isChecked = false;
                    }
                    continue;
                }
                current.isChecked = true;
                current = priorityQueue.First();
                priorityQueue.Clear();
            }

            List<BuildingBlock> path = new List<BuildingBlock>();
            BuildingBlock parent = destination;
            do {
                path.Add(parent);
                parent = parent.Parent;
            } while (parent != null);
            path.Reverse();
            return path;
        }

        public void CheckNeighbors(BuildingBlock currentPoint, SortedSet<BuildingBlock> priorityQueue) {
            foreach (BuildingBlock neighbour in priorityQueue) {
                if (currentPoint.DistanceToPoint(neighbour) + currentPoint.LengthFromSource < neighbour.LengthFromSource) {
                    neighbour.LengthFromSource = currentPoint.DistanceToPoint(neighbour) + currentPoint.LengthFromSource;
                    neighbour.Parent = currentPoint;
                }
            }
        }
    }
}
