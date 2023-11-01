using System.Security.Cryptography;

namespace DartRad.Utilities
{
    public class TokenHelper
    {
        public static string GenerateResetPasswordToken()
        {
            byte[] tokenData = new byte[32]; // 256 bits
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(tokenData);
            }
            return Convert.ToBase64String(tokenData);
        }
    }
}
