using System;
using System.Linq;

namespace Butler.Schema.Data.Mime.Encoders {

    [Serializable]
    public class ByteEncoderException : ExchangeDataException {

        public ByteEncoderException(string message)
            : base(message) {}

        public ByteEncoderException(string message, Exception innerException)
            : base(message, innerException) {}

        protected ByteEncoderException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) {}

    }

}