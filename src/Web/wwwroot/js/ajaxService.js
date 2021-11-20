export { ajaxService };

let ajaxService = (function () {

    let baseUrl = "http://localhost:5000/api/notes";

    async function sendRequest(url, method, headers, body) {
        let response = await fetch(`${baseUrl}/${url}`, {
            method,
            headers,
            body
        });

        alertIfError(response);

        return response;
    }

    async function getNotes() {
        let response = await sendRequest("", "GET", {}, null);
        if (response.ok) {
            return await response.json();
        }
    }

    async function updateNote(noteId, noteText) {
        let headers = { "Content-Type": "application/json" };
        let body = JSON.stringify({ text: noteText });
        await sendRequest(`${noteId}`, "PUT", headers, body);
    }

    async function createNote() {
        let body = JSON.stringify({ text: "" });
        let headers = { "Content-Type": "application/json" };
        await sendRequest("", "POST", headers, body);
    }

    async function deleteNote(noteId) {
        await sendRequest(`${noteId}`, "DELETE", {}, null);
    }

    async function togglePinned(note) {
        let method = note.pinned ? "DELETE" : "PUT";
        await sendRequest(`${note.id}/pin`, method, {}, null);
    }

    async function toggleLocked(note) {
        let method = note.locked ? "DELETE" : "PUT";
        await sendRequest(`${note.id}/lock`, method, {}, null);
    }

    async function toggleArchived(note) {
        let method = note.archived ? "DELETE" : "PUT";
        await sendRequest(`${note.id}/archive`, method, {}, null);
    }

    async function togglePublished(note) {
        let method = note.published ? "DELETE" : "PUT";
        await sendRequest(`${note.id}/publish`, method, {}, null);
    }

    function alertIfError(response) {
        if (!response.ok) {
            alert(response.statusText);
        }
    }

    return {
        createNote,
        getNotes,
        updateNote,
        deleteNote,
        toggleArchived,
        toggleLocked,
        togglePinned,
        togglePublished
    }

})();