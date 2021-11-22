using Noteapp.Desktop.MVVM;
using Noteapp.Desktop.Networking;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows.Input;

namespace Noteapp.Desktop.ViewModels
{
    public class ApplicationViewModel : NotifyPropertyChanged
    {
        private IPageViewModel _currentPageViewModel;

        public List<IPageViewModel> PageViewModels { get; }
        public ICommand ChangePageCommand { get; }

        public ApplicationViewModel()
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://localhost:5000/")
            };
            var apiCaller = new ApiCaller(httpClient);

            PageViewModels = new();
            PageViewModels.Add(new NotesViewModel(apiCaller));
            PageViewModels.Add(new RegisterViewModel(apiCaller));
            PageViewModels.Add(new LoginViewModel(apiCaller));

            CurrentPageViewModel = PageViewModels[0];

            ChangePageCommand = new RelayCommand(
                vm => ChangeViewModel((IPageViewModel)vm),
                vm => vm is IPageViewModel);
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
