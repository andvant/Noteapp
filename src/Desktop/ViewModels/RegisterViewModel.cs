using Noteapp.Desktop.MVVM;
using Noteapp.Desktop.Networking;
using System.Windows;
using System.Windows.Input;

namespace Noteapp.Desktop.ViewModels
{
    public class RegisterViewModel : NotifyPropertyChanged, IPageViewModel
    {
        public string Name => PageNames.Register;

        private readonly ApiService _apiService;

        public string Email { get; set; }
        public string Password { get; set; }

        public ICommand RegisterCommand { get; }

        public RegisterViewModel(ApiService apiService)
        {
            _apiService = apiService;
            RegisterCommand = new RelayCommand(RegisterCommandExecute);
        }

        private async void RegisterCommandExecute(object parameter)
        {
            await _apiService.Register(Email, Password);
            MessageBox.Show("Successfully registered.");
        }
    }
}
