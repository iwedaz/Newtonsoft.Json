using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;



namespace Newtonsoft.Json
{
    internal sealed class JsonReaderChainer : JsonReader
    {
        //  bool -- marks that reader has been exhausted
        private readonly Dictionary<JsonReader, bool> _readers = new();
        private JsonReader _currentReader;


        public JsonReaderChainer(params JsonReader[] readers)
        {
            if (readers == null)
            {
                throw new ArgumentNullException(nameof(readers));
            }

            if (readers.Length <= 1)
            {
                throw new ArgumentException("Collection must have at least 2 items", nameof(readers));
            }

            if (readers.Any(x => x is null))
            {
                throw new ArgumentException("Collection has null values", nameof(readers));
            }

            if (readers.Distinct().Count() != readers.Length)
            {
                throw new ArgumentException("Collection has duplicated JsonReader objects", nameof(readers));
            }

            foreach (var reader in readers)
            {
                _readers.Add(reader, false);
            }

            _currentReader = readers[0];
        }


        public override bool Read()
        {
            foreach (var reader in _readers)
            {
                if (!reader.Value)
                {
                    _currentReader = reader.Key;

                    var isRead = _currentReader.Read();
                    if (!isRead)
                    {
                        _readers[_currentReader] = true; // EOS, reader is exhausted
                        continue;
                    }

                    return true; // reader is read successfully
                }
            }

            return false;
        }


        #region Ovverides

        public override char QuoteChar
        {
            get => _currentReader.QuoteChar;
            protected internal set => _currentReader.QuoteChar = value;
        }
        public override JsonToken TokenType => _currentReader.TokenType;
        public override object? Value => _currentReader.Value;
        public override Type? ValueType => _currentReader.ValueType;
        public override int Depth => _currentReader.Depth;
        public override string Path => _currentReader.Path;

        public override int? ReadAsInt32() => _currentReader.ReadAsInt32();

        public override string? ReadAsString() => _currentReader.ReadAsString();

        public override byte[]? ReadAsBytes() => _currentReader.ReadAsBytes();

        public override double? ReadAsDouble() => _currentReader.ReadAsDouble();

        public override bool? ReadAsBoolean() => _currentReader.ReadAsBoolean();

        public override decimal? ReadAsDecimal() => _currentReader.ReadAsDecimal();

        public override DateTime? ReadAsDateTime() => _currentReader.ReadAsDateTime();

        public override DateTimeOffset? ReadAsDateTimeOffset() => _currentReader.ReadAsDateTimeOffset();

        protected override void Dispose(bool disposing)
        {
            foreach (var reader in _readers)
            {
                (reader.Key as IDisposable)?.Dispose();
            }
        }

        public override void Close()
        {
            foreach (var reader in _readers)
            {
                reader.Key.Close();
            }
        }

        public override Task<bool> ReadAsync(CancellationToken cancellationToken = default) =>
            _currentReader.ReadAsync(cancellationToken);

        public override Task<bool?> ReadAsBooleanAsync(CancellationToken cancellationToken = default) =>
            _currentReader.ReadAsBooleanAsync(cancellationToken);

        public override Task<byte[]?> ReadAsBytesAsync(CancellationToken cancellationToken = default) =>
            _currentReader.ReadAsBytesAsync(cancellationToken);

        public override Task<DateTime?> ReadAsDateTimeAsync(CancellationToken cancellationToken = default) =>
            _currentReader.ReadAsDateTimeAsync(cancellationToken);

        public override Task<DateTimeOffset?> ReadAsDateTimeOffsetAsync(CancellationToken cancellationToken = default) =>
            _currentReader.ReadAsDateTimeOffsetAsync(cancellationToken);

        public override Task<decimal?> ReadAsDecimalAsync(CancellationToken cancellationToken = default) =>
            _currentReader.ReadAsDecimalAsync(cancellationToken);

        public override Task<double?> ReadAsDoubleAsync(CancellationToken cancellationToken = default) =>
            _currentReader.ReadAsDoubleAsync(cancellationToken);

        public override Task<int?> ReadAsInt32Async(CancellationToken cancellationToken = default) =>
            _currentReader.ReadAsInt32Async(cancellationToken);

        public override Task<string?> ReadAsStringAsync(CancellationToken cancellationToken = default) =>
            _currentReader.ReadAsStringAsync(cancellationToken);

        #endregion
    }
}
