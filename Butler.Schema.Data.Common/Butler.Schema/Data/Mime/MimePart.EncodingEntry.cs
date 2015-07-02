using System;
using System.Collections.Generic;
using System.Linq;

namespace Butler.Schema.Data.Mime {

    public partial class MimePart {

        private struct EncodingEntry {

            internal EncodingEntry(byte[] name, ContentTransferEncoding type) {
                Name = name;
                Type = type;
            }

            internal readonly byte[] Name;
            internal readonly ContentTransferEncoding Type;

        }

    }

}