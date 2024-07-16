using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.Security
{
    public static class Sha256Hasher
    {
        public static string GetSaltedHash(string value, string salt)
        {
            var toBeHashed = value + salt;
            return GetHash(toBeHashed);
        }

        public static string GetHash(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            using var sha256Alg = SHA256.Create();
            byte[] hashedBytes = sha256Alg.ComputeHash(bytes);
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
