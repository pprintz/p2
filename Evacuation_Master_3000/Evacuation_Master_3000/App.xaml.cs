using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;

using System.Windows.Threading;

namespace Evacuation_Master_3000 {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private void StartUp(object sender, StartupEventArgs e) {
            IData data = new Data();
            IUserInterface ui = new UserInterface();
            Controller controller = new Controller(data, ui);
            controller.Start();
        }

        private static void ff(object sender, DispatcherUnhandledExceptionEventArgs e) {
            MessageBox.Show(e.Exception.Message);
        }
    }
}
