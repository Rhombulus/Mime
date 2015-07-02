namespace Butler.Schema.Globalization {

    [System.Serializable]
    public class CharsetNotInstalledException : InvalidCharsetException {

        public CharsetNotInstalledException(int codePage)
            : base(codePage, Resources.GlobalizationStrings.NotInstalledCodePage(codePage)) {}

        public CharsetNotInstalledException(string charsetName)
            : base(charsetName, Resources.GlobalizationStrings.NotInstalledCharset(charsetName == null ? "<null>" : charsetName)) {}

        internal CharsetNotInstalledException(string charsetName, int codePage)
            : base(charsetName, codePage, Resources.GlobalizationStrings.NotInstalledCharsetCodePage(codePage, charsetName == null ? "<null>" : charsetName)) {}

        public CharsetNotInstalledException(int codePage, string message)
            : base(codePage, message) {}

        public CharsetNotInstalledException(string charsetName, string message)
            : base(charsetName, message) {}

        public CharsetNotInstalledException(int codePage, string message, System.Exception innerException)
            : base(codePage, message, innerException) {}

        public CharsetNotInstalledException(string charsetName, string message, System.Exception innerException)
            : base(charsetName, message, innerException) {}

        protected CharsetNotInstalledException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) {}

    }

}