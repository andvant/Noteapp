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
        public DateTime? CurrentSnapshotDate => Snapshots?[CurrentSnapshotIndex]?.Created;

        public ICommand ListCommand { get; }
        public ICommand CreateCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand ToggleLockedCommand { get; }
        public ICommand ToggleArchivedCommand { get; }
        public ICommand TogglePinnedCommand { get; }
        public ICommand TogglePublishedCommand { get; }
        public ICommand SortyByCreatedCommand { get; }
        public ICommand SortyByUpdatedCommand { get; }
        public ICommand SortyByTextCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand ShowHistoryCommand { get; }
        public ICommand RestoreSnapshotCommand { get; }
        public ICommand CancelHistoryCommand { get; }
        public ICommand ToggleShowArchivedCommand { get; }

        public NotesViewModel(ApiService apiService)
        {
            _apiService = apiService;

            ListCommand = new RelayCommand(async () => await List());
            CreateCommand = new RelayCommand(Create);
            SaveCommand = new RelayCommand(async () => await Save(), CanSave);
            DeleteCommand = new RelayCommand(Delete);
            ToggleLockedCommand = new RelayCommand(ToggleLocked);
            ToggleArchivedCommand = new RelayCommand(ToggleArchived);
            TogglePinnedCommand = new RelayCommand(TogglePinned);
            TogglePublishedCommand = new RelayCommand(TogglePublished);
            SortyByCreatedCommand = new RelayCommand(SortByCreated, CanSort);
            SortyByUpdatedCommand = new RelayCommand(SortByUpdated, CanSort);
            SortyByTextCommand = new RelayCommand(SortByText, CanSort);
            ExportCommand = new RelayCommand(Export);
            ImportCommand = new RelayCommand(Import);
            ShowHistoryCommand = new RelayCommand(ShowHistory, CanShowHistory);
            RestoreSnapshotCommand = new RelayCommand(RestoreSnapshot, CanRestoreSnapshot);
            CancelHistoryCommand = new RelayCommand(CancelHistory);
            ToggleShowArchivedCommand = new RelayCommand(ToggleShowArchived);

            ListCommand.Execute(null);
        }

        private async Task List()
        {
            var selectedNoteId = SelectedNote?.Id;

            var notes = await _apiService.GetNotes(ShowArchived);

            foreach (var note in notes)
            {
                note.Text = await TryDecrypt(note.Text);
            }

            Notes = CreateNoteCollection(notes);
            SelectedNote = Notes.FirstOrDefault(note => note.Id == selectedNoteId) ?? Notes.FirstOrDefault();
        }

        private async void Create()
        {
            var newNote = await _apiService.CreateNote();
            Notes.Add(newNote);
            SelectedNote = newNote;
        }

        private async Task Save()
        {
            string text = await TryEncrypt(SelectedNote.Text);

            var updatedNote = await _apiService.UpdateNote(SelectedNote.Id, text);

            updatedNote.Text = await TryDecrypt(updatedNote.Text);
            ChangeNote(SelectedNote, updatedNote);
        }

        private bool CanSave()
        {
            return SelectedNote != null && !SelectedNote.Locked && HistoryVisibility == Visibility.Collapsed;
        }

        private async void Delete()
        {
            await _apiService.DeleteNote(SelectedNote.Id);
            Notes.Remove(SelectedNote);

            SelectedNote = Notes.FirstOrDefault();
        }

        private async void ToggleLocked()
        {
            var updatedNote = await _apiService.ToggleLocked(SelectedNote.Id, SelectedNote.Locked);

            updatedNote.Text = await TryDecrypt(updatedNote.Text);
            ChangeNote(SelectedNote, updatedNote);
        }

        private async void ToggleArchived()
        {
            var updatedNote = await _apiService.ToggleArchived(SelectedNote.Id, SelectedNote.Archived);

            updatedNote.Text = await TryDecrypt(updatedNote.Text);

            ChangeNote(SelectedNote, updatedNote);

            Notes.Remove(updatedNote);

            SelectedNote = Notes.FirstOrDefault();
        }

        private async void TogglePinned()
        {
            var updatedNote = await _apiService.TogglePinned(SelectedNote.Id, SelectedNote.Pinned);

            updatedNote.Text = await TryDecrypt(updatedNote.Text);
            ChangeNote(SelectedNote, updatedNote);
            Notes = CreateNoteCollection(Notes);
        }

        private async void TogglePublished()
        {
            var updatedNote = await _apiService.TogglePublished(SelectedNote.Id, SelectedNote.Published);

            updatedNote.Text = await TryDecrypt(updatedNote.Text);
            ChangeNote(SelectedNote, updatedNote);
        }

        private void SortByCreated()
        {
            var notes = new List<Note>(Notes);
            Comparison<Note> comparison = (note1, note2) => DateTime.Compare(note2.Created, note1.Created);
            notes.Sort(comparison);
            if (_descendingCreated) notes.Reverse();
            _descendingCreated = !_descendingCreated;
            _descendingText = false;
            _descendingUpdated = false;
            Notes = CreateNoteCollection(notes);
        }

        private void SortByUpdated()
        {
            var notes = new List<Note>(Notes);
            Comparison<Note> comparison = (note1, note2) => DateTime.Compare(note2.Updated, note1.Updated);
            notes.Sort(comparison);
            if (_descendingUpdated) notes.Reverse();
            _descendingUpdated = !_descendingUpdated;
            _descendingCreated = false;
            _descendingText = false;
            Notes = CreateNoteCollection(notes);
        }

        private void SortByText()
        {
            var notes = new List<Note>(Notes);
            Comparison<Note> comparison = (note1, note2) => string.Compare(note1.Text, note2.Text,
                StringComparison.CurrentCultureIgnoreCase);
            notes.Sort(comparison);
            if (_descendingText) notes.Reverse();
            _descendingText = !_descendingText;
            _descendingCreated = false;
            _descendingUpdated = false;
            Notes = CreateNoteCollection(notes);
        }

        private bool CanSort()
        {
            return Notes?.Count > 0;
        }

        private void Export()
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

        private async void Import()
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
                await List();
            }
        }

        private async void RestoreSnapshot()
        {
            HistoryVisibility = Visibility.Collapsed;

            SelectedNote.Text = CurrentSnapshotText;
            Snapshots = null;
            await Save();
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

        private async void ToggleShowArchived()
        {
            ShowArchived = !ShowArchived;
            await List();
            SelectedNote = Notes.FirstOrDefault();
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

            try
            {
                return await Protector.Decrypt(text, userInfo.EncryptionKey);
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

            return await Protector.Encrypt(text, userInfo.EncryptionKey);
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
