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

namespace Evacuation_Master_3000 {
    /// <summary>
    /// Interaction logic for ExportWindow.xaml
    /// </summary>
    public partial class ExportWindow : Window, INotifyPropertyChanged {
        /// <summary>
        /// Instantiates a new Export Window to handle the export of a grid.
        /// </summary>
        public ExportWindow() {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            Extension = ImportExportSettings.Extension;
            FileName = ImportExportSettings.FileName;
            Path = ImportExportSettings.GridDirectoryPath;

            InitializeComponent();
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
        public string FileName { get; set; }
        private string _description;
        private string _header;
        public string Description {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }
        public string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                OnPropertyChanged("Header");
            }
        }
        private int _gridWidth;
        private int _gridHeight;
        public int GridWidth {
            get { return _gridWidth; }
            set {
                _gridWidth = value;
                OnPropertyChanged("GridWidth");
            }
        }
        public int GridHeight {
            get { return _gridHeight; }
            set {
                _gridHeight = value;
                OnPropertyChanged("GridHeight");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        /* Overvej brug af WindowsAPICodePack til en lidt pænere folderbrowser! */
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
    }
}
