using System.Linq;

namespace Butler.Schema.Mime {

    public partial class MimePart {

        private class PartContentWriteStream : Internal.AppendStreamOnDataStorage {

            public PartContentWriteStream(MimePart mimePart, ContentTransferEncoding contentTransferEncoding)
                : base(new Internal.TemporaryDataStorage()) {
                _mimePart = mimePart;
                _contentTransferEncoding = contentTransferEncoding;
            }

            protected override void Dispose(bool disposing) {
                if (disposing && _mimePart != null) {
                    using (ThreadAccessGuard.EnterPublic(_mimePart.AccessToken)) {
                        Internal.ReadableDataStorage readableDataStorage = this.ReadableWritableStorage;
                        readableDataStorage.AddRef();
                        base.Dispose(true);
                        if (!_mimePart.isDisposed)
                            _mimePart.SetStorage(readableDataStorage, 0L, readableDataStorage.Length, 0L, _contentTransferEncoding, LineTerminationState.Unknown);
                        readableDataStorage.Release();
                    }
                    _mimePart = null;
                } else
                    base.Dispose(disposing);
            }

            private readonly ContentTransferEncoding _contentTransferEncoding;
            private MimePart _mimePart;

        }

    }

}