using System.Security.Cryptography;
using System.Text;

namespace IdentityService.Application.Helpers
{
    public static class PasswordGenerator
    {
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
        public static string Generate(int length = 12)
        {
            if (length <= 0)
                throw new ArgumentException("Password length must be greater than zero.", nameof(length));

            var data = new byte[length];
            using (var crypto = RandomNumberGenerator.Create())
            {
                crypto.GetBytes(data);
            }

            var result = new StringBuilder(length);
            foreach (var b in data)
            {
                result.Append(Chars[b % Chars.Length]);
            }

            return result.ToString();
        }
    }
}
