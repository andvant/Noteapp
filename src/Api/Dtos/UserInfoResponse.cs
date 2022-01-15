using System;

namespace Noteapp.Api.Dtos
{
    public record UserInfoResponse(string AccessToken, string Email, string EncryptionSalt, DateTime RegistrationDate);
}
