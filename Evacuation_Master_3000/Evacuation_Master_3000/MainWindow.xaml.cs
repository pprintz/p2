using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using static Evacuation_Master_3000.Settings;

namespace Evacuation_Master_3000
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        private readonly Grid _grid;

        private static MainWindow _mainWindow;
        public static bool makeWall;
        public static bool makeDoor;
        public static bool makePath;
        public static bool makeFree;
        public static bool makePerson;
        public static bool lineTool;
        public static BuildingBlock _previousPoint;
        private ZoomDrag _zoomDrag;

        public MainWindow() {

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            _grid = new Grid();
            
            // GridContainer is a canvas, which is part of the XML
            // It makes changes to the _grid and GridContainer -- maybe they should be ref or out? ??      <----- KIG HER DENNIS 
            NewOrImport newImp = new NewOrImport(GridContainer, _grid, GridNewOrLoadWindow.NewOrImport.New);

            // Used for zooming and changes made in the window size etc.
            ScrollViewerComponent.UpdateLayout();
            SetupZoomDrag();

            _mainWindow = this;

            
        }

        
        private void SetupZoomDrag() {
            _zoomDrag = new ZoomDrag() {
                Slider = SliderComponent,
                ScrollViewer = ScrollViewerComponent,
                Container = GridContainer
            };
            SuperSliderSolutionGridAndFriends.MouseEnter += _zoomDrag.MouseEnter;
            SuperSliderSolutionGridAndFriends.MouseLeave += _zoomDrag.MouseLeave;
            SuperSliderSolutionGridAndFriends.MouseWheel += _zoomDrag.ZoomMouseWheel;
            SliderComponent.ValueChanged += _zoomDrag.OnSliderValueChanged;
            ScrollViewerComponent.ScrollChanged += _zoomDrag.OnScrollViewerScrollChanged;
            ScrollViewerComponent.MouseRightButtonDown += _zoomDrag.OnMouseRightButtonDown;
            ScrollViewerComponent.MouseRightButtonUp += _zoomDrag.OnMouseRightButtonUp;
            ScrollViewerComponent.MouseMove += _zoomDrag.OnMouseMove;
        }

        private void GetOptions(object sender, RoutedEventArgs e) {
            //MessageBox.Show($"With/height:\n\t-Grid size: {gridsss.ActualWidth}/{gridsss.ActualHeight}\n\t-Canvas size: {GridContainer.ActualWidth}/{GridContainer.ActualHeight}");
            MessageBox.Show($"With/height:\n\t-Canvas size: {GridContainer.ActualWidth}/{GridContainer.ActualHeight}");
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e) {
            Export exp = new Export(_grid);
        }
        private void LoadButtonClick(object sender, RoutedEventArgs e) {
            NewOrImport imp = new NewOrImport(GridContainer, _grid, GridNewOrLoadWindow.NewOrImport.Import);
         
        }


        public static List<Person> PList = new List<Person>();
        private void StartPath(object sender, RoutedEventArgs e)
        {
            _grid.CalculateAllNeighbours();
            List<BuildingBlock> allPoints = _grid.AllPoints.Values.ToList();
            Graph graph = new Graph(allPoints);
            foreach (Person person in PList) {
                CalculatePath(person, graph);
            }
            Simulate();
            //int currentStartPointIndex = 0;
            //int currentEndPointIndex = 1;
            //List<BuildingBlock> pointsInPath = new List<BuildingBlock>();
            //while (currentEndPointIndex < BuildingBlock.Path.Count) {
            //    pointsInPath.AddRange(graph.AStar(BuildingBlock.Path[currentStartPointIndex], BuildingBlock.Path[currentEndPointIndex]));
            //    currentStartPointIndex++;
            //    currentEndPointIndex++;
            //}
            //ColorizePath(pointsInPath);

        }

        public void Simulate()
        {
            int max = PList.Max(p => p.AmountOfMoves);
            for (int index = 0; index < max; index++) {
                foreach (Person person in PList) {
                    person.Move();
                }
                Yield(1000000);
            }
        }
        private void CalculatePath(Person person, Graph graf)
        {
            /* Exception needed for when not path is given, and when a person can't find a route. */
            int sourceIndex = 0;
            int destIndex = 1;
            person.PathList.AddRange(graf.AStar(person.Position, BuildingBlock.Path[sourceIndex]));
            while (destIndex < BuildingBlock.Path.Count) {
                person.PathList.AddRange(graf.AStar(BuildingBlock.Path[sourceIndex], BuildingBlock.Path[destIndex]));
                sourceIndex++;
                destIndex++;
            }
            person.AmountOfMoves = person.PathList.Count;
        }
        private void Yield(long ticks) {
            long dtEnd = DateTime.Now.AddTicks(ticks).Ticks;
            while (DateTime.Now.Ticks < dtEnd) {
                Dispatcher.Invoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate (object unused) { return null; }, null);
            }
        }

        public static void InputLineTool(BuildingBlock point)
        {
            if (_previousPoint != null)
            {
                _mainWindow.DrawLine(_previousPoint, point);
            }
            _previousPoint = point;
        }



        public void DrawLine(BuildingBlock from, BuildingBlock to)
        {
            from.OnClick(null, null);
            int deltaX = to.X - from.X;
            int deltaY = to.Y - from.Y;
            if (deltaX-deltaY == 0)
            {
                return;
            }
            double deltaTilt = Math.Min(Math.Abs((double)deltaY / (double)deltaX), Math.Abs(deltaY)) * Math.Sign((double)deltaY / (double)deltaX);
            double tilt = 0;

            int i = 0;
            do
            {
                int j = 0;
                do
                {
                    int x = i + from.X;
                    int y = (int)(tilt) + from.Y + j;
                    BuildingBlock point;
                    string s = ImportExportSettings.Coordinate(x,y);
                    _grid.AllPoints.TryGetValue(s, out point);
                    point.OnClick(null, null);
                    j += Math.Sign(deltaY);
                } while (Math.Abs(j) < Math.Abs(deltaTilt));
                tilt += deltaTilt * Math.Sign(deltaX);
                i += Math.Sign(deltaX);
            } while (Math.Abs(i) < Math.Abs(deltaX));
            
            to.OnClick(null, null);
        }

        private void MakeWallChecked(object sender, RoutedEventArgs e)
        {
            makeWall = true;
        }
        private void MakeDoorChecked(object sender, RoutedEventArgs e)
        {
            makeDoor = true;
        }
        private void MakePathChecked(object sender, RoutedEventArgs e)
        {
            makePath = true;
        }
        private void MakeFreeChecked(object sender, RoutedEventArgs e)
        {
            makeFree = true;
        }
        private void MakePersonChecked(object sender, RoutedEventArgs e)
        {
            makePerson = true;
        }
        private void MakeWallUnchecked(object sender, RoutedEventArgs e)
        {
            makeWall = false;
        }
        private void MakeDoorUnchecked(object sender, RoutedEventArgs e)
        {
            makeDoor = false;
        }
        private void MakePathUnchecked(object sender, RoutedEventArgs e)
        {
            makePath = false;
        }
        private void MakeFreeUnchecked(object sender, RoutedEventArgs e)
        {
            makeFree = false;
        }
        private void MakePersonUnchecked(object sender, RoutedEventArgs e)
        {
            makePerson = false;
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.LeftShift:
                    lineTool = true;
                    break;
                case Key.LeftCtrl:
                    _zoomDrag.CanZoom = true;
                    break;
            }
        }


        private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.LeftShift:
                    lineTool = false;
                    _previousPoint = null;
                    break;
                case Key.LeftCtrl:
                    _zoomDrag.CanZoom = false;
                    break;
            }
        }

        private void GridContainer_SizeChanged(object sender, SizeChangedEventArgs e) {
            ScrollViewerComponent.UpdateLayout();
        }

        private void HeatmapToggle(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox?.IsChecked != null) ShowHeatMap = checkBox.IsChecked.Value;
        }
    }
}
