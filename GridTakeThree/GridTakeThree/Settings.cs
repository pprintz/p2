using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridTakeThree {
    static class Settings {
        public static bool ShowHeatMap { get; set; } = false;
        public static int PersonCount { get { return MainWindow.PList.Count(); } }
    }
}
