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

            SetupUserControlTabs(parentWindow);
        }

        private void SetupUserControlTabs(TheRealMainWindow parentWindow)
        {
            TabItem floorPlanControls = new TabItem
            {
                Header = "Floorplan controls",
                Content = new CP_FloorPlanControls(parentWindow)
            };
            //UserControlTabPanel.Items.Add(floorPlanControls);

            TabItem simulationControls = new TabItem()
            {
                Header = "Simulation controls",
                Content = new CP_SimulationControls()
            };
            //UserControlTabPanel.Items.Add(simulationControls);

            TabItem simulationStats = new TabItem() {
                Header = "Simulation informaion",
                Content = new CP_SimulationStats()
            };
            //UserControlTabPanel.Items.Add(simulationStats);

            TabItem importExport = new TabItem() {
                Header = "Import/Export",
                Content = new CP_ImportExport(parentWindow)
            };
            //UserControlTabPanel.Items.Add(importExport);

            UserControlTabPanel.Items.Insert(0, floorPlanControls);
            UserControlTabPanel.Items.Insert(1, simulationControls);
            UserControlTabPanel.Items.Insert(2, simulationStats);
            UserControlTabPanel.Items.Insert(3, importExport);
        }
    }
}
