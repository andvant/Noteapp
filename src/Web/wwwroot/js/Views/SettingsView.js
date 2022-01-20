import ApiService from "../ApiService.js"
import AppData from "../AppData.js";
import Utils from "../Utils.js";

let SettingsView = { render, init }

async function render() {
    return /*html*/ `
        <div class="secondary-view">
            <div class="settings-row">
                <label>Email</label><label id="email"></label>
            </div>
            <div class="settings-row">
                <label>Registration date</label><label id="registration-date"></label>
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
    const email = document.getElementById('email');
    const registrationDate = document.getElementById('registration-date');

    logoutButton.addEventListener('click', logout);
    deleteAccountButton.addEventListener('click', deleteAccount);

    updateInfo();

    function updateInfo() {
        let userInfo = AppData.getUserInfo();
        email.textContent = userInfo.email ?? "Anonymous";
        registrationDate.textContent = Utils.dateToLocaleString(userInfo.registrationDate);;
    }

    function logout() {
        AppData.deleteLocalData();
        outputMessageDiv.textContent = 'Successfully logged out';
        updateInfo();
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