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
using System.Windows.Shapes;
using System.ComponentModel;
using static Evacuation_Master_3000.ImportExportSettings;

namespace Evacuation_Master_3000 {
    /// <summary>
    /// Interaction logic for ExportWindow.xaml
    /// </summary>
    public partial class ExportWindow : Window, INotifyPropertyChanged {
        public ExportWindow(TheRealMainWindow parentWindow) {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            ParentWindow = parentWindow;
            Extension = ImportExportSettings.Extension;
            FileName = ImportExportSettings.FileName;
            Path = ImportExportSettings.GridDirectoryPath;
            WindowBaseHeight = Height;
            BrowseButton.Click += BrowseDirectory;
            //ExportButton.Click += 
        }

        private TheRealMainWindow ParentWindow { get; }
        private double WindowBaseHeight { get; }
        public string FileName { get; set; }
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
        private void OnPropertyChanged(string property) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        private void BrowseDirectory(object sender, RoutedEventArgs e) {
            /* Setup the folder browser */
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowNewFolderButton = true;
            dialog.SelectedPath = Path;

            /* Open/Show the folder browser dialog - Capture the result (what button was pressed) */
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            /* If the user pressed OK, change the path */
            if (result == System.Windows.Forms.DialogResult.OK)
                Path = dialog.SelectedPath.ToString();
        }

        public void OnShowWindow() {
            DockPanel templateHeader = HeaderTemplate;
            double heightOfTemplate = templateHeader.ActualHeight; //ELLER 25 FLAT OUT!!! BECAUSE OF REASONS!! DON'T YOU DARE ASK ABOUT IT! IT IS HOW IT IS!!!!1!!!one!
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
