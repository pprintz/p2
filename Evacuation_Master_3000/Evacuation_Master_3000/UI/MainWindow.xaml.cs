using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Evacuation_Master_3000 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        public MainWindow(UserInterface userInterface) {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            TheUserInterface = userInterface;
            ImportWindow = new NewImportWindow(this);
            ExportWindow = new ExportWindow(this);
            FloorPlanVisualiserControl = new FloorPlanVisualiser(this);
            ControlPanelControl = new ControlPanel(this);
            SimulationControlsControl = new SimulationControls(this);
            ZoomControl = new Zoom(this);
            Closed += OnCloseWindow;
            SetupWindow();
        }

        private void OnCloseWindow(object sender, EventArgs e) {
            Environment.Exit(0);
        }

        public UserInterface TheUserInterface { get; }
        public NewImportWindow ImportWindow { get; }
        public ExportWindow ExportWindow { get; }
        public FloorPlanVisualiser FloorPlanVisualiserControl { get; }
        public ControlPanel ControlPanelControl { get; }
        private SimulationControls SimulationControlsControl { get; }
        private Zoom ZoomControl { get; }

        private void SetupWindow() {
            MainWindowGrid.Children.Add(FloorPlanVisualiserControl);
            Grid.SetColumn(FloorPlanVisualiserControl, 0);
            Grid.SetRow(FloorPlanVisualiserControl, 0);

            MainWindowGrid.Children.Add(ControlPanelControl);
            Grid.SetColumn(ControlPanelControl, 1);
            Grid.SetRow(ControlPanelControl, 0);

            MainWindowGrid.Children.Add(SimulationControlsControl);
            Grid.SetColumn(SimulationControlsControl, 1);
            Grid.SetRow(SimulationControlsControl, 1);

            MainWindowGrid.Children.Add(ZoomControl);
            Grid.SetColumn(ZoomControl, 0);
            Grid.SetRow(ZoomControl, 1);
        }

        public void ShowWindow() {
            Show();
            ImportWindow.OnShowWindow(NewImportWindow.NewOrImport.New);
        }

        private void TheRealMainWindow_OnKeyUp(object sender, KeyEventArgs e) {
            if (e.SystemKey == Settings.LineToolKey) {
                FloorPlanVisualiserControl.LineToolReleased();
            }
        }
    }
}
