using System.Security.Cryptography;
using System.Text;

namespace FInantialAPI.Utilities
{
    public class HashingUtilities
    {
        public static string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public static string HashPin(string pin, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] saltedPinBytes = Encoding.UTF8.GetBytes(pin + salt);
                byte[] hashBytes = sha256.ComputeHash(saltedPinBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
