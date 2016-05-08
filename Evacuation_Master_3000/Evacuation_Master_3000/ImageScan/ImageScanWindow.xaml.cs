using System.Windows;
using System.Windows.Controls;
using Evacuation_Master_3000.ImageScan;
using Evacuation_Master_3000.UI.ControlPanelUI;

namespace Evacuation_Master_3000
{
    /// <summary>
    /// Interaction logic for ImageScanWindow.xaml
    /// </summary>
    public partial class ImageScanWindow : Window
    {
        public ImageScanWindow(string filePath)
        {
            InitializeComponent();

            CpImageScanControls = new CP_ImageScanControls(this, filePath);
            ImageContainer.Children.Add(CpImageScanControls);
            Grid.SetColumn(CpImageScanControls,1);
            CpImageScanPicture = new CP_ImageScanPicture(this, filePath);
            ImageContainer.Children.Add(CpImageScanPicture);
            Grid.SetColumn(CpImageScanPicture, 0);

            Show();
        }

        public CP_ImageScanControls CpImageScanControls { get; }
        public CP_ImageScanPicture CpImageScanPicture { get; }
    }
}
