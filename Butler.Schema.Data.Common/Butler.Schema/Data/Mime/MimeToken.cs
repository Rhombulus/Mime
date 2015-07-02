using System;
using System.Linq;

namespace Butler.Schema.Data.Mime {

    internal struct MimeToken {

        public MimeToken(MimeTokenId id, int length) {
            Id = id;
            Length = (short) length;
        }

        public MimeTokenId Id;
        public short Length;

    }

}