using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evacuation_Master_3000
{
    public delegate void PersonEvacuated(Person person);
    public delegate IEnumerable<BuildingBlock> ExtendedPathRequest(Person person);
    public delegate void PersonMoved(Person person);
    public delegate void SimulationEnd();
    public delegate void TileTypeChange(Tile tile);
    public delegate void ResetClicked();
    public delegate Dictionary<int, Person> UISimulationStart(IFloorPlan floorPlan, bool heatMapActive, bool stepByStep, IPathfinding pathfinding, int milliseconds);
    public delegate IFloorPlan ImportFloorPlan(string fileName);
    public delegate IFloorPlan ExportFloorPlan(string filePath, IFloorPlan floorPlan, Dictionary<int, Person> allPeople);
    public delegate IFloorPlan NewFloorPlan(int width, int height, int floorAmount, string description);
    public delegate IFloorPlan RevertToPeopleStartPositions();
    public delegate void Tick();
    public delegate void ChangeVisualFloor(int currentFloor);
    public delegate void ExportFloorPlanFeedBack(string message);
    public delegate void ImportFloorPlanFeedBack();
}
