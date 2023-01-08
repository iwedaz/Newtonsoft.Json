using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json.Serialization;



namespace Newtonsoft.Json
{
    /// <summary>
    /// Allow coerce json property value after serialization and before deserialization
    /// </summary>
    public abstract class JsonCoerceHandler
    {
        /// <summary>
        /// Call <see cref="CoerceBeforeReadHandler"/> if true. Default value is false
        /// </summary>
        public virtual bool UseBeforeRead => false;
        
        /// <summary>
        /// Call <see cref="CoerceAfterWriteHandler"/> if true. Default value is false
        /// </summary>
        public virtual bool UseAfterWrite => false;


        /// <summary>
        /// Coerce json property value before deserialization. Only works if value is valid json string.
        /// Don't forget overwrite <see cref="UseBeforeRead"/>
        /// </summary>
        /// <param name="jsonString">Valid json string</param>
        /// <param name="propertyValueType">Property type of deserializable object</param>
        /// <param name="serializer">Current deserializer</param>
        /// <param name="deserializationStack">Stack of deserializable objects, when populating them from json</param>
        /// <returns>Must be valid json string</returns>
        protected abstract string CoerceBeforeReadHandler(string? jsonString, Type propertyValueType, JsonSerializer serializer, IReadOnlyCollection<object> deserializationStack);
        
        /// <summary>
        /// Coerce json property value after serialization.
        /// Don't forget overwrite <see cref="UseAfterWrite"/>
        /// </summary>
        /// <param name="rawJson">Serialized property value as raw json</param>
        /// <param name="serializer">Current serializer</param>
        /// <param name="serializationStack">Stack of serializable objects, when converting them in json</param>
        /// <returns>Must be valid json string</returns>
        protected abstract string CoerceAfterWriteHandler(string? rawJson, JsonSerializer serializer, IReadOnlyCollection<object> serializationStack);


        internal JsonReader CoerceBeforeRead(
            JsonReader reader,
            Type objectType,
            JsonContract? contract,
            bool hasConverter,
            JsonSerializer serializer,
            IReadOnlyCollection<object> deserializationStack)
        {
            if (!UseBeforeRead)
            {
                return reader;
            }

            var jsonString = ReadString(reader);
            /*
            if (jsonString is null)
            {
                return reader;
            }
            */

            var coercedJsonString = CoerceBeforeReadHandler(jsonString, objectType, serializer, deserializationStack);

            var textReader = new StringReader(coercedJsonString);
            var jsonTextReader = new JsonTextReader(textReader);
            jsonTextReader.ReadForType(contract, hasConverter);

            var chainer = new JsonReaderChainer(jsonTextReader, reader);
            return chainer;
        }

        internal void CoerceAfterWrite(
            JsonWriter writer,
            string? rawJson,
            JsonSerializer serializer,
            IReadOnlyCollection<object> serializationStack)
        {
            if (!UseAfterWrite)
            {
                writer.WriteValue(rawJson);
                return;
            }

            var coercedJsonString = CoerceAfterWriteHandler(rawJson, serializer, serializationStack);
            writer.WriteValue(coercedJsonString);
        }

        private string? ReadString(JsonReader reader)
        {
            string? createdValue;

            do
            {
                switch (reader.TokenType)
                {
                    case JsonToken.String:
                    {
                        createdValue = (string)reader.Value!;
                        return createdValue;
                    }
                    case JsonToken.Null:
                    case JsonToken.Undefined:
                    {
                        createdValue = reader.Value! as string;
                        return createdValue;
                    }
                    case JsonToken.Comment:
                        // ignore
                        break;
                    default:
                        throw JsonSerializationException.Create(reader, "CoerceHandler: Unexpected token while deserializing json string: " + reader.TokenType);
                }
            } while (reader.Read());

            throw JsonSerializationException.Create(reader, "CoerceHandler: Unexpected end when deserializing object.");
        }
    }
}
