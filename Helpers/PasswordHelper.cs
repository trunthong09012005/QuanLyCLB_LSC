using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyCLB_LSC.Helpers
{
    public static class PasswordHelper
    {
        /// <summary>
        /// Hash mật khẩu bằng SHA256 (hex lowercase)
        /// </summary>
        public static string HashPassword(string password)
        {
            if (password == null) return string.Empty;
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        /// <summary>
        /// Verify mật khẩu bằng cách so sánh SHA256 hash
        /// </summary>
        public static bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
                return false;

            var hashOfInput = HashPassword(password);
            return string.Equals(hashOfInput, hash, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Hash bằng SHA256 (alias)
        /// </summary>
        public static string HashSha256(string password) => HashPassword(password);

        /// <summary>
        /// Kiểm tra độ mạnh của mật khẩu
        /// </summary>
        public static bool IsStrongPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasUpper && hasLower && hasDigit && hasSpecialChar;
        }

        /// <summary>
        /// Tạo mật khẩu ngẫu nhiên
        /// </summary>
        public static string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
            using (var rng = RandomNumberGenerator.Create())
            {
                var tokenBuffer = new byte[length];
                rng.GetBytes(tokenBuffer);
                var chars = new StringBuilder();
                foreach (byte b in tokenBuffer)
                {
                    chars.Append(validChars[b % validChars.Length]);
                }
                return chars.ToString();
            }
        }
    }
}
