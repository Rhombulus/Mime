using System.Linq;

namespace Butler.Schema.Data.Mime {

    public class AddressHeader : Header, System.Collections.Generic.IEnumerable<AddressItem>, System.Collections.IEnumerable {

        public AddressHeader(string name)
            : this(name, Header.GetHeaderId(name, true)) {
            var type = Header.TypeFromHeaderId(this.HeaderId);
            if (this.HeaderId != HeaderId.Unknown && type != typeof (AddressHeader))
                throw new System.ArgumentException(Resources.Strings.NameNotValidForThisHeaderType(name, "AddressHeader", type.Name));
        }

        internal AddressHeader(string name, HeaderId headerId)
            : base(name, headerId) {}

        public override sealed string Value {
            get {
                return null;
            }
            set {
                throw new System.NotSupportedException(Resources.Strings.UnicodeMimeHeaderAddressNotSupported);
            }
        }

        public override sealed bool RequiresSMTPUTF8 {
            get {
                if (!parsed)
                    this.Parse();
                for (var mimeNode = this.FirstChild; mimeNode != null; mimeNode = mimeNode.NextSibling) {
                    var addressItem = mimeNode as AddressItem;
                    if (addressItem != null && addressItem.RequiresSMTPUTF8)
                        return true;
                }
                return false;
            }
        }

        internal override byte[] RawValue {
            get {
                return null;
            }
            set {
                base.RawValue = value;
            }
        }

        System.Collections.Generic.IEnumerator<AddressItem> System.Collections.Generic.IEnumerable<AddressItem>.GetEnumerator() {
            return new Enumerator<AddressItem>(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return new Enumerator<AddressItem>(this);
        }

        internal override void RawValueAboutToChange() {
            parsed = true;
            this.InternalRemoveAll();
            if (parser != null)
                parser.Reset();
            parsed = false;
        }

        public override bool TryGetValue(out string value) {
            value = null;
            return false;
        }

        public Enumerator<AddressItem> GetEnumerator() {
            return new Enumerator<AddressItem>(this);
        }

        public override sealed MimeNode Clone() {
            var addressHeader = new AddressHeader(this.Name, this.HeaderId);
            this.CopyTo(addressHeader);
            return addressHeader;
        }

        public override sealed void CopyTo(object destination) {
            if (destination == null)
                throw new System.ArgumentNullException(nameof(destination));
            if (destination == this)
                return;
            var addressHeader = destination as AddressHeader;
            if (addressHeader == null)
                throw new System.ArgumentException(Resources.Strings.CantCopyToDifferentObjectType);
            base.CopyTo(destination);
            addressHeader.parsed = parsed;
            addressHeader.parser = parser == null ? null : new MimeAddressParser(addressHeader.Lines, parser);
        }

        public static AddressHeader Parse(string name, string value, AddressParserFlags flags) {
            var addressHeader = new AddressHeader(name);
            if (!string.IsNullOrEmpty(value)) {
                var data = Internal.ByteString.StringToBytes(value, true);
                addressHeader.parser = new MimeAddressParser();
                addressHeader.parser.Initialize(
                    new MimeStringList(data, 0, data.Length),
                    AddressParserFlags.None != (flags & AddressParserFlags.IgnoreComments),
                    AddressParserFlags.None != (flags & AddressParserFlags.AllowSquareBrackets),
                    true);
                addressHeader.staticParsing = true;
                addressHeader.Parse();
            }
            return addressHeader;
        }

        public override sealed bool IsValueValid(string value) {
            return false;
        }

        internal override long WriteTo(System.IO.Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer) {
            var nameLength = this.WriteName(stream, ref scratchBuffer);
            currentLineLength.IncrementBy((int) nameLength);
            if (!this.IsDirty && this.RawLength != 0) {
                if (this.IsProtected) {
                    var num = nameLength + Header.WriteLines(this.Lines, stream);
                    currentLineLength.SetAs(0);
                    return num;
                }
                if (this.InternalLastChild == null) {
                    var merge = false;
                    if (!this.IsHeaderLineTooLong(nameLength, out merge)) {
                        var num = nameLength + Header.WriteLines(this.Lines, stream);
                        currentLineLength.SetAs(0);
                        return num;
                    }
                }
            }
            if (!parsed)
                this.Parse();
            var mimeNode = this.FirstChild;
            var num1 = 0;
            for (; mimeNode != null; mimeNode = mimeNode.NextSibling) {
                if (1 < ++num1) {
                    stream.Write(MimeString.Comma, 0, MimeString.Comma.Length);
                    nameLength += MimeString.Comma.Length;
                    currentLineLength.IncrementBy(MimeString.Comma.Length);
                }
                nameLength += mimeNode.WriteTo(stream, encodingOptions, filter, ref currentLineLength, ref scratchBuffer);
            }
            return nameLength + Header.WriteLineEnd(stream, ref currentLineLength);
        }

        internal override void RemoveAllUnparsed() {
            parsed = true;
        }

        internal override MimeNode ParseNextChild() {
            if (parsed)
                return null;
            var internalLastChild = this.InternalLastChild;
            MimeNode mimeNode;
            if (internalLastChild is MimeGroup) {
                do
                    ; while (internalLastChild.ParseNextChild() != null);
                mimeNode = internalLastChild.InternalNextSibling;
            } else
                mimeNode = this.ParseNextMailBox(false);
            parsed = mimeNode == null;
            return mimeNode;
        }

        internal override void CheckChildrenLimit(int countLimit, int bytesLimit) {
            if (parser == null)
                parser = new MimeAddressParser();
            if (!parser.Initialized) {
                parser.Initialize(
                    this.Lines,
                    false,
                    false,
                    this.GetHeaderDecodingOptions()
                        .AllowUTF8);
            }
            int actual;
            for (actual = 0; actual <= countLimit; ++actual) {
                var displayName = new MimeStringList();
                var address = new MimeStringList();
                if (AddressParserResult.End == parser.ParseNextMailbox(ref displayName, ref address)) {
                    parser.Reset();
                    return;
                }
                if (displayName.Length > bytesLimit)
                    throw new MimeException(Resources.Strings.TooManyTextValueBytes(displayName.Length, bytesLimit));
            }
            throw new MimeException(Resources.Strings.TooManyAddressItems(actual, countLimit));
        }

        internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild) {
            if (!(newChild is MimeRecipient) && !(newChild is MimeGroup))
                throw new System.ArgumentException(Resources.Strings.NewChildNotRecipientOrGroup, nameof(newChild));
            return refChild;
        }

