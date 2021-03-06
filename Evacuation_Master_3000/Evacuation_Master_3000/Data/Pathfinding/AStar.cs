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
                    throw new PersonException(); // Person can't evacuate
                }
                BuildingBlock destination = FindNextPathTarget(person);
                pathList.AddRange(GetPathUsingAStar(person, destination));
                if (destination.Type == Tile.Types.Stair)
                {
                    //Walks to the neighbour stairs where the priority is lower(this prevents moving up though).. Look next else if
                    if (destination.BuildingBlockNeighbours.Count(n => n.Priority < destination.Priority) != 0)
                    {
                        destination = destination.BuildingBlockNeighbours.OrderBy(n => n.Priority).First(n => n.Priority < destination.Priority);
                    }
                    else if (destination.BuildingBlockNeighbours.Count(n => n.Z > destination.Z) != 0)
                    {
                        destination = destination.BuildingBlockNeighbours.First(n => n.Z > destination.Z);
                    }
                    person.Position = destination;
                    person.CurrentRoom = destination.Room;
                    destination = FindNextPathTarget(person);
                    pathList.AddRange(GetPathUsingAStar(person, destination));
                }
                if (destination.Type == Tile.Types.Exit)
                {
                    break;
                }
                //If its a door, then it takes the next free tile in the opposite room of the person position.
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

        private IEnumerable<Tile> GetPathUsingAStar(IEvacuateable person, BuildingBlock destination)
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
                foreach (BuildingBlock buildingBlock in currentPosition.BuildingBlockNeighbours)
                {
                    if (buildingBlock.IsChecked == false)
                    {
                        if (!closedSet.ContainsKey(Settings.Coordinate(buildingBlock.X, buildingBlock.Y, buildingBlock.Z)))
                        {
                            priorityQueue.Add(buildingBlock);
                        }
                    }
                }
                CheckNeighbors(currentPosition, priorityQueue);
                if (priorityQueue.Count == 0)
                {
                    if (closedSet.ContainsKey(Settings.Coordinate(currentPosition.X, currentPosition.Y, currentPosition.Z)) == false)
                        closedSet.Add(Settings.Coordinate(currentPosition.X, currentPosition.Y, currentPosition.Z), currentPosition);

                    foreach (BuildingBlock buildingBlock in unvisitedVertices)
                    {
                        buildingBlock.IsChecked = false;
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
        //Checks the total weight of each of the neighbours, and sets the parent. This is needed to sort the SortedSet
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