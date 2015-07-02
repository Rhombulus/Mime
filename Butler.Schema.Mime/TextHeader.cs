using System.Linq;

namespace Butler.Schema.Mime {

    public class TextHeader : Header {

        public TextHeader(string name, string value)
            : this(name, Header.GetHeaderId(name, true)) {
            var type = Header.TypeFromHeaderId(this.HeaderId);
            if (this.HeaderId != HeaderId.Unknown && type != typeof (TextHeader) && type != typeof (AsciiTextHeader))
                throw new System.ArgumentException(Resources.Strings.NameNotValidForThisHeaderType(name, "TextHeader", type.Name));
            this.Value = value;
        }

        internal TextHeader(string name, HeaderId headerId)
            : base(name, headerId) {}

        public override sealed string Value {
            get {
                if (this.decodedValue == null) {
                    DecodingResults decodingResults;
                    var decodedValue = this.GetDecodedValue(this.GetHeaderDecodingOptions(), out decodingResults);
                    if (decodingResults.DecodingFailed)
                        MimeCommon.ThrowDecodingFailedException(ref decodingResults);
                    this.decodedValue = decodedValue;
                }
                return this.decodedValue;
            }
            set {
                this.SetRawValue(null, true);
                decodedValue = value;
            }
        }

        internal override byte[] RawValue {
            get {
                var mimeStringList = this.RawLength != 0 || decodedValue == null || decodedValue.Length == 0 ? this.Lines : this.GetEncodedValue(this.GetDocumentEncodingOptions(), ValueEncodingStyle.Normal);
                if (mimeStringList.Length == 0)
                    return MimeString.EmptyByteArray;
                return mimeStringList.GetSz() ?? MimeString.EmptyByteArray;
            }
            set {
                base.RawValue = value;
            }
        }

        internal override void ForceParse() {
            var str = this.Value;
        }

        internal override void RawValueAboutToChange() {
            decodedValue = null;
        }

        public override sealed MimeNode Clone() {
            var textHeader = new TextHeader(this.Name, this.HeaderId);
            this.CopyTo(textHeader);
            return textHeader;
        }

        public override sealed void CopyTo(object destination) {
            if (destination == null)
                throw new System.ArgumentNullException(nameof(destination));
            if (destination == this)
                return;
            var textHeader = destination as TextHeader;
            if (textHeader == null)
                throw new System.ArgumentException(Resources.Strings.CantCopyToDifferentObjectType);
            base.CopyTo(destination);
            textHeader.decodedValue = decodedValue;
        }

        public override bool TryGetValue(out string value) {
            DecodingResults decodingResults;
            return this.TryGetValue(this.GetHeaderDecodingOptions(), out decodingResults, out value);
        }

        public bool TryGetValue(DecodingOptions decodingOptions, out DecodingResults decodingResults, out string value) {
            value = this.GetDecodedValue(decodingOptions, out decodingResults);
            if (!decodingResults.DecodingFailed)
                return true;
            value = null;
            return false;
        }

        internal string GetDecodedValue(DecodingOptions decodingOptions, out DecodingResults decodingResults) {
            string str1 = null;
            if (this.Lines.Length == 0) {
                var str2 = decodedValue != null ? decodedValue : string.Empty;
                decodingResults = new DecodingResults();
                return str2;
            }
            if (decodingOptions.Charset == null)
                decodingOptions.Charset = this.GetDefaultHeaderDecodingCharset(null, null);
            if (!MimeCommon.TryDecodeValue(this.Lines, 4026531840U, decodingOptions, out decodingResults, out str1))
                str1 = null;
            return str1;
        }

        internal MimeStringList GetEncodedValue(EncodingOptions encodingOptions, ValueEncodingStyle encodingStyle) {
            if (string.IsNullOrEmpty(decodedValue))
                return this.Lines;
            return MimeCommon.EncodeValue(decodedValue, encodingOptions, encodingStyle);
        }

        public override sealed bool IsValueValid(string value) {
            return true;
        }

        internal override long WriteTo(System.IO.Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer) {
            var nameLength = this.WriteName(stream, ref scratchBuffer);
            currentLineLength.IncrementBy((int) nameLength);
            MimeStringList mimeStringList;
            if (this.RawLength == 0 && decodedValue != null && decodedValue.Length != 0)
                mimeStringList = this.GetEncodedValue(encodingOptions, ValueEncodingStyle.Normal);
            else if ((EncodingFlags.ForceReencode & encodingOptions.EncodingFlags) != EncodingFlags.None) {
                this.ForceParse();
                mimeStringList = this.GetEncodedValue(encodingOptions, ValueEncodingStyle.Normal);
            } else {
                var merge = false;
                if (!this.IsDirty && this.RawLength != 0) {
                    if (this.IsProtected) {
                        var num = nameLength + Header.WriteLines(this.Lines, stream);
                        currentLineLength.SetAs(0);
                        return num;
                    }
                    if (!this.IsHeaderLineTooLong(nameLength, out merge)) {
                        var num = nameLength + Header.WriteLines(this.Lines, stream);
                        currentLineLength.SetAs(0);
                        return num;
                    }
                }
                mimeStringList = this.Lines;
                if (merge)
                    mimeStringList = Header.MergeLines(mimeStringList);
            }
            return nameLength + Header.QuoteAndFold(stream, mimeStringList, 4026531840U, false, mimeStringList.Length > 0, encodingOptions.AllowUTF8, 0, ref currentLineLength, ref scratchBuffer) +
                   Header.WriteLineEnd(stream, ref currentLineLength);
        }

        internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild) {
            throw new System.NotSupportedException(Resources.Strings.ChildrenCannotBeAddedToTextHeader);
        }

        private string decodedValue;

    }

}