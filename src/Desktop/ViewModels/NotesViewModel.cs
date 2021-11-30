using Microsoft.Win32;
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
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Noteapp.Desktop.ViewModels
{
    public class NotesViewModel : NotifyPropertyChanged, IPageViewModel
    {
        public string Name => PageNames.Notes;

        private ObservableCollection<Note> _notes;
        private Note _selectedNote;
        private readonly ApiCaller _apiCaller;
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

        // Commands
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

        public NotesViewModel(ApiCaller apiCaller)
        {
            _apiCaller = apiCaller;

            ListCommand = new RelayCommand(ListCommandExecute);
            CreateCommand = new RelayCommand(CreateCommandExecute);
            UpdateCommand = new RelayCommand(UpdateCommandExecute, UpdateCommandCanExecute);
            DeleteCommand = new RelayCommand(DeleteCommandExecute);
            ToggleLockedCommand = new RelayCommand(ToggleLockedCommandExecute);
            ToggleArchivedCommand = new RelayCommand(ToggleArchivedCommandExecute);
            TogglePinnedCommand = new RelayCommand(TogglePinnedCommandExecute);
            TogglePublishedCommand = new RelayCommand(TogglePublishedCommandExecute);
            SortyByCreatedCommand = new RelayCommand(SortByCreatedCommandExecute, SortByCanExecute);
            SortyByUpdatedCommand = new RelayCommand(SortByUpdatedCommandExecute, SortByCanExecute);
            SortyByTextCommand = new RelayCommand(SortByTextCommandExecute, SortByCanExecute);
            ExportNotesCommand = new RelayCommand(ExportNotesCommandExecute);
            ImportNotesCommand = new RelayCommand(ImportNotesCommandExecute);
            ShowHistoryCommand = new RelayCommand(ShowHistoryCommandExecute, ShowHistoryCommandCanExecute);
            RestoreSnapshotCommand = new RelayCommand(RestoreSnapshotCommandExecute);
            CancelHistoryCommand = new RelayCommand(CancelHistoryCommandExecute);
            ToggleShowArchivedCommand = new RelayCommand(ToggleArchivedViewCommandExecute);

            ListCommand.Execute(null);
        }

        private async void ListCommandExecute(object parameter)
        {
            var selectedNoteId = SelectedNote?.Id;

            var notes = await _apiCaller.GetNotes(ShowArchived);

            foreach (var note in notes)
            {
                note.Text = await TryDecryptText(note.Text);
            }

            Notes = new ObservableCollection<Note>(OrderByPinned(notes));

            SelectedNote = Notes.FirstOrDefault(note => note.Id == selectedNoteId);
        }

        private async void CreateCommandExecute(object parameter)
        {
            var newNote = await _apiCaller.CreateNote();
            Notes.Add(newNote);
            SelectedNote = newNote;
        }

        private async void UpdateCommandExecute(object parameter)
        {
            string encryptedText = await TryEncryptText(SelectedNote.Text);

            var updatedNote = await _apiCaller.UpdateNote(SelectedNote.Id, encryptedText);

            updatedNote.Text = await TryDecryptText(updatedNote.Text);

            int noteIndex = Notes.IndexOf(SelectedNote);
            Notes[noteIndex] = updatedNote;
            SelectedNote = updatedNote;
        }

        private bool UpdateCommandCanExecute(object parameter)
        {
            return SelectedNote != null && !SelectedNote.Locked && HistoryVisibility == Visibility.Collapsed;
        }

        private async void DeleteCommandExecute(object noteId)
        {
            await _apiCaller.DeleteNote((int)noteId);
            var note = Notes.Single(note => note.Id == (int)noteId);
            Notes.Remove(note);
        }

        private async void ToggleLockedCommandExecute(object parameter)
        {
            var note = (Note)parameter;
            var updatedNote = await _apiCaller.ToggleLocked(note.Id, note.Locked);

            updatedNote.Text = await TryDecryptText(updatedNote.Text);

            int noteIndex = Notes.IndexOf(note);
            Notes[noteIndex] = updatedNote;
            SelectedNote = updatedNote;
        }

        private async void ToggleArchivedCommandExecute(object parameter)
        {
            var note = (Note)parameter;
            var updatedNote = await _apiCaller.ToggleArchived(note.Id, note.Archived);

            updatedNote.Text = await TryDecryptText(updatedNote.Text);

            int noteIndex = Notes.IndexOf(note);
            Notes[noteIndex] = updatedNote;
            SelectedNote = updatedNote;

            if (updatedNote.Archived != ShowArchived)
            {
                Notes.Remove(updatedNote);
            }
        }

        private async void TogglePinnedCommandExecute(object parameter)
        {
            var note = (Note)parameter;
            var updatedNote = await _apiCaller.TogglePinned(note.Id, note.Pinned);

            updatedNote.Text = await TryDecryptText(updatedNote.Text);

            int noteIndex = Notes.IndexOf(note);
            Notes[noteIndex] = updatedNote;
            SelectedNote = updatedNote;

            Notes = new ObservableCollection<Note>(OrderByPinned(Notes));
        }

        private async void TogglePublishedCommandExecute(object parameter)
        {
            var note = (Note)parameter;
            var updatedNote = await _apiCaller.TogglePublished(note.Id, note.Published);

            updatedNote.Text = await TryDecryptText(updatedNote.Text);

            int noteIndex = Notes.IndexOf(note);
            Notes[noteIndex] = updatedNote;
            SelectedNote = updatedNote;
        }

        private void SortByCreatedCommandExecute(object parameter)
        {
            Comparison<Note> comparison = (note1, note2) => DateTime.Compare(note1.Created, note2.Created);
            SortNotes(comparison, ref _descendingCreated);
        }

        private void SortByUpdatedCommandExecute(object parameter)
        {
            Comparison<Note> comparison = (note1, note2) => DateTime.Compare(note1.Updated, note2.Updated);
            SortNotes(comparison, ref _descendingUpdated);
        }

        private void SortByTextCommandExecute(object parameter)
        {
            Comparison<Note> comparison = (note1, note2) => string.Compare(note1.Text, note2.Text);
            SortNotes(comparison, ref _descendingText);
        }

        private bool SortByCanExecute(object parameter)
        {
            return Notes?.Count > 0;
        }

        private void ExportNotesCommandExecute(object parameter)
        {
            var dialog = new SaveFileDialog()
            {
                FileName = "NotesBackup-[date]",
                Filter = "JSON file|*.json"
            };
            var result = dialog.ShowDialog();
            if (result == true)
            {
                File.WriteAllText(dialog.FileName,
                    JsonSerializer.Serialize(Notes, new JsonSerializerOptions() { WriteIndented = true }));
            }
        }

        private async void ImportNotesCommandExecute(object parameter)
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "JSON file|*.json"
            };
            var result = dialog.ShowDialog();

            if (result == true)
            {
                string json = File.ReadAllText(dialog.FileName);
                var notes = JsonSerializer.Deserialize<IEnumerable<Note>>(json);

                foreach (var note in notes)
                {
                    note.Text = await TryEncryptText(note.Text);
                }

                await _apiCaller.BulkCreateNotes(notes);
                ListCommand.Execute(null);
            }
        }
        private void RestoreSnapshotCommandExecute(object parameter)
        {
            HistoryVisibility = Visibility.Collapsed;

            SelectedNote.Text = CurrentSnapshotText;
            Snapshots = null;
            UpdateCommand.Execute(null);
        }

        private async void ShowHistoryCommandExecute(object parameter)
        {
            HistoryVisibility = Visibility.Visible;
            _oldNoteText = SelectedNote.Text;

            var snapshots = await _apiCaller.GetAllSnapshots(SelectedNote.Id);

            foreach (var snapshot in snapshots)
            {
                snapshot.Text = await TryDecryptText(snapshot.Text);
            }

            Snapshots = new ObservableCollection<NoteSnapshot>(snapshots);
            CurrentSnapshotIndex = MaximumSnapshotIndex;
        }

        private bool ShowHistoryCommandCanExecute(object parameter)
        {
            return HistoryVisibility == Visibility.Collapsed && SelectedNote != null;
        }

        private void CancelHistoryCommandExecute(object parameter)
        {
            HistoryVisibility = Visibility.Collapsed;

            SelectedNote.Text = _oldNoteText;
            Snapshots = null;
        }

        private void ToggleArchivedViewCommandExecute(object parameter)
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

            Notes = new ObservableCollection<Note>(OrderByPinned(notes));
        }

        private IOrderedEnumerable<Note> OrderByPinned(IEnumerable<Note> notes)
        {
            return notes.OrderBy(note => !note.Pinned);
        }

        private async Task<string> TryDecryptText(string text)
        {
            // TODO: use DI
            var userInfo = await SessionManager.GetUserInfo();
            var protector = new Protector(userInfo?.EncryptionKey);

            string result;
            try
            {
                result = protector.Decrypt(text);
            }
            catch
            {
                result = $"[not decrypted]{text}";
            }
            return result;
        }

        private async Task<string> TryEncryptText(string text)
        {
            var userInfo = await SessionManager.GetUserInfo();
            var protector = new Protector(userInfo?.EncryptionKey);

            string result;
            try
            {
                result = protector.Encrypt(text);
            }
            catch
            {
                result = $"[not encrypted]{text}";
            }
            return result;
        }
    }
}
