using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

[assembly: InternalsVisibleTo("P2TestEnvironment")]
namespace Evacuation_Master_3000
{
    internal class Data : IData
    {
        public Data()
        {
            _unevacuatedPeople = new List<Person>();
            TheFloorPlan = new FloorPlan();
            _allPeople = new Dictionary<int, Person>();
        }

        public IFloorPlan TheFloorPlan { get; }
        private Dictionary<int, Person> _allPeople;
        private Dictionary<int, Person> AllPeople => _allPeople;
        public static event FunctionDone OnPathCalculationDone;
        public static event Tick OnTick;

        public Dictionary<int, Person> PrepareSimulation(IFloorPlan floorPlan)
        {
            if (UserInterface.BuildingHasBeenChanged)
            {
                TheFloorPlan.Initiate();
                foreach (Tile tile in floorPlan.Tiles.Values)
                {
                    tile.OriginalType = tile.Type;
                    if (tile.OriginalType == Tile.Types.Person)
                    {
                        Person current = new Person(tile as BuildingBlock);
                        if (AllPeople.Values.All(p => !Equals(p.OriginalPosition, tile)))
                        {
                            AllPeople.Add(current.ID, current);
                        }
                    }
                }
            }
            foreach (Tile value in floorPlan.Tiles.Values.Where(t => t.OriginalType == Tile.Types.Person))
            {
                if (AllPeople.Values.All(p => !Equals(p.OriginalPosition, value)))
                {
                    Person newPerson = new Person(value as BuildingBlock);
                    AllPeople.Add(newPerson.ID, newPerson);
                }
            }
            return AllPeople;
        }

        public Dictionary<int, Person> StartSimulation(bool heatmap, IPathfinding pathfindingAlgorithm, int simulationSpeed)                                           //<---- kan formentligt være void?
        {
            foreach (Person person in AllPeople.Values.Where(p => p.SimulationSpeed != simulationSpeed))
            {
                person.SimulationSpeed = simulationSpeed;
            }
            if (UserInterface.BuildingHasBeenChanged)
            {
                if (AllPeople != null)
                {
                    foreach (Person person in AllPeople.Values.Where(p => !p.NoPathAvailable))
                    {
                        person.PathList.Clear();
                        if (person.NewPersonInGrid)
                        {
                            // Refactor into one function <<<<<<<<<<<<<<<<<<<<
                            person.Evacuated = false;
                            person.OnPersonEvacuated += RemoveEvacuatedPerson;
                            person.SimulationSpeed = simulationSpeed;
                            person.OnExtendedPathRequest += FindNewPath;
                            person.NewPersonInGrid = false;
                        }
                        person.PathList.AddRange(
                            pathfindingAlgorithm.CalculatePath(person).Cast<BuildingBlock>().ToList());
                    }
                }
                UserInterface.BuildingHasBeenChanged = false;
            }

            if (AllPeople == null) return null;

            foreach (Person person in AllPeople.Values.Where(p => p.PathList.Count == 0 && !p.NoPathAvailable))
            {
                person.Evacuated = false;
                person.OnPersonEvacuated += RemoveEvacuatedPerson;
                person.SimulationSpeed = simulationSpeed;
                person.OnExtendedPathRequest += FindNewPath;
                person.PathList.AddRange(
                    pathfindingAlgorithm.CalculatePath(person).Cast<BuildingBlock>().ToList());
            }
            OnPathCalculationDone?.Invoke(this, null);
            StartTicks();
            return AllPeople;
            
        }

        private void StartTicks()
        {
            while (AllPeople.Values.Any(p => !p.Evacuated && !p.NoPathAvailable) && !UserInterface.IsSimulationPaused && !UserInterface.HasSimulationEnded)
            {
                Yield(1);
                if (!UserInterface.HasSimulationEnded)
                {
                    OnTick?.Invoke();
                }
                // unchecked throws an OverflowException if we've spent more than 600+ hours on one tick.
            }

        }
        public static void Yield(long ticks)
        {
            long dtEnd = DateTime.Now.AddTicks(ticks).Ticks;
            while (DateTime.Now.Ticks < dtEnd)
            {
                Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate { return null; }, null);
            }
        }
        private static void RemoveEvacuatedPerson(Person person)
        {
            OnTick -= person.ConditionalMove;
        }

        private IEnumerable<BuildingBlock> FindNewPath(Person person)
        {
            BuildingBlock target = person.PathList[person.StepsTaken + 1];
            BuildingBlock pos = person.PathList[person.StepsTaken];
            if (AllPeople.Values.Count(p => p.PathList[p.StepsTaken] == target && p.PathList.Count > p.StepsTaken + 1 && p.PathList[p.StepsTaken + 1] == pos) == 1)
            {
                Person personBlocking = AllPeople.Values.First(p => p.PathList[p.StepsTaken] == target);
                if (personBlocking.PathList.Count > 0 &&
                    (person.Position as BuildingBlock).BuildingBlockNeighbours.Any(n => n.Type != Tile.Types.Person))
                {
                    if (personBlocking.PathList.Count(b => b.Type != Tile.Types.Person) <
                        person.PathList.Count(b => b.Type != Tile.Types.Person))
                    {
                        person.PathList.Clear();
                        person.PathList.AddRange(personBlocking.PathList);
                        person.StepsTaken = personBlocking.StepsTaken + 1;
                    }
                }
            }
            return null;
        }

        private readonly List<Person> _unevacuatedPeople;

        public IFloorPlan ImportFloorPlan(string fileName)
        {
            BuildingInformationCollection buildingInformation = Import.ImportBuilding(fileName);
            IFloorPlan temporaryFloorPlan = CreateFloorPlan(buildingInformation);
            Import.EffectuateFloorPlanSettings(buildingInformation, ref temporaryFloorPlan, ref _allPeople);
            return temporaryFloorPlan;
        }

        private IFloorPlan CreateFloorPlan(BuildingInformationCollection buildingInformation)
        {
            string[] headers = new string[buildingInformation.Floors];
            for (int currentFloor = 0; currentFloor < buildingInformation.Floors; currentFloor++)
            {
                if (buildingInformation.FloorCollection[currentFloor] == null)
                    continue;

                headers[currentFloor] = buildingInformation.FloorCollection[currentFloor].Header;
            }
            return CreateFloorPlan(buildingInformation.Width, buildingInformation.Height, buildingInformation.Floors, buildingInformation.Description, headers);
        }

        public IFloorPlan CreateFloorPlan(int width, int height, int floorAmount, string description)
        {
            return CreateFloorPlan(width, height, floorAmount, description, null);
        }
        private IFloorPlan CreateFloorPlan(int width, int height, int floorAmount, string description, string[] headers)
        {
            TheFloorPlan.CreateFloorPlan(width, height, floorAmount, description, headers);
            return TheFloorPlan;
        }

        public IFloorPlan ExportFloorPlan(string filePath, IFloorPlan floorPlan, Dictionary<int, Person> allPeople)
        {
            Export.ExportBuilding(filePath, floorPlan, allPeople);
            return TheFloorPlan;   

        }
    }
}