using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace CorePass.Core.Storage
{
    public class VaultStore
    {
        private readonly string _filePath;

        public VaultStore(string filePath)
        {
            _filePath = filePath;
        }

        public void SaveVault(Vault vault, string password)
        {
            byte[] data = JsonSerializer.SerializeToUtf8Bytes(vault);
            byte[] encryptedData = Encrypt(data, password);
            File.WriteAllBytes(_filePath, encryptedData);
        }

        public Vault LoadVault(string password)
        {
            if (!File.Exists(_filePath))
                throw new FileNotFoundException("Vault file not found.");

            byte[] encryptedData = File.ReadAllBytes(_filePath);
            byte[] decryptedData = Decrypt(encryptedData, password);
            return JsonSerializer.Deserialize<Vault>(decryptedData) 
                   ?? throw new Exception("Failed to deserialize vault.");
        }

        private static byte[] Encrypt(byte[] data, string password)
        {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            byte[] salt = RandomNumberGenerator.GetBytes(16);

            using var keyDerivation = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            byte[] key = keyDerivation.GetBytes(32);
            byte[] iv = keyDerivation.GetBytes(16);
            aes.Key = key;
            aes.IV = iv;

            using var ms = new MemoryStream();
            ms.Write(salt, 0, salt.Length);

            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();
            }

            return ms.ToArray();
        }

        private static byte[] Decrypt(byte[] encryptedData, string password)
        {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            byte[] salt = new byte[16];
            Array.Copy(encryptedData, 0, salt, 0, 16);

            using var keyDerivation = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            byte[] key = keyDerivation.GetBytes(32);
            byte[] iv = keyDerivation.GetBytes(16);
            aes.Key = key;
            aes.IV = iv;

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(encryptedData, 16, encryptedData.Length - 16);
                cs.FlushFinalBlock();
            }

            return ms.ToArray();
        }
    }
}
