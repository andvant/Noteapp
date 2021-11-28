import * as LoginView from "./Views/LoginView.js";
import * as RegisterView from "./Views/RegisterView.js";
import * as NotesView from "./Views/NotesView.js";
import * as SettingsView from "./Views/SettingsView.js";

const registerViewButton = document.getElementById('register-view-button');
const loginViewButton = document.getElementById('login-view-button');
const notesViewButton = document.getElementById('notes-view-button');
const settingsViewButton = document.getElementById('settings-view-button');
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

settingsViewButton.addEventListener('click', async () => {
    currentViewDiv.innerHTML = '';

    currentViewDiv.innerHTML = await SettingsView.render();
    await SettingsView.init();
});

currentViewDiv.innerHTML = await NotesView.render();
await NotesView.init();