import ApiService from "../ApiService.js"
import LocalDataManager from "../LocalDataManager.js";
import Utils from "../Utils.js";

let SettingsView = { render, init }

async function render() {
    let userInfo = LocalDataManager.getUserInfo();
    let email = userInfo.email ?? "Anonymous";
    let registrationDate = Utils.dateToLocaleString(userInfo.registration_date) ?? "";

    return /*html*/ `
        <div class="secondary-view">
            <label>Email: ${email}</label>
            <label>Registration date: ${registrationDate}</label>
            <div id="logout-button" class="btn btn-lg">Log out</div>
            <div id="delete-account-button" class="btn btn-lg">Delete account</div>
        </div>`
}

async function init() {

    const logoutButton = document.getElementById('logout-button');
    const deleteAccountButton = document.getElementById('delete-account-button');

    logoutButton.addEventListener('click', logout);
    deleteAccountButton.addEventListener('click', deleteAccount);

    function logout() {
        LocalDataManager.deleteUserInfo();
        LocalDataManager.deleteNotes();
        alert('Successfully logged out.');
    }

    async function deleteAccount() {
        if (await ApiService.deleteAccount()) {
            alert('Account successfully deleted.');
            logout();
        }
    }
}

export default SettingsView;