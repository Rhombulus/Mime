namespace Butler.Schema.Data.Common {

    [System.Serializable]
    public class TransientException : LocalizedException {

        public TransientException(LocalizedString localizedString)
            : this(localizedString, null) {}

        public TransientException(LocalizedString localizedString, System.Exception innerException)
            : base(localizedString, innerException) {
            this.LocalizedString = localizedString;
        }

        protected TransientException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext context)
            : base(serializationInfo, context) {}

        public new LocalizedString LocalizedString { get; }

    }

}