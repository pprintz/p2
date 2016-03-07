using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace GridTakeThree
{
    class Graph
    {
        public List<Point> vertices = new List<Point>();
        private static bool firstRun = true;

        public Graph(List<Point> vertices)
        {
            this.vertices = vertices;
        }

        public List<Point> dijkstra(Point source, Point destination)
        {
            OrderedBag<Point> priorityQueue = new OrderedBag<Point>();
            Dictionary<string, Point> closedSet = new Dictionary<string, Point>();
            List<Point> unvisitedVertices = vertices.ToList();
            Point current;

            foreach (Point point in unvisitedVertices)
            {
                point.lengthToDestination = point.DistanceToPoint(destination);
                if (firstRun)
                    firstRun = false;
                else
                    point.LengthFromSource = 100000;
            }

            unvisitedVertices.Remove(source);
            source.LengthFromSource = 0;
            unvisitedVertices.Insert(0, source);
            current = unvisitedVertices[0];

            while (current != destination)
            {
                foreach (Point point in current.Neighbours)
                {
                    if (point.isChecked == false)
                    {
                        if (!(closedSet.ContainsKey($"{point.X},{point.Y}")))
                        {
                            priorityQueue.Add(point);
                        }
                    }
                }
                checkEgdes(current, priorityQueue);
                if (priorityQueue.Count == 0)
                {
                    closedSet.Add($"{current.X},{current.Y}", current);
                    current = source;
                    foreach (Point point in unvisitedVertices)
                    {
                        point.isChecked = false;
                    }
                    continue;
                }
                current.isChecked = true;
                current = priorityQueue[0];
                priorityQueue.Clear();
            }

            List<Point> path = new List<Point>();
            Point parent = destination;
            do
            {
                path.Add(parent);
                parent.Elevation = Point.ElevationTypes.Exit;
                parent.ColorizePoint();
                parent = parent.Parent;
            } while (parent != null);
            path.Reverse();
            return path;
        }

        public void checkEgdes(Point currentPoint, OrderedBag<Point> priorityQueue)
        {
            foreach (Point neighbour in priorityQueue)
            {
                if (currentPoint.DistanceToPoint(neighbour) + currentPoint.LengthFromSource < neighbour.LengthFromSource)
                {
                    neighbour.LengthFromSource = currentPoint.DistanceToPoint(neighbour) + currentPoint.LengthFromSource;
                    neighbour.Parent = currentPoint;
                }
            }
        }
    }
}
