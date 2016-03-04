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
            InitializeProgram();
        }

        private void InitializeProgram() {
            GridSettings.WindowHeight = Height;
            GridSettings.WindowWidth = Width;
        }
        Grid grid;
        private void button_Click(object sender, RoutedEventArgs e) {
            grid = new Grid(canvas);
            grid.CreateGrid();
            MessageBox.Show((GridSettings.PointsInHeight * GridSettings.PointsPerRow).ToString());
        }
        public static bool makeWall = false;
        public static bool makeDoor = false;
        public static bool makePath = false;
        public static bool makeFree = true;

        private void MakeWall(object sender, RoutedEventArgs e) {
            makeWall = !makeWall;
        }

        private void MakeDoor(object sender, RoutedEventArgs e) {
            makeDoor = !makeDoor;
        }

        private void MakePath(object sender, RoutedEventArgs e) {
            makePath = !makePath;
        }

        private void MakeFree(object sender, RoutedEventArgs e) {
            makeWall = false;
            makeDoor = false;
            makePath = false;
            makeFree = true;
        }

        private void ForceCalculateNeighbours(object sender, RoutedEventArgs e) {
            grid.CalculateAllNeighbours();
        }

        private void StartPath(object sender, RoutedEventArgs e) {
            List<Point> allPoints = new List<Point>();
            foreach (Point item in grid.AllPoints.Values) {
                allPoints.Add(item);
            }
            Graph graph = new Graph(allPoints);
            graph.dijkstra(Point.Path[0], Point.Path[1]);
        }
    }
}
