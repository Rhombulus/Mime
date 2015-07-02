using System.Linq;

namespace Butler.Schema.Mime {

    public partial class MimePart : MimeNode, System.IDisposable, System.Collections.Generic.IEnumerable<MimePart> {

        public MimePart() {
            accessToken = new MimePartThreadAccessToken(this);
            headers = new HeaderList(this);
        }

        public MimePart(string contentType)
            : this() {
            if (contentType == null)
                throw new System.ArgumentNullException(nameof(contentType));
            using (ThreadAccessGuard.EnterPublic(accessToken))
                headers.InternalAppendChild(new ContentTypeHeader(contentType));
        }

        public MimePart(string contentType, string transferEncoding, System.IO.Stream contentStream, CachingMode cachingMode)
            : this(contentType) {
            using (ThreadAccessGuard.EnterPublic(accessToken))
                this.SetContentStream(transferEncoding, contentStream, cachingMode);
        }

        public MimePart(string contentType, ContentTransferEncoding transferEncoding, System.IO.Stream contentStream, CachingMode cachingMode)
            : this(contentType) {
            using (ThreadAccessGuard.EnterPublic(accessToken))
                this.SetContentStream(transferEncoding, contentStream, cachingMode);
        }

        public HeaderList Headers {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    return headers;
            }
        }

