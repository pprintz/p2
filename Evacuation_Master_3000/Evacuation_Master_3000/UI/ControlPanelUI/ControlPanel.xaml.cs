using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace Evacuation_Master_3000
{
    /// <summary>
    /// Interaction logic for ControlPanel.xaml
    /// </summary>
    public partial class ControlPanel : UserControl
    {
        public ControlPanel(TheRealMainWindow parentWindow)
        {
            InitializeComponent();
            ParentWindow = parentWindow;
            FloorPlanControls = new CP_FloorPlanControls(ParentWindow);
            SimulationControls = new CP_SimulationControls();
            SimulationStats = new CP_SimulationStats();
            ImportExport = new CP_ImportExport(ParentWindow);
            Import.OnImportFeedBack += OnImportedFloorPlan;
            SetupUserControlTabs(parentWindow);
        }

        private void OnImportedFloorPlan() {
            UserControlTabPanel.SelectedIndex = 0;
        }

        public TheRealMainWindow ParentWindow { get; set; }

        public CP_FloorPlanControls FloorPlanControls { get; set; }
        public CP_SimulationControls SimulationControls { get; set; }
        public CP_SimulationStats SimulationStats { get; set; }
        public CP_ImportExport ImportExport { get; set; }

        private void SetupUserControlTabs(TheRealMainWindow parentWindow)
        {
            TabItem floorPlanControls = new TabItem
            {
                Header = "Floorplan controls",
                Content = FloorPlanControls
            };

            TabItem simulationControls = new TabItem()
            {
                Header = "Simulation controls",
                Content = SimulationControls
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
            UserControlTabPanel.Items.Insert(1, simulationControls);
            UserControlTabPanel.Items.Insert(2, simulationStats);
            UserControlTabPanel.Items.Insert(3, importExport);

            UserControlTabPanel.SelectedIndex = 3;
        }
    }
}
