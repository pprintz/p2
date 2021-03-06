﻿using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace Evacuation_Master_3000 {
    /// <summary>
    /// Interaction logic for ExportWindow.xaml
    /// </summary>
    public partial class ExportWindow : Window, INotifyPropertyChanged {
        public ExportWindow(MainWindow parentWindow) {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            ParentWindow = parentWindow;
            Extension = Settings.Extension;
            FileName = string.Empty;
            Path = Settings.GridDirectoryPath;
            WindowBaseHeight = Height;
            BrowseButton.Click += BrowseDirectory;
            ExportButton.Click += ExportBuilding;

            Export.OnExportFeedBack += OnExportFeedBack;
            Closing += OnWindowClosing;
        }

        private void ExportBuilding(object sender, RoutedEventArgs e) {
            ParentWindow.TheUserInterface.ExportFloorPlan(Path + "\\" + FileName + Extension);
        }

        private MainWindow ParentWindow { get; }
        private double WindowBaseHeight { get; }
        private string _fileName;
        public string FileName {
            get { return _fileName; }
            set {
                _fileName = value;
                ExportButton.IsEnabled = !string.IsNullOrWhiteSpace(_fileName);
                OnPropertyChanged("Path");
            }
        }
        public string Extension { get; }
        private string _path;
        public string Path {
            get { return _path; }
            set {
                _path = value;
                OnPropertyChanged("Path");
            }
        }
        private string _description;
        public string Description {
            get { return _description; }
            set {
                _description = value;
                OnPropertyChanged("Description");
            }
        }
        private string[] Headers { get; set; }
        private int FloorAmount { get; set; }
        private DockPanel[] HeadersPanel { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void BrowseDirectory(object sender, RoutedEventArgs e) {
            /* Setup the folder browser */
            var dialog = new System.Windows.Forms.FolderBrowserDialog
            {
                ShowNewFolderButton = true,
                SelectedPath = Path
            };

            /* Open/Show the folder browser dialog - Capture the result (what button was pressed) */
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            /* If the user pressed OK, change the path */
            if (result == System.Windows.Forms.DialogResult.OK) 
                Path = dialog.SelectedPath;
        }

        private void OnExportFeedBack(string message, Export.ExportOutcomes outcome) {
            Hide();
            ParentWindow.TheUserInterface.DisplayGeneralMessage(message, $"Export {outcome.ToString()}");
        }

        /* Cancels the Close-event when the window is closed. 
        Instead of closing (and disposing) of the window we instead make it invisible.
        This makes for faster loads of the window for future uses */
        private void OnWindowClosing(object sender, CancelEventArgs e) {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }

        public void OnShowWindow() {
            DockPanel templateHeader = HeaderTemplate;
            double heightOfTemplate = templateHeader.ActualHeight;
            Headers = ParentWindow.TheUserInterface.LocalFloorPlan.Headers;
            FloorAmount = ParentWindow.TheUserInterface.LocalFloorPlan.FloorAmount;
            HeadersPanel = new DockPanel[FloorAmount];

            for(int floor = 0; floor < FloorAmount; floor++) {
                TextBlock label = new TextBlock() {
                    Name = $"Header{floor}Label",
                    Text = $"Header {floor}: "
                };

                TextBox textBox = new TextBox() {
                    Name = $"Header{floor}TextBox",
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Width = 200
                };

                templateHeader.Name = $"Header{floor}Panel";

                templateHeader.Children.Add(label);
                templateHeader.Children.Add(textBox);

                HeadersPanel[floor] = templateHeader;
            }

            Height = WindowBaseHeight + FloorAmount * heightOfTemplate; //Change the Window Height to fit the header panels

            Show();
        }
    }
}
