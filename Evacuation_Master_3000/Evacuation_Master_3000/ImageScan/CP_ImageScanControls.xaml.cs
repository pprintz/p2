using System;
using System.Windows;
using System.Windows.Controls;
using Evacuation_Master_3000.ImageScan;

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
            FilePathTextbox.Text = System.IO.Path.GetFileNameWithoutExtension(filepath) + Settings.Extension;
            ParentWindow = parentWindow;
            //ContrastSlider.MouseLeftButtonUp += OnChangeInVisualsRequested;
        }

        private ImageScanWindow ParentWindow { get; }

        private void OnChangeInVisualsRequested(object sender, EventArgs e)
        {
            ParentWindow.CpImageScanPicture.CreateOrUpdateVisualRepresentation();
        }


        private void OnSaveButtonClicked(object sender, RoutedEventArgs e)
        {
            string filePath = ParentWindow.CpImageScanControls.FilePathTextbox.Text;
            ParentWindow.CpImageScanPicture.SaveAsGridFile(filePath);

            OnImageSuccesfullyScanned?.Invoke(filePath);

            ParentWindow.Close();
        }

        public delegate void ImageSuccesfullyScanned(string filePath);

        public event ImageSuccesfullyScanned OnImageSuccesfullyScanned;


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
