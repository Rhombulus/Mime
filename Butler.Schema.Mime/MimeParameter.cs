using System;
using System.Linq;

namespace Butler.Schema.Data.Mime {

    public class MimeParameter : MimeNode {

        public MimeParameter(string name) {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            this.Name = Header.NormalizeString(name);
        }

        public MimeParameter(string name, string value)
            : this(name) {
            decodedValue = value;
        }

        public string Name { get; private set; }

        public string Value {
            get {
                DecodingResults decodingResults;
                if (decodedValue == null && !this.TryGetValue(this.GetHeaderDecodingOptions(), out decodingResults, out decodedValue))
                    MimeCommon.ThrowDecodingFailedException(ref decodingResults);
                return decodedValue;
            }
            set {
                if (this.SegmentNumber == 0) {
                    this.RemoveAll();
                    this.SegmentNumber = -1;
                } else if (0 < this.SegmentNumber)
                    throw new NotSupportedException(Resources.Strings.CantSetValueOfRfc2231ContinuationSegment);
                this.RawValue = null;
                decodedValue = value;
            }
        }

        internal byte[] RawValue {
            get {
                if (valueFragments.Length == 0 && decodedValue != null && 0 < decodedValue.Length)
                    valueFragments = this.EncodeValue(decodedValue, this.Name == "charset" ? EncodingOptionsAscii : this.GetDocumentEncodingOptions());
                return valueFragments.GetSz(4026531839U);
            }
            set {
                if (this.SegmentNumber == 0) {
                    this.RemoveAll();
                    this.SegmentNumber = -1;
                } else if (0 < this.SegmentNumber)
                    throw new NotSupportedException(Resources.Strings.CantSetRawValueOfRfc2231ContinuationSegment);
                decodedValue = null;
                valueFragments.Reset();
                valueEncoded = false;
                if (value != null && 0 < value.Length)
                    valueFragments.AppendFragment(new MimeString(value));
                this.SetDirty();
            }
        }

        internal int RawLength => valueFragments.GetLength(4026531839U);

        internal bool ValueEncoded {
            set {
                valueEncoded = value;
            }
        }

        internal int SegmentNumber { get; set; } = -1;

        internal bool AllowAppend {
            set {
                allowAppend = value;
            }
        }

        public override sealed MimeNode Clone() {
            var mimeParameter = new MimeParameter(this.Name);
            this.CopyTo(mimeParameter);
            return mimeParameter;
        }

        public override sealed void CopyTo(object destination) {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (destination == this)
                return;
            var mimeParameter = destination as MimeParameter;
            if (mimeParameter == null)
                throw new ArgumentException(Resources.Strings.CantCopyToDifferentObjectType);
            mimeParameter.AllowAppend = true;
            base.CopyTo(destination);
            mimeParameter.AllowAppend = false;
            mimeParameter.valueEncoded = valueEncoded;
            mimeParameter.SegmentNumber = this.SegmentNumber;
            mimeParameter.valueFragments = valueFragments.Clone();
            mimeParameter.decodedValue = decodedValue;
            mimeParameter.Name = this.Name;
        }

        public bool TryGetValue(out string value) {
            DecodingResults decodingResults;
            return this.TryGetValue(this.GetHeaderDecodingOptions(), out decodingResults, out value);
        }

        public bool TryGetValue(DecodingOptions decodingOptions, out DecodingResults decodingResults, out string value) {
            if (decodingOptions.Charset == null)
                decodingOptions.Charset = this.GetDefaultHeaderDecodingCharset(null, null);
            if ((DecodingFlags.Rfc2231 & decodingOptions.DecodingFlags) != DecodingFlags.None) {
                if (this.SegmentNumber == 0)
                    return this.TryDecodeRfc2231(ref decodingOptions, out decodingResults, out value);
                if (0 < this.SegmentNumber)
                    throw new NotSupportedException(Resources.Strings.CantGetValueOfRfc2231ContinuationSegment);
            }
            if (valueFragments.Length != 0)
                return MimeCommon.TryDecodeValue(valueFragments, 4026531839U, decodingOptions, out decodingResults, out value);
            decodingResults = new DecodingResults();
            value = decodedValue != null ? decodedValue : string.Empty;
            return true;
        }

        internal static string CorrectValue(string value) {
            if (-1 == value.IndexOfAny(ControlCharacters))
                return value;
            return MimeParameter.CorrectValue(value.ToCharArray());
        }

