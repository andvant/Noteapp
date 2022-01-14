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
            <div id="output-message"></div>
        </div>`
}

async function init() {

    const loginButton = document.getElementById('login-button');
    const outputMessageDiv = document.getElementById('output-message');

    loginButton.addEventListener('click', login);

    async function loginX() {
        let email = document.getElementById('email').value;
        let password = document.getElementById('password').value;
        let staySignedIn = document.getElementById('stay-signed-in').checked;
        
        outputMessageDiv.textContent = '';
        let userInfoResponse = await ApiService.login(email, password);

        if (userInfoResponse != null) {
            AppData.createAndSaveUserInfo(userInfoResponse, staySignedIn);
            outputMessageDiv.textContent = 'Successfully logged in';
        }
        else {
            outputMessageDiv.textContent = 'Failed to log in';
        }
    }

    async function login() {
        let email = document.getElementById('email').value;
        let password = document.getElementById('password').value;
        let staySignedIn = document.getElementById('stay-signed-in').checked;
        
        outputMessageDiv.textContent = '';
        let result = await ApiService.login(email, password);
        
        if (result.isSuccess()) {
            let userInfoResponse = result.content;
            AppData.createAndSaveUserInfo(userInfoResponse, staySignedIn);
            outputMessageDiv.textContent = 'Successfully logged in';
        }
        else {
            outputMessageDiv.textContent = `Failed to log in: ${result.errorMessage}`;
        }
    }
}

export default LoginView;