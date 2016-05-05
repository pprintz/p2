namespace Evacuation_Master_3000
{
    public interface IEvacuateable
    {
        int ID { get; }
        void ConditionalMove();
        Tile Position { get; set; } // Should be private set
        bool Evacuated { get; set; } // Should be private set
        int CurrentRoom { get; set; }
        int AmountOfTicksSpent { get; set; }
        }
}