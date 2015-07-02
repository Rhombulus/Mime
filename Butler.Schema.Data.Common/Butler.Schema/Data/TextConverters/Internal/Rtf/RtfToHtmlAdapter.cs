// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Rtf.RtfToHtmlAdapter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Rtf
{
  internal class RtfToHtmlAdapter : IProducerConsumer, IDisposable
  {
    private IProducerConsumer consumerOrProducer;
    private RtfParser parser;
    private ConverterOutput output;
    private RtfToHtml rtfToHtml;
    private int countTokens;
    private IProgressMonitor progressMonitor;

    public RtfToHtmlAdapter(RtfParser parser, ConverterOutput output, RtfToHtml rtfToHtml, IProgressMonitor progressMonitor)
    {
      this.parser = parser;
      this.output = output;
      this.rtfToHtml = rtfToHtml;
      this.progressMonitor = progressMonitor;
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
      if (this.consumerOrProducer != null)
        this.consumerOrProducer.Run();
      else
        this.ParseAndWatch();
    }

    public bool Flush()
    {
      if (this.consumerOrProducer != null)
        return this.consumerOrProducer.Flush();
      this.Run();
      return false;
    }

    private void ParseAndWatch()
    {
      while (!this.parser.ParseRun())
      {
        if (this.parser.ParseBufferFull)
        {
          this.Restart(RtfEncapsulation.None);
          return;
        }
        if (!this.parser.ReadMoreData(false))
          return;
      }
      switch (this.parser.RunKind)
      {
        case RtfRunKind.Ignore:
          break;
        case RtfRunKind.Begin:
          if (this.countTokens++ == 0)
            break;
          this.Restart(RtfEncapsulation.None);
          break;
        case RtfRunKind.Keyword:
          if (this.countTokens++ > 10)
          {
            this.Restart(RtfEncapsulation.None);
            break;
          }
          if ((int) this.parser.KeywordId == 292)
          {
            if (this.parser.KeywordValue >= 1)
            {
              this.Restart(RtfEncapsulation.Html);
              break;
            }
            this.Restart(RtfEncapsulation.None);
            break;
          }
          if ((int) this.parser.KeywordId != 329)
            break;
          this.Restart(RtfEncapsulation.Text);
          break;
        default:
          this.Restart(RtfEncapsulation.None);
          break;
      }
    }

    private void Restart(RtfEncapsulation encapsulation)
    {
      this.parser.Restart();
      this.consumerOrProducer = this.rtfToHtml.CreateChain(encapsulation, this.parser, this.output, this.progressMonitor);
    }
  }
}
