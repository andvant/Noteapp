using Noteapp.Core.Interfaces;
using Noteapp.Desktop.MVVM;
using Noteapp.Desktop.Session;
using System.Windows;
using System.Windows.Input;

namespace Noteapp.Desktop.ViewModels
{
    public class LoginViewModel : NotifyPropertyChanged, IPageViewModel
    {
        public string Name => PageNames.Login;

        private readonly IApiCaller _apiCaller;
        private readonly SessionManager _sessionManager;

        public string Email { get; set; }
        public string Password { get; set; }

        public ICommand LoginCommand { get; }
        public ICommand LogoutCommand { get; }

        public LoginViewModel(IApiCaller apiCaller, SessionManager sessionManager)
        {
            _apiCaller = apiCaller;
            _sessionManager = sessionManager;
            LoginCommand = new RelayCommand(LoginCommandExecute);
            LogoutCommand = new RelayCommand(LogoutCommandExecute);
        }

        private async void LoginCommandExecute(object parameter)
        {
            var userInfo = await _apiCaller.Login(Email, Password);
            _apiCaller.AccessToken = userInfo.access_token;
            await _sessionManager.SaveUserInfo(userInfo);
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

