using System.Linq;

namespace Butler.Schema.Mime {

    public class MimeWriter : System.IDisposable {

        public MimeWriter(System.IO.Stream data)
            : this(data, true, MimeCommon.DefaultEncodingOptions) {}

        public MimeWriter(System.IO.Stream data, bool forceMime, EncodingOptions encodingOptions) {
            if (data == null)
                throw new System.ArgumentNullException(nameof(data));
            if (!data.CanWrite)
                throw new System.ArgumentException("Stream must support writing", nameof(data));
            this.forceMime = forceMime;
            this.data = data;
            this.encodingOptions = encodingOptions;
            shimStream = new WriterQueueStream(this);
        }

        public int PartDepth {
            get {
                this.AssertOpen();
                return partDepth;
            }
        }

        public int EmbeddedDepth {
            get {
                this.AssertOpen();
                return embeddedDepth;
            }
        }

        public int StreamOffset {
            get {
                this.AssertOpen();
                return bytesWritten;
            }
        }

        public EncodingOptions EncodingOptions {
            get {
                this.AssertOpen();
                return encodingOptions;
            }
        }

        public string PartBoundary {
            get {
                this.AssertOpen();
                string str = null;
                switch (state) {
                    case MimeWriteState.StartPart:
                    case MimeWriteState.Headers:
                        if (currentPart.IsMultipart)
                            str = Internal.ByteString.BytesToString(currentPart.Boundary, false);
                        break;
                }
                return str;
            }
        }

        public void Dispose() {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (data == null)
                return;
            try {
                if (!disposing)
                    return;
                while (partDepth != 0)
                    this.EndPart();
                this.FlushWriteQueue();
                if (lineTermination == LineTerminationState.CRLF)
                    return;
                if (lineTermination == LineTerminationState.CR)
                    data.Write(MimeString.CrLf, 1, 1);
                else
                    data.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
                lineTermination = LineTerminationState.CRLF;
            } finally {
                if (disposing) {
                    if (encodedPartContent != null)
                        encodedPartContent.Dispose();
                    if (partContent != null)
                        partContent.Dispose();
                    data.Dispose();
                }
                state = MimeWriteState.Complete;
                encodedPartContent = null;
                partContent = null;
                data = null;
            }
        }

        public virtual void Close() {
            this.Dispose();
        }

        public void WritePart(MimeReader reader) {
            if (reader == null)
                throw new System.ArgumentNullException(nameof(reader));
            this.AssertOpen();
            if (!MimeReader.StateIsOneOf(reader.ReaderState, MimeReaderState.PartStart | MimeReaderState.InlineStart))
                throw new System.InvalidOperationException(Resources.Strings.OperationNotValidInThisReaderState);
            this.StartPart();
            var headerReader = reader.HeaderReader;
            while (headerReader.ReadNextHeader())
                this.WriteHeader(headerReader);
            this.WriteContent(reader);
            this.EndPart();
        }

        public void WriteHeader(MimeHeaderReader reader) {
            this.AssertOpen();
            Header.ReadFrom(reader)
                  .WriteTo(this);
        }

        public void WriteAddress(MimeAddressReader reader) {
            this.AssertOpen();
            if (reader.IsGroup) {
                this.StartGroup(reader.DisplayName);
                var groupRecipientReader = reader.GroupRecipientReader;
                while (groupRecipientReader.ReadNextAddress()) {
                    var displayName = groupRecipientReader.DisplayName;
                    var email = groupRecipientReader.Email;
                    if (displayName == null || email == null)
                        throw new ButlerSchemaException(Resources.Strings.AddressReaderIsNotPositionedOnAddress);
                    this.WriteRecipient(displayName, email);
                }
                this.EndGroup();
            } else
                this.WriteRecipient(reader.DisplayName, reader.Email);
        }

        public void WriteParameter(MimeParameterReader reader) {
            this.AssertOpen();
            this.WriteParameter(reader.Name, reader.Value);
        }

