using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Evacuation_Master_3000.Settings;

namespace Evacuation_Master_3000 {
    /// <summary>
    /// Interaction logic for Zoom.xaml
    /// </summary>
    public partial class Zoom : UserControl {
        public Zoom(MainWindow parentWindow) {
            InitializeComponent();

            ParentWindow = parentWindow;
            FloorPlanVisualRepresentation = ParentWindow.floorPlanVisualiserControl;
            ParentWindow.TheUserInterface.OnBuildingPlanSuccessfullLoaded += ZoomToFit;

            SetupZoom();
        }
        private MainWindow ParentWindow { get; }
        private FloorPlanVisualiser FloorPlanVisualRepresentation { get; }
        private bool IsMouseHoveringVisual { get; set; }
        private bool _isAbleToZoom;
        private bool IsAbleToZoom {
            get { return _isAbleToZoom && IsMouseHoveringVisual; }
            set {
                _isAbleToZoom = value;
            }
        }
        private bool HorizontalScrollEnabled { get; set; }

        private void SetupZoom() {
            FloorPlanVisualRepresentation.MouseEnter += MouseInVisual;
            FloorPlanVisualRepresentation.MouseLeave += MouseNotInVisual;
            FloorPlanVisualRepresentation.PreviewMouseWheel += OnMouseWheelActivity;
            ParentWindow.PreviewKeyDown += OnKeyHandler;
            ParentWindow.PreviewKeyUp += OnKeyHandler;
            ZoomSlider.ValueChanged += OnSliderValueChanged;
            ZoomSliderText.MouseLeftButtonDown += ResetZoom;
            ZoomSliderText.MouseRightButtonDown += ZoomToFit;
            //OnSliderValueChanged(null, null);
        }

        private void MouseInVisual(object sender, MouseEventArgs e) => IsMouseHoveringVisual = true;
        private void MouseNotInVisual(object sender, MouseEventArgs e) => IsMouseHoveringVisual = false;
        private void ResetZoom(object sender, MouseButtonEventArgs e) => ZoomSlider.Value = 1;
        private void ZoomToFit(object sender, MouseButtonEventArgs e) => ZoomToFit();

        private void OnMouseWheelActivity(object sender, MouseWheelEventArgs e) {
            e.Handled = IsAbleToZoom || HorizontalScrollEnabled;            //Handle the MouseWheelEventArgs if any custom made criteria is met
            //Should we zoom?
            if (IsAbleToZoom) {
                if (e.Delta > 0)
                    ZoomSlider.Value += 0.1;
                else
                    ZoomSlider.Value -= 0.1;
            //Or should we scroll horizontally?
            } else if (HorizontalScrollEnabled) {
                if(e.Delta > 0)
                    FloorPlanVisualRepresentation.VisualContainerScrollViewer.LineLeft();
                else
                    FloorPlanVisualRepresentation.VisualContainerScrollViewer.LineRight();
            }
        }

        private void OnKeyHandler(object sender, KeyEventArgs e) {
            bool value = e.IsDown;
            /* Key-values are found in the Settings.cs-file (public static properties) */
            if (e.Key == ZoomKey) 
                IsAbleToZoom = value;
            else if (e.Key == HorizontalScrollKey)
                HorizontalScrollEnabled = value; 
        }

        private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            //var centerOfViewport = new Point(FloorPlanVisualRepresentation.VisualContainerScrollViewer.ViewportWidth / 2,
            //    FloorPlanVisualRepresentation.VisualContainerScrollViewer.ViewportHeight / 2);
            //FloorPlanVisualRepresentation.VisualContainerScrollViewer.TranslatePoint(centerOfViewport, FloorPlanVisualRepresentation.VisualContainer);
            FloorPlanVisualRepresentation.VisualContainerScaleTransform.ScaleX = ZoomSlider.Value;
            FloorPlanVisualRepresentation.VisualContainerScaleTransform.ScaleY = ZoomSlider.Value;
            //_lastCenterPositionOnTarget = ScrollViewer.TranslatePoint(centerOfViewport, Container);
            ZoomSliderText.Text = (Math.Round(ZoomSlider.Value, 2, MidpointRounding.AwayFromZero)*100).ToString() + "%";
        }

        public void ZoomToFit() {

            int buildingWidth = ParentWindow.TheUserInterface.LocalFloorPlan.Width * FloorPlanVisualRepresentation.TileSize;
            int buildingHeight = ParentWindow.TheUserInterface.LocalFloorPlan.Height * FloorPlanVisualRepresentation.TileSize;
            int scrollBarThickness = 50;

            double parentWindowVisualiserWidth = FloorPlanVisualRepresentation.ActualWidth - scrollBarThickness;
            double parentWindowVisualiserHeight = FloorPlanVisualRepresentation.ActualHeight - scrollBarThickness;

            double widthToFit = parentWindowVisualiserWidth / buildingWidth;
            double heightToFit = parentWindowVisualiserHeight / buildingHeight;

            ZoomSlider.Value = widthToFit < heightToFit ? widthToFit : heightToFit;
        }
    }
}
