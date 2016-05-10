using System.Collections.Generic;

namespace Evacuation_Master_3000
{
    public class DataSimulationStatistics
    {
        public DataSimulationStatistics(Person person)
        {
            MovementSteps = new List<MovementStep>();
        }

        public List<MovementStep> MovementSteps { get; set; }
        public void CountTicksBeingBlocked(int ticksSpentTryingToMove)
        {
            TicksWaited += ticksSpentTryingToMove;
        }
        public double DistanceTraveled { get; set; }
        public int TicksWaited { get; set; }
    }
}