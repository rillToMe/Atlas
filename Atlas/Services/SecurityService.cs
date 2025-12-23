using System.Security.Cryptography;
using System.Text;

namespace Atlas.Services
{
    public class SecurityService
    {
        public string HashPin(string pin)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(pin);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public bool VerifyPin(string pin, string hash)
        {
            var pinHash = HashPin(pin);
            return pinHash == hash;
        }

        public bool IsValidPin(string pin)
        {
            // Numeric only, 4-8 digits
            if (string.IsNullOrWhiteSpace(pin))
                return false;

            if (pin.Length < 4 || pin.Length > 8)
                return false;

            return pin.All(char.IsDigit);
        }
    }
}