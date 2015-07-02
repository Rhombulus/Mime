using System;
using System.Linq;

namespace Butler.Schema.Data.Mime {

    public enum EncodingScheme : byte {

        None,
        Rfc2047,
        Rfc2231,
        Jis,
        EightBit

    }

}