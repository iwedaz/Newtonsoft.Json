using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace Newtonsoft.Json.Tests.Serialization.CoerceHandler
{
    internal class Encryptor : IDisposable
    {
        private const string userKeyStr = "user_key_123";
        private const string userSaltStr = "user_salt_456";
        private readonly Aes _encryptor;

        public Encryptor()
        {
            var userKey = Encoding.UTF8.GetBytes(userKeyStr);
            var userSalt = Encoding.UTF8.GetBytes(userSaltStr);
            var pdb = new Rfc2898DeriveBytes(userKey, userSalt, 10240, HashAlgorithmName.SHA512);
            _encryptor = Aes.Create();
            _encryptor.Key = pdb.GetBytes(32); // 256 bit key
            _encryptor.IV = pdb.GetBytes(16);  // 128 bit IV
            _encryptor.Padding = PaddingMode.PKCS7;
        }

        public string Encrypt(string clearText)
        {
            var clearBytes = Encoding.UTF8.GetBytes(clearText);
            byte[] cipherBytes;

            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, _encryptor.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(clearBytes, 0, clearBytes.Length);
                cs.FlushFinalBlock();
                cs.Close();

                cipherBytes = ms.ToArray();
            }

            var cipherText = Convert.ToBase64String(cipherBytes);

            return cipherText;
        }

        public string Decrypt(string cipherText)
        {
            var cipherBytes = Convert.FromBase64String(cipherText);

            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, _encryptor.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(cipherBytes, 0, cipherBytes.Length);
                cs.FlushFinalBlock();
                cs.Close();

                cipherBytes = ms.ToArray();
            }

            var clearText = Encoding.UTF8.GetString(cipherBytes);
            return clearText;
        }

        public void Dispose()
        {
            _encryptor.Dispose();
        }
    }
}
