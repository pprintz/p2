namespace Evacuation_Master_3000 {
    public struct PersonInformation {
        public int ID { get; set; }
        public string Position { get; }
        public double MovementSpeed { get; set; }
    }

    class BuildingInformationCollection {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Floors { get; }
        public string Description { get; set; }
        private int _numberOfPeople;
        public int NumberOfPeople {
            get { return _numberOfPeople; }
            set {
                _numberOfPeople = value;
                PeopleCollection = new PersonInformation[NumberOfPeople];
            }
        }
        public PersonInformation[] PeopleCollection { get; private set; }
        public FloorInformation[] FloorCollection { get; private set; }
    }
}
