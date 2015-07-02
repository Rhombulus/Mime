using System.Linq;

namespace Butler.Schema.Mime {

    internal enum PartContentParseOptionInternal {

        Parse,
        ParseRawContent,
        ParseEmbeddedMessage,
        Skip

    }

}