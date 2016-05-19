using System;
using System.Collections.Generic;
using System.Linq;

namespace Evacuation_Master_3000
{
    internal class AStar : IPathfinding
    {
        public AStar(IFloorPlan floorPlan)
        {
            TheFloorPlan = floorPlan;
            ListOfBuildingBlocks = TheFloorPlan.Tiles.Values.Cast<BuildingBlock>().ToList();
        }
        public IFloorPlan TheFloorPlan { get; }

        private List<BuildingBlock> ListOfBuildingBlocks { get; set; }

        private BuildingBlock FindNextPathTarget(IEvacuateable person)
        {
            int targetPriority;

            BuildingBlock currentPosition = person.Position as BuildingBlock;
            if (currentPosition == null) return null;

            if (currentPosition.Priority % 2 == 0)
            {
                targetPriority = currentPosition.Priority - 2;
            }
            else
            {
                targetPriority = currentPosition.Priority - 1;
            }
            if (person.Position.Type == Tile.Types.Stair && targetPriority >= 1000)
            {
                targetPriority += 2;
            }
            return ListOfBuildingBlocks.Where(
                 b =>
                    b.Priority == targetPriority && b.Z == person.Position.Z &&
                    b.BuildingBlockNeighbours.Any(n => n.Room == person.CurrentRoom)).OrderBy(b => b.DistanceTo(person.Position)).First();
        }
        public IEnumerable<Tile> CalculatePath(IEvacuateable person)
        {
            /* Exception needed for when not path is given, and when a person can't find a route. */
            BuildingBlock current = person.Position as BuildingBlock;
            if (current == null) return null;

            person.CurrentRoom = current.Room;
            List<Tile> pathList = new List<Tile>();
            while (current.Type != Tile.Types.Exit)
            {
                if (current.Priority == int.MaxValue)
                {
                    throw new PersonException(); //<<---- Personen kan ikke finde ud?? I så fald skal der ikke kastes en exception, men i stedet skal personen håndteres (som i tages ud af simuleringen, der skal gives besked til analyse-delen, at personen ikke er medtaget i simuleringen osv osv osv)
                }
                BuildingBlock destination = FindNextPathTarget(person);
                pathList.AddRange(GetPathFromSourceToDestinationAStar(person, destination));
                if (destination.Type == Tile.Types.Stair)
                {
                    if (destination.BuildingBlockNeighbours.Count(n => n.Priority < destination.Priority) != 0)
                    {
                        destination = destination.BuildingBlockNeighbours.First(n => n.Priority < destination.Priority);
                    }
                    else if (destination.BuildingBlockNeighbours.Count(n => n.Z > destination.Z) != 0)
                    {
                        destination = destination.BuildingBlockNeighbours.First(n => n.Z > destination.Z);
                    }
                    person.Position = destination;
                    person.CurrentRoom = destination.Room;
                    destination = FindNextPathTarget(person);
                    pathList.AddRange(GetPathFromSourceToDestinationAStar(person, destination));
                }
                if (destination.Type == Tile.Types.Exit)
                {
                    break;
                }
                if (destination.Priority % 2 == 0 && destination.Type == Tile.Types.Door)
                {
                    person.Position =
                        destination.BuildingBlockNeighbours.Where(
                            n =>
                                n.Type == Tile.Types.Free &&
                                n.Room != person.CurrentRoom && n.Room != 0)
                            .OrderBy(n => n.DistanceTo(destination))
                            .First();
                    person.CurrentRoom = ((person.Position as BuildingBlock).Room);
                }
            }
            person.Position = pathList.First();
            return pathList.Distinct();
        }

        private IEnumerable<Tile> GetPathFromSourceToDestinationAStar(IEvacuateable person, BuildingBlock destination)
        {
            bool firstRun = true;
            SortedSet<BuildingBlock> priorityQueue = new SortedSet<BuildingBlock>(Comparer<BuildingBlock>.Default);
            Dictionary<string, BuildingBlock> closedSet = new Dictionary<string, BuildingBlock>();
            List<BuildingBlock> unvisitedVertices = TheFloorPlan.Tiles.Values.Cast<BuildingBlock>().ToList();

            foreach (BuildingBlock point in unvisitedVertices)
            {
                point.LengthToDestination = point.DiagonalDistanceTo(destination);
                point.Parent = null;
                if (firstRun)
                    firstRun = false;
                else
                    point.LengthFromSource = 100000;
                point.IsChecked = false;
            }

            unvisitedVertices.Remove(person.Position as BuildingBlock);
            BuildingBlock currentPosition = person.Position as BuildingBlock;
            if (currentPosition == null) return null;
            currentPosition.LengthFromSource = 0;
            unvisitedVertices.Insert(0, currentPosition);

            while (!Equals(currentPosition, destination))
            {
                foreach (BuildingBlock point in currentPosition.BuildingBlockNeighbours)
                {
                    if (point.IsChecked == false)
                    {
                        if (!closedSet.ContainsKey(ImportExportSettings.Coordinate(point.X, point.Y, point.Z)))
                        {
                            priorityQueue.Add(point);
                        }
                    }
                }
                CheckNeighbors(currentPosition, priorityQueue);
                if (priorityQueue.Count == 0)
                {
                    if (closedSet.ContainsKey(ImportExportSettings.Coordinate(currentPosition.X, currentPosition.Y, currentPosition.Z)) == false)
                        closedSet.Add(ImportExportSettings.Coordinate(currentPosition.X, currentPosition.Y, currentPosition.Z), currentPosition);

                    foreach (BuildingBlock point in unvisitedVertices)
                    {
                        point.IsChecked = false;
                    }
                    continue;
                }
                currentPosition.IsChecked = true;
                currentPosition = priorityQueue.First();
                priorityQueue.Clear();
            }

            List<BuildingBlock> path = new List<BuildingBlock>();
            BuildingBlock parent = destination;
            do
            {
                path.Add(parent);
                parent = parent.Parent;
            } while (parent != null);
            path.Reverse();
            return path;
        }
        private void CheckNeighbors(BuildingBlock currentPoint, SortedSet<BuildingBlock> priorityQueue)
        {
            foreach (BuildingBlock neighbour in priorityQueue)
            {
                if (currentPoint.DistanceTo(neighbour) + currentPoint.LengthFromSource < neighbour.LengthFromSource)
                {
                    neighbour.LengthFromSource = currentPoint.DistanceTo(neighbour) + currentPoint.LengthFromSource;
                    neighbour.Parent = currentPoint;
                }
            }
        }
    }
}