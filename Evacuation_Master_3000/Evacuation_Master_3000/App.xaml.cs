using System.Windows;

namespace Evacuation_Master_3000 {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private void StartUp(object sender, StartupEventArgs e) {
            IData data = new Data();
            IUserInterface ui = new UserInterface();
            Controller controller = new Controller(data, ui);
            controller.UI.Display();
        }
    }
}
