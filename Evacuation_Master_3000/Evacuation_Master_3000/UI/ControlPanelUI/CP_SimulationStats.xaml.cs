using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
        public CP_SimulationStats(MainWindow parentWindow)
        {
            InitializeComponent();
            SetupStats();
            Person.OnPersonMoved += UpdateSimulationStats;
            Data.OnTick += UpdateTicksAndTime;
            UserInterface.OnReset += ResetPeopleAndSimulationInformation;
            _parentWindow = parentWindow;
        }

        private MainWindow _parentWindow;

        private void ResetPeopleAndSimulationInformation()
        {
            foreach (Person person in _parentWindow.TheUserInterface.LocalPeopleDictionary.Values)
            {
                if (person.OriginalPosition != person.Position)
                {
                    person.Position.Type = person.Position.OriginalType;
                    person.Position = person.OriginalPosition;
                    person.Position.Type = person.Position.OriginalType;
                }
                person.AmountOfTicksSpent = 0;
                person.PersonInteractionStats.DistanceTraveled = 0;
                person.PersonInteractionStats.TicksWaited = 0;
                if (person.Evacuated)
                {
                    person.Evacuated = false;
                    person.PathList.Clear();
                    Console.WriteLine(person.PersonInteractionStats.MovementSteps.Count);
                    foreach (MovementStep movementStep in person.PersonInteractionStats.MovementSteps)
                    {
                        person.PathList.Add(movementStep.SourceTile as BuildingBlock);
                    }
                    person.PathList.Add(person.PersonInteractionStats.MovementSteps.Last().DestinationTile as BuildingBlock);
                }
                if (person.PathList.First() != person.OriginalPosition)
                {
                    person.PathList.Clear();
                }
                person.stepsTaken = 0;
                person.PersonInteractionStats.MovementSteps.Clear();
            }
            ticks = -1;
            EvacuatedPeopleList.Clear();
            UpdateTicksAndTime();
            PersonsEvacuatedProgressBarFill.Width = 0;
            PersonsEvacuatedProgressBarText.Text = "0%";
            CurrentNumberOfEvacuatedPersons.Text = "0";
        }


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
            int peopleCount = _parentWindow.TheUserInterface.LocalPeopleDictionary.Count;
            TotalPersonCount.Text = peopleCount + "";
            fillWidthPerPerson = (PersonsEvacuatedProgressBarBackground.ActualWidth) / peopleCount;
            if (person.Evacuated)
            {
                if (!EvacuatedPeopleList.Contains(person))
                {
                    EvacuatedPeopleList.Add(person);
                    int multiplier = (int)Math.Round(255f / peopleCount);
                    int count = multiplier * EvacuatedPeopleList.Count;
                    PersonsEvacuatedProgressBarFill.Fill = BarColor(count);
                    PersonsEvacuatedProgressBarFill.Width = fillWidthPerPerson * EvacuatedPeopleList.Count;
                    CurrentNumberOfEvacuatedPersons.Text = EvacuatedPeopleList.Count + "";
                    double percentageEvacuated = ((double)EvacuatedPeopleList.Count) / peopleCount * 100;
                    PersonsEvacuatedProgressBarText.Text = Math.Round(percentageEvacuated, 2) + "%";
                    person.PersonInteractionStats.TimeWhenEvacuated = Math.Round((double)ticks / 100, 2);
                }
                if (peopleCount == EvacuatedPeopleList.Count)
                {
                    UserInterface.HasSimulationEnded = true;
                    StringBuilder sb = new StringBuilder();
                    //UserInterface.HasSimulationEnded = true;
                    sb.AppendLine($"Statistics for simulation run at {DateTime.Now} by {Environment.UserName}{Environment.NewLine}");
                    double timeElapsed = EvacuatedPeopleList.Max(p => p.PersonInteractionStats.TimeWhenEvacuated);
                    sb.AppendLine($"Time elapsed in simulation: {timeElapsed} seconds");
                    sb.AppendLine($"Amount of people: {EvacuatedPeopleList.Count}{Environment.NewLine}");
                    double quarterOfTimeElapsed = timeElapsed * 0.25;
                    double halfOfTimeElapsed = timeElapsed * 0.50;
                    double threeQuarterOfTimeElapsed = timeElapsed * 0.75;
                    sb.AppendLine(
                        $"Amount of people evacuated at {quarterOfTimeElapsed} seconds: " +
                        $"{Math.Round((double)EvacuatedPeopleList.Count(p => p.PersonInteractionStats.TimeWhenEvacuated <= quarterOfTimeElapsed) / (double)EvacuatedPeopleList.Count * 100, 2)}%");
                    sb.AppendLine(
                        $"Amount of people evacuated at {halfOfTimeElapsed} seconds: " +
                        $"{Math.Round((double)EvacuatedPeopleList.Count(p => p.PersonInteractionStats.TimeWhenEvacuated <= halfOfTimeElapsed) / (double)EvacuatedPeopleList.Count * 100, 2)}%");
                    sb.AppendLine(
                        $"Amount of people evacuated at {threeQuarterOfTimeElapsed} seconds: " +
                        $"{Math.Round((double)EvacuatedPeopleList.Count(p => p.PersonInteractionStats.TimeWhenEvacuated <= threeQuarterOfTimeElapsed) / (double)EvacuatedPeopleList.Count * 100, 2)}%");
                    sb.AppendLine(
                        $"Amount of people evacuated at {timeElapsed} seconds: " +
                        $"{EvacuatedPeopleList.Count(p => p.PersonInteractionStats.TimeWhenEvacuated <= timeElapsed) / EvacuatedPeopleList.Count * 100}%");

                    double averageTime = EvacuatedPeopleList.Sum(p => p.PersonInteractionStats.TimeWhenEvacuated) /
                                         EvacuatedPeopleList.Count;
                    sb.AppendLine($"Average evacuation time:{Math.Round(averageTime, 2)}");

                    double[] timearray = EvacuatedPeopleList.Select(p => p.PersonInteractionStats.TimeWhenEvacuated).ToArray();
                    var sortedTimearray = timearray.OrderBy(t => t);
                    var median = sortedTimearray.ElementAt(sortedTimearray.Count() / 2);

                    sb.AppendLine($"{Environment.NewLine}Evacuated before {median} seconds:");
                    foreach (Person person1 in EvacuatedPeopleList.Where(p => p.PersonInteractionStats.TimeWhenEvacuated < median))
                    {
                        sb.AppendLine($"Person: {person1.ID}    Time:{person1.PersonInteractionStats.TimeWhenEvacuated} seconds");
                    }

                    sb.AppendLine($"{Environment.NewLine}Evacuated after or at {median} seconds:");
                    foreach (Person person1 in EvacuatedPeopleList.Where(p => p.PersonInteractionStats.TimeWhenEvacuated >= median))
                    {
                        sb.AppendLine($"Person: {person1.ID}    Time:{person1.PersonInteractionStats.TimeWhenEvacuated} seconds");
                    }

                    Person personWithLongestDistance = EvacuatedPeopleList.OrderByDescending(p => p.PersonInteractionStats.DistanceTraveled).First();
                    Person personWithLongestTimeBeforeEvacuated = EvacuatedPeopleList.OrderByDescending(p => p.PersonInteractionStats.TimeWhenEvacuated).First();
                    sb.AppendLine($"{Environment.NewLine}Longest distance:{Environment.NewLine}" +
                                  $"{personWithLongestDistance.ID} - {personWithLongestDistance.PersonInteractionStats.DistanceTraveled} m{Environment.NewLine}");
                    sb.AppendLine($"Most time before evacuated: {Environment.NewLine}" +
                                  $"{personWithLongestTimeBeforeEvacuated.ID} - {personWithLongestDistance.PersonInteractionStats.TimeWhenEvacuated} seconds{Environment.NewLine}");

                    foreach (Person person1 in EvacuatedPeopleList.OrderBy(p => p.ID))
                    {
                        sb.AppendLine(
                            "_____________________________________________________________________________________________________________");
                        int movementStepCounter = 0;
                        sb.AppendLine(
                            $"Person: {person1.ID}, Position{ImportExportSettings.Coordinate(person.OriginalPosition)}{Environment.NewLine}" +
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
                    }
                    string stringToWrite = sb.ToString();
                    Directory.CreateDirectory(Environment.CurrentDirectory + @"\Statistics");
                    string nameOfStatsFile = $"{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}.txt";
                    string path = Environment.CurrentDirectory + @"\Statistics\" + nameOfStatsFile;
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
