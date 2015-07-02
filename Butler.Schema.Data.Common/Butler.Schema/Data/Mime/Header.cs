using System;
using System.Linq;

namespace Butler.Schema.Data.Mime {

    public abstract class Header : MimeNode {

        internal Header(string name, HeaderId headerId) {
            this.Name = name;
            this.HeaderId = headerId;
        }

        public string Name { get; }
        public HeaderId HeaderId { get; }
        public abstract string Value { get; set; }
        public virtual bool RequiresSMTPUTF8 => false;

        internal virtual byte[] RawValue {
            get {
                if (lines.Length == 0)
                    return MimeString.EmptyByteArray;
                return lines.GetSz() ?? MimeString.EmptyByteArray;
            }
            set {
                this.SetRawValue(value, true);
            }
        }

        internal MimeString FirstRawToken {
            get {
                var valueParser = new ValueParser(
                    lines.Length != 0 ? lines : new MimeStringList(this.RawValue),
                    this.GetHeaderDecodingOptions()
                        .AllowUTF8);
                var phrase = new MimeStringList();
                valueParser.ParseCFWS(false, ref phrase, true);
                return valueParser.ParseToken();
            }
        }

        internal MimeStringList Lines {
            get {
                return lines;
            }
            set {
                this.SetRawValue(value, true);
            }
        }

        internal int RawLength => lines.Length;
        internal bool IsDirty { get; private set; }

        internal bool IsProtected {
            get {
                var mimePart = this.GetTreeRoot() as MimePart;
                return mimePart != null && mimePart.IsProtectedHeader(this.Name);
            }
        }

        public static Header Create(string name) {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            var headerId = Header.GetHeaderId(name, true);
            if (headerId != HeaderId.Unknown)
                return Header.Create(name, headerId);
            return new TextHeader(name, HeaderId.Unknown);
        }

        public static Header Create(HeaderId headerId) {
            return Header.Create(null, headerId);
        }

        internal static Header Create(string name, HeaderId headerId) {
            if (headerId < HeaderId.Unknown || headerId > (HeaderId) MimeData.nameIndex.Length)
                throw new ArgumentException(CtsResources.Strings.InvalidHeaderId, nameof(headerId));
            if (headerId == HeaderId.Unknown)
                throw new ArgumentException(CtsResources.Strings.CannotDetermineHeaderNameFromId, nameof(headerId));
            Header header;
            switch (MimeData.headerNames[(int) MimeData.nameIndex[(int) headerId]].headerType) {
                case HeaderType.AsciiText:
                    header = new AsciiTextHeader(MimeData.headerNames[(int) MimeData.nameIndex[(int) headerId]].name, headerId);
                    break;
                case HeaderType.Date:
                    header = new DateHeader(MimeData.headerNames[(int) MimeData.nameIndex[(int) headerId]].name, headerId);
                    break;
                case HeaderType.Received:
                    header = new ReceivedHeader();
                    break;
                case HeaderType.ContentType:
                    header = new ContentTypeHeader();
                    break;
                case HeaderType.ContentDisposition:
                    header = new ContentDispositionHeader();
                    break;
                case HeaderType.Address:
                    header = new AddressHeader(MimeData.headerNames[(int) MimeData.nameIndex[(int) headerId]].name, headerId);
                    break;
                default:
                    header = new TextHeader(MimeData.headerNames[(int) MimeData.nameIndex[(int) headerId]].name, headerId);
                    break;
            }
            if (!string.IsNullOrEmpty(name) && !header.MatchName(name))
                throw new ArgumentException("name");
            return header;
        }

        public static bool IsHeaderNameValid(string name) {
            if (string.IsNullOrEmpty(name))
                return false;
            for (var index = 0; index < name.Length; ++index) {
                if (name[index] >= 128 || !MimeScan.IsField((byte) name[index]))
                    return false;
            }
            return true;
        }

        public static HeaderId GetHeaderId(string name) {
            return Header.GetHeaderId(name, true);
        }

        public static Header ReadFrom(MimeHeaderReader reader) {
            if (reader.MimeReader == null)
                throw new ArgumentNullException(nameof(reader));
            return reader.MimeReader.ReadHeaderObject();
        }

        public virtual bool TryGetValue(out string value) {
            value = this.Value;
            return true;
        }

        public virtual bool IsValueValid(string value) {
            return false;
        }

