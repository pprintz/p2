using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
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

        private void UpdateVisualOnReset()
        {
            foreach (BuildingBlock buildingBlock in localFloorPlan.Tiles.Values.Where(t => t.OriginalType != t.Type).Cast<BuildingBlock>())
            {
                var rectangle = FloorContainer[buildingBlock.Z].Children.Cast<Rectangle>()
                    .Single(c => Coordinate(buildingBlock) == c.Tag.ToString());
                ColorizeBuildingBlock(rectangle, buildingBlock.OriginalType);
            }
            foreach (BuildingBlock buildingBlock in localFloorPlan.Tiles.Values.Cast<BuildingBlock>().Where(b => b.HeatmapCounter != 0))
            {
                buildingBlock.HeatmapCounter = 0;
                Rectangle rectangle =
                    FloorContainer[buildingBlock.Z].Children.Cast<Rectangle>()
                        .Single(c => Coordinate(buildingBlock) == c.Tag.ToString());
                ColorizeBuildingBlock(rectangle, buildingBlock.OriginalType);
            }
        }

        private readonly MainWindow _mainWindow;
        private IFloorPlan localFloorPlan { get; set; }
        private Dictionary<string, Person> localPeople { get; set; }
        private Dictionary<string, Tile> tilesWithChanges { get; set; }
        private Grid[] FloorContainer;
        private SwitchBetweenFloorsControl floorSwitcherControls { get; set; }              //<<------ OBS er det nødvendigt med property til at gemme floorswitchcontrols i???
        private Dictionary<string, Rectangle> AllRectangles { get; }
        public void ImplementFloorPlan(IFloorPlan floorPlan, Dictionary<int, Person> people)
        {

            localPeople = people.ToDictionary(k => Coordinate(k.Value.Position), v => v.Value);
            //localPeople = people.Where(p => !localPeople.Values.Contains(p as Person)).ToDictionary(k => Coordinate(k.Position.X, k.Position.Y, k.Position.Z), v => v as Person);
            //First find all tiles with changes - this is done with clever use of lambda expressions
            //tilesWithChanges = floorPlan.Tiles.Values.Where(t => !t.Equals(localFloorPlan.Tiles[Coordinate(t.X, t.Y, t.Z)])).ToDictionary(k => Coordinate(k.X, k.Y, k.Z), v => v);

            //Override the local floorplan to correspond to the new floorplan
            localFloorPlan = floorPlan;

            /* Ideen er vel, at man skal kalde ImplementFloorPlan() for hver ændring, d.v.s. ifm alle simuleringer osv. 
            I så fald skal der tages højde for, at der ikke genereres en ny visualRepresentation hver gang
            - det er nemmere at iterate gennem alle ændringer og ændre elevation types tilsvarende */
            if (FloorContainer == null)
                CreateVisualRepresentation();

            //Update the visual representation of the floorplan
            UpdateVisualRepresentation();

            /* Add controls to switch between floors, if there are more than 1 floor*/
            if (localFloorPlan.FloorAmount > 1)
                AddFloorPlanSwitcherControls(localFloorPlan.FloorAmount);
        }

        private void CreateVisualRepresentation()
        {
            int tileSize = 10;
            int width = localFloorPlan.Width;
            int height = localFloorPlan.Height;
            int floorAmount = localFloorPlan.FloorAmount;
            FloorContainer = new Grid[floorAmount];

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
                            Height = tileSize,
                            Width = tileSize,
                            Fill = new SolidColorBrush(Colors.White),
                            Tag = Coordinate(x, y, z), /* Makes binding rectangles to buildingblocks easier */
                            Margin = new Thickness(0, 0, x * tileSize * 2 + x, y * tileSize * 2 + y)
                        };

                        if (localFloorPlan.Tiles[Coordinate(x, y, z)].Type != Tile.Types.Free)
                            ColorizeBuildingBlock(figure, localFloorPlan.Tiles[Coordinate(x, y, z)].Type);

                        BuildingBlock current = (localFloorPlan.Tiles[Coordinate(x, y, z)] as BuildingBlock);
                        current.figure = figure;                                                //<<-------------------- Lige nu bliver current.figure ikke brugt til de to nedenstående assignments - er det meningen/hensigten?
                        figure.ToolTip = current.Priority + " , " + current.Room;
                        figure.MouseLeftButtonDown += OnBuildingBlockClick;


                        AllRectangles.Add(Coordinate(x, y, z), figure);
                        container.Children.Add(figure);
                    }
                }
                FloorContainer[z] = container;
            }

            VisualContainer.Children.Add(FloorContainer[0]);
        }

        private void AddFloorPlanSwitcherControls(int floorAmount)
        {
            /* Setup and insert floor switcher controls */
            floorSwitcherControls = new SwitchBetweenFloorsControl();
            floorSwitcherControls.OnChangeVisualFloorChange += ChangeFloor; /* Callback når der er trykket op/ned mellem floors */
            floorSwitcherControls.HorizontalAlignment = HorizontalAlignment.Right;
            floorSwitcherControls.VerticalAlignment = VerticalAlignment.Bottom;
            floorSwitcherControls.Margin = new Thickness(0, 0, 25, 25);
            floorSwitcherControls.SetupFloorSwitcherVisuals(localFloorPlan.FloorAmount);

            OverlayContainer.Children.Add(floorSwitcherControls);
        }

        private void UpdateVisualRepresentation()
        {
            //for(int z = 0; z < localFloorPlan.FloorAmount; z++) {
            //    for(int y = 0; y < localFloorPlan.Height; y++) {
            //        for(int x = 0; x < localFloorPlan.Width; x++) {

            //        }
            //    }
            //}
        }

        public void UpdateTile(IEnumerable<Tile> tilesToChange)
        {

        }

        public void ChangeFloor(int currentFloor)
        {
            VisualContainer.Children.Clear();
            VisualContainer.Children.Add(FloorContainer[currentFloor]);
            /* Logik der sørger for at det er den korrekte floor der vises */

            //Obs kan problemet løses lettere i stil af dette: ??
            //VisualContainer.Children[currentFloor].Visibility = Visibility.Visible;
        }

        public delegate Tile.Types BuildingBlockTypeFetch();
        public BuildingBlockTypeFetch OnBuildingBlockTypeFetch;
        private BuildingBlock previousBlock;

        private void OnBuildingBlockClick(object sender, MouseButtonEventArgs e)
        {
            Tile.Types type = (Tile.Types)OnBuildingBlockTypeFetch?.Invoke();       //Get the type of the currently radio'ed FloorPlanControl-type
            Rectangle senderRectangle = sender as Rectangle;                    //Get a reference to the sender rectangle
            if (senderRectangle == null) throw new GeneralInternalException();
            BuildingBlock senderBlock = (BuildingBlock)localFloorPlan.Tiles[senderRectangle.Tag.ToString()];

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
            block.Type = targetType;
            ColorizeBuildingBlock(block.figure, targetType);
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

                    SetBlockType((BuildingBlock)localFloorPlan.Tiles[Coordinate(x, y, block.Z)], targetType);

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

        private static bool firstTime = true;

        private void UpdateVisualsOnEvacuatableMoved(Person person)
        {
            if (firstTime)
            {
                foreach (Grid grid in FloorContainer)
                {
                    foreach (Rectangle rect in grid.Children.Cast<Rectangle>())
                    {
                        BuildingBlock current = localFloorPlan.Tiles[rect.Tag.ToString()] as BuildingBlock;
                        rect.ToolTip = current?.Priority + " ," + current?.Room;
                    }
                }
            }
            BuildingBlock prev = person.PathList[person.stepsTaken - 1];
            BuildingBlock next = person.PathList[person.stepsTaken];
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

        /* Might need re-work - lavet QnD! */
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
            // Stolen and modified from: http://www.andrewnoske.com/wiki/Code_-_heatmaps_and_color_gradients

            double value = Math.Min((double)block.HeatmapCounter / (double)_mainWindow.TheUserInterface.LocalPeopleDictionary.Count, 1.0);
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
                value = value * (colorAmount - 1); // Will multiply value by 3.
                idx1 = (int)Math.Floor(value); // Our desired color will be after this index.
                idx2 = Math.Min(idx1 + 1, colorAmount - 1); // ... and before this index (inclusive).
                fractBetween = value - idx1; // Distance between the two indexes (0-1).
            }


            int red = (int)Math.Round((color[idx2, 0] - color[idx1, 0]) * fractBetween + color[idx1, 0]);
            int green = (int)Math.Round((color[idx2, 1] - color[idx1, 1]) * fractBetween + color[idx1, 1]);
            int blue = (int)Math.Round((color[idx2, 2] - color[idx1, 2]) * fractBetween + color[idx1, 2]);
            return new Color
            {
                A = 255,
                R = Convert.ToByte(red),
                G = Convert.ToByte(green),
                B = Convert.ToByte(blue)
            };
        }


        //Change single tile on mouse click
        //change 2 tiles (person move)
    }
}
