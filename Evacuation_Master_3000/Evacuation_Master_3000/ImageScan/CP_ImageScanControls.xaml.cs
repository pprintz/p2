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
            ContrastSlider.MouseLeftButtonUp += OnMouseLeftButtonUp;
            ContrastSlider.ValueChanged += ContrastSlider_OnValueChanged;
        }

        private ImageScanWindow ParentWindow { get; }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ParentWindow.CpImageScanPicture.threshold = ContrastSlider.Value;
            foreach (var rect in ParentWindow.CpImageScanPicture.BuildingBlockContainer.Children)
            {
                if (rect is Rectangle)
                {
                    Rectangle rectangle = rect as Rectangle;
                    int[] coords = rectangle.Tag as int[];
                    bool recolour = ParentWindow.CpImageScanPicture._pixels[coords[1], coords[0]] >= ParentWindow.CpImageScanPicture.threshold;
                    rectangle.Fill = recolour == true
                        ? new SolidColorBrush(Colors.Blue)
                        : new SolidColorBrush(Colors.Green);
                };
            }
        }




        private void DoneButtom_OnClick(object sender, RoutedEventArgs e)
        {
           // ParentWindow.CpImageScanPicture.CreateGridFile();
        }

        private void ContrastSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //ParentWindow.CpImageScanPicture.threshold = ContrastSlider.Value;
            //foreach (var rect in ParentWindow.CpImageScanPicture.BuildingBlockContainer.Children)
            //{
            //    if (rect is Rectangle)
            //    {
            //        Rectangle rectangle = rect as Rectangle;
            //        int[] coords = rectangle.Tag as int[];
            //        bool recolour = ParentWindow.CpImageScanPicture._pixels[coords[1], coords[0]] >=
            //                        ParentWindow.CpImageScanPicture.threshold;
            //        rectangle.Fill = recolour == true
            //            ? new SolidColorBrush(Colors.Blue)
            //            : new SolidColorBrush(Colors.Green);
            //    }
            //    ;
            //}
        }


        private void CalculateContrast_OnClick(object sender, RoutedEventArgs e)
        {
            ParentWindow.CpImageScanPicture.threshold = ContrastSlider.Value;
            foreach (var rect in ParentWindow.CpImageScanPicture.BuildingBlockContainer.Children)
            {
                if (rect is Rectangle)
                {
                    Rectangle rectangle = rect as Rectangle;
                    int[] coords = rectangle.Tag as int[];
                    bool recolour = ParentWindow.CpImageScanPicture._pixels[coords[1], coords[0]] >=
                                    ParentWindow.CpImageScanPicture.threshold;
                    rectangle.Fill = recolour == true
                        ? new SolidColorBrush(Colors.White)
                        : new SolidColorBrush(Colors.Black);
                }
                ;
            }
        }

        private void MakeFree_OnClick(object sender, RoutedEventArgs e)
        {
            CheckBox chbox = sender as CheckBox;
            ParentWindow.CpImageScanPicture.applySobelFilter = (bool) chbox.IsChecked;
        }
    }
}
