namespace Butler.Schema.Globalization {

    internal struct FeInboundCharsetDetector {

        public FeInboundCharsetDetector(int defaultCodePage, bool strongDefault, bool enableIsoDetection, bool enableUtf8Detection, bool enableDbcsDetection) {
            this.defaultCodePage = (ushort) defaultCodePage;
            this.strongDefault = strongDefault;
            stateIso = FEData.ST.ERR;
            stateUtf8 = FEData.ST.ERR;
            stateGbkWan = FEData.ST.ERR;
            stateEucKrCn = FEData.ST.ERR;
            stateEucJp = FEData.ST.ERR;
            stateSJis = FEData.ST.ERR;
            stateBig5 = FEData.ST.ERR;
            stateJisEsc = FEData.JS.S0;
            countJapaneseEsc = 0;
            countKoreanDesignator = 0;
            countSo = 0;
            count8bit = 0;
            if (enableDbcsDetection) {
                switch (defaultCodePage) {
                    case 52936:
                    case 54936:
                    case 51936:
                    case 50227:
                    case 20936:
                    case 936:
                        stateGbkWan = FEData.ST.ST0;
                        stateEucKrCn = FEData.ST.ST0;
                        break;
                    case 51949:
                    case 20949:
                    case 50225:
                    case 949:
                    case 1361:
                        stateGbkWan = FEData.ST.ST0;
                        stateEucKrCn = FEData.ST.ST0;
                        break;
                    case 50220:
                    case 50221:
                    case 50222:
                    case 51932:
                    case 20932:
                    case 932:
                        stateSJis = FEData.ST.ST0;
                        stateEucJp = FEData.ST.ST0;
                        break;
                    case 950:
                        stateBig5 = FEData.ST.ST0;
                        stateGbkWan = FEData.ST.ST0;
                        break;
                    case 0:
                        stateSJis = FEData.ST.ST0;
                        stateEucJp = FEData.ST.ST0;
                        stateGbkWan = FEData.ST.ST0;
                        stateEucKrCn = FEData.ST.ST0;
                        stateBig5 = FEData.ST.ST0;
                        break;
                }
            }
            if (enableIsoDetection)
                stateIso = FEData.ST.ST0;
            if (!enableUtf8Detection)
                return;
            stateUtf8 = FEData.ST.ST0;
        }

        public bool SJisPossible => stateSJis != FEData.ST.ERR;
        public bool EucJpPossible => stateEucJp != FEData.ST.ERR;
        public bool Iso2022JpPossible => stateIso != FEData.ST.ERR;
        public bool Iso2022KrPossible => stateIso != FEData.ST.ERR;
        public bool Utf8Possible => stateUtf8 != FEData.ST.ERR;
        public bool GbkPossible => stateGbkWan != FEData.ST.ERR;
        public bool WansungPossible => stateGbkWan != FEData.ST.ERR;
        public bool EucKrPossible => stateEucKrCn != FEData.ST.ERR;
        public bool EucCnPossible => stateEucKrCn != FEData.ST.ERR;
        public bool Big5Possible => stateBig5 != FEData.ST.ERR;
        public bool PureAscii => (int) count8bit + (int) countKoreanDesignator + (int) countJapaneseEsc + (int) countSo == 0;

        public bool Iso2022JpVeryLikely {
            get {
                if (this.Iso2022JpPossible && countJapaneseEsc > 0)
                    return countKoreanDesignator == 0;
                return false;
            }
        }

        public bool Iso2022JpLikely {
            get {
                if (!this.Iso2022JpPossible)
                    return false;
                if (countJapaneseEsc > countKoreanDesignator)
                    return true;
                if (countKoreanDesignator == 0)
                    return countSo != 0;
                return false;
            }
        }

        public bool Iso2022KrVeryLikely {
            get {
                if (this.Iso2022KrPossible && countKoreanDesignator > 0)
                    return countJapaneseEsc == 0;
                return false;
            }
        }

        public bool Iso2022KrLikely {
            get {
                if (!this.Iso2022KrPossible)
                    return false;
                if (countKoreanDesignator > countJapaneseEsc)
                    return true;
                if (countJapaneseEsc == 0)
                    return countSo != 0;
                return false;
            }
        }

