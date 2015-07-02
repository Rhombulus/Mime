using System;
using System.Collections.Generic;
using System.Linq;

namespace Butler.Schema.Data.Mime {

    public class MimeDocument : IDisposable {

        public MimeDocument()
            : this(DecodingOptions.Default, MimeLimits.Default) {}

        public MimeDocument(DecodingOptions headerDecodingOptions, MimeLimits mimeLimits) {
            if (mimeLimits == null)
                throw new ArgumentNullException(nameof(mimeLimits));
            decodingOptions = headerDecodingOptions;
            limits = mimeLimits;
            accessToken = new MimeDocumentThreadAccessToken(this);
        }

        public EndOfHeadersCallback EndOfHeaders {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    return eohCallback;
            }
            set {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    eohCallback = value;
            }
        }

        public MimePart RootPart {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    return root;
            }
            set {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(accessToken)) {
                    if (value == null)
                        throw new ArgumentNullException(nameof(value));
                    if (value.Parent != null)
                        throw new ArgumentException(CtsResources.Strings.RootPartCantHaveAParent);
                    this.ThrowIfReadOnly("MimeDocument.set_RootPart");
                    if (reader != null)
                        throw new InvalidOperationException("Cannot set a new document root part while document loading is not complete");
                    lastPart = null;
                    contentStart = 0L;
                    complianceStatus = MimeComplianceStatus.Compliant;
                    stopLoading = false;
                    if (root != null)
                        root.ParentDocument = null;
                    root = value;
                    root.ParentDocument = this;
                    parsedSize = 0L;
                    this.IncrementVersion();
                }
            }
        }

        public DecodingOptions HeaderDecodingOptions {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    return decodingOptions;
            }
            internal set {
                this.ThrowIfDisposed();
                this.ThrowIfReadOnly("MimeDocument.set_HeaderDecodingOptions");
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    decodingOptions = value;
            }
        }

        public MimeLimits MimeLimits {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    return limits;
            }
        }

        public MimeComplianceMode ComplianceMode {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    return complianceMode;
            }
            set {
                this.ThrowIfDisposed();
                this.ThrowIfReadOnly("MimeDocument.set_ComplianceMode");
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    complianceMode = value;
            }
        }

        public MimeComplianceStatus ComplianceStatus {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    return complianceStatus;
            }
        }

        public bool RequiresSMTPUTF8 {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    return root.RequiresSMTPUTF8;
            }
        }

        public int Version { get; private set; }
        internal static bool FixMimeForTestUseOnly { get; set; } = true;
        internal ObjectThreadAccessToken AccessToken => (ObjectThreadAccessToken) accessToken;
        internal bool IsReadOnly { get; private set; }

        internal DecodingOptions EffectiveHeaderDecodingOptions {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(accessToken)) {
                    var decodingOptions = this.decodingOptions;
                    var mimeTreeCharset = this.GetMimeTreeCharset();
                    if (mimeTreeCharset != null)
                        decodingOptions.Charset = mimeTreeCharset;
                    return decodingOptions;
                }
            }
        }

        internal EncodingOptions EncodingOptions {
            get {
                this.ThrowIfDisposed();
                using (ThreadAccessGuard.EnterPublic(accessToken)) {
                    if (encodingOptions == null)
                        encodingOptions = new EncodingOptions(this.GetMimeTreeCharset());
                    return encodingOptions;
                }
            }
        }

        internal bool CreateValidateStorage {
            set {
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    createValidateStorage = value;
            }
        }

        internal long Position {
            get {
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    return reader.StreamOffset;
            }
        }

        internal long ParsedSize {
            get {
                using (ThreadAccessGuard.EnterPublic(accessToken))
                    return parsedSize;
            }
        }

        private bool CreateDomObjects {
            get {
                using (ThreadAccessGuard.EnterPrivate(accessToken))
                    return loadEmbeddedMessages || lastPart == null || !lastPart.IsEmbeddedMessage;
            }
        }

        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public MimePart Load(System.IO.Stream stream, CachingMode cachingMode) {
            this.ThrowIfDisposed();
            this.ThrowIfReadOnly("MimeDocument.Load");
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                if (stream == null)
                    throw new ArgumentNullException(nameof(stream));
                if (root != null)
                    throw new InvalidOperationException(CtsResources.Strings.CannotLoadIntoNonEmptyDocument);
                if (reader != null)
                    throw new InvalidOperationException("Cannot load document again while previous load is not complete");
                switch (cachingMode) {
                    case CachingMode.Copy:
                        this.InitializePushMode(true);
                        var discard = true;
                        try {
                            var buffer = new byte[4096];
                            while (!stopLoading) {
                                var count = stream.Read(buffer, 0, buffer.Length);
                                if (count != 0)
                                    this.Write(buffer, 0, count);
                                else
                                    break;
                            }
                            discard = false;
                            break;
                        } finally {
                            this.Flush(discard);
                        }
                    case CachingMode.Source:
                    case CachingMode.SourceTakeOwnership:
                        if (createValidateStorage) {
                            if (!stream.CanSeek)
                                throw new NotSupportedException(CtsResources.Strings.CachingModeSourceButStreamCannotSeek);
                            stream.Position = 0L;
                            backingStorage = new Internal.ReadableDataStorageOnStream(stream, cachingMode == CachingMode.SourceTakeOwnership);
                        }
                        reader = new MimeReader(stream, true, decodingOptions, limits, true, true, expectBinaryContent);
                        reader.DangerousSetFixBadMimeBoundary(dangerousFixBadMimeBoundary);
                        try {
                            this.BuildDom(null, 0, 0, true);
                        } finally {
                            reader.DisconnectInputStream();
                        }
                        parsedSize = reader.StreamOffset;
                        reader.Dispose();
                        reader = null;
                        if (backingStorage != null) {
                            backingStorage.Release();
                            backingStorage = null;
                        }
                        break;
                    default:
                        throw new ArgumentException("Invalid Caching Mode value", nameof(cachingMode));
                }
                return this.RootPart;
            }
        }

        public System.IO.Stream GetLoadStream() {
            return this.GetLoadStream(true);
        }

        public MimeDocument Clone() {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                if (reader != null)
                    throw new NotSupportedException(CtsResources.Strings.DocumentCloneNotSupportedInThisState);
                var mimeDocument = (MimeDocument) this.MemberwiseClone();
                if (root != null) {
                    mimeDocument.root = (MimePart) root.Clone();
                    mimeDocument.root.ParentDocument = mimeDocument;
                }
                mimeDocument.contentPositionStack = null;
                mimeDocument.lastPart = null;
                return mimeDocument;
            }
        }

        public long WriteTo(System.IO.Stream stream) {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                if (System.IO.Stream.Null != stream || cachedSizeVersion != this.Version) {
                    cachedSizeVersion = this.Version;
                    cachedSize = root == null ? 0L : root.WriteTo(stream, this.EncodingOptions, null);
                }
                return cachedSize;
            }
        }

        public long WriteTo(System.IO.Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter) {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                if (root == null)
                    return 0L;
                if (encodingOptions == null)
                    encodingOptions = this.EncodingOptions;
                return root.WriteTo(stream, encodingOptions, filter);
            }
        }

        internal void DangerousSetFixBadMimeBoundary(bool value) {
            if (reader != null)
                throw new InvalidOperationException("Cannot change FixBadMimeBoundary flag while previous load is not complete");
            dangerousFixBadMimeBoundary = value;
        }

        internal System.IO.Stream GetLoadStream(bool expectBinaryContent) {
            this.ThrowIfDisposed();
            this.ThrowIfReadOnly("MimeDocument.GetLoadStream");
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                if (root != null)
                    throw new InvalidOperationException(CtsResources.Strings.CannotLoadIntoNonEmptyDocument);
                if (reader != null)
                    throw new InvalidOperationException(CtsResources.Strings.CannotGetLoadStreamMoreThanOnce);
                this.expectBinaryContent = expectBinaryContent;
                this.InitializePushMode(this.expectBinaryContent);
                return new PushStream(this);
            }
        }

        internal void SetReadOnly(bool makeReadOnly) {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                if (makeReadOnly == this.IsReadOnly)
                    return;
                if (makeReadOnly)
                    this.CompleteParse();
                this.SetReadOnlyInternal(makeReadOnly);
            }
        }

        internal void CompleteParse() {
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                this.BuildDomAndCompleteParse(this.RootPart);
                var encodingOptions = this.EncodingOptions;
                this.GetMimeTreeCharset();
            }
        }

        internal void SetReadOnlyInternal(bool makeReadOnly) {
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                this.IsReadOnly = makeReadOnly;
                using (var enumerator = root.Subtree.GetEnumerator(MimePart.SubtreeEnumerationOptions.IncludeEmbeddedMessages, false)) {
                    while (enumerator.MoveNext())
                        enumerator.Current.SetReadOnlyInternal(makeReadOnly);
                }
            }
        }

        internal void IncrementVersion() {
            using (ThreadAccessGuard.EnterPublic(accessToken))
                this.Version = int.MaxValue == this.Version ? 1 : this.Version + 1;
        }

        internal void BuildEmbeddedDom(MimePart part) {
            this.ThrowIfDisposed();
            this.ThrowIfReadOnly("MimeDocument.BuildEmbeddedDom");
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                if (reader != null || stopLoading)
                    return;
                var mimePart = root;
                var flag = loadEmbeddedMessages;
                var ofHeadersCallback = eohCallback;
                eohCallback = null;
                loadEmbeddedMessages = true;
                reader = new MimeReader(null, true, decodingOptions, limits, true, true, expectBinaryContent);
                reader.DangerousSetFixBadMimeBoundary(dangerousFixBadMimeBoundary);
                try {
                    using (var enumerator = part.Subtree.GetEnumerator(MimePart.SubtreeEnumerationOptions.IncludeEmbeddedMessages, true)) {
                        while (enumerator.MoveNext()) {
                            var current = enumerator.Current;
                            if (current.InternalLastChild == null && current.Storage != null && current.IsEmbeddedMessage) {
                                this.ParseOnePart(current);
                                root.ParentDocument = null;
                                current.InternalInsertAfter(root, null);
                            }
                        }
                    }
                } finally {
                    reader.DisconnectInputStream();
                    reader.Dispose();
                    reader = null;
                    if (backingStorage != null) {
                        backingStorage.Release();
                        backingStorage = null;
                    }
                    backingStorageOffset = 0L;
                    root = mimePart;
                    loadEmbeddedMessages = flag;
                    eohCallback = ofHeadersCallback;
                }
            }
        }

        internal void BuildDomAndCompleteParse(MimePart rootPart) {
            using (ThreadAccessGuard.EnterPublic(accessToken)) {
                if (reader != null)
                    throw new InvalidOperationException("do not call BuildDomAndCompleteParse() before Load is complete");
                if (stopLoading)
                    throw new InvalidOperationException("do not call BuildDomAndCompleteParse() after canceling Load");
                if (rootPart.InternalLastChild == null && rootPart.Storage == null)
                    this.ParseAllHeaders(rootPart);
                else {
                    var mimePart1 = root;
                    var flag1 = loadEmbeddedMessages;
                    var flag2 = parseCompletely;
                    var ofHeadersCallback = eohCallback;
                    eohCallback = null;
                    loadEmbeddedMessages = true;
                    parseCompletely = true;
                    reader = new MimeReader(null, true, decodingOptions, limits, true, true, expectBinaryContent);
                    reader.DangerousSetFixBadMimeBoundary(dangerousFixBadMimeBoundary);
                    try {
                        var stack = new Stack<MimePart>(5);
                        stack.Push(rootPart);
                        label_13:
                        if (stack.Count <= 0)
                            return;
                        var part = stack.Pop();
                        do {
                            var internalLastChild = part.InternalLastChild;
                            this.ParseAllHeaders(part);
                            var mimePart2 = part.FirstChild as MimePart;
                            if (mimePart2 != null)
                                stack.Push(mimePart2);
                            part = part.NextSibling as MimePart;
                        } while (part != null);
                        goto label_13;
                    } finally {
                        reader.DisconnectInputStream();
                        reader.Dispose();
                        reader = null;
                        if (backingStorage != null) {
                            backingStorage.Release();
                            backingStorage = null;
                        }
                        backingStorageOffset = 0L;
                        root = mimePart1;
                        loadEmbeddedMessages = flag1;
                        parseCompletely = flag2;
                        eohCallback = ofHeadersCallback;
                    }
                }
            }
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing && !isDisposed) {
                if (backingStorageWriteStream != null) {
                    backingStorageWriteStream.Dispose();
                    backingStorageWriteStream = null;
                }
                if (backingStorage != null) {
                    backingStorage.Release();
                    backingStorage = null;
                }
                if (reader != null) {
                    reader.Dispose();
                    reader = null;
                }
                if (root != null) {
                    root.Dispose();
                    root = null;
                }
            }
            isDisposed = true;
        }

        private static bool IsContentBinary(System.IO.Stream stream, int bytesToExamine, int thresholdPercentage) {
            var offset = 0;
            var num1 = 0;
            var buffer = new byte[bytesToExamine];
            while (offset < buffer.Length) {
                var num2 = stream.Read(buffer, offset, buffer.Length - offset);
                if (num2 != 0)
                    offset += num2;
                else
                    break;
            }
            if (offset < 1)
                return false;
            for (var index = 0; index < offset; ++index) {
                if ((buffer[index] & 128) != 0)
                    ++num1;
            }
            return num1*100/offset >= thresholdPercentage;
        }

        private Globalization.Charset GetMimeTreeCharset() {
            this.ThrowIfDisposed();
            using (ThreadAccessGuard.EnterPrivate(accessToken)) {
                if (searchMimeTreeCharset) {
                    searchMimeTreeCharset = false;
                    if (root != null)
                        mimeTreeCharset = root.FindMimeTreeCharset();
                }
                return mimeTreeCharset;
            }
        }

        private void ParseOnePart(MimePart nextPart) {
            using (ThreadAccessGuard.EnterPrivate(accessToken)) {
                root = null;
                try {
                    backingStorage = nextPart.Storage;
                    backingStorage.AddRef();
                    backingStorageOffset = nextPart.DataStart + nextPart.BodyOffset;
                    System.IO.Stream contentReadStream;
                    using (contentReadStream = nextPart.GetRawContentReadStream()) {
                        reader.Reset(contentReadStream);
                        this.BuildDom(null, 0, 0, true, nextPart.IsEmbeddedMessage);
                    }
                } finally {
                    backingStorage.Release();
                    backingStorage = null;
                }
            }
        }

        private void Flush(bool discard) {
            this.ThrowIfDisposed();
            this.ThrowIfReadOnly("MimeDocument.Flush");
            using (ThreadAccessGuard.EnterPrivate(accessToken)) {
                if (reader == null)
                    return;
                if (!discard) {
                    backingStorageWriteStream.Flush();
                    if (!stopLoading)
                        this.BuildDom(null, 0, 0, true);
                }
                parsedSize = reader.StreamOffset;
                reader.Dispose();
                reader = null;
                backingStorageWriteStream.Dispose();
                backingStorageWriteStream = null;
                backingStorage.Release();
                backingStorage = null;
            }
        }

        private void Write(byte[] buffer, int offset, int count) {
            this.ThrowIfDisposed();
            this.ThrowIfReadOnly("MimeDocument.Write");
            using (ThreadAccessGuard.EnterPrivate(accessToken)) {
                if (reader == null)
                    throw new InvalidOperationException(CtsResources.Strings.CannotWriteAfterFlush);
                if (count == 0)
                    return;
                backingStorageWriteStream.Write(buffer, offset, count);
                if (stopLoading)
                    return;
                this.BuildDom(buffer, offset, count, false);
            }
        }

        private void InitializePushMode(bool expectBinaryContent) {
            using (ThreadAccessGuard.EnterPrivate(accessToken)) {
                var temporaryDataStorage = new Internal.TemporaryDataStorage();
                backingStorage = temporaryDataStorage;
                backingStorageWriteStream = temporaryDataStorage.OpenWriteStream(true);
                reader = new MimeReader(null, true, decodingOptions, limits, true, true, expectBinaryContent);
                reader.DangerousSetFixBadMimeBoundary(dangerousFixBadMimeBoundary);
            }
        }

        private void BuildDom(byte[] buffer, int offset, int length, bool eof) {
            this.BuildDom(buffer, offset, length, eof, false);
        }

        private void BuildDom(byte[] buffer, int offset, int length, bool eof, bool parseHeaders) {
            using (ThreadAccessGuard.EnterPrivate(accessToken)) {
                while (!reader.EndOfFile && (!reader.DataExhausted || length != 0 || eof) && !stopLoading) {
                    if (reader.DataExhausted) {
                        var num = reader.AddMoreData(buffer, offset, length, eof);
                        offset += num;
                        length -= num;
                    }
                    if (reader.TryReachNextState()) {
                        var complianceStatus = reader.ComplianceStatus;
                        switch (reader.ReaderState) {
                            case MimeReaderState.InlineBody:
                            case MimeReaderState.End:
                            case MimeReaderState.PartBody:
                                if (reader.ReaderState != MimeReaderState.PartBody) {
                                    this.complianceStatus |= complianceStatus;
                                    if (this.ComplianceMode == MimeComplianceMode.Strict && this.ComplianceStatus != MimeComplianceStatus.Compliant)
                                        throw new MimeException(CtsResources.Strings.StrictComplianceViolation);
                                }
                                continue;
                            case MimeReaderState.InlineEnd:
                                complianceStatus = this.CompletePart(true, parseHeaders);
                                goto case MimeReaderState.InlineBody;
                            case MimeReaderState.PartEnd:
                                complianceStatus = this.CompletePart(false, parseHeaders);
                                goto case MimeReaderState.InlineBody;
                            case MimeReaderState.InlineStart:
                                this.StartPart(true);
                                goto case MimeReaderState.InlineBody;
                            case MimeReaderState.EndOfHeaders:
                                this.EndPartHeaders();
                                goto case MimeReaderState.InlineBody;
                            case MimeReaderState.PartStart:
                                this.StartPart(false);
                                goto case MimeReaderState.InlineBody;
                            case MimeReaderState.HeaderStart:
                                if (!reader.TryCompleteCurrentHeader(embeddedMessagePartDepth == 0 || this.CreateDomObjects))
                                    goto case MimeReaderState.InlineBody;
                                goto case MimeReaderState.HeaderComplete;
                            case MimeReaderState.HeaderComplete:
                                if (embeddedMessagePartDepth == 0 || this.CreateDomObjects) {
                                    var currentHeaderObject = reader.CurrentHeaderObject;
                                    if (currentHeaderObject != null)
                                        lastPart.Headers.InternalAppendChild(currentHeaderObject);
                                }
                                goto case MimeReaderState.InlineBody;
                            default:
                                throw new InvalidOperationException("unexpected reader state");
                        }
                    }
                }
            }
        }

        private void StartPart(bool inline) {
            using (ThreadAccessGuard.EnterPrivate(accessToken)) {
                if (this.CreateDomObjects) {
                    var part1 = new MimePart();
                    if (root == null) {
                        root = part1;
                        part1.ParentDocument = this;
                    } else {
                        if (inline) {
                            if (!root.IsMultipart) {
                                var mimePart = lastPart == null ? root : (MimePart) lastPart.FirstChild;
                                var part2 = new MimePart();
                                var first1 = mimePart.Headers.FindFirst(HeaderId.ContentType);
                                var first2 = mimePart.Headers.FindFirst(HeaderId.ContentTransferEncoding);
                                if (first1 != null) {
                                    mimePart.Headers.InternalRemoveChild(first1);
                                    part2.Headers.InternalAppendChild(first1);
                                }
                                if (first2 != null) {
                                    mimePart.Headers.InternalRemoveChild(first2);
                                    part2.Headers.InternalAppendChild(first2);
                                }
                                Header header1 = new AsciiTextHeader("X-ConvertedToMime", HeaderId.Unknown);
                                header1.RawValue = MimeString.ConvertedToMimeUU;
                                mimePart.Headers.InternalAppendChild(header1);
                                var storage = mimePart.Storage;
                                part2.SetStorageImpl(storage, mimePart.DataStart + mimePart.BodyOffset, mimePart.DataEnd, 0L, mimePart.BodyCte, mimePart.BodyLineTermination);
                                mimePart.SetStorageImpl(null, 0L, 0L);
                                Header header2 = new ContentTypeHeader("multipart/mixed");
                                mimePart.Headers.InternalInsertAfter(header2, null);
                                mimePart.InternalAppendChild(part2);
                                lastPart = mimePart;
                                if (eohCallback != null) {
                                    bool stopLoading;
                                    eohCallback(part2, out stopLoading);
                                    if (stopLoading)
                                        this.stopLoading = true;
                                }
                            }
                            var str = reader.InlineFileName;
                            if (string.IsNullOrEmpty(str))
                                str = "unnamed.dat";
                            var header3 = Header.Create(HeaderId.ContentTransferEncoding);
                            header3.RawValue = MimeString.Uuencode;
                            part1.Headers.InternalAppendChild(header3);
                            Header header4 = new ContentDispositionHeader("attachment");
                            var mimeParameter1 = new MimeParameter("filename", str);
                            header4.InternalAppendChild(mimeParameter1);
                            part1.Headers.InternalAppendChild(header4);
                            Header header5 = new ContentTypeHeader("application/octet-stream");
                            var mimeParameter2 = new MimeParameter("name", str);
                            header5.InternalAppendChild(mimeParameter2);
                            part1.Headers.InternalAppendChild(header5);
                            if (eohCallback != null) {
                                bool stopLoading;
                                eohCallback(part1, out stopLoading);
                                if (stopLoading)
                                    this.stopLoading = true;
                            }
                        }
                        lastPart.InternalInsertAfter(part1, lastPart.InternalLastChild);
                    }
                    if (contentPositionStack == null)
                        contentPositionStack = new ContentPositionEntry[4];
                    else if (contentPositionStack.Length == contentPositionStackTop) {
                        var contentPositionEntryArray = new ContentPositionEntry[contentPositionStack.Length*2];
                        Array.Copy(contentPositionStack, 0, contentPositionEntryArray, 0, contentPositionStackTop);
                        contentPositionStack = contentPositionEntryArray;
                    }
                    contentPositionStack[contentPositionStackTop++] = new ContentPositionEntry(contentStart, headersEnd, contentTransferEncoding);
                    lastPart = part1;
                    contentStart = headersEnd = reader.StreamOffset;
                    contentTransferEncoding = inline ? reader.ContentTransferEncoding : ContentTransferEncoding.Unknown;
                } else if (embeddedMessagePartDepth == 0)
                    embeddedMessagePartDepth = reader.Depth;
                complianceStatus |= reader.ComplianceStatus;
                reader.ResetComplianceStatus();
            }
        }

        private void EndPartHeaders() {
            using (ThreadAccessGuard.EnterPrivate(accessToken)) {
                complianceStatus |= reader.ComplianceStatus;
                reader.ResetComplianceStatus();
                if (embeddedMessagePartDepth != 0 && !this.CreateDomObjects)
                    return;
                headersEnd = reader.StreamOffset;
                contentTransferEncoding = reader.ContentTransferEncoding;
                if (MimeDocument.FixMimeForTestUseOnly) {
                    if ((lastPart == root || ((MimePart) lastPart.Parent).IsEmbeddedMessage) && lastPart.Headers.FindFirst(HeaderId.MimeVersion) == null) {
                        var header = Header.Create(HeaderId.MimeVersion);
                        header.RawValue = MimeString.Version1;
                        lastPart.Headers.InternalAppendChild(header);
                    }
                    if (lastPart.Headers.FindFirst(HeaderId.ContentType) == null) {
                        var flag = false;
                        var mimePart = lastPart.Parent as MimePart;
                        if (mimePart != null && mimePart.Headers.FindFirst(HeaderId.ContentType)
                                                        .Value == "multipart/digest")
                            flag = true;
                        lastPart.Headers.InternalAppendChild(new ContentTypeHeader(flag ? "message/rfc822" : "text/plain"));
                    }
                }
                if (parseCompletely)
                    this.ParseAllHeaders(lastPart);
                if (eohCallback == null)
                    return;
                bool stopLoading;
                eohCallback(lastPart, out stopLoading);
                if (!stopLoading)
                    return;
                this.stopLoading = true;
            }
        }

        private void ParseAllHeaders(MimePart part) {
            using (ThreadAccessGuard.EnterPrivate(accessToken)) {
                foreach (var header in part.Headers)
                    header.ForceParse();
            }
        }

        private MimeComplianceStatus CompletePart(bool inline, bool parseHeaders) {
            using (ThreadAccessGuard.EnterPrivate(accessToken)) {
                var complianceStatus1 = reader.ComplianceStatus;
                if (embeddedMessagePartDepth == 0 || this.CreateDomObjects) {
                    if (createValidateStorage) {
                        if (parseHeaders)
                            this.ParseAllHeaders(lastPart);
                        lastPart.SetStorageImpl(backingStorage, contentStart + backingStorageOffset, reader.StreamOffset + backingStorageOffset, headersEnd - contentStart, contentTransferEncoding, reader.LineTerminationState);
                        var complianceStatus2 = MimeComplianceStatus.InvalidWrapping | MimeComplianceStatus.BareLinefeedInBody | MimeComplianceStatus.UnexpectedBinaryContent;
                        if (MimeDocument.FixMimeForTestUseOnly && (complianceStatus2 & complianceStatus1) != MimeComplianceStatus.Compliant && this.FixPartContent())
                            complianceStatus1 &= ~complianceStatus2;
                    }
                    var contentTypeHeader = lastPart.Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
                    if (contentTypeHeader != null && contentTypeHeader.IsMultipart && !lastPart.HasChildren)
                        contentTypeHeader.RawValue = MimeString.TextPlain;
                    lastPart = lastPart.Parent as MimePart;
                    --contentPositionStackTop;
                    contentStart = contentPositionStack[contentPositionStackTop].ContentStart;
                    headersEnd = contentPositionStack[contentPositionStackTop].HeadersEnd;
                    contentTransferEncoding = contentPositionStack[contentPositionStackTop].ContentTransferEncoding;
                }
                if (0 < embeddedMessagePartDepth && embeddedMessagePartDepth == reader.Depth)
                    embeddedMessagePartDepth = 0;
                reader.ResetComplianceStatus();
                return complianceStatus1;
            }
        }

        private bool FixPartContent() {
            using (ThreadAccessGuard.EnterPrivate(accessToken)) {
                var contentTypeHeader1 = lastPart.Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
                if (contentTypeHeader1.IsMultipart || contentTypeHeader1.MediaType == "message" || lastPart.BodyCte == ContentTransferEncoding.Unknown)
                    return false;
                var mimePart1 = lastPart;
                do {
                    var mimePart2 = mimePart1.Parent as MimePart;
                    if (mimePart2 != null) {
                        if (mimePart1 == mimePart2.FirstChild) {
                            var contentTypeHeader2 = mimePart2.Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
                            if (contentTypeHeader2 != null && contentTypeHeader2.Value == "multipart/signed")
                                return false;
                        }
                        if (mimePart2.Headers.FindFirst("DKIM-Signature") != null)
                            return false;
                    }
                    mimePart1 = mimePart2;
                } while (mimePart1 != null);
                var first = lastPart.Headers.FindFirst(HeaderId.ContentTransferEncoding);
                if (first == null) {
                    var header = Header.Create(HeaderId.ContentTransferEncoding);
                    lastPart.Headers.InternalAppendChild(header);
                    header.RawValue = MimeString.QuotedPrintable;
                    return true;
                }
                var encodingType = MimePart.GetEncodingType(first.FirstRawToken);
                switch (encodingType) {
                    case ContentTransferEncoding.Unknown:
                    case ContentTransferEncoding.SevenBit:
                    case ContentTransferEncoding.EightBit:
                        first.RawValue = MimeString.QuotedPrintable;
                        return true;
                    case ContentTransferEncoding.QuotedPrintable:
                        this.ForceReencoding(encodingType);
                        return true;
                    case ContentTransferEncoding.Base64:
                        var flag = false;
                        if ((reader.ComplianceStatus & MimeComplianceStatus.UnexpectedBinaryContent) != MimeComplianceStatus.Compliant) {
                            var start = lastPart.DataStart + lastPart.BodyOffset;
                            var num = Math.Min(lastPart.DataEnd - start, 1000L);
                            var end = start + num;
                            if (num > 10L) {
                                using (var stream = lastPart.Storage.OpenReadStream(start, end))
                                    flag = MimeDocument.IsContentBinary(stream, (int) num, 10);
                            }
                        }
                        if (flag)
                            this.RepairBrokenExchangeMime(encodingType);
                        else
                            this.ForceReencoding(encodingType);
                        return true;
                    default:
                        return false;
                }
            }
        }

        private void RepairBrokenExchangeMime(ContentTransferEncoding encoding) {
            var encodingDataStorage = new EncodingDataStorage(lastPart.Storage, lastPart.DataStart + lastPart.BodyOffset, lastPart.DataEnd, encoding);
            lastPart.SetStorageImpl(encodingDataStorage, 0L, long.MaxValue, 0L, encoding, LineTerminationState.CRLF);
            encodingDataStorage.Release();
        }

        private void ForceReencoding(ContentTransferEncoding encoding) {
            var decodingDataStorage = new DecodingDataStorage(lastPart.Storage, lastPart.DataStart + lastPart.BodyOffset, lastPart.DataEnd, encoding);
            lastPart.SetStorageImpl(decodingDataStorage, 0L, long.MaxValue, 0L, ContentTransferEncoding.Binary, LineTerminationState.CRLF);
            decodingDataStorage.Release();
        }

        private void ThrowIfDisposed() {
            if (isDisposed)
                throw new ObjectDisposedException("MimeDocument");
        }

        private void ThrowIfReadOnly(string method) {
            if (this.IsReadOnly)
                throw new ReadOnlyMimeException(method);
        }

        private readonly MimeDocumentThreadAccessToken accessToken;
        private readonly MimeLimits limits;
        private Internal.DataStorage backingStorage;
        private long backingStorageOffset;
        private System.IO.Stream backingStorageWriteStream;
        private long cachedSize;
        private int cachedSizeVersion = -1;
        private MimeComplianceMode complianceMode;
        private MimeComplianceStatus complianceStatus;
        private ContentPositionEntry[] contentPositionStack;
        private int contentPositionStackTop;
        private long contentStart;
        private ContentTransferEncoding contentTransferEncoding;
        private bool createValidateStorage = true;
        private bool dangerousFixBadMimeBoundary = true;
        private DecodingOptions decodingOptions = DecodingOptions.Default;
        private int embeddedMessagePartDepth;
        private EncodingOptions encodingOptions;
        private EndOfHeadersCallback eohCallback;
        private bool expectBinaryContent;
        private long headersEnd;
        private bool isDisposed;
        private MimePart lastPart;
        private bool loadEmbeddedMessages;
        private Globalization.Charset mimeTreeCharset;
        private bool parseCompletely;
        private long parsedSize;
        private MimeReader reader;
        private MimePart root;
        private bool searchMimeTreeCharset = true;
        private bool stopLoading;


        public delegate void EndOfHeadersCallback(MimePart part, out bool stopLoading);


        private struct ContentPositionEntry {

            public ContentPositionEntry(long contentStart, long headersEnd, ContentTransferEncoding contentTransferEncoding) {
                ContentStart = contentStart;
                HeadersEnd = headersEnd;
                ContentTransferEncoding = contentTransferEncoding;
            }

            public readonly long ContentStart;
            public readonly ContentTransferEncoding ContentTransferEncoding;
            public readonly long HeadersEnd;

        }


        private class MimeDocumentThreadAccessToken : ObjectThreadAccessToken {

            internal MimeDocumentThreadAccessToken(MimeDocument parent) {}

        }


        private class PushStream : System.IO.Stream {

            public PushStream(MimeDocument document) {
                this.document = document;
            }

            public override bool CanRead => false;
            public override bool CanSeek => false;
            public override bool CanWrite => document != null;

            public override long Length {
                get {
                    throw new NotSupportedException();
                }
            }

            public override long Position {
                get {
                    throw new NotSupportedException();
                }
                set {
                    throw new NotSupportedException();
                }
            }

            public override void Flush() {
                if (document == null)
                    throw new ObjectDisposedException("stream");
                using (ThreadAccessGuard.EnterPublic(document.AccessToken)) {
                    if (badState)
                        return;
                    if (document.stopLoading)
                        throw new InvalidOperationException(CtsResources.Strings.LoadingStopped);
                    badState = true;
                    document.Flush(false);
                    badState = false;
                }
            }

            public override int Read(byte[] buffer, int offset, int count) {
                throw new NotSupportedException();
            }

            public override long Seek(long offset, System.IO.SeekOrigin origin) {
                throw new NotSupportedException();
            }

            public override void SetLength(long length) {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count) {
                if (document == null)
                    throw new ObjectDisposedException("stream");
                using (ThreadAccessGuard.EnterPublic(document.AccessToken)) {
                    if (badState)
                        return;
                    if (document.stopLoading)
                        throw new InvalidOperationException(CtsResources.Strings.LoadingStopped);
                    badState = true;
                    document.Write(buffer, offset, count);
                    badState = false;
                }
            }

            protected override void Dispose(bool disposing) {
                if (disposing && document != null) {
                    using (ThreadAccessGuard.EnterPublic(document.AccessToken)) {
                        document.Flush(badState);
                        document = null;
                    }
                }
                base.Dispose(disposing);
            }

            private bool badState;
            private MimeDocument document;

        }


        private class CodingDataStorage : Internal.DataStorage {

            public CodingDataStorage(Internal.DataStorage storage, long start, long end, ContentTransferEncoding cte, bool encode) {
                storage.AddRef();
                this.storage = storage;
                this.start = start;
                this.end = end;
                this.cte = cte;
                this.encode = encode;
            }

            public override System.IO.Stream OpenReadStream(long start, long end) {
                this.ThrowIfDisposed();
                start = this.start + start;
                end = end != long.MaxValue ? this.start + end : this.end;
                var encoder = encode ? EncodingDataStorage.CreateEncoder(cte) : MimePart.CreateDecoder(cte);
                if (encoder == null)
                    return storage.OpenReadStream(start, end);
                return new Encoders.EncoderStream(storage.OpenReadStream(start, end), encoder, Encoders.EncoderStreamAccess.Read, true);
            }

            protected override void Dispose(bool disposing) {
                if (disposing && !this.IsDisposed && storage != null) {
                    storage.Release();
                    storage = null;
                }
                base.Dispose(disposing);
            }

            private readonly ContentTransferEncoding cte;
            private readonly bool encode;
            private readonly long end;
            private readonly long start;
            private Internal.DataStorage storage;

        }


        private class EncodingDataStorage : CodingDataStorage {

            public EncodingDataStorage(Internal.DataStorage storage, long start, long end, ContentTransferEncoding cte)
                : base(storage, start, end, cte, true) {}

            internal static Encoders.ByteEncoder CreateEncoder(ContentTransferEncoding encoding) {
                switch (encoding) {
                    case ContentTransferEncoding.QuotedPrintable:
                        return new Encoders.QPEncoder();
                    case ContentTransferEncoding.Base64:
                        return new Encoders.Base64Encoder();
                    case ContentTransferEncoding.UUEncode:
                        return new Encoders.UUEncoder();
                    default:
                        return null;
                }
            }

        }


        private class DecodingDataStorage : CodingDataStorage {

            public DecodingDataStorage(Internal.DataStorage storage, long start, long end, ContentTransferEncoding cte)
                : base(storage, start, end, cte, false) {}

        }

    }

}