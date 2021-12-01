using Noteapp.Desktop.Exceptions;
using Noteapp.Desktop.Models;
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
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/notes{filter}");
            var response = await SendRequestAsync(request);
            return await response.Content.ReadFromJsonAsync<IEnumerable<Note>>();
        }

        public async Task<Note> CreateNote()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/notes");
            request.Content = JsonContent.Create(new { text = string.Empty });
            var response = await SendRequestAsync(request);
            return await response.Content.ReadFromJsonAsync<Note>();
        }

        public async Task BulkCreateNotes(IEnumerable<Note> notes)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/notes/bulk");
            request.Content = JsonContent.Create(notes);
            await SendRequestAsync(request);
        }

        public async Task<Note> UpdateNote(int noteId, string text)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"api/notes/{noteId}");
            request.Content = JsonContent.Create(new { text });
            var response = await SendRequestAsync(request);
            return await response.Content.ReadFromJsonAsync<Note>();
        }

        public async Task DeleteNote(int noteId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/notes/{noteId}");
            await SendRequestAsync(request);
        }

        public async Task<Note> ToggleLocked(int noteId, bool locked)
        {
            var method = locked ? HttpMethod.Delete : HttpMethod.Put;
            var request = new HttpRequestMessage(method, $"api/notes/{noteId}/lock");
            var response = await SendRequestAsync(request);
            return await response.Content.ReadFromJsonAsync<Note>();
        }

        public async Task<Note> ToggleArchived(int noteId, bool archived)
        {
            var method = archived ? HttpMethod.Delete : HttpMethod.Put;
            var request = new HttpRequestMessage(method, $"api/notes/{noteId}/archive");
            var response = await SendRequestAsync(request);
            return await response.Content.ReadFromJsonAsync<Note>();
        }

        public async Task<Note> TogglePinned(int noteId, bool pinned)
        {
            var method = pinned ? HttpMethod.Delete : HttpMethod.Put;
            var request = new HttpRequestMessage(method, $"api/notes/{noteId}/pin");
            var response = await SendRequestAsync(request);
            return await response.Content.ReadFromJsonAsync<Note>();
        }

        public async Task<Note> TogglePublished(int noteId, bool published)
        {
            var method = published ? HttpMethod.Delete : HttpMethod.Put;
            var request = new HttpRequestMessage(method, $"api/notes/{noteId}/publish");
            var response = await SendRequestAsync(request);
            return await response.Content.ReadFromJsonAsync<Note>();
        }

        public async Task<IEnumerable<NoteSnapshot>> GetAllSnapshots(int noteId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/notes/{noteId}/snapshots");
            var response = await SendRequestAsync(request);
            return await response.Content.ReadFromJsonAsync<IEnumerable<NoteSnapshot>>();
        }

        public async Task Register(string email, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/account/register");
            request.Content = JsonContent.Create(new { email, password });
            await SendRequestAsync(request);
        }

        public async Task<UserInfoDto> Login(string email, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/account/token");
            request.Content = JsonContent.Create(new { email, password });
            var response = await SendRequestAsync(request);

            return await response.Content.ReadFromJsonAsync<UserInfoDto>();
        }

        public async Task DeleteAccount()
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, "api/account/delete");
            await SendRequestAsync(request);
        }

        private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request)
        {
            AddAuthorizationHeader(request);

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request);
            }
            catch (HttpRequestException ex)
            {
                throw new ApiConnectionException(ex);
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = $"{response.ReasonPhrase}\n{await response.Content.ReadAsStringAsync()}";

                throw new ApiBadResponseException(errorMessage);
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
    }
}
