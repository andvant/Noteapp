using Noteapp.Desktop.Data;
using Noteapp.Desktop.Dtos;
using Noteapp.Desktop.Logging;
using Noteapp.Desktop.Models;
using Noteapp.Desktop.Security;
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

        public async Task<IEnumerable<Note>> GetNotes()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "notes");
            var response = await SendRequest(request);
            return await GetNotesFromResponse(response);
        }

        public async Task<Note> CreateNote(Note note)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "notes");
            return await GetUpdatedNoteFromServer(request, note);
        }

        public async Task<bool> BulkCreateNotes(IEnumerable<Note> notes)
        {
            var noteRequests = new List<NoteRequest>();

            foreach (var note in notes)
            {
                var noteRequest = new NoteRequest(note);
                noteRequest.Text = await Protector.TryEncrypt(noteRequest.Text);
                noteRequests.Add(noteRequest);
            }

            var request = new HttpRequestMessage(HttpMethod.Post, "notes/bulk");
            request.Content = JsonContent.Create(noteRequests);
            return await SendRequest(request) != null;
        }

        public async Task<Note> UpdateNote(Note note)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"notes/{note.Id}");
            return await GetUpdatedNoteFromServer(request, note);
        }

        public async Task<bool> DeleteNote(int noteId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"notes/{noteId}");
            return await SendRequest(request) != null;
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

        public async Task<UserInfoResponse> Login(string email, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "account/token");
            request.Content = JsonContent.Create(new { email, password });
            var response = await SendRequest(request);
            return response != null ? await response.Content.ReadFromJsonAsync<UserInfoResponse>() : null;
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
            if (!string.IsNullOrWhiteSpace(AppData.UserInfo.AccessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AppData.UserInfo.AccessToken);
            }
        }

        private async Task<Note> GetUpdatedNoteFromServer(HttpRequestMessage request, Note note)
        {
            var noteRequest = new NoteRequest(note);
            noteRequest.Text = await Protector.TryEncrypt(noteRequest.Text);
            request.Content = JsonContent.Create(noteRequest);
            var response = await SendRequest(request);

            if (response == null) return null;
            var returnedNote = await response.Content.ReadFromJsonAsync<Note>();
            returnedNote.Text = await Protector.TryDecrypt(returnedNote.Text);
            return returnedNote;
        }

        private async Task<IEnumerable<Note>> GetNotesFromResponse(HttpResponseMessage response)
        {
            if (response == null) return null;
            var notes = await response.Content.ReadFromJsonAsync<IEnumerable<Note>>();
            foreach (var note in notes)
            {
                note.Text = await Protector.TryDecrypt(note.Text);
            }
            return notes;
        }

        private async Task<IEnumerable<NoteSnapshot>> GetSnapshotsFromResponse(HttpResponseMessage response)
        {
            if (response == null) return null;
            var snapshots = await response.Content.ReadFromJsonAsync<IEnumerable<NoteSnapshot>>();
            foreach (var snapshot in snapshots)
            {
                snapshot.Text = await Protector.TryDecrypt(snapshot.Text);
            }
            return snapshots;
        }
    }
}
