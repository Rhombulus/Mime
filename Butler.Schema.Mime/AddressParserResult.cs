using System;
using System.Linq;

namespace Butler.Schema.Data.Mime {

    internal enum AddressParserResult {

        Mailbox,
        GroupStart,
        GroupInProgress,
        End

    }

}