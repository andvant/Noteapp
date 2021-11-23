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
const notesListDiv = document.getElementById('notes-list')
const noteTextElement = document.getElementById('note-text');

document.addEventListener('DOMContentLoaded', async () => {
    addEventListenersToButtons();
    await updateNoteList();
});

function addEventListenersToButtons() {
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

    sortByCreatedButton.addEventListener('click', async () => {
        let notes = noteService.getLocalNotes();
        noteService.sortByCreated(notes);
        addNoteElements(noteService.getNotesToBeDisplayed(notes));
    });

    sortByUpdatedButton.addEventListener('click', async () => {
        let notes = noteService.getLocalNotes();
        noteService.sortByUpdated(notes);
        addNoteElements(noteService.getNotesToBeDisplayed(notes));
    });

    sortByTextButton.addEventListener('click', async () => {
        let notes = noteService.getLocalNotes();
        noteService.sortByText(notes);
        addNoteElements(noteService.getNotesToBeDisplayed(notes));
    });

    loginButton.addEventListener('click', async () => {
        await ajaxService.login(getLoginEmail(), getLoginPassword());
    });

    registerButton.addEventListener('click', async () => {
        await ajaxService.register(getRegisterEmail(), getRegisterPassword());
    });
}

async function updateNoteList() {
    let notes = await ajaxService.getNotes();
    noteService.saveLocalNotes(notes);

    addNoteElements(noteService.getNotesToBeDisplayed(notes));
    setCheckboxes(noteService.getSelectedNote());
}

function getNoteId(noteElement) {
    return noteElement.id.split('-')[1];
}

function getSelectedNoteText() {
    return noteTextElement.value;
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
