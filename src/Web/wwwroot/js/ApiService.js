let ApiService = {
    getNotes,
    createNote,
    updateNote,
    deleteNote,
    bulkCreateNotes,
    toggleLocked,
    toggleArchived,
    togglePinned,
    togglePublished,
    getAllSnapshots,
    login,
    logout,
    register,
    deleteAccount
}

const BASE_URL = "http://localhost:5000/api";
let _userInfo = JSON.parse(localStorage.getItem('userInfo'));

async function getNotes(archived) {
    let filter = archived === true ? "?archived=true" : archived === false ? "?archived=false" : "";
    let response = await sendRequest(`notes${filter}`, "GET");
    return await response.json();
}

async function createNote() {
    let headers = { "Content-Type": "application/json" };
    let body = JSON.stringify({ text: "" });
    let response = await sendRequest("notes", "POST", headers, body);
    return await response.json();
}

async function updateNote(noteId, noteText) {
    let headers = { "Content-Type": "application/json" };
    let body = JSON.stringify({ text: noteText });
    let response = await sendRequest(`notes/${noteId}`, "PUT", headers, body);
    return await response.json();
}

async function deleteNote(noteId) {
    await sendRequest(`notes/${noteId}`, "DELETE");
}

async function bulkCreateNotes(notesJson) {
    let headers = { "Content-Type": "application/json" };
    let body = notesJson;
    await sendRequest("notes/bulk", "POST", headers, body);
}

async function toggleLocked(note) {
    let method = note.locked ? "DELETE" : "PUT";
    let response = await sendRequest(`notes/${note.id}/lock`, method);
    return await response.json();
}

async function toggleArchived(note) {
    let method = note.archived ? "DELETE" : "PUT";
    let response = await sendRequest(`notes/${note.id}/archive`, method);
    return await response.json();
}

async function togglePinned(note) {
    let method = note.pinned ? "DELETE" : "PUT";
    let response = await sendRequest(`notes/${note.id}/pin`, method);
    return await response.json();
}

async function togglePublished(note) {
    let method = note.published ? "DELETE" : "PUT";
    let response = await sendRequest(`notes/${note.id}/publish`, method);
    return await response.json();
}

async function getAllSnapshots(noteId) {
    let response = await sendRequest(`notes/${noteId}/snapshots`);
    return await response.json();
}

async function login(email, password) {
    let headers = { "Content-Type": "application/json" };
    let body = JSON.stringify({ email, password });
    let response = await sendRequest("account/token", "POST", headers, body);

    _userInfo = await response.json();
    localStorage.setItem('userInfo', JSON.stringify(_userInfo));
    alert('Successfully logged in.')
}

function logout() {
    _userInfo = null;
    localStorage.removeItem('userInfo');
    alert('Successfully logged out.')
}

async function register(email, password) {
    let headers = { "Content-Type": "application/json" };
    let body = JSON.stringify({ email, password });
    await sendRequest("account/register", "POST", headers, body);
    alert('Successfully registered.')
}

async function deleteAccount() {
    await sendRequest("account/delete", "DELETE");
    alert('Account successfully deleted.')
    logout();
}

async function sendRequest(url, method, headers = {}, body = null) {

    if (_userInfo != null) {
        headers['Authorization'] = `Bearer ${_userInfo.access_token}`;
    }
    let response;
    try {
        response = await fetch(`${BASE_URL}/${url}`, { method, headers, body });
    }
    catch {
        alert('Failed to connect to the server.')
        throw new Error('Failed to connect to the server');
    }

    if (!response.ok) {
        alert(`Received unsuccessful response from the server:\n${response.statusText}\n${await response.text()}`);
        throw new Error('Bad response from the server');
    }

    return response;
}

export default ApiService;