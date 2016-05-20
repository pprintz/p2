using System;
using System.Collections.Generic;
using System.Linq;

namespace Evacuation_Master_3000
{
    public class Person : IEvacuateable
    {
        // Remember old path unless changes has been made <-- needs to be implemented
        private static int _idCounter = 1;
        private static List<int> IdsInUse { get; } = new List<int>();
        public readonly List<BuildingBlock> PathList = new List<BuildingBlock>();
        public double MovementSpeed { get; }

        public int ID { get; }
        public double SimulationSpeed { get; set; }
        public event ExtendedPathRequest OnExtendedPathRequest;
        private static readonly Random Rand = new Random();
        public double MovementSpeedInMetersPerSecond { get; }
        private int _ticksToWaitBeforeNextMove;
        private int _ticksSpentWaiting;
        public int StepsTaken;
        private int _roundsWaitedBecauseOfBlock;
        public bool NewPersonInGrid { get; set; } = true;
        private bool _firstRun = true;
        public bool NoPathAvailable { get; }
        public Tile Position { get; set; }
        public Tile OriginalPosition { get; set; }
        public int CurrentRoom { get; set; }
        public int AmountOfTicksSpent { get; set; }
        private Tile _target;
        public DataSimulationStatistics PersonInteractionStats { get; }
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
                    _firstRun = true;
                    StepsTaken = 0;
                    Data.OnTick += ConditionalMove;

                }
            }
        }
        public Person(BuildingBlock position) : this(0, 0, position) { }
        internal Person(int id, double movementSpeed, BuildingBlock position)
        {
            if (id <= 0)
            { /* Negative or zero-valued id means this is a totally new person */
                int newID;
                do
                {
                    newID = _idCounter++;
                } while (IdsInUse.Contains(newID));
                ID = newID;
            }
            else
            {
                /* non-zeroed, positive value means this is an existing person */
                if (IdsInUse.Contains(id))
                    throw new PersonException($"A user with ID {id} already exists!");
                ID = id;
            }

            PersonInteractionStats = new DataSimulationStatistics();
            MovementSpeed = movementSpeed < 5 ? 5 + Rand.NextDouble() * 10 : movementSpeed; // Less than 5 means that it was not created.
            MovementSpeedInMetersPerSecond = (MovementSpeed * 1000) / 60 / 60;
            Position = position;
            if (position.Priority == Int16.MaxValue)
            {
                NoPathAvailable = true;
            }
            OriginalPosition = position;
        }

        public bool UpdateTickCondition  = false;

        public void ConditionalMove()
        { // Person should be removed from event thingy when evacuated
            if (UpdateTickCondition)
            {
                double percentageStepDone = AmountOfTicksSpent/(double)_ticksToWaitBeforeNextMove;
                ResetTickConditions();
                AmountOfTicksSpent = (int)(_ticksToWaitBeforeNextMove*percentageStepDone);
                UpdateTickCondition = false;
            }
            AmountOfTicksSpent++;
            if (_target == null)
            {
                _target = PathList[StepsTaken + 1];
                ResetTickConditions();
                _firstRun = false;
            }
            if (_ticksSpentWaiting == _ticksToWaitBeforeNextMove)
            {
                Move();
                ResetTickConditions();
            }
            else
            {
                _ticksSpentWaiting++;
            }
        }
        private void Move()
        {
            if ((Position as BuildingBlock).Priority == Int16.MaxValue)
            {
                throw new PersonException($"Could not get a path or extended path for person {ID}.");
            }
                // Clear old Tile and increment heatMapCounter
                //Position.Type = Tile.Types.Free;                //<--- Skal være default type for den individuelle buildingBlock
                ((BuildingBlock)Position).HeatmapCounter++;
            if (StepsTaken + 1 < PathList.Count)
            {
                _target = PathList[StepsTaken + 1];
            }
            if (_target.Type != Tile.Types.Person || _target.OriginalType == Tile.Types.Stair)
            {
                // Move to new tile and check if evacuated. If not, keep going.
                if (StepsTaken + 1 < PathList.Count)
                {
                    PersonInteractionStats.MovementSteps.Add(new MovementStep(this, PathList[StepsTaken],
                        PathList[StepsTaken + 1])
                    { TicksAtArrival = AmountOfTicksSpent });
                    StepsTaken++;
                }
                Position = _target;
                if (Position.Type == Tile.Types.Exit)
                {
                    Evacuated = true;
                }
                _roundsWaitedBecauseOfBlock = 0;
                OnPersonMoved?.Invoke(this);
            }
            else
            {
                // Counts up the heatmapcounter for every "round" the person needs to wait before moving.
                ((BuildingBlock)Position).HeatmapCounter++;
                PersonInteractionStats.CountTicksBeingBlocked(_ticksSpentWaiting);
                _roundsWaitedBecauseOfBlock++;
                if (_roundsWaitedBecauseOfBlock >= 3 && _target.Type == Tile.Types.Person)
                {
                    OnExtendedPathRequest?.Invoke(this);
                    _target = PathList[StepsTaken + 1];
                    if (PathList.Count > StepsTaken + 1 && _target.Type != Tile.Types.Person)
                    {
                        PersonInteractionStats.MovementSteps.Add(new MovementStep(this, PathList[StepsTaken],
                      PathList[StepsTaken + 1])
                        { TicksAtArrival = AmountOfTicksSpent });
                        StepsTaken++;
                        Position = PathList[StepsTaken + 1];
                        _roundsWaitedBecauseOfBlock = 0;
                        OnPersonMoved?.Invoke(this);
                    }
                }
            }
        }
        private void ResetTickConditions()
        {
            _ticksSpentWaiting = 0;
            _ticksToWaitBeforeNextMove = (int)Math.Round((Position.DistanceTo(_target) * GlobalVariables.BlockWidthInMeters / MovementSpeedInMetersPerSecond) * SimulationSpeed);
            if (StepsTaken > 0 && StepsTaken != PathList.Count - 1)
            {
                _ticksToWaitBeforeNextMove = (int)Math.Round((Position.DistanceTo(PathList[StepsTaken + 1]) * GlobalVariables.BlockWidthInMeters / MovementSpeedInMetersPerSecond) * SimulationSpeed);
            }
        }
    }
}