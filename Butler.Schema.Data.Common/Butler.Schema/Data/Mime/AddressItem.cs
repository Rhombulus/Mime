using System;
using System.Linq;

namespace Butler.Schema.Data.Mime {

    public abstract class AddressItem : MimeNode {

        internal AddressItem() {}

        internal AddressItem(string displayName) {
            decodedDisplayName = displayName;
        }

        internal AddressItem(ref MimeStringList displayName) {
            displayNameFragments.TakeOver(ref displayName);
        }

        public string DisplayName {
            get {
                DecodingResults decodingResults;
                if (decodedDisplayName == null && !this.TryGetDisplayName(this.GetHeaderDecodingOptions(), out decodingResults, out decodedDisplayName))
                    MimeCommon.ThrowDecodingFailedException(ref decodingResults);
                return decodedDisplayName;
            }
            set {
                displayNameFragments.Reset();
                decodedDisplayName = value;
                this.SetDirty();
            }
        }

        public virtual bool RequiresSMTPUTF8 => false;

        internal string DecodedDisplayName {
            set {
                decodedDisplayName = value;
            }
        }

        public override void CopyTo(object destination) {
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (destination == this)
                return;
            var addressItem = destination as AddressItem;
            if (addressItem == null)
                throw new ArgumentException(CtsResources.Strings.CantCopyToDifferentObjectType);
            base.CopyTo(destination);
            addressItem.displayNameFragments = displayNameFragments.Clone();
            addressItem.decodedDisplayName = decodedDisplayName;
        }

        public bool TryGetDisplayName(out string displayName) {
            DecodingResults decodingResults;
            return this.TryGetDisplayName(this.GetHeaderDecodingOptions(), out decodingResults, out displayName);
        }

        public bool TryGetDisplayName(DecodingOptions decodingOptions, out DecodingResults decodingResults, out string displayName) {
            if (displayNameFragments.Count == 0) {
                displayName = decodedDisplayName;
                decodingResults = new DecodingResults();
                return true;
            }
            if (decodingOptions.Charset == null)
                decodingOptions.Charset = this.GetDefaultHeaderDecodingCharset(null, null);
            return MimeCommon.TryDecodeValue(displayNameFragments, 4026531839U, decodingOptions, out decodingResults, out displayName);
        }

        private bool IsQuotingRequired(string displayName, bool allowUTF8) {
            return this.IsQuotingRequired(new MimeString(displayName), allowUTF8);
        }

        private bool IsQuotingRequired(MimeString mimeStr, bool allowUTF8) {
            var writeState = WriteState.Begin;
            var mimeString = new MimeString(WordBreakBytes, 0, WordBreakBytes.Length);
            int offset;
            int count;
            var data = mimeStr.GetData(out offset, out count);
            while (count != 0) {
                switch (writeState) {
                    case WriteState.Begin:
                        var characterCount = 0;
                        var endOf = MimeScan.FindEndOf(MimeScan.Token.Atom, data, offset, count, out characterCount, allowUTF8);
                        if (endOf == 0) {
                            if (count <= 3 || offset != 0 || !mimeString.HasPrefixEq(data, 0, 3))
                                return true;
                            offset += 3;
                            count -= 3;
                            writeState = WriteState.Begin;
                            continue;
                        }
                        offset += endOf;
                        count -= endOf;
                        writeState = WriteState.Atom;
                        continue;
                    case WriteState.Atom:
                        if ((count < 2 || data[offset] != 32) && (count < 1 || data[offset] != 46))
                            return true;
                        ++offset;
                        --count;
                        writeState = WriteState.Begin;
                        continue;
                    default:
                        continue;
                }
            }
            return false;
        }

        internal bool IsQuotingRequired(MimeStringList displayNameFragments, bool allowUTF8) {
            for (var index = 0; index != displayNameFragments.Count; ++index) {
                var mimeStr = displayNameFragments[index];
                if (((int) mimeStr.Mask & -268435457) != 0 && this.IsQuotingRequired(mimeStr, allowUTF8))
                    return true;
            }
            return false;
        }

        internal string QuoteString(string inputString) {
            var stringBuilder = new System.Text.StringBuilder(inputString.Length + 2);
            stringBuilder.Append("\"");
            for (var index = 0; index < inputString.Length; ++index) {
                var ch = inputString[index];
                if (ch < 128 && MimeScan.IsEscapingRequired((byte) ch))
                    stringBuilder.Append("\\");
                stringBuilder.Append(ch);
            }
            stringBuilder.Append("\"");
            return stringBuilder.ToString();
        }

        internal void ResetDisplayNameFragments() {
            displayNameFragments.Reset();
        }

        internal MimeStringList GetDisplayNameToWrite(EncodingOptions encodingOptions) {
            var mimeStringList = displayNameFragments;
            if (mimeStringList.GetLength(4026531839U) == 0 && decodedDisplayName != null && decodedDisplayName.Length != 0) {
                mimeStringList =
                    MimeCommon.EncodeValue(
                        (encodingOptions.EncodingFlags & EncodingFlags.QuoteDisplayNameBeforeRfc2047Encoding) == EncodingFlags.None || !this.IsQuotingRequired(decodedDisplayName, encodingOptions.AllowUTF8) ||
                        !MimeCommon.IsEncodingRequired(decodedDisplayName, encodingOptions.AllowUTF8)
                            ? decodedDisplayName
                            : this.QuoteString(decodedDisplayName),
                        encodingOptions,
                        ValueEncodingStyle.Phrase);
                displayNameFragments = mimeStringList;
            } else if ((EncodingFlags.ForceReencode & encodingOptions.EncodingFlags) != EncodingFlags.None)
                mimeStringList = MimeCommon.EncodeValue(this.DisplayName, encodingOptions, ValueEncodingStyle.Phrase);
            return mimeStringList;
        }

        internal static readonly byte[] WordBreakBytes = Internal.ByteString.StringToBytes(" =?", true);
        private string decodedDisplayName;
        private MimeStringList displayNameFragments;


        private enum WriteState {

            Begin,
            Atom

        }

    }

}