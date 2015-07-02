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
    private HtmlWriter writer;
    private HtmlInjection injection;
    private bool filterHtml;
    private HtmlTagCallback callback;
    private HtmlFormatOutputCallbackContext callbackContext;
    private bool outputFragment;
    private bool recognizeHyperlinks;
    private int hyperlinkLevel;
    private HtmlFormatOutput.EndTagActionEntry[] endTagActionStack;
    private int endTagActionStackTop;

    internal HtmlWriter Writer
    {
      get
      {
        return this.writer;
      }
      set
      {
        this.writer = value;
      }
    }

    public override bool OutputCodePageSameAsInput
    {
      get
      {
        return false;
      }
    }

    public override Encoding OutputEncoding
    {
      set
      {
        throw new InvalidOperationException();
      }
    }

    public override bool CanAcceptMoreOutput
    {
      get
      {
        return this.writer.CanAcceptMore;
      }
    }

    public HtmlFormatOutput(HtmlWriter writer, HtmlInjection injection, bool outputFragment, Stream formatTraceStream, Stream formatOutputTraceStream, bool filterHtml, HtmlTagCallback callback, bool recognizeHyperlinks)
      : base(formatOutputTraceStream)
    {
      this.writer = writer;
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
        this.writer.WriteAttributeName(HtmlNameIndex.Id);
        this.writer.WriteAttributeValue(@string);
      }
    }

    bool IRestartable.CanRestart()
    {
      if (this.writer != null)
        return ((IRestartable) this.writer).CanRestart();
      return false;
    }

    void IRestartable.Restart()
    {
      ((IRestartable) this.writer).Restart();
      this.Restart();
      if (this.injection != null)
        this.injection.Reset();
      this.hyperlinkLevel = 0;
    }

    void IRestartable.DisableRestart()
    {
      if (this.writer == null)
        return;
      ((IRestartable) this.writer).DisableRestart();
    }

    public override bool Flush()
    {
      if (!base.Flush())
        return false;
      this.writer.Flush();
      return true;
    }

    internal void SetWriter(HtmlWriter writer)
    {
      this.writer = writer;
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
        this.writer.WriteStartTag(HtmlNameIndex.Html);
        if (this.callback != null)
        {
          if (this.callbackContext == null)
            this.callbackContext = new HtmlFormatOutputCallbackContext(this);
          this.callbackContext.InitializeTag(false, HtmlNameIndex.Head, false);
        }
        else
          this.writer.WriteStartTag(HtmlNameIndex.Head);
        if (this.callback != null)
        {
          this.callbackContext.InitializeFragment(false);
          this.callback((HtmlTagContext) this.callbackContext, this.writer);
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
          if (this.writer.HasEncoding)
          {
            this.writer.WriteStartTag(HtmlNameIndex.Meta);
            this.writer.WriteAttribute(HtmlNameIndex.HttpEquiv, "Content-Type");
            this.writer.WriteAttributeName(HtmlNameIndex.Content);
            this.writer.WriteAttributeValueInternal("text/html; charset=");
            this.writer.WriteAttributeValue(Globalization.Charset.GetCharset(this.writer.Encoding).Name);
            this.writer.WriteNewLine(true);
          }
          this.writer.WriteStartTag(HtmlNameIndex.Meta);
          this.writer.WriteAttribute(HtmlNameIndex.Name, "Generator");
          this.writer.WriteAttribute(HtmlNameIndex.Content, "Microsoft Exchange Server");
          this.writer.WriteNewLine(true);
          if (this.Comment != null)
          {
            this.writer.WriteMarkupText("<!-- " + this.Comment + " -->");
            this.writer.WriteNewLine(true);
          }
          this.writer.WriteStartTag(HtmlNameIndex.Style);
          this.writer.WriteMarkupText("<!-- .EmailQuote { margin-left: 1pt; padding-left: 4pt; border-left: #800000 2px solid; } -->");
          this.writer.WriteEndTag(HtmlNameIndex.Style);
        }
        if (flag1)
        {
          this.callbackContext.InitializeTag(true, HtmlNameIndex.Head, tagDropped);
          this.callbackContext.InitializeFragment(false);
          this.callback((HtmlTagContext) this.callbackContext, this.writer);
          this.callbackContext.UninitializeFragment();
        }
        else if (!tagDropped)
        {
          this.writer.WriteEndTag(HtmlNameIndex.Head);
          this.writer.WriteNewLine(true);
        }
        this.writer.WriteStartTag(HtmlNameIndex.Body);
        this.writer.WriteNewLine(true);
      }
      else
      {
        this.writer.WriteStartTag(HtmlNameIndex.Div);
        this.writer.WriteAttribute(HtmlNameIndex.Class, "BodyFragment");
        this.writer.WriteNewLine(true);
      }
      if (this.injection != null && this.injection.HaveHead)
        this.injection.Inject(true, this.writer);
      this.ApplyCharFormat();
      return true;
    }

    protected override void EndDocument()
    {
      this.RevertCharFormat();
      if (this.injection != null && this.injection.HaveTail)
        this.injection.Inject(false, this.writer);
      if (!this.outputFragment)
      {
        this.writer.WriteNewLine(true);
        this.writer.WriteEndTag(HtmlNameIndex.Body);
        this.writer.WriteNewLine(true);
        this.writer.WriteEndTag(HtmlNameIndex.Html);
      }
      else
      {
        this.writer.WriteNewLine(true);
        this.writer.WriteEndTag(HtmlNameIndex.Div);
      }
      this.writer.WriteNewLine(true);
    }

    protected override void StartEndBaseFont()
    {
    }

    protected override bool StartTable()
    {
      Format.PropertyValue distinctProperty = this.GetDistinctProperty(Format.PropertyId.FontFace);
      if (!distinctProperty.IsNull)
      {
        this.writer.WriteStartTag(HtmlNameIndex.Font);
        this.writer.WriteAttributeName(HtmlNameIndex.Face);
        if (distinctProperty.IsMultiValue)
        {
          Format.MultiValue multiValue = this.FormatStore.GetMultiValue(distinctProperty);
          for (int index = 0; index < multiValue.Length; ++index)
          {
            string @string = multiValue.GetStringValue(index).GetString();
            if (index != 0)
              this.writer.WriteAttributeValue(",");
            this.writer.WriteAttributeValue(@string);
          }
        }
        else
          this.writer.WriteAttributeValue(this.FormatStore.GetStringValue(distinctProperty).GetString());
      }
      this.writer.WriteNewLine(true);
      this.writer.WriteStartTag(HtmlNameIndex.Table);
      this.OutputTableTagAttributes();
      bool styleAttributeOpen = false;
      this.OutputTableCssProperties(ref styleAttributeOpen);
      this.OutputBlockCssProperties(ref styleAttributeOpen);
      this.writer.WriteNewLine(true);
      return true;
    }

    protected override void EndTable()
    {
      this.writer.WriteNewLine(true);
      this.writer.WriteEndTag(HtmlNameIndex.Table);
      this.writer.WriteNewLine(true);
      if (this.GetDistinctProperty(Format.PropertyId.FontFace).IsNull)
        return;
      this.writer.WriteEndTag(HtmlNameIndex.Font);
    }

    protected override bool StartTableColumnGroup()
    {
      this.writer.WriteNewLine(true);
      this.writer.WriteStartTag(HtmlNameIndex.ColGroup);
      Format.PropertyValue distinctProperty1 = this.GetDistinctProperty(Format.PropertyId.Width);
      if (!distinctProperty1.IsNull && distinctProperty1.IsAbsRelLength)
        this.writer.WriteAttribute(HtmlNameIndex.Width, distinctProperty1.PixelsInteger.ToString());
      Format.PropertyValue distinctProperty2 = this.GetDistinctProperty(Format.PropertyId.NumColumns);
      if (!distinctProperty2.IsNull && distinctProperty2.IsAbsRelLength)
        this.writer.WriteAttribute(HtmlNameIndex.Span, distinctProperty2.Integer.ToString());
      bool styleAttributeOpen = false;
      this.OutputTableColumnCssProperties(ref styleAttributeOpen);
      return true;
    }

    protected override void EndTableColumnGroup()
    {
      this.writer.WriteEndTag(HtmlNameIndex.ColGroup);
      this.writer.WriteNewLine(true);
    }

    protected override void StartEndTableColumn()
    {
      this.writer.WriteStartTag(HtmlNameIndex.Col);
      Format.PropertyValue distinctProperty1 = this.GetDistinctProperty(Format.PropertyId.Width);
      if (!distinctProperty1.IsNull && distinctProperty1.IsAbsRelLength)
        this.writer.WriteAttribute(HtmlNameIndex.Width, distinctProperty1.PixelsInteger.ToString());
      Format.PropertyValue distinctProperty2 = this.GetDistinctProperty(Format.PropertyId.NumColumns);
      if (!distinctProperty2.IsNull && distinctProperty2.IsAbsRelLength)
        this.writer.WriteAttribute(HtmlNameIndex.Span, distinctProperty2.Integer.ToString());
      bool styleAttributeOpen = false;
      this.OutputTableColumnCssProperties(ref styleAttributeOpen);
      this.writer.WriteNewLine(true);
    }

    protected override bool StartTableCaption()
    {
      this.writer.WriteNewLine(true);
      if (!this.CurrentNode.Parent.IsNull && this.CurrentNode.Parent.NodeType == Format.FormatContainerType.Table)
      {
        this.writer.WriteStartTag(HtmlNameIndex.Caption);
        Format.FormatStyle style = this.FormatStore.GetStyle(13);
        this.SubtractDefaultContainerPropertiesFromDistinct(style.FlagProperties, style.PropertyList);
        Format.PropertyValue distinctProperty = this.GetDistinctProperty(Format.PropertyId.BlockAlignment);
        if (!distinctProperty.IsNull)
        {
          string blockAlignmentString = HtmlSupport.GetBlockAlignmentString(distinctProperty);
          if (blockAlignmentString != null)
            this.writer.WriteAttribute(HtmlNameIndex.Align, blockAlignmentString);
        }
        this.writer.WriteNewLine(true);
      }
      this.ApplyCharFormat();
      return true;
    }

    protected override void EndTableCaption()
    {
      this.RevertCharFormat();
      if (!this.CurrentNode.Parent.IsNull && this.CurrentNode.Parent.NodeType == Format.FormatContainerType.Table)
      {
        this.writer.WriteNewLine(true);
        this.writer.WriteEndTag(HtmlNameIndex.Caption);
      }
      this.writer.WriteNewLine(true);
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
      this.writer.WriteNewLine(true);
      this.writer.WriteStartTag(HtmlNameIndex.TR);
      Format.PropertyValue distinctProperty = this.GetDistinctProperty(Format.PropertyId.Height);
      if (!distinctProperty.IsNull && distinctProperty.IsAbsRelLength)
        this.writer.WriteAttribute(HtmlNameIndex.Height, distinctProperty.PixelsInteger.ToString());
      bool styleAttributeOpen = false;
      this.OutputBlockCssProperties(ref styleAttributeOpen);
      this.writer.WriteNewLine(true);
      return true;
    }

    protected override void EndTableRow()
    {
      this.writer.WriteNewLine(true);
      this.writer.WriteEndTag(HtmlNameIndex.TR);
      this.writer.WriteNewLine(true);
    }

    protected override bool StartTableCell()
    {
      Format.PropertyValue distinctProperty = this.GetDistinctProperty(Format.PropertyId.MergedCell);
      if (distinctProperty.IsNull || !distinctProperty.Bool)
      {
        this.writer.WriteNewLine(true);
        this.writer.WriteStartTag(HtmlNameIndex.TD);
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
      this.writer.WriteEndTag(HtmlNameIndex.TD);
      this.writer.WriteNewLine(true);
    }

    protected override bool StartList()
    {
      this.writer.WriteNewLine(true);
      Format.PropertyValue effectiveProperty = this.GetEffectiveProperty(Format.PropertyId.ListStyle);
      bool flag = true;
      if (effectiveProperty.IsNull || effectiveProperty.Enum == 1)
      {
        this.writer.WriteStartTag(HtmlNameIndex.UL);
      }
      else
      {
        this.writer.WriteStartTag(HtmlNameIndex.OL);
        flag = false;
      }
      Format.PropertyValue distinctProperty1 = this.GetDistinctProperty(Format.PropertyId.RightToLeft);
      if (!distinctProperty1.IsNull)
        this.writer.WriteAttribute(HtmlNameIndex.Dir, distinctProperty1.Bool ? "rtl" : "ltr");
      if (!flag && effectiveProperty.Enum != 2)
        this.writer.WriteAttribute(HtmlNameIndex.Type, HtmlFormatOutput.listType[effectiveProperty.Enum]);
      Format.PropertyValue distinctProperty2 = this.GetDistinctProperty(Format.PropertyId.ListStart);
      if (!flag && distinctProperty2.IsInteger && distinctProperty2.Integer != 1)
        this.writer.WriteAttribute(HtmlNameIndex.Start, distinctProperty2.Integer.ToString());
      bool styleAttributeOpen = false;
      this.OutputBlockCssProperties(ref styleAttributeOpen);
      this.writer.WriteNewLine(true);
      this.ApplyCharFormat();
      return true;
    }

    protected override void EndList()
    {
      this.RevertCharFormat();
      Format.PropertyValue effectiveProperty = this.GetEffectiveProperty(Format.PropertyId.ListStyle);
      this.writer.WriteNewLine(true);
      if (effectiveProperty.IsNull || effectiveProperty.Enum == 1)
        this.writer.WriteEndTag(HtmlNameIndex.UL);
      else
        this.writer.WriteEndTag(HtmlNameIndex.OL);
      this.writer.WriteNewLine(true);
    }

    protected override bool StartListItem()
    {
      this.writer.WriteNewLine(true);
      this.writer.WriteStartTag(HtmlNameIndex.LI);
      bool styleAttributeOpen = false;
      this.OutputBlockCssProperties(ref styleAttributeOpen);
      this.ApplyCharFormat();
      return true;
    }

    protected override void EndListItem()
    {
      this.RevertCharFormat();
      this.writer.WriteEndTag(HtmlNameIndex.LI);
      this.writer.WriteNewLine(true);
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
        this.writer.WriteStartTag(HtmlNameIndex.A);
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
          this.writer.WriteAttributeName(HtmlNameIndex.Href);
          this.writer.WriteAttributeValue(url);
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
            this.writer.WriteAttributeName(HtmlNameIndex.Target);
            this.writer.WriteAttributeValue(targetString);
          }
        }
        this.WriteIdAttribute(this.callback != null);
      }
      if (this.callback != null)
      {
        this.callbackContext.InitializeFragment(false);
        this.callback((HtmlTagContext) this.callbackContext, this.writer);
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
      if (this.writer.IsTagOpen)
        this.writer.WriteTagEnd();
      return !flag2;
    }

    protected override void EndHyperLink()
    {
      --this.hyperlinkLevel;
      this.RevertCharFormat();
      this.CloseHyperLink();
      if (!this.writer.IsTagOpen)
        return;
      this.writer.WriteTagEnd();
    }

    protected override bool StartBookmark()
    {
      Format.PropertyValue distinctProperty = this.GetDistinctProperty(Format.PropertyId.BookmarkName);
      if (!distinctProperty.IsNull)
      {
        this.writer.WriteStartTag(HtmlNameIndex.A);
        string @string = this.FormatStore.GetStringValue(distinctProperty).GetString();
        this.writer.WriteAttributeName(HtmlNameIndex.Name);
        this.writer.WriteAttributeValue(@string);
      }
      this.ApplyCharFormat();
      if (this.writer.IsTagOpen)
        this.writer.WriteTagEnd();
      return true;
    }

    protected override void EndBookmark()
    {
      this.RevertCharFormat();
      if (!this.GetDistinctProperty(Format.PropertyId.BookmarkName).IsNull)
        this.writer.WriteEndTag(HtmlNameIndex.A);
      if (!this.writer.IsTagOpen)
        return;
      this.writer.WriteTagEnd();
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
        this.writer.WriteStartTag(HtmlNameIndex.Img);
      Format.PropertyValue distinctProperty1 = this.GetDistinctProperty(Format.PropertyId.Width);
      if (!distinctProperty1.IsNull)
      {
        BufferString bufferString = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, distinctProperty1);
        if (bufferString.Length != 0)
        {
          if (this.callback != null)
            this.callbackContext.AddAttribute(HtmlNameIndex.Width, bufferString.ToString());
          else
            this.writer.WriteAttribute(HtmlNameIndex.Width, bufferString);
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
            this.writer.WriteAttribute(HtmlNameIndex.Height, bufferString);
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
            this.writer.WriteAttribute(HtmlNameIndex.Align, blockAlignmentString);
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
            this.writer.WriteAttribute(HtmlNameIndex.Border, bufferString);
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
          this.writer.WriteAttributeName(HtmlNameIndex.Src);
          this.writer.WriteAttributeValue(url);
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
          this.writer.WriteAttributeName(HtmlNameIndex.Dir);
          this.writer.WriteAttributeValue(distinctProperty6.Bool ? "rtl" : "ltr");
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
          this.writer.WriteAttributeName(HtmlNameIndex.Lang);
          this.writer.WriteAttributeValue(culture.Name);
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
          this.writer.WriteAttributeName(HtmlNameIndex.Alt);
          this.writer.WriteAttributeValue(@string);
        }
      }
      if (this.callback != null)
      {
        this.callbackContext.InitializeFragment(true);
        this.callback((HtmlTagContext) this.callbackContext, this.writer);
        this.callbackContext.UninitializeFragment();
      }
      if (!this.writer.IsTagOpen)
        return;
      this.writer.WriteTagEnd();
    }

    protected override void StartEndHorizontalLine()
    {
      this.writer.WriteNewLine(true);
      this.writer.WriteStartTag(HtmlNameIndex.HR);
      Format.PropertyValue distinctProperty1 = this.GetDistinctProperty(Format.PropertyId.Width);
      if (!distinctProperty1.IsNull)
      {
        BufferString bufferString = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, distinctProperty1);
        if (bufferString.Length != 0)
          this.writer.WriteAttribute(HtmlNameIndex.Width, bufferString);
      }
      Format.PropertyValue distinctProperty2 = this.GetDistinctProperty(Format.PropertyId.Height);
      if (!distinctProperty2.IsNull && distinctProperty2.IsAbsRelLength)
        this.writer.WriteAttribute(HtmlNameIndex.Size, distinctProperty2.PixelsInteger.ToString());
      Format.PropertyValue distinctProperty3 = this.GetDistinctProperty(Format.PropertyId.BlockAlignment);
      if (!distinctProperty3.IsNull)
      {
        string horizontalAlignmentString = HtmlSupport.GetHorizontalAlignmentString(distinctProperty3);
        if (horizontalAlignmentString != null)
          this.writer.WriteAttribute(HtmlNameIndex.Align, horizontalAlignmentString);
      }
      Format.PropertyValue distinctProperty4 = this.GetDistinctProperty(Format.PropertyId.FontColor);
      if (!distinctProperty4.IsNull)
      {
        BufferString bufferString = HtmlSupport.FormatColor(ref this.scratchBuffer, distinctProperty4);
        if (bufferString.Length != 0)
          this.writer.WriteAttribute(HtmlNameIndex.Color, bufferString);
      }
      if (!distinctProperty1.IsNull)
      {
        this.writer.WriteAttributeName(HtmlNameIndex.Style);
        if (!distinctProperty1.IsNull)
        {
          BufferString bufferString = HtmlSupport.FormatLength(ref this.scratchBuffer, distinctProperty1);
          if (bufferString.Length != 0)
          {
            this.writer.WriteAttributeValue("width:");
            this.writer.WriteAttributeValue(bufferString);
            this.writer.WriteAttributeValue(";");
          }
        }
      }
      if (this.writer.LiteralWhitespaceNesting != 0)
        return;
      this.writer.WriteNewLine(true);
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
      this.writer.StartTextChunk();
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
                this.writer.WriteNbsp(effectiveLength);
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
                  this.writer.WriteTextInternal(this.scratchBuffer.Buffer, 0, offset1);
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
                  this.callback((HtmlTagContext) this.callbackContext, this.writer);
                  this.callbackContext.UninitializeFragment();
                  if (this.writer.IsTagOpen)
                    this.writer.WriteTagEnd();
                  if (!this.callbackContext.IsDeleteInnerContent)
                    this.writer.WriteTextInternal(this.scratchBuffer.Buffer, offset1, length);
                  if (this.callbackContext.IsInvokeCallbackForEndTag)
                  {
                    this.callbackContext.InitializeTag(true, HtmlNameIndex.A, this.callbackContext.IsDeleteEndTag);
                    this.callbackContext.InitializeFragment(false);
                    this.callback((HtmlTagContext) this.callbackContext, this.writer);
                    this.callbackContext.UninitializeFragment();
                  }
                  else if (!this.callbackContext.IsDeleteEndTag)
                    this.writer.WriteEndTag(HtmlNameIndex.A);
                  if (this.writer.IsTagOpen)
                    this.writer.WriteTagEnd();
                }
                else
                {
                  this.writer.WriteStartTag(HtmlNameIndex.A);
                  this.writer.WriteAttributeName(HtmlNameIndex.Href);
                  if (addHttpPrefix)
                    this.writer.WriteAttributeValue("http://");
                  else if (addFilePrefix)
                    this.writer.WriteAttributeValue("file://");
                  this.writer.WriteAttributeValue(this.scratchBuffer.Buffer, offset1, length);
                  this.writer.WriteTagEnd();
                  this.writer.WriteTextInternal(this.scratchBuffer.Buffer, offset1, length);
                  this.writer.WriteEndTag(HtmlNameIndex.A);
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
                this.writer.WriteTextInternal(buffer, offset2, count);
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
                  if (this.writer.LiteralWhitespaceNesting == 0)
                    this.writer.WriteStartTag(HtmlNameIndex.BR);
                  this.writer.WriteNewLine(false);
                }
              }
            }
            else
              this.writer.WriteTabulation(effectiveLength);
          }
          else
            this.writer.WriteSpace(effectiveLength);
          textRun.MoveNext();
