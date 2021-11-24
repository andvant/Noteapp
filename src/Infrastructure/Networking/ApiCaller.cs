using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Noteapp.Infrastructure.Networking
{
    public class ApiCaller : IApiCaller
    {
        private readonly HttpClient _httpClient;

        public string AccessToken { get; set; }

        public ApiCaller(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Note>> GetNonArchivedNotes()
        {
            return await GetNotes("?archived=false");
        }

        public async Task<IEnumerable<Note>> GetArchivedNotes()
        {
            return await GetNotes("?archived=true");
        }

        public async Task<IEnumerable<Note>> GetAllNotes()
        {
            return await GetNotes(string.Empty);
        }

        public async Task CreateNote()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/notes");
            request.Content = JsonContent.Create(new { text = string.Empty });
            await SendRequestAsync(request);
        }

        public async Task BulkCreateNotes(IEnumerable<Note> notes)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/notes/bulk");
            request.Content = JsonContent.Create(notes);
            await SendRequestAsync(request);
        }

        public async Task EditNote(int noteId, string text)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"api/notes/{noteId}");
            request.Content = JsonContent.Create(new { text });
            await SendRequestAsync(request);
        }

        public async Task DeleteNote(int noteId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/notes/{noteId}");
            await SendRequestAsync(request);
        }

        public async Task ToggleLocked(int noteId, bool locked)
        {
            var method = locked ? HttpMethod.Delete : HttpMethod.Put;
            var request = new HttpRequestMessage(method, $"api/notes/{noteId}/lock");
            await SendRequestAsync(request);
        }

        public async Task ToggleArchived(int noteId, bool archived)
        {
            var method = archived ? HttpMethod.Delete : HttpMethod.Put;
            var request = new HttpRequestMessage(method, $"api/notes/{noteId}/archive");
            await SendRequestAsync(request);
        }

        public async Task TogglePinned(int noteId, bool pinned)
        {
            var method = pinned ? HttpMethod.Delete : HttpMethod.Put;
            var request = new HttpRequestMessage(method, $"api/notes/{noteId}/pin");
            await SendRequestAsync(request);
        }

        public async Task TogglePublished(int noteId, bool published)
        {
            var method = published ? HttpMethod.Delete : HttpMethod.Put;
            var request = new HttpRequestMessage(method, $"api/notes/{noteId}/publish");
            await SendRequestAsync(request);
        }

        public async Task Register(string email, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/account/register");
            request.Content = JsonContent.Create(new { email, password });
            await SendRequestAsync(request);
        }

        public async Task<UserInfo> Login(string email, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/account/token");
            request.Content = JsonContent.Create(new { email, password });
            var response = await SendRequestAsync(request);

            return await response.Content.ReadFromJsonAsync<UserInfo>();
        }

        private async Task<IEnumerable<Note>> GetNotes(string filter)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/notes/{filter}");
            var response = await SendRequestAsync(request);
            return await response.Content.ReadFromJsonAsync<IEnumerable<Note>>();
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
            if (!string.IsNullOrWhiteSpace(AccessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            }
        }
    }
}
