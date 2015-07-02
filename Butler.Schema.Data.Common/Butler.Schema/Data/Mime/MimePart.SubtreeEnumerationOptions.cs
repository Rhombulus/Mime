using System;
using System.Collections.Generic;
using System.Linq;

namespace Butler.Schema.Data.Mime {

    public partial class MimePart {

        [Flags]
        public enum SubtreeEnumerationOptions : byte {

            None = 0,
            IncludeEmbeddedMessages = 1,
            RevisitParent = 2

        }

    }

}