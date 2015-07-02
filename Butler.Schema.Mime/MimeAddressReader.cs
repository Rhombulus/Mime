using System;
using System.Linq;

namespace Butler.Schema.Data.Mime {

    public struct MimeAddressReader {

        internal MimeAddressReader(MimeReader reader, bool topLevel) {
            this.reader = reader;
            this.topLevel = topLevel;
        }

        public bool IsGroup {
            get {
                this.AssertGood(true);
                if (topLevel)
                    return reader.GroupInProgress;
                return false;
            }
        }

        public MimeAddressReader GroupRecipientReader {
            get {
                if (!this.IsGroup)
                    throw new InvalidOperationException(Resources.Strings.AddressReaderIsNotPositionedOnAGroup);
                return new MimeAddressReader(reader, false);
            }
        }

        public string DisplayName {
            get {
                DecodingResults decodingResults;
                string displayName;
                if (!this.TryGetDisplayName(reader.HeaderDecodingOptions, out decodingResults, out displayName))
                    MimeCommon.ThrowDecodingFailedException(ref decodingResults);
                return displayName;
            }
        }

        public string Email {
            get {
                this.AssertGood(true);
                return reader.ReadRecipientEmail(topLevel);
            }
        }

        public bool ReadNextAddress() {
            this.AssertGood(false);
            return reader.ReadNextDescendant(topLevel);
        }

        public bool TryGetDisplayName(out string displayName) {
            DecodingResults decodingResults;
            return this.TryGetDisplayName(reader.HeaderDecodingOptions, out decodingResults, out displayName);
        }

        public bool TryGetDisplayName(DecodingOptions decodingOptions, out DecodingResults decodingResults, out string displayName) {
            this.AssertGood(true);
            return reader.TryReadDisplayName(topLevel, decodingOptions, out decodingResults, out displayName);
        }

        private void AssertGood(bool checkPositionedOnAddress) {
            if (reader == null)
                throw new NotSupportedException(Resources.Strings.AddressReaderNotInitialized);
            reader.AssertGoodToUse(true, true);
            if (reader.ReaderState != MimeReaderState.HeaderComplete || reader.CurrentHeaderObject == null || !(reader.CurrentHeaderObject is AddressHeader))
                throw new NotSupportedException(Resources.Strings.ReaderIsNotPositionedOnAddressHeader);
            if (!topLevel && !reader.GroupInProgress)
                throw new InvalidOperationException(Resources.Strings.AddressReaderIsNotPositionedOnAddress);
            if (checkPositionedOnAddress && !reader.IsCurrentChildValid(topLevel))
                throw new InvalidOperationException(Resources.Strings.AddressReaderIsNotPositionedOnAddress);
        }

        private readonly MimeReader reader;
        private readonly bool topLevel;

    }

}