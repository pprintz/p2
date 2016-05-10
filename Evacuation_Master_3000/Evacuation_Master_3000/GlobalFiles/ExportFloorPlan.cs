namespace Evacuation_Master_3000
{
    public delegate IFloorPlan ExportFloorPlan(IFloorPlan floorPlan, string[] headers, string description, string fileName);
}