import LoginView    from "./Views/LoginView.js";
import RegisterView from "./Views/RegisterView.js";
import NotesView    from "./Views/NotesView.js";
import SettingsView from "./Views/SettingsView.js";
import LocalDataManager from "./LocalDataManager.js";

const registerViewButton = document.getElementById('register-view-button');
const loginViewButton    = document.getElementById('login-view-button');
const notesViewButton    = document.getElementById('notes-view-button');
const settingsViewButton = document.getElementById('settings-view-button');
const currentViewDiv     = document.getElementById('current-view');

registerViewButton.onclick = async () => await renderView(RegisterView);
loginViewButton.onclick    = async () => await renderView(LoginView);
notesViewButton.onclick    = async () => await renderView(NotesView);
settingsViewButton.onclick = async () => await renderView(SettingsView);

LocalDataManager.loadUserInfoToMemory();
await renderView(NotesView);

async function renderView(view) {
    currentViewDiv.innerHTML = await view.render();
    await view.init();
}