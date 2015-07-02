using System.Linq;

namespace Butler.Schema.Mime {

    internal class MimeParser {

        public MimeParser(bool expectBinaryContent) {
            parseInlineAttachments = true;
            this.expectBinaryContent = expectBinaryContent;
            this.Reset();
        }

        public MimeParser(bool parseEmbeddedMessages, bool parseInlineAttachments, bool expectBinaryContent) {
            this.parseEmbeddedMessages = parseEmbeddedMessages;
            this.parseInlineAttachments = parseInlineAttachments;
            this.expectBinaryContent = expectBinaryContent;
            this.Reset();
        }

        public bool IsEndOfFile => state == ParseState.EndOfFile;
        public int Position { get; private set; }
        public int Depth { get; private set; }

        public int PartDepth {
            get {
                if (this.Depth != 0)
                    return parseStack[this.Depth - 1].PartDepth;
                return 0;
            }
        }

        public int HeaderNameLength { get; private set; }
        public int HeaderDataOffset { get; private set; }
        public bool IsHeaderComplete { get; private set; }
        public MimeComplianceStatus ComplianceStatus { get; set; }
        public bool IsMime { get; private set; }
        public MajorContentType ContentType => currentLevel.ContentType;
        public ContentTransferEncoding TransferEncoding => currentLevel.TransferEncoding;
        public ContentTransferEncoding InlineFormat { get; private set; }

        public void Reset() {
            state = ParseState.Headers;
            currentOffset = 0;
            lineOffset = 0;
            this.IsMime = false;
            currentLevel.Reset(true);
            this.Depth = 0;
            this.Position = 0;
            lastTokenLength = 0;
            firstHeader = true;
            nextBoundaryLevel = -1;
            nextBoundaryEnd = false;
            this.HeaderNameLength = 0;
            this.HeaderDataOffset = 0;
            this.IsHeaderComplete = false;
            this.InlineFormat = ContentTransferEncoding.Unknown;
            this.ComplianceStatus = MimeComplianceStatus.Compliant;
        }

        public void SetMIME() {
            if (this.Depth != 0 && parseStack[this.Depth - 1].ContentType != MajorContentType.MessageRfc822)
                return;
            if (this.Depth == 0)
                this.IsMime = true;
            currentLevel.IsMime = true;
        }

        public void SetContentType(MajorContentType contentType, MimeString boundaryValue) {
            currentLevel.SetContentType(contentType, boundaryValue);
            if (contentType == MajorContentType.Multipart)
                return;
            nextBoundaryLevel = -1;
        }

        public void SetTransferEncoding(ContentTransferEncoding encoding) {
            currentLevel.TransferEncoding = encoding;
        }

        public void SetStreamMode() {
            state = ParseState.Body;
            currentLevel.StreamMode = true;
        }

        public void ReportConsumedData(int lengthConsumed) {
            lastTokenLength -= lengthConsumed;
            this.Position += lengthConsumed;
        }

