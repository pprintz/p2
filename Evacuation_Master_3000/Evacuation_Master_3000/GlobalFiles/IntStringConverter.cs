using System;
using System.Windows.Data;

namespace Evacuation_Master_3000
{
    public class IntStringConverter : IValueConverter
    {
        public int EmptyStringValue { private get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            if (value is string)
                return value;
            if (value is int && (int) value == EmptyStringValue)
                return string.Empty;
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            string s1 = value as string;
            if (s1 != null)
            {
                string s = s1;
                int number;
                return int.TryParse(s, out number) ? number : EmptyStringValue;
            }
            return value;
        }
    }
}