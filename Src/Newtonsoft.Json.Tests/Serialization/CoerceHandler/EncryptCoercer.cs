using System;
using System.Collections.Generic;
using System.Linq;



namespace Newtonsoft.Json.Tests.Serialization.CoerceHandler
{
    public sealed class EncryptCoercer : JsonCoerceHandler, IDisposable
    {
        private readonly Encryptor _encryptor = new Encryptor();

        public override bool UseBeforeRead => true;
        public override bool UseAfterWrite => true;


        protected override string CoerceBeforeReadHandler(
            string jsonString, Type propertyValueType,
            JsonSerializer serializer,
            IReadOnlyCollection<object> deserializationStack)
        {
            if (String.IsNullOrEmpty(jsonString))
            {
                return jsonString;
            }
            
            var topModel = deserializationStack.Reverse().FirstOrDefault(x => x is TopLevelModel);
            if (topModel is TopLevelModel topLevelModel && topLevelModel.IsEncrypted)
            {
                var clearJson = _encryptor.Decrypt(jsonString);
                return clearJson;
            }

            return jsonString;
        }

        protected override string CoerceAfterWriteHandler(
            string rawJson,
            JsonSerializer serializer,
            IReadOnlyCollection<object> serializationStack)
        {
            if (String.IsNullOrEmpty(rawJson))
            {
                return rawJson;
            }
            
            var topModel = serializationStack.Reverse().FirstOrDefault(x => x is TopLevelModel);
            if (topModel is TopLevelModel topLevelModel && topLevelModel.IsEncrypted)
            {
                var cipherText = _encryptor.Encrypt(rawJson);
                return cipherText;
            }

            return rawJson;
        }

        public void Dispose()
        {
            _encryptor?.Dispose();
        }
    }
}