        public MimeToken Parse(byte[] data, int start, int end, bool flush) {
            var num1 = start + currentOffset;
            var line = start + lineOffset;
            switch (state) {
                case ParseState.Headers:
                    var flag = false;
                    var nextNL = Internal.ByteString.IndexOf(data, 10, num1, end - num1);
                    if (nextNL == -1)
                        nextNL = end;
                    if (nextNL == end) {
                        if (end - start <= 998 && !flush || !flush && end - start <= 999 && data[end - 1] == 13) {
                            currentOffset = end - start;
                            return new MimeToken(MimeTokenId.None, 0);
                        }
                    } else if (nextNL == start || data[nextNL - 1] != 13) {
                        this.ComplianceStatus |= MimeComplianceStatus.BareLinefeedInHeader;
                        flag = true;
                    } else
                        --nextNL;
                    this.HeaderNameLength = 0;
                    this.HeaderDataOffset = 0;
                    int num2;
                    if (nextNL - start > 998) {
                        this.ComplianceStatus |= MimeComplianceStatus.InvalidWrapping;
                        currentOffset = nextNL - (start + 998);
                        lineOffset = line - (start + 998);
                        nextNL = start + 998;
                        num2 = 0;
                    } else {
                        currentOffset = 0;
                        lineOffset = nextNL == end ? line - nextNL : 0;
                        num2 = nextNL == end ? 0 : (flag ? 1 : 2);
                    }
                    if (nextNL == start) {
                        state = ParseState.EndOfHeaders;
                        lastTokenLength = num2;
                        return new MimeToken(MimeTokenId.EndOfHeaders, lastTokenLength);
                    }
                    this.IsHeaderComplete = num2 != 0 && nextNL + num2 < end && (data[nextNL + num2] != 32 && data[nextNL + num2] != 9);
                    if (!firstHeader && (line < start || data[line] == 32 || data[line] == 9)) {
                        lastTokenLength = nextNL + num2 - start;
                        return new MimeToken(MimeTokenId.HeaderContinuation, lastTokenLength);
                    }
                    firstHeader = false;
                    var characterCount = 0;
                    this.HeaderNameLength = MimeScan.FindEndOf(MimeScan.Token.Field, data, start, nextNL - start, out characterCount, false);
                    if (this.HeaderNameLength == 0) {
                        this.ComplianceStatus |= MimeComplianceStatus.InvalidHeader;
                        lastTokenLength = nextNL + num2 - start;
                        return new MimeToken(MimeTokenId.Header, lastTokenLength);
                    }
                    var offset = start + this.HeaderNameLength;
                    if (offset == nextNL || data[offset] != 58) {
                        offset += MimeScan.SkipLwsp(data, offset, nextNL - offset);
                        if (offset == nextNL || data[offset] != 58) {
                            this.HeaderNameLength = 0;
                            if (this.IsMime && (this.Depth > 0 || currentLevel.ContentType == MajorContentType.Multipart) &&
                                (nextNL - line > 2 && data[line] == 45 && (data[line + 1] == 45 && this.FindBoundary(data, line, nextNL, out nextBoundaryLevel, out nextBoundaryEnd)))) {
                                this.ComplianceStatus |= MimeComplianceStatus.MissingBodySeparator;
                                if (nextBoundaryLevel != this.Depth)
                                    this.ComplianceStatus |= MimeComplianceStatus.MissingBoundary;
                                lineOffset = 0;
                                currentOffset = nextNL - start;
                                state = ParseState.EndOfHeaders;
                                return new MimeToken(MimeTokenId.EndOfHeaders, 0);
                            }
                            this.ComplianceStatus |= MimeComplianceStatus.InvalidHeader;
                            lastTokenLength = nextNL + num2 - start;
                            return new MimeToken(MimeTokenId.Header, lastTokenLength);
                        }
                    }
                    this.HeaderDataOffset = offset + 1 - start;
                    lastTokenLength = nextNL + num2 - start;
                    return new MimeToken(MimeTokenId.Header, lastTokenLength);
                case ParseState.EndOfHeaders:
                    this.CheckMimeConstraints();
                    if (this.IsMime && parseEmbeddedMessages && (currentLevel.ContentType == MajorContentType.MessageRfc822 && !currentLevel.StreamMode)) {
                        this.PushLevel(false);
                        lastTokenLength = 0;
                        return new MimeToken(MimeTokenId.EmbeddedStart, 0);
                    }
                    state = ParseState.Body;
                    goto case 2;
                case ParseState.Body:
                    return this.ParseBody(data, start, end, flush, line, num1);
                default:
                    return new MimeToken(MimeTokenId.EndOfFile, 0);
            }
        }

        private static bool IsUUEncodeBegin(byte[] data, int line, int nextNL) {
            var mimeString = new MimeString(data, line, nextNL - line);
            if (mimeString.Length < 13 || !mimeString.HasPrefixEq(UUBegin, 0, 6))
                return false;
            var index = 6;
            while (index < 10 && (48 <= mimeString[index] && 55 >= mimeString[index]))
                ++index;
            return index != 6 && 32 == mimeString[index];
        }

        private static bool IsUUEncodeEnd(byte[] data, int line, int nextNL) {
            var mimeString = new MimeString(data, line, nextNL - line);
            return mimeString.Length >= 3 && mimeString.HasPrefixEq(UUEnd, 0, 3);
        }

