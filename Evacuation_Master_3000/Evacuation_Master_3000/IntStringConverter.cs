#region

using System;
using System.Windows.Data;

#endregion

namespace Evacuation_Master_3000
{
    public class IntStringConverter : IValueConverter
    {
        public int EmptyStringValue { get; set; }

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
            if (value is string)
            {
                string s = (string) value;
                int number;
                if (int.TryParse(s, out number))
                    return number;
                return EmptyStringValue;
            }
            return value;
        }
    }
}