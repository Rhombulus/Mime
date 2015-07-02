using System.Linq;

namespace Butler.Schema.Mime {

    public class AsciiTextHeader : Header {

        public AsciiTextHeader(string name, string value)
            : this(name, Header.GetHeaderId(name, true)) {
            this.Value = value;
        }

        internal AsciiTextHeader(string name, HeaderId headerId)
            : base(name, headerId) {}

        public override sealed string Value {
            get {
                return this.GetRawValue(true);
            }
            set {
                this.SetRawValue(value, true, true);
            }
        }

        public override sealed bool RequiresSMTPUTF8 => !MimeString.IsPureASCII(this.Lines);

        public override sealed MimeNode Clone() {
            var asciiTextHeader = new AsciiTextHeader(this.Name, this.HeaderId);
            this.CopyTo(asciiTextHeader);
            return asciiTextHeader;
        }

        public override sealed void CopyTo(object destination) {
            if (destination == null)
                throw new System.ArgumentNullException(nameof(destination));
            if (destination == this)
                return;
            if (!(destination is AsciiTextHeader))
                base.CopyTo(destination);
            else
                base.CopyTo(destination);
        }

        public override sealed bool IsValueValid(string value) {
            return true;
        }

        internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild) {
            throw new System.NotSupportedException(Resources.Strings.AddingChildrenToAsciiTextHeader);
        }

        internal const bool AllowUTF8Value = true;

    }

}