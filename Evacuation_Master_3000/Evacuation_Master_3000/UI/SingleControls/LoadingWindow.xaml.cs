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
using System.Windows.Shapes;
using WpfAnimatedGif;

namespace Evacuation_Master_3000.UI.SingleControls
{
    /// <summary>
    /// Interaction logic for LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        public LoadingWindow()
        {
            InitializeComponent();
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(@"C:\Users\peter\Desktop\p2\Evacuation_Master_3000\Evacuation_Master_3000\UI\SingleControls\loading.gif");
            image.EndInit();
            ImageBehavior.SetAnimatedSource(LoadingGif, image);
            _controller = ImageBehavior.GetAnimationController(LoadingGif);
        }
        private ImageAnimationController _controller ;

        public void StartAnimation()
        {
            ImageBehavior.SetAutoStart(LoadingGif, true);
            Show();
        }
    }
}
