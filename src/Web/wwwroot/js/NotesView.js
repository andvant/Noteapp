import * as AjaxService from "./AjaxService.js";
import * as NoteService from "./NoteService.js";

export {
    render,
    init
}

async function render() {
    return `
        <div class="notes-view">
            <div class="notes">
                <div class="notes-new">
                    <button id="new-note-button">New note</button>
                    <button id="archived-view-button">Archived</button>
                </div>
                <div class="notes-sort">
                    <label>Sort by:</label>
                    <button id="sortby-created-button">Created</button>
                    <button id="sortby-updated-button">Updated</button>
                    <button id="sortby-text-button">Text</button>
                </div>
                <div id="notes-list"></div>
            </div>
            <div class="third-column">
                <div class="selected-note">
                    <div class="selected-note-actions">
                        <button id="save-note-button">Save note</button>
                        <button id="delete-note-button">Delete note</button>
                        <button id="pin-note-button">Pin note</button>
                        <input id="note-pinned" type="checkbox" disabled />
                        <button id="lock-note-button">Lock note</button>
                        <input id="note-locked" type="checkbox" disabled />
                        <button id="archive-note-button">Archive note</button>
                        <input id="note-archived" type="checkbox" disabled />
                        <button id="publish-note-button">Publish note</button>
                        <input id="note-published" type="checkbox" disabled />
                        <button id="import-notes-button">Import notes</button>
                        <input id="import-notes-input" type="file" style="display: none;" />
                        <button id="export-notes-button">Export notes</button>
                        <button id="history-button">History</button>
                    </div>
                    <div class="selected-note-text">
                        <textarea id="note-text" placeholder="Note text here..."></textarea>
                    </div>
                </div>
                <div class="note-history">
                    <input type="range" id="history-slider" min="0" max="9" step="1" />
                    <div id="snapshot-date"></div>
                    <button id="cancel-history-button">Cancel</button>
                    <button id="restore-snapshot-button">Restore</button>
                </div>
            </div>
        </div>`
}

