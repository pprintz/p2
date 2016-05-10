using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;
using System.Diagnostics;
using System.Windows.Forms.VisualStyles;

[assembly: InternalsVisibleTo("P2TestEnvironment")]
namespace Evacuation_Master_3000
{
    internal class Data : IData
    {
        public Data()
        {
            _unevacuatedPeople = new List<Person>();
            TheFloorPlan = new FloorPlan();
            AllPeople = new Dictionary<int, Person>();
            //Person.OnPersonMoved += Statistics?
        }


        public IFloorPlan TheFloorPlan { get; private set; }
        public Dictionary<int, Person> AllPeople { get; set; }
        public event PersonMoved OnSendPersonMoved;

        public IEnumerable<Person> GetPeople(Predicate<Person> predicate)
        {
            return AllPeople.Values.Where(predicate.Invoke);
        }

        public static bool SimulationStart = true;
        public Dictionary<int, Person> StartSimulation(IFloorPlan floorPlan, bool heatmap, bool stepByStep, IPathfinding pathfindingAlgorithm, int tickLength)
        {
            if (SimulationStart)
            {
                TheFloorPlan.Initiate();
                foreach (Tile tile in floorPlan.Tiles.Values)
                {
                    tile.OriginalType = tile.Type;
                }
                foreach (BuildingBlock tile in TheFloorPlan.Tiles.Values.Where(t => t.Type == Tile.Types.Person).Cast<BuildingBlock>())
                {
                    Person current = new Person(tile, tickLength);
                    if (!AllPeople.ContainsKey(current.ID))
                    {
                        AllPeople.Add(current.ID, current);
                    }
                }
                if (AllPeople != null)
                {
                    _unevacuatedPeople.AddRange(AllPeople.Values);

                    foreach (Person person in AllPeople.Values.Where(p => p.PathList.Count == 0))
                    {
                        person.OnPersonEvacuated += RemoveEvacuatedPerson;
                        //person.OnExtendedPathRequest += IPathfinding.
                        person.PathList.AddRange(pathfindingAlgorithm.CalculatePath(person).Cast<BuildingBlock>().ToList());
                        OnTick += person.ConditionalMove;
                    }
                }
                SimulationStart = false;
            }
            StartTicks();
            return null;
        }

        public void StartTicks()
        {
            while (AllPeople.Values.Any(p => !p.Evacuated) && !UserInterface.IsSimulationPaused)
            {
                Stopwatch stopWatch = Stopwatch.StartNew();
                OnTick?.Invoke();
                Yield(1);
                stopWatch.Stop();
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

        public static event Tick OnTick;

        public DataSimulationStatistics GetSimulationStatistics()
        {
            throw new NotImplementedException();
        }

        private readonly List<Person> _unevacuatedPeople;

        public IFloorPlan ImportFloorPlan(string fileName)
        {
            Import import = new Import(fileName);
            IFloorPlan temporaryFloorPlan = CreateFloorPlan(import.Width, import.Height, import.FloorAmount, import.Description, import.Headers);
            import.ImportFloorPlanSettings(temporaryFloorPlan /*, AllPeople*/); //AllPeople skal sendes med, så der kan tilføjes personer, hvis der er personer i det importerede grid!
            return temporaryFloorPlan;
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

        public void ResetData()
        {
            //Reset floorplan
            //Reset Persons
        }

    }

}