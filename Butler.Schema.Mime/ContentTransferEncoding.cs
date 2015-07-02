using System.Linq;

namespace Butler.Schema.Mime {

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