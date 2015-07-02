using System.Linq;

namespace Butler.Schema.Data.Mime {

    internal enum LineTerminationState : byte {

        CRLF,
        CR,
        Other,
        Unknown,
        NotInteresting

    }

}