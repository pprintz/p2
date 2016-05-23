using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Evacuation_Master_3000.ImageScan;
using static Evacuation_Master_3000.ImportExportSettings;
using System.Windows.Controls;

namespace Evacuation_Master_3000
{
    /// <summary>
    /// Interaction logic for FloorPlanVisualiser.xaml
    /// </summary>
    public partial class FloorPlanVisualiser
    {
        public FloorPlanVisualiser(MainWindow mainWindow)
        {
            InitializeComponent();
            Person.OnPersonMoved += UpdateVisualsOnEvacuatableMoved;
            UserInterface.OnReset += UpdateVisualOnReset;
            _mainWindow = mainWindow;
            AllRectangles = new Dictionary<string, Rectangle>();
        }
        /// <summary>
        /// Resets the visualisation of the building, according to each tile's original type.
        /// </summary>
        private void UpdateVisualOnReset()
        {
            foreach (BuildingBlock buildingBlock in LocalFloorPlan.Tiles.Values.Where(t => t.OriginalType != t.Type).Cast<BuildingBlock>())
            {
                var rectangle = _floorContainer[buildingBlock.Z].Children.Cast<Rectangle>()
                    .Single(c => Coordinate(buildingBlock) == c.Tag.ToString());
                ColorizeBuildingBlock(rectangle, buildingBlock.OriginalType);
            }
            foreach (BuildingBlock buildingBlock in LocalFloorPlan.Tiles.Values.Cast<BuildingBlock>().Where(b => b.HeatmapCounter != 0))
            {
                buildingBlock.HeatmapCounter = 0;
                Rectangle rectangle =
                    _floorContainer[buildingBlock.Z].Children.Cast<Rectangle>()
                        .Single(c => Coordinate(buildingBlock) == c.Tag.ToString());
                ColorizeBuildingBlock(rectangle, buildingBlock.OriginalType);
            }
        }

        private readonly MainWindow _mainWindow;
        private IFloorPlan LocalFloorPlan { get; set; }
        private Dictionary<string, Person> LocalPeople { get; set; }
        private Grid[] _floorContainer;
        private SwitchBetweenFloorsControl FloorSwitcherControls { get; set; }            
        private Dictionary<string, Rectangle> AllRectangles { get; }
        public int TileSize { get; } = 10;

        public void ImplementFloorPlan(IFloorPlan floorPlan, Dictionary<int, Person> people)
        {

            LocalPeople = people.ToDictionary(k => Coordinate(k.Value.Position), v => v.Value);

            //Override the local floorplan to correspond to the new floorplan
            LocalFloorPlan = floorPlan;

            /* Ideen er vel, at man skal kalde ImplementFloorPlan() for hver ændring, d.v.s. ifm alle simuleringer osv. 
            I så fald skal der tages højde for, at der ikke genereres en ny visualRepresentation hver gang
            - det er nemmere at iterate gennem alle ændringer og ændre elevation types tilsvarende */
            if (_floorContainer == null)
                CreateVisualRepresentation();

            /* Add controls to switch between floors, if there are more than 1 floor*/
            if (LocalFloorPlan.FloorAmount > 1)
                AddFloorPlanSwitcherControls(LocalFloorPlan.FloorAmount);
        }

