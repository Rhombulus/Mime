using System;
using System.Linq;

namespace Butler.Schema.Data.Mime {

    internal enum PartContentParseOptionInternal {

        Parse,
        ParseRawContent,
        ParseEmbeddedMessage,
        Skip

    }

}