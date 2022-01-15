using System;

namespace Noteapp.Desktop.Dtos
{
    public record UserInfoResponse(string AccessToken, string Email, string EncryptionSalt, DateTime RegistrationDate);
}
