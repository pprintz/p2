using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.ComponentModel;

namespace GridTakeThree {
    class ZoomDrag : INotifyPropertyChanged {
        public ZoomDrag() { }
        public Slider slider { get; set; }
        public ScrollViewer scrollViewer { get; set; }
        public Canvas Container { get; set; }
        private double ZoomValue { get; set; }
        private bool CanZoom { get; set; }

        private System.Windows.Point? lastCenterPositionOnTarget;
        private System.Windows.Point? lastMousePositionOnTarget;
        private System.Windows.Point? lastDragPoint;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public void MouseEnter(object sender, MouseEventArgs e) {
            CanZoom = true;
        }
        public void MouseLeave(object sender, MouseEventArgs e) {
            CanZoom = false;
        }
        public void ZoomMouseWheel(object sender, MouseWheelEventArgs e) {
            if (CanZoom) {
                if (e.Delta > 0) {
                    if (slider.Value < slider.Maximum) {
                        ZoomValue += 0.1;
                        slider.Value += 0.1;
                    }
                }
                else {
                    if (slider.Value > slider.Minimum) {
                        ZoomValue -= 0.1;
                        slider.Value -= 0.1;
                    }
                }
            }
        }

        public void OnScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e) {
            if (e.ExtentHeightChange != 0 || e.ExtentWidthChange != 0) {
                System.Windows.Point? targetBefore = null;
                System.Windows.Point? targetNow = null;

                if (!lastMousePositionOnTarget.HasValue) {
                    if (lastCenterPositionOnTarget.HasValue) {
                        var centerOfViewport = new System.Windows.Point(scrollViewer.ViewportWidth / 2,
                                                         scrollViewer.ViewportHeight / 2);
                        System.Windows.Point centerOfTargetNow =
                              scrollViewer.TranslatePoint(centerOfViewport, Container);

                        targetBefore = lastCenterPositionOnTarget;
                        targetNow = centerOfTargetNow;
                    }
                }
                else {
                    targetBefore = lastMousePositionOnTarget;
                    targetNow = Mouse.GetPosition(Container);

                    lastMousePositionOnTarget = null;
                }

                if (targetBefore.HasValue) {
                    double dXInTargetPixels = targetNow.Value.X - targetBefore.Value.X;
                    double dYInTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;

                    double multiplicatorX = e.ExtentWidth / Container.Width;
                    double multiplicatorY = e.ExtentHeight / Container.Height;

                    double newOffsetX = scrollViewer.HorizontalOffset -
                                        dXInTargetPixels * multiplicatorX;
                    double newOffsetY = scrollViewer.VerticalOffset -
                                        dYInTargetPixels * multiplicatorY;

                    if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY)) {
                        return;
                    }

                    scrollViewer.ScrollToHorizontalOffset(newOffsetX);
                    scrollViewer.ScrollToVerticalOffset(newOffsetY);
                }
            }
        }

        public void OnMouseMove(object sender, MouseEventArgs e) {
            if (lastDragPoint.HasValue) {
                System.Windows.Point posNow = e.GetPosition(scrollViewer);

                double dX = posNow.X - lastDragPoint.Value.X;
                double dY = posNow.Y - lastDragPoint.Value.Y;

                lastDragPoint = posNow;

                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - dX);
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - dY);
            }
        }
        public void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e) {

            var mousePos = e.GetPosition(scrollViewer);
            if (mousePos.X <= scrollViewer.ViewportWidth && mousePos.Y <
                scrollViewer.ViewportHeight) //make sure we still can use the scrollbars
            {
                scrollViewer.Cursor = Cursors.SizeAll;
                lastDragPoint = mousePos;
                Mouse.Capture(scrollViewer);
            }
        }



        public void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e) {

            scrollViewer.Cursor = Cursors.Arrow;
            scrollViewer.ReleaseMouseCapture();
            lastDragPoint = null;
        }
    }
}
