using System;
using System.Collections.Generic;

namespace Evacuation_Master_3000
{
    internal interface IData
    {
        IFloorPlan TheFloorPlan { get; }
        Dictionary<int, Person> PrepareSimulation(IFloorPlan floorPlan);
        Dictionary<int, Person> StartSimulation(bool heatmap, bool stepByStep, IPathfinding pathfindingAlgorithm, int tickLength);
        event PersonMoved OnSendPersonMoved;
        DataSimulationStatistics GetSimulationStatistics();
        IFloorPlan ImportFloorPlan(string fileName);
        IFloorPlan CreateFloorPlan(int width, int height, int floorAmount, string description);
        IFloorPlan ExportFloorPlan(string filePath, IFloorPlan floorPlan, Dictionary<int, Person> allPeople);
        // Maybe CreateOrImportFloorPlan doesn't need the standard values since the delegate has them.
    }
}