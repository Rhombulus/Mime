using System;
using System.Linq;

namespace Butler.Schema.Data.Mime {

    public class ReceivedHeader : Header {

        internal ReceivedHeader()
            : base("Received", HeaderId.Received) {}

        public ReceivedHeader(string from, string fromTcpInfo, string by, string byTcpInfo, string forMailbox, string with, string id, string via, string date)
            : base("Received", HeaderId.Received) {
            var num1 = -1;
            var num2 = Internal.ByteString.StringToBytesCount(from, true);
            var num3 = Internal.ByteString.StringToBytesCount(fromTcpInfo, true);
            var num4 = Internal.ByteString.StringToBytesCount(by, true);
            var num5 = Internal.ByteString.StringToBytesCount(byTcpInfo, true);
            var num6 = Internal.ByteString.StringToBytesCount(forMailbox, true);
            var num7 = Internal.ByteString.StringToBytesCount(with, true);
            var num8 = Internal.ByteString.StringToBytesCount(id, true);
            var num9 = Internal.ByteString.StringToBytesCount(via, true);
            var num10 = Internal.ByteString.StringToBytesCount(date, false);
            var num11 = num1 + (from != null ? num2 + ParamFrom.Length + 2 : 0) + (fromTcpInfo != null ? num3 + (from == null ? ParamFrom.Length + 1 : 0) + 3 : 0) + (by != null ? num4 + ParamBy.Length + 2 : 0) +
                        (byTcpInfo != null ? num5 + (by == null ? ParamBy.Length + 1 : 0) + 3 : 0) + (forMailbox != null ? num6 + ParamFor.Length + 2 : 0) + (with != null ? num7 + ParamWith.Length + 2 : 0) +
                        (id != null ? num8 + ParamId.Length + 2 : 0) + (via != null ? num9 + ParamVia.Length + 2 : 0);
            var length = num11 + (date != null ? num10 + (-1 == num11 ? 3 : 2) : 0);
            if (-1 == length)
                return;
            var numArray = new byte[length];
            var offset = 0;
            this.AppendNameValue(ParamFrom, from, numArray, ref offset);
            if (fromTcpInfo != null) {
                if (from == null)
                    this.AppendName(ParamFrom, numArray, ref offset);
                numArray[offset++] = 32;
                numArray[offset++] = 40;
                Internal.ByteString.StringToBytes(fromTcpInfo, numArray, offset, true);
                offset += num3;
                numArray[offset++] = 41;
            }
            this.AppendNameValue(ParamBy, by, numArray, ref offset);
            if (byTcpInfo != null) {
                if (by == null)
                    this.AppendName(ParamBy, numArray, ref offset);
                numArray[offset++] = 32;
                numArray[offset++] = 40;
                Internal.ByteString.StringToBytes(byTcpInfo, numArray, offset, true);
                offset += num5;
                numArray[offset++] = 41;
            }
            this.AppendNameValue(ParamFor, forMailbox, numArray, ref offset);
            this.AppendNameValue(ParamWith, with, numArray, ref offset);
            this.AppendNameValue(ParamId, id, numArray, ref offset);
            this.AppendNameValue(ParamVia, via, numArray, ref offset);
            if (date != null) {
                Internal.ByteString.ValidateStringArgument(date, false);
                numArray[offset++] = 59;
                numArray[offset++] = 32;
                Internal.ByteString.StringToBytes(date, numArray, offset, false);
                offset += num10;
            }
            this.RawValue = numArray;
        }

        public string From {
            get {
                if (!parsed)
                    this.Parse();
                return fromValue;
            }
        }

        public string FromTcpInfo {
            get {
                if (!parsed)
                    this.Parse();
                return fromTcpInfoValue;
            }
        }

        public string By {
            get {
                if (!parsed)
                    this.Parse();
                return byValue;
            }
        }

        public string ByTcpInfo {
            get {
                if (!parsed)
                    this.Parse();
                return byTcpInfoValue;
            }
        }

        public string Via {
            get {
                if (!parsed)
                    this.Parse();
                return viaValue;
            }
        }

        public string With {
            get {
                if (!parsed)
                    this.Parse();
                return withValue;
            }
        }

        public string Id {
            get {
                if (!parsed)
                    this.Parse();
                return idValue;
            }
        }

        public string For {
            get {
                if (!parsed)
                    this.Parse();
                return forValue;
            }
        }

        public string Date {
            get {
                if (!parsed)
                    this.Parse();
                return dateValue;
            }
        }