async function init() {
    const saveNoteButton = document.getElementById('save-note-button');
    const newNoteButton = document.getElementById('new-note-button');
    const deleteNoteButton = document.getElementById('delete-note-button');
    const pinNoteButton = document.getElementById('pin-note-button');
    const lockNoteButton = document.getElementById('lock-note-button');
    const archiveNoteButton = document.getElementById('archive-note-button');
    const publishNoteButton = document.getElementById('publish-note-button');
    const sortByCreatedButton = document.getElementById('sortby-created-button');
    const sortByUpdatedButton = document.getElementById('sortby-updated-button');
    const sortByTextButton = document.getElementById('sortby-text-button');
    const importNotesButton = document.getElementById('import-notes-button');
    const importNotesInput = document.getElementById('import-notes-input');
    const exportNotesButton = document.getElementById('export-notes-button');
    const notesListDiv = document.getElementById('notes-list')
    const noteTextElement = document.getElementById('note-text');
    const archivedViewButton = document.getElementById('archived-view-button');

    // Note history
    const historyDiv = document.getElementsByClassName('note-history')[0];
    const snapshotDateDiv = document.getElementById('snapshot-date');
    const historySlider = document.getElementById('history-slider');
    const historyButton = document.getElementById('history-button');
    const cancelHistoryButton = document.getElementById('cancel-history-button');
    const restoreSnapshotButton = document.getElementById('restore-snapshot-button');
    let oldNoteText;
    let snapshots;

    let archivedView = false;

    addEventListeners();
    await updateNoteList();

    async function updateNoteList() {
        let notes = await AjaxService.getNotes();
        NoteService.saveLocalNotes(notes);

        addNoteElements(NoteService.getNotesForDisplay(notes, archivedView));
        setCheckboxes(NoteService.getSelectedNote());
    }

    function addEventListeners() {
        saveNoteButton.addEventListener('click', async () => {
            await AjaxService.updateNote(NoteService.getSelectedNoteId(), getSelectedNoteText());
            await updateNoteList();
        });

        newNoteButton.addEventListener('click', async () => {
            await AjaxService.createNote();
            await updateNoteList();
        });

        deleteNoteButton.addEventListener('click', async () => {
            await AjaxService.deleteNote(NoteService.getSelectedNoteId());
            await updateNoteList();
        });

        pinNoteButton.addEventListener('click', async () => {
            await AjaxService.togglePinned(NoteService.getSelectedNote());
            await updateNoteList();
        });

        lockNoteButton.addEventListener('click', async () => {
            await AjaxService.toggleLocked(NoteService.getSelectedNote());
            await updateNoteList();
        });

        archiveNoteButton.addEventListener('click', async () => {
            await AjaxService.toggleArchived(NoteService.getSelectedNote());
            await updateNoteList();
        });

        publishNoteButton.addEventListener('click', async () => {
            await AjaxService.togglePublished(NoteService.getSelectedNote());
            await updateNoteList();
        });

        sortByCreatedButton.addEventListener('click', () => {
            let notes = NoteService.getLocalNotes();
            NoteService.sortByCreated(notes);
            addNoteElements(NoteService.getNotesForDisplay(notes, archivedView));
        });

        sortByUpdatedButton.addEventListener('click', () => {
            let notes = NoteService.getLocalNotes();
            NoteService.sortByUpdated(notes);
            addNoteElements(NoteService.getNotesForDisplay(notes, archivedView));
        });

        sortByTextButton.addEventListener('click', () => {
            let notes = NoteService.getLocalNotes();
            NoteService.sortByText(notes);
            addNoteElements(NoteService.getNotesForDisplay(notes, archivedView));
        });

        importNotesButton.addEventListener('click', () => {
            importNotesInput.click();
        });

        importNotesInput.addEventListener('change', () => {
            const reader = new FileReader();
            reader.readAsText(importNotesInput.files[0]);
            reader.onload = async () => {
                let notesJson = reader.result;
                await AjaxService.bulkCreateNotes(notesJson);
            }
        });

        exportNotesButton.addEventListener('click', () => {
            let notesJson = JSON.stringify(NoteService.getLocalNotes());
            let blob = new Blob([notesJson], {
                type: "application/json"
            });
            let a = document.createElement("a");
            a.href = URL.createObjectURL(blob);
            a.download = "exportedNotes-[date].json";
            a.click();
        });

        archivedViewButton.addEventListener('click', async () => {
            archivedView = !archivedView;
            await updateNoteList();
        });

        // Note history
        historyButton.addEventListener('click', async () => {
            if (historyDiv.style.display == 'block') return;

            snapshots = await AjaxService.getAllSnapshots(NoteService.getSelectedNoteId());

            oldNoteText = getSelectedNoteText();
            historySlider.setAttribute('min', '0');
            historySlider.setAttribute('max', (snapshots.length - 1).toString());
            historySlider.value = snapshots.length - 1;

            snapshotDateDiv.textContent = snapshots[historySlider.value].date;
            setSelectedNoteText(snapshots[historySlider.value].text);

            historyDiv.style.display = 'block';
        });

        cancelHistoryButton.addEventListener('click', () => {
            setSelectedNoteText(oldNoteText);
            historyDiv.style.display = 'none';
        });

        historySlider.addEventListener('input', () => {
            snapshotDateDiv.textContent = snapshots[historySlider.value].created;
            setSelectedNoteText(snapshots[historySlider.value].text);
        });

        restoreSnapshotButton.addEventListener('click', () => {
            setSelectedNoteText(snapshots[historySlider.value].text);
            saveNoteButton.click();
            historyDiv.style.display = 'none';
            oldNoteText = null;
        });
    }

    function getNoteId(noteElement) {
        return noteElement.id.split('-')[1];
    }

    function getSelectedNoteText() {
        return noteTextElement.value;
    }

    function setSelectedNoteText(text) {
        noteTextElement.value = text;
    }

    function makeNoteReadOnly(locked) {
        noteTextElement.readOnly = locked ? true : false;
        saveNoteButton.disabled = locked ? true : false;
    }

    function addNoteElements(notes) {
        notesListDiv.innerHTML = '';

        for (let note of notes) {
            let noteDiv = document.createElement('div');
            noteDiv.classList.add('note');
            noteDiv.id = `note-${note.id}`;
            noteDiv.innerHTML = `
                <div>Id: ${note.id}</div>
                <div>AuthorId: ${note.authorId}</div>
                <div>Created: ${new Date(note.created).toISOString()}</div>
                <div>Updated: ${new Date(note.updated).toISOString()}</div>
                <div>PublicURL: ${note.publicUrl}</div>`;
            notesListDiv.append(noteDiv);
        }

        addNoteClickHandlers();
    }

    function noteSelectedHandler(event) {
        let noteElement = event.target.closest('.note');
        let noteId = getNoteId(noteElement);
        let note = NoteService.getLocalNote(noteId);

        NoteService.setSelectedNoteId(noteId);
        setSelectedNoteText(note.text);

        setCheckboxes(note);
        makeNoteReadOnly(note.locked);
    }

    function addNoteClickHandlers() {
        notesListDiv.addEventListener('click', noteSelectedHandler);
    }

    function setCheckboxes(note) {
        document.getElementById("note-pinned").checked = note.pinned;
        document.getElementById("note-locked").checked = note.locked;
        document.getElementById("note-archived").checked = note.archived;
        document.getElementById("note-published").checked = note.published;
    }
}
