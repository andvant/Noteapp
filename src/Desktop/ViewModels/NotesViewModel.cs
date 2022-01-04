﻿using Microsoft.Win32;
using Noteapp.Desktop.Extensions;
using Noteapp.Desktop.LocalData;
using Noteapp.Desktop.Models;
using Noteapp.Desktop.MVVM;
using Noteapp.Desktop.Networking;
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

        private readonly ApiService _apiService;
        private bool _descendingUpdated;
        private bool _descendingCreated;
        private bool _descendingText;
        private string _webBaseUrl;

        private const int SAVE_DELAY_MS = 1000;
        private HashSet<Note> _notesCurrentlyBeingSaved = new();

        private ObservableCollection<Note> _notes;
        public ObservableCollection<Note> Notes
        {
            get => _notes;
            set
            {
                Set(ref _notes, value);
                OnPropertyChanged(nameof(ShownNotes));
            }
        }

        public IEnumerable<Note> ShownNotes => Notes
            .Where(note => note.Archived == ShowArchived)
            .OrderBy(note => !note.Pinned);

        private Note _selectedNote;
        public Note SelectedNote
        {
            get => _selectedNote;
            set
            {
                if (HistoryVisible) CancelHistory();
                if (value != null)
                {
                    value.TextChanged = false;
                }
                Set(ref _selectedNote, value);
            }
        }

        private bool _showArchived;
        public bool ShowArchived
        {
            get => _showArchived;
            set
            {
                Set(ref _showArchived, value);
                OnPropertyChanged(nameof(ShownNotes));
            }
        }

        // Note history
        private string _oldNoteText;

        private bool _historyVisible = false;
        public bool HistoryVisible
        {
            get => _historyVisible;
            set => Set(ref _historyVisible, value);
        }

        private ObservableCollection<NoteSnapshot> _snapshots;
        public ObservableCollection<NoteSnapshot> Snapshots
        {
            get => _snapshots;
            set
            {
                Set(ref _snapshots, value);
                OnPropertyChanged(nameof(MaximumSnapshotIndex));
            }
        }

        private int _currentSnapshotIndex;
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
        public ICommand CopyLinkCommand { get; }
        public ICommand SortyByCreatedCommand { get; }
        public ICommand SortyByUpdatedCommand { get; }
        public ICommand SortyByTextCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand ShowHistoryCommand { get; }
        public ICommand RestoreSnapshotCommand { get; }
        public ICommand CancelHistoryCommand { get; }
        public ICommand ToggleShowArchivedCommand { get; }
        public ICommand SaveAfterDelayCommand { get; }

        public NotesViewModel(ApiService apiService, string webBaseUrl)
        {
            _apiService = apiService;
            _webBaseUrl = webBaseUrl;

            ListCommand = new RelayCommand(async () => await List());
            CreateCommand = new RelayCommand(Create);
            SaveCommand = new RelayCommand(async () => await Save(SelectedNote), CanSave);
            DeleteCommand = new RelayCommand(Delete);
            ToggleLockedCommand = new RelayCommand(ToggleLocked);
            ToggleArchivedCommand = new RelayCommand(ToggleArchived);
            TogglePinnedCommand = new RelayCommand(TogglePinned);
            TogglePublishedCommand = new RelayCommand(TogglePublished);
            CopyLinkCommand = new RelayCommand(CopyLink);
            SortyByCreatedCommand = new RelayCommand(SortByCreated, CanSort);
            SortyByUpdatedCommand = new RelayCommand(SortByUpdated, CanSort);
            SortyByTextCommand = new RelayCommand(SortByText, CanSort);
            ExportCommand = new RelayCommand(Export);
            ImportCommand = new RelayCommand(Import);
            ShowHistoryCommand = new RelayCommand(ShowHistory, CanShowHistory);
            RestoreSnapshotCommand = new RelayCommand(RestoreSnapshot, CanRestoreSnapshot);
            CancelHistoryCommand = new RelayCommand(CancelHistory);
            ToggleShowArchivedCommand = new RelayCommand(ToggleShowArchived);
            SaveAfterDelayCommand = new RelayCommand(SaveAfterDelay);

            Notes = CreateNoteCollection(LocalDataManager.GetNotes());

            ListCommand.Execute(null);
        }

        private async Task List()
        {
            SelectedNote ??= ShownNotes.FirstOrDefault();

            var notes = await _apiService.GetNotes();
            if (notes != null)
            {
                await SynchronizeNotes(notes);
                SelectedNote ??= ShownNotes.FirstOrDefault();
            }
        }

        private async void Create()
        {
            var newLocalNote = Note.CreateLocalNote();
            newLocalNote.Archived = ShowArchived;

            Notes.Add(newLocalNote);
            SelectedNote = newLocalNote;

            var newNote = await _apiService.CreateNote(newLocalNote);
            if (newNote != null)
            {
                ChangeNote(newLocalNote, newNote);
            }

            LocalDataManager.SaveNotes(Notes);
        }

        private async Task<bool> Save(Note note)
        {
            note.Synchronized = false;

            var updatedNote = note.Local
                ? await _apiService.CreateNote(note)
                : await _apiService.UpdateNote(note);

            if (updatedNote != null)
            {
                ChangeNote(note, updatedNote);
            }

            LocalDataManager.SaveNotes(Notes);

            return updatedNote != null;
        }

        private async void SaveAfterDelay()
        {
            if (CanSave())
            {
                var note = SelectedNote;

                _notesCurrentlyBeingSaved.Add(note);

                await Task.Delay(SAVE_DELAY_MS);
                await Save(note);

                _notesCurrentlyBeingSaved.Remove(note);
            }
        }

        private bool CanSave()
        {
            return SelectedNote != null && SelectedNote.TextChanged && !SelectedNote.Locked &&
                !HistoryVisible && !_notesCurrentlyBeingSaved.Contains(SelectedNote);
        }

        private async void Delete()
        {
            var note = SelectedNote;
            Notes.Remove(note);
            SelectFirstNote();
            if (!note.Local)
            {
                await _apiService.DeleteNote(note.Id);
            }

            LocalDataManager.SaveNotes(Notes);
        }

        private async void ToggleLocked()
        {
            SelectedNote.Locked = !SelectedNote.Locked;
            await Save(SelectedNote);
        }

        private async void TogglePinned()
        {
            SelectedNote.Pinned = !SelectedNote.Pinned;
            await Save(SelectedNote);
        }

        private async void ToggleArchived()
        {
            var note = SelectedNote;
            SelectedNote.Archived = !SelectedNote.Archived;
            OnPropertyChanged(nameof(ShownNotes));
            SelectFirstNote();
            await Save(note);
        }

        private async void TogglePublished()
        {
            var note = SelectedNote;
            var synchronizedBeforePublishing = SelectedNote.Synchronized;

            SelectedNote.Published = !SelectedNote.Published;
            var success = await Save(SelectedNote);

            if (!success)
            {
                note.Published = !note.Published;
                note.Synchronized = synchronizedBeforePublishing;
                LocalDataManager.SaveNotes(Notes);
            }
        }

        private void CopyLink()
        {
            if (SelectedNote?.PublicUrl != null)
            {
                Clipboard.SetText($"{_webBaseUrl}p/{SelectedNote.PublicUrl}");
            }
            else
            {
                MessageBox.Show("Note is not published!");
            }
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
            return ShownNotes?.Count() > 0;
        }

        private void Export()
        {
            var dialog = new SaveFileDialog()
            {
                FileName = $"ExportedNotes-{DateTime.Now.ToShortDateString()}",
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
                var importedNotes = json.FromJson<IEnumerable<Note>>().ToList();

                importedNotes.ForEach(note =>
                {
                    note.Id = 0;
                    note.Local = true;
                    note.Synchronized = false;
                });

                Notes = CreateNoteCollection(Notes.Concat(importedNotes));
                LocalDataManager.SaveNotes(Notes);

                if (await _apiService.BulkCreateNotes(importedNotes))
                {
                    await List();
                }
            }
        }

        private async void RestoreSnapshot()
        {
            SelectedNote.Text = CurrentSnapshotText;
            Snapshots = null;
            await Save(SelectedNote);
            HistoryVisible = false;
            _oldNoteText = null;
        }

        private bool CanRestoreSnapshot()
        {
            return CurrentSnapshotIndex < MaximumSnapshotIndex;
        }

        private async void ShowHistory()
        {
            var snapshots = await _apiService.GetAllSnapshots(SelectedNote.Id);
            if (snapshots == null) return;

            _oldNoteText = SelectedNote.Text;
            Snapshots = new ObservableCollection<NoteSnapshot>(snapshots);
            CurrentSnapshotIndex = MaximumSnapshotIndex;
            HistoryVisible = true;
        }

        private bool CanShowHistory()
        {
            return !HistoryVisible && SelectedNote != null;
        }

        private void CancelHistory()
        {
            SelectedNote.Text = _oldNoteText;
            Snapshots = null;
            HistoryVisible = false;
        }

        private void ToggleShowArchived()
        {
            ShowArchived = !ShowArchived;
            SelectFirstNote();
        }

        private void ChangeNote(Note oldNote, Note newNote)
        {
            Notes.Insert(Notes.IndexOf(oldNote), newNote);
            if (oldNote == SelectedNote)
            {
                SelectedNote = newNote;
            }
            Notes.Remove(oldNote);
        }

        private void SelectFirstNote()
        {
            SelectedNote = ShownNotes.FirstOrDefault();
        }

        private ObservableCollection<Note> CreateNoteCollection(IEnumerable<Note> notes)
        {
            var noteCollection = new ObservableCollection<Note>(notes);
            noteCollection.CollectionChanged += (o, e) => OnPropertyChanged(nameof(ShownNotes));
            return noteCollection;
        }

        private async Task SynchronizeNotes(IEnumerable<Note> fetchedNotes)
        {
            var localNotes = Notes.ToList();
            var joinedNotes = localNotes.Join(fetchedNotes, local => local.Id, fetched => fetched.Id,
                (local, fetched) => (local, fetched));

            // process notes that exist both locally and on the server
            foreach (var note in joinedNotes)
            {
                await SynchronizeJoinedNote(note);
            }

            // process notes that were fetched from the server but don't exist locally
            foreach (var fetchedNote in fetchedNotes
                .ExceptBy(joinedNotes.Select(note => note.fetched.Id), note => note.Id))
            {
                Notes.Add(fetchedNote);
            }

            // process notes that exist locally but were not fetched from the server
            foreach (var localNote in localNotes
                .ExceptBy(joinedNotes.Select(note => note.local.Id), note => note.Id)
                .Union(localNotes.Where(note => note.Local)))
            {
                await SynchronizeLocalNote(localNote);
            }

            LocalDataManager.SaveNotes(Notes);
        }

        private async Task SynchronizeJoinedNote((Note local, Note fetched) note)
        {
            if (note.local.Synchronized)
            {
                if (note.fetched.Updated != note.local.Updated)
                {
                    // fetched note is newer than local copy; update local note
                    ChangeNote(note.local, note.fetched);
                }
                else
                {
                    // note is up-to-date with the server; do nothing
                }
            }
            else
            {
                if (note.fetched.Updated == note.local.Updated)
                {
                    // synchronizing local note with the server
                    var updatedNote = await _apiService.UpdateNote(note.local);
                    if (updatedNote != null)
                    {
                        ChangeNote(note.local, updatedNote);
                    }
                }
                else
                {
                    // there are unsynchronized local and remote versions of the same note.
                    // keep the fetched version and send a request to create a new note for the local version
                    var newNote = await _apiService.CreateNote(note.local);

                    if (newNote != null)
                    {
                        Notes.Add(newNote);
                        ChangeNote(note.local, note.fetched);
                    }
                }
            }
        }

        private async Task SynchronizeLocalNote(Note localNote)
        {
            if (localNote.Synchronized)
            {
                // note was deleted on the server; remove local note
                Notes.Remove(localNote);
            }
            else
            {
                // note was never sent to the server, or changes made locally were not synchronized
                // before the note was deleted on the server.
                // send a request to create a new note with local note's text
                var newNote = await _apiService.CreateNote(localNote);
                if (newNote != null)
                {
                    ChangeNote(localNote, newNote);
                }
            }
        }
    }
}
