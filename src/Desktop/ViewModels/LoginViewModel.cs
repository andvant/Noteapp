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

        public string Email { get; set; }
        public string Password { get; set; }

        public ICommand LoginCommand { get; }

        public LoginViewModel(ApiService apiService)
        {
            _apiService = apiService;
            LoginCommand = new RelayCommand(LoginCommandExecute);
        }

        private async void LoginCommandExecute()
        {
            var userInfoDto = await _apiService.Login(Email, Password);

            _apiService.AccessToken = userInfoDto.access_token;
            var encryptionkey = Protector.GenerateEncryptionKey(Password, userInfoDto.encryption_salt);
            await SessionManager.SaveUserInfo(userInfoDto, encryptionkey);
            MessageBox.Show("Successfully logged in.");
        }
    }
}

