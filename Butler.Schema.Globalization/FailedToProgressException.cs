namespace Butler.Schema.Globalization {

    [System.Serializable]
    public class FailedToProgressException : ButlerSchemaException {

        public FailedToProgressException(string message)
            : base(message) {}

        protected FailedToProgressException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) {}

    }

}