import { ajaxService } from "./ajaxService.js";

"use strict";

document.addEventListener('DOMContentLoaded', async () => {
    addEventListenersForButtons();
    await updateNoteList();
});

function addEventListenersForButtons() {
    document.getElementById('save-note-button').addEventListener('click', async () => {
        await ajaxService.apiUpdateNote(getSelectedNoteId(), getSelectedNoteText());
        await updateNoteList();
    });

    document.getElementById('new-note-button').addEventListener('click', async () => {
        await ajaxService.apiCreateNote();
        await updateNoteList();
    });

    document.getElementById('delete-note-button').addEventListener('click', async () => {
        await ajaxService.apiDeleteNote(getSelectedNoteId());
        await updateNoteList();
    });

    document.getElementById('pin-note-button').addEventListener('click', async () => {
        await ajaxService.apiTogglePinned(getSelectedNote());
        await updateNoteList();
    });

    document.getElementById('lock-note-button').addEventListener('click', async () => {
        await ajaxService.apiToggleLocked(getSelectedNote());
        await updateNoteList();
    });

    document.getElementById('archive-note-button').addEventListener('click', async () => {
        await ajaxService.apiToggleArchived(getSelectedNote());
        await updateNoteList();
    });

    document.getElementById('publish-note-button').addEventListener('click', async () => {
        await ajaxService.apiTogglePublished(getSelectedNote());
        await updateNoteList();
    });

    document.getElementById('sort-created-button').addEventListener('click', async () => {
        let notes = await ajaxService.apiGetNotes();
        sortCreated(notes);
        addNoteElements(getNotesToBeDisplayed(notes));

        await updateNoteList();
    });

    document.getElementById('sort-updated-button').addEventListener('click', async () => {
        let notes = await ajaxService.apiGetNotes();
        sortUpdated(notes);
        addNoteElements(getNotesToBeDisplayed(notes));

        await updateNoteList();
    });

    document.getElementById('sort-text-button').addEventListener('click', async () => {
        let notes = await ajaxService.apiGetNotes();
        sortText(notes);
        addNoteElements(getNotesToBeDisplayed(notes));

        await updateNoteList();
    });
}

async function updateNoteList() {
    let notes = await ajaxService.apiGetNotes();
    saveNotesToLocalStorage(notes);

    addNoteElements(getNotesToBeDisplayed(notes));
    
    setCheckboxes(getSelectedNote());
}

function orderByPinned(notes) {
    return notes.sort((note1, note2) => note1.pinned === note2.pinned ? 0 : note1.pinned ? -1 : 1);
}

function getNotesToBeDisplayed(notes) {
    let archivedNotes = getNonArchivedNotes(notes);
    return orderByPinned(archivedNotes);
}

function getNonArchivedNotes(notes) {
    return notes.filter(note => !note.archived);
}

function getNoteId(noteElement) {
    return noteElement.id.split('-')[1];
}

function getNote(noteId) {
    return JSON.parse(localStorage.getItem('notes')).find(note => note.id == noteId);
}

function getNoteTextElement() {
    return document.getElementById('note-text');
}

function setSelectedNoteId(noteId) {
    localStorage.setItem('selectedNoteId', noteId);
}

function getSelectedNoteId() {
    return localStorage.getItem('selectedNoteId');
}

function getSelectedNote() {
    return getNote(getSelectedNoteId());
}

function setSelectedNoteText(text) {
    getNoteTextElement().value = text;
}

function getSelectedNoteText() {
    return getNoteTextElement().value;
}

function saveNotesToLocalStorage(notes) {
    localStorage.setItem('notes', JSON.stringify(notes));
}

function noteSelectedHandler(event) {
    let noteElement = event.target.closest('.note');
    let noteId = getNoteId(noteElement);
    let noteText = getNote(noteId).text;

    setSelectedNoteId(noteId);
    setSelectedNoteText(noteText);

    let note = getNote(noteId);

    setCheckboxes(note);

    disableNoteEditingIfLocked(note.locked);
}

function addNoteClickHandlers() {
    document.getElementById('notes-list').addEventListener('click', noteSelectedHandler);
}

function disableNoteEditingIfLocked(locked) {
    if (locked) {
        getNoteTextElement().readOnly = true;
        document.getElementById('save-note-button').disabled = true;
    }
    else {
        getNoteTextElement().readOnly = false;
        document.getElementById('save-note-button').disabled = false;
    }
}

function addNoteElements(notes) {
    let noteList = document.getElementById('notes-list');

    noteList.innerHTML = '';

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
        noteList.append(noteDiv);
    }

    addNoteClickHandlers();
}

function setCheckboxes(note) {
    document.getElementById("note-pinned").checked = note.pinned;
    document.getElementById("note-locked").checked = note.locked;
    document.getElementById("note-archived").checked = note.archived;
    document.getElementById("note-published").checked = note.published;
}

function sortCreated(notes) {
    notes.sort((note1, note2) => new Date(note1.created) - new Date(note2.created));

    let descending = localStorage.getItem('sortCreatedDescending') === 'true';
    reverseIfNeeded(notes, descending);
    localStorage.setItem('sortCreatedDescending', descending ? 'false' : 'true');
}

function sortUpdated(notes) {
    notes.sort((note1, note2) => new Date(note1.updated) - new Date(note2.updated));

    let descending = localStorage.getItem('sortUpdatedDescending') === 'true';
    reverseIfNeeded(notes, descending);
    localStorage.setItem('sortUpdatedDescending', descending ? 'false' : 'true');
}

function sortText(notes) {
    notes.sort((note1, note2) => note1.text > note2.text ? 1 : -1);

    let descending = localStorage.getItem('sortTextDescending') === 'true';
    reverseIfNeeded(notes, descending);
    localStorage.setItem('sortTextDescending', descending ? 'false' : 'true');
}

function reverseIfNeeded(notes, descending) {
    if (descending) {
        notes.reverse();
    }
}