        public override sealed string Value {
            get {
                return this.GetRawValue(true);
            }
            set {
                throw new NotSupportedException(Resources.Strings.UnicodeMimeHeaderReceivedNotSupported);
            }
        }

        public override sealed bool RequiresSMTPUTF8 => !MimeString.IsPureASCII(this.Lines);

        internal override void RawValueAboutToChange() {
            parsed = false;
        }

        public override sealed MimeNode Clone() {
            var receivedHeader = new ReceivedHeader();
            this.CopyTo(receivedHeader);
            return receivedHeader;
        }

        public override sealed void CopyTo(object destination) {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (destination == this)
                return;
            var receivedHeader = destination as ReceivedHeader;
            if (receivedHeader == null)
                throw new ArgumentException(Resources.Strings.CantCopyToDifferentObjectType);
            base.CopyTo(destination);
            receivedHeader.parsed = parsed;
            receivedHeader.fromValue = fromValue;
            receivedHeader.fromTcpInfoValue = fromTcpInfoValue;
            receivedHeader.byValue = byValue;
            receivedHeader.byTcpInfoValue = byTcpInfoValue;
            receivedHeader.viaValue = viaValue;
            receivedHeader.withValue = withValue;
            receivedHeader.idValue = idValue;
            receivedHeader.forValue = forValue;
            receivedHeader.dateValue = dateValue;
        }

        public override sealed bool IsValueValid(string value) {
            return false;
        }

        internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild) {
            throw new NotSupportedException(Resources.Strings.ChildrenCannotBeAddedToReceivedHeader);
        }

        private void AppendNameValue(byte[] name, string value, byte[] array, ref int offset) {
            if (value == null)
                return;
            this.AppendName(name, array, ref offset);
            array[offset++] = 32;
            var num = Internal.ByteString.StringToBytes(value, array, offset, true);
            offset += num;
        }

        private void AppendName(byte[] name, byte[] array, ref int offset) {
            if (offset != 0)
                array[offset++] = 32;
            Buffer.BlockCopy(name, 0, array, offset, name.Length);
            offset += name.Length;
        }

        private void Reset() {
            parsed = false;
            fromValue = null;
            fromTcpInfoValue = null;
            byValue = null;
            byTcpInfoValue = null;
            viaValue = null;
            withValue = null;
            idValue = null;
            forValue = null;
            dateValue = null;
        }

        private void Parse() {
            this.Reset();
            parsed = true;
            var headerDecodingOptions = this.GetHeaderDecodingOptions();
            var valueParser = new ValueParser(this.Lines, headerDecodingOptions.AllowUTF8);
            var mimeStringList1 = new MimeStringList();
            var mimeStringList2 = new MimeStringList();
            var mimeString1 = new MimeString();
            var mimeString2 = MimeString.Empty;
            var parseState1 = ParseState.None;
            while (true) {
                valueParser.ParseWhitespace(true, ref mimeStringList1);
                var num1 = valueParser.ParseGet();
                if (num1 != 0) {
                    if (59 == num1)
                        parseState1 = ParseState.Date;
                    else if (40 == num1 && parseState1 == ParseState.DomainValue)
                        parseState1 = ParseState.DomainAddress;
                    else {
                        valueParser.ParseUnget();
                        mimeString1 = valueParser.ParseToken();
                        if (mimeString1.Length == 0) {
                            int num2 = valueParser.ParseGet();
                            mimeStringList2.TakeOverAppend(ref mimeStringList1);
                            valueParser.ParseAppendLastByte(ref mimeStringList2);
                            continue;
                        }
                        var parseState2 = this.StateFromToken(mimeString1);
                        if (ParseState.None != parseState2)
                            parseState1 = parseState2;
                    }
                    switch (parseState1) {
                        case ParseState.Domain:
                        case ParseState.OptInfo:
                            if (mimeString2.Length != 0)
                                this.FinishClause(ref mimeString2, ref mimeStringList2, headerDecodingOptions.AllowUTF8);
                            else
                                mimeStringList2.Reset();
                            mimeString2 = mimeString1;
                            valueParser.ParseWhitespace(false, ref mimeStringList1);
                            mimeStringList1.Reset();
                            ++parseState1;
                            continue;
                        case ParseState.DomainValue:
                            mimeStringList2.TakeOverAppend(ref mimeStringList1);
                            mimeStringList2.AppendFragment(mimeString1);
                            continue;
                        case ParseState.DomainAddress:
                            var flag = mimeString2.CompareEqI(ParamFrom);
                            this.FinishClause(ref mimeString2, ref mimeStringList2, headerDecodingOptions.AllowUTF8);
                            mimeStringList1.Reset();
                            valueParser.ParseUnget();
                            valueParser.ParseComment(true, false, ref mimeStringList2, true);
                            var sz1 = mimeStringList2.GetSz();
                            var str = sz1 == null ? null : Internal.ByteString.BytesToString(sz1, headerDecodingOptions.AllowUTF8);
                            if (flag)
                                fromTcpInfoValue = str;
                            else
                                byTcpInfoValue = str;
                            mimeStringList2.Reset();
                            parseState1 = ParseState.None;
                            continue;
                        case ParseState.OptInfoValue:
                            mimeStringList2.TakeOverAppend(ref mimeStringList1);
                            mimeStringList2.AppendFragment(mimeString1);
                            continue;
                        case ParseState.Date:
                            this.FinishClause(ref mimeString2, ref mimeStringList2, headerDecodingOptions.AllowUTF8);
                            mimeStringList1.Reset();
                            valueParser.ParseWhitespace(false, ref mimeStringList1);
                            valueParser.ParseToEnd(ref mimeStringList2);
                            var sz2 = mimeStringList2.GetSz();
                            dateValue = sz2 == null ? null : Internal.ByteString.BytesToString(sz2, false);
                            continue;
                        case ParseState.None:
                            mimeStringList2.Reset();
                            continue;
                        default:
                            continue;
                    }
                }
                break;
            }
            this.FinishClause(ref mimeString2, ref mimeStringList2, headerDecodingOptions.AllowUTF8);
        }

