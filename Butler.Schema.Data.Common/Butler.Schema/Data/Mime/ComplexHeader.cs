using System;
using System.Collections.Generic;
using System.Linq;

namespace Butler.Schema.Data.Mime {

    public abstract class ComplexHeader : Header, IEnumerable<MimeParameter>, System.Collections.IEnumerable {

        internal ComplexHeader(string name, HeaderId headerId)
            : base(name, headerId) {}

        public MimeParameter this[string name] {
            get {
                if (name == null)
                    throw new ArgumentNullException(nameof(name));
                for (var mimeNode = this.FirstChild; mimeNode != null; mimeNode = mimeNode.NextSibling) {
                    var mimeParameter = mimeNode as MimeParameter;
                    if (mimeParameter.IsName(name))
                        return mimeParameter;
                }
                return null;
            }
        }

        IEnumerator<MimeParameter> IEnumerable<MimeParameter>.GetEnumerator() {
            return new Enumerator<MimeParameter>(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return new Enumerator<MimeParameter>(this);
        }

        public Enumerator<MimeParameter> GetEnumerator() {
            return new Enumerator<MimeParameter>(this);
        }

        internal override long WriteTo(System.IO.Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer) {
            var num1 = this.WriteName(stream, ref scratchBuffer);
            currentLineLength.IncrementBy((int) num1);
            if (!this.IsDirty && this.RawLength != 0 && this.IsProtected) {
                var num2 = Header.WriteLines(this.Lines, stream);
                var num3 = num1 + num2;
                currentLineLength.SetAs(0);
                return num3;
            }
            if (!parsed)
                this.Parse();
            var num4 = num1 + this.WriteValue(stream, encodingOptions, filter, ref currentLineLength, ref scratchBuffer);
            for (var mimeNode = this.FirstChild; mimeNode != null; mimeNode = mimeNode.NextSibling) {
                stream.Write(MimeString.Semicolon, 0, MimeString.Semicolon.Length);
                var num2 = num4 + MimeString.Semicolon.Length;
                currentLineLength.IncrementBy(MimeString.Semicolon.Length);
                num4 = num2 + mimeNode.WriteTo(stream, encodingOptions, filter, ref currentLineLength, ref scratchBuffer);
            }
            return num4 + Header.WriteLineEnd(stream, ref currentLineLength);
        }

        internal virtual long WriteValue(System.IO.Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer) {
            var num1 = 0L;
            var str = this.Value;
            if (str != null) {
                var val2 = Internal.ByteString.StringToBytesCount(str, encodingOptions.AllowUTF8) + 1;
                if (scratchBuffer == null || scratchBuffer.Length < val2)
                    scratchBuffer = new byte[Math.Max(998, val2)];
                scratchBuffer[0] = 32;
                var num2 = Internal.ByteString.StringToBytes(str, scratchBuffer, 1, encodingOptions.AllowUTF8);
                stream.Write(scratchBuffer, 0, num2 + 1);
                num1 += num2 + 1;
                currentLineLength.IncrementBy(str.Length + 1, num2 + 1);
            }
            return num1;
        }

        internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild) {
            var mimeParameter1 = newChild as MimeParameter;
            if (mimeParameter1 == null)
                throw new ArgumentException(CtsResources.Strings.NewChildNotMimeParameter, nameof(newChild));
            var mimeParameter2 = this[mimeParameter1.Name];
            if (mimeParameter2 != null) {
                if (mimeParameter2 == refChild)
                    refChild = mimeParameter2.PreviousSibling;
                this.InternalRemoveChild(mimeParameter2);
            }
            return refChild;
        }

        protected void Parse() {
            parsed = true;
            var headerDecodingOptions = this.GetHeaderDecodingOptions();
            var parser = new ValueParser(this.Lines, headerDecodingOptions.AllowUTF8);
            this.ParseValue(parser, true);
            this.ParseParameters(ref parser, headerDecodingOptions, int.MaxValue, int.MaxValue);
        }

        internal override void ForceParse() {
            if (parsed)
                return;
            this.Parse();
        }

        internal override void CheckChildrenLimit(int countLimit, int bytesLimit) {
            var headerDecodingOptions = this.GetHeaderDecodingOptions();
            var parser = new ValueParser(this.Lines, headerDecodingOptions.AllowUTF8);
            this.ParseValue(parser, false);
            this.ParseParameters(ref parser, headerDecodingOptions, countLimit, bytesLimit);
        }

        internal override void RemoveAllUnparsed() {
            parsed = true;
        }

        internal override MimeNode ParseNextChild() {
            if (parsed)
                return null;
            var internalLastChild = this.InternalLastChild;
            this.Parse();
            if (internalLastChild != null)
                return internalLastChild.NextSibling;
            return this.FirstChild;
        }

        internal abstract void ParseValue(ValueParser parser, bool storeValue);

