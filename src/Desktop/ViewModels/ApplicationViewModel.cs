using Microsoft.Extensions.Configuration;
using Noteapp.Desktop.Data;
using Noteapp.Desktop.MVVM;
using Noteapp.Desktop.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Timers;
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
            AppData.LoadUserInfoToMemory();
            var configuration = CreateConfiguration().Get<Configuration>();

            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(configuration.ApiBaseUrl)
            };

            var apiService = new ApiService(httpClient);

            var registerVM = new RegisterViewModel(apiService);
            var loginVM = new LoginViewModel(apiService);
            var notesVM = new NotesViewModel(apiService, configuration);
            var settingsVM = new SettingsViewModel(apiService);

            Pages.AddRange(new IPage[] { registerVM, loginVM, notesVM, settingsVM });
            CurrentPage = notesVM;

            ChangePageCommand = new RelayCommand(ChangePage);
        }

        private void ChangePage(object parameter)
        {
            var page = (IPage)parameter;
            CurrentPage = page;
            page.RefreshPage();
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
