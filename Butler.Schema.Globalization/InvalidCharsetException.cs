namespace Butler.Schema.Globalization {

    [System.Serializable]
    public class InvalidCharsetException : ButlerSchemaException {

        public InvalidCharsetException(int codePage)
            : base(Resources.GlobalizationStrings.InvalidCodePage(codePage)) {
            this.CodePage = codePage;
        }

        public InvalidCharsetException(string charsetName)
            : base(Resources.GlobalizationStrings.InvalidCharset(charsetName ?? "<null>")) {
            this.CharsetName = charsetName;
        }

        public InvalidCharsetException(int codePage, string message)
            : base(message) {
            this.CodePage = codePage;
        }

        public InvalidCharsetException(string charsetName, string message)
            : base(message) {
            this.CharsetName = charsetName;
        }

        internal InvalidCharsetException(string charsetName, int codePage, string message)
            : base(message) {
            this.CodePage = codePage;
            this.CharsetName = charsetName;
        }

        public InvalidCharsetException(int codePage, string message, System.Exception innerException)
            : base(message, innerException) {
            this.CodePage = codePage;
        }

        public InvalidCharsetException(string charsetName, string message, System.Exception innerException)
            : base(message, innerException) {
            this.CharsetName = charsetName;
        }

        protected InvalidCharsetException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) {
            this.CodePage = info.GetInt32("codePage");
            this.CharsetName = info.GetString("charsetName");
        }

        public int CodePage { get; }
        public string CharsetName { get; }

    }

}