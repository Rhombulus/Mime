using System.Linq;

namespace Butler.Schema.Data.Mime {

    public enum ContentTransferEncoding {

        Unknown,
        SevenBit,
        EightBit,
        Binary,
        QuotedPrintable,
        Base64,
        UUEncode,
        BinHex

    }

}