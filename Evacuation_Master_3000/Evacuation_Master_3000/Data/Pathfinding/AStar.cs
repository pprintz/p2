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

        private List<BuildingBlock> ListOfBuildingBlocks { get; }

        private BuildingBlock FindNextPathTarget(IEvacuateable person)
        {
            int targetPriority;
            if (((BuildingBlock) person.Position).Priority % 2 == 0)
            {
                targetPriority = ((BuildingBlock) person.Position).Priority - 2;
            }
            else
            {
                targetPriority = ((BuildingBlock) person.Position).Priority - 1;
            }
            if (person.Position.Type == Tile.Types.Stair && targetPriority != 0)
            {
                targetPriority += 2;
            }
            return ListOfBuildingBlocks.Where(
                 b =>
                    b.Priority == targetPriority && b.Z == person.Position.Z &&
                    b.BNeighbours.Any(n => n.Room == person.CurrentRoom)).OrderBy(b => b.DistanceTo(person.Position)).First();
        }
        public IEnumerable<Tile> CalculatePath(IEvacuateable person)
        {
            /* Exception needed for when not path is given, and when a person can't find a route. */
            List<BuildingBlock> exitList =
                ListOfBuildingBlocks.Where(b => b.Type == Tile.Types.Exit)
                    .OrderBy(b => b.DistanceTo(person.Position)).ToList();

            person.CurrentRoom = ((BuildingBlock) person.Position).Room;
            List<Tile> pathList = new List<Tile>();
            BuildingBlock theBlock = (BuildingBlock) person.Position;
            while (theBlock != null && theBlock.Type != Tile.Types.Exit)
            {
                var block = person.Position as BuildingBlock;
                if (block != null && block.Priority == int.MaxValue)
                {
                    throw new PersonException(); //<<---- Personen kan ikke finde ud?? I så fald skal der ikke kastes en exception, men i stedet skal personen håndteres (som i tages ud af simuleringen, der skal gives besked til analyse-delen, at personen ikke er medtaget i simuleringen osv osv osv)
                }
                BuildingBlock dest = FindNextPathTarget(person);
                pathList.AddRange(GetPathFromSourceToDestinationAStar(person, dest));
                if (dest.Type == Tile.Types.Stair)
                {
                    dest = dest.BNeighbours.First(n => n.Z < dest.Z);
                    person.Position = dest;
                    person.CurrentRoom = dest.Room;
                    dest = FindNextPathTarget(person);
                    pathList.AddRange(GetPathFromSourceToDestinationAStar(person, dest));
                }
                if (dest.Type == Tile.Types.Exit)
                {
                    break;
                }
                if (dest.Priority % 2 == 0 && dest.Type == Tile.Types.Door)
                {
                    person.Position =
                        dest.BNeighbours.Where(
                            n =>
                                n.Type == Tile.Types.Free &&
                                n.Room != person.CurrentRoom && n.Room != 0)
                            .OrderBy(n => n.DistanceTo(dest))
                            .First();
                    BuildingBlock buildingBlock = (BuildingBlock) person.Position;
                    if (buildingBlock != null)
                        person.CurrentRoom = (buildingBlock.Room);
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
                    point.LengthFromSource = int.MaxValue;
                point.IsChecked = false;
            }

            unvisitedVertices.Remove(person.Position as BuildingBlock);
            BuildingBlock buildingBlock = person.Position as BuildingBlock;
            if (buildingBlock != null) buildingBlock.LengthFromSource = 0;
            unvisitedVertices.Insert(0, (person.Position as BuildingBlock));
            BuildingBlock current = unvisitedVertices[0];

            while (!Equals(current, destination))
            {
                if (current != null)
                {
                    foreach (BuildingBlock point in current.BNeighbours)
                    {
                        if (point.IsChecked == false)
                        {
                            if (!closedSet.ContainsKey(ImportExportSettings.Coordinate(point.X, point.Y, point.Z)))
                            {
                                priorityQueue.Add(point);
                            }
                        }
                    }
                    CheckNeighbors(current, priorityQueue);
                    if (priorityQueue.Count == 0)
                    {
                        if (closedSet.ContainsKey(ImportExportSettings.Coordinate(current.X, current.Y, current.Z)) == false)
                            closedSet.Add(ImportExportSettings.Coordinate(current.X, current.Y, current.Z), current);

                        current = person.Position as BuildingBlock;
                        foreach (BuildingBlock point in unvisitedVertices)
                        {
                            point.IsChecked = false;
                        }
                        continue;
                    }
                    current.IsChecked = true;
                }
                current = priorityQueue.First();
                priorityQueue.Clear();
            }

            List<BuildingBlock> path = new List<BuildingBlock>();
            BuildingBlock parent = destination;
            do
            {
                path.Add(parent);
                parent = parent?.Parent;
            } while (parent != null);
            path.Reverse();
            return path;
        }
        private static void CheckNeighbors(BuildingBlock currentPoint, SortedSet<BuildingBlock> priorityQueue)
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