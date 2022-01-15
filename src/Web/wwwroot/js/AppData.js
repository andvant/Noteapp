let AppData = {
    createAndSaveUserInfo,
    getUserInfo,
    saveUserInfo,
    loadUserInfoToMemory,
    readNotes,
    saveNotes,
    exportNotes,
    importNotes,
    deleteLocalData
}

let _userInfo = {};
let _isPersisted = true;

function getUserInfo() {
    return _userInfo;
}

function createAndSaveUserInfo(userInfoResponse, isPersisted) {
    _isPersisted = isPersisted;
    saveUserInfo(userInfoResponse);
}

function saveUserInfo(userInfo) {
    _userInfo = userInfo;
    if (_isPersisted) {
        localStorage.setItem('userInfo', JSON.stringify(_userInfo));
    }
}

function loadUserInfoToMemory() {
    _userInfo = JSON.parse(localStorage.getItem('userInfo')) ?? {};
}

function readNotes() {
    let notesJson = localStorage.getItem('notes');
    return notesJson != null ? JSON.parse(notesJson) : [];
}

function saveNotes(notes) {
    if (_isPersisted) {
        localStorage.setItem('notes', JSON.stringify(notes));
    }
}

function exportNotes(notes) {
    let exportedNotes = [];
    for (let note of notes) {
        exportedNotes.push(new ExportedNote(note));
    }
    let notesJson = JSON.stringify(exportedNotes, null, 2);
    let blob = new Blob([notesJson], {
        type: "application/json"
    });
    let a = document.createElement("a");
    a.href = URL.createObjectURL(blob);
    a.download = `ExportedNotes-${new Date().toLocaleDateString()}.json`;
    a.click();
}

function importNotes(file) {
    return new Promise((resolve) => {
        const reader = new FileReader();
        reader.onloadend = () => resolve(JSON.parse(reader.result));
        reader.readAsText(file);
    });
}

function deleteLocalData() {
    _userInfo = {};
    localStorage.removeItem('userInfo');
    localStorage.removeItem('notes');
}

function ExportedNote(note) {
    this.id = note.id;
    this.created = note.created;
    this.updated = note.updatedLocal;
    this.text = note.text;
    this.locked = note.locked;
    this.archived = note.archived;
    this.pinned = note.pinned;
    this.published = note.published;
}

export default AppData;