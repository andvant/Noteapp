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

        public int MaximumSnapshotIndex => Snapshots.Count - 1;
        public string CurrentSnapshotText => Snapshots[CurrentSnapshotIndex].Text;
        public string CurrentSnapshotDate => Snapshots[CurrentSnapshotIndex].Created.ToString();

        // Commands
        public ICommand ListCommand { get; }
        public ICommand CreateCommand { get; }
        public ICommand EditCommand { get; }
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
            EditCommand = new RelayCommand(EditCommandExecute, EditCommandCanExecute);
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

            // TODO: use DI
            var userInfo = await SessionManager.GetUserInfo();

            foreach (var note in notes)
            {
                try
                {
                    var protector = new Protector(userInfo?.EncryptionKey);
                    note.Text = protector.Decrypt(note.Text);
                }
                catch
                {
                    note.Text = $"[not decrypted]{note.Text}";
                }
            }

            Notes = new ObservableCollection<Note>(OrderByPinned(notes));

            SelectedNote = Notes.FirstOrDefault(note => note.Id == selectedNoteId);
        }

        private async void CreateCommandExecute(object parameter)
        {
            await _apiCaller.CreateNote();
            ListCommand.Execute(null);
        }

        private async void EditCommandExecute(object parameter)
        {
            // TODO: use DI
            var userInfo = await SessionManager.GetUserInfo();

            string text;
            try
            {
                var protector = new Protector(userInfo?.EncryptionKey);
                text = protector.Encrypt(SelectedNote.Text);
            }
            catch
            {
                text = $"[not encrypted]{SelectedNote.Text}";
            }

            await _apiCaller.EditNote(SelectedNote.Id, text);
            ListCommand.Execute(null);
        }

        private bool EditCommandCanExecute(object parameter)
        {
            return SelectedNote != null && !SelectedNote.Locked && HistoryVisibility == Visibility.Collapsed;
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

        private async void ExportNotesCommandExecute(object parameter)
        {
            var notes = await _apiCaller.GetNotes();

            var dialog = new SaveFileDialog()
            {
                FileName = "NotesBackup-[date]",
                Filter = "JSON file|*.json"
            };
            var result = dialog.ShowDialog();
            if (result == true)
            {
                File.WriteAllText(dialog.FileName,
                    JsonSerializer.Serialize(notes, new JsonSerializerOptions() { WriteIndented = true }));
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
                await _apiCaller.BulkCreateNotes(notes);
                ListCommand.Execute(null);
            }
        }
        private void RestoreSnapshotCommandExecute(object parameter)
        {
            HistoryVisibility = Visibility.Collapsed;

            SelectedNote.Text = CurrentSnapshotText;
            Snapshots = null;
            EditCommand.Execute(null);
        }

        private async void ShowHistoryCommandExecute(object parameter)
        {
            HistoryVisibility = Visibility.Visible;
            _oldNoteText = SelectedNote.Text;

            var snapshots = await _apiCaller.GetAllSnapshots(SelectedNote.Id);
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
    }
}
