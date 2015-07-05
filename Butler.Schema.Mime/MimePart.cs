using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Butler.Schema.Globalization;
using Butler.Schema.Internal;
using Butler.Schema.Mime.Encoders;
using Butler.Schema.Resources;

namespace Butler.Schema.Mime {

    public partial class MimePart : MimeNode, IDisposable, IEnumerable<MimePart> {

        public MimePart() {
            _accessToken = new MimePartThreadAccessToken(this);
            headers = new HeaderList(this);
        }

        public MimePart(string contentType)
            : this() {
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));
            using (ThreadAccessGuard.EnterPublic(_accessToken))
                headers.InternalAppendChild(new ContentTypeHeader(contentType));
        }

        public MimePart(string contentType, string transferEncoding, Stream contentStream, CachingMode cachingMode)
            : this(contentType) {
            using (ThreadAccessGuard.EnterPublic(_accessToken))
                this.SetContentStream(transferEncoding, contentStream, cachingMode);
        }

        public MimePart(string contentType, ContentTransferEncoding transferEncoding, Stream contentStream, CachingMode cachingMode)
            : this(contentType) {
            using (ThreadAccessGuard.EnterPublic(_accessToken))
                this.SetContentStream(transferEncoding, contentStream, cachingMode);
        }

        public HeaderList Headers {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(_accessToken))
                    return headers;
            }
        }

        public string ContentType {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                    var contentTypeHeader = headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
                    if (contentTypeHeader != null)
                        return contentTypeHeader.Value;
                    var mimePart = this.Parent as MimePart;
                    var parentContentTypeHeader = mimePart?.Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
                    if (parentContentTypeHeader != null && parentContentTypeHeader.Value == "multipart/digest")
                        return "message/rfc822";
                    return "text/plain";
                }
            }
        }

        public string ContentDescription {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                    return headers.FindFirst(HeaderId.ContentDescription)?.Value;
                }
            }
        }

        public string ContentId {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                    return headers.FindFirst(HeaderId.ContentDescription)?.Value;
                }
            }
        }

        public bool IsMultipart {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                    var contentTypeHeader = headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
                    return contentTypeHeader != null && contentTypeHeader.IsMultipart;
                }
            }
        }

        public bool IsEmbeddedMessage {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                    var contentTypeHeader = headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
                    return contentTypeHeader != null && contentTypeHeader.IsEmbeddedMessage;
                }
            }
        }

        public bool RequiresSMTPUTF8 {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                    return this.GetSubtree(SubtreeEnumerationOptions.IncludeEmbeddedMessages, true)
                               .SelectMany(p => p.Headers)
                               .Any(header => header.RequiresSMTPUTF8);
                    using (var enumerator = this.Subtree.GetEnumerator(SubtreeEnumerationOptions.IncludeEmbeddedMessages, true)) {
                        while (enumerator.MoveNext()) {
                            if (enumerator.Current.Headers.Any(header => header.RequiresSMTPUTF8))
                                return true;
                        }
                    }
                    return false;
                }
            }
        }

        internal bool IsAnyMessage {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                    var contentTypeHeader = headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
                    return contentTypeHeader != null && contentTypeHeader.IsAnyMessage;
                }
            }
        }

        internal int CacheMapStamp {
            get {
                using (ThreadAccessGuard.EnterPublic(_accessToken))
                    return _cacheMapStamp;
            }
            set {
                using (ThreadAccessGuard.EnterPublic(_accessToken))
                    _cacheMapStamp = value;
            }
        }

        public ContentTransferEncoding ContentTransferEncoding {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                    var first = headers.FindFirst(HeaderId.ContentTransferEncoding);
                    if (first != null && first.FirstRawToken.Length != 0)
                        return MimePart.GetEncodingType(first.FirstRawToken);
                    return ContentTransferEncoding.SevenBit;
                }
            }
        }

        public int Version { get; private set; }
        internal ObjectThreadAccessToken AccessToken => _accessToken;

        internal ObjectThreadAccessToken ParentAccessToken {
            get {
                MimeNode treeRoot;
                return MimeNode.GetParentDocument(this, out treeRoot)?.AccessToken;
            }
        }

        public PartSubtree Subtree {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(_accessToken))
                    return new PartSubtree(this);
            }
        }

        internal DataStorage Storage {
            get {
                using (ThreadAccessGuard.EnterPublic(_accessToken))
                    return _dataStorage;
            }
        }

        internal long DataStart {
            get {
                using (ThreadAccessGuard.EnterPublic(_accessToken))
                    return _storageInfo.DataStart;
            }
        }

        internal long DataEnd {
            get {
                using (ThreadAccessGuard.EnterPublic(_accessToken))
                    return _storageInfo.DataEnd;
            }
        }

        internal long DataLength {
            get {
                using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                    if (_dataStorage == null)
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
                using (ThreadAccessGuard.EnterPublic(_accessToken))
                    return _storageInfo.BodyOffset;
            }
        }

        internal ContentTransferEncoding BodyCte {
            get {
                using (ThreadAccessGuard.EnterPublic(_accessToken))
                    return _storageInfo.BodyCte;
            }
        }

        internal LineTerminationState BodyLineTermination {
            get {
                using (ThreadAccessGuard.EnterPublic(_accessToken))
                    return _storageInfo.BodyLineTermination;
            }
        }

        internal byte[] Boundary {
            get {
                using (ThreadAccessGuard.EnterPublic(_accessToken))
                    return _boundary ?? (_boundary = MimePart.GetBoundary(this.Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader));
            }
        }

        internal bool IsSignedContent {
            get {
                using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                    if (this.Parent == null || this != this.Parent.FirstChild || (_dataStorage == null || 0L == _storageInfo.BodyOffset) || this.Version != 0)
                        return false;
                    var contentTypeHeader = (this.Parent as MimePart).Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
                    return contentTypeHeader != null && contentTypeHeader.Value == "multipart/signed";
                }
            }
        }

        internal bool IsProtectedContent {
            get {
                using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                    if (this.Parent != null && _dataStorage != null && (0L != _storageInfo.BodyOffset && this.Version == 0))
                        return (this.Parent as MimePart).Headers.FindFirst("DKIM-Signature") != null;
                    return false;
                }
            }
        }

        internal MimeDocument ParentDocument {
            get {
                return _parentDocument;
            }
            set {
                this.ThrowIfReadOnly("MimePart.set_ParentDocument");
                using (ThreadAccessGuard.EnterPublic(_accessToken))
                    _parentDocument = value;
            }
        }

        internal bool ContentDirty {
            get {
                using (ThreadAccessGuard.EnterPublic(_accessToken))
                    return _contentDirty;
            }
            set {
                using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                    if (_contentDirty != value)
                        this.ThrowIfReadOnly("MimePart.set_ContentDirty");
                    _contentDirty = value;
                }
            }
        }

        internal bool ContentPersisted {
            get {
                using (ThreadAccessGuard.EnterPublic(_accessToken))
                    return _contentPersisted && _storageInfo.BodyCte == this.ContentTransferEncoding;
            }
            set {
                using (ThreadAccessGuard.EnterPublic(_accessToken))
                    _contentPersisted = value;
            }
        }

        internal DataStorage DeferredStorage {
            get {
                using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                    if (!this.IsReadOnly)
                        return this.Storage;
                    lock (_deferredStorageLock)
                        return _deferredStorageInfo != null ? _deferredStorage : _dataStorage;
                }
            }
        }

        internal long DeferredDataStart {
            get {
                using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                    if (!this.IsReadOnly)
                        return this.DataStart;
                    lock (_deferredStorageLock)
                        return _deferredStorageInfo?.DataStart ?? _storageInfo.DataStart;
                }
            }
        }

        internal long DeferredDataEnd {
            get {
                using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                    if (!this.IsReadOnly)
                        return this.DataEnd;
                    lock (_deferredStorageLock)
                        return _deferredStorageInfo?.DataEnd ?? _storageInfo.DataEnd;
                }
            }
        }

        internal long DeferredDataLength {
            get {
                using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                    if (!this.IsReadOnly)
                        return this.DataLength;
                    DataStorage storage;
                    DataStorageInfo storageInfo;
                    lock (_deferredStorageLock) {
                        storage = _deferredStorage;
                        storageInfo = _deferredStorageInfo;
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
                using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                    if (!this.IsReadOnly)
                        return this.BodyOffset;
                    lock (_deferredStorageLock)
                        return _deferredStorageInfo?.BodyOffset ?? _storageInfo.BodyOffset;
                }
            }
        }

        internal ContentTransferEncoding DeferredBodyCte {
            get {
                using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                    if (!this.IsReadOnly)
                        return this.BodyCte;
                    lock (_deferredStorageLock)
                        return _deferredStorageInfo?.BodyCte ?? _storageInfo.BodyCte;
                }
            }
        }

        internal LineTerminationState DeferredBodyLineTermination {
            get {
                using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                    if (!this.IsReadOnly)
                        return this.BodyLineTermination;
                    lock (_deferredStorageLock)
                        return _deferredStorageInfo?.BodyLineTermination ?? _storageInfo.BodyLineTermination;
                }
            }
        }

        public void Dispose() {
            this.Dispose(true);
        }

        public IEnumerator<MimePart> GetEnumerator() {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(_accessToken))
                return new Enumerator<MimePart>(this);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }

        internal IEnumerable<MimePart> GetSubtree(SubtreeEnumerationOptions options, bool includeUnparsed) {
            return this.Subtree.GetEnumerator(SubtreeEnumerationOptions.IncludeEmbeddedMessages, true)
                       .Enumerate();
        }

        public static MimePart LoadMime(string path, CachingMode cachingMode = CachingMode.Source) {
            return MimePart.LoadMime(File.OpenRead(path), cachingMode);
        }

        public static MimePart LoadMime(Stream stream, CachingMode cachingMode = CachingMode.Source) {
            var doc = new MimeDocument();
            return doc.Load(stream, cachingMode);
        }

        public Stream GetRawContentReadStream() {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(_accessToken))
                return this.GetRawContentReadStream(_dataStorage, _storageInfo);
        }

        internal Stream GetDeferredRawContentReadStream() {
            using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                if (!this.IsReadOnly)
                    return this.GetRawContentReadStream();
                DataStorage storage;
                DataStorageInfo storageInfo;
                lock (_deferredStorageLock) {
                    storage = _deferredStorage;
                    storageInfo = _deferredStorageInfo;
                }
                if (storageInfo == null)
                    return this.GetRawContentReadStream();
                return this.GetRawContentReadStream(storage, storageInfo);
            }
        }

        private Stream GetRawContentReadStream(DataStorage storage, DataStorageInfo storageInfo) {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPrivate(_accessToken)) {
                Stream result;
                if (!this.TryGetContentReadStream(storage, storageInfo, this.ContentTransferEncoding, out result))
                    throw new MimeException(Strings.CannotDecodeContentStream);
                return result;
            }
        }

        public Stream GetContentReadStream() {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                Stream result;
                if (!this.TryGetContentReadStream(_dataStorage, _storageInfo, ContentTransferEncoding.Binary, out result))
                    throw new MimeException(Strings.CannotDecodeContentStream);
                return result;
            }
        }

        public bool TryGetContentReadStream(out Stream result) {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(_accessToken))
                return this.TryGetContentReadStream(_dataStorage, _storageInfo, ContentTransferEncoding.Binary, out result);
        }

        private bool TryGetContentReadStream(DataStorage dataStorage, DataStorageInfo storageInfo, ContentTransferEncoding desiredCte, out Stream result) {
            using (ThreadAccessGuard.EnterPrivate(_accessToken)) {
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
                            result = DataStorage.NewEmptyReadStream();
                            flag = true;
                            return true;
                        }
                        var temporaryDataStorage = new TemporaryDataStorage();
                        try {
                            using (Stream stream = temporaryDataStorage.OpenWriteStream(true))
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
                            result = new EncoderStream(result, decoder, EncoderStreamAccess.Read, true);
                        }
                        if (!MimePart.EncodingIsTransparent(desiredCte)) {
                            var encoder = MimePart.CreateEncoder(result, desiredCte);
                            result = new EncoderStream(result, encoder, EncoderStreamAccess.Read, true);
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

        public Stream GetRawContentWriteStream() {
            this.ThrowIfDisposed();
            this.ThrowIfReadOnly("MimePart.GetRawContentWriteStream");
            using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                if (this.IsMultipart)
                    throw new NotSupportedException(Strings.ModifyingRawContentOfMultipartNotSupported);
                this.SetStorage(null, 0L, 0L);
                return new PartContentWriteStream(this, ContentTransferEncoding.Unknown);
            }
        }

        public Stream GetContentWriteStream(ContentTransferEncoding transferEncoding) {
            this.ThrowIfDisposed();
            this.ThrowIfReadOnly("MimePart.GetContentWriteStream");
            using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                if (this.IsMultipart)
                    throw new NotSupportedException(Strings.ModifyingRawContentOfMultipartNotSupported);
                this.UpdateTransferEncoding(transferEncoding);
                this.SetStorage(null, 0L, 0L);
                return new PartContentWriteStream(this, ContentTransferEncoding.Binary);
            }
        }

        public Stream GetContentWriteStream(string transferEncoding) {
            this.ThrowIfDisposed();
            this.ThrowIfReadOnly("MimePart.GetContentWriteStream");
            using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                if (transferEncoding == null)
                    throw new ArgumentNullException(nameof(transferEncoding));
                return this.GetContentWriteStream(MimePart.GetEncodingType(new MimeString(transferEncoding)));
            }
        }

        public void SetContentStream(string transferEncoding, Stream contentStream, CachingMode cachingMode) {
            this.ThrowIfDisposed();
            this.ThrowIfReadOnly("MimePart.SetContentStream");
            using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                var transferEncoding1 = ContentTransferEncoding.Unknown;
                if (transferEncoding != null) {
                    transferEncoding1 = MimePart.GetEncodingType(new MimeString(transferEncoding));
                    if (transferEncoding1 == ContentTransferEncoding.Unknown)
                        throw new ArgumentException("Transfer encoding is unknown or not supported", nameof(transferEncoding));
                }
                this.SetContentStream(transferEncoding1, contentStream, cachingMode);
            }
        }

        public void SetContentStream(ContentTransferEncoding transferEncoding, Stream contentStream, CachingMode cachingMode) {
            this.ThrowIfDisposed();
            this.ThrowIfReadOnly("MimePart.SetContentStream");
            using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                if (contentStream == null)
                    throw new ArgumentNullException(nameof(contentStream));
                if (!contentStream.CanRead)
                    throw new ArgumentException(Strings.StreamMustSupportRead, nameof(contentStream));
                if (this.IsMultipart)
                    throw new NotSupportedException(Strings.ModifyingRawContentOfMultipartNotSupported);
                var dataStart = 0L;
                var dataEnd = long.MaxValue;
                if (transferEncoding != ContentTransferEncoding.Unknown) {
                    this.UpdateTransferEncoding(transferEncoding);
                    transferEncoding = ContentTransferEncoding.Binary;
                }
                DataStorage storage;
                switch (cachingMode) {
                    case CachingMode.Copy:
                        var temporaryDataStorage = new TemporaryDataStorage();
                        storage = temporaryDataStorage;
                        using (Stream destStream = temporaryDataStorage.OpenWriteStream(true)) {
                            byte[] scratchBuffer = null;
                            dataEnd = DataStorage.CopyStreamToStream(contentStream, destStream, long.MaxValue, ref scratchBuffer);
                            break;
                        }
                    case CachingMode.Source:
                    case CachingMode.SourceTakeOwnership:
                        if (!contentStream.CanSeek)
                            throw new NotSupportedException(Strings.CachingModeSourceButStreamCannotSeek);
                        var streamOnDataStorage = contentStream as StreamOnDataStorage;
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
                        storage = new ReadableDataStorageOnStream(contentStream, cachingMode == CachingMode.SourceTakeOwnership);
                        break;
                    default:
                        throw new ArgumentException("Invalid Caching Mode value", nameof(cachingMode));
                }
                this.SetStorage(storage, dataStart, dataEnd, 0L, transferEncoding, LineTerminationState.Unknown);
                storage.Release();
            }
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing && !_isDisposed) {
                using (var enumerator = this.Subtree.GetEnumerator(SubtreeEnumerationOptions.IncludeEmbeddedMessages, false)) {
                    while (enumerator.MoveNext()) {
                        if (enumerator.Current._dataStorage != null) {
                            enumerator.Current._dataStorage.Release();
                            enumerator.Current._dataStorage = null;
                        }
                        if (enumerator.Current.headers != null)
                            enumerator.Current.headers.InternalDetachParent();
                        enumerator.Current.headers = null;
                        enumerator.Current._boundary = null;
                        enumerator.Current._parentDocument = null;
                        if (enumerator.Current._deferredStorage != null) {
                            enumerator.Current._deferredStorage.Release();
                            enumerator.Current._deferredStorage = null;
                            enumerator.Current._deferredStorageInfo = null;
                        }
                        enumerator.Current._isDisposed = true;
                        GC.SuppressFinalize(enumerator.Current);
                    }
                }
            } else
                _isDisposed = true;
        }

        //public new Enumerator<MimePart> GetEnumerator() {
        //    this.ThrowIfDisposed();
        //    using (ThreadAccessGuard.EnterPublic(accessToken))
        //        return new Enumerator<MimePart>(this);
        //}

        public override sealed MimeNode Clone() {
            using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                var mimePart = new MimePart();
                this.CopyTo(mimePart);
                return mimePart;
            }
        }

        public override sealed void CopyTo(object destination) {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                if (destination == null)
                    throw new ArgumentNullException(nameof(destination));
                if (destination == this)
                    return;
                var dstPart = destination as MimePart;
                if (dstPart == null)
                    throw new ArgumentException(Strings.CantCopyToDifferentObjectType);
                using (ThreadAccessGuard.EnterPublic(dstPart._accessToken)) {
                    byte[] scratchBuffer = null;
                    var dstStorage = new TemporaryDataStorage();
                    using (Stream dstStream = dstStorage.OpenWriteStream(true))
                        this.CopyPartTo(false, dstPart, dstStorage, dstStream, 0L, ref scratchBuffer);
                    dstStorage.Release();
                    dstPart.SetDirty();
                }
            }
        }

        public long WriteTo(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter) {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                if (stream == null)
                    throw new ArgumentNullException(nameof(stream));
                if (encodingOptions == null)
                    encodingOptions = this.GetDocumentEncodingOptions();
                byte[] scratchBuffer = null;
                var currentLineLength = new MimeStringLength(0);
                return this.WriteTo(stream, encodingOptions, filter, ref currentLineLength, ref scratchBuffer);
            }
        }

        internal bool IsProtectedHeader(string headerName) {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                if (!string.IsNullOrEmpty(headerName) && this.Headers.FindFirst("DKIM-Signature") != null) {
                    if (_protectedHeaders == null)
                        this.PopulateProtectedHeaders();
                    foreach (var str in _protectedHeaders) {
                        if (headerName.Equals(str, StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                }
                return false;
            }
        }

        private void PopulateProtectedHeaders() {
            _protectedHeaders = new List<string>();
            _protectedHeaders.Add("DKIM-Signature");
            var refHeader = this.Headers.FindFirst("DKIM-Signature");
            var headerDecodingOptions = this.GetHeaderDecodingOptions();
            for (; refHeader != null; refHeader = this.Headers.FindNext(refHeader)) {
                var valueParser1 = new ValueParser(refHeader.Lines, headerDecodingOptions.AllowUTF8);
                var phrase = new MimeStringList();
                const bool handleISO2022 = true;
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
                                          .Equals("h", StringComparison.OrdinalIgnoreCase)) {
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
                                            _protectedHeaders.Add(Header.NormalizeString(mimeString2.Data, mimeString2.Offset, mimeString2.Length, false));
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
            var num = rawValue?.Length ?? 0;
            if (num == 0 || 70 < num)
                mimeParameter.RawValue = ContentTypeHeader.CreateBoundary();
            var numArray = new byte[MimeString.CRLF2Dashes.Length + rawValue.Length + MimeString.CrLf.Length];
            const int dstOffset1 = 0;
            Buffer.BlockCopy(MimeString.CRLF2Dashes, 0, numArray, dstOffset1, MimeString.CRLF2Dashes.Length);
            var length = MimeString.CRLF2Dashes.Length;
            Buffer.BlockCopy(rawValue, 0, numArray, length, rawValue.Length);
            var dstOffset2 = length + rawValue.Length;
            Buffer.BlockCopy(MimeString.CrLf, 0, numArray, dstOffset2, MimeString.CrLf.Length);
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
            return MimePart.EncodingIsTransparent(cte1) && MimePart.EncodingIsTransparent(cte2);
        }

        internal static bool EncodingIsTransparent(ContentTransferEncoding cte) {
            if (cte != ContentTransferEncoding.Binary && cte != ContentTransferEncoding.SevenBit)
                return cte == ContentTransferEncoding.EightBit;
            return true;
        }

        internal static ByteEncoder CreateEncoder(Stream stream, ContentTransferEncoding encoding) {
            switch (encoding) {
                case ContentTransferEncoding.QuotedPrintable:
                    return new QPEncoder();
                case ContentTransferEncoding.Base64:
                    return new Base64Encoder();
                case ContentTransferEncoding.UUEncode:
                    return new UUEncoder();
                case ContentTransferEncoding.BinHex:
                    return new BinHexEncoder(
                        new MacBinaryHeader {
                            DataForkLength = stream.Length,
                            FileName = "binhex.dat"
                        });
                default:
                    return null;
            }
        }

        internal static ByteEncoder CreateDecoder(ContentTransferEncoding encoding) {
            switch (encoding) {
                case ContentTransferEncoding.QuotedPrintable:
                    return new QPDecoder();
                case ContentTransferEncoding.Base64:
                    return new Base64Decoder();
                case ContentTransferEncoding.UUEncode:
                    return new UUDecoder();
                case ContentTransferEncoding.BinHex:
                    return new BinHexDecoder(true);
                default:
                    return null;
            }
        }

        internal static long CopyStorageToStream(
            DataStorage srcStorage, long start, long end, LineTerminationState srcLineTermination, Stream destStream, ref byte[] scratchBuffer, ref LineTerminationState lineTermination) {
            var num1 = 0L;
            if (lineTermination == LineTerminationState.NotInteresting || srcLineTermination != LineTerminationState.Unknown) {
                CountingWriteStream countingWriteStream = null;
                if ((Stream.Null == destStream || (countingWriteStream = destStream as CountingWriteStream) != null && countingWriteStream.IsCountingOnly) && end != long.MaxValue) {
                    var num2 = end - start;
                    countingWriteStream?.Add(num2);
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
            using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                _boundary = null;
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
            using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                if (_dataStorage == null)
                    return;
                this.SetStorageImpl(null, 0L, 0L);
                this.ContentPersisted = false;
                this.ContentDirty = true;
            }
        }

        internal override void RemoveAllUnparsed() {
            this.ThrowIfDisposed();
            this.ThrowIfReadOnly("MimePart.RemoveAllUnparsed");
            using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                if (_dataStorage == null || !this.IsEmbeddedMessage)
                    return;
                this.SetStorageImpl(null, 0L, 0L);
                this.ContentPersisted = false;
                this.ContentDirty = true;
            }
        }

        internal override MimeNode ParseNextChild() {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                if (this.InternalLastChild != null || _dataStorage == null || !this.IsEmbeddedMessage)
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
            using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                var mimePart = newChild as MimePart;
                if (mimePart == null)
                    throw new ArgumentException(Strings.NewChildIsNotAPart);
                MimeNode mimeNode = this;
                while (mimeNode != newChild) {
                    mimeNode = mimeNode.Parent;
                    if (mimeNode == null) {
                        this.ThrowIfReadOnly("MimePart.ValidateNewChild");
                        if (mimePart._parentDocument != null) {
                            mimePart._parentDocument.RootPart = new MimePart();
                            mimePart._parentDocument = null;
                        }
                        if (this.IsEmbeddedMessage && this.InternalLastChild != null) {
                            this.InternalRemoveChild(this.InternalLastChild);
                            refChild = null;
                        }
                        if (_dataStorage != null) {
                            this.SetStorageImpl(null, 0L, 0L);
                            this.ContentPersisted = false;
                            this.ContentDirty = true;
                        }
                        return refChild;
                    }
                }
                throw new ArgumentException(Strings.ThisPartBelongsToSubtreeOfNewChild);
            }
        }

        internal void CopyPartTo(bool signedOrProtectedContent, MimePart dstPart, TemporaryDataStorage dstStorage, Stream dstStream, long position, ref byte[] scratchBuffer) {
            using (ThreadAccessGuard.EnterPublic(_accessToken))
            using (ThreadAccessGuard.EnterPublic(dstPart._accessToken)) {
                var num1 = 0L;
                MimePart mimePart = null;
                using (var enumerator = this.Subtree.GetEnumerator(SubtreeEnumerationOptions.IncludeEmbeddedMessages | SubtreeEnumerationOptions.RevisitParent, false)) {
                    while (enumerator.MoveNext()) {
                        if (enumerator.FirstVisit) {
                            if (enumerator.Depth != 0)
                                dstPart = new MimePart();
                            else if (signedOrProtectedContent) {
                                var dataStorage = enumerator.Current._dataStorage;
                                var num2 = enumerator.Current._storageInfo.DataStart;
                                num1 = position - num2;
                                mimePart = dstPart;
                                continue;
                            }
                            if (signedOrProtectedContent && enumerator.Current._dataStorage != null) {
                                dstPart.SetStorageImpl(
                                    dstStorage,
                                    num1 + enumerator.Current._storageInfo.DataStart,
                                    num1 + enumerator.Current._storageInfo.DataEnd,
                                    enumerator.Current._storageInfo.BodyOffset,
                                    enumerator.Current._storageInfo.BodyCte,
                                    enumerator.Current.BodyLineTermination);
                            } else if (enumerator.Current.IsSignedContent || enumerator.Current.IsProtectedContent) {
                                var num2 = enumerator.Current._dataStorage.CopyContentToStream(enumerator.Current._storageInfo.DataStart, enumerator.Current._storageInfo.DataEnd, dstStream, ref scratchBuffer);
                                dstPart.SetStorageImpl(dstStorage, position, position + num2, enumerator.Current._storageInfo.BodyOffset, enumerator.Current._storageInfo.BodyCte, enumerator.Current._storageInfo.BodyLineTermination);
                                if (!enumerator.LastVisit) {
                                    enumerator.Current.CopyPartTo(true, dstPart, dstStorage, null, position, ref scratchBuffer);
                                    enumerator.SkipChildren();
                                }
                                position += num2;
                            } else if (enumerator.Current._dataStorage == null || enumerator.Current.IsMultipart || enumerator.Current.IsEmbeddedMessage && !enumerator.LastVisit)
                                dstPart.SetStorageImpl(null, 0L, 0L);
                            else {
                                var num2 = enumerator.Current._dataStorage.CopyContentToStream(
                                    enumerator.Current._storageInfo.DataStart + enumerator.Current._storageInfo.BodyOffset,
                                    enumerator.Current._storageInfo.DataEnd,
                                    dstStream,
                                    ref scratchBuffer);
                                dstPart.SetStorageImpl(dstStorage, position, position + num2, 0L, enumerator.Current._storageInfo.BodyCte, enumerator.Current._storageInfo.BodyLineTermination);
                                position += num2;
                            }
                            dstPart._contentDirty = enumerator.Depth == 0 && !signedOrProtectedContent;
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
                                if (mimePart.IsEmbeddedMessage && mimePart._dataStorage != null && mimePart.InternalLastChild == null)
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

        internal override long WriteTo(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer) {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(_accessToken)) {
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
                                        enumerator.Current._dataStorage,
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
                                        filter?.ClosePart(enumerator.Current, stream);
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
                                            if (enumerator.Current._dataStorage != null) {
                                                num1 += MimePart.CopyStorageToStream(
                                                    enumerator.Current._dataStorage,
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
                                        filter?.ClosePart(enumerator.Current, stream);
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
                            buffer = ((MimePart) enumerator.Current.Parent).Boundary;
                            filter?.ClosePart(enumerator.Current, stream);
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
                    countingWriteStream2?.Dispose();
                }
                return num1;
            }
        }

        internal void SetStorage(DataStorage storage, long dataStart, long dataEnd) {
            using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                this.SetStorageImpl(storage, dataStart, dataEnd);
                this.ContentPersisted = false;
                this.ContentDirty = true;
                this.SetDirty();
            }
        }

        internal void SetStorage(DataStorage storage, long dataStart, long dataEnd, long bodyOffset, ContentTransferEncoding bodyCte, LineTerminationState bodyLineTermination) {
            using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                this.SetStorageImpl(storage, dataStart, dataEnd, bodyOffset, bodyCte, bodyLineTermination);
                this.ContentPersisted = false;
                this.ContentDirty = true;
                this.SetDirty();
            }
        }

        internal void SetStorageImpl(DataStorage storage, long dataStart, long dataEnd) {
            using (ThreadAccessGuard.EnterPublic(_accessToken))
                this.SetStorageImpl(storage, dataStart, dataEnd, 0L, ContentTransferEncoding.Binary, LineTerminationState.Unknown);
        }

        internal void SetStorageImpl(DataStorage storage, long dataStart, long dataEnd, long bodyOffset, ContentTransferEncoding bodyCte, LineTerminationState bodyLineTermination) {
            this.ThrowIfDisposed();
            this.ThrowIfReadOnly("MimePart.SetStorageImpl");
            using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                if (storage != null) {
                    storage.AddRef();
                    storage.SetReadOnly(false);
                }
                _dataStorage?.Release();
                _dataStorage = storage;
                _storageInfo.DataStart = dataStart;
                _storageInfo.DataEnd = dataEnd;
                _storageInfo.BodyOffset = bodyOffset;
                _storageInfo.BodyCte = bodyCte;
                _storageInfo.BodyLineTermination = bodyLineTermination;
            }
        }

        internal void SetDeferredStorageImpl(DataStorage storage, long dataStart, long dataEnd) {
            using (ThreadAccessGuard.EnterPublic(_accessToken))
                this.SetDeferredStorageImpl(storage, dataStart, dataEnd, 0L, ContentTransferEncoding.Binary, LineTerminationState.Unknown);
        }

        internal void SetDeferredStorageImpl(DataStorage storage, long dataStart, long dataEnd, long bodyOffset, ContentTransferEncoding bodyCte, LineTerminationState bodyLineTermination) {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                if (this.IsReadOnly) {
                    storage?.AddRef();
                    var dataStorageInfo = new DataStorageInfo {
                        DataStart = dataStart,
                        DataEnd = dataEnd,
                        BodyOffset = bodyOffset,
                        BodyCte = bodyCte,
                        BodyLineTermination = bodyLineTermination
                    };
                    lock (_deferredStorageLock) {
                        _deferredStorage?.Release();
                        _deferredStorage = storage;
                        _deferredStorageInfo = dataStorageInfo;
                    }
                } else
                    this.SetStorageImpl(storage, dataStart, dataEnd, bodyOffset, bodyCte, bodyLineTermination);
            }
        }

        internal void SetReadOnlyInternal(bool makeReadOnly) {
            using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                if (!makeReadOnly && _deferredStorageInfo != null) {
                    _dataStorage?.Release();
                    _dataStorage = _deferredStorage;
                    _storageInfo = _deferredStorageInfo;
                    _deferredStorage = null;
                    _deferredStorageInfo = null;
                }
                if (_dataStorage == null)
                    return;
                this.Storage.SetReadOnly(makeReadOnly);
            }
        }

        internal Charset FindMimeTreeCharset() {
            using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                Charset charset = null;
                var mimePart = this;
                while (!mimePart.IsEmbeddedMessage && mimePart.FirstChild != null)
                    mimePart = (MimePart) mimePart.FirstChild;
                var complexHeader = mimePart.Headers.FindFirst(HeaderId.ContentType) as ComplexHeader;
                var mimeParameter = complexHeader?["charset"];
                var rawValue = mimeParameter?.RawValue;
                if (rawValue != null) {
                    var name = ByteString.BytesToString(rawValue, false);
                    if (name != null && Charset.TryGetCharset(name, out charset) && charset.AsciiSupport < CodePageAsciiSupport.Fine)
                        charset = charset.Culture.MimeCharset;
                }
                return charset;
            }
        }

        private void IncrementVersion() {
            using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                this.Version = int.MaxValue == this.Version ? 1 : this.Version + 1;
                _protectedHeaders = null;
            }
        }

        internal void UpdateTransferEncoding(ContentTransferEncoding transferEncoding) {
            this.ThrowIfDisposed();
            this.ThrowIfReadOnly("MimePart.UpdateTransferEncoding");
            using (ThreadAccessGuard.EnterPublic(_accessToken)) {
                var encodingName = MimePart.GetEncodingName(transferEncoding);
                if (encodingName == null)
                    throw new ArgumentException("Transfer encoding is unknown or not supported", nameof(transferEncoding));
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
            if (_isDisposed)
                throw new ObjectDisposedException("MimePart");
        }

        private static readonly EncodingEntry[] encoding_map = {
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

        private readonly MimePartThreadAccessToken _accessToken;
        private readonly object _deferredStorageLock = new object();
        private byte[] _boundary;
        private int _cacheMapStamp;
        private bool _contentDirty;
        private bool _contentPersisted;
        private DataStorage _dataStorage;
        private DataStorage _deferredStorage;
        private DataStorageInfo _deferredStorageInfo;
        private bool _isDisposed;
        private MimeDocument _parentDocument;
        private List<string> _protectedHeaders;
        private DataStorageInfo _storageInfo = new DataStorageInfo();
        private HeaderList headers;

    }

}