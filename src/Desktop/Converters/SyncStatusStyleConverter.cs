using Noteapp.Desktop.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Noteapp.Desktop.Converters
{
    public class SyncStatusStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SyncStatus status = (SyncStatus)value;
            var synchronizingStyle = Application.Current.TryFindResource("SyncStatusSynchronizing") as Style;
            var synchronizedStyle = Application.Current.TryFindResource("SyncStatusSynchronized") as Style;
            var notSynchronizedStyle = Application.Current.TryFindResource("SyncStatusNotSynchronized") as Style;

            return status switch
            {
                SyncStatus.Synchronizing => synchronizingStyle,
                SyncStatus.Synchronized => synchronizedStyle,
                SyncStatus.NotSynchronized => notSynchronizedStyle,
                _ => "ERROR"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
