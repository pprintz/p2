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
        private void button_Click(object sender, RoutedEventArgs e) {
            grid = new Grid(canvas, 800, 400);
            grid.CreateGrid();
        }
        public static bool makeWall;
        public static bool makeDoor;
        public static bool makePath;
        public static bool makeFree;

        private void StartPath(object sender, RoutedEventArgs e) {
            grid.CalculateAllNeighbours();
            List<Point> allPoints = new List<Point>();
            foreach (Point item in grid.AllPoints.Values) {
                allPoints.Add(item);
            }
            Graph graph = new Graph(allPoints);
            graph.dijkstra(Point.Path[0], Point.Path[1]);
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

    }
}
