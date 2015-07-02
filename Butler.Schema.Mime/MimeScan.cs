using System.Linq;

namespace Butler.Schema.Mime {

    internal static class MimeScan {

        public static bool IsLWSP(byte ch) {
            return 0 != (Dictionary[ch] & Token.Lwsp);
        }

        public static bool IsFWSP(byte ch) {
            return 0 != (Dictionary[ch] & Token.Fwsp);
        }

        public static bool IsCTRL(byte ch) {
            return 0 != (Dictionary[ch] & Token.Ctl);
        }

        public static bool IsAtom(byte ch) {
            return 0 != (Dictionary[ch] & Token.Atom);
        }

        public static bool IsToken(byte ch) {
            return 0 != (Dictionary[ch] & Token.Token);
        }

        public static bool IsAlpha(byte ch) {
            return 0 != (Dictionary[ch] & Token.Alpha);
        }

        public static bool IsDigit(byte ch) {
            return 0 != (Dictionary[ch] & Token.Digit);
        }

        public static bool IsAlphaOrDigit(byte ch) {
            return 0 != (Dictionary[ch] & (Token.Digit | Token.Alpha));
        }

        public static bool IsHex(byte ch) {
            return 0 != (Dictionary[ch] & Token.Hex);
        }

        public static bool IsBChar(byte ch) {
            return 0 != (Dictionary[ch] & Token.BChar);
        }

        public static bool IsField(byte ch) {
            return 0 != (Dictionary[ch] & Token.Field);
        }

        public static bool IsUTF8NonASCII(byte[] bytes, int startOffset, int endOffset, out int bytesUsed) {
            if (endOffset == -1)
                endOffset = bytes.Length;
            return Internal.ByteString.IsUTF8NonASCII(
                startOffset < endOffset ? bytes[startOffset] : (byte) 0,
                startOffset + 1 < endOffset ? bytes[startOffset + 1] : (byte) 0,
                startOffset + 2 < endOffset ? bytes[startOffset + 2] : (byte) 0,
                startOffset + 3 < endOffset ? bytes[startOffset + 3] : (byte) 0,
                out bytesUsed);
        }

        public static bool IsEncodingRequired(byte ch) {
            return 0 == (Dictionary[ch] & (Token.Spec | Token.Atom | Token.Fwsp));
        }

        public static bool IsEscapingRequired(byte ch) {
            if (ch != 34 && ch != 92)
                return ch == 13;
            return true;
        }

        public static bool IsSegmentEncodingRequired(byte ch) {
            if ((Dictionary[ch] & (Token.Ctl | Token.TSpec | Token.Lwsp)) == 0 && ch != 39 && ch != 42)
                return ch == 37;
            return true;
        }

        public static int FindEndOf(Token token, byte[] bytes, int start, int length, out int characterCount, bool allowUTF8) {
            var startOffset = start - 1;
            var endOffset = start + length;
            characterCount = 0;
            while (++startOffset < endOffset) {
                if ((Dictionary[bytes[startOffset]] & token) != 0)
                    ++characterCount;
                else if (allowUTF8 && bytes[startOffset] >= 128) {
                    var bytesUsed = 0;
                    if (MimeScan.IsUTF8NonASCII(bytes, startOffset, endOffset, out bytesUsed) && ((Token.Token | Token.Atom) & token) != 0) {
                        startOffset += bytesUsed - 1;
                        ++characterCount;
                    } else
                        break;
                } else
                    break;
            }
            return startOffset - start;
        }

        public static int FindEndOf(Token token, string value, int currentOffset, bool allowUTF8) {
            var index = currentOffset - 1;
            do
                ; while (++index < value.Length && (value[index] < 128 && (Dictionary[value[index]] & token) != 0 || allowUTF8 && value[index] >= 128 && ((Token.Token | Token.Atom) & token) != 0));
            return index - currentOffset;
        }

        public static int FindNextOf(Token token, byte[] bytes, int start, int length, out int characterCount, bool allowUTF8) {
            var startOffset = start - 1;
            var endOffset = start + length;
            characterCount = 0;
            while (++startOffset < endOffset && (Dictionary[bytes[startOffset]] & token) == 0) {
                if (allowUTF8 && bytes[startOffset] >= 128) {
                    var bytesUsed = 0;
                    if (MimeScan.IsUTF8NonASCII(bytes, startOffset, endOffset, out bytesUsed)) {
                        if (((Token.Token | Token.Atom) & token) == 0) {
                            startOffset += bytesUsed - 1;
                            ++characterCount;
                            continue;
                        }
                        break;
                    }
                }
                ++characterCount;
            }
            return startOffset - start;
        }

        public static int SkipLwsp(byte[] bytes, int offset, int length) {
            var characterCount = 0;
            return MimeScan.FindEndOf(Token.Lwsp, bytes, offset, length, out characterCount, false);
        }

        public static int SkipToLwspOrEquals(byte[] bytes, int start, int length) {
            var index = start - 1;
            var num1 = start + length;
            while (++index < num1) {
                var num2 = bytes[index];
                var token = Dictionary[num2];
                if ((token & (Token.TSpec | Token.Lwsp)) != 0 && ((token & Token.Lwsp) != 0 || num2 == 61))
                    break;
            }
            return index - start;
        }

        public static int ScanComment(byte[] bytes, int start, int length, bool handleISO2022, ref int level, ref bool quotedPair) {
            var index = start - 1;
            var num1 = start + length;
            while (++index < num1) {
                var num2 = bytes[index];
                if (quotedPair)
                    quotedPair = false;
                else if (92 == num2)
                    quotedPair = true;
                else if (40 == num2)
                    ++level;
                else if (41 == num2) {
                    --level;
                    if (level == 0) {
                        ++index;
                        break;
                    }
                } else if (handleISO2022 && (num2 == 14 || num2 == 27))
                    break;
            }
            return index - start;
        }

