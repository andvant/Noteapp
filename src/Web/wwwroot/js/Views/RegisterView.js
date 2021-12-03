import ApiService from "../ApiService.js";

let RegisterView = { render, init }

async function render() {
    return /*html*/ `
        <div id="register-view">
            <input type="text" placeholder="Email" id="email" />
            <input type="text" placeholder="Password" id="password" />
            <div id="register-button" class="btn">Register</div>
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