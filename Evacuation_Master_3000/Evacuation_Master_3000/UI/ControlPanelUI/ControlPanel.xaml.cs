using System.Windows.Controls;

namespace Evacuation_Master_3000
{
    /// <summary>
    /// Interaction logic for ControlPanel.xaml
    /// </summary>
    public partial class ControlPanel
    {
        public ControlPanel(MainWindow parentWindow)
        {
            InitializeComponent();
            ParentWindow = parentWindow;
            FloorPlanControls = new CP_FloorPlanControls(ParentWindow);
            SimulationStats = new CP_SimulationStats(ParentWindow);
            ImportExport = new CP_ImportExport(ParentWindow);
            Import.OnImportFeedBack += OnImportedFloorPlan;
            SetupUserControlTabs();
        }

        private void OnImportedFloorPlan() {
            UserControlTabPanel.SelectedIndex = 0;
        }

        private MainWindow ParentWindow { get;  }

        private CP_FloorPlanControls FloorPlanControls { get; }
        public CP_SimulationStats SimulationStats { get; }
        private CP_ImportExport ImportExport { get;}

        private void SetupUserControlTabs()
        {
            TabItem floorPlanControls = new TabItem
            {
                Header = "Floorplan controls",
                Content = FloorPlanControls
            };

            TabItem simulationStats = new TabItem()
            {
                Header = "Simulation informaion",
                Content = SimulationStats
            };

            TabItem importExport = new TabItem()
            {
                Header = "Import/Export",
                Content = ImportExport
            };

            UserControlTabPanel.Items.Insert(0, floorPlanControls);
            UserControlTabPanel.Items.Insert(1, simulationStats);
            UserControlTabPanel.Items.Insert(2, importExport);

            UserControlTabPanel.SelectedIndex = 0;
        }
    }
}
