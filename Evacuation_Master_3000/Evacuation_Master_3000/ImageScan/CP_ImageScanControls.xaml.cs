using System;
using System.Collections.Generic;
using System.IO;
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
using Evacuation_Master_3000.ImageScan;

namespace Evacuation_Master_3000.UI.ControlPanelUI
{
    /// <summary>
    /// Interaction logic for CP_ImageScanControls.xaml
    /// </summary>
    public partial class CP_ImageScanControls : UserControl
    {
        public CP_ImageScanControls(ImageScanWindow parentWindow, string filepath)
        {
            InitializeComponent();
            FilePathTextbox.Text = System.IO.Path.GetFileNameWithoutExtension(filepath) + ImportExportSettings.Extension;
            ParentWindow = parentWindow;
            ContrastSlider.ValueChanged += OnSliderValueChanged;

        }

        private ImageScanWindow ParentWindow { get; }

        private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ParentWindow.CpImageScanPicture.ContrastChanger(sender, e);
        }

        private void DoneButtom_OnClick(object sender, RoutedEventArgs e)
        {
           // ParentWindow.CpImageScanPicture.CreateGridFile();
        }
    }
}
