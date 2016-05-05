﻿using System;
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
    public partial class SimulationControls : UserControl
    {
        public SimulationControls(TheRealMainWindow parentWindow)
        {
            InitializeComponent();
            _parentWindow = parentWindow;
            StartSimulationButton.Click += OnSimulationStartClick;
        }

        private void OnSimulationStartClick(object sender, RoutedEventArgs e)
        {
            _parentWindow.TheUserInterface.SimulationStart(
                (bool)_parentWindow.controlPanelControl.SimulationControls.HeatmapToggle.IsChecked,
                (bool)_parentWindow.controlPanelControl.SimulationControls.StepByStepToggle.IsChecked, new AStar(_parentWindow.TheUserInterface.LocalFloorPlan), 100);
        }
        TheRealMainWindow _parentWindow;
    }
}
