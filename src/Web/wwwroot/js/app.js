async function run() {
    addEventListenersForButtons();

    await updateNoteList();
}

function addEventListenersForButtons() {
    document.getElementById('save-note-button').addEventListener('click', async () => {
        await apiUpdateNote();
    });

    document.getElementById('new-note-button').addEventListener('click', async () => {
        await apiCreateNote();
    });

    document.getElementById('delete-note-button').addEventListener('click', async () => {
        await apiDeleteNote();
    });

    document.getElementById('pin-note-button').addEventListener('click', async () => {
        await apiTogglePinned();
    });

    document.getElementById('lock-note-button').addEventListener('click', async () => {
        await apiToggleLocked();
    });

    document.getElementById('archive-note-button').addEventListener('click', async () => {
        await apiToggleArchived();
    });

    document.getElementById('publish-note-button').addEventListener('click', async () => {
        await apiTogglePublished();
    });
}

async function updateNoteList() {
    let notes = await apiGetNotes();
    saveNotesToLocalStorage(notes);


    addNoteElements(getNotesToBeDisplayed(notes));

    addNoteClickEvents();
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

function addNoteClickEvents() {
    document.getElementById('notes-list').addEventListener('click', e => {
        let noteElement = e.target.closest('.note');
        let noteId = getNoteId(noteElement);
        let noteText = getNote(noteId).text;

        setSelectedNoteId(noteId);

        setSelectedNoteText(noteText);


        let note = getNote(noteId);

        setCheckboxes(note);

        if (note.locked) {
            getNoteTextElement().readOnly = true;
            document.getElementById('save-note-button').disabled = true;
        }
        else {
            document.getElementById('save-note-button').disabled = false;
        }
    });
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

function setSelectedNoteText(text) {
    getNoteTextElement().value = text;
}

function getSelectedNoteText() {
    return getNoteTextElement().value;
}

function saveNotesToLocalStorage(notes) {
    localStorage.setItem('notes', JSON.stringify(notes));
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
}

function setCheckboxes(note) {
    document.getElementById("note-pinned").checked = note.pinned;
    document.getElementById("note-locked").checked = note.locked;
    document.getElementById("note-archived").checked = note.archived;
    document.getElementById("note-published").checked = note.published;
}

// calling the api
async function apiGetNotes() {
    let notes = null;

    let response = await fetch("http://localhost:5000/api/notes", {
        method: "GET"
    });

    if (response.ok) {
        notes = await response.json();
    }
    else {
        alert(response.statusText);
    }

    return notes;
}

async function apiUpdateNote() {
    let selectedNoteId = getSelectedNoteId();
    let noteText = getSelectedNoteText();
    let response = await fetch(`http://localhost:5000/api/notes/${selectedNoteId}`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ text: noteText })
    });

    await processResponse(response);
}

async function apiDeleteNote() {
    let selectedNoteId = getSelectedNoteId();
    let response = await fetch(`http://localhost:5000/api/notes/${selectedNoteId}`, {
        method: "DELETE"
    });

    if (response.ok) {
        await updateNoteList();
    }
    else {
        alert(response.statusText);
    }
}

async function apiCreateNote() {
    let response = await fetch(`http://localhost:5000/api/notes`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ text: "" })
    });

    await processResponse(response);
}

async function apiDeleteNote() {
    let selectedNoteId = getSelectedNoteId();
    let response = await fetch(`http://localhost:5000/api/notes/${selectedNoteId}`, {
        method: "DELETE"
    });

    await processResponse(response);
}

async function apiTogglePinned() {
    let selectedNoteId = getSelectedNoteId();
    let note = getNote(selectedNoteId);
    let method = note.pinned ? "DELETE" : "PUT";
    let response = await fetch(`http://localhost:5000/api/notes/${selectedNoteId}/pin`, {
        method
    });

    await processResponse(response);
}

async function apiToggleLocked() {
    let selectedNoteId = getSelectedNoteId();
    let note = getNote(selectedNoteId);
    let method = note.locked ? "DELETE" : "PUT";
    let response = await fetch(`http://localhost:5000/api/notes/${selectedNoteId}/lock`, {
        method
    });

    await processResponse(response);
}

async function apiToggleArchived() {
    let selectedNoteId = getSelectedNoteId();
    let note = getNote(selectedNoteId);
    let method = note.archived ? "DELETE" : "PUT";
    let response = await fetch(`http://localhost:5000/api/notes/${selectedNoteId}/archive`, {
        method
    });

    await processResponse(response);
}

async function apiTogglePublished() {
    let selectedNoteId = getSelectedNoteId();
    let note = getNote(selectedNoteId);
    let method = note.published ? "DELETE" : "PUT";
    let response = await fetch(`http://localhost:5000/api/notes/${selectedNoteId}/publish`, {
        method
    });

    await processResponse(response);
}

async function processResponse(response) {
    if (response.ok) {
        await updateNoteList();
    }
    else {
        alert(response.statusText);
    }
}