using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace Evacuation_Master_3000
{
    /// <summary>
    /// Interaction logic for CP_SimulationStats.xaml
    /// </summary>
    public partial class CP_SimulationStats : UserControl
    {
        public CP_SimulationStats()
        {
            InitializeComponent();
            SetupStats();
            Person.OnPersonMoved += UpdateSimulationStats;
            Data.OnTick += UpdateTicksAndTime;
            UserInterface.OnReset += ResetPeopleAndSimulationInformation;

        }

        private void ResetPeopleAndSimulationInformation()
        {
            foreach (Person person in PeopleList)
            {
                person.Position.Type = Tile.Types.Free;
                person.Position = person.OriginalPosition;
                person.Position.Type = person.Position.OriginalType;
                person.AmountOfTicksSpent = 0;
                person.PersonInteractionStats.DistanceTraveled = 0;
                person.PersonInteractionStats.MovementSteps.Clear();
                person.PersonInteractionStats.TicksWaited = 0;
                if (person.Evacuated)
                {
                    person.Evacuated = false;
                }
                person.stepsTaken = 0;
            }
            ticks = -1;
            EvacuatedPeopleList.Clear();
            UpdateTicksAndTime();
            PersonsEvacuatedProgressBarFill.Width = 0;
            PersonsEvacuatedProgressBarText.Text = "0%";
            CurrentNumberOfEvacuatedPersons.Text = "0";
        }


        List<Person> PeopleList = new List<Person>();
        List<Person> EvacuatedPeopleList = new List<Person>();
        private int ticks;
        private double fillWidthPerPerson;

        private void UpdateTicksAndTime()
        {
            ticks++;
            TimeElapsedInDateTimeFormat.Text = Math.Round((double)ticks / 100, 2) + " Seconds";
            TicksElapsed.Text = ticks + " Ticks";
        }
        private void UpdateSimulationStats(Person person)
        {
            if (!PeopleList.Contains(person))
            {
                PeopleList.Add(person);
                TotalPersonCount.Text = PeopleList.Count + "";
                fillWidthPerPerson = (PersonsEvacuatedProgressBarBackground.ActualWidth) / PeopleList.Count;
            }
            if (person.Evacuated)
            {
                if (!EvacuatedPeopleList.Contains(person))
                {
                    EvacuatedPeopleList.Add(person);
                    int multiplier = (int)Math.Round(255f / PeopleList.Count);
                    int count = multiplier * EvacuatedPeopleList.Count;
                    PersonsEvacuatedProgressBarFill.Fill = BarColor(count);
                    PersonsEvacuatedProgressBarFill.Width = fillWidthPerPerson * EvacuatedPeopleList.Count;
                    CurrentNumberOfEvacuatedPersons.Text = EvacuatedPeopleList.Count + "";
                    double percentageEvacuated = ((double)EvacuatedPeopleList.Count) / PeopleList.Count * 100;
                    PersonsEvacuatedProgressBarText.Text = Math.Round(percentageEvacuated, 2) + "%";
                    person.PersonInteractionStats.TimeWhenEvacuated = Math.Round((double)ticks / 100, 2);
                }
                if (PeopleList.Count == EvacuatedPeopleList.Count)
                {
                    StringBuilder sb = new StringBuilder();
                    //UserInterface.HasSimulationEnded = true;
                    Person personWithLongestDistance = EvacuatedPeopleList.OrderByDescending(p => p.PersonInteractionStats.DistanceTraveled).First();
                    Person personWithLongestTimeBeforeEvacuated = EvacuatedPeopleList.OrderByDescending(p => p.PersonInteractionStats.TimeWhenEvacuated).First();
                    sb.AppendLine($"Longest distance:{Environment.NewLine}" +
                                  $"{personWithLongestDistance.ID} - {personWithLongestDistance.PersonInteractionStats.DistanceTraveled} m{Environment.NewLine}");
                    sb.AppendLine($"Most time before evacuated: {Environment.NewLine}" +
                                  $"{personWithLongestTimeBeforeEvacuated.ID} - {personWithLongestDistance.PersonInteractionStats.TimeWhenEvacuated} seconds{Environment.NewLine}");

                    foreach (Person person1 in EvacuatedPeopleList.OrderBy(p => p.ID))
                    {
                        int movementStepCounter = 0;
                        sb.AppendLine(
                            $"Person: {person1.ID}, Position({ImportExportSettings.Coordinate(person.OriginalPosition)}){Environment.NewLine}" +
                            $"Steps taken: {person1.stepsTaken}{Environment.NewLine}" +
                            $"Distance traveled in meters: {person1.PersonInteractionStats.DistanceTraveled} m{Environment.NewLine}" +
                            $"Movement speed: {Math.Round(person1.MovementSpeedInMetersPerSecond, 2)} m/s{Environment.NewLine}" +
                            $"Time at evacuation: {person1.PersonInteractionStats.TimeWhenEvacuated} seconds{Environment.NewLine}" +
                            $"Time waited because of blocked path: {Math.Round((double)person1.PersonInteractionStats.TicksWaited / 100, 2)} seconds{Environment.NewLine}{Environment.NewLine}" +
                            $"List of steps:");
                        foreach (MovementStep movementStep in person1.PersonInteractionStats.MovementSteps)
                        {
                            movementStepCounter++;
                            sb.AppendLine(
                                $"Step:{movementStepCounter}   From: {ImportExportSettings.Coordinate(movementStep.SourceTile)}\n" +
                                $" - To: {ImportExportSettings.Coordinate(movementStep.DestinationTile)}\n" +
                                $"      Distance in meters: {movementStep.DistanceInMeters} m{Environment.NewLine}" +
                                $"          Time at arrival: {Math.Round((double)movementStep.TicksAtArrival / 100, 2)} seconds{Environment.NewLine}");
                        }
                        sb.AppendLine("\n\n\n");
                    }
                    string stringToWrite = sb.ToString();
                    Directory.CreateDirectory(Environment.CurrentDirectory + @"\Statistics");
                    string path = Environment.CurrentDirectory + @"\Statistics\SimulationStats.txt";
                    File.WriteAllText(path, stringToWrite);
                }
            }
        }

        private SolidColorBrush BarColor(int count)
        {
            int red, green, blue;
            if (count > 255)
            {
                count = 255;
            }
            red = 255 - count;
            green = 0 + count;
            blue = 0;
            return new SolidColorBrush(Color.FromRgb((byte)red, (byte)green, (byte)blue));
        }

        private void SetupStats()
        {

        }
    }
}
