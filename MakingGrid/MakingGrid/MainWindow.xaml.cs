using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
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

namespace MakingGrid
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public class RandomColor
        {
            Type colorType = typeof(System.Drawing.Color);
            PropertyInfo[] proInfos;
            Random rand = new Random();
            public RandomColor()
            {
                proInfos = colorType.GetProperties(BindingFlags.Static | BindingFlags.Public);
            }

            public System.Drawing.Color NextColor()
            {
                System.Drawing.Color ColorName = System.Drawing.Color.FromName(proInfos[rand.Next(0, proInfos.Length)].Name);
                return ColorName;
            }
        }
        private void Windom_Loaded(object sender, RoutedEventArgs e)
        {

            for (int C = 0; C < 50; C++)
            {
                MyGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            


            for (int r = 0; r < 50; r++)
            {
                MyGrid.RowDefinitions.Add(new RowDefinition());

            }


            Button btn;
            int i = 0;
            RandomColor randomColor = new RandomColor();

            for (int row = 0; row < MyGrid.RowDefinitions.Count; row++)
            {
                for (int column = 0; column <  MyGrid.ColumnDefinitions.Count; column++)
                {
                    i++;
                    btn = new Button();

                    btn.Width = Width / MyGrid.ColumnDefinitions.Count;
                    btn.Height = Height / MyGrid.RowDefinitions.Count;
                        ;


                    //btn.Content = i.ToString();

                    BrushConverter converter = new BrushConverter();
                    Brush brush = (Brush)converter.ConvertFromString(randomColor.NextColor().Name);
                    btn.Background = brush;

                    //btn.FontSize = 0;
                    btn.Margin = new Thickness(0);
                    btn.Padding = new Thickness(0);
                    btn.VerticalAlignment = VerticalAlignment.Center;
                    btn.SetValue(Grid.ColumnProperty, column);
                    btn.SetValue(Grid.RowProperty, row);
                    MyGrid.Children.Add(btn);
                }
            }
        }
    }
   
}
