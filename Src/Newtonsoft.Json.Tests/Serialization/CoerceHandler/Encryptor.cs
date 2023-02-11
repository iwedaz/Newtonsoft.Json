using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Microsoft.Extensions.Caching.Memory;


namespace Newtonsoft.Json.Tests.Serialization.CoerceHandler
{
    public sealed class Encryptor : IDisposable
    {
        #region consts

        private const string _sk1 = "01187bbba19d92d0c43f33f576786f5747269ed531fa";
        private const string _sk2 = "40e0de024f38e07fa233056b91f5e43a2725368a43945";
        private const string _sk3 = "a985bd40e30a3b29237197ecfb1f6ce3036c6ca";
        private static readonly string _sk4 =
            _sk1.ToUpper()
                .Insert(_sk1.Length, _sk2.ToLower())
            + new string(_sk3.ToCharArray().Reverse().ToArray());

        private const string _s1 = "80a2a743dee3b5ee0c08449c5d4f518f8884996ef6e8468378a";
        private const string _s2 = "831df216f17f919054f2060bd0c81dddc49d1a1616fae0626";
        private const string _s3 = "cf1e5e267bc8c21c4230ab3b550a";
        private static readonly string _s4 =
            _s1.ToUpper()
            .Insert(_s1.Length, _s2.ToLower())
            + new string(_s3.ToCharArray().Reverse().ToArray());
        private readonly byte[] _sb = Encoding.UTF8.GetBytes(_s4);

        private const int DefaultKeyGeyIterations = 256000;
        private const string DefaultKeyGeyAlgorithm = "SHA512";

        #endregion


        public Encryptor()
        {
            _ = GetAes(_sk4, _sb, 1000, HashAlgorithmName.SHA1.Name); // legacy
            _ = GetAes(_sk4, _sb, DefaultKeyGeyIterations, DefaultKeyGeyAlgorithm);
        }

        private byte[] Encrypt(byte[] clearBytes,
            string encryptionKey = null, byte[] salt = null,
            int keyGeyIterations = DefaultKeyGeyIterations, string keyGenAlgorithm = DefaultKeyGeyAlgorithm)
        {
            var (_, Encryptor, _) = GetAes(encryptionKey, salt, keyGeyIterations, keyGenAlgorithm);

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, Encryptor, CryptoStreamMode.Write))
            {
                cs.Write(clearBytes, 0, clearBytes.Length);
            }

