namespace Noteapp.Api.Dtos
{
    public record UserInfoResponse(string access_token, string email, string encryption_salt);
}
