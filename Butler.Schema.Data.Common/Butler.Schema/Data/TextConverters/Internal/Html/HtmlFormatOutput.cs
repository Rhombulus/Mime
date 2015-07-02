// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Html.HtmlFormatOutput
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters.Internal.Html
{
  internal class HtmlFormatOutput : Format.FormatOutput, IRestartable
  {
    private static string[] listType = new string[7]
    {
      null,
      null,
      "1",
      "a",
      "A",
      "i",
      "I"
    };
    private static Format.Property[] defaultHyperlinkProperties = new Format.Property[1]
    {
      new Format.Property(Format.PropertyId.FontColor, new Format.PropertyValue(new Format.RGBT(0U, 0U, (uint) byte.MaxValue)))
    };
    private const int MaxRecognizedHyperlinkLength = 4096;
      private HtmlInjection injection;
    private bool filterHtml;
    private HtmlTagCallback callback;
    private HtmlFormatOutputCallbackContext callbackContext;
    private bool outputFragment;
    private bool recognizeHyperlinks;
    private int hyperlinkLevel;
    private HtmlFormatOutput.EndTagActionEntry[] endTagActionStack;
    private int endTagActionStackTop;

    internal HtmlWriter Writer { get; set; }

      public override bool OutputCodePageSameAsInput => false;

      public override Encoding OutputEncoding
    {
      set
      {
        throw new InvalidOperationException();
      }
    }

    public override bool CanAcceptMoreOutput => this.Writer.CanAcceptMore;

      public HtmlFormatOutput(HtmlWriter writer, HtmlInjection injection, bool outputFragment, Stream formatTraceStream, Stream formatOutputTraceStream, bool filterHtml, HtmlTagCallback callback, bool recognizeHyperlinks)
      : base(formatOutputTraceStream)
    {
      this.Writer = writer;
      this.injection = injection;
      this.outputFragment = outputFragment;
      this.filterHtml = filterHtml;
      this.callback = callback;
      this.recognizeHyperlinks = recognizeHyperlinks;
    }

    private static bool IsHyperLinkStartDelimiter(char c)
    {
      if ((int) c != 60 && (int) c != 34 && ((int) c != 39 && (int) c != 40))
        return (int) c == 91;
      return true;
    }

    private static bool IsHyperLinkEndDelimiter(char c)
    {
      if ((int) c != 62 && (int) c != 34 && ((int) c != 39 && (int) c != 41))
        return (int) c == 93;
      return true;
    }

    private void WriteIdAttribute(bool saveToCallbackContext)
    {
      Format.PropertyValue distinctProperty = this.GetDistinctProperty(Format.PropertyId.Id);
      if (distinctProperty.IsNull)
        return;
      string @string = this.FormatStore.GetStringValue(distinctProperty).GetString();
      if (saveToCallbackContext)
      {
        this.callbackContext.AddAttribute(HtmlNameIndex.Id, @string);
      }
      else
      {
        this.Writer.WriteAttributeName(HtmlNameIndex.Id);
        this.Writer.WriteAttributeValue(@string);
      }
    }

    bool IRestartable.CanRestart()
    {
      if (this.Writer != null)
        return ((IRestartable) this.Writer).CanRestart();
      return false;
    }

    void IRestartable.Restart()
    {
      ((IRestartable) this.Writer).Restart();
      this.Restart();
      if (this.injection != null)
        this.injection.Reset();
      this.hyperlinkLevel = 0;
    }

    void IRestartable.DisableRestart()
    {
      if (this.Writer == null)
        return;
      ((IRestartable) this.Writer).DisableRestart();
    }

    public override bool Flush()
    {
      if (!base.Flush())
        return false;
      this.Writer.Flush();
      return true;
    }

    internal void SetWriter(HtmlWriter writer)
    {
      this.Writer = writer;
    }

    protected override bool StartRoot()
    {
      return true;
    }

    protected override void EndRoot()
    {
    }

    protected override bool StartDocument()
    {
      if (!this.outputFragment)
      {
        bool flag1 = false;
        bool flag2 = false;
        bool tagDropped = false;
        this.Writer.WriteStartTag(HtmlNameIndex.Html);
        if (this.callback != null)
        {
          if (this.callbackContext == null)
            this.callbackContext = new HtmlFormatOutputCallbackContext(this);
          this.callbackContext.InitializeTag(false, HtmlNameIndex.Head, false);
        }
        else
          this.Writer.WriteStartTag(HtmlNameIndex.Head);
        if (this.callback != null)
        {
          this.callbackContext.InitializeFragment(false);
          this.callback((HtmlTagContext) this.callbackContext, this.Writer);
          this.callbackContext.UninitializeFragment();
          if (this.callbackContext.IsInvokeCallbackForEndTag)
            flag1 = true;
          if (this.callbackContext.IsDeleteInnerContent)
            flag2 = true;
          if (this.callbackContext.IsDeleteEndTag)
            tagDropped = true;
        }
        if (!flag2)
        {
          if (this.Writer.HasEncoding)
          {
            this.Writer.WriteStartTag(HtmlNameIndex.Meta);
            this.Writer.WriteAttribute(HtmlNameIndex.HttpEquiv, "Content-Type");
            this.Writer.WriteAttributeName(HtmlNameIndex.Content);
            this.Writer.WriteAttributeValueInternal("text/html; charset=");
            this.Writer.WriteAttributeValue(Globalization.Charset.GetCharset(this.Writer.Encoding).Name);
            this.Writer.WriteNewLine(true);
          }
          this.Writer.WriteStartTag(HtmlNameIndex.Meta);
          this.Writer.WriteAttribute(HtmlNameIndex.Name, "Generator");
          this.Writer.WriteAttribute(HtmlNameIndex.Content, "Microsoft Exchange Server");
          this.Writer.WriteNewLine(true);
          if (this.Comment != null)
          {
            this.Writer.WriteMarkupText("<!-- " + this.Comment + " -->");
            this.Writer.WriteNewLine(true);
          }
          this.Writer.WriteStartTag(HtmlNameIndex.Style);
          this.Writer.WriteMarkupText("<!-- .EmailQuote { margin-left: 1pt; padding-left: 4pt; border-left: #800000 2px solid; } -->");
          this.Writer.WriteEndTag(HtmlNameIndex.Style);
        }
        if (flag1)
        {
          this.callbackContext.InitializeTag(true, HtmlNameIndex.Head, tagDropped);
          this.callbackContext.InitializeFragment(false);
          this.callback((HtmlTagContext) this.callbackContext, this.Writer);
          this.callbackContext.UninitializeFragment();
        }
        else if (!tagDropped)
        {
          this.Writer.WriteEndTag(HtmlNameIndex.Head);
          this.Writer.WriteNewLine(true);
        }
        this.Writer.WriteStartTag(HtmlNameIndex.Body);
        this.Writer.WriteNewLine(true);
      }
      else
      {
        this.Writer.WriteStartTag(HtmlNameIndex.Div);
        this.Writer.WriteAttribute(HtmlNameIndex.Class, "BodyFragment");
        this.Writer.WriteNewLine(true);
      }
      if (this.injection != null && this.injection.HaveHead)
        this.injection.Inject(true, this.Writer);
      this.ApplyCharFormat();
      return true;
    }

    protected override void EndDocument()
    {
      this.RevertCharFormat();
      if (this.injection != null && this.injection.HaveTail)
        this.injection.Inject(false, this.Writer);
      if (!this.outputFragment)
      {
        this.Writer.WriteNewLine(true);
        this.Writer.WriteEndTag(HtmlNameIndex.Body);
        this.Writer.WriteNewLine(true);
        this.Writer.WriteEndTag(HtmlNameIndex.Html);
      }
      else
      {
        this.Writer.WriteNewLine(true);
        this.Writer.WriteEndTag(HtmlNameIndex.Div);
      }
      this.Writer.WriteNewLine(true);
    }

    protected override void StartEndBaseFont()
    {
    }

    protected override bool StartTable()
    {
      Format.PropertyValue distinctProperty = this.GetDistinctProperty(Format.PropertyId.FontFace);
      if (!distinctProperty.IsNull)
      {
        this.Writer.WriteStartTag(HtmlNameIndex.Font);
        this.Writer.WriteAttributeName(HtmlNameIndex.Face);
        if (distinctProperty.IsMultiValue)
        {
          Format.MultiValue multiValue = this.FormatStore.GetMultiValue(distinctProperty);
          for (int index = 0; index < multiValue.Length; ++index)
          {
            string @string = multiValue.GetStringValue(index).GetString();
            if (index != 0)
              this.Writer.WriteAttributeValue(",");
            this.Writer.WriteAttributeValue(@string);
          }
        }
        else
          this.Writer.WriteAttributeValue(this.FormatStore.GetStringValue(distinctProperty).GetString());
      }
      this.Writer.WriteNewLine(true);
      this.Writer.WriteStartTag(HtmlNameIndex.Table);
      this.OutputTableTagAttributes();
      bool styleAttributeOpen = false;
      this.OutputTableCssProperties(ref styleAttributeOpen);
      this.OutputBlockCssProperties(ref styleAttributeOpen);
      this.Writer.WriteNewLine(true);
      return true;
    }

    protected override void EndTable()
    {
      this.Writer.WriteNewLine(true);
      this.Writer.WriteEndTag(HtmlNameIndex.Table);
      this.Writer.WriteNewLine(true);
      if (this.GetDistinctProperty(Format.PropertyId.FontFace).IsNull)
        return;
      this.Writer.WriteEndTag(HtmlNameIndex.Font);
    }

    protected override bool StartTableColumnGroup()
    {
      this.Writer.WriteNewLine(true);
      this.Writer.WriteStartTag(HtmlNameIndex.ColGroup);
      Format.PropertyValue distinctProperty1 = this.GetDistinctProperty(Format.PropertyId.Width);
      if (!distinctProperty1.IsNull && distinctProperty1.IsAbsRelLength)
        this.Writer.WriteAttribute(HtmlNameIndex.Width, distinctProperty1.PixelsInteger.ToString());
      Format.PropertyValue distinctProperty2 = this.GetDistinctProperty(Format.PropertyId.NumColumns);
      if (!distinctProperty2.IsNull && distinctProperty2.IsAbsRelLength)
        this.Writer.WriteAttribute(HtmlNameIndex.Span, distinctProperty2.Integer.ToString());
      bool styleAttributeOpen = false;
      this.OutputTableColumnCssProperties(ref styleAttributeOpen);
      return true;
    }

    protected override void EndTableColumnGroup()
    {
      this.Writer.WriteEndTag(HtmlNameIndex.ColGroup);
      this.Writer.WriteNewLine(true);
    }

    protected override void StartEndTableColumn()
    {
      this.Writer.WriteStartTag(HtmlNameIndex.Col);
      Format.PropertyValue distinctProperty1 = this.GetDistinctProperty(Format.PropertyId.Width);
      if (!distinctProperty1.IsNull && distinctProperty1.IsAbsRelLength)
        this.Writer.WriteAttribute(HtmlNameIndex.Width, distinctProperty1.PixelsInteger.ToString());
      Format.PropertyValue distinctProperty2 = this.GetDistinctProperty(Format.PropertyId.NumColumns);
      if (!distinctProperty2.IsNull && distinctProperty2.IsAbsRelLength)
        this.Writer.WriteAttribute(HtmlNameIndex.Span, distinctProperty2.Integer.ToString());
      bool styleAttributeOpen = false;
      this.OutputTableColumnCssProperties(ref styleAttributeOpen);
      this.Writer.WriteNewLine(true);
    }

    protected override bool StartTableCaption()
    {
      this.Writer.WriteNewLine(true);
      if (!this.CurrentNode.Parent.IsNull && this.CurrentNode.Parent.NodeType == Format.FormatContainerType.Table)
      {
        this.Writer.WriteStartTag(HtmlNameIndex.Caption);
        Format.FormatStyle style = this.FormatStore.GetStyle(13);
        this.SubtractDefaultContainerPropertiesFromDistinct(style.FlagProperties, style.PropertyList);
        Format.PropertyValue distinctProperty = this.GetDistinctProperty(Format.PropertyId.BlockAlignment);
        if (!distinctProperty.IsNull)
        {
          string blockAlignmentString = HtmlSupport.GetBlockAlignmentString(distinctProperty);
          if (blockAlignmentString != null)
            this.Writer.WriteAttribute(HtmlNameIndex.Align, blockAlignmentString);
        }
        this.Writer.WriteNewLine(true);
      }
      this.ApplyCharFormat();
      return true;
    }

    protected override void EndTableCaption()
    {
      this.RevertCharFormat();
      if (!this.CurrentNode.Parent.IsNull && this.CurrentNode.Parent.NodeType == Format.FormatContainerType.Table)
      {
        this.Writer.WriteNewLine(true);
        this.Writer.WriteEndTag(HtmlNameIndex.Caption);
      }
      this.Writer.WriteNewLine(true);
    }

    protected override bool StartTableExtraContent()
    {
      return this.StartBlockContainer();
    }

    protected override void EndTableExtraContent()
    {
      this.EndBlockContainer();
    }

    protected override bool StartTableRow()
    {
      this.Writer.WriteNewLine(true);
      this.Writer.WriteStartTag(HtmlNameIndex.TR);
      Format.PropertyValue distinctProperty = this.GetDistinctProperty(Format.PropertyId.Height);
      if (!distinctProperty.IsNull && distinctProperty.IsAbsRelLength)
        this.Writer.WriteAttribute(HtmlNameIndex.Height, distinctProperty.PixelsInteger.ToString());
      bool styleAttributeOpen = false;
      this.OutputBlockCssProperties(ref styleAttributeOpen);
      this.Writer.WriteNewLine(true);
      return true;
    }

    protected override void EndTableRow()
    {
      this.Writer.WriteNewLine(true);
      this.Writer.WriteEndTag(HtmlNameIndex.TR);
      this.Writer.WriteNewLine(true);
    }

    protected override bool StartTableCell()
    {
      Format.PropertyValue distinctProperty = this.GetDistinctProperty(Format.PropertyId.MergedCell);
      if (distinctProperty.IsNull || !distinctProperty.Bool)
      {
        this.Writer.WriteNewLine(true);
        this.Writer.WriteStartTag(HtmlNameIndex.TD);
        this.OutputTableCellTagAttributes();
        bool styleAttributeOpen = false;
        this.OutputBlockCssProperties(ref styleAttributeOpen);
        this.ApplyCharFormat();
      }
      return true;
    }

    protected override void EndTableCell()
    {
      Format.PropertyValue distinctProperty = this.GetDistinctProperty(Format.PropertyId.MergedCell);
      if (!distinctProperty.IsNull && distinctProperty.Bool)
        return;
      this.RevertCharFormat();
      this.Writer.WriteEndTag(HtmlNameIndex.TD);
      this.Writer.WriteNewLine(true);
    }

    protected override bool StartList()
    {
      this.Writer.WriteNewLine(true);
      Format.PropertyValue effectiveProperty = this.GetEffectiveProperty(Format.PropertyId.ListStyle);
      bool flag = true;
      if (effectiveProperty.IsNull || effectiveProperty.Enum == 1)
      {
        this.Writer.WriteStartTag(HtmlNameIndex.UL);
      }
      else
      {
        this.Writer.WriteStartTag(HtmlNameIndex.OL);
        flag = false;
      }
      Format.PropertyValue distinctProperty1 = this.GetDistinctProperty(Format.PropertyId.RightToLeft);
      if (!distinctProperty1.IsNull)
        this.Writer.WriteAttribute(HtmlNameIndex.Dir, distinctProperty1.Bool ? "rtl" : "ltr");
      if (!flag && effectiveProperty.Enum != 2)
        this.Writer.WriteAttribute(HtmlNameIndex.Type, HtmlFormatOutput.listType[effectiveProperty.Enum]);
      Format.PropertyValue distinctProperty2 = this.GetDistinctProperty(Format.PropertyId.ListStart);
      if (!flag && distinctProperty2.IsInteger && distinctProperty2.Integer != 1)
        this.Writer.WriteAttribute(HtmlNameIndex.Start, distinctProperty2.Integer.ToString());
      bool styleAttributeOpen = false;
      this.OutputBlockCssProperties(ref styleAttributeOpen);
      this.Writer.WriteNewLine(true);
      this.ApplyCharFormat();
      return true;
    }

    protected override void EndList()
    {
      this.RevertCharFormat();
      Format.PropertyValue effectiveProperty = this.GetEffectiveProperty(Format.PropertyId.ListStyle);
      this.Writer.WriteNewLine(true);
      if (effectiveProperty.IsNull || effectiveProperty.Enum == 1)
        this.Writer.WriteEndTag(HtmlNameIndex.UL);
      else
        this.Writer.WriteEndTag(HtmlNameIndex.OL);
      this.Writer.WriteNewLine(true);
    }

    protected override bool StartListItem()
    {
      this.Writer.WriteNewLine(true);
      this.Writer.WriteStartTag(HtmlNameIndex.LI);
      bool styleAttributeOpen = false;
      this.OutputBlockCssProperties(ref styleAttributeOpen);
      this.ApplyCharFormat();
      return true;
    }

    protected override void EndListItem()
    {
      this.RevertCharFormat();
      this.Writer.WriteEndTag(HtmlNameIndex.LI);
      this.Writer.WriteNewLine(true);
    }

    protected override bool StartHyperLink()
    {
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      Format.FlagProperties flags = new Format.FlagProperties();
      flags.Set(Format.PropertyId.Underline, true);
      this.SubtractDefaultContainerPropertiesFromDistinct(flags, HtmlFormatOutput.defaultHyperlinkProperties);
      if (this.callback != null)
      {
        if (this.callbackContext == null)
          this.callbackContext = new HtmlFormatOutputCallbackContext(this);
        this.callbackContext.InitializeTag(false, HtmlNameIndex.A, false);
      }
      else
        this.Writer.WriteStartTag(HtmlNameIndex.A);
      Format.PropertyValue distinctProperty1 = this.GetDistinctProperty(Format.PropertyId.HyperlinkUrl);
      if (!distinctProperty1.IsNull)
      {
        string url = this.FormatStore.GetStringValue(distinctProperty1).GetString();
        if (this.filterHtml && !HtmlToHtmlConverter.IsUrlSafe(url, this.callback != null))
          url = string.Empty;
        if (this.callback != null)
        {
          this.callbackContext.AddAttribute(HtmlNameIndex.Href, url);
        }
        else
        {
          this.Writer.WriteAttributeName(HtmlNameIndex.Href);
          this.Writer.WriteAttributeValue(url);
        }
        Format.PropertyValue distinctProperty2 = this.GetDistinctProperty(Format.PropertyId.HyperlinkTarget);
        if (!distinctProperty2.IsNull)
        {
          string targetString = HtmlSupport.GetTargetString(distinctProperty2);
          if (this.callback != null)
          {
            this.callbackContext.AddAttribute(HtmlNameIndex.Target, targetString);
          }
          else
          {
            this.Writer.WriteAttributeName(HtmlNameIndex.Target);
            this.Writer.WriteAttributeValue(targetString);
          }
        }
        this.WriteIdAttribute(this.callback != null);
      }
      if (this.callback != null)
      {
        this.callbackContext.InitializeFragment(false);
        this.callback((HtmlTagContext) this.callbackContext, this.Writer);
        this.callbackContext.UninitializeFragment();
        if (this.callbackContext.IsInvokeCallbackForEndTag)
          flag3 = true;
        if (this.callbackContext.IsDeleteInnerContent)
          flag2 = true;
        if (this.callbackContext.IsDeleteEndTag)
          flag1 = true;
        if (flag1 || flag3)
        {
          if (this.endTagActionStack == null)
            this.endTagActionStack = new HtmlFormatOutput.EndTagActionEntry[4];
          else if (this.endTagActionStack.Length == this.endTagActionStackTop)
          {
            HtmlFormatOutput.EndTagActionEntry[] endTagActionEntryArray = new HtmlFormatOutput.EndTagActionEntry[this.endTagActionStack.Length * 2];
            Array.Copy((Array) this.endTagActionStack, 0, (Array) endTagActionEntryArray, 0, this.endTagActionStackTop);
            this.endTagActionStack = endTagActionEntryArray;
          }
          this.endTagActionStack[this.endTagActionStackTop].TagLevel = this.hyperlinkLevel;
          this.endTagActionStack[this.endTagActionStackTop].Drop = flag1;
          this.endTagActionStack[this.endTagActionStackTop].Callback = flag3;
          ++this.endTagActionStackTop;
        }
      }
      ++this.hyperlinkLevel;
      if (!flag2)
        this.ApplyCharFormat();
      else
        this.CloseHyperLink();
      if (this.Writer.IsTagOpen)
        this.Writer.WriteTagEnd();
      return !flag2;
    }

    protected override void EndHyperLink()
    {
      --this.hyperlinkLevel;
      this.RevertCharFormat();
      this.CloseHyperLink();
      if (!this.Writer.IsTagOpen)
        return;
      this.Writer.WriteTagEnd();
    }

    protected override bool StartBookmark()
    {
      Format.PropertyValue distinctProperty = this.GetDistinctProperty(Format.PropertyId.BookmarkName);
      if (!distinctProperty.IsNull)
      {
        this.Writer.WriteStartTag(HtmlNameIndex.A);
        string @string = this.FormatStore.GetStringValue(distinctProperty).GetString();
        this.Writer.WriteAttributeName(HtmlNameIndex.Name);
        this.Writer.WriteAttributeValue(@string);
      }
      this.ApplyCharFormat();
      if (this.Writer.IsTagOpen)
        this.Writer.WriteTagEnd();
      return true;
    }

    protected override void EndBookmark()
    {
      this.RevertCharFormat();
      if (!this.GetDistinctProperty(Format.PropertyId.BookmarkName).IsNull)
        this.Writer.WriteEndTag(HtmlNameIndex.A);
      if (!this.Writer.IsTagOpen)
        return;
      this.Writer.WriteTagEnd();
    }

    protected override void StartEndImage()
    {
      if (this.callback != null)
      {
        if (this.callbackContext == null)
          this.callbackContext = new HtmlFormatOutputCallbackContext(this);
        this.callbackContext.InitializeTag(false, HtmlNameIndex.Img, false);
      }
      else
        this.Writer.WriteStartTag(HtmlNameIndex.Img);
      Format.PropertyValue distinctProperty1 = this.GetDistinctProperty(Format.PropertyId.Width);
      if (!distinctProperty1.IsNull)
      {
        BufferString bufferString = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, distinctProperty1);
        if (bufferString.Length != 0)
        {
          if (this.callback != null)
            this.callbackContext.AddAttribute(HtmlNameIndex.Width, bufferString.ToString());
          else
            this.Writer.WriteAttribute(HtmlNameIndex.Width, bufferString);
        }
      }
      Format.PropertyValue distinctProperty2 = this.GetDistinctProperty(Format.PropertyId.Height);
      if (!distinctProperty2.IsNull)
      {
        BufferString bufferString = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, distinctProperty2);
        if (bufferString.Length != 0)
        {
          if (this.callback != null)
            this.callbackContext.AddAttribute(HtmlNameIndex.Height, bufferString.ToString());
          else
            this.Writer.WriteAttribute(HtmlNameIndex.Height, bufferString);
        }
      }
      Format.PropertyValue distinctProperty3 = this.GetDistinctProperty(Format.PropertyId.BlockAlignment);
      if (!distinctProperty3.IsNull)
      {
        string blockAlignmentString = HtmlSupport.GetBlockAlignmentString(distinctProperty3);
        if (blockAlignmentString != null)
        {
          if (this.callback != null)
            this.callbackContext.AddAttribute(HtmlNameIndex.Align, blockAlignmentString);
          else
            this.Writer.WriteAttribute(HtmlNameIndex.Align, blockAlignmentString);
        }
      }
      Format.PropertyValue distinctProperty4 = this.GetDistinctProperty(Format.PropertyId.TableBorder);
      if (!distinctProperty4.IsNull)
      {
        BufferString bufferString = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, distinctProperty4);
        if (bufferString.Length != 0)
        {
          if (this.callback != null)
            this.callbackContext.AddAttribute(HtmlNameIndex.Border, bufferString.ToString());
          else
            this.Writer.WriteAttribute(HtmlNameIndex.Border, bufferString);
        }
      }
      Format.PropertyValue distinctProperty5 = this.GetDistinctProperty(Format.PropertyId.ImageUrl);
      if (!distinctProperty5.IsNull)
      {
        string url = this.FormatStore.GetStringValue(distinctProperty5).GetString();
        if (this.filterHtml && !HtmlToHtmlConverter.IsUrlSafe(url, this.callback != null))
          url = string.Empty;
        if (this.callback != null)
        {
          this.callbackContext.AddAttribute(HtmlNameIndex.Src, url);
        }
        else
        {
          this.Writer.WriteAttributeName(HtmlNameIndex.Src);
          this.Writer.WriteAttributeValue(url);
        }
      }
      Format.PropertyValue distinctProperty6 = this.GetDistinctProperty(Format.PropertyId.RightToLeft);
      if (distinctProperty6.IsBool)
      {
        if (this.callback != null)
        {
          this.callbackContext.AddAttribute(HtmlNameIndex.Dir, distinctProperty6.Bool ? "rtl" : "ltr");
        }
        else
        {
          this.Writer.WriteAttributeName(HtmlNameIndex.Dir);
          this.Writer.WriteAttributeValue(distinctProperty6.Bool ? "rtl" : "ltr");
        }
      }
      distinctProperty6 = this.GetDistinctProperty(Format.PropertyId.Language);
      Globalization.Culture culture;
      if (distinctProperty6.IsInteger && (Globalization.Culture.TryGetCulture(distinctProperty6.Integer, out culture) || string.IsNullOrEmpty(culture.Name)))
      {
        if (this.callback != null)
        {
          this.callbackContext.AddAttribute(HtmlNameIndex.Lang, culture.Name);
        }
        else
        {
          this.Writer.WriteAttributeName(HtmlNameIndex.Lang);
          this.Writer.WriteAttributeValue(culture.Name);
        }
      }
      distinctProperty6 = this.GetDistinctProperty(Format.PropertyId.ImageAltText);
      if (!distinctProperty6.IsNull)
      {
        string @string = this.FormatStore.GetStringValue(distinctProperty6).GetString();
        if (this.callback != null)
        {
          this.callbackContext.AddAttribute(HtmlNameIndex.Alt, @string);
        }
        else
        {
          this.Writer.WriteAttributeName(HtmlNameIndex.Alt);
          this.Writer.WriteAttributeValue(@string);
        }
      }
      if (this.callback != null)
      {
        this.callbackContext.InitializeFragment(true);
        this.callback((HtmlTagContext) this.callbackContext, this.Writer);
        this.callbackContext.UninitializeFragment();
      }
      if (!this.Writer.IsTagOpen)
        return;
      this.Writer.WriteTagEnd();
    }

    protected override void StartEndHorizontalLine()
    {
      this.Writer.WriteNewLine(true);
      this.Writer.WriteStartTag(HtmlNameIndex.HR);
      Format.PropertyValue distinctProperty1 = this.GetDistinctProperty(Format.PropertyId.Width);
      if (!distinctProperty1.IsNull)
      {
        BufferString bufferString = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, distinctProperty1);
        if (bufferString.Length != 0)
          this.Writer.WriteAttribute(HtmlNameIndex.Width, bufferString);
      }
      Format.PropertyValue distinctProperty2 = this.GetDistinctProperty(Format.PropertyId.Height);
      if (!distinctProperty2.IsNull && distinctProperty2.IsAbsRelLength)
        this.Writer.WriteAttribute(HtmlNameIndex.Size, distinctProperty2.PixelsInteger.ToString());
      Format.PropertyValue distinctProperty3 = this.GetDistinctProperty(Format.PropertyId.BlockAlignment);
      if (!distinctProperty3.IsNull)
      {
        string horizontalAlignmentString = HtmlSupport.GetHorizontalAlignmentString(distinctProperty3);
        if (horizontalAlignmentString != null)
          this.Writer.WriteAttribute(HtmlNameIndex.Align, horizontalAlignmentString);
      }
      Format.PropertyValue distinctProperty4 = this.GetDistinctProperty(Format.PropertyId.FontColor);
      if (!distinctProperty4.IsNull)
      {
        BufferString bufferString = HtmlSupport.FormatColor(ref this.scratchBuffer, distinctProperty4);
        if (bufferString.Length != 0)
          this.Writer.WriteAttribute(HtmlNameIndex.Color, bufferString);
      }
      if (!distinctProperty1.IsNull)
      {
        this.Writer.WriteAttributeName(HtmlNameIndex.Style);
        if (!distinctProperty1.IsNull)
        {
          BufferString bufferString = HtmlSupport.FormatLength(ref this.scratchBuffer, distinctProperty1);
          if (bufferString.Length != 0)
          {
            this.Writer.WriteAttributeValue("width:");
            this.Writer.WriteAttributeValue(bufferString);
            this.Writer.WriteAttributeValue(";");
          }
        }
      }
      if (this.Writer.LiteralWhitespaceNesting != 0)
        return;
      this.Writer.WriteNewLine(true);
    }

    protected override bool StartInline()
    {
      this.ApplyCharFormat();
      return true;
    }

    protected override void EndInline()
    {
      this.RevertCharFormat();
    }

    protected override bool StartMap()
    {
      return this.StartBlockContainer();
    }

    protected override void EndMap()
    {
      this.EndBlockContainer();
    }

    protected override void StartEndArea()
    {
    }

    protected override bool StartForm()
    {
      return this.StartInlineContainer();
    }

    protected override void EndForm()
    {
      this.EndInlineContainer();
    }

    protected override bool StartFieldSet()
    {
      return this.StartBlockContainer();
    }

    protected override void EndFieldSet()
    {
      this.EndBlockContainer();
    }

    protected override bool StartSelect()
    {
      return true;
    }

    protected override void EndSelect()
    {
    }

    protected override bool StartOptionGroup()
    {
      return true;
    }

    protected override void EndOptionGroup()
    {
    }

    protected override bool StartOption()
    {
      return true;
    }

    protected override void EndOption()
    {
    }

    protected override bool StartText()
    {
      this.ApplyCharFormat();
      this.Writer.StartTextChunk();
      return true;
    }

    protected override bool ContinueText(uint beginTextPosition, uint endTextPosition)
    {
      if ((int) beginTextPosition != (int) endTextPosition)
      {
        Format.TextRun textRun = this.FormatStore.GetTextRun(beginTextPosition);
        do
        {
          int effectiveLength = textRun.EffectiveLength;
          Format.TextRunType type = textRun.Type;
          if ((uint) type <= 16384U)
          {
            if (type != Format.TextRunType.NonSpace)
            {
              if (type == Format.TextRunType.NbSp)
                this.Writer.WriteNbsp(effectiveLength);
            }
            else
            {
              int start = 0;
              int offset1;
              int length;
              bool addFilePrefix;
              bool addHttpPrefix;
              if (this.recognizeHyperlinks && this.hyperlinkLevel == 0 && (effectiveLength > 10 && effectiveLength < 4096) && this.RecognizeHyperLink(textRun, out offset1, out length, out addFilePrefix, out addHttpPrefix))
              {
                if (offset1 != 0)
                  this.Writer.WriteTextInternal(this.scratchBuffer.Buffer, 0, offset1);
                if (this.callback != null)
                {
                  if (this.callbackContext == null)
                    this.callbackContext = new HtmlFormatOutputCallbackContext(this);
                  this.callbackContext.InitializeTag(false, HtmlNameIndex.A, false);
                  string str = new string(this.scratchBuffer.Buffer, offset1, length);
                  if (addHttpPrefix)
                    str = "http://" + str;
                  else if (addFilePrefix)
                    str = "file://" + str;
                  this.callbackContext.AddAttribute(HtmlNameIndex.Href, str);
                  this.callbackContext.InitializeFragment(false);
                  this.callback((HtmlTagContext) this.callbackContext, this.Writer);
                  this.callbackContext.UninitializeFragment();
                  if (this.Writer.IsTagOpen)
                    this.Writer.WriteTagEnd();
                  if (!this.callbackContext.IsDeleteInnerContent)
                    this.Writer.WriteTextInternal(this.scratchBuffer.Buffer, offset1, length);
                  if (this.callbackContext.IsInvokeCallbackForEndTag)
                  {
                    this.callbackContext.InitializeTag(true, HtmlNameIndex.A, this.callbackContext.IsDeleteEndTag);
                    this.callbackContext.InitializeFragment(false);
                    this.callback((HtmlTagContext) this.callbackContext, this.Writer);
                    this.callbackContext.UninitializeFragment();
                  }
                  else if (!this.callbackContext.IsDeleteEndTag)
                    this.Writer.WriteEndTag(HtmlNameIndex.A);
                  if (this.Writer.IsTagOpen)
                    this.Writer.WriteTagEnd();
                }
                else
                {
                  this.Writer.WriteStartTag(HtmlNameIndex.A);
                  this.Writer.WriteAttributeName(HtmlNameIndex.Href);
                  if (addHttpPrefix)
                    this.Writer.WriteAttributeValue("http://");
                  else if (addFilePrefix)
                    this.Writer.WriteAttributeValue("file://");
                  this.Writer.WriteAttributeValue(this.scratchBuffer.Buffer, offset1, length);
                  this.Writer.WriteTagEnd();
                  this.Writer.WriteTextInternal(this.scratchBuffer.Buffer, offset1, length);
                  this.Writer.WriteEndTag(HtmlNameIndex.A);
                }
                start += offset1 + length;
                if (start == effectiveLength)
                {
                  textRun.MoveNext();
                  goto label_44;
                }
              }
              do
              {
                char[] buffer;
                int offset2;
                int count;
                textRun.GetChunk(start, out buffer, out offset2, out count);
                this.Writer.WriteTextInternal(buffer, offset2, count);
                start += count;
              }
              while (start != effectiveLength);
            }
          }
          else if (type != Format.TextRunType.Space)
          {
            if (type != Format.TextRunType.Tabulation)
            {
              if (type == Format.TextRunType.NewLine)
              {
                while (effectiveLength-- != 0)
                {
                  if (this.Writer.LiteralWhitespaceNesting == 0)
                    this.Writer.WriteStartTag(HtmlNameIndex.BR);
                  this.Writer.WriteNewLine(false);
                }
              }
            }
            else
              this.Writer.WriteTabulation(effectiveLength);
          }
          else
            this.Writer.WriteSpace(effectiveLength);
          textRun.MoveNext();
label_44:;
        }
        while (textRun.Position < endTextPosition);
      }
      return true;
    }

    protected override void EndText()
    {
      this.Writer.EndTextChunk();
      this.RevertCharFormat();
    }

    protected override bool StartBlockContainer()
    {
      this.Writer.WriteNewLine(true);
      Format.PropertyValue distinctProperty1 = this.GetDistinctProperty(Format.PropertyId.Preformatted);
      Format.PropertyValue distinctProperty2 = this.GetDistinctProperty(Format.PropertyId.QuotingLevelDelta);
      if (!distinctProperty1.IsNull && distinctProperty1.Bool)
      {
        Format.FormatStyle style = this.FormatStore.GetStyle(14);
        this.SubtractDefaultContainerPropertiesFromDistinct(Format.FlagProperties.AllOff, style.PropertyList);
        this.Writer.WriteStartTag(HtmlNameIndex.Pre);
      }
      else if (!distinctProperty2.IsNull && distinctProperty2.Integer != 0)
      {
        for (int index = 0; index < distinctProperty2.Integer; ++index)
        {
          this.Writer.WriteStartTag(HtmlNameIndex.Div);
          this.Writer.WriteAttribute(HtmlNameIndex.Class, "EmailQuote");
        }
      }
      else
      {
        if (this.SourceFormat == Format.SourceFormat.Text)
          this.ApplyCharFormat();
        this.Writer.WriteStartTag(HtmlNameIndex.Div);
        if (this.SourceFormat == Format.SourceFormat.Text)
          this.Writer.WriteAttribute(HtmlNameIndex.Class, "PlainText");
      }
      this.OutputBlockTagAttributes();
      bool styleAttributeOpen = false;
      this.OutputBlockCssProperties(ref styleAttributeOpen);
      if (this.SourceFormat != Format.SourceFormat.Text)
        this.ApplyCharFormat();
      if (this.CurrentNode.FirstChild.IsNull)
        this.Writer.WriteText(' ');
      else if (this.CurrentNode.FirstChild == this.CurrentNode.LastChild && this.CurrentNode.FirstChild.NodeType == Format.FormatContainerType.Text)
      {
        Format.FormatNode firstChild = this.CurrentNode.FirstChild;
        if ((int) firstChild.BeginTextPosition + 1 == (int) firstChild.EndTextPosition && this.FormatStore.GetTextRun(firstChild.BeginTextPosition).Type == Format.TextRunType.Space)
        {
          this.Writer.WriteText(' ');
          this.EndBlockContainer();
          return false;
        }
      }
      return true;
    }

    protected override void EndBlockContainer()
    {
      Format.PropertyValue distinctProperty1 = this.GetDistinctProperty(Format.PropertyId.Preformatted);
      Format.PropertyValue distinctProperty2 = this.GetDistinctProperty(Format.PropertyId.QuotingLevelDelta);
      if (this.SourceFormat != Format.SourceFormat.Text)
        this.RevertCharFormat();
      if (!distinctProperty1.IsNull && distinctProperty1.Bool)
        this.Writer.WriteEndTag(HtmlNameIndex.Pre);
      else if (!distinctProperty2.IsNull && distinctProperty2.Integer != 0)
      {
        for (int index = 0; index < distinctProperty2.Integer; ++index)
          this.Writer.WriteEndTag(HtmlNameIndex.Div);
      }
      else
      {
        this.Writer.WriteEndTag(HtmlNameIndex.Div);
        if (this.SourceFormat == Format.SourceFormat.Text)
          this.RevertCharFormat();
      }
      this.Writer.WriteNewLine(true);
    }

    protected override bool StartInlineContainer()
    {
      return true;
    }

    protected override void EndInlineContainer()
    {
    }

    protected override void Dispose(bool disposing)
    {
      if (this.Writer != null && this.Writer != null)
        this.Writer.Dispose();
      this.Writer = (HtmlWriter) null;
      base.Dispose(disposing);
    }

    private void CloseHyperLink()
    {
      bool tagDropped = false;
      bool flag = false;
      if (this.endTagActionStackTop != 0 && this.endTagActionStack[this.endTagActionStackTop - 1].TagLevel == this.hyperlinkLevel)
      {
        --this.endTagActionStackTop;
        tagDropped = this.endTagActionStack[this.endTagActionStackTop].Drop;
        flag = this.endTagActionStack[this.endTagActionStackTop].Callback;
      }
      if (flag)
      {
        this.callbackContext.InitializeTag(true, HtmlNameIndex.A, tagDropped);
        this.callbackContext.InitializeFragment(false);
        this.callback((HtmlTagContext) this.callbackContext, this.Writer);
        this.callbackContext.UninitializeFragment();
      }
      else
      {
        if (tagDropped)
          return;
        this.Writer.WriteEndTag(HtmlNameIndex.A);
      }
    }

    private void ApplyCharFormat()
    {
      this.scratchBuffer.Reset();
      Format.FlagProperties distinctFlags = this.GetDistinctFlags();
      Format.PropertyValue distinctProperty1 = this.GetDistinctProperty(Format.PropertyId.FontSize);
      if (!distinctProperty1.IsNull && !distinctProperty1.IsHtmlFontUnits && !distinctProperty1.IsRelativeHtmlFontUnits)
      {
        this.scratchBuffer.Append("font-size:");
        HtmlSupport.AppendCssFontSize(ref this.scratchBuffer, distinctProperty1);
        this.scratchBuffer.Append(';');
      }
      Format.PropertyValue distinctProperty2 = this.GetDistinctProperty(Format.PropertyId.BackColor);
      if (distinctProperty2.IsColor)
      {
        this.scratchBuffer.Append("background-color:");
        HtmlSupport.AppendColor(ref this.scratchBuffer, distinctProperty2);
        this.scratchBuffer.Append(';');
      }
      Globalization.Culture culture = (Globalization.Culture) null;
      Format.PropertyValue distinctProperty3 = this.GetDistinctProperty(Format.PropertyId.Language);
      if (distinctProperty3.IsInteger && (!Globalization.Culture.TryGetCulture(distinctProperty3.Integer, out culture) || string.IsNullOrEmpty(culture.Name)))
        culture = (Globalization.Culture) null;
      if ((this.CurrentNode.NodeType & Format.FormatContainerType.BlockFlag) == Format.FormatContainerType.Null)
      {
        Format.PropertyValue distinctProperty4 = this.GetDistinctProperty(Format.PropertyId.Display);
        Format.PropertyValue distinctProperty5 = this.GetDistinctProperty(Format.PropertyId.UnicodeBiDi);
        if (!distinctProperty4.IsNull)
        {
          string displayString = HtmlSupport.GetDisplayString(distinctProperty4);
          if (displayString != null)
          {
            this.scratchBuffer.Append("display:");
            this.scratchBuffer.Append(displayString);
            this.scratchBuffer.Append(";");
          }
        }
        if (distinctFlags.IsDefined(Format.PropertyId.Visible))
          this.scratchBuffer.Append(distinctFlags.IsOn(Format.PropertyId.Visible) ? "visibility:visible;" : "visibility:hidden;");
        if (!distinctProperty5.IsNull)
        {
          string unicodeBiDiString = HtmlSupport.GetUnicodeBiDiString(distinctProperty5);
          if (unicodeBiDiString != null)
          {
            this.scratchBuffer.Append("unicode-bidi:");
            this.scratchBuffer.Append(unicodeBiDiString);
            this.scratchBuffer.Append(";");
          }
        }
      }
      if (distinctFlags.IsDefinedAndOff(Format.PropertyId.FirstFlag))
        this.scratchBuffer.Append("font-weight:normal;");
      if (distinctFlags.IsDefined(Format.PropertyId.SmallCaps))
        this.scratchBuffer.Append(distinctFlags.IsOn(Format.PropertyId.SmallCaps) ? "font-variant:small-caps;" : "font-variant:normal;");
      if (distinctFlags.IsDefined(Format.PropertyId.Capitalize))
        this.scratchBuffer.Append(distinctFlags.IsOn(Format.PropertyId.Capitalize) ? "text-transform:uppercase;" : "text-transform:none;");
      Format.PropertyValue distinctProperty6 = this.GetDistinctProperty(Format.PropertyId.FontFace);
      Format.PropertyValue distinctProperty7 = this.GetDistinctProperty(Format.PropertyId.FontColor);
      if (!distinctProperty6.IsNull || !distinctProperty1.IsNull || !distinctProperty7.IsNull)
      {
        this.Writer.WriteStartTag(HtmlNameIndex.Font);
        if (!distinctProperty6.IsNull)
        {
          this.Writer.WriteAttributeName(HtmlNameIndex.Face);
          if (distinctProperty6.IsMultiValue)
          {
            Format.MultiValue multiValue = this.FormatStore.GetMultiValue(distinctProperty6);
            for (int index = 0; index < multiValue.Length; ++index)
            {
              string @string = multiValue.GetStringValue(index).GetString();
              if (index != 0)
                this.Writer.WriteAttributeValue(",");
              this.Writer.WriteAttributeValue(@string);
            }
          }
          else
            this.Writer.WriteAttributeValue(this.FormatStore.GetStringValue(distinctProperty6).GetString());
        }
        BufferString bufferString;
        if (!distinctProperty1.IsNull)
        {
          bufferString = HtmlSupport.FormatFontSize(ref this.scratchValueBuffer, distinctProperty1);
          if (bufferString.Length != 0)
            this.Writer.WriteAttribute(HtmlNameIndex.Size, bufferString);
        }
        if (!distinctProperty7.IsNull)
        {
          bufferString = HtmlSupport.FormatColor(ref this.scratchValueBuffer, distinctProperty7);
          if (bufferString.Length != 0)
            this.Writer.WriteAttribute(HtmlNameIndex.Color, bufferString);
        }
      }
      if (this.scratchBuffer.Length != 0 || distinctFlags.IsDefined(Format.PropertyId.RightToLeft) || culture != null)
      {
        this.Writer.WriteStartTag(HtmlNameIndex.Span);
        if (this.scratchBuffer.Length != 0)
        {
          this.Writer.WriteAttributeName(HtmlNameIndex.Style);
          this.Writer.WriteAttributeValue(this.scratchBuffer.BufferString);
        }
        if (distinctFlags.IsDefined(Format.PropertyId.RightToLeft))
        {
          this.Writer.WriteAttributeName(HtmlNameIndex.Dir);
          this.Writer.WriteAttributeValue(distinctFlags.IsOn(Format.PropertyId.RightToLeft) ? "rtl" : "ltr");
        }
        if (culture != null)
        {
          this.Writer.WriteAttributeName(HtmlNameIndex.Lang);
          this.Writer.WriteAttributeValue(culture.Name);
        }
      }
      if (distinctFlags.IsDefinedAndOn(Format.PropertyId.FirstFlag))
        this.Writer.WriteStartTag(HtmlNameIndex.B);
      if (distinctFlags.IsDefinedAndOn(Format.PropertyId.Italic))
        this.Writer.WriteStartTag(HtmlNameIndex.I);
      if (distinctFlags.IsDefinedAndOn(Format.PropertyId.Underline))
        this.Writer.WriteStartTag(HtmlNameIndex.U);
      if (distinctFlags.IsDefinedAndOn(Format.PropertyId.Subscript))
        this.Writer.WriteStartTag(HtmlNameIndex.Sub);
      if (distinctFlags.IsDefinedAndOn(Format.PropertyId.Superscript))
        this.Writer.WriteStartTag(HtmlNameIndex.Sup);
      if (!distinctFlags.IsDefinedAndOn(Format.PropertyId.Strikethrough))
        return;
      this.Writer.WriteStartTag(HtmlNameIndex.Strike);
    }

    private void RevertCharFormat()
    {
      Format.FlagProperties distinctFlags = this.GetDistinctFlags();
      bool flag1 = false;
      bool flag2 = false;
      Format.PropertyValue distinctProperty1 = this.GetDistinctProperty(Format.PropertyId.FontSize);
      if (!distinctProperty1.IsNull && !distinctProperty1.IsHtmlFontUnits && !distinctProperty1.IsRelativeHtmlFontUnits)
        flag2 = true;
      if (this.GetDistinctProperty(Format.PropertyId.BackColor).IsColor)
        flag2 = true;
      Globalization.Culture culture = (Globalization.Culture) null;
      Format.PropertyValue distinctProperty2 = this.GetDistinctProperty(Format.PropertyId.Language);
      if (distinctProperty2.IsInteger && Globalization.Culture.TryGetCulture(distinctProperty2.Integer, out culture) && !string.IsNullOrEmpty(culture.Name))
        flag2 = true;
      if ((this.CurrentNode.NodeType & Format.FormatContainerType.BlockFlag) == Format.FormatContainerType.Null)
      {
        Format.PropertyValue distinctProperty3 = this.GetDistinctProperty(Format.PropertyId.Display);
        Format.PropertyValue distinctProperty4 = this.GetDistinctProperty(Format.PropertyId.UnicodeBiDi);
        if (!distinctProperty3.IsNull && HtmlSupport.GetDisplayString(distinctProperty3) != null)
          flag2 = true;
        if (distinctFlags.IsDefined(Format.PropertyId.Visible))
          flag2 = true;
        if (!distinctProperty4.IsNull && HtmlSupport.GetUnicodeBiDiString(distinctProperty4) != null)
          flag2 = true;
      }
      if (distinctFlags.IsDefinedAndOff(Format.PropertyId.FirstFlag))
        flag2 = true;
      if (distinctFlags.IsDefined(Format.PropertyId.SmallCaps))
        flag2 = true;
      if (distinctFlags.IsDefined(Format.PropertyId.Capitalize))
        flag2 = true;
      if (distinctFlags.IsDefined(Format.PropertyId.RightToLeft))
        flag2 = true;
      Format.PropertyValue distinctProperty5 = this.GetDistinctProperty(Format.PropertyId.FontFace);
      Format.PropertyValue distinctProperty6 = this.GetDistinctProperty(Format.PropertyId.FontColor);
      if (!distinctProperty5.IsNull || !distinctProperty1.IsNull || !distinctProperty6.IsNull)
        flag1 = true;
      if (distinctFlags.IsDefinedAndOn(Format.PropertyId.Strikethrough))
        this.Writer.WriteEndTag(HtmlNameIndex.Strike);
      if (distinctFlags.IsDefinedAndOn(Format.PropertyId.Superscript))
        this.Writer.WriteEndTag(HtmlNameIndex.Sup);
      if (distinctFlags.IsDefinedAndOn(Format.PropertyId.Subscript))
        this.Writer.WriteEndTag(HtmlNameIndex.Sub);
      if (distinctFlags.IsDefinedAndOn(Format.PropertyId.Underline))
        this.Writer.WriteEndTag(HtmlNameIndex.U);
      if (distinctFlags.IsDefinedAndOn(Format.PropertyId.Italic))
        this.Writer.WriteEndTag(HtmlNameIndex.I);
      if (distinctFlags.IsDefinedAndOn(Format.PropertyId.FirstFlag))
        this.Writer.WriteEndTag(HtmlNameIndex.B);
      if (flag2)
        this.Writer.WriteEndTag(HtmlNameIndex.Span);
      if (!flag1)
        return;
      this.Writer.WriteEndTag(HtmlNameIndex.Font);
    }

    private void OutputBlockCssProperties(ref bool styleAttributeOpen)
    {
      Format.PropertyValue distinctProperty1 = this.GetDistinctProperty(Format.PropertyId.Display);
      Format.PropertyValue distinctProperty2 = this.GetDistinctProperty(Format.PropertyId.Visible);
      Format.PropertyValue distinctProperty3 = this.GetDistinctProperty(Format.PropertyId.Height);
      Format.PropertyValue distinctProperty4 = this.GetDistinctProperty(Format.PropertyId.Width);
      Format.PropertyValue distinctProperty5 = this.GetDistinctProperty(Format.PropertyId.UnicodeBiDi);
      Format.PropertyValue distinctProperty6 = this.GetDistinctProperty(Format.PropertyId.FirstLineIndent);
      Format.PropertyValue distinctProperty7 = this.GetDistinctProperty(Format.PropertyId.TextAlignment);
      Format.PropertyValue distinctProperty8 = this.GetDistinctProperty(Format.PropertyId.BackColor);
      Format.PropertyValue distinctProperty9 = this.GetDistinctProperty(Format.PropertyId.Margins);
      Format.PropertyValue distinctProperty10 = this.GetDistinctProperty(Format.PropertyId.RightMargin);
      Format.PropertyValue distinctProperty11 = this.GetDistinctProperty(Format.PropertyId.BottomMargin);
      Format.PropertyValue distinctProperty12 = this.GetDistinctProperty(Format.PropertyId.LeftMargin);
      Format.PropertyValue distinctProperty13 = this.GetDistinctProperty(Format.PropertyId.Paddings);
      Format.PropertyValue distinctProperty14 = this.GetDistinctProperty(Format.PropertyId.RightPadding);
      Format.PropertyValue distinctProperty15 = this.GetDistinctProperty(Format.PropertyId.BottomPadding);
      Format.PropertyValue distinctProperty16 = this.GetDistinctProperty(Format.PropertyId.LeftPadding);
      Format.PropertyValue distinctProperty17 = this.GetDistinctProperty(Format.PropertyId.BorderWidths);
      Format.PropertyValue distinctProperty18 = this.GetDistinctProperty(Format.PropertyId.RightBorderWidth);
      Format.PropertyValue distinctProperty19 = this.GetDistinctProperty(Format.PropertyId.BottomBorderWidth);
      Format.PropertyValue distinctProperty20 = this.GetDistinctProperty(Format.PropertyId.LeftBorderWidth);
      Format.PropertyValue distinctProperty21 = this.GetDistinctProperty(Format.PropertyId.BorderStyles);
      Format.PropertyValue distinctProperty22 = this.GetDistinctProperty(Format.PropertyId.RightBorderStyle);
      Format.PropertyValue distinctProperty23 = this.GetDistinctProperty(Format.PropertyId.BottomBorderStyle);
      Format.PropertyValue distinctProperty24 = this.GetDistinctProperty(Format.PropertyId.LeftBorderStyle);
      Format.PropertyValue distinctProperty25 = this.GetDistinctProperty(Format.PropertyId.BorderColors);
      Format.PropertyValue distinctProperty26 = this.GetDistinctProperty(Format.PropertyId.RightBorderColor);
      Format.PropertyValue distinctProperty27 = this.GetDistinctProperty(Format.PropertyId.BottomBorderColor);
      Format.PropertyValue distinctProperty28 = this.GetDistinctProperty(Format.PropertyId.LeftBorderColor);
      if (!distinctProperty2.IsNull || !distinctProperty1.IsNull || (!distinctProperty5.IsNull || !distinctProperty4.IsNull) || !distinctProperty3.IsNull)
      {
        if (!styleAttributeOpen)
        {
          this.Writer.WriteAttributeName(HtmlNameIndex.Style);
          styleAttributeOpen = true;
        }
        if (!distinctProperty1.IsNull)
        {
          string displayString = HtmlSupport.GetDisplayString(distinctProperty1);
          if (displayString != null)
          {
            this.scratchBuffer.Append("display:");
            this.scratchBuffer.Append(displayString);
            this.scratchBuffer.Append(";");
          }
        }
        if (!distinctProperty2.IsNull)
          this.scratchBuffer.Append(distinctProperty2.Bool ? "visibility:visible;" : "visibility:hidden;");
        if (!distinctProperty4.IsNull)
        {
          BufferString bufferString = HtmlSupport.FormatLength(ref this.scratchBuffer, distinctProperty4);
          if (bufferString.Length != 0)
          {
            this.Writer.WriteAttributeValue("width:");
            this.Writer.WriteAttributeValue(bufferString);
            this.Writer.WriteAttributeValue(";");
          }
        }
        if (!distinctProperty3.IsNull)
        {
          BufferString bufferString = HtmlSupport.FormatLength(ref this.scratchBuffer, distinctProperty3);
          if (bufferString.Length != 0)
          {
            this.Writer.WriteAttributeValue("height:");
            this.Writer.WriteAttributeValue(bufferString);
            this.Writer.WriteAttributeValue(";");
          }
        }
        if (!distinctProperty5.IsNull)
        {
          string unicodeBiDiString = HtmlSupport.GetUnicodeBiDiString(distinctProperty5);
          if (unicodeBiDiString != null)
          {
            this.Writer.WriteAttributeValue("unicode-bidi:");
            this.Writer.WriteAttributeValue(unicodeBiDiString);
            this.Writer.WriteAttributeValue(";");
          }
        }
      }
      if (!distinctProperty6.IsNull || !distinctProperty7.IsNull || !distinctProperty8.IsNull)
      {
        if (!styleAttributeOpen)
        {
          this.Writer.WriteAttributeName(HtmlNameIndex.Style);
          styleAttributeOpen = true;
        }
        if (!distinctProperty6.IsNull)
        {
          BufferString bufferString = HtmlSupport.FormatLength(ref this.scratchBuffer, distinctProperty6);
          if (bufferString.Length != 0)
          {
            this.Writer.WriteAttributeValue("text-indent:");
            this.Writer.WriteAttributeValue(bufferString);
            this.Writer.WriteAttributeValue(";");
          }
        }
        if (!distinctProperty7.IsNull && distinctProperty7.IsEnum && distinctProperty7.Enum < HtmlSupport.TextAlignmentEnumeration.Length)
        {
          this.Writer.WriteAttributeValue("text-align:");
          this.Writer.WriteAttributeValue(HtmlSupport.TextAlignmentEnumeration[distinctProperty7.Enum].Name);
          this.Writer.WriteAttributeValue(";");
        }
        if (!distinctProperty8.IsNull)
        {
          BufferString bufferString = HtmlSupport.FormatColor(ref this.scratchBuffer, distinctProperty8);
          if (bufferString.Length != 0)
          {
            this.Writer.WriteAttributeValue("background-color:");
            this.Writer.WriteAttributeValue(bufferString);
            this.Writer.WriteAttributeValue(";");
          }
        }
      }
      if (!distinctProperty9.IsNull || !distinctProperty10.IsNull || (!distinctProperty11.IsNull || !distinctProperty12.IsNull))
      {
        if (!styleAttributeOpen)
        {
          this.Writer.WriteAttributeName(HtmlNameIndex.Style);
          styleAttributeOpen = true;
        }
        this.OutputMarginAndPaddingProperties("margin", distinctProperty9, distinctProperty10, distinctProperty11, distinctProperty12);
      }
      if (!distinctProperty13.IsNull || !distinctProperty14.IsNull || (!distinctProperty15.IsNull || !distinctProperty16.IsNull))
      {
        if (!styleAttributeOpen)
        {
          this.Writer.WriteAttributeName(HtmlNameIndex.Style);
          styleAttributeOpen = true;
        }
        this.OutputMarginAndPaddingProperties("padding", distinctProperty13, distinctProperty14, distinctProperty15, distinctProperty16);
      }
      if (distinctProperty17.IsNull && distinctProperty18.IsNull && (distinctProperty19.IsNull && distinctProperty20.IsNull) && (distinctProperty21.IsNull && distinctProperty22.IsNull && (distinctProperty23.IsNull && distinctProperty24.IsNull)) && (distinctProperty25.IsNull && distinctProperty26.IsNull && (distinctProperty27.IsNull && distinctProperty28.IsNull)))
        return;
      if (!styleAttributeOpen)
      {
        this.Writer.WriteAttributeName(HtmlNameIndex.Style);
        styleAttributeOpen = true;
      }
      this.OutputBorderProperties(distinctProperty17, distinctProperty18, distinctProperty19, distinctProperty20, distinctProperty21, distinctProperty22, distinctProperty23, distinctProperty24, distinctProperty25, distinctProperty26, distinctProperty27, distinctProperty28);
    }

    private void OutputMarginAndPaddingProperties(string name, Format.PropertyValue topValue, Format.PropertyValue rightValue, Format.PropertyValue bottomValue, Format.PropertyValue leftValue)
    {
      int num = 0;
      if (!topValue.IsNull)
        ++num;
      if (!rightValue.IsNull)
        ++num;
      if (!bottomValue.IsNull)
        ++num;
      if (!leftValue.IsNull)
        ++num;
      if (num == 4)
      {
        this.Writer.WriteAttributeValue(name);
        this.Writer.WriteAttributeValue(":");
        if (topValue == rightValue && topValue == bottomValue && topValue == leftValue)
          this.OutputLengthPropertyValue(topValue);
        else if (topValue == bottomValue && rightValue == leftValue)
          this.OutputCompositeLengthPropertyValue(topValue, rightValue);
        else
          this.OutputCompositeLengthPropertyValue(topValue, rightValue, bottomValue, leftValue);
        this.Writer.WriteAttributeValue(";");
      }
      else
      {
        if (!topValue.IsNull)
        {
          this.Writer.WriteAttributeValue(name);
          this.Writer.WriteAttributeValue("-top:");
          this.OutputLengthPropertyValue(topValue);
          this.Writer.WriteAttributeValue(";");
        }
        if (!rightValue.IsNull)
        {
          this.Writer.WriteAttributeValue(name);
          this.Writer.WriteAttributeValue("-right:");
          this.OutputLengthPropertyValue(rightValue);
          this.Writer.WriteAttributeValue(";");
        }
        if (!bottomValue.IsNull)
        {
          this.Writer.WriteAttributeValue(name);
          this.Writer.WriteAttributeValue("-bottom:");
          this.OutputLengthPropertyValue(bottomValue);
          this.Writer.WriteAttributeValue(";");
        }
        if (leftValue.IsNull)
          return;
        this.Writer.WriteAttributeValue(name);
        this.Writer.WriteAttributeValue("-left:");
        this.OutputLengthPropertyValue(leftValue);
        this.Writer.WriteAttributeValue(";");
      }
    }

    private void OutputBorderProperties(Format.PropertyValue topBorderWidth, Format.PropertyValue rightBorderWidth, Format.PropertyValue bottomBorderWidth, Format.PropertyValue leftBorderWidth, Format.PropertyValue topBorderStyle, Format.PropertyValue rightBorderStyle, Format.PropertyValue bottomBorderStyle, Format.PropertyValue leftBorderStyle, Format.PropertyValue topBorderColor, Format.PropertyValue rightBorderColor, Format.PropertyValue bottomBorderColor, Format.PropertyValue leftBorderColor)
    {
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      int num4 = 0;
      int num5 = 0;
      int num6 = 0;
      int num7 = 0;
      if (!topBorderWidth.IsNull)
      {
        ++num1;
        ++num4;
      }
      if (!rightBorderWidth.IsNull)
      {
        ++num1;
        ++num5;
      }
      if (!bottomBorderWidth.IsNull)
      {
        ++num1;
        ++num6;
      }
      if (!leftBorderWidth.IsNull)
      {
        ++num1;
        ++num7;
      }
      if (!topBorderStyle.IsNull)
      {
        ++num2;
        ++num4;
      }
      if (!rightBorderStyle.IsNull)
      {
        ++num2;
        ++num5;
      }
      if (!bottomBorderStyle.IsNull)
      {
        ++num2;
        ++num6;
      }
      if (!leftBorderStyle.IsNull)
      {
        ++num2;
        ++num7;
      }
      if (!topBorderColor.IsNull)
      {
        ++num3;
        ++num4;
      }
      if (!rightBorderColor.IsNull)
      {
        ++num3;
        ++num5;
      }
      if (!bottomBorderColor.IsNull)
      {
        ++num3;
        ++num6;
      }
      if (!leftBorderColor.IsNull)
      {
        ++num3;
        ++num7;
      }
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      bool flag4 = false;
      bool flag5 = false;
      bool flag6 = false;
      if (num1 == 4 && topBorderWidth == bottomBorderWidth && rightBorderWidth == leftBorderWidth)
      {
        flag2 = true;
        flag1 = topBorderWidth == rightBorderWidth;
      }
      if (num2 == 4 && topBorderStyle == bottomBorderStyle && rightBorderStyle == leftBorderStyle)
      {
        flag4 = true;
        flag3 = topBorderStyle == rightBorderStyle;
      }
      if (num3 == 4 && topBorderColor == bottomBorderColor && rightBorderColor == leftBorderColor)
      {
        flag6 = true;
        flag5 = topBorderColor == rightBorderColor;
      }
      if (num1 == 4 && num2 == 4 && num3 == 4)
      {
        if (flag1 && flag3 && flag5)
        {
          this.Writer.WriteAttributeValue("border:");
          this.OutputCompositeBorderSidePropertyValue(topBorderWidth, topBorderStyle, topBorderColor);
          this.Writer.WriteAttributeValue(";");
        }
        else
        {
          this.Writer.WriteAttributeValue("border-width:");
          if (flag1)
            this.OutputBorderWidthPropertyValue(topBorderWidth);
          else if (flag2)
            this.OutputCompositeBorderWidthPropertyValue(topBorderWidth, rightBorderWidth);
          else
            this.OutputCompositeBorderWidthPropertyValue(topBorderWidth, rightBorderWidth, bottomBorderWidth, leftBorderWidth);
          this.Writer.WriteAttributeValue(";");
          this.Writer.WriteAttributeValue("border-style:");
          if (flag3)
            this.OutputBorderStylePropertyValue(topBorderStyle);
          else if (flag4)
            this.OutputCompositeBorderStylePropertyValue(topBorderStyle, rightBorderStyle);
          else
            this.OutputCompositeBorderStylePropertyValue(topBorderStyle, rightBorderStyle, bottomBorderStyle, leftBorderStyle);
          this.Writer.WriteAttributeValue(";");
          this.Writer.WriteAttributeValue("border-color:");
          if (flag5)
            this.OutputBorderColorPropertyValue(topBorderColor);
          else if (flag6)
            this.OutputCompositeBorderColorPropertyValue(topBorderColor, rightBorderColor);
          else
            this.OutputCompositeBorderColorPropertyValue(topBorderColor, rightBorderColor, bottomBorderColor, leftBorderColor);
          this.Writer.WriteAttributeValue(";");
        }
      }
      else
      {
        bool flag7 = false;
        bool flag8 = false;
        bool flag9 = false;
        bool flag10 = false;
        bool flag11 = false;
        bool flag12 = false;
        bool flag13 = false;
        bool flag14 = false;
        bool flag15 = false;
        bool flag16 = false;
        bool flag17 = false;
        bool flag18 = false;
        if (num1 == 4 || num2 == 4 || num3 == 4)
        {
          if (num1 == 4)
          {
            this.Writer.WriteAttributeValue("border-width:");
            if (flag1)
              this.OutputBorderWidthPropertyValue(topBorderWidth);
            else if (flag2)
              this.OutputCompositeBorderWidthPropertyValue(topBorderWidth, rightBorderWidth);
            else
              this.OutputCompositeBorderWidthPropertyValue(topBorderWidth, rightBorderWidth, bottomBorderWidth, leftBorderWidth);
            this.Writer.WriteAttributeValue(";");
            flag7 = true;
            flag8 = true;
            flag9 = true;
            flag10 = true;
          }
          if (num2 == 4)
          {
            this.Writer.WriteAttributeValue("border-style:");
            if (flag3)
              this.OutputBorderStylePropertyValue(topBorderStyle);
            else if (flag4)
              this.OutputCompositeBorderStylePropertyValue(topBorderStyle, rightBorderStyle);
            else
              this.OutputCompositeBorderStylePropertyValue(topBorderStyle, rightBorderStyle, bottomBorderStyle, leftBorderStyle);
            this.Writer.WriteAttributeValue(";");
            flag11 = true;
            flag12 = true;
            flag13 = true;
            flag14 = true;
          }
          if (num3 == 4)
          {
            this.Writer.WriteAttributeValue("border-color:");
            if (flag5)
              this.OutputBorderColorPropertyValue(topBorderColor);
            else if (flag6)
              this.OutputCompositeBorderColorPropertyValue(topBorderColor, rightBorderColor);
            else
              this.OutputCompositeBorderColorPropertyValue(topBorderColor, rightBorderColor, bottomBorderColor, leftBorderColor);
            this.Writer.WriteAttributeValue(";");
            flag15 = true;
            flag16 = true;
            flag17 = true;
            flag18 = true;
          }
        }
        else if (num4 == 3 || num5 == 3 || (num6 == 3 || num7 == 3))
        {
          if (num4 == 3)
          {
            this.Writer.WriteAttributeValue("border-top:");
            this.OutputCompositeBorderSidePropertyValue(topBorderWidth, topBorderStyle, topBorderColor);
            this.Writer.WriteAttributeValue(";");
            flag7 = true;
            flag11 = true;
            flag15 = true;
          }
          if (num5 == 3)
          {
            this.Writer.WriteAttributeValue("border-right:");
            this.OutputCompositeBorderSidePropertyValue(rightBorderWidth, rightBorderStyle, rightBorderColor);
            this.Writer.WriteAttributeValue(";");
            flag8 = true;
            flag12 = true;
            flag16 = true;
          }
          if (num6 == 3)
          {
            this.Writer.WriteAttributeValue("border-bottom:");
            this.OutputCompositeBorderSidePropertyValue(bottomBorderWidth, bottomBorderStyle, bottomBorderColor);
            this.Writer.WriteAttributeValue(";");
            flag9 = true;
            flag13 = true;
            flag17 = true;
          }
          if (num7 == 3)
          {
            this.Writer.WriteAttributeValue("border-left:");
            this.OutputCompositeBorderSidePropertyValue(leftBorderWidth, leftBorderStyle, leftBorderColor);
            this.Writer.WriteAttributeValue(";");
            flag10 = true;
            flag14 = true;
            flag18 = true;
          }
        }
        if (!flag7 && !topBorderWidth.IsNull)
        {
          this.Writer.WriteAttributeValue("border-top-width:");
          this.OutputBorderWidthPropertyValue(topBorderWidth);
          this.Writer.WriteAttributeValue(";");
        }
        if (!flag8 && !rightBorderWidth.IsNull)
        {
          this.Writer.WriteAttributeValue("border-right-width:");
          this.OutputBorderWidthPropertyValue(rightBorderWidth);
          this.Writer.WriteAttributeValue(";");
        }
        if (!flag9 && !bottomBorderWidth.IsNull)
        {
          this.Writer.WriteAttributeValue("border-bottom-width:");
          this.OutputBorderWidthPropertyValue(bottomBorderWidth);
          this.Writer.WriteAttributeValue(";");
        }
        if (!flag10 && !leftBorderWidth.IsNull)
        {
          this.Writer.WriteAttributeValue("border-left-width:");
          this.OutputBorderWidthPropertyValue(leftBorderWidth);
          this.Writer.WriteAttributeValue(";");
        }
        if (!flag11 && !topBorderStyle.IsNull)
        {
          this.Writer.WriteAttributeValue("border-top-style:");
          this.OutputBorderStylePropertyValue(topBorderStyle);
          this.Writer.WriteAttributeValue(";");
        }
        if (!flag12 && !rightBorderStyle.IsNull)
        {
          this.Writer.WriteAttributeValue("border-right-style:");
          this.OutputBorderStylePropertyValue(rightBorderStyle);
          this.Writer.WriteAttributeValue(";");
        }
        if (!flag13 && !bottomBorderStyle.IsNull)
        {
          this.Writer.WriteAttributeValue("border-bottom-style:");
          this.OutputBorderStylePropertyValue(bottomBorderStyle);
          this.Writer.WriteAttributeValue(";");
        }
        if (!flag14 && !leftBorderStyle.IsNull)
        {
          this.Writer.WriteAttributeValue("border-left-style:");
          this.OutputBorderStylePropertyValue(leftBorderStyle);
          this.Writer.WriteAttributeValue(";");
        }
        if (!flag15 && !topBorderColor.IsNull)
        {
          this.Writer.WriteAttributeValue("border-top-color:");
          this.OutputBorderColorPropertyValue(topBorderColor);
          this.Writer.WriteAttributeValue(";");
        }
        if (!flag16 && !rightBorderColor.IsNull)
        {
          this.Writer.WriteAttributeValue("border-right-color:");
          this.OutputBorderColorPropertyValue(rightBorderColor);
          this.Writer.WriteAttributeValue(";");
        }
        if (!flag17 && !bottomBorderColor.IsNull)
        {
          this.Writer.WriteAttributeValue("border-bottom-color:");
          this.OutputBorderColorPropertyValue(bottomBorderColor);
          this.Writer.WriteAttributeValue(";");
        }
        if (flag18 || leftBorderColor.IsNull)
          return;
        this.Writer.WriteAttributeValue("border-left-color:");
        this.OutputBorderColorPropertyValue(leftBorderColor);
        this.Writer.WriteAttributeValue(";");
      }
    }

    private void OutputCompositeBorderSidePropertyValue(Format.PropertyValue width, Format.PropertyValue style, Format.PropertyValue color)
    {
      this.OutputBorderWidthPropertyValue(width);
      this.Writer.WriteAttributeValue(" ");
      this.OutputBorderStylePropertyValue(style);
      this.Writer.WriteAttributeValue(" ");
      this.OutputBorderColorPropertyValue(color);
    }

    private void OutputCompositeLengthPropertyValue(Format.PropertyValue topBottom, Format.PropertyValue rightLeft)
    {
      this.OutputLengthPropertyValue(topBottom);
      this.Writer.WriteAttributeValue(" ");
      this.OutputLengthPropertyValue(rightLeft);
    }

    private void OutputCompositeLengthPropertyValue(Format.PropertyValue top, Format.PropertyValue right, Format.PropertyValue bottom, Format.PropertyValue left)
    {
      this.OutputLengthPropertyValue(top);
      this.Writer.WriteAttributeValue(" ");
      this.OutputLengthPropertyValue(right);
      this.Writer.WriteAttributeValue(" ");
      this.OutputLengthPropertyValue(bottom);
      this.Writer.WriteAttributeValue(" ");
      this.OutputLengthPropertyValue(left);
    }

    private void OutputCompositeBorderWidthPropertyValue(Format.PropertyValue topBottom, Format.PropertyValue rightLeft)
    {
      this.OutputBorderWidthPropertyValue(topBottom);
      this.Writer.WriteAttributeValue(" ");
      this.OutputBorderWidthPropertyValue(rightLeft);
    }

    private void OutputCompositeBorderWidthPropertyValue(Format.PropertyValue top, Format.PropertyValue right, Format.PropertyValue bottom, Format.PropertyValue left)
    {
      this.OutputBorderWidthPropertyValue(top);
      this.Writer.WriteAttributeValue(" ");
      this.OutputBorderWidthPropertyValue(right);
      this.Writer.WriteAttributeValue(" ");
      this.OutputBorderWidthPropertyValue(bottom);
      this.Writer.WriteAttributeValue(" ");
      this.OutputBorderWidthPropertyValue(left);
    }

    private void OutputCompositeBorderStylePropertyValue(Format.PropertyValue topBottom, Format.PropertyValue rightLeft)
    {
      this.OutputBorderStylePropertyValue(topBottom);
      this.Writer.WriteAttributeValue(" ");
      this.OutputBorderStylePropertyValue(rightLeft);
    }

    private void OutputCompositeBorderStylePropertyValue(Format.PropertyValue top, Format.PropertyValue right, Format.PropertyValue bottom, Format.PropertyValue left)
    {
      this.OutputBorderStylePropertyValue(top);
      this.Writer.WriteAttributeValue(" ");
      this.OutputBorderStylePropertyValue(right);
      this.Writer.WriteAttributeValue(" ");
      this.OutputBorderStylePropertyValue(bottom);
      this.Writer.WriteAttributeValue(" ");
      this.OutputBorderStylePropertyValue(left);
    }

    private void OutputCompositeBorderColorPropertyValue(Format.PropertyValue topBottom, Format.PropertyValue rightLeft)
    {
      this.OutputBorderColorPropertyValue(topBottom);
      this.Writer.WriteAttributeValue(" ");
      this.OutputBorderColorPropertyValue(rightLeft);
    }

    private void OutputCompositeBorderColorPropertyValue(Format.PropertyValue top, Format.PropertyValue right, Format.PropertyValue bottom, Format.PropertyValue left)
    {
      this.OutputBorderColorPropertyValue(top);
      this.Writer.WriteAttributeValue(" ");
      this.OutputBorderColorPropertyValue(right);
      this.Writer.WriteAttributeValue(" ");
      this.OutputBorderColorPropertyValue(bottom);
      this.Writer.WriteAttributeValue(" ");
      this.OutputBorderColorPropertyValue(left);
    }

    private void OutputLengthPropertyValue(Format.PropertyValue width)
    {
      BufferString bufferString = HtmlSupport.FormatLength(ref this.scratchBuffer, width);
      if (bufferString.Length != 0)
        this.Writer.WriteAttributeValue(bufferString);
      else
        this.Writer.WriteAttributeValue("0");
    }

    private void OutputBorderWidthPropertyValue(Format.PropertyValue width)
    {
      BufferString bufferString = HtmlSupport.FormatLength(ref this.scratchBuffer, width);
      if (bufferString.Length != 0)
        this.Writer.WriteAttributeValue(bufferString);
      else
        this.Writer.WriteAttributeValue("medium");
    }

    private void OutputBorderStylePropertyValue(Format.PropertyValue style)
    {
      string borderStyleString = HtmlSupport.GetBorderStyleString(style);
      if (borderStyleString != null)
        this.Writer.WriteAttributeValue(borderStyleString);
      else
        this.Writer.WriteAttributeValue("solid");
    }

    private void OutputBorderColorPropertyValue(Format.PropertyValue color)
    {
      BufferString bufferString = HtmlSupport.FormatColor(ref this.scratchBuffer, color);
      if (bufferString.Length != 0)
        this.Writer.WriteAttributeValue(bufferString);
      else
        this.Writer.WriteAttributeValue("black");
    }

    private void OutputTableCssProperties(ref bool styleAttributeOpen)
    {
      Format.PropertyValue distinctProperty1 = this.GetDistinctProperty(Format.PropertyId.Overloaded1);
      Format.PropertyValue distinctProperty2 = this.GetDistinctProperty(Format.PropertyId.Overloaded2);
      Format.PropertyValue distinctProperty3 = this.GetDistinctProperty(Format.PropertyId.TableShowEmptyCells);
      Format.PropertyValue distinctProperty4 = this.GetDistinctProperty(Format.PropertyId.TableCaptionSideTop);
      Format.PropertyValue distinctProperty5 = this.GetDistinctProperty(Format.PropertyId.TableBorderSpacingVertical);
      Format.PropertyValue distinctProperty6 = this.GetDistinctProperty(Format.PropertyId.TableBorderSpacingHorizontal);
      if (distinctProperty1.IsNull && distinctProperty2.IsNull && (distinctProperty3.IsNull && distinctProperty4.IsNull) && (distinctProperty5.IsNull && distinctProperty6.IsNull))
        return;
      if (!styleAttributeOpen)
      {
        this.Writer.WriteAttributeName(HtmlNameIndex.Style);
        styleAttributeOpen = true;
      }
      if (!distinctProperty1.IsNull)
      {
        this.Writer.WriteAttributeValue("table-layout:");
        this.Writer.WriteAttributeValue(distinctProperty1.Bool ? "fixed" : "auto");
        this.Writer.WriteAttributeValue(";");
      }
      if (!distinctProperty2.IsNull)
      {
        this.Writer.WriteAttributeValue("border-collapse:");
        this.Writer.WriteAttributeValue(distinctProperty2.Bool ? "collapse" : "separate");
        this.Writer.WriteAttributeValue(";");
      }
      if (!distinctProperty3.IsNull)
      {
        this.Writer.WriteAttributeValue("empty-cells:");
        this.Writer.WriteAttributeValue(distinctProperty3.Bool ? "show" : "hide");
        this.Writer.WriteAttributeValue(";");
      }
      if (!distinctProperty4.IsNull)
      {
        this.Writer.WriteAttributeValue("caption-side:");
        this.Writer.WriteAttributeValue(distinctProperty4.Bool ? "top" : "bottom");
        this.Writer.WriteAttributeValue(";");
      }
      if (distinctProperty5.IsNull || distinctProperty5.IsNull)
        return;
      BufferString bufferString1 = HtmlSupport.FormatLength(ref this.scratchBuffer, distinctProperty5);
      if (bufferString1.Length == 0)
        return;
      this.Writer.WriteAttributeValue("border-spacing:");
      this.Writer.WriteAttributeValue(bufferString1);
      if (distinctProperty5 != distinctProperty6)
      {
        BufferString bufferString2 = HtmlSupport.FormatLength(ref this.scratchBuffer, distinctProperty6);
        if (bufferString2.Length != 0)
        {
          this.Writer.WriteAttributeValue(" ");
          this.Writer.WriteAttributeValue(bufferString2);
        }
      }
      this.Writer.WriteAttributeValue(";");
    }

    private void OutputTableColumnCssProperties(ref bool styleAttributeOpen)
    {
      Format.PropertyValue distinctProperty1 = this.GetDistinctProperty(Format.PropertyId.Width);
      Format.PropertyValue distinctProperty2 = this.GetDistinctProperty(Format.PropertyId.BackColor);
      if (distinctProperty2.IsNull && distinctProperty1.IsNull)
        return;
      if (!styleAttributeOpen)
      {
        this.Writer.WriteAttributeName(HtmlNameIndex.Style);
        styleAttributeOpen = true;
      }
      if (!distinctProperty1.IsNull)
      {
        BufferString bufferString = HtmlSupport.FormatLength(ref this.scratchBuffer, distinctProperty1);
        if (bufferString.Length != 0)
        {
          this.Writer.WriteAttributeValue("width:");
          this.Writer.WriteAttributeValue(bufferString);
          this.Writer.WriteAttributeValue(";");
        }
      }
      if (distinctProperty2.IsNull)
        return;
      BufferString bufferString1 = HtmlSupport.FormatColor(ref this.scratchBuffer, distinctProperty2);
      if (bufferString1.Length == 0)
        return;
      this.Writer.WriteAttributeValue("background-color:");
      this.Writer.WriteAttributeValue(bufferString1);
    }

    private void OutputBlockTagAttributes()
    {
      Format.PropertyValue distinctProperty1 = this.GetDistinctProperty(Format.PropertyId.RightToLeft);
      if (!distinctProperty1.IsNull)
        this.Writer.WriteAttribute(HtmlNameIndex.Dir, distinctProperty1.Bool ? "rtl" : "ltr");
      Format.PropertyValue distinctProperty2 = this.GetDistinctProperty(Format.PropertyId.TextAlignment);
      if (!distinctProperty2.IsNull)
      {
        string textAlignmentString = HtmlSupport.GetTextAlignmentString(distinctProperty2);
        if (textAlignmentString != null)
          this.Writer.WriteAttribute(HtmlNameIndex.Align, textAlignmentString);
      }
      this.WriteIdAttribute(false);
    }

    private void OutputTableTagAttributes()
    {
      Format.PropertyValue distinctProperty1 = this.GetDistinctProperty(Format.PropertyId.Width);
      if (!distinctProperty1.IsNull)
      {
        BufferString bufferString = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, distinctProperty1);
        if (bufferString.Length != 0)
          this.Writer.WriteAttribute(HtmlNameIndex.Width, bufferString);
      }
      Format.PropertyValue distinctProperty2 = this.GetDistinctProperty(Format.PropertyId.BlockAlignment);
      if (!distinctProperty2.IsNull)
      {
        string horizontalAlignmentString = HtmlSupport.GetHorizontalAlignmentString(distinctProperty2);
        if (horizontalAlignmentString != null)
          this.Writer.WriteAttribute(HtmlNameIndex.Align, horizontalAlignmentString);
      }
      Format.PropertyValue distinctProperty3 = this.GetDistinctProperty(Format.PropertyId.RightToLeft);
      if (!distinctProperty3.IsNull)
        this.Writer.WriteAttribute(HtmlNameIndex.Dir, distinctProperty3.Bool ? "rtl" : "ltr");
      Format.PropertyValue distinctProperty4 = this.GetDistinctProperty(Format.PropertyId.TableBorder);
      if (!distinctProperty4.IsNull)
      {
        BufferString bufferString = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, distinctProperty4);
        if (bufferString.Length != 0)
          this.Writer.WriteAttribute(HtmlNameIndex.Border, bufferString);
      }
      Format.PropertyValue distinctProperty5 = this.GetDistinctProperty(Format.PropertyId.TableFrame);
      if (!distinctProperty5.IsNull)
      {
        string tableFrameString = HtmlSupport.GetTableFrameString(distinctProperty5);
        if (tableFrameString != null)
          this.Writer.WriteAttribute(HtmlNameIndex.Frame, tableFrameString);
      }
      Format.PropertyValue distinctProperty6 = this.GetDistinctProperty(Format.PropertyId.TableRules);
      if (!distinctProperty6.IsNull)
      {
        string tableRulesString = HtmlSupport.GetTableRulesString(distinctProperty6);
        if (tableRulesString != null)
          this.Writer.WriteAttribute(HtmlNameIndex.Rules, tableRulesString);
      }
      Format.PropertyValue distinctProperty7 = this.GetDistinctProperty(Format.PropertyId.TableCellSpacing);
      if (!distinctProperty7.IsNull)
      {
        BufferString bufferString = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, distinctProperty7);
        if (bufferString.Length != 0)
          this.Writer.WriteAttribute(HtmlNameIndex.CellSpacing, bufferString);
      }
      Format.PropertyValue distinctProperty8 = this.GetDistinctProperty(Format.PropertyId.TableCellPadding);
      if (distinctProperty8.IsNull)
        return;
      BufferString bufferString1 = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, distinctProperty8);
      if (bufferString1.Length == 0)
        return;
      this.Writer.WriteAttribute(HtmlNameIndex.CellPadding, bufferString1);
    }

    private void OutputTableCellTagAttributes()
    {
      Format.PropertyValue distinctProperty1 = this.GetDistinctProperty(Format.PropertyId.NumColumns);
      if (distinctProperty1.IsInteger && distinctProperty1.Integer != 1)
        this.Writer.WriteAttribute(HtmlNameIndex.ColSpan, distinctProperty1.Integer.ToString());
      Format.PropertyValue distinctProperty2 = this.GetDistinctProperty(Format.PropertyId.NumRows);
      if (distinctProperty2.IsInteger && distinctProperty2.Integer != 1)
        this.Writer.WriteAttribute(HtmlNameIndex.RowSpan, distinctProperty2.Integer.ToString());
      Format.PropertyValue distinctProperty3 = this.GetDistinctProperty(Format.PropertyId.Width);
      if (!distinctProperty3.IsNull && distinctProperty3.IsAbsRelLength)
        this.Writer.WriteAttribute(HtmlNameIndex.Width, distinctProperty3.PixelsInteger.ToString());
      Format.PropertyValue distinctProperty4 = this.GetDistinctProperty(Format.PropertyId.TextAlignment);
      if (!distinctProperty4.IsNull)
      {
        string textAlignmentString = HtmlSupport.GetTextAlignmentString(distinctProperty4);
        if (textAlignmentString != null)
          this.Writer.WriteAttribute(HtmlNameIndex.Align, textAlignmentString);
      }
      Format.PropertyValue distinctProperty5 = this.GetDistinctProperty(Format.PropertyId.BlockAlignment);
      if (!distinctProperty5.IsNull)
      {
        string verticalAlignmentString = HtmlSupport.GetVerticalAlignmentString(distinctProperty5);
        if (verticalAlignmentString != null)
          this.Writer.WriteAttribute(HtmlNameIndex.Valign, verticalAlignmentString);
      }
      Format.PropertyValue distinctProperty6 = this.GetDistinctProperty(Format.PropertyId.TableCellNoWrap);
      if (distinctProperty6.IsNull || !distinctProperty6.Bool)
        return;
      this.Writer.WriteAttribute(HtmlNameIndex.NoWrap, string.Empty);
    }

    private bool RecognizeHyperLink(Format.TextRun run, out int offset, out int length, out bool addFilePrefix, out bool addHttpPrefix)
    {
      this.scratchBuffer.Reset();
      int start = run.AppendFragment(0, ref this.scratchBuffer, 30);
      offset = 0;
      length = 0;
      bool flag1 = false;
      while (offset < Math.Min(start - 10, 20))
      {
        if (HtmlFormatOutput.IsHyperLinkStartDelimiter(this.scratchBuffer[offset]))
        {
          flag1 = true;
          break;
        }
        ++offset;
      }
      if (!flag1)
        offset = 0;
      bool flag2 = false;
      while (offset < start - 4 && HtmlFormatOutput.IsHyperLinkStartDelimiter(this.scratchBuffer[offset]))
      {
        flag2 = true;
        ++offset;
      }
      bool flag3 = false;
      addHttpPrefix = false;
      addFilePrefix = false;
      if ((int) this.scratchBuffer[offset] == 92)
      {
        if ((int) this.scratchBuffer[offset + 1] == 92 && char.IsLetterOrDigit(this.scratchBuffer[offset + 2]))
        {
          flag3 = true;
          addFilePrefix = true;
        }
      }
      else if (start - offset > 4 && (int) this.scratchBuffer[offset] == 104)
      {
        if ((int) this.scratchBuffer[offset + 1] == 116 && (int) this.scratchBuffer[offset + 2] == 116 && (int) this.scratchBuffer[offset + 3] == 112 && ((int) this.scratchBuffer[offset + 4] == 58 || start - offset > 5 && (int) this.scratchBuffer[offset + 4] == 115 && (int) this.scratchBuffer[offset + 5] == 58))
          flag3 = true;
      }
      else if (start - offset > 3 && (int) this.scratchBuffer[offset] == 102)
      {
        if ((int) this.scratchBuffer[offset + 1] == 116 && (int) this.scratchBuffer[offset + 2] == 112 && (int) this.scratchBuffer[offset + 3] == 58)
          flag3 = true;
        else if (start - offset > 6 && (int) this.scratchBuffer[offset + 1] == 105 && ((int) this.scratchBuffer[offset + 2] == 108 && (int) this.scratchBuffer[offset + 3] == 101) && ((int) this.scratchBuffer[offset + 4] == 58 && (int) this.scratchBuffer[offset + 5] == 47 && (int) this.scratchBuffer[offset + 6] == 47))
          flag3 = true;
      }
      else if (start - offset > 6 && (int) this.scratchBuffer[offset] == 109)
      {
        if ((int) this.scratchBuffer[offset + 1] == 97 && (int) this.scratchBuffer[offset + 2] == 105 && ((int) this.scratchBuffer[offset + 3] == 108 && (int) this.scratchBuffer[offset + 4] == 116) && ((int) this.scratchBuffer[offset + 5] == 111 && (int) this.scratchBuffer[offset + 6] == 58))
          flag3 = true;
      }
      else if (start - offset > 3 && (int) this.scratchBuffer[offset] == 119)
      {
        if ((int) this.scratchBuffer[offset + 1] == 119 && (int) this.scratchBuffer[offset + 2] == 119 && (int) this.scratchBuffer[offset + 3] == 46)
        {
          flag3 = true;
          addHttpPrefix = true;
        }
      }
      else if (start - offset > 7 && (int) this.scratchBuffer[offset] == 110 && ((int) this.scratchBuffer[offset + 1] == 111 && (int) this.scratchBuffer[offset + 2] == 116) && ((int) this.scratchBuffer[offset + 3] == 101 && (int) this.scratchBuffer[offset + 4] == 115 && ((int) this.scratchBuffer[offset + 5] == 58 && (int) this.scratchBuffer[offset + 6] == 47)) && (int) this.scratchBuffer[offset + 7] == 47)
        flag3 = true;
      if (flag3)
      {
        int num = start + run.AppendFragment(start, ref this.scratchBuffer, 4096 - start);
        if (flag2)
        {
          while (num > offset && !HtmlFormatOutput.IsHyperLinkEndDelimiter(this.scratchBuffer[num - 1]))
            --num;
          while (num > offset && HtmlFormatOutput.IsHyperLinkEndDelimiter(this.scratchBuffer[num - 1]))
            --num;
        }
        else
        {
          while (HtmlFormatOutput.IsHyperLinkEndDelimiter(this.scratchBuffer[num - 1]) || (int) this.scratchBuffer[num - 1] == 46 || ((int) this.scratchBuffer[num - 1] == 44 || (int) this.scratchBuffer[num - 1] == 59))
            --num;
        }
        length = num - offset;
      }
      return flag3;
    }

    private struct EndTagActionEntry
    {
      public int TagLevel;
      public bool Drop;
      public bool Callback;
    }
  }
}
