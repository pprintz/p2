using System.Collections.Generic;

namespace Evacuation_Master_3000
{
    internal interface IData
    {
        IFloorPlan TheFloorPlan { get; }
        Dictionary<int, Person> PrepareSimulation(IFloorPlan floorPlan);
        Dictionary<int, Person> StartSimulation(bool heatmap, IPathfinding pathfindingAlgorithm, int simulationSpeed);
        IFloorPlan ImportFloorPlan(string fileName);
        IFloorPlan CreateFloorPlan(int width, int height, int floorAmount, string description);
        IFloorPlan ExportFloorPlan(string filePath, IFloorPlan floorPlan, Dictionary<int, Person> allPeople);
    }
}