        public void WriteContent(MimeReader reader) {
            if (reader == null)
                throw new System.ArgumentNullException(nameof(reader));
            this.AssertOpen();
            if (contentWritten)
                throw new System.InvalidOperationException(Resources.Strings.ContentAlreadyWritten);
            using (var contentReadStream = reader.GetRawContentReadStream()) {
                if (contentReadStream == null)
                    return;
                using (var contentWriteStream = this.GetRawContentWriteStream())
                    Internal.DataStorage.CopyStreamToStream(contentReadStream, contentWriteStream, long.MaxValue, ref scratchBuffer);
            }
        }

        public void StartHeader(string name) {
            this.AssertOpen();
            this.WriteHeader(Header.Create(name));
        }

        public void StartHeader(HeaderId headerId) {
            this.AssertOpen();
            this.WriteHeader(Header.Create(headerId));
        }

        public void WriteHeaderValue(string value) {
            this.AssertOpen();
            if (headerValueWritten)
                throw new System.InvalidOperationException(Resources.Strings.CannotWriteHeaderValueMoreThanOnce);
            if (lastHeader == null)
                throw new System.InvalidOperationException(Resources.Strings.CannotWriteHeaderValueHere);
            headerValueWritten = true;
            if (value == null)
                return;
            if (!(lastHeader is TextHeader))
                lastHeader.RawValue = Internal.ByteString.StringToBytes(value, encodingOptions.AllowUTF8);
            else
                lastHeader.Value = value;
        }

        public void WriteHeaderValue(System.DateTime value) {
            this.AssertOpen();
            if (headerValueWritten)
                throw new System.InvalidOperationException(Resources.Strings.CannotWriteHeaderValueMoreThanOnce);
            if (lastHeader == null)
                throw new System.InvalidOperationException(Resources.Strings.CannotWriteHeaderValueHere);
            headerValueWritten = true;
            var timeZoneOffset = System.TimeSpan.Zero;
            var utcDateTime = value.ToUniversalTime();
            if (value.Kind != System.DateTimeKind.Utc)
                timeZoneOffset = System.TimeZoneInfo.Local.GetUtcOffset(value);
            Header.WriteName(shimStream, lastHeader.Name, ref scratchBuffer);
            var currentLineLength = new MimeStringLength(0);
            DateHeader.WriteDateHeaderValue(shimStream, utcDateTime, timeZoneOffset, ref currentLineLength);
            lastHeader = null;
        }

        public void WriteContent(byte[] buffer, int offset, int count) {
            MimeCommon.CheckBufferArguments(buffer, offset, count);
            this.AssertOpen();
            if (contentWritten)
                throw new System.InvalidOperationException(Resources.Strings.ContentAlreadyWritten);
            if (encodedPartContent != null)
                encodedPartContent.Write(buffer, offset, count);
            else {
                using (var contentWriteStream = this.GetContentWriteStream())
                    contentWriteStream.Write(buffer, offset, count);
            }
        }

        public void WriteContent(System.IO.Stream sourceStream) {
            if (sourceStream == null)
                throw new System.ArgumentNullException("stream");
            this.AssertOpen();
            if (contentWritten)
                throw new System.InvalidOperationException(Resources.Strings.ContentAlreadyWritten);
            var stream1 = encodedPartContent;
            System.IO.Stream stream2 = null;
            try {
                if (stream1 == null) {
                    stream2 = this.GetContentWriteStream();
                    stream1 = stream2;
                }
                var buffer = new byte[4096];
                int count;
                while (0 < (count = sourceStream.Read(buffer, 0, 4096)))
                    stream1.Write(buffer, 0, count);
            } finally {
                if (stream2 != null)
                    stream2.Dispose();
            }
        }

        public void WriteRawContent(byte[] buffer, int offset, int count) {
            MimeCommon.CheckBufferArguments(buffer, offset, count);
            this.AssertOpen();
            if (contentWritten)
                throw new System.InvalidOperationException(Resources.Strings.ContentAlreadyWritten);
            (partContent ?? this.GetRawContentWriteStream()).Write(buffer, offset, count);
        }

