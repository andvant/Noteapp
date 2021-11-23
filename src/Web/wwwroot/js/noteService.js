export {
    getNotesToBeDisplayed,
    getLocalNotes,
    getLocalNote,
    getSelectedNote,
    sortByCreated,
    sortByUpdated,
    sortByText,
    setSelectedNoteId,
    getSelectedNoteId,
    saveLocalNotes
}

function orderByPinned(notes) {
    return notes.sort((note1, note2) => note1.pinned === note2.pinned ? 0 : note1.pinned ? 1 : -1);
}

function getNonArchivedNotes(notes) {
    return notes.filter(note => !note.archived);
}

function getNotesToBeDisplayed(notes) {
    return orderByPinned(getNonArchivedNotes(notes));
}

function getLocalNotes() {
    return JSON.parse(localStorage.getItem('notes'));
}

function getLocalNote(noteId) {
    return getLocalNotes().find(note => note.id == noteId);
}

function saveLocalNotes(notes) {
    localStorage.setItem('notes', JSON.stringify(notes));
}

function getSelectedNote() {
    return getLocalNote(getSelectedNoteId());
}

function getDescending(property) {
    return localStorage.getItem(`sortBy_${property}_descending`) === 'true';
}

function toggleDescending(property) {
    let descending = getDescending(property);
    localStorage.setItem(`sortBy_${property}_descending`, descending ? 'false' : 'true');
}

function reverseIfDescending(notes, property) {
    let descending = getDescending(property);
    if (descending) notes.reverse();
    toggleDescending(property);
}

function sortByCreated(notes) {
    notes.sort((note1, note2) => new Date(note1.created) - new Date(note2.created));
    reverseIfDescending(notes, 'created');
}

function sortByUpdated(notes) {
    notes.sort((note1, note2) => new Date(note1.updated) - new Date(note2.updated));
    reverseIfDescending(notes, 'updated');
}

function sortByText(notes) {
    notes.sort((note1, note2) => note1.text > note2.text ? 1 : -1);
    reverseIfDescending(notes, 'text');
}

function setSelectedNoteId(noteId) {
    localStorage.setItem('selectedNoteId', noteId);
}

function getSelectedNoteId() {
    return localStorage.getItem('selectedNoteId');
}
