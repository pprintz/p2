namespace Evacuation_Master_3000
{
    public interface IUserInterface
    {
        void Display();
        void DisplayGeneralMessage(string errorMessage);
        event PrepareSimulation OnPrepareSimulation;
        event UISimulationStart OnUISimulationStart;
        event ImportFloorPlan OnImportFloorPlan;
        event ExportFloorPlan OnExportFloorPlan;
        event NewFloorPlan OnNewFloorPlan;
        IFloorPlan LocalFloorPlan { get; }
    }
}