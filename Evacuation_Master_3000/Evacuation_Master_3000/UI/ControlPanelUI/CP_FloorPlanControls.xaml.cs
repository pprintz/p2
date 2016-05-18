using System;
using System.Windows;
using System.Windows.Controls;

namespace Evacuation_Master_3000
{
    /// <summary>
    /// Interaction logic for CP_FloorPlanControls.xaml
    /// </summary>
    public partial class CP_FloorPlanControls
    {
        public CP_FloorPlanControls(MainWindow parentWindow)
        {
            InitializeComponent();
            MakeWall.Click += OnRadioButtonClicked;
            MakeDoor.Click += OnRadioButtonClicked;
            MakeFree.Click += OnRadioButtonClicked;
            MakePath.Click += OnRadioButtonClicked;
            MakePerson.Click += OnRadioButtonClicked;
            MakeStair.Click += OnRadioButtonClicked;
            parentWindow.floorPlanVisualiserControl.OnBuildingBlockTypeFetch += OnFetchType;

            MakeWall.IsChecked = true;
            OnRadioButtonClicked(MakeWall, null);
        }

        private Tile.Types TileType { get; set; }

        private void OnRadioButtonClicked(object sender, RoutedEventArgs e) {
            RadioButton radioButton = sender as RadioButton;

            Tile.Types type;
            Enum.TryParse(radioButton?.Tag.ToString(), out type);
            TileType = type;
        }

        private Tile.Types OnFetchType() {
            return TileType;
        }
    }
}
