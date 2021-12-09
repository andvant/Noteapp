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
    }

    function addNote(newNote) {
        _notes.push(newNote);
        addNoteElement(newNote);
    }

    function addNoteElement(note) {
        notesListDiv.insertAdjacentHTML('beforeend', createNoteHtml(note));
    }

    function removeNote(noteId) {
        let noteIndex = _notes.indexOf(getNote(noteId))
        _notes.splice(noteIndex, 1);
        removeNoteElement(noteId);
        actionMenu.classList.remove('show');
    }

    function removeNoteElement(noteId) {
        let noteDiv = document.getElementById(`note-${noteId}`);
        noteDiv.remove();
    }

    function updateNote(updatedNote) {
        changeNote(updatedNote);
        updateNoteElement(updatedNote);
        selectNote(getNoteElement(updatedNote.id));
        updateSelectedNoteElements();
    }

    function updateNoteElement(note) {
        getNoteElement(note.id).outerHTML = createNoteHtml(note);
    }

    function getNoteElement(noteId) {
        return document.getElementById(`note-${noteId}`);
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

    function getNoteId(noteElement) {
        return parseInt(noteElement.id.split('-')[1]);
    }

    function getTextElementValue() {
        return noteTextElement.value;
    }

    function setTextElementValue(text) {
        noteTextElement.value = text ?? "";
    }

    function makeSelectedNoteReadOnly() {
        let note = getSelectedNote();
        noteTextElement.readOnly = note?.locked ?? false;
        saveButton.disabled = note?.locked ?? false;
    }

    function selectNote(noteElement) {
        getNoteElement(_selectedNoteId)?.classList.remove('note-selected');
        noteElement.classList.add('note-selected');
        _selectedNoteId = getNoteId(noteElement);
        updateSelectedNoteElements();
    }

    function setSelectedNoteCheckboxes() {
        let note = getSelectedNote();
        document.getElementById("pin-button").checked = note?.pinned ?? false;
        document.getElementById("lock-button").checked = note?.locked ?? false;
        document.getElementById("archive-button").checked = note?.archived ?? false;
        document.getElementById("publish-button").checked = note?.published ?? false;
    }

    function createNoteHtml(note) {
        let textPreview = note.text == '' ? "New note..." : note.text?.split(/\r?\n/)[0]?.substring(0, 30);

        return /*html*/ `
            <div id="note-${note.id}" class="note">
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

    function dateToLocaleString(date) {
        let options = { year: 'numeric', month: 'long', day: 'numeric', hour: 'numeric', minute: 'numeric' };
        return new Date(date).toLocaleString({}, options);
    }

    function updateSelectedNoteElements() {
        setTextElementValue(getSelectedNote()?.text);
        setSelectedNoteCheckboxes();
        makeSelectedNoteReadOnly();
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

    function sortByPinned() {
        _notes.sort((note1, note2) => note1.pinned === note2.pinned ? 0 : note1.pinned ? -1 : 1);
    }

    function selectFirstNote() {
        if (notesListDiv.childElementCount > 0) {
            selectNote(notesListDiv.firstElementChild)
        }
        else {
            _selectedNoteId = null;
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
        let newNote = await ApiService.createNote();
        if (!_showArchived) {
            addNote(newNote);
            selectNote(getNoteElement(newNote.id));
            noteTextElement.focus();
        }
    }

    async function saveButtonHandler() {
        let updatedNote = await ApiService.updateNote(_selectedNoteId, getTextElementValue());
        updateNote(updatedNote);
    }

    async function deleteButtonHandler() {
        await ApiService.deleteNote(_selectedNoteId);
        removeNote(_selectedNoteId);
        selectFirstNote();
    }

    async function lockCheckboxHandler() {
        let updatedNote = await ApiService.toggleLocked(getSelectedNote());
        updateNote(updatedNote);
    }

    async function archiveCheckboxHandler() {
        let updatedNote = await ApiService.toggleArchived(getSelectedNote());
        updateNote(updatedNote);
        removeNote(updatedNote.id);
        selectFirstNote();
    }

    async function pinCheckboxHandler() {
        let updatedNote = await ApiService.togglePinned(getSelectedNote());
        updateNote(updatedNote);
        addNoteElements();
        selectNote(getNoteElement(updatedNote.id));
    }

    async function publishCheckboxHandler() {
        let updatedNote = await ApiService.togglePublished(getSelectedNote());
        updateNote(updatedNote);
    }

    function noteSelectedHandler(event) {
        cancelHistoryButtonHandler();
        let noteElement = event.target.closest('.note');
        selectNote(noteElement);
    }

    async function historyButtonHandler() {
        if (historyDiv.classList.contains('show')) {
            cancelHistoryButtonHandler();
            return;
        }

        _snapshots = await ApiService.getAllSnapshots(_selectedNoteId);

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
        await saveButtonHandler();
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
            let notesJson = reader.result;
            await ApiService.bulkCreateNotes(notesJson);
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
        let note = getSelectedNote();
        if (note?.published) {
            await navigator.clipboard.writeText(getFullNoteUrl(note.publicUrl));
            copyLinkButton.textContent = 'Copied!';
        }
        else {
            copyLinkButton.textContent = 'Note is not published!';
        }
    }

    function getFullNoteUrl(publicUrl) {
        return `${window.location.protocol}//${window.location.host}/p/${publicUrl}`;
    }
}

export default NotesView;