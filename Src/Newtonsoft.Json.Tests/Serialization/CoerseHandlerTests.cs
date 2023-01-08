using Newtonsoft.Json.Tests.Serialization.CoerceHandler;

using Xunit;


namespace Newtonsoft.Json.Tests.Serialization
{
    public class CoerceHandlerTests
    {
        [Fact]
        public void Serialization_EncryptCoerceHandler_Success()
        {
            var model = new TopLevelModel();
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
    }
}
