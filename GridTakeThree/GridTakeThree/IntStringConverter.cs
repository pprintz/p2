using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;

namespace GridTakeThree {
    public class IntStringConverter : IValueConverter {
        public int EmptyStringValue { get; set; }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == null)
                return null;
            else if (value is string)
                return value;
            else if (value is int && (int)value == EmptyStringValue)
                return string.Empty;
            else
                return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value is string) {
                string s = (string)value;
                int number;
                if (int.TryParse(s, out number))
                    return number;
                else
                    return EmptyStringValue;
            }
            return value;
        }
    }
}
