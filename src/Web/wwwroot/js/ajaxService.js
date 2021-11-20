export { ajaxService };

let ajaxService = (function () {

    let baseUrl = "http://localhost:5000/api/notes";

    async function apiGetNotes() {
        let notes = null;

        let response = await fetch(baseUrl, {
            method: "GET"
        });

        if (response.ok) {
            notes = await response.json();
        }

        alertIfError(response);

        return notes;
    }

    async function apiUpdateNote(noteId, noteText) {
        let response = await fetch(`${baseUrl}/${noteId}`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ text: noteText })
        });

        alertIfError(response);
    }

    async function apiCreateNote() {
        let response = await fetch(baseUrl, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ text: "" })
        });

        alertIfError(response);
    }

    async function apiDeleteNote(noteId) {
        let response = await fetch(`${baseUrl}/${noteId}`, {
            method: "DELETE"
        });

        alertIfError(response);
    }

    async function apiTogglePinned(note) {
        let method = note.pinned ? "DELETE" : "PUT";
        let response = await fetch(`${baseUrl}/${note.id}/pin`, {
            method
        });

        alertIfError(response);
    }

    async function apiToggleLocked(note) {
        let method = note.locked ? "DELETE" : "PUT";
        let response = await fetch(`${baseUrl}/${noteId}/lock`, {
            method
        });

        alertIfError(response);
    }

    async function apiToggleArchived(note) {
        let method = note.archived ? "DELETE" : "PUT";
        let response = await fetch(`${baseUrl}/${note.id}/archive`, {
            method
        });

        alertIfError(response);
    }

    async function apiTogglePublished(note) {
        let method = note.published ? "DELETE" : "PUT";
        let response = await fetch(`${baseUrl}/${note.id}/publish`, {
            method
        });

        alertIfError(response);
    }

    function alertIfError(response) {
        if (!response.ok) {
            alert(response.statusText);
        }
    }

    return {
        apiCreateNote,
        apiGetNotes,
        apiUpdateNote,
        apiDeleteNote,
        apiToggleArchived,
        apiToggleLocked,
        apiTogglePinned,
        apiTogglePublished
    }

})();