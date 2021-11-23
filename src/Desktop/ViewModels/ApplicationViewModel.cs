﻿using Noteapp.Core.Interfaces;
using Noteapp.Desktop.MVVM;
using Noteapp.Desktop.Session;
using Noteapp.Infrastructure.Networking;
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
            // has to be the same instance in LoginViewModel and NotesViewModel
            IApiCaller apiCaller = new ApiCaller(httpClient);
            SessionManager sessionManager = new SessionManager();

            LoadAccessToken(apiCaller, sessionManager);

            PageViewModels.Add(new RegisterViewModel(apiCaller));
            PageViewModels.Add(new LoginViewModel(apiCaller, sessionManager));
            PageViewModels.Add(new NotesViewModel(apiCaller));

            CurrentPageViewModel = PageViewModels.Find(vm => vm.Name == PageNames.Notes);

            ChangePageCommand = new RelayCommand(
                vm => ChangeViewModel((IPageViewModel)vm),
                vm => vm is IPageViewModel);
        }

        private void LoadAccessToken(IApiCaller apiCaller, SessionManager sessionManager)
        {
            apiCaller.AccessToken = sessionManager.GetUserInfo()?.Result?.access_token;
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