        private MimeToken ParseBody(byte[] data, int start, int end, bool flush, int line, int current) {
            var num1 = line <= start ? 0 : line - start;
            int nextNL;
            int sizeNL;
            bool flag;
            while (true) {
                if (expectBinaryContent)
                    nextNL = Internal.ByteString.IndexOf(data, 10, current, end - current);
                else {
                    bool containsBinary;
                    nextNL = Internal.ByteString.IndexOf(data, 10, current, end - current, out containsBinary);
                    if (containsBinary)
                        this.ComplianceStatus |= MimeComplianceStatus.UnexpectedBinaryContent;
                }
                if (nextNL == -1)
                    nextNL = end;
                if (nextNL == end) {
                    sizeNL = 0;
                    if (end - start != 0 && data[end - 1] == 13 && !flush) {
                        --nextNL;
                        --end;
                    }
                } else if (nextNL == start || data[nextNL - 1] != 13) {
                    if (currentLevel.TransferEncoding != ContentTransferEncoding.Binary)
                        this.ComplianceStatus |= MimeComplianceStatus.BareLinefeedInBody;
                    sizeNL = 1;
                } else {
                    --nextNL;
                    sizeNL = 2;
                }
                if (nextNL - line > 998 && currentLevel.TransferEncoding != ContentTransferEncoding.Binary)
                    this.ComplianceStatus |= MimeComplianceStatus.InvalidWrapping;
                if (nextBoundaryLevel == -1) {
                    if (!this.IsMime || line < start || nextNL != end && nextNL == line || (nextNL != line && data[line] != 45 || nextNL - line > 998)) {
                        if (!parseInlineAttachments || currentLevel.IsMime || line < start || nextNL != end && nextNL == line ||
                            nextNL != line && (this.InlineFormat == ContentTransferEncoding.Unknown && (data[line] | 32) != 98 || this.InlineFormat == ContentTransferEncoding.UUEncode && (data[line] | 32) != 101) || nextNL - line > 998) {
                            if (nextNL != end) {
                                current = nextNL + sizeNL;
                                line = current;
                                num1 = sizeNL;
                                continue;
                            }
                            break;
                        }
                        flag = false;
                    } else
                        flag = true;
                    if (nextNL != end || flush) {
                        if (!flag || nextNL - line <= 2 || this.Depth == 0 && currentLevel.ContentType != MajorContentType.Multipart ||
                            (data[line + 1] != 45 || !this.FindBoundary(data, line, nextNL, out nextBoundaryLevel, out nextBoundaryEnd))) {
                            if (!parseInlineAttachments || currentLevel.IsMime || !this.IsInlineBoundary(data, line, nextNL, end, out nextBoundaryLevel, out nextBoundaryEnd)) {
                                if (nextNL != end) {
                                    current = nextNL + sizeNL;
                                    line = current;
                                    num1 = sizeNL;
                                    continue;
                                }
                                goto label_30;
                            }
                            flag = false;
                        }
                        if (nextBoundaryLevel == this.Depth && (currentLevel.Epilogue || nextBoundaryEnd)) {
                            this.ComplianceStatus |= MimeComplianceStatus.MissingBoundary;
                            nextBoundaryLevel = -1;
                            currentLevel.Epilogue = true;
                            if (nextNL != end) {
                                current = nextNL + sizeNL;
                                line = current;
                                num1 = sizeNL;
                            } else
                                goto label_36;
                        } else
                            goto label_37;
                    } else
                        goto label_26;
                } else
                    goto label_39;
            }
            var num2 = end;
            goto label_40;
            label_26:
            num2 = line < start + num1 || !flag ? line : line - num1;
            goto label_40;
            label_30:
            num2 = end;
            goto label_40;
            label_36:
            num2 = end;
            goto label_40;
            label_37:
            if (line - start > (flag ? num1 : 0)) {
                num2 = line - (flag ? num1 : 0);
                goto label_40;
            }
            label_39:
            return this.ProcessBoundary(start, line, nextNL, sizeNL);
            label_40:
            lineOffset = line - num2;
            currentOffset = nextNL - num2;
            if (num2 != start) {
                lastTokenLength = num2 - start;
                return new MimeToken(MimeTokenId.PartData, lastTokenLength);
            }
            if (!flush)
                return new MimeToken(MimeTokenId.None, 0);
            return this.ProcessEOF();
        }

