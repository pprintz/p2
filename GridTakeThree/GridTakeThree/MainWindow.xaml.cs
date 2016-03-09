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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GridTakeThree
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this;
        }

        /*private void InitializeProgram() {
            grid.WindowHeight = Height;
            grid.WindowWidth = Width;
        }*/

        private Grid grid;
        private void CreateGrid()
        {
            grid = new Grid(canvas, 800, 400);
            grid.CreateGrid();
        }

        private static MainWindow mainWindow;
        public static bool makeWall;
        public static bool makeDoor;
        public static bool makePath;
        public static bool makeFree;
        public static bool lineTool = false;
        private static Point previousPoint;

        private void StartPath(object sender, RoutedEventArgs e)
        {
            grid.CalculateAllNeighbours();
            List<Point> allPoints = new List<Point>();
            foreach (Point item in grid.AllPoints.Values)
            {
                allPoints.Add(item);
            }
            int currentStartPointIndex = 0;
            int currentEndPointIndex = 1;
            Graph graph = new Graph(allPoints);
            while (currentEndPointIndex < Point.Path.Count)
            {
                graph.AStar(Point.Path[currentStartPointIndex], Point.Path[currentEndPointIndex]);
                currentStartPointIndex++;
                currentEndPointIndex++;
            }

        }

        public static void InputLineTool(Point point)
        {
            if (previousPoint != null)
            {
                mainWindow.DrawLine(previousPoint, point);
            }
            previousPoint = point;
        }



        public void DrawLine(Point from, Point to)
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
                    Point point;
                    string s = $"({x}, {y})";
                    grid.AllPoints.TryGetValue(s, out point);
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

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.LeftShift:
                    lineTool = true;
                    break;
                default:
                    CreateGrid();
                    break;
            }
        }


        private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.LeftShift:
                    lineTool = false;
                    previousPoint = null;
                    break;
            }
        }
    }
}
