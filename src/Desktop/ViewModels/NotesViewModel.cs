using Noteapp.Desktop.MVVM;
using Noteapp.Desktop.Networking;
using Noteapp.Core.Entities;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;

namespace Noteapp.Desktop.ViewModels
{
    public class NotesViewModel : NotifyPropertyChanged, IPageViewModel
    {
        public string Name => "Notes";

        private ObservableCollection<Note> _notes;
        private Note _selectedNote;
        private readonly ApiCaller _apiCaller;

        public ObservableCollection<Note> Notes
        {
            get => _notes;
            set => Set(ref _notes, value);
        }

        public Note SelectedNote
        {
            get => _selectedNote;
            set => Set(ref _selectedNote, value);
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
            CreateNoteCommand = new RelayCommand(CreateNoteCommandExecute);
            EditNoteCommand = new RelayCommand(EditNoteCommandExecute, EditNoteCommandCanExecute);

            ListNotesCommand.Execute(null);
        }

        private async void ListNotesCommandExecute(object parameter)
        {
            var notes = await _apiCaller.GetNotes();
            var noteId = SelectedNote?.Id;
            Notes = new ObservableCollection<Note>(notes);
            SelectedNote = Notes.FirstOrDefault(note => note.Id == noteId);
        }

        private async void CreateNoteCommandExecute(object parameter)
        {
            await _apiCaller.CreateNote();
            ListNotesCommand.Execute(null);
        }

        private async void EditNoteCommandExecute(object parameter)
        {
            await _apiCaller.EditNote(SelectedNote.Id, SelectedNote.Text);
            ListNotesCommand.Execute(null);
        }

        private bool EditNoteCommandCanExecute(object parameter)
        {
            return SelectedNote != null;
        }

        private async void DeleteNoteCommandExecute(object parameter)
        {
            await _apiCaller.DeleteNote((int)parameter);
            ListNotesCommand.Execute(null);
        }
    }
}
