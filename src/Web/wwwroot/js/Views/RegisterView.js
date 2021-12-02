import ApiService from "../ApiService.js";

let RegisterView = { render, init }

async function render() {
    return /*html*/ `
        <div id="register-view">
            <label>Email</label>
            <input type="text" placeholder="Email here" id="email" />
            <label>Password</label>
            <input type="text" placeholder="Password here" id="password" />
            <button id="register-button">Register</button>
        </div>`
}

async function init() {
    const registerButton = document.getElementById('register-button');

    registerButton.addEventListener('click', async () => {
        let email = document.getElementById('email').value;
        let password = document.getElementById('password').value;
        await ApiService.register(email, password);
    });
}

export default RegisterView;