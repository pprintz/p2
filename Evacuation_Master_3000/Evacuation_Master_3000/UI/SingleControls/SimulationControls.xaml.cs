using System;
using System.ComponentModel;
using System.Windows;
using Evacuation_Master_3000.UI.SingleControls;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;

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

        private bool FirstRun = true;
        private LoadingWindow loadingWindow;
        private void OnSimulationStartClick(object sender, RoutedEventArgs e)
        {
            if (UserInterface.HasSimulationEnded && UserInterface.IsSimulationReady)
            {
                Thread thread = new Thread(() =>
                {
                    if (loadingWindow == null)
                    {
                        loadingWindow = new LoadingWindow();
                    }
                    loadingWindow.StartAnimation();
                    System.Windows.Threading.Dispatcher.Run();
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                FirstRun = false;
                UserInterface.HasSimulationEnded = false;
                _parentWindow.ControlPanelControl.UserControlTabPanel.SelectedIndex = 2;
                _parentWindow.TheUserInterface.SimulationStart(
                    _parentWindow.ControlPanelControl.SimulationControls.HeatmapToggle.IsChecked != null &&
                    (bool)_parentWindow.ControlPanelControl.SimulationControls.HeatmapToggle.IsChecked,
                    _parentWindow.ControlPanelControl.SimulationControls.StepByStepToggle.IsChecked != null &&
                    (bool)_parentWindow.ControlPanelControl.SimulationControls.StepByStepToggle.IsChecked,
                    new AStar(_parentWindow.TheUserInterface.LocalFloorPlan), 100);
            }
        }
    }
}

