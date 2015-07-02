namespace Butler.Schema.Data.Globalization {

    public class OutboundCodePageDetector {

        public OutboundCodePageDetector() {
            detector.Initialize();
        }

        internal static bool IsCodePageDetectable(int cpid, bool onlyValid) {
            return CodePageDetect.IsCodePageDetectable(cpid, onlyValid);
        }

        internal static int[] GetDefaultCodePagePriorityList() {
            return CodePageDetect.GetDefaultPriorityList();
        }

        internal static char[] GetCommonExceptionCharacters() {
            return CodePageDetect.GetCommonExceptionCharacters();
        }

        public void Reset() {
            detector.Reset();
        }

        public void AddText(char ch) {
            detector.AddData(ch);
        }

        public void AddText(char[] buffer, int index, int count) {
            if (buffer == null)
                throw new System.ArgumentNullException(nameof(buffer));
            if (index < 0 || index > buffer.Length)
                throw new System.ArgumentOutOfRangeException(nameof(index), Resources.GlobalizationStrings.IndexOutOfRange);
            if (count < 0 || count > buffer.Length)
                throw new System.ArgumentOutOfRangeException(nameof(count), Resources.GlobalizationStrings.CountOutOfRange);
            if (buffer.Length - index < count)
                throw new System.ArgumentOutOfRangeException(nameof(count), Resources.GlobalizationStrings.CountTooLarge);
            detector.AddData(buffer, index, count);
        }

        public void AddText(char[] buffer) {
            if (buffer == null)
                throw new System.ArgumentNullException(nameof(buffer));
            detector.AddData(buffer, 0, buffer.Length);
        }

        public void AddText(string value, int index, int count) {
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));
            if (index < 0 || index > value.Length)
                throw new System.ArgumentOutOfRangeException(nameof(index), Resources.GlobalizationStrings.IndexOutOfRange);
            if (count < 0 || count > value.Length)
                throw new System.ArgumentOutOfRangeException(nameof(count), Resources.GlobalizationStrings.CountOutOfRange);
            if (value.Length - index < count)
                throw new System.ArgumentOutOfRangeException(nameof(count), Resources.GlobalizationStrings.CountTooLarge);
            detector.AddData(value, index, count);
        }

        public void AddText(string value) {
            if (value == null)
                throw new System.ArgumentNullException(nameof(value));
            detector.AddData(value, 0, value.Length);
        }

        public void AddText(System.IO.TextReader reader, int maxCharacters) {
            if (reader == null)
                throw new System.ArgumentNullException(nameof(reader));
            if (maxCharacters < 0)
                throw new System.ArgumentOutOfRangeException(nameof(maxCharacters), Resources.GlobalizationStrings.MaxCharactersCannotBeNegative);
            detector.AddData(reader, maxCharacters);
        }

        public void AddText(System.IO.TextReader reader) {
            if (reader == null)
                throw new System.ArgumentNullException(nameof(reader));
            detector.AddData(reader, int.MaxValue);
        }

        public int GetCodePage() {
            return detector.GetCodePage(Culture.Default.CodepageDetectionPriorityOrder, false, false, true);
        }

        public int GetCodePage(Culture culture, bool allowCommonFallbackExceptions) {
            if (culture == null)
                culture = Culture.Default;
            return detector.GetCodePage(culture.CodepageDetectionPriorityOrder, allowCommonFallbackExceptions, false, true);
        }

        public int GetCodePage(Charset preferredCharset, bool allowCommonFallbackExceptions) {
            var detectionPriorityOrder = Culture.Default.CodepageDetectionPriorityOrder;
            if (preferredCharset != null)
                detectionPriorityOrder = CultureCharsetDatabase.GetAdjustedCodepageDetectionPriorityOrder(preferredCharset, detectionPriorityOrder);
            return detector.GetCodePage(detectionPriorityOrder, allowCommonFallbackExceptions, false, true);
        }

        internal int GetCodePage(int[] codePagePriorityList, FallbackExceptions fallbackExceptions, bool onlyValidCodePages) {
            if (codePagePriorityList != null) {
                for (var index = 0; index < codePagePriorityList.Length; ++index) {
                    if (!CodePageDetect.IsCodePageDetectable(codePagePriorityList[index], false))
                        throw new System.ArgumentException(Resources.GlobalizationStrings.PriorityListIncludesNonDetectableCodePage, nameof(codePagePriorityList));
                }
            }
            return detector.GetCodePage(codePagePriorityList, fallbackExceptions > FallbackExceptions.None, fallbackExceptions > FallbackExceptions.Common, onlyValidCodePages);
        }

        public int[] GetCodePages() {
            return detector.GetCodePages(Culture.Default.CodepageDetectionPriorityOrder, false, false, true);
        }

        public int[] GetCodePages(Culture culture, bool allowCommonFallbackExceptions) {
            if (culture == null)
                culture = Culture.Default;
            return detector.GetCodePages(culture.CodepageDetectionPriorityOrder, allowCommonFallbackExceptions, false, true);
        }

        public int[] GetCodePages(Charset preferredCharset, bool allowCommonFallbackExceptions) {
            var detectionPriorityOrder = Culture.Default.CodepageDetectionPriorityOrder;
            if (preferredCharset != null)
                detectionPriorityOrder = CultureCharsetDatabase.GetAdjustedCodepageDetectionPriorityOrder(preferredCharset, detectionPriorityOrder);
            return detector.GetCodePages(detectionPriorityOrder, allowCommonFallbackExceptions, false, true);
        }

        internal int[] GetCodePages(int[] codePagePriorityList, FallbackExceptions fallbackExceptions, bool onlyValidCodePages) {
            if (codePagePriorityList != null) {
                for (var index = 0; index < codePagePriorityList.Length; ++index) {
                    if (!CodePageDetect.IsCodePageDetectable(codePagePriorityList[index], false))
                        throw new System.ArgumentException(Resources.GlobalizationStrings.PriorityListIncludesNonDetectableCodePage, nameof(codePagePriorityList));
                }
            }
            return detector.GetCodePages(codePagePriorityList, fallbackExceptions > FallbackExceptions.None, fallbackExceptions > FallbackExceptions.Common, onlyValidCodePages);
        }

        public int GetCodePageCoverage(int codePage) {
            var charset = Charset.GetCharset(codePage);
            if (charset.UnicodeCoverage == CodePageUnicodeCoverage.Complete)
                return 100;
            if (!charset.IsDetectable) {
                if (charset.DetectableCodePageWithEquivalentCoverage == 0)
                    throw new System.ArgumentException("codePage is not detectable");
                codePage = charset.DetectableCodePageWithEquivalentCoverage;
            }
            return detector.GetCodePageCoverage(codePage);
        }

        public int GetBestWindowsCodePage() {
            return detector.GetBestWindowsCodePage(false, false);
        }

        internal int GetBestWindowsCodePage(int preferredCodePage) {
            return detector.GetBestWindowsCodePage(false, false, preferredCodePage);
        }

        private CodePageDetect detector;

    }

}