using System.Linq;

namespace Butler.Schema.Data.Mime {

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