using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json.Serialization;


namespace Newtonsoft.Json
{
    /// <summary>
    /// Allow coercion for json property value after serialization and/or before deserialization.
    /// Pay attentions! Only works for class/struct/record fields or properties. Check <see cref="JsonCoerceAttribute"/>
    ///
    /// Coercion "after serialization" replace raw json for valid json string (it ignore original serialization type).
    ///
    /// Therefore, Coercion "before deserialization" works only for valid json string
    /// and must be based on required deserialization type.
    /// If coercion "before deserialization" get not valid json string,
    /// it will skip it as well as in case <see cref="UseBeforeRead"/> equal false.
    /// </summary>
    public abstract class JsonCoerceHandler
    {
        #region Before deserialization
        
        /// <summary>
        /// Use coercion before deserialization.
        /// Call <see cref="CoerceBeforeReadHandler"/> if true.
        /// Default value is false
        /// </summary>
        public virtual bool UseBeforeRead => false;

        /// <summary>
        /// Coerce json property value before deserialization.
        /// Only works if value is valid json string.
        /// Don't forget overwrite <see cref="UseBeforeRead"/>
        /// </summary>
        /// <param name="jsonString">Valid json string or null</param>
        /// <param name="deserializedObjectMemberProperty">Property of deserializable object's member</param>
        /// <param name="deserializedObjectProperty">
        /// Property of deserializable object.
        /// Different with <paramref name="deserializedObjectMemberProperty"/>
        /// in case serializer is resolving constructor parameters
        /// </param>
        /// <param name="serializer">Current deserializer</param>
        /// <param name="deserializationStack">
        /// Stack of deserializable objects, when populating them from json.
        /// In case of creation object by constructor with parameters, stack also contains <see cref="Type"/> of created object
        /// and all deserialized members as <see cref="ValueTuple{T1,T2}"/> (string? PropertyName, object? Value).
        /// That members are deserialized in advance in order to fill constructor parameters.
        /// Based on that information you can build logic of coercion
        /// </param>
        /// <returns> <b>IsCoerced</b> must be false if coercion wasn't applied. </returns>
        /// <returns><b>JsonString</b>, coerced value, must be valid json string, valid raw json or null</returns>
        protected abstract (bool IsCoerced, string? JsonString) CoerceBeforeReadHandler(string jsonString,
            JsonProperty? deserializedObjectMemberProperty,
            JsonProperty? deserializedObjectProperty,
            JsonSerializer serializer,
            IReadOnlyCollection<object> deserializationStack);

        
        internal JsonReader CoerceBeforeRead(
            JsonReader reader,
            JsonProperty? deserializedObjectMemberProperty,
            JsonProperty? deserializedObjectProperty,
            bool hasConverter,
            JsonSerializer serializer,
            IReadOnlyCollection<object> deserializationStack)
        {
            if (!UseBeforeRead)
            {
                return reader;
            }

            // skip stream start and json comments
            reader.MoveToContent();

            // we expect only json string
            if (reader.TokenType is not JsonToken.String)
            {
                return reader;
            }

            var jsonString = (string) reader.Value!;

            var coercingResult = CoerceBeforeReadHandler(
                jsonString, deserializedObjectMemberProperty, deserializedObjectProperty, serializer, deserializationStack);

            var validJsonString = jsonString;

            if (coercingResult.IsCoerced)
            {
                validJsonString = coercingResult.JsonString ?? JsonConvert.Null;
            }

            var contract = deserializedObjectProperty?.PropertyContract ?? deserializedObjectMemberProperty?.PropertyContract;
            var deserializedObjectType = deserializedObjectProperty?.PropertyType ?? deserializedObjectMemberProperty?.PropertyType;
            
            if (// convert to valid json string
                // if string
                (contract?.ContractType is JsonContractType.String || deserializedObjectType == typeof(string))
                ||
                // if primitive type that doesn't have json equivalent
                ((contract?.ContractType is JsonContractType.Primitive
                    && contract.InternalReadType is (
                        ReadType.ReadAsBytes
                        or ReadType.ReadAsString
                        or ReadType.ReadAsDateTime
                        or ReadType.ReadAsDateTimeOffset)
                    )
                    || (deserializedObjectType == typeof(byte[])
                        || deserializedObjectType == typeof(string)
                        || deserializedObjectType == typeof(DateTime)
                        || deserializedObjectType == typeof(DateTimeOffset)
                        || deserializedObjectType == typeof(TimeSpan)
                    )
                )
                ||
                // if deserializable type is System.Object
                (!coercingResult.IsCoerced
                    && (contract?.ContractType is JsonContractType.Object
                        && deserializedObjectType == typeof(object))
                )
            )
            {
                validJsonString = ToValidJsonString(coercingResult.JsonString);
            }
                
            var textReader = new StringReader(validJsonString);
            var jsonTextReader = new JsonTextReader(textReader);
            jsonTextReader.ReadForType(contract, hasConverter);

            var chainer = new JsonReaderChainer(jsonTextReader, reader);
            return chainer;
        }
        
        #endregion


        #region After serialization
        
        /// <summary>
        /// Use coercion after serialization.
        /// Call <see cref="CoerceAfterWriteHandler"/> if true.
        /// Default value is false
        /// </summary>
        public virtual bool UseAfterWrite => false;
        
        
        /// <summary>
        /// Coerce json property value after serialization.
        /// Replace raw json value with valid json string.
        /// Don't forget overwrite <see cref="UseAfterWrite"/>
        /// </summary>
        /// <param name="rawJson">Serialized property value as raw json</param>
        /// <param name="serializedObjectMemberProperty">Property of serializable object's member</param>
        /// <param name="serializer">Current serializer</param>
        /// <param name="serializationStack">Stack of serializable objects, when converting them into json</param>
        /// <returns>Must be valid json string, valid raw json or null</returns>
        protected abstract (bool IsCoerced, string? JsonString) CoerceAfterWriteHandler(
            string? rawJson,
            JsonProperty serializedObjectMemberProperty,
            JsonSerializer serializer,
            IReadOnlyCollection<object> serializationStack);

        
        internal void CoerceAfterWrite(
            JsonWriter writer,
            string? rawJson,
            JsonProperty serializedObjectMemberProperty,
            JsonSerializer serializer,
            IReadOnlyCollection<object> serializationStack)
        {
            if (!UseAfterWrite)
            {
                writer.WriteRawValue(rawJson);
                return;
            }

            var coercingResult = CoerceAfterWriteHandler(
                rawJson, serializedObjectMemberProperty, serializer, serializationStack);

            var validJsonString = rawJson;

            if (coercingResult.IsCoerced)
            {
                validJsonString = ToValidJsonString(coercingResult.JsonString);
            }

            writer.WriteRawValue(validJsonString);
        }
        
        #endregion
        
        
        private static string ToValidJsonString(string? str)
        {
            if (str is null)
            {
                str = JsonConvert.Null;
            }
            else if (str[0] is not '"' || str[str.Length - 1] is not '"')
            {
                str = JsonConvert.ToString(str);
            }

            return str;
        }
    }
}
