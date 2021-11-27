import * as ajaxService from "./ajaxService.js";
import * as noteService from "./noteService.js";

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
const loginButton = document.getElementById('login-button');
const registerButton = document.getElementById('register-button');
const logoutButton = document.getElementById('logout-button');
const importNotesButton = document.getElementById('import-notes-button');
const importNotesInput = document.getElementById('import-notes-input');
const exportNotesButton = document.getElementById('export-notes-button');
const notesListDiv = document.getElementById('notes-list')
const noteTextElement = document.getElementById('note-text');
const archivedViewButton = document.getElementById('archived-view-button');

let archivedView = false;

// Note history
const historyDiv = document.getElementsByClassName('note-history')[0];
const snapshotDateDiv = document.getElementById('snapshot-date');
const historySlider = document.getElementById('history-slider');
const historyButton = document.getElementById('history-button');
const cancelHistoryButton = document.getElementById('cancel-history-button');
const restoreSnapshotButton = document.getElementById('restore-snapshot-button');
let oldNoteText;
let snapshots;

document.addEventListener('DOMContentLoaded', async () => {
    addEventListeners();
    await updateNoteList();
});

function addEventListeners() {
    saveNoteButton.addEventListener('click', async () => {
        await ajaxService.updateNote(noteService.getSelectedNoteId(), getSelectedNoteText());
        await updateNoteList();
    });

    newNoteButton.addEventListener('click', async () => {
        await ajaxService.createNote();
        await updateNoteList();
    });

    deleteNoteButton.addEventListener('click', async () => {
        await ajaxService.deleteNote(noteService.getSelectedNoteId());
        await updateNoteList();
    });

    pinNoteButton.addEventListener('click', async () => {
        await ajaxService.togglePinned(noteService.getSelectedNote());
        await updateNoteList();
    });

    lockNoteButton.addEventListener('click', async () => {
        await ajaxService.toggleLocked(noteService.getSelectedNote());
        await updateNoteList();
    });

    archiveNoteButton.addEventListener('click', async () => {
        await ajaxService.toggleArchived(noteService.getSelectedNote());
        await updateNoteList();
    });

    publishNoteButton.addEventListener('click', async () => {
        await ajaxService.togglePublished(noteService.getSelectedNote());
        await updateNoteList();
    });

    sortByCreatedButton.addEventListener('click', () => {
        let notes = noteService.getLocalNotes();
        noteService.sortByCreated(notes);
        addNoteElements(noteService.getNotesForDisplay(notes, archivedView));
    });

    sortByUpdatedButton.addEventListener('click', () => {
        let notes = noteService.getLocalNotes();
        noteService.sortByUpdated(notes);
        addNoteElements(noteService.getNotesForDisplay(notes, archivedView));
    });

    sortByTextButton.addEventListener('click', () => {
        let notes = noteService.getLocalNotes();
        noteService.sortByText(notes);
        addNoteElements(noteService.getNotesForDisplay(notes, archivedView));
    });

    loginButton.addEventListener('click', async () => {
        await ajaxService.login(getLoginEmail(), getLoginPassword());
    });

    registerButton.addEventListener('click', async () => {
        await ajaxService.register(getRegisterEmail(), getRegisterPassword());
    });

    logoutButton.addEventListener('click', () => {
        ajaxService.logout();
    });

    importNotesButton.addEventListener('click', () => {
        importNotesInput.click();
    });

    importNotesInput.addEventListener('change', () => {
        const reader = new FileReader();
        reader.readAsText(importNotesInput.files[0]);
        reader.onload = async () => {
            let notesJson = reader.result;
            await ajaxService.bulkCreateNotes(notesJson);
        }
    });

    exportNotesButton.addEventListener('click', () => {
        let notesJson = JSON.stringify(noteService.getLocalNotes());
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

        snapshots = await ajaxService.getAllSnapshots(noteService.getSelectedNoteId());

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

async function updateNoteList() {
    let notes = await ajaxService.getNotes();
    noteService.saveLocalNotes(notes);

    addNoteElements(noteService.getNotesForDisplay(notes, archivedView));
    setCheckboxes(noteService.getSelectedNote());
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

function getLoginEmail() {
    return document.getElementById('login-email').value;
}

function getLoginPassword() {
    return document.getElementById('login-password').value;
}

function getRegisterEmail() {
    return document.getElementById('register-email').value;
}

function getRegisterPassword() {
    return document.getElementById('register-password').value;
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
        noteDiv.innerHTML =
            `<div>Id: ${note.id}</div>` +
            `<div>AuthorId: ${note.authorId}</div>` +
            `<div>Created: ${new Date(note.created).toISOString()}</div>` +
            `<div>Updated: ${new Date(note.updated).toISOString()}</div>` +
            `<div>PublicURL: ${note.publicUrl}</div>`;
        `<div>Text: ${note.text}</div>`;
        notesListDiv.append(noteDiv);
    }

    addNoteClickHandlers();
}

function noteSelectedHandler(event) {
    let noteElement = event.target.closest('.note');
    let noteId = getNoteId(noteElement);
    let note = noteService.getLocalNote(noteId);

    noteService.setSelectedNoteId(noteId);
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
