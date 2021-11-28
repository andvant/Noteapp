using Noteapp.Desktop.MVVM;
using Noteapp.Desktop.Networking;
using Noteapp.Desktop.Security;
using Noteapp.Desktop.Session;
using System.Windows;
using System.Windows.Input;

namespace Noteapp.Desktop.ViewModels
{
    public class LoginViewModel : NotifyPropertyChanged, IPageViewModel
    {
        public string Name => PageNames.Login;

        private readonly ApiCaller _apiCaller;
        private readonly SessionManager _sessionManager;

        public string Email { get; set; }
        public string Password { get; set; }

        public ICommand LoginCommand { get; }
        public ICommand LogoutCommand { get; }

        public LoginViewModel(ApiCaller apiCaller, SessionManager sessionManager)
        {
            _apiCaller = apiCaller;
            _sessionManager = sessionManager;
            LoginCommand = new RelayCommand(LoginCommandExecute);
            LogoutCommand = new RelayCommand(LogoutCommandExecute);
        }

        private async void LoginCommandExecute(object parameter)
        {
            var userInfoDto = await _apiCaller.Login(Email, Password);

            _apiCaller.AccessToken = userInfoDto.access_token;
            var encryptionkey = Protector.GenerateEncryptionKey(Password, userInfoDto.encryption_salt);
            await _sessionManager.SaveUserInfo(userInfoDto, encryptionkey);
            MessageBox.Show("Successfully logged in.");
        }

        private void LogoutCommandExecute(object parameter)
        {
            _apiCaller.AccessToken = null;
            _sessionManager.DeleteUserInfo();
            MessageBox.Show("Successfully logged out.");
        }
    }
}

