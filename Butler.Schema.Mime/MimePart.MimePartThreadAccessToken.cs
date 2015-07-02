using System.Linq;

namespace Butler.Schema.Mime {

    public partial class MimePart {

        private class MimePartThreadAccessToken : ObjectThreadAccessToken {

            internal MimePartThreadAccessToken(MimePart parent) {}

        }

    }

}