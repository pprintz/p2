using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using static Evacuation_Master_3000.ImportExportSettings;

namespace Evacuation_Master_3000 {
    /// <summary>
    /// Interaction logic for NewImportWindow.xaml
    /// </summary>
    public partial class NewImportWindow : INotifyPropertyChanged {
        public NewImportWindow(TheRealMainWindow parentWindow, NewOrImport window = NewOrImport.New) {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            ParentWindow = parentWindow;

            /* Initial setup for the window */
            NewImportTabControl.SelectedIndex = (int)window;
            BuildingWidth = DefaultWidth;
            BuildingHeight = DefaultHeight;
            BuildingFloorAmount = DefaultFloorAmount;
            Description = DefaultDescription;

            BuildingWidthTextBox.PreviewTextInput += NumberValidationTextBox;
            BuildingHeightTextBox.PreviewTextInput += NumberValidationTextBox;
            BuildingFloorAmountTextBox.PreviewTextInput += NumberValidationTextBox;

            BrowseButton.Click += BrowseFiles;
            BrowsePngFiles.Click += BrowseImages;
            CreateNewButtun.Click += CreateNewBuilding;

            Closing += OnWindowClosing;
        }

        private TheRealMainWindow ParentWindow { get; }
        public enum NewOrImport { New, Import }
        private int BuildingWidthAndHeightMax = 500;
        private int _buildingWidth;
        public int BuildingWidth
        {
            get { return _buildingWidth; }
            set
            {
                _buildingWidth = value;
                if (_buildingWidth > BuildingWidthAndHeightMax)
                {
                    _buildingWidth = BuildingWidthAndHeightMax;
                }
                OnPropertyChanged("BuildingWidth");
            }
        }
        private int _buildingHeight;
        public int BuildingHeight
        {
            get { return _buildingHeight; }
            set
            {
                _buildingHeight = value;
                if (_buildingHeight > BuildingWidthAndHeightMax)
                {
                    _buildingHeight = BuildingWidthAndHeightMax;
                }
                OnPropertyChanged("BuildingHeight");
            }
        }
        private int BuildingFloorMax = 40;
        private int _buildingFloorAmount;
        public int BuildingFloorAmount
        {
            get { return _buildingFloorAmount; }
            set
            {
                _buildingFloorAmount = value;
                if (_buildingFloorAmount > BuildingFloorMax)
                {
                    _buildingFloorAmount = BuildingFloorMax;
                }
                OnPropertyChanged("BuildingFloorAmount");
            }
        }
        public string Description { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BrowseFiles(object sender, RoutedEventArgs e) {
            OpenFileDialog open = new OpenFileDialog { Filter = "Grid files|*" + Extension };
            if (open.ShowDialog() == true) {
                ParentWindow.TheUserInterface.ImportFloorPlan(open.FileName);
                OnSucces();
            }
        }

        private void BrowseImages(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            if (open.ShowDialog() == true)
            {
                new ImageScanWindow(open.FileName);
                
                ParentWindow.TheUserInterface.ImportFloorPlan(open.FileName);
                OnSucces();
            }
        }

        

        private void CreateNewBuilding(object sender, RoutedEventArgs e) {
            ParentWindow.TheUserInterface.CreateFloorplan(BuildingWidth, BuildingHeight, BuildingFloorAmount, Description);
            OnSucces();
        }

        public void OnShowWindow(NewOrImport window) {
            NewImportTabControl.SelectedIndex = (int)window;
            Show();
        }

        private void OnSucces() {
            Hide();
        }

        /* Cancels the Close-event when the window is closed. 
        Instead of closing (and disposing) of the window we instead make it invisible.
        This makes for faster loads of the window for future uses */
        private void OnWindowClosing(object sender, CancelEventArgs e) {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }
    }
}
