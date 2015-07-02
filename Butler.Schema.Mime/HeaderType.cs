using System;
using System.Linq;

namespace Butler.Schema.Data.Mime {

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