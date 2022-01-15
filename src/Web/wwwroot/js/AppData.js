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

function deleteLocalData() {
    _userInfo = {};
    localStorage.removeItem('userInfo');
    localStorage.removeItem('notes');
}

export default AppData;