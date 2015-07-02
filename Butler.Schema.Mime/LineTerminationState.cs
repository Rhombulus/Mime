using System.Linq;

namespace Butler.Schema.Mime {

    internal enum LineTerminationState : byte {

        CRLF,
        CR,
        Other,
        Unknown,
        NotInteresting

    }

}