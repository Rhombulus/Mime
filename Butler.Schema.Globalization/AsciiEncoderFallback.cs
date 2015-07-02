namespace Butler.Schema.Data.Globalization {

    internal class AsciiEncoderFallback : System.Text.EncoderFallback {

        public override int MaxCharCount => AsciiFallbackBuffer.MaxCharCount;

        public static string GetCharacterFallback(char charUnknown) {
            if (charUnknown <= 339 && charUnknown >= 130) {
                switch (charUnknown) {
                    case 'Ĳ':
                        return "IJ";
                    case 'ĳ':
                        return "ij";
                    case 'Œ':
                        return "OE";
                    case 'œ':
                        return "oe";
                    case '\x0082':
                    case '\x0091':
                    case '\x0092':
                        return "'";
                    case '\x0083':
                        return "f";
                    case '\x0084':
                    case '\x0093':
                    case '\x0094':
                        return "\"";
                    case '\x0085':
                        return "...";
                    case '\x008B':
                        return "<";
                    case '\x008C':
                        return "OE";
                    case '\x0095':
                        return "*";
                    case '\x0096':
                        return "-";
                    case '\x0097':
                        return "-";
                    case '\x0098':
                        return "~";
                    case '\x0099':
                        return "(tm)";
                    case '\x009B':
                        return ">";
                    case '\x009C':
                        return "oe";
                    case ' ':
                        return " ";
                    case '¢':
                        return "c";
                    case '¤':
                        return "$";
                    case '¥':
                        return "Y";
                    case '¦':
                        return "|";
                    case '©':
                        return "(c)";
                    case '«':
                        return "<";
                    case '\x00AD':
                        return string.Empty;
                    case '®':
                        return "(r)";
                    case '\x00B2':
                        return "^2";
                    case '\x00B3':
                        return "^3";
                    case '·':
                        return "*";
                    case '¸':
                        return ",";
                    case '\x00B9':
                        return "^1";
                    case '»':
                        return ">";
                    case '\x00BC':
                        return "(1/4)";
                    case '\x00BD':
                        return "(1/2)";
                    case '\x00BE':
                        return "(3/4)";
                    case 'Æ':
                        return "AE";
                    case 'æ':
                        return "ae";
                }
            } else if (charUnknown >= 8194 && charUnknown <= 8482) {
                switch (charUnknown) {
                    case '‹':
                        return "<";
                    case '›':
                        return ">";
                    case '€':
                        return "EUR";
                    case '™':
                        return "(tm)";
                    case ' ':
                    case ' ':
                        return " ";
                    case '‑':
                        return "-";
                    case '–':
                    case '—':
                        return "-";
                    case '‘':
                    case '’':
                    case '‚':
                        return "'";
                    case '“':
                    case '”':
                    case '„':
                        return "\"";
                    case '•':
                        return "*";
                    case '…':
                        return "...";
                }
            } else if (charUnknown >= 9785 && charUnknown <= 9786) {
                switch (charUnknown) {
                    case '☹':
                        return ":(";
                    case '☺':
                        return ":)";
                }
            }
            return null;
        }

        public override System.Text.EncoderFallbackBuffer CreateFallbackBuffer() {
            return new AsciiFallbackBuffer();
        }


        private class AsciiFallbackBuffer : System.Text.EncoderFallbackBuffer {

            public static int MaxCharCount => 5;

            public override int Remaining {
                get {
                    if (fallbackString != null)
                        return fallbackString.Length - fallbackIndex;
                    return 0;
                }
            }

            public override bool Fallback(char charUnknown, int index) {
                fallbackIndex = 0;
                fallbackString = AsciiEncoderFallback.GetCharacterFallback(charUnknown);
                if (fallbackString == null)
                    fallbackString = "?";
                return true;
            }

            public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index) {
                fallbackIndex = 0;
                fallbackString = "?";
                return true;
            }

            public override char GetNextChar() {
                if (fallbackString == null || fallbackIndex == fallbackString.Length)
                    return char.MinValue;
                return fallbackString[fallbackIndex++];
            }

            public override bool MovePrevious() {
                if (fallbackIndex <= 0)
                    return false;
                --fallbackIndex;
                return true;
            }

            public override void Reset() {
                fallbackString = "?";
                fallbackIndex = 0;
            }

            private int fallbackIndex;
            private string fallbackString;

        }

    }

}