        public void WriteRawContent(System.IO.Stream sourceStream) {
            if (sourceStream == null)
                throw new System.ArgumentNullException("stream");
            this.AssertOpen();
            if (contentWritten)
                throw new System.InvalidOperationException(Resources.Strings.ContentAlreadyWritten);
            var stream = partContent ?? this.GetRawContentWriteStream();
            var buffer = new byte[4096];
            int count;
            while (0 < (count = sourceStream.Read(buffer, 0, 4096)))
                stream.Write(buffer, 0, count);
        }

        public void EndContent() {
            this.AssertOpen();
            if (encodedPartContent == null)
                return;
            encodedPartContent.Dispose();
            encodedPartContent = null;
            contentWritten = true;
            if (partContent == null)
                return;
            partContent.Dispose();
            partContent = null;
        }

        public void Flush() {
            this.AssertOpen();
            if (state == MimeWriteState.Initial)
                return;
            this.FlushHeader();
            this.EndContent();
            this.FlushWriteQueue();
        }

        internal void WriteMimeNode(MimeNode node) {
            if (node == null)
                throw new System.ArgumentNullException(nameof(node));
            var header1 = node as Header;
            if (header1 != null) {
                this.WriteHeader(header1);
                this.FlushHeader();
            } else {
                var mimePart = node as MimePart;
                if (mimePart != null) {
                    this.StartPart();
                    mimePart.WriteTo(shimStream, encodingOptions);
                    this.EndPart();
                } else {
                    var headerList = node as HeaderList;
                    if (headerList != null) {
                        foreach (var header2 in headerList)
                            this.WriteHeader(header1);
                        this.FlushHeader();
                    } else {
                        node = node.Clone();
                        var recipient = node as MimeRecipient;
                        if (recipient != null)
                            this.WriteRecipient(recipient);
                        else {
                            var parameter = node as MimeParameter;
                            if (parameter != null)
                                this.WriteParameter(parameter);
                            else {
                                var group = node as MimeGroup;
                                if (group == null)
                                    return;
                                this.StartGroup(group);
                                this.EndGroup();
                            }
                        }
                    }
                }
            }
        }

        public void WriteHeader(string name, string value) {
            this.AssertOpen();
            this.StartHeader(name);
            this.WriteHeaderValue(value);
            this.FlushHeader();
        }

        public void WriteHeader(HeaderId headerId, string value) {
            this.AssertOpen();
            this.StartHeader(headerId);
            this.WriteHeaderValue(value);
            this.FlushHeader();
        }

        public void WriteParameter(string name, string value) {
            if (name == null)
                throw new System.ArgumentNullException(nameof(name));
            this.AssertOpen();
            this.WriteParameter(new MimeParameter(name, value));
        }

        public void StartGroup(string name) {
            if (name == null)
                throw new System.ArgumentNullException(nameof(name));
            this.AssertOpen();
            this.StartGroup(new MimeGroup(name));
        }

        public void EndGroup() {
            this.AssertOpen();
            if (state != MimeWriteState.GroupRecipients)
                throw new System.InvalidOperationException(Resources.Strings.CannotWriteGroupEndHere);
            state = MimeWriteState.Recipients;
        }

        public void WriteRecipient(string displayName, string address) {
            if (address == null)
                throw new System.ArgumentNullException(nameof(address));
            this.AssertOpen();
            this.WriteRecipient(new MimeRecipient(displayName, address));
        }

        public MimeWriter GetEmbeddedMessageWriter() {
            this.AssertOpen();
            if (contentWritten)
                throw new System.InvalidOperationException(Resources.Strings.ContentAlreadyWritten);
            switch (state) {
                case MimeWriteState.Initial:
                case MimeWriteState.Complete:
                case MimeWriteState.PartContent:
                case MimeWriteState.EndPart:
                    throw new System.InvalidOperationException(Resources.Strings.CannotWritePartContentNow);
                default:
                    return new MimeWriter(this.GetRawContentWriteStream()) {
                        embeddedDepth = embeddedDepth + 1
                    };
            }
        }

