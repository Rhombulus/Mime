// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.HtmlInjection
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  internal class HtmlInjection : Injection
  {
    protected bool filterHtml;
    protected HtmlTagCallback callback;
    protected bool injectingHead;
    protected IProgressMonitor progressMonitor;
    protected Internal.Html.IHtmlParser documentParser;
    protected Internal.Html.HtmlParser fragmentParser;
    protected Internal.Html.HtmlToHtmlConverter fragmentToHtmlConverter;
    protected Internal.Html.HtmlToTextConverter fragmentToTextConverter;
    private HtmlFragmentToRtfConverter fragmentToRtfConverter;

    public bool Active
    {
      get
      {
        return this.documentParser != null;
      }
    }

    public bool InjectingHead
    {
      get
      {
        return this.injectingHead;
      }
    }

    public HtmlInjection(string injectHead, string injectTail, HeaderFooterFormat injectionFormat, bool filterHtml, HtmlTagCallback callback, bool testBoundaryConditions, Stream traceStream, IProgressMonitor progressMonitor)
    {
      this.injectHead = injectHead;
      this.injectTail = injectTail;
      this.injectionFormat = injectionFormat;
      this.filterHtml = filterHtml;
      this.callback = callback;
      this.testBoundaryConditions = testBoundaryConditions;
      this.progressMonitor = progressMonitor;
    }

    public Internal.Html.IHtmlParser Push(bool head, Internal.Html.IHtmlParser documentParser)
    {
      if (head)
      {
        if (this.injectHead != null && !this.headInjected)
        {
          this.documentParser = documentParser;
          if (this.fragmentParser == null)
            this.fragmentParser = new Internal.Html.HtmlParser((ConverterInput) new ConverterBufferInput(this.injectHead, this.progressMonitor), false, this.injectionFormat == HeaderFooterFormat.Text, 64, 8, this.testBoundaryConditions);
          else
            this.fragmentParser.Initialize(this.injectHead, this.injectionFormat == HeaderFooterFormat.Text);
          this.injectingHead = true;
          return (Internal.Html.IHtmlParser) this.fragmentParser;
        }
      }
      else
      {
        if (this.injectHead != null && !this.headInjected)
          this.headInjected = true;
        if (this.injectTail != null && !this.tailInjected)
        {
          this.documentParser = documentParser;
          if (this.fragmentParser == null)
            this.fragmentParser = new Internal.Html.HtmlParser((ConverterInput) new ConverterBufferInput(this.injectTail, this.progressMonitor), false, this.injectionFormat == HeaderFooterFormat.Text, 64, 8, this.testBoundaryConditions);
          else
            this.fragmentParser.Initialize(this.injectTail, this.injectionFormat == HeaderFooterFormat.Text);
          this.injectingHead = false;
          return (Internal.Html.IHtmlParser) this.fragmentParser;
        }
      }
      return documentParser;
    }

    public Internal.Html.IHtmlParser Pop()
    {
      if (this.injectingHead)
      {
        this.headInjected = true;
        if (this.injectTail == null)
        {
          ((IDisposable) this.fragmentParser).Dispose();
          this.fragmentParser = (Internal.Html.HtmlParser) null;
        }
      }
      else
      {
        this.tailInjected = true;
        ((IDisposable) this.fragmentParser).Dispose();
        this.fragmentParser = (Internal.Html.HtmlParser) null;
      }
      Internal.Html.IHtmlParser htmlParser = this.documentParser;
      this.documentParser = (Internal.Html.IHtmlParser) null;
      return htmlParser;
    }

    public void Inject(bool head, HtmlWriter writer)
    {
      if (head)
      {
        if (this.injectHead == null || this.headInjected)
          return;
        if (this.injectionFormat == HeaderFooterFormat.Text)
        {
          writer.WriteStartTag(Internal.Html.HtmlNameIndex.TT);
          writer.WriteStartTag(Internal.Html.HtmlNameIndex.Pre);
          writer.WriteNewLine();
        }
        this.CreateHtmlToHtmlConverter(this.injectHead, writer);
        do
          ;
        while (!this.fragmentToHtmlConverter.Flush());
        this.headInjected = true;
        if (this.injectTail == null)
        {
          ((IDisposable) this.fragmentToHtmlConverter).Dispose();
          this.fragmentToHtmlConverter = (Internal.Html.HtmlToHtmlConverter) null;
        }
        if (this.injectionFormat != HeaderFooterFormat.Text)
          return;
        writer.WriteEndTag(Internal.Html.HtmlNameIndex.Pre);
        writer.WriteEndTag(Internal.Html.HtmlNameIndex.TT);
      }
      else
      {
        if (this.injectHead != null && !this.headInjected)
          this.headInjected = true;
        if (this.injectTail == null || this.tailInjected)
          return;
        if (this.injectionFormat == HeaderFooterFormat.Text)
        {
          writer.WriteStartTag(Internal.Html.HtmlNameIndex.TT);
          writer.WriteStartTag(Internal.Html.HtmlNameIndex.Pre);
          writer.WriteNewLine();
        }
        if (this.fragmentToHtmlConverter == null)
          this.CreateHtmlToHtmlConverter(this.injectTail, writer);
        else
          this.fragmentToHtmlConverter.Initialize(this.injectTail, this.injectionFormat == HeaderFooterFormat.Text);
        do
          ;
        while (!this.fragmentToHtmlConverter.Flush());
        ((IDisposable) this.fragmentToHtmlConverter).Dispose();
        this.fragmentToHtmlConverter = (Internal.Html.HtmlToHtmlConverter) null;
        this.tailInjected = true;
        if (this.injectionFormat != HeaderFooterFormat.Text)
          return;
        writer.WriteEndTag(Internal.Html.HtmlNameIndex.Pre);
        writer.WriteEndTag(Internal.Html.HtmlNameIndex.TT);
      }
    }

    private void CreateHtmlToHtmlConverter(string fragment, HtmlWriter writer)
    {
      Internal.Html.HtmlParser parser1 = new Internal.Html.HtmlParser((ConverterInput) new ConverterBufferInput(fragment, this.progressMonitor), false, this.injectionFormat == HeaderFooterFormat.Text, 64, 8, this.testBoundaryConditions);
      Internal.Html.IHtmlParser parser2 = (Internal.Html.IHtmlParser) parser1;
      if (this.injectionFormat == HeaderFooterFormat.Html)
        parser2 = (Internal.Html.IHtmlParser) new Internal.Html.HtmlNormalizingParser(parser1, (HtmlInjection) null, false, 4096, this.testBoundaryConditions, (Stream) null, true, 0);
      this.fragmentToHtmlConverter = new Internal.Html.HtmlToHtmlConverter(parser2, writer, true, this.injectionFormat == HeaderFooterFormat.Html, this.filterHtml, this.callback, true, false, this.traceStream, true, 0, -1, false, this.progressMonitor);
    }

    public override void Inject(bool head, Internal.Text.TextOutput output)
    {
      if (head)
      {
        if (this.injectHead == null || this.headInjected)
          return;
        this.fragmentToTextConverter = new Internal.Html.HtmlToTextConverter((Internal.Html.IHtmlParser) new Internal.Html.HtmlParser((ConverterInput) new ConverterBufferInput(this.injectHead, this.progressMonitor), false, this.injectionFormat == HeaderFooterFormat.Text, 64, 8, this.testBoundaryConditions), output, (Injection) null, true, this.injectionFormat == HeaderFooterFormat.Text, false, this.traceStream, true, 0, false, true, true);
        do
          ;
        while (!this.fragmentToTextConverter.Flush());
        this.headInjected = true;
        if (this.injectTail != null)
          return;
        ((IDisposable) this.fragmentToTextConverter).Dispose();
        this.fragmentToTextConverter = (Internal.Html.HtmlToTextConverter) null;
      }
      else
      {
        if (this.injectHead != null && !this.headInjected)
          this.headInjected = true;
        if (this.injectTail == null || this.tailInjected)
          return;
        if (this.fragmentToTextConverter == null)
          this.fragmentToTextConverter = new Internal.Html.HtmlToTextConverter((Internal.Html.IHtmlParser) new Internal.Html.HtmlParser((ConverterInput) new ConverterBufferInput(this.injectTail, this.progressMonitor), false, this.injectionFormat == HeaderFooterFormat.Text, 64, 8, this.testBoundaryConditions), output, (Injection) null, true, this.injectionFormat == HeaderFooterFormat.Text, false, this.traceStream, true, 0, false, true, true);
        else
          this.fragmentToTextConverter.Initialize(this.injectTail, this.injectionFormat == HeaderFooterFormat.Text);
        do
          ;
        while (!this.fragmentToTextConverter.Flush());
        ((IDisposable) this.fragmentToTextConverter).Dispose();
        this.fragmentToTextConverter = (Internal.Html.HtmlToTextConverter) null;
        this.tailInjected = true;
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (this.fragmentToHtmlConverter != null)
      {
        ((IDisposable) this.fragmentToHtmlConverter).Dispose();
        this.fragmentToHtmlConverter = (Internal.Html.HtmlToHtmlConverter) null;
      }
      if (this.fragmentToTextConverter != null)
      {
        ((IDisposable) this.fragmentToTextConverter).Dispose();
        this.fragmentToTextConverter = (Internal.Html.HtmlToTextConverter) null;
      }
      if (this.fragmentParser != null)
      {
        ((IDisposable) this.fragmentParser).Dispose();
        this.fragmentParser = (Internal.Html.HtmlParser) null;
      }
      this.Reset();
      base.Dispose(disposing);
    }

    public override void CompileForRtf(Internal.Rtf.RtfOutput output)
    {
      if (this.fragmentToRtfConverter == null)
      {
        if (this.injectHead != null)
        {
          this.fragmentToRtfConverter = new HtmlFragmentToRtfConverter(output, this.traceStream, this.progressMonitor);
          this.fragmentToRtfConverter.PrepareHead(this.injectHead);
        }
        if (this.injectTail != null)
        {
          if (this.fragmentToRtfConverter == null)
            this.fragmentToRtfConverter = new HtmlFragmentToRtfConverter(output, this.traceStream, this.progressMonitor);
          this.fragmentToRtfConverter.PrepareTail(this.injectTail);
        }
      }
      this.fragmentToRtfConverter.EndPrepare();
    }

    public override void InjectRtfFonts(int firstAvailableFontHandle)
    {
      if (this.fragmentToRtfConverter == null)
        return;
      this.fragmentToRtfConverter.InjectFonts(firstAvailableFontHandle);
    }

    public override void InjectRtfColors(int nextColorIndex)
    {
      if (this.fragmentToRtfConverter == null)
        return;
      this.fragmentToRtfConverter.InjectColors(nextColorIndex);
    }

    public override void InjectRtf(bool head, bool immediatelyAfterText)
    {
      if (head)
      {
        if (this.injectHead == null || this.headInjected)
          return;
        this.fragmentToRtfConverter.InjectHead();
        this.headInjected = true;
      }
      else
      {
        if (this.injectHead != null && !this.headInjected)
        {
          this.fragmentToRtfConverter.InjectHead();
          this.headInjected = true;
        }
        if (this.injectTail == null || this.tailInjected)
          return;
        this.fragmentToRtfConverter.InjectTail(immediatelyAfterText);
        this.tailInjected = true;
      }
    }
  }
}
