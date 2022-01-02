import ApiService from "../ApiService.js";

let NotesView = { render, init }

let _notes;
let _selectedNote;
let _snapshots;
let _showArchived = false;
let _oldNoteText;
let _sortByUpdatedDescending = false;
let _sortByCreatedDescending = false;
let _sortByTextDescending = false;

const SAVE_DELAY_MS = 1000;
let _notesCurrentlyBeingSaved = new Set();

async function render() {
    return /*html*/ `
        <div id="notes-view">
            <div id="notes-all">

                <div id="notes-actions">
                    <div id="toggle-show-archived-button" class="btn">Archived</div>
                    <div id="new-button" class="btn">+</div>
                    <div id="list-button" class="btn">List</div>
                </div>

                <div id="notes-sort">
                    <label>Sort by:</label>
                    <div id="sortby-created-button" class="btn">Created</div>
                    <div id="sortby-updated-button" class="btn">Updated</div>
                    <div id="sortby-text-button" class="btn">Text</div>
                </div>

                <div id="notes-list"></div>

            </div>
            <div id="selected-note-column">

                <div id="selected-note">
                    <div id="selected-note-actions">
                        <div id="save-button-div">
                            <div id="save-button" class="btn">Save</div>
                        </div>

                        <div id="selected-note-menu">
                            <div class="dropdown">
                                <img id="action-menu-button" src="../../menu.svg" class="menu-icon"/>
                                <div id="action-menu">
                                    <label for="pin-button" class="action">Pin to top
                                        <input id="pin-button" type="checkbox" />
                                    </label>
                                    <label for="lock-button" class="action">Lock
                                        <input id="lock-button" type="checkbox" />
                                    </label>
                                    <label for="archive-button" class="action">Archive
                                        <input id="archive-button" type="checkbox" />
                                    </label>
                                    <label for="publish-button" class="action">Publish
                                        <input id="publish-button" type="checkbox" />
                                    </label>
                                    <div id="copy-link-button" class="action">Copy Link</div>
                                    <div id="history-button" class="action">Note history</div>
                                    <div id="export-button" class="action">Export</div>
                                    <div id="import-button" class="action">Import</div>
                                    <input id="import-input" type="file" style="display: none;" />
                                    <div id="delete-button" class="action">Delete</div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="selected-note-text">
                        <textarea id="note-text" placeholder="New note..."></textarea>
                    </div>

                </div>

                <div id="note-history">
                    <div id="snapshot-date"></div>
                    <input type="range" id="history-slider" step="1" />
                    <div id="note-history-buttons">
                        <div id="cancel-history-button" class="btn">Cancel</div>
                        <div id="restore-snapshot-button" class="btn">Restore</div>
                    </div>
                </div>
            </div>
        </div>`
}

