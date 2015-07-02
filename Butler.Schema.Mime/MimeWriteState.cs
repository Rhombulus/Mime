using System;
using System.Linq;

namespace Butler.Schema.Data.Mime {

    internal enum MimeWriteState {

        Initial,
        Complete,
        StartPart,
        Headers,
        Parameters,
        Recipients,
        GroupRecipients,
        PartContent,
        EndPart

    }

}