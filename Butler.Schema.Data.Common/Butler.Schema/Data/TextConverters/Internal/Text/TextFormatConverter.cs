// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Text.TextFormatConverter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Text
{
  internal class TextFormatConverter : Format.FormatConverter, IProducerConsumer, IDisposable
  {
    protected TextParser parser;
    private Format.FormatOutput output;
    protected int lineLength;
    protected int newLines;
    protected int spaces;
    protected int nbsps;
    protected bool paragraphStarted;
    protected Injection injection;

    private bool CanFlush
    {
      get
      {
        return this.output.CanAcceptMoreOutput;
      }
    }

    public TextFormatConverter(TextParser parser, Format.FormatOutput output, Injection injection, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum, Stream formatConverterTraceStream)
      : base(formatConverterTraceStream)
    {
      this.parser = parser;
      this.output = output;
      if (this.output != null)
        this.output.Initialize(this.Store, Format.SourceFormat.Text, "converted from text");
      this.injection = injection;
      this.InitializeDocument();
      if (this.injection != null)
      {
        int num = this.injection.HaveHead ? 1 : 0;
      }
      this.OpenContainer(Format.FormatContainerType.Block, false);
      this.Last.SetProperty(Format.PropertyPrecedence.NonStyle, Format.PropertyId.FontSize, new Format.PropertyValue(Format.LengthUnits.Points, 10));
    }

    public TextFormatConverter(TextParser parser, Format.FormatStore store, Injection injection, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum, Stream formatConverterTraceStream)
      : base(store, formatConverterTraceStream)
    {
      this.parser = parser;
      this.injection = injection;
      this.InitializeDocument();
      this.OpenContainer(Format.FormatContainerType.Block, false);
      this.Last.SetProperty(Format.PropertyPrecedence.NonStyle, Format.PropertyId.FontSize, new Format.PropertyValue(Format.LengthUnits.Points, 10));
    }

    public void Initialize(string fragment)
    {
      this.parser.Initialize(fragment);
      this.lineLength = 0;
      this.newLines = 0;
      this.spaces = 0;
      this.nbsps = 0;
      this.paragraphStarted = false;
    }

    public override void Run()
    {
      if (this.output != null && this.MustFlush)
      {
        if (!this.CanFlush)
          return;
        this.FlushOutput();
      }
      else
      {
        if (this.EndOfFile)
          return;
        TextTokenId tokenId = this.parser.Parse();
        if (tokenId == TextTokenId.None)
          return;
        this.Process(tokenId);
      }
    }

    public bool Flush()
    {
      this.Run();
      if (this.EndOfFile)
        return !this.MustFlush;
      return false;
    }

    void IDisposable.Dispose()
    {
      if (this.parser != null)
        ((IDisposable) this.parser).Dispose();
      this.parser = (TextParser) null;
      GC.SuppressFinalize((object) this);
    }

    private bool FlushOutput()
    {
      if (!this.output.Flush())
        return false;
      this.MustFlush = false;
      return true;
    }

    protected void Process(TextTokenId tokenId)
    {
      switch (tokenId)
      {
        case TextTokenId.EndOfFile:
          if (this.injection != null && this.injection.HaveTail)
            this.AddLineBreak(1);
          this.CloseContainer();
          this.CloseAllContainersAndSetEOF();
          break;
        case TextTokenId.Text:
          this.OutputFragmentSimple(this.parser.Token);
          break;
        case TextTokenId.EncodingChange:
          if (this.output == null || !this.output.OutputCodePageSameAsInput)
            break;
          this.output.OutputEncoding = this.parser.Token.TokenEncoding;
          break;
      }
    }

    private void OutputFragmentSimple(TextToken token)
    {
      Token.RunEnumerator enumerator = token.Runs.GetEnumerator();
      while (enumerator.MoveNext())
      {
        TokenRun current = enumerator.Current;
        if (current.IsTextRun)
        {
          switch (current.TextType)
          {
            case RunTextType.Nbsp:
              this.AddNbsp(current.Length);
              continue;
            case RunTextType.NonSpace:
            case RunTextType.Unknown:
              this.AddNonSpaceText(current.RawBuffer, current.RawOffset, current.RawLength);
              continue;
            case RunTextType.Tabulation:
              this.AddTabulation(current.Length);
              continue;
            case RunTextType.UnusualWhitespace:
            case RunTextType.Space:
              this.AddSpace(current.Length);
              continue;
            case RunTextType.NewLine:
              this.AddLineBreak(1);
              continue;
            default:
              continue;
          }
        }
        else if (current.IsSpecial)
        {
          int num = (int) current.Kind;
        }
      }
    }
  }
}