        public bool SJisLikely {
            get {
                if (this.SJisPossible)
                    return count8bit >= 6;
                return false;
            }
        }

        public bool EucJpLikely {
            get {
                if (this.EucJpPossible)
                    return count8bit >= 6;
                return false;
            }
        }

        public bool Utf8Likely {
            get {
                if (this.Utf8Possible)
                    return count8bit >= 6;
                return false;
            }
        }

        public bool Utf8VeryLikely {
            get {
                if (this.Utf8Possible)
                    return count8bit >= 18;
                return false;
            }
        }

        public bool GbkLikely {
            get {
                if (this.GbkPossible)
                    return count8bit >= 6;
                return false;
            }
        }

        public bool WansungLikely {
            get {
                if (this.WansungPossible)
                    return count8bit >= 6;
                return false;
            }
        }

        public bool EucKrLikely {
            get {
                if (this.EucKrPossible)
                    return count8bit >= 6;
                return false;
            }
        }

        public bool EucCnLikely {
            get {
                if (this.EucCnPossible)
                    return count8bit >= 6;
                return false;
            }
        }

        public bool Big5Likely {
            get {
                if (this.Big5Possible)
                    return count8bit >= 6;
                return false;
            }
        }

        public static bool IsSupportedFarEastCharset(Schema.Globalization.Charset charset) {
            switch (charset.CodePage) {
                case 52936:
                case 54936:
                case 51936:
                case 51949:
                case 20949:
                case 50220:
                case 50221:
                case 50222:
                case 50225:
                case 50227:
                case 51932:
                case 1361:
                case 20932:
                case 20936:
                case 932:
                case 936:
                case 949:
                case 950:
                    return true;
                default:
                    return false;
            }
        }

        public void Reset(int codePage, bool strongDefault, bool enableIsoDetection, bool enableUtf8Detection, bool enableDbcsDetection) {
            this = new FeInboundCharsetDetector(codePage, strongDefault, enableIsoDetection, enableUtf8Detection, enableDbcsDetection);
        }

        public void Reset() {
            this = new FeInboundCharsetDetector(0, false, true, true, true);
        }

        public void RunJisStateMachine(byte bt) {
            var jc = FEData.JisCharClass[bt];
            stateJisEsc = FEData.JisEscNextState[(int) stateJisEsc, (int) jc];
            if ((stateJisEsc & FEData.JS.CNTA) == FEData.JS.S0)
                return;
            if (stateJisEsc == FEData.JS.CNTA) {
                if (countSo != byte.MaxValue)
                    ++countSo;
            } else if (stateJisEsc == FEData.JS.CNTJ) {
                if (countJapaneseEsc != byte.MaxValue)
                    ++countJapaneseEsc;
            } else if (stateJisEsc == FEData.JS.CNTK && countKoreanDesignator != byte.MaxValue)
                ++countKoreanDesignator;
            stateJisEsc = FEData.JS.S0;
        }

        public void RunDbcsStateMachines(FEData.CC cc) {
            if (stateSJis != FEData.ST.ERR)
                stateSJis = FEData.SJisNextState[(int) stateSJis, (int) cc];
            if (stateEucJp != FEData.ST.ERR)
                stateEucJp = FEData.EucJpNextState[(int) stateEucJp, (int) cc];
            if (stateUtf8 != FEData.ST.ERR)
                stateUtf8 = FEData.Utf8NextState[(int) stateUtf8, (int) cc];
            if (stateGbkWan != FEData.ST.ERR)
                stateGbkWan = FEData.GbkWanNextState[(int) stateGbkWan, (int) cc];
            if (stateEucKrCn != FEData.ST.ERR)
                stateEucKrCn = FEData.EucKrCnNextState[(int) stateEucKrCn, (int) cc];
            if (stateBig5 == FEData.ST.ERR)
                return;
            stateBig5 = FEData.Big5NextState[(int) stateBig5, (int) cc];
        }

