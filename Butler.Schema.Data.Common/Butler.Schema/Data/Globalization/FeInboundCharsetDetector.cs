// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Globalization.FeInboundCharsetDetector
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Globalization
{
  internal struct FeInboundCharsetDetector
  {
    private ushort defaultCodePage;
    private bool strongDefault;
    private FEData.ST stateIso;
    private FEData.ST stateUtf8;
    private FEData.ST stateSJis;
    private FEData.ST stateEucJp;
    private FEData.ST stateGbkWan;
    private FEData.ST stateEucKrCn;
    private FEData.ST stateBig5;
    private FEData.JS stateJisEsc;
    private byte countJapaneseEsc;
    private byte countKoreanDesignator;
    private byte countSo;
    private byte count8bit;

    public bool SJisPossible
    {
      get
      {
        return this.stateSJis != FEData.ST.ERR;
      }
    }

    public bool EucJpPossible
    {
      get
      {
        return this.stateEucJp != FEData.ST.ERR;
      }
    }

    public bool Iso2022JpPossible
    {
      get
      {
        return this.stateIso != FEData.ST.ERR;
      }
    }

    public bool Iso2022KrPossible
    {
      get
      {
        return this.stateIso != FEData.ST.ERR;
      }
    }

    public bool Utf8Possible
    {
      get
      {
        return this.stateUtf8 != FEData.ST.ERR;
      }
    }

    public bool GbkPossible
    {
      get
      {
        return this.stateGbkWan != FEData.ST.ERR;
      }
    }

    public bool WansungPossible
    {
      get
      {
        return this.stateGbkWan != FEData.ST.ERR;
      }
    }

    public bool EucKrPossible
    {
      get
      {
        return this.stateEucKrCn != FEData.ST.ERR;
      }
    }

    public bool EucCnPossible
    {
      get
      {
        return this.stateEucKrCn != FEData.ST.ERR;
      }
    }

    public bool Big5Possible
    {
      get
      {
        return this.stateBig5 != FEData.ST.ERR;
      }
    }

    public bool PureAscii
    {
      get
      {
        return (int) this.count8bit + (int) this.countKoreanDesignator + (int) this.countJapaneseEsc + (int) this.countSo == 0;
      }
    }

    public bool Iso2022JpVeryLikely
    {
      get
      {
        if (this.Iso2022JpPossible && (int) this.countJapaneseEsc > 0)
          return (int) this.countKoreanDesignator == 0;
        return false;
      }
    }

    public bool Iso2022JpLikely
    {
      get
      {
        if (!this.Iso2022JpPossible)
          return false;
        if ((int) this.countJapaneseEsc > (int) this.countKoreanDesignator)
          return true;
        if ((int) this.countKoreanDesignator == 0)
          return (int) this.countSo != 0;
        return false;
      }
    }

    public bool Iso2022KrVeryLikely
    {
      get
      {
        if (this.Iso2022KrPossible && (int) this.countKoreanDesignator > 0)
          return (int) this.countJapaneseEsc == 0;
        return false;
      }
    }

    public bool Iso2022KrLikely
    {
      get
      {
        if (!this.Iso2022KrPossible)
          return false;
        if ((int) this.countKoreanDesignator > (int) this.countJapaneseEsc)
          return true;
        if ((int) this.countJapaneseEsc == 0)
          return (int) this.countSo != 0;
        return false;
      }
    }

    public bool SJisLikely
    {
      get
      {
        if (this.SJisPossible)
          return (int) this.count8bit >= 6;
        return false;
      }
    }

    public bool EucJpLikely
    {
      get
      {
        if (this.EucJpPossible)
          return (int) this.count8bit >= 6;
        return false;
      }
    }

    public bool Utf8Likely
    {
      get
      {
        if (this.Utf8Possible)
          return (int) this.count8bit >= 6;
        return false;
      }
    }

    public bool Utf8VeryLikely
    {
      get
      {
        if (this.Utf8Possible)
          return (int) this.count8bit >= 18;
        return false;
      }
    }

    public bool GbkLikely
    {
      get
      {
        if (this.GbkPossible)
          return (int) this.count8bit >= 6;
        return false;
      }
    }

    public bool WansungLikely
    {
      get
      {
        if (this.WansungPossible)
          return (int) this.count8bit >= 6;
        return false;
      }
    }

    public bool EucKrLikely
    {
      get
      {
        if (this.EucKrPossible)
          return (int) this.count8bit >= 6;
        return false;
      }
    }

    public bool EucCnLikely
    {
      get
      {
        if (this.EucCnPossible)
          return (int) this.count8bit >= 6;
        return false;
      }
    }

    public bool Big5Likely
    {
      get
      {
        if (this.Big5Possible)
          return (int) this.count8bit >= 6;
        return false;
      }
    }

    public FeInboundCharsetDetector(int defaultCodePage, bool strongDefault, bool enableIsoDetection, bool enableUtf8Detection, bool enableDbcsDetection)
    {
      this.defaultCodePage = (ushort) defaultCodePage;
      this.strongDefault = strongDefault;
      this.stateIso = FEData.ST.ERR;
      this.stateUtf8 = FEData.ST.ERR;
      this.stateGbkWan = FEData.ST.ERR;
      this.stateEucKrCn = FEData.ST.ERR;
      this.stateEucJp = FEData.ST.ERR;
      this.stateSJis = FEData.ST.ERR;
      this.stateBig5 = FEData.ST.ERR;
      this.stateJisEsc = FEData.JS.S0;
      this.countJapaneseEsc = (byte) 0;
      this.countKoreanDesignator = (byte) 0;
      this.countSo = (byte) 0;
      this.count8bit = (byte) 0;
      if (enableDbcsDetection)
      {
        switch (defaultCodePage)
        {
          case 52936:
          case 54936:
          case 51936:
          case 50227:
          case 20936:
          case 936:
            this.stateGbkWan = FEData.ST.ST0;
            this.stateEucKrCn = FEData.ST.ST0;
            break;
          case 51949:
          case 20949:
          case 50225:
          case 949:
          case 1361:
            this.stateGbkWan = FEData.ST.ST0;
            this.stateEucKrCn = FEData.ST.ST0;
            break;
          case 50220:
          case 50221:
          case 50222:
          case 51932:
          case 20932:
          case 932:
            this.stateSJis = FEData.ST.ST0;
            this.stateEucJp = FEData.ST.ST0;
            break;
          case 950:
            this.stateBig5 = FEData.ST.ST0;
            this.stateGbkWan = FEData.ST.ST0;
            break;
          case 0:
            this.stateSJis = FEData.ST.ST0;
            this.stateEucJp = FEData.ST.ST0;
            this.stateGbkWan = FEData.ST.ST0;
            this.stateEucKrCn = FEData.ST.ST0;
            this.stateBig5 = FEData.ST.ST0;
            break;
        }
      }
      if (enableIsoDetection)
        this.stateIso = FEData.ST.ST0;
      if (!enableUtf8Detection)
        return;
      this.stateUtf8 = FEData.ST.ST0;
    }

    public static bool IsSupportedFarEastCharset(Charset charset)
    {
      switch (charset.CodePage)
      {
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

    public void Reset(int codePage, bool strongDefault, bool enableIsoDetection, bool enableUtf8Detection, bool enableDbcsDetection)
    {
      this = new FeInboundCharsetDetector(codePage, strongDefault, enableIsoDetection, enableUtf8Detection, enableDbcsDetection);
    }

    public void Reset()
    {
      this = new FeInboundCharsetDetector(0, false, true, true, true);
    }

    public void RunJisStateMachine(byte bt)
    {
      FEData.JC jc = FEData.JisCharClass[(int) bt];
      this.stateJisEsc = FEData.JisEscNextState[(int) this.stateJisEsc, (int) jc];
      if ((this.stateJisEsc & FEData.JS.CNTA) == FEData.JS.S0)
        return;
      if (this.stateJisEsc == FEData.JS.CNTA)
      {
        if ((int) this.countSo != (int) byte.MaxValue)
          ++this.countSo;
      }
      else if (this.stateJisEsc == FEData.JS.CNTJ)
      {
        if ((int) this.countJapaneseEsc != (int) byte.MaxValue)
          ++this.countJapaneseEsc;
      }
      else if (this.stateJisEsc == FEData.JS.CNTK && (int) this.countKoreanDesignator != (int) byte.MaxValue)
        ++this.countKoreanDesignator;
      this.stateJisEsc = FEData.JS.S0;
    }

    public void RunDbcsStateMachines(FEData.CC cc)
    {
      if (this.stateSJis != FEData.ST.ERR)
        this.stateSJis = FEData.SJisNextState[(int) this.stateSJis, (int) cc];
      if (this.stateEucJp != FEData.ST.ERR)
        this.stateEucJp = FEData.EucJpNextState[(int) this.stateEucJp, (int) cc];
      if (this.stateUtf8 != FEData.ST.ERR)
        this.stateUtf8 = FEData.Utf8NextState[(int) this.stateUtf8, (int) cc];
      if (this.stateGbkWan != FEData.ST.ERR)
        this.stateGbkWan = FEData.GbkWanNextState[(int) this.stateGbkWan, (int) cc];
      if (this.stateEucKrCn != FEData.ST.ERR)
        this.stateEucKrCn = FEData.EucKrCnNextState[(int) this.stateEucKrCn, (int) cc];
      if (this.stateBig5 == FEData.ST.ERR)
        return;
      this.stateBig5 = FEData.Big5NextState[(int) this.stateBig5, (int) cc];
    }

    public void AddBytes(byte[] bytes, int offset, int length, bool eof)
    {
      if (this.stateSJis == FEData.ST.ERR && this.stateEucJp == FEData.ST.ERR && (this.stateIso == FEData.ST.ERR && this.stateGbkWan == FEData.ST.ERR) && (this.stateEucKrCn == FEData.ST.ERR && this.stateBig5 == FEData.ST.ERR && this.stateUtf8 == FEData.ST.ERR))
        return;
      int num = offset + length;
      while (offset < num)
      {
        byte bt = bytes[offset++];
        if ((int) bt > (int) sbyte.MaxValue && (int) this.count8bit != (int) byte.MaxValue)
          ++this.count8bit;
        if (this.stateIso != FEData.ST.ERR && (int) bt <= (int) sbyte.MaxValue)
          this.RunJisStateMachine(bt);
        this.RunDbcsStateMachines(FEData.CharClass[(int) bt]);
      }
      if (!eof)
        return;
      this.RunDbcsStateMachines(FEData.CC.eof);
    }

    public int GetCodePageChoice()
    {
      return this.GetCodePageChoice((int) this.defaultCodePage, this.strongDefault);
    }

    public int GetCodePageChoice(int defaultCodePage, bool strongDefault)
    {
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
      if (defaultCodePage == 932 || defaultCodePage == 51932)
      {
        if (this.Iso2022JpLikely)
          return 50222;
        if (this.SJisPossible && defaultCodePage == 932 && strongDefault)
          return 932;
        if (this.EucJpPossible && defaultCodePage == 51932 && strongDefault)
          return 51932;
        if (!this.Utf8Likely)
        {
          if (this.EucJpPossible)
            return 51932;
          if (this.SJisPossible)
            return 932;
        }
      }
      else if (defaultCodePage == 949 || defaultCodePage == 51949)
      {
        if (this.Iso2022KrLikely)
          return 50225;
        if (this.WansungPossible && defaultCodePage == 949 && strongDefault)
          return 949;
        if (this.EucKrPossible && defaultCodePage == 51949 && strongDefault)
          return 51949;
        if (!this.Utf8Likely)
        {
          if (this.WansungPossible)
            return 949;
          if (this.EucKrPossible)
            return 51949;
        }
      }
      else if (defaultCodePage == 936 || defaultCodePage == 51936)
      {
        if (this.GbkPossible && defaultCodePage == 936 && strongDefault)
          return 936;
        if (this.EucCnPossible && defaultCodePage == 51936 && strongDefault)
          return 51936;
        if (!this.Utf8Likely)
        {
          if (this.GbkPossible)
            return 936;
          if (this.EucCnPossible)
            return 51936;
        }
      }
      else if (defaultCodePage == 950)
      {
        if (this.Big5Possible && strongDefault)
          return 950;
        if (!this.Utf8Likely)
        {
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
  }
}
