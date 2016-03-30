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
using System.Text.RegularExpressions;
using System.ComponentModel;
using static GridTakeThree.ImportExportSettings;

namespace GridTakeThree {
    /// <summary>
    /// Interaction logic for GridNewOrLoadWindow.xaml
    /// </summary>
    public partial class GridNewOrLoadWindow : Window, INotifyPropertyChanged {
        public GridNewOrLoadWindow(NewOrImport window) {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();

            /* Initial setup for the window */
            tabControl.SelectedIndex = (int)window;
            ImportBrowse = ImportOrBrowse.Browse;
            ImportHeader = DefaultImportHeader;
            GridWidth = DefaultWidth;
            GridHeight = DefaultHeight;
            NewHeader = DefaultNewHeader;
            Description = DefaultDescription;

            HeightInput.PreviewTextInput += NumberValidationTextBox;
            WidthInput.PreviewTextInput += NumberValidationTextBox;
        }


        public enum NewOrImport { New, Import }
        public enum ImportOrBrowse { Browse, Import }
        private ImportOrBrowse _importBrowse;
        public ImportOrBrowse ImportBrowse {
            get { return _importBrowse; }
            set {
                _importBrowse = value;
                switch (ImportBrowse) {
                    case ImportOrBrowse.Browse:
                        ImportGroup.Visibility = Visibility.Collapsed;
                        ImportHeader = DefaultImportHeader;
                        BrowseGroup.Visibility = Visibility.Visible;
                        break;
                    case ImportOrBrowse.Import:
                        BrowseGroup.Visibility = Visibility.Collapsed;
                        ImportGroup.Visibility = Visibility.Visible;
                        break;
                    default:
                        break;
                }
            }
        }
        private int _gridWidth;
        private int _gridHeight;
        public int GridWidth
        {
            get { return _gridWidth; }
            set
            {
                _gridWidth = value;
                OnPropertyChanged("GridWidth");
            }
        }
        public int GridHeight
        {
            get { return _gridHeight; }
            set
            {
                _gridHeight = value;
                OnPropertyChanged("GridHeight");
            }
        }
        private string _importHeader;
        public string ImportHeader
        {
            get { return _importHeader; }
            set {
                _importHeader = value;
                OnPropertyChanged("ImportHeader");
            }
        }
        private string _newHeader;
        public string NewHeader
        {
            get { return _newHeader; }
            set {
                _newHeader = value;
                OnPropertyChanged("NewHeader");
            }
        }
        private string _description;
        public string Description
        {
            get { return _description; }
            set {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e) {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

    }
}
