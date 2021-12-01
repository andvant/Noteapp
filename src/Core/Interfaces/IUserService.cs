using System.Threading.Tasks;

namespace Noteapp.Core.Interfaces
{
    public interface IUserService
    {
        Task Register(string email, string password);
        Task ValidatePassword(string email, string password);
        string GetEncryptionSalt(string email);
        Task Delete(int userId);
    }
}