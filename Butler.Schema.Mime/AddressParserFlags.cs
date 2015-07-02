using System.Linq;

namespace Butler.Schema.Data.Mime {

    [System.Flags]
    public enum AddressParserFlags {

        None = 0,
        IgnoreComments = 1,
        AllowSquareBrackets = 2

    }

}