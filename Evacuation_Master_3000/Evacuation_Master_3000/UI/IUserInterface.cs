namespace Evacuation_Master_3000
{
    public interface IUserInterface
    {
        // Sends params dict with 
        void Display();
        void DisplayGeneralMessage(string errorMessage);
        void DisplayStatistics(DataSimulationStatistics dataSimulationStatistics);
        event PrepareSimulation OnPrepareSimulation;
        event UISimulationStart OnUISimulationStart;
        event ImportFloorPlan OnImportFloorPlan;
        event ExportFloorPlan OnExportFloorPlan;
        event NewFloorPlan OnNewFloorPlan;
        IFloorPlan LocalFloorPlan { get; }
    }
}