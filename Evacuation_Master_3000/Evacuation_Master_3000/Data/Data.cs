using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms.VisualStyles;
using NUnit.Framework.Compatibility;

namespace Evacuation_Master_3000
{
    internal class Data : IData
    {
        public Data()
        {
            _unevacuatedPeople = new List<Person>();
            TheFloorPlan = new FloorPlan();
        }

        public IFloorPlan TheFloorPlan { get; private set; } 

        public IEnumerable<Person> GetPeople(Predicate<Person> predicate)
        {
            return AllPeople.Values.Where(predicate.Invoke);
        }


        public void StartSimulation(IPathfinding pathfindingAlgorithm, int millisecondsPerTick)
        {
            _unevacuatedPeople.AddRange(AllPeople.Values);
            foreach (Person person in AllPeople.Values)
            {
                person.OnPersonEvacuated += RemoveEvacuatedPerson;
                //person.OnExtendedPathRequest += IPathfinding.NewPath;
                OnTick += person.ConditionalMove;
            }
            while (_unevacuatedPeople.Count > 0)
            {
                Stopwatch stopWatch = Stopwatch.StartNew();
                OnTick?.Invoke();
                stopWatch.Stop();
                // unchecked throws an OverflowException if we've spent more than 600+ hours on one tick.
                int elapsedMilliseconds = unchecked((int) stopWatch.ElapsedMilliseconds);
                int millisecondsToWait = elapsedMilliseconds < millisecondsPerTick
                    ? millisecondsPerTick - elapsedMilliseconds
                    : 0;
                Thread.Sleep(millisecondsToWait);
            }
        }

        private void RemoveEvacuatedPerson(Person person)
        {
            _unevacuatedPeople.Remove(person);
        }

        public event Tick OnTick;

        public DataSimulationStatistics GetSimulationStatistics()
        {
            throw new NotImplementedException();
        }

        public Dictionary<int, Person> AllPeople { get; set; }
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
        private IFloorPlan CreateFloorPlan(int width, int height, int floorAmount, string description, string[] headers) {
            TheFloorPlan.CreateFloorPlan(width, height, floorAmount, description, headers);
            return TheFloorPlan;
        }

        public void ResetData() {
            //Reset floorplan
            //Reset Persons
        }
    }

}