        private void PushLevel(bool inheritMime) {
            if (parseStack == null || this.Depth == parseStack.Length) {
                var parseLevelArray = new ParseLevel[parseStack == null ? 4 : parseStack.Length*2];
                if (parseStack != null)
                    System.Array.Copy(parseStack, 0, parseLevelArray, 0, this.Depth);
                for (var index = 0; index < this.Depth; ++index) {
                    parseStack[index] = new ParseLevel();
                }
                parseStack = parseLevelArray;
            }
            if (currentLevel.ContentType != MajorContentType.MessageRfc822)
                currentLevel.PartDepth = this.Depth == 0 ? 1 : parseStack[this.Depth - 1].PartDepth + 1;
            parseStack[this.Depth++] = currentLevel;
            currentLevel.Reset(!inheritMime);
            state = ParseState.Headers;
            firstHeader = true;
        }

        private void CheckMimeConstraints() {
            if (!this.IsMime) {
                currentLevel.SetContentType(MajorContentType.Other, new MimeString());
                currentLevel.TransferEncoding = ContentTransferEncoding.SevenBit;
            } else {
                if ((currentLevel.ContentType == MajorContentType.Multipart || currentLevel.ContentType == MajorContentType.MessageRfc822 || currentLevel.ContentType == MajorContentType.Message) &&
                    currentLevel.TransferEncoding > ContentTransferEncoding.Binary)
                    this.ComplianceStatus |= MimeComplianceStatus.InvalidTransferEncoding;
                if (this.Depth == 0 || currentLevel.TransferEncoding > ContentTransferEncoding.Binary || currentLevel.TransferEncoding <= parseStack[this.Depth - 1].TransferEncoding)
                    return;
                this.ComplianceStatus |= MimeComplianceStatus.InvalidTransferEncoding;
            }
        }

        private bool FindBoundary(byte[] data, int line, int nextNL, out int nextBoundaryLevel, out bool nextBoundaryEnd) {
            while (nextNL > line && MimeScan.IsLWSP(data[nextNL - 1]))
                --nextNL;
            var crc = Internal.ByteString.ComputeCrc(data, line, nextNL - line);
            bool term;
            if (currentLevel.IsBoundary(data, line, nextNL - line, crc, out term)) {
                nextBoundaryLevel = this.Depth;
                nextBoundaryEnd = term;
                return true;
            }
            for (var index = this.Depth - 1; index >= 0; --index) {
                if (parseStack[index].IsBoundary(data, line, nextNL - line, crc, out term)) {
                    nextBoundaryLevel = index;
                    nextBoundaryEnd = term;
                    return true;
                }
            }
            nextBoundaryLevel = -1;
            nextBoundaryEnd = false;
            return false;
        }

        private bool IsInlineBoundary(byte[] data, int line, int nextNL, int end, out int nextBoundaryLevel, out bool nextBoundaryEnd) {
            switch (this.InlineFormat) {
                case ContentTransferEncoding.Unknown:
                    if ((data[line] | 32) == 98 && nextNL - line >= 11 && (nextNL != end && MimeParser.IsUUEncodeBegin(data, line, nextNL))) {
                        nextBoundaryLevel = -100;
                        nextBoundaryEnd = false;
                        return true;
                    }
                    break;
                case ContentTransferEncoding.UUEncode:
                    if ((data[line] | 32) == 101 && nextNL - line >= 3 && MimeParser.IsUUEncodeEnd(data, line, nextNL)) {
                        nextBoundaryLevel = -100;
                        nextBoundaryEnd = true;
                        return true;
                    }
                    break;
            }
            nextBoundaryLevel = -1;
            nextBoundaryEnd = false;
            return false;
        }

