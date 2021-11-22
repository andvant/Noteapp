using Noteapp.Core.Entities;
using Noteapp.Desktop.Exceptions;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Noteapp.Desktop.Networking
{
    public class ApiCaller
    {
        private readonly HttpClient _httpClient;

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

        public async Task Login(string email, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/account/login");
            request.Content = JsonContent.Create(new { email, password });
            await SendRequestAsync(request);
        }

        private async Task<IEnumerable<Note>> GetNotes(string filter)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/notes/{filter}");
            var response = await SendRequestAsync(request);
            return await response.Content.ReadFromJsonAsync<IEnumerable<Note>>();
        }

        private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request)
        {
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
    }
}