        public override void CopyTo(object destination) {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (destination == this)
                return;
            var header = destination as Header;
            if (header == null)
                throw new ArgumentException(CtsResources.Strings.CantCopyToDifferentObjectType);
            base.CopyTo(destination);
            header.lines = lines.Clone();
            header.IsDirty = this.IsDirty;
        }

        internal static Type TypeFromHeaderId(HeaderId headerId) {
            switch (MimeData.headerNames[(int) MimeData.nameIndex[(int) headerId]].headerType) {
                case HeaderType.AsciiText:
                    return typeof (AsciiTextHeader);
                case HeaderType.Date:
                    return typeof (DateHeader);
                case HeaderType.Received:
                    return typeof (ReceivedHeader);
                case HeaderType.ContentType:
                    return typeof (ContentTypeHeader);
                case HeaderType.ContentDisposition:
                    return typeof (ContentDispositionHeader);
                case HeaderType.Address:
                    return typeof (AddressHeader);
                default:
                    return typeof (TextHeader);
            }
        }

        internal static HeaderId GetHeaderId(string name, bool validateArgument) {
            if (name == null) {
                if (validateArgument)
                    throw new ArgumentNullException(nameof(name));
                return HeaderId.Unknown;
            }
            if (name.Length == 0) {
                if (validateArgument)
                    throw new ArgumentException("Header name cannot be an empty string", nameof(name));
                return HeaderId.Unknown;
            }
            if (validateArgument) {
                for (var position = 0; position < name.Length; ++position) {
                    if (name[position] >= 128 || !MimeScan.IsField((byte) name[position]))
                        throw new ArgumentException(CtsResources.Strings.InvalidHeaderName(name, position), nameof(name));
                }
            }
            var headerNameIndex = Header.LookupName(name);
            return MimeData.headerNames[(int) headerNameIndex].publicHeaderId;
        }

        internal static HeaderId GetHeaderId(byte[] name, int offset, int length) {
            var headerNameIndex = Header.LookupName(name, offset, length);
            return MimeData.headerNames[(int) headerNameIndex].publicHeaderId;
        }

        internal static string GetHeaderName(HeaderId headerId) {
            return MimeData.headerNames[(int) MimeData.nameIndex[(int) headerId]].name;
        }

        internal static Header CreateGeneralHeader(string name) {
            return new TextHeader(name, HeaderId.Unknown);
        }

        internal static bool IsRestrictedHeader(HeaderId headerId) {
            return MimeData.headerNames[(int) MimeData.nameIndex[(int) headerId]].restricted;
        }

        internal static long WriteLines(MimeStringList lines, System.IO.Stream stream) {
            if (lines.Count == 0) {
                var currentLineLength = new MimeStringLength(0);
                return Header.WriteLineEnd(stream, ref currentLineLength);
            }
            var num = 0L;
            for (var index = 0; index < lines.Count; ++index) {
                int offset;
                int count;
                var data = lines[index].GetData(out offset, out count);
                if (count != 0) {
                    if (!MimeScan.IsLWSP(data[offset])) {
                        stream.Write(MimeString.Space, 0, MimeString.Space.Length);
                        num += MimeString.Space.Length;
                    }
                    stream.Write(data, offset, count);
                    num += count;
                }
                var currentLineLength = new MimeStringLength(0);
                num += Header.WriteLineEnd(stream, ref currentLineLength);
            }
            return num;
        }

