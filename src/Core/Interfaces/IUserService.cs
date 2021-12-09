using System.Threading.Tasks;

namespace Noteapp.Core.Interfaces
{
    public interface IUserService
    {
        public Task Register(string email, string password);
        public Task ValidatePassword(string email, string password);
        public Task<string> GetEncryptionSalt(string email);
        public Task Delete(int userId);
    }
}