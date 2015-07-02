using System.Linq;

namespace Butler.Schema.Data.Mime {

    public struct DecodingOptions {

        public DecodingOptions(DecodingFlags decodingFlags) {
            this = new DecodingOptions(decodingFlags, (string) null);
        }

        public DecodingOptions(DecodingFlags decodingFlags, System.Text.Encoding encoding) {
            this.DecodingFlags = decodingFlags;
            this.Charset = encoding == null ? null : Globalization.Charset.GetCharset(encoding);
        }

        public DecodingOptions(DecodingFlags decodingFlags, string charsetName) {
            this.DecodingFlags = decodingFlags;
            this.Charset = charsetName == null ? null : Globalization.Charset.GetCharset(charsetName);
        }

        internal DecodingOptions(string charsetName) {
            this = new DecodingOptions(Default.DecodingFlags, charsetName);
        }

        internal static Globalization.Charset DefaultCharset {
            get {
                if (defaultCharset == null) {
                    var charset = Globalization.Charset.DefaultMimeCharset;
                    if (!charset.IsAvailable || charset.AsciiSupport < Globalization.CodePageAsciiSupport.Fine)
                        charset = Globalization.Charset.UTF8;
                    defaultCharset = charset;
                }
                return defaultCharset;
            }
        }

        public DecodingFlags DecodingFlags { get; internal set; }

        public string CharsetName {
            get {
                if (this.Charset != null)
                    return this.Charset.Name;
                return null;
            }
        }

        public System.Text.Encoding CharsetEncoding {
            get {
                if (this.Charset != null && this.Charset.IsAvailable)
                    return this.Charset.GetEncoding();
                return null;
            }
        }

        public bool AllowUTF8 => (this.DecodingFlags & DecodingFlags.Utf8) == DecodingFlags.Utf8;
        internal Globalization.Charset Charset { get; set; }
        public static readonly DecodingOptions None = new DecodingOptions();
        public static readonly DecodingOptions Default = new DecodingOptions(DecodingFlags.AllEncodings);
        private static Globalization.Charset defaultCharset;

    }

}