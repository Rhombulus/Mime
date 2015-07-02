namespace Butler.Schema.Globalization {

    internal static class FEData {

        static FEData() {
            var stArray = new ST[2, 20];
            stArray[0, 3] = ST.ERR;
            stArray[0, 4] = ST.ST1;
            stArray[0, 5] = ST.ST1;
            stArray[0, 6] = ST.ST1;
            stArray[0, 7] = ST.ST1;
            stArray[0, 8] = ST.ERR;
            stArray[0, 12] = ST.ST1;
            stArray[0, 13] = ST.ST1;
            stArray[0, 14] = ST.ST1;
            stArray[0, 15] = ST.ST1;
            stArray[0, 16] = ST.ST1;
            stArray[0, 17] = ST.ERR;
            stArray[0, 18] = ST.ERR;
            stArray[1, 0] = ST.ERR;
            stArray[1, 1] = ST.ERR;
            stArray[1, 17] = ST.ERR;
            stArray[1, 18] = ST.ERR;
            stArray[1, 19] = ST.ERR;
            SJisNextState = stArray;
            EucJpNextState = new ST[4, 20] {
                {
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ERR,
                    ST.ERR,
                    ST.ST2,
                    ST.ST3,
                    ST.ERR,
                    ST.ERR,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ERR,
                    ST.ST0
                }, {
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ERR,
                    ST.ERR
                }, {
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR
                }, {
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ERR,
                    ST.ERR
                }
            };
            GbkWanNextState = new ST[2, 20] {
                {
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ERR,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ERR,
                    ST.ST0
                }, {
                    ST.ERR,
                    ST.ERR,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ERR,
                    ST.ERR
                }
            };
            EucKrCnNextState = new ST[2, 20] {
                {
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ERR,
                    ST.ST0
                }, {
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ERR,
                    ST.ERR
                }
            };
            Big5NextState = new ST[2, 20] {
                {
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ERR,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ERR,
                    ST.ST0
                }, {
                    ST.ERR,
                    ST.ERR,
                    ST.ST0,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ERR,
                    ST.ERR
                }
            };
            Utf8NextState = new ST[6, 20] {
                {
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ST1,
                    ST.ST4,
                    ST.ST2,
                    ST.ST5,
                    ST.ST3,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ST0
                }, {
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ST0,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR
                }, {
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ST1,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR
                }, {
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ST2,
                    ST.ST2,
                    ST.ST2,
                    ST.ST2,
                    ST.ST2,
                    ST.ST2,
                    ST.ST2,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR
                }, {
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ST1,
                    ST.ST1,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR
                }, {
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ST2,
                    ST.ST2,
                    ST.ST2,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR,
                    ST.ERR
                }
            };
            var jcArray = new JC[128];
            jcArray[14] = JC.so;
            jcArray[15] = JC.si;
            jcArray[27] = JC.esc;
            jcArray[36] = JC.dlr;
            jcArray[38] = JC.amp;
            jcArray[40] = JC.opr;
            jcArray[41] = JC.cpr;
            jcArray[64] = JC.at;
            jcArray[66] = JC.tkB;
            jcArray[67] = JC.tkC;
            jcArray[68] = JC.tkD;
            jcArray[72] = JC.tkH;
            jcArray[73] = JC.tkI;
            jcArray[74] = JC.tkJ;
            JisCharClass = jcArray;
            var jsArray = new JS[7, 15];
            jsArray[0, 1] = JS.CNTA;
            jsArray[0, 3] = JS.S1;
            jsArray[1, 4] = JS.S2;
            jsArray[1, 5] = JS.S6;
            jsArray[1, 6] = JS.S5;
            jsArray[2, 6] = JS.S3;
            jsArray[2, 7] = JS.S4;
            jsArray[2, 8] = JS.CNTJ;
            jsArray[2, 9] = JS.CNTJ;
            jsArray[3, 11] = JS.CNTJ;
            jsArray[4, 10] = JS.CNTK;
            jsArray[5, 9] = JS.CNTJ;
            jsArray[5, 12] = JS.CNTJ;
            jsArray[5, 13] = JS.CNTJ;
            jsArray[5, 14] = JS.CNTJ;
            jsArray[6, 8] = JS.CNTJ;
            JisEscNextState = jsArray;
        }

        internal static CC[] CharClass = new CC[256] {
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.ctlws,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x213f,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.x407e,
            CC.ctlws,
            CC.x0080,
            CC.x818d,
            CC.x818d,
            CC.x818d,
            CC.x818d,
            CC.x818d,
            CC.x818d,
            CC.x818d,
            CC.x818d,
            CC.x818d,
            CC.x818d,
            CC.x818d,
            CC.x818d,
            CC.x818d,
            CC.x008e,
            CC.x008f,
            CC.x909f,
            CC.x909f,
            CC.x909f,
            CC.x909f,
            CC.x909f,
            CC.x909f,
            CC.x909f,
            CC.x909f,
            CC.x909f,
            CC.x909f,
            CC.x909f,
            CC.x909f,
            CC.x909f,
            CC.x909f,
            CC.x909f,
            CC.x909f,
            CC.x00a0,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xa1bf,
            CC.xc0c1,
            CC.xc0c1,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.xc2df,
            CC.x00e0,
            CC.xe1ef,
            CC.xe1ef,
            CC.xe1ef,
            CC.xe1ef,
            CC.xe1ef,
            CC.xe1ef,
            CC.xe1ef,
            CC.xe1ef,
            CC.xe1ef,
            CC.xe1ef,
            CC.xe1ef,
            CC.xe1ef,
            CC.xe1ef,
            CC.xe1ef,
            CC.xe1ef,
            CC.x00f0,
            CC.xf1f7,
            CC.xf1f7,
            CC.xf1f7,
            CC.xf1f7,
            CC.xf1f7,
            CC.xf1f7,
            CC.xf1f7,
            CC.xf8fc,
            CC.xf8fc,
            CC.xf8fc,
            CC.xf8fc,
            CC.xf8fc,
            CC.xfdfe,
            CC.xfdfe,
            CC.x00ff
        };

        internal static ST[,] SJisNextState;
        internal static ST[,] EucJpNextState;
        internal static ST[,] GbkWanNextState;
        internal static ST[,] EucKrCnNextState;
        internal static ST[,] Big5NextState;
        internal static ST[,] Utf8NextState;
        internal static JC[] JisCharClass;
        internal static JS[,] JisEscNextState;


        internal enum CC : byte {

            ctlws,
            x213f,
            x407e,
            x0080,
            x818d,
            x008e,
            x008f,
            x909f,
            x00a0,
            xa1bf,
            xc0c1,
            xc2df,
            x00e0,
            xe1ef,
            x00f0,
            xf1f7,
            xf8fc,
            xfdfe,
            x00ff,
            eof

        }


        internal enum ST : byte {

            ACC = 0,
            ST0 = 0,
            ST1 = (byte) 1,
            ST2 = (byte) 2,
            ST3 = (byte) 3,
            ST4 = (byte) 4,
            ST5 = (byte) 5,
            ERR = (byte) 127

        }


        internal enum JC : byte {

            ni,
            so,
            si,
            esc,
            dlr,
            amp,
            opr,
            cpr,
            at,
            tkB,
            tkC,
            tkD,
            tkH,
            tkI,
            tkJ

        }


        internal enum JS : byte {

            S0 = 0,
            S1 = (byte) 1,
            S2 = (byte) 2,
            S3 = (byte) 3,
            S4 = (byte) 4,
            S5 = (byte) 5,
            S6 = (byte) 6,
            CNTA = (byte) 128,
            CNTJ = (byte) 129,
            CNTK = (byte) 130

        }

    }

}