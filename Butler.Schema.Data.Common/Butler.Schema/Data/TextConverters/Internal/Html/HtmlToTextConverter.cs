// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Html.HtmlToTextConverter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Html
{
  internal class HtmlToTextConverter : IProducerConsumer, IRestartable, IReusable, IDisposable
  {
    private bool nextParagraphCloseWideGap = true;
    private readonly bool outputImageLinks;
    private readonly bool outputAnchorLinks;
    private readonly bool shouldUseNarrowGapForPTagHtmlToTextConversion;
    private bool convertFragment;
    private IHtmlParser parser;
    private bool endOfFile;
    private Text.TextOutput output;
    private HtmlToken token;
    private bool treatNbspAsBreakable;
    protected bool normalizedInput;
    private HtmlToTextConverter.NormalizerContext normalizerContext;
    private TextMapping textMapping;
    private bool lineStarted;
    private bool wideGap;
    private bool afterFirstParagraph;
    private bool ignoreNextP;
    private int listLevel;
    private int listIndex;
    private bool listOrdered;
    private bool insideComment;
    private bool insidePre;
    private bool insideAnchor;
    private ScratchBuffer urlScratch;
    private int imageHeightPixels;
    private int imageWidthPixels;
    private ScratchBuffer imageAltText;
    private ScratchBuffer scratch;
    private Injection injection;
    private UrlCompareSink urlCompareSink;

    public HtmlToTextConverter(IHtmlParser parser, Text.TextOutput output, Injection injection, bool convertFragment, bool preformattedText, bool testTreatNbspAsBreakable, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum, bool shouldUseNarrowGapForPTagHtmlToTextConversion, bool outputAnchorLinks, bool outputImageLinks)
    {
      this.normalizedInput = parser is HtmlNormalizingParser;
      this.treatNbspAsBreakable = testTreatNbspAsBreakable;
      this.convertFragment = convertFragment;
      this.output = output;
      this.parser = parser;
      this.parser.SetRestartConsumer((IRestartable) this);
      this.injection = injection;
      if (!convertFragment)
      {
        this.output.OpenDocument();
        if (this.injection != null && this.injection.HaveHead)
          this.injection.Inject(true, this.output);
      }
      else
        this.insidePre = preformattedText;
      this.shouldUseNarrowGapForPTagHtmlToTextConversion = shouldUseNarrowGapForPTagHtmlToTextConversion;
      this.outputAnchorLinks = outputImageLinks;
      this.outputImageLinks = outputImageLinks;
    }

    private void Reinitialize()
    {
      this.endOfFile = false;
      this.normalizerContext.HasSpace = false;
      this.normalizerContext.EatSpace = false;
      this.normalizerContext.OneNL = false;
      this.normalizerContext.LastCh = char.MinValue;
      this.lineStarted = false;
      this.wideGap = false;
      this.nextParagraphCloseWideGap = true;
      this.afterFirstParagraph = false;
      this.ignoreNextP = false;
      this.insideComment = false;
      this.insidePre = false;
      this.insideAnchor = false;
      if (this.urlCompareSink != null)
        this.urlCompareSink.Reset();
      this.listLevel = 0;
      this.listIndex = 0;
      this.listOrdered = false;
      if (!this.convertFragment)
      {
        this.output.OpenDocument();
        if (this.injection != null)
        {
          this.injection.Reset();
          if (this.injection.HaveHead)
            this.injection.Inject(true, this.output);
        }
      }
      this.textMapping = TextMapping.Unicode;
    }

    public void Run()
    {
      if (this.endOfFile)
        return;
      HtmlTokenId tokenId = this.parser.Parse();
      if (tokenId == HtmlTokenId.None)
        return;
      this.Process(tokenId);
    }

    public bool Flush()
    {
      if (!this.endOfFile)
        this.Run();
      return this.endOfFile;
    }

    private void Process(HtmlTokenId tokenId)
    {
      this.token = this.parser.Token;
      switch (tokenId)
      {
        case HtmlTokenId.EndOfFile:
          if (this.lineStarted)
          {
            this.output.OutputNewLine();
            this.lineStarted = false;
          }
          if (!this.convertFragment)
          {
            if (this.injection != null && this.injection.HaveHead)
            {
              if (this.wideGap)
              {
                this.output.OutputNewLine();
                this.wideGap = false;
              }
              this.injection.Inject(false, this.output);
            }
            this.output.CloseDocument();
            this.output.Flush();
          }
          this.endOfFile = true;
          break;
        case HtmlTokenId.Text:
          if (this.insideComment)
            break;
          if (this.insideAnchor && this.urlCompareSink.IsActive)
            this.token.Text.WriteTo((ITextSink) this.urlCompareSink);
          if (this.insidePre)
          {
            this.ProcessPreformatedText();
            break;
          }
          if (this.normalizedInput)
          {
            this.ProcessText();
            break;
          }
          this.NormalizeProcessText();
          break;
        case HtmlTokenId.EncodingChange:
          if (!this.output.OutputCodePageSameAsInput)
            break;
          this.output.OutputEncoding = this.token.TokenEncoding;
          break;
        case HtmlTokenId.Tag:
          if (this.token.TagIndex <= HtmlTagIndex.Unknown)
            break;
          HtmlDtd.TagDefinition tagDefinition = HtmlToTextConverter.GetTagDefinition(this.token.TagIndex);
          if (this.normalizedInput)
          {
            if (!this.token.IsEndTag)
            {
              if (this.token.IsTagBegin)
                this.PushElement(tagDefinition);
              this.ProcessStartTagAttributes(tagDefinition);
              break;
            }
            if (!this.token.IsTagBegin)
              break;
            this.PopElement(tagDefinition);
            break;
          }
          if (!this.token.IsEndTag)
          {
            if (this.token.IsTagBegin)
            {
              this.LFillTagB(tagDefinition);
              this.PushElement(tagDefinition);
              this.RFillTagB(tagDefinition);
            }
            this.ProcessStartTagAttributes(tagDefinition);
            break;
          }
          if (!this.token.IsTagBegin)
            break;
          this.LFillTagE(tagDefinition);
          this.PopElement(tagDefinition);
          this.RFillTagE(tagDefinition);
          break;
      }
    }

    private void PushElement(HtmlDtd.TagDefinition tagDef)
    {
      HtmlTagIndex htmlTagIndex = tagDef.TagIndex;
      if ((uint) htmlTagIndex <= 67U)
      {
        if ((uint) htmlTagIndex <= 38U)
        {
          switch (htmlTagIndex)
          {
            case HtmlTagIndex.A:
              if (this.insideAnchor)
              {
                this.EndAnchor();
                goto label_45;
              }
              else
                goto label_45;
            case HtmlTagIndex.BR:
              goto label_25;
            case HtmlTagIndex.Comment:
              break;
            case HtmlTagIndex.DD:
              if (this.lineStarted)
              {
                this.EndLine();
                goto label_45;
              }
              else
                goto label_45;
            case HtmlTagIndex.Dir:
              goto label_27;
            case HtmlTagIndex.DL:
              this.EndParagraph(true);
              goto label_45;
            case HtmlTagIndex.DT:
              if (this.lineStarted)
              {
                this.EndLine();
                goto label_45;
              }
              else
                goto label_45;
            default:
              goto label_43;
          }
        }
        else if ((uint) htmlTagIndex <= 53U)
        {
          if (htmlTagIndex != HtmlTagIndex.Font)
          {
            if (htmlTagIndex == HtmlTagIndex.HR)
            {
              this.EndParagraph(false);
              this.OutputText("________________________________");
              this.EndParagraph(false);
              goto label_45;
            }
            else
              goto label_43;
          }
          else
            goto label_45;
        }
        else
        {
          switch (htmlTagIndex)
          {
            case HtmlTagIndex.Image:
            case HtmlTagIndex.Img:
              goto label_45;
            case HtmlTagIndex.LI:
              this.EndParagraph(false);
              this.OutputText("  ");
              for (int index = 0; index < this.listLevel - 1; ++index)
                this.OutputText("   ");
              if (this.listLevel > 1 || !this.listOrdered)
              {
                this.OutputText("*");
                this.output.OutputSpace(3);
                goto label_45;
              }
              else
              {
                string text = this.listIndex.ToString();
                this.OutputText(text);
                this.OutputText(".");
                this.output.OutputSpace(text.Length == 1 ? 2 : 1);
                ++this.listIndex;
                goto label_45;
              }
            case HtmlTagIndex.Listing:
              goto label_42;
            default:
              goto label_43;
          }
        }
      }
      else if ((uint) htmlTagIndex <= 99U)
      {
        if ((uint) htmlTagIndex <= 91U)
        {
          switch (htmlTagIndex)
          {
            case HtmlTagIndex.Menu:
            case HtmlTagIndex.OL:
              goto label_27;
            case HtmlTagIndex.NoEmbed:
            case HtmlTagIndex.NoFrames:
            case HtmlTagIndex.Script:
              break;
            case HtmlTagIndex.Option:
              goto label_25;
            case HtmlTagIndex.P:
              if (!this.ignoreNextP)
                this.EndParagraph(true);
              this.nextParagraphCloseWideGap = true;
              goto label_45;
            case HtmlTagIndex.PlainText:
            case HtmlTagIndex.Pre:
              goto label_42;
            default:
              goto label_43;
          }
        }
        else if (htmlTagIndex != HtmlTagIndex.Span)
        {
          if (htmlTagIndex != HtmlTagIndex.Style)
            goto label_43;
        }
        else
          goto label_45;
      }
      else if ((uint) htmlTagIndex <= 110U)
      {
        switch (htmlTagIndex)
        {
          case HtmlTagIndex.TD:
          case HtmlTagIndex.TH:
            if (this.lineStarted)
            {
              this.output.OutputTabulation(1);
              goto label_45;
            }
            else
              goto label_45;
          case HtmlTagIndex.Title:
            break;
          default:
            goto label_43;
        }
      }
      else if (htmlTagIndex != HtmlTagIndex.UL)
      {
        if (htmlTagIndex == HtmlTagIndex.Xmp)
          goto label_42;
        else
          goto label_43;
      }
      else
        goto label_27;
      this.insideComment = true;
      goto label_45;
label_25:
      this.EndLine();
      goto label_45;
label_27:
      this.EndParagraph(this.listLevel == 0);
      if (this.listLevel < 10)
      {
        ++this.listLevel;
        if (this.listLevel == 1)
        {
          this.listIndex = 1;
          this.listOrdered = this.token.TagIndex == HtmlTagIndex.OL;
        }
      }
      this.nextParagraphCloseWideGap = false;
      goto label_45;
label_42:
      this.EndParagraph(true);
      this.insidePre = true;
      goto label_45;
label_43:
      if (tagDef.BlockElement)
        this.EndParagraph(false);
label_45:
      this.ignoreNextP = false;
      if (tagDef.TagIndex != HtmlTagIndex.LI)
        return;
      this.ignoreNextP = true;
    }

    private void ProcessStartTagAttributes(HtmlDtd.TagDefinition tagDef)
    {
      HtmlTagIndex htmlTagIndex = tagDef.TagIndex;
      if ((uint) htmlTagIndex <= 42U)
      {
        if (htmlTagIndex != HtmlTagIndex.A)
        {
          if (htmlTagIndex != HtmlTagIndex.Font)
            return;
          foreach (HtmlAttribute attr in this.token.Attributes)
          {
            if (attr.NameIndex == HtmlNameIndex.Face)
            {
              this.scratch.Reset();
              this.scratch.AppendHtmlAttributeValue(attr, 4096);
              RecognizeInterestingFontName interestingFontName = new RecognizeInterestingFontName();
              for (int index = 0; index < this.scratch.Length && !interestingFontName.IsRejected; ++index)
                interestingFontName.AddCharacter(this.scratch.Buffer[index]);
              this.textMapping = interestingFontName.TextMapping;
              break;
            }
          }
        }
        else
        {
          if (!this.outputAnchorLinks)
            return;
          foreach (HtmlAttribute attr in this.token.Attributes)
          {
            if (attr.NameIndex == HtmlNameIndex.Href)
            {
              if (attr.IsAttrBegin)
                this.urlScratch.Reset();
              this.urlScratch.AppendHtmlAttributeValue(attr, 4096);
              break;
            }
          }
          if (!this.token.IsTagEnd)
            return;
          BufferString bufferString = this.urlScratch.BufferString;
          bufferString.TrimWhitespace();
          if (bufferString.Length != 0 && (int) bufferString[0] != 35 && ((int) bufferString[0] != 63 && (int) bufferString[0] != 59))
          {
            if (!this.lineStarted)
              this.StartParagraphOrLine();
            string str = bufferString.ToString();
            if (str.IndexOf(' ') != -1)
              str = str.Replace(" ", "%20");
            this.output.OpenAnchor(str);
            this.insideAnchor = true;
            if (this.urlCompareSink == null)
              this.urlCompareSink = new UrlCompareSink();
            this.urlCompareSink.Initialize(str);
          }
          this.urlScratch.Reset();
        }
      }
      else
      {
        switch (htmlTagIndex)
        {
          case HtmlTagIndex.Image:
          case HtmlTagIndex.Img:
            if (!this.outputImageLinks)
              break;
            foreach (HtmlAttribute attr in this.token.Attributes)
            {
              if (attr.NameIndex == HtmlNameIndex.Src)
              {
                if (attr.IsAttrBegin)
                  this.urlScratch.Reset();
                this.urlScratch.AppendHtmlAttributeValue(attr, 4096);
              }
              else if (attr.NameIndex == HtmlNameIndex.Alt)
              {
                if (attr.IsAttrBegin)
                  this.imageAltText.Reset();
                this.imageAltText.AppendHtmlAttributeValue(attr, 4096);
              }
              else if (attr.NameIndex == HtmlNameIndex.Height)
              {
                if (!attr.Value.IsEmpty)
                {
                  Format.PropertyValue propertyValue;
                  if (attr.Value.IsContiguous)
                  {
                    propertyValue = HtmlSupport.ParseNumber(attr.Value.ContiguousBufferString, HtmlSupport.NumberParseFlags.Length);
                  }
                  else
                  {
                    this.scratch.Reset();
                    this.scratch.AppendHtmlAttributeValue(attr, 4096);
                    propertyValue = HtmlSupport.ParseNumber(this.scratch.BufferString, HtmlSupport.NumberParseFlags.Length);
                  }
                  if (propertyValue.IsAbsRelLength)
                  {
                    this.imageHeightPixels = propertyValue.PixelsInteger;
                    if (this.imageHeightPixels == 0)
                      this.imageHeightPixels = 1;
                  }
                }
              }
              else if (attr.NameIndex == HtmlNameIndex.Width && !attr.Value.IsEmpty)
              {
                Format.PropertyValue propertyValue;
                if (attr.Value.IsContiguous)
                {
                  propertyValue = HtmlSupport.ParseNumber(attr.Value.ContiguousBufferString, HtmlSupport.NumberParseFlags.Length);
                }
                else
                {
                  this.scratch.Reset();
                  this.scratch.AppendHtmlAttributeValue(attr, 4096);
                  propertyValue = HtmlSupport.ParseNumber(this.scratch.BufferString, HtmlSupport.NumberParseFlags.Length);
                }
                if (propertyValue.IsAbsRelLength)
                {
                  this.imageWidthPixels = propertyValue.PixelsInteger;
                  if (this.imageWidthPixels == 0)
                    this.imageWidthPixels = 1;
                }
              }
            }
            if (!this.token.IsTagEnd)
              break;
            string imageUrl = (string) null;
            string imageAltText = (string) null;
            BufferString bufferString1 = this.imageAltText.BufferString;
            bufferString1.TrimWhitespace();
            if (bufferString1.Length != 0)
              imageAltText = bufferString1.ToString();
            if (imageAltText == null || this.output.ImageRenderingCallbackDefined)
            {
              BufferString bufferString2 = this.urlScratch.BufferString;
              bufferString2.TrimWhitespace();
              if (bufferString2.Length != 0)
                imageUrl = bufferString2.ToString();
            }
            if (!this.lineStarted)
              this.StartParagraphOrLine();
            this.output.OutputImage(imageUrl, imageAltText, this.imageWidthPixels, this.imageHeightPixels);
            this.urlScratch.Reset();
            this.imageAltText.Reset();
            this.imageHeightPixels = 0;
            this.imageWidthPixels = 0;
            break;
          case HtmlTagIndex.P:
            if (!this.shouldUseNarrowGapForPTagHtmlToTextConversion && (!this.token.Attributes.Find(HtmlNameIndex.Class) || !this.token.Attributes.Current.Value.CaseInsensitiveCompareEqual("msonormal")))
              break;
            this.wideGap = false;
            this.nextParagraphCloseWideGap = false;
            break;
          case HtmlTagIndex.Span:
            foreach (HtmlAttribute attr in this.token.Attributes)
            {
              if (attr.NameIndex == HtmlNameIndex.Style)
              {
                this.scratch.Reset();
                this.scratch.AppendHtmlAttributeValue(attr, 4096);
                RecognizeInterestingFontNameInInlineStyle nameInInlineStyle = new RecognizeInterestingFontNameInInlineStyle();
                for (int index = 0; index < this.scratch.Length && !nameInInlineStyle.IsFinished; ++index)
                  nameInInlineStyle.AddCharacter(this.scratch.Buffer[index]);
                this.textMapping = nameInInlineStyle.TextMapping;
                break;
              }
            }
            break;
        }
      }
    }

    private void PopElement(HtmlDtd.TagDefinition tagDef)
    {
      HtmlTagIndex htmlTagIndex = tagDef.TagIndex;
      if ((uint) htmlTagIndex <= 67U)
      {
        if ((uint) htmlTagIndex <= 38U)
        {
          if ((uint) htmlTagIndex <= 23U)
          {
            if (htmlTagIndex != HtmlTagIndex.A)
            {
              if (htmlTagIndex == HtmlTagIndex.BR)
                goto label_24;
              else
                goto label_31;
            }
            else if (this.insideAnchor)
            {
              this.EndAnchor();
              goto label_33;
            }
            else
              goto label_33;
          }
          else
          {
            switch (htmlTagIndex)
            {
              case HtmlTagIndex.Comment:
                break;
              case HtmlTagIndex.DD:
              case HtmlTagIndex.DT:
                goto label_33;
              case HtmlTagIndex.Dir:
                goto label_26;
              default:
                goto label_31;
            }
          }
        }
        else if ((uint) htmlTagIndex <= 53U)
        {
          if (htmlTagIndex != HtmlTagIndex.Font)
          {
            if (htmlTagIndex == HtmlTagIndex.HR)
            {
              this.EndParagraph(false);
              this.OutputText("________________________________");
              this.EndParagraph(false);
              goto label_33;
            }
            else
              goto label_31;
          }
          else
            goto label_30;
        }
        else
        {
          switch (htmlTagIndex)
          {
            case HtmlTagIndex.Image:
            case HtmlTagIndex.Img:
              goto label_33;
            case HtmlTagIndex.Listing:
              goto label_29;
            default:
              goto label_31;
          }
        }
      }
      else if ((uint) htmlTagIndex <= 99U)
      {
        if ((uint) htmlTagIndex <= 91U)
        {
          switch (htmlTagIndex)
          {
            case HtmlTagIndex.Menu:
            case HtmlTagIndex.OL:
              goto label_26;
            case HtmlTagIndex.NoEmbed:
            case HtmlTagIndex.NoFrames:
            case HtmlTagIndex.Script:
              break;
            case HtmlTagIndex.Option:
              goto label_24;
            case HtmlTagIndex.P:
              this.EndParagraph(this.nextParagraphCloseWideGap);
              this.nextParagraphCloseWideGap = true;
              goto label_33;
            case HtmlTagIndex.PlainText:
            case HtmlTagIndex.Pre:
              goto label_29;
            default:
              goto label_31;
          }
        }
        else if (htmlTagIndex != HtmlTagIndex.Span)
        {
          if (htmlTagIndex != HtmlTagIndex.Style)
            goto label_31;
        }
        else
          goto label_30;
      }
      else if ((uint) htmlTagIndex <= 110U)
      {
        switch (htmlTagIndex)
        {
          case HtmlTagIndex.TD:
          case HtmlTagIndex.TH:
            this.lineStarted = true;
            goto label_33;
          case HtmlTagIndex.Title:
            break;
          default:
            goto label_31;
        }
      }
      else if (htmlTagIndex != HtmlTagIndex.UL)
      {
        if (htmlTagIndex == HtmlTagIndex.Xmp)
          goto label_29;
        else
          goto label_31;
      }
      else
        goto label_26;
      this.insideComment = false;
      goto label_33;
label_24:
      this.EndLine();
      goto label_33;
label_26:
      if (this.listLevel != 0)
        --this.listLevel;
      this.EndParagraph(this.listLevel == 0);
      goto label_33;
label_29:
      this.EndParagraph(true);
      this.insidePre = false;
      goto label_33;
label_30:
      this.textMapping = TextMapping.Unicode;
      goto label_33;
label_31:
      if (tagDef.BlockElement)
        this.EndParagraph(false);
label_33:
      this.ignoreNextP = false;
    }

    private void ProcessText()
    {
      if (!this.lineStarted)
        this.StartParagraphOrLine();
      Token.RunEnumerator enumerator = this.token.Runs.GetEnumerator();
      while (enumerator.MoveNext())
      {
        TokenRun current = enumerator.Current;
        if (current.IsTextRun)
        {
          if (current.IsAnyWhitespace)
            this.output.OutputSpace(1);
          else if (current.TextType == RunTextType.Nbsp)
          {
            if (this.treatNbspAsBreakable)
              this.output.OutputSpace(current.Length);
            else
              this.output.OutputNbsp(current.Length);
          }
          else if (current.IsLiteral)
            this.output.OutputNonspace(current.Literal, this.textMapping);
          else
            this.output.OutputNonspace(current.RawBuffer, current.RawOffset, current.RawLength, this.textMapping);
        }
      }
    }

    private void ProcessPreformatedText()
    {
      if (!this.lineStarted)
        this.StartParagraphOrLine();
      Token.RunEnumerator enumerator = this.token.Runs.GetEnumerator();
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
                if (this.treatNbspAsBreakable)
                {
                  this.output.OutputSpace(current.Length);
                  continue;
                }
                this.output.OutputNbsp(current.Length);
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
            this.output.OutputNonspace(current.Literal, this.textMapping);
          else
            this.output.OutputNonspace(current.RawBuffer, current.RawOffset, current.RawLength, this.textMapping);
        }
      }
    }

    private void NormalizeProcessText()
    {
      Token.RunEnumerator runs = this.token.Runs;
      runs.MoveNext(true);
      while (runs.IsValidPosition)
      {
        TokenRun current = runs.Current;
        if (current.IsAnyWhitespace)
        {
          int num = 0;
          do
          {
            num += runs.Current.TextType == RunTextType.NewLine ? 1 : 2;
          }
          while (runs.MoveNext(true) && runs.Current.TextType <= RunTextType.UnusualWhitespace);
          this.NormalizeAddSpace(num == 1);
        }
        else if (current.TextType == RunTextType.Nbsp)
        {
          this.NormalizeAddNbsp(current.Length);
          runs.MoveNext(true);
        }
        else
        {
          this.NormalizeAddNonspace(current);
          runs.MoveNext(true);
        }
      }
    }

    private void NormalizeAddNonspace(TokenRun run)
    {
      if (!this.lineStarted)
        this.StartParagraphOrLine();
      if (this.normalizerContext.HasSpace)
      {
        this.normalizerContext.HasSpace = false;
        if ((int) this.normalizerContext.LastCh == 0 || !this.normalizerContext.OneNL || !ParseSupport.TwoFarEastNonHanguelChars(this.normalizerContext.LastCh, run.FirstChar))
          this.output.OutputSpace(1);
      }
      if (run.IsLiteral)
        this.output.OutputNonspace(run.Literal, this.textMapping);
      else
        this.output.OutputNonspace(run.RawBuffer, run.RawOffset, run.RawLength, this.textMapping);
      this.normalizerContext.EatSpace = false;
      this.normalizerContext.LastCh = run.LastChar;
      this.normalizerContext.OneNL = false;
    }

    private void NormalizeAddNbsp(int count)
    {
      if (!this.lineStarted)
        this.StartParagraphOrLine();
      if (this.normalizerContext.HasSpace)
      {
        this.normalizerContext.HasSpace = false;
        this.output.OutputSpace(1);
      }
      if (this.treatNbspAsBreakable)
        this.output.OutputSpace(count);
      else
        this.output.OutputNbsp(count);
      this.normalizerContext.EatSpace = false;
      this.normalizerContext.LastCh = ' ';
      this.normalizerContext.OneNL = false;
    }

    private void NormalizeAddSpace(bool oneNL)
    {
      if (!this.normalizerContext.EatSpace && this.afterFirstParagraph)
        this.normalizerContext.HasSpace = true;
      if ((int) this.normalizerContext.LastCh == 0)
        return;
      if (oneNL && !this.normalizerContext.OneNL)
        this.normalizerContext.OneNL = true;
      else
        this.normalizerContext.LastCh = char.MinValue;
    }

    private void LFillTagB(HtmlDtd.TagDefinition tagDef)
    {
      if (this.insidePre)
        return;
      this.LFill(tagDef.Fill.LB);
    }

    private void RFillTagB(HtmlDtd.TagDefinition tagDef)
    {
      if (this.insidePre)
        return;
      this.RFill(tagDef.Fill.RB);
    }

    private void LFillTagE(HtmlDtd.TagDefinition tagDef)
    {
      if (this.insidePre)
        return;
      this.LFill(tagDef.Fill.LE);
    }

    private void RFillTagE(HtmlDtd.TagDefinition tagDef)
    {
      if (this.insidePre)
        return;
      this.RFill(tagDef.Fill.RE);
    }

    private void LFill(HtmlDtd.FillCode codeLeft)
    {
      this.normalizerContext.LastCh = char.MinValue;
      if (!this.normalizerContext.HasSpace)
        return;
      if (codeLeft == HtmlDtd.FillCode.PUT)
      {
        if (!this.lineStarted)
          this.StartParagraphOrLine();
        this.output.OutputSpace(1);
        this.normalizerContext.EatSpace = true;
      }
      this.normalizerContext.HasSpace = codeLeft == HtmlDtd.FillCode.NUL;
    }

    private void RFill(HtmlDtd.FillCode code)
    {
      if (code == HtmlDtd.FillCode.EAT)
      {
        this.normalizerContext.HasSpace = false;
        this.normalizerContext.EatSpace = true;
      }
      else
      {
        if (code != HtmlDtd.FillCode.PUT)
          return;
        this.normalizerContext.EatSpace = false;
      }
    }

    private static HtmlDtd.TagDefinition GetTagDefinition(HtmlTagIndex tagIndex)
    {
      if (tagIndex == HtmlTagIndex._NULL)
        return (HtmlDtd.TagDefinition) null;
      return HtmlDtd.tags[(int) tagIndex];
    }

    private void EndAnchor()
    {
      if (!this.urlCompareSink.IsMatch)
      {
        if (!this.lineStarted)
          this.StartParagraphOrLine();
        this.output.CloseAnchor();
      }
      else
        this.output.CancelAnchor();
      this.insideAnchor = false;
      this.urlCompareSink.Reset();
    }

    private void OutputText(string text)
    {
      if (!this.lineStarted)
        this.StartParagraphOrLine();
      this.output.OutputNonspace(text, this.textMapping);
    }

    private void StartParagraphOrLine()
    {
      if (this.wideGap)
      {
        if (this.afterFirstParagraph)
          this.output.OutputNewLine();
        this.wideGap = false;
      }
      this.lineStarted = true;
      this.afterFirstParagraph = true;
    }

    private void EndLine()
    {
      this.output.OutputNewLine();
      this.lineStarted = false;
      this.wideGap = false;
    }

    private void EndParagraph(bool wideGap)
    {
      if (this.insideAnchor)
        this.EndAnchor();
      if (this.lineStarted)
      {
        this.output.OutputNewLine();
        this.lineStarted = false;
      }
      this.wideGap = this.wideGap || wideGap;
    }

    void IDisposable.Dispose()
    {
      if (this.parser != null)
        ((IDisposable) this.parser).Dispose();
      if (!this.convertFragment && this.output != null && this.output != null)
        ((IDisposable) this.output).Dispose();
      if (this.token != null && this.token is IDisposable)
        ((IDisposable) this.token).Dispose();
      if (this.injection != null)
        ((IDisposable) this.injection).Dispose();
      this.parser = (IHtmlParser) null;
      this.output = (Text.TextOutput) null;
      this.token = (HtmlToken) null;
      this.injection = (Injection) null;
      GC.SuppressFinalize((object) this);
    }

    bool IRestartable.CanRestart()
    {
      if (!this.convertFragment)
        return ((IRestartable) this.output).CanRestart();
      return true;
    }

    void IRestartable.Restart()
    {
      if (!this.convertFragment)
        ((IRestartable) this.output).Restart();
      this.Reinitialize();
    }

    void IRestartable.DisableRestart()
    {
      if (this.convertFragment)
        return;
      ((IRestartable) this.output).DisableRestart();
    }

    void IReusable.Initialize(object newSourceOrDestination)
    {
      ((IReusable) this.parser).Initialize(newSourceOrDestination);
      ((IReusable) this.output).Initialize(newSourceOrDestination);
      this.Reinitialize();
      this.parser.SetRestartConsumer((IRestartable) this);
    }

    public void Initialize(string fragment, bool preformatedText)
    {
      if (this.normalizedInput)
        ((HtmlNormalizingParser) this.parser).Initialize(fragment, preformatedText);
      else
        ((HtmlParser) this.parser).Initialize(fragment, preformatedText);
      if (!this.convertFragment)
        ((IReusable) this.output).Initialize((object) null);
      this.Reinitialize();
    }

    private struct NormalizerContext
    {
      public char LastCh;
      public bool OneNL;
      public bool HasSpace;
      public bool EatSpace;
    }
  }
}
