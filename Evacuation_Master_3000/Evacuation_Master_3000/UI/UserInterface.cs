using System;
using System.Collections.Generic;

namespace Evacuation_Master_3000
{
    public class UserInterface : IUserInterface
    {
        public UserInterface() {
            TheMainWindow = new MainWindow(this);
        }

        public static bool IsSimulationPaused = false;
        private static bool _hasSimulationEnded;
        public static bool HasSimulationEnded
        {
            get { return _hasSimulationEnded; }
            set
            {
                if (value)
                {
                    OnSimulationEnd?.Invoke();
                }
                _hasSimulationEnded = value;
            }
        }

        public static bool ResetButtonClicked
        {
            set
            {
                if (value)
                {
                    OnReset?.Invoke();
                }
            }
        }

        public static event ResetClicked OnReset;
        public static event SimulationEnd OnSimulationEnd;
        private MainWindow TheMainWindow { get; }
        public event PrepareSimulation OnPrepareSimulation;
        public event UISimulationStart OnUISimulationStart;
        public event ImportFloorPlan OnImportFloorPlan;
        public event ExportFloorPlan OnExportFloorPlan;
        public event NewFloorPlan OnNewFloorPlan;
        public event BuildingPlanSuccessfullLoaded OnBuildingPlanSuccessfullLoaded;
        public IFloorPlan LocalFloorPlan { get; private set; }
        private Dictionary<int, Person> People { get; set; } = new Dictionary<int, Person>();
        public IReadOnlyDictionary<int, Person> LocalPeopleDictionary => People;
        private bool _floorplanHasBeenCreated;

        public bool HeatMapActivated { get; private set; }

        public void Display() {
            TheMainWindow.ShowWindow();
        }

        public void DisplayGeneralErrorMessage(string errorMessage) {
            throw new NotImplementedException();
        }

        public void DisplayStatistics(DataSimulationStatistics dataSimulationStatistics) {
            throw new NotImplementedException();
        }

        

        public void CreateFloorplan(int width, int height, int floorAmount, string description)
        {
            if (!_floorplanHasBeenCreated)
            {
                LocalFloorPlan = OnNewFloorPlan?.Invoke(width, height, floorAmount, description);
                TheMainWindow.floorPlanVisualiserControl.ImplementFloorPlan(LocalFloorPlan, People);
                _floorplanHasBeenCreated = true;
                OnBuildingPlanSuccessfullLoaded?.Invoke();
            }
            else
            {

                System.Windows.MessageBox.Show("A building has already been made.");
            }
        }

        public void ImportFloorPlan(string filePath) {
            LocalFloorPlan = OnImportFloorPlan?.Invoke(filePath);
            People = OnPrepareSimulation?.Invoke(LocalFloorPlan);
            TheMainWindow.floorPlanVisualiserControl.ImplementFloorPlan(LocalFloorPlan, People);
            OnBuildingPlanSuccessfullLoaded?.Invoke();
        }

        public void ExportFloorPlan(string filePath) {

            People = OnPrepareSimulation?.Invoke(LocalFloorPlan);
            LocalFloorPlan = OnExportFloorPlan?.Invoke(filePath, LocalFloorPlan, People);
        }

        public void SimulationStart(bool showHeatMap, bool stepByStep, IPathfinding pathfinding, int milliseconds)
        {
            HeatMapActivated = showHeatMap;
            People = OnPrepareSimulation?.Invoke(LocalFloorPlan);
            /*People = */OnUISimulationStart?.Invoke(showHeatMap, stepByStep, pathfinding, milliseconds);
            
        }

        private void VisualizeFloorPlan() {
            //TheMainWindow.floorPlanVisualiserControl.
        }
    }
}
