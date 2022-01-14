using System.Net.Http;

namespace Noteapp.Desktop.Networking
{
    public class ApiResponse : BaseResponse
    {
        public HttpContent Content { get; set; }
    }
}
