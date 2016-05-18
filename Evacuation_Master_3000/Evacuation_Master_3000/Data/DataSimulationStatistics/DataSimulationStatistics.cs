using System;
using System.Collections.Generic;

namespace Evacuation_Master_3000
{
    public class DataSimulationStatistics
    {
        public DataSimulationStatistics()
        {
            MovementSteps = new List<MovementStep>();
        }
        public int TicksWaited { get; set; }
        public double TimeWhenEvacuated;
        public List<MovementStep> MovementSteps { get; }
        private double _distanceTraveled;
        public double DistanceTraveled
        {
            get { return Math.Round(_distanceTraveled, 2); }
            set { _distanceTraveled = value; }
        }

        public void CountTicksBeingBlocked(int ticksSpentTryingToMove)
        {
            TicksWaited += ticksSpentTryingToMove;
        }

        

       
    }
}