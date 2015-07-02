using System.Linq;

namespace Butler.Schema.Data.Mime {

    public class MimeRecipient : AddressItem {

        public MimeRecipient() {}

        public MimeRecipient(string displayName, string email)
            : base(displayName) {
            if (email == null)
                throw new System.ArgumentNullException(nameof(email));
            emailAddressFragments.Append(new MimeString(email));
        }

        internal MimeRecipient(ref MimeStringList address, ref MimeStringList displayName)
            : base(ref displayName) {
            emailAddressFragments.TakeOverAppend(ref address);
        }

        public string Email {
            get {
                var sz = emailAddressFragments.GetSz();
                if (sz != null)
                    return Internal.ByteString.BytesToString(sz, true);
                return string.Empty;
            }
            set {
                if (value == null)
                    throw new System.ArgumentNullException(nameof(value));
                if (!MimeAddressParser.IsWellFormedAddress(value, true))
                    throw new System.ArgumentException("Address string must be well-formed", nameof(value));
                emailAddressFragments.Reset();
                emailAddressFragments.Append(new MimeString(value));
                this.SetDirty();
            }
        }

        public override sealed bool RequiresSMTPUTF8 => !MimeString.IsPureASCII(emailAddressFragments);

        public static MimeRecipient Parse(string address, AddressParserFlags flags) {
            var mimeRecipient = new MimeRecipient();
            if (!string.IsNullOrEmpty(address)) {
                var data = Internal.ByteString.StringToBytes(address, true);
                var mimeAddressParser = new MimeAddressParser();
                mimeAddressParser.Initialize(
                    new MimeStringList(data, 0, data.Length),
                    AddressParserFlags.None != (flags & AddressParserFlags.IgnoreComments),
                    AddressParserFlags.None != (flags & AddressParserFlags.AllowSquareBrackets),
                    true);
                var displayName = new MimeStringList();
                var num = (int) mimeAddressParser.ParseNextMailbox(ref displayName, ref mimeRecipient.emailAddressFragments);
                MimeRecipient.ConvertDisplayNameBack(mimeRecipient, displayName, true);
            }
            return mimeRecipient;
        }

        public static bool IsEmailValid(string email, bool allowUTF8 = false) {
            return MimeAddressParser.IsWellFormedAddress(email, allowUTF8);
        }

        public override sealed MimeNode Clone() {
            var mimeRecipient = new MimeRecipient();
            this.CopyTo(mimeRecipient);
            return mimeRecipient;
        }

        public override sealed void CopyTo(object destination) {
            if (destination == null)
                throw new System.ArgumentNullException(nameof(destination));
            if (destination == this)
                return;
            var mimeRecipient = destination as MimeRecipient;
            if (mimeRecipient == null)
                throw new System.ArgumentException(Resources.Strings.CantCopyToDifferentObjectType);
            base.CopyTo(destination);
            mimeRecipient.emailAddressFragments = emailAddressFragments.Clone();
        }

        internal static void ConvertDisplayNameBack(AddressItem addressItem, MimeStringList displayNameFragments, bool allowUTF8) {
            var sz = displayNameFragments.GetSz(4026531839U);
            if (sz == null)
                addressItem.DecodedDisplayName = null;
            else {
                var str = Internal.ByteString.BytesToString(sz, allowUTF8);
                addressItem.DecodedDisplayName = str;
            }
        }

        internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild) {
            throw new System.NotSupportedException(Resources.Strings.RecipientsCannotHaveChildNodes);
        }

        internal override long WriteTo(System.IO.Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer) {
            var displayNameToWrite = this.GetDisplayNameToWrite(encodingOptions);
            var num1 = 0L;
            var num2 = 0;
            if (this.NextSibling != null)
                ++num2;
            else if (this.Parent is MimeGroup) {
                ++num2;
                if (this.Parent.NextSibling != null)
                    ++num2;
            }
            var sz = emailAddressFragments.GetSz();
            var countInChars = Internal.ByteString.BytesToCharCount(sz, encodingOptions.AllowUTF8);
            if (displayNameToWrite.GetLength(4026531839U) != 0) {
                num1 += Header.QuoteAndFold(
                    stream,
                    displayNameToWrite,
                    4026531839U,
                    this.IsQuotingRequired(displayNameToWrite, encodingOptions.AllowUTF8),
                    true,
                    encodingOptions.AllowUTF8,
                    countInChars == 0 ? num2 : 0,
                    ref currentLineLength,
                    ref scratchBuffer);
            }
            if (countInChars != 0) {
                var num3 = 1 < currentLineLength.InChars ? 1 : 0;
                if (1 < currentLineLength.InChars) {
                    if (currentLineLength.InChars + countInChars + 2 + num2 + num3 > 78) {
                        var num4 = num1 + Header.WriteLineEnd(stream, ref currentLineLength);
                        stream.Write(Header.LineStartWhitespace, 0, Header.LineStartWhitespace.Length);
                        num1 = num4 + Header.LineStartWhitespace.Length;
                        currentLineLength.IncrementBy(Header.LineStartWhitespace.Length);
                    } else {
                        stream.Write(MimeString.Space, 0, MimeString.Space.Length);
                        num1 += MimeString.Space.Length;
                        currentLineLength.IncrementBy(MimeString.Space.Length);
                    }
                }
                stream.Write(MimeString.LessThan, 0, MimeString.LessThan.Length);
                var num5 = num1 + MimeString.LessThan.Length;
                currentLineLength.IncrementBy(MimeString.LessThan.Length);
                stream.Write(sz, 0, sz.Length);
                var num6 = num5 + sz.Length;
                currentLineLength.IncrementBy(countInChars, sz.Length);
                stream.Write(MimeString.GreaterThan, 0, MimeString.GreaterThan.Length);
                num1 = num6 + MimeString.GreaterThan.Length;
                currentLineLength.IncrementBy(MimeString.GreaterThan.Length);
            }
            return num1;
        }

        private MimeStringList emailAddressFragments;

    }

}