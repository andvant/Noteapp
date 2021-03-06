using Noteapp.Desktop.MVVM;
using Noteapp.Desktop.Networking;
using System;
using System.Windows.Input;

namespace Noteapp.Desktop.ViewModels
{
    public class RegisterViewModel : NotifyPropertyChanged, IPage
    {
        public string Name => PageNames.Register;

        private readonly ApiService _apiService;

        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        private string _outputMessage;
        public string OutputMessage
        {
            get => _outputMessage;
            set => Set(ref _outputMessage, value);
        }

        public ICommand RegisterCommand { get; }

        public RegisterViewModel(ApiService apiService)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
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
            Email = string.Empty;
            Password = string.Empty;
        }
    }
}
