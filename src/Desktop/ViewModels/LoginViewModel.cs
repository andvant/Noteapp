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

        public string Email { get; set; }
        public string Password { get; set; }

        public ICommand LoginCommand { get; }

        public LoginViewModel(ApiCaller apiCaller)
        {
            _apiCaller = apiCaller;
            LoginCommand = new RelayCommand(LoginCommandExecute);
        }

        private async void LoginCommandExecute(object parameter)
        {
            var userInfoDto = await _apiCaller.Login(Email, Password);

            _apiCaller.AccessToken = userInfoDto.access_token;
            var encryptionkey = Protector.GenerateEncryptionKey(Password, userInfoDto.encryption_salt);
            await SessionManager.SaveUserInfo(userInfoDto, encryptionkey);
            MessageBox.Show("Successfully logged in.");
        }
    }
}

