let AppData = {
    createAndSaveUserInfo,
    getUserInfo,
    saveUserInfo,
    deleteUserInfo,
    loadUserInfoToMemory,
    readNotes,
    saveNotes,
    deleteNotes,
    exportNotes,
    importNotes
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

function deleteUserInfo() {
    _userInfo = {};
    localStorage.removeItem('userInfo');
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

export default AppData;