using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using System.Diagnostics;

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
            //Person.OnPersonMoved += Statistics?
        }

        private IPathfinding pathfindingAlgorithm;
        public IFloorPlan TheFloorPlan { get; private set; }
        private Dictionary<int, Person> _allPeople;
        public Dictionary<int, Person> AllPeople { get { return _allPeople; } }
        public event PersonMoved OnSendPersonMoved;

        public IEnumerable<Person> GetPeople(Predicate<Person> predicate)
        {
            return AllPeople.Values.Where(predicate.Invoke);
        }


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
                        if (!AllPeople.Values.Any(p => p.OriginalPosition == tile))
                        {
                            AllPeople.Add(current.ID, current);
                        }
                    }
                }
            }
            foreach (Tile value in floorPlan.Tiles.Values.Where(t => t.OriginalType == Tile.Types.Person))
            {
                if (!AllPeople.Values.Any(p => p.OriginalPosition == value))
                {
                    Person newPerson = new Person(value as BuildingBlock);
                    AllPeople.Add(newPerson.ID, newPerson);
                }
            }
            return AllPeople;
        }

        public static bool SimulationStart = true;
        public Dictionary<int, Person> StartSimulation(bool heatmap, bool stepByStep, IPathfinding pathfindingAlgorithm, int tickLength)                                           //<---- kan formentligt være void?
        {
            this.pathfindingAlgorithm = pathfindingAlgorithm;
            if (UserInterface.BuildingHasBeenChanged)
            {
                if (AllPeople != null)
                {
                    foreach (Person person in AllPeople.Values)
                    {
                        person.PathList.Clear();
                        if (person.NewPersonInGrid)
                        {
                            person.Evacuated = false;
                            person.OnPersonEvacuated += RemoveEvacuatedPerson;
                            person.TickLength = tickLength;
                            person.OnExtendedPathRequest += FindNewPath;
                            person.NewPersonInGrid = false;
                        }
                        person.PathList.AddRange(
                            pathfindingAlgorithm.CalculatePath(person).Cast<BuildingBlock>().ToList());
                    }
                }
                UserInterface.BuildingHasBeenChanged = false;
            }
            foreach (Person person1 in AllPeople.Values.Where(p => p.PathList.Count == 0))
            {
                person1.Evacuated = false;
                person1.OnPersonEvacuated += RemoveEvacuatedPerson;
                person1.TickLength = tickLength;
                person1.OnExtendedPathRequest += FindNewPath;
                person1.PathList.AddRange(
                    pathfindingAlgorithm.CalculatePath(person1).Cast<BuildingBlock>().ToList());
            }
            StartTicks();
            return AllPeople;
        }

        public void StartTicks()
        {
            while (AllPeople.Values.Any(p => !p.Evacuated) && !UserInterface.IsSimulationPaused && !UserInterface.HasSimulationEnded)
            {
                Yield(1);
                if (!UserInterface.HasSimulationEnded)
                {
                    OnTick?.Invoke();
                }
                // unchecked throws an OverflowException if we've spent more than 600+ hours on one tick.
            }

        }
        private void Yield(long ticks)
        {
            long dtEnd = DateTime.Now.AddTicks(ticks).Ticks;
            while (DateTime.Now.Ticks < dtEnd)
            {
                Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate (object unused) { return null; }, null);
            }
        }
        private void RemoveEvacuatedPerson(Person person)
        {
            OnTick -= person.ConditionalMove;
        }

        private IEnumerable<BuildingBlock> FindNewPath(Person person)
        {
            BuildingBlock target = person.PathList[person.stepsTaken + 1];
            BuildingBlock pos = person.PathList[person.stepsTaken];
            if (AllPeople.Values.Count(p => p.PathList[p.stepsTaken] == target && p.PathList.Count > p.stepsTaken + 1 && p.PathList[p.stepsTaken + 1] == pos) == 1)
            {
                Person personBlocking = AllPeople.Values.First(p => p.PathList[p.stepsTaken] == target);
                if (personBlocking.PathList.Count > 0 &&
                    (person.Position as BuildingBlock).BNeighbours.Any(n => n.Type != Tile.Types.Person))
                {
                    if (personBlocking.PathList.Count(b => b.Type != Tile.Types.Person) <
                        person.PathList.Count(b => b.Type != Tile.Types.Person))
                    {
                        person.PathList.Clear();
                        person.PathList.AddRange(personBlocking.PathList);
                        person.stepsTaken = personBlocking.stepsTaken + 1;
                    }
                }
            }
            return null;
        }

        public static event Tick OnTick;

        public DataSimulationStatistics GetSimulationStatistics()
        {
            throw new NotImplementedException();
        }

        private readonly List<Person> _unevacuatedPeople;

        public IFloorPlan ImportFloorPlan(string fileName)
        {
            BuildingInformationCollection buildingInformation = Import.ImportBuilding(fileName);
            IFloorPlan temporaryFloorPlan = CreateFloorPlan(buildingInformation);
            Import.EffectuateFloorPlanSettings(buildingInformation, ref temporaryFloorPlan, ref _allPeople);
            return temporaryFloorPlan;
        }

        public IFloorPlan CreateFloorPlan(BuildingInformationCollection buildingInformation)
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
            return TheFloorPlan;                                                                            ////<<<-------- OBS OBS OBS skal TheFloorPlan sættes til floorPlan (metode parameter)???

        }

        public void ResetData()
        {
            //Reset floorplan
            //Reset Persons
        }

    }

}