        private static long WriteToken(byte[] token, int tokenOffset, MimeStringLength tokenLength, System.IO.Stream stream, ref MimeStringLength currentLineLength, ref LineBuffer lineBuffer, ref bool autoAddedLastLWSP, bool allowUTF8) {
            var num1 = 0L;
            var flag1 = token != null && tokenLength.InChars == 1 && MimeScan.IsFWSP(token[tokenOffset]);
            if (!flag1 && currentLineLength.InChars + lineBuffer.Length.InChars + tokenLength.InChars > 78 && lineBuffer.LengthTillLastLWSP.InBytes >= 0) {
                if (lineBuffer.LengthTillLastLWSP.InBytes > 0) {
                    stream.Write(lineBuffer.Bytes, 0, lineBuffer.LengthTillLastLWSP.InBytes);
                    num1 += lineBuffer.LengthTillLastLWSP.InBytes;
                    currentLineLength.IncrementBy(lineBuffer.LengthTillLastLWSP);
                }
                if (currentLineLength.InBytes > 0)
                    num1 += Header.WriteLineEnd(stream, ref currentLineLength);
                if (autoAddedLastLWSP) {
                    autoAddedLastLWSP = false;
                    lineBuffer.LengthTillLastLWSP.IncrementBy(1);
                }
                if (lineBuffer.LengthTillLastLWSP.InBytes != lineBuffer.Length.InBytes) {
                    Buffer.BlockCopy(lineBuffer.Bytes, lineBuffer.LengthTillLastLWSP.InBytes, lineBuffer.Bytes, 0, lineBuffer.Length.InBytes - lineBuffer.LengthTillLastLWSP.InBytes);
                    lineBuffer.Length.DecrementBy(lineBuffer.LengthTillLastLWSP);
                    if (lineBuffer.Length.InBytes > 0 && MimeScan.IsFWSP(lineBuffer.Bytes[0]))
                        lineBuffer.LengthTillLastLWSP.SetAs(0);
                    else
                        lineBuffer.LengthTillLastLWSP.SetAs(-1);
                } else {
                    lineBuffer.Length.SetAs(0);
                    lineBuffer.LengthTillLastLWSP.SetAs(-1);
                }
                var flag2 = false;
                if (lineBuffer.Length.InBytes > 0) {
                    if (!MimeScan.IsFWSP(lineBuffer.Bytes[0]))
                        flag2 = true;
                } else if (!flag1)
                    flag2 = true;
                if (flag2) {
                    stream.Write(LineStartWhitespace, 0, LineStartWhitespace.Length);
                    num1 += LineStartWhitespace.Length;
                    currentLineLength.IncrementBy(LineStartWhitespace.Length);
                }
            }
            if (currentLineLength.InBytes + lineBuffer.Length.InBytes + tokenLength.InBytes > 998) {
                if (lineBuffer.Length.InBytes > 0) {
                    stream.Write(lineBuffer.Bytes, 0, lineBuffer.Length.InBytes);
                    num1 += lineBuffer.Length.InBytes;
                    currentLineLength.IncrementBy(lineBuffer.Length);
                    lineBuffer.Length.SetAs(0);
                    autoAddedLastLWSP = false;
                    lineBuffer.LengthTillLastLWSP.SetAs(-1);
                }
                if (token != null) {
                    while (currentLineLength.InBytes + tokenLength.InBytes > 998) {
                        var num2 = Math.Max(0, 998 - currentLineLength.InBytes);
                        var countInChars = 0;
                        var num3 = 0;
                        if (allowUTF8) {
                            var num4 = 0;
                            while (num4 < tokenLength.InBytes) {
                                var num5 = token[tokenOffset + num4];
                                var bytesUsed = 1;
                                if (num5 >= 128 && !MimeScan.IsUTF8NonASCII(token, tokenOffset + num4, tokenOffset + tokenLength.InBytes, out bytesUsed))
                                    bytesUsed = 1;
                                if (num3 + bytesUsed <= num2) {
                                    ++countInChars;
                                    num3 += bytesUsed;
                                    num4 += bytesUsed;
                                } else
                                    break;
                            }
                        } else {
                            countInChars = num2;
                            num3 = num2;
                        }
                        stream.Write(token, tokenOffset, num3);
                        var num6 = num1 + num3;
                        currentLineLength.IncrementBy(countInChars, num3);
                        tokenOffset += num3;
                        tokenLength.DecrementBy(countInChars, num3);
                        num1 = num6 + Header.WriteLineEnd(stream, ref currentLineLength);
                        if (!flag1) {
                            stream.Write(LineStartWhitespace, 0, LineStartWhitespace.Length);
                            num1 += LineStartWhitespace.Length;
                            currentLineLength.IncrementBy(LineStartWhitespace.Length);
                        }
                    }
                }
            }
            if (token != null) {
                Buffer.BlockCopy(token, tokenOffset, lineBuffer.Bytes, lineBuffer.Length.InBytes, tokenLength.InBytes);
                if (flag1 && (lineBuffer.Length.InBytes == 0 || !MimeScan.IsFWSP(lineBuffer.Bytes[lineBuffer.Length.InBytes - 1]))) {
                    autoAddedLastLWSP = false;
                    lineBuffer.LengthTillLastLWSP.SetAs(lineBuffer.Length);
                }
                lineBuffer.Length.IncrementBy(tokenLength);
            }
            return num1;
        }

