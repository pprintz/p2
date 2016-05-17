using System;
using System.Windows;

namespace Evacuation_Master_3000
{

    internal class Controller
    {
        public IData Data { get; }
        public IUserInterface UI { get; }

        public Controller(IData data, IUserInterface ui)
        {
            Data = data;
            UI = ui;
            UI.OnImportFloorPlan += Data.ImportFloorPlan;
            UI.OnNewFloorPlan += Data.CreateFloorPlan;
            UI.OnExportFloorPlan += Data.ExportFloorPlan;
            UI.OnUISimulationStart += Data.StartSimulation;
            UI.OnPrepareSimulation += Data.PrepareSimulation;
        }

        public void Start() {
            try {
                UI.Display();
            } catch (Exception e) {
                MessageBox.Show(e.Message);
            }
        }

        private void PrepareAndStartSimulation()
        {
            //_data.TheFloorPlan.PrepareForSimulation; neighbours + priorities + find people
            //_data.StartSimulation(Astar, _ui.TickSetting);
        }

    }
}