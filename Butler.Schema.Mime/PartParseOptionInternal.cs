using System.Linq;

namespace Butler.Schema.Mime {

    internal enum PartParseOptionInternal {

        Parse,
        ParseSkipHeaders,
        ParseRawOuterContent,
        Skip

    }

}