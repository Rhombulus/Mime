using System.Linq;

namespace Butler.Schema.Data.Mime {

    [System.Serializable]
    public class MimeException : ButlerSchemaException {

        public MimeException(string message)
            : base(Resources.Strings.InternalMimeError + " " + message) {}

        public MimeException(string message, System.Exception innerException)
            : base(Resources.Strings.InternalMimeError + " " + message, innerException) {}

        protected MimeException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) {}

    }

}