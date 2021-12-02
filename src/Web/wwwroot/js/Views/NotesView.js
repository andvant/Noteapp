import ApiService from "../ApiService.js";

let NotesView = { render, init }

let _notes;
let _selectedNoteId;
let _snapshots;
let _showArchived = false;
let _oldNoteText;
let _sortByUpdatedDescending = false;
let _sortByCreatedDescending = false;
let _sortByTextDescending = false;

async function render() {
    return /*html*/ `
        <div id="notes-view">
            <div id="notes-all">
                <div id="notes-actions">
                    <button id="toggle-show-archived-button">Archived</button>
                    <button id="new-button">New</button>
                    <button id="list-button">List</button>
                </div>
                <div id="notes-sort">
                    <label>Sort by:</label>
                    <button id="sortby-created-button">Created</button>
                    <button id="sortby-updated-button">Updated</button>
                    <button id="sortby-text-button">Text</button>
                </div>
                <div id="notes-list"></div>
            </div>
            <div id="selected-note-column">
                <div id="selected-note">
                    <div id="selected-note-actions">
                        <button id="save-button">Save</button>
                        
                        <label for="pin-checkbox">Pin
                        <input id="pin-checkbox" type="checkbox" />
                        </label>
                        
                        <label for="lock-checkbox">Lock
                        <input id="lock-checkbox" type="checkbox" />
                        </label>
                        
                        <label for="archive-checkbox">Archive
                        <input id="archive-checkbox" type="checkbox" />
                        </label>
                        
                        <label for="publish-checkbox">Publish
                        <input id="publish-checkbox" type="checkbox" />
                        </label>
                        
                        <button id="export-notes-button">Export</button>
                        <button id="import-notes-button">Import</button>
                        <input id="import-notes-input" type="file" style="display: none;" />
                        <button id="history-button">History</button>
                        <button id="delete-button">Delete</button>
                    </div>
                    <div id="selected-note-text">
                        <textarea id="note-text" placeholder="Note text here..."></textarea>
                    </div>
                </div>
                <div id="note-history">
                    <div id="snapshot-date"></div>
                    <input type="range" id="history-slider" min="0" max="9" step="1" />
                    <div id="note-history-buttons">
                        <button id="cancel-history-button">Cancel</button>
                        <button id="restore-snapshot-button">Restore</button>
                    </div>
                </div>
            </div>
        </div>`
}

