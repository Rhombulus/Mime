// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.TextToRtfFragmentConverter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  internal class TextToRtfFragmentConverter : IProducerConsumer, IDisposable
  {
    protected Internal.Text.TextParser parser;
    protected bool endOfFile;
    protected Internal.Rtf.RtfOutput output;

    public TextToRtfFragmentConverter(Internal.Text.TextParser parser, Internal.Rtf.RtfOutput output, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum)
    {
      this.output = output;
      this.parser = parser;
    }

    public void Initialize(string fragment)
    {
      this.parser.Initialize(fragment);
      this.endOfFile = false;
    }

    public void Run()
    {
      if (this.endOfFile)
        return;
      Internal.Text.TextTokenId tokenId = this.parser.Parse();
      if (tokenId == Internal.Text.TextTokenId.None)
        return;
      this.Process(tokenId);
    }

    public bool Flush()
    {
      if (!this.endOfFile)
        this.Run();
      return this.endOfFile;
    }

    protected void Process(Internal.Text.TextTokenId tokenId)
    {
      switch (tokenId)
      {
        case Internal.Text.TextTokenId.EndOfFile:
          this.endOfFile = true;
          break;
        case Internal.Text.TextTokenId.Text:
          Token.RunEnumerator enumerator = this.parser.Token.Runs.GetEnumerator();
          while (enumerator.MoveNext())
          {
            TokenRun current = enumerator.Current;
            if (current.IsTextRun)
            {
              switch (current.TextType)
              {
                case RunTextType.UnusualWhitespace:
                case RunTextType.Space:
                  for (int index = 0; index < current.Length; ++index)
                    this.output.WriteText(" ");
                  continue;
                case RunTextType.Nbsp:
                  for (int index = 0; index < current.Length; ++index)
                    this.output.WriteText(" ");
                  continue;
                case RunTextType.NonSpace:
                  this.output.WriteText(current.RawBuffer, current.RawOffset, current.RawLength);
                  continue;
                case RunTextType.NewLine:
                  this.output.WriteControlText("\\line\r\n", false);
                  continue;
                case RunTextType.Tabulation:
                  for (int index = 0; index < current.Length; ++index)
                    this.output.WriteControlText("\\tab", true);
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
          break;
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing && this.parser != null)
        ((IDisposable) this.parser).Dispose();
      this.parser = (Internal.Text.TextParser) null;
      this.output = (Internal.Rtf.RtfOutput) null;
    }
  }
}
