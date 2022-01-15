using System;
using System.Globalization;
using System.Windows.Data;

namespace Noteapp.Desktop.Converters
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var date = (DateTime?)value;
            return date.HasValue ? date?.ToLocalTime().ToString("f") : "-";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
