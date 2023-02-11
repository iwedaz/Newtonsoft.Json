using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Newtonsoft.Json.Tests.Converters;


namespace Newtonsoft.Json.Tests.Serialization.CoerceHandler
{
    public sealed record TopLevelModel
    {
        public bool IsEncrypted = false;

        public TopLevelModel(int intProp, byte[] bytes)
        {
            IntProp = intProp;
            ArrayBytes = bytes;
            byte[] array = { 72, 101, 108, 108, 111, 32, 87, 111, 114, 108, 100, 33 };
            CollectionOfArrBytes = new ReadOnlyCollection<byte[]>(new List<byte[]>() { array, array });
        }

        [JsonCoerce(typeof(EncryptCoercer))]
        public object IntProp { get; set; } = 10;

        [JsonCoerce(typeof(EncryptCoercer))]
        public object StrProp { get; set; } = "top_str_0";

        [JsonCoerce(typeof(EncryptCoercer))]
        public object Messages { get; set; } = new List<string>() { "message_1", "message_2"};

        [JsonCoerce(typeof(EncryptCoercer))]
        public object CollectionOfArrBytes { get; set; }

        [JsonCoerce(typeof(EncryptCoercer))]
        public object CoerceTestEnum { get; set; } = CoerceTestEnumType.Test2;

        [JsonCoerce(typeof(EncryptCoercer))]
        public object HistoryDate { get; set; } = DateTime.Now;

        [JsonCoerce(typeof(EncryptCoercer))]
        public object CreateDate { get; set; } = DateTime.UtcNow;

        [JsonCoerce(typeof(EncryptCoercer))]
        public object LastModified { get; set; } = DateTime.Now.AddHours(1) - DateTime.UtcNow;

        [JsonCoerce(typeof(EncryptCoercer))]
        public object ArrayBytes = new byte[] { 72, 101, 108, 108, 111, 32, 87, 111, 114, 108, 100, 33 };

        [JsonCoerce(typeof(EncryptCoercer))]
        public object ArrayMessages = new string[] { "message_1", "message_2", "message_3" };

        [JsonCoerce(typeof(EncryptCoercer))]
        public object ArrayDates = new DateTime[] { DateTime.Now, DateTime.UtcNow, DateTime.Now.AddHours(1) };

        [JsonCoerce(typeof(EncryptCoercer))]
        public object MidLevelModel { get; set; } = new MidLevelModel();

        [JsonCoerce(typeof(EncryptCoercer))]
        [JsonConverter(typeof(DateIntConverter))]
        public object DateWithConverter { get; set; } = new DateTime(2000, 12, 12, 20, 10, 0, DateTimeKind.Utc);

        [JsonCoerce(typeof(EncryptCoercer))]
        public object Secret { get; set; } = "top_secret_user_text";
    }

    [JsonConverter(typeof(MetadataValueJsonConverter))]
    public sealed record MidLevelModel
    {
        public object IntProp { get; set; } = 11;
        public object StrProp0 { get; set; } = "mid_str_0";

        [JsonCoerce(typeof(EncryptCoercer))]
        public object LowLevelModel { get; set; } = new LowLevelModel(45, "low_str");
        public object DoubleProp { get; set; } = 0.9;
        public object StrProp1 { get; set; } = "mid_str_1";
        public object BoolProp { get; set; } = true;
        [JsonCoerce(typeof(EncryptCoercer))]
        public object Secret { get; set; } = "mid_secret_user_text";
        public object StrProp2 { get; set; } = "mid_str_2";
    }

    public sealed record LowLevelModel
    {
        public LowLevelModel(int i, string g)
        {
            IntProp = i;
            StrProp0 = g;
        }

        public object IntProp { get; set; } = 12;
        public object StrProp0 { get; set; } = "low_str_0";

        [JsonCoerce(typeof(EncryptCoercer))]
        public object Secret { get; set; } = "low_secret_user_text";
        public object BoolProp { get; set; } = true;
        public object StrProp1 { get; set; } = "low_str_1";
    }


    public sealed record ThinModelStr
    {
        [JsonCoerce(typeof(EncryptCoercer))]
        public string Secret { get; set; } = "thin_secret_user_text";
    }

    public sealed record ThinModelInt
    {
        [JsonCoerce(typeof(EncryptCoercer))]
        public int Secret { get; set; } = 15;
    }


    enum CoerceTestEnumType {
        Test1 = 0,
        Test2 = 1
    }
}
