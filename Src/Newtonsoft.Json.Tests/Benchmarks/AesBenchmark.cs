using BenchmarkDotNet.Attributes;

using Newtonsoft.Json.Tests.Serialization.CoerceHandler;


namespace Newtonsoft.Json.Tests.Benchmarks
{
    [MemoryDiagnoser]
    public class AesBenchmark
    {
        private const string clearBytesStr =
            "dh[984YRNFG3QIGHRLDHFJHF083IJa" +
            "h9ay09UJ4KJG3Q;U9UYOU42QJRKDFAJPJLDKD@#$%^&*()" +
            "CVBNM<>:LKJHGTYIOP{_)(*&^%$EWSXCVBNMKIOP{}+_)(*^" +
            "%#@!QASXCDEdfhjikemwmd,slkhyrfhajmfcvorehtifnjnojhr" +
            "`12345678ijher83q4featg97gnjf95rfahivgibczveahf]   jjdButfe7tf`    j`uhwg`n" +
            "MXHYE^Y$HNJDIU$y98r3ofy3mLKMDNuc898UiU98Urmra)*0UJ87Y498UMS,MNSDGPP[S[F]" +
            @"]\]\]\][\[SA\]PF]OTI]A\rotrf/c/af[p[;/cz.;r[ppojbahvveq1```\`]\];'f.cxzrlaktiy085903q957"
            +"'[]praiityifbhugryO*)(_(@*^$*&!+__(@_*$+@$()_@*YISJDKeoiwgvcwf!@#$%^&*(";

        private readonly Encryptor _es = new();

        public bool Test()
        {
            var cipherStr = _es.Encrypt(clearBytesStr);
            var clearStr = _es.Decrypt(cipherStr);
            var eq =  clearStr is clearBytesStr;
            return eq;
        }


        [Params(1, 10)]
        public int NumberOfElements;


        [Benchmark]
        public bool Benchmark1()
        {
            bool eq = true;
            for (int i = 0; i < NumberOfElements; i++)
            {
                var cipherStr = _es.Encrypt(clearBytesStr, keyGeyIterations: 1000, keyGenAlgorithm: "SHA1");
                var clearStr = _es.Decrypt(cipherStr, keyGeyIterations: 1000, keyGenAlgorithm: "SHA1");
                eq &=  clearStr is clearBytesStr;
            }

            return eq;
        }

        [Benchmark]
        public bool Benchmark2()
        {
            bool eq = true;
            for (int i = 0; i < NumberOfElements; i++)
            {
                var cipherStr = _es.Encrypt(clearBytesStr);
                var clearStr = _es.Decrypt(cipherStr);
                eq &=  clearStr is clearBytesStr;
            }

            return eq;
        }
    }
}
