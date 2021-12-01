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

        public string Email => SessionManager.GetUserInfo()?.Email ?? "Anonymous";
        public bool EncryptionEnabled
        {
            get => SessionManager.GetUserInfo()?.EncryptionEnabled ?? false;
            set => ToggleEncryption();
        }

        public ICommand LogoutCommand { get; }
        public ICommand DeleteAccountCommand { get; }

        public SettingsViewModel(ApiService apiService)
        {
            _apiService = apiService;
            LogoutCommand = new RelayCommand(Logout);
            DeleteAccountCommand = new RelayCommand(DeleteAccount);
        }

        private void Logout()
        {
            SessionManager.DeleteUserInfo();
            MessageBox.Show("Successfully logged out.");
            OnPropertyChanged(nameof(Email));
            OnPropertyChanged(nameof(EncryptionEnabled));
        }

        private async void DeleteAccount()
        {
            await _apiService.DeleteAccount();
            MessageBox.Show("Account successfully deleted.");
            LogoutCommand.Execute(null);
        }

        private void ToggleEncryption()
        {
            var userInfo = SessionManager.GetUserInfo();
            if (userInfo is null)
            {
                MessageBox.Show("Have to be logged in to enable encryption.");
                return;
            }
            userInfo.EncryptionEnabled = !userInfo.EncryptionEnabled;
            SessionManager.SaveUserInfo(userInfo).RunSynchronously();
        }
    }
}
