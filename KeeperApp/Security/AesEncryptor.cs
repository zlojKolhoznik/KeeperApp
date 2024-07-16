using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KeeperApp.Security
{
    public static class AesEncryptor
    {
        private static string key;

        public static void ConfigureKey(string keySource)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(keySource, nameof(keySource));

            using var hashAlg = SHA256.Create();
            byte[] sourceBytes = Encoding.UTF8.GetBytes(keySource);
            byte[] hash = hashAlg.ComputeHash(sourceBytes);
            key = Convert.ToBase64String(hash);
        }

        public static void UnconfigureKey()
        {
            key = string.Empty;
        }

        public static bool IsKeyConfigured() => !string.IsNullOrEmpty(key);

        public static string Encrypt(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
            if (!IsKeyConfigured())
            {
                throw new ArgumentNullException(nameof(key), "Encryption key cannot be empty");
            }
            using var aesAlg = Aes.Create();
            string iv = Convert.ToBase64String(aesAlg.IV);
            aesAlg.Key = Convert.FromBase64String(key);
            using var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            byte[] valueBytes = Encoding.UTF8.GetBytes(value);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(valueBytes, 0, valueBytes.Length);
            string encryptedValue = Convert.ToBase64String(encryptedBytes);
            return $"{iv}.{encryptedValue}";
        }

        public static string Decrypt(string value) 
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
            if (!IsKeyConfigured())
            {
                throw new ArgumentNullException(nameof(key), "Encryption key cannot be empty");
            }
            string ivString = value.Split('.')[0];
            string encryptedData = value.Split('.')[1];
            using var aesAlg = Aes.Create();
            aesAlg.Key = Convert.FromBase64String(key);
            aesAlg.IV = Convert.FromBase64String(ivString);
            using var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
            byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            string decryptedValue = Encoding.UTF8.GetString(decryptedBytes);
            return decryptedValue;
        }
    }
}
