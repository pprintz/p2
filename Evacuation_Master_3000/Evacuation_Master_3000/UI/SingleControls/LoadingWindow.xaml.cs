using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfAnimatedGif;

namespace Evacuation_Master_3000.UI.SingleControls
{
    public partial class LoadingWindow : Window
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
            //this.Dispatcher.Thread.Abort();
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

        private ImageAnimationController _controller;
        private void UpdateLoadingOnPriorityDone(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(() => { LoadingHeaderText.Text = "Calculating paths for persons.."; });

        }
        public void StartAnimation()
        {
            this.Dispatcher.Invoke(() =>
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(@"C:\Users\peter\Desktop\p2\Evacuation_Master_3000\Evacuation_Master_3000\UI\SingleControls\loading.gif");
                image.EndInit();
                ImageBehavior.SetAnimatedSource(LoadingGif, image);
                _controller = ImageBehavior.GetAnimationController(LoadingGif);
                Show();
                LoadingHeaderText.Text = "Resetting floor..";
            });
        }
    }
}