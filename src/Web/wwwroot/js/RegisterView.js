import * as AjaxService from "./AjaxService.js";

export {
    render,
    init
}

async function render() {
    return `
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
        await AjaxService.register(email, password);
    });
}
