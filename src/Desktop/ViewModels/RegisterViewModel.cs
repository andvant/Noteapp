using Noteapp.Desktop.MVVM;
using Noteapp.Desktop.Networking;
using System.Windows.Input;

namespace Noteapp.Desktop.ViewModels
{
    public class RegisterViewModel : NotifyPropertyChanged, IPage
    {
        public string Name => PageNames.Register;

        private readonly ApiService _apiService;

        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        private string _registerResult;
        public string OutputMessage
        {
            get => _registerResult;
            set => Set(ref _registerResult, value);
        }

        public ICommand RegisterCommand { get; }

        public RegisterViewModel(ApiService apiService)
        {
            _apiService = apiService;
            RegisterCommand = new RelayCommand(Register);
        }

        private async void Register()
        {
            OutputMessage = string.Empty;
            var result = await _apiService.Register(Email, Password);
            if (result.IsSuccess)
            {
                OutputMessage = "Successfully registered";
            }
            else
            {
                OutputMessage = $"Failed to register: {result.ErrorMessage}";
            }
        }

        public void RefreshPage()
        {
            OutputMessage = string.Empty;
        }
    }
}
