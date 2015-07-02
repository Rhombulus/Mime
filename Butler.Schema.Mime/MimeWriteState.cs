using System.Linq;

namespace Butler.Schema.Mime {

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