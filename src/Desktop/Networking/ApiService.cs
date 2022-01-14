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
            return await ReadNotesFromResponse(response);
        }

        public async Task<Note> CreateNote(Note note)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "notes");
            return await SendRequestAndReadNoteFromResponse(request, note);
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
            return (await SendRequest(request)).IsSuccess;
        }

        public async Task<Note> UpdateNote(Note note)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"notes/{note.Id}");
            return await SendRequestAndReadNoteFromResponse(request, note);
        }

        public async Task<bool> DeleteNote(int noteId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"notes/{noteId}");
            return (await SendRequest(request)).IsSuccess;
        }

        public async Task<IEnumerable<NoteSnapshot>> GetAllSnapshots(int noteId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"notes/{noteId}/snapshots");
            var response = await SendRequest(request);
            return await ReadSnapshotsFromResponse(response);
        }

        public async Task<RegisterResult> Register(string email, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "account/register");
            request.Content = JsonContent.Create(new { email, password });
            var response = await SendRequest(request, false);

            var registerResult = new RegisterResult();
            registerResult.ErrorMessage = response.ErrorMessage;
            return registerResult;
        }

        public async Task<LoginResult> Login(string email, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "account/token");
            request.Content = JsonContent.Create(new { email, password });
            var response = await SendRequest(request, false);

            var loginResult = new LoginResult();
            loginResult.ErrorMessage = response.ErrorMessage;
            loginResult.UserInfoResponse = response.IsSuccess ?
                await response.Content.ReadFromJsonAsync<UserInfoResponse>() : null;
            return loginResult;
        }

        public async Task<bool> DeleteAccount()
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, "account/delete");
            return (await SendRequest(request)).IsSuccess;
        }

        private async Task<ApiResponse> SendRequest(HttpRequestMessage request, bool authorized = true)
        {
            if (authorized) AddAuthorizationHeader(request);

            HttpResponseMessage response;
            var apiResponse = new ApiResponse();
            try
            {
                response = await _httpClient.SendAsync(request);
            }
            catch (HttpRequestException ex)
            {
                Logger.Log(ex.Message);
                apiResponse.ErrorMessage = "Failed to connect to the server";
                return apiResponse;
            }

            if (response.IsSuccessStatusCode)
            {
                apiResponse.Content = response.Content;
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                Logger.Log(error.ErrorMessage);
                apiResponse.ErrorMessage = error.ErrorMessage ?? "ERROR";
            }

            return apiResponse;
        }

        private void AddAuthorizationHeader(HttpRequestMessage request)
        {
            var accessToken = AppData.UserInfo.AccessToken;
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }

        private async Task<Note> SendRequestAndReadNoteFromResponse(HttpRequestMessage request, Note note)
        {
            var noteRequest = new NoteRequest(note);
            noteRequest.Text = await Protector.TryEncrypt(noteRequest.Text);
            request.Content = JsonContent.Create(noteRequest);
            var response = await SendRequest(request);

            if (!response.IsSuccess) return null;
            var updatedNote = await response.Content.ReadFromJsonAsync<Note>();
            updatedNote.Text = await Protector.TryDecrypt(updatedNote.Text);
            return updatedNote;
        }

        private async Task<IEnumerable<Note>> ReadNotesFromResponse(ApiResponse response)
        {
            if (!response.IsSuccess) return null;
            var notes = await response.Content.ReadFromJsonAsync<IEnumerable<Note>>();
            foreach (var note in notes)
            {
                note.Text = await Protector.TryDecrypt(note.Text);
            }
            return notes;
        }

        private async Task<IEnumerable<NoteSnapshot>> ReadSnapshotsFromResponse(ApiResponse response)
        {
            if (!response.IsSuccess) return null;
            var snapshots = await response.Content.ReadFromJsonAsync<IEnumerable<NoteSnapshot>>();
            foreach (var snapshot in snapshots)
            {
                snapshot.Text = await Protector.TryDecrypt(snapshot.Text);
            }
            return snapshots;
        }
    }
}
