using System.Collections.Generic;

namespace Evacuation_Master_3000
{
    internal class AStar : IPathfinding
    {
        public AStar(IFloorPlan floorPlan)
        {
            TheFloorPlan = floorPlan;
        }
        public IFloorPlan TheFloorPlan { get; }

        public IEnumerable<Tile> CalculatePath(IEvacuateable person)
        {
            // Event simulate button clicked -> find neighbours -> find priorities and then the magic begins
            
            // Peters magic
            return null;
        }
        
    }
}