﻿using System.Windows;
using System.Windows.Controls;

namespace Evacuation_Master_3000
{
    /// <summary>
    /// Interaction logic for CP_ImportExport.xaml
    /// </summary>
    public partial class CP_ImportExport
    {
        public CP_ImportExport(MainWindow parentWindow)
        {
            InitializeComponent();
            ParentWindow = parentWindow;

            ExportButton.Click += OnExport;
            ImportButton.Click += OnImport;
            NewButton.Click += OnNew;
            HeaderComboBox.SelectionChanged += OnSelectionChanged;
            HeaderTextBox.TextChanged += OnHeaderTextChanged;
            ParentWindow.TheUserInterface.OnBuildingPlanSuccessfullLoaded += OnSuccessfullBuildingLoadUp;
            HeaderComboBox.SelectedIndex = 0;
            FileInformationGroup.Visibility = Visibility.Collapsed;
            DimensionsGroup.Visibility = Visibility.Collapsed;
            FileNamePanel.Visibility = Visibility.Collapsed;
        }

        private MainWindow ParentWindow { get; }
        private IFloorPlan TheFloorPlan { get; set; }

        private void OnHeaderTextChanged(object sender, TextChangedEventArgs e) {
            TheFloorPlan.Headers[HeaderComboBox.SelectedIndex] = (sender as TextBox).Text;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            HeaderTextBox.Text = TheFloorPlan.Headers[(sender as ComboBox).SelectedIndex];
        }

        private void OnNew(object sender, RoutedEventArgs e) {
            ParentWindow.ImportWindow.OnShowWindow(NewImportWindow.NewOrImport.New);
        }

        private void OnImport(object sender, RoutedEventArgs e) {
            ParentWindow.ImportWindow.OnShowWindow(NewImportWindow.NewOrImport.Import);
        }

        private void OnExport(object sender, RoutedEventArgs e) {
            ParentWindow.ExportWindow.OnShowWindow();
        }

        private void OnSuccessfullBuildingLoadUp() {
            TheFloorPlan = ParentWindow.TheUserInterface.LocalFloorPlan;

            DescriptionTextBox.Text = string.IsNullOrEmpty(TheFloorPlan.Description) ? string.Empty : TheFloorPlan.Description;

            for(int currentFloor = 0; currentFloor < TheFloorPlan.FloorAmount; currentFloor++) {
                ComboBoxItem comboBox = new ComboBoxItem {Content = $"Floor {currentFloor}"};
                HeaderComboBox.Items.Add(comboBox);
            }

            WidthText.Text = TheFloorPlan.Width/2 + "m";
            HeightText.Text = TheFloorPlan.Height/2 + "m";
            FloorsText.Text = TheFloorPlan.FloorAmount.ToString();

            FileInformationGroup.Visibility = Visibility.Visible;
            DimensionsGroup.Visibility = Visibility.Visible;
        }

    }
}
