import ApiService from "../ApiService.js";
import AppData from "../AppData.js";

let LoginView = { render, init }

async function render() {
    return /*html*/ `
        <div class="secondary-view">
            <input type="text" placeholder="Email" id="email" />
            <input type="password" placeholder="Password" id="password" />
            <label for="stay-signed-in">Stay signed in</label>
            <input type="checkbox" placeholder="Password" id="stay-signed-in" checked/>
            <div id="login-button" class="btn btn-lg">Log in</div>
        </div>`
}

async function init() {

    const loginButton = document.getElementById('login-button');

    loginButton.addEventListener('click', login);

    async function login() {
        let email = document.getElementById('email').value;
        let password = document.getElementById('password').value;
        let staySignedIn = document.getElementById('stay-signed-in').checked;

        let userInfoResponse = await ApiService.login(email, password);

        if (userInfoResponse != null) {
            AppData.createAndSaveUserInfo(userInfoResponse, staySignedIn);
            alert('Successfully logged in.')
        }
    }
}

export default LoginView;