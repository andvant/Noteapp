let LocalDataManager = {
    saveUserInfo,
    getUserInfo,
    deleteUserInfo,
    loadUserInfoToMemory,
    getNotes,
    saveNotes,
    deleteNotes
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

export default LocalDataManager;