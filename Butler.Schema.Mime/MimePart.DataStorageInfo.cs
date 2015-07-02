using System.Linq;

namespace Butler.Schema.Mime {

    public partial class MimePart {

        private class DataStorageInfo {

            public ContentTransferEncoding BodyCte;
            public LineTerminationState BodyLineTermination;
            public long BodyOffset;
            public long DataEnd;
            public long DataStart;

        }

    }

}