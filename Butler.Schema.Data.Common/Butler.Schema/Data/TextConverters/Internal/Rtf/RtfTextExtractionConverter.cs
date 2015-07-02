// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Rtf.RtfTextExtractionConverter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Rtf
{
  internal class RtfTextExtractionConverter : IProducerConsumer, IDisposable
  {
    private int invisibleLevel = int.MaxValue;
    private RtfParser parser;
    private bool endOfFile;
    private ConverterOutput output;
    private bool firstKeyword;
    private bool ignorableDestination;
    private int level;
    private int skipLevel;

    public RtfTextExtractionConverter(RtfParser parser, ConverterOutput output, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum)
    {
      this.output = output;
      this.parser = parser;
    }

    void IDisposable.Dispose()
    {
      if (this.parser != null && this.parser is IDisposable)
        ((IDisposable) this.parser).Dispose();
      if (this.output != null && this.output != null)
        this.output.Dispose();
      this.parser = (RtfParser) null;
      this.output = (ConverterOutput) null;
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
      if (!this.endOfFile)
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
      ++this.level;
      this.firstKeyword = true;
      this.ignorableDestination = false;
    }

    private void ProcessEndGroup()
    {
      if (this.skipLevel != 0)
      {
        if (this.level != this.skipLevel)
        {
          --this.level;
          return;
        }
        this.skipLevel = 0;
      }
      this.firstKeyword = false;
      if (this.level <= 1)
        return;
      --this.level;
      if (this.invisibleLevel == int.MaxValue || this.level >= this.invisibleLevel)
        return;
      this.invisibleLevel = int.MaxValue;
    }

    private void ProcessKeywords(RtfToken token)
    {
      if (this.skipLevel != 0 && this.level >= this.skipLevel)
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
            case (short) byte.MaxValue:
            case (short) 257:
            case (short) 268:
            case (short) 315:
            case (short) 316:
            case (short) 319:
            case (short) 210:
            case (short) 246:
            case (short) 252:
            case (short) 50:
            case (short) 175:
            case (short) 201:
            case (short) 8:
            case (short) 15:
            case (short) 24:
              this.skipLevel = this.level;
              return;
            default:
              if (this.ignorableDestination)
              {
                this.skipLevel = this.level;
                return;
              }
              break;
          }
        }
        switch (rtfKeyword.Id)
        {
          case (short) 119:
          case (short) 126:
            this.output.Write("\t");
            continue;
          case (short) 136:
            this.invisibleLevel = rtfKeyword.Value != 0 ? this.level : int.MaxValue;
            continue;
          case (short) 40:
          case (short) 48:
          case (short) 68:
            this.output.Write("\r\n");
            continue;
          default:
            continue;
        }
      }
    }

    private void ProcessText(RtfToken token)
    {
      if (this.skipLevel != 0 && this.level >= this.skipLevel)
        return;
      this.firstKeyword = false;
      if (this.level >= this.invisibleLevel)
        return;
      token.Text.WriteTo((ITextSink) this.output);
    }

    private void ProcessEOF()
    {
      this.output.Write("\r\n");
      this.output.Flush();
      this.endOfFile = true;
    }
  }
}
