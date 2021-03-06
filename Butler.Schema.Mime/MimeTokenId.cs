﻿using System.Linq;

namespace Butler.Schema.Mime {

    internal enum MimeTokenId : short {

        None,
        Header,
        HeaderContinuation,
        EndOfHeaders,
        PartData,
        NestedStart,
        NestedNext,
        NestedEnd,
        InlineStart,
        InlineEnd,
        EmbeddedStart,
        EmbeddedEnd,
        EndOfFile

    }

}