        public System.IO.Stream GetRawContentWriteStream() {
            this.AssertOpen();
            if (contentWritten)
                throw new System.InvalidOperationException(Resources.Strings.ContentAlreadyWritten);
            switch (state) {
                case MimeWriteState.Initial:
                case MimeWriteState.Complete:
                case MimeWriteState.EndPart:
                    throw new System.InvalidOperationException(Resources.Strings.CannotWritePartContentNow);
                case MimeWriteState.StartPart:
                case MimeWriteState.Headers:
                case MimeWriteState.Parameters:
                case MimeWriteState.Recipients:
                case MimeWriteState.GroupRecipients:
                    this.FlushHeader();
                    if (!foundMimeVersion) {
                        if (forceMime && partDepth == 1)
                            this.WriteMimeVersion();
                        else
                            currentPart.IsMultipart = false;
                    }
                    if (MimeWriteState.StartPart != state)
                        this.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
                    break;
                case MimeWriteState.PartContent:
                    return partContent;
            }
            if (currentPart.IsMultipart)
                throw new System.InvalidOperationException(Resources.Strings.MultipartCannotContainContent);
            state = MimeWriteState.PartContent;
            partContent = new WriterContentStream(this);
            return partContent;
        }

        public System.IO.Stream GetContentWriteStream() {
            this.AssertOpen();
            if (contentWritten)
                throw new System.InvalidOperationException(Resources.Strings.ContentAlreadyWritten);
            if (partContent != null)
                throw new System.InvalidOperationException(Resources.Strings.PartContentIsBeingWritten);
            var contentWriteStream = this.GetRawContentWriteStream();
            if (contentTransferEncoding == ContentTransferEncoding.SevenBit || contentTransferEncoding == ContentTransferEncoding.EightBit || contentTransferEncoding == ContentTransferEncoding.Binary)
                return contentWriteStream;
            if (contentTransferEncoding == ContentTransferEncoding.BinHex)
                throw new System.NotSupportedException(Resources.Strings.BinHexNotSupportedForThisMethod);
            var encoder = MimePart.CreateEncoder(null, contentTransferEncoding);
            if (encoder == null)
                throw new System.NotSupportedException(Resources.Strings.UnrecognizedTransferEncodingUsed);
            encodedPartContent = new Schema.Mime.Encoders.EncoderStream(contentWriteStream, encoder, Schema.Mime.Encoders.EncoderStreamAccess.Write);
            return new Internal.SuppressCloseStream(encodedPartContent);
        }

        public void StartPart() {
            this.AssertOpen();
            switch (state) {
                case MimeWriteState.Complete:
                case MimeWriteState.PartContent:
                    throw new System.InvalidOperationException(Resources.Strings.CannotStartPartHere);
                default:
                    if (partDepth != 0) {
                        this.FlushHeader();
                        if (!currentPart.IsMultipart)
                            throw new System.InvalidOperationException(Resources.Strings.NonMultiPartPartsCannotHaveChildren);
                        if (!foundMimeVersion && forceMime && partDepth == 1)
                            this.WriteMimeVersion();
                        this.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
                        this.WriteBoundary(currentPart.Boundary, false);
                    }
                    var part = new PartData();
                    this.PushPart(ref part);
                    state = MimeWriteState.StartPart;
                    contentWritten = false;
                    contentTransferEncoding = ContentTransferEncoding.SevenBit;
                    break;
            }
        }

        public void EndPart() {
            this.AssertOpen();
            switch (state) {
                case MimeWriteState.Initial:
                case MimeWriteState.Complete:
                    throw new System.InvalidOperationException(Resources.Strings.CannotEndPartHere);
                case MimeWriteState.StartPart:
                case MimeWriteState.Headers:
                case MimeWriteState.Parameters:
                case MimeWriteState.Recipients:
                case MimeWriteState.GroupRecipients:
                    this.FlushHeader();
                    if (!foundMimeVersion) {
                        if (forceMime && partDepth == 1)
                            this.WriteMimeVersion();
                        else
                            currentPart.IsMultipart = false;
                    }
                    this.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
                    break;
                case MimeWriteState.PartContent:
                    if (encodedPartContent != null) {
                        encodedPartContent.Dispose();
                        encodedPartContent = null;
                    }
                    if (partContent != null) {
                        partContent.Dispose();
                        partContent = null;
                    }
                    contentWritten = true;
                    break;
            }
            state = MimeWriteState.EndPart;
            if (currentPart.IsMultipart) {
                this.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
                this.WriteBoundary(currentPart.Boundary, true);
            }
            this.PopPart();
            if (partDepth != 0)
                return;
            state = MimeWriteState.Complete;
        }

