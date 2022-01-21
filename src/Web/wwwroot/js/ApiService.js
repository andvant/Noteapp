import AppData from "./AppData.js";
import Config from "./Config.js";

let ApiService = {
    getNotes,
    createNote,
    updateNote,
    deleteNote,
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
        apiResponse.errorMessage = 'Failed to connect to the server';
        return apiResponse;
    }

    let content = await response.text();
    if (response.ok) {
        apiResponse.content = content.length > 0 ? JSON.parse(content) : null;
    }
    else {
        apiResponse.errorMessage = content.length > 0 ?
            JSON.parse(content).errorMessage : "ERROR";
    }
    return apiResponse;
}

function addAuthorizationHeader(headers) {
    let userInfo = AppData.getUserInfo();
    if (userInfo.accessToken != null) {
        headers['Authorization'] = `Bearer ${userInfo.accessToken}`;
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
    this.locked = note.locked;
    this.archived = note.archived;
    this.pinned = note.pinned;
    this.published = note.published;
}

export default ApiService;