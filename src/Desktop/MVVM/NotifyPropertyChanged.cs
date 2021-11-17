using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Noteapp.Desktop.MVVM
{
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // might return true if field was updated and false otherwise
        protected void Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return; 

            field = value;

            OnPropertyChanged(propertyName);
        }
    }
}