async function init() {

    const saveButton = document.getElementById('save-button');
    const newButton = document.getElementById('new-button');
    const listButton = document.getElementById('list-button');
    const deleteButton = document.getElementById('delete-button');
    const pinCheckbox = document.getElementById('pin-checkbox');
    const lockCheckbox = document.getElementById('lock-checkbox');
    const archiveCheckbox = document.getElementById('archive-checkbox');
    const publishCheckbox = document.getElementById('publish-checkbox');
    const sortByCreatedButton = document.getElementById('sortby-created-button');
    const sortByUpdatedButton = document.getElementById('sortby-updated-button');
    const sortByTextButton = document.getElementById('sortby-text-button');
    const importNotesButton = document.getElementById('import-notes-button');
    const importNotesInput = document.getElementById('import-notes-input');
    const exportNotesButton = document.getElementById('export-notes-button');
    const notesListDiv = document.getElementById('notes-list')
    const noteTextElement = document.getElementById('note-text');
    const toggleShowArchivedButton = document.getElementById('toggle-show-archived-button');

    const historyDiv = document.getElementById('note-history');
    const snapshotDateDiv = document.getElementById('snapshot-date');
    const historySlider = document.getElementById('history-slider');
    const historyButton = document.getElementById('history-button');
    const cancelHistoryButton = document.getElementById('cancel-history-button');
    const restoreSnapshotButton = document.getElementById('restore-snapshot-button');

    addListeners();
    listButton.click();

    function addListeners() {
        listButton.addEventListener('click', async () => {
            let notes = await ApiService.getNotes(_showArchived);
            _notes = notes;
            addNotes();
        });

        newButton.addEventListener('click', async () => {
            let newNote = await ApiService.createNote();
            addNote(newNote);
        });

        saveButton.addEventListener('click', async () => {
            let updatedNote = await ApiService.updateNote(_selectedNoteId, getTextElementValue());
            updateNote(updatedNote);
        });

        deleteButton.addEventListener('click', async () => {
            let noteId = _selectedNoteId;
            await ApiService.deleteNote(noteId);
            removeNote(noteId);
        });

        lockCheckbox.addEventListener('change', async () => {
            let updatedNote = await ApiService.toggleLocked(getSelectedNote());
            updateNote(updatedNote);
            makeNoteReadOnly(updatedNote.locked);
        });

        archiveCheckbox.addEventListener('change', async () => {
            let updatedNote = await ApiService.toggleArchived(getSelectedNote());
            updateNote(updatedNote);
            removeNoteElement(updatedNote.id);
        });

        pinCheckbox.addEventListener('change', async () => {
            let updatedNote = await ApiService.togglePinned(getSelectedNote());
            updateNote(updatedNote);
            addNoteElements();
        });

        publishCheckbox.addEventListener('change', async () => {
            let updatedNote = await ApiService.togglePublished(getSelectedNote());
            updateNote(updatedNote);
        });

        sortByCreatedButton.addEventListener('click', () => {
            sortByCreated();
            addNoteElements();
        });

        sortByUpdatedButton.addEventListener('click', () => {
            sortByUpdated();
            addNoteElements();
        });

        sortByTextButton.addEventListener('click', () => {
            sortByText();
            addNoteElements();
        });

        importNotesButton.addEventListener('click', () => {
            importNotesInput.click();
        });

        importNotesInput.addEventListener('change', importNotes);
        exportNotesButton.addEventListener('click', exportNotes);

        toggleShowArchivedButton.addEventListener('click', async () => {
            _showArchived = !_showArchived;
            listButton.click();
        });

        historyButton.addEventListener('click', showHistory);

        cancelHistoryButton.addEventListener('click', () => {
            setTextElementValue(_oldNoteText);
            historyDiv.style.display = 'none';
        });

        historySlider.addEventListener('input', () => {
            snapshotDateDiv.textContent = _snapshots[historySlider.value].created;
            setTextElementValue(_snapshots[historySlider.value].text);
        });

        restoreSnapshotButton.addEventListener('click', () => {
            setTextElementValue(_snapshots[historySlider.value].text);
            saveButton.click();
            historyDiv.style.display = 'none';
            _oldNoteText = null;
        });

        notesListDiv.addEventListener('click', noteSelectedHandler);
    }

    function addNote(newNote) {
        _notes.push(newNote);
        addNoteElement(newNote);
    }

    function addNoteElement(note) {
        notesListDiv.insertAdjacentHTML('beforeend', createNoteHtml(note));
    }

    function removeNote(noteId) {
        _notes.pop(getNote(noteId));
        removeNoteElement(noteId);
    }

    function removeNoteElement(noteId) {
        let noteDiv = document.getElementById(`note-${noteId}`);
        noteDiv.remove();
    }

    function updateNote(updatedNote) {
        changeNote(updatedNote);
        updateNoteElement(updatedNote);
        setCheckboxesAndTextForSelectedNote()
    }

    function updateNoteElement(note) {
        getNoteElement(note.id).outerHTML = createNoteHtml(note);
    }

    function getNoteElement(noteId) {
        return document.getElementById(`note-${noteId}`);
    }

    function addNotes() {
        addNoteElements();
        setCheckboxesAndTextForSelectedNote()
    }

    function addNoteElements() {
        sortByPinned();
        notesListDiv.innerHTML = '';
        for (let note of _notes) {
            notesListDiv.insertAdjacentHTML('beforeend', createNoteHtml(note));
        }
    }

    function getNoteId(noteElement) {
        return parseInt(noteElement.id.split('-')[1]);
    }

    function getTextElementValue() {
        return noteTextElement.value;
    }

    function setTextElementValue(text) {
        if (text === undefined) return;
        noteTextElement.value = text;
    }

    function makeNoteReadOnly(locked) {
        noteTextElement.readOnly = locked ? true : false;
        saveButton.disabled = locked ? true : false;
    }

    function noteSelectedHandler(event) {
        cancelHistoryButton.click();
        let noteElement = event.target.closest('.note');
        getNoteElement(_selectedNoteId)?.classList.remove('note-selected');
        noteElement.classList.add('note-selected');
        let noteId = getNoteId(noteElement);
        let note = getNote(noteId);

        _selectedNoteId = noteId;
        setCheckboxesAndTextForSelectedNote()
        makeNoteReadOnly(note?.locked);
    }

    function setCheckboxes(note) {
        if (!note) return;
        document.getElementById("pin-checkbox").checked = note.pinned;
        document.getElementById("lock-checkbox").checked = note.locked;
        document.getElementById("archive-checkbox").checked = note.archived;
        document.getElementById("publish-checkbox").checked = note.published;
    }

    function createNoteHtml(note) {
        return /*html*/ `
            <div id="note-${note.id}" class="note">
                <div>Id: ${note.id}</div>
                <div>AuthorId: ${note.authorId}</div>
                <div>Created: ${dateToString(note.created)}</div>
                <div>Updated: ${dateToString(note.updated)}</div>
                <div>PublicURL: ${note.publicUrl}</div>
            </div>`
    }

    function dateToString(date) {
        return new Date(date).toISOString();
    }

    function importNotes() {
        const reader = new FileReader();
        reader.readAsText(importNotesInput.files[0]);
        reader.onload = async () => {
            let notesJson = reader.result;
            await ApiService.bulkCreateNotes(notesJson);
        }
        listButton.click();
    }

    function exportNotes() {
        let notesJson = JSON.stringify(_notes);
        let blob = new Blob([notesJson], {
            type: "application/json"
        });
        let a = document.createElement("a");
        a.href = URL.createObjectURL(blob);
        a.download = "exportedNotes-[date].json";
        a.click();
        a.remove();
    }

    function setCheckboxesAndTextForSelectedNote() {
        setCheckboxes(getSelectedNote());
        setTextElementValue(getSelectedNote()?.text);
    }

    function getNote(noteId) {
        return _notes.find(note => note.id === noteId);
    }

    function getSelectedNote() {
        return getNote(_selectedNoteId);
    }
    
    function changeNote(note) {
        let oldNote = getNote(note.id)
        let noteIndex = _notes.indexOf(oldNote);
        _notes[noteIndex] = note;
    }

    function sortByCreated() {
        _notes.sort((note1, note2) => new Date(note1.created) - new Date(note2.created));
        if (_sortByCreatedDescending) _notes.reverse();
        _sortByCreatedDescending = !_sortByCreatedDescending;
    }
    
    function sortByUpdated() {
        _notes.sort((note1, note2) => new Date(note1.updated) - new Date(note2.updated));
        if (_sortByUpdatedDescending) _notes.reverse();
        _sortByUpdatedDescending = !_sortByUpdatedDescending;
    }
    
    function sortByText() {
        _notes.sort((note1, note2) => note1.text > note2.text ? 1 : -1);
        if (_sortByTextDescending) _notes.reverse();
        _sortByTextDescending = !_sortByTextDescending;
    }
    
    function sortByPinned() {
        _notes.sort((note1, note2) => note1.pinned === note2.pinned ? 0 : note1.pinned ? -1 : 1);
    }

    async function showHistory() {
        if (historyDiv.style.display == 'flex') {
            cancelHistoryButton.click();
            return;
        }
        
        _snapshots = await ApiService.getAllSnapshots(_selectedNoteId);

        _oldNoteText = getTextElementValue();
        historySlider.setAttribute('min', '0');
        historySlider.setAttribute('max', (_snapshots.length - 1).toString());
        historySlider.value = _snapshots.length - 1;

        snapshotDateDiv.textContent = _snapshots[historySlider.value].created;
        setTextElementValue(_snapshots[historySlider.value].text);

        historyDiv.style.display = 'flex';
    }
}

export default NotesView;