using System.Linq;

namespace Butler.Schema.Mime.Encoders {

    [System.Serializable]
    public class ByteEncoderException : ButlerSchemaException {

        public ByteEncoderException(string message)
            : base(message) {}

        public ByteEncoderException(string message, System.Exception innerException)
            : base(message, innerException) {}

        protected ByteEncoderException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) {}

    }

}