namespace Evacuation_Master_3000
{
    public interface IEvacuateable
    {
        int ID { get; }
        void ConditionalMove();
        Tile Position { get; } // Should be private set
        bool Evacuated { get; } // Should be private set
    }
}