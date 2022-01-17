import ApiService from "../ApiService.js"
import AppData from "../AppData.js";
import Utils from "../Utils.js";

let SettingsView = { render, init }

async function render() {
    let userInfo = AppData.getUserInfo();
    let email = userInfo.email ?? "Anonymous";
    let registrationDate = Utils.dateToLocaleString(userInfo.registrationDate);

    return /*html*/ `
        <div class="secondary-view">
            <div class="settings-row">
                <label>Email</label><label>${email}</label>
            </div>
            <div class="settings-row">
                <label>Registration date</label><label>${registrationDate}</label>
            </div>
            <div id="logout-button" class="btn btn-lg">Log out</div>
            <div id="delete-account-button" class="btn btn-lg">Delete account</div>
            <div id="output-message"></div>
        </div>`
}

async function init() {

    const logoutButton = document.getElementById('logout-button');
    const deleteAccountButton = document.getElementById('delete-account-button');
    const outputMessageDiv = document.getElementById('output-message');

    logoutButton.addEventListener('click', logout);
    deleteAccountButton.addEventListener('click', deleteAccount);

    function logout() {
        AppData.deleteLocalData();
        outputMessageDiv.textContent = 'Successfully logged out';
    }

    async function deleteAccount() {
        if (await ApiService.deleteAccount()) {
            outputMessageDiv.textContent = 'Account successfully deleted';
            logout();
        }
        else {
            outputMessageDiv.textContent = 'Failed to delete account';
        }
    }
}

export default SettingsView;