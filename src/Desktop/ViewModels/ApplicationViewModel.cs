using Microsoft.Extensions.Configuration;
using Noteapp.Desktop.MVVM;
using Noteapp.Desktop.Networking;
using Noteapp.Desktop.Session;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
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
            SessionManager.LoadUserInfoToMemory();
            var configuration = CreateConfiguration();

            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(configuration["ApiBaseUrl"])
            };

            var apiService = new ApiService(httpClient);

            Pages.Add(new RegisterViewModel(apiService));
            Pages.Add(new LoginViewModel(apiService));
            Pages.Add(new NotesViewModel(apiService, configuration["WebBaseUrl"]));
            Pages.Add(new SettingsViewModel(apiService));

            CurrentPage = Pages.Find(vm => vm.Name == PageNames.Notes);

            ChangePageCommand = new RelayCommand(ChangePage);
        }

        private void ChangePage(object page)
        {
            CurrentPage = (IPage)page;

            if (page is NotesViewModel notesVM)
            {
                notesVM.ListCommand.Execute(null);
            }
        }

        private IConfiguration CreateConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();
        }
    }
}