label_44:;
        }
        while (textRun.Position < endTextPosition);
      }
      return true;
    }

    protected override void EndText()
    {
      this.writer.EndTextChunk();
      this.RevertCharFormat();
    }

    protected override bool StartBlockContainer()
    {
      this.writer.WriteNewLine(true);
      Format.PropertyValue distinctProperty1 = this.GetDistinctProperty(Format.PropertyId.Preformatted);
      Format.PropertyValue distinctProperty2 = this.GetDistinctProperty(Format.PropertyId.QuotingLevelDelta);
      if (!distinctProperty1.IsNull && distinctProperty1.Bool)
      {
        Format.FormatStyle style = this.FormatStore.GetStyle(14);
        this.SubtractDefaultContainerPropertiesFromDistinct(Format.FlagProperties.AllOff, style.PropertyList);
        this.writer.WriteStartTag(HtmlNameIndex.Pre);
      }
      else if (!distinctProperty2.IsNull && distinctProperty2.Integer != 0)
      {
        for (int index = 0; index < distinctProperty2.Integer; ++index)
        {
          this.writer.WriteStartTag(HtmlNameIndex.Div);
          this.writer.WriteAttribute(HtmlNameIndex.Class, "EmailQuote");
        }
      }
      else
      {
        if (this.SourceFormat == Format.SourceFormat.Text)
          this.ApplyCharFormat();
        this.writer.WriteStartTag(HtmlNameIndex.Div);
        if (this.SourceFormat == Format.SourceFormat.Text)
          this.writer.WriteAttribute(HtmlNameIndex.Class, "PlainText");
      }
      this.OutputBlockTagAttributes();
      bool styleAttributeOpen = false;
      this.OutputBlockCssProperties(ref styleAttributeOpen);
      if (this.SourceFormat != Format.SourceFormat.Text)
        this.ApplyCharFormat();
      if (this.CurrentNode.FirstChild.IsNull)
        this.writer.WriteText(' ');
      else if (this.CurrentNode.FirstChild == this.CurrentNode.LastChild && this.CurrentNode.FirstChild.NodeType == Format.FormatContainerType.Text)
      {
        Format.FormatNode firstChild = this.CurrentNode.FirstChild;
        if ((int) firstChild.BeginTextPosition + 1 == (int) firstChild.EndTextPosition && this.FormatStore.GetTextRun(firstChild.BeginTextPosition).Type == Format.TextRunType.Space)
        {
          this.writer.WriteText(' ');
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
        this.writer.WriteEndTag(HtmlNameIndex.Pre);
      else if (!distinctProperty2.IsNull && distinctProperty2.Integer != 0)
      {
        for (int index = 0; index < distinctProperty2.Integer; ++index)
          this.writer.WriteEndTag(HtmlNameIndex.Div);
      }
      else
      {
        this.writer.WriteEndTag(HtmlNameIndex.Div);
        if (this.SourceFormat == Format.SourceFormat.Text)
          this.RevertCharFormat();
      }
      this.writer.WriteNewLine(true);
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
      if (this.writer != null && this.writer != null)
        this.writer.Dispose();
      this.writer = (HtmlWriter) null;
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
        this.callback((HtmlTagContext) this.callbackContext, this.writer);
        this.callbackContext.UninitializeFragment();
      }
      else
      {
        if (tagDropped)
          return;
        this.writer.WriteEndTag(HtmlNameIndex.A);
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
        this.writer.WriteStartTag(HtmlNameIndex.Font);
        if (!distinctProperty6.IsNull)
        {
          this.writer.WriteAttributeName(HtmlNameIndex.Face);
          if (distinctProperty6.IsMultiValue)
          {
            Format.MultiValue multiValue = this.FormatStore.GetMultiValue(distinctProperty6);
            for (int index = 0; index < multiValue.Length; ++index)
            {
              string @string = multiValue.GetStringValue(index).GetString();
              if (index != 0)
                this.writer.WriteAttributeValue(",");
              this.writer.WriteAttributeValue(@string);
            }
          }
          else
            this.writer.WriteAttributeValue(this.FormatStore.GetStringValue(distinctProperty6).GetString());
        }
        BufferString bufferString;
        if (!distinctProperty1.IsNull)
        {
          bufferString = HtmlSupport.FormatFontSize(ref this.scratchValueBuffer, distinctProperty1);
          if (bufferString.Length != 0)
            this.writer.WriteAttribute(HtmlNameIndex.Size, bufferString);
        }
        if (!distinctProperty7.IsNull)
        {
          bufferString = HtmlSupport.FormatColor(ref this.scratchValueBuffer, distinctProperty7);
          if (bufferString.Length != 0)
            this.writer.WriteAttribute(HtmlNameIndex.Color, bufferString);
        }
      }
      if (this.scratchBuffer.Length != 0 || distinctFlags.IsDefined(Format.PropertyId.RightToLeft) || culture != null)
      {
        this.writer.WriteStartTag(HtmlNameIndex.Span);
        if (this.scratchBuffer.Length != 0)
        {
          this.writer.WriteAttributeName(HtmlNameIndex.Style);
          this.writer.WriteAttributeValue(this.scratchBuffer.BufferString);
        }
        if (distinctFlags.IsDefined(Format.PropertyId.RightToLeft))
        {
          this.writer.WriteAttributeName(HtmlNameIndex.Dir);
          this.writer.WriteAttributeValue(distinctFlags.IsOn(Format.PropertyId.RightToLeft) ? "rtl" : "ltr");
        }
        if (culture != null)
        {
          this.writer.WriteAttributeName(HtmlNameIndex.Lang);
          this.writer.WriteAttributeValue(culture.Name);
        }
      }
      if (distinctFlags.IsDefinedAndOn(Format.PropertyId.FirstFlag))
        this.writer.WriteStartTag(HtmlNameIndex.B);
      if (distinctFlags.IsDefinedAndOn(Format.PropertyId.Italic))
        this.writer.WriteStartTag(HtmlNameIndex.I);
      if (distinctFlags.IsDefinedAndOn(Format.PropertyId.Underline))
        this.writer.WriteStartTag(HtmlNameIndex.U);
      if (distinctFlags.IsDefinedAndOn(Format.PropertyId.Subscript))
        this.writer.WriteStartTag(HtmlNameIndex.Sub);
      if (distinctFlags.IsDefinedAndOn(Format.PropertyId.Superscript))
        this.writer.WriteStartTag(HtmlNameIndex.Sup);
      if (!distinctFlags.IsDefinedAndOn(Format.PropertyId.Strikethrough))
        return;
      this.writer.WriteStartTag(HtmlNameIndex.Strike);
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
        this.writer.WriteEndTag(HtmlNameIndex.Strike);
      if (distinctFlags.IsDefinedAndOn(Format.PropertyId.Superscript))
        this.writer.WriteEndTag(HtmlNameIndex.Sup);
      if (distinctFlags.IsDefinedAndOn(Format.PropertyId.Subscript))
        this.writer.WriteEndTag(HtmlNameIndex.Sub);
      if (distinctFlags.IsDefinedAndOn(Format.PropertyId.Underline))
        this.writer.WriteEndTag(HtmlNameIndex.U);
      if (distinctFlags.IsDefinedAndOn(Format.PropertyId.Italic))
        this.writer.WriteEndTag(HtmlNameIndex.I);
      if (distinctFlags.IsDefinedAndOn(Format.PropertyId.FirstFlag))
        this.writer.WriteEndTag(HtmlNameIndex.B);
      if (flag2)
        this.writer.WriteEndTag(HtmlNameIndex.Span);
      if (!flag1)
        return;
      this.writer.WriteEndTag(HtmlNameIndex.Font);
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
          this.writer.WriteAttributeName(HtmlNameIndex.Style);
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
            this.writer.WriteAttributeValue("width:");
            this.writer.WriteAttributeValue(bufferString);
            this.writer.WriteAttributeValue(";");
          }
        }
        if (!distinctProperty3.IsNull)
        {
          BufferString bufferString = HtmlSupport.FormatLength(ref this.scratchBuffer, distinctProperty3);
          if (bufferString.Length != 0)
          {
            this.writer.WriteAttributeValue("height:");
            this.writer.WriteAttributeValue(bufferString);
            this.writer.WriteAttributeValue(";");
          }
        }
        if (!distinctProperty5.IsNull)
        {
          string unicodeBiDiString = HtmlSupport.GetUnicodeBiDiString(distinctProperty5);
          if (unicodeBiDiString != null)
          {
            this.writer.WriteAttributeValue("unicode-bidi:");
            this.writer.WriteAttributeValue(unicodeBiDiString);
            this.writer.WriteAttributeValue(";");
          }
        }
      }
      if (!distinctProperty6.IsNull || !distinctProperty7.IsNull || !distinctProperty8.IsNull)
      {
        if (!styleAttributeOpen)
        {
          this.writer.WriteAttributeName(HtmlNameIndex.Style);
          styleAttributeOpen = true;
        }
        if (!distinctProperty6.IsNull)
        {
          BufferString bufferString = HtmlSupport.FormatLength(ref this.scratchBuffer, distinctProperty6);
          if (bufferString.Length != 0)
          {
            this.writer.WriteAttributeValue("text-indent:");
            this.writer.WriteAttributeValue(bufferString);
            this.writer.WriteAttributeValue(";");
          }
        }
        if (!distinctProperty7.IsNull && distinctProperty7.IsEnum && distinctProperty7.Enum < HtmlSupport.TextAlignmentEnumeration.Length)
        {
          this.writer.WriteAttributeValue("text-align:");
          this.writer.WriteAttributeValue(HtmlSupport.TextAlignmentEnumeration[distinctProperty7.Enum].Name);
          this.writer.WriteAttributeValue(";");
        }
        if (!distinctProperty8.IsNull)
        {
          BufferString bufferString = HtmlSupport.FormatColor(ref this.scratchBuffer, distinctProperty8);
          if (bufferString.Length != 0)
          {
            this.writer.WriteAttributeValue("background-color:");
            this.writer.WriteAttributeValue(bufferString);
            this.writer.WriteAttributeValue(";");
          }
        }
      }
      if (!distinctProperty9.IsNull || !distinctProperty10.IsNull || (!distinctProperty11.IsNull || !distinctProperty12.IsNull))
      {
        if (!styleAttributeOpen)
        {
          this.writer.WriteAttributeName(HtmlNameIndex.Style);
          styleAttributeOpen = true;
        }
        this.OutputMarginAndPaddingProperties("margin", distinctProperty9, distinctProperty10, distinctProperty11, distinctProperty12);
      }
      if (!distinctProperty13.IsNull || !distinctProperty14.IsNull || (!distinctProperty15.IsNull || !distinctProperty16.IsNull))
      {
        if (!styleAttributeOpen)
        {
          this.writer.WriteAttributeName(HtmlNameIndex.Style);
          styleAttributeOpen = true;
        }
        this.OutputMarginAndPaddingProperties("padding", distinctProperty13, distinctProperty14, distinctProperty15, distinctProperty16);
      }
      if (distinctProperty17.IsNull && distinctProperty18.IsNull && (distinctProperty19.IsNull && distinctProperty20.IsNull) && (distinctProperty21.IsNull && distinctProperty22.IsNull && (distinctProperty23.IsNull && distinctProperty24.IsNull)) && (distinctProperty25.IsNull && distinctProperty26.IsNull && (distinctProperty27.IsNull && distinctProperty28.IsNull)))
        return;
      if (!styleAttributeOpen)
      {
        this.writer.WriteAttributeName(HtmlNameIndex.Style);
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
        this.writer.WriteAttributeValue(name);
        this.writer.WriteAttributeValue(":");
        if (topValue == rightValue && topValue == bottomValue && topValue == leftValue)
          this.OutputLengthPropertyValue(topValue);
        else if (topValue == bottomValue && rightValue == leftValue)
          this.OutputCompositeLengthPropertyValue(topValue, rightValue);
        else
          this.OutputCompositeLengthPropertyValue(topValue, rightValue, bottomValue, leftValue);
        this.writer.WriteAttributeValue(";");
      }
      else
      {
        if (!topValue.IsNull)
        {
          this.writer.WriteAttributeValue(name);
          this.writer.WriteAttributeValue("-top:");
          this.OutputLengthPropertyValue(topValue);
          this.writer.WriteAttributeValue(";");
        }
        if (!rightValue.IsNull)
        {
          this.writer.WriteAttributeValue(name);
          this.writer.WriteAttributeValue("-right:");
          this.OutputLengthPropertyValue(rightValue);
          this.writer.WriteAttributeValue(";");
        }
        if (!bottomValue.IsNull)
        {
          this.writer.WriteAttributeValue(name);
          this.writer.WriteAttributeValue("-bottom:");
          this.OutputLengthPropertyValue(bottomValue);
          this.writer.WriteAttributeValue(";");
        }
        if (leftValue.IsNull)
          return;
        this.writer.WriteAttributeValue(name);
        this.writer.WriteAttributeValue("-left:");
        this.OutputLengthPropertyValue(leftValue);
        this.writer.WriteAttributeValue(";");
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
          this.writer.WriteAttributeValue("border:");
          this.OutputCompositeBorderSidePropertyValue(topBorderWidth, topBorderStyle, topBorderColor);
          this.writer.WriteAttributeValue(";");
        }
        else
        {
          this.writer.WriteAttributeValue("border-width:");
          if (flag1)
            this.OutputBorderWidthPropertyValue(topBorderWidth);
          else if (flag2)
            this.OutputCompositeBorderWidthPropertyValue(topBorderWidth, rightBorderWidth);
          else
            this.OutputCompositeBorderWidthPropertyValue(topBorderWidth, rightBorderWidth, bottomBorderWidth, leftBorderWidth);
          this.writer.WriteAttributeValue(";");
          this.writer.WriteAttributeValue("border-style:");
          if (flag3)
            this.OutputBorderStylePropertyValue(topBorderStyle);
          else if (flag4)
            this.OutputCompositeBorderStylePropertyValue(topBorderStyle, rightBorderStyle);
          else
            this.OutputCompositeBorderStylePropertyValue(topBorderStyle, rightBorderStyle, bottomBorderStyle, leftBorderStyle);
          this.writer.WriteAttributeValue(";");
          this.writer.WriteAttributeValue("border-color:");
          if (flag5)
            this.OutputBorderColorPropertyValue(topBorderColor);
          else if (flag6)
            this.OutputCompositeBorderColorPropertyValue(topBorderColor, rightBorderColor);
          else
            this.OutputCompositeBorderColorPropertyValue(topBorderColor, rightBorderColor, bottomBorderColor, leftBorderColor);
          this.writer.WriteAttributeValue(";");
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
            this.writer.WriteAttributeValue("border-width:");
            if (flag1)
              this.OutputBorderWidthPropertyValue(topBorderWidth);
            else if (flag2)
              this.OutputCompositeBorderWidthPropertyValue(topBorderWidth, rightBorderWidth);
            else
              this.OutputCompositeBorderWidthPropertyValue(topBorderWidth, rightBorderWidth, bottomBorderWidth, leftBorderWidth);
            this.writer.WriteAttributeValue(";");
            flag7 = true;
            flag8 = true;
            flag9 = true;
            flag10 = true;
          }
          if (num2 == 4)
          {
            this.writer.WriteAttributeValue("border-style:");
            if (flag3)
              this.OutputBorderStylePropertyValue(topBorderStyle);
            else if (flag4)
              this.OutputCompositeBorderStylePropertyValue(topBorderStyle, rightBorderStyle);
            else
              this.OutputCompositeBorderStylePropertyValue(topBorderStyle, rightBorderStyle, bottomBorderStyle, leftBorderStyle);
            this.writer.WriteAttributeValue(";");
            flag11 = true;
            flag12 = true;
            flag13 = true;
            flag14 = true;
          }
          if (num3 == 4)
          {
            this.writer.WriteAttributeValue("border-color:");
            if (flag5)
              this.OutputBorderColorPropertyValue(topBorderColor);
            else if (flag6)
              this.OutputCompositeBorderColorPropertyValue(topBorderColor, rightBorderColor);
            else
              this.OutputCompositeBorderColorPropertyValue(topBorderColor, rightBorderColor, bottomBorderColor, leftBorderColor);
            this.writer.WriteAttributeValue(";");
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
            this.writer.WriteAttributeValue("border-top:");
            this.OutputCompositeBorderSidePropertyValue(topBorderWidth, topBorderStyle, topBorderColor);
            this.writer.WriteAttributeValue(";");
            flag7 = true;
            flag11 = true;
            flag15 = true;
          }
          if (num5 == 3)
          {
            this.writer.WriteAttributeValue("border-right:");
            this.OutputCompositeBorderSidePropertyValue(rightBorderWidth, rightBorderStyle, rightBorderColor);
            this.writer.WriteAttributeValue(";");
            flag8 = true;
            flag12 = true;
            flag16 = true;
          }
          if (num6 == 3)
          {
            this.writer.WriteAttributeValue("border-bottom:");
            this.OutputCompositeBorderSidePropertyValue(bottomBorderWidth, bottomBorderStyle, bottomBorderColor);
            this.writer.WriteAttributeValue(";");
            flag9 = true;
            flag13 = true;
            flag17 = true;
          }
          if (num7 == 3)
          {
            this.writer.WriteAttributeValue("border-left:");
            this.OutputCompositeBorderSidePropertyValue(leftBorderWidth, leftBorderStyle, leftBorderColor);
            this.writer.WriteAttributeValue(";");
            flag10 = true;
            flag14 = true;
            flag18 = true;
          }
        }
        if (!flag7 && !topBorderWidth.IsNull)
        {
          this.writer.WriteAttributeValue("border-top-width:");
          this.OutputBorderWidthPropertyValue(topBorderWidth);
          this.writer.WriteAttributeValue(";");
        }
        if (!flag8 && !rightBorderWidth.IsNull)
        {
          this.writer.WriteAttributeValue("border-right-width:");
          this.OutputBorderWidthPropertyValue(rightBorderWidth);
          this.writer.WriteAttributeValue(";");
        }
        if (!flag9 && !bottomBorderWidth.IsNull)
        {
          this.writer.WriteAttributeValue("border-bottom-width:");
          this.OutputBorderWidthPropertyValue(bottomBorderWidth);
          this.writer.WriteAttributeValue(";");
        }
        if (!flag10 && !leftBorderWidth.IsNull)
        {
          this.writer.WriteAttributeValue("border-left-width:");
          this.OutputBorderWidthPropertyValue(leftBorderWidth);
          this.writer.WriteAttributeValue(";");
        }
        if (!flag11 && !topBorderStyle.IsNull)
        {
          this.writer.WriteAttributeValue("border-top-style:");
          this.OutputBorderStylePropertyValue(topBorderStyle);
          this.writer.WriteAttributeValue(";");
        }
        if (!flag12 && !rightBorderStyle.IsNull)
        {
          this.writer.WriteAttributeValue("border-right-style:");
          this.OutputBorderStylePropertyValue(rightBorderStyle);
          this.writer.WriteAttributeValue(";");
        }
        if (!flag13 && !bottomBorderStyle.IsNull)
        {
          this.writer.WriteAttributeValue("border-bottom-style:");
          this.OutputBorderStylePropertyValue(bottomBorderStyle);
          this.writer.WriteAttributeValue(";");
        }
        if (!flag14 && !leftBorderStyle.IsNull)
        {
          this.writer.WriteAttributeValue("border-left-style:");
          this.OutputBorderStylePropertyValue(leftBorderStyle);
          this.writer.WriteAttributeValue(";");
        }
        if (!flag15 && !topBorderColor.IsNull)
        {
          this.writer.WriteAttributeValue("border-top-color:");
          this.OutputBorderColorPropertyValue(topBorderColor);
          this.writer.WriteAttributeValue(";");
        }
        if (!flag16 && !rightBorderColor.IsNull)
        {
          this.writer.WriteAttributeValue("border-right-color:");
          this.OutputBorderColorPropertyValue(rightBorderColor);
          this.writer.WriteAttributeValue(";");
        }
        if (!flag17 && !bottomBorderColor.IsNull)
        {
          this.writer.WriteAttributeValue("border-bottom-color:");
          this.OutputBorderColorPropertyValue(bottomBorderColor);
          this.writer.WriteAttributeValue(";");
        }
        if (flag18 || leftBorderColor.IsNull)
          return;
        this.writer.WriteAttributeValue("border-left-color:");
        this.OutputBorderColorPropertyValue(leftBorderColor);
        this.writer.WriteAttributeValue(";");
      }
    }

    private void OutputCompositeBorderSidePropertyValue(Format.PropertyValue width, Format.PropertyValue style, Format.PropertyValue color)
    {
      this.OutputBorderWidthPropertyValue(width);
      this.writer.WriteAttributeValue(" ");
      this.OutputBorderStylePropertyValue(style);
      this.writer.WriteAttributeValue(" ");
      this.OutputBorderColorPropertyValue(color);
    }

    private void OutputCompositeLengthPropertyValue(Format.PropertyValue topBottom, Format.PropertyValue rightLeft)
    {
      this.OutputLengthPropertyValue(topBottom);
      this.writer.WriteAttributeValue(" ");
      this.OutputLengthPropertyValue(rightLeft);
    }

    private void OutputCompositeLengthPropertyValue(Format.PropertyValue top, Format.PropertyValue right, Format.PropertyValue bottom, Format.PropertyValue left)
    {
      this.OutputLengthPropertyValue(top);
      this.writer.WriteAttributeValue(" ");
      this.OutputLengthPropertyValue(right);
      this.writer.WriteAttributeValue(" ");
      this.OutputLengthPropertyValue(bottom);
      this.writer.WriteAttributeValue(" ");
      this.OutputLengthPropertyValue(left);
    }

    private void OutputCompositeBorderWidthPropertyValue(Format.PropertyValue topBottom, Format.PropertyValue rightLeft)
    {
      this.OutputBorderWidthPropertyValue(topBottom);
      this.writer.WriteAttributeValue(" ");
      this.OutputBorderWidthPropertyValue(rightLeft);
    }

    private void OutputCompositeBorderWidthPropertyValue(Format.PropertyValue top, Format.PropertyValue right, Format.PropertyValue bottom, Format.PropertyValue left)
    {
      this.OutputBorderWidthPropertyValue(top);
      this.writer.WriteAttributeValue(" ");
      this.OutputBorderWidthPropertyValue(right);
      this.writer.WriteAttributeValue(" ");
      this.OutputBorderWidthPropertyValue(bottom);
      this.writer.WriteAttributeValue(" ");
      this.OutputBorderWidthPropertyValue(left);
    }

    private void OutputCompositeBorderStylePropertyValue(Format.PropertyValue topBottom, Format.PropertyValue rightLeft)
    {
      this.OutputBorderStylePropertyValue(topBottom);
      this.writer.WriteAttributeValue(" ");
      this.OutputBorderStylePropertyValue(rightLeft);
    }

    private void OutputCompositeBorderStylePropertyValue(Format.PropertyValue top, Format.PropertyValue right, Format.PropertyValue bottom, Format.PropertyValue left)
    {
      this.OutputBorderStylePropertyValue(top);
      this.writer.WriteAttributeValue(" ");
      this.OutputBorderStylePropertyValue(right);
      this.writer.WriteAttributeValue(" ");
      this.OutputBorderStylePropertyValue(bottom);
      this.writer.WriteAttributeValue(" ");
      this.OutputBorderStylePropertyValue(left);
    }

    private void OutputCompositeBorderColorPropertyValue(Format.PropertyValue topBottom, Format.PropertyValue rightLeft)
    {
      this.OutputBorderColorPropertyValue(topBottom);
      this.writer.WriteAttributeValue(" ");
      this.OutputBorderColorPropertyValue(rightLeft);
    }

    private void OutputCompositeBorderColorPropertyValue(Format.PropertyValue top, Format.PropertyValue right, Format.PropertyValue bottom, Format.PropertyValue left)
    {
      this.OutputBorderColorPropertyValue(top);
      this.writer.WriteAttributeValue(" ");
      this.OutputBorderColorPropertyValue(right);
      this.writer.WriteAttributeValue(" ");
      this.OutputBorderColorPropertyValue(bottom);
      this.writer.WriteAttributeValue(" ");
      this.OutputBorderColorPropertyValue(left);
    }

    private void OutputLengthPropertyValue(Format.PropertyValue width)
    {
      BufferString bufferString = HtmlSupport.FormatLength(ref this.scratchBuffer, width);
      if (bufferString.Length != 0)
        this.writer.WriteAttributeValue(bufferString);
      else
        this.writer.WriteAttributeValue("0");
    }

    private void OutputBorderWidthPropertyValue(Format.PropertyValue width)
    {
      BufferString bufferString = HtmlSupport.FormatLength(ref this.scratchBuffer, width);
      if (bufferString.Length != 0)
        this.writer.WriteAttributeValue(bufferString);
      else
        this.writer.WriteAttributeValue("medium");
    }

    private void OutputBorderStylePropertyValue(Format.PropertyValue style)
    {
      string borderStyleString = HtmlSupport.GetBorderStyleString(style);
      if (borderStyleString != null)
        this.writer.WriteAttributeValue(borderStyleString);
      else
        this.writer.WriteAttributeValue("solid");
    }

    private void OutputBorderColorPropertyValue(Format.PropertyValue color)
    {
      BufferString bufferString = HtmlSupport.FormatColor(ref this.scratchBuffer, color);
      if (bufferString.Length != 0)
        this.writer.WriteAttributeValue(bufferString);
      else
        this.writer.WriteAttributeValue("black");
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
        this.writer.WriteAttributeName(HtmlNameIndex.Style);
        styleAttributeOpen = true;
      }
      if (!distinctProperty1.IsNull)
      {
        this.writer.WriteAttributeValue("table-layout:");
        this.writer.WriteAttributeValue(distinctProperty1.Bool ? "fixed" : "auto");
        this.writer.WriteAttributeValue(";");
      }
      if (!distinctProperty2.IsNull)
      {
        this.writer.WriteAttributeValue("border-collapse:");
        this.writer.WriteAttributeValue(distinctProperty2.Bool ? "collapse" : "separate");
        this.writer.WriteAttributeValue(";");
      }
      if (!distinctProperty3.IsNull)
      {
        this.writer.WriteAttributeValue("empty-cells:");
        this.writer.WriteAttributeValue(distinctProperty3.Bool ? "show" : "hide");
        this.writer.WriteAttributeValue(";");
      }
      if (!distinctProperty4.IsNull)
      {
        this.writer.WriteAttributeValue("caption-side:");
        this.writer.WriteAttributeValue(distinctProperty4.Bool ? "top" : "bottom");
        this.writer.WriteAttributeValue(";");
      }
      if (distinctProperty5.IsNull || distinctProperty5.IsNull)
        return;
      BufferString bufferString1 = HtmlSupport.FormatLength(ref this.scratchBuffer, distinctProperty5);
      if (bufferString1.Length == 0)
        return;
      this.writer.WriteAttributeValue("border-spacing:");
      this.writer.WriteAttributeValue(bufferString1);
      if (distinctProperty5 != distinctProperty6)
      {
        BufferString bufferString2 = HtmlSupport.FormatLength(ref this.scratchBuffer, distinctProperty6);
        if (bufferString2.Length != 0)
        {
          this.writer.WriteAttributeValue(" ");
          this.writer.WriteAttributeValue(bufferString2);
        }
      }
      this.writer.WriteAttributeValue(";");
    }

    private void OutputTableColumnCssProperties(ref bool styleAttributeOpen)
    {
      Format.PropertyValue distinctProperty1 = this.GetDistinctProperty(Format.PropertyId.Width);
      Format.PropertyValue distinctProperty2 = this.GetDistinctProperty(Format.PropertyId.BackColor);
      if (distinctProperty2.IsNull && distinctProperty1.IsNull)
        return;
      if (!styleAttributeOpen)
      {
        this.writer.WriteAttributeName(HtmlNameIndex.Style);
        styleAttributeOpen = true;
      }
      if (!distinctProperty1.IsNull)
      {
        BufferString bufferString = HtmlSupport.FormatLength(ref this.scratchBuffer, distinctProperty1);
        if (bufferString.Length != 0)
        {
          this.writer.WriteAttributeValue("width:");
          this.writer.WriteAttributeValue(bufferString);
          this.writer.WriteAttributeValue(";");
        }
      }
      if (distinctProperty2.IsNull)
        return;
      BufferString bufferString1 = HtmlSupport.FormatColor(ref this.scratchBuffer, distinctProperty2);
      if (bufferString1.Length == 0)
        return;
      this.writer.WriteAttributeValue("background-color:");
      this.writer.WriteAttributeValue(bufferString1);
    }

    private void OutputBlockTagAttributes()
    {
      Format.PropertyValue distinctProperty1 = this.GetDistinctProperty(Format.PropertyId.RightToLeft);
      if (!distinctProperty1.IsNull)
        this.writer.WriteAttribute(HtmlNameIndex.Dir, distinctProperty1.Bool ? "rtl" : "ltr");
      Format.PropertyValue distinctProperty2 = this.GetDistinctProperty(Format.PropertyId.TextAlignment);
      if (!distinctProperty2.IsNull)
      {
        string textAlignmentString = HtmlSupport.GetTextAlignmentString(distinctProperty2);
        if (textAlignmentString != null)
          this.writer.WriteAttribute(HtmlNameIndex.Align, textAlignmentString);
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
          this.writer.WriteAttribute(HtmlNameIndex.Width, bufferString);
      }
      Format.PropertyValue distinctProperty2 = this.GetDistinctProperty(Format.PropertyId.BlockAlignment);
      if (!distinctProperty2.IsNull)
      {
        string horizontalAlignmentString = HtmlSupport.GetHorizontalAlignmentString(distinctProperty2);
        if (horizontalAlignmentString != null)
          this.writer.WriteAttribute(HtmlNameIndex.Align, horizontalAlignmentString);
      }
      Format.PropertyValue distinctProperty3 = this.GetDistinctProperty(Format.PropertyId.RightToLeft);
      if (!distinctProperty3.IsNull)
        this.writer.WriteAttribute(HtmlNameIndex.Dir, distinctProperty3.Bool ? "rtl" : "ltr");
      Format.PropertyValue distinctProperty4 = this.GetDistinctProperty(Format.PropertyId.TableBorder);
      if (!distinctProperty4.IsNull)
      {
        BufferString bufferString = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, distinctProperty4);
        if (bufferString.Length != 0)
          this.writer.WriteAttribute(HtmlNameIndex.Border, bufferString);
      }
      Format.PropertyValue distinctProperty5 = this.GetDistinctProperty(Format.PropertyId.TableFrame);
      if (!distinctProperty5.IsNull)
      {
        string tableFrameString = HtmlSupport.GetTableFrameString(distinctProperty5);
        if (tableFrameString != null)
          this.writer.WriteAttribute(HtmlNameIndex.Frame, tableFrameString);
      }
      Format.PropertyValue distinctProperty6 = this.GetDistinctProperty(Format.PropertyId.TableRules);
      if (!distinctProperty6.IsNull)
      {
        string tableRulesString = HtmlSupport.GetTableRulesString(distinctProperty6);
        if (tableRulesString != null)
          this.writer.WriteAttribute(HtmlNameIndex.Rules, tableRulesString);
      }
      Format.PropertyValue distinctProperty7 = this.GetDistinctProperty(Format.PropertyId.TableCellSpacing);
      if (!distinctProperty7.IsNull)
      {
        BufferString bufferString = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, distinctProperty7);
        if (bufferString.Length != 0)
          this.writer.WriteAttribute(HtmlNameIndex.CellSpacing, bufferString);
      }
      Format.PropertyValue distinctProperty8 = this.GetDistinctProperty(Format.PropertyId.TableCellPadding);
      if (distinctProperty8.IsNull)
        return;
      BufferString bufferString1 = HtmlSupport.FormatPixelOrPercentageLength(ref this.scratchBuffer, distinctProperty8);
      if (bufferString1.Length == 0)
        return;
      this.writer.WriteAttribute(HtmlNameIndex.CellPadding, bufferString1);
    }

    private void OutputTableCellTagAttributes()
    {
      Format.PropertyValue distinctProperty1 = this.GetDistinctProperty(Format.PropertyId.NumColumns);
      if (distinctProperty1.IsInteger && distinctProperty1.Integer != 1)
        this.writer.WriteAttribute(HtmlNameIndex.ColSpan, distinctProperty1.Integer.ToString());
      Format.PropertyValue distinctProperty2 = this.GetDistinctProperty(Format.PropertyId.NumRows);
      if (distinctProperty2.IsInteger && distinctProperty2.Integer != 1)
        this.writer.WriteAttribute(HtmlNameIndex.RowSpan, distinctProperty2.Integer.ToString());
      Format.PropertyValue distinctProperty3 = this.GetDistinctProperty(Format.PropertyId.Width);
      if (!distinctProperty3.IsNull && distinctProperty3.IsAbsRelLength)
        this.writer.WriteAttribute(HtmlNameIndex.Width, distinctProperty3.PixelsInteger.ToString());
      Format.PropertyValue distinctProperty4 = this.GetDistinctProperty(Format.PropertyId.TextAlignment);
      if (!distinctProperty4.IsNull)
      {
        string textAlignmentString = HtmlSupport.GetTextAlignmentString(distinctProperty4);
        if (textAlignmentString != null)
          this.writer.WriteAttribute(HtmlNameIndex.Align, textAlignmentString);
      }
      Format.PropertyValue distinctProperty5 = this.GetDistinctProperty(Format.PropertyId.BlockAlignment);
      if (!distinctProperty5.IsNull)
      {
        string verticalAlignmentString = HtmlSupport.GetVerticalAlignmentString(distinctProperty5);
        if (verticalAlignmentString != null)
          this.writer.WriteAttribute(HtmlNameIndex.Valign, verticalAlignmentString);
      }
      Format.PropertyValue distinctProperty6 = this.GetDistinctProperty(Format.PropertyId.TableCellNoWrap);
      if (distinctProperty6.IsNull || !distinctProperty6.Bool)
        return;
      this.writer.WriteAttribute(HtmlNameIndex.NoWrap, string.Empty);
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
