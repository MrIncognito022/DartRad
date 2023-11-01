using System;
using System.Security.Cryptography;
namespace DartRad.Utilities
{
    public class InviteCodeGenerator
    {
        public static string GenerateInviteCode()
        {
            // Generate a new GUID
            Guid guid = Guid.NewGuid();

            // Convert the GUID to a byte array
            byte[] bytes = guid.ToByteArray();

            // Create a new instance of the SHA256 hashing algorithm
            SHA256 sha256 = SHA256.Create();

            // Compute the hash of the byte array
            byte[] hash = sha256.ComputeHash(bytes);

            // Convert the hash to a string
            string hashString = BitConverter.ToString(hash).Replace("-", "").ToLower();

            // Take the first 8 characters of the hash as the invitation code
            string inviteCode = hashString.Substring(0, 8);

            return inviteCode;
        }
    }
    
}
