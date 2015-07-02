using System;
using System.Linq;

namespace Butler.Schema.Data.Mime {

    public class MimeReader : IDisposable {

        public MimeReader(System.IO.Stream mime)
            : this(mime, true, DecodingOptions.Default, MimeLimits.Unlimited, false, true) {
            if (mime == null)
                throw new ArgumentNullException(nameof(mime));
        }

        public MimeReader(System.IO.Stream mime, bool inferMime, DecodingOptions decodingOptions, MimeLimits mimeLimits)
            : this(mime, inferMime, decodingOptions, mimeLimits, false, true) {
            if (mime == null)
                throw new ArgumentNullException(nameof(mime));
        }

        internal MimeReader(System.IO.Stream mime, bool inferMime, DecodingOptions decodingOptions, MimeLimits mimeLimits, bool parseEmbeddedMessages, bool parseInline)
            : this(mime, inferMime, decodingOptions, mimeLimits, parseEmbeddedMessages, parseInline, true) {}

        internal MimeReader(System.IO.Stream mime, bool inferMime, DecodingOptions decodingOptions, MimeLimits mimeLimits, bool parseEmbeddedMessages, bool parseInline, bool expectBinaryContent) {
            if (mime != null && !mime.CanRead)
                throw new ArgumentException(Resources.Strings.StreamMustAllowRead, nameof(mime));
            mimeStream = mime;
            parser = new MimeParser(true, parseInline, expectBinaryContent);
            data = new byte[5120];
            this.inferMime = inferMime;
            this.HeaderDecodingOptions = decodingOptions;
            limits = mimeLimits;
            this.parseEmbeddedMessages = parseEmbeddedMessages;
        }

        private MimeReader(MimeReader parent) {
            parentReader = parent;
            parentReader.childReader = this;
            mimeStream = parent.mimeStream;
            parser = parent.parser;
            data = parent.data;
            dataOffset = parent.dataOffset;
            dataCount = parent.dataCount;
            dataEOF = parent.dataEOF;
            outerContentStream = parent.outerContentStream;
            outerContentDepth = -1;
            inferMime = parent.inferMime;
            limits = parent.limits;
            this.HeaderDecodingOptions = parent.HeaderDecodingOptions;
            partCount = parent.partCount;
            headerCount = parent.headerCount;
            cumulativeHeaderBytes = parent.cumulativeHeaderBytes;
            embeddedDepth = parent.embeddedDepth + 1;
        }

