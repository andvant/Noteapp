import ApiService from "../ApiService.js";

let LoginView = { render, init }

async function render() {
    return /*html*/ `
        <div id="login-view">
            <input type="text" placeholder="Email" id="email" />
            <input type="text" placeholder="Password" id="password" />
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

export default LoginView;