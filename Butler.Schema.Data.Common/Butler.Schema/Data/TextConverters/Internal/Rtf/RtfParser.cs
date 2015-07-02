// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Rtf.RtfParser
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters.Internal.Rtf
{
  internal class RtfParser : RtfParserBase
  {
    private const int UndefinedDefaultFontIndex = 0;
    private Stream pullSource;
    private ConverterStream pushSource;
    private IProgressMonitor progressMonitor;
    private int bytesReadTotal;
    private RtfRunEntry[] runQueue;
    private int runQueueTail;
    private bool overflowRun;
    private RtfToken token;
    private short fontsCount;
    private RtfParser.RtfParserFont[] fonts;
    private short[] fontDirectory;
    private RtfParser.RtfParserState state;
    private short defaultFontHandle;
    private RecognizeInterestingFontName fontNameRecognizer;

    public RtfToken Token => this.token;

      public short CurrentFontIndex => this.state.FontIndex;

      public short CurrentFontSize => this.state.FontSize;

      public bool CurrentFontBold => this.state.Bold;

      public bool CurrentFontItalic => this.state.Italic;

      public bool CurrentRunBiDi => this.state.BiDi;

      public bool CurrentRunComplexScript => this.state.ComplexScript;

      public ushort CurrentCodePage => this.currentCodePage;

      public TextMapping CurrentTextMapping => this.currentTextMapping;

      public RtfSupport.CharRep CurrentCharRep => this.currentCharRep;

      public short CurrentLanguage => this.state.Language;

      public short DefaultLanguage => this.state.DefaultLanguage;

      public short DefaultLanguageFE => this.state.DefaultLanguageFE;

      public RtfParser(Stream input, bool push, int inputBufferSize, bool testBoundaryConditions, IProgressMonitor progressMonitor, IReportBytes reportBytes)
      : base(inputBufferSize, testBoundaryConditions, reportBytes)
    {
      this.progressMonitor = progressMonitor;
      if (push)
        this.pushSource = (ConverterStream) input;
      else
        this.pullSource = input;
      this.runQueue = new RtfRunEntry[testBoundaryConditions ? 15 : 256];
      this.token = new RtfToken(this.ParseBuffer, this.runQueue);
      this.fonts = new RtfParser.RtfParserFont[testBoundaryConditions ? 5 : 16];
      this.fontDirectory = new short[testBoundaryConditions ? 9 : 65];
      this.Initialize();
    }

    internal RtfParser(Stream input, int inputBufferSize, bool testBoundaryConditions, IProgressMonitor progressMonitor, RtfParserBase previewParser, IReportBytes reportBytes)
      : base(inputBufferSize, testBoundaryConditions, previewParser, reportBytes)
    {
      this.progressMonitor = progressMonitor;
      this.pullSource = input;
      this.runQueue = new RtfRunEntry[testBoundaryConditions ? 15 : 256];
      this.token = new RtfToken(this.ParseBuffer, this.runQueue);
      this.fonts = new RtfParser.RtfParserFont[testBoundaryConditions ? 5 : 16];
      this.fontDirectory = new short[testBoundaryConditions ? 9 : 65];
      this.Initialize();
    }

    public void Restart()
    {
      this.InitializeBase();
      this.Initialize();
    }

    public short FontIndex(short fontHandle)
    {
      if ((int) fontHandle < 0)
        return (short) -1;
      if ((int) fontHandle < this.fontDirectory.Length)
        return this.fontDirectory[(int) fontHandle];
      for (int index = (int) this.fontsCount - 1; index >= 0; --index)
      {
        if ((int) this.fonts[index].Handle == (int) fontHandle)
          return (short) index;
      }
      return (short) -1;
    }

    public int FontCodePage(short fontIndex)
    {
      return (int) this.fonts[(int) fontIndex].CodePage;
    }

    public RtfSupport.CharRep FontCharRep(short fontIndex)
    {
      return this.fonts[(int) fontIndex].CharRep;
    }

    public short FontHandle(short fontIndex)
    {
      return this.fonts[(int) fontIndex].Handle;
    }

    public RtfFontFamily FontFamily(short fontIndex)
    {
      return this.fonts[(int) fontIndex].Family;
    }

    public RtfFontPitch FontPitch(short fontIndex)
    {
      return this.fonts[(int) fontIndex].Pitch;
    }

    public RtfTokenId Parse()
    {
      RtfTokenId tokenId = RtfTokenId.None;
      this.runQueueTail = 0;
      if (this.overflowRun)
      {
        tokenId = RtfToken.TokenIdFromRunKind(this.run.Kind);
        this.runQueue[this.runQueueTail++] = this.run;
        this.overflowRun = false;
        if (this.run.Kind == RtfRunKind.Begin)
          this.state.Push();
        else if (this.run.Kind == RtfRunKind.End)
          this.PopState();
        else if (this.run.Kind == RtfRunKind.Keyword && RTFData.keywords[(int) this.run.KeywordId].affectsParsing)
          this.ProcessInterestingKeyword();
        if (tokenId <= RtfTokenId.Binary)
        {
          int length = this.ParseOffset - this.ParseStart;
          this.token.Initialize(tokenId, this.runQueueTail, this.ParseStart, this.ParseOffset - this.ParseStart);
          this.ReportConsumed(length);
          return tokenId;
        }
      }
      if (!this.EndOfFileVisible && this.ParseBufferNeedsRefill && !this.ReadMoreData(true))
      {
        if (this.runQueueTail != 0)
          this.overflowRun = true;
        this.token.Reset();
        return RtfTokenId.None;
      }
      ushort num = (ushort) 0;
      while (this.CanQueueRun() && this.ParseRun())
      {
        RtfTokenId rtfTokenId = RtfToken.TokenIdFromRunKind(this.run.Kind);
        if (tokenId == RtfTokenId.None)
          tokenId = rtfTokenId;
        else if (rtfTokenId != tokenId && rtfTokenId != RtfTokenId.None)
        {
          this.overflowRun = true;
          num = this.run.Length;
          break;
        }
        this.runQueue[this.runQueueTail++] = this.run;
        if (this.run.Kind == RtfRunKind.Begin)
          this.state.Push();
        else if (this.run.Kind == RtfRunKind.End)
          this.PopState();
        else if (this.run.Kind == RtfRunKind.Keyword && RTFData.keywords[(int) this.run.KeywordId].affectsParsing)
          this.ProcessInterestingKeyword();
        if (tokenId <= RtfTokenId.Binary)
          break;
      }
      if (tokenId == RtfTokenId.None)
        tokenId = RtfTokenId.Keywords;
      else if (tokenId == RtfTokenId.Text)
      {
        this.token.SetCodePage((int) this.currentCodePage, this.currentTextMapping);
        if (this.state.Destination == RtfParser.RtfParserDestination.FontTable && !this.fontNameRecognizer.IsRejected)
        {
          int parseStart = this.ParseStart;
          for (int index1 = 0; index1 < this.runQueueTail; ++index1)
          {
            if (this.runQueue[index1].Kind == RtfRunKind.Text)
            {
              for (int index2 = 0; index2 < (int) this.runQueue[index1].Length; ++index2)
                this.fontNameRecognizer.AddCharacter(this.ParseBuffer[parseStart + index2]);
            }
            else if (this.run.Kind != RtfRunKind.Ignore && this.run.Kind != RtfRunKind.Zero)
              this.fontNameRecognizer.AddCharacter(byte.MaxValue);
            parseStart += (int) this.runQueue[index1].Length;
          }
        }
        else if (!this.overflowRun && this.lastLeadByte && this.runQueue[this.runQueueTail - 1].Kind != RtfRunKind.Ignore)
        {
          this.overflowRun = true;
          num = this.run.Length;
          --this.runQueueTail;
        }
      }
      int length1 = this.ParseOffset - (int) num - this.ParseStart;
      this.token.Initialize(tokenId, this.runQueueTail, this.ParseStart, length1);
      this.ReportConsumed(length1);
      return tokenId;
    }

    internal bool ReadMoreData(bool compact)
    {
      int offset;
      int bufferSpace = this.GetBufferSpace(compact, out offset);
      if (bufferSpace == 0)
        return true;
      bool eof;
      int num;
      if (this.pushSource != null)
      {
        byte[] chunkBuffer;
        int chunkOffset;
        int chunkCount;
        if (!this.pushSource.GetInputChunk(out chunkBuffer, out chunkOffset, out chunkCount, out eof))
          return false;
        num = Math.Min(chunkCount, bufferSpace);
        if (num != 0)
        {
          Buffer.BlockCopy((Array) chunkBuffer, chunkOffset, (Array) this.ParseBuffer, offset, num);
          this.pushSource.ReportRead(num);
          this.bytesReadTotal += num;
        }
      }
      else
      {
        num = this.pullSource.Read(this.ParseBuffer, offset, bufferSpace);
        if (num == 0)
        {
          eof = true;
        }
        else
        {
          eof = false;
          this.bytesReadTotal += num;
          this.progressMonitor.ReportProgress();
        }
      }
      this.ReportMoreDataAvailable(num, eof);
      return true;
    }

    private void Initialize()
    {
      this.runQueueTail = 0;
      this.overflowRun = false;
      this.token.Reset();
      this.fonts[0].Initialize((short) -1, (ushort) 1252);
      this.fonts[0].Family = RtfFontFamily.Swiss;
      this.fontsCount = (short) 1;
      for (int index = 0; index < this.fontDirectory.Length; ++index)
        this.fontDirectory[index] = (short) -1;
      this.state.Initialize();
    }

    private bool CanQueueRun()
    {
      return this.runQueueTail < this.runQueue.Length - 1;
    }

    private void PopState()
    {
      if (this.state.Level == 0)
        return;
      short fontIndex = this.state.FontIndex;
      RtfParser.RtfParserDestination destination = this.state.Destination;
      this.state.Pop();
      if (destination == RtfParser.RtfParserDestination.FontTable)
      {
        if (this.state.Destination != RtfParser.RtfParserDestination.FontTable)
        {
          if ((int) fontIndex >= 0)
          {
            this.fonts[(int) fontIndex].TextMapping = this.fontNameRecognizer.TextMapping;
            if (this.fonts[(int) fontIndex].TextMapping == TextMapping.Unicode && this.fonts[(int) fontIndex].CharRep == RtfSupport.CharRep.SYMBOL_INDEX)
              this.fonts[(int) fontIndex].TextMapping = TextMapping.OtherSymbol;
          }
          this.fontNameRecognizer = new RecognizeInterestingFontName();
          this.state.FontIndex = (short) 0;
          this.state.FontIndexAscii = (short) 0;
          if ((int) this.defaultCodePage == 0)
          {
            if ((int) this.state.DefaultLanguageFE != -1 && (int) this.state.DefaultLanguageFE != 1033)
              this.defaultCodePage = RtfSupport.CodePageFromCharRep(RtfSupport.CharRepFromLanguage((int) this.state.DefaultLanguageFE));
            else if ((int) this.state.DefaultLanguage != -1 && (int) this.state.DefaultLanguage != 1033)
              this.defaultCodePage = RtfSupport.CodePageFromCharRep(RtfSupport.CharRepFromLanguage((int) this.state.DefaultLanguage));
          }
          this.SelectCurrentFont();
        }
        else
          this.state.FontIndex = fontIndex;
      }
      this.SetCodePage(this.state.CodePage, this.state.TextMapping);
      this.currentCharRep = this.state.CharRep;
      this.bytesSkipForUnicodeEscape = this.state.BytesSkipForUnicodeEscape;
    }

    private void ProcessInterestingKeyword()
    {
      int num1 = this.run.Value;
      Globalization.Culture culture;
      short fontIndex;
      switch (this.run.KeywordId)
      {
        case (short) 312:
          fontIndex = this.FontIndex((short) num1);
          if ((int) fontIndex == -1)
            fontIndex = this.state.DefaultFontIndex;
          if (!this.state.InAssocFont && this.state.AssociateRunKind != RtfTextRunKind.None)
          {
            if (this.state.AssociateRunKind == RtfTextRunKind.Dbch && !RtfSupport.IsFECharRep(this.FontCharRep(fontIndex)))
              fontIndex = this.state.DefaultFontIndex;
            this.state.AssociateFontIndex = fontIndex;
            this.state.AssociateRunKind = RtfTextRunKind.None;
            return;
          }
          break;
        case (short) 329:
        case (short) 292:
        case (short) 37:
          this.state.SetDetectSingleChpRtl();
          return;
        case (short) 291:
        case (short) 115:
          if (!this.state.DualChpRtfCS)
          {
            this.state.ItalicOther = 0 != num1;
            this.state.ItalicCS = 0 != num1;
          }
          else if (!this.state.FillBiProps)
            this.state.ItalicOther = 0 != num1;
          else
            this.state.ItalicCS = 0 != num1;
          this.state.Italic = 0 != num1;
          return;
        case (short) 307:
        case (short) 109:
          if (!this.state.DualChpRtfCS)
          {
            this.state.BoldOther = 0 != num1;
            this.state.BoldCS = 0 != num1;
          }
          else if (!this.state.FillBiProps)
            this.state.BoldOther = 0 != num1;
          else
            this.state.BoldCS = 0 != num1;
          this.state.Bold = 0 != num1;
          return;
        case (short) 308:
        case (short) 266:
        case (short) 93:
        case (short) 94:
        case (short) 72:
        case (short) 49:
        case (short) 18:
        case (short) 30:
          if (this.state.Destination != RtfParser.RtfParserDestination.FontTable || (int) this.state.FontIndex < 0)
            return;
          this.fonts[(int) this.state.FontIndex].Family = (RtfFontFamily) RTFData.keywords[(int) this.run.KeywordId].idx;
          if ((int) this.run.KeywordId != 266 || this.fonts[(int) this.state.FontIndex].CharRep != RtfSupport.CharRep.DEFAULT_INDEX)
            return;
          this.fonts[(int) this.state.FontIndex].CharRep = RtfSupport.CharRep.SYMBOL_INDEX;
          return;
        case (short) 280:
          if (num1 < 0 || num1 > (int) short.MaxValue)
            return;
          this.defaultFontHandle = (short) num1;
          short num2 = this.FontIndex((short) num1);
          if ((int) num2 != -1)
          {
            this.state.DefaultFontIndex = num2;
            return;
          }
          if ((int) this.fonts[0].Handle != -1)
            return;
          this.fonts[0].Handle = (short) num1;
          if ((int) this.fonts[0].Handle >= this.fontDirectory.Length)
            return;
          this.fontDirectory[(int) this.fonts[0].Handle] = (short) 0;
          return;
        case (short) 281:
        case (short) 238:
        case (short) 71:
          if (num1 <= 0 || num1 > (int) short.MaxValue || !Globalization.Culture.TryGetCulture(num1, out culture))
            return;
          this.state.Language = (short) num1;
          if (!RtfSupport.IsBiDiLanguage(num1))
            return;
          this.state.CharRepBiDi = RtfSupport.CharRepFromLanguage(num1);
          if (this.state.AssociateRunKind == RtfTextRunKind.Ltrch)
            this.state.AssociateRunKind = RtfTextRunKind.Rtlch;
          if (this.state.CharRepBiDi <= RtfSupport.CharRep.NCHARSETS || (int) this.state.FontIndex < 0)
            return;
          this.fonts[(int) this.state.FontIndex].CharRep = this.state.CharRepBiDi;
          return;
        case (short) 287:
        case (short) 164:
        case (short) 134:
          if ((int) this.run.KeywordId == 164)
            this.state.SetFcs(num1 != 0);
          else
            this.state.SetRtlch((int) this.run.KeywordId == 287);
          if (this.state.FillBiProps)
          {
            if ((int) this.state.FontIndexCS != -1)
              this.state.FontIndex = this.state.FontIndexCS;
            if ((int) this.state.FontSizeCS != 0)
              this.state.FontSize = this.state.FontSizeCS;
            this.state.Bold = this.state.BoldCS;
            this.state.Italic = this.state.ItalicCS;
          }
          else
          {
            if ((int) this.state.FontIndexOther != -1)
              this.state.FontIndex = this.state.FontIndexOther;
            if ((int) this.state.FontSizeOther != 0)
              this.state.FontSize = this.state.FontSizeOther;
            this.state.Bold = this.state.BoldOther;
            this.state.Italic = this.state.ItalicOther;
          }
          this.SelectCurrentFont();
          return;
        case (short) 239:
          return;
        case (short) 240:
          if (num1 <= 0 || num1 >= (int) ushort.MaxValue)
            return;
          Encoding encoding;
          if (num1 != 42 && !Globalization.Charset.TryGetEncoding(num1, out encoding))
            num1 = (int) this.defaultCodePage;
          if (!this.state.CodePageFixed)
          {
            this.state.CodePage = (ushort) num1;
            this.state.TextMapping = TextMapping.Unicode;
            if (num1 != 42 || this.state.Destination != RtfParser.RtfParserDestination.FontTable)
              this.SetCodePage((ushort) num1, TextMapping.Unicode);
          }
          if (this.state.Destination != RtfParser.RtfParserDestination.FontTable || (int) this.state.FontIndex < 0)
            return;
          this.fonts[(int) this.state.FontIndex].CodePage = (ushort) num1;
          this.fonts[(int) this.state.FontIndex].CharRep = RtfSupport.CharRepFromCodePage((ushort) num1);
          if ((int) this.defaultCodePage == 0 && num1 != 42)
            this.defaultCodePage = (ushort) num1;
          if (!RtfSupport.IsBiDiCharRep(this.fonts[(int) this.state.FontIndex].CharRep) || RtfSupport.IsBiDiCharRep(this.state.CharRepBiDi))
            return;
          this.state.CharRepBiDi = this.fonts[(int) this.state.FontIndex].CharRep;
          return;
        case (short) 265:
        case (short) 185:
        case (short) 169:
          if (num1 < 0 || num1 > 3276)
            return;
          this.state.FontSizeOther = (short) (num1 * 10);
          this.state.FontSize = this.state.FontSizeOther;
          return;
        case (short) 267:
          return;
        case (short) 268:
          if (!this.run.Lead || this.state.Destination != RtfParser.RtfParserDestination.FontTable || (int) this.state.FontIndex < 0)
            return;
          this.state.Destination = RtfParser.RtfParserDestination.AltFontName;
          return;
        case (short) 269:
          return;
        case (short) 270:
          return;
        case (short) 271:
          if (this.state.Level != 1 || !this.run.Lead)
            return;
          this.state.CodePage = (ushort) 65001;
          this.state.TextMapping = TextMapping.Unicode;
          this.state.CodePageFixed = true;
          this.defaultCodePage = (ushort) 65001;
          this.SetCodePage((ushort) 65001, TextMapping.Unicode);
          return;
        case (short) 214:
        case (short) 60:
          if (!Globalization.Culture.TryGetCulture((int) (short) num1, out culture))
            return;
          this.state.DefaultLanguage = (short) num1;
          return;
        case (short) 222:
        case (short) 137:
        case (short) 95:
          this.state.AssociateRunKind = (RtfTextRunKind) RTFData.keywords[(int) this.run.KeywordId].idx;
          if ((int) this.state.AssociateFontIndex != -1)
            this.state.FontIndex = this.state.AssociateFontIndex;
          if ((int) this.state.FontSizeOther != 0)
            this.state.FontSize = this.state.FontSizeOther;
          this.state.Bold = this.state.BoldOther;
          this.state.Italic = this.state.ItalicOther;
          return;
        case (short) 223:
          fontIndex = this.FontIndex((short) num1);
          if ((int) fontIndex == -1)
          {
            fontIndex = this.state.DefaultFontIndex;
            break;
          }
          break;
        case (short) 210:
          if (!this.run.Lead || this.state.Destination != RtfParser.RtfParserDestination.FontTable || (int) this.state.FontIndex < 0)
            return;
          this.state.Destination = RtfParser.RtfParserDestination.RealFontName;
          return;
        case (short) 149:
          if (!Globalization.Culture.TryGetCulture((int) (short) num1, out culture) || !RtfSupport.IsFECharRep(RtfSupport.CharRepFromLanguage((int) (short) num1)))
            return;
          this.state.DefaultLanguageFE = (short) num1;
          return;
        case (short) 165:
          if (num1 <= 0 || num1 >= (int) ushort.MaxValue || this.state.CodePageFixed)
            return;
          this.defaultCodePage = (ushort) num1;
          this.fonts[0].CodePage = (ushort) num1;
          this.state.CodePage = this.defaultCodePage;
          this.SetCodePage(this.defaultCodePage, TextMapping.Unicode);
          return;
        case (short) 166:
          return;
        case (short) 167:
          return;
        case (short) 168:
          return;
        case (short) 170:
          this.state.SetPlain();
          this.SelectCurrentFont();
          return;
        case (short) 175:
          if (!this.run.Lead || this.state.Destination != RtfParser.RtfParserDestination.Default)
            return;
          this.state.Destination = RtfParser.RtfParserDestination.FontTable;
          this.state.FontIndex = (short) -1;
          if ((int) this.defaultCodePage == 0 || this.state.CodePageFixed)
            return;
          this.SetCodePage(this.defaultCodePage, TextMapping.Unicode);
          return;
        case (short) 113:
          if (this.state.Destination != RtfParser.RtfParserDestination.FontTable)
          {
            fontIndex = this.FontIndex((short) num1);
            if ((int) fontIndex == -1)
            {
              fontIndex = this.state.DefaultFontIndex;
              break;
            }
            break;
          }
          if ((int) this.state.FontIndex >= 0)
          {
            this.fonts[(int) this.state.FontIndex].TextMapping = this.fontNameRecognizer.TextMapping;
            if (this.fonts[(int) this.state.FontIndex].TextMapping == TextMapping.Unicode && this.fonts[(int) this.state.FontIndex].CharRep == RtfSupport.CharRep.SYMBOL_INDEX)
              this.fonts[(int) this.state.FontIndex].TextMapping = TextMapping.OtherSymbol;
          }
          this.fontNameRecognizer = new RecognizeInterestingFontName();
          if (num1 < 0 || num1 > (int) short.MaxValue)
            return;
          this.state.FontIndex = this.FontIndex((short) num1);
          if ((int) this.state.FontIndex >= 0)
            return;
          if ((int) this.fontsCount == this.fonts.Length)
          {
            if ((int) this.fontsCount >= 2048)
              throw new TextConvertersException(CtsResources.TextConvertersStrings.InputDocumentTooComplex);
            RtfParser.RtfParserFont[] rtfParserFontArray = new RtfParser.RtfParserFont[this.fonts.Length * 2];
            Array.Copy((Array) this.fonts, (Array) rtfParserFontArray, (int) this.fontsCount);
            this.fonts = rtfParserFontArray;
          }
          if ((int) this.defaultFontHandle == (int) (short) num1)
            this.state.DefaultFontIndex = this.fontsCount;
          this.state.FontIndex = this.fontsCount++;
          this.fonts[(int) this.state.FontIndex].Initialize((short) num1, this.defaultCodePage);
          this.fonts[(int) this.state.FontIndex].Handle = (short) num1;
          if ((int) this.fonts[(int) this.state.FontIndex].Handle >= this.fontDirectory.Length)
            return;
          this.fontDirectory[(int) this.fonts[(int) this.state.FontIndex].Handle] = this.state.FontIndex;
          return;
        case (short) 114:
          return;
        case (short) 133:
          return;
        case (short) 135:
          return;
        case (short) 136:
          return;
        case (short) 81:
          if (0 > num1 || num1 > 2 || (int) this.state.BytesSkipForUnicodeEscape == num1)
            return;
          this.state.BytesSkipForUnicodeEscape = this.bytesSkipForUnicodeEscape = (byte) num1;
          return;
        case (short) 42:
          if (this.state.Destination != RtfParser.RtfParserDestination.FontTable || (int) this.state.FontIndex < 0 || (num1 < 0 || num1 > 2))
            return;
          this.fonts[(int) this.state.FontIndex].Pitch = (RtfFontPitch) num1;
          return;
        case (short) 34:
          if (this.state.Destination != RtfParser.RtfParserDestination.FontTable || (int) this.state.FontIndex < 0)
            return;
          this.fonts[(int) this.state.FontIndex].CharRep = RtfSupport.CharRepFromCharSet(num1);
          this.fonts[(int) this.state.FontIndex].CodePage = RtfSupport.CodePageFromCharRep(this.fonts[(int) this.state.FontIndex].CharRep);
          if (!this.state.CodePageFixed && (int) this.fonts[(int) this.state.FontIndex].CodePage != 42)
          {
            this.state.CodePage = this.fonts[(int) this.state.FontIndex].CodePage;
            this.state.TextMapping = TextMapping.Unicode;
            if ((int) this.defaultCodePage == 0)
              this.defaultCodePage = this.state.CodePage;
            this.SetCodePage(this.state.CodePage, this.state.TextMapping);
          }
          if (!RtfSupport.IsRtlCharSet(num1) || RtfSupport.IsBiDiCharRep(this.state.CharRepBiDi))
            return;
          this.state.CharRepBiDi = this.fonts[(int) this.state.FontIndex].CharRep;
          return;
        case (short) 1:
          if (!this.run.Lead || this.state.Destination != RtfParser.RtfParserDestination.FontTable)
            return;
          this.state.Destination = RtfParser.RtfParserDestination.IgnorableDestinationInFontTable;
          return;
        default:
          return;
      }
      if (this.state.AssociateRunKind != RtfTextRunKind.None)
      {
        if (this.state.AssociateRunKind == RtfTextRunKind.Dbch && !RtfSupport.IsFECharRep(this.FontCharRep(fontIndex)))
          fontIndex = this.state.DefaultFontIndex;
        this.state.AssociateFontIndex = fontIndex;
        if (!this.state.DualChpRtfCS)
          this.state.FontIndexCS = fontIndex;
        this.state.AssociateRunKind = RtfTextRunKind.None;
      }
      else if (this.state.DualChpRtfCS)
      {
        if (this.state.FillBiProps)
        {
          this.state.FontIndexCS = fontIndex;
        }
        else
        {
          this.state.FontIndexAscii = fontIndex;
          if (!RtfSupport.IsFECharRep(this.FontCharRep(fontIndex)))
            this.state.FontIndexOther = fontIndex;
          else
            this.state.FontIndexDbCh = fontIndex;
        }
      }
      else
      {
        if (RtfSupport.IsBiDiCharRep(this.FontCharRep(fontIndex)))
          this.state.FontIndexCS = fontIndex;
        this.state.FontIndexAscii = fontIndex;
        if (!RtfSupport.IsFECharRep(this.FontCharRep(fontIndex)))
          this.state.FontIndexOther = fontIndex;
        else
          this.state.FontIndexDbCh = fontIndex;
      }
      this.state.FontIndex = fontIndex;
      this.SelectCurrentFont();
    }

    private void SelectCurrentFont()
    {
      if ((int) this.state.FontIndex == -1)
        this.state.FontIndex = this.state.DefaultFontIndex;
      if (this.state.Destination != RtfParser.RtfParserDestination.FontTable)
      {
        this.state.CharRep = this.currentCharRep = this.fonts[(int) this.state.FontIndex].CharRep;
        if (this.state.CharRep == RtfSupport.CharRep.DEFAULT_INDEX && (int) this.state.Language != -1)
        {
          RtfSupport.CharRep charRep = RtfSupport.CharRepFromLanguage((int) this.state.Language);
          if (charRep > RtfSupport.CharRep.NCHARSETS)
          {
            this.state.CharRep = this.currentCharRep = charRep;
            if (charRep == this.state.CharRepBiDi)
              this.state.FontIndexCS = this.state.FontIndex;
          }
        }
        if (RtfSupport.IsBiDiCharRep(this.state.CharRep) && (int) this.fonts[(int) this.state.FontIndex].CodePage == 1252)
          this.fonts[(int) this.state.FontIndex].CodePage = RtfSupport.CodePageFromCharRep(this.state.CharRep);
      }
      if (this.state.CodePageFixed)
        return;
      if ((int) this.fonts[(int) this.state.FontIndex].CodePage == 0)
      {
        this.state.CodePage = this.defaultCodePage;
        this.state.TextMapping = TextMapping.Unicode;
      }
      else
      {
        this.state.CodePage = this.fonts[(int) this.state.FontIndex].CodePage;
        this.state.TextMapping = this.fonts[(int) this.state.FontIndex].TextMapping;
      }
      if ((int) this.state.CodePage <= 0)
      {
        this.state.CodePage = (ushort) 1252;
        this.state.TextMapping = TextMapping.Unicode;
      }
      else if ((int) this.state.CodePage == 1255)
      {
        if (!RtfSupport.IsHebrewLanguage((int) this.state.Language))
          this.state.Language = (short) 13;
      }
      else if ((int) this.state.CodePage == 1256)
      {
        if (!RtfSupport.IsArabicLanguage((int) this.state.Language))
          this.state.Language = (short) 1;
      }
      else if ((int) this.state.CodePage == 874 && !RtfSupport.IsThaiLanguage((int) this.state.Language))
        this.state.Language = (short) 30;
      if (this.state.DualChpRtfCS && this.state.InAssocFont)
        return;
      this.SetCodePage(this.state.CodePage, this.state.TextMapping);
    }

    internal enum RtfParserDestination : byte
    {
      Default,
      FontTable,
      RealFontName,
      AltFontName,
      IgnorableDestinationInFontTable,
    }

    internal struct RtfParserState
    {
      public int Level;
      public RtfParser.RtfParserState.State Current;
      private RtfParser.RtfParserState.State[] stack;
      private int stackTop;
      private bool codePageFixed;
      private short defaultFontIndex;
      private short defaultLanguage;
      private short defaultLanguageFE;
      private RtfSupport.CharRep charRepBiDi;
      private bool detectSingleChpRtl;
      private bool detectAssocRtl;
      private bool detectAssocSA;
      private bool inAssocRtl;
      private bool inAssocSA;

      public bool DualChpRtfCS
      {
        get
        {
          if (this.detectAssocSA)
            return true;
          if (this.detectAssocRtl)
            return !this.detectSingleChpRtl;
          return false;
        }
      }

      public bool FillBiProps
      {
        get
        {
          if (this.BiDi && this.ComplexScript || this.BiDi && !this.inAssocSA)
            return true;
          if (this.ComplexScript)
            return !this.inAssocRtl;
          return false;
        }
      }

      public bool InAssocFont
      {
        get
        {
          if (!this.inAssocRtl)
            return this.inAssocSA;
          return true;
        }
      }

      public bool CodePageFixed
      {
        get
        {
          return this.codePageFixed;
        }
        set
        {
          this.codePageFixed = value;
        }
      }

      public short DefaultFontIndex
      {
        get
        {
          return this.defaultFontIndex;
        }
        set
        {
          this.defaultFontIndex = value;
        }
      }

      public short DefaultLanguage
      {
        get
        {
          return this.defaultLanguage;
        }
        set
        {
          this.defaultLanguage = value;
        }
      }

      public short DefaultLanguageFE
      {
        get
        {
          return this.defaultLanguageFE;
        }
        set
        {
          this.defaultLanguageFE = value;
        }
      }

      public RtfSupport.CharRep CharRepBiDi
      {
        get
        {
          return this.charRepBiDi;
        }
        set
        {
          this.charRepBiDi = value;
        }
      }

      public RtfParser.RtfParserDestination Destination
      {
        get
        {
          return this.Current.Dest;
        }
        set
        {
          if (this.Current.Dest == value)
            return;
          this.SetDirty();
          this.Current.Dest = value;
        }
      }

      public bool BiDi
      {
        get
        {
          return (this.Current.Flags & RtfParser.RtfParserState.StateFlags.BiDi) != RtfParser.RtfParserState.StateFlags.None;
        }
        set
        {
          if (this.BiDi == value)
            return;
          this.SetDirty();
          this.Current.Flags ^= RtfParser.RtfParserState.StateFlags.BiDi;
        }
      }

      public bool ComplexScript
      {
        get
        {
          return (this.Current.Flags & RtfParser.RtfParserState.StateFlags.ComplexScript) != RtfParser.RtfParserState.StateFlags.None;
        }
        set
        {
          if (this.ComplexScript == value)
            return;
          this.SetDirty();
          this.Current.Flags ^= RtfParser.RtfParserState.StateFlags.ComplexScript;
        }
      }

      public bool Bold
      {
        get
        {
          return (this.Current.Flags & RtfParser.RtfParserState.StateFlags.Bold) != RtfParser.RtfParserState.StateFlags.None;
        }
        set
        {
          if (this.Bold == value)
            return;
          this.SetDirty();
          this.Current.Flags ^= RtfParser.RtfParserState.StateFlags.Bold;
        }
      }

      public bool BoldCS
      {
        get
        {
          return (this.Current.Flags & RtfParser.RtfParserState.StateFlags.BoldCS) != RtfParser.RtfParserState.StateFlags.None;
        }
        set
        {
          if (this.BoldCS == value)
            return;
          this.SetDirty();
          this.Current.Flags ^= RtfParser.RtfParserState.StateFlags.BoldCS;
        }
      }

      public bool BoldOther
      {
        get
        {
          return (this.Current.Flags & RtfParser.RtfParserState.StateFlags.BoldOther) != RtfParser.RtfParserState.StateFlags.None;
        }
        set
        {
          if (this.BoldOther == value)
            return;
          this.SetDirty();
          this.Current.Flags ^= RtfParser.RtfParserState.StateFlags.BoldOther;
        }
      }

      public bool Italic
      {
        get
        {
          return (this.Current.Flags & RtfParser.RtfParserState.StateFlags.Italic) != RtfParser.RtfParserState.StateFlags.None;
        }
        set
        {
          if (this.Italic == value)
            return;
          this.SetDirty();
          this.Current.Flags ^= RtfParser.RtfParserState.StateFlags.Italic;
        }
      }

      public bool ItalicCS
      {
        get
        {
          return (this.Current.Flags & RtfParser.RtfParserState.StateFlags.ItalicCS) != RtfParser.RtfParserState.StateFlags.None;
        }
        set
        {
          if (this.ItalicCS == value)
            return;
          this.SetDirty();
          this.Current.Flags ^= RtfParser.RtfParserState.StateFlags.ItalicCS;
        }
      }

      public bool ItalicOther
      {
        get
        {
          return (this.Current.Flags & RtfParser.RtfParserState.StateFlags.ItalicOther) != RtfParser.RtfParserState.StateFlags.None;
        }
        set
        {
          if (this.ItalicOther == value)
            return;
          this.SetDirty();
          this.Current.Flags ^= RtfParser.RtfParserState.StateFlags.ItalicOther;
        }
      }

      public byte BytesSkipForUnicodeEscape
      {
        get
        {
          return this.Current.BytesSkipForUnicodeEscape;
        }
        set
        {
          if ((int) this.Current.BytesSkipForUnicodeEscape == (int) value)
            return;
          this.SetDirty();
          this.Current.BytesSkipForUnicodeEscape = value;
        }
      }

      public ushort CodePage
      {
        get
        {
          return this.Current.CodePage;
        }
        set
        {
          if ((int) this.Current.CodePage == (int) value)
            return;
          this.SetDirty();
          this.Current.CodePage = value;
        }
      }

      public TextMapping TextMapping
      {
        get
        {
          return this.Current.TextMapping;
        }
        set
        {
          if (this.Current.TextMapping == value)
            return;
          this.SetDirty();
          this.Current.TextMapping = value;
        }
      }

      public RtfSupport.CharRep CharRep
      {
        get
        {
          return this.Current.CharRep;
        }
        set
        {
          if (this.Current.CharRep == value)
            return;
          this.SetDirty();
          this.Current.CharRep = value;
        }
      }

      public short Language
      {
        get
        {
          return this.Current.Language;
        }
        set
        {
          if ((int) this.Current.Language == (int) value)
            return;
          this.SetDirty();
          this.Current.Language = value;
        }
      }

      public short FontIndex
      {
        get
        {
          return this.Current.FontIndex;
        }
        set
        {
          if ((int) this.Current.FontIndex == (int) value)
            return;
          this.SetDirty();
          this.Current.FontIndex = value;
        }
      }

      public short FontIndexAscii
      {
        get
        {
          return this.Current.FontIndexAscii;
        }
        set
        {
          if ((int) this.Current.FontIndexAscii == (int) value)
            return;
          this.SetDirty();
          this.Current.FontIndexAscii = value;
        }
      }

      public short FontIndexDbCh
      {
        get
        {
          return this.Current.FontIndexDbCh;
        }
        set
        {
          if ((int) this.Current.FontIndexDbCh == (int) value)
            return;
          this.SetDirty();
          this.Current.FontIndexDbCh = value;
        }
      }

      public short FontIndexCS
      {
        get
        {
          return this.Current.FontIndexCS;
        }
        set
        {
          if ((int) this.Current.FontIndexCS == (int) value)
            return;
          this.SetDirty();
          this.Current.FontIndexCS = value;
        }
      }

      public short FontIndexOther
      {
        get
        {
          return this.Current.FontIndexOther;
        }
        set
        {
          if ((int) this.Current.FontIndexOther == (int) value)
            return;
          this.SetDirty();
          this.Current.FontIndexOther = value;
        }
      }

      public short FontSize
      {
        get
        {
          return this.Current.FontSize;
        }
        set
        {
          if ((int) this.Current.FontSize == (int) value)
            return;
          this.SetDirty();
          this.Current.FontSize = value;
        }
      }

      public short FontSizeCS
      {
        get
        {
          return this.Current.FontSizeCS;
        }
        set
        {
          if ((int) this.Current.FontSizeCS == (int) value)
            return;
          this.SetDirty();
          this.Current.FontSizeCS = value;
        }
      }

      public short FontSizeOther
      {
        get
        {
          return this.Current.FontSizeOther;
        }
        set
        {
          if ((int) this.Current.FontSizeOther == (int) value)
            return;
          this.SetDirty();
          this.Current.FontSizeOther = value;
        }
      }

      public RtfTextRunKind AssociateRunKind
      {
        get
        {
          return this.Current.RunKind;
        }
        set
        {
          if (this.Current.RunKind == value)
            return;
          this.SetDirty();
          this.Current.RunKind = value;
        }
      }

      public short AssociateFontIndex
      {
        get
        {
          if (this.Current.RunKind == RtfTextRunKind.None)
            return this.Current.FontIndex;
          if (this.Current.RunKind == RtfTextRunKind.Dbch)
            return this.Current.FontIndexDbCh;
          if (this.Current.RunKind == RtfTextRunKind.Loch)
            return this.Current.FontIndexAscii;
          return this.Current.FontIndexOther;
        }
        set
        {
          if (this.Current.RunKind == RtfTextRunKind.None)
          {
            if ((int) this.Current.FontIndex == (int) value)
              return;
            this.SetDirty();
            this.Current.FontIndex = value;
          }
          else if (this.Current.RunKind == RtfTextRunKind.Dbch)
          {
            if ((int) this.Current.FontIndexDbCh == (int) value)
              return;
            this.SetDirty();
            this.Current.FontIndexDbCh = value;
          }
          else if (this.Current.RunKind == RtfTextRunKind.Loch)
          {
            if ((int) this.Current.FontIndexAscii == (int) value)
              return;
            this.SetDirty();
            this.Current.FontIndexAscii = value;
          }
          else
          {
            if ((int) this.Current.FontIndexOther == (int) value)
              return;
            this.SetDirty();
            this.Current.FontIndexOther = value;
          }
        }
      }

      public void Initialize()
      {
        this.Level = 0;
        this.stackTop = 0;
        this.defaultFontIndex = (short) 0;
        this.defaultLanguage = (short) -1;
        this.defaultLanguageFE = (short) -1;
        this.charRepBiDi = RtfSupport.CharRep.ANSI_INDEX;
        this.codePageFixed = false;
        this.Current.Depth = (short) 1;
        this.Current.Dest = RtfParser.RtfParserDestination.Default;
        this.Current.BytesSkipForUnicodeEscape = (byte) 1;
        this.SetPlain();
      }

      public void SetDetectSingleChpRtl()
      {
        this.detectSingleChpRtl = true;
      }

      public void SetRtlch(bool value)
      {
        this.detectAssocRtl = true;
        this.inAssocRtl = !this.inAssocRtl;
        this.BiDi = value;
      }

      public void SetFcs(bool value)
      {
        this.detectAssocSA = true;
        this.inAssocSA = !this.inAssocSA;
        this.ComplexScript = value;
      }

      public void SetPlain()
      {
        this.SetDirty();
        this.Current.Flags = RtfParser.RtfParserState.StateFlags.None;
        this.Current.RunKind = RtfTextRunKind.None;
        this.Current.FontIndex = this.defaultFontIndex;
        this.Current.FontIndexCS = (short) -1;
        this.Current.FontIndexAscii = (short) -1;
        this.Current.FontIndexDbCh = (short) -1;
        this.Current.FontIndexOther = this.defaultFontIndex;
        this.Current.FontSize = RtfParserBase.TwelvePointsInTwips;
        this.Current.FontSizeCS = (short) 0;
        this.Current.FontSizeOther = (short) 0;
        this.Current.CodePage = (ushort) 0;
        this.Current.TextMapping = TextMapping.Unicode;
        this.Current.Language = (int) this.defaultLanguageFE != -1 ? this.defaultLanguageFE : this.defaultLanguage;
        this.Current.CharRep = RtfSupport.CharRep.DEFAULT_INDEX;
      }

      public void Push()
      {
        ++this.Level;
        ++this.Current.Depth;
        if ((int) this.Current.Depth != (int) short.MaxValue)
          return;
        this.PushReally();
      }

      public void SetDirty()
      {
        if ((int) this.Current.Depth <= 1)
          return;
        this.PushReally();
      }

      public void Pop()
      {
        if (this.Level <= 1)
          return;
        --this.Current.Depth;
        if ((int) this.Current.Depth == 0 && this.stackTop != 0)
          this.Current = this.stack[--this.stackTop];
        --this.Level;
      }

      private void PushReally()
      {
        if (this.Level >= 4096)
          throw new TextConvertersException(CtsResources.TextConvertersStrings.InputDocumentTooComplex);
        if (this.stack == null || this.stackTop == this.stack.Length)
        {
          RtfParser.RtfParserState.State[] stateArray;
          if (this.stack != null)
          {
            stateArray = new RtfParser.RtfParserState.State[this.stackTop * 2];
            Array.Copy((Array) this.stack, 0, (Array) stateArray, 0, this.stackTop);
          }
          else
            stateArray = new RtfParser.RtfParserState.State[8];
          this.stack = stateArray;
        }
        this.stack[this.stackTop] = this.Current;
        --this.stack[this.stackTop].Depth;
        ++this.stackTop;
        this.Current.Depth = (short) 1;
      }

      internal enum StateFlagsIndex : byte
      {
        BiDi,
        ComplexScript,
        Bold,
        BoldCS,
        BoldOther,
        Italic,
        ItalicCS,
        ItalicOther,
      }

      [Flags]
      internal enum StateFlags : byte
      {
        None = (byte) 0,
        BiDi = (byte) 1,
        ComplexScript = (byte) 2,
        Bold = (byte) 4,
        BoldCS = (byte) 8,
        BoldOther = (byte) 16,
        Italic = (byte) 32,
        ItalicCS = (byte) 64,
        ItalicOther = (byte) 128,
      }

      public struct State
      {
        public short Depth;
        public RtfParser.RtfParserState.StateFlags Flags;
        public RtfParser.RtfParserDestination Dest;
        public byte BytesSkipForUnicodeEscape;
        public RtfSupport.CharRep CharRep;
        public ushort CodePage;
        public TextMapping TextMapping;
        public short Language;
        public short FontIndex;
        public short FontIndexAscii;
        public short FontIndexDbCh;
        public short FontIndexCS;
        public short FontIndexOther;
        public short FontSize;
        public short FontSizeCS;
        public short FontSizeOther;
        public RtfTextRunKind RunKind;
      }
    }

    internal struct RtfParserFont
    {
      public short Handle;
      public ushort CodePage;
      public RtfSupport.CharRep CharRep;
      public RtfFontFamily Family;
      public RtfFontPitch Pitch;
      public TextMapping TextMapping;

      public void Initialize(short handle, ushort cpid)
      {
        this.Handle = handle;
        this.CodePage = cpid;
        this.CharRep = RtfSupport.CharRep.DEFAULT_INDEX;
        this.Family = RtfFontFamily.Default;
        this.Pitch = RtfFontPitch.Default;
        this.TextMapping = TextMapping.Unicode;
      }
    }
  }
}
