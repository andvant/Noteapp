import ApiService from "../ApiService.js";

let LoginView = { render, init }

async function render() {
    return /*html*/ `
        <div class="secondary-view">
            <input type="text" placeholder="Email" id="email" />
            <input type="password" placeholder="Password" id="password" />
            <div id="login-button" class="btn btn-lg">Log in</div>
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