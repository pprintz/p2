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
    public delegate Dictionary<int, Person> SimulationStart(IFloorPlan floorPlan, bool heatMapActive, bool stepByStep);
    public delegate IFloorPlan ImportFloorPlan(string fileName);
    public delegate IFloorPlan ExportFloorPlan(IFloorPlan floorPlan, string[] headers, string description, string fileName);
    public delegate IFloorPlan NewFloorPlan(int width, int height, int floorAmount, string description);
    public delegate IFloorPlan RevertToPeopleStartPositions();
    public delegate void Tick();
    public delegate void ChangeVisualFloor(int currentFloor);
}
