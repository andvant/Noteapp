﻿import ApiService from "../ApiService.js";

let RegisterView = { render, init }

async function render() {
    return /*html*/ `
        <div class="secondary-view">
            <input type="text" placeholder="Email" id="email" />
            <input type="password" placeholder="Password" id="password" />
            <div id="register-button" class="btn btn-lg">Register</div>
        </div>`
}

async function init() {

    const registerButton = document.getElementById('register-button');

    registerButton.addEventListener('click', register);

    async function register() {
        let email = document.getElementById('email').value;
        let password = document.getElementById('password').value;
        
        if (await ApiService.register(email, password)) {
            alert('Successfully registered.')
        }
    }
}

export default RegisterView;