        internal void Write(byte[] data, int offset, int count) {
            if (0 >= count)
                return;
            this.QueueWrite(data, offset, count);
        }

        internal void QueueWrite(byte[] data, int offset, int count) {
            bytesWritten += count;
            lineTermination = MimeCommon.AdvanceLineTerminationState(lineTermination, data, offset, count);
            if ((writeCount == 1 ? currentWrite.Length - currentWrite.Count : (writeCount == 0 ? QueuedWrite.QueuedWriteSize : 0)) >= count) {
                if (writeCount == 0) {
                    var write = new QueuedWrite();
                    this.PushWrite(ref write);
                }
                currentWrite.Append(data, offset, count);
            } else {
                var write = new QueuedWrite();
                if (count < QueuedWrite.QueuedWriteSize && writeCount > 0)
                    write = currentWrite;
                this.FlushWriteQueue();
                if (count < QueuedWrite.QueuedWriteSize && write.Length > 0) {
                    write.Reset();
                    write.Append(data, offset, count);
                    this.PushWrite(ref write);
                } else
                    this.data.Write(data, offset, count);
            }
        }

        private void WriteHeader(Header header) {
            switch (state) {
                case MimeWriteState.Initial:
                case MimeWriteState.Complete:
                case MimeWriteState.PartContent:
                case MimeWriteState.EndPart:
                    throw new System.InvalidOperationException(Resources.Strings.CannotWriteHeadersHere);
                default:
                    state = MimeWriteState.Headers;
                    this.FlushHeader();
                    lastHeader = header;
                    break;
            }
        }

        private void WriteParameter(MimeParameter parameter) {
            if (lastHeader == null || !(lastHeader is ComplexHeader))
                throw new System.InvalidOperationException(Resources.Strings.CannotWriteParametersOnThisHeader);
            switch (state) {
                case MimeWriteState.Complete:
                case MimeWriteState.StartPart:
                case MimeWriteState.Recipients:
                case MimeWriteState.PartContent:
                case MimeWriteState.EndPart:
                    throw new System.InvalidOperationException(Resources.Strings.CannotWriteParametersHere);
                default:
                    state = MimeWriteState.Parameters;
                    var contentTypeHeader = lastHeader as ContentTypeHeader;
                    if (contentTypeHeader != null && contentTypeHeader.IsMultipart && parameter.Name == "boundary") {
                        var str = parameter.Value;
                        if (str.Length == 0)
                            throw new System.ArgumentException(Resources.Strings.CannotWriteEmptyOrNullBoundary);
                        currentPart.Boundary = Internal.ByteString.StringToBytes(str, false);
                    }
                    lastHeader.InternalAppendChild(parameter);
                    break;
            }
        }

        private void WriteRecipient(MimeRecipient recipient) {
            if (lastHeader == null || !(lastHeader is AddressHeader))
                throw new System.InvalidOperationException(Resources.Strings.CannotWriteRecipientsHere);
            MimeNode mimeNode;
            switch (state) {
                case MimeWriteState.Complete:
                case MimeWriteState.StartPart:
                case MimeWriteState.PartContent:
                case MimeWriteState.EndPart:
                    throw new System.InvalidOperationException(Resources.Strings.CannotWriteRecipientsHere);
                case MimeWriteState.GroupRecipients:
                    mimeNode = lastHeader.LastChild;
                    break;
                default:
                    state = MimeWriteState.Recipients;
                    mimeNode = lastHeader;
                    break;
            }
            mimeNode.InternalAppendChild(recipient);
        }

        private void StartGroup(MimeGroup group) {
            switch (state) {
                case MimeWriteState.Complete:
                case MimeWriteState.StartPart:
                case MimeWriteState.PartContent:
                case MimeWriteState.EndPart:
                    throw new System.InvalidOperationException(Resources.Strings.CannotWriteGroupStartHere);
                default:
                    if (lastHeader == null || !(lastHeader is AddressHeader))
                        throw new System.InvalidOperationException(Resources.Strings.CannotWriteGroupStartHere);
                    state = MimeWriteState.GroupRecipients;
                    lastHeader.InternalAppendChild(@group);
                    break;
            }
        }