        public static int ScanQuotedString(byte[] bytes, int start, int length, bool handleISO2022, ref bool quotedPair) {
            var index = start - 1;
            var num1 = start + length;
            while (++index < num1) {
                var num2 = bytes[index];
                if (quotedPair)
                    quotedPair = false;
                else if (92 == num2 || 34 == num2 || handleISO2022 && (num2 == 14 || num2 == 27))
                    break;
            }
            return index - start;
        }

        public static int ScanJISString(byte[] bytes, int start, int length, ref bool done) {
            var index = start - 1;
            var num = start + length;
            while (++index < num) {
                if (bytes[index] < 33) {
                    done = true;
                    break;
                }
            }
            return index - start;
        }

        private static readonly Token[] Dictionary = new Token[256] {
            Token.Ctl,
            Token.Ctl,
            Token.Ctl,
            Token.Ctl,
            Token.Ctl,
            Token.Ctl,
            Token.Ctl,
            Token.Ctl,
            Token.Ctl,
            Token.Ctl | Token.Lwsp | Token.Fwsp,
            Token.Ctl | Token.Lwsp,
            Token.Ctl,
            Token.Ctl,
            Token.Ctl | Token.Lwsp,
            Token.Ctl,
            Token.Ctl,
            Token.Ctl,
            Token.Ctl,
            Token.Ctl,
            Token.Ctl,
            Token.Ctl,
            Token.Ctl,
            Token.Ctl,
            Token.Ctl,
            Token.Ctl,
            Token.Ctl,
            Token.Ctl,
            Token.Ctl,
            Token.Ctl,
            Token.Ctl,
            Token.Ctl,
            Token.Ctl,
            Token.Lwsp | Token.BChar | Token.Fwsp,
            Token.Token | Token.Atom | Token.Field,
            Token.Spec | Token.TSpec | Token.Field,
            Token.Token | Token.Atom | Token.Field,
            Token.Token | Token.Atom | Token.Field,
            Token.Token | Token.Atom | Token.Field,
            Token.Token | Token.Atom | Token.Field,
            Token.Token | Token.Atom | Token.BChar | Token.Field,
            Token.Spec | Token.TSpec | Token.BChar | Token.Field,
            Token.Spec | Token.TSpec | Token.BChar | Token.Field,
            Token.Token | Token.Atom | Token.Field,
            Token.Token | Token.Atom | Token.BChar | Token.Field,
            Token.Spec | Token.TSpec | Token.BChar | Token.Field,
            Token.Token | Token.Atom | Token.BChar | Token.Field,
            Token.Spec | Token.Token | Token.BChar | Token.Field,
            Token.TSpec | Token.Atom | Token.BChar | Token.Field,
            Token.Token | Token.Atom | Token.Digit | Token.Hex | Token.BChar | Token.Field,
            Token.Token | Token.Atom | Token.Digit | Token.Hex | Token.BChar | Token.Field,
            Token.Token | Token.Atom | Token.Digit | Token.Hex | Token.BChar | Token.Field,
            Token.Token | Token.Atom | Token.Digit | Token.Hex | Token.BChar | Token.Field,
            Token.Token | Token.Atom | Token.Digit | Token.Hex | Token.BChar | Token.Field,
            Token.Token | Token.Atom | Token.Digit | Token.Hex | Token.BChar | Token.Field,
            Token.Token | Token.Atom | Token.Digit | Token.Hex | Token.BChar | Token.Field,
            Token.Token | Token.Atom | Token.Digit | Token.Hex | Token.BChar | Token.Field,
            Token.Token | Token.Atom | Token.Digit | Token.Hex | Token.BChar | Token.Field,
            Token.Token | Token.Atom | Token.Digit | Token.Hex | Token.BChar | Token.Field,
            Token.Spec | Token.TSpec | Token.BChar,
            Token.Spec | Token.TSpec | Token.Field,
            Token.Spec | Token.TSpec | Token.Field,
            Token.TSpec | Token.Atom | Token.BChar | Token.Field,
            Token.Spec | Token.TSpec | Token.Field,
            Token.TSpec | Token.Atom | Token.BChar | Token.Field,
            Token.Spec | Token.TSpec | Token.Field,
            Token.Token | Token.Atom | Token.Hex | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.Hex | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.Hex | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.Hex | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.Hex | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.Hex | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Spec | Token.TSpec | Token.Field,
            Token.Spec | Token.TSpec | Token.Field,
            Token.Spec | Token.TSpec | Token.Field,
            Token.Token | Token.Atom | Token.Field,
            Token.Token | Token.Atom | Token.BChar | Token.Field,
            Token.Token | Token.Atom | Token.Field,
            Token.Token | Token.Atom | Token.Hex | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.Hex | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.Hex | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.Hex | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.Hex | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.Hex | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.BChar | Token.Field | Token.Alpha,
            Token.Token | Token.Atom | Token.Field,
            Token.Token | Token.Atom | Token.Field,
            Token.Token | Token.Atom | Token.Field,
            Token.Token | Token.Atom | Token.Field,
            Token.Ctl,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0
        };


        [System.Flags]
        internal enum Token : short {

            Ctl = (short) 1,
            Spec = (short) 2,
            TSpec = (short) 4,
            Token = (short) 8,
            Atom = (short) 16,
            Digit = (short) 32,
            Hex = (short) 64,
            Lwsp = (short) 128,
            BChar = (short) 256,
            Field = (short) 512,
            Alpha = (short) 1024,
            Fwsp = (short) 2048

        }

    }

}