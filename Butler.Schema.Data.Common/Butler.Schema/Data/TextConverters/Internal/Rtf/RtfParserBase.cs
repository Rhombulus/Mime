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
      private bool firstKeyword;
    private int bytesToSkip;
    private int binLength;
    private DbcsLeadBits leadMask;
    private IReportBytes reportBytes;

    public byte[] ParseBuffer { get; }

      public int ParseStart { get; private set; }

      public int ParseOffset { get; private set; }

      public int ParseEnd { get; private set; }

      public bool ParseBufferFull => this.ParseEnd + 128 >= this.ParseBuffer.Length;

      public bool EndOfFile
    {
      get
      {
        if (this.EndOfFileVisible)
          return this.ParseOffset == this.ParseEnd;
        return false;
      }
    }

    public bool EndOfFileVisible { get; private set; }

      public bool ParseBufferNeedsRefill => this.ParseEnd - this.ParseOffset < 128;

      public RtfRunKind RunKind => this.run.Kind;

      public short KeywordId => this.run.KeywordId;

      public int KeywordValue => this.run.Value;

      public RtfParserBase(int inputBufferSize, bool testBoundaryConditions, IReportBytes reportBytes)
    {
      this.ParseBuffer = new byte[1 + (testBoundaryConditions ? 133 : Math.Min((int) short.MaxValue, inputBufferSize))];
      this.reportBytes = reportBytes;
      this.InitializeBase();
    }

    public RtfParserBase(int inputBufferSize, bool testBoundaryConditions, RtfParserBase previewParser, IReportBytes reportBytes)
    {
      int num = 1 + (testBoundaryConditions ? 133 : Math.Min((int) short.MaxValue, inputBufferSize));
      if (previewParser.ParseBuffer.Length < num)
      {
        this.ParseBuffer = new byte[1 + (testBoundaryConditions ? 133 : Math.Min((int) short.MaxValue, inputBufferSize))];
        Buffer.BlockCopy((Array) previewParser.ParseBuffer, 0, (Array) this.ParseBuffer, 0, previewParser.ParseEnd);
      }
      else
        this.ParseBuffer = previewParser.ParseBuffer;
      this.ParseEnd = previewParser.ParseEnd;
      this.EndOfFileVisible = previewParser.EndOfFileVisible;
      this.reportBytes = reportBytes;
      this.InitializeBase();
    }

    public int GetBufferSpace(bool compact, out int offset)
    {
      if (compact && this.ParseStart != 0 && (this.ParseEnd - this.ParseStart < 128 || this.ParseEnd + 128 > this.ParseBuffer.Length - 1))
      {
        if (this.ParseEnd != this.ParseStart)
          Buffer.BlockCopy((Array) this.ParseBuffer, this.ParseStart, (Array) this.ParseBuffer, 0, this.ParseEnd - this.ParseStart);
        this.ParseOffset -= this.ParseStart;
        this.ParseEnd -= this.ParseStart;
        this.ParseStart = 0;
      }
      offset = this.ParseEnd;
      return this.ParseBuffer.Length - 1 - this.ParseEnd;
    }

    public void ReportMoreDataAvailable(int length, bool endOfFileVisible)
    {
      this.ParseEnd += length;
      this.ParseBuffer[this.ParseEnd] = (byte) 0;
      this.EndOfFileVisible = endOfFileVisible;
      if (this.reportBytes == null)
        return;
      this.reportBytes.ReportBytesRead(length);
    }

    public bool ParseRun()
    {
      if (this.ParseEnd == this.ParseOffset)
      {
        if (!this.EndOfFileVisible)
          return false;
        this.run.Initialize(RtfRunKind.EndOfFile, 0, 0);
        return true;
      }
      if (this.binLength != 0)
      {
        int length = Math.Min(this.ParseEnd - this.ParseOffset, this.binLength);
        this.run.Initialize(RtfRunKind.Binary, length, 0, 0 != this.bytesToSkip, false);
        this.binLength -= length;
        this.ParseOffset += length;
        if (this.binLength == 0 && this.bytesToSkip != 0)
          --this.bytesToSkip;
        return true;
      }
      int index = this.ParseOffset;
      switch (this.ParseBuffer[index])
      {
        case (byte) 9:
          if (!this.lastLeadByte)
          {
            this.run.InitializeKeyword((short) 126, 0, 1, this.SkipIfNecessary(1), this.firstKeyword);
            ++this.ParseOffset;
            this.firstKeyword = false;
            return true;
          }
          break;
        case (byte) 10:
        case (byte) 13:
          byte num;
          do
          {
            num = this.ParseBuffer[++index];
          }
          while ((int) num == 13 || (int) num == 10);
          this.run.Initialize(RtfRunKind.Ignore, index - this.ParseOffset, 0, false, false);
          this.ParseOffset = index;
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
            ++this.ParseOffset;
            this.firstKeyword = true;
            this.bytesToSkip = 0;
            return true;
          }
          break;
        case (byte) 125:
          if (!this.lastLeadByte)
          {
            this.run.Initialize(RtfRunKind.End, 1, 0);
            ++this.ParseOffset;
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
      this.ParseStart += length;
    }

    protected void InitializeBase()
    {
      this.ParseStart = 0;
      this.ParseOffset = 0;
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
      if (1 == this.ParseEnd - this.ParseOffset)
      {
        if (!this.EndOfFileVisible)
          return false;
        this.run.Initialize(RtfRunKind.Text, 1, 0, this.SkipIfNecessary(1), false);
        ++this.ParseOffset;
        return true;
      }
      byte ch = this.ParseBuffer[this.ParseOffset + 1];
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
          this.ParseOffset += 2;
          return true;
        case '~':
          unescaped2 = 160;
          goto label_13;
        case ':':
          this.lastLeadByte = false;
          this.run.InitializeKeyword((short) 3, 0, 2, this.SkipIfNecessary(1), this.firstKeyword);
          this.ParseOffset += 2;
          return true;
        case '*':
          this.lastLeadByte = false;
          this.run.InitializeKeyword((short) 1, 0, 2, this.SkipIfNecessary(1), this.firstKeyword);
          this.ParseOffset += 2;
          return true;
        case '-':
          unescaped2 = 173;
          goto label_13;
        case '\t':
          this.lastLeadByte = false;
          this.run.InitializeKeyword((short) 126, 0, 2, this.SkipIfNecessary(1), this.firstKeyword);
          this.ParseOffset += 2;
          return true;
        case '\n':
        case '\r':
          this.lastLeadByte = false;
          this.run.InitializeKeyword((short) 68, 0, 2, this.SkipIfNecessary(1), this.firstKeyword);
          this.ParseOffset += 2;
          return true;
        case '\'':
          this.EnsureCodePage();
          if (this.ParseEnd - this.ParseOffset < 4)
          {
            if (!this.EndOfFileVisible)
              return false;
            this.run.Initialize(RtfRunKind.Text, 1, 0, this.SkipIfNecessary(1), false);
            ++this.ParseOffset;
            this.lastLeadByte = false;
            return true;
          }
          int unescaped3 = RtfSupport.Unescape(this.ParseBuffer[this.ParseOffset + 2], this.ParseBuffer[this.ParseOffset + 3]);
          if (unescaped3 > (int) byte.MaxValue)
          {
            if (this.lastLeadByte)
            {
              this.lastLeadByte = false;
              this.run.Initialize(RtfRunKind.Text, 1, 0, this.SkipIfNecessary(1), false);
              ++this.ParseOffset;
              return true;
            }
            unescaped3 = 63;
          }
          else
          {
            if ((unescaped3 == 13 || unescaped3 == 10) && !this.lastLeadByte)
            {
              this.run.InitializeKeyword((short) 68, 0, 4, this.SkipIfNecessary(1), this.firstKeyword);
              this.ParseOffset += 4;
              return true;
            }
            if (unescaped3 == 0)
              unescaped3 = 32;
            this.lastLeadByte = !this.lastLeadByte && ParseSupport.IsLeadByte((byte) unescaped3, this.leadMask);
          }
          this.run.Initialize(RtfRunKind.Escape, 4, unescaped3, this.SkipIfNecessary(1), this.lastLeadByte);
          this.ParseOffset += 4;
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
      this.ParseOffset += 2;
      return true;
label_13:
      this.EnsureCodePage();
      this.lastLeadByte = false;
      this.run.Initialize(RtfRunKind.Unicode, 2, unescaped2, this.SkipIfNecessary(1), false);
      this.ParseOffset += 2;
      return true;
    }

    private bool ParseKeyword(byte ch)
    {
      int index = this.ParseOffset + 1;
      short hash = (short) 0;
      do
      {
        hash = RTFData.AddHash(hash, ch);
        ch = this.ParseBuffer[++index];
      }
      while ((uint) (((int) ch | 32) - 97) <= 25U);
      int num1 = index - (this.ParseOffset + 1);
      bool flag1 = false;
      bool flag2 = false;
      int unescaped = 0;
      if ((int) ch == 45)
      {
        flag1 = true;
        flag2 = true;
        ++index;
        ch = this.ParseBuffer[index];
      }
      if ((uint) ch - 48U <= 9U)
      {
        flag1 = true;
        do
        {
          unescaped = unescaped * 10 + ((int) ch - 48);
          ch = this.ParseBuffer[++index];
        }
        while ((uint) ch - 48U <= 9U);
        if (flag2)
          unescaped = -unescaped;
      }
      if (index > this.ParseOffset + 128 - 1)
      {
        index = this.ParseOffset + 128 - 1;
        ch = this.ParseBuffer[index];
        unescaped = 0;
        num1 = Math.Min(num1, index - (this.ParseOffset + 1));
      }
      if ((int) ch == 32)
        ++index;
      else if ((int) ch == 0 && index == this.ParseEnd && !this.EndOfFileVisible)
        return false;
      int num2 = 0;
      if (num1 != 1 || ((int) this.ParseBuffer[this.ParseOffset + 1] | 32) != 117)
      {
        short keywordId = this.LookupKeyword(hash, this.ParseOffset + 1, num1);
        if ((int) RTFData.keywords[(int) keywordId].character == 0)
        {
          if (!flag1)
            unescaped = (int) RTFData.keywords[(int) keywordId].defaultValue;
          if ((int) keywordId == 47)
            this.binLength = unescaped > 0 ? unescaped : 0;
          bool skip = ((int) keywordId != 47 || this.binLength == 0) && this.SkipIfNecessary(1);
          this.run.InitializeKeyword(keywordId, unescaped, index - this.ParseOffset, skip, this.firstKeyword);
          this.ParseOffset = index;
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
      this.run.Initialize(RtfRunKind.Unicode, index - this.ParseOffset, unescaped, this.SkipIfNecessary(1), ParseSupport.IsHighSurrogate((char) unescaped));
      this.ParseOffset = index;
      this.bytesToSkip += num2;
      return true;
    }

    private bool ParseTextRun()
    {
      int index = this.ParseOffset;
      int val1 = this.ParseEnd;
      byte num = this.ParseBuffer[index];
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
          ch = this.ParseBuffer[++index];
        }
        while ((int) ch == 0 && index != val1);
        charClass1 = ParseSupport.GetCharClass(ch);
        skip = this.SkipIfNecessary(index - this.ParseOffset);
        kind = RtfRunKind.Zero;
      }
      else if (this.leadMask == (DbcsLeadBits) 0)
      {
        this.lastLeadByte = false;
        if (this.bytesToSkip == 0)
        {
          do
            ;
          while (!ParseSupport.RtfInterestingCharacter(ParseSupport.GetCharClass(this.ParseBuffer[++index])));
          skip = false;
        }
        else
        {
          CharClass charClass2;
          do
          {
            charClass2 = ParseSupport.GetCharClass(this.ParseBuffer[++index]);
          }
          while (index != val1 && !ParseSupport.RtfInterestingCharacter(charClass2));
          skip = this.SkipIfNecessary(index - this.ParseOffset);
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
            num = this.ParseBuffer[++index];
            charClass2 = ParseSupport.GetCharClass(num);
          }
          while (index != val1 && !ParseSupport.RtfInterestingCharacter(charClass2));
          if (!this.lastLeadByte)
            goto label_20;
        }
        while (index != val1 && ((int) num == 123 || (int) num == 125));
        if (index - this.ParseOffset > 1)
        {
          --index;
          this.lastLeadByte = false;
          charClass1 = ParseSupport.GetCharClass(this.ParseBuffer[index]);
        }
        else if (index == val1 && !this.EndOfFileVisible && val1 == this.ParseEnd)
        {
          this.lastLeadByte = false;
          return false;
        }
label_20:
        skip = this.SkipIfNecessary(index - this.ParseOffset);
        kind = RtfRunKind.Text;
      }
      this.run.Initialize(kind, index - this.ParseOffset, 0, skip, this.lastLeadByte);
      this.ParseOffset = index;
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
          if (RTFData.keywords[(int) num].name.Length == nameLength && (int) RTFData.keywords[(int) num].name[0] == (int) this.ParseBuffer[nameOffset])
          {
            int index = 1;
            while (index < nameLength && (int) RTFData.keywords[(int) num].name[index] == (int) this.ParseBuffer[nameOffset + index])
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
