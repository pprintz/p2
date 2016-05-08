using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Evacuation_Master_3000.UI.ControlPanelUI
{
    /// <summary>
    /// Interaction logic for CP_ImageScanControls.xaml
    /// </summary>
    public partial class CP_ImageScanControls
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
            ParentWindow.CpImageScanPicture.Threshold = ContrastSlider.Value;
            foreach (var rect in ParentWindow.CpImageScanPicture.BuildingBlockContainer.Children)
            {
                if (rect is Rectangle)
                {
                    Rectangle rectangle = rect as Rectangle;
                    int[] coords = rectangle.Tag as int[];
                    bool recolour = ParentWindow.CpImageScanPicture.Pixels[coords[1], coords[0]] >= ParentWindow.CpImageScanPicture.Threshold;
                    rectangle.Fill = recolour
                        ? new SolidColorBrush(Colors.Blue)
                        : new SolidColorBrush(Colors.Green);
                };
            }
        }




        private void DoneButton_OnClick(object sender, RoutedEventArgs e)
        {
           ParentWindow.CpImageScanPicture.CreateGridFile("fifi.grid", "head1", "desc", 100);
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
            //        bool recolour = ParentWindow.CpImageScanPicture.Pixels[coords[1], coords[0]] >=
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
            ParentWindow.CpImageScanPicture.Threshold = ContrastSlider.Value;
            foreach (var rect in ParentWindow.CpImageScanPicture.BuildingBlockContainer.Children)
            {
                if (rect is Rectangle)
                {
                    Rectangle rectangle = rect as Rectangle;
                    int[] coords = rectangle.Tag as int[];
                    bool recolour = ParentWindow.CpImageScanPicture.Pixels[coords[1], coords[0]] >=
                                    ParentWindow.CpImageScanPicture.Threshold;
                    if (ParentWindow.CpImageScanPicture.SobelFilterActivated)
                        rectangle.Fill = recolour
                            ? new SolidColorBrush(Colors.Black)
                            : new SolidColorBrush(Colors.White);
                    else
                    {
                        rectangle.Fill = recolour ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Black);
                    }
                }
                
            }
        }

        private void MakeFree_OnClick(object sender, RoutedEventArgs e)
        {
            CheckBox chbox = sender as CheckBox;
            if(chbox?.IsChecked != null)
                ParentWindow.CpImageScanPicture.SobelFilterActivated = (bool) chbox.IsChecked;
            // else
                // throw some exception?
        }
    }
}
