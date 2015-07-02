using System.Linq;

namespace Butler.Schema.Data.Mime {

    [System.Flags]
    public enum EncodingFlags : byte {

        None = 0,
        ForceReencode = (byte) 1,
        EnableRfc2231 = (byte) 2,
        QuoteDisplayNameBeforeRfc2047Encoding = (byte) 4,
        AllowUTF8 = (byte) 8

    }

}