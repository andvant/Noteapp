export {
    createNote,
    getNotes,
    updateNote,
    deleteNote,
    toggleArchived,
    toggleLocked,
    togglePinned,
    togglePublished,
    login,
    register,
    logout
}

const baseUrl = "http://localhost:5000/api";

let accessToken = localStorage.getItem('accessToken');

async function getNotes() {
    let response = await sendRequest("notes", "GET", {}, null);
    if (response?.ok) {
        return await response.json();
    }
}

async function updateNote(noteId, noteText) {
    let headers = { "Content-Type": "application/json" };
    let body = JSON.stringify({ text: noteText });
    await sendRequest(`notes/${noteId}`, "PUT", headers, body);
}

async function createNote() {
    let headers = { "Content-Type": "application/json" };
    let body = JSON.stringify({ text: "" });
    await sendRequest("notes", "POST", headers, body);
}

async function deleteNote(noteId) {
    await sendRequest(`notes/${noteId}`, "DELETE", {}, null);
}

async function togglePinned(note) {
    let method = note.pinned ? "DELETE" : "PUT";
    await sendRequest(`notes/${note.id}/pin`, method, {}, null);
}

async function toggleLocked(note) {
    let method = note.locked ? "DELETE" : "PUT";
    await sendRequest(`notes/${note.id}/lock`, method, {}, null);
}

async function toggleArchived(note) {
    let method = note.archived ? "DELETE" : "PUT";
    await sendRequest(`notes/${note.id}/archive`, method, {}, null);
}

async function togglePublished(note) {
    let method = note.published ? "DELETE" : "PUT";
    await sendRequest(`notes/${note.id}/publish`, method, {}, null);
}

async function login(email, password) {
    let headers = { "Content-Type": "application/json" };
    let body = JSON.stringify({ email, password });
    let response = await sendRequest("account/token", "POST", headers, body);

    if (response?.ok) {
        accessToken = (await response.json()).access_token
        localStorage.setItem('accessToken', accessToken);
        alert('Successfully logged in.')
    }
}

async function register(email, password) {
    let headers = { "Content-Type": "application/json" };
    let body = JSON.stringify({ email, password });
    let response = await sendRequest("account/register", "POST", headers, body);

    if (response?.ok) {
        alert('Successfully registered.')
    }
}

function logout() {
    accessToken = null;
    localStorage.removeItem('accessToken');
    alert('Successfully logged out.')
}

async function sendRequest(url, method, headers, body) {

    if (accessToken != null) {
        headers['Authorization'] = `Bearer ${accessToken}`;
    }
    let response;
    try {
        response = await fetch(`${baseUrl}/${url}`, { method, headers, body });
    }
    catch {
        alert(`Failed to connect to the server.`)
        return null;
    }

    if (!response.ok) {
        alert(`${response.statusText}\n${await response.text()}`);
    }

    return response;
}
