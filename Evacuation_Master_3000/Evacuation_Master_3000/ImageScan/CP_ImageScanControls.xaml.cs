using System;
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
            ContrastSlider.MouseLeftButtonUp += OnChangeInVisualsRequested;
        }

        private ImageScanWindow ParentWindow { get; }

        private void OnChangeInVisualsRequested(object sender, EventArgs e)
        {
            ParentWindow.CpImageScanPicture.CreateOrUpdateVisualRepresentation();
        }


        private void OnSaveButtonClicked(object sender, RoutedEventArgs e)
        {
           ParentWindow.CpImageScanPicture.CreateGridFile(ParentWindow.CpImageScanControls.FilePathTextbox.Text, "", "", (int)Math.Round(ParentWindow.CpImageScanControls.ContrastSlider.Value));
        }

        private void OnSobelFilterCheckboxChange(object sender, RoutedEventArgs e)
        {
            CheckBox chbox = sender as CheckBox;
            if (chbox?.IsChecked != null)
                ParentWindow.CpImageScanPicture.SobelFilterActivated = (bool) chbox.IsChecked;
            else
            {
                throw new GeneralInternalException($"{sender} can not be connected to this function. Only checkboxes are allowed.");
            }
        }
    }
}
