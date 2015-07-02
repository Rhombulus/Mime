using System.Linq;

namespace Butler.Schema.Mime {

    public class ContentDispositionHeader : ComplexHeader {

        public ContentDispositionHeader()
            : base("Content-Disposition", HeaderId.ContentDisposition) {
            disp = "attachment";
            parsed = true;
        }

        public ContentDispositionHeader(string value)
            : base("Content-Disposition", HeaderId.ContentDisposition) {
            this.Value = value;
        }

        public override sealed string Value {
            get {
                if (!parsed)
                    this.Parse();
                return disp;
            }
            set {
                if (value == null)
                    throw new System.ArgumentNullException(nameof(value));
                if (ValueParser.ParseToken(value, 0, false) != value.Length)
                    throw new System.ArgumentException("Value should be a valid token", nameof(value));
                if (!parsed)
                    this.Parse();
                if (value == disp)
                    return;
                this.SetRawValue(null, true);
                parsed = true;
                disp = Header.NormalizeString(value);
            }
        }

        internal override byte[] RawValue {
            get {
                if (!parsed)
                    this.Parse();
                return Internal.ByteString.StringToBytes(disp, false) ?? MimeString.EmptyByteArray;
            }
            set {
                base.RawValue = value;
            }
        }

        internal override void RawValueAboutToChange() {
            this.Reset();
        }

        public override sealed MimeNode Clone() {
            var dispositionHeader = new ContentDispositionHeader();
            this.CopyTo(dispositionHeader);
            return dispositionHeader;
        }

        public override sealed void CopyTo(object destination) {
            if (destination == null)
                throw new System.ArgumentNullException(nameof(destination));
            if (destination == this)
                return;
            var dispositionHeader = destination as ContentDispositionHeader;
            if (dispositionHeader == null)
                throw new System.ArgumentException(Resources.Strings.CantCopyToDifferentObjectType, nameof(destination));
            base.CopyTo(destination);
            dispositionHeader.parsed = parsed;
            dispositionHeader.disp = disp;
        }

        public override sealed bool IsValueValid(string value) {
            return value != null && ValueParser.ParseToken(value, 0, false) == value.Length;
        }

        internal override long WriteValue(System.IO.Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer) {
            if (disp.Length == 0)
                disp = "attachment";
            return base.WriteValue(stream, encodingOptions, filter, ref currentLineLength, ref scratchBuffer);
        }

        internal override void ParseValue(ValueParser parser, bool storeValue) {
            var phrase = new MimeStringList();
            parser.ParseCFWS(false, ref phrase, true);
            var mimeString = parser.ParseToken();
            if (!storeValue)
                return;
            if (mimeString.Length == 0)
                disp = string.Empty;
            else
                disp = Header.NormalizeString(mimeString.Data, mimeString.Offset, mimeString.Length, false);
        }

        internal override void AppendLine(MimeString line, bool markDirty) {
            if (parsed)
                this.Reset();
            base.AppendLine(line, markDirty);
        }

        private void Reset() {
            this.InternalRemoveAll();
            parsed = false;
            disp = null;
        }

        internal const bool AllowUTF8Value = false;
        private const string DefaultDisposition = "attachment";
        private string disp;

    }

}