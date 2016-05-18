using System.Windows;

namespace Evacuation_Master_3000
{
    public partial class SimulationControls
    {
        public SimulationControls(MainWindow parentWindow)
        {
            InitializeComponent();
            _parentWindow = parentWindow;
            StartSimulationButton.Click += OnSimulationStartClick;
            Data.OnTick += ChangeSimulationControls;
            PauseSimulationButton.Click += OnPauseAndContinueButtonClick;
            ResetSimulationButton.Click += OnResetButtonClick;
        }

        private readonly MainWindow _parentWindow;

        private void OnResetButtonClick(object sender, RoutedEventArgs e)
        {
            UserInterface.ResetButtonClicked = true;
            ChangeSimulationControlsOnEnd();
            UserInterface.IsSimulationPaused = false;
            PauseSimulationButton.Content = "Pause";
            UserInterface.HasSimulationEnded = true;
        }

        private void OnPauseAndContinueButtonClick(object sender, RoutedEventArgs e)
        {
            if (!UserInterface.IsSimulationPaused)
            {
                UserInterface.IsSimulationPaused = true;
                PauseSimulationButton.Content = "Continue";
            }
            else
            {
                PauseSimulationButton.Content = "Pause";
                UserInterface.IsSimulationPaused = false;
                OnSimulationStartClick(sender, e);
            }
        }
        private void ChangeSimulationControls()
        {
            StartSimulationButton.Visibility = Visibility.Hidden;
            PostSimulationControls.Visibility = Visibility.Visible;
        }
        private void ChangeSimulationControlsOnEnd()
        {
            StartSimulationButton.Visibility = Visibility.Visible;
            PostSimulationControls.Visibility = Visibility.Collapsed;
        }

        private void OnSimulationStartClick(object sender, RoutedEventArgs e)
        {
            UserInterface.HasSimulationEnded = false;
            _parentWindow.ControlPanelControl.UserControlTabPanel.SelectedIndex = 2;
            _parentWindow.TheUserInterface.SimulationStart(
                _parentWindow.ControlPanelControl.SimulationControls.HeatmapToggle.IsChecked != null && (bool)_parentWindow.ControlPanelControl.SimulationControls.HeatmapToggle.IsChecked,
                _parentWindow.ControlPanelControl.SimulationControls.StepByStepToggle.IsChecked != null && (bool)_parentWindow.ControlPanelControl.SimulationControls.StepByStepToggle.IsChecked, new AStar(_parentWindow.TheUserInterface.LocalFloorPlan), 100);
        }

    }
}

