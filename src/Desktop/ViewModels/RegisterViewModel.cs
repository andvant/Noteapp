using Noteapp.Desktop.MVVM;
using Noteapp.Desktop.Networking;
using System.Windows;
using System.Windows.Input;

namespace Noteapp.Desktop.ViewModels
{
    public class RegisterViewModel : NotifyPropertyChanged, IPage
    {
        public string Name => PageNames.Register;

        private readonly ApiService _apiService;

        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public ICommand RegisterCommand { get; }

        public RegisterViewModel(ApiService apiService)
        {
            _apiService = apiService;
            RegisterCommand = new RelayCommand(Register);
        }

        private async void Register()
        {
            await _apiService.Register(Email, Password);
            MessageBox.Show("Successfully registered.");
        }
    }
}
