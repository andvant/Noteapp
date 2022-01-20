using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Noteapp.Desktop.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object visible, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
