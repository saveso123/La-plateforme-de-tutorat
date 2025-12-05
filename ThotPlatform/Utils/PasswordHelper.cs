using System;
using System.Security.Cryptography;
using System.Text;

namespace ThotPlatform.Utils
{
    /// <summary>
    /// Classe utilitaire pour le hachage et la verification des mots de passe
    /// </summary>
    public static class PasswordHelper
    {
        /// <summary>
        /// Hache un mot de passe en utilisant SHA256
        /// </summary>
        /// <param name="password">Mot de passe en clair</param>
        /// <returns>Mot de passe hache</returns>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Le mot de passe ne peut pas etre vide", nameof(password));

            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        /// <summary>
        /// Verifie si un mot de passe correspond a son hash
        /// </summary>
        /// <param name="password">Mot de passe en clair</param>
        /// <param name="hashedPassword">Mot de passe hache</param>
        /// <returns>True si le mot de passe correspond</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
                return false;

            var hashOfInput = HashPassword(password);
            return hashOfInput.Equals(hashedPassword);
        }

        /// <summary>
        /// Genere un mot de passe aleatoire
        /// </summary>
        /// <param name="length">Longueur du mot de passe</param>
        /// <returns>Mot de passe aleatoire</returns>
        public static string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
            var random = new Random();
            var password = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                password.Append(validChars[random.Next(validChars.Length)]);
            }

            return password.ToString();
        }
    }
}