        internal static string CorrectValue(char[] value) {
            var length = ControlCharacters.Length;
            for (var index1 = 0; index1 < value.Length; ++index1) {
                for (var index2 = 0; index2 < length; ++index2) {
                    if (value[index1] == ControlCharacters[index2]) {
                        value[index1] = ' ';
                        break;
                    }
                }
            }
            return new string(value);
        }

        internal bool FallBackIfRequired(byte[] bytes, DecodingOptions decodingOptions, out string value) {
            if ((DecodingFlags.FallbackToRaw & decodingOptions.DecodingFlags) != DecodingFlags.None) {
                if (bytes == null)
                    value = string.Empty;
                else {
                    value = Internal.ByteString.BytesToString(bytes, decodingOptions.AllowUTF8);
                    if ((DecodingFlags.AllowControlCharacters & decodingOptions.DecodingFlags) == DecodingFlags.None)
                        value = MimeParameter.CorrectValue(value);
                }
                return true;
            }
            value = null;
            return false;
        }

        internal bool IsName(string name) {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            return this.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
        }

        internal override long WriteTo(System.IO.Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer) {
            var fragments = valueFragments;
            var num1 = 0L;
            if (valueFragments.Length == 0 && decodedValue != null && 0 < decodedValue.Length) {
                fragments = this.EncodeValue(decodedValue, encodingOptions);
                valueFragments = fragments;
            } else if ((EncodingFlags.ForceReencode & encodingOptions.EncodingFlags) != EncodingFlags.None && 0 >= this.SegmentNumber)
                fragments = this.EncodeValue(this.Value, encodingOptions);
            var quoteOutput = false;
            if (this.IsQuotingReqired() || fragments.Length == 0)
                quoteOutput = true;
            else {
                for (var index = 0; index < fragments.Count; ++index) {
                    var str = fragments[index];
                    var characterCount = 0;
                    var num2 = ValueParser.ParseToken(str, out characterCount, encodingOptions.AllowUTF8);
                    if (268435456 != (int) str.Mask && str.Length != num2) {
                        quoteOutput = true;
                        break;
                    }
                }
            }
            MimeNode mimeNode = null;
            if (this.SegmentNumber == 0) {
                mimeNode = this.FirstChild;
                while (mimeNode != null && !(mimeNode is MimeParameter))
                    mimeNode = mimeNode.NextSibling;
            }
            var mimeString = this.SegmentNumber == 0 && mimeNode != null || 0 < this.SegmentNumber ? new MimeString(this.SegmentNumber.ToString()) : new MimeString();
            if (1 < currentLineLength.InChars) {
                var num2 = 1 + this.Name.Length + 1;
                var num3 = Internal.ByteString.BytesToCharCount(fragments.GetSz(), encodingOptions.AllowUTF8);
                if (mimeString.Length != 0)
                    num2 += 1 + mimeString.Length;
                if (valueEncoded)
                    ++num2;
                var num4 = num3;
                if (quoteOutput)
                    num4 += 2;
                if (this.NextSibling != null) {
                    if (num3 == 0)
                        ++num2;
                    else
                        ++num4;
                }
                var num5 = num4 + num2;
                if (currentLineLength.InChars + num5 > 78) {
                    var num6 = num1 + Header.WriteLineEnd(stream, ref currentLineLength);
                    stream.Write(Header.LineStartWhitespace, 0, Header.LineStartWhitespace.Length);
                    num1 = num6 + Header.LineStartWhitespace.Length;
                    currentLineLength.IncrementBy(Header.LineStartWhitespace.Length);
                } else {
                    stream.Write(MimeString.Space, 0, MimeString.Space.Length);
                    num1 += MimeString.Space.Length;
                    currentLineLength.IncrementBy(MimeString.Space.Length);
                }
            }
            var val2 = Internal.ByteString.StringToBytesCount(this.Name, false);
            if (scratchBuffer == null || scratchBuffer.Length < val2)
                scratchBuffer = new byte[Math.Max(998, val2)];
            var num7 = Internal.ByteString.StringToBytes(this.Name, scratchBuffer, 0, false);
            stream.Write(scratchBuffer, 0, num7);
            var num8 = num1 + num7;
            currentLineLength.IncrementBy(this.Name.Length, num7);
            if (mimeString.Length != 0) {
                stream.Write(MimeString.Asterisk, 0, MimeString.Asterisk.Length);
                var num2 = num8 + MimeString.Asterisk.Length;
                currentLineLength.IncrementBy(MimeString.Asterisk.Length);
                mimeString.WriteTo(stream);
                num8 = num2 + mimeString.Length;
                currentLineLength.IncrementBy(mimeString.Length);
            }
            if (valueEncoded) {
                stream.Write(MimeString.Asterisk, 0, MimeString.Asterisk.Length);
                num8 += MimeString.Asterisk.Length;
                currentLineLength.IncrementBy(MimeString.Asterisk.Length);
            }
            stream.Write(MimeString.EqualTo, 0, MimeString.EqualTo.Length);
            var num9 = num8 + MimeString.EqualTo.Length;
            currentLineLength.IncrementBy(MimeString.EqualTo.Length);
            var lastLineReserve = 0;
            if (this.NextSibling != null)
                ++lastLineReserve;
            var num10 = num9 + Header.QuoteAndFold(stream, fragments, 4026531839U, quoteOutput, false, encodingOptions.AllowUTF8, lastLineReserve, ref currentLineLength, ref scratchBuffer);
            var num11 = 0;
            for (; mimeNode != null; mimeNode = mimeNode.NextSibling) {
                var mimeParameter = mimeNode as MimeParameter;
                if (mimeParameter != null) {
                    ++num11;
                    mimeParameter.SegmentNumber = num11;
                    stream.Write(MimeString.Semicolon, 0, MimeString.Semicolon.Length);
                    var num2 = num10 + MimeString.Semicolon.Length;
                    currentLineLength.IncrementBy(MimeString.Semicolon.Length);
                    var num3 = num2 + Header.WriteLineEnd(stream, ref currentLineLength);
                    stream.Write(Header.LineStartWhitespace, 0, Header.LineStartWhitespace.Length);
                    var num4 = num3 + Header.LineStartWhitespace.Length;
                    currentLineLength.IncrementBy(Header.LineStartWhitespace.Length);
                    num10 = num4 + mimeNode.WriteTo(stream, encodingOptions, null, ref currentLineLength, ref scratchBuffer);
                }
            }
            return num10;
        }

