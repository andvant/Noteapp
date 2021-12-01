using Noteapp.Desktop.MVVM;
using Noteapp.Desktop.Networking;
using Noteapp.Desktop.Session;
using System.Windows;
using System.Windows.Input;

namespace Noteapp.Desktop.ViewModels
{
    public class SettingsViewModel : NotifyPropertyChanged, IPage
    {
        public string Name => PageNames.Settings;

        private readonly ApiService _apiService;

        public string Email => SessionManager.GetUserInfo()?.Result?.Email ?? "Anonymous";
        public ICommand LogoutCommand { get; }
        public ICommand DeleteAccountCommand { get; }

        public SettingsViewModel(ApiService apiService)
        {
            _apiService = apiService;
            LogoutCommand = new RelayCommand(LogoutCommandExecute);
            DeleteAccountCommand = new RelayCommand(DeleteAccountCommandExecute);
        }

        private void LogoutCommandExecute(object parameter)
        {
            _apiService.AccessToken = null;
            SessionManager.DeleteUserInfo();
            MessageBox.Show("Successfully logged out.");
            OnPropertyChanged(nameof(Email));
        }

        private async void DeleteAccountCommandExecute(object parameter)
        {
            await _apiService.DeleteAccount();
            MessageBox.Show("Account successfully deleted.");
            LogoutCommand.Execute(null);
        }
    }
}

