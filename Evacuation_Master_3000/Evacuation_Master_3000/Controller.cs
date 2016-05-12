namespace Evacuation_Master_3000
{

    internal class Controller
    {
        public IData _data;
        public IUserInterface _ui;

        public Controller(IData data, IUserInterface ui)
        {
            _data = data;
            _ui = ui;
            _ui.OnImportFloorPlan += _data.ImportFloorPlan;
            _ui.OnNewFloorPlan += _data.CreateFloorPlan;
            _ui.OnExportFloorPlan += _data.ExportFloorPlan;
            _ui.OnUISimulationStart += data.StartSimulation;
            // _ui.OnSimulationStart += _data.StartSimulation;
        }

        private void PrepareAndStartSimulation()
        {
            //_data.TheFloorPlan.PrepareForSimulation; neighbours + priorities + find people
            //_data.StartSimulation(Astar, _ui.TickSetting);
        }

    }
}