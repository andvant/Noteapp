let Notes = [];

let LocalDataManager = {
    createAndSaveUserInfo,
    saveUserInfo,
    getUserInfo,
    deleteUserInfo,
    loadUserInfoToMemory,
    readNotes,
    saveNotes,
    deleteNotes,
    exportNotes,
    importNotes,
    Notes
}

let _userInfo = null;
let _isPersisted = true;

function createAndSaveUserInfo(userInfoResponse, isPersisted) {
    _isPersisted = isPersisted
    saveUserInfo(userInfoResponse);
}

function saveUserInfo(userInfo) {
    _userInfo = userInfo;
    if (_isPersisted) {
        localStorage.setItem('userInfo', JSON.stringify(userInfo));
    }
}

function getUserInfo() {
    return _userInfo ?? {};
}

function loadUserInfoToMemory() {
    _userInfo = JSON.parse(localStorage.getItem('userInfo'));
}

function deleteUserInfo() {
    _userInfo = null;
    localStorage.removeItem('userInfo');
}

function readNotes() {
    let notesJson = localStorage.getItem('notes');
    return notesJson != null ? JSON.parse(notesJson) : [];
}

function saveNotes() {
    if (_isPersisted) {
        localStorage.setItem('notes', JSON.stringify(Notes));
    }
}

function deleteNotes() {
    Notes = [];
    localStorage.removeItem('notes');
}

function exportNotes() {
    let notesJson = JSON.stringify(Notes, null, 2);
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