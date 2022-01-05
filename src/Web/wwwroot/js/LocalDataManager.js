let LocalDataManager = {
    saveUserInfo,
    getUserInfo,
    deleteUserInfo,
    loadUserInfoToMemory,
    getNotes,
    saveNotes,
    deleteNotes,
    exportNotes,
    importNotes
}

let _userInfo = null;

function saveUserInfo(userInfo) {
    _userInfo = userInfo;
    localStorage.setItem('userInfo', JSON.stringify(userInfo));
}

function getUserInfo() {
    return _userInfo;
}

function loadUserInfoToMemory() {
    _userInfo = localStorage.getItem('userInfo');
}

function deleteUserInfo() {
    _userInfo = null;
    localStorage.removeItem('userInfo');
}

function getNotes() {
    let notesJson = localStorage.getItem('notes');
    return notesJson != null ? JSON.parse(notesJson) : [];
}

function saveNotes(notes) {
    localStorage.setItem('notes', JSON.stringify(notes));
}

function deleteNotes() {
    localStorage.removeItem('notes');
}

function exportNotes(notes) {
    let notesJson = JSON.stringify(notes, null, 2);
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

export default LocalDataManager;