        internal void ParseParameters(ref ValueParser parser, DecodingOptions decodingOptions, int countLimit, int bytesLimit) {
            var phrase = new MimeStringList();
            var list = new MimeStringList();
            var goodValue = false;
            var actual = 0;
            var flag1 = DecodingFlags.None != (DecodingFlags.Rfc2231 & decodingOptions.DecodingFlags);
            byte num1;
            do {
                parser.ParseCFWS(false, ref phrase, handleISO2022);
                num1 = parser.ParseGet();
                if (num1 != 59) {
                    if (num1 == 0)
                        break;
                    parser.ParseUnget();
                    parser.ParseSkipToNextDelimiterByte(59);
                } else {
                    parser.ParseCFWS(false, ref phrase, handleISO2022);
                    var mimeString = parser.ParseToken();
                    if (mimeString.Length != 0 && mimeString.Length < 128) {
                        parser.ParseCFWS(false, ref phrase, handleISO2022);
                        num1 = parser.ParseGet();
                        switch (num1) {
                            case 61:
                                parser.ParseCFWS(false, ref phrase, handleISO2022);
                                parser.ParseParameterValue(ref list, ref goodValue, handleISO2022);
                                if (int.MaxValue != countLimit || int.MaxValue != bytesLimit) {
                                    if (++actual > countLimit)
                                        throw new MimeException(CtsResources.Strings.TooManyParameters(actual, countLimit));
                                    if (list.Length > bytesLimit)
                                        throw new MimeException(CtsResources.Strings.TooManyTextValueBytes(list.Length, bytesLimit));
                                    break;
                                }
                                var flag2 = false;
                                var num2 = -1;
                                if (flag1) {
                                    var num3 = Internal.ByteString.IndexOf(mimeString.Data, 42, mimeString.Offset, mimeString.Length);
                                    if (num3 > 0) {
                                        var num4 = mimeString.Offset + mimeString.Length;
                                        var index = num3 + 1;
                                        num2 = 0;
                                        for (; index != num4 && (int) mimeString.Data[index] >= 48 && (int) mimeString.Data[index] <= 57; ++index) {
                                            num2 = num2*10 + (mimeString.Data[index] - 48);
                                            if (10000 < num2) {
                                                num2 = -1;
                                                break;
                                            }
                                        }
                                        if (-1 != num2) {
                                            var flag3 = 42 == mimeString.Data[num4 - 1];
                                            if (index < num4 - 1 || index < num4 && !flag3)
                                                num2 = -1;
                                            else {
                                                flag2 = flag3;
                                                mimeString.TrimRight(num4 - num3);
                                            }
                                        }
                                    }
                                }
                                var name = Header.NormalizeString(mimeString.Data, mimeString.Offset, mimeString.Length, false);
                                var mimeParameter1 = new MimeParameter(name);
                                mimeParameter1.AppendValue(ref list);
                                mimeParameter1.SegmentNumber = num2;
                                mimeParameter1.ValueEncoded = flag2;
                                MimeNode oldChild;
                                MimeNode nextSibling1;
                                for (oldChild = this.FirstChild; oldChild != null; oldChild = nextSibling1) {
                                    nextSibling1 = oldChild.NextSibling;
                                    var mimeParameter2 = oldChild as MimeParameter;
                                    if (mimeParameter2 != null && mimeParameter2.Name == name)
                                        break;
                                }
                                if (0 >= num2) {
                                    if (oldChild != null) {
                                        mimeParameter1.AllowAppend = true;
                                        MimeNode nextSibling2;
                                        for (var mimeNode = oldChild.FirstChild; mimeNode != null; mimeNode = nextSibling2) {
                                            nextSibling2 = mimeNode.NextSibling;
                                            if (mimeNode is MimeParameter) {
                                                oldChild.RemoveChild(mimeNode);
                                                mimeParameter1.InternalAppendChild(mimeNode);
                                            }
                                        }
                                        mimeParameter1.AllowAppend = false;
                                        this.InternalRemoveChild(oldChild);
                                    }
                                    this.InternalAppendChild(mimeParameter1);
                                    break;
                                }
                                if (oldChild == null) {
                                    var mimeParameter2 = new MimeParameter(name);
                                    mimeParameter2.SegmentNumber = 0;
                                    this.InternalAppendChild(mimeParameter2);
                                    oldChild = mimeParameter2;
                                }
                                MimeNode refChild;
                                MimeNode previousSibling;
                                for (refChild = oldChild.LastChild; refChild != null; refChild = previousSibling) {
                                    previousSibling = refChild.PreviousSibling;
                                    var mimeParameter2 = refChild as MimeParameter;
                                    if (mimeParameter2 != null && mimeParameter2.SegmentNumber <= num2)
                                        break;
                                }
                                var mimeParameter3 = (MimeParameter) oldChild;
                                mimeParameter3.AllowAppend = true;
                                mimeParameter3.InternalInsertAfter(mimeParameter1, refChild);
                                mimeParameter3.AllowAppend = false;
                                break;
                            case 0:
                                return;
                            default:
                                parser.ParseUnget();
                                break;
                        }
                    }
                }
            } while (num1 != 0);
        }

        protected bool handleISO2022 = true;
        protected bool parsed;

    }

}