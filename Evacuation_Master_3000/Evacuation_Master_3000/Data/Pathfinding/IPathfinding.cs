using System.Collections.Generic;

namespace Evacuation_Master_3000
{
    public interface IPathfinding
    {
        IFloorPlan TheFloorPlan { get; }
        IEnumerable<Tile> CalculatePath(IEvacuateable person);

    }
}