using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evacuation_Master_3000
{
    public class UserInterface : IUserInterface
    {
        public UserInterface() {
            TheMainWindow = new TheRealMainWindow(this);
        }

        private TheRealMainWindow TheMainWindow { get; }
        public event SimulationStart OnSimulationStart;
        public event ImportFloorPlan OnImportFloorPlan;
        public event ExportFloorPlan OnExportFloorPlan;
        public event NewFloorPlan OnNewFloorPlan;
        public IFloorPlan LocalFloorPlan { get; private set; }
        private Dictionary<int, Person> people { get; set; } = new Dictionary<int, Person>();

        public void Display() {
            TheMainWindow.Show();
        }

        public void DisplayGeneralErrorMessage(string errorMessage) {
            throw new NotImplementedException();
        }

        public void DisplayStatistics(DataSimulationStatistics dataSimulationStatistics) {
            throw new NotImplementedException();
        }

        public void CreateFloorplan(int width, int height, int floorAmount, string description) {
            LocalFloorPlan = OnNewFloorPlan?.Invoke(width, height, floorAmount, description);
            TheMainWindow.floorPlanVisualiserControl.ImplementFloorPlan(LocalFloorPlan, people);
        }

        public void ImportFloorPlan(string filePath) {
            LocalFloorPlan = OnImportFloorPlan?.Invoke(filePath);
            
        }

        public void SimulationStart(bool showHeatMap, bool stepByStep) {
            people = OnSimulationStart?.Invoke(LocalFloorPlan, showHeatMap, stepByStep);
        }

        private void VisualizeFloorPlan() {
            //TheMainWindow.floorPlanVisualiserControl.
        }
    }
}
