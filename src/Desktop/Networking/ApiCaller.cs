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

        public async Task<IEnumerable<Note>> GetNotes()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, string.Empty);
            var response = await SendRequestAsync(request);
            return await response.Content.ReadFromJsonAsync<IEnumerable<Note>>();
        }

        public async Task CreateNote()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, string.Empty);
            request.Content = JsonContent.Create(new { text = string.Empty });
            await SendRequestAsync(request);
        }

        public async Task EditNote(int noteId, string text)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"{noteId}");
            request.Content = JsonContent.Create(new { text });
            await SendRequestAsync(request);
        }

        public async Task DeleteNote(int noteId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{noteId}");
            await SendRequestAsync(request);
        }

        public async Task ToggleLocked(int noteId, bool locked)
        {
            var method = locked ? HttpMethod.Delete : HttpMethod.Put;
            var request = new HttpRequestMessage(method, $"{noteId}/lock");
            await SendRequestAsync(request);
        }

        public async Task ToggleArchived(int noteId, bool archived)
        {
            var method = archived ? HttpMethod.Delete : HttpMethod.Put;
            var request = new HttpRequestMessage(method, $"{noteId}/archive");
            await SendRequestAsync(request);
        }

        public async Task TogglePinned(int noteId, bool pinned)
        {
            var method = pinned ? HttpMethod.Delete : HttpMethod.Put;
            var request = new HttpRequestMessage(method, $"{noteId}/pin");
            await SendRequestAsync(request);
        }

        public async Task TogglePublished(int noteId, bool published)
        {
            var method = published ? HttpMethod.Delete : HttpMethod.Put;
            var request = new HttpRequestMessage(method, $"{noteId}/publish");
            await SendRequestAsync(request);
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
