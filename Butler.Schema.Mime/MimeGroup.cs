using System.Linq;

namespace Butler.Schema.Mime {

    public class MimeGroup : AddressItem, System.Collections.Generic.IEnumerable<MimeRecipient>, System.Collections.IEnumerable {

        public MimeGroup() {}

        public MimeGroup(string displayName)
            : base(displayName) {}

        internal MimeGroup(ref MimeStringList displayName)
            : base(ref displayName) {}

        System.Collections.Generic.IEnumerator<MimeRecipient> System.Collections.Generic.IEnumerable<MimeRecipient>.GetEnumerator() {
            return new Enumerator<MimeRecipient>(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return new Enumerator<MimeRecipient>(this);
        }

        public Enumerator<MimeRecipient> GetEnumerator() {
            return new Enumerator<MimeRecipient>(this);
        }

        public override sealed MimeNode Clone() {
            var mimeGroup = new MimeGroup();
            this.CopyTo(mimeGroup);
            return mimeGroup;
        }

        public override sealed void CopyTo(object destination) {
            if (destination == null)
                throw new System.ArgumentNullException(nameof(destination));
            if (destination == this)
                return;
            var mimeGroup = destination as MimeGroup;
            if (mimeGroup == null)
                throw new System.ArgumentException(Resources.Strings.CantCopyToDifferentObjectType);
            do
                ; while (mimeGroup.ParseNextChild() != null);
            do
                ; while (this.ParseNextChild() != null);
            base.CopyTo(destination);
        }

        internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild) {
            if (!(newChild is MimeRecipient))
                throw new System.ArgumentException(Resources.Strings.NewChildIsNotARecipient);
            return refChild;
        }

        internal override long WriteTo(System.IO.Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer) {
            var nextSibling = this.NextSibling;
            var displayNameToWrite = this.GetDisplayNameToWrite(encodingOptions);
            var num1 = 0L;
            if (displayNameToWrite.GetLength(4026531839U) != 0) {
                var lastLineReserve = 1;
                if (this.FirstChild == null)
                    ++lastLineReserve;
                if (this.NextSibling != null)
                    ++lastLineReserve;
                var num2 = num1 +
                           Header.QuoteAndFold(
                               stream,
                               displayNameToWrite,
                               4026531839U,
                               this.IsQuotingRequired(displayNameToWrite, encodingOptions.AllowUTF8),
                               true,
                               encodingOptions.AllowUTF8,
                               lastLineReserve,
                               ref currentLineLength,
                               ref scratchBuffer);
                stream.Write(MimeString.Colon, 0, MimeString.Colon.Length);
                num1 = num2 + MimeString.Colon.Length;
                currentLineLength.IncrementBy(MimeString.Colon.Length);
            }
            var mimeNode = this.FirstChild;
            var num3 = 0;
            for (; mimeNode != null; mimeNode = mimeNode.NextSibling) {
                if (1 < ++num3) {
                    stream.Write(MimeString.Comma, 0, MimeString.Comma.Length);
                    num1 += MimeString.Comma.Length;
                    currentLineLength.IncrementBy(MimeString.Comma.Length);
                }
                num1 += mimeNode.WriteTo(stream, encodingOptions, filter, ref currentLineLength, ref scratchBuffer);
            }
            stream.Write(MimeString.Semicolon, 0, MimeString.Semicolon.Length);
            var num4 = num1 + MimeString.Semicolon.Length;
            currentLineLength.IncrementBy(MimeString.Semicolon.Length);
            return num4;
        }

        internal override MimeNode ParseNextChild() {
            MimeNode mimeNode = null;
            if (!parsed && this.Parent != null) {
                var addressHeader = this.Parent as AddressHeader;
                if (addressHeader != null)
                    mimeNode = addressHeader.ParseNextMailBox(true);
            }
            parsed = mimeNode == null;
            return mimeNode;
        }

        private bool parsed;

    }

}