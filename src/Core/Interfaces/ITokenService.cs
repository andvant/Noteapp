using System.Threading.Tasks;

namespace Noteapp.Core.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateToken(string userEmail);
    }
}