using System;
using System.Security.Cryptography;
using System.Text;

namespace Progetto.Infrastructure
{
    public static class PasswordHasher
    {
        /// <summary>
        /// Genera l'hash SHA-256 di una password
        /// </summary>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException(nameof(password));
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        /// <summary>
        /// Verifica se una password corrisponde all'hash
        /// </summary>
        public static bool VerifyPassword(string password, string hash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
            {
                return false;
            }

            string passwordHash = HashPassword(password);
            return passwordHash == hash;
        }
    }
}
