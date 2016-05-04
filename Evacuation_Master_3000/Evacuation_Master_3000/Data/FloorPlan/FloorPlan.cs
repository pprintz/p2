using System.Collections.Generic;
using System.Linq;
using static Evacuation_Master_3000.ImportExportSettings;
namespace Evacuation_Master_3000
{
    internal class FloorPlan : IFloorPlan
    {
        // Hash code should be calculated on everything except the Tiles heatmap, so the UI's floorplan can be equal to DATA's floorplan
        // Even though the heatmap has been actiavted. We just need to reset the heatmap on reruns.
        public FloorPlan()
        {
            Tiles = new Dictionary<string, Tile>();
            floorPlanAlreadyExist = false;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int FloorAmount { get; private set; }
        public string Description { get; private set; }
        public Dictionary<string, Tile> Tiles { get; }
        public Dictionary<string, BuildingBlock> BuildingBlocks { get; set; }
        public string[] Headers { get; set; }
        private bool floorPlanAlreadyExist;

        public void CreateFloorPlan(int width, int height, int floorAmount, string description, string[] headers)
        {
            if (floorPlanAlreadyExist)
                return;

            Width = width;
            Height = height;
            FloorAmount = floorAmount;
            Description = description;
            Headers = headers;

            for (int z = 0; z < FloorAmount; z++)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        Tiles.Add(Coordinate(x, y, z), new BuildingBlock(x, y, z));
                    }
                }
            }
            BuildingBlocks = Tiles.ToDictionary(k => k.Key, v => v.Value as BuildingBlock);
            floorPlanAlreadyExist = true;
        }

        public void Initiate()
        {
            CalculateNeighbours();
            CalculatePriorities();

        }
        private void CalculateNeighbours()
        {
            foreach (var buildingBlock in BuildingBlocks.Values.Where(t => t.Type != Tile.Types.Wall))
            {
                int topLeftNeighbourX = buildingBlock.X - 1;
                int topLeftNeighbourY = buildingBlock.Y - 1;
                int currentZ = buildingBlock.Z;
                for (int currentY = topLeftNeighbourY; currentY <= topLeftNeighbourY + 2; currentY++)
                {
                    for (int currentX = topLeftNeighbourX; currentX <= topLeftNeighbourX + 2; currentX++)
                    {
                        string coordinate = Coordinate(currentX, currentY, currentZ);
                        if (Tiles.ContainsKey(coordinate))
                        {
                            BuildingBlock currentBuildingBlock = BuildingBlocks[coordinate];
                            if (buildingBlock != currentBuildingBlock &&
                                currentBuildingBlock.Type != Tile.Types.Wall &&
                                currentBuildingBlock.Type != Tile.Types.Furniture)

                                buildingBlock.BNeighbours.Add(currentBuildingBlock);
                        }
                    }
                }
            }
        }
        private void CheckForConnectionsThroughDiagonalUnwalkableElements()
        {
            //foreach (KeyValuePair<string, BuildingBlock> pair in AllPoints) {
            //    if (pair.Value.Type == BuildingBlock.Types.Wall ||
            //        pair.Value.Type == BuildingBlock.Types.Furniture) {
            //        foreach (BuildingBlock neighbour in pair.Value.Neighbours) {
            //if (neighbour.DistanceTo(pair.Value) > 1) // Then it is a diagonal
            //{
            //    var illegalConnectedPointCoordinateSetOne = Coordinate(pair.Value.X,
            //        neighbour.Y);
            //    var illegalConnectedPointCoordinateSetTwo = Coordinate(neighbour.X,
            //        pair.Value.Y);
            //    if (!AllPoints.ContainsKey(illegalConnectedPointCoordinateSetOne) ||
            //        !AllPoints.ContainsKey(illegalConnectedPointCoordinateSetTwo)) continue;
            //    AllPoints[illegalConnectedPointCoordinateSetOne].Neighbours.Remove(
            //        AllPoints[illegalConnectedPointCoordinateSetTwo]);
            //    AllPoints[illegalConnectedPointCoordinateSetTwo].Neighbours.Remove(
            //        AllPoints[illegalConnectedPointCoordinateSetOne]);
            //}
            //        }
            //    }
            //}
        }
        private void CalculatePriorities()
        {
            IEnumerable<BuildingBlock> doorlist =
                BuildingBlocks.Values.Where(d => d.Type == BuildingBlock.Types.Door);
            IEnumerable<BuildingBlock> exitList =
                BuildingBlocks.Values.Where(p => p.Type == BuildingBlock.Types.Exit);
            foreach (BuildingBlock exitBuildingBlock in exitList)
            {
                exitBuildingBlock.Priority = 0;
                InitializeRoomPriority(exitBuildingBlock, exitBuildingBlock.Priority);
                do
                {
                    foreach (BuildingBlock door in doorlist)
                    {
                        //Checks for each door, if the doors priority is larger than the surrounding buildingblocks
                        if (door.Priority >
                            (BuildingBlocks.Values.Where(
                                b => b.BNeighbours.Contains(door) && b.Type == BuildingBlock.Types.Free || b.Type == BuildingBlock.Types.Person)
                                .Min(n => n.Priority)))
                        {
                            door.Priority =
                                (BuildingBlocks.Values.Where(b => (b.Type == BuildingBlock.Types.Free || b.Type == Tile.Types.Person) &&
                                                                   b.BNeighbours.Contains(door)).Min(n => n.Priority));
                            door.Priority++;
                            InitializeRoomPriority(door, door.Priority);
                        }
                    }
                } while (doorlist.Any(d => d.Priority - d.BNeighbours.Min(n => n.Priority) > 2));
            }

        }
        public static int GlobalRoomCounter;
        private void InitializeRoomPriority(BuildingBlock door, int priorityCounter)
        {
            GlobalRoomCounter++;
            bool done = false;
            List<BuildingBlock> tileList = BuildingBlocks.Values.Where(t => t.Type == Tile.Types.Free ||
                                                                   t.Type == Tile.Types.Person).ToList();
            priorityCounter++;
            BuildingBlock current = null;
            var currentList = door.BNeighbours.Where(n => (n.Type == BuildingBlock.Types.Free || n.Type == Tile.Types.Person) && n.Priority > priorityCounter).ToList();
            do
            {
                if (currentList.Count == 0)
                {
                    done = true;
                }
                else
                {
                    foreach (BuildingBlock block in currentList)
                    {
                        block.Priority = priorityCounter;
                        if (block.Room == 0)
                        {
                            block.Room = GlobalRoomCounter;
                        }
                    }
                    if (tileList.Any(b => b.BNeighbours.Any(n => n.Priority == priorityCounter) &&
                        b.Priority > priorityCounter &&
                        (b.Type == Tile.Types.Free || b.Type == Tile.Types.Person)))
                    {
                        foreach (BuildingBlock block in tileList.Where(b => b.BNeighbours.Any(n => n.Priority == priorityCounter) &&
                            b.Priority > priorityCounter &&
                            (b.Type == Tile.Types.Free || b.Type == Tile.Types.Person)))
                        {
                            block.Priority = priorityCounter;
                            if (block.Room == 0)
                            {
                                block.Room = GlobalRoomCounter;
                            }
                        }
                    }
                    else
                    {
                        done = true;
                    }
                }
            } while (!done);
        }
    }
}