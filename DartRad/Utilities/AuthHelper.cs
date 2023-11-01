using Microsoft.AspNetCore.Identity;

namespace DartRad.Utilities
{
    public class AuthHelper
    {
       
        public static string HashPassword(string password)
        {
            var passwordHasher = new PasswordHasher<string>();
            return passwordHasher.HashPassword(null, password);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            var passwordHasher = new PasswordHasher<string>();
            return passwordHasher.VerifyHashedPassword(null, hashedPassword, password) == PasswordVerificationResult.Success;
        }
    }
}
