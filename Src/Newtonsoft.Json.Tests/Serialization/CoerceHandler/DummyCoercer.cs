using System;
using System.Collections.Generic;

using Newtonsoft.Json.Serialization;


namespace Newtonsoft.Json.Tests.Serialization.CoerceHandler
{
    internal sealed class DummyCoercer : JsonCoerceHandler
    {
        public override bool UseAfterWrite => false;
        protected override (bool IsCoerced, string JsonString) CoerceBeforeReadHandler(string jsonString, JsonProperty deserializedObjectMemberProperty, JsonProperty deserializedObjectProperty,
            JsonSerializer serializer, IReadOnlyCollection<object> deserializationStack) =>
            throw new NotImplementedException();

        protected override (bool IsCoerced, string JsonString) CoerceAfterWriteHandler(
            string rawJson, JsonProperty serializedObjectMemberProperty,
            JsonSerializer serializer, IReadOnlyCollection<object> serializationStack) =>
            throw new NotImplementedException();

        public override bool UseBeforeRead => false;

        
    }
}
