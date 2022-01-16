using Noteapp.Desktop.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Noteapp.Desktop.Converters
{
    public class SyncStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SyncStatus status = (SyncStatus)value;
            return status switch
            {
                SyncStatus.Synchronizing => "Synchronizing...",
                SyncStatus.Synchronized => "All notes are synchronized",
                SyncStatus.NotSynchronized => "Synchronization failed (changes saved locally)",
                _ => "ERROR"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
