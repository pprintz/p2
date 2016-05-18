using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System.Threading;

namespace Evacuation_Master_3000 {
    public static class GlobalExceptionHandler {
        internal static void LogException(object sender, ThreadExceptionEventArgs e) {
            MessageBox.Show($"MAJOR EXCEPTION BRAAAAAAAAAAAAH!! \n\nLOOK AT THIS SHIT:\n{e.Exception.Message}\nLook at it and feel bad!");
        }
    }
}
