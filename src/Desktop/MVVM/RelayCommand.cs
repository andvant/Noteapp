using System;
using System.Windows.Input;

namespace Noteapp.Desktop.MVVM
{
    public class RelayCommand : ICommand
    {
        private Action<object> _execute;
        private Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            if (execute is null) throw new ArgumentException(nameof(execute));

            _execute = execute;
            _canExecute = canExecute;
        }

        public RelayCommand(Action execute)
        {
            if (execute is null) throw new ArgumentException(nameof(execute));

            _execute = _ => execute();
            _canExecute = _ => true;
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute is null) throw new ArgumentException(nameof(execute));

            _execute = _ => execute();
            _canExecute = _ => canExecute();
        }


        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute is null || _canExecute(parameter);
        }
    }
}
