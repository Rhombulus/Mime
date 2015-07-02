using System.Linq;

namespace Butler.Schema.Mime {

    internal enum HeaderType : byte {

        Text,
        AsciiText,
        Date,
        Received,
        ContentType,
        ContentDisposition,
        Address

    }

}