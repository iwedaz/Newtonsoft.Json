using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Tests.Converters;
using Newtonsoft.Json.Tests.Documentation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;

namespace Newtonsoft.Json.Tests.Serialization.CoerceHandler
{
    internal sealed record TopLevelModel
    {
        public TopLevelModel() {
            byte[] array = { 72, 101, 108, 108, 111, 32, 87, 111, 114, 108, 100, 33 };
            CollectionOfArrBytes = new ReadOnlyCollection<byte[]>(new List<byte[]>() { array, array });
        }

        [JsonCoerce(typeof(EncryptCoercer))]
        public int IntProp { get; set; } = 10;
        [JsonCoerce(typeof(EncryptCoercer))]
        public string StrProp { get; set; } = "top_str_0";
        [JsonCoerce(typeof(EncryptCoercer))]
        public IList<string> Messages { get; set; } = new List<string>() { "message_1", "message_2"};
        [JsonCoerce(typeof(EncryptCoercer))]
        public IReadOnlyCollection<byte[]> CollectionOfArrBytes { get; set; }
        [JsonCoerce(typeof(EncryptCoercer))]
        public CoerceTestEnum CoerceTestEnum { get; set; } = CoerceTestEnum.Test2;
        [JsonCoerce(typeof(EncryptCoercer))]
        public DateTime HistoryDate { get; set; } = DateTime.Now;
        [JsonCoerce(typeof(EncryptCoercer))]
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        [JsonCoerce(typeof(EncryptCoercer))]
        public TimeSpan LastModified { get; set; } = DateTime.Now.AddHours(1) - DateTime.UtcNow;
        [JsonCoerce(typeof(EncryptCoercer))]
        public byte[] ArrayBytes = { 72, 101, 108, 108, 111, 32, 87, 111, 114, 108, 100, 33 };
        [JsonCoerce(typeof(EncryptCoercer))]
        public string[] ArrayMessages = { "message_1", "message_2", "message_3" };
        [JsonCoerce(typeof(EncryptCoercer))]
        public DateTime[] ArrayDates = { DateTime.Now, DateTime.UtcNow, DateTime.Now.AddHours(1) };
        [JsonCoerce(typeof(EncryptCoercer))]
        public MidLevelModel MidLevelModel { get; set; } = new();
        [JsonCoerce(typeof(EncryptCoercer))]
        [JsonConverter(typeof(DateIntConverter))]
        public object DateWithConverter { get; set; } = new DateTime(2000, 12, 12, 20, 10, 0, DateTimeKind.Utc);
        [JsonCoerce(typeof(EncryptCoercer))]
        public string Secret { get; set; } = "top_secret_user_text";
        public bool IsEncrypted { get; set; } = true;
    }

    internal sealed record MidLevelModel
    {
        public int IntProp { get; set; } = 11;
        public string StrProp0 { get; set; } = "mid_str_0";

        [JsonCoerce(typeof(EncryptCoercer))]
        public LowLevelModel LowLevelModel { get; set; } = new();
        public double DoubleProp { get; set; } = 0.9;
        public string StrProp1 { get; set; } = "mid_str_1";
        public bool BoolProp { get; set; } = true;
        [JsonCoerce(typeof(EncryptCoercer))]
        public string Secret { get; set; } = "mid_secret_user_text";
        public string StrProp2 { get; set; } = "mid_str_2";
    }

    internal sealed record LowLevelModel
    {
        public int IntProp { get; set; } = 12;
        public string StrProp0 { get; set; } = "low_str_0";

        [JsonCoerce(typeof(EncryptCoercer))]
        public string Secret { get; set; } = "low_secret_user_text";
        public bool BoolProp { get; set; } = true;
        public string StrProp1 { get; set; } = "low_str_1";
    }

    enum CoerceTestEnum {
        Test1 = 0,
        Test2 = 1
    }
}
