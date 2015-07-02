// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Text.TextToTextConverter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Text
{
  internal class TextToTextConverter : IProducerConsumer, IDisposable
  {
    protected TextParser parser;
    protected bool endOfFile;
    protected TextOutput output;
    protected bool convertFragment;
    protected int lineLength;
    protected int newLines;
    protected int spaces;
    protected int nbsps;
    protected bool paragraphStarted;
    protected bool treatNbspAsBreakable;
    private bool started;
    protected Injection injection;

    public TextToTextConverter(TextParser parser, TextOutput output, Injection injection, bool convertFragment, bool treatNbspAsBreakable, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum)
    {
      this.treatNbspAsBreakable = treatNbspAsBreakable;
      this.convertFragment = convertFragment;
      this.output = output;
      this.parser = parser;
      this.injection = injection;
    }

    public void Initialize(string fragment)
    {
      this.parser.Initialize(fragment);
      this.endOfFile = false;
      this.lineLength = 0;
      this.newLines = 0;
      this.spaces = 0;
      this.nbsps = 0;
      this.paragraphStarted = false;
      this.started = false;
    }

    public void Run()
    {
      if (this.endOfFile)
        return;
      TextTokenId tokenId = this.parser.Parse();
      if (tokenId == TextTokenId.None)
        return;
      this.Process(tokenId);
    }

    public bool Flush()
    {
      if (!this.endOfFile)
        this.Run();
      return this.endOfFile;
    }

    protected void Process(TextTokenId tokenId)
    {
      if (!this.started)
      {
        if (!this.convertFragment)
        {
          this.output.OpenDocument();
          if (this.injection != null && this.injection.HaveHead)
            this.injection.Inject(true, this.output);
        }
        this.output.SetQuotingLevel(0);
        this.started = true;
      }
      switch (tokenId)
      {
        case TextTokenId.EndOfFile:
          if (!this.convertFragment)
          {
            if (this.injection != null && this.injection.HaveTail)
            {
              if (!this.output.LineEmpty)
                this.output.OutputNewLine();
              this.injection.Inject(false, this.output);
            }
            this.output.Flush();
          }
          else
            this.output.CloseParagraph();
          this.endOfFile = true;
          break;
        case TextTokenId.Text:
          this.OutputFragmentSimple(this.parser.Token);
          break;
        case TextTokenId.EncodingChange:
          if (this.convertFragment || !this.output.OutputCodePageSameAsInput)
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
            case RunTextType.UnusualWhitespace:
            case RunTextType.Space:
              this.output.OutputSpace(current.Length);
              continue;
            case RunTextType.Nbsp:
              if (this.treatNbspAsBreakable)
              {
                this.output.OutputSpace(current.Length);
                continue;
              }
              this.output.OutputNbsp(current.Length);
              continue;
            case RunTextType.NonSpace:
              this.output.OutputNonspace(current.RawBuffer, current.RawOffset, current.RawLength, TextMapping.Unicode);
              continue;
            case RunTextType.NewLine:
              this.output.OutputNewLine();
              continue;
            case RunTextType.Tabulation:
              this.output.OutputTabulation(current.Length);
              continue;
            default:
              continue;
          }
        }
        else if (current.IsSpecial && (int) current.Kind == 167772160)
          this.output.SetQuotingLevel((int) (ushort) current.Value);
      }
    }

    void IDisposable.Dispose()
    {
      if (this.parser != null)
        ((IDisposable) this.parser).Dispose();
      if (!this.convertFragment && this.output != null && this.output != null)
        ((IDisposable) this.output).Dispose();
      if (this.injection != null)
        ((IDisposable) this.injection).Dispose();
      this.parser = (TextParser) null;
      this.output = (TextOutput) null;
      this.injection = (Injection) null;
      GC.SuppressFinalize((object) this);
    }
  }
}
