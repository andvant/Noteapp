namespace Noteapp.Core.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(string userEmail);
    }
}