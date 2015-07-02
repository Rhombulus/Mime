// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Rtf.HtmlInRtfExtractingInput
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Rtf
{
  internal class HtmlInRtfExtractingInput : ConverterInput
  {
    private RtfParser parser;
    private bool firstKeyword;
    private bool ignorableDestination;
    private HtmlInRtfExtractingInput.RtfState state;
    private bool ignoreRtf;
    private RtfToken incompleteToken;
    private char[] parseBuffer;
    private int parseStart;
    private int parseEnd;

    public HtmlInRtfExtractingInput(RtfParser parser, int maxParseToken, bool testBoundaryConditions, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum)
      : base((IProgressMonitor) null)
    {
      this.parser = parser;
      this.state = new HtmlInRtfExtractingInput.RtfState(16);
      this.maxTokenSize = testBoundaryConditions ? 123 : maxParseToken;
      this.parseBuffer = new char[Math.Min((long) maxParseToken + 1L, 4096L)];
    }

    public override bool ReadMore(ref char[] buffer, ref int start, ref int current, ref int end)
    {
      if (this.parseBuffer.Length - this.parseEnd < 6 && !this.EnsureFreeSpace())
        return true;
      int num = this.parseEnd;
      if (this.incompleteToken != null)
      {
        if (this.incompleteToken.Id == RtfTokenId.Keywords)
          this.ProcessKeywords(this.incompleteToken);
        else
          this.ProcessText(this.incompleteToken);
      }
      while (!this.endOfFile && this.parseBuffer.Length - this.parseEnd >= 6)
      {
        RtfTokenId tokenId = this.parser.Parse();
        if (tokenId != RtfTokenId.None)
          this.Process(tokenId);
        else
          break;
      }
      buffer = this.parseBuffer;
      if (start != this.parseStart)
      {
        current = this.parseStart + (current - start);
        start = this.parseStart;
      }
      end = this.parseEnd;
      if (end == num)
        return this.endOfFile;
      return true;
    }

    public override void ReportProcessed(int processedSize)
    {
      this.parseStart += processedSize;
    }

    public override int RemoveGap(int gapBegin, int gapEnd)
    {
      if (gapEnd == this.parseEnd)
      {
        this.parseEnd = gapBegin;
        this.parseBuffer[gapBegin] = char.MinValue;
        return gapBegin;
      }
      Buffer.BlockCopy((Array) this.parseBuffer, gapEnd, (Array) this.parseBuffer, gapBegin, this.parseEnd - gapEnd);
      this.parseEnd = gapBegin + (this.parseEnd - gapEnd);
      this.parseBuffer[this.parseEnd] = char.MinValue;
      return this.parseEnd;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.parser != null && this.parser is IDisposable)
        ((IDisposable) this.parser).Dispose();
      this.parser = (RtfParser) null;
      base.Dispose(disposing);
    }

    private void Process(RtfTokenId tokenId)
    {
      switch (tokenId)
      {
        case RtfTokenId.EndOfFile:
          this.ProcessEOF();
          break;
        case RtfTokenId.Begin:
          this.ProcessBeginGroup();
          break;
        case RtfTokenId.End:
          this.ProcessEndGroup();
          break;
        case RtfTokenId.Binary:
          this.ProcessBinary(this.parser.Token);
          break;
        case RtfTokenId.Keywords:
          this.ProcessKeywords(this.parser.Token);
          break;
        case RtfTokenId.Text:
          this.ProcessText(this.parser.Token);
          break;
      }
    }

    private void ProcessBeginGroup()
    {
      this.state.Push();
      if (this.state.Skip)
        return;
      this.firstKeyword = true;
      this.ignorableDestination = false;
    }

    private void ProcessEndGroup()
    {
      if (this.state.Skip)
      {
        this.state.Pop();
        if (this.state.Skip)
          return;
      }
      this.firstKeyword = false;
      if (!this.state.CanPop)
        return;
      this.state.Pop();
    }

    private void ProcessKeywords(RtfToken token)
    {
      if (this.state.Skip)
        return;
      foreach (RtfKeyword rtfKeyword in token.Keywords)
      {
        if (this.firstKeyword)
        {
          if ((int) rtfKeyword.Id == 1)
          {
            this.ignorableDestination = true;
            continue;
          }
          this.firstKeyword = false;
          switch (rtfKeyword.Id)
          {
            case (short) 268:
              if (this.state.Destination == RtfDestination.FontTable)
              {
                this.state.Destination = RtfDestination.AltFontName;
                continue;
              }
              continue;
            case (short) 315:
            case (short) 246:
            case (short) 50:
            case (short) 15:
            case (short) 24:
              this.state.PushSkipGroup();
              return;
            case (short) 319:
              this.state.Destination = RtfDestination.ListText;
              continue;
            case (short) 252:
              if (this.state.Destination == RtfDestination.RTF)
              {
                this.state.Destination = RtfDestination.ColorTable;
                continue;
              }
              continue;
            case (short) 175:
              if (this.state.Destination == RtfDestination.RTF)
              {
                this.state.Destination = RtfDestination.FontTable;
                continue;
              }
              continue;
            case (short) 177:
              this.state.Destination = RtfDestination.HtmlTagIndex;
              continue;
            case (short) 210:
              if (this.state.Destination == RtfDestination.FontTable)
              {
                this.state.Destination = RtfDestination.RealFontName;
                continue;
              }
              continue;
            default:
              if (this.ignorableDestination)
              {
                this.state.PushSkipGroup();
                return;
              }
              break;
          }
        }
        switch (rtfKeyword.Id)
        {
          case (short) 126:
          case (short) 119:
            if (!this.ignoreRtf && (this.state.Destination == RtfDestination.RTF || this.state.Destination == RtfDestination.HtmlTagIndex))
            {
              this.Output("\t");
              break;
            }
            break;
          case (short) 153:
            this.ignoreRtf = rtfKeyword.Value != 0;
            break;
          case (short) 40:
          case (short) 48:
          case (short) 68:
            if (!this.ignoreRtf && (this.state.Destination == RtfDestination.RTF || this.state.Destination == RtfDestination.HtmlTagIndex))
            {
              this.Output("\r\n");
              break;
            }
            break;
        }
        if (this.parseBuffer.Length - this.parseEnd < 6)
        {
          this.incompleteToken = token;
          return;
        }
      }
      this.incompleteToken = (RtfToken) null;
    }

    private void ProcessText(RtfToken token)
    {
      if (this.state.Skip)
        return;
      switch (this.state.Destination)
      {
        case RtfDestination.RTF:
          if (this.ignoreRtf)
            break;
          goto case RtfDestination.HtmlTagIndex;
        case RtfDestination.HtmlTagIndex:
          token.StripZeroBytes = true;
          this.parseEnd += token.Text.Read(this.parseBuffer, this.parseEnd, this.parseBuffer.Length - this.parseEnd - 1);
          this.parseBuffer[this.parseEnd] = char.MinValue;
          if (this.parseEnd == this.parseBuffer.Length - 1)
          {
            this.incompleteToken = token;
            break;
          }
          this.incompleteToken = (RtfToken) null;
          break;
        default:
          this.firstKeyword = false;
          break;
      }
    }

    private void ProcessBinary(RtfToken token)
    {
    }

    private void ProcessEOF()
    {
      while (this.state.CanPop)
        this.ProcessEndGroup();
      this.endOfFile = true;
    }

    private void Output(string str)
    {
      str.CopyTo(0, this.parseBuffer, this.parseEnd, str.Length);
      this.parseEnd += str.Length;
      this.parseBuffer[this.parseEnd] = char.MinValue;
    }

    private bool EnsureFreeSpace()
    {
      if (this.parseBuffer.Length - (this.parseEnd - this.parseStart) < 6 || this.parseStart < 1 && (long) this.parseBuffer.Length < (long) this.maxTokenSize + 1L)
      {
        if ((long) this.parseBuffer.Length >= (long) this.maxTokenSize + 5L + 1L)
          return false;
        long num = (long) (this.parseBuffer.Length * 2);
        if (num > (long) this.maxTokenSize + 5L + 1L)
          num = (long) this.maxTokenSize + 5L + 1L;
        if (num > (long) int.MaxValue)
          num = (long) int.MaxValue;
        char[] chArray = new char[(int) num];
        Buffer.BlockCopy((Array) this.parseBuffer, this.parseStart * 2, (Array) chArray, 0, (this.parseEnd - this.parseStart + 1) * 2);
        this.parseBuffer = chArray;
        this.parseEnd = this.parseEnd - this.parseStart;
        this.parseStart = 0;
      }
      else
      {
        Buffer.BlockCopy((Array) this.parseBuffer, this.parseStart * 2, (Array) this.parseBuffer, 0, (this.parseEnd - this.parseStart + 1) * 2);
        this.parseEnd = this.parseEnd - this.parseStart;
        this.parseStart = 0;
      }
      return true;
    }

    private struct RtfState
    {
      private int level;
      private int skipLevel;
      private HtmlInRtfExtractingInput.RtfState.State current;
      private HtmlInRtfExtractingInput.RtfState.State[] stack;
      private int stackTop;

      public RtfDestination Destination
      {
        get
        {
          return this.current.Dest;
        }
        set
        {
          this.SetDirty();
          this.current.Dest = value;
        }
      }

      public bool Skip => this.skipLevel != 0;

        public bool CanPop => this.level > 1;

        public RtfState(int initialStackSize)
      {
        this.level = 0;
        this.skipLevel = 0;
        this.current = new HtmlInRtfExtractingInput.RtfState.State();
        this.stack = new HtmlInRtfExtractingInput.RtfState.State[initialStackSize];
        this.stackTop = 0;
        this.current.LevelsDeep = (short) 1;
        this.current.Dest = RtfDestination.RTF;
      }

      public void Push()
      {
        ++this.level;
        ++this.current.LevelsDeep;
        if ((int) this.current.LevelsDeep != (int) short.MaxValue)
          return;
        this.PushReally();
      }

      public void PushSkipGroup()
      {
        this.skipLevel = this.level;
        this.Push();
      }

      public void Pop()
      {
        if (this.level <= 1)
          return;
        if ((int) --this.current.LevelsDeep == 0 && this.stackTop != 0)
          this.current = this.stack[--this.stackTop];
        if (--this.level != this.skipLevel)
          return;
        this.skipLevel = 0;
      }

      public void SetDirty()
      {
        if ((int) this.current.LevelsDeep <= 1)
          return;
        this.PushReally();
      }

      private void PushReally()
      {
        if (this.stackTop >= 4096)
          throw new TextConvertersException(CtsResources.TextConvertersStrings.InputDocumentTooComplex);
        if (this.stackTop == this.stack.Length)
        {
          HtmlInRtfExtractingInput.RtfState.State[] stateArray = new HtmlInRtfExtractingInput.RtfState.State[this.stackTop * 2];
          Array.Copy((Array) this.stack, 0, (Array) stateArray, 0, this.stackTop);
          this.stack = stateArray;
        }
        this.stack[this.stackTop] = this.current;
        --this.stack[this.stackTop].LevelsDeep;
        ++this.stackTop;
        this.current.LevelsDeep = (short) 1;
      }

      public struct State
      {
        public RtfDestination Dest;
        public short LevelsDeep;
      }
    }
  }
}
