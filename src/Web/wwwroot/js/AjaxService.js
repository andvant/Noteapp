export {
    createNote,
    bulkCreateNotes,
    getNotes,
    updateNote,
    deleteNote,
    toggleArchived,
    toggleLocked,
    togglePinned,
    togglePublished,
    getAllSnapshots,
    login,
    register,
    deleteAccount,
    logout
}

const baseUrl = "http://localhost:5000/api";
let userInfo = JSON.parse(localStorage.getItem('userInfo'));

async function getNotes() {
    //let filter = archived === true ? "?archived=true" : archived === false ? "?archived=false" : "";
    let response = await sendRequest("notes", "GET");
    if (response?.ok) {
        return await response.json();
    }
}

async function updateNote(noteId, noteText) {
    let headers = { "Content-Type": "application/json" };
    let body = JSON.stringify({ text: noteText });
    let response = await sendRequest(`notes/${noteId}`, "PUT", headers, body);
    if (response?.ok) {
        return await response.json();
    }
}

async function createNote() {
    let headers = { "Content-Type": "application/json" };
    let body = JSON.stringify({ text: "" });
    let response = await sendRequest("notes", "POST", headers, body);
    if (response?.ok) {
        return await response.json();
    }
}

async function bulkCreateNotes(notesJson) {
    let headers = { "Content-Type": "application/json" };
    let body = notesJson;
    await sendRequest("notes/bulk", "POST", headers, body);
}

async function deleteNote(noteId) {
    let response = await sendRequest(`notes/${noteId}`, "DELETE");
    if (!response.ok) {
        throw new Error("Failed to delete a note.");
    }
}

async function togglePinned(note) {
    let method = note.pinned ? "DELETE" : "PUT";
    let response = await sendRequest(`notes/${note.id}/pin`, method);
    if (response?.ok) {
        return await response.json();
    }
}

async function toggleLocked(note) {
    let method = note.locked ? "DELETE" : "PUT";
    let response = await sendRequest(`notes/${note.id}/lock`, method);
    if (response?.ok) {
        return await response.json();
    }
}

async function toggleArchived(note) {
    let method = note.archived ? "DELETE" : "PUT";
    let response = await sendRequest(`notes/${note.id}/archive`, method);
    if (response?.ok) {
        return await response.json();
    }
}

async function togglePublished(note) {
    let method = note.published ? "DELETE" : "PUT";
    let response = await sendRequest(`notes/${note.id}/publish`, method);
    if (response?.ok) {
        return await response.json();
    }
}

async function getAllSnapshots(noteId) {
    let response = await sendRequest(`notes/${noteId}/snapshots`);
    if (response?.ok) {
        return await response.json();
    }
}

async function login(email, password) {
    let headers = { "Content-Type": "application/json" };
    let body = JSON.stringify({ email, password });
    let response = await sendRequest("account/token", "POST", headers, body);

    if (response?.ok) {
        userInfo = await response.json();
        localStorage.setItem('userInfo', JSON.stringify(userInfo));
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

async function deleteAccount() {
    let response = await sendRequest("account/delete", "DELETE");

    if (response?.ok) {
        alert('Account successfully deleted.')
        logout();
    }
}

function logout() {
    userInfo = null;
    localStorage.removeItem('userInfo');
    alert('Successfully logged out.')
}

async function sendRequest(url, method, headers = {}, body = null) {

    if (userInfo != null) {
        headers['Authorization'] = `Bearer ${userInfo.access_token}`;
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
