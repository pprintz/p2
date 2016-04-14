#region

using System.Linq;

#endregion

namespace Evacuation_Master_3000
{
    static class Settings
    {
        public static bool ShowHeatMap { get; set; } = false;

        public static int PersonCount
        {
            get { return MainWindow.PList.Count(); }
        }
    }
}