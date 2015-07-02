using System.Linq;

namespace Butler.Schema.Mime {

    public class EncodingOptions {

        public EncodingOptions(string charsetName, string cultureName, EncodingFlags encodingFlags) {
            this.CultureName = cultureName;
            this.EncodingFlags = encodingFlags;
            charset = charsetName == null ? null : Globalization.Charset.GetCharset(charsetName);
            if (charset == null)
                return;
            charset.GetEncoding();
        }

        internal EncodingOptions(Globalization.Charset charset) {
            this.charset = charset;
            this.CultureName = null;
            this.EncodingFlags = EncodingFlags.None;
        }

        public string CharsetName {
            get {
                if (charset != null)
                    return charset.Name;
                return null;
            }
        }

        public string CultureName { get; }
        public EncodingFlags EncodingFlags { get; }
        public bool AllowUTF8 => (this.EncodingFlags & EncodingFlags.AllowUTF8) == EncodingFlags.AllowUTF8;

        internal Globalization.Charset GetEncodingCharset() {
            return charset ?? DefaultCharset;
        }

        internal static readonly Globalization.Charset DefaultCharset = DecodingOptions.DefaultCharset;
        private readonly Globalization.Charset charset;

    }

}