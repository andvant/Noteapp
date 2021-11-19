async function run() {
    document.getElementById('save-note-button').addEventListener('click', async () => {
        await apiUpdateNote();
    });

    document.getElementById('new-note-button').addEventListener('click', async () => {
        await apiCreateNote();
    });

    document.getElementById('delete-note-button').addEventListener('click', async () => {
        await apiDeleteNote();
    });

    await updateNoteList();
}

async function updateNoteList() {
    let notes = await apiGetNotes();
    saveNotesToLocalStorage(notes);

    addNoteElements(notes);

    addNoteClickEvents();
}


function getNoteId(noteElement) {
    return noteElement.id.split('-')[1];
}

function getNote(id) {
    return JSON.parse(localStorage.getItem('notes')).find(note => note.id == id);
}

function addNoteClickEvents() {
    document.getElementById('notes-list').addEventListener('click', e => {
        let noteElement = e.target.closest('.note');
        let noteId = getNoteId(noteElement);
        let noteText = getNote(noteId).text;

        localStorage.setItem('selectedNoteId', noteId);

        setSelectedNoteText(noteText);
    });
}

function setSelectedNoteText(text) {
    document.getElementById('note-text').value = text;
}

function getSelectedNoteText() {
    return document.getElementById('note-text').value;
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
            `<div>Text: ${note.text}</div>`;
        noteList.append(noteDiv);
    }
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
    let selectedNoteId = localStorage.getItem('selectedNoteId');
    let noteText = getSelectedNoteText();
    let response = await fetch(`http://localhost:5000/api/notes/${selectedNoteId}`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ text: noteText })
    });

    if (response.ok) {
        await updateNoteList();
    }
    else {
        alert(response.statusText);
    }
}

async function apiDeleteNote() {
    let selectedNoteId = localStorage.getItem('selectedNoteId');
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

    if (response.ok) {
        await updateNoteList();
    }
    else {
        alert(response.statusText);
    }
}