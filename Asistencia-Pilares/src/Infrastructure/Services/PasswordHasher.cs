using System;
using System.Security.Cryptography;
using AsistenciaAPI.Application.Common.Interfaces;

namespace AsistenciaAPI.Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        // PBKDF2 parameters
        private const int Iterations = 100_000;
        private const int SaltSize = 16; // bytes
        private const int KeySize = 32; // bytes

        public string Hash(string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            var salt = new byte[SaltSize];
            RandomNumberGenerator.Fill(salt);

            using var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            var derived = deriveBytes.GetBytes(KeySize);

            // store as: iterations.saltBase64.hashBase64
            return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(derived)}";
        }

        public bool Verify(string hashedPassword, string providedPassword)
        {
            if (hashedPassword is null) return false;
            if (providedPassword is null) return false;

            var parts = hashedPassword.Split('.', 3);
            if (parts.Length != 3) return false;

            if (!int.TryParse(parts[0], out var iterations)) return false;
            var salt = Convert.FromBase64String(parts[1]);
            var expected = Convert.FromBase64String(parts[2]);

            using var deriveBytes = new Rfc2898DeriveBytes(providedPassword, salt, iterations, HashAlgorithmName.SHA256);
            var derived = deriveBytes.GetBytes(expected.Length);

            return CryptographicOperations.FixedTimeEquals(derived, expected);
        }
    }
}
