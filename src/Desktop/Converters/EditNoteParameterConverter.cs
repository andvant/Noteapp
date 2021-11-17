using Noteapp.Desktop.Parameters;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Noteapp.Desktop.Converters
{
    public class EditNoteParameterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return new EditNoteParameter { NoteId = (int)values[0], Text = (string)values[1] };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
