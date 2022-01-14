﻿using Noteapp.Desktop.Data;
using Noteapp.Desktop.MVVM;
using Noteapp.Desktop.Networking;
using System;
using System.Windows.Input;

namespace Noteapp.Desktop.ViewModels
{
    public class SettingsViewModel : NotifyPropertyChanged, IPage
    {
        public string Name => PageNames.Settings;

        private readonly ApiService _apiService;

        public string Email => AppData.UserInfo.Email;
        public bool EncryptionEnabled => AppData.UserInfo.EncryptionEnabled;
        public DateTime RegistrationDate => AppData.UserInfo.RegistrationDate;

        private string _output;
        public string OutputMessage
        {
            get => _output;
            set => Set(ref _output, value);
        }

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
            AppData.DeleteUserInfo();
            AppData.DeleteNotes();
            OutputMessage = "Successfully logged out";
            OnPropertyChanged(nameof(Email));
            OnPropertyChanged(nameof(EncryptionEnabled));
        }

        private async void DeleteAccount()
        {
            if (await _apiService.DeleteAccount())
            {
                OutputMessage = "Account successfully deleted";
                Logout();
            }
            else
            {
                OutputMessage = "Failed to delete account";
            }
        }

        private async void ToggleEncryption()
        {
            if (string.IsNullOrWhiteSpace(AppData.UserInfo.EncryptionKey))
            {
                OutputMessage = "You have to be logged in to enable encryption!";
                return;
            }
            AppData.UserInfo.EncryptionEnabled = !AppData.UserInfo.EncryptionEnabled;
            await AppData.SaveUserInfo();
        }

        public void RefreshPage()
        {
            OutputMessage = string.Empty;
        }
    }
}
