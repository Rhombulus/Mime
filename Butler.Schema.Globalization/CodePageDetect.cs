namespace Butler.Schema.Data.Globalization {

    internal struct CodePageDetect {

        public static bool IsCodePageDetectable(int cpid, bool validOnly) {
            var num = CodePageDetectData.codePageIndex[cpid%CodePageDetectData.codePageIndex.Length];
            if (num == byte.MaxValue || CodePageDetectData.codePages[num].cpid != cpid && (CodePageDetectData.codePages[num].cpid != 38598 || cpid != 28598) && (CodePageDetectData.codePages[num].cpid != 936 || cpid != 54936))
                return false;
            if (!validOnly)
                return true;
            return 0 != ((int) ValidCodePagesMask & (int) CodePageDetectData.codePages[num].mask);
        }

        public static char[] GetCommonExceptionCharacters() {
            return CodePageDetectData.commonExceptions.Clone() as char[];
        }

        public static int[] GetDefaultPriorityList() {
            var numArray = new int[CodePageDetectData.codePages.Length];
            for (var index = 0; index < CodePageDetectData.codePages.Length; ++index) {
                numArray[index] = CodePageDetectData.codePages[index].cpid;
            }
            return numArray;
        }

        public void Initialize() {
            maskMap = new int[CodePageDetectData.codePageMask.Length];
        }

        public void Reset() {
            for (var index = 0; index < maskMap.Length; ++index) {
                maskMap[index] = 0;
            }
        }

        public void AddData(char ch) {
            ++maskMap[CodePageDetectData.index[ch]];
        }

        public void AddData(char[] buffer, int offset, int count) {
            ++count;
            while (--count != 0) {
                ++maskMap[CodePageDetectData.index[buffer[offset]]];
                ++offset;
            }
        }

        public void AddData(string buffer, int offset, int count) {
            ++count;
            while (--count != 0) {
                ++maskMap[CodePageDetectData.index[buffer[offset]]];
                ++offset;
            }
        }

        public void AddData(System.IO.TextReader reader, int maxCharacters) {
            var buffer = new char[1024];
            while (maxCharacters != 0) {
                var count = System.Math.Min(buffer.Length, maxCharacters);
                var num1 = reader.Read(buffer, 0, count);
                if (num1 == 0)
                    break;
                var index = 0;
                maxCharacters -= num1;
                var num2 = num1 + 1;
                while (--num2 != 0) {
                    ++maskMap[CodePageDetectData.index[buffer[index]]];
                    ++index;
                }
            }
        }

        public int GetCodePage(int[] codePagePriorityList, bool allowCommonFallbackExceptions, bool allowAnyFallbackExceptions, bool onlyValidCodePages) {
            var cumulativeMask = uint.MaxValue;
            for (var index = 0; index < maskMap.Length; ++index) {
                if (maskMap[index] != 0) {
                    var num = CodePageDetectData.codePageMask[index];
                    if (allowAnyFallbackExceptions || allowCommonFallbackExceptions && ((int) CodePageDetectData.fallbackMask[index] & 1) != 0)
                        num |= CodePageDetectData.fallbackMask[index] & 4294967294U;
                    cumulativeMask &= num;
                    if ((int) cumulativeMask == 0)
                        break;
                }
            }
            if (onlyValidCodePages)
                cumulativeMask &= ValidCodePagesMask;
            return CodePageDetect.GetCodePage(ref cumulativeMask, codePagePriorityList);
        }

        public int[] GetCodePages(int[] codePagePriorityList, bool allowCommonFallbackExceptions, bool allowAnyFallbackExceptions, bool onlyValidCodePages) {
            var cumulativeMask = uint.MaxValue;
            for (var index = 0; index < maskMap.Length; ++index) {
                if (maskMap[index] != 0) {
                    var num = CodePageDetectData.codePageMask[index];
                    if (allowAnyFallbackExceptions || allowCommonFallbackExceptions && ((int) CodePageDetectData.fallbackMask[index] & 1) != 0)
                        num |= CodePageDetectData.fallbackMask[index] & 4294967294U;
                    cumulativeMask &= num;
                    if ((int) cumulativeMask == 0)
                        break;
                }
            }
            if (onlyValidCodePages)
                cumulativeMask &= ValidCodePagesMask;
            var numArray = new int[CodePageDetect.GetCodePageCount(cumulativeMask)];
            var index1 = 0;
            while ((int) cumulativeMask != 0)
                numArray[index1++] = CodePageDetect.GetCodePage(ref cumulativeMask, codePagePriorityList);
            numArray[index1] = 65001;
            return numArray;
        }

        public int GetCodePageCoverage(int codePage) {
            var num1 = 0U;
            var num2 = 0;
            var num3 = 0;
            var num4 = CodePageDetectData.codePageIndex[codePage%CodePageDetectData.codePageIndex.Length];
            if (num4 != byte.MaxValue && (CodePageDetectData.codePages[num4].cpid == codePage || CodePageDetectData.codePages[num4].cpid == 38598 && codePage == 28598 || CodePageDetectData.codePages[num4].cpid == 936 && codePage == 54936))
                num1 = CodePageDetectData.codePages[num4].mask;
            if ((int) num1 != 0) {
                for (var index = 0; index < maskMap.Length; ++index) {
                    if ((int) num1 == ((int) CodePageDetectData.codePageMask[index] & (int) num1))
                        num2 += maskMap[index];
                    num3 += maskMap[index];
                }
            }
            if (num3 != 0)
                return (int) (num2*100L/num3);
            return 0;
        }

        public int GetBestWindowsCodePage(bool allowCommonFallbackExceptions, bool allowAnyFallbackExceptions) {
            return this.GetBestWindowsCodePage(allowCommonFallbackExceptions, allowAnyFallbackExceptions, 0);
        }

        public int GetBestWindowsCodePage(bool allowCommonFallbackExceptions, bool allowAnyFallbackExceptions, int preferredCodePage) {
            var num1 = 0;
            if (codePageList == null)
                codePageList = new int[CodePageDetectData.codePages.Length + 1];
            else {
                for (var index = 0; index < codePageList.Length; ++index) {
                    codePageList[index] = 0;
                }
            }
            for (var index1 = 0; index1 < maskMap.Length; ++index1) {
                if (maskMap[index1] != 0) {
                    var num2 = CodePageDetectData.codePageMask[index1];
                    if (allowAnyFallbackExceptions || allowCommonFallbackExceptions && ((int) CodePageDetectData.fallbackMask[index1] & 1) != 0)
                        num2 |= CodePageDetectData.fallbackMask[index1] & 4294967294U;
                    var num3 = num2 & 260038656U;
                    if ((int) num3 != 0) {
                        var index2 = 0;
                        while ((int) num3 != 0) {
                            if (((int) num3 & 1) != 0)
                                codePageList[index2] += maskMap[index1];
                            ++index2;
                            num3 >>= 1;
                        }
                    }
                    num1 += maskMap[index1];
                }
            }
            var num4 = 0;
            var index3 = 0;
            var num5 = 260038656U;
            var index4 = 0;
            while ((int) num5 != 0) {
                if (CodePageDetectData.codePages[index4].windowsCodePage && codePageList[index4] > num4) {
                    num4 = codePageList[index4];
                    index3 = index4;
                } else if (codePageList[index4] == num4 && (CodePageDetectData.codePages[index4].cpid == 1252 || CodePageDetectData.codePages[index4].cpid == preferredCodePage))
                    index3 = index4;
                num5 &= ~CodePageDetectData.codePages[index4].mask;
                ++index4;
            }
            return CodePageDetectData.codePages[index3].cpid;
        }

        internal static int GetCodePage(ref uint cumulativeMask, int[] codePagePriorityList) {
            if ((int) cumulativeMask != 0) {
                if (codePagePriorityList != null) {
                    for (var index = 0; index < codePagePriorityList.Length; ++index) {
                        var num = CodePageDetectData.codePageIndex[codePagePriorityList[index]%CodePageDetectData.codePageIndex.Length];
                        if (num != byte.MaxValue && ((int) cumulativeMask & (int) CodePageDetectData.codePages[num].mask) != 0 &&
                            (CodePageDetectData.codePages[num].cpid == codePagePriorityList[index] || CodePageDetectData.codePages[num].cpid == 38598 && codePagePriorityList[index] == 28598 ||
                             CodePageDetectData.codePages[num].cpid == 936 && codePagePriorityList[index] == 54936)) {
                            cumulativeMask &= ~CodePageDetectData.codePages[num].mask;
                            return codePagePriorityList[index];
                        }
                    }
                }
                for (var index = 0; index < CodePageDetectData.codePages.Length; ++index) {
                    if (((int) cumulativeMask & (int) CodePageDetectData.codePages[index].mask) != 0) {
                        cumulativeMask &= ~CodePageDetectData.codePages[index].mask;
                        return CodePageDetectData.codePages[index].cpid;
                    }
                }
            }
            return 65001;
        }

        internal static int GetCodePageCount(uint cumulativeMask) {
            var num = 1;
            while ((int) cumulativeMask != 0) {
                if (((int) cumulativeMask & 1) != 0)
                    ++num;
                cumulativeMask >>= 1;
            }
            return num;
        }

        private static uint InitializeValidCodePagesMask() {
            var num = 0U;
            for (var index = 0; index < CodePageDetectData.codePages.Length; ++index) {
                Charset charset;
                if (Charset.TryGetCharset(CodePageDetectData.codePages[index].cpid, out charset) && charset.IsAvailable)
                    num |= CodePageDetectData.codePages[index].mask;
            }
            return num;
        }

        internal static uint ValidCodePagesMask = CodePageDetect.InitializeValidCodePagesMask();
        private int[] codePageList;
        private int[] maskMap;

    }

}