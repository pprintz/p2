using System.Collections.Generic;

namespace Evacuation_Master_3000
{
    internal interface IPathfinding
    {
        IFloorPlan TheFloorPlan { get; }
        IEnumerable<Tile> CalculatePath(IEvacuateable person);

    }
}