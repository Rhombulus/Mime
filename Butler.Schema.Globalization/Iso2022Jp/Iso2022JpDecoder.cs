namespace Butler.Schema.Globalization.Iso2022Jp {

    internal class Iso2022JpDecoder : System.Text.Decoder {

        internal Iso2022JpDecoder(Iso2022JpEncoding parent) {
            killSwitch = Iso2022JpEncoding.InternalReadKillSwitch();
            this.parent = parent;
            switch (killSwitch) {
                case Iso2022DecodingMode.Default:
                    defaultDecoder = parent.DefaultEncoding.GetDecoder();
                    goto case 2;
                case Iso2022DecodingMode.Override:
                    subDecoders = DecodeToCp932.GetStandardOrder();
                    cp932Decoder = System.Text.Encoding.GetEncoding(932)
                                         .GetDecoder();
                    currentSubDecoder = -1;
                    lastChanceDecoderIndex = DecodeToCp932.LastChanceDecoderIndex;
                    leftovers = null;
                    escape = new Escape();
                    goto case 2;
                case Iso2022DecodingMode.Throw:
                    sbSummary = new System.Text.StringBuilder(32);
                    break;
                default:
                    throw new System.InvalidOperationException("Invalid Iso2022DecodingMode!");
            }
        }

        public override unsafe void Convert(byte* bytes, int byteCount, char* chars, int charCount, bool flush, out int bytesUsed, out int charsUsed, out bool completed) {
            throw new System.NotImplementedException();
        }

        public override void Convert(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, int charCount, bool flush, out int bytesUsed, out int charsUsed, out bool completed) {
            switch (killSwitch) {
                case Iso2022DecodingMode.Default:
                    defaultDecoder.Convert(bytes, byteIndex, byteCount, chars, charIndex, charCount, flush, out bytesUsed, out charsUsed, out completed);
                    break;
                case Iso2022DecodingMode.Override:
                    bytesUsed = 0;
                    charsUsed = 0;
                    completed = false;
                    var bytesUsed1 = 0;
                    var charsUsed1 = 0;
                    var completed1 = false;
                    if (leftovers != null) {
                        this.ConvertBuffer(leftovers, 0, leftovers.Length, chars, charIndex, charCount, flush, out bytesUsed1, out charsUsed1, out completed1);
                        if (!completed1)
                            throw new System.ArgumentException("Caller did not provide enough space for previously unprocessed data!", nameof(chars));
                        charIndex += charsUsed1;
                        charCount -= charsUsed1;
                        bytesUsed += bytesUsed1;
                        leftovers = null;
                    }
                    if (byteCount == 0) {
                        completed = true;
                        break;
                    }
                    this.ConvertBuffer(bytes, byteIndex, byteCount, chars, charIndex, charCount, flush, out bytesUsed, out charsUsed, out completed);
                    var length = byteCount - (bytesUsed - bytesUsed1);
                    var sourceIndex = byteIndex + (byteCount - length);
                    if (length <= 0 || bytes[sourceIndex] == 0)
                        break;
                    leftovers = new byte[length];
                    System.Array.Copy(bytes, sourceIndex, leftovers, 0, length);
                    break;
                case Iso2022DecodingMode.Throw:
                    throw new System.NotImplementedException();
                default:
                    throw new System.InvalidOperationException();
            }
        }

        private void ConvertBuffer(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, int charCount, bool flush, out int bytesUsed, out int charsUsed, out bool completed) {
            bytesUsed = 0;
            charsUsed = 0;
            completed = false;
            var usedOut = 0;
            while (byteCount > 0) {
                var bytesConsumed = 0;
                var usedIn = 0;
                var bytesUsed1 = 0;
                var charsUsed1 = 0;
                int cp932ByteCount;
                this.GetNextRun(bytes, byteIndex, byteCount, out bytesConsumed, out cp932ByteCount);
                if (bytesConsumed == 0 && bytes[byteIndex] == 0) {
                    if (charCount > 0) {
                        chars[charIndex] = ' ';
                        ++charsUsed;
                        ++charIndex;
                        --charCount;
                    }
                    ++bytesUsed;
                    ++byteIndex;
                    --byteCount;
                    if (byteCount == 0) {
                        completed = true;
                        break;
                    }
                } else {
                    var numArray = new byte[cp932ByteCount];
                    bool complete;
                    subDecoders[currentSubDecoder].ConvertToCp932(bytes, byteIndex, bytesConsumed, numArray, 0, cp932ByteCount, flush, escape, out usedIn, out usedOut, out complete);
                    bytesUsed += usedIn;
                    byteIndex += usedIn;
                    byteCount -= usedIn;
                    if (bytesConsumed != usedIn)
                        throw new System.InvalidOperationException(string.Format("{2}.GetNextRun input length {0} does not match ConvertToCp932 length {1}", bytesConsumed, usedIn, subDecoders[currentSubDecoder].GetType()));
                    if (usedOut != cp932ByteCount)
                        throw new System.InvalidOperationException(string.Format("{2}.GetNextRun output length {0} does not match ConvertToCp932 length {1}", cp932ByteCount, usedOut, subDecoders[currentSubDecoder].GetType()));
                    var flag = byteCount > 0 && bytes[byteIndex] == 0;
                    if (cp932ByteCount == 0) {
                        if (byteCount <= 0 || flag) {
                            completed = true;
                            break;
                        }
                    } else {
                        var count = cp932ByteCount;
                        var charCount1 = cp932Decoder.GetCharCount(numArray, 0, cp932ByteCount, flush);
                        while (charCount1 > charCount) {
                            --count;
                            charCount1 = cp932Decoder.GetCharCount(numArray, 0, count, flush);
                            complete = false;
                        }
                        cp932Decoder.Convert(numArray, 0, usedOut, chars, charIndex, charCount, flush, out bytesUsed1, out charsUsed1, out completed);
                        charsUsed += charsUsed1;
                        charIndex += charsUsed1;
                        charCount -= charsUsed1;
                        completed = flag || completed && complete;
                    }
                }
            }
        }

        private void GetNextRun(byte[] bytes, int byteIndex, int byteCount, out int bytesConsumed, out int cp932ByteCount) {
            bytesConsumed = 0;
            cp932ByteCount = 0;
            DecodeToCp932.MapEscape(bytes, byteIndex, byteCount, escape);
            if (escape.Sequence == EscapeSequence.Incomplete) {
                bytesConsumed += escape.BytesInCurrentBuffer;
                currentSubDecoder = lastChanceDecoderIndex;
                sbSummary.Append("|");
            } else {
                int bytesConsumed1;
                int bytesProduced;
                if (currentSubDecoder != -1 && !escape.IsValidEscapeSequence &&
                    (subDecoders[currentSubDecoder].GetRunLength(bytes, byteIndex, byteCount, escape, out bytesConsumed1, out bytesProduced) == ValidationResult.Valid && bytesConsumed1 > 0)) {
                    cp932ByteCount = bytesProduced;
                    bytesConsumed = bytesConsumed1;
                    sbSummary.AppendFormat("|{0}", bytesConsumed);
                } else {
                    ++runCount;
                    if (escape.IsValidEscapeSequence) {
                        for (var index = 0; index < subDecoders.Length; ++index) {
                            if (subDecoders[index].IsEscapeSequenceHandled(escape)) {
                                if (subDecoders[index].GetRunLength(bytes, byteIndex, byteCount, escape, out bytesConsumed1, out bytesProduced) != ValidationResult.Valid) {
                                    throw new System.InvalidOperationException(
                                        string.Format(
                                            "{0}.IsEscapeSequenceCovered and GetRunLength must agree on validity",
                                            subDecoders[index].GetType()
                                                              .Name));
                                }
                                cp932ByteCount = bytesProduced;
                                bytesConsumed = bytesConsumed1;
                                currentSubDecoder = index;
                                sbSummary.AppendFormat("E{0}{1}", escape.Abbreviation, bytesConsumed);
                                return;
                            }
                        }
                    } else {
                        for (var index = 0; index < subDecoders.Length; ++index) {
                            if (subDecoders[index].GetRunLength(bytes, byteIndex, byteCount, escape, out bytesConsumed1, out bytesProduced) == ValidationResult.Valid &&
                                (bytesConsumed1 > 0 || bytesConsumed1 < byteCount && bytes[byteIndex + bytesConsumed1] == 0)) {
                                currentSubDecoder = index;
                                cp932ByteCount = bytesProduced;
                                bytesConsumed = bytesConsumed1;
                                sbSummary.AppendFormat("{0}{1}", subDecoders[index].Abbreviation, bytesConsumed);
                                return;
                            }
                        }
                    }
                    throw new System.InvalidOperationException("What happened to the last-chance decoder?");
                }
            }
        }

        public override unsafe int GetCharCount(byte* bytes, int count, bool flush) {
            throw new System.NotImplementedException();
        }

        public override int GetCharCount(byte[] bytes, int index, int count) {
            return this.GetCharCount(bytes, index, count, false);
        }

        public override int GetCharCount(byte[] bytes, int index, int count, bool flush) {
            switch (killSwitch) {
                case Iso2022DecodingMode.Default:
                    return defaultDecoder.GetCharCount(bytes, index, count, flush);
                case Iso2022DecodingMode.Override:
                    var chars = new char[parent.GetMaxCharCount(count)];
                    int bytesUsed;
                    int charsUsed;
                    bool completed;
                    this.Convert(bytes, index, count, chars, 0, chars.Length, flush, out bytesUsed, out charsUsed, out completed);
                    this.Reset();
                    return charsUsed;
                case Iso2022DecodingMode.Throw:
                    throw new System.NotImplementedException();
                default:
                    throw new System.InvalidOperationException();
            }
        }

        public override unsafe int GetChars(byte* bytes, int byteCount, char* chars, int charCount, bool flush) {
            throw new System.NotImplementedException();
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex) {
            return this.GetChars(bytes, byteIndex, byteCount, chars, charIndex, false);
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex, bool flush) {
            switch (killSwitch) {
                case Iso2022DecodingMode.Default:
                    return defaultDecoder.GetChars(bytes, byteIndex, byteCount, chars, charIndex, flush);
                case Iso2022DecodingMode.Override:
                    int bytesUsed;
                    int charsUsed;
                    bool completed;
                    this.Convert(bytes, byteIndex, byteCount, chars, charIndex, chars.Length - charIndex, flush, out bytesUsed, out charsUsed, out completed);
                    if (!completed)
                        throw new System.ArgumentException("insufficient capacity", nameof(chars));
                    return charsUsed;
                case Iso2022DecodingMode.Throw:
                    throw new System.NotImplementedException();
                default:
                    throw new System.InvalidOperationException();
            }
        }

        public override void Reset() {
            switch (killSwitch) {
                case Iso2022DecodingMode.Default:
                    defaultDecoder.Reset();
                    break;
                case Iso2022DecodingMode.Override:
                    foreach (var decodeToCp932 in subDecoders)
                        decodeToCp932.Reset();
                    currentSubDecoder = -1;
                    leftovers = null;
                    escape.Reset();
                    runCount = 0;
                    sbSummary = new System.Text.StringBuilder(32);
                    break;
                case Iso2022DecodingMode.Throw:
                    throw new System.NotImplementedException();
                default:
                    throw new System.InvalidOperationException();
            }
        }

        public override string ToString() {
            return string.Format("{0} runs; {1}", runCount, sbSummary);
        }

        private const int SummarySize = 32;
        private readonly System.Text.Decoder cp932Decoder;
        private readonly System.Text.Decoder defaultDecoder;
        private readonly Escape escape;
        private readonly Iso2022DecodingMode killSwitch;
        private readonly int lastChanceDecoderIndex;
        private readonly Iso2022JpEncoding parent;
        private readonly DecodeToCp932[] subDecoders;
        private int currentSubDecoder;
        private byte[] leftovers;
        private int runCount;
        private System.Text.StringBuilder sbSummary;

    }

}