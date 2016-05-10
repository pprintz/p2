using System.Collections.Generic;

namespace Evacuation_Master_3000
{
    public delegate Dictionary<int, Person> UISimulationStart(IFloorPlan floorPlan, bool heatMapActive, bool stepByStep, IPathfinding pathfinding, int milliseconds);
}