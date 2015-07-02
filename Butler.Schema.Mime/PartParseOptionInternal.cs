using System.Linq;

namespace Butler.Schema.Data.Mime {

    internal enum PartParseOptionInternal {

        Parse,
        ParseSkipHeaders,
        ParseRawOuterContent,
        Skip

    }

}