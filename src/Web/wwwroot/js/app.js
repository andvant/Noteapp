import * as LoginView from "./LoginView.js";
import * as RegisterView from "./RegisterView.js";
import * as NotesView from "./NotesView.js";

const registerViewButton = document.getElementById('register-view-button');
const loginViewButton = document.getElementById('login-view-button');
const notesViewButton = document.getElementById('notes-view-button');
const currentViewDiv = document.getElementById('current-view');

registerViewButton.addEventListener('click', async () => {
    currentViewDiv.innerHTML = '';

    currentViewDiv.innerHTML = await RegisterView.render();
    await RegisterView.init();
});

loginViewButton.addEventListener('click', async () => {
    currentViewDiv.innerHTML = '';

    currentViewDiv.innerHTML = await LoginView.render();
    await LoginView.init();
});

notesViewButton.addEventListener('click', async () => {
    currentViewDiv.innerHTML = '';

    currentViewDiv.innerHTML = await NotesView.render();
    await NotesView.init();
});

currentViewDiv.innerHTML = await NotesView.render();
await NotesView.init();