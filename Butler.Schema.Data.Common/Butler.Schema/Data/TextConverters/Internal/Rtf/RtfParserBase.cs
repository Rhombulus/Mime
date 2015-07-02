// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Rtf.RtfParserBase
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Rtf
{
  internal class RtfParserBase
  {
    protected static readonly short TwelvePointsInTwips = (short) 240;
    public const int ReadThreshold = 128;
    protected RtfRunEntry run;
    protected ushort defaultCodePage;
    protected ushort currentCodePage;
    protected TextMapping currentTextMapping;
    protected RtfSupport.CharRep currentCharRep;
    protected bool lastLeadByte;
    protected byte bytesSkipForUnicodeEscape;
    private byte[] parseBuffer;
    private int parseStart;
    private int parseOffset;
    private int parseEnd;
    private bool endOfFileVisible;
    private bool firstKeyword;
    private int bytesToSkip;
    private int binLength;
    private DbcsLeadBits leadMask;
    private IReportBytes reportBytes;

    public byte[] ParseBuffer => this.parseBuffer;

      public int ParseStart => this.parseStart;

      public int ParseOffset => this.parseOffset;

      public int ParseEnd => this.parseEnd;

      public bool ParseBufferFull => this.parseEnd + 128 >= this.parseBuffer.Length;

      public bool EndOfFile
    {
      get
      {
        if (this.endOfFileVisible)
          return this.parseOffset == this.parseEnd;
        return false;
      }
    }

    public bool EndOfFileVisible => this.endOfFileVisible;

      public bool ParseBufferNeedsRefill => this.parseEnd - this.parseOffset < 128;

      public RtfRunKind RunKind => this.run.Kind;

      public short KeywordId => this.run.KeywordId;

      public int KeywordValue => this.run.Value;

      public RtfParserBase(int inputBufferSize, bool testBoundaryConditions, IReportBytes reportBytes)
    {
      this.parseBuffer = new byte[1 + (testBoundaryConditions ? 133 : Math.Min((int) short.MaxValue, inputBufferSize))];
      this.reportBytes = reportBytes;
      this.InitializeBase();
    }

    public RtfParserBase(int inputBufferSize, bool testBoundaryConditions, RtfParserBase previewParser, IReportBytes reportBytes)
    {
      int num = 1 + (testBoundaryConditions ? 133 : Math.Min((int) short.MaxValue, inputBufferSize));
      if (previewParser.ParseBuffer.Length < num)
      {
        this.parseBuffer = new byte[1 + (testBoundaryConditions ? 133 : Math.Min((int) short.MaxValue, inputBufferSize))];
        Buffer.BlockCopy((Array) previewParser.ParseBuffer, 0, (Array) this.parseBuffer, 0, previewParser.ParseEnd);
      }
      else
        this.parseBuffer = previewParser.ParseBuffer;
      this.parseEnd = previewParser.ParseEnd;
      this.endOfFileVisible = previewParser.EndOfFileVisible;
      this.reportBytes = reportBytes;
      this.InitializeBase();
    }

    public int GetBufferSpace(bool compact, out int offset)
    {
      if (compact && this.parseStart != 0 && (this.parseEnd - this.parseStart < 128 || this.parseEnd + 128 > this.parseBuffer.Length - 1))
      {
        if (this.parseEnd != this.parseStart)
          Buffer.BlockCopy((Array) this.parseBuffer, this.parseStart, (Array) this.parseBuffer, 0, this.parseEnd - this.parseStart);
        this.parseOffset -= this.parseStart;
        this.parseEnd -= this.parseStart;
        this.parseStart = 0;
      }
      offset = this.parseEnd;
      return this.parseBuffer.Length - 1 - this.parseEnd;
    }

    public void ReportMoreDataAvailable(int length, bool endOfFileVisible)
    {
      this.parseEnd += length;
      this.parseBuffer[this.parseEnd] = (byte) 0;
      this.endOfFileVisible = endOfFileVisible;
      if (this.reportBytes == null)
        return;
      this.reportBytes.ReportBytesRead(length);
    }

    public bool ParseRun()
    {
      if (this.parseEnd == this.parseOffset)
      {
        if (!this.endOfFileVisible)
          return false;
        this.run.Initialize(RtfRunKind.EndOfFile, 0, 0);
        return true;
      }
      if (this.binLength != 0)
      {
        int length = Math.Min(this.parseEnd - this.parseOffset, this.binLength);
        this.run.Initialize(RtfRunKind.Binary, length, 0, 0 != this.bytesToSkip, false);
        this.binLength -= length;
        this.parseOffset += length;
        if (this.binLength == 0 && this.bytesToSkip != 0)
          --this.bytesToSkip;
        return true;
      }
      int index = this.parseOffset;
      switch (this.parseBuffer[index])
      {
        case (byte) 9:
          if (!this.lastLeadByte)
          {
            this.run.InitializeKeyword((short) 126, 0, 1, this.SkipIfNecessary(1), this.firstKeyword);
            ++this.parseOffset;
            this.firstKeyword = false;
            return true;
          }
          break;
        case (byte) 10:
        case (byte) 13:
          byte num;
          do
          {
            num = this.parseBuffer[++index];
          }
          while ((int) num == 13 || (int) num == 10);
          this.run.Initialize(RtfRunKind.Ignore, index - this.parseOffset, 0, false, false);
          this.parseOffset = index;
          return true;
        case (byte) 92:
          if (!this.ParseKeywordRun())
            return false;
          this.firstKeyword = false;
          return true;
        case (byte) 123:
          if (!this.lastLeadByte)
          {
            this.run.Initialize(RtfRunKind.Begin, 1, 0);
            ++this.parseOffset;
            this.firstKeyword = true;
            this.bytesToSkip = 0;
            return true;
          }
          break;
        case (byte) 125:
          if (!this.lastLeadByte)
          {
            this.run.Initialize(RtfRunKind.End, 1, 0);
            ++this.parseOffset;
            this.firstKeyword = false;
            this.bytesToSkip = 0;
            return true;
          }
          break;
      }
      this.EnsureCodePage();
      this.firstKeyword = false;
      return this.ParseTextRun();
    }

    protected void ReportConsumed(int length)
    {
      this.parseStart += length;
    }

    protected void InitializeBase()
    {
      this.parseStart = 0;
      this.parseOffset = 0;
      this.bytesSkipForUnicodeEscape = (byte) 1;
      this.firstKeyword = false;
      this.bytesToSkip = 0;
      this.binLength = 0;
      this.defaultCodePage = (ushort) 0;
      this.currentCodePage = (ushort) 0;
      this.currentTextMapping = TextMapping.Unicode;
      this.leadMask = (DbcsLeadBits) 0;
      this.lastLeadByte = false;
      this.currentCharRep = RtfSupport.CharRep.DEFAULT_INDEX;
      this.run.Reset();
    }

    protected void SetCodePage(ushort codePage, TextMapping textMapping)
    {
      if ((int) codePage != (int) this.currentCodePage)
      {
        this.currentCodePage = codePage;
        this.leadMask = ParseSupport.GetCodePageLeadMask((int) codePage);
      }
      if (textMapping == this.currentTextMapping)
        return;
      this.currentTextMapping = textMapping;
    }

    private void EnsureCodePage()
    {
      if ((int) this.currentCodePage != 0)
        return;
      this.SetCodePage((int) this.defaultCodePage == 0 ? (ushort) 1252 : this.defaultCodePage, TextMapping.Unicode);
    }

    private bool ParseKeywordRun()
    {
      if (1 == this.parseEnd - this.parseOffset)
      {
        if (!this.endOfFileVisible)
          return false;
        this.run.Initialize(RtfRunKind.Text, 1, 0, this.SkipIfNecessary(1), false);
        ++this.parseOffset;
        return true;
      }
      byte ch = this.parseBuffer[this.parseOffset + 1];
      int unescaped1;
      int unescaped2;
      switch ((char) ch)
      {
        case '_':
          unescaped2 = 8209;
          goto label_13;
        case '{':
        case '}':
        case '\\':
          unescaped1 = (int) ch;
          break;
        case '|':
          this.lastLeadByte = false;
          this.run.InitializeKeyword((short) 2, 0, 2, this.SkipIfNecessary(1), this.firstKeyword);
          this.parseOffset += 2;
          return true;
        case '~':
          unescaped2 = 160;
          goto label_13;
        case ':':
          this.lastLeadByte = false;
          this.run.InitializeKeyword((short) 3, 0, 2, this.SkipIfNecessary(1), this.firstKeyword);
          this.parseOffset += 2;
          return true;
        case '*':
          this.lastLeadByte = false;
          this.run.InitializeKeyword((short) 1, 0, 2, this.SkipIfNecessary(1), this.firstKeyword);
          this.parseOffset += 2;
          return true;
        case '-':
          unescaped2 = 173;
          goto label_13;
        case '\t':
          this.lastLeadByte = false;
          this.run.InitializeKeyword((short) 126, 0, 2, this.SkipIfNecessary(1), this.firstKeyword);
          this.parseOffset += 2;
          return true;
        case '\n':
        case '\r':
          this.lastLeadByte = false;
          this.run.InitializeKeyword((short) 68, 0, 2, this.SkipIfNecessary(1), this.firstKeyword);
          this.parseOffset += 2;
          return true;
        case '\'':
          this.EnsureCodePage();
          if (this.parseEnd - this.parseOffset < 4)
          {
            if (!this.endOfFileVisible)
              return false;
            this.run.Initialize(RtfRunKind.Text, 1, 0, this.SkipIfNecessary(1), false);
            ++this.parseOffset;
            this.lastLeadByte = false;
            return true;
          }
          int unescaped3 = RtfSupport.Unescape(this.parseBuffer[this.parseOffset + 2], this.parseBuffer[this.parseOffset + 3]);
          if (unescaped3 > (int) byte.MaxValue)
          {
            if (this.lastLeadByte)
            {
              this.lastLeadByte = false;
              this.run.Initialize(RtfRunKind.Text, 1, 0, this.SkipIfNecessary(1), false);
              ++this.parseOffset;
              return true;
            }
            unescaped3 = 63;
          }
          else
          {
            if ((unescaped3 == 13 || unescaped3 == 10) && !this.lastLeadByte)
            {
              this.run.InitializeKeyword((short) 68, 0, 4, this.SkipIfNecessary(1), this.firstKeyword);
              this.parseOffset += 4;
              return true;
            }
            if (unescaped3 == 0)
              unescaped3 = 32;
            this.lastLeadByte = !this.lastLeadByte && ParseSupport.IsLeadByte((byte) unescaped3, this.leadMask);
          }
          this.run.Initialize(RtfRunKind.Escape, 4, unescaped3, this.SkipIfNecessary(1), this.lastLeadByte);
          this.parseOffset += 4;
          return true;
        default:
          if (!ParseSupport.AlphaCharacter(ParseSupport.GetCharClass(ch)))
          {
            unescaped1 = (int) ch;
            if (unescaped1 == 0)
            {
              unescaped1 = 32;
              break;
            }
            break;
          }
          this.lastLeadByte = false;
          return this.ParseKeyword(ch);
      }
      this.EnsureCodePage();
      this.lastLeadByte = !this.lastLeadByte && ParseSupport.IsLeadByte((byte) unescaped1, this.leadMask);
      this.run.Initialize(RtfRunKind.Escape, 2, unescaped1, this.SkipIfNecessary(1), this.lastLeadByte);
      this.parseOffset += 2;
      return true;
label_13:
      this.EnsureCodePage();
      this.lastLeadByte = false;
      this.run.Initialize(RtfRunKind.Unicode, 2, unescaped2, this.SkipIfNecessary(1), false);
      this.parseOffset += 2;
      return true;
    }

    private bool ParseKeyword(byte ch)
    {
      int index = this.parseOffset + 1;
      short hash = (short) 0;
      do
      {
        hash = RTFData.AddHash(hash, ch);
        ch = this.parseBuffer[++index];
      }
      while ((uint) (((int) ch | 32) - 97) <= 25U);
      int num1 = index - (this.parseOffset + 1);
      bool flag1 = false;
      bool flag2 = false;
      int unescaped = 0;
      if ((int) ch == 45)
      {
        flag1 = true;
        flag2 = true;
        ++index;
        ch = this.parseBuffer[index];
      }
      if ((uint) ch - 48U <= 9U)
      {
        flag1 = true;
        do
        {
          unescaped = unescaped * 10 + ((int) ch - 48);
          ch = this.parseBuffer[++index];
        }
        while ((uint) ch - 48U <= 9U);
        if (flag2)
          unescaped = -unescaped;
      }
      if (index > this.parseOffset + 128 - 1)
      {
        index = this.parseOffset + 128 - 1;
        ch = this.parseBuffer[index];
        unescaped = 0;
        num1 = Math.Min(num1, index - (this.parseOffset + 1));
      }
      if ((int) ch == 32)
        ++index;
      else if ((int) ch == 0 && index == this.parseEnd && !this.endOfFileVisible)
        return false;
      int num2 = 0;
      if (num1 != 1 || ((int) this.parseBuffer[this.parseOffset + 1] | 32) != 117)
      {
        short keywordId = this.LookupKeyword(hash, this.parseOffset + 1, num1);
        if ((int) RTFData.keywords[(int) keywordId].character == 0)
        {
          if (!flag1)
            unescaped = (int) RTFData.keywords[(int) keywordId].defaultValue;
          if ((int) keywordId == 47)
            this.binLength = unescaped > 0 ? unescaped : 0;
          bool skip = ((int) keywordId != 47 || this.binLength == 0) && this.SkipIfNecessary(1);
          this.run.InitializeKeyword(keywordId, unescaped, index - this.parseOffset, skip, this.firstKeyword);
          this.parseOffset = index;
          return true;
        }
        unescaped = (int) RTFData.keywords[(int) keywordId].character;
      }
      else
      {
        num2 = (int) this.bytesSkipForUnicodeEscape;
        if (unescaped < 0)
          unescaped &= (int) ushort.MaxValue;
        else if (unescaped > 1114111)
          unescaped = 63;
        if (this.currentCharRep == RtfSupport.CharRep.SYMBOL_INDEX && 61440 <= unescaped && unescaped <= 61695)
          unescaped -= 61440;
        if (unescaped == 0)
          unescaped = 32;
      }
      this.run.Initialize(RtfRunKind.Unicode, index - this.parseOffset, unescaped, this.SkipIfNecessary(1), ParseSupport.IsHighSurrogate((char) unescaped));
      this.parseOffset = index;
      this.bytesToSkip += num2;
      return true;
    }

    private bool ParseTextRun()
    {
      int index = this.parseOffset;
      int val1 = this.parseEnd;
      byte num = this.parseBuffer[index];
      if (this.bytesToSkip != 0)
        val1 = Math.Min(val1, index + this.bytesToSkip);
      CharClass charClass1;
      bool skip;
      RtfRunKind kind;
      if ((int) num == 0)
      {
        this.lastLeadByte = false;
        byte ch;
        do
        {
          ch = this.parseBuffer[++index];
        }
        while ((int) ch == 0 && index != val1);
        charClass1 = ParseSupport.GetCharClass(ch);
        skip = this.SkipIfNecessary(index - this.parseOffset);
        kind = RtfRunKind.Zero;
      }
      else if (this.leadMask == (DbcsLeadBits) 0)
      {
        this.lastLeadByte = false;
        if (this.bytesToSkip == 0)
        {
          do
            ;
          while (!ParseSupport.RtfInterestingCharacter(ParseSupport.GetCharClass(this.parseBuffer[++index])));
          skip = false;
        }
        else
        {
          CharClass charClass2;
          do
          {
            charClass2 = ParseSupport.GetCharClass(this.parseBuffer[++index]);
          }
          while (index != val1 && !ParseSupport.RtfInterestingCharacter(charClass2));
          skip = this.SkipIfNecessary(index - this.parseOffset);
        }
        kind = RtfRunKind.Text;
      }
      else
      {
        do
        {
          CharClass charClass2;
          do
          {
            this.lastLeadByte = !this.lastLeadByte && ParseSupport.IsLeadByte(num, this.leadMask);
            num = this.parseBuffer[++index];
            charClass2 = ParseSupport.GetCharClass(num);
          }
          while (index != val1 && !ParseSupport.RtfInterestingCharacter(charClass2));
          if (!this.lastLeadByte)
            goto label_20;
        }
        while (index != val1 && ((int) num == 123 || (int) num == 125));
        if (index - this.parseOffset > 1)
        {
          --index;
          this.lastLeadByte = false;
          charClass1 = ParseSupport.GetCharClass(this.parseBuffer[index]);
        }
        else if (index == val1 && !this.endOfFileVisible && val1 == this.parseEnd)
        {
          this.lastLeadByte = false;
          return false;
        }
label_20:
        skip = this.SkipIfNecessary(index - this.parseOffset);
        kind = RtfRunKind.Text;
      }
      this.run.Initialize(kind, index - this.parseOffset, 0, skip, this.lastLeadByte);
      this.parseOffset = index;
      return true;
    }

    private bool SkipIfNecessary(int length)
    {
      if (this.bytesToSkip == 0)
        return false;
      this.bytesToSkip -= length;
      return true;
    }

    private short LookupKeyword(short hash, int nameOffset, int nameLength)
    {
      short num = RTFData.keywordHashTable[(int) hash];
      if ((int) num != 0)
      {
        bool flag = false;
        do
        {
          if (RTFData.keywords[(int) num].name.Length == nameLength && (int) RTFData.keywords[(int) num].name[0] == (int) this.parseBuffer[nameOffset])
          {
            int index = 1;
            while (index < nameLength && (int) RTFData.keywords[(int) num].name[index] == (int) this.parseBuffer[nameOffset + index])
              ++index;
            if (index == nameLength)
            {
              flag = true;
              break;
            }
          }
        }
        while ((int) ++num < RTFData.keywords.Length && (int) RTFData.keywords[(int) num].hash == (int) hash);
        if (!flag)
          num = (short) 0;
      }
      return num;
    }
  }
}
