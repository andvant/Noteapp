using Noteapp.Desktop.MVVM;
using Noteapp.Desktop.Networking;
using Noteapp.Desktop.Security;
using Noteapp.Desktop.Session;
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

        public ICommand LoginCommand { get; }

        public LoginViewModel(ApiService apiService)
        {
            _apiService = apiService;
            LoginCommand = new RelayCommand(Login);
        }

        private async void Login()
        {
            var userInfoDto = await _apiService.Login(Email, Password);

            var encryptionkey = Protector.GenerateEncryptionKey(Password, userInfoDto.encryption_salt);
            await SessionManager.SaveUserInfo(userInfoDto, encryptionkey);
            MessageBox.Show("Successfully logged in.");
        }
    }
}

