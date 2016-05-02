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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Evacuation_Master_3000
{
    /// <summary>
    /// Interaction logic for CP_FloorPlanControls.xaml
    /// </summary>
    public partial class CP_FloorPlanControls : UserControl
    {
        public CP_FloorPlanControls(TheRealMainWindow parentWindow)
        {
            InitializeComponent();
            ParentWindow = parentWindow;
            MakeWall.Click += OnRadioButtonClicked;
            MakeDoor.Click += OnRadioButtonClicked;
            MakeFree.Click += OnRadioButtonClicked;
            MakePath.Click += OnRadioButtonClicked;
            MakePerson.Click += OnRadioButtonClicked;
            parentWindow.floorPlanVisualiserControl.OnBuildingBlockTypeFetch += OnFetchType;
        }

        private TheRealMainWindow ParentWindow { get; }

        //enum RadioButtonTypes { MakeWall, MakeDoor, MakeFree, MakePath, MakePerson }
        //RadioButtonTypes RadioButtonType;

        private Tile.Types TileType { get; set; }

        private void OnRadioButtonClicked(object sender, RoutedEventArgs e) {
            RadioButton radioButton = sender as RadioButton;
            //RadioButtonTypes.TryParse(radioButton.Name, out RadioButtonType);

            //switch (RadioButtonType) {
            //    case RadioButtonTypes.MakeWall:
            //        break;
            //    case RadioButtonTypes.MakeDoor:
            //        break;
            //    case RadioButtonTypes.MakeFree:
            //        break;
            //    case RadioButtonTypes.MakePath:
            //        break;
            //    case RadioButtonTypes.MakePerson:
            //        break;
            //}
            Tile.Types type;
            Enum.TryParse(radioButton.Tag.ToString(), out type);
            TileType = type;
        }

        private Tile.Types OnFetchType() {
            return TileType;
        }
    }
}
