using System.Linq;

namespace Butler.Schema.Mime {

    public partial class MimePart {

        [System.Flags]
        public enum SubtreeEnumerationOptions : byte {

            None = 0,
            IncludeEmbeddedMessages = 1,
            RevisitParent = 2

        }

    }

}