        internal MimeReader(IMimeHandlerInternal handler, bool inferMime, DecodingOptions decodingOptions, MimeLimits mimeLimits, bool parseInline) {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));
            this.handler = handler;
            parser = new MimeParser(true, parseInline, true);
            data = new byte[5120];
            this.inferMime = inferMime;
            this.HeaderDecodingOptions = decodingOptions;
            limits = mimeLimits;
            parseEmbeddedMessages = true;
        }

        public MimeLimits MimeLimits {
            get {
                this.AssertGoodToUse(false, false);
                return limits;
            }
        }

        public DecodingOptions HeaderDecodingOptions { get; }

        public MimeComplianceStatus ComplianceStatus {
            get {
                this.AssertGoodToUse(false, false);
                return parser.ComplianceStatus;
            }
        }

        public long StreamOffset {
            get {
                this.AssertGoodToUse(false, true);
                return parser.Position;
            }
        }

        internal int Depth {
            get {
                this.AssertGoodToUse(false, true);
                return depth;
            }
        }

        public int PartDepth {
            get {
                this.AssertGoodToUse(false, true);
                if (depth != 0)
                    return parser.PartDepth + 1;
                return 0;
            }
        }

        public int EmbeddedDepth {
            get {
                this.AssertGoodToUse(false, true);
                return embeddedDepth;
            }
        }

        internal MimeReaderState ReaderState { get; private set; } = MimeReaderState.Start;
        internal bool DataExhausted { get; private set; }
        internal bool EndOfFile => this.ReaderState == MimeReaderState.End;

        public MimeHeaderReader HeaderReader {
            get {
                this.AssertGoodToUse(true, true);
                if (!MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderComplete | MimeReaderState.EndOfHeaders | MimeReaderState.InlineStart))
                    throw new InvalidOperationException(Resources.Strings.OperationNotValidInThisReaderState);
                return new MimeHeaderReader(this);
            }
        }

        internal HeaderId HeaderId {
            get {
                this.AssertGoodToUse(false, true);
                if (!MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.HeaderStart | MimeReaderState.HeaderIncomplete | MimeReaderState.HeaderComplete))
                    throw new InvalidOperationException(Resources.Strings.OperationNotValidInThisReaderState);
                return currentHeaderId;
            }
        }

        internal string HeaderName {
            get {
                this.AssertGoodToUse(false, true);
                if (!MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.HeaderStart | MimeReaderState.HeaderIncomplete | MimeReaderState.HeaderComplete))
                    throw new InvalidOperationException(Resources.Strings.OperationNotValidInThisReaderState);
                return currentHeaderName;
            }
        }

        public bool IsMultipart {
            get {
                this.AssertGoodToUse(false, true);
                if (this.ReaderState == MimeReaderState.InlineStart)
                    return false;
                if (this.ReaderState != MimeReaderState.EndOfHeaders)
                    throw new InvalidOperationException(Resources.Strings.OperationNotValidInThisReaderState);
                if (currentPartMajorType == MajorContentType.Multipart)
                    return parser.IsMime;
                return false;
            }
        }

        public bool IsEmbeddedMessage {
            get {
                this.AssertGoodToUse(false, true);
                if (this.ReaderState == MimeReaderState.InlineStart)
                    return false;
                if (this.ReaderState != MimeReaderState.EndOfHeaders)
                    throw new InvalidOperationException(Resources.Strings.OperationNotValidInThisReaderState);
                if (currentPartMajorType == MajorContentType.MessageRfc822)
                    return parser.IsMime;
                return false;
            }
        }

        public string ContentType {
            get {
                this.AssertGoodToUse(false, true);
                if (this.ReaderState == MimeReaderState.InlineStart)
                    return "application/octet-stream";
                if (this.ReaderState != MimeReaderState.EndOfHeaders)
                    throw new InvalidOperationException(Resources.Strings.OperationNotValidInThisReaderState);
                return currentPartContentType;
            }
        }

        internal ContentTransferEncoding ContentTransferEncoding {
            get {
                this.AssertGoodToUse(false, true);
                if (this.ReaderState != MimeReaderState.EndOfHeaders && this.ReaderState != MimeReaderState.InlineStart)
                    throw new InvalidOperationException(Resources.Strings.OperationNotValidInThisReaderState);
                return currentPartContentTransferEncoding;
            }
        }

        public bool IsInline {
            get {
                this.AssertGoodToUse(false, true);
                if (MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.Start | MimeReaderState.End))
                    throw new InvalidOperationException(Resources.Strings.OperationNotValidInThisReaderState);
                return MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.InlineStart | MimeReaderState.InlineBody | MimeReaderState.InlineEnd);
            }
        }

        public string InlineFileName {
            get {
                this.AssertGoodToUse(false, true);
                if (!this.IsInline)
                    throw new InvalidOperationException(Resources.Strings.OperationNotValidInThisReaderState);
                return inlineFileName.ToString();
            }
        }

        internal LineTerminationState LineTerminationState { get; private set; }
        internal Header CurrentHeaderObject { get; private set; }

        internal bool GroupInProgress {
            get {
                if (currentChild != null)
                    return currentChild is MimeGroup;
                return false;
            }
        }

        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (parser == null)
                return;
            if (disposing) {
                if (childReader != null)
                    throw new InvalidOperationException(Resources.Strings.EmbeddedMessageReaderNeedsToBeClosedFirst);
                if (parentReader == null) {
                    if (mimeStream != null)
                        mimeStream.Dispose();
                } else {
                    parentReader.partCount = partCount;
                    parentReader.headerCount = headerCount;
                    parentReader.cumulativeHeaderBytes = cumulativeHeaderBytes;
                    parentReader.dataOffset = dataOffset;
                    parentReader.dataCount = dataCount;
                    parentReader.dataEOF = dataEOF;
                    parentReader.currentToken = currentToken;
                    parentReader.cleanupDepth = depth + cleanupDepth;
                    parentReader.ReaderState = MimeReaderState.EmbeddedEnd;
                    parentReader.childReader = null;
                    parentReader = null;
                }
                if (contentStream != null)
                    contentStream.Dispose();
                if (outerContentStream != null) {
                    var num = outerContentDepth;
                }
            }
            this.ReaderState = MimeReaderState.End;
            mimeStream = null;
            handler = null;
            contentStream = null;
            outerContentStream = null;
            data = null;
            parser = null;
            this.CurrentHeaderObject = null;
            currentChild = null;
            currentGrandChild = null;
        }

        public void Close() {
            this.Dispose();
        }

        internal void DisconnectInputStream() {
            mimeStream = null;
        }

        public bool ReadNextPart() {
            this.AssertGoodToUse(true, true);
            this.TrySkipToNextPartBoundary(false);
            return MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.PartStart | MimeReaderState.InlineStart);
        }

        public bool ReadFirstChildPart() {
            this.AssertGoodToUse(true, true);
            if (this.ReaderState == MimeReaderState.InlineStart)
                return false;
            if (!MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.Start | MimeReaderState.EndOfHeaders)) {
                if (!MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderComplete))
                    throw new InvalidOperationException(Resources.Strings.OperationNotValidInThisReaderState);
                do {
                    this.TryReachNextState();
                } while (this.ReaderState != MimeReaderState.EndOfHeaders);
            }
            if (this.ReaderState == MimeReaderState.EndOfHeaders && !this.IsMultipart && (!this.IsEmbeddedMessage || !parseEmbeddedMessages))
                return false;
            this.TrySkipToNextPartBoundary(true);
            return this.ReaderState == MimeReaderState.PartStart;
        }

        public bool ReadNextSiblingPart() {
            this.AssertGoodToUse(true, true);
            if (this.ReaderState == MimeReaderState.End)
                return false;
            if (MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderComplete | MimeReaderState.EndOfHeaders)) {
                createHeader = false;
                while (this.ReaderState != MimeReaderState.EndOfHeaders)
                    this.TryReachNextState();
                parser.SetContentType(MajorContentType.Other, new MimeString());
            }
            if (!MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.Start | MimeReaderState.PartEnd | MimeReaderState.InlineEnd)) {
                var num = depth;
                do {
                    this.TrySkipToNextPartBoundary(true);
                } while (depth > num || !MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.PartEnd | MimeReaderState.InlineEnd));
            }
            this.TrySkipToNextPartBoundary(true);
            return MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.PartStart | MimeReaderState.InlineStart);
        }

        public void EnableReadingUnparsedHeaders() {
            enableReadingOuterContent = true;
        }

        public void ReadHeaders() {
            this.AssertGoodToUse(true, true);
            if (MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.EndOfHeaders | MimeReaderState.InlineStart))
                return;
            if (!MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderComplete))
                throw new InvalidOperationException(Resources.Strings.OperationNotValidInThisReaderState);
            createHeader = false;
            do {
                this.TryReachNextState();
            } while (this.ReaderState != MimeReaderState.EndOfHeaders);
        }

        internal bool ReadNextHeader() {
            this.AssertGoodToUse(true, true);
            if (MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.EndOfHeaders | MimeReaderState.InlineStart))
                return false;
            if (!MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderComplete))
                throw new InvalidOperationException(Resources.Strings.OperationNotValidInThisReaderState);
            do {
                this.TryReachNextState();
            } while (!MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.HeaderStart | MimeReaderState.EndOfHeaders));
            return this.ReaderState == MimeReaderState.HeaderStart;
        }

        internal Header ReadHeaderObject() {
            if (!MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.HeaderStart | MimeReaderState.HeaderComplete))
                throw new InvalidOperationException(Resources.Strings.OperationNotValidInThisReaderState);
            if (this.ReaderState == MimeReaderState.HeaderStart)
                this.TryCompleteCurrentHeader(true);
            return this.CurrentHeaderObject;
        }

        internal bool TryCompleteCurrentHeader(bool createHeader) {
            this.createHeader = this.createHeader || createHeader;
            if (!this.TryReachNextState())
                return false;
            currentHeaderConsumed = false;
            currentChildConsumed = false;
            currentChild = null;
            currentGrandChild = null;
            return true;
        }

        internal void Reset(System.IO.Stream stream) {
            mimeStream = stream;
            parser.Reset();
            this.ReaderState = MimeReaderState.Start;
            depth = 0;
            cleanupDepth = 0;
            embeddedDepth = 0;
            this.DataExhausted = false;
            dataEOF = false;
            dataOffset = 0;
            dataCount = 0;
            currentToken = new MimeToken();
            this.CurrentHeaderObject = null;
            currentChild = null;
            currentGrandChild = null;
            decoder = null;
            readRawData = false;
            contentStream = null;
            enableReadingOuterContent = false;
            outerContentStream = null;
            outerContentDepth = 0;
            childReader = null;
            parentReader = null;
            skipPart = false;
            skipHeaders = false;
            skipHeader = false;
            partCount = 0;
            headerCount = 0;
            cumulativeHeaderBytes = 0;
            currentTextHeaderBytes = 0;
        }

        internal void DangerousSetFixBadMimeBoundary(bool value) {
            FixBadMimeBoundary = value;
        }

        internal HeaderList ReadHeaderList() {
            this.AssertGoodToUse(true, true);
            if (!MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.PartStart | MimeReaderState.InlineStart))
                throw new InvalidOperationException(Resources.Strings.OperationNotValidInThisReaderState);
            var headerList = new HeaderList(null);
            if (this.ReaderState == MimeReaderState.InlineStart)
                return headerList;
            do {
                this.TryReachNextState();
                if (this.ReaderState == MimeReaderState.HeaderStart)
                    createHeader = true;
                else if (this.ReaderState == MimeReaderState.HeaderComplete && this.CurrentHeaderObject != null)
                    headerList.InternalAppendChild(this.CurrentHeaderObject);
            } while (this.ReaderState != MimeReaderState.EndOfHeaders);
            return headerList;
        }

        internal bool ReadNextDescendant(bool topLevel) {
            this.AssertGoodToUse(true, true);
            if (this.ReaderState != MimeReaderState.HeaderComplete)
                throw new InvalidOperationException(Resources.Strings.OperationNotValidInThisReaderState);
            if (topLevel) {
                if (currentHeaderConsumed)
                    return false;
                if (currentChild == null)
                    currentChild = this.CurrentHeaderObject.FirstChild;
                else {
                    currentGrandChild = null;
                    currentChild = currentChild.NextSibling;
                }
                if (currentChild == null)
                    currentHeaderConsumed = true;
                currentChildConsumed = false;
                currentGrandChild = null;
                return currentChild != null;
            }
            if (currentChild == null)
                throw new InvalidOperationException(Resources.Strings.OperationNotValidInThisReaderState);
            if (currentChildConsumed)
                return false;
            currentGrandChild = currentGrandChild != null ? currentGrandChild.NextSibling : currentChild.FirstChild;
            if (currentGrandChild == null)
                currentChildConsumed = true;
            return currentGrandChild != null;
        }

        internal bool IsCurrentChildValid(bool topLevel) {
            if (!topLevel)
                return currentGrandChild != null;
            return currentChild != null;
        }

        public void CopyOuterContentTo(System.IO.Stream stream) {
            this.AssertGoodToUse(false, true);
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanWrite)
                throw new ArgumentException(Resources.Strings.StreamMustSupportWriting, nameof(stream));
            if (!MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.PartStart | MimeReaderState.InlineStart))
                throw new InvalidOperationException(Resources.Strings.OperationNotValidInThisReaderState);
            if (outerContentStream != null)
                throw new NotSupportedException(Resources.Strings.OnlyOneOuterContentPushStreamAllowed);
            outerContentStream = stream;
            outerContentDepth = depth;
        }

        public int ReadRawContent(byte[] buffer, int offset, int count) {
            this.AssertGoodToUse(true, true);
            MimeCommon.CheckBufferArguments(buffer, offset, count);
            if (!MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.PartBody | MimeReaderState.InlineBody)) {
                if (MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.PartEnd | MimeReaderState.InlineEnd))
                    return 0;
                this.TryInitializeReadContent(false);
            }
            if (contentStream != null)
                throw new NotSupportedException(Resources.Strings.CannotReadContentWhileStreamIsActive);
            if (!readRawData)
                throw new NotSupportedException(Resources.Strings.CannotMixReadRawContentAndReadContent);
            int readCount;
            this.ReadPartData(buffer, offset, count, out readCount);
            return readCount;
        }

        public int ReadContent(byte[] buffer, int offset, int count) {
            this.AssertGoodToUse(true, true);
            MimeCommon.CheckBufferArguments(buffer, offset, count);
            if (!MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.PartBody | MimeReaderState.InlineBody)) {
                if (MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.PartEnd | MimeReaderState.InlineEnd))
                    return 0;
                if (!this.TryInitializeReadContent(true))
                    throw new MimeException(Resources.Strings.CannotDecodeContentStream);
            }
            if (contentStream != null)
                throw new NotSupportedException(Resources.Strings.CannotReadContentWhileStreamIsActive);
            if (readRawData)
                throw new NotSupportedException(Resources.Strings.CannotMixReadRawContentAndReadContent);
            int readCount;
            this.ReadPartData(buffer, offset, count, out readCount);
            return readCount;
        }

        public System.IO.Stream GetContentReadStream() {
            System.IO.Stream result;
            if (!this.TryGetContentReadStream(out result))
                throw new MimeException(Resources.Strings.CannotDecodeContentStream);
            return result;
        }

        public bool TryGetContentReadStream(out System.IO.Stream result) {
            this.AssertGoodToUse(true, true);
            if (!this.TryInitializeReadContent(true)) {
                result = null;
                return false;
            }
            contentStream = new ContentReadStream(this);
            result = contentStream;
            return true;
        }

        public System.IO.Stream GetRawContentReadStream() {
            this.AssertGoodToUse(true, true);
            this.TryInitializeReadContent(false);
            contentStream = new ContentReadStream(this);
            return contentStream;
        }

        private bool TryInitializeReadContent(bool decode) {
            if (!MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.EndOfHeaders | MimeReaderState.InlineStart)) {
                if (!MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderComplete))
                    throw new InvalidOperationException(Resources.Strings.OperationNotValidInThisReaderState);
                if (this.ReaderState == MimeReaderState.PartStart && enableReadingOuterContent) {
                    parser.SetStreamMode();
                    this.ReaderState = MimeReaderState.PartBody;
                    decoder = null;
                    readRawData = !decode;
                    return true;
                }
                while (this.TryReachNextState()) {
                    if (this.ReaderState == MimeReaderState.EndOfHeaders)
                        goto label_8;
                }
                return false;
            }
            label_8:
            var mimeReaderState1 = this.ReaderState;
            MimeReaderState mimeReaderState2;
            if (this.ReaderState == MimeReaderState.EndOfHeaders) {
                parser.SetContentType(MajorContentType.Other, new MimeString());
                mimeReaderState2 = MimeReaderState.PartBody;
            } else
                mimeReaderState2 = MimeReaderState.InlineBody;
            if (decode) {
                var transferEncoding = parser.TransferEncoding;
                switch (transferEncoding) {
                    case ContentTransferEncoding.SevenBit:
                    case ContentTransferEncoding.EightBit:
                    case ContentTransferEncoding.Binary:
                        decoder = null;
                        break;
                    default:
                        decoder = MimePart.CreateDecoder(transferEncoding);
                        if (decoder == null)
                            return false;
                        break;
                }
                readRawData = false;
            } else {
                decoder = null;
                readRawData = true;
            }
            this.ReaderState = mimeReaderState2;
            return true;
        }

        public MimeReader GetEmbeddedMessageReader() {
            this.AssertGoodToUse(true, true);
            if (this.ReaderState != MimeReaderState.EndOfHeaders) {
                if (!MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderComplete))
                    throw new InvalidOperationException(Resources.Strings.OperationNotValidInThisReaderState);
                do {
                    this.TryReachNextState();
                } while (this.ReaderState != MimeReaderState.EndOfHeaders);
            }
            if (currentPartMajorType != MajorContentType.MessageRfc822 || !parser.IsMime)
                throw new InvalidOperationException(Resources.Strings.CurrentPartIsNotEmbeddedMessage);
            parser.SetContentType(MajorContentType.MessageRfc822, new MimeString());
            this.TryReachNextState();
            childReader = new MimeReader(this);
            return childReader;
        }

        public void ResetComplianceStatus() {
            parser.ComplianceStatus = MimeComplianceStatus.Compliant;
        }

        private int TrimEndOfLine(int offset, int length) {
            if (length >= 1 && data[offset + length - 1] == 10) {
                --length;
                while (length >= 1 && data[offset + length - 1] == 13)
                    --length;
            }
            return length;
        }

        private void ParseAndCheckSize() {
            currentToken = parser.Parse(data, dataOffset, dataOffset + dataCount, dataEOF);
            if (parser.Position > this.MimeLimits.MaxSize)
                throw new MimeException(Resources.Strings.InputStreamTooLong(parser.Position, this.MimeLimits.MaxSize));
        }

        private void CheckHeaderBytesLimits() {
            cumulativeHeaderBytes += currentToken.Length;
            if (cumulativeHeaderBytes > this.MimeLimits.MaxHeaderBytes)
                throw new MimeException(Resources.Strings.TooManyHeaderBytes(cumulativeHeaderBytes, this.MimeLimits.MaxHeaderBytes));
            if (currentToken.Id == MimeTokenId.Header)
                currentTextHeaderBytes = 0;
            currentTextHeaderBytes += currentToken.Length;
            var type = Header.TypeFromHeaderId(currentHeaderId);
            if ((type == typeof (TextHeader) || type == typeof (AsciiTextHeader)) && currentTextHeaderBytes > this.MimeLimits.MaxTextValueBytesPerValue)
                throw new MimeException(Resources.Strings.TooManyTextValueBytes(currentTextHeaderBytes, this.MimeLimits.MaxTextValueBytesPerValue));
        }

        private void CheckPartsLimit() {
            if (++partCount > this.MimeLimits.MaxParts)
                throw new MimeException(Resources.Strings.TooManyParts(partCount, this.MimeLimits.MaxParts));
            if (this.PartDepth > this.MimeLimits.MaxPartDepth)
                throw new MimeException(Resources.Strings.PartNestingTooDeep(this.PartDepth, this.MimeLimits.MaxPartDepth));
            if (embeddedDepth > this.MimeLimits.MaxEmbeddedDepth)
                throw new MimeException(Resources.Strings.EmbeddedNestingTooDeep(embeddedDepth, this.MimeLimits.MaxEmbeddedDepth));
        }

        internal string ReadRecipientEmail(bool topLevel) {
            string str = null;
            MimeRecipient mimeRecipient;
            if (topLevel) {
                mimeRecipient = currentChild as MimeRecipient;
                if (mimeRecipient == null)
                    throw new NotSupportedException(Resources.Strings.CurrentAddressIsGroupAndCannotHaveEmail);
            } else
                mimeRecipient = currentGrandChild as MimeRecipient;
            if (mimeRecipient != null)
                str = mimeRecipient.Email;
            return str;
        }

        internal bool TryReadDisplayName(bool topLevel, DecodingOptions decodingOptions, out DecodingResults decodingResults, out string displayName) {
            return (!topLevel ? currentGrandChild as AddressItem : currentChild as AddressItem).TryGetDisplayName(decodingOptions, out decodingResults, out displayName);
        }

        internal string ReadParameterName() {
            return (currentChild as MimeParameter).Name;
        }

        internal bool TryReadParameterValue(DecodingOptions decodingOptions, out DecodingResults decodingResults, out string value) {
            return (currentChild as MimeParameter).TryGetValue(decodingOptions, out decodingResults, out value);
        }

        internal int AddMoreData(byte[] buffer, int offset, int length, bool endOfFile) {
            this.CompactDataBuffer();
            int count;
            if (length != 0) {
                count = Math.Min(length, data.Length - (dataOffset + dataCount) - 2);
                Buffer.BlockCopy(buffer, offset, data, dataOffset + dataCount, count);
                length -= count;
                dataCount += count;
            } else
                count = 0;
            dataEOF = length == 0 && endOfFile;
            return count;
        }

        private bool ReadMoreData() {
            this.CompactDataBuffer();
            var offset = dataOffset + dataCount;
            var num = mimeStream.Read(data, offset, data.Length - offset - 2);
            if (num == 0) {
                if (!eofMeansEndOfFile)
                    return false;
                dataEOF = true;
                return true;
            }
            dataCount += num;
            return true;
        }

        private void CompactDataBuffer() {
            if (dataCount == 0)
                dataOffset = 0;
            else {
                if (data.Length - dataOffset + dataCount >= data.Length/2)
                    return;
                Buffer.BlockCopy(data, dataOffset, data, 0, dataCount);
                dataOffset = 0;
            }
        }

        internal void Write(byte[] buffer, int offset, int length) {
            if (dataEOF)
                throw new InvalidOperationException(Resources.Strings.CannotWriteAfterFlush);
            while (true) {
                while (currentToken.Id == MimeTokenId.None && this.ReaderState != MimeReaderState.Start) {
                    this.ParseAndCheckSize();
                    if (currentToken.Id == MimeTokenId.None) {
                        if (length == 0)
                            return;
                        var num = this.AddMoreData(buffer, offset, length, false);
                        offset += num;
                        length -= num;
                    } else
                        break;
                }
                this.HandleTokenInPushMode();
            }
        }

        internal void Flush() {
            if (dataEOF)
                return;
            dataEOF = true;
            do {
                if (currentToken.Id == MimeTokenId.None && this.ReaderState != MimeReaderState.Start)
                    this.ParseAndCheckSize();
                this.HandleTokenInPushMode();
            } while (this.ReaderState != MimeReaderState.End);
        }

        private void HandleTokenInPushMode() {
            if (MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.PartBody | MimeReaderState.InlineBody) && !skipPart &&
                (currentToken.Id == MimeTokenId.PartData || this.ReaderState == MimeReaderState.InlineBody && (currentToken.Id == MimeTokenId.InlineStart || currentToken.Id == MimeTokenId.InlineEnd)))
                handler.PartContent(data, dataOffset, currentToken.Length);
            if (!this.RunStateMachine())
                return;
            switch (this.ReaderState) {
                case MimeReaderState.InlineEnd:
                case MimeReaderState.PartEnd:
                    handler.PartEnd();
                    break;
                case MimeReaderState.End:
                    handler.EndOfFile();
                    break;
                case MimeReaderState.InlineStart:
                case MimeReaderState.PartStart:
                    skipPart = false;
                    skipHeaders = false;
                    PartParseOptionInternal partParseOption;
                    System.IO.Stream outerContentWriteStream;
                    handler.PartStart(this.ReaderState == MimeReaderState.InlineStart, this.ReaderState == MimeReaderState.InlineStart ? this.InlineFileName : null, out partParseOption, out outerContentWriteStream);
                    if (outerContentWriteStream != null) {
                        if (outerContentStream != null)
                            throw new NotSupportedException(Resources.Strings.MimeHandlerErrorMoreThanOneOuterContentPushStream);
                        outerContentStream = outerContentWriteStream;
                        outerContentDepth = depth;
                    }
                    if (partParseOption == PartParseOptionInternal.Skip) {
                        skipPart = true;
                        parser.SetStreamMode();
                        this.ReaderState = this.ReaderState == MimeReaderState.InlineStart ? MimeReaderState.InlineBody : MimeReaderState.PartBody;
                    } else if (partParseOption == PartParseOptionInternal.ParseSkipHeaders)
                        skipHeaders = true;
                    else if (partParseOption == PartParseOptionInternal.ParseRawOuterContent) {
                        parser.SetStreamMode();
                        this.ReaderState = this.ReaderState == MimeReaderState.InlineStart ? MimeReaderState.InlineBody : MimeReaderState.PartBody;
                    }
                    if (skipPart || this.ReaderState != MimeReaderState.InlineStart)
                        break;
                    goto case MimeReaderState.EndOfHeaders;
                case MimeReaderState.EndOfHeaders:
                    PartContentParseOptionInternal partContentParseOption;
                    handler.EndOfHeaders(parser.IsMime ? currentPartContentType : (this.ReaderState == MimeReaderState.InlineStart ? "application/octet-stream" : "text/plain"), parser.TransferEncoding, out partContentParseOption);
                    if (partContentParseOption == PartContentParseOptionInternal.Skip) {
                        if (this.ReaderState != MimeReaderState.InlineStart)
                            parser.SetContentType(MajorContentType.Other, new MimeString());
                        this.ReaderState = this.ReaderState == MimeReaderState.InlineStart ? MimeReaderState.InlineBody : MimeReaderState.PartBody;
                        skipPart = true;
                        break;
                    }
                    if (partContentParseOption == PartContentParseOptionInternal.ParseRawContent) {
                        if (this.ReaderState != MimeReaderState.InlineStart)
                            parser.SetContentType(MajorContentType.Other, new MimeString());
                        this.ReaderState = this.ReaderState == MimeReaderState.InlineStart ? MimeReaderState.InlineBody : MimeReaderState.PartBody;
                        break;
                    }
                    if (partContentParseOption == PartContentParseOptionInternal.ParseEmbeddedMessage) {
                        if (currentPartMajorType == MajorContentType.MessageRfc822)
                            break;
                        throw new NotSupportedException(Resources.Strings.MimeHandlerErrorNotEmbeddedMessage);
                    }
                    if (currentPartMajorType == MajorContentType.MessageRfc822)
                        parser.SetContentType(MajorContentType.Other, new MimeString());
                    if (currentPartMajorType == MajorContentType.Multipart && parser.IsMime)
                        break;
                    this.ReaderState = this.ReaderState == MimeReaderState.InlineStart ? MimeReaderState.InlineBody : MimeReaderState.PartBody;
                    break;
                case MimeReaderState.HeaderStart:
                    if (!skipHeaders) {
                        HeaderParseOptionInternal headerParseOption;
                        handler.HeaderStart(currentHeaderId, currentHeaderName, out headerParseOption);
                        skipHeader = headerParseOption == HeaderParseOptionInternal.Skip;
                        if (skipHeader)
                            break;
                        createHeader = true;
                        break;
                    }
                    skipHeader = true;
                    break;
                case MimeReaderState.HeaderComplete:
                    if (this.CurrentHeaderObject == null || skipHeader)
                        break;
                    handler.Header(this.CurrentHeaderObject);
                    break;
            }
        }

        private bool TrySkipToNextPartBoundary(bool stopAtPartEnd) {
            while (this.ReaderState != MimeReaderState.End) {
                if (!this.TryReachNextState())
                    return false;
                if (MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.PartStart | MimeReaderState.InlineStart) || stopAtPartEnd && MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.PartEnd | MimeReaderState.InlineEnd))
                    break;
            }
            return true;
        }

        internal bool TryReachNextState() {
            while (this.ReaderState != MimeReaderState.End) {
                if (currentToken.Id == MimeTokenId.None) {
                    if (this.ReaderState == MimeReaderState.Start) {
                        this.RunStateMachine();
                        break;
                    }
                    this.ParseAndCheckSize();
                    if (currentToken.Id == MimeTokenId.None) {
                        if (mimeStream == null || !this.ReadMoreData()) {
                            this.DataExhausted = true;
                            return false;
                        }
                        continue;
                    }
                }
                if (this.RunStateMachine())
                    break;
            }
            this.DataExhausted = false;
            return true;
        }

        private bool RunStateMachine() {
            switch (this.ReaderState) {
                case MimeReaderState.InlineJunk:
                    if (currentToken.Id == MimeTokenId.PartData) {
                        this.ConsumeCurrentToken();
                        break;
                    }
                    if (currentToken.Id == MimeTokenId.InlineStart) {
                        ++depth;
                        this.StartPart();
                        this.ParseInlineFileName();
                        this.ReaderState = MimeReaderState.InlineStart;
                        return true;
                    }
                    if (currentToken.Id == MimeTokenId.EmbeddedEnd && parseEmbeddedMessages) {
                        this.ConsumeCurrentToken();
                        this.EndPart();
                        this.ReaderState = MimeReaderState.PartEnd;
                        return true;
                    }
                    this.ReaderState = MimeReaderState.End;
                    return true;
                case MimeReaderState.EmbeddedEnd:
                    if (currentToken.Id == MimeTokenId.EmbeddedEnd) {
                        if (cleanupDepth == 0) {
                            this.ConsumeCurrentToken();
                            this.EndPart();
                            this.ReaderState = MimeReaderState.PartEnd;
                            return true;
                        }
                        --cleanupDepth;
                    } else if (currentToken.Id == MimeTokenId.InlineStart || currentToken.Id == MimeTokenId.NestedStart)
                        ++cleanupDepth;
                    else if (currentToken.Id == MimeTokenId.InlineEnd || currentToken.Id == MimeTokenId.NestedEnd)
                        --cleanupDepth;
                    this.ConsumeCurrentToken();
                    break;
                case MimeReaderState.InlineBody:
                    if (currentToken.Id == MimeTokenId.InlineEnd) {
                        this.ConsumeCurrentToken();
                        this.EndPart();
                        this.ReaderState = MimeReaderState.InlineEnd;
                        return true;
                    }
                    this.ConsumeCurrentToken();
                    break;
                case MimeReaderState.InlineEnd:
                    --depth;
                    this.ReaderState = MimeReaderState.InlineJunk;
                    goto case MimeReaderState.InlineJunk;
                case MimeReaderState.PartEpilogue:
                    if (currentToken.Id == MimeTokenId.PartData) {
                        this.ConsumeCurrentToken();
                        break;
                    }
                    this.EndPart();
                    this.ReaderState = MimeReaderState.PartEnd;
                    return true;
                case MimeReaderState.PartEnd:
                    if (currentToken.Id == MimeTokenId.NestedNext) {
                        this.StartPart();
                        this.ConsumeCurrentToken();
                        this.ReaderState = MimeReaderState.PartStart;
                        return true;
                    }
                    if (currentToken.Id == MimeTokenId.NestedEnd) {
                        this.ConsumeCurrentToken();
                        --depth;
                        this.ReaderState = MimeReaderState.PartEpilogue;
                        break;
                    }
                    if (currentToken.Id == MimeTokenId.InlineStart) {
                        this.StartPart();
                        this.ParseInlineFileName();
                        this.ReaderState = MimeReaderState.InlineStart;
                        return true;
                    }
                    if (currentToken.Id == MimeTokenId.EmbeddedEnd && parseEmbeddedMessages) {
                        this.ConsumeCurrentToken();
                        --depth;
                        --embeddedDepth;
                        this.EndPart();
                        return true;
                    }
                    --depth;
                    this.ReaderState = MimeReaderState.End;
                    return true;
                case MimeReaderState.InlineStart:
                    this.ReaderState = MimeReaderState.InlineBody;
                    return true;
                case MimeReaderState.EndOfHeaders:
                    if (currentToken.Id == MimeTokenId.EmbeddedStart) {
                        this.ConsumeCurrentToken();
                        if (parseEmbeddedMessages) {
                            ++depth;
                            ++embeddedDepth;
                            this.StartPart();
                            this.ReaderState = MimeReaderState.PartStart;
                            return true;
                        }
                        this.ReaderState = MimeReaderState.Embedded;
                        return true;
                    }
                    if (currentToken.Id == MimeTokenId.NestedStart) {
                        ++depth;
                        this.StartPart();
                        this.ConsumeCurrentToken();
                        this.ReaderState = MimeReaderState.PartStart;
                        return true;
                    }
                    if (currentToken.Id == MimeTokenId.PartData) {
                        if (parser.ContentType == MajorContentType.Multipart) {
                            this.ReaderState = MimeReaderState.PartPrologue;
                            goto case MimeReaderState.PartPrologue;
                        }
                        this.ReaderState = MimeReaderState.PartBody;
                        return true;
                    }
                    this.EndPart();
                    this.ReaderState = MimeReaderState.PartEnd;
                    return true;
                case MimeReaderState.PartPrologue:
                    if (currentToken.Id == MimeTokenId.NestedStart) {
                        ++depth;
                        this.StartPart();
                        this.ConsumeCurrentToken();
                        this.ReaderState = MimeReaderState.PartStart;
                        return true;
                    }
                    if (currentToken.Id == MimeTokenId.PartData) {
                        this.ConsumeCurrentToken();
                        break;
                    }
                    this.EndPart();
                    this.ReaderState = MimeReaderState.PartEnd;
                    return true;
                case MimeReaderState.PartBody:
                    if (currentToken.Id == MimeTokenId.PartData) {
                        this.ConsumeCurrentToken();
                        break;
                    }
                    this.EndPart();
                    this.ReaderState = MimeReaderState.PartEnd;
                    return true;
                case MimeReaderState.Start:
                    ++depth;
                    this.StartPart();
                    this.ReaderState = MimeReaderState.PartStart;
                    return true;
                case MimeReaderState.PartStart:
                case MimeReaderState.HeaderComplete:
                    if (currentToken.Id == MimeTokenId.Header) {
                        this.StartHeader();
                        this.ReaderState = MimeReaderState.HeaderStart;
                        return true;
                    }
                    this.ConsumeCurrentToken();
                    this.ReaderState = MimeReaderState.EndOfHeaders;
                    return true;
                case MimeReaderState.HeaderStart:
                    this.CreateHeader();
                    this.ContinueHeader();
                    if (parser.IsHeaderComplete) {
                        this.EndHeader();
                        this.ConsumeCurrentToken();
                        this.ReaderState = MimeReaderState.HeaderComplete;
                        return true;
                    }
                    this.ConsumeCurrentToken();
                    this.ReaderState = MimeReaderState.HeaderIncomplete;
                    break;
                case MimeReaderState.HeaderIncomplete:
                    if (currentToken.Id == MimeTokenId.HeaderContinuation) {
                        this.ContinueHeader();
                        this.ConsumeCurrentToken();
                        if (parser.IsHeaderComplete) {
                            this.EndHeader();
                            this.ReaderState = MimeReaderState.HeaderComplete;
                            return true;
                        }
                        break;
                    }
                    this.EndHeader();
                    this.ReaderState = MimeReaderState.HeaderComplete;
                    return true;
                default:
                    throw new InvalidOperationException();
            }
            return false;
        }

        private void ConsumeCurrentToken() {
            if (currentToken.Length != 0) {
                if (outerContentStream != null)
                    outerContentStream.Write(data, dataOffset, currentToken.Length);
                this.LineTerminationState = MimeCommon.AdvanceLineTerminationState(this.LineTerminationState, data, dataOffset, currentToken.Length);
                parser.ReportConsumedData(currentToken.Length);
                dataOffset += currentToken.Length;
                dataCount -= currentToken.Length;
                currentToken.Length = 0;
            }
            currentToken.Id = MimeTokenId.None;
        }

        private void StartPart() {
            this.CheckPartsLimit();
            currentPartMajorType = MajorContentType.Other;
            currentPartContentType = null;
            currentPartContentTransferEncoding = ContentTransferEncoding.SevenBit;
            enableReadingOuterContent = false;
            this.CurrentHeaderObject = null;
            createHeader = false;
            inlineFileName = new MimeString();
            decoder = null;
            contentStream = null;
        }

        private void EndPart() {
            if (outerContentStream == null || depth != outerContentDepth)
                return;
            outerContentStream.Flush();
            outerContentStream = null;
        }

        private void ParseInlineFileName() {
            currentPartContentType = "application/octet-stream";
            currentPartContentTransferEncoding = parser.InlineFormat;
            if (parser.InlineFormat == ContentTransferEncoding.UUEncode) {
                var offset1 = dataOffset + 6;
                while (offset1 < dataOffset + currentToken.Length && (data[offset1] >= 48 && data[offset1] <= 55))
                    ++offset1;
                var offset2 = offset1 + MimeScan.SkipLwsp(data, offset1, dataOffset + currentToken.Length - offset1);
                var count = this.TrimEndOfLine(offset2, dataOffset + currentToken.Length - offset2);
                inlineFileName = new MimeString(data, offset2, count);
            } else
                inlineFileName = new MimeString();
        }

        private void StartHeader() {
            if (++headerCount > this.MimeLimits.MaxHeaders)
                throw new MimeException(Resources.Strings.TooManyHeaders(headerCount, this.MimeLimits.MaxHeaders));
            currentHeaderId = parser.HeaderNameLength == 0 ? HeaderId.Unknown : Header.GetHeaderId(data, dataOffset, parser.HeaderNameLength);
            currentHeaderName = parser.HeaderNameLength == 0 ? null : Internal.ByteString.BytesToString(data, dataOffset, parser.HeaderNameLength, false);
            this.CurrentHeaderObject = null;
            var flag = false;
            if (currentHeaderId == HeaderId.ContentType || currentHeaderId == HeaderId.ContentTransferEncoding || currentHeaderId == HeaderId.MimeVersion)
                flag = true;
            else if (this.MimeLimits.MaxAddressItemsPerHeader < int.MaxValue || this.MimeLimits.MaxParametersPerHeader < int.MaxValue || this.MimeLimits.MaxTextValueBytesPerValue < int.MaxValue) {
                var type = Header.TypeFromHeaderId(currentHeaderId);
                if (type == typeof (AddressHeader) && (this.MimeLimits.MaxParametersPerHeader < int.MaxValue || this.MimeLimits.MaxAddressItemsPerHeader < int.MaxValue) ||
                    (type == typeof (ContentTypeHeader) || type == typeof (ContentDispositionHeader)) && (this.MimeLimits.MaxParametersPerHeader < int.MaxValue || this.MimeLimits.MaxAddressItemsPerHeader < int.MaxValue))
                    flag = true;
            }
            createHeader = flag;
        }

        private void CreateHeader() {
            this.CurrentHeaderObject = !createHeader || parser.HeaderNameLength == 0 ? null : (currentHeaderId == HeaderId.Unknown ? Header.CreateGeneralHeader(currentHeaderName) : Header.Create(currentHeaderName, currentHeaderId));
            currentHeaderEmpty = true;
        }

        private void ContinueHeader() {
            this.CheckHeaderBytesLimits();
            if (this.CurrentHeaderObject == null)
                return;
            var offset = dataOffset + parser.HeaderDataOffset;
            var length = currentToken.Length - parser.HeaderDataOffset;
            var num1 = this.TrimEndOfLine(offset, length);
            if (currentHeaderEmpty) {
                var num2 = MimeScan.SkipLwsp(data, offset, num1);
                offset += num2;
                num1 -= num2;
            }
            if (num1 == 0)
                return;
            currentHeaderEmpty = false;
            this.CurrentHeaderObject.AppendLine(MimeString.CopyData(data, offset, num1), false);
        }

        private void EndHeader() {
            if (this.CurrentHeaderObject == null)
                return;
            if (this.CurrentHeaderObject is ComplexHeader) {
                if (this.MimeLimits.MaxParametersPerHeader < int.MaxValue || this.MimeLimits.MaxTextValueBytesPerValue < int.MaxValue)
                    this.CurrentHeaderObject.CheckChildrenLimit(this.MimeLimits.MaxParametersPerHeader, this.MimeLimits.MaxTextValueBytesPerValue);
            } else if (this.CurrentHeaderObject is AddressHeader && (this.MimeLimits.MaxAddressItemsPerHeader < int.MaxValue || this.MimeLimits.MaxTextValueBytesPerValue < int.MaxValue))
                this.CurrentHeaderObject.CheckChildrenLimit(this.MimeLimits.MaxAddressItemsPerHeader, this.MimeLimits.MaxTextValueBytesPerValue);
            if (this.CurrentHeaderObject.HeaderId == HeaderId.MimeVersion && this.PartDepth == 1)
                parser.SetMIME();
            else if (this.CurrentHeaderObject.HeaderId == HeaderId.ContentTransferEncoding) {
                if (inferMime && this.PartDepth == 1)
                    parser.SetMIME();
                var encodingType = MimePart.GetEncodingType(this.CurrentHeaderObject.FirstRawToken);
                parser.SetTransferEncoding(encodingType);
                currentPartContentTransferEncoding = encodingType;
            } else {
                if (this.CurrentHeaderObject.HeaderId != HeaderId.ContentType)
                    return;
                var contentType = MajorContentType.Other;
                string str = null;
                var boundaryValue = new MimeString();
                var contentTypeHeader = this.CurrentHeaderObject as ContentTypeHeader;
                if (contentTypeHeader != null) {
                    if (contentTypeHeader.IsMultipart) {
                        var mimeParameter = contentTypeHeader["boundary"];
                        if (mimeParameter != null) {
                            var rawValue = mimeParameter.RawValue;
                            var count = 0;
                            if (rawValue != null && (count = rawValue.Length) != 0) {
                                while (count != 0 && MimeScan.IsLWSP(rawValue[count - 1]))
                                    --count;
                                if (count != 0 && count <= 994) {
                                    if (FixBadMimeBoundary) {
                                        var index = 0;
                                        if (count == rawValue.Length && count <= 70) {
                                            while (index < count && MimeScan.IsBChar(rawValue[index]))
                                                ++index;
                                        }
                                        if (index != count) {
                                            parser.ComplianceStatus |= MimeComplianceStatus.InvalidBoundary;
                                            mimeParameter.RawValue = ContentTypeHeader.CreateBoundary();
                                        }
                                    }
                                    contentType = MajorContentType.Multipart;
                                    boundaryValue = new MimeString(rawValue, 0, count);
                                }
                            }
                            if (rawValue == null || count == 0 || count > 994) {
                                parser.ComplianceStatus |= MimeComplianceStatus.InvalidBoundary;
                                contentTypeHeader.Value = "text/plain";
                            }
                        } else {
                            parser.ComplianceStatus |= MimeComplianceStatus.MissingBoundaryParameter;
                            contentTypeHeader.Value = "text/plain";
                        }
                    } else if (contentTypeHeader.IsEmbeddedMessage)
                        contentType = MajorContentType.MessageRfc822;
                    else if (contentTypeHeader.IsAnyMessage)
                        contentType = MajorContentType.Message;
                    str = contentTypeHeader.Value;
                }
                if (inferMime && this.PartDepth == 1)
                    parser.SetMIME();
                currentPartMajorType = contentType;
                currentPartContentType = str;
                if (contentType == MajorContentType.Multipart || parseEmbeddedMessages)
                    parser.SetContentType(contentType, boundaryValue);
                else
                    parser.SetContentType(MajorContentType.Other, new MimeString());
            }
        }

        internal bool ReadPartData(byte[] buffer, int offset, int count, out int readCount) {
            readCount = 0;
            this.DataExhausted = false;
            var completed = true;
            while (count != 0) {
                if (currentToken.Id == MimeTokenId.None) {
                    this.ParseAndCheckSize();
                    if (currentToken.Id == MimeTokenId.None) {
                        if (mimeStream == null || !this.ReadMoreData()) {
                            this.DataExhausted = true;
                            return false;
                        }
                        continue;
                    }
                }
                int inputUsed;
                int outputUsed;
                if (currentToken.Id != MimeTokenId.PartData && currentToken.Id != MimeTokenId.InlineStart && currentToken.Id != MimeTokenId.InlineEnd ||
                    this.ReaderState == MimeReaderState.PartBody && currentToken.Id == MimeTokenId.InlineStart) {
                    if (decoder != null) {
                        decoder.Convert(data, 0, 0, buffer, offset, count, true, out inputUsed, out outputUsed, out completed);
                        count -= outputUsed;
                        offset += outputUsed;
                        readCount += outputUsed;
                        if (!completed)
                            break;
                    }
                    this.EndPart();
                    this.ReaderState = MimeReaderState.PartEnd;
                    break;
                }
                if (decoder != null) {
                    decoder.Convert(data, dataOffset, currentToken.Length, buffer, offset, count, currentToken.Id == MimeTokenId.InlineEnd, out inputUsed, out outputUsed, out completed);
                    count -= outputUsed;
                    offset += outputUsed;
                    readCount += outputUsed;
                } else {
                    inputUsed = outputUsed = Math.Min(count, currentToken.Length);
                    if (outputUsed != 0) {
                        if (buffer != null) {
                            Buffer.BlockCopy(data, dataOffset, buffer, offset, outputUsed);
                            count -= outputUsed;
                            offset += outputUsed;
                        }
                        readCount += outputUsed;
                    }
                }
                if (inputUsed != 0) {
                    if (outerContentStream != null)
                        outerContentStream.Write(data, dataOffset, inputUsed);
                    this.LineTerminationState = MimeCommon.AdvanceLineTerminationState(this.LineTerminationState, data, dataOffset, inputUsed);
                    parser.ReportConsumedData(inputUsed);
                    dataOffset += inputUsed;
                    dataCount -= inputUsed;
                    currentToken.Length -= (short) inputUsed;
                }
                if (currentToken.Length == 0) {
                    if (currentToken.Id == MimeTokenId.InlineEnd) {
                        if (completed) {
                            this.EndPart();
                            currentToken.Id = MimeTokenId.None;
                            this.ReaderState = MimeReaderState.InlineEnd;
                        }
                        break;
                    }
                    currentToken.Id = MimeTokenId.None;
                } else
                    break;
            }
            return true;
        }

        internal static bool StateIsOneOf(MimeReaderState state, MimeReaderState set) {
            return (state & set) != 0;
        }

        internal void AssertGoodToUse(bool pullModeOnly, bool noEmbeddedReader) {
            if (parser == null)
                throw new ObjectDisposedException("MimeReader");
            if (pullModeOnly && mimeStream == null)
                throw new ObjectDisposedException("MimeReader");
            if (noEmbeddedReader && childReader != null)
                throw new InvalidOperationException(Resources.Strings.EmbeddedMessageReaderNeedsToBeClosedFirst);
        }

        internal void SetEofMeansEndOfFile(bool eofMeansEndOfFile) {
            this.eofMeansEndOfFile = eofMeansEndOfFile;
        }

        internal bool TryReadNextPart() {
            this.AssertGoodToUse(false, true);
            if (!this.TrySkipToNextPartBoundary(false))
                return false;
            return MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.PartStart | MimeReaderState.InlineStart);
        }

        internal bool TryReadFirstChildPart() {
            this.AssertGoodToUse(false, true);
            if (this.ReaderState == MimeReaderState.InlineStart)
                return false;
            if (!MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.Start | MimeReaderState.EndOfHeaders | MimeReaderState.PartPrologue)) {
                if (!MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderIncomplete | MimeReaderState.HeaderComplete))
                    throw new InvalidOperationException(Resources.Strings.OperationNotValidInThisReaderState);
                while (this.TryReachNextState()) {
                    if (this.ReaderState == MimeReaderState.EndOfHeaders)
                        goto label_8;
                }
                return false;
            }
            label_8:
            if (this.ReaderState == MimeReaderState.EndOfHeaders && !this.IsMultipart && (!this.IsEmbeddedMessage || !parseEmbeddedMessages) || !this.TrySkipToNextPartBoundary(true))
                return false;
            return this.ReaderState == MimeReaderState.PartStart;
        }

        internal bool TryReadNextSiblingPart() {
            this.AssertGoodToUse(false, true);
            if (this.ReaderState == MimeReaderState.End)
                return false;
            if (MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderIncomplete | MimeReaderState.HeaderComplete | MimeReaderState.EndOfHeaders)) {
                createHeader = false;
                while (this.ReaderState != MimeReaderState.EndOfHeaders) {
                    if (!this.TryReachNextState())
                        return false;
                }
                parser.SetContentType(MajorContentType.Other, new MimeString());
            }
            if (!MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.Start | MimeReaderState.PartEnd | MimeReaderState.InlineEnd)) {
                var num = depth;
                while (this.TrySkipToNextPartBoundary(true)) {
                    if (depth <= num && MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.PartEnd | MimeReaderState.InlineEnd))
                        goto label_13;
                }
                return false;
            }
            label_13:
            if (!this.TrySkipToNextPartBoundary(true))
                return false;
            return MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.PartStart | MimeReaderState.InlineStart);
        }

        internal HeaderList TryReadHeaderList() {
            this.AssertGoodToUse(false, true);
            if (!MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.PartStart | MimeReaderState.InlineStart) &&
                (!MimeReader.StateIsOneOf(this.ReaderState, MimeReaderState.HeaderStart | MimeReaderState.HeaderIncomplete | MimeReaderState.HeaderComplete) || this.headerList == null))
                throw new InvalidOperationException(Resources.Strings.OperationNotValidInThisReaderState);
            if (this.ReaderState == MimeReaderState.InlineStart)
                return new HeaderList(null);
            HeaderList headerList;
            if (this.headerList == null)
                headerList = new HeaderList(null);
            else {
                headerList = this.headerList;
                this.headerList = null;
            }
            while (this.TryReachNextState()) {
                if (this.ReaderState == MimeReaderState.HeaderStart)
                    createHeader = true;
                else if (this.ReaderState == MimeReaderState.HeaderComplete && this.CurrentHeaderObject != null)
                    headerList.InternalAppendChild(this.CurrentHeaderObject);
                if (this.ReaderState == MimeReaderState.EndOfHeaders)
                    return headerList;
            }
            this.headerList = headerList;
            return null;
        }

        internal System.IO.Stream TryGetRawContentReadStream() {
            this.AssertGoodToUse(false, true);
            if (!this.TryInitializeReadContent(false))
                return null;
            contentStream = new ContentReadStream(this);
            return contentStream;
        }

        private const int DataBufferSize = 5120;
        private readonly bool inferMime;
        private readonly MimeLimits limits;
        private readonly bool parseEmbeddedMessages;
        private MimeReader childReader;
        private int cleanupDepth;
        private System.IO.Stream contentStream;
        private bool createHeader;
        private int cumulativeHeaderBytes;
        private MimeNode currentChild;
        private bool currentChildConsumed;
        private MimeNode currentGrandChild;
        private bool currentHeaderConsumed;
        private bool currentHeaderEmpty;
        private HeaderId currentHeaderId;
        private string currentHeaderName;
        private ContentTransferEncoding currentPartContentTransferEncoding;
        private string currentPartContentType;
        private MajorContentType currentPartMajorType;
        private int currentTextHeaderBytes;
        private MimeToken currentToken;
        private byte[] data;
        private int dataCount;
        private bool dataEOF;
        private int dataOffset;
        private Encoders.ByteEncoder decoder;
        private int depth;
        private int embeddedDepth;
        private bool enableReadingOuterContent;
        private bool eofMeansEndOfFile = true;
        private bool FixBadMimeBoundary = true;
        private IMimeHandlerInternal handler;
        private int headerCount;
        private HeaderList headerList;
        private MimeString inlineFileName;
        private System.IO.Stream mimeStream;
        private int outerContentDepth;
        private System.IO.Stream outerContentStream;
        private MimeReader parentReader;
        private MimeParser parser;
        private int partCount = 1;
        private bool readRawData;
        private bool skipHeader;
        private bool skipHeaders;
        private bool skipPart;


        private class ContentReadStream : System.IO.Stream {

            public ContentReadStream(MimeReader reader) {
                this.reader = reader;
            }

            public override bool CanRead => reader != null;
            public override bool CanWrite => false;
            public override bool CanSeek => false;

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

            public override int Read(byte[] buffer, int offset, int count) {
                MimeCommon.CheckBufferArguments(buffer, offset, count);
                if (reader.contentStream != this)
                    throw new NotSupportedException(Resources.Strings.StreamNoLongerValid);
                if (!MimeReader.StateIsOneOf(reader.ReaderState, MimeReaderState.PartBody | MimeReaderState.InlineBody)) {
                    if (MimeReader.StateIsOneOf(reader.ReaderState, MimeReaderState.PartEnd | MimeReaderState.InlineEnd))
                        return 0;
                    throw new NotSupportedException(Resources.Strings.StreamNoLongerValid);
                }
                int readCount;
                reader.ReadPartData(buffer, offset, count, out readCount);
                return readCount;
            }

            public override void Write(byte[] buffer, int offset, int count) {
                throw new NotSupportedException();
            }

            public override long Seek(long offset, System.IO.SeekOrigin origin) {
                throw new NotSupportedException();
            }

            public override void Flush() {
                throw new NotSupportedException();
            }

            public override void SetLength(long value) {
                throw new NotSupportedException();
            }

            protected override void Dispose(bool disposing) {
                base.Dispose(disposing);
            }

            private readonly MimeReader reader;

        }

    }

}