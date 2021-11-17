using Noteapp.Desktop.MVVM;
using Noteapp.Desktop.Networking;
using Noteapp.Desktop.Parameters;
using Noteapp.Core.Entities;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Noteapp.Desktop.ViewModels
{
    public class NotesViewModel : NotifyPropertyChanged, IPageViewModel
    {
        public string Name => "Notes";

        private string _createNoteText;
        private ObservableCollection<Note> _notes;
        private readonly ApiCaller _apiCaller;

        public string CreateNoteText
        {
            get => _createNoteText;
            set => Set(ref _createNoteText, value);
        }
        public ObservableCollection<Note> Notes
        {
            get => _notes;
            set => Set(ref _notes, value);
        }

        public ICommand ListNotesCommand { get; }
        public ICommand CreateNoteCommand { get; }
        public ICommand DeleteNoteCommand { get; }
        public ICommand EditNoteCommand { get; }
        public ICommand SortByLastModifiedCommand { get; }
        public ICommand SortByCreatedCommand { get; }
        public ICommand SortByTextCommand { get; }

        public NotesViewModel(ApiCaller apiCaller)
        {
            _apiCaller = apiCaller;

            ListNotesCommand = new RelayCommand(ListNotesCommandExecute);
            DeleteNoteCommand = new RelayCommand(DeleteNoteCommandExecute);
            CreateNoteCommand = new RelayCommand(CreateNoteCommandExecute, CreateNoteCommandCanExecute);
            EditNoteCommand = new RelayCommand(EditNoteCommandExecute);
        }

        private async void ListNotesCommandExecute(object parameter)
        {
            var notes = await _apiCaller.GetNotes();
            Notes = new ObservableCollection<Note>(notes);
        }

        private async void CreateNoteCommandExecute(object parameter)
        {
            await _apiCaller.CreateNote(CreateNoteText);
            ListNotesCommand.Execute(null);
        }

        private bool CreateNoteCommandCanExecute(object parameter)
        {
            return !string.IsNullOrWhiteSpace(CreateNoteText);
        }

        private async void EditNoteCommandExecute(object parameter)
        {
            var editNoteParameter = (EditNoteParameter)parameter;
            await _apiCaller.EditNote(editNoteParameter.NoteId, editNoteParameter.Text);
            ListNotesCommand.Execute(null);
        }

        private async void DeleteNoteCommandExecute(object parameter)
        {
            await _apiCaller.DeleteNote((int)parameter);
            ListNotesCommand.Execute(null);
        }
    }
}