        private void FlushHeader() {
            headerValueWritten = false;
            if (lastHeader == null)
                return;
            if (lastHeader.HeaderId == HeaderId.MimeVersion && partDepth == 1)
                foundMimeVersion = true;
            else if (lastHeader.HeaderId == HeaderId.ContentTransferEncoding) {
                var data = lastHeader.Value;
                if (data != null)
                    contentTransferEncoding = MimePart.GetEncodingType(new MimeString(data));
            } else if (lastHeader.HeaderId == HeaderId.ContentType) {
                var contentTypeHeader = lastHeader as ContentTypeHeader;
                if (contentTypeHeader.IsMultipart) {
                    currentPart.IsMultipart = true;
                    currentPart.Boundary = contentTypeHeader["boundary"].RawValue;
                } else
                    currentPart.IsMultipart = false;
                currentPart.HasContentType = true;
            }
            lastHeader.WriteTo(shimStream, encodingOptions);
            lastHeader = null;
        }

        private void WriteMimeVersion() {
            foundMimeVersion = true;
            this.QueueWrite(MimeString.MimeVersion, 0, MimeString.MimeVersion.Length);
            state = MimeWriteState.Headers;
        }

        private void FlushWriteQueue() {
            if (writeCount != 0)
                writeQueue[writeCount - 1] = currentWrite;
            for (var index = 0; index < writeCount; ++index) {
                data.Write(writeQueue[index].Data, writeQueue[index].Offset, writeQueue[index].Count);
                writeQueue[index] = new QueuedWrite();
            }
            writeCount = 0;
        }

        private void WriteBoundary(byte[] boundary, bool final) {
            var data = !final ? endBoundarySuffix : terminateBoundarySuffix;
            this.Write(boundaryPrefix, 0, boundaryPrefix.Length);
            this.Write(boundary, 0, boundary.Length);
            this.Write(data, 0, data.Length);
        }

        private void PushPart(ref PartData part) {
            if (partStack == null) {
                partStack = new PartData[8];
                partDepth = 0;
            } else if (partStack.Length == partDepth) {
                var partDataArray = new PartData[partStack.Length*2];
                System.Array.Copy(partStack, 0, partDataArray, 0, partStack.Length);
                for (var index = 0; index < partDepth; ++index) {
                    partStack[index] = new PartData();
                }
                partStack = partDataArray;
            }
            if (partDepth != 0)
                partStack[partDepth - 1] = currentPart;
            partStack[partDepth++] = part;
            currentPart = part;
        }

        private void PopPart() {
            --partDepth;
            partStack[partDepth] = new PartData();
            currentPart = partStack[partDepth > 0 ? partDepth - 1 : 0];
        }

        private void AssertOpen() {
            if (data == null)
                throw new System.ObjectDisposedException("MimeWriter");
        }

        private void PushWrite(ref QueuedWrite write) {
            if (writeQueue == null) {
                writeQueue = new QueuedWrite[16];
                writeCount = 0;
            } else if (writeQueue.Length == writeCount) {
                var queuedWriteArray = new QueuedWrite[writeQueue.Length*2];
                System.Array.Copy(writeQueue, 0, queuedWriteArray, 0, writeQueue.Length);
                for (var index = 0; index < writeQueue.Length; ++index) {
                    writeQueue[index] = new QueuedWrite();
                }
                writeQueue = queuedWriteArray;
            }
            if (writeCount != 0)
                writeQueue[writeCount - 1] = currentWrite;
            writeQueue[writeCount++] = write;
            currentWrite = write;
        }

        private static readonly byte[] terminateBoundarySuffix = new byte[4] {
            45,
            45,
            13,
            10
        };

        private static readonly byte[] endBoundarySuffix = MimeString.CrLf;

        private static readonly byte[] boundaryPrefix = new byte[2] {
            45,
            45
        };