            var cipherBytes = ms.ToArray();
            return cipherBytes;
        }

        public string Encrypt(string clearText,
            string encryptionKey = null, byte[] salt = null,
            int keyGeyIterations = DefaultKeyGeyIterations, string keyGenAlgorithm = DefaultKeyGeyAlgorithm)
        {
            var clearBytes = Encoding.UTF8.GetBytes(clearText);
            var cipherBytes = Encrypt(clearBytes, encryptionKey, salt, keyGeyIterations, keyGenAlgorithm);
            var result = Convert.ToBase64String(cipherBytes);
            return result;
        }


        private byte[] Decrypt(byte[] cipherBytes,
            string encryptionKey = null, byte[] salt = null,
            int keyGeyIterations = DefaultKeyGeyIterations, string keyGenAlgorithm = DefaultKeyGeyAlgorithm)
        {
            var (_, _, Decryptor) = GetAes(encryptionKey, salt, keyGeyIterations, keyGenAlgorithm);

            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, Decryptor, CryptoStreamMode.Write))
            {
                cs.Write(cipherBytes, 0, cipherBytes.Length);
            }

            var clearBytes = ms.ToArray();
            return clearBytes;
        }

        public string Decrypt(string cipherText,
            string encryptionKey = null, byte[] salt = null,
            int keyGeyIterations = DefaultKeyGeyIterations, string keyGenAlgorithm = DefaultKeyGeyAlgorithm)
        {
            cipherText = cipherText.Replace(" ", "+");
            var cipherBytes = Convert.FromBase64String(cipherText);
            var clearBytes = Decrypt(cipherBytes, encryptionKey, salt, keyGeyIterations, keyGenAlgorithm);
            var result = Encoding.UTF8.GetString(clearBytes);
            return result;
        }

        public void Dispose()
        {
            _aesCache?.Dispose();
        }


        #region Aes cache

        private AesBucket GetAes(
            string userKey, byte[] userSalt,
            int keyGeyIterations, string keyGenAlgorithm)
        {
            if (keyGeyIterations <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(keyGeyIterations));
            }

            if (keyGenAlgorithm is null or not ("SHA1" or "SHA256" or "SHA384" or "SHA512"))
            {
                throw new ArgumentException("Value is null or not SHA1, SHA256, SHA384, SHA512", nameof(keyGenAlgorithm));
            }

            var keyGenAlg = keyGenAlgorithm switch
            {
                nameof(HashAlgorithmName.SHA1) => HashAlgorithmName.SHA1,
                nameof(HashAlgorithmName.SHA256) => HashAlgorithmName.SHA256,
                nameof(HashAlgorithmName.SHA384) => HashAlgorithmName.SHA384,
                nameof(HashAlgorithmName.SHA512) => HashAlgorithmName.SHA512,
                _ => HashAlgorithmName.SHA512,
            };

            if (String.IsNullOrWhiteSpace(userKey) && userSalt is null)
            {
                return GetOrCreateCache(new UserBucket(_sk4, _sb, keyGeyIterations, keyGenAlg));
            }

            var userBucket = new UserBucket(userKey, userSalt, keyGeyIterations, keyGenAlg);
            var aesBucket = GetOrCreateCache(userBucket);
            return aesBucket;
        }

        private record struct UserBucket(
            string Key,
            byte[] Salt,
            int KeyGeyIterations,
            HashAlgorithmName HashAlgorithm);


        private sealed class AesBucket : IDisposable
        {
            public Aes AesAlgorithm { get; }
            public ICryptoTransform Encryptor { get; }
            public ICryptoTransform Decryptor { get; }

            public AesBucket(Aes aesAlgorithm, ICryptoTransform encryptor, ICryptoTransform decryptor)
            {
                AesAlgorithm = aesAlgorithm;
                Encryptor = encryptor;
                Decryptor = decryptor;
            }

            public void Deconstruct(
                out Aes _AesAlgorithm,
                out ICryptoTransform _Encryptor,
                out ICryptoTransform _Decryptor)
            {
                _AesAlgorithm = AesAlgorithm;
                _Encryptor = Encryptor;
                _Decryptor = Decryptor;
            }

            public void Dispose()
            {
                AesAlgorithm?.Dispose();
                Encryptor?.Dispose();
                Decryptor?.Dispose();
            }
        }


        private readonly IMemoryCache _aesCache = new MemoryCache(
            new MemoryCacheOptions { ExpirationScanFrequency = TimeSpan.FromMinutes(10) });

        private AesBucket GetOrCreateCache(UserBucket userBucket)
        {
            if (_aesCache.TryGetValue<AesBucket>(userBucket, out var aesBucket))
            {
                return aesBucket;
            }

            var pdb = new Rfc2898DeriveBytes(
                userBucket.Key,
                userBucket.Salt,
                userBucket.KeyGeyIterations,
                userBucket.HashAlgorithm);
            var psk = pdb.GetBytes(32);
            var piv = pdb.GetBytes(16);

            var aes = Aes.Create();
            aes.Key = psk;
            aes.IV = piv;
            aes.Padding = PaddingMode.PKCS7;

            var encryptor = aes.CreateEncryptor();
            var decryptor = aes.CreateDecryptor();

            aesBucket = new AesBucket(aes, encryptor, decryptor);

            var opts = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(1))
                .RegisterPostEvictionCallback((_, value, reason, _) =>
                {
                    if (reason is EvictionReason.Replaced or EvictionReason.Expired)
                    {
                        (value as IDisposable)?.Dispose();
                    }
                });

            _aesCache.Set(userBucket, aesBucket, opts);

            return aesBucket;
        }

        #endregion
    }
}
