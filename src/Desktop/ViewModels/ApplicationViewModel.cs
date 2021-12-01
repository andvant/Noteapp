﻿using Noteapp.Desktop.MVVM;
using Noteapp.Desktop.Networking;
using Noteapp.Desktop.Session;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;

namespace Noteapp.Desktop.ViewModels
{
    public class ApplicationViewModel : NotifyPropertyChanged
    {
        public List<IPage> Pages { get; } = new();
        private IPage _currentPage;

        public IPage CurrentPage
        {
            get => _currentPage;
            set => Set(ref _currentPage, value);
        }

        public ICommand ChangePageCommand { get; }

        public ApplicationViewModel()
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://localhost:5000/")
            };
            // has to be the same instance of ApiService in LoginViewModel and NotesViewModel
            var apiService = new ApiService(httpClient);

            LoadAccessToken(apiService);

            Pages.Add(new RegisterViewModel(apiService));
            Pages.Add(new LoginViewModel(apiService));
            Pages.Add(new NotesViewModel(apiService));
            Pages.Add(new SettingsViewModel(apiService));

            CurrentPage = Pages.Find(vm => vm.Name == PageNames.Notes);

            ChangePageCommand = new RelayCommand(ChangePage);
        }

        private void LoadAccessToken(ApiService apiService)
        {
            apiService.AccessToken = SessionManager.GetUserInfo()?.Result?.AccessToken;
        }

        private void ChangePage(object page)
        {
            CurrentPage = (IPage)page;

            if (page is NotesViewModel notesVM)
            {
                notesVM.ListCommand.Execute(null);
            }
        }
    }
}