        private void CreateVisualRepresentation()
        {
            int width = LocalFloorPlan.Width;
            int height = LocalFloorPlan.Height;
            int floorAmount = LocalFloorPlan.FloorAmount;
            _floorContainer = new Grid[floorAmount];

            for (int z = 0; z < floorAmount; z++)
            {
                Grid container = new Grid()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                };
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Rectangle figure = new Rectangle
                        {
                            Height = TileSize,
                            Width = TileSize,
                            Fill = new SolidColorBrush(Colors.White),
                            Tag = Coordinate(x, y, z), /* Makes binding rectangles to buildingblocks easier */
                            Margin = new Thickness(0, 0, x * TileSize * 2 + x, y * TileSize * 2 + y)
                        };

                        if (LocalFloorPlan.Tiles[Coordinate(x, y, z)].Type != Tile.Types.Free)
                            ColorizeBuildingBlock(figure, LocalFloorPlan.Tiles[Coordinate(x, y, z)].Type);

                        BuildingBlock current = (LocalFloorPlan.Tiles[Coordinate(x, y, z)] as BuildingBlock);
                        current.Figure = figure;                                                //<<-------------------- Lige nu bliver current.figure ikke brugt til de to nedenstående assignments - er det meningen/hensigten?
                        figure.ToolTip = current.ToString();
                        figure.MouseLeftButtonDown += OnBuildingBlockClick;


                        AllRectangles.Add(Coordinate(x, y, z), figure);
                        container.Children.Add(figure);
                    }
                }
                _floorContainer[z] = container;
            }

            VisualContainer.Children.Add(_floorContainer[0]);
        }

        private void AddFloorPlanSwitcherControls(int floorAmount)
        {
            /* Setup and insert floor switcher controls */
            FloorSwitcherControls = new SwitchBetweenFloorsControl();
            FloorSwitcherControls.OnChangeVisualFloorChange += ChangeFloor; /* Callback når der er trykket op/ned mellem floors */
            FloorSwitcherControls.HorizontalAlignment = HorizontalAlignment.Right;
            FloorSwitcherControls.VerticalAlignment = VerticalAlignment.Bottom;
            FloorSwitcherControls.Margin = new Thickness(0, 0, 25, 25);
            FloorSwitcherControls.SetupFloorSwitcherVisuals(LocalFloorPlan.FloorAmount);

            OverlayContainer.Children.Add(FloorSwitcherControls);
        }

        private void ChangeFloor(int currentFloor)
        {
            VisualContainer.Children.Clear();
            VisualContainer.Children.Add(_floorContainer[currentFloor]);
            /* Logik der sørger for at det er den korrekte floor der vises */
        }

        public delegate Tile.Types BuildingBlockTypeFetch();
        public BuildingBlockTypeFetch OnBuildingBlockTypeFetch;
        private BuildingBlock previousBlock;

        private void OnBuildingBlockClick(object sender, MouseButtonEventArgs e)
        {
            Tile.Types type = (Tile.Types)OnBuildingBlockTypeFetch?.Invoke();       //Get the type of the currently radio'ed FloorPlanControl-type
            Rectangle senderRectangle = sender as Rectangle;                    //Get a reference to the sender rectangle
            if (senderRectangle == null) throw new GeneralInternalException();
            BuildingBlock senderBlock = (BuildingBlock)LocalFloorPlan.Tiles[senderRectangle.Tag.ToString()];

            SetBlockType(senderBlock, type);
            if (Keyboard.IsKeyDown(Settings.LineToolKey))
            {
                if (previousBlock != null)
                {
                    DrawLine(senderBlock, type);
                }
                previousBlock = senderBlock;
            }
        }

        private void SetBlockType(BuildingBlock block, Tile.Types targetType)
        {
            if (targetType != Tile.Types.Person)
            {
                UserInterface.BuildingHasBeenChanged = true;
            }
            block.Type = targetType;
            block.OriginalType = targetType;
            ColorizeBuildingBlock(block.Figure, targetType);
        }

        private void DrawLine(BuildingBlock block, Tile.Types targetType)
        {
            int deltaX = block.X - previousBlock.X;
            int deltaY = block.Y - previousBlock.Y;
            if (deltaX == 0 && deltaY == 0)
            {
                // The same block has been pressed twice.
                return;
            }
            double deltaTilt = Math.Min(Math.Abs((double)deltaY / (double)deltaX), Math.Abs((double)deltaY)) * Math.Sign((double)deltaY / (double)deltaX);
            double tilt = 0;

            int i = 0;
            do
            {
                int j = 0;
                do
                {
                    int x = i + previousBlock.X;
                    int y = (int)(tilt) + previousBlock.Y + j;

                    SetBlockType((BuildingBlock)LocalFloorPlan.Tiles[Coordinate(x, y, block.Z)], targetType);

                    j += Math.Sign(deltaY);
                } while (Math.Abs(j) < Math.Abs(deltaTilt));

                tilt += deltaTilt * Math.Sign(deltaX);
                i += Math.Sign(deltaX);
            } while (Math.Abs(i) < Math.Abs(deltaX));
        }

        public void LineToolReleased()
        {
            previousBlock = null;
        }

        public static bool FirstTime = true;

        /// <summary>
        /// Updates the visualisation of the building according to a persons step in the simulation.
        /// </summary>
        /// <param name="person"></param>
        private void UpdateVisualsOnEvacuatableMoved(Person person)
        {
            if (FirstTime)
            {
                foreach (Grid grid in _floorContainer)
                {
                    foreach (Rectangle rect in grid.Children.Cast<Rectangle>())
                    {
                        BuildingBlock current = LocalFloorPlan.Tiles[rect.Tag.ToString()] as BuildingBlock;
                        rect.ToolTip = current?.Priority + " ," + current?.Room;
                    }
                }
                FirstTime = false;
            }
            BuildingBlock prev = person.PathList[person.StepsTaken - 1];
            BuildingBlock next = person.PathList[person.StepsTaken];
            Rectangle prevRectangleToColorize;
            AllRectangles.TryGetValue(Coordinate(prev), out prevRectangleToColorize);
            if (prev.OriginalType == Tile.Types.Person)
            {
                prev.Type = Tile.Types.Free;
                if (_mainWindow.TheUserInterface.HeatMapActivated)
                    ColorRectangle(prevRectangleToColorize, CalculateHeatMapColor(prev));
                else
                    ColorizeBuildingBlock(prevRectangleToColorize, Tile.Types.Free);
            }
            else
            {
                prev.Type = prev.OriginalType;
                if (_mainWindow.TheUserInterface.HeatMapActivated)
                    ColorRectangle(prevRectangleToColorize, CalculateHeatMapColor(prev));
                else
                    ColorizeBuildingBlock(prevRectangleToColorize, prev.OriginalType);
            }
            Rectangle nextRectangleToColorize;
            AllRectangles.TryGetValue(Coordinate(next), out nextRectangleToColorize);
            if (next.OriginalType == Tile.Types.Exit || next.OriginalType == Tile.Types.Stair)
            {
                next.Type = next.OriginalType;
                ColorizeBuildingBlock(nextRectangleToColorize, next.OriginalType);
            }
            else
            {
                next.Type = Tile.Types.Person;
                ColorizeBuildingBlock(nextRectangleToColorize, next.Type);
            }
        }

        private void ColorizeBuildingBlock(Rectangle buildingBlockRepresentation, Tile.Types type)
        {
            Color newColor;
            switch (type)
            {
                case Tile.Types.Free:
                    newColor = Colors.White;
                    break;
                case Tile.Types.Occupied:
                    newColor = Colors.Green;
                    break;
                case Tile.Types.Furniture:
                    newColor = Colors.Gray;
                    break;
                case Tile.Types.Wall:
                    newColor = Colors.Black;
                    break;
                case Tile.Types.Door:
                    newColor = Colors.Pink;
                    break;
                case Tile.Types.Exit:
                    newColor = Colors.Blue;
                    break;
                case Tile.Types.Person:
                    newColor = Colors.BlueViolet;
                    break;
                case Tile.Types.Stair:
                    newColor = Colors.Thistle;
                    break;
                default:
                    newColor = Colors.BlanchedAlmond;
                    break;
            }

            ColorRectangle(buildingBlockRepresentation, newColor);
        }

        private void ColorRectangle(Rectangle rect, Color color)
        {
            rect.Fill = new SolidColorBrush(color);
        }

        private Color CalculateHeatMapColor(BuildingBlock block)
        {
            // Modified from: http://www.andrewnoske.com/wiki/Code_-_heatmaps_and_color_gradients

            double value =
                Math.Min(
                    (double) block.HeatmapCounter/((double) _mainWindow.TheUserInterface.LocalPeopleDictionary.Count*2),
                    1.0);
            int colorAmount = 4;
            double[,] color =
            {
                {
                    0,
                    0,
                    255
                }
                ,
                {
                    0,
                    255,
                    0
                }
                ,
                {
                    255,
                    255,
                    0
                }
                ,
                {
                    255,
                    0,
                    0
                }
            };
            // A static array of 4 colors:  (blue,   green,  yellow,  red) using {r,g,b} for each.

            int idx1; // |-- Our desired color will be between these two indexes in "color".
            int idx2; // |
            double fractBetween = 0; // Fraction between "idx1" and "idx2" where our value is.

            if (value <= 0)
            {
                idx1 = idx2 = 0;
            } // accounts for an input <=0
            else if (value >= 255)
            {
                idx1 = idx2 = colorAmount - 1;
            } // accounts for an input >=0
            else
            {
                value = value*(colorAmount - 1); // Will multiply value by 3.
                idx1 = (int) Math.Floor(value); // Our desired color will be after this index.
                idx2 = Math.Min(idx1 + 1, colorAmount - 1); // ... and before this index (inclusive).
                fractBetween = value - idx1; // Distance between the two indexes (0-1).
            }


            int red = (int) Math.Round((color[idx2, 0] - color[idx1, 0])*fractBetween + color[idx1, 0]);
            int green = (int) Math.Round((color[idx2, 1] - color[idx1, 1])*fractBetween + color[idx1, 1]);
            int blue = (int) Math.Round((color[idx2, 2] - color[idx1, 2])*fractBetween + color[idx1, 2]);
            return new Color
            {
                A = 255,
                R = Convert.ToByte(red),
                G = Convert.ToByte(green),
                B = Convert.ToByte(blue)
            };
        }
    }
}