        internal static long QuoteAndFold(
            System.IO.Stream stream, MimeStringList fragments, uint inputMask, bool quoteOutput, bool addSpaceAtStart, bool allowUTF8, int lastLineReserve, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer) {
            var num = 0L;
            var lineBuffer = new LineBuffer();
            lineBuffer.Length = new MimeStringLength(0);
            lineBuffer.LengthTillLastLWSP = new MimeStringLength(-1);
            if (scratchBuffer == null || scratchBuffer.Length < 998)
                scratchBuffer = new byte[998];
            lineBuffer.Bytes = scratchBuffer;
            var token = quoteOutput ? MimeScan.Token.Spec | MimeScan.Token.Fwsp : MimeScan.Token.Fwsp;
            var autoAddedLastLWSP = false;
            if (addSpaceAtStart && currentLineLength.InBytes != 0) {
                num += Header.WriteToken(Space, 0, new MimeStringLength(1), stream, ref currentLineLength, ref lineBuffer, ref autoAddedLastLWSP, allowUTF8);
                autoAddedLastLWSP = true;
            }
            if (quoteOutput)
                num += Header.WriteToken(DoubleQuote, 0, new MimeStringLength(1), stream, ref currentLineLength, ref lineBuffer, ref autoAddedLastLWSP, allowUTF8);
            for (var index = 0; index < fragments.Count; ++index) {
                var mimeString = fragments[index];
                var offset = 0;
                var count = 0;
                var data = mimeString.GetData(out offset, out count);
                if (((int) mimeString.Mask & (int) inputMask) != 0) {
                    do {
                        var characterCount = 0;
                        var nextOf = MimeScan.FindNextOf(token, data, offset, count, out characterCount, allowUTF8);
                        if (nextOf > 0) {
                            num += Header.WriteToken(data, offset, new MimeStringLength(characterCount, nextOf), stream, ref currentLineLength, ref lineBuffer, ref autoAddedLastLWSP, allowUTF8);
                            offset += nextOf;
                            count -= nextOf;
                        }
                        if (count != 0) {
                            switch (data[offset]) {
                                case 34:
                                case 92:
                                    if (((int) mimeString.Mask & -536870913) != 0) {
                                        num += Header.WriteToken(
                                            new byte[2] {
                                                92,
                                                data[offset]
                                            },
                                            0,
                                            new MimeStringLength(2),
                                            stream,
                                            ref currentLineLength,
                                            ref lineBuffer,
                                            ref autoAddedLastLWSP,
                                            allowUTF8);
                                        ++offset;
                                        --count;
                                        goto label_15;
                                    }
                                    break;
                            }
                            num += Header.WriteToken(
                                new byte[1] {
                                    data[offset]
                                },
                                0,
                                new MimeStringLength(1),
                                stream,
                                ref currentLineLength,
                                ref lineBuffer,
                                ref autoAddedLastLWSP,
                                allowUTF8);
                            ++offset;
                            --count;
                        }
                        label_15:
                        ;
                    } while (count != 0);
                }
            }
            if (quoteOutput)
                num += Header.WriteToken(DoubleQuote, 0, new MimeStringLength(1), stream, ref currentLineLength, ref lineBuffer, ref autoAddedLastLWSP, allowUTF8);
            if (lastLineReserve > 0)
                num += Header.WriteToken(null, 0, new MimeStringLength(lastLineReserve), stream, ref currentLineLength, ref lineBuffer, ref autoAddedLastLWSP, allowUTF8);
            if (lineBuffer.Length.InBytes > 0) {
                stream.Write(lineBuffer.Bytes, 0, lineBuffer.Length.InBytes);
                num += lineBuffer.Length.InBytes;
                currentLineLength.IncrementBy(lineBuffer.Length);
            }
            return num;
        }

        internal static int WriteName(System.IO.Stream stream, string name, ref byte[] scratchBuffer) {
            if (scratchBuffer == null || scratchBuffer.Length < name.Length)
                scratchBuffer = new byte[Math.Max(998, name.Length)];
            Internal.ByteString.StringToBytes(name, scratchBuffer, 0, false);
            stream.Write(scratchBuffer, 0, name.Length);
            stream.Write(MimeString.Colon, 0, MimeString.Colon.Length);
            return name.Length + MimeString.Colon.Length;
        }