        private MimeToken ProcessBoundary(int start, int line, int nextNL, int sizeNL) {
            if (nextBoundaryLevel < 0) {
                lineOffset = 0;
                currentOffset = 0;
                if (!nextBoundaryEnd) {
                    this.InlineFormat = nextBoundaryLevel == -100 ? ContentTransferEncoding.UUEncode : ContentTransferEncoding.BinHex;
                    nextBoundaryLevel = -1;
                    lastTokenLength = nextNL + sizeNL - start;
                    return new MimeToken(MimeTokenId.InlineStart, lastTokenLength);
                }
                this.InlineFormat = ContentTransferEncoding.Unknown;
                nextBoundaryLevel = -1;
                lastTokenLength = nextNL + sizeNL - start;
                return new MimeToken(MimeTokenId.InlineEnd, lastTokenLength);
            }
            if (nextBoundaryLevel == this.Depth) {
                lineOffset = 0;
                currentOffset = 0;
                nextBoundaryLevel = -1;
                this.PushLevel(true);
                lastTokenLength = nextNL + sizeNL - start;
                return new MimeToken(MimeTokenId.NestedStart, lastTokenLength);
            }
            if (nextBoundaryLevel == this.Depth - 1) {
                if (currentLevel.ContentType == MajorContentType.Multipart && !currentLevel.Epilogue)
                    this.ComplianceStatus |= MimeComplianceStatus.MissingBoundary;
                lineOffset = 0;
                currentOffset = 0;
                nextBoundaryLevel = -1;
                if (nextBoundaryEnd) {
                    currentLevel = parseStack[--this.Depth];
                    currentLevel.Epilogue = true;
                    parseStack[this.Depth].Reset(false);
                    lastTokenLength = nextNL + sizeNL - start;
                    return new MimeToken(MimeTokenId.NestedEnd, lastTokenLength);
                }
                currentLevel.Reset(false);
                state = ParseState.Headers;
                firstHeader = true;
                lastTokenLength = nextNL + sizeNL - start;
                return new MimeToken(MimeTokenId.NestedNext, lastTokenLength);
            }
            lineOffset = line - start;
            currentOffset = nextNL - start;
            if (this.InlineFormat != ContentTransferEncoding.Unknown) {
                this.ComplianceStatus |= MimeComplianceStatus.MissingBoundary;
                this.InlineFormat = ContentTransferEncoding.Unknown;
                return new MimeToken(MimeTokenId.InlineEnd, 0);
            }
            currentLevel = parseStack[--this.Depth];
            currentLevel.Epilogue = true;
            parseStack[this.Depth].Reset(false);
            if (currentLevel.ContentType == MajorContentType.MessageRfc822)
                return new MimeToken(MimeTokenId.EmbeddedEnd, 0);
            this.ComplianceStatus |= MimeComplianceStatus.MissingBoundary;
            return new MimeToken(MimeTokenId.NestedEnd, 0);
        }

        private MimeToken ProcessEOF() {
            if (this.InlineFormat != ContentTransferEncoding.Unknown) {
                this.ComplianceStatus |= MimeComplianceStatus.MissingBoundary;
                this.InlineFormat = ContentTransferEncoding.Unknown;
                return new MimeToken(MimeTokenId.InlineEnd, 0);
            }
            if (this.Depth != 0) {
                currentLevel = parseStack[--this.Depth];
                currentLevel.Epilogue = true;
                parseStack[this.Depth].Reset(false);
                if (currentLevel.ContentType == MajorContentType.MessageRfc822)
                    return new MimeToken(MimeTokenId.EmbeddedEnd, 0);
                this.ComplianceStatus |= MimeComplianceStatus.MissingBoundary;
                return new MimeToken(MimeTokenId.NestedEnd, 0);
            }
            state = ParseState.EndOfFile;
            currentLevel.Reset(true);
            return new MimeToken(MimeTokenId.EndOfFile, 0);
        }

