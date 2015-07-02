// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.TextInjection
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters
{
  internal class TextInjection : Injection
  {
    private const int NextColorIndex = 0;
    protected Internal.Text.TextToTextConverter fragmentToTextConverter;
    protected IProgressMonitor progressMonitor;
    private int firstAvailableFontHandle;
    private Internal.Rtf.RtfOutput rtfOutput;
    private TextToRtfFragmentConverter fragmentToRtfConverter;
    private bool fontsWritten;
    private bool colorsWritten;

    public TextInjection(string injectHead, string injectTail, bool testBoundaryConditions, Stream traceStream, IProgressMonitor progressMonitor)
    {
      this.injectHead = injectHead;
      this.injectTail = injectTail;
      this.injectionFormat = HeaderFooterFormat.Text;
      this.testBoundaryConditions = testBoundaryConditions;
      this.traceStream = traceStream;
      this.progressMonitor = progressMonitor;
    }

    public override void Inject(bool head, Internal.Text.TextOutput output)
    {
      if (head)
      {
        if (this.injectHead == null || this.headInjected)
          return;
        if (this.fragmentToTextConverter == null)
          this.fragmentToTextConverter = new Internal.Text.TextToTextConverter(new Internal.Text.TextParser((ConverterInput) new ConverterBufferInput(this.injectHead, this.progressMonitor), false, false, 64, this.testBoundaryConditions), output, (Injection) null, true, false, this.traceStream, true, 0);
        else
          this.fragmentToTextConverter.Initialize(this.injectHead);
        do
          ;
        while (!this.fragmentToTextConverter.Flush());
        this.headInjected = true;
        if (this.injectTail != null)
          return;
        ((IDisposable) this.fragmentToTextConverter).Dispose();
        this.fragmentToTextConverter = (Internal.Text.TextToTextConverter) null;
      }
      else
      {
        if (this.injectHead != null && !this.headInjected)
          this.headInjected = true;
        if (this.injectTail == null || this.tailInjected)
          return;
        if (this.fragmentToTextConverter == null)
          this.fragmentToTextConverter = new Internal.Text.TextToTextConverter(new Internal.Text.TextParser((ConverterInput) new ConverterBufferInput(this.injectTail, this.progressMonitor), false, false, 64, this.testBoundaryConditions), output, (Injection) null, true, false, this.traceStream, true, 0);
        else
          this.fragmentToTextConverter.Initialize(this.injectTail);
        do
          ;
        while (!this.fragmentToTextConverter.Flush());
        ((IDisposable) this.fragmentToTextConverter).Dispose();
        this.fragmentToTextConverter = (Internal.Text.TextToTextConverter) null;
        this.tailInjected = true;
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.fragmentToTextConverter != null)
        {
          ((IDisposable) this.fragmentToTextConverter).Dispose();
          this.fragmentToTextConverter = (Internal.Text.TextToTextConverter) null;
        }
        if (this.fragmentToRtfConverter != null)
        {
          this.fragmentToRtfConverter.Dispose();
          this.fragmentToRtfConverter = (TextToRtfFragmentConverter) null;
        }
      }
      this.Reset();
      base.Dispose(disposing);
    }

    public override void CompileForRtf(Internal.Rtf.RtfOutput output)
    {
      this.rtfOutput = output;
    }

    public override void InjectRtf(bool head, bool immediatelyAfterText)
    {
      if (!this.fontsWritten)
      {
        this.rtfOutput.WriteControlText("{\\fonttbl", false);
        this.InjectRtfFonts(1);
        this.rtfOutput.WriteControlText("}", false);
      }
      if (!this.colorsWritten)
      {
        this.rtfOutput.WriteControlText("{\\colortbl;", false);
        this.InjectRtfColors(1);
        this.rtfOutput.WriteControlText("}", false);
      }
      if (head)
      {
        if (this.injectHead == null || this.headInjected)
          return;
        this.rtfOutput.WriteControlText("\\pard\\plain", true);
        this.rtfOutput.WriteKeyword("\\cf", 0);
        this.rtfOutput.WriteKeyword("\\f", this.firstAvailableFontHandle);
        this.rtfOutput.WriteControlText("\\fs24", true);
        if (this.fragmentToRtfConverter == null)
          this.fragmentToRtfConverter = new TextToRtfFragmentConverter(new Internal.Text.TextParser((ConverterInput) new ConverterBufferInput(this.injectHead, this.progressMonitor), false, false, 64, this.testBoundaryConditions), this.rtfOutput, this.traceStream, false, 0);
        else
          this.fragmentToRtfConverter.Initialize(this.injectHead);
        do
          ;
        while (!this.fragmentToRtfConverter.Flush());
        this.headInjected = true;
        this.rtfOutput.WriteControlText("\\par\r\n", false);
        this.rtfOutput.WriteControlText("\\plain\r\n", false);
        this.rtfOutput.RtfLineLength = 0;
        if (this.injectTail != null)
          return;
        this.fragmentToRtfConverter.Dispose();
        this.fragmentToRtfConverter = (TextToRtfFragmentConverter) null;
      }
      else
      {
        if (this.injectHead != null && !this.headInjected)
        {
          this.InjectRtf(true, false);
          this.headInjected = true;
        }
        if (this.injectTail == null || this.tailInjected)
          return;
        if (immediatelyAfterText)
          this.rtfOutput.WriteControlText("\\par\r\n", true);
        this.rtfOutput.WriteControlText("\\pard\\plain", true);
        this.rtfOutput.WriteKeyword("\\cf", 0);
        this.rtfOutput.WriteKeyword("\\f", this.firstAvailableFontHandle);
        this.rtfOutput.WriteControlText("\\fs24", true);
        if (this.fragmentToRtfConverter == null)
          this.fragmentToRtfConverter = new TextToRtfFragmentConverter(new Internal.Text.TextParser((ConverterInput) new ConverterBufferInput(this.injectTail, this.progressMonitor), false, false, 64, this.testBoundaryConditions), this.rtfOutput, this.traceStream, false, 0);
        else
          this.fragmentToRtfConverter.Initialize(this.injectTail);
        do
          ;
        while (!this.fragmentToRtfConverter.Flush());
        this.tailInjected = true;
        this.fragmentToRtfConverter.Dispose();
        this.fragmentToRtfConverter = (TextToRtfFragmentConverter) null;
      }
    }

    public override void InjectRtfFonts(int firstAvailableFontHandle)
    {
      Globalization.SimpleCodepageDetector codepageDetector = new Globalization.SimpleCodepageDetector();
      if (this.injectHead != null)
        codepageDetector.AddData(this.injectHead, 0, this.injectHead.Length);
      if (this.injectTail != null)
        codepageDetector.AddData(this.injectTail, 0, this.injectTail.Length);
      int windowsCodePage = codepageDetector.GetWindowsCodePage((int[]) null, true);
      Encoding encoding;
      if (!Globalization.Charset.TryGetEncoding(windowsCodePage, out encoding))
        encoding = Globalization.Charset.DefaultWindowsCharset.GetEncoding();
      this.rtfOutput.SetEncoding(encoding);
      int num = Internal.Rtf.RtfSupport.CharSetFromCodePage((ushort) windowsCodePage);
      this.firstAvailableFontHandle = firstAvailableFontHandle;
      this.rtfOutput.WriteControlText("{", false);
      this.rtfOutput.WriteKeyword("\\f", firstAvailableFontHandle);
      this.rtfOutput.WriteControlText("\\fmodern", true);
      this.rtfOutput.WriteKeyword("\\fcharset", num);
      this.rtfOutput.WriteControlText(" Courier New;}", false);
      this.fontsWritten = true;
    }

    public override void InjectRtfColors(int nextColorIndex)
    {
      this.rtfOutput.WriteControlText("\\red0\\green0\\blue0;", false);
      this.colorsWritten = true;
    }
  }
}
