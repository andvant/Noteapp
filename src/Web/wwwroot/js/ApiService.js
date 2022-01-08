import LocalDataManager from "./LocalDataManager.js";

let ApiService = {
    getNotes,
    createNote,
    updateNote,
    deleteNote,
    bulkCreateNotes,
    getAllSnapshots,
    login,
    register,
    deleteAccount
}

const API_BASE_URL = "http://localhost:5000/api/";

async function getNotes() {
    let response = await sendRequest("notes", "GET");
    return await getNotesFromResponse(response);
}

async function createNote(note) {
    let headers = { "Content-Type": "application/json" };
    let body = JSON.stringify(new NoteRequest(note));
    let response = await sendRequest("notes", "POST", headers, body);
    return await getNoteFromResponse(response);
}

async function bulkCreateNotes(notes) {
    let headers = { "Content-Type": "application/json" };
    let body = JSON.stringify(notes.map(note => new NoteRequest(note)));
    return await sendRequest("notes/bulk", "POST", headers, body) != null;
}

async function updateNote(note) {
    let headers = { "Content-Type": "application/json" };
    let body = JSON.stringify(new NoteRequest(note));
    let response = await sendRequest(`notes/${note.id}`, "PUT", headers, body);
    return await getNoteFromResponse(response);
}

async function deleteNote(noteId) {
    return await sendRequest(`notes/${noteId}`, "DELETE") != null;
}

async function getAllSnapshots(noteId) {
    let response = await sendRequest(`notes/${noteId}/snapshots`);
    return response != null ? await response.json() : null;
}

async function login(email, password) {
    let headers = { "Content-Type": "application/json" };
    let body = JSON.stringify({ email, password });
    let response = await sendRequest("account/token", "POST", headers, body);
    return response != null ? await response.json() : null;
}

async function register(email, password) {
    let headers = { "Content-Type": "application/json" };
    let body = JSON.stringify({ email, password });
    return await sendRequest("account/register", "POST", headers, body) != null;
}

async function deleteAccount() {
    return await sendRequest("account/delete", "DELETE") != null;
}

async function sendRequest(url, method, headers = {}, body = null) {
    addAuthorizationHeader(headers);

    let response;
    try {
        response = await fetch(`${API_BASE_URL}${url}`, { method, headers, body });
    }
    catch {
        console.log('Failed to connect to the server.');
        return null;
    }

    if (!response.ok) {
        console.log(`Received unsuccessful response from the server:\n${response.statusText}\n${await response.text()}`);
        return null;
    }

    return response;
}

function addAuthorizationHeader(headers) {
    let userInfo = LocalDataManager.getUserInfo();
    if (userInfo.access_token != null) {
        headers['Authorization'] = `Bearer ${userInfo.access_token}`;
    }
}

function NoteRequest(note) {
    this.text = note.text;
    this.pinned = note.pinned;
    this.locked = note.locked;
    this.archived = note.archived;
    this.published = note.published;
}

async function getNoteFromResponse(response) {
    if (response == null) return null;

    let note = await response.json();
    note.synchronized = true;
    return note;
}

async function getNotesFromResponse(response) {
    if (response == null) return null;

    let notes = await response.json();
    notes.forEach(note => note.synchronized = true);
    return notes;
}

export default ApiService;