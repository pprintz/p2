using System.Collections.Generic;

namespace Evacuation_Master_3000
{
    public delegate void PersonEvacuated(Person person);
    public delegate IEnumerable<BuildingBlock> ExtendedPathRequest(Person person);
    public delegate void PersonMoved(Person person);
    public delegate void SimulationEnd();
    public delegate void TileTypeChange(Tile tile);
    public delegate void ResetClicked();
    public delegate Dictionary<int, Person> PrepareSimulation(IFloorPlan floorPlan);
    public delegate Dictionary<int, Person> UISimulationStart(bool heatmap, bool stepByStep, IPathfinding pathfindingAlgorithm, int tickLength);
    public delegate IFloorPlan ImportFloorPlan(string fileName);
    public delegate IFloorPlan ExportFloorPlan(string filePath, IFloorPlan floorPlan, Dictionary<int, Person> allPeople);
    public delegate IFloorPlan NewFloorPlan(int width, int height, int floorAmount, string description);
    public delegate IFloorPlan RevertToPeopleStartPositions();
    public delegate void Tick();
    public delegate void ChangeVisualFloor(int currentFloor);
    public delegate void ExportFloorPlanFeedBack(string message);
    public delegate void ImportFloorPlanFeedBack();
}
