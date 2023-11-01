using DartRad.Models;

namespace DartRad.Services
{
    public interface IAuthService
    {
        Task<AuthResult> Authenticate(string email, string password, string role);
    }
}
