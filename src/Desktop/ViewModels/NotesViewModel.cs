using Microsoft.Win32;
using Noteapp.Desktop.Extensions;
using Noteapp.Desktop.Models;
using Noteapp.Desktop.MVVM;
using Noteapp.Desktop.Networking;
using Noteapp.Desktop.Security;
using Noteapp.Desktop.Session;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Noteapp.Desktop.ViewModels
{
    public class NotesViewModel : NotifyPropertyChanged, IPage
    {
        public string Name => PageNames.Notes;

        private ObservableCollection<Note> _notes;
        private Note _selectedNote;
        private readonly ApiService _apiService;
        private bool _descendingUpdated;
        private bool _descendingCreated;
        private bool _descendingText;

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
        public bool ShowArchived { get; set; }

        // Note history
        private Visibility _historyVisibility = Visibility.Collapsed;
        private ObservableCollection<NoteSnapshot> _snapshots;
        private int _currentSnapshotIndex;
        private string _oldNoteText;
        public Visibility HistoryVisibility
        {
            get => _historyVisibility;
            set => Set(ref _historyVisibility, value);
        }

        public ObservableCollection<NoteSnapshot> Snapshots
        {
            get => _snapshots;
            set
            {
                Set(ref _snapshots, value);
                OnPropertyChanged(nameof(MaximumSnapshotIndex));
            }
        }
        public int CurrentSnapshotIndex
        {
            get => _currentSnapshotIndex;
            set
            {
                Set(ref _currentSnapshotIndex, value);
                SelectedNote.Text = CurrentSnapshotText;
                OnPropertyChanged(nameof(CurrentSnapshotDate));
            }
        }
        public int MaximumSnapshotIndex => (Snapshots?.Count - 1) ?? 0;
        public string CurrentSnapshotText => Snapshots?[CurrentSnapshotIndex]?.Text;
        public string CurrentSnapshotDate => Snapshots?[CurrentSnapshotIndex]?.Created.ToString();

        public ICommand ListCommand { get; }
        public ICommand CreateCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ToggleLockedCommand { get; }
        public ICommand ToggleArchivedCommand { get; }
        public ICommand TogglePinnedCommand { get; }
        public ICommand TogglePublishedCommand { get; }
        public ICommand SortyByCreatedCommand { get; }
        public ICommand SortyByUpdatedCommand { get; }
        public ICommand SortyByTextCommand { get; }
        public ICommand ExportNotesCommand { get; }
        public ICommand ImportNotesCommand { get; }
        public ICommand ShowHistoryCommand { get; }
        public ICommand RestoreSnapshotCommand { get; }
        public ICommand CancelHistoryCommand { get; }
        public ICommand ToggleShowArchivedCommand { get; }

        public NotesViewModel(ApiService apiService)
        {
            _apiService = apiService;

            ListCommand = new RelayCommand(List);
            CreateCommand = new RelayCommand(Create);
            UpdateCommand = new RelayCommand(Update, CanUpdate);
            DeleteCommand = new RelayCommand(Delete);
            ToggleLockedCommand = new RelayCommand(ToggleLocked);
            ToggleArchivedCommand = new RelayCommand(ToggleArchived);
            TogglePinnedCommand = new RelayCommand(TogglePinned);
            TogglePublishedCommand = new RelayCommand(TogglePublished);
            SortyByCreatedCommand = new RelayCommand(SortByCreated, CanSort);
            SortyByUpdatedCommand = new RelayCommand(SortByUpdated, CanSort);
            SortyByTextCommand = new RelayCommand(SortByText, CanSort);
            ExportNotesCommand = new RelayCommand(ExportNotes);
            ImportNotesCommand = new RelayCommand(ImportNotes);
            ShowHistoryCommand = new RelayCommand(ShowHistory, CanShowHistory);
            RestoreSnapshotCommand = new RelayCommand(RestoreSnapshot, CanRestoreSnapshot);
            CancelHistoryCommand = new RelayCommand(CancelHistory);
            ToggleShowArchivedCommand = new RelayCommand(ToggleArchivedView);

            ListCommand.Execute(null);
        }

        private async void List()
        {
            var selectedNoteId = SelectedNote?.Id;

            var notes = await _apiService.GetNotes(ShowArchived);

            foreach (var note in notes)
            {
                note.Text = await TryDecrypt(note.Text);
            }

            Notes = CreateNoteCollection(notes);
            SelectedNote = Notes.FirstOrDefault(note => note.Id == selectedNoteId);
        }

        private async void Create()
        {
            var newNote = await _apiService.CreateNote();
            Notes.Add(newNote);
            SelectedNote = newNote;
        }

        private async void Update()
        {
            string text = await TryEncrypt(SelectedNote.Text);

            var updatedNote = await _apiService.UpdateNote(SelectedNote.Id, text);

            updatedNote.Text = await TryDecrypt(updatedNote.Text);
            ChangeNote(SelectedNote, updatedNote);
        }

        private bool CanUpdate()
        {
            return SelectedNote != null && !SelectedNote.Locked && HistoryVisibility == Visibility.Collapsed;
        }

        private async void Delete(object noteId)
        {
            await _apiService.DeleteNote((int)noteId);
            var note = Notes.Single(note => note.Id == (int)noteId);
            Notes.Remove(note);
        }

        private async void ToggleLocked(object parameter)
        {
            var note = (Note)parameter;
            var updatedNote = await _apiService.ToggleLocked(note.Id, note.Locked);

            updatedNote.Text = await TryDecrypt(updatedNote.Text);
            ChangeNote(note, updatedNote);
        }

        private async void ToggleArchived(object parameter)
        {
            var note = (Note)parameter;
            var updatedNote = await _apiService.ToggleArchived(note.Id, note.Archived);

            updatedNote.Text = await TryDecrypt(updatedNote.Text);

            ChangeNote(note, updatedNote);

            if (updatedNote.Archived != ShowArchived)
            {
                Notes.Remove(updatedNote);
            }
        }

        private async void TogglePinned(object parameter)
        {
            var note = (Note)parameter;
            var updatedNote = await _apiService.TogglePinned(note.Id, note.Pinned);

            updatedNote.Text = await TryDecrypt(updatedNote.Text);
            ChangeNote(note, updatedNote);
            Notes = CreateNoteCollection(Notes);
        }

        private async void TogglePublished(object parameter)
        {
            var note = (Note)parameter;
            var updatedNote = await _apiService.TogglePublished(note.Id, note.Published);

            updatedNote.Text = await TryDecrypt(updatedNote.Text);
            ChangeNote(note, updatedNote);
        }

        private void SortByCreated()
        {
            Comparison<Note> comparison = (note1, note2) => DateTime.Compare(note1.Created, note2.Created);
            SortNotes(comparison, ref _descendingCreated);
        }

        private void SortByUpdated()
        {
            Comparison<Note> comparison = (note1, note2) => DateTime.Compare(note1.Updated, note2.Updated);
            SortNotes(comparison, ref _descendingUpdated);
        }

        private void SortByText()
        {
            Comparison<Note> comparison = (note1, note2) => string.Compare(note1.Text, note2.Text);
            SortNotes(comparison, ref _descendingText);
        }

        private bool CanSort()
        {
            return Notes?.Count > 0;
        }

        private void ExportNotes()
        {
            var dialog = new SaveFileDialog()
            {
                FileName = "NotesBackup-[date]",
                Filter = "JSON file|*.json"
            };
            var result = dialog.ShowDialog();
            if (result == true)
            {
                File.WriteAllText(dialog.FileName, Notes.ToJson());
            }
        }

        private async void ImportNotes()
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "JSON file|*.json"
            };
            var result = dialog.ShowDialog();

            if (result == true)
            {
                string json = File.ReadAllText(dialog.FileName);
                var notes = json.FromJson<IEnumerable<Note>>();

                foreach (var note in notes)
                {
                    note.Text = await TryEncrypt(note.Text);
                }

                await _apiService.BulkCreateNotes(notes);
                ListCommand.Execute(null);
            }
        }

        private void RestoreSnapshot()
        {
            HistoryVisibility = Visibility.Collapsed;

            SelectedNote.Text = CurrentSnapshotText;
            Snapshots = null;
            UpdateCommand.Execute(null);
        }

        private bool CanRestoreSnapshot()
        {
            return CurrentSnapshotIndex < MaximumSnapshotIndex;
        }

        private async void ShowHistory()
        {
            HistoryVisibility = Visibility.Visible;
            _oldNoteText = SelectedNote.Text;

            var snapshots = await _apiService.GetAllSnapshots(SelectedNote.Id);

            foreach (var snapshot in snapshots)
            {
                snapshot.Text = await TryDecrypt(snapshot.Text);
            }

            Snapshots = new ObservableCollection<NoteSnapshot>(snapshots);
            CurrentSnapshotIndex = MaximumSnapshotIndex;
        }

        private bool CanShowHistory()
        {
            return HistoryVisibility == Visibility.Collapsed && SelectedNote != null;
        }

        private void CancelHistory()
        {
            HistoryVisibility = Visibility.Collapsed;

            SelectedNote.Text = _oldNoteText;
            Snapshots = null;
        }

        private void ToggleArchivedView()
        {
            ShowArchived = !ShowArchived;
            ListCommand.Execute(null);
        }

        private void SortNotes(Comparison<Note> comparison, ref bool descending)
        {
            var notes = new List<Note>(Notes);
            notes.Sort(comparison);

            if (descending) notes.Reverse();
            descending = !descending;

            Notes = CreateNoteCollection(notes);
        }

        private IOrderedEnumerable<Note> OrderByPinned(IEnumerable<Note> notes)
        {
            return notes.OrderBy(note => !note.Pinned);
        }

        private async Task<string> TryDecrypt(string text)
        {
            var userInfo = SessionManager.GetUserInfo();

            if (userInfo is null || !userInfo.EncryptionEnabled)
            {
                return text;
            }

            var protector = new Protector(userInfo.EncryptionKey);
            try
            {
                return await protector.Decrypt(text);
            }
            catch
            {
                return text;
            }
        }

        private async Task<string> TryEncrypt(string text)
        {
            var userInfo = SessionManager.GetUserInfo();

            if (userInfo is null || !userInfo.EncryptionEnabled)
            {
                return text;
            }

            var protector = new Protector(userInfo.EncryptionKey);
            return await protector.Encrypt(text);
        }

        private void ChangeNote(Note oldNote, Note newNote)
        {
            int noteIndex = Notes.IndexOf(oldNote);
            Notes[noteIndex] = newNote;
            SelectedNote = newNote;
        }

        private ObservableCollection<Note> CreateNoteCollection(IEnumerable<Note> notes)
        {
            return new ObservableCollection<Note>(OrderByPinned(notes));
        }
    }
}
