using Noteapp.Core.Interfaces;
using Noteapp.Desktop.MVVM;
using System.Windows.Input;

namespace Noteapp.Desktop.ViewModels
{
    public class RegisterViewModel : NotifyPropertyChanged, IPageViewModel
    {
        public string Name => PageNames.Register;

        private readonly IApiCaller _apiCaller;

        public string Email { get; set; }
        public string Password { get; set; }

        public ICommand RegisterCommand { get; private set; }

        public RegisterViewModel(IApiCaller apiCaller)
        {
            _apiCaller = apiCaller;
            RegisterCommand = new RelayCommand(RegisterCommandExecute);
        }

        private async void RegisterCommandExecute(object parameter)
        {
            await _apiCaller.Register(Email, Password);
        }
    }
}
