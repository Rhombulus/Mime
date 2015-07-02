// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Rtf.RtfToRtfConverter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Rtf
{
  internal class RtfToRtfConverter : IProducerConsumer, IDisposable
  {
    private RtfOutput output;
    private bool endOfFile;
    private RtfParser parser;
    private bool firstKeyword;
    private RtfToRtfConverter.RtfState state;
    private short maxFontHandle;
    private int colorsCount;
    private int lineLength;
    private int lineBreaks;
    private Injection injection;
    private int color;
    private ScratchBuffer scratch;

    public RtfToRtfConverter(RtfParser parser, Stream destination, bool push, Injection injection, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum)
    {
      this.output = new RtfOutput(destination, push, false);
      this.parser = parser;
      this.injection = injection;
      this.state = new RtfToRtfConverter.RtfState(128);
      if (this.injection == null)
        return;
      this.injection.CompileForRtf(this.output);
    }

    void IDisposable.Dispose()
    {
      if (this.parser != null && this.parser is IDisposable)
        ((IDisposable) this.parser).Dispose();
      if (this.output != null && this.output != null)
        ((IDisposable) this.output).Dispose();
      this.parser = (RtfParser) null;
      this.output = (RtfOutput) null;
      this.scratch.DisposeBuffer();
      GC.SuppressFinalize((object) this);
    }

    public void Run()
    {
      if (this.endOfFile)
        return;
      RtfTokenId tokenId = this.parser.Parse();
      if (tokenId == RtfTokenId.None)
        return;
      this.Process(tokenId);
    }

    public bool Flush()
    {
      this.Run();
      return this.endOfFile;
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
      if (this.state.Level < 0)
        return;
      if (this.state.SkipLevel != 0 || this.state.Level < 0 || this.firstKeyword)
        this.output.WriteControlText("{", false);
      this.state.Push();
      this.firstKeyword = true;
    }

    private void ProcessEndGroup()
    {
      if (this.state.Level <= 0)
        return;
      if (this.state.SkipLevel != 0 && this.state.Level == this.state.SkipLevel)
        this.state.SkipLevel = 0;
      this.firstKeyword = false;
      this.EndGroup();
      this.state.Pop();
      if (this.state.Level == 0)
      {
        --this.state.Level;
        if (this.injection != null && this.injection.HaveTail && !this.injection.TailDone)
        {
          if (this.lineLength != 0)
          {
            this.output.WriteControlText("\\par\r\n", false);
            this.output.RtfLineLength = 0;
            this.lineBreaks = 1;
          }
          if (this.output.RtfLineLength != 0)
          {
            this.output.WriteControlText("\r\n", false);
            this.output.RtfLineLength = 0;
          }
          this.injection.InjectRtf(false, this.lineBreaks <= 1);
        }
      }
      this.output.WriteControlText("}", false);
    }

    private void ProcessKeywords(RtfToken token)
    {
      if (this.state.Level < 0)
        return;
      foreach (RtfKeyword kw in token.Keywords)
      {
        if (this.state.SkipLevel != 0)
        {
          this.WriteKeyword(kw);
        }
        else
        {
          if (this.firstKeyword)
          {
            this.firstKeyword = false;
            if ((int) kw.Id == 1)
            {
              this.state.SkipGroup();
            }
            else
            {
              switch (kw.Id)
              {
                case (short) 277:
                case (short) 315:
                case (short) 316:
                case (short) 273:
                case (short) 246:
                case (short) 253:
                case (short) 258:
                case (short) 230:
                case (short) 241:
                case (short) 201:
                case (short) 15:
                case (short) 24:
                  this.state.SkipGroup();
                  break;
                case (short) 267:
                  this.output.WriteControlText("{", false);
                  this.WriteKeyword(kw);
                  continue;
                case (short) 268:
                  if (this.state.Destination == RtfDestination.FontTable && (int) this.state.FontIndex >= 0)
                  {
                    this.state.Destination = RtfDestination.AltFontName;
                    break;
                  }
                  break;
                case (short) 252:
                  if (this.state.Destination == RtfDestination.RTF)
                  {
                    this.state.Destination = RtfDestination.ColorTable;
                    break;
                  }
                  break;
                case (short) 210:
                  if (this.state.Destination == RtfDestination.FontTable && (int) this.state.FontIndex >= 0)
                  {
                    this.state.Destination = RtfDestination.RealFontName;
                    break;
                  }
                  break;
                case (short) 175:
                  if (this.state.Destination == RtfDestination.RTF)
                  {
                    this.state.FontIndex = (short) -1;
                    this.state.Destination = RtfDestination.FontTable;
                    break;
                  }
                  break;
                default:
                  if (this.state.Destination == RtfDestination.RTF && this.injection != null && (this.injection.HaveHead && !this.injection.HeadDone))
                  {
                    this.output.WriteControlText("\r\n", false);
                    this.output.RtfLineLength = 0;
                    this.injection.InjectRtf(true, false);
                    break;
                  }
                  break;
              }
            }
            this.output.WriteControlText("{", false);
          }
          switch (kw.Id)
          {
            case (short) 309:
            case (short) 148:
            case (short) 88:
              if (this.state.Destination == RtfDestination.ColorTable)
              {
                this.color &= ~((int) byte.MaxValue << (int) RTFData.keywords[(int) kw.Id].idx * 8);
                this.color |= (kw.Value & (int) byte.MaxValue) << (int) RTFData.keywords[(int) kw.Id].idx * 8;
                break;
              }
              break;
            case (short) 331:
            case (short) 265:
            case (short) 284:
            case (short) 206:
            case (short) 170:
            case (short) 109:
            case (short) 112:
            case (short) 115:
            case (short) 116:
            case (short) 76:
              if (this.state.Destination == RtfDestination.RTF && this.injection != null && (this.injection.HaveHead && !this.injection.HeadDone))
              {
                this.output.WriteControlText("\r\n", false);
                this.output.RtfLineLength = 0;
                this.injection.InjectRtf(true, false);
                break;
              }
              break;
            case (short) 200:
            case (short) 184:
            case (short) 126:
              if (this.state.Destination == RtfDestination.RTF && this.injection != null && (this.injection.HaveHead && !this.injection.HeadDone))
              {
                this.output.WriteControlText("\r\n", false);
                this.output.RtfLineLength = 0;
                this.injection.InjectRtf(true, false);
                break;
              }
              break;
            case (short) 113:
              if (this.state.Destination == RtfDestination.FontTable || this.state.Destination == RtfDestination.AltFontName || this.state.Destination == RtfDestination.RealFontName)
              {
                if ((int) this.state.FontIndex >= 0)
                  this.state.FontIndex = (short) -1;
                short fontIndex = this.parser.FontIndex((short) kw.Value);
                if ((int) fontIndex >= 0)
                {
                  this.state.FontIndex = fontIndex;
                  if ((int) this.parser.FontHandle(fontIndex) > (int) this.maxFontHandle)
                  {
                    this.maxFontHandle = this.parser.FontHandle(fontIndex);
                    break;
                  }
                  break;
                }
                break;
              }
              break;
            case (short) 48:
            case (short) 68:
              if (this.state.Destination == RtfDestination.RTF)
              {
                this.lineLength = 0;
                ++this.lineBreaks;
                goto case (short) 200;
              }
              else
                goto case (short) 200;
          }
          this.WriteKeyword(kw);
        }
      }
    }

    private void ProcessText(RtfToken token)
    {
      if (this.state.Level < 0)
        return;
      if (this.state.Destination == RtfDestination.RTF && this.state.SkipLevel == 0)
      {
        bool flag = true;
        foreach (RtfRun rtfRun in token.Runs)
        {
          if (rtfRun.Kind != RtfRunKind.Ignore)
          {
            flag = false;
            break;
          }
        }
        token.Runs.Rewind();
        if (!flag)
        {
          if (this.injection != null && this.injection.HaveHead && !this.injection.HeadDone)
          {
            this.output.WriteControlText("\r\n", false);
            this.output.RtfLineLength = 0;
            this.injection.InjectRtf(true, false);
          }
          ++this.lineLength;
          this.lineBreaks = 0;
        }
      }
      if (this.firstKeyword && this.state.SkipLevel == 0)
      {
        this.output.WriteControlText("{", false);
        this.firstKeyword = false;
      }
      this.WriteToken(token);
      token.Runs.Rewind();
      switch (this.state.Destination)
      {
        case RtfDestination.ColorTable:
          this.scratch.Reset();
          while (this.scratch.AppendRtfTokenText(token, 256))
          {
            for (int index = 0; index < this.scratch.Length; ++index)
            {
              if ((int) this.scratch[index] == 59)
              {
                ++this.colorsCount;
                this.color = 0;
              }
            }
            this.scratch.Reset();
          }
          break;
      }
    }

    private void ProcessBinary(RtfToken token)
    {
      if (this.state.Level < 0)
        return;
      this.WriteToken(token);
    }

    private void ProcessEOF()
    {
      while (this.state.Level > 0)
        this.ProcessEndGroup();
      this.output.Flush();
      this.endOfFile = true;
    }

    private void EndGroup()
    {
      switch (this.state.Destination)
      {
        case RtfDestination.FontTable:
          if (this.state.ParentDestination != RtfDestination.FontTable)
          {
            if (this.injection == null)
              break;
            this.output.WriteControlText("\r\n", false);
            this.output.RtfLineLength = 0;
            this.injection.InjectRtfFonts((int) this.maxFontHandle + 1);
            break;
          }
          if ((int) this.state.FontIndex < 0)
            break;
          this.state.ParentFontIndex = this.state.FontIndex;
          break;
        case RtfDestination.ColorTable:
          if (this.state.ParentDestination == RtfDestination.ColorTable || this.injection == null)
            break;
          if (this.color != 0)
          {
            ++this.colorsCount;
            this.output.WriteControlText(";", false);
          }
          this.output.WriteControlText("\r\n", false);
          this.output.RtfLineLength = 0;
          this.injection.InjectRtfColors(this.colorsCount);
          break;
      }
    }

    private void WriteToken(RtfToken token)
    {
      foreach (RtfRun rtfRun in token.Runs)
        this.output.WriteBinary(rtfRun.Buffer, rtfRun.Offset, rtfRun.Length);
    }

    private void WriteKeyword(RtfKeyword kw)
    {
      this.output.WriteBinary(kw.Buffer, kw.Offset, kw.Length);
    }

    internal struct RtfState
    {
      public int Level;
      public int SkipLevel;
      public RtfToRtfConverter.RtfState.State Current;
      private RtfToRtfConverter.RtfState.State[] stack;
      private int stackTop;

      public RtfDestination Destination
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

      public RtfDestination ParentDestination
      {
        get
        {
          if ((int) this.Current.Depth <= 1 && this.stackTop != 0)
            return this.stack[this.stackTop - 1].Dest;
          return this.Current.Dest;
        }
      }

      public short ParentFontIndex
      {
        get
        {
          if ((int) this.Current.Depth <= 1 && this.stackTop != 0)
            return this.stack[this.stackTop - 1].FontIndex;
          return this.Current.FontIndex;
        }
        set
        {
          this.Current.FontIndex = value;
          if (this.stackTop <= 0)
            return;
          this.stack[this.stackTop - 1].FontIndex = value;
        }
      }

      public RtfState(int initialStackSize)
      {
        this.Level = 0;
        this.SkipLevel = 0;
        this.Current = new RtfToRtfConverter.RtfState.State();
        this.stack = new RtfToRtfConverter.RtfState.State[initialStackSize];
        this.stackTop = 0;
        this.Current.Dest = RtfDestination.RTF;
        this.Current.FontIndex = (short) 0;
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
        if (this.Level <= 0)
          return;
        --this.Current.Depth;
        if ((int) this.Current.Depth == 0 && this.stackTop != 0)
          this.Current = this.stack[--this.stackTop];
        --this.Level;
      }

      public void SkipGroup()
      {
        this.SkipLevel = this.Level;
      }

      private void PushReally()
      {
        if (this.stackTop >= 4096)
          throw new TextConvertersException(CtsResources.TextConvertersStrings.InputDocumentTooComplex);
        if (this.stack == null || this.stackTop == this.stack.Length)
        {
          RtfToRtfConverter.RtfState.State[] stateArray;
          if (this.stack != null)
          {
            stateArray = new RtfToRtfConverter.RtfState.State[this.stack.Length * 2];
            Array.Copy((Array) this.stack, 0, (Array) stateArray, 0, this.stackTop);
          }
          else
            stateArray = new RtfToRtfConverter.RtfState.State[4];
          this.stack = stateArray;
        }
        this.stack[this.stackTop] = this.Current;
        --this.stack[this.stackTop].Depth;
        ++this.stackTop;
        this.Current.Depth = (short) 1;
      }

      public struct State
      {
        public short Depth;
        public RtfDestination Dest;
        public short FontIndex;
      }
    }
  }
}