async function init() {

    const listButton = document.getElementById('list-button');
    const newButton = document.getElementById('new-button');
    const saveButton = document.getElementById('save-button');
    const deleteButton = document.getElementById('delete-button');
    const pinButton = document.getElementById('pin-button');
    const lockButton = document.getElementById('lock-button');
    const archiveButton = document.getElementById('archive-button');
    const publishButton = document.getElementById('publish-button');
    const copyLinkButton = document.getElementById('copy-link-button');
    const sortByCreatedButton = document.getElementById('sortby-created-button');
    const sortByUpdatedButton = document.getElementById('sortby-updated-button');
    const sortByTextButton = document.getElementById('sortby-text-button');
    const importButton = document.getElementById('import-button');
    const importInput = document.getElementById('import-input');
    const exportButton = document.getElementById('export-button');
    const notesListDiv = document.getElementById('notes-list')
    const noteTextElement = document.getElementById('note-text');
    const toggleShowArchivedButton = document.getElementById('toggle-show-archived-button');

    const historyDiv = document.getElementById('note-history');
    const snapshotDateDiv = document.getElementById('snapshot-date');
    const historySlider = document.getElementById('history-slider');
    const historyButton = document.getElementById('history-button');
    const cancelHistoryButton = document.getElementById('cancel-history-button');
    const restoreSnapshotButton = document.getElementById('restore-snapshot-button');

    const actionMenuButton = document.getElementById('action-menu-button');
    const actionMenu = document.getElementById('action-menu');

    addListeners();
    await listButtonHandler();

    selectFirstNote();

    function addListeners() {
        listButton.addEventListener('click', listButtonHandler);
        newButton.addEventListener('click', newButtonHandler);
        saveButton.addEventListener('click', saveButtonHandler);
        deleteButton.addEventListener('click', deleteButtonHandler);
        lockButton.addEventListener('change', lockCheckboxHandler);
        archiveButton.addEventListener('change', archiveCheckboxHandler);
        pinButton.addEventListener('change', pinCheckboxHandler);
        publishButton.addEventListener('change', publishCheckboxHandler);
        copyLinkButton.addEventListener('click', copyLinkButtonHandler);

        sortByCreatedButton.addEventListener('click', sortByCreatedButtonHandler);
        sortByUpdatedButton.addEventListener('click', sortByUpdatedButtonHandler);
        sortByTextButton.addEventListener('click', sortByTextButtonHandler);

        importButton.addEventListener('click', () => importInput.click());
        importInput.addEventListener('change', importHandler);
        exportButton.addEventListener('click', exportHandler);

        notesListDiv.addEventListener('click', noteSelectedHandler);
        toggleShowArchivedButton.addEventListener('click', toggleShowArchivedButtonHandler);

        historyButton.addEventListener('click', historyButtonHandler);
        cancelHistoryButton.addEventListener('click', cancelHistoryButtonHandler);
        historySlider.addEventListener('input', historySliderHandler);
        restoreSnapshotButton.addEventListener('click', restoreSnapshotButtonHandler);

        actionMenuButton.addEventListener('click', actionMenuButtonHandler);
        document.addEventListener('click', hideActionMenuIfClickedAway);
        noteTextElement.addEventListener('input', saveAfterDelay);
    }

    function addNote(newNote) {
        _notes.push(newNote);
        addNoteElement(newNote);
    }

    function addNoteElement(note) {
        notesListDiv.insertAdjacentHTML('beforeend', createNoteHtml(note));
    }

    function removeNote() {
        let noteIndex = _notes.indexOf(_selectedNote)
        _notes.splice(noteIndex, 1);
        getNoteElement(_selectedNote).remove();
        actionMenu.classList.remove('show');
    }

    function updateNote(updatedNote) {
        changeNote(updatedNote);
        updateNoteElement(updatedNote);
        selectNote(updatedNote);
        updateSelectedNoteElements();
    }

    function updateNoteElement(note) {
        getNoteElement(note).outerHTML = createNoteHtml(note);
    }

    function getNoteElement(note) {
        return document.getElementById(`note-${note?.elementId}`);
    }

    function addNotes() {
        addNoteElements();
        updateSelectedNoteElements();
    }

    function addNoteElements() {
        sortByPinned();
        notesListDiv.innerHTML = '';
        for (let note of _notes) {
            notesListDiv.insertAdjacentHTML('beforeend', createNoteHtml(note));
        }
    }

    function getNoteElementId(noteElement) {
        let id = noteElement.id;
        return id.substring(id.indexOf('-') + 1);
    }

    function getTextElementValue() {
        return noteTextElement.value;
    }

    function setTextElementValue(text) {
        noteTextElement.value = text ?? "";
    }

    function makeSelectedNoteReadOnly() {
        noteTextElement.readOnly = _selectedNote?.locked ?? false;
        saveButton.disabled = _selectedNote?.locked ?? false;
    }

    function selectNote(note) {
        getNoteElement(_selectedNote)?.classList.remove('note-selected');
        getNoteElement(note).classList.add('note-selected');
        _selectedNote = note;
        updateSelectedNoteElements();
    }

    function setSelectedNoteCheckboxes() {
        document.getElementById("pin-button").checked = _selectedNote?.pinned ?? false;
        document.getElementById("lock-button").checked = _selectedNote?.locked ?? false;
        document.getElementById("archive-button").checked = _selectedNote?.archived ?? false;
        document.getElementById("publish-button").checked = _selectedNote?.published ?? false;
    }

    function createNoteHtml(note) {
        note.elementId = generateNoteElementId();
        let textPreview = note.text == '' ? "New note..." : note.text?.split(/\r?\n/)[0]?.substring(0, 30);

        return /*html*/ `
            <div id="note-${note.elementId}" class="note">
                <div class="note-flags">
                    <label class="note-pinned" style="display: ${note.pinned ? "inline-block" : "none"};">Pinned</label>
                    <label class="note-archived" style="display: ${note.archived ? "inline-block" : "none"};">Archived</label>
                    <label class="note-locked" style="display: ${note.locked ? "inline-block" : "none"};">Locked</label>
                    <label class="note-published" style="display: ${note.published ? "inline-block" : "none"};">Published</label>
                </div>
                <div><strong>${textPreview}</strong></div>
                <div>Updated: ${dateToLocaleString(note.updated)}</div>
            </div>`
    }

    function generateNoteElementId()
    {
        const ALPHABET = 'abcdef0123456789';
        const ELEMENT_ID_LENGTH = 8;

        let elementId = '';

        for (let i = 0; i < ELEMENT_ID_LENGTH; i++)
        {
            let index = Math.floor((Math.random() * 1_000_000_000) % ALPHABET.length)
            elementId += ALPHABET[index];
        }

        return elementId;
    }

    function dateToLocaleString(date) {
        let options = { year: 'numeric', month: 'long', day: 'numeric', hour: 'numeric', minute: 'numeric' };
        return new Date(date).toLocaleString({}, options);
    }

    function updateSelectedNoteElements() {
        setTextElementValue(_selectedNote?.text);
        setSelectedNoteCheckboxes();
        makeSelectedNoteReadOnly();
    }

    function getNote(noteElement) {
        return _notes.find(note => note.elementId === getNoteElementId(noteElement));
    }

    // TODO: think if need to change implementation
    function changeNote(newNote) {
        let oldNote = _notes.find(note => note.elementId === newNote.elementId);
        let noteIndex = _notes.indexOf(oldNote);
        _notes[noteIndex] = newNote;
    }

    function sortByPinned() {
        _notes.sort((note1, note2) => note1.pinned === note2.pinned ? 0 : note1.pinned ? -1 : 1);
    }

    function selectFirstNote() {
        if (notesListDiv.childElementCount > 0) {
            selectNote(getNote(notesListDiv.firstElementChild))
        }
        else {
            _selectedNote = null;
            updateSelectedNoteElements();
        }
    }

    // Event handlers
    async function listButtonHandler() {
        let notes = await ApiService.getNotes(_showArchived);
        _notes = notes;
        addNotes();
        selectFirstNote();
    }

    async function newButtonHandler() {
        // TODO: write a constructor function
        let newLocalNote = {
            id: 0,
            elementId: generateNoteElementId(),
            text: '',
            pinned: false,
            locked: false,
            archived: false,
            published: false,
            synchronized: false,
            local: true // TODO: think if needed
        };
        let newNote = await ApiService.createNote(newLocalNote);
        newNote = newLocalNote.elementId; // TODO: think if belongs here
        if (!_showArchived) {
            addNote(newNote);
            selectNote(newNote);
            noteTextElement.focus();
        }
    }

    async function saveButtonHandler() {
        if (!canSave()) return;
        await save(_selectedNote);
    }

    async function save(note) {
        note.text = getTextElementValue();
        let updatedNote = await ApiService.updateNote(note);
        updatedNote.elementId = note.elementId; // TODO: think if setting elementId belongs here
        updateNote(updatedNote);
    }

    async function deleteButtonHandler() {
        await ApiService.deleteNote(_selectedNote.id);
        removeNote(_selectedNote.elementId);
        selectFirstNote();
    }

    async function lockCheckboxHandler() {
        _selectedNote.locked = !_selectedNote.locked;
        await save(_selectedNote);
    }

    async function archiveCheckboxHandler() {
        _selectedNote.archived = !_selectedNote.archived;
        await save(_selectedNote);
        // removeNote(updatedNote.id);
        // selectFirstNote();
    }

    // TODO: resort notes afterwards
    async function pinCheckboxHandler() {
        _selectedNote.pinned = !_selectedNote.pinned;
        await save(_selectedNote);
        // addNoteElements();
        // selectNote(getNoteElement(updatedNote.id));
    }

    async function publishCheckboxHandler() {
        _selectedNote.published = !_selectedNote.published;
        await save(_selectedNote);
    }

    function noteSelectedHandler(event) {
        cancelHistoryButtonHandler();
        let noteElement = event.target.closest('.note');
        selectNote(getNote(noteElement));
    }

    function historyVisible() {
        return historyDiv.classList.contains('show');
    }

    async function historyButtonHandler() {
        if (historyVisible()) {
            cancelHistoryButtonHandler();
            return;
        }

        let noteId = _selectedNote.id;
        _snapshots = await ApiService.getAllSnapshots(noteId);

        _oldNoteText = getTextElementValue();
        historySlider.setAttribute('min', '0');
        historySlider.setAttribute('max', (_snapshots.length - 1).toString());
        historySlider.value = _snapshots.length - 1;

        historySliderHandler();

        historyDiv.classList.add('show');
    }

    function cancelHistoryButtonHandler() {
        setTextElementValue(_oldNoteText);
        historyDiv.classList.remove('show');
    }

    function historySliderHandler() {
        snapshotDateDiv.textContent = dateToLocaleString(_snapshots[historySlider.value].created);
        setTextElementValue(_snapshots[historySlider.value].text);
    }

    async function restoreSnapshotButtonHandler() {
        setTextElementValue(_snapshots[historySlider.value].text);
        await save(_selectedNote);
        historyDiv.classList.remove('show');
        _oldNoteText = null;
    }

    function sortByCreatedButtonHandler() {
        _notes.sort((note1, note2) => new Date(note2.created) - new Date(note1.created));
        if (_sortByCreatedDescending) _notes.reverse();
        _sortByCreatedDescending = !_sortByCreatedDescending;
        _sortByUpdatedDescending = false;
        _sortByTextDescending = false;
        addNoteElements();
    }

    function sortByUpdatedButtonHandler() {
        _notes.sort((note1, note2) => new Date(note2.updated) - new Date(note1.updated));
        if (_sortByUpdatedDescending) _notes.reverse();
        _sortByUpdatedDescending = !_sortByUpdatedDescending;
        _sortByCreatedDescending = false;
        _sortByTextDescending = false;
        addNoteElements();
    }

    function sortByTextButtonHandler() {
        _notes.sort((note1, note2) => note1.text.localeCompare(note2.text));
        if (_sortByTextDescending) _notes.reverse();
        _sortByTextDescending = !_sortByTextDescending;
        _sortByCreatedDescending = false;
        _sortByUpdatedDescending = false;
        addNoteElements();
    }

    async function importHandler() {
        const reader = new FileReader();
        reader.readAsText(importInput.files[0]);
        reader.onload = async () => {
            let notes = JSON.parse(reader.result);
            await ApiService.bulkCreateNotes(notes);
            await listButtonHandler();
        };
    }

    async function exportHandler() {
        let allNotes = await ApiService.getNotes();
        let notesJson = JSON.stringify(allNotes, null, 2);
        let blob = new Blob([notesJson], {
            type: "application/json"
        });
        let a = document.createElement("a");
        a.href = URL.createObjectURL(blob);
        a.download = `ExportedNotes-${new Date().toLocaleDateString()}.json`;
        a.click();
    }

    async function toggleShowArchivedButtonHandler() {
        _showArchived = !_showArchived;
        await listButtonHandler();
        toggleShowArchivedButton.classList.toggle('show-archived');
    }

    function actionMenuButtonHandler() {
        copyLinkButton.textContent = 'Copy Link';
        actionMenu.classList.toggle('show');
    }

    function hideActionMenuIfClickedAway(event) {
        if (!event.target.matches('#action-menu-button, #action-menu *')) {
            actionMenu.classList.remove('show');
        }
    }

    async function copyLinkButtonHandler() {
        if (_selectedNote?.published) {
            await navigator.clipboard.writeText(getFullNoteUrl(_selectedNote.publicUrl));
            copyLinkButton.textContent = 'Copied!';
        }
        else {
            copyLinkButton.textContent = 'Note is not published!';
        }
    }

    function getFullNoteUrl(publicUrl) {
        return `${window.location.protocol}//${window.location.host}/p/${publicUrl}`;
    }

    function canSave() {
        return _selectedNote != null && !_selectedNote.locked && !historyVisible() &&
            !_notesCurrentlyBeingSaved.has(_selectedNote);
    }

    async function saveAfterDelay() {
        if (canSave()) {
            let note = _selectedNote;

            _notesCurrentlyBeingSaved.add(note);

            await delay(SAVE_DELAY_MS);
            await save(note);

            _notesCurrentlyBeingSaved.delete(note);
        }

        function delay(ms) {
            return new Promise(resolve => setTimeout(resolve, ms));
        }
    }
}

export default NotesView;