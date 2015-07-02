namespace Butler.Schema.Data.Globalization {

    [System.Serializable]
    public class UnknownCultureException : ButlerSchemaException {

        public UnknownCultureException(int localeId)
            : base(Resources.GlobalizationStrings.InvalidLocaleId(localeId)) {
            this.LocaleId = localeId;
        }

        public UnknownCultureException(string cultureName)
            : base(Resources.GlobalizationStrings.InvalidCultureName(cultureName ?? "<null>")) {
            this.CultureName = cultureName;
        }

        public UnknownCultureException(int localeId, string message)
            : base(message) {
            this.LocaleId = localeId;
        }

        public UnknownCultureException(string cultureName, string message)
            : base(message) {
            this.CultureName = cultureName;
        }

        public UnknownCultureException(int localeId, string message, System.Exception innerException)
            : base(message, innerException) {
            this.LocaleId = localeId;
        }

        public UnknownCultureException(string cultureName, string message, System.Exception innerException)
            : base(message, innerException) {
            this.CultureName = cultureName;
        }

        protected UnknownCultureException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) {
            this.LocaleId = info.GetInt32("localeId");
            this.CultureName = info.GetString("cultureName");
        }

        public int LocaleId { get; }
        public string CultureName { get; }

    }

}