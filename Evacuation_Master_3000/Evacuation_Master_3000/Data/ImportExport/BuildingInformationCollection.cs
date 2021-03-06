﻿namespace Evacuation_Master_3000 {
    public struct PersonInformation {
        public int ID { get; set; }
        public string Position { get; set; }
        public double MovementSpeed { get; set; }
    }

    internal class BuildingInformationCollection {
        public int Width { get; set; }
        public int Height { get; set; }
        private int _floors;
        public PersonInformation[] PeopleCollection { get; private set; }
        public FloorInformation[] FloorCollection { get; private set; }
        public int Floors {
            get { return _floors; }
            set {
                _floors = value;
                FloorCollection = new FloorInformation[Floors];
            }
        }
        public string Description { get; set; }
        private int _numberOfPeople;
        public int NumberOfPeople {
            get { return _numberOfPeople; }
            set {
                _numberOfPeople = value;
                PeopleCollection = new PersonInformation[NumberOfPeople];
            }
        }
 
    }
}