        private readonly EncodingOptions encodingOptions;
        private readonly bool forceMime = true;
        private readonly WriterQueueStream shimStream;
        private int bytesWritten;
        private ContentTransferEncoding contentTransferEncoding = ContentTransferEncoding.SevenBit;
        private bool contentWritten;
        private PartData currentPart;
        private QueuedWrite currentWrite;
        private System.IO.Stream data;
        private int embeddedDepth;
        private System.IO.Stream encodedPartContent;
        private bool foundMimeVersion;
        private bool headerValueWritten;
        private Header lastHeader;
        private LineTerminationState lineTermination;
        private WriterContentStream partContent;
        private int partDepth;
        private PartData[] partStack;
        private byte[] scratchBuffer;
        private MimeWriteState state;
        private int writeCount;
        private QueuedWrite[] writeQueue;


        private struct PartData {

            public bool IsMultipart { get; set; }
            public bool HasContentType { get; set; }
            public byte[] Boundary { get; set; }

        }


        private struct QueuedWrite {

            public int Length => this.Data.Length;
            public byte[] Data { get; private set; }
            public int Offset { get; private set; }
            public int Count { get; private set; }

            public bool Full {
                get {
                    if (this.Data != null)
                        return this.Count == this.Data.Length;
                    return false;
                }
            }

            public void Reset() {
                this.Count = 0;
                this.Offset = 0;
            }

            public int Append(byte[] buffer, int offset, int count) {
                if (this.Full)
                    return 0;
                if (this.Data == null)
                    this.Data = new byte[QueuedWriteSize];
                var count1 = System.Math.Min(count, this.Data.Length - this.Count);
                System.Buffer.BlockCopy(buffer, offset, this.Data, this.Count, count1);
                this.Count += count1;
                return count1;
            }

            public static readonly int QueuedWriteSize = 4096;

        }


        internal class WriterQueueStream : System.IO.Stream {

            public WriterQueueStream(MimeWriter writer) {
                this.writer = writer;
            }

            public override bool CanRead => false;
            public override bool CanWrite => true;
            public override bool CanSeek => false;

            public override long Length {
                get {
                    throw new System.NotSupportedException();
                }
            }

            public override long Position {
                get {
                    throw new System.NotSupportedException();
                }
                set {
                    throw new System.NotSupportedException();
                }
            }

            public override int Read(byte[] buffer, int offset, int count) {
                throw new System.NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count) {
                if (count <= 0)
                    return;
                writer.QueueWrite(buffer, offset, count);
            }

            public override long Seek(long offset, System.IO.SeekOrigin origin) {
                throw new System.NotSupportedException();
            }

            public override void Flush() {
                throw new System.NotSupportedException();
            }

            public override void SetLength(long value) {
                throw new System.NotSupportedException();
            }

            protected override void Dispose(bool disposing) {
                base.Dispose(disposing);
            }

            private readonly MimeWriter writer;

        }


        private class WriterContentStream : System.IO.Stream {

            public WriterContentStream(MimeWriter writer) {
                this.writer = writer;
            }

            public override bool CanRead => false;
            public override bool CanWrite => true;
            public override bool CanSeek => false;

            public override long Length {
                get {
                    throw new System.NotSupportedException();
                }
            }

            public override long Position {
                get {
                    throw new System.NotSupportedException();
                }
                set {
                    throw new System.NotSupportedException();
                }
            }

            public override int Read(byte[] buffer, int offset, int count) {
                throw new System.NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count) {
                MimeCommon.CheckBufferArguments(buffer, offset, count);
                if (writer.contentWritten)
                    throw new System.InvalidOperationException(Resources.Strings.ContentAlreadyWritten);
                writer.Write(buffer, offset, count);
            }

            public override long Seek(long offset, System.IO.SeekOrigin origin) {
                throw new System.NotSupportedException();
            }

            public override void Flush() {
                if (writer.contentWritten)
                    throw new System.InvalidOperationException(Resources.Strings.ContentAlreadyWritten);
            }

            public override void SetLength(long value) {
                throw new System.NotSupportedException();
            }

            protected override void Dispose(bool disposing) {
                base.Dispose(disposing);
            }

            private readonly MimeWriter writer;

        }

    }

}