        internal static HeaderNameIndex LookupName(string name) {
            if (name.Length >= 2 && name.Length <= 31) {
                var index1 = MimeData.HashName(name);
                var index2 = (int) MimeData.nameHashTable[index1];
                if (index2 > 0) {
                    do {
                        var str = MimeData.headerNames[index2].name;
                        if (name.Length == str.Length && name.Equals(str, StringComparison.OrdinalIgnoreCase))
                            return (HeaderNameIndex) index2;
                        ++index2;
                    } while (MimeData.headerNames[index2].hash == index1);
                }
            }
            return HeaderNameIndex.Unknown;
        }

        internal static HeaderNameIndex LookupName(byte[] nameBuffer, int offset, int length) {
            if (length >= 2 && length <= 31) {
                var index1 = MimeData.HashName(nameBuffer, offset, length);
                var index2 = (int) MimeData.nameHashTable[index1];
                if (index2 > 0) {
                    while (!Internal.ByteString.EqualsI(MimeData.headerNames[index2].name, nameBuffer, offset, length, false)) {
                        ++index2;
                        if (MimeData.headerNames[index2].hash != index1)
                            goto label_5;
                    }
                    return (HeaderNameIndex) index2;
                }
            }
            label_5:
            return HeaderNameIndex.Unknown;
        }

        internal static string NormalizeString(string value) {
            if (value.Length >= 2 && value.Length <= 32) {
                var index1 = MimeData.HashValue(value);
                var index2 = MimeData.valueHashTable[index1];
                if (index2 > 0) {
                    do {
                        var str = MimeData.values[index2].value;
                        if (value.Length == str.Length && value.Equals(str, StringComparison.OrdinalIgnoreCase))
                            return str;
                        ++index2;
                    } while (MimeData.values[index2].hash == index1);
                }
            }
            return value.ToLowerInvariant();
        }

        internal static string NormalizeString(string value, int offset, int length) {
            if (length >= 2 && length <= 32) {
                var index1 = MimeData.HashValue(value, offset, length);
                var index2 = MimeData.valueHashTable[index1];
                if (index2 > 0) {
                    do {
                        var strA = MimeData.values[index2].value;
                        if (length == strA.Length && string.Compare(strA, 0, value, offset, length, StringComparison.OrdinalIgnoreCase) == 0)
                            return strA;
                        ++index2;
                    } while (MimeData.values[index2].hash == index1);
                }
            }
            return value.Substring(offset, length)
                        .ToLowerInvariant();
        }

        internal static string NormalizeString(byte[] value, int offset, int length, bool allowUTF8) {
            if (length >= 2 && length <= 32) {
                var index1 = MimeData.HashValue(value, offset, length);
                var index2 = MimeData.valueHashTable[index1];
                if (index2 > 0) {
                    do {
                        var str1 = MimeData.values[index2].value;
                        if (Internal.ByteString.EqualsI(str1, value, offset, length, allowUTF8))
                            return str1;
                        ++index2;
                    } while (MimeData.values[index2].hash == index1);
                }
            }
            return Internal.ByteString.BytesToString(value, offset, length, allowUTF8)
                           .ToLowerInvariant();
        }

        internal string GetRawValue(bool allowUTF8) {
            var rawValue = this.RawValue;
            if (rawValue == null || rawValue.Length == 0)
                return string.Empty;
            return Internal.ByteString.BytesToString(rawValue, allowUTF8);
        }

        internal void SetRawValue(string value, bool markDirty, bool allowUTF8) {
            if (string.IsNullOrEmpty(value))
                this.SetRawValue(null, markDirty);
            else {
                var numArray = Internal.ByteString.StringToBytes(value, allowUTF8);
                var num1 = Internal.ByteString.IndexOf(numArray, 10, 0, numArray.Length);
                if (num1 == -1)
                    this.SetRawValue(numArray, markDirty);
                else {
                    this.RawValueAboutToChange();
                    lines = new MimeStringList();
                    var offset = 0;
                    do {
                        var num2 = num1;
                        while (num2 > offset && numArray[num2 - 1] == 13)
                            --num2;
                        if (num2 > offset)
                            lines.Append(new MimeString(numArray, offset, num2 - offset));
                        offset = num1 + 1;
                        num1 = Internal.ByteString.IndexOf(numArray, 10, offset, numArray.Length - offset);
                    } while (num1 != -1);
                    if (offset != numArray.Length)
                        lines.Append(new MimeString(numArray, offset, numArray.Length - offset));
                    if (!markDirty)
                        return;
                    this.SetDirty();
                }
            }
        }

