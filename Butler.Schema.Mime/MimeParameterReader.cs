using System.Linq;

namespace Butler.Schema.Data.Mime {

    public struct MimeParameterReader {

        internal MimeParameterReader(MimeReader reader) {
            this.reader = reader;
        }

        public string Name {
            get {
                this.AssertGood(true);
                return reader.ReadParameterName();
            }
        }

        public string Value {
            get {
                DecodingResults decodingResults;
                string str;
                if (!this.TryGetValue(reader.HeaderDecodingOptions, out decodingResults, out str))
                    MimeCommon.ThrowDecodingFailedException(ref decodingResults);
                return str;
            }
        }

        public bool ReadNextParameter() {
            this.AssertGood(false);
            return reader.ReadNextDescendant(true);
        }

        public bool TryGetValue(out string value) {
            DecodingResults decodingResults;
            return this.TryGetValue(reader.HeaderDecodingOptions, out decodingResults, out value);
        }

        public bool TryGetValue(DecodingOptions decodingOptions, out DecodingResults decodingResults, out string value) {
            this.AssertGood(true);
            return reader.TryReadParameterValue(decodingOptions, out decodingResults, out value);
        }

        private void AssertGood(bool checkPositionedOnParameter) {
            if (reader == null)
                throw new System.NotSupportedException(Resources.Strings.ParameterReaderNotInitialized);
            reader.AssertGoodToUse(true, true);
            if (reader.ReaderState != MimeReaderState.HeaderComplete || reader.CurrentHeaderObject == null || !(reader.CurrentHeaderObject is ComplexHeader))
                throw new System.NotSupportedException(Resources.Strings.ReaderIsNotPositionedOnHeaderWithParameters);
            if (checkPositionedOnParameter && !reader.IsCurrentChildValid(true))
                throw new System.InvalidOperationException(Resources.Strings.ParameterReaderIsNotPositionedOnParameter);
        }

        private readonly MimeReader reader;

    }

}