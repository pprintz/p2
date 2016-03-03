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

        Button btn;
        List<Button> btnList = new List<Button>();

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
        
        public void Windom_Loaded(object sender, RoutedEventArgs e)
        {

            for (int C = 0; C < 50; C++)
            {
                MyGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            


            for (int r = 0; r < 50; r++)
            {
                MyGrid.RowDefinitions.Add(new RowDefinition());

            }


            //Button btn;
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
                        


                    //rect. = i.ToString();

                    BrushConverter converter = new BrushConverter();
                    Brush brush = (Brush)converter.ConvertFromString(randomColor.NextColor().Name);
                    //rect.Fill = new SolidColorBrush(Colors.Red);
                    btn.Background = brush;

                    //btn.FontSize = 0;
                    btn.Margin = new Thickness(0);
                    //btn.Padding = new Thickness(0);
                    btn.VerticalAlignment = VerticalAlignment.Center;
                    btn.SetValue(Grid.ColumnProperty, column);
                    btn.SetValue(Grid.RowProperty, row);
                    MyGrid.Children.Add(btn);
                    btnList.Add(btn);
                }
            }
        }

        //public void isWindowSizeChanged(Button btn)
        //{
        //    Button btnnew = new Button();
        //    //int btnwidth;
        //    //btnwidth = (int)GridLength.Auto;
        //    btnnew.Width = GridLength.Auto.Value;
        //    btnnew.Height = GridLength.Auto.Value;  
        //}

        private void MyGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //int btnwidth;
            //btnwidth = (int)GridLength.Auto;
            //btn.Width = GridLength.Auto.Value;
            //btn.Height = GridLength.Auto.Value;
            //ResizeMode btn = ResizeMode.CanResizeWithGrip;
            foreach (Button btn in btnList)
            {
                new GridLength(1, GridUnitType.Star);
                //btn.ActualWidth = GridLength.Auto.Value;
                //ActualHeight = GridLength.Auto.Value;
                btn.Width = 1000;
                btn.Height = 1000;
                //btn.Height = GridLength.Auto.Value;
                //btn.Width = GridLength.Auto.Value;
            }
            
        }
    }   
   
}
