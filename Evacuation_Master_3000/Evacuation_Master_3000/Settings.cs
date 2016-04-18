#region

using System.Linq;

#endregion

namespace Evacuation_Master_3000
{
    static class Settings
    {
        public static bool ShowHeatMap { get; set; }

        public static int PersonCount => MainWindow.PList.Count;

    }
}