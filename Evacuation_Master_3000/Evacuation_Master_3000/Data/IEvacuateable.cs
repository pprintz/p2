namespace Evacuation_Master_3000
{
    public interface IEvacuateable
    {
        int ID { get; }
        void ConditionalMove();
        Tile Position { get; set; }
        Tile OriginalPosition { get; set; }
        bool Evacuated { get; set; }
        int CurrentRoom { get; set; }
        int AmountOfTicksSpent { get; set; }
        }
}