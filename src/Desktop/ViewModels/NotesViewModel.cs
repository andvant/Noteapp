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

        public ICommand ListCommand { get; }
        public ICommand CreateCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ToggleLockedCommand { get; }
        public ICommand ToggleArchivedCommand { get; }
        public ICommand TogglePinnedCommand { get; }
        public ICommand TogglePublishedCommand { get; }

        public NotesViewModel(ApiCaller apiCaller)
        {
            _apiCaller = apiCaller;

            ListCommand = new RelayCommand(ListCommandExecute);
            CreateCommand = new RelayCommand(CreateCommandExecute);
            EditCommand = new RelayCommand(EditCommandExecute, EditCommandCanExecute);
            DeleteCommand = new RelayCommand(DeleteCommandExecute);
            ToggleLockedCommand = new RelayCommand(ToggleLockedCommandExecute);
            ToggleArchivedCommand = new RelayCommand(ToggleArchivedCommandExecute);
            TogglePinnedCommand = new RelayCommand(TogglePinnedCommandExecute);
            TogglePublishedCommand = new RelayCommand(TogglePublishedCommandExecute);

            ListCommand.Execute(null);
        }

        private async void ListCommandExecute(object parameter)
        {
            var selectedNoteId = SelectedNote?.Id;

            var notes = await _apiCaller.GetNotes();
            Notes = new ObservableCollection<Note>(notes);

            SelectedNote = Notes.FirstOrDefault(note => note.Id == selectedNoteId);
        }

        private async void CreateCommandExecute(object parameter)
        {
            await _apiCaller.CreateNote();
            ListCommand.Execute(null);
        }

        private async void EditCommandExecute(object parameter)
        {
            await _apiCaller.EditNote(SelectedNote.Id, SelectedNote.Text);
            ListCommand.Execute(null);
        }

        private bool EditCommandCanExecute(object parameter)
        {
            return SelectedNote != null;
        }

        private async void DeleteCommandExecute(object parameter)
        {
            await _apiCaller.DeleteNote((int)parameter);
            ListCommand.Execute(null);
        }

        private async void ToggleLockedCommandExecute(object parameter)
        {
            var note = (Note)parameter;
            await _apiCaller.ToggleLocked(note.Id, note.Locked);
            ListCommand.Execute(null);
        }

        private async void ToggleArchivedCommandExecute(object parameter)
        {
            var note = (Note)parameter;
            await _apiCaller.ToggleArchived(note.Id, note.Archived);
            ListCommand.Execute(null);
        }

        private async void TogglePinnedCommandExecute(object parameter)
        {
            var note = (Note)parameter;
            await _apiCaller.TogglePinned(note.Id, note.Pinned);
            ListCommand.Execute(null);
        }

        private async void TogglePublishedCommandExecute(object parameter)
        {
            var note = (Note)parameter;
            await _apiCaller.TogglePublished(note.Id, note.Published);
            ListCommand.Execute(null);
        }
    }
}
