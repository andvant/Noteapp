﻿using Noteapp.Desktop.MVVM;
using Noteapp.Desktop.Networking;
using Noteapp.Desktop.Session;
using System.Windows;
using System.Windows.Input;

namespace Noteapp.Desktop.ViewModels
{
    public class SettingsViewModel : NotifyPropertyChanged, IPageViewModel
    {
        public string Name => PageNames.Settings;

        private readonly ApiCaller _apiCaller;

        public string Email => SessionManager.GetUserInfo()?.Result?.Email ?? "Anonymous";
        public ICommand LogoutCommand { get; }
        public ICommand DeleteAccountCommand { get; }

        public SettingsViewModel(ApiCaller apiCaller)
        {
            _apiCaller = apiCaller;
            LogoutCommand = new RelayCommand(LogoutCommandExecute);
            DeleteAccountCommand = new RelayCommand(DeleteAccountCommandExecute);
        }

        private void LogoutCommandExecute(object parameter)
        {
            _apiCaller.AccessToken = null;
            SessionManager.DeleteUserInfo();
            MessageBox.Show("Successfully logged out.");
            OnPropertyChanged(nameof(Email));
        }

        private async void DeleteAccountCommandExecute(object parameter)
        {
            await _apiCaller.DeleteAccount();
            MessageBox.Show("Account successfully deleted.");
            LogoutCommand.Execute(null);
        }
    }
}

