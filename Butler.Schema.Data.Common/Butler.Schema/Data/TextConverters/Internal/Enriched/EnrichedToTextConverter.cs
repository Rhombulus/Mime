// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Enriched.EnrichedToTextConverter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Enriched
{
  internal class EnrichedToTextConverter : IProducerConsumer, IDisposable
  {
    private EnrichedParser parser;
    private bool endOfFile;
    private Text.TextOutput output;
    private Html.HtmlToken token;
    private bool treatNbspAsBreakable;
    private bool insideParam;
    private int quotingLevel;
    private Injection injection;

    public EnrichedToTextConverter(EnrichedParser parser, Text.TextOutput output, Injection injection, bool testTreatNbspAsBreakable, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum)
    {
      this.treatNbspAsBreakable = testTreatNbspAsBreakable;
      this.output = output;
      this.parser = parser;
      this.injection = injection;
      this.output.OpenDocument();
      if (this.injection == null || !this.injection.HaveHead)
        return;
      this.injection.Inject(true, this.output);
    }

    public void Run()
    {
      if (this.endOfFile)
        return;
      Html.HtmlTokenId tokenId = this.parser.Parse();
      if (tokenId == Html.HtmlTokenId.None)
        return;
      this.Process(tokenId);
    }

    public bool Flush()
    {
      if (!this.endOfFile)
        this.Run();
      return this.endOfFile;
    }

    private void Process(Html.HtmlTokenId tokenId)
    {
      this.token = this.parser.Token;
      switch (tokenId)
      {
        case Html.HtmlTokenId.EndOfFile:
          if (this.injection != null && this.injection.HaveTail)
          {
            if (!this.output.LineEmpty)
              this.output.OutputNewLine();
            this.injection.Inject(false, this.output);
          }
          this.output.CloseDocument();
          this.output.Flush();
          this.endOfFile = true;
          break;
        case Html.HtmlTokenId.Text:
          if (this.insideParam)
            break;
          this.OutputText(this.token);
          break;
        case Html.HtmlTokenId.EncodingChange:
          if (!this.output.OutputCodePageSameAsInput)
            break;
          this.output.OutputEncoding = this.token.TokenEncoding;
          break;
        case Html.HtmlTokenId.Tag:
          if (!this.token.IsTagBegin)
            break;
          Html.HtmlNameIndex htmlNameIndex = this.token.NameIndex;
          if ((uint) htmlNameIndex <= 105U)
          {
            if ((uint) htmlNameIndex <= 24U)
            {
              if (htmlNameIndex != Html.HtmlNameIndex.Nofill && htmlNameIndex != Html.HtmlNameIndex.FlushRight)
              {
                if (htmlNameIndex != Html.HtmlNameIndex.Param)
                  break;
                this.insideParam = !this.token.IsEndTag;
                break;
              }
            }
            else
            {
              if ((uint) htmlNameIndex <= 65U)
              {
                if (htmlNameIndex == Html.HtmlNameIndex.Color || htmlNameIndex == Html.HtmlNameIndex.Italic)
                  break;
                break;
              }
              switch (htmlNameIndex)
              {
                case Html.HtmlNameIndex.Center:
                case Html.HtmlNameIndex.FlushBoth:
                case Html.HtmlNameIndex.ParaIndent:
                  break;
                case Html.HtmlNameIndex.Height:
                  return;
                case Html.HtmlNameIndex.Underline:
                  return;
                default:
                  return;
              }
            }
          }
          else if ((uint) htmlNameIndex <= 148U)
          {
            switch (htmlNameIndex)
            {
              case Html.HtmlNameIndex.Fixed:
                return;
              case Html.HtmlNameIndex.Smaller:
                return;
              case Html.HtmlNameIndex.Bold:
                return;
              case Html.HtmlNameIndex.Strike:
                return;
              case Html.HtmlNameIndex.FlushLeft:
                break;
              default:
                return;
            }
          }
          else
          {
            if ((uint) htmlNameIndex <= 159U)
            {
              if (htmlNameIndex != Html.HtmlNameIndex.Excerpt)
              {
                if (htmlNameIndex != Html.HtmlNameIndex.Lang)
                  break;
                break;
              }
              if (!this.output.LineEmpty)
                this.output.OutputNewLine();
              if (!this.token.IsEndTag)
                ++this.quotingLevel;
              else
                --this.quotingLevel;
              this.output.SetQuotingLevel(this.quotingLevel);
              break;
            }
            if (htmlNameIndex == Html.HtmlNameIndex.Bigger || htmlNameIndex == Html.HtmlNameIndex.FontFamily)
              break;
            break;
          }
          if (this.output.LineEmpty)
            break;
          this.output.OutputNewLine();
          break;
      }
    }

    private void OutputText(Html.HtmlToken token)
    {
      Token.RunEnumerator enumerator = token.Runs.GetEnumerator();
      while (enumerator.MoveNext())
      {
        TokenRun current = enumerator.Current;
        if (current.IsTextRun)
        {
          if (current.IsAnyWhitespace)
          {
            switch (current.TextType)
            {
              case RunTextType.NewLine:
                this.output.OutputNewLine();
                continue;
              case RunTextType.Tabulation:
                this.output.OutputTabulation(current.Length);
                continue;
              default:
                this.output.OutputSpace(current.Length);
                continue;
            }
          }
          else if (current.TextType == RunTextType.Nbsp)
          {
            if (this.treatNbspAsBreakable)
              this.output.OutputSpace(current.Length);
            else
              this.output.OutputNbsp(current.Length);
          }
          else if (current.IsLiteral)
            this.output.OutputNonspace(current.Literal, TextMapping.Unicode);
          else
            this.output.OutputNonspace(current.RawBuffer, current.RawOffset, current.RawLength, TextMapping.Unicode);
        }
      }
    }

    void IDisposable.Dispose()
    {
      if (this.parser != null)
        this.parser.Dispose();
      if (this.output != null && this.output != null)
        ((IDisposable) this.output).Dispose();
      if (this.token != null && this.token is IDisposable)
        ((IDisposable) this.token).Dispose();
      this.parser = (EnrichedParser) null;
      this.output = (Text.TextOutput) null;
      this.token = (Html.HtmlToken) null;
      GC.SuppressFinalize((object) this);
    }
  }
}
