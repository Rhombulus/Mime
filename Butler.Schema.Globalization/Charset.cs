namespace Butler.Schema.Data.Globalization {

    [System.Serializable]
    public class Charset {

        internal Charset(int codePage, string name) {
            this.CodePage = codePage;
            this.Name = name;
            this.Culture = null;
            available = true;
            mapIndex = -1;
        }

        public static Charset DefaultMimeCharset => Culture.Default.MimeCharset;
        public static bool FallbackToDefaultCharset => Culture.FallbackToDefaultCharset;
        public static Charset DefaultWebCharset => Culture.Default.WebCharset;
        public static Charset DefaultWindowsCharset => Culture.Default.WindowsCharset;
        public static Charset ASCII => CultureCharsetDatabase.Data.AsciiCharset;
        public static Charset UTF8 => CultureCharsetDatabase.Data.Utf8Charset;
        public static Charset Unicode => CultureCharsetDatabase.Data.UnicodeCharset;
        public int CodePage { get; }
        public string Name { get; private set; }
        public Culture Culture { get; private set; }

        public bool IsDetectable {
            get {
                if (mapIndex >= 0)
                    return CodePageFlags.None != (CodePageMapData.codePages[mapIndex].flags & CodePageFlags.Detectable);
                return false;
            }
        }

        public bool IsAvailable {
            get {
                if (!available)
                    return false;
                if (encoding == null)
                    return this.CheckAvailable();
                return true;
            }
        }

        public bool IsWindowsCharset { get; private set; }
        public string Description { get; private set; }
        internal static int MaxCharsetNameLength => CultureCharsetDatabase.Data.MaxCharsetNameLength;
        internal int MapIndex => (int) mapIndex;

        internal CodePageKind Kind {
            get {
                if (mapIndex >= 0)
                    return CodePageMapData.codePages[mapIndex].kind;
                return CodePageKind.Unknown;
            }
        }

        internal CodePageAsciiSupport AsciiSupport {
            get {
                if (mapIndex >= 0)
                    return CodePageMapData.codePages[mapIndex].asciiSupport;
                return CodePageAsciiSupport.Unknown;
            }
        }

        internal CodePageUnicodeCoverage UnicodeCoverage {
            get {
                if (mapIndex >= 0)
                    return CodePageMapData.codePages[mapIndex].unicodeCoverage;
                return CodePageUnicodeCoverage.Unknown;
            }
        }

        internal bool IsSevenBit {
            get {
                if (mapIndex >= 0)
                    return CodePageFlags.None != (CodePageMapData.codePages[mapIndex].flags & CodePageFlags.SevenBit);
                return false;
            }
        }

        internal int DetectableCodePageWithEquivalentCoverage {
            get {
                if (mapIndex < 0)
                    return 0;
                if ((CodePageMapData.codePages[mapIndex].flags & CodePageFlags.Detectable) == CodePageFlags.None)
                    return CodePageMapData.codePages[mapIndex].detectCpid;
                return this.CodePage;
            }
        }

        public static Charset GetCharset(string name) {
            Charset charset;
            if (!Charset.TryGetCharset(name, out charset))
                throw new InvalidCharsetException(name);
            return charset;
        }

        public static bool TryGetCharset(string name, out Charset charset) {
            if (name == null) {
                charset = null;
                return false;
            }
            if (CultureCharsetDatabase.Data.NameToCharset.TryGetValue(name, out charset))
                return true;
            if (name.StartsWith("cp", System.StringComparison.OrdinalIgnoreCase) || name.StartsWith("ms", System.StringComparison.OrdinalIgnoreCase)) {
                var codePage = 0;
                for (var index = 2; index < name.Length; ++index) {
                    if (name[index] < 48 || name[index] > 57)
                        return false;
                    codePage = codePage*10 + (name[index] - 48);
                    if (codePage >= 65536)
                        return false;
                }
                if (codePage == 0)
                    return false;
                return Charset.TryGetCharset(codePage, out charset);
            }
            return (Charset.FallbackToDefaultCharset && Charset.DefaultMimeCharset != null ||
                    !Charset.FallbackToDefaultCharset && Charset.DefaultMimeCharset != null && Charset.DefaultMimeCharset.Name.Equals("iso-2022-jp", System.StringComparison.OrdinalIgnoreCase)) &&
                   CultureCharsetDatabase.Data.NameToCharset.TryGetValue(Charset.DefaultMimeCharset.Name, out charset);
        }

        public static Charset GetCharset(int codePage) {
            Charset charset;
            if (!Charset.TryGetCharset(codePage, out charset))
                throw new InvalidCharsetException(codePage);
            return charset;
        }

        public static Charset GetCharset(System.Text.Encoding encoding) {
            return Charset.GetCharset(CodePageMap.GetCodePage(encoding));
        }

        public static bool TryGetCharset(int codePage, out Charset charset) {
            return CultureCharsetDatabase.Data.CodePageToCharset.TryGetValue(codePage, out charset);
        }

        public static bool TryGetEncoding(int codePage, out System.Text.Encoding encoding) {
            Charset charset;
            if (Charset.TryGetCharset(codePage, out charset))
                return charset.TryGetEncoding(out encoding);
            encoding = null;
            return false;
        }

        public static bool TryGetEncoding(string name, out System.Text.Encoding encoding) {
            Charset charset;
            if (Charset.TryGetCharset(name, out charset))
                return charset.TryGetEncoding(out encoding);
            encoding = null;
            return false;
        }

        public static System.Text.Encoding GetEncoding(int codePage) {
            return Charset.GetCharset(codePage)
                          .GetEncoding();
        }

        public static System.Text.Encoding GetEncoding(string name) {
            return Charset.GetCharset(name)
                          .GetEncoding();
        }

        public System.Text.Encoding GetEncoding() {
            System.Text.Encoding encoding;
            if (!this.TryGetEncoding(out encoding))
                throw new CharsetNotInstalledException(this.CodePage, this.Name);
            return encoding;
        }

        internal void SetCulture(Culture culture) {
            this.Culture = culture;
        }

        internal void SetDescription(string description) {
            this.Description = description;
        }

        internal void SetDefaultName(string name) {
            this.Name = name;
        }

        internal void SetWindows() {
            this.IsWindowsCharset = true;
        }

        internal void SetMapIndex(int index) {
            mapIndex = (short) index;
        }

        internal bool CheckAvailable() {
            System.Text.Encoding encoding;
            return this.TryGetEncoding(out encoding);
        }

        public static bool TryGetCharset(System.Text.Encoding encoding, out Charset charset) {
            return Charset.TryGetCharset(encoding.CodePage, out charset);
        }

        public bool TryGetEncoding(out System.Text.Encoding encoding) {
            if (this.encoding == null) {
                if (available) {
                    try {
                        this.encoding = this.CodePage != 20127
                                            ? (this.CodePage == 28591 || this.CodePage == 28599
                                                   ? new RemapEncoding(this.CodePage)
                                                   : (this.CodePage == 50220 || this.CodePage == 50221 || this.CodePage == 50222 ? new Iso2022JpEncoding(this.CodePage) : System.Text.Encoding.GetEncoding(this.CodePage)))
                                            : System.Text.Encoding.GetEncoding(this.CodePage, new AsciiEncoderFallback(), System.Text.DecoderFallback.ReplacementFallback);
                    } catch (System.ArgumentException ex) {
                        this.encoding = null;
                    } catch (System.NotSupportedException ex) {
                        this.encoding = null;
                    }
                    if (this.encoding == null)
                        available = false;
                }
            }
            encoding = this.encoding;
            return encoding != null;
        }

        private bool available;
        private System.Text.Encoding encoding;
        private short mapIndex;

    }

}