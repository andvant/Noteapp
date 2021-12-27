using Noteapp.Desktop.Logging;
using Noteapp.Desktop.Models;
using Noteapp.Desktop.Security;
using Noteapp.Desktop.Session;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Noteapp.Desktop.Networking
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Note>> GetNotes(bool? archived = null)
        {
            string filter = archived.HasValue ? $"?archived={archived.Value}" : string.Empty;
            var request = new HttpRequestMessage(HttpMethod.Get, $"notes{filter}");
            var response = await SendRequest(request);
            return await GetNotesFromResponse(response);
        }

        public async Task<Note> CreateNote(string text = "")
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "notes");
            request.Content = JsonContent.Create(new { text = await TryEncrypt(text) });
            var response = await SendRequest(request);
            return await GetNoteFromResponse(response);
        }

        public async Task<bool> BulkCreateNotes(IEnumerable<Note> notes)
        {
            foreach (var note in notes)
            {
                note.Text = await TryEncrypt(note.Text);
            }

            var request = new HttpRequestMessage(HttpMethod.Post, "notes/bulk");
            request.Content = JsonContent.Create(notes);
            return await SendRequest(request) != null;
        }

        public async Task<Note> UpdateNote(int noteId, string text)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"notes/{noteId}");
            request.Content = JsonContent.Create(new { text = await TryEncrypt(text) });
            var response = await SendRequest(request);
            return await GetNoteFromResponse(response);
        }

        public async Task<bool> DeleteNote(int noteId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"notes/{noteId}");
            return await SendRequest(request) != null;
        }

        public async Task<Note> ToggleLocked(int noteId, bool locked)
        {
            var method = locked ? HttpMethod.Delete : HttpMethod.Put;
            var request = new HttpRequestMessage(method, $"notes/{noteId}/lock");
            var response = await SendRequest(request);
            return await GetNoteFromResponse(response);
        }

        public async Task<Note> ToggleArchived(int noteId, bool archived)
        {
            var method = archived ? HttpMethod.Delete : HttpMethod.Put;
            var request = new HttpRequestMessage(method, $"notes/{noteId}/archive");
            var response = await SendRequest(request);
            return await GetNoteFromResponse(response);
        }

        public async Task<Note> TogglePinned(int noteId, bool pinned)
        {
            var method = pinned ? HttpMethod.Delete : HttpMethod.Put;
            var request = new HttpRequestMessage(method, $"notes/{noteId}/pin");
            var response = await SendRequest(request);
            return await GetNoteFromResponse(response);
        }

        public async Task<Note> TogglePublished(int noteId, bool published)
        {
            var method = published ? HttpMethod.Delete : HttpMethod.Put;
            var request = new HttpRequestMessage(method, $"notes/{noteId}/publish");
            var response = await SendRequest(request);
            return await GetNoteFromResponse(response);
        }

        public async Task<IEnumerable<NoteSnapshot>> GetAllSnapshots(int noteId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"notes/{noteId}/snapshots");
            var response = await SendRequest(request);
            return await GetSnapshotsFromResponse(response);
        }

        public async Task<bool> Register(string email, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "account/register");
            request.Content = JsonContent.Create(new { email, password });
            return await SendRequest(request) != null;
        }

        public async Task<UserInfoDto> Login(string email, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "account/token");
            request.Content = JsonContent.Create(new { email, password });
            var response = await SendRequest(request);
            return response != null ? await response.Content.ReadFromJsonAsync<UserInfoDto>() : null;
        }

        public async Task<bool> DeleteAccount()
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, "account/delete");
            return await SendRequest(request) != null;
        }

        private async Task<HttpResponseMessage> SendRequest(HttpRequestMessage request)
        {
            AddAuthorizationHeader(request);

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request);
            }
            catch (HttpRequestException ex)
            {
                Logger.Log(ex.Message);
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = $"{response.ReasonPhrase}\n{await response.Content.ReadAsStringAsync()}";
                Logger.Log(errorMessage);
                return null;
            }

            return response;
        }

        private void AddAuthorizationHeader(HttpRequestMessage request)
        {
            var userInfo = SessionManager.GetUserInfo();
            if (!string.IsNullOrWhiteSpace(userInfo?.AccessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userInfo.AccessToken);
            }
        }

        private async Task<Note> GetNoteFromResponse(HttpResponseMessage response)
        {
            if (response == null) return null;
            var note = await response.Content.ReadFromJsonAsync<Note>();
            note.Text = await TryDecrypt(note.Text);
            return note;
        }

        private async Task<IEnumerable<Note>> GetNotesFromResponse(HttpResponseMessage response)
        {
            if (response == null) return null;
            var notes = await response.Content.ReadFromJsonAsync<IEnumerable<Note>>();
            foreach (var note in notes)
            {
                note.Text = await TryDecrypt(note.Text);
            }
            return notes;
        }

        private async Task<IEnumerable<NoteSnapshot>> GetSnapshotsFromResponse(HttpResponseMessage response)
        {
            if (response == null) return null;
            var snapshots = await response.Content.ReadFromJsonAsync<IEnumerable<NoteSnapshot>>();
            foreach (var snapshot in snapshots)
            {
                snapshot.Text = await TryDecrypt(snapshot.Text);
            }
            return snapshots;
        }

        private async Task<string> TryDecrypt(string text)
        {
            var userInfo = SessionManager.GetUserInfo();

            if (userInfo is null || !userInfo.EncryptionEnabled)
            {
                return text;
            }

            try
            {
                return await Protector.Decrypt(text, userInfo.EncryptionKey);
            }
            catch // text was not encrypted and thus could not be decrypted
            {
                return text;
            }
        }

        private async Task<string> TryEncrypt(string text)
        {
            var userInfo = SessionManager.GetUserInfo();

            if (userInfo is null || !userInfo.EncryptionEnabled)
            {
                return text;
            }

            return await Protector.Encrypt(text, userInfo.EncryptionKey);
        }
    }
}
