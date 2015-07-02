using System.Linq;

namespace Butler.Schema.Mime {

    internal enum AddressParserResult {

        Mailbox,
        GroupStart,
        GroupInProgress,
        End

    }

}