using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            Data.OnTick += UpdateTicks;
        }


        private int AmountOfEvacuatedPeople;
        private int TotalPeople;
        List<Person> PeopleList = new List<Person>();
        List<Person> EvacuatedPeopleList = new List<Person>();
        private int ticks;
        private double fillWidthPerPerson;
        int red, green, blue;
        private void UpdateTicks()
        {
            ticks++;
            TimeElapsedInDateTimeFormat.Text = ticks + " Ticks";

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
                    AmountOfEvacuatedPeople++;
                    EvacuatedPeopleList.Add(person);
                    int multiplier = (int)Math.Round(255f / PeopleList.Count);
                    int count = multiplier * EvacuatedPeopleList.Count;
                    PersonsEvacuatedProgressBarFill.Fill = BarColor(count);
                    PersonsEvacuatedProgressBarFill.Width += fillWidthPerPerson;
                    CurrentNumberOfEvacuatedPersons.Text = EvacuatedPeopleList.Count + "";
                    double percentageEvacuated = ((double)EvacuatedPeopleList.Count) / ((double)PeopleList.Count) * 100;
                    PersonsEvacuatedProgressBarText.Text = Math.Round(percentageEvacuated, 2) + "%";
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
