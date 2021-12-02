import ApiService from "../ApiService.js"

let SettingsView = { render, init }

async function render() {
    let email = JSON.parse(localStorage.getItem('userInfo'))?.email ?? "Anonymous";

    return /*html*/ `
        <div id="settings-view">
            <label>Email</label>
            <label>${email}</label>
            <button id="logout-button">Log out</button>
            <button id="delete-account-button">Delete account</button>
        </div>`
}

async function init() {
    const logoutButton = document.getElementById('logout-button');
    const deleteAccountButton = document.getElementById('delete-account-button');

    logoutButton.addEventListener('click', () => {
        ApiService.logout();
    });

    deleteAccountButton.addEventListener('click', async () => {
        await ApiService.deleteAccount();
    });
}

export default SettingsView;