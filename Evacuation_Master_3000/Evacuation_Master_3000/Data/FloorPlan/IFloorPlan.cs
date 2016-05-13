using System.Collections.Generic;

namespace Evacuation_Master_3000
{
    public interface IFloorPlan {
        int Width { get; }
        int Height { get; }
        int FloorAmount { get; }
        string Description { get; }
        Dictionary<string, Tile> Tiles { get; }
        string[] Headers { get; set; }
        void CreateFloorPlan(int width, int height, int floorAmount, string description, string[] headers);
        void CreateFloorPlan(int width, int height, Dictionary<string, Tile> tiles);
        void Initiate();
    }
}