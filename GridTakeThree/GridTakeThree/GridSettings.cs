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
    static class GridSettings {
        //public static int GridSize { get; private set; } // Ved ikke helt, hvordan det skal anvendes?
        public static int GridSpacing { get; private set; } = 10;
        public static int PointSize { get; private set; } = 10;

        public static double MaxPointDistance {
            get { return Math.Sqrt(Math.Pow(GridSpacing - 0, 2) + Math.Pow(GridSpacing - 0, 2)); }
        }

        public static double WindowWidth { get; set; }
        public static double WindowHeight { get; set; }

        public static int PointsPerRow { get { return (int)((WindowWidth - 200) / GridSpacing); } }
        public static int PointsInHeight { get { return (int)(WindowHeight / GridSpacing); } }
    }
}