        public string ContentType {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(accessToken)) {
                    var contentTypeHeader1 = headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
                    if (contentTypeHeader1 != null)
                        return contentTypeHeader1.Value;
                    var mimePart = this.Parent as MimePart;
                    if (mimePart != null) {
                        var contentTypeHeader2 = mimePart.Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
                        if (contentTypeHeader2 != null && contentTypeHeader2.Value == "multipart/digest")
                            return "message/rfc822";
                    }
                    return "text/plain";
                }
            }
        }

        public bool IsMultipart {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(accessToken)) {
                    var contentTypeHeader = headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
                    return contentTypeHeader != null && contentTypeHeader.IsMultipart;
                }
            }
        }

        public bool IsEmbeddedMessage {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(accessToken)) {
                    var contentTypeHeader = headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
                    return contentTypeHeader != null && contentTypeHeader.IsEmbeddedMessage;
                }
            }
        }

        public bool RequiresSMTPUTF8 {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(accessToken)) {
                    using (var enumerator = this.Subtree.GetEnumerator(SubtreeEnumerationOptions.IncludeEmbeddedMessages, true)) {
                        while (enumerator.MoveNext()) {
                            foreach (var header in enumerator.Current.Headers) {
                                if (header.RequiresSMTPUTF8)
                                    return true;
                            }
                        }
                    }
                    return false;
                }
            }
        }

        internal bool IsAnyMessage {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(accessToken)) {
                    var contentTypeHeader = headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
                    return contentTypeHeader != null && contentTypeHeader.IsAnyMessage;
                }
            }
        }

        internal int CacheMapStamp {
            get {
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    return cacheMapStamp;
            }
            set {
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    cacheMapStamp = value;
            }
        }

        public ContentTransferEncoding ContentTransferEncoding {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(accessToken)) {
                    var first = headers.FindFirst(HeaderId.ContentTransferEncoding);
                    if (first != null && first.FirstRawToken.Length != 0)
                        return MimePart.GetEncodingType(first.FirstRawToken);
                    return ContentTransferEncoding.SevenBit;
                }
            }
        }

        public int Version { get; private set; }
        internal ObjectThreadAccessToken AccessToken => accessToken;

        internal ObjectThreadAccessToken ParentAccessToken {
            get {
                MimeNode treeRoot;
                var parentDocument = MimeNode.GetParentDocument(this, out treeRoot);
                if (parentDocument == null)
                    return null;
                return parentDocument.AccessToken;
            }
        }

        public PartSubtree Subtree {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    return new PartSubtree(this);
            }
        }

        internal Internal.DataStorage Storage {
            get {
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    return dataStorage;
            }
        }

        internal long DataStart {
            get {
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    return _storageInfo.DataStart;
            }
        }

        internal long DataEnd {
            get {
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    return _storageInfo.DataEnd;
            }
        }

        internal long DataLength {
            get {
                using (ThreadAccessGuard.EnterPublic(accessToken)) {
                    if (dataStorage == null)
                        return 0L;
                    if (MimePart.IsEqualContentTransferEncoding(_storageInfo.BodyCte, this.ContentTransferEncoding) && _storageInfo.DataEnd != long.MaxValue)
                        return _storageInfo.DataEnd - _storageInfo.DataStart - _storageInfo.BodyOffset;
                    using (var contentReadStream = this.GetRawContentReadStream()) {
                        if (contentReadStream.CanSeek)
                            return contentReadStream.Length;
                        var buffer = new byte[4096];
                        var num1 = 0L;
                        int num2;
                        while ((num2 = contentReadStream.Read(buffer, 0, buffer.Length)) != 0)
                            num1 += num2;
                        return num1;
                    }
                }
            }
        }

        internal long BodyOffset {
            get {
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    return _storageInfo.BodyOffset;
            }
        }

        internal ContentTransferEncoding BodyCte {
            get {
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    return _storageInfo.BodyCte;
            }
        }

        internal LineTerminationState BodyLineTermination {
            get {
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    return _storageInfo.BodyLineTermination;
            }
        }

        internal byte[] Boundary {
            get {
                using (ThreadAccessGuard.EnterPublic(accessToken)) {
                    if (boundary == null)
                        boundary = MimePart.GetBoundary(this.Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader);
                    return boundary;
                }
            }
        }

        internal bool IsSignedContent {
            get {
                using (ThreadAccessGuard.EnterPublic(accessToken)) {
                    if (this.Parent == null || this != this.Parent.FirstChild || (dataStorage == null || 0L == _storageInfo.BodyOffset) || this.Version != 0)
                        return false;
                    var contentTypeHeader = (this.Parent as MimePart).Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
                    return contentTypeHeader != null && contentTypeHeader.Value == "multipart/signed";
                }
            }
        }

        internal bool IsProtectedContent {
            get {
                using (ThreadAccessGuard.EnterPublic(accessToken)) {
                    if (this.Parent != null && dataStorage != null && (0L != _storageInfo.BodyOffset && this.Version == 0))
                        return (this.Parent as MimePart).Headers.FindFirst("DKIM-Signature") != null;
                    return false;
                }
            }
        }

        internal MimeDocument ParentDocument {
            get {
                return parentDocument;
            }
            set {
                this.ThrowIfReadOnly("MimePart.set_ParentDocument");
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    parentDocument = value;
            }
        }

        internal bool ContentDirty {
            get {
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    return contentDirty;
            }
            set {
                using (ThreadAccessGuard.EnterPublic(accessToken)) {
                    if (contentDirty != value)
                        this.ThrowIfReadOnly("MimePart.set_ContentDirty");
                    contentDirty = value;
                }
            }
        }

        internal bool ContentPersisted {
            get {
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    return contentPersisted && _storageInfo.BodyCte == this.ContentTransferEncoding;
            }
            set {
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    contentPersisted = value;
            }
        }

        internal Internal.DataStorage DeferredStorage {
            get {
                using (ThreadAccessGuard.EnterPublic(accessToken)) {
                    if (!this.IsReadOnly)
                        return this.Storage;
                    lock (deferredStorageLock)
                        return deferredStorageInfo != null ? deferredStorage : dataStorage;
                }
            }
        }

        internal long DeferredDataStart {
            get {
                using (ThreadAccessGuard.EnterPublic(accessToken)) {
                    if (!this.IsReadOnly)
                        return this.DataStart;
                    lock (deferredStorageLock)
                        return deferredStorageInfo != null ? deferredStorageInfo.DataStart : _storageInfo.DataStart;
                }
            }
        }

        internal long DeferredDataEnd {
            get {
                using (ThreadAccessGuard.EnterPublic(accessToken)) {
                    if (!this.IsReadOnly)
                        return this.DataEnd;
                    lock (deferredStorageLock)
                        return deferredStorageInfo != null ? deferredStorageInfo.DataEnd : _storageInfo.DataEnd;
                }
            }
        }

        internal long DeferredDataLength {
            get {
                using (ThreadAccessGuard.EnterPublic(accessToken)) {
                    if (!this.IsReadOnly)
                        return this.DataLength;
                    Internal.DataStorage storage;
                    DataStorageInfo storageInfo;
                    lock (deferredStorageLock) {
                        storage = deferredStorage;
                        storageInfo = deferredStorageInfo;
                    }
                    if (storageInfo == null)
                        return this.DataLength;
                    if (storage == null)
                        return 0L;
                    if (MimePart.IsEqualContentTransferEncoding(storageInfo.BodyCte, this.ContentTransferEncoding) && storageInfo.DataEnd != long.MaxValue)
                        return storageInfo.DataEnd - storageInfo.DataStart - storageInfo.BodyOffset;
                    using (var contentReadStream = this.GetRawContentReadStream(storage, storageInfo)) {
                        if (contentReadStream.CanSeek)
                            return contentReadStream.Length;
                        var buffer = new byte[4096];
                        var num1 = 0L;
                        int num2;
                        while ((num2 = contentReadStream.Read(buffer, 0, buffer.Length)) != 0)
                            num1 += num2;
                        return num1;
                    }
                }
            }
        }

        internal long DeferredBodyOffset {
            get {
                using (ThreadAccessGuard.EnterPublic(accessToken)) {
                    if (!this.IsReadOnly)
                        return this.BodyOffset;
                    lock (deferredStorageLock)
                        return deferredStorageInfo != null ? deferredStorageInfo.BodyOffset : _storageInfo.BodyOffset;
                }
            }
        }

        internal ContentTransferEncoding DeferredBodyCte {
            get {
                using (ThreadAccessGuard.EnterPublic(accessToken)) {
                    if (!this.IsReadOnly)
                        return this.BodyCte;
                    lock (deferredStorageLock)
                        return deferredStorageInfo != null ? deferredStorageInfo.BodyCte : _storageInfo.BodyCte;
                }
            }
        }

        internal LineTerminationState DeferredBodyLineTermination {
            get {
                using (ThreadAccessGuard.EnterPublic(accessToken)) {
                    if (!this.IsReadOnly)
                        return this.BodyLineTermination;
                    lock (deferredStorageLock)
                        return deferredStorageInfo != null ? deferredStorageInfo.BodyLineTermination : _storageInfo.BodyLineTermination;
                }
            }
        }

        public void Dispose() {
            this.Dispose(true);
        }

        System.Collections.Generic.IEnumerator<MimePart> System.Collections.Generic.IEnumerable<MimePart>.GetEnumerator() {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(accessToken))
                return new Enumerator<MimePart>(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(accessToken))
                return new Enumerator<MimePart>(this);
        }

        public System.IO.Stream GetRawContentReadStream() {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(accessToken))
                return this.GetRawContentReadStream(dataStorage, _storageInfo);
        }

        internal System.IO.Stream GetDeferredRawContentReadStream() {
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                if (!this.IsReadOnly)
                    return this.GetRawContentReadStream();
                Internal.DataStorage storage;
                DataStorageInfo storageInfo;
                lock (deferredStorageLock) {
                    storage = deferredStorage;
                    storageInfo = deferredStorageInfo;
                }
                if (storageInfo == null)
                    return this.GetRawContentReadStream();
                return this.GetRawContentReadStream(storage, storageInfo);
            }
        }

        private System.IO.Stream GetRawContentReadStream(Internal.DataStorage storage, DataStorageInfo storageInfo) {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPrivate(accessToken)) {
                System.IO.Stream result;
                if (!this.TryGetContentReadStream(storage, storageInfo, this.ContentTransferEncoding, out result))
                    throw new MimeException(Resources.Strings.CannotDecodeContentStream);
                return result;
            }
        }

        public System.IO.Stream GetContentReadStream() {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                System.IO.Stream result;
                if (!this.TryGetContentReadStream(dataStorage, _storageInfo, ContentTransferEncoding.Binary, out result))
                    throw new MimeException(Resources.Strings.CannotDecodeContentStream);
                return result;
            }
        }

        public bool TryGetContentReadStream(out System.IO.Stream result) {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(accessToken))
                return this.TryGetContentReadStream(dataStorage, _storageInfo, ContentTransferEncoding.Binary, out result);
        }

        private bool TryGetContentReadStream(Internal.DataStorage dataStorage, DataStorageInfo storageInfo, ContentTransferEncoding desiredCte, out System.IO.Stream result) {
            using (ThreadAccessGuard.EnterPrivate(accessToken)) {
                result = null;
                var transferEncoding = storageInfo.BodyCte;
                if (transferEncoding == ContentTransferEncoding.Unknown)
                    transferEncoding = this.ContentTransferEncoding;
                if (!MimePart.IsEqualContentTransferEncoding(desiredCte, transferEncoding)) {
                    if (desiredCte == ContentTransferEncoding.Unknown || transferEncoding == ContentTransferEncoding.Unknown)
                        return false;
                    if (this.IsMultipart || this.IsAnyMessage) {
                        transferEncoding = ContentTransferEncoding.Binary;
                        desiredCte = ContentTransferEncoding.Binary;
                    }
                }
                var flag = false;
                try {
                    if (dataStorage == null) {
                        if (this.IsMultipart && this.InternalLastChild != null)
                            return true;
                        if (!this.IsEmbeddedMessage || this.InternalLastChild == null) {
                            result = Internal.DataStorage.NewEmptyReadStream();
                            flag = true;
                            return true;
                        }
                        var temporaryDataStorage = new Internal.TemporaryDataStorage();
                        try {
                            using (System.IO.Stream stream = temporaryDataStorage.OpenWriteStream(true))
                                this.FirstChild.WriteTo(stream);
                            result = temporaryDataStorage.OpenReadStream(0L, temporaryDataStorage.Length);
                        } finally {
                            temporaryDataStorage.Release();
                        }
                    } else
                        result = dataStorage.OpenReadStream(storageInfo.DataStart + storageInfo.BodyOffset, storageInfo.DataEnd);
                    if (!MimePart.IsEqualContentTransferEncoding(desiredCte, transferEncoding)) {
                        if (!MimePart.EncodingIsTransparent(transferEncoding)) {
                            var decoder = MimePart.CreateDecoder(transferEncoding);
                            result = new Schema.Mime.Encoders.EncoderStream(result, decoder, Schema.Mime.Encoders.EncoderStreamAccess.Read, true);
                        }
                        if (!MimePart.EncodingIsTransparent(desiredCte)) {
                            var encoder = MimePart.CreateEncoder(result, desiredCte);
                            result = new Schema.Mime.Encoders.EncoderStream(result, encoder, Schema.Mime.Encoders.EncoderStreamAccess.Read, true);
                        }
                    }
                    flag = true;
                } finally {
                    if (!flag && result != null) {
                        result.Dispose();
                        result = null;
                    }
                }
                return true;
            }
        }

        public System.IO.Stream GetRawContentWriteStream() {
            this.ThrowIfDisposed();
            this.ThrowIfReadOnly("MimePart.GetRawContentWriteStream");
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                if (this.IsMultipart)
                    throw new System.NotSupportedException(Resources.Strings.ModifyingRawContentOfMultipartNotSupported);
                this.SetStorage(null, 0L, 0L);
                return new PartContentWriteStream(this, ContentTransferEncoding.Unknown);
            }
        }

        public System.IO.Stream GetContentWriteStream(ContentTransferEncoding transferEncoding) {
            this.ThrowIfDisposed();
            this.ThrowIfReadOnly("MimePart.GetContentWriteStream");
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                if (this.IsMultipart)
                    throw new System.NotSupportedException(Resources.Strings.ModifyingRawContentOfMultipartNotSupported);
                this.UpdateTransferEncoding(transferEncoding);
                this.SetStorage(null, 0L, 0L);
                return new PartContentWriteStream(this, ContentTransferEncoding.Binary);
            }
        }

        public System.IO.Stream GetContentWriteStream(string transferEncoding) {
            this.ThrowIfDisposed();
            this.ThrowIfReadOnly("MimePart.GetContentWriteStream");
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                if (transferEncoding == null)
                    throw new System.ArgumentNullException(nameof(transferEncoding));
                return this.GetContentWriteStream(MimePart.GetEncodingType(new MimeString(transferEncoding)));
            }
        }

        public void SetContentStream(string transferEncoding, System.IO.Stream contentStream, CachingMode cachingMode) {
            this.ThrowIfDisposed();
            this.ThrowIfReadOnly("MimePart.SetContentStream");
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                var transferEncoding1 = ContentTransferEncoding.Unknown;
                if (transferEncoding != null) {
                    transferEncoding1 = MimePart.GetEncodingType(new MimeString(transferEncoding));
                    if (transferEncoding1 == ContentTransferEncoding.Unknown)
                        throw new System.ArgumentException("Transfer encoding is unknown or not supported", nameof(transferEncoding));
                }
                this.SetContentStream(transferEncoding1, contentStream, cachingMode);
            }
        }

        public void SetContentStream(ContentTransferEncoding transferEncoding, System.IO.Stream contentStream, CachingMode cachingMode) {
            this.ThrowIfDisposed();
            this.ThrowIfReadOnly("MimePart.SetContentStream");
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                if (contentStream == null)
                    throw new System.ArgumentNullException(nameof(contentStream));
                if (!contentStream.CanRead)
                    throw new System.ArgumentException(Resources.Strings.StreamMustSupportRead, nameof(contentStream));
                if (this.IsMultipart)
                    throw new System.NotSupportedException(Resources.Strings.ModifyingRawContentOfMultipartNotSupported);
                var dataStart = 0L;
                var dataEnd = long.MaxValue;
                if (transferEncoding != ContentTransferEncoding.Unknown) {
                    this.UpdateTransferEncoding(transferEncoding);
                    transferEncoding = ContentTransferEncoding.Binary;
                }
                Internal.DataStorage storage;
                switch (cachingMode) {
                    case CachingMode.Copy:
                        var temporaryDataStorage = new Internal.TemporaryDataStorage();
                        storage = temporaryDataStorage;
                        using (System.IO.Stream destStream = temporaryDataStorage.OpenWriteStream(true)) {
                            byte[] scratchBuffer = null;
                            dataEnd = Internal.DataStorage.CopyStreamToStream(contentStream, destStream, long.MaxValue, ref scratchBuffer);
                            break;
                        }
                    case CachingMode.Source:
                    case CachingMode.SourceTakeOwnership:
                        if (!contentStream.CanSeek)
                            throw new System.NotSupportedException(Resources.Strings.CachingModeSourceButStreamCannotSeek);
                        var streamOnDataStorage = contentStream as Internal.StreamOnDataStorage;
                        if (streamOnDataStorage != null) {
                            storage = streamOnDataStorage.Storage;
                            dataStart = streamOnDataStorage.Start;
                            dataEnd = streamOnDataStorage.End;
                            storage.AddRef();
                            if (cachingMode == CachingMode.SourceTakeOwnership) {
                                contentStream.Dispose();
                                contentStream = null;
                            }
                            break;
                        }
                        storage = new Internal.ReadableDataStorageOnStream(contentStream, cachingMode == CachingMode.SourceTakeOwnership);
                        break;
                    default:
                        throw new System.ArgumentException("Invalid Caching Mode value", nameof(cachingMode));
                }
                this.SetStorage(storage, dataStart, dataEnd, 0L, transferEncoding, LineTerminationState.Unknown);
                storage.Release();
            }
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing && !isDisposed) {
                using (var enumerator = this.Subtree.GetEnumerator(SubtreeEnumerationOptions.IncludeEmbeddedMessages, false)) {
                    while (enumerator.MoveNext()) {
                        if (enumerator.Current.dataStorage != null) {
                            enumerator.Current.dataStorage.Release();
                            enumerator.Current.dataStorage = null;
                        }
                        if (enumerator.Current.headers != null)
                            enumerator.Current.headers.InternalDetachParent();
                        enumerator.Current.headers = null;
                        enumerator.Current.boundary = null;
                        enumerator.Current.parentDocument = null;
                        if (enumerator.Current.deferredStorage != null) {
                            enumerator.Current.deferredStorage.Release();
                            enumerator.Current.deferredStorage = null;
                            enumerator.Current.deferredStorageInfo = null;
                        }
                        enumerator.Current.isDisposed = true;
                        System.GC.SuppressFinalize(enumerator.Current);
                    }
                }
            } else
                isDisposed = true;
        }

        public new Enumerator<MimePart> GetEnumerator() {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(accessToken))
                return new Enumerator<MimePart>(this);
        }

        public override sealed MimeNode Clone() {
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                var mimePart = new MimePart();
                this.CopyTo(mimePart);
                return mimePart;
            }
        }

        public override sealed void CopyTo(object destination) {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                if (destination == null)
                    throw new System.ArgumentNullException(nameof(destination));
                if (destination == this)
                    return;
                var dstPart = destination as MimePart;
                if (dstPart == null)
                    throw new System.ArgumentException(Resources.Strings.CantCopyToDifferentObjectType);
                using (ThreadAccessGuard.EnterPublic(dstPart.accessToken)) {
                    byte[] scratchBuffer = null;
                    var dstStorage = new Internal.TemporaryDataStorage();
                    using (System.IO.Stream dstStream = dstStorage.OpenWriteStream(true))
                        this.CopyPartTo(false, dstPart, dstStorage, dstStream, 0L, ref scratchBuffer);
                    dstStorage.Release();
                    dstPart.SetDirty();
                }
            }
        }

        public long WriteTo(System.IO.Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter) {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                if (stream == null)
                    throw new System.ArgumentNullException(nameof(stream));
                if (encodingOptions == null)
                    encodingOptions = this.GetDocumentEncodingOptions();
                byte[] scratchBuffer = null;
                var currentLineLength = new MimeStringLength(0);
                return this.WriteTo(stream, encodingOptions, filter, ref currentLineLength, ref scratchBuffer);
            }
        }

        internal bool IsProtectedHeader(string headerName) {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                if (!string.IsNullOrEmpty(headerName) && this.Headers.FindFirst("DKIM-Signature") != null) {
                    if (protectedHeaders == null)
                        this.PopulateProtectedHeaders();
                    foreach (var str in protectedHeaders) {
                        if (headerName.Equals(str, System.StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                }
                return false;
            }
        }

        private void PopulateProtectedHeaders() {
            protectedHeaders = new System.Collections.Generic.List<string>();
            protectedHeaders.Add("DKIM-Signature");
            var refHeader = this.Headers.FindFirst("DKIM-Signature");
            var headerDecodingOptions = this.GetHeaderDecodingOptions();
            for (; refHeader != null; refHeader = this.Headers.FindNext(refHeader)) {
                var valueParser1 = new ValueParser(refHeader.Lines, headerDecodingOptions.AllowUTF8);
                var phrase = new MimeStringList();
                var handleISO2022 = true;
                byte num1;
                do {
                    valueParser1.ParseCFWS(false, ref phrase, handleISO2022);
                    num1 = valueParser1.ParseGet();
                    if (num1 != 59) {
                        if (num1 != 0)
                            valueParser1.ParseUnget();
                        else
                            break;
                    }
                    valueParser1.ParseCFWS(false, ref phrase, handleISO2022);
                    var mimeString1 = valueParser1.ParseToken();
                    if (mimeString1.Length == 0 || mimeString1.Length >= 128)
                        valueParser1.ParseSkipToNextDelimiterByte(59);
                    else {
                        valueParser1.ParseCFWS(false, ref phrase, handleISO2022);
                        num1 = valueParser1.ParseGet();
                        switch (num1) {
                            case 61:
                                valueParser1.ParseCFWS(false, ref phrase, handleISO2022);
                                var lines = new MimeStringList();
                                var goodValue = false;
                                valueParser1.ParseParameterValue(ref lines, ref goodValue, handleISO2022);
                                if (Header.NormalizeString(mimeString1.Data, mimeString1.Offset, mimeString1.Length, false)
                                          .Equals("h", System.StringComparison.OrdinalIgnoreCase)) {
                                    var valueParser2 = new ValueParser(lines, false);
                                    byte num2;
                                    do {
                                        valueParser2.ParseCFWS(false, ref phrase, handleISO2022);
                                        num2 = valueParser2.ParseGet();
                                        if (num2 != 58) {
                                            if (num2 != 0)
                                                valueParser2.ParseUnget();
                                            else
                                                break;
                                        }
                                        valueParser2.ParseCFWS(false, ref phrase, handleISO2022);
                                        var mimeString2 = valueParser2.ParseToken();
                                        if (mimeString2.Length == 0 || mimeString2.Length >= 128)
                                            valueParser2.ParseSkipToNextDelimiterByte(58);
                                        else
                                            protectedHeaders.Add(Header.NormalizeString(mimeString2.Data, mimeString2.Offset, mimeString2.Length, false));
                                    } while (num2 != 0);
                                }
                                break;
                            case 0:
                                break;
                            default:
                                valueParser1.ParseUnget();
                                break;
                        }
                    }
                } while (num1 != 0);
            }
        }

        internal static byte[] GetBoundary(ContentTypeHeader contentType) {
            if (contentType == null || !contentType.IsMultipart)
                return null;
            var mimeParameter = contentType["boundary"];
            if (mimeParameter == null) {
                mimeParameter = new MimeParameter("boundary");
                contentType.InternalAppendChild(mimeParameter);
                mimeParameter.RawValue = ContentTypeHeader.CreateBoundary();
            }
            var rawValue = mimeParameter.RawValue;
            var num = rawValue != null ? rawValue.Length : 0;
            if (num == 0 || 70 < num)
                mimeParameter.RawValue = ContentTypeHeader.CreateBoundary();
            var numArray = new byte[MimeString.CRLF2Dashes.Length + rawValue.Length + MimeString.CrLf.Length];
            var dstOffset1 = 0;
            System.Buffer.BlockCopy(MimeString.CRLF2Dashes, 0, numArray, dstOffset1, MimeString.CRLF2Dashes.Length);
            var length = MimeString.CRLF2Dashes.Length;
            System.Buffer.BlockCopy(rawValue, 0, numArray, length, rawValue.Length);
            var dstOffset2 = length + rawValue.Length;
            System.Buffer.BlockCopy(MimeString.CrLf, 0, numArray, dstOffset2, MimeString.CrLf.Length);
            return numArray;
        }

        internal static ContentTransferEncoding GetEncodingType(MimeString str) {
            if (str.Length == 0)
                return ContentTransferEncoding.Unknown;
            return encoding_map.Where(map => str.CompareEqI(map.Name))
                               .Select(map => map.Type)
                               .DefaultIfEmpty(ContentTransferEncoding.Unknown)
                               .First();
        }

        internal static byte[] GetEncodingName(ContentTransferEncoding encoding) {
            for (var index = 0; index < encoding_map.Length; ++index) {
                if (encoding_map[index].Type == encoding)
                    return encoding_map[index].Name;
            }
            return null;
        }

        internal static bool IsEqualContentTransferEncoding(ContentTransferEncoding cte1, ContentTransferEncoding cte2) {
            if (cte1 == cte2)
                return true;
            if (MimePart.EncodingIsTransparent(cte1))
                return MimePart.EncodingIsTransparent(cte2);
            return false;
        }

        internal static bool EncodingIsTransparent(ContentTransferEncoding cte) {
            if (cte != ContentTransferEncoding.Binary && cte != ContentTransferEncoding.SevenBit)
                return cte == ContentTransferEncoding.EightBit;
            return true;
        }

        internal static Schema.Mime.Encoders.ByteEncoder CreateEncoder(System.IO.Stream stream, ContentTransferEncoding encoding) {
            switch (encoding) {
                case ContentTransferEncoding.QuotedPrintable:
                    return new Schema.Mime.Encoders.QPEncoder();
                case ContentTransferEncoding.Base64:
                    return new Schema.Mime.Encoders.Base64Encoder();
                case ContentTransferEncoding.UUEncode:
                    return new Schema.Mime.Encoders.UUEncoder();
                case ContentTransferEncoding.BinHex:
                    return new Schema.Mime.Encoders.BinHexEncoder(
                        new Schema.Mime.Encoders.MacBinaryHeader {
                            DataForkLength = stream.Length,
                            FileName = "binhex.dat"
                        });
                default:
                    return null;
            }
        }

        internal static Schema.Mime.Encoders.ByteEncoder CreateDecoder(ContentTransferEncoding encoding) {
            switch (encoding) {
                case ContentTransferEncoding.QuotedPrintable:
                    return new Schema.Mime.Encoders.QPDecoder();
                case ContentTransferEncoding.Base64:
                    return new Schema.Mime.Encoders.Base64Decoder();
                case ContentTransferEncoding.UUEncode:
                    return new Schema.Mime.Encoders.UUDecoder();
                case ContentTransferEncoding.BinHex:
                    return new Schema.Mime.Encoders.BinHexDecoder(true);
                default:
                    return null;
            }
        }

        internal static long CopyStorageToStream(
            Internal.DataStorage srcStorage, long start, long end, LineTerminationState srcLineTermination, System.IO.Stream destStream, ref byte[] scratchBuffer, ref LineTerminationState lineTermination) {
            var num1 = 0L;
            if (lineTermination == LineTerminationState.NotInteresting || srcLineTermination != LineTerminationState.Unknown) {
                CountingWriteStream countingWriteStream = null;
                if ((System.IO.Stream.Null == destStream || (countingWriteStream = destStream as CountingWriteStream) != null && countingWriteStream.IsCountingOnly) && end != long.MaxValue) {
                    var num2 = end - start;
                    if (countingWriteStream != null)
                        countingWriteStream.Add(num2);
                    if (lineTermination != LineTerminationState.NotInteresting)
                        lineTermination = srcLineTermination;
                    return num2;
                }
                var num3 = srcStorage.CopyContentToStream(start, end, destStream, ref scratchBuffer);
                if (lineTermination != LineTerminationState.NotInteresting)
                    lineTermination = srcLineTermination;
                return num3;
            }
            if (scratchBuffer == null || scratchBuffer.Length < 16384)
                scratchBuffer = new byte[16384];
            using (var stream = srcStorage.OpenReadStream(start, end)) {
                while (true) {
                    var count = stream.Read(scratchBuffer, 0, scratchBuffer.Length);
                    if (count != 0) {
                        if (lineTermination != LineTerminationState.NotInteresting)
                            lineTermination = MimeCommon.AdvanceLineTerminationState(lineTermination, scratchBuffer, 0, count);
                        destStream.Write(scratchBuffer, 0, count);
                        num1 += count;
                    } else
                        break;
                }
            }
            return num1;
        }

        internal override void SetDirty() {
            this.ThrowIfDisposed();
            this.ThrowIfReadOnly("MimePart.SetDirty");
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                boundary = null;
                var mimePart1 = this;
                var flag = false;
                MimePart mimePart2;
                do {
                    if (flag) {
                        mimePart1.SetStorageImpl(null, 0L, 0L);
                        mimePart1.ContentPersisted = false;
                        mimePart1.ContentDirty = true;
                    }
                    mimePart1.IncrementVersion();
                    flag = true;
                    mimePart2 = mimePart1;
                    mimePart1 = mimePart1.Parent as MimePart;
                } while (mimePart1 != null);
                if (mimePart2.ParentDocument == null)
                    return;
                mimePart2.ParentDocument.IncrementVersion();
            }
        }

        internal override void ChildRemoved(MimeNode oldChild) {
            this.ThrowIfDisposed();
            this.ThrowIfReadOnly("MimePart.ChildRemoved");
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                if (dataStorage == null)
                    return;
                this.SetStorageImpl(null, 0L, 0L);
                this.ContentPersisted = false;
                this.ContentDirty = true;
            }
        }

        internal override void RemoveAllUnparsed() {
            this.ThrowIfDisposed();
            this.ThrowIfReadOnly("MimePart.RemoveAllUnparsed");
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                if (dataStorage == null || !this.IsEmbeddedMessage)
                    return;
                this.SetStorageImpl(null, 0L, 0L);
                this.ContentPersisted = false;
                this.ContentDirty = true;
            }
        }

        internal override MimeNode ParseNextChild() {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                if (this.InternalLastChild != null || dataStorage == null || !this.IsEmbeddedMessage)
                    return null;
                this.ThrowIfReadOnly("MimePart.ParseNextChild");
                MimeDocument mimeDocument = null;
                MimeDocument document;
                MimeNode treeRoot;
                this.GetMimeDocumentOrTreeRoot(out document, out treeRoot);
                try {
                    if (document == null) {
                        mimeDocument = new MimeDocument();
                        document = mimeDocument;
                    }
                    document.BuildEmbeddedDom((MimePart) treeRoot);
                    return this.InternalLastChild;
                } finally {
                    if (mimeDocument != null)
                        mimeDocument.Dispose();
                }
            }
        }

        internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild) {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                var mimePart = newChild as MimePart;
                if (mimePart == null)
                    throw new System.ArgumentException(Resources.Strings.NewChildIsNotAPart);
                MimeNode mimeNode = this;
                while (mimeNode != newChild) {
                    mimeNode = mimeNode.Parent;
                    if (mimeNode == null) {
                        this.ThrowIfReadOnly("MimePart.ValidateNewChild");
                        if (mimePart.parentDocument != null) {
                            mimePart.parentDocument.RootPart = new MimePart();
                            mimePart.parentDocument = null;
                        }
                        if (this.IsEmbeddedMessage && this.InternalLastChild != null) {
                            this.InternalRemoveChild(this.InternalLastChild);
                            refChild = null;
                        }
                        if (dataStorage != null) {
                            this.SetStorageImpl(null, 0L, 0L);
                            this.ContentPersisted = false;
                            this.ContentDirty = true;
                        }
                        return refChild;
                    }
                }
                throw new System.ArgumentException(Resources.Strings.ThisPartBelongsToSubtreeOfNewChild);
            }
        }

        internal void CopyPartTo(bool signedOrProtectedContent, MimePart dstPart, Internal.TemporaryDataStorage dstStorage, System.IO.Stream dstStream, long position, ref byte[] scratchBuffer) {
            using (ThreadAccessGuard.EnterPublic(accessToken))
            using (ThreadAccessGuard.EnterPublic(dstPart.accessToken)) {
                var num1 = 0L;
                MimePart mimePart = null;
                using (var enumerator = this.Subtree.GetEnumerator(SubtreeEnumerationOptions.IncludeEmbeddedMessages | SubtreeEnumerationOptions.RevisitParent, false)) {
                    while (enumerator.MoveNext()) {
                        if (enumerator.FirstVisit) {
                            if (enumerator.Depth != 0)
                                dstPart = new MimePart();
                            else if (signedOrProtectedContent) {
                                var dataStorage = enumerator.Current.dataStorage;
                                var num2 = enumerator.Current._storageInfo.DataStart;
                                num1 = position - num2;
                                mimePart = dstPart;
                                continue;
                            }
                            if (signedOrProtectedContent && enumerator.Current.dataStorage != null) {
                                dstPart.SetStorageImpl(
                                    dstStorage,
                                    num1 + enumerator.Current._storageInfo.DataStart,
                                    num1 + enumerator.Current._storageInfo.DataEnd,
                                    enumerator.Current._storageInfo.BodyOffset,
                                    enumerator.Current._storageInfo.BodyCte,
                                    enumerator.Current.BodyLineTermination);
                            } else if (enumerator.Current.IsSignedContent || enumerator.Current.IsProtectedContent) {
                                var num2 = enumerator.Current.dataStorage.CopyContentToStream(enumerator.Current._storageInfo.DataStart, enumerator.Current._storageInfo.DataEnd, dstStream, ref scratchBuffer);
                                dstPart.SetStorageImpl(dstStorage, position, position + num2, enumerator.Current._storageInfo.BodyOffset, enumerator.Current._storageInfo.BodyCte, enumerator.Current._storageInfo.BodyLineTermination);
                                if (!enumerator.LastVisit) {
                                    enumerator.Current.CopyPartTo(true, dstPart, dstStorage, null, position, ref scratchBuffer);
                                    enumerator.SkipChildren();
                                }
                                position += num2;
                            } else if (enumerator.Current.dataStorage == null || enumerator.Current.IsMultipart || enumerator.Current.IsEmbeddedMessage && !enumerator.LastVisit)
                                dstPart.SetStorageImpl(null, 0L, 0L);
                            else {
                                var num2 = enumerator.Current.dataStorage.CopyContentToStream(
                                    enumerator.Current._storageInfo.DataStart + enumerator.Current._storageInfo.BodyOffset,
                                    enumerator.Current._storageInfo.DataEnd,
                                    dstStream,
                                    ref scratchBuffer);
                                dstPart.SetStorageImpl(dstStorage, position, position + num2, 0L, enumerator.Current._storageInfo.BodyCte, enumerator.Current._storageInfo.BodyLineTermination);
                                position += num2;
                            }
                            dstPart.contentDirty = enumerator.Depth == 0 && !signedOrProtectedContent;
                            enumerator.Current.headers.CopyTo(dstPart.headers);
                            if (!signedOrProtectedContent && !enumerator.Current.IsSignedContent && !enumerator.Current.IsProtectedContent) {
                                var contentTypeHeader = dstPart.headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
                                if (contentTypeHeader != null && contentTypeHeader.IsMultipart) {
                                    var mimeParameter = contentTypeHeader["boundary"];
                                    if (mimeParameter != null)
                                        mimeParameter.RawValue = ContentTypeHeader.CreateBoundary();
                                }
                            }
                            if (mimePart != null) {
                                if (mimePart.IsEmbeddedMessage && mimePart.dataStorage != null && mimePart.InternalLastChild == null)
                                    mimePart.InternalInsertAfter(dstPart, null);
                                else
                                    mimePart.InternalAppendChild(dstPart);
                            }
                            if (!enumerator.LastVisit)
                                mimePart = dstPart;
                        } else if (enumerator.LastVisit && mimePart != null)
                            mimePart = mimePart.Parent as MimePart;
                    }
                }
            }
        }

        internal override long WriteTo(System.IO.Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer) {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                var num1 = 0L;
                CountingWriteStream countingWriteStream1 = null;
                CountingWriteStream countingWriteStream2 = null;
                var num2 = 0L;
                if (filter != null) {
                    countingWriteStream1 = stream as CountingWriteStream;
                    if (countingWriteStream1 == null) {
                        countingWriteStream2 = new CountingWriteStream(stream);
                        countingWriteStream1 = countingWriteStream2;
                        stream = countingWriteStream1;
                    }
                    num2 = countingWriteStream1.Count;
                }
                byte[] buffer = null;
                var flag = true;
                var lineTermination = this.IsMultipart ? LineTerminationState.NotInteresting : LineTerminationState.CRLF;
                using (var enumerator = this.Subtree.GetEnumerator(SubtreeEnumerationOptions.IncludeEmbeddedMessages | SubtreeEnumerationOptions.RevisitParent, false)) {
                    while (enumerator.MoveNext()) {
                        if (enumerator.FirstVisit) {
                            if (filter != null && filter.FilterPart(enumerator.Current, stream))
                                enumerator.SkipChildren();
                            else {
                                if (buffer != null) {
                                    if (flag) {
                                        stream.Write(buffer, 0, buffer.Length);
                                        num1 += buffer.Length;
                                    } else {
                                        stream.Write(buffer, 2, buffer.Length - 2);
                                        num1 += buffer.Length - 2;
                                        flag = true;
                                    }
                                    if (LineTerminationState.NotInteresting != lineTermination)
                                        lineTermination = LineTerminationState.CRLF;
                                }
                                if (enumerator.Current.IsSignedContent) {
                                    num1 += MimePart.CopyStorageToStream(
                                        enumerator.Current.dataStorage,
                                        enumerator.Current._storageInfo.DataStart,
                                        enumerator.Current._storageInfo.DataEnd,
                                        enumerator.Current._storageInfo.BodyLineTermination,
                                        stream,
                                        ref scratchBuffer,
                                        ref lineTermination);
                                    if (filter != null)
                                        filter.ClosePart(enumerator.Current, stream);
                                    enumerator.SkipChildren();
                                } else {
                                    if (filter == null || !filter.FilterHeaderList(enumerator.Current.Headers, stream)) {
                                        num1 += enumerator.Current.Headers.WriteTo(stream, encodingOptions, filter, ref currentLineLength, ref scratchBuffer);
                                        flag = true;
                                    } else
                                        flag = false;
                                    if (filter != null && filter.FilterPartBody(enumerator.Current, stream)) {
                                        if (filter != null)
                                            filter.ClosePart(enumerator.Current, stream);
                                        enumerator.SkipChildren();
                                        flag = true;
                                    } else if (enumerator.Current.IsMultipart) {
                                        if (!enumerator.LastVisit)
                                            buffer = enumerator.Current.Boundary;
                                        else {
                                            if (flag) {
                                                stream.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
                                                num1 += MimeString.CrLf.Length;
                                            }
                                            if (filter != null)
                                                filter.ClosePart(enumerator.Current, stream);
                                        }
                                    } else if (enumerator.Current.IsEmbeddedMessage && !enumerator.LastVisit) {
                                        if (flag) {
                                            stream.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
                                            num1 += MimeString.CrLf.Length;
                                        }
                                        buffer = null;
                                    } else {
                                        if (flag) {
                                            stream.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
                                            num1 += MimeString.CrLf.Length;
                                        }
                                        if (enumerator.Current.ContentTransferEncoding == enumerator.Current._storageInfo.BodyCte) {
                                            if (enumerator.Current.dataStorage != null) {
                                                num1 += MimePart.CopyStorageToStream(
                                                    enumerator.Current.dataStorage,
                                                    enumerator.Current._storageInfo.DataStart + enumerator.Current._storageInfo.BodyOffset,
                                                    enumerator.Current._storageInfo.DataEnd,
                                                    enumerator.Current._storageInfo.BodyLineTermination,
                                                    stream,
                                                    ref scratchBuffer,
                                                    ref lineTermination);
                                            }
                                        } else {
                                            if (scratchBuffer == null || scratchBuffer.Length < 16384)
                                                scratchBuffer = new byte[16384];
                                            using (var contentReadStream = enumerator.Current.GetRawContentReadStream()) {
                                                while (true) {
                                                    var count = contentReadStream.Read(scratchBuffer, 0, scratchBuffer.Length);
                                                    if (count != 0) {
                                                        if (lineTermination != LineTerminationState.NotInteresting)
                                                            lineTermination = MimeCommon.AdvanceLineTerminationState(lineTermination, scratchBuffer, 0, count);
                                                        stream.Write(scratchBuffer, 0, count);
                                                        num1 += count;
                                                    } else
                                                        break;
                                                }
                                            }
                                        }
                                        if (filter != null)
                                            filter.ClosePart(enumerator.Current, stream);
                                        if (!enumerator.LastVisit)
                                            enumerator.SkipChildren();
                                    }
                                }
                            }
                        } else if (enumerator.LastVisit) {
                            if (buffer != null) {
                                stream.Write(buffer, 0, buffer.Length - 2);
                                num1 += buffer.Length - 2;
                                stream.Write(MimeString.TwoDashesCRLF, 0, MimeString.TwoDashesCRLF.Length);
                                num1 += MimeString.TwoDashesCRLF.Length;
                                if (LineTerminationState.NotInteresting != lineTermination)
                                    lineTermination = LineTerminationState.CRLF;
                            }
                            buffer = enumerator.Current.Parent == null ? null : ((MimePart) enumerator.Current.Parent).Boundary;
                            if (filter != null)
                                filter.ClosePart(enumerator.Current, stream);
                        }
                    }
                }
                if (!this.IsSignedContent) {
                    switch (lineTermination) {
                        case LineTerminationState.CR:
                            stream.Write(MimeString.CrLf, 1, 1);
                            ++num1;
                            break;
                        case LineTerminationState.Other:
                            stream.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
                            num1 += MimeString.CrLf.Length;
                            break;
                    }
                }
                if (countingWriteStream1 != null) {
                    num1 = countingWriteStream1.Count - num2;
                    if (countingWriteStream2 != null)
                        countingWriteStream2.Dispose();
                }
                return num1;
            }
        }

        internal void SetStorage(Internal.DataStorage storage, long dataStart, long dataEnd) {
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                this.SetStorageImpl(storage, dataStart, dataEnd);
                this.ContentPersisted = false;
                this.ContentDirty = true;
                this.SetDirty();
            }
        }

        internal void SetStorage(Internal.DataStorage storage, long dataStart, long dataEnd, long bodyOffset, ContentTransferEncoding bodyCte, LineTerminationState bodyLineTermination) {
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                this.SetStorageImpl(storage, dataStart, dataEnd, bodyOffset, bodyCte, bodyLineTermination);
                this.ContentPersisted = false;
                this.ContentDirty = true;
                this.SetDirty();
            }
        }

        internal void SetStorageImpl(Internal.DataStorage storage, long dataStart, long dataEnd) {
            using (ThreadAccessGuard.EnterPublic(accessToken))
                this.SetStorageImpl(storage, dataStart, dataEnd, 0L, ContentTransferEncoding.Binary, LineTerminationState.Unknown);
        }

        internal void SetStorageImpl(Internal.DataStorage storage, long dataStart, long dataEnd, long bodyOffset, ContentTransferEncoding bodyCte, LineTerminationState bodyLineTermination) {
            this.ThrowIfDisposed();
            this.ThrowIfReadOnly("MimePart.SetStorageImpl");
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                if (storage != null) {
                    storage.AddRef();
                    storage.SetReadOnly(false);
                }
                if (dataStorage != null)
                    dataStorage.Release();
                dataStorage = storage;
                _storageInfo.DataStart = dataStart;
                _storageInfo.DataEnd = dataEnd;
                _storageInfo.BodyOffset = bodyOffset;
                _storageInfo.BodyCte = bodyCte;
                _storageInfo.BodyLineTermination = bodyLineTermination;
            }
        }

        internal void SetDeferredStorageImpl(Internal.DataStorage storage, long dataStart, long dataEnd) {
            using (ThreadAccessGuard.EnterPublic(accessToken))
                this.SetDeferredStorageImpl(storage, dataStart, dataEnd, 0L, ContentTransferEncoding.Binary, LineTerminationState.Unknown);
        }

        internal void SetDeferredStorageImpl(Internal.DataStorage storage, long dataStart, long dataEnd, long bodyOffset, ContentTransferEncoding bodyCte, LineTerminationState bodyLineTermination) {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                if (this.IsReadOnly) {
                    if (storage != null)
                        storage.AddRef();
                    var dataStorageInfo = new DataStorageInfo();
                    dataStorageInfo.DataStart = dataStart;
                    dataStorageInfo.DataEnd = dataEnd;
                    dataStorageInfo.BodyOffset = bodyOffset;
                    dataStorageInfo.BodyCte = bodyCte;
                    dataStorageInfo.BodyLineTermination = bodyLineTermination;
                    lock (deferredStorageLock) {
                        if (deferredStorage != null)
                            deferredStorage.Release();
                        deferredStorage = storage;
                        deferredStorageInfo = dataStorageInfo;
                    }
                } else
                    this.SetStorageImpl(storage, dataStart, dataEnd, bodyOffset, bodyCte, bodyLineTermination);
            }
        }

        internal void SetReadOnlyInternal(bool makeReadOnly) {
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                if (!makeReadOnly && deferredStorageInfo != null) {
                    if (dataStorage != null)
                        dataStorage.Release();
                    dataStorage = deferredStorage;
                    _storageInfo = deferredStorageInfo;
                    deferredStorage = null;
                    deferredStorageInfo = null;
                }
                if (dataStorage == null)
                    return;
                this.Storage.SetReadOnly(makeReadOnly);
            }
        }

        internal Globalization.Charset FindMimeTreeCharset() {
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                Globalization.Charset charset = null;
                var mimePart = this;
                while (!mimePart.IsEmbeddedMessage && mimePart.FirstChild != null)
                    mimePart = (MimePart) mimePart.FirstChild;
                var complexHeader = mimePart.Headers.FindFirst(HeaderId.ContentType) as ComplexHeader;
                if (complexHeader != null) {
                    var mimeParameter = complexHeader["charset"];
                    if (mimeParameter != null) {
                        var rawValue = mimeParameter.RawValue;
                        if (rawValue != null) {
                            var name = Internal.ByteString.BytesToString(rawValue, false);
                            if (name != null && Globalization.Charset.TryGetCharset(name, out charset) && charset.AsciiSupport < Globalization.CodePageAsciiSupport.Fine)
                                charset = charset.Culture.MimeCharset;
                        }
                    }
                }
                return charset;
            }
        }

        private void IncrementVersion() {
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                this.Version = int.MaxValue == this.Version ? 1 : this.Version + 1;
                protectedHeaders = null;
            }
        }

        internal void UpdateTransferEncoding(ContentTransferEncoding transferEncoding) {
            this.ThrowIfDisposed();
            this.ThrowIfReadOnly("MimePart.UpdateTransferEncoding");
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                var encodingName = MimePart.GetEncodingName(transferEncoding);
                if (encodingName == null)
                    throw new System.ArgumentException("Transfer encoding is unknown or not supported", nameof(transferEncoding));
                var first = headers.FindFirst(HeaderId.ContentTransferEncoding);
                if (first == null) {
                    first = Header.Create(HeaderId.ContentTransferEncoding);
                    headers.InternalAppendChild(first);
                } else if (ContentTransferEncoding.SevenBit == transferEncoding) {
                    first.RemoveFromParent();
                    return;
                }
                first.RawValue = encodingName;
            }
        }

        private void ThrowIfDisposed() {
            if (isDisposed)
                throw new System.ObjectDisposedException("MimePart");
        }

        private static readonly EncodingEntry[] encoding_map = new EncodingEntry[9] {
            new EncodingEntry(MimeString.Base64, ContentTransferEncoding.Base64),
            new EncodingEntry(MimeString.QuotedPrintable, ContentTransferEncoding.QuotedPrintable),
            new EncodingEntry(MimeString.Encoding7Bit, ContentTransferEncoding.SevenBit),
            new EncodingEntry(MimeString.Encoding8Bit, ContentTransferEncoding.EightBit),
            new EncodingEntry(MimeString.Binary, ContentTransferEncoding.Binary),
            new EncodingEntry(MimeString.Uuencode, ContentTransferEncoding.UUEncode),
            new EncodingEntry(MimeString.MacBinhex, ContentTransferEncoding.BinHex),
            new EncodingEntry(MimeString.XUuencode, ContentTransferEncoding.UUEncode),
            new EncodingEntry(MimeString.Uue, ContentTransferEncoding.UUEncode)
        };

        private readonly MimePartThreadAccessToken accessToken;
        private readonly object deferredStorageLock = new object();
        private DataStorageInfo _storageInfo = new DataStorageInfo();
        private byte[] boundary;
        private int cacheMapStamp;
        private bool contentDirty;
        private bool contentPersisted;
        private Internal.DataStorage dataStorage;
        private Internal.DataStorage deferredStorage;
        private DataStorageInfo deferredStorageInfo;
        private HeaderList headers;
        private bool isDisposed;
        private MimeDocument parentDocument;
        private System.Collections.Generic.List<string> protectedHeaders;

    }

}