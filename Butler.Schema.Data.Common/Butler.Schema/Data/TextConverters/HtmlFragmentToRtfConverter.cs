// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.HtmlFragmentToRtfConverter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  internal class HtmlFragmentToRtfConverter
  {
    private Internal.Format.FormatStore formatStore;
    private Internal.Format.FormatNode headFragmentNode;
    private Internal.Format.FormatNode tailFragmentNode;
    private Internal.Html.HtmlFormatConverter converter;
    private Internal.Rtf.RtfFormatOutput formatOutput;
    private Internal.Rtf.RtfOutput output;
    private bool colorsWritten;
    private bool fontsWritten;

    public HtmlFragmentToRtfConverter(Internal.Rtf.RtfOutput output, Stream traceStream, IProgressMonitor progressMonitor)
    {
      Internal.Html.HtmlNormalizingParser parser = new Internal.Html.HtmlNormalizingParser(new Internal.Html.HtmlParser((ConverterInput) new ConverterBufferInput(string.Empty, progressMonitor), false, false, 64, 8, false), (HtmlInjection) null, false, 4096, false, (Stream) null, true, 0);
      this.formatStore = new Internal.Format.FormatStore();
      this.formatStore.InitializeCodepageDetector();
      this.converter = new Internal.Html.HtmlFormatConverter(parser, this.formatStore, true, false, traceStream, true, 0, (Stream) null, progressMonitor);
      this.output = output;
    }

    public void PrepareHead(string headHtml)
    {
      this.headFragmentNode = this.converter.Initialize(headHtml);
      this.converter.ConvertToStore();
    }

    public void PrepareTail(string tailHtml)
    {
      this.tailFragmentNode = this.converter.Initialize(tailHtml);
      this.converter.ConvertToStore();
    }

    public void EndPrepare()
    {
      this.converter = (Internal.Html.HtmlFormatConverter) null;
      this.formatOutput = new Internal.Rtf.RtfFormatOutput(this.output, (Stream) null, (Stream) null);
      this.formatOutput.Initialize(this.formatStore, Internal.Format.SourceFormat.Html, (string) null);
    }

    public void InjectFonts(int firstAvailableFontHandle)
    {
      this.formatOutput.OutputFonts(firstAvailableFontHandle);
      this.fontsWritten = true;
    }

    public void InjectColors(int nextColorIndex)
    {
      this.formatOutput.OutputColors(nextColorIndex);
      this.colorsWritten = true;
    }

    public void InjectHead()
    {
      if (!this.fontsWritten)
      {
        this.output.WriteControlText("{\\fonttbl", false);
        this.InjectFonts(1);
        this.output.WriteControlText("}", false);
      }
      if (!this.colorsWritten)
      {
        this.output.WriteControlText("{\\colortbl;", false);
        this.InjectColors(1);
        this.output.WriteControlText("}", false);
      }
      this.InjectFragment(this.headFragmentNode);
      this.output.WriteControlText("\\pard\\par\r\n", false);
      this.output.RtfLineLength = 0;
    }

    public void InjectTail(bool immediatelyAfterText)
    {
      if (!this.fontsWritten)
      {
        this.output.WriteControlText("{\\fonttbl", false);
        this.InjectFonts(1);
        this.output.WriteControlText("}", false);
      }
      if (!this.colorsWritten)
      {
        this.output.WriteControlText("{\\colortbl;", false);
        this.InjectColors(1);
        this.output.WriteControlText("}", false);
      }
      if (immediatelyAfterText)
        this.output.WriteControlText("\\pard\\par\r\n", true);
      this.output.WriteControlText("\\pard\\plain", true);
      this.InjectFragment(this.tailFragmentNode);
    }

    private void InjectFragment(Internal.Format.FormatNode fragmentNode)
    {
      this.formatOutput.OutputFragment(fragmentNode);
    }
  }
}
