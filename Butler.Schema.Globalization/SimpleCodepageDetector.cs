namespace Butler.Schema.Globalization {

    internal struct SimpleCodepageDetector {

        public void AddData(char ch) {
            mask |= ~CodePageDetectData.codePageMask[CodePageDetectData.index[ch]];
        }

        public void AddData(char[] buffer, int offset, int count) {
            ++count;
            while (--count != 0) {
                mask |= ~CodePageDetectData.codePageMask[CodePageDetectData.index[buffer[offset]]];
                ++offset;
            }
        }

        public void AddData(string buffer, int offset, int count) {
            ++count;
            while (--count != 0) {
                mask |= ~CodePageDetectData.codePageMask[CodePageDetectData.index[buffer[offset]]];
                ++offset;
            }
        }

        public int GetCodePage(int[] codePagePriorityList, bool onlyValidCodePages) {
            var cumulativeMask = ~mask;
            if (onlyValidCodePages)
                cumulativeMask &= CodePageDetect.ValidCodePagesMask;
            return CodePageDetect.GetCodePage(ref cumulativeMask, codePagePriorityList);
        }

        public int GetWindowsCodePage(int[] codePagePriorityList, bool onlyValidCodePages) {
            var cumulativeMask = ~mask & 260038656U;
            if (onlyValidCodePages)
                cumulativeMask &= CodePageDetect.ValidCodePagesMask;
            return (int) cumulativeMask == 0 ? 1252 : CodePageDetect.GetCodePage(ref cumulativeMask, codePagePriorityList);
        }

        private uint mask;

    }

}