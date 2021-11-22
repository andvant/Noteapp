using Noteapp.Core.Interfaces;
using Noteapp.Desktop.MVVM;
using System.Windows.Input;

namespace Noteapp.Desktop.ViewModels
{
    public class LoginViewModel : NotifyPropertyChanged, IPageViewModel
    {
        public string Name => "Login";

        private readonly IApiCaller _apiCaller;

        public string Email { get; set; }
        public string Password { get; set; }

        public ICommand LoginCommand { get; private set; }
        public ICommand LogoutCommand { get; private set; }
        public ICommand CheckAuthCommand { get; private set; }

        public LoginViewModel(IApiCaller apiCaller)
        {
            _apiCaller = apiCaller;
            LoginCommand = new RelayCommand(LoginCommandExecute);
        }

        private async void LoginCommandExecute(object parameter)
        {
            await _apiCaller.Login(Email, Password);
        }
    }
}
