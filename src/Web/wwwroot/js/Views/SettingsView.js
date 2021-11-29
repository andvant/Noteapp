import * as AjaxService from "../AjaxService.js"

export {
    render,
    init
}

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
        AjaxService.logout();
    });

    deleteAccountButton.addEventListener('click', async () => {
        await AjaxService.deleteAccount();
    });
}