        internal override void AppendLine(MimeString line, bool markDirty) {
            base.AppendLine(line, markDirty);
            parsed = false;
        }

        internal MimeNode ParseNextMailBox(bool fromGroup) {
            if (parsed)
                return null;
            var headerDecodingOptions = this.GetHeaderDecodingOptions();
            if (parser == null)
                parser = new MimeAddressParser();
            if (!parser.Initialized)
                parser.Initialize(this.Lines, false, false, headerDecodingOptions.AllowUTF8);
            var displayName = new MimeStringList();
            var address = new MimeStringList();
            var addressParserResult = parser.ParseNextMailbox(ref displayName, ref address);
            switch (addressParserResult) {
                case AddressParserResult.Mailbox:
                case AddressParserResult.GroupInProgress:
                    var mimeRecipient = new MimeRecipient(ref address, ref displayName);
                    if (staticParsing)
                        MimeRecipient.ConvertDisplayNameBack(mimeRecipient, displayName, headerDecodingOptions.AllowUTF8);
                    if (addressParserResult == AddressParserResult.GroupInProgress) {
                        var mimeGroup = this.InternalLastChild as MimeGroup;
                        mimeGroup.InternalInsertAfter(mimeRecipient, mimeGroup.InternalLastChild);
                        return mimeRecipient;
                    }
                    this.InternalInsertAfter(mimeRecipient, this.InternalLastChild);
                    if (!fromGroup)
                        return mimeRecipient;
                    return null;
                case AddressParserResult.GroupStart:
                    var mimeGroup1 = new MimeGroup(ref displayName);
                    if (staticParsing)
                        MimeRecipient.ConvertDisplayNameBack(mimeGroup1, displayName, headerDecodingOptions.AllowUTF8);
                    this.InternalInsertAfter(mimeGroup1, this.InternalLastChild);
                    return mimeGroup1;
                case AddressParserResult.End:
                    return null;
                default:
                    return null;
            }
        }

        private void Parse() {
            while (!parsed)
                this.ParseNextChild();
            if (!staticParsing)
                return;
            staticParsing = false;
        }

        internal override void ForceParse() {
            this.Parse();
        }

        internal const bool AllowUTF8Value = true;
        private bool parsed;
        private MimeAddressParser parser;
        private bool staticParsing;

    }

}