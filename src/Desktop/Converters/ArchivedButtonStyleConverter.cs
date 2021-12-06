using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Noteapp.Desktop.Converters
{
    internal class ArchivedButtonStyleConverter : IValueConverter
    {
        public object Convert(object showArchived, Type targetType, object parameter, CultureInfo culture)
        {
            var archivedButtonNormalStyle = Application.Current.TryFindResource("ArchivedButtonNormal") as Style;
            var archivedButtonPressedStyle = Application.Current.TryFindResource("ArchivedButtonPressed") as Style;
            return (bool)showArchived ? archivedButtonPressedStyle : archivedButtonNormalStyle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
