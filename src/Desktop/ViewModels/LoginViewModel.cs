using Noteapp.Desktop.LocalData;
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

        public ICommand LoginCommand { get; }

        public LoginViewModel(ApiService apiService)
        {
            _apiService = apiService;
            LoginCommand = new RelayCommand(Login);
        }

        private async void Login()
        {
            var userInfoResponse = await _apiService.Login(Email, Password);
            if (userInfoResponse != null)
            {
                var encryptionKey = Protector.GenerateEncryptionKey(Password, userInfoResponse.encryption_salt);
                await LocalDataManager.CreateAndSaveUserInfo(userInfoResponse, encryptionKey, StaySignedIn);
                MessageBox.Show("Successfully logged in.");
            }
        }
    }
}

