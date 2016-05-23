using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Evacuation_Master_3000 {
    /// <summary>
    /// Interaction logic for SwitchBetweenFloorsControl.xaml
    /// </summary>
    public partial class SwitchBetweenFloorsControl : UserControl {
        public SwitchBetweenFloorsControl() {
            InitializeComponent();

            ArrowUp.Click += SwitchFloor;
            ArrowDown.Click += SwitchFloor;
        }

        public ChangeVisualFloor OnChangeVisualFloorChange;
        const string floorOffPath = "/UI/Visual/floor_switcher_bar_off_2.png";
        const string floorOnPath = "/UI/Visual/floor_switcher_bar_on.png";
        private Image[] FloorBars;
        private int FloorAmount { get; set; }
        private int _currentFloor = 0;
        private int CurrentFloor {
            get { return _currentFloor; }
            set {
                bool changedFloor = true;
                if (value < 0 || value > FloorAmount)
                    changedFloor = false;

                if (value >= FloorAmount)
                    _currentFloor = FloorAmount - 1;
                else if (value <= 0)
                    _currentFloor = 0;
                else 
                    _currentFloor = value;

                if(changedFloor)
                    OnChangeVisualFloorChange?.Invoke(CurrentFloor); //Event is only invoked, when a floor has actually changed!
            }
        }
        enum OnOrNot { OnCurrentFloor, NotOnCurrentFloor }

        public void SetupFloorSwitcherVisuals(int floorAmount) {
            FloorAmount = floorAmount;
            FloorBars = new Image[FloorAmount];
            for (int floor = FloorAmount; floor > 0; floor--) {
                Image img = new Image() {
                    Source = ChangeImageSource(OnOrNot.NotOnCurrentFloor),
                    Width = 20,
                    Height = 10
                };
                FloorBars[floor-1] = img;
                FloorBarContainer.Children.Add(img);
            }
            FloorBars[CurrentFloor].Source = ChangeImageSource(OnOrNot.OnCurrentFloor);
        }

        private ImageSource ChangeImageSource(OnOrNot onOrNor) {
            return (onOrNor == OnOrNot.OnCurrentFloor) ?
                new BitmapImage(new Uri(floorOnPath, UriKind.Relative))
                : new BitmapImage(new Uri(floorOffPath, UriKind.Relative));
        }

        private void SwitchFloor(object sender, RoutedEventArgs e) {
            Button senderObject = sender as Button;

            FloorBars[CurrentFloor].Source =  ChangeImageSource(OnOrNot.NotOnCurrentFloor);
            CurrentFloor += senderObject.Name == "ArrowUp" ? 1 : -1;
            FloorBars[CurrentFloor].Source = ChangeImageSource(OnOrNot.OnCurrentFloor);
        }
    }
}
