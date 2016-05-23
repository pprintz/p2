using System;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfAnimatedGif;

namespace Evacuation_Master_3000.UI.SingleControls
{
    public partial class LoadingWindow
    {
        public LoadingWindow()
        {
            InitializeComponent();
            FloorPlan.OnFloorPlanReset += UpdateLoadingOnResetDone;
            FloorPlan.OnCalculateNeighboursDone += UpdateLoadingOnNeighboursDone;
            FloorPlan.OnCalculatePriorityDone += UpdateLoadingOnPriorityDone;
            Data.OnPathCalculationDone += StopLoading;
            ImageBehavior.SetAutoStart(LoadingGif, true);
            SimulationControls.OnStopLoadingWindow += StopLoading;
        }


        private void StopLoading(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() => { Visibility = Visibility.Hidden; });
        }
        private void UpdateLoadingOnResetDone(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() => { LoadingHeaderText.Text = "Calculating neighbours.."; });
        }
        private void UpdateLoadingOnNeighboursDone(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() => { LoadingHeaderText.Text = "Calculating priorities.."; });
        }

        private void UpdateLoadingOnPriorityDone(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() => { LoadingHeaderText.Text = "Calculating paths for persons.."; });

        }
        public void StartAnimation()
        {
            Dispatcher.Invoke(() =>
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(Environment.CurrentDirectory + @"\..\..\UI\SingleControls\loading.gif");
                image.EndInit();
                ImageBehavior.SetAnimatedSource(LoadingGif, image);
                Show();
                this.Focus();
                if (UserInterface.BuildingHasBeenChanged)
                {
                    LoadingHeaderText.Text = "Calculating path for new people";
                }
                else
                {
                    LoadingHeaderText.Text = "Resetting floor..";
                }
            });
        }
    }
}