import ApiService from "../ApiService.js";
import AppData from "../AppData.js";
import Utils from "../Utils.js";
import Config from "../Config.js";

let NotesView = { render, init }

let _notes = [];
let _selectedNote = null;
let _snapshots;
let _showArchived = false;
let _oldNoteText;
let _notesCurrentlyBeingSaved = new Set();

const NotesSorting = Object.freeze({
    ByCreatedAscending: 0,
    ByCreatedDescending: 1,
    ByUpdatedAscending: 2,
    ByUpdatedDescending: 3,
    ByTextAscending: 4,
    ByTextDescending: 5
});
const SyncStatus = Object.freeze({
    Synchronizing: 0,
    Synchronized: 1,
    NotSynchronized: 2
});

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
                            <div id="sync-status"></div>
                            <!--<div id="save-button" class="btn">Save</div>-->
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
    //const saveButton = document.getElementById('save-button');
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
    const syncStatus = document.getElementById('sync-status');

    addListeners();

    setNotes(AppData.readNotes());
    await listNotes();
    enableAutoRelisting(Config.AUTO_RELISTING_MS);

    function addListeners() {
        listButton.addEventListener('click', listNotes);
        newButton.addEventListener('click', createNote);
        //saveButton.addEventListener('click', async () => { if (canSave()) await saveNote(_selectedNote); });
        deleteButton.addEventListener('click', deleteNote);
        lockButton.addEventListener('change', toggleLocked);
        archiveButton.addEventListener('change', toggleArchived);
        pinButton.addEventListener('change', togglePinned);
        publishButton.addEventListener('change', togglePublished);
        copyLinkButton.addEventListener('click', copyLink);

        sortByCreatedButton.addEventListener('click', sortByCreated);
        sortByUpdatedButton.addEventListener('click', sortByUpdated);
        sortByTextButton.addEventListener('click', sortByText);

        importButton.addEventListener('click', () => importInput.click());
        importInput.addEventListener('change', importNotes);
        exportButton.addEventListener('click', exportNotes);

        notesListDiv.addEventListener('click', noteSelectedHandler);
        toggleShowArchivedButton.addEventListener('click', toggleShowArchived);

        historyButton.addEventListener('click', showHistory);
        cancelHistoryButton.addEventListener('click', cancelHistory);
        historySlider.addEventListener('input', updateHistorySlider);
        restoreSnapshotButton.addEventListener('click', restoreSnapshot);

        actionMenuButton.addEventListener('click', showActionMenu);
        document.addEventListener('click', hideActionMenuIfClickedAway);
        noteTextElement.addEventListener('input', updateSelectedNoteText);
        noteTextElement.addEventListener('input', saveAfterDelay);
    }

    function getShownNotes() {
        let notes = _notes.slice();
        notes = notes.filter(note => (note.archived === _showArchived) && !note.deleted);
        sortByChosenProperty(notes);
        notes.sort((noteLeft, noteRight) => noteLeft.pinned === noteRight.pinned ? 0 : noteLeft.pinned ? -1 : 1);
        return notes;
    }

    async function listNotes() {
        selectFirstNote();
        let notes = await ApiService.getNotes();
        if (notes != null) {
            await synchronizeNotes(notes);
            updateSyncStatus();
            selectFirstNote();
        }
    }

    async function createNote() {
        let newLocalNote = new Note(_showArchived);

        addNote(newLocalNote);
        selectNote(newLocalNote);
        noteTextElement.focus();

        let newNote = await ApiService.createNote(newLocalNote);
        if (newNote != null) {
            updateNote(newLocalNote, newNote);
        }

        AppData.saveNotes(_notes);
    }

    async function saveNote(note) {
        note.synchronized = false;
        note.updatedLocal = new Date();
        updateNote(note, note);

        let updatedNote = await createOrUpdateNote(note);
        if (updatedNote != null) {
            updateNote(note, updatedNote);
        }

        AppData.saveNotes(_notes);
        return updatedNote;
    }

    async function saveAfterDelay() {
        if (canSave()) {
            let note = _selectedNote;

            _notesCurrentlyBeingSaved.add(note);
            updateSyncStatus();

            await delay(Config.SAVE_DELAY_MS);
            await saveNote(note);

            _notesCurrentlyBeingSaved.delete(note);
            updateSyncStatus();
        }

        function delay(ms) {
            return new Promise(resolve => setTimeout(resolve, ms));
        }
    }

    function canSave() {
        return _selectedNote != null && !_selectedNote.locked && !historyVisible() &&
            !_notesCurrentlyBeingSaved.has(_selectedNote);
    }

    async function deleteNote() {
        let note = _selectedNote;
        if (note.local) {
            removeNote(note);
            selectFirstNote();
            actionMenu.classList.remove('show');
        }
        else {
            note.deleted = true;
            note.synchronized = true;
            selectFirstNote();
            actionMenu.classList.remove('show');
            removeNoteElement(note);
            if (await ApiService.deleteNote(note.id)) {
                removeNote(note);
            }
        }

        AppData.saveNotes(_notes);
    }

    async function toggleLocked() {
        _selectedNote.locked = !_selectedNote.locked;
        await saveNote(_selectedNote);
    }

    async function togglePinned() {
        _selectedNote.pinned = !_selectedNote.pinned;
        let updatedNote = await saveNote(_selectedNote);
        addNoteElements(); // to resort the notes by pinned
        selectNote(updatedNote);
    }

    async function toggleArchived() {
        let note = _selectedNote;
        _selectedNote.archived = !_selectedNote.archived;
        removeNoteElement(note);
        selectFirstNote();
        actionMenu.classList.remove('show');
        await saveNote(note);
    }

    async function togglePublished() {
        let note = _selectedNote;
        let noteCopy = JSON.parse(JSON.stringify(note));
        noteCopy.published = !noteCopy.published;

        let updatedNote = await createOrUpdateNote(noteCopy);
        if (updatedNote != null) {
            updateNote(note, updatedNote);
            AppData.saveNotes(_notes);
        }
    }

    function addNote(note) {
        _notes.push(note);
        if (note.archived === _showArchived) {
            notesListDiv.insertAdjacentHTML('beforeend', createNoteHtml(note));
        }
    }

    function setNotes(notes) {
        _notes = notes;
        addNoteElements();
        updateSelectedNoteElements();
    }

    function removeNote(note) {
        _notes.splice(_notes.indexOf(note), 1);
        removeNoteElement(note);
    }

    function removeNoteElement(note) {
        getNoteElement(note)?.remove();
    }

    function updateNote(oldNote, newNote) {
        changeNote(oldNote, newNote);
        let noteElement = getNoteElement(newNote);
        if (noteElement != null) {
            noteElement.outerHTML = createNoteHtml(newNote);
        }
        if (_selectedNote == oldNote) {
            selectNote(newNote);
        }
        updateSelectedNoteElements();
        updateSyncStatus();
    }

    function addNoteElements() {
        let notes = getShownNotes();
        let selectedNote = _selectedNote;
        notesListDiv.innerHTML = '';
        for (let note of notes) {
            notesListDiv.insertAdjacentHTML('beforeend', createNoteHtml(note));
        }
        selectNote(selectedNote);
    }

    function getNoteElement(note) {
        return document.getElementById(`note-${note?.elementId}`);
    }

    function getNoteElementId(noteElement) {
        let id = noteElement.id;
        return id.substring(id.indexOf('-') + 1);
    }

    function setTextElementValue(text) {
        noteTextElement.value = text ?? '';
    }

    function makeSelectedNoteReadOnly() {
        noteTextElement.readOnly = _selectedNote?.locked ?? false;
        //saveButton.disabled = _selectedNote?.locked ?? false;
    }

    function updateSyncStatus() {
        let status;
        switch (getSyncStatus()) {
            case SyncStatus.Synchronizing:
                status = 'Synchronizing...'; break;
            case SyncStatus.Synchronized:
                status = 'All notes are synchronized'; break;
            case SyncStatus.NotSynchronized:
                status = 'Synchronization failed (changes saved locally)'; break;
        }
        syncStatus.textContent = status;
    }

    function setSelectedNoteCheckboxes() {
        document.getElementById("pin-button").checked = _selectedNote?.pinned ?? false;
        document.getElementById("lock-button").checked = _selectedNote?.locked ?? false;
        document.getElementById("archive-button").checked = _selectedNote?.archived ?? false;
        document.getElementById("publish-button").checked = _selectedNote?.published ?? false;
    }

    function createNoteHtml(note) {
        note.elementId = Utils.generateUniqueId();
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
                <div>Updated: ${Utils.dateToLocaleString(note.updatedLocal)}</div>
            </div>`
    }

    function updateSelectedNoteElements() {
        setTextElementValue(_selectedNote?.text);
        setSelectedNoteCheckboxes();
        makeSelectedNoteReadOnly();
    }

    function getNote(noteElement) {
        return _notes.find(note => note.elementId === getNoteElementId(noteElement));
    }

    function changeNote(oldNote, newNote) {
        newNote.elementId = oldNote.elementId;
        let noteIndex = _notes.indexOf(oldNote);
        _notes[noteIndex] = newNote;
    }

    function selectNote(note) {
        getNoteElement(_selectedNote)?.classList.remove('note-selected');
        getNoteElement(note)?.classList.add('note-selected');
        _selectedNote = note;
        updateSelectedNoteElements();
    }

    function selectFirstNote() {
        let shownNotes = getShownNotes();
        if (shownNotes.includes(_selectedNote)) return;
        if (shownNotes.length > 0) {
            selectNote(shownNotes[0]);
        }
        else {
            _selectedNote = null;
            updateSelectedNoteElements();
        }
    }

    function noteSelectedHandler(event) {
        cancelHistory();
        let noteElement = event.target.closest('.note');
        selectNote(getNote(noteElement));
    }

    function historyVisible() {
        return historyDiv.classList.contains('show');
    }

    async function showHistory() {
        if (historyVisible()) {
            cancelHistory();
            return;
        }

        _snapshots = await ApiService.getAllSnapshots(_selectedNote.id);
        if (_snapshots == null) return;

        _oldNoteText = _selectedNote.text;
        historySlider.setAttribute('min', '0');
        historySlider.setAttribute('max', (_snapshots.length - 1).toString());
        historySlider.value = _snapshots.length - 1;

        updateHistorySlider();
        historyDiv.classList.add('show');
    }

    function cancelHistory() {
        setTextElementValue(_oldNoteText);
        historyDiv.classList.remove('show');
    }

    function updateHistorySlider() {
        snapshotDateDiv.textContent = Utils.dateToLocaleString(_snapshots[historySlider.value].created);
        setTextElementValue(_snapshots[historySlider.value].text);
    }

    async function restoreSnapshot() {
        setTextElementValue(_snapshots[historySlider.value].text);
        await saveNote(_selectedNote);
        historyDiv.classList.remove('show');
        _oldNoteText = null;
    }

    function sortByCreated() {
        sort(NotesSorting.ByCreatedAscending, NotesSorting.ByCreatedDescending);
    }

    function sortByUpdated() {
        sort(NotesSorting.ByUpdatedAscending, NotesSorting.ByUpdatedDescending);
    }

    function sortByText() {
        sort(NotesSorting.ByTextAscending, NotesSorting.ByTextDescending);
    }

    function sort(sortingAscending, sortingDescending) {
        let userInfo = AppData.getUserInfo();
        userInfo.notesSorting = userInfo.notesSorting === sortingAscending
            ? sortingDescending : sortingAscending;
        AppData.saveUserInfo(userInfo);
        addNoteElements();
    }

    function sortByChosenProperty(notes) {
        let compareFn;
        let sorting = AppData.getUserInfo().notesSorting;
        switch (sorting) {
            case NotesSorting.ByCreatedAscending:
                compareFn = (note1, note2) => new Date(note1.created) - new Date(note2.created); break;
            case NotesSorting.ByCreatedDescending:
                compareFn = (note1, note2) => new Date(note2.created) - new Date(note1.created); break;
            case NotesSorting.ByUpdatedAscending:
                compareFn = (note1, note2) => new Date(note1.updatedLocal) - new Date(note2.updatedLocal); break;
            case NotesSorting.ByUpdatedDescending:
                compareFn = (note1, note2) => new Date(note2.updatedLocal) - new Date(note1.updatedLocal); break;
            case NotesSorting.ByTextAscending:
                compareFn = (note1, note2) => note1.text.localeCompare(note2.text); break;
            case NotesSorting.ByTextDescending:
                compareFn = (note1, note2) => note2.text.localeCompare(note1.text); break;
            default:
                compareFn = (note1, note2) => 0; break;
        }
        notes.sort(compareFn);
    }

    async function exportNotes() {
        AppData.exportNotes(_notes);
    }

    async function importNotes() {
        let importedNotes = await AppData.importNotes(importInput.files[0]);

        if (importedNotes != null) {
            importedNotes.forEach(note => {
                note.id = 0;
                note.local = true;
                note.synchronized = false;
            })

            setNotes(_notes.concat(importedNotes));
            AppData.saveNotes(_notes);

            if (await ApiService.bulkCreateNotes(importedNotes)) {
                await listNotes();
            }
        }
    }

    async function toggleShowArchived() {
        _showArchived = !_showArchived;
        addNoteElements();
        selectFirstNote();
        toggleShowArchivedButton.classList.toggle('show-archived');
    }

    function showActionMenu() {
        copyLinkButton.textContent = 'Copy Link';
        actionMenu.classList.toggle('show');
    }

    function hideActionMenuIfClickedAway(event) {
        if (!event.target.matches('#action-menu-button, #action-menu *')) {
            actionMenu.classList.remove('show');
        }
    }

    async function copyLink() {
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

    function updateSelectedNoteText() {
        _selectedNote.text = noteTextElement.value;
    }

    async function synchronizeNotes(fetchedNotes) {
        let localNotes = _notes.slice();
        let joinedNotes = [];
        for (let fetchedNote of fetchedNotes) {
            let localNote = localNotes.find(local => local.id == fetchedNote.id);
            if (localNote) {
                joinedNotes.push({ local: localNote, fetched: fetchedNote });
            }
        }

        // process notes that exist both locally and on the server
        for (let note of joinedNotes) {
            await synchronizeJoinedNote(note);
        }

        // process notes that were fetched from the server but don't exist locally
        for (let fetchedNote of getFetchedOnlyNotes(fetchedNotes, joinedNotes)) {
            addNote(fetchedNote);
        }

        // process notes that exist locally but were not fetched from the server
        for (let localNote of getLocalOnlyNotes(localNotes, joinedNotes)) {
            await synchronizeLocalNote(localNote);
        }

        AppData.saveNotes(_notes);
    }

    async function synchronizeJoinedNote(note) {
        if (note.local.synchronized)
        {
            if (!notesInSync(note.fetched, note.local))
            {
                // fetched note is newer than local copy; update local note
                updateNote(note.local, note.fetched);
            }
            else
            {
                // note is up-to-date with the server; resend delete request for locally deleted note
                if (note.local.deleted) {
                    if (await ApiService.deleteNote(note.local.id)) {
                        removeNote(note.local);
                    }
                }
            }
        }
        else
        {
            if (notesInSync(note.fetched, note.local))
            {
                // synchronizing local note with the server
                let updatedNote = await ApiService.updateNote(note.local);
                if (updatedNote != null)
                {
                    updateNote(note.local, updatedNote);
                }
            }
            else
            {
                // there are unsynchronized local and remote versions of the same note.
                // keep the fetched version and send a request to create a new note for the local version
                let newNote = await ApiService.createNote(note.local);

                if (newNote != null)
                {
                    addNote(newNote);
                    updateNote(note.local, note.fetched);
                }
            }
        }
    }

    async function synchronizeLocalNote(localNote) {
        if (localNote.synchronized || localNote.deleted)
        {
            // note was deleted on the server; remove local note
            removeNote(localNote);
        }
        else
        {
            // note was never sent to the server, or changes made locally were not synchronized
            // before the note was deleted on the server.
            // send a request to create a new note with local note's text
            let newNote = await ApiService.createNote(localNote);
            if (newNote != null)
            {
                updateNote(localNote, newNote);
            }
        }
    }

    function getFetchedOnlyNotes(fetchedNotes, joinedNotes) {
        return fetchedNotes.filter(fetchedNote =>
            joinedNotes.find(note => note.fetched.id == fetchedNote.id) == undefined);
    }

    function getLocalOnlyNotes(localNotes, joinedNotes) {
        return localNotes.filter(localNote =>
            (joinedNotes.find(note => note.local.id == localNote.id) == undefined) || localNote.local);
    }

    function notesInSync(noteLeft, noteRight) {
        return noteLeft.updated == noteRight.updated &&
            noteLeft.pinned == noteRight.pinned &&
            noteLeft.locked == noteRight.locked &&
            noteLeft.archived == noteRight.archived &&
            noteLeft.publicUrl == noteRight.publicUrl
    }

    async function createOrUpdateNote(note) {
        return note.local ? await ApiService.createNote(note) : await ApiService.updateNote(note);
    }

    function getSyncStatus() {
        return _notesCurrentlyBeingSaved.size > 0 ? SyncStatus.Synchronizing
            : _notes.some(note => !note.synchronized) ? SyncStatus.NotSynchronized : SyncStatus.Synchronized;
    }

    function enableAutoRelisting(ms) {
        clearInterval(window.autoRelistingInterval);
        window.autoRelistingInterval = setInterval(listNotes, ms);
    }

    function Note(archived) {
        this.id = 0;
        this.text = '';
        this.pinned = false;
        this.locked = false;
        this.archived = archived;
        this.published = false;
        this.synchronized = false;
        this.local = true;
        this.deleted = false;
        this.updatedLocal = new Date();
    }
}

export default NotesView;