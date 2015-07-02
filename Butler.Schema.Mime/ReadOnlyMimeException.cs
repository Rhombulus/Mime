using System;
using System.Linq;

namespace Butler.Schema.Data.Mime {

    internal class ReadOnlyMimeException : InvalidOperationException {

        public ReadOnlyMimeException(string method)
            : base(string.Format("{0} was called on a read-only MIME document.", method)) {}

    }

}