        private static readonly byte[] UUBegin = Internal.ByteString.StringToBytes("begin ", true);
        private static readonly byte[] UUEnd = Internal.ByteString.StringToBytes("end", true);
        private readonly bool expectBinaryContent;
        private readonly bool parseEmbeddedMessages;
        private readonly bool parseInlineAttachments;
        private ParseLevel currentLevel;
        private int currentOffset;
        private bool firstHeader;
        private int lastTokenLength;
        private int lineOffset;
        private bool nextBoundaryEnd;
        private int nextBoundaryLevel;
        private ParseLevel[] parseStack;
        private ParseState state;


        private enum ParseState {

            Headers,
            EndOfHeaders,
            Body,
            EndOfFile

        }


        private struct ParseLevel {

            public MajorContentType ContentType { get; private set; }
            public ContentTransferEncoding TransferEncoding { get; set; }
            public int PartDepth { get; set; }
            public bool IsMime { get; set; }
            public bool StreamMode { get; set; }

            public void Reset(bool cleanMimeState) {
                this.StreamMode = false;
                this.ContentType = MajorContentType.Other;
                Epilogue = false;
                this.TransferEncoding = ContentTransferEncoding.SevenBit;
                if (cleanMimeState)
                    this.IsMime = false;
                boundaryValue = new MimeString();
                boundaryCrc = 0U;
                endBoundaryCrc = 0U;
                this.PartDepth = 0;
            }

            public void SetContentType(MajorContentType contentType, MimeString boundaryValue) {
                if (contentType == MajorContentType.Multipart) {
                    int offset;
                    int count;
                    var data = boundaryValue.GetData(out offset, out count);
                    var numArray = new byte[MimeString.TwoDashes.Length + count + MimeString.TwoDashes.Length];
                    var length = MimeString.TwoDashes.Length;
                    System.Buffer.BlockCopy(MimeString.TwoDashes, 0, numArray, 0, length);
                    System.Buffer.BlockCopy(data, offset, numArray, length, count);
                    var num1 = length + count;
                    boundaryCrc = Internal.ByteString.ComputeCrc(numArray, 0, num1);
                    System.Buffer.BlockCopy(MimeString.TwoDashes, 0, numArray, num1, MimeString.TwoDashes.Length);
                    var num2 = num1 + MimeString.TwoDashes.Length;
                    endBoundaryCrc = Internal.ByteString.ComputeCrc(numArray, 0, num2);
                    this.boundaryValue = new MimeString(numArray, 0, num2);
                } else {
                    this.boundaryValue = new MimeString();
                    boundaryCrc = 0U;
                    endBoundaryCrc = 0U;
                }
                this.ContentType = contentType;
            }

            //public bool IsBoundary(byte[] bytes, int offset, int length, long crc, out bool term)
            //{
            //  if (crc == (long) this.boundaryCrc && this.boundaryValue.Length - 2 == length)
            //  {
            //    term = false;
            //    return this.boundaryValue.HasPrefixEq(bytes, offset, length);
            //  }
            //  if (crc == (long) this.endBoundaryCrc && this.boundaryValue.Length == length)
            //  {
            //    // ISSUE: explicit reference operation
            //    // ISSUE: cast to a reference type
            //    // ISSUE: explicit reference operation
            //    return (bool) (^(sbyte&) @term = (sbyte) this.boundaryValue.HasPrefixEq(bytes, offset, length));
            //  }
            //  // ISSUE: explicit reference operation
            //  // ISSUE: cast to a reference type
            //  // ISSUE: explicit reference operation
            //  return (bool) (^(sbyte&) @term = (sbyte) false);
            //}
            public bool IsBoundary(byte[] bytes, int offset, int length, long crc, out bool term) {
                bool flag2;
                if ((crc == boundaryCrc) && ((boundaryValue.Length - 2) == length)) {
                    term = false;
                    return boundaryValue.HasPrefixEq(bytes, offset, length);
                }
                if ((crc == endBoundaryCrc) && (boundaryValue.Length == length)) {
                    bool flag;
                    term = flag = boundaryValue.HasPrefixEq(bytes, offset, length);
                    return flag;
                }
                term = flag2 = false;
                return flag2;
            }

            private uint boundaryCrc;
            private MimeString boundaryValue;
            private uint endBoundaryCrc;
            public bool Epilogue;

        }

    }

}