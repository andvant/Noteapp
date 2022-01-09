using System;

namespace Noteapp.Api.Dtos
{
    public record UserInfoResponse(string access_token, string email, string encryption_salt, DateTime registration_date);
}
