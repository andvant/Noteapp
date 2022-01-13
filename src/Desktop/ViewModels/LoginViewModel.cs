using Noteapp.Desktop.Data;
using Noteapp.Desktop.MVVM;
using Noteapp.Desktop.Networking;
using Noteapp.Desktop.Security;
using System.Windows;
using System.Windows.Input;

namespace Noteapp.Desktop.ViewModels
{
    public class LoginViewModel : NotifyPropertyChanged, IPage
    {
        public string Name => PageNames.Login;

        private readonly ApiService _apiService;

        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool StaySignedIn { get; set; } = true;

        private string _loginResult;
        public string OutputMessage
        {
            get => _loginResult;
            set => Set(ref _loginResult, value);
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel(ApiService apiService)
        {
            _apiService = apiService;
            LoginCommand = new RelayCommand(Login);
        }

        private async void Login()
        {
            OutputMessage = string.Empty;
            var userInfoResponse = await _apiService.Login(Email, Password);
            if (userInfoResponse != null)
            {
                var encryptionKey = Protector.GenerateEncryptionKey(Password, userInfoResponse.encryption_salt);
                await AppData.CreateAndSaveUserInfo(userInfoResponse, encryptionKey, StaySignedIn);
                OutputMessage = "Successfully logged in";
            }
            else
            {
                OutputMessage = "Failed to log in";
            }
        }

        public void RefreshPage()
        {
            OutputMessage = string.Empty;
        }
    }
}