        public void AddBytes(byte[] bytes, int offset, int length, bool eof) {
            if (stateSJis == FEData.ST.ERR && stateEucJp == FEData.ST.ERR && (stateIso == FEData.ST.ERR && stateGbkWan == FEData.ST.ERR) && (stateEucKrCn == FEData.ST.ERR && stateBig5 == FEData.ST.ERR && stateUtf8 == FEData.ST.ERR))
                return;
            var num = offset + length;
            while (offset < num) {
                var bt = bytes[offset++];
                if (bt > sbyte.MaxValue && count8bit != byte.MaxValue)
                    ++count8bit;
                if (stateIso != FEData.ST.ERR && bt <= sbyte.MaxValue)
                    this.RunJisStateMachine(bt);
                this.RunDbcsStateMachines(FEData.CharClass[bt]);
            }
            if (!eof)
                return;
            this.RunDbcsStateMachines(FEData.CC.eof);
        }

        public int GetCodePageChoice() {
            return this.GetCodePageChoice(defaultCodePage, strongDefault);
        }

        public int GetCodePageChoice(int defaultCodePage, bool strongDefault) {
            if (this.PureAscii)
                return defaultCodePage;
            if (this.Iso2022JpVeryLikely)
                return 50220;
            if (this.Iso2022KrVeryLikely)
                return 50225;
            if (this.Utf8VeryLikely)
                return 65001;
            if (defaultCodePage == 50220 || defaultCodePage == 50221 || defaultCodePage == 50222)
                defaultCodePage = 932;
            else if (defaultCodePage == 50225)
                defaultCodePage = 949;
            else if (defaultCodePage == 20932)
                defaultCodePage = 51932;
            else if (defaultCodePage == 20949)
                defaultCodePage = 51949;
            if (defaultCodePage == 932 || defaultCodePage == 51932) {
                if (this.Iso2022JpLikely)
                    return 50222;
                if (this.SJisPossible && defaultCodePage == 932 && strongDefault)
                    return 932;
                if (this.EucJpPossible && defaultCodePage == 51932 && strongDefault)
                    return 51932;
                if (!this.Utf8Likely) {
                    if (this.EucJpPossible)
                        return 51932;
                    if (this.SJisPossible)
                        return 932;
                }
            } else if (defaultCodePage == 949 || defaultCodePage == 51949) {
                if (this.Iso2022KrLikely)
                    return 50225;
                if (this.WansungPossible && defaultCodePage == 949 && strongDefault)
                    return 949;
                if (this.EucKrPossible && defaultCodePage == 51949 && strongDefault)
                    return 51949;
                if (!this.Utf8Likely) {
                    if (this.WansungPossible)
                        return 949;
                    if (this.EucKrPossible)
                        return 51949;
                }
            } else if (defaultCodePage == 936 || defaultCodePage == 51936) {
                if (this.GbkPossible && defaultCodePage == 936 && strongDefault)
                    return 936;
                if (this.EucCnPossible && defaultCodePage == 51936 && strongDefault)
                    return 51936;
                if (!this.Utf8Likely) {
                    if (this.GbkPossible)
                        return 936;
                    if (this.EucCnPossible)
                        return 51936;
                }
            } else if (defaultCodePage == 950) {
                if (this.Big5Possible && strongDefault)
                    return 950;
                if (!this.Utf8Likely) {
                    if (this.Big5Possible)
                        return 950;
                    if (this.GbkLikely)
                        return 932;
                }
            }
            if (this.Utf8Likely && !strongDefault)
                return 65001;
            return defaultCodePage;
        }

        private readonly ushort defaultCodePage;
        private readonly FEData.ST stateIso;
        private readonly bool strongDefault;
        private byte count8bit;
        private byte countJapaneseEsc;
        private byte countKoreanDesignator;
        private byte countSo;
        private FEData.ST stateBig5;
        private FEData.ST stateEucJp;
        private FEData.ST stateEucKrCn;
        private FEData.ST stateGbkWan;
        private FEData.JS stateJisEsc;
        private FEData.ST stateSJis;
        private FEData.ST stateUtf8;

    }

}