namespace Butler.Schema.Data.Globalization {

    internal class CodePageMap : CodePageMapData {

        public bool ChoseCodePage(int codePage) {
            if (codePage == this.codePage)
                return true;
            this.codePage = codePage;
            ranges = null;
            if (codePage == 1200)
                return true;
            for (var index = codePages.Length - 1; index >= 0; --index) {
                if (codePages[index].cpid == codePage) {
                    ranges = codePages[index].ranges;
                    lastRangeIndex = ranges.Length/2;
                    lastRange = ranges[lastRangeIndex];
                    return true;
                }
            }
            return false;
        }

        public bool ChoseCodePage(System.Text.Encoding encoding) {
            return this.ChoseCodePage(CodePageMap.GetCodePage(encoding));
        }

        public bool IsUnsafeExtendedCharacter(char ch) {
            if (ranges == null)
                return false;
            if (ch <= lastRange.last) {
                if (ch >= lastRange.first) {
                    if (lastRange.offset != ushort.MaxValue)
                        return (bitmap[lastRange.offset + (ch - lastRange.first)] & lastRange.mask) == 0;
                    return false;
                }
                var index = lastRangeIndex;
                while (--index >= 0) {
                    if (ch >= ranges[index].first) {
                        if (ch <= ranges[index].last) {
                            if (ch == ranges[index].first)
                                return false;
                            lastRangeIndex = index;
                            lastRange = ranges[index];
                            if (lastRange.offset != ushort.MaxValue)
                                return (bitmap[lastRange.offset + (ch - lastRange.first)] & lastRange.mask) == 0;
                            return false;
                        }
                        break;
                    }
                }
            } else {
                var index = lastRangeIndex;
                while (++index < ranges.Length) {
                    if (ch <= ranges[index].last) {
                        if (ch >= ranges[index].first) {
                            if (ch == ranges[index].first)
                                return false;
                            lastRangeIndex = index;
                            lastRange = ranges[index];
                            if (lastRange.offset != ushort.MaxValue)
                                return (bitmap[lastRange.offset + (ch - lastRange.first)] & lastRange.mask) == 0;
                            return false;
                        }
                        break;
                    }
                }
            }
            return true;
        }

        public static System.Text.Encoding GetEncoding(int codePage) {
            return System.Text.Encoding.GetEncoding(codePage);
        }

        public static int GetCodePage(System.Text.Encoding encoding) {
            return encoding.CodePage;
        }

        public static int GetWindowsCodePage(System.Text.Encoding encoding) {
            return encoding.WindowsCodePage;
        }

        private int codePage;
        private CodePageRange lastRange;
        private int lastRangeIndex;
        private CodePageRange[] ranges;

    }

}