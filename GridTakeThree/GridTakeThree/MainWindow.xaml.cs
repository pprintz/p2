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

namespace GridTakeThree {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        /*private void InitializeProgram() {
            grid.WindowHeight = Height;
            grid.WindowWidth = Width;
        }*/

        private Grid grid;
        private void CreateGrid() {
            grid = new Grid(canvas, 800, 400);
            grid.CreateGrid();
        }
        public static bool makeWall;
        public static bool makeDoor;
        public static bool makePath;
        public static bool makeFree;
        private bool lineTool = false;

        private void StartPath(object sender, RoutedEventArgs e) {
            grid.CalculateAllNeighbours();
            List<Point> allPoints = new List<Point>();
            foreach (Point item in grid.AllPoints.Values) {
                allPoints.Add(item);
            }
            int currentStartPointIndex = 0;
            int currentEndPointIndex = 1;
            Graph graph = new Graph(allPoints);
            while (currentEndPointIndex < Point.Path.Count)
            {
                graph.dijkstra(Point.Path[currentStartPointIndex], Point.Path[currentEndPointIndex]);
                currentStartPointIndex++;
                currentEndPointIndex++;
            }
           
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
                case Key.Space:
                    CreateGrid();
                    break;
                case Key.LeftShift:
                    lineTool = true;
                    break;
            }
        }


        private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.LeftShift:
                    lineTool = false;
                    break;
            }
        }
    }
}
