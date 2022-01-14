import AppData from "./AppData.js";
import Config from "./Config.js";

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

async function getNotes() {
    let response = await sendRequest("notes", "GET");
    if (!response.isSuccess()) return null;

    let notes = response.content;
    notes.forEach(note => {
        note.synchronized = true;
        note.updatedLocal = note.updated;
    });
    return notes;
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
    return (await sendRequest("notes/bulk", "POST", headers, body)).isSuccess();
}

async function updateNote(note) {
    let headers = { "Content-Type": "application/json" };
    let body = JSON.stringify(new NoteRequest(note));
    let response = await sendRequest(`notes/${note.id}`, "PUT", headers, body);
    return await getNoteFromResponse(response);
}

async function deleteNote(noteId) {
    return (await sendRequest(`notes/${noteId}`, "DELETE")).isSuccess();
}

async function getAllSnapshots(noteId) {
    let response = await sendRequest(`notes/${noteId}/snapshots`);
    return response.isSuccess() ? response.content : null;
}

async function register(email, password) {
    let headers = { "Content-Type": "application/json" };
    let body = JSON.stringify({ email, password });
    return await sendRequest("account/register", "POST", headers, body, false);
}

async function login(email, password) {
    let headers = { "Content-Type": "application/json" };
    let body = JSON.stringify({ email, password });
    return await sendRequest("account/token", "POST", headers, body, false);
}

async function deleteAccount() {
    return (await sendRequest("account/delete", "DELETE")).isSuccess();
}

async function sendRequest(url, method, headers = {}, body = null, authorized = true) {
    if (authorized) addAuthorizationHeader(headers);

    let response;
    let apiResponse = new ApiResponse();
    try {
        response = await fetch(`${Config.API_BASE_URL}${url}`, { method, headers, body });
    }
    catch {
        console.log('Failed to connect to the server');
        apiResponse.errorMessage = 'Failed to connect to the server';
        return apiResponse;
    }

    if (response.ok) {
        let text = await response.text();
        if (text.length > 0) {
            apiResponse.content = JSON.parse(text);
        }
    }
    else {
        let error = await response.json();
        console.log(error.errorMessage);
        apiResponse.errorMessage = error.errorMessage;
    }
    return apiResponse;
}

function addAuthorizationHeader(headers) {
    let userInfo = AppData.getUserInfo();
    if (userInfo.access_token != null) {
        headers['Authorization'] = `Bearer ${userInfo.access_token}`;
    }
}

async function getNoteFromResponse(response) {
    if (!response.isSuccess()) return null;

    let note = response.content;
    note.synchronized = true;
    note.updatedLocal = note.updated;
    return note;
}

class ApiResponse {
    content = null;
    errorMessage = null;
    constructor() {}
    isSuccess() {
        return this.errorMessage == null || this.errorMessage == '';
    }
}

function NoteRequest(note) {
    this.text = note.text;
    this.pinned = note.pinned;
    this.locked = note.locked;
    this.archived = note.archived;
    this.published = note.published;
}

export default ApiService;