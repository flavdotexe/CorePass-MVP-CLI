using System;
using System.Security.Cryptography;
using Konscious.Security.Cryptography;
using Sodium;
using System.Text;
using CorePass.Core.Model;

namespace CorePass.Core.Crypto;

public static class CryptoEngine
{
    public static (byte[] masterKey, byte[] salt) DeriveMasterKey(ReadOnlySpan<byte> passwordUtf8, KdfParams p, byte[]? saltOpt = null)
    {
        byte[] salt = saltOpt ?? SodiumCore.GetRandomBytes(16);
        var argon = new Argon2id(passwordUtf8.ToArray())
        {
            MemorySize = Math.Max(8 * 1024, p.MemoryMB * 1024),
            Iterations = Math.Max(2, p.Iterations),
            DegreeOfParallelism = Math.Max(1, p.Parallelism)
        };
        argon.Salt = salt;
        var mk = argon.GetBytes(32);
        return (mk, salt);
    }
    public static byte[] HkdfExpand(byte[] key, string info, int len = 32)
    {
        using var hkdf = new HMACSHA256(key);
        var okm = new byte[len];
        var t = Array.Empty<byte>();
        int pos = 0; byte counter = 1;
        while (pos < len)
        {
            hkdf.Initialize();
            hkdf.TransformBlock(t, 0, t.Length, null, 0);
            var infoBytes = Encoding.UTF8.GetBytes(info);
            hkdf.TransformBlock(infoBytes, 0, infoBytes.Length, null, 0);
            hkdf.TransformFinalBlock(new[] { counter }, 0, 1);
            t = hkdf.Hash!;
            int n = Math.Min(t.Length, len - pos);
            Buffer.BlockCopy(t, 0, okm, pos, n);
            pos += n; counter++;
        }
        CryptographicOperations.ZeroMemory(t);
        return okm;
    }
        public static (byte[] nonce, byte[] cipher) Seal(byte[] key, byte[] plaintext, byte[] aad)
    {
        byte[] nonce = SodiumCore.GetRandomBytes(24);
        var cipher = SecretAeadXChaCha20Poly1305.Encrypt(plaintext, aad, nonce, key);
        CryptographicOperations.ZeroMemory(plaintext);
        return (nonce, cipher);
    }
        public static byte[] Open(byte[] key, byte[] nonce, byte[] cipher, byte[] aad)
    {
        return SecretAeadXChaCha20Poly1305.Decrypt(cipher, aad, nonce, key);
    }
}
