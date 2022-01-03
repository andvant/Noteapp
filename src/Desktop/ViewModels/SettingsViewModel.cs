using Noteapp.Desktop.LocalData;
using Noteapp.Desktop.MVVM;
using Noteapp.Desktop.Networking;
using System.Windows;
using System.Windows.Input;

namespace Noteapp.Desktop.ViewModels
{
    public class SettingsViewModel : NotifyPropertyChanged, IPage
    {
        public string Name => PageNames.Settings;

        private readonly ApiService _apiService;

        public string Email => LocalDataManager.GetUserInfo()?.Email ?? "Anonymous";
        public bool EncryptionEnabled => LocalDataManager.GetUserInfo()?.EncryptionEnabled ?? false;

        public ICommand LogoutCommand { get; }
        public ICommand DeleteAccountCommand { get; }
        public ICommand ToggleEncryptionCommand { get; }

        public SettingsViewModel(ApiService apiService)
        {
            _apiService = apiService;
            LogoutCommand = new RelayCommand(Logout);
            DeleteAccountCommand = new RelayCommand(DeleteAccount);
            ToggleEncryptionCommand = new RelayCommand(ToggleEncryption);
        }

        private void Logout()
        {
            LocalDataManager.DeleteUserInfo();
            LocalDataManager.DeleteNotes();
            MessageBox.Show("Successfully logged out.");
            OnPropertyChanged(nameof(Email));
            OnPropertyChanged(nameof(EncryptionEnabled));
        }

        private async void DeleteAccount()
        {
            if (await _apiService.DeleteAccount())
            {
                MessageBox.Show("Account successfully deleted.");
                Logout();
            }
        }

        private async void ToggleEncryption()
        {
            var userInfo = LocalDataManager.GetUserInfo();
            if (userInfo is null)
            {
                MessageBox.Show("You have to be logged in to enable encryption!");
                return;
            }
            userInfo.EncryptionEnabled = !userInfo.EncryptionEnabled;
            await LocalDataManager.SaveUserInfo(userInfo);
        }
    }
}
