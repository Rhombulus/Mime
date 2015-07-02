using System.Linq;

namespace Butler.Schema.Data.Mime {

    public partial class MimePart {

        private class MimePartThreadAccessToken : ObjectThreadAccessToken {

            internal MimePartThreadAccessToken(MimePart parent) {}

        }

    }

}