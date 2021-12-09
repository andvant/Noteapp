using System.Threading.Tasks;

namespace Noteapp.Core.Interfaces
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(string userEmail);
    }
}