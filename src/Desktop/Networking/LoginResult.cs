using Noteapp.Desktop.Dtos;

namespace Noteapp.Desktop.Networking
{
    public class LoginResult : BaseResponse
    {
        public UserInfoResponse UserInfoResponse { get; set; }
    }
}
