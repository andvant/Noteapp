import * as AjaxService from "./AjaxService.js";

export {
    render,
    init
}

async function render() {
    return `
        <div id="login-view">
            <label>Email</label>
            <input type="text" placeholder="Email here" id="email" />
            <label>Password</label>
            <input type="text" placeholder="Password here" id="password" />
            <button id="login-button">Log in</button>
            <button id="logout-button">Log out</button>
        </div>`
}

async function init() {
    const loginButton = document.getElementById('login-button');
    const logoutButton = document.getElementById('logout-button');

    loginButton.addEventListener('click', async () => {
        let email = document.getElementById('email').value;
        let password = document.getElementById('password').value;
        await AjaxService.login(email, password);
    });

    logoutButton.addEventListener('click', () => {
        AjaxService.logout();
    });
}