        internal override MimeNode ParseNextChild() {
            if (valueFragments.Length != 0 || decodedValue == null || decodedValue.Length == 0)
                return null;
            if (this.InternalLastChild != null)
                return null;
            valueFragments = this.EncodeValue(decodedValue, this.GetDocumentEncodingOptions());
            return this.FirstChild;
        }

        internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild) {
            if (allowAppend)
                return refChild;
            throw new NotSupportedException(Resources.Strings.ParametersCannotHaveChildNodes);
        }

        internal void AppendValue(ref MimeStringList list) {
            valueFragments.TakeOverAppend(ref list);
        }

        private static byte DecodeHexadecimal(byte ch) {
            ch -= 48;
            if (ch <= 9)
                return ch;
            ch -= 17;
            if (ch <= 5)
                return (byte) (ch + 10U);
            ch -= 32;
            if (ch <= 5)
                return (byte) (ch + 10U);
            return byte.MaxValue;
        }

        private bool IsQuotingReqired() {
            if (this.Name != "boundary" && this.Name != "filename" && (this.Name != "name" && this.Name != "id"))
                return this.Name == "charset";
            return true;
        }

        private MimeStringList EncodeValue(string value, EncodingOptions encodingOptions) {
            valueFragments.Reset();
            var count = valueFragments.Count;
            if ((EncodingFlags.EnableRfc2231 & encodingOptions.EncodingFlags) == EncodingFlags.None)
                return MimeCommon.EncodeValue(value, encodingOptions, ValueEncodingStyle.Normal);
            this.EncodeRfc2231(encodingOptions);
            return valueFragments;
        }

