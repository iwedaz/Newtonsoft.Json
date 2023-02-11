using System;
using System.Collections.Generic;

using BenchmarkDotNet.Attributes;

using Newtonsoft.Json.Tests.Serialization.CoerceHandler;


namespace Newtonsoft.Json.Tests.Benchmarks
{
    [MemoryDiagnoser]
    public class JsonCoercerBenchmark
    {
        private readonly TopLevelModel _model;
        private readonly IReadOnlyCollection<TopLevelModel> _models;
        private readonly JsonSerializerSettings _settings;

        public JsonCoercerBenchmark()
        {
            _model = new TopLevelModel(15, new byte[] { 110, 113, 107, 99, 32 }) { IsEncrypted = true, StrProp = "elem_0" };
            _models = new [] {
                new TopLevelModel(15, new byte[] { 110, 113, 107, 99, 32 }) { IsEncrypted = true, StrProp = "elem_1" },
                new TopLevelModel(15, new byte[] { 111, 114, 108, 100, 33 }) { IsEncrypted = true, StrProp = "elem_2" },
            };

            _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
        }

        [Params(1, 10)]
        public int NumberOfElements;

        //[Benchmark]
        public IReadOnlyCollection<TopLevelModel> Test()
        {
            IReadOnlyCollection<TopLevelModel> restoredModel = Array.Empty<TopLevelModel>();
            for (int i = 0; i < NumberOfElements; i++)
            {
                var json = JsonConvert.SerializeObject(_models, Formatting.Indented, _settings);
                restoredModel = JsonConvert.DeserializeObject<List<TopLevelModel>>(json, _settings);
            }
            return restoredModel;
        }

        [Benchmark]
        public TopLevelModel Test1()
        {
            TopLevelModel restoredModel = null;
            for (int i = 0; i < NumberOfElements; i++)
            {
                var json = JsonConvert.SerializeObject(_model, Formatting.Indented, _settings);
                restoredModel = JsonConvert.DeserializeObject<TopLevelModel>(json, _settings);
            }
            return restoredModel;
        }
    }
}
