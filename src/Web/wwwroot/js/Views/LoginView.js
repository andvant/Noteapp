import * as ApiService from "../ApiService.js";

export {
    render,
    init
}

async function render() {
    return /*html*/ `
        <div id="login-view">
            <label>Email</label>
            <input type="text" placeholder="Email here" id="email" />
            <label>Password</label>
            <input type="text" placeholder="Password here" id="password" />
            <button id="login-button">Log in</button>
        </div>`
}

async function init() {
    const loginButton = document.getElementById('login-button');

    loginButton.addEventListener('click', async () => {
        let email = document.getElementById('email').value;
        let password = document.getElementById('password').value;
        await ApiService.login(email, password);
    });
}