        internal override void ForceParse() {
            if (parsed)
                return;
            this.Parse();
        }

        private void FinishClause(ref MimeString param, ref MimeStringList value, bool allowUTF8) {
            if (param.Length == 0)
                return;
            var sz = value.GetSz();
            var str = sz == null ? null : Internal.ByteString.BytesToString(sz, allowUTF8);
            var crcI = param.ComputeCrcI();
            if (crcI <= 2556329580U) {
                if ((int) crcI != 271896810) {
                    if ((int) crcI != 2115158205) {
                        if ((int) crcI == -1738637716)
                            fromValue = str;
                    } else
                        byValue = str;
                } else
                    forValue = str;
            } else if ((int) crcI != -1177273070) {
                if ((int) crcI != -554265150) {
                    if ((int) crcI == -18844241)
                        idValue = str;
                } else
                    viaValue = str;
            } else
                withValue = str;
            value.Reset();
            param = MimeString.Empty;
        }

        private ParseState StateFromToken(MimeString token) {
            var crcI = token.ComputeCrcI();
            if (crcI <= 2556329580U) {
                if ((int) crcI == 271896810)
                    return ParseState.OptInfo;
                if ((int) crcI == 2115158205 || (int) crcI == -1738637716)
                    return ParseState.Domain;
            } else if ((int) crcI == -1177273070 || (int) crcI == -554265150 || (int) crcI == -18844241)
                return ParseState.OptInfo;
            return ParseState.None;
        }

        internal const bool AllowUTF8Value = true;
        private const uint ParamFromCRC = 2556329580U;
        private const uint ParamByCRC = 2115158205U;
        private const uint ParamViaCRC = 3740702146U;
        private const uint ParamWithCRC = 3117694226U;
        private const uint ParamIdCRC = 4276123055U;
        private const uint ParamForCRC = 271896810U;
        private static readonly byte[] ParamFrom = Internal.ByteString.StringToBytes("from", true);
        private static readonly byte[] ParamBy = Internal.ByteString.StringToBytes("by", true);
        private static readonly byte[] ParamVia = Internal.ByteString.StringToBytes("via", true);
        private static readonly byte[] ParamWith = Internal.ByteString.StringToBytes("with", true);
        private static readonly byte[] ParamId = Internal.ByteString.StringToBytes("id", true);
        private static readonly byte[] ParamFor = Internal.ByteString.StringToBytes("for", true);
        private static readonly byte[] ParamDate = Internal.ByteString.StringToBytes("date", true);
        private static readonly byte[] ParamFromTcpInfo = Internal.ByteString.StringToBytes("x-from-tcp-info", true);
        private static readonly byte[] ParamByTcpInfo = Internal.ByteString.StringToBytes("x-by-tcp-info", true);
        private string byTcpInfoValue;
        private string byValue;
        private string dateValue;
        private string forValue;
        private string fromTcpInfoValue;
        private string fromValue;
        private string idValue;
        private bool parsed;
        private string viaValue;
        private string withValue;


        private enum ParseState {

            Domain,
            DomainValue,
            DomainAddress,
            OptInfo,
            OptInfoValue,
            Date,
            None

        }

    }

}