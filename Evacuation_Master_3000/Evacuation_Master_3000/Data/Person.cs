using System;
using System.Collections.Generic;

namespace Evacuation_Master_3000
{
    public class Person : IEvacuateable
    {
        // Remember old path unless changes has been made <-- needs to be implemented
        private static int _idCounter = 1;
        private static List<int> _idsInUse { get; } = new List<int>();
        public readonly List<BuildingBlock> PathList = new List<BuildingBlock>();
        public double MovementSpeed { get; }
        public double MovementSpeedInMetersPerSecond { get; set; }
        private int ticksToWaitBeforeNextMove;
        private int ticksSpentWaiting;
        public int NumberOfBlocks { get; set; }
        public int stepsTaken;
        private bool firstRun = true;
        public Tile Position { get; set; }
        public Tile OriginalPosition { get; set; }
        public int CurrentRoom { get; set; }
        public int AmountOfTicksSpent { get; set; }
        private Tile _target;
        public DataSimulationStatistics PersonInteractionStats { get; set; }
        public event PersonEvacuated OnPersonEvacuated;
        public static event PersonMoved OnPersonMoved;
        private bool _evacuated;

        public bool Evacuated
        {
            get { return _evacuated; }
            set
            {
                _evacuated = value;
                if (value)
                {
                    OnPersonEvacuated?.Invoke(this);
                }
                else
                {
                    firstRun = true;
                    stepsTaken = 0;
                    Data.OnTick += ConditionalMove;
                }
            }
        }
        public int ID { get; }
        public double TickLength { get; set; }
        public event ExtendedPathRequest OnExtendedPathRequest;
        private static Random rand = new Random();
        public Person(BuildingBlock position) : this(0, 0, position) { }
        internal Person(int id, double movementSpeed, BuildingBlock position)
        {
            if (id <= 0)
            { /* Negative or zero-valued id means this is a totally new person */
                int newID;
                do
                {
                    newID = _idCounter++;
                } while (_idsInUse.Contains(newID));
                ID = newID;
            }
            else { /* non-zeroed, positive value means this is an existing person */
                if (_idsInUse.Contains(id))
                    throw new PersonException($"A user with ID {id} already exists!");
                else
                    ID = id;
            }

            PersonInteractionStats = new DataSimulationStatistics(this);
            MovementSpeed = movementSpeed < 5 ? 5 + rand.NextDouble() * 10 : movementSpeed; // Less than 5 means that it was not created.
            MovementSpeedInMetersPerSecond = (MovementSpeed * 1000) / 60 / 60;
            Position = position;
            OriginalPosition = position;
        }


        public void ConditionalMove()
        { // Person should be removed from event thingy when evacuated
            AmountOfTicksSpent++;
            if (firstRun)
            {
                _target = PathList[stepsTaken + 1];
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
                //Position.Type = Tile.Types.Free;                //<--- Skal være default type for den individuelle buildingBlock
                ((BuildingBlock)Position).HeatmapCounter++;
                if (stepsTaken + 1 < PathList.Count)
                {
                    _target = PathList[stepsTaken + 1];
                }
                if (_target.Type != Tile.Types.Person || _target.OriginalType == Tile.Types.Stair)
                {

                    // Move to new tile and check if evacuated. If not, keep going.
                    if (stepsTaken + 1 < PathList.Count)
                    {
                        PersonInteractionStats.MovementSteps.Add(new MovementStep(this, PathList[stepsTaken],
                            PathList[stepsTaken + 1])
                        { TicksAtArrival = AmountOfTicksSpent });
                        stepsTaken++;
                    }
                    Position = _target;
                    if (Position.Type == Tile.Types.Exit)
                    {
                        Evacuated = true;
                    }
                    OnPersonMoved?.Invoke(this);
                }
                else
                {
                    // Counts up the heatmapcounter for every "round" the person needs to wait before moving.
                    if (AmountOfTicksSpent % ticksToWaitBeforeNextMove == 0)
                        ((BuildingBlock)Position).HeatmapCounter++;
                    PersonInteractionStats.CountTicksBeingBlocked(ticksSpentWaiting);

                }
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new PersonException($"Person with id: {ID} could not find a path out.    {stepsTaken} {PathList.Count}"); // Check if this can ever happen due to catch above ^
            }
        }
        private void ResetTickConditions()
        {
            ticksSpentWaiting = 0;
            ticksToWaitBeforeNextMove = (int)Math.Round((Position.DistanceTo(_target) * GlobalVariables.BlockWidthInMeters / MovementSpeedInMetersPerSecond) * TickLength);
            if (stepsTaken > 0 && stepsTaken != PathList.Count - 1)
            {
                ticksToWaitBeforeNextMove = (int)Math.Round((Position.DistanceTo(PathList[stepsTaken + 1]) * GlobalVariables.BlockWidthInMeters / MovementSpeedInMetersPerSecond) * TickLength);
            }
        }
    }
}