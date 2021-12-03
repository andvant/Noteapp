import ApiService from "../ApiService.js"

let SettingsView = { render, init }

async function render() {
    let email = JSON.parse(localStorage.getItem('userInfo'))?.email ?? "Anonymous";

    return /*html*/ `
        <div id="settings-view">
            <label>Email: ${email}</label>
            <div id="logout-button" class="btn">Log out</div>
            <div id="delete-account-button" class="btn">Delete account</div>
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