﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Evacuation_Master_3000.ImportExportSettings;

namespace Evacuation_Master_3000
{
    /// <summary>
    /// Interaction logic for FloorPlanVisualiser.xaml
    /// </summary>
    public partial class FloorPlanVisualiser : UserControl
    {
        public FloorPlanVisualiser()
        {
            InitializeComponent();
            Person.OnPersonMoved += UpdateVisualsOnEvacuatableMoved;
            UserInterface.OnReset += UpdateVisualOnReset;
        }

        private void UpdateVisualOnReset()
        {
            foreach (Tile tile in localFloorPlan.Tiles.Values.Where(t => t.OriginalType != t.Type))
            {
                var rectangle = FloorContainer[tile.Z].Children.Cast<Rectangle>()
                    .Single(c => Coordinate(tile) == c.Tag.ToString());
                ColorizeBuildingBlock(rectangle, tile.OriginalType);
            }
        }

        private IFloorPlan localFloorPlan { get; set; }
        private Dictionary<string, Person> localPeople { get; set; }
        private Dictionary<string, Tile> tilesWithChanges { get; set; }
        private UniformGrid[] FloorContainer;
        private SwitchBetweenFloorsControl floorSwitcherControls { get; set; }              //<<------ OBS er det nødvendigt med property til at gemme floorswitchcontrols i???

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
            FloorContainer = new UniformGrid[floorAmount];

            for (int z = 0; z < floorAmount; z++)
            {
                UniformGrid container = new UniformGrid()
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
                            Fill = z % 2 == 0 ? new SolidColorBrush(Colors.Aqua) : new SolidColorBrush(Colors.Bisque),
                            Tag = Coordinate(x, y, z) /* Makes binding rectangles to buildingblocks easier */
                        };
                        BuildingBlock current = (localFloorPlan.Tiles[Coordinate(x, y, z)] as BuildingBlock);
                        current.figure = figure;
                        figure.ToolTip = current.Priority + " , " + current.Room;
                        figure.MouseLeftButtonDown += OnBuildingBlockClick;
                        container.Children.Add(figure);
                    }
                }
                FloorContainer[z] = container;
            }
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
            VisualContainer.Children.Add(FloorContainer[0]);
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
            BuildingBlock senderBlock = (BuildingBlock) localFloorPlan.Tiles[senderRectangle.Tag.ToString()];

            SetBlockType(senderBlock, type);
            if (Keyboard.IsKeyDown(Key.LeftShift) && previousBlock != null)
            {
                DrawLine(senderBlock,type);
            }
            previousBlock = senderBlock;
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
            if (deltaX - deltaY == 0)
            {
                return;
            }
            double deltaTilt = Math.Min(Math.Abs((double)deltaY / (double)deltaX), Math.Abs(deltaY)) * Math.Sign((double)deltaY / (double)deltaX);
            double tilt = 0;

            int i = 0;
            do
            {
                int j = 0;
                do
                {
                    int x = i + previousBlock.X;
                    int y = (int)(tilt) + previousBlock.Y + j;

                    SetBlockType((BuildingBlock)localFloorPlan.Tiles[Coordinate(x,y,block.Z)],targetType);

                    j += Math.Sign(deltaY);
                } while (Math.Abs(j) < Math.Abs(deltaTilt));

                tilt += deltaTilt * Math.Sign(deltaX);
                i += Math.Sign(deltaX);
            } while (Math.Abs(i) < Math.Abs(deltaX));
        }

        private static bool firstTime = true;

        private void UpdateVisualsOnEvacuatableMoved(Person person)
        {
            if (firstTime)
            {
                foreach (UniformGrid uniformGrid in FloorContainer)
                {
                    foreach (var rect in uniformGrid.Children.Cast<Rectangle>())
                    {
                        BuildingBlock current = localFloorPlan.Tiles[rect.Tag.ToString()] as BuildingBlock;
                        rect.ToolTip = current.Priority + " ," + current.Room;
                    }
                }
            }
            BuildingBlock prev = person.PathList[person.stepsTaken - 1];
            BuildingBlock next = person.PathList[person.stepsTaken];
            foreach (Rectangle child in FloorContainer[prev.Z].Children.Cast<Rectangle>())
            {
                if (child.Tag.ToString() == ImportExportSettings.Coordinate(prev))
                {
                    if (prev.OriginalType == Tile.Types.Person)
                    {
                        prev.Type = Tile.Types.Free;
                        ColorizeBuildingBlock(child, Tile.Types.Free);
                    }
                    else
                    {
                        prev.Type = prev.OriginalType;
                        ColorizeBuildingBlock(child, prev.OriginalType);
                    }
                }
                else if (child.Tag.ToString() == ImportExportSettings.Coordinate(next))
                {
                    if (next.OriginalType == Tile.Types.Exit || next.OriginalType == Tile.Types.Stair)
                    {
                        next.Type = next.OriginalType;
                        ColorizeBuildingBlock(child, next.OriginalType);
                    }
                    else
                    {
                        next.Type = Tile.Types.Person;
                        ColorizeBuildingBlock(child, next.Type);
                    }
                }
            }

        }

        /* Might need re-work - lavet QnD! */
        private void ColorizeBuildingBlock(Rectangle buildingBlockRepresentation, Tile.Types type)
        {
            SolidColorBrush newColor;
            switch (type)
            {
                case Tile.Types.Free:
                    newColor = new SolidColorBrush(Colors.White);
                    break;
                case Tile.Types.Occupied:
                    newColor = new SolidColorBrush(Colors.Green);
                    break;
                case Tile.Types.Furniture:
                    newColor = new SolidColorBrush(Colors.Gray);
                    break;
                case Tile.Types.Wall:
                    newColor = new SolidColorBrush(Colors.Black);
                    break;
                case Tile.Types.Door:
                    newColor = new SolidColorBrush(Colors.Pink);
                    break;
                case Tile.Types.Exit:
                    newColor = new SolidColorBrush(Colors.Blue);
                    break;
                case Tile.Types.Person:
                    newColor = new SolidColorBrush(Colors.Red);
                    break;
                case Tile.Types.Stair:
                    newColor = new SolidColorBrush(Colors.BlueViolet);
                    break;
                default:
                    newColor = new SolidColorBrush(Colors.BlanchedAlmond);
                    break;
            }

            buildingBlockRepresentation.Fill = newColor;
        }



        //Change single tile on mouse click
        //change 2 tiles (person move)
    }
}
