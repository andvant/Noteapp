﻿using Noteapp.Desktop.MVVM;
using Noteapp.Desktop.Networking;
using Noteapp.Desktop.Session;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows.Input;

namespace Noteapp.Desktop.ViewModels
{
    public class ApplicationViewModel : NotifyPropertyChanged
    {
        private IPageViewModel _currentPageViewModel;

        public List<IPageViewModel> PageViewModels { get; } = new();
        public ICommand ChangePageCommand { get; }

        public ApplicationViewModel()
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://localhost:5000/")
            };
            // has to be the same instance of ApiCaller in LoginViewModel and NotesViewModel
            var apiCaller = new ApiCaller(httpClient);
            var sessionManager = new SessionManager();

            LoadAccessToken(apiCaller, sessionManager);

            PageViewModels.Add(new RegisterViewModel(apiCaller));
            PageViewModels.Add(new LoginViewModel(apiCaller, sessionManager));
            PageViewModels.Add(new NotesViewModel(apiCaller));

            CurrentPageViewModel = PageViewModels.Find(vm => vm.Name == PageNames.Notes);

            ChangePageCommand = new RelayCommand(
                vm => ChangeViewModel((IPageViewModel)vm),
                vm => vm is IPageViewModel);
        }

        private void LoadAccessToken(ApiCaller apiCaller, SessionManager sessionManager)
        {
            apiCaller.AccessToken = sessionManager.GetUserInfo()?.Result?.AccessToken;
        }

        public IPageViewModel CurrentPageViewModel
        {
            get => _currentPageViewModel;
            set => Set(ref _currentPageViewModel, value);
        }

        private void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
            {
                PageViewModels.Add(viewModel);
            }

            CurrentPageViewModel = viewModel;
        }
    }
}