        private void EncodeRfc2231(EncodingOptions encodingOptions) {
            if (!MimeCommon.IsEncodingRequired(decodedValue, encodingOptions.AllowUTF8)) {
                valueEncoded = false;
                this.SegmentNumber = -1;
                valueFragments.AppendFragment(new MimeString(decodedValue));
            } else {
                var encodingCharset = encodingOptions.GetEncodingCharset();
                var encoding = encodingCharset.GetEncoding();
                var bytesOffset1 = 0;
                var num1 = 0;
                var str = encodingOptions.CultureName == null ? string.Empty : encodingOptions.CultureName;
                if (encodingOptions.AllowUTF8 && encodingCharset.CodePage != 20127 && encodingCharset.CodePage != 65001 || (!encodingOptions.AllowUTF8 && encodingCharset.CodePage != 20127 || "en-us" != str)) {
                    var name = encodingCharset.Name;
                    var numArray1 = new byte[name.Length + str.Length + 2];
                    var num2 = bytesOffset1 + Internal.ByteString.StringToBytes(name, numArray1, bytesOffset1, false);
                    var num3 = num1 + name.Length;
                    var numArray2 = numArray1;
                    var index1 = num2;
                    var num4 = 1;
                    var bytesOffset2 = index1 + num4;
                    var num5 = 39;
                    numArray2[index1] = (byte) num5;
                    var num6 = num3 + 1;
                    var num7 = bytesOffset2 + Internal.ByteString.StringToBytes(str, numArray1, bytesOffset2, false);
                    var num8 = num6 + str.Length;
                    var numArray3 = numArray1;
                    var index2 = num7;
                    var num9 = 1;
                    var count = index2 + num9;
                    var num10 = 39;
                    numArray3[index2] = (byte) num10;
                    num1 = num8 + 1;
                    valueFragments.AppendFragment(new MimeString(numArray1, 0, count));
                    valueEncoded = true;
                }
                var num11 = 78 - this.Name.Length - 6;
                var num12 = 2;
                var bytes = encoding.GetBytes(decodedValue);
                var sourceIndex = this.EncodeRfc2231Segment(bytes, 0, num11 - num12 - num1, encodingOptions);
                this.SegmentNumber = sourceIndex < bytes.Length ? 0 : -1;
                var num13 = 1;
                var num14 = 10;
                this.AllowAppend = true;
                while (sourceIndex < bytes.Length) {
                    var mimeParameter = new MimeParameter(this.Name);
                    if (num14 == num13) {
                        ++num12;
                        num14 *= 10;
                    }
                    sourceIndex = mimeParameter.EncodeRfc2231Segment(bytes, sourceIndex, num11 - num12, encodingOptions);
                    mimeParameter.SegmentNumber = num13++;
                    this.InternalAppendChild(mimeParameter);
                    if (10000 == num13)
                        break;
                }
                this.AllowAppend = false;
            }
        }

        private int EncodeRfc2231Segment(byte[] source, int sourceIndex, int maxValueLength, EncodingOptions encodingOptions) {
            var data = new byte[maxValueLength*4];
            var count = 0;
            var num1 = 0;
            while (sourceIndex < source.Length) {
                var bytesUsed = 1;
                var ch = source[sourceIndex];
                var flag = true;
                if (ch >= 128) {
                    if (encodingOptions.AllowUTF8 && MimeScan.IsUTF8NonASCII(source, sourceIndex, source.Length, out bytesUsed))
                        flag = false;
                } else if (!MimeScan.IsSegmentEncodingRequired(ch))
                    flag = false;
                if (flag) {
                    if (num1 + 3 <= maxValueLength) {
                        var numArray1 = data;
                        var index1 = count;
                        var num2 = 1;
                        var num3 = index1 + num2;
                        var num4 = 37;
                        numArray1[index1] = (byte) num4;
                        var numArray2 = data;
                        var index2 = num3;
                        var num5 = 1;
                        var num6 = index2 + num5;
                        int num7 = OctetEncoderMap[ch >> 4];
                        numArray2[index2] = (byte) num7;
                        var numArray3 = data;
                        var index3 = num6;
                        var num8 = 1;
                        count = index3 + num8;
                        int num9 = OctetEncoderMap[ch & 15];
                        numArray3[index3] = (byte) num9;
                        num1 += 3;
                        valueEncoded = true;
                        ++sourceIndex;
                    } else
                        break;
                } else if (num1 + 1 <= maxValueLength) {
                    for (; bytesUsed > 0; --bytesUsed) {
                        data[count++] = source[sourceIndex++];
                    }
                    ++num1;
                } else
                    break;
            }
            valueFragments.AppendFragment(new MimeString(data, 0, count));
            return sourceIndex;
        }

