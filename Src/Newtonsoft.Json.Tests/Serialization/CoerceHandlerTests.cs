using Newtonsoft.Json.Tests.Serialization.CoerceHandler;

using Xunit;


namespace Newtonsoft.Json.Tests.Serialization
{
    public class CoerceHandlerTests
    {
        [Fact]
        public void Serialization_EncryptCoerceHandler_Success()
        {
            var model = new TopLevelModel(15, new byte[]{111, 114, 108, 100, 33 }) { IsEncrypted = true };
            string json = JsonConvert.SerializeObject(model, Formatting.Indented);
            var restoredModel = JsonConvert.DeserializeObject<TopLevelModel>(json);
            Assert.Equal(model.StrProp, restoredModel.StrProp);
            Assert.Equal(model.CoerceTestEnum, restoredModel.CoerceTestEnum);
            Assert.Equal(model.ArrayBytes, restoredModel.ArrayBytes);
            Assert.Equal(model.ArrayDates, restoredModel.ArrayDates);
            Assert.Equal(model.ArrayMessages, restoredModel.ArrayMessages);
            Assert.Equal(model.CollectionOfArrBytes, restoredModel.CollectionOfArrBytes);
            Assert.Equal(model.CreateDate, restoredModel.CreateDate);
            Assert.Equal(model.HistoryDate, restoredModel.HistoryDate);
            Assert.Equal(model.IntProp, restoredModel.IntProp);
            Assert.Equal(model.LastModified, restoredModel.LastModified);
            Assert.Equal(model.Messages, restoredModel.Messages);
            Assert.Equal(model.MidLevelModel, restoredModel.MidLevelModel);
            Assert.Equal(model.Secret, restoredModel.Secret);
            Assert.Equal(model.DateWithConverter, restoredModel.DateWithConverter);
        }

        [Fact]
        public void Serialization_EncryptCoerceHandler1_Success()
        {
            var models = new [] {
                new TopLevelModel(15, new byte[] { 111, 114, 108, 100, 33 }) { IsEncrypted = false, StrProp = "elem_2" },
                new TopLevelModel(15, new byte[] { 111, 114, 108, 100, 33 }) { IsEncrypted = true, StrProp = "elem_2" },
            };
            
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            
            var json = JsonConvert.SerializeObject(models, Formatting.Indented, settings);
            var restoredModel = JsonConvert.DeserializeObject<System.Collections.Generic.List<TopLevelModel>>(json, settings);
            
            Assert.Equal(restoredModel[0], restoredModel[1] with { IsEncrypted = false });
            Assert.Equal(models[0].StrProp, restoredModel[0].StrProp);
            Assert.Equal(models[0].CoerceTestEnum, restoredModel[0].CoerceTestEnum);
            Assert.Equal(models[0].ArrayBytes, restoredModel[0].ArrayBytes);
            Assert.Equal(models[0].ArrayDates, restoredModel[0].ArrayDates);
            Assert.Equal(models[0].ArrayMessages, restoredModel[0].ArrayMessages);
            Assert.Equal(models[0].CollectionOfArrBytes, restoredModel[0].CollectionOfArrBytes);
            Assert.Equal(models[0].CreateDate, restoredModel[0].CreateDate);
            Assert.Equal(models[0].HistoryDate, restoredModel[0].HistoryDate);
            Assert.Equal(models[0].IntProp, restoredModel[0].IntProp);
            Assert.Equal(models[0].LastModified, restoredModel[0].LastModified);
            Assert.Equal(models[0].Messages, restoredModel[0].Messages);
            Assert.Equal(models[0].MidLevelModel, restoredModel[0].MidLevelModel);
            Assert.Equal(models[0].Secret, restoredModel[0].Secret);
            Assert.Equal(models[0].DateWithConverter, restoredModel[0].DateWithConverter);
            Assert.Equal(models[1].StrProp, restoredModel[1].StrProp);
            Assert.Equal(models[1].CoerceTestEnum, restoredModel[1].CoerceTestEnum);
            Assert.Equal(models[1].ArrayBytes, restoredModel[1].ArrayBytes);
            Assert.Equal(models[1].ArrayDates, restoredModel[1].ArrayDates);
            Assert.Equal(models[1].ArrayMessages, restoredModel[1].ArrayMessages);
            Assert.Equal(models[1].CollectionOfArrBytes, restoredModel[1].CollectionOfArrBytes);
            Assert.Equal(models[1].CreateDate, restoredModel[1].CreateDate);
            Assert.Equal(models[1].HistoryDate, restoredModel[1].HistoryDate);
            Assert.Equal(models[1].IntProp, restoredModel[1].IntProp);
            Assert.Equal(models[1].LastModified, restoredModel[1].LastModified);
            Assert.Equal(models[1].Messages, restoredModel[1].Messages);
            Assert.Equal(models[1].MidLevelModel, restoredModel[1].MidLevelModel);
            Assert.Equal(models[1].Secret, restoredModel[1].Secret);
            Assert.Equal(models[1].DateWithConverter, restoredModel[1].DateWithConverter);
        }

        [Fact]
        public void Serialization_EncryptCoerceHandler2_Success()
        {
            var models = new [] {
                new MidLevelModel() { StrProp0 = "elem_1" },
                new MidLevelModel() { StrProp0 = "elem_2" },
                new MidLevelModel() { StrProp0 = "elem_3" },
                new MidLevelModel() { StrProp0 = "elem_4" },
            };
            string json = JsonConvert.SerializeObject(models, Formatting.Indented);
            var restoredModel = JsonConvert.DeserializeObject<System.Collections.Generic.List<MidLevelModel>>(json);
            Assert.Equal(models[0].StrProp0, restoredModel[0].StrProp0);
            Assert.Equal(models[0].LowLevelModel, restoredModel[0].LowLevelModel);
            Assert.Equal(models[0].Secret, restoredModel[0].Secret);
            Assert.Equal(models[0].IntProp, restoredModel[0].IntProp);
            Assert.Equal(models[0].BoolProp, restoredModel[0].BoolProp);
            Assert.Equal(models[0].DoubleProp, restoredModel[0].DoubleProp);
            Assert.Equal(models[0].StrProp1, restoredModel[0].StrProp1);
            Assert.Equal(models[0].StrProp2, restoredModel[0].StrProp2);
        }

        [Fact]
        public void Serialization_EncryptCoerceHandler3_Success()
        {

            string json0 = "{\"Secret\":\"thin_secret_user_text\"}";
            var restoredModel0 = JsonConvert.DeserializeObject<ThinModelStr>(json0);
            Assert.Equal("thin_secret_user_text", restoredModel0.Secret);

            string json1 = JsonConvert.SerializeObject(new ThinModelStr { Secret = null } , Formatting.None);
            var restoredModel1 = JsonConvert.DeserializeObject<ThinModelStr>(json1);
            Assert.Null(restoredModel1.Secret);

            string json2 = JsonConvert.SerializeObject(new ThinModelInt(), Formatting.None);
            var restoredModel2 = JsonConvert.DeserializeObject<ThinModelInt>(json2);
            Assert.Equal(15, restoredModel2.Secret);
        }
    }
}
