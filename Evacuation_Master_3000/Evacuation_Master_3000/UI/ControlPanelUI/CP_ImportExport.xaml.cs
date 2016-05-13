using System.Windows;
using System.Windows.Controls;

namespace Evacuation_Master_3000
{
    /// <summary>
    /// Interaction logic for CP_ImportExport.xaml
    /// </summary>
    public partial class CP_ImportExport : UserControl
    {
        public CP_ImportExport(TheRealMainWindow parentWindow)
        {
            InitializeComponent();
            ParentWindow = parentWindow;

            ExportButton.Click += OnExport;
            ImportButton.Click += OnImport;
            NewButton.Click += OnNew;
        }

        private TheRealMainWindow ParentWindow { get; }

        private void OnNew(object sender, RoutedEventArgs e) {
            ParentWindow.importWindow.OnShowWindow(NewImportWindow.NewOrImport.New);
        }

        private void OnImport(object sender, RoutedEventArgs e) {
            ParentWindow.importWindow.OnShowWindow(NewImportWindow.NewOrImport.Import);
        }

        private void OnExport(object sender, RoutedEventArgs e) {
            ParentWindow.exportWindow.OnShowWindow();
        }

    }
}
