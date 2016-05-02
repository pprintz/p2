using System;
using System.Collections.Generic;

namespace Evacuation_Master_3000
{
    public class Person : IEvacuateable
    {
        // Remember old path unless changes has been made <-- needs to be implemented
        private static int _idCounter = 1;
        public int AmountOfMoves = 0;
        public readonly List<BuildingBlock> PathList = new List<BuildingBlock>();
        private double MovementSpeed { get; }
        private int ticksToWaitBeforeNextMove;
        private int ticksSpentWaiting;
        private int stepsTaken;
        private bool firstRun = true;
        public Tile Position { get; private set; }
        private Tile _target;
        public event PersonEvacuated OnPersonEvacuated;
        public event PersonMoved OnPersonMoved;
        private bool _evacuated;
        public bool Evacuated
        { 
            get { return _evacuated; }
            private set
            {
                _evacuated = value;
                if (value)
                {
                    OnPersonEvacuated?.Invoke(this);
                }
            }
        }

        
        public int ID { get; }
        private double TickLength { get; }
        public event ExtendedPathRequest OnExtendedPathRequest;
        public Person(BuildingBlock position, double tickLength)
        {
            ID = _idCounter++;
            Random rand = new Random();
            MovementSpeed = 5 + rand.NextDouble() * 10;
            Position = position;
            TickLength = tickLength;
        }

       
        public void ConditionalMove()
        { // Person should be removed from event thingy when evacuated
            if (firstRun)
            {
                _target = PathList[stepsTaken];
                ResetTickConditions();
                firstRun = false;

            }
            if (ticksSpentWaiting == ticksToWaitBeforeNextMove)
            {
                Move();
                ResetTickConditions();
            }
            else
            {
                ticksSpentWaiting++;
            }
        }

        private void Move()
        {
            if (PathList.Count == stepsTaken)
            {
                if (OnExtendedPathRequest != null)
                {
                    PathList.AddRange(OnExtendedPathRequest.Invoke(this));
                }
                else
                {
                    throw new PersonException($"Could not get a path or extended path for person {ID}.");
                }
                      
            }
            try
            {
                // Clear old Tile and increment heatMapCounter
                Position.Type = Tile.Types.Free;                //<--- Skal være default type for den individuelle buildingBlock
                ((BuildingBlock)Position).HeatmapCounter++;

                // Move to new tile and check if evacuated. If not, keep going.
                Position = _target;
                stepsTaken++;
                OnPersonMoved?.Invoke(this);
                if (Position.Type == Tile.Types.Exit)
                {
                    Evacuated = true;
                }
                else
                {
                    Position.Type = Tile.Types.Person;

                    // Set new target
                    _target = PathList[stepsTaken];
                }
                
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new PersonException($"Person with id: {ID} could not find a path out."); // Check if this can ever happen due to catch above ^
            }
        }

        private void ResetTickConditions()
        {
            ticksSpentWaiting = 0;
            ticksToWaitBeforeNextMove = (int)Math.Round(Position.DistanceTo(_target) / MovementSpeed / TickLength);
        }
    }
}