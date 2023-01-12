#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Serialization;


namespace Newtonsoft.Json.Tests.Serialization.CoerceHandler
{
    internal sealed class EncryptCoercer : JsonCoerceHandler, IDisposable
    {
        private readonly Encryptor _encryptor = new Encryptor();

        public override bool UseBeforeRead => true;
        public override bool UseAfterWrite => true;


        protected override (bool IsCoerced, string? JsonString) CoerceBeforeReadHandler(string jsonString,
            JsonProperty? deserializedObjectMemberProperty,
            JsonProperty? deserializedObjectProperty,
            JsonSerializer serializer,
            IReadOnlyCollection<object> deserializationStack)
        {
            if (String.IsNullOrEmpty(jsonString))
            {
                return (false, jsonString);
            }

            var topModel = deserializationStack
                .Reverse()
                .FirstOrDefault(x => x is TopLevelModel);
            if (topModel is TopLevelModel topLevelModel && topLevelModel.IsEncrypted)
            {
                var clearJson = _encryptor.Decrypt(jsonString);
                return (true, clearJson);
            }

            if ((deserializationStack.FirstOrDefault(x => x is Type) as Type) == typeof(TopLevelModel))
            {
                var parameters = deserializationStack
                    .OfType<ValueTuple<string?, object?>>()
                    .ToList().AsReadOnly();

                if (parameters.Count > 0)
                {
                    var isEncryptedProperty = parameters.FirstOrDefault(x => x.Item1 is nameof(TopLevelModel.IsEncrypted));
                    if (isEncryptedProperty != default && isEncryptedProperty.Item2 is bool value && value)
                    {
                        var clearJson = _encryptor.Decrypt(jsonString);
                        return (true, clearJson);
                    }
                }
            }

            return (false, jsonString);
        }

        protected override (bool IsCoerced, string? JsonString) CoerceAfterWriteHandler(
            string? rawJson,
            JsonProperty serializedObjectMemberProperty,
            JsonSerializer serializer,
            IReadOnlyCollection<object> serializationStack)
        {
            if (String.IsNullOrEmpty(rawJson))
            {
                return (false, rawJson);
            }

            var topModel = serializationStack.Reverse().FirstOrDefault(x => x is TopLevelModel);
            if (topModel is TopLevelModel topLevelModel && topLevelModel.IsEncrypted)
            {
                var cipherText = _encryptor.Encrypt(rawJson);
                return (true, cipherText);
            }

            return (false, rawJson);
        }

        public void Dispose()
        {
            _encryptor?.Dispose();
        }
    }
}
