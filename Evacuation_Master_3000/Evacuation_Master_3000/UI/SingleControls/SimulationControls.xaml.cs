using System.Windows;
using Evacuation_Master_3000.UI.SingleControls;
using System.Threading;

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
        private LoadingWindow _loadingWindow;

        private void OnResetButtonClick(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                if (_loadingWindow == null)
                {
                    _loadingWindow = new LoadingWindow();
                }
                _loadingWindow.StartAnimation();
                System.Windows.Threading.Dispatcher.Run();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            UserInterface.ResetButtonClicked = true;
            ChangeSimulationControlsOnEnd();
            UserInterface.IsSimulationPaused = false;
            PauseSimulationButton.Content = "Pause";
            UserInterface.HasSimulationEnded = true;
            OnStopLoadingWindow?.Invoke(this, null);
        }

        public static event FunctionDone OnStopLoadingWindow;
        private void OnPauseAndContinueButtonClick(object sender, RoutedEventArgs e)
        {
            if (!UserInterface.IsSimulationPaused)
            {
                _parentWindow.ControlPanelControl.SimulationStats.SimulationSpeed.IsEnabled = true;
                UserInterface.IsSimulationPaused = true;
                PauseSimulationButton.Content = "Continue";
            }
            else
            {
                _parentWindow.ControlPanelControl.SimulationStats.SimulationSpeed.IsEnabled = false;
                PauseSimulationButton.Content = "Pause";
                UserInterface.IsSimulationReady = true;
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
            if (UserInterface.HasSimulationEnded && UserInterface.IsSimulationReady)
            {
                Thread thread = new Thread(() =>
                {
                    if (_loadingWindow == null)
                    {
                        _loadingWindow = new LoadingWindow();
                    }
                    _loadingWindow.StartAnimation();
                    System.Windows.Threading.Dispatcher.Run();
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                UserInterface.HasSimulationEnded = false;
                _parentWindow.ControlPanelControl.UserControlTabPanel.SelectedIndex = 1;
                _parentWindow.TheUserInterface.SimulationStart(
                    _parentWindow.ControlPanelControl.SimulationStats.HeatmapToggle.IsChecked != null &&
                    (bool)_parentWindow.ControlPanelControl.SimulationStats.HeatmapToggle.IsChecked,
                    new AStar(_parentWindow.TheUserInterface.LocalFloorPlan), (int)_parentWindow.ControlPanelControl.SimulationStats.SimulationSpeed.Value);
            }
            else
            {
                _parentWindow.ControlPanelControl.UserControlTabPanel.SelectedIndex = 1;
                _parentWindow.TheUserInterface.SimulationStart(
                    _parentWindow.ControlPanelControl.SimulationStats.HeatmapToggle.IsChecked != null &&
                    (bool)_parentWindow.ControlPanelControl.SimulationStats.HeatmapToggle.IsChecked,
                    new AStar(_parentWindow.TheUserInterface.LocalFloorPlan), (int)_parentWindow.ControlPanelControl.SimulationStats.SimulationSpeed.Value);
            }
        }
    }
}

