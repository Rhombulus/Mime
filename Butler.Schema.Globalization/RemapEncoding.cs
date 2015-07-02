namespace Butler.Schema.Data.Globalization {

    [System.Serializable]
    internal class RemapEncoding : System.Text.Encoding {

        public RemapEncoding(int codePage)
            : base(codePage) {
            if (codePage == 28591) {
                encodingEncoding = System.Text.Encoding.GetEncoding(28591);
                decodingEncoding = System.Text.Encoding.GetEncoding(1252);
            } else {
                if (codePage != 28599)
                    throw new System.ArgumentException();
                encodingEncoding = System.Text.Encoding.GetEncoding(28599);
                decodingEncoding = System.Text.Encoding.GetEncoding(1254);
            }
        }

        public override int CodePage => encodingEncoding.CodePage;
        public override string BodyName => encodingEncoding.BodyName;
        public override string EncodingName => encodingEncoding.EncodingName;
        public override string HeaderName => encodingEncoding.HeaderName;
        public override string WebName => encodingEncoding.WebName;
        public override int WindowsCodePage => encodingEncoding.WindowsCodePage;
        public override bool IsBrowserDisplay => encodingEncoding.IsBrowserDisplay;
        public override bool IsBrowserSave => encodingEncoding.IsBrowserSave;
        public override bool IsMailNewsDisplay => encodingEncoding.IsMailNewsDisplay;
        public override bool IsMailNewsSave => encodingEncoding.IsMailNewsSave;
        public override bool IsSingleByte => encodingEncoding.IsSingleByte;

        public override byte[] GetPreamble() {
            return encodingEncoding.GetPreamble();
        }

        public override int GetMaxByteCount(int charCount) {
            return encodingEncoding.GetMaxByteCount(charCount);
        }

        public override int GetMaxCharCount(int byteCount) {
            return decodingEncoding.GetMaxCharCount(byteCount);
        }

        public override int GetByteCount(char[] chars, int index, int count) {
            return encodingEncoding.GetByteCount(chars, index, count);
        }

        public override int GetByteCount(string s) {
            return encodingEncoding.GetByteCount(s);
        }

        public override unsafe int GetByteCount(char* chars, int count) {
            return encodingEncoding.GetByteCount(chars, count);
        }

        public override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex) {
            return encodingEncoding.GetBytes(s, charIndex, charCount, bytes, byteIndex);
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex) {
            return encodingEncoding.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
        }

        public override unsafe int GetBytes(char* chars, int charCount, byte* bytes, int byteCount) {
            return encodingEncoding.GetBytes(chars, charCount, bytes, byteCount);
        }

        public override int GetCharCount(byte[] bytes, int index, int count) {
            return decodingEncoding.GetCharCount(bytes, index, count);
        }

        public override unsafe int GetCharCount(byte* bytes, int count) {
            return decodingEncoding.GetCharCount(bytes, count);
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex) {
            return decodingEncoding.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
        }

        public override unsafe int GetChars(byte* bytes, int byteCount, char* chars, int charCount) {
            return decodingEncoding.GetChars(bytes, byteCount, chars, charCount);
        }

        public override string GetString(byte[] bytes, int index, int count) {
            return decodingEncoding.GetString(bytes, index, count);
        }

        public override System.Text.Decoder GetDecoder() {
            return decodingEncoding.GetDecoder();
        }

        public override System.Text.Encoder GetEncoder() {
            return encodingEncoding.GetEncoder();
        }

        public override object Clone() {
            return (System.Text.Encoding) this.MemberwiseClone();
        }

        private readonly System.Text.Encoding decodingEncoding;
        private readonly System.Text.Encoding encodingEncoding;

    }

}