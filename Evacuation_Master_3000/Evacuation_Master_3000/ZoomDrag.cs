#region

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

#endregion

namespace Evacuation_Master_3000
{
    class ZoomDrag
    {
        private Point? _lastCenterPositionOnTarget;
        private Point? _lastDragPoint;
        private Point? _lastMousePositionOnTarget;

        public Slider Slider { get; set; }
        public ScrollViewer ScrollViewer { get; set; }
        //public ScaleTransform scaleTransform { get; set; }
        public Canvas Container { get; set; }
        private double ZoomValue { get; set; }
        public bool CanZoom { get; set; }
        private bool MouseInGrid { get; set; }

        public void MouseEnter(object sender, MouseEventArgs e)
        {
            MouseInGrid = true;
        }

        public void MouseLeave(object sender, MouseEventArgs e)
        {
            MouseInGrid = false;
        }

        public void ZoomMouseWheel(object sender, MouseWheelEventArgs e)
        {
            _lastMousePositionOnTarget = Mouse.GetPosition(Container);
            e.Handled = CanZoom ? true : false;
            if (CanZoom && MouseInGrid)
            {
                if (e.Delta > 0)
                {
                    if (Slider.Value < Slider.Maximum)
                    {
                        ZoomValue += 0.1;
                        Slider.Value += 0.1;
                    }
                }
                else
                {
                    if (Slider.Value > Slider.Minimum)
                    {
                        ZoomValue -= 0.1;
                        Slider.Value -= 0.1;
                    }
                }
            }
        }

        public void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //scaleTransform.ScaleX = e.NewValue;
            //scaleTransform.ScaleY = e.NewValue;

            var centerOfViewport = new Point(ScrollViewer.ViewportWidth/2,
                ScrollViewer.ViewportHeight/2);
            _lastCenterPositionOnTarget = ScrollViewer.TranslatePoint(centerOfViewport, Container);
        }

        public void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange != 0 || e.ExtentWidthChange != 0)
            {
                Point? targetBefore = null;
                Point? targetNow = null;

                if (!_lastMousePositionOnTarget.HasValue)
                {
                    if (_lastCenterPositionOnTarget.HasValue)
                    {
                        Point centerOfViewport = new Point(ScrollViewer.ViewportWidth/2,
                            ScrollViewer.ViewportHeight/2);
                        Point centerOfTargetNow =
                            ScrollViewer.TranslatePoint(centerOfViewport, Container);

                        targetBefore = _lastCenterPositionOnTarget;
                        targetNow = centerOfTargetNow;
                    }
                }
                else
                {
                    targetBefore = _lastMousePositionOnTarget;
                    targetNow = Mouse.GetPosition(Container);

                    _lastMousePositionOnTarget = null;
                }

                if (targetBefore.HasValue)
                {
                    double dXInTargetPixels = targetNow.Value.X - targetBefore.Value.X;
                    double dYInTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;

                    double multiplicatorX = e.ExtentWidth/Container.Width;
                    double multiplicatorY = e.ExtentHeight/Container.Height;

                    double newOffsetX = ScrollViewer.HorizontalOffset -
                                        dXInTargetPixels*multiplicatorX;
                    double newOffsetY = ScrollViewer.VerticalOffset -
                                        dYInTargetPixels*multiplicatorY;

                    if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY))
                    {
                        return;
                    }

                    ScrollViewer.ScrollToHorizontalOffset(newOffsetX);
                    ScrollViewer.ScrollToVerticalOffset(newOffsetY);
                }
            }
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_lastDragPoint.HasValue)
            {
                Point posNow = e.GetPosition(ScrollViewer);

                double dX = posNow.X - _lastDragPoint.Value.X;
                double dY = posNow.Y - _lastDragPoint.Value.Y;

                _lastDragPoint = posNow;

                ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset - dX);
                ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset - dY);
            }
        }

        public void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(ScrollViewer);
            if (mousePos.X <= ScrollViewer.ViewportWidth && mousePos.Y <
                ScrollViewer.ViewportHeight) //make sure we still can use the scrollbars
            {
                ScrollViewer.Cursor = Cursors.SizeAll;
                _lastDragPoint = mousePos;
                Mouse.Capture(ScrollViewer);
            }
        }


        public void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ScrollViewer.Cursor = Cursors.Arrow;
            ScrollViewer.ReleaseMouseCapture();
            _lastDragPoint = null;
        }
    }
}