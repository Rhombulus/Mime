using System;
using System.Linq;

namespace Butler.Schema.Data.Mime {

    [Serializable]
    public class MimeException : ExchangeDataException {

        public MimeException(string message)
            : base(CtsResources.Strings.InternalMimeError + " " + message) {}

        public MimeException(string message, Exception innerException)
            : base(CtsResources.Strings.InternalMimeError + " " + message, innerException) {}

        protected MimeException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) {}

    }

}