        internal void SetRawValue(byte[] value, bool markDirty) {
            this.RawValueAboutToChange();
            lines = value == null || value.Length == 0 ? new MimeStringList() : new MimeStringList(new MimeString(value));
            if (!markDirty)
                return;
            this.SetDirty();
        }

        internal void SetRawValue(MimeStringList newLines, bool markDirty) {
            this.RawValueAboutToChange();
            lines = newLines;
            if (!markDirty)
                return;
            this.SetDirty();
        }

        internal virtual void RawValueAboutToChange() {}
        internal virtual void ForceParse() {}

        internal bool IsName(string name) {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            return this.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
        }

        internal bool MatchName(string name) {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (this.Name.Equals(name))
                return true;
            if (!this.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                return false;
            customName = name;
            return true;
        }

        internal override void SetDirty() {
            this.IsDirty = true;
            base.SetDirty();
        }

        internal long WriteName(System.IO.Stream stream, ref byte[] scratchBuffer) {
            if (this.IsDirty || !this.IsProtected)
                return Header.WriteName(stream, this.Name, ref scratchBuffer);
            var name = string.IsNullOrEmpty(customName) ? this.Name : customName;
            return Header.WriteName(stream, name, ref scratchBuffer);
        }

        internal static long WriteLineEnd(System.IO.Stream stream, ref MimeStringLength currentLineLength) {
            var num1 = 0L;
            stream.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
            var num2 = num1 + MimeString.CrLf.Length;
            currentLineLength.SetAs(0);
            return num2;
        }

        internal bool IsHeaderLineTooLong(long nameLength, out bool merge) {
            var num = 0;
            var flag1 = false;
            merge = false;
            for (var index = 0; index < lines.Count; ++index) {
                int offset;
                int count;
                var flag2 = MimeScan.IsLWSP(lines[index].GetData(out offset, out count)[offset]);
                if (num != 0 && !flag2) {
                    flag1 = true;
                    merge = true;
                }
                num += count;
                if (index == 0 && count + nameLength + 1L > 998L)
                    flag1 = true;
            }
            return flag1;
        }

        internal override long WriteTo(System.IO.Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer) {
            var nameLength = this.WriteName(stream, ref scratchBuffer);
            currentLineLength.IncrementBy((int) nameLength);
            var merge = false;
            if (!this.IsDirty && this.RawLength != 0) {
                if (this.IsProtected) {
                    var num = nameLength + Header.WriteLines(lines, stream);
                    currentLineLength.SetAs(0);
                    return num;
                }
                if (!this.IsHeaderLineTooLong(nameLength, out merge)) {
                    var num = nameLength + Header.WriteLines(lines, stream);
                    currentLineLength.SetAs(0);
                    return num;
                }
            }
            var mimeStringList = lines;
            if (merge)
                mimeStringList = Header.MergeLines(mimeStringList);
            return nameLength + Header.QuoteAndFold(stream, mimeStringList, 4026531840U, false, mimeStringList.Length > 0, encodingOptions.AllowUTF8, 0, ref currentLineLength, ref scratchBuffer) +
                   Header.WriteLineEnd(stream, ref currentLineLength);
        }

        internal void AppendLine(MimeString line) {
            this.AppendLine(line, true);
        }

        internal virtual void AppendLine(MimeString line, bool markDirty) {
            this.RawValueAboutToChange();
            lines.Append(line);
            if (!markDirty)
                return;
            this.SetDirty();
        }

        internal static MimeStringList MergeLines(MimeStringList lines) {
            if (lines.Length == 0)
                return lines;
            var numArray = new byte[lines.Length];
            var destinationIndex = 0;
            for (var index = 0; index < lines.Count; ++index) {
                var mimeString = lines[index];
                mimeString.CopyTo(numArray, destinationIndex);
                destinationIndex += mimeString.Length;
            }
            return new MimeStringList(numArray);
        }

        internal const bool AllowUTF8Name = false;
        internal static readonly byte[] LineStartWhitespace = MimeString.Tabulation;

        internal static readonly byte[] DoubleQuote = new byte[1] {
            34
        };

        internal static readonly byte[] Space = new byte[1] {
            32
        };

        private string customName;
        private MimeStringList lines;


        private struct LineBuffer {

            public byte[] Bytes;
            public MimeStringLength Length;
            public MimeStringLength LengthTillLastLWSP;

        }

    }

}