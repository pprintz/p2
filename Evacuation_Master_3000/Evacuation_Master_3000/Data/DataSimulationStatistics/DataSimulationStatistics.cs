using System.Collections.Generic;

namespace Evacuation_Master_3000
{
    public class DataSimulationStatistics
    {
        public DataSimulationStatistics(Person person)
        {
            this._person = person;
        }

        public List<MovementStep> MovementSteps { get; set; }
        public void CountTicksBeingBlocked(int ticksSpentTryingToMove)
        {
            TicksWaited += ticksSpentTryingToMove;
        }
        private Person _person;
        public double DistanceTraveled { get; set; }
        public int TicksWaited { get; set; }
    }
}