        private bool TryDecodeRfc2231(ref DecodingOptions decodingOptions, out DecodingResults decodingResults, out string value) {
            decodingResults = new DecodingResults();
            decodingResults.EncodingScheme = EncodingScheme.Rfc2231;
            Globalization.Charset charset = null;
            var sz = valueFragments.GetSz(4026531839U);
            var sourceIndex = 0;
            if (valueEncoded) {
                var num1 = sz == null ? -1 : Internal.ByteString.IndexOf(sz, 39, 0, sz.Length);
                if (-1 < num1 && num1 < sz.Length - 1) {
                    int offset;
                    var num2 = Internal.ByteString.IndexOf(sz, 39, offset = num1 + 1, sz.Length - offset);
                    if (-1 < num2) {
                        decodingResults.CharsetName = Internal.ByteString.BytesToString(sz, 0, offset - 1, false);
                        decodingResults.CultureName = Internal.ByteString.BytesToString(sz, offset, num2 - offset, false);
                        if (!Globalization.Charset.TryGetCharset(decodingResults.CharsetName, out charset)) {
                            decodingResults.DecodingFailed = true;
                            return this.FallBackIfRequired(sz, decodingOptions, out value);
                        }
                        sourceIndex = num2 + 1;
                    }
                }
            }
            if (charset == null)
                charset = decodingOptions.Charset ?? DecodingOptions.DefaultCharset;
            decodingResults.CharsetName = charset.Name;
            System.Text.Encoding encoding;
            if (!charset.TryGetEncoding(out encoding)) {
                decodingResults.DecodingFailed = true;
                return this.FallBackIfRequired(sz, decodingOptions, out value);
            }
            var length = valueFragments.Length - sourceIndex;
            for (var mimeNode = this.FirstChild; mimeNode != null; mimeNode = mimeNode.NextSibling) {
                var mimeParameter = mimeNode as MimeParameter;
                if (mimeParameter != null)
                    length += mimeParameter.RawLength;
            }
            var numArray = new byte[length];
            var num = 0;
            if (sz != null && sourceIndex < sz.Length)
                num += this.DecodeRfc2231Octets(valueEncoded, sz, sourceIndex, numArray, 0);
            for (var mimeNode = this.FirstChild; mimeNode != null; mimeNode = mimeNode.NextSibling) {
                var mimeParameter = mimeNode as MimeParameter;
                if (mimeParameter != null && mimeParameter.RawValue != null)
                    num += this.DecodeRfc2231Octets(mimeParameter.valueEncoded, mimeParameter.RawValue, 0, numArray, num);
            }
            value = num != 0 ? encoding.GetString(numArray, 0, num) : string.Empty;
            if ((DecodingFlags.AllowControlCharacters & decodingOptions.DecodingFlags) == DecodingFlags.None)
                value = MimeParameter.CorrectValue(value);
            return true;
        }

        private int DecodeRfc2231Octets(bool decode, byte[] source, int sourceIndex, byte[] destination, int destinationIndex) {
            if (decode) {
                var num1 = destinationIndex;
                while (sourceIndex < source.Length) {
                    if (37 == source[sourceIndex] && sourceIndex + 2 < source.Length) {
                        var num2 = MimeParameter.DecodeHexadecimal(source[sourceIndex + 1]);
                        if (byte.MaxValue != num2) {
                            var num3 = MimeParameter.DecodeHexadecimal(source[sourceIndex + 2]);
                            if (byte.MaxValue != num3) {
                                sourceIndex += 3;
                                destination[destinationIndex++] = (byte) (((uint) num2 << 4) + num3);
                                continue;
                            }
                        }
                    }
                    destination[destinationIndex++] = source[sourceIndex++];
                }
                return destinationIndex - num1;
            }
            var count = source.Length - sourceIndex;
            Buffer.BlockCopy(source, sourceIndex, destination, destinationIndex, count);
            return count;
        }

        internal const bool AllowUTF8Name = false;
        private const string DefaultLanguage = "en-us";
        private static readonly EncodingOptions EncodingOptionsAscii = new EncodingOptions(Globalization.Charset.ASCII);

        private static readonly byte[] OctetEncoderMap = new byte[16] {
            48,
            49,
            50,
            51,
            52,
            53,
            54,
            55,
            56,
            57,
            65,
            66,
            67,
            68,
            69,
            70
        };

        private static readonly char[] ControlCharacters = new char[5] {
            char.MinValue,
            '\r',
            '\n',
            '\f',
            '\v'
        };

        private bool allowAppend;
        private string decodedValue;
        private bool valueEncoded;
        private MimeStringList valueFragments;

    }

}