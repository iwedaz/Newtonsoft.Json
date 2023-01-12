using System;



namespace Newtonsoft.Json.Tests.Serialization.CoerceHandler
{
    internal sealed class MetadataValueJsonConverter : JsonConverter<MidLevelModel>
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, MidLevelModel value, JsonSerializer serializer) =>
            throw new NotSupportedException();

        public override MidLevelModel ReadJson(
            JsonReader reader,
            Type objectType,
            MidLevelModel existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var obj = Newtonsoft.Json.Linq.JObject.Load(reader);

            var result = new MidLevelModel();
            serializer.Populate(obj.CreateReader(), result);
            return result;
        }
    }
}
