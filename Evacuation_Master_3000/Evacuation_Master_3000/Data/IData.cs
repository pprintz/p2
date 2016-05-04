using System;
using System.Collections.Generic;

namespace Evacuation_Master_3000
{
    internal interface IData
    {
        IFloorPlan TheFloorPlan { get; }
        IEnumerable<Person> GetPeople(Predicate<Person> predicate);
        Dictionary<int, Person> StartSimulation(IFloorPlan floorPlan, bool heatmap, bool stepByStep, IPathfinding pathfindingAlgorithm, int tickLength);
        void ResetData();
        event PersonMoved OnSendPersonMoved;
        DataSimulationStatistics GetSimulationStatistics();
        IFloorPlan ImportFloorPlan(string fileName);
        IFloorPlan CreateFloorPlan(int width, int height, int floorAmount, string description);
        // Maybe CreateOrImportFloorPlan doesn't need the standard values since the delegate has them.
    }
}