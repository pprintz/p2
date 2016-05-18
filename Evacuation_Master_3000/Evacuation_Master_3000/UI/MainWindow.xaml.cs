﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Evacuation_Master_3000
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(UserInterface userInterface) {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            TheUserInterface = userInterface;
            importWindow = new NewImportWindow(this);
            exportWindow = new ExportWindow(this);
            floorPlanVisualiserControl = new FloorPlanVisualiser(this);
            controlPanelControl = new ControlPanel(this);
            simulationControlsControl = new SimulationControls(this);
            zoomControl = new Zoom(this);
            Closed += OnCloseWindow;
            SetupWindow();
        }

        private void OnCloseWindow(object sender, EventArgs e) {
            Environment.Exit(0);
        }

        public UserInterface TheUserInterface { get; }
        public NewImportWindow importWindow { get; }
        public ExportWindow exportWindow { get; }
        public FloorPlanVisualiser floorPlanVisualiserControl { get; }
        public ControlPanel controlPanelControl { get; }
        public SimulationControls simulationControlsControl { get; }
        public Zoom zoomControl { get; }
        
        private void SetupWindow()
        {
            MainWindowGrid.Children.Add(floorPlanVisualiserControl);
            Grid.SetColumn(floorPlanVisualiserControl, 0);
            Grid.SetRow(floorPlanVisualiserControl, 0);

            MainWindowGrid.Children.Add(controlPanelControl);
            Grid.SetColumn(controlPanelControl, 1);
            Grid.SetRow(controlPanelControl, 0);

            MainWindowGrid.Children.Add(simulationControlsControl);
            Grid.SetColumn(simulationControlsControl, 1);
            Grid.SetRow(simulationControlsControl, 1);

            MainWindowGrid.Children.Add(zoomControl);
            Grid.SetColumn(zoomControl, 0);
            Grid.SetRow(zoomControl, 1);
        }

        
        
        public void ShowWindow() {
            Show();
            importWindow.OnShowWindow(NewImportWindow.NewOrImport.New);
        }

        private void TheRealMainWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.SystemKey == Settings.LineToolKey)
            {
                floorPlanVisualiserControl.LineToolReleased();
            }
        }
    }
}