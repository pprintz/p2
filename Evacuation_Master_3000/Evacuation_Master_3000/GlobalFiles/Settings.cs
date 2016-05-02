#region

using System.Linq;
using System.Windows.Input;

#endregion

namespace Evacuation_Master_3000
{
    internal static class Settings
    {
        public static bool ShowHeatMap { get; set; } //Skal evt videregives som parameter i stedet for at være en public static bool ??

        public static Key ZoomKey { get; set; } = Key.LeftCtrl;
        public static Key HorizontalScrollKey { get; set; } = Key.LeftAlt;
        public static Key LineToolKey { get; set; } = Key.LeftShift;

        ////////////public static int PersonCount => MainWindow.PList.Count;

    }
}