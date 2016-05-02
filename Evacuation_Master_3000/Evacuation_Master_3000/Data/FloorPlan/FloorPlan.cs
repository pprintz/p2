using System.Collections.Generic;
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
        public string[] Headers { get; set; }
        private bool floorPlanAlreadyExist;

        public void CreateFloorPlan(int width, int height, int floorAmount, string description, string[] headers) {
            if (floorPlanAlreadyExist)
                return;

            Width = width;
            Height = height;
            FloorAmount = floorAmount;
            Description = description;
            Headers = headers;

            for (int z = 0; z < FloorAmount; z++) {
                for (int y = 0; y < Height; y++) {
                    for (int x = 0; x < Width; x++) {
                        Tiles.Add(Coordinate(x, y, z), new BuildingBlock(x, y, z));
                    }
                }
            }

            floorPlanAlreadyExist = true;
        }

        public void CalculateNeighbours() {

        }

        private void CheckForConnectionsThroughDiagonalUnwalkableElements() {
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
    }
}