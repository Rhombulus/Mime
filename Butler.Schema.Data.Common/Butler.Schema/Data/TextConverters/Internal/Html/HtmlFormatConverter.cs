// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Html.HtmlFormatConverter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Html
{
  internal class HtmlFormatConverter : Format.FormatConverter, IProducerConsumer, IRestartable, IDisposable
  {
    protected char[] literalBuffer = new char[2];
    protected bool convertFragment;
    protected HtmlNormalizingParser parser;
    protected HtmlToken token;
    protected bool treatNbspAsBreakable;
    protected bool insideComment;
    protected bool insideStyle;
    protected bool insidePre;
    protected int insideList;
    protected int temporarilyClosedLevels;
    protected Format.FormatOutput output;
    private Css.CssParser cssParser;
    protected ConverterBufferInput cssParserInput;
    private Dictionary<HtmlFormatConverter.StyleSelector, int> styleDictionary;
    private int[] styleHandleIndex;
    private int styleHandleIndexCount;
    private ScratchBuffer scratch;
    private Format.Property[] parsedProperties;
    private PropertyValueParsingMethod attributeParsingMethod;
    private Format.PropertyId attributePropertyId;
    private IProgressMonitor progressMonitor;

    private bool CanFlush
    {
      get
      {
        return this.output.CanAcceptMoreOutput;
      }
    }

    public HtmlFormatConverter(HtmlNormalizingParser parser, Format.FormatOutput output, bool testTreatNbspAsBreakable, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum, Stream formatConverterTraceStream, IProgressMonitor progressMonitor)
      : base(formatConverterTraceStream)
    {
      this.parser = parser;
      this.parser.SetRestartConsumer((IRestartable) this);
      this.progressMonitor = progressMonitor;
      this.output = output;
      if (this.output != null)
        this.output.Initialize(this.Store, Format.SourceFormat.Html, "converted from html");
      this.treatNbspAsBreakable = testTreatNbspAsBreakable;
      this.InitializeDocument();
    }

    public HtmlFormatConverter(HtmlNormalizingParser parser, Format.FormatStore formatStore, bool fragment, bool testTreatNbspAsBreakable, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum, Stream formatConverterTraceStream, IProgressMonitor progressMonitor)
      : base(formatStore, formatConverterTraceStream)
    {
      this.parser = parser;
      this.parser.SetRestartConsumer((IRestartable) this);
      this.progressMonitor = progressMonitor;
      this.treatNbspAsBreakable = testTreatNbspAsBreakable;
      this.convertFragment = fragment;
      if (fragment)
        return;
      this.InitializeDocument();
    }

    public Format.FormatNode Initialize(string fragment)
    {
      this.parser.Initialize(fragment, false);
      Format.FormatNode formatNode = this.InitializeFragment();
      this.Initialize();
      return formatNode;
    }

    bool IRestartable.CanRestart()
    {
      if (this.output != null)
        return ((IRestartable) this.output).CanRestart();
      return true;
    }

    void IRestartable.Restart()
    {
      if (this.output != null)
        ((IRestartable) this.output).Restart();
      this.Store.Initialize();
      this.InitializeDocument();
      this.Initialize();
    }

    void IRestartable.DisableRestart()
    {
      if (this.output == null)
        return;
      ((IRestartable) this.output).DisableRestart();
    }

    public override void Run()
    {
      if (this.output != null && this.MustFlush)
      {
        if (!this.CanFlush)
          return;
        this.FlushOutput();
      }
      else
      {
        if (this.EndOfFile)
          return;
        HtmlTokenId tokenId = this.parser.Parse();
        if (tokenId == HtmlTokenId.None)
          return;
        this.Process(tokenId);
      }
    }

    public bool Flush()
    {
      this.Run();
      if (this.EndOfFile)
        return !this.MustFlush;
      return false;
    }

    void IDisposable.Dispose()
    {
      if (this.parser != null)
        ((IDisposable) this.parser).Dispose();
      if (this.token != null && this.token is IDisposable)
        ((IDisposable) this.token).Dispose();
      this.parser = (HtmlNormalizingParser) null;
      this.token = (HtmlToken) null;
      this.literalBuffer = (char[]) null;
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Process(HtmlTokenId tokenId)
    {
      this.token = this.parser.Token;
      switch (tokenId)
      {
        case HtmlTokenId.EndOfFile:
          this.CloseAllContainersAndSetEOF();
          break;
        case HtmlTokenId.Text:
          if (this.insideStyle)
          {
            this.token.Text.WriteTo((ITextSink) this.cssParserInput);
            break;
          }
          if (this.insideComment)
            break;
          if (this.insidePre)
          {
            this.ProcessPreformatedText();
            break;
          }
          this.ProcessText();
          break;
        case HtmlTokenId.EncodingChange:
          if (this.output == null || !this.output.OutputCodePageSameAsInput)
            break;
          this.output.OutputEncoding = this.token.TokenEncoding;
          break;
        case HtmlTokenId.Tag:
          if (this.token.TagIndex <= HtmlTagIndex.Unknown)
          {
            if (!this.insideStyle || this.token.TagIndex != HtmlTagIndex._COMMENT)
              break;
            this.token.Text.WriteTo((ITextSink) this.cssParserInput);
            break;
          }
          HtmlDtd.TagDefinition tagDefinition = HtmlFormatConverter.GetTagDefinition(this.token.TagIndex);
          if (!this.token.IsEndTag)
          {
            if (this.token.IsTagBegin)
              this.PushElement(tagDefinition, this.token.IsEmptyScope);
            this.ProcessStartTagAttributes(tagDefinition);
            break;
          }
          if (!this.token.IsTagEnd)
            break;
          this.PopElement(this.BuildStackTop - 1 - this.temporarilyClosedLevels, this.token.Argument != 1);
          break;
        case HtmlTokenId.OverlappedClose:
          this.temporarilyClosedLevels = this.token.Argument;
          break;
        case HtmlTokenId.OverlappedReopen:
          this.temporarilyClosedLevels = 0;
          break;
      }
    }

    protected override Format.FormatNode GetParentForNewNode(Format.FormatNode node, Format.FormatNode parent, int stackPos, out int propContainerInheritanceStopLevel)
    {
      switch (node.NodeType)
      {
        case Format.FormatContainerType.TableColumnGroup:
          propContainerInheritanceStopLevel = stackPos;
          Format.FormatNode formatNode1 = parent.NodeType == Format.FormatContainerType.Table ? parent : this.FindStackAncestor(stackPos, Format.FormatContainerType.Table);
          Format.FormatNode formatNode2 = Format.FormatNode.Null;
          Format.FormatNode formatNode3 = formatNode1.FirstChild;
          if (!formatNode3.IsNull && formatNode3.NodeType == Format.FormatContainerType.TableCaption)
          {
            formatNode2 = formatNode3;
            formatNode3 = formatNode3.NextSibling;
          }
          if (formatNode3.IsNull || formatNode3.NodeType != Format.FormatContainerType.TableDefinition)
          {
            formatNode3 = this.Store.AllocateNode(Format.FormatContainerType.TableDefinition);
            formatNode3.InheritanceMaskIndex = 1;
            formatNode3.SetOutOfOrder();
            if (formatNode2.IsNull)
              formatNode1.PrependChild(formatNode3);
            else
              formatNode2.InsertSiblingAfter(formatNode3);
          }
          parent = formatNode3;
          break;
        case Format.FormatContainerType.TableColumn:
          Format.FormatNode formatNode4 = parent.NodeType == Format.FormatContainerType.TableColumnGroup ? parent : this.FindStackParentForColumn(stackPos);
          if (!formatNode4.IsNull)
          {
            propContainerInheritanceStopLevel = stackPos;
            parent = formatNode4;
            break;
          }
          goto case Format.FormatContainerType.TableColumnGroup;
        case Format.FormatContainerType.TableCaption:
          propContainerInheritanceStopLevel = stackPos;
          Format.FormatNode formatNode5 = parent.NodeType == Format.FormatContainerType.Table ? parent : this.FindStackAncestor(stackPos, Format.FormatContainerType.Table);
          Format.FormatNode newChildNode1 = formatNode5.FirstChild;
          if (newChildNode1.IsNull || newChildNode1.NodeType != Format.FormatContainerType.TableCaption)
          {
            newChildNode1 = this.Store.AllocateNode(Format.FormatContainerType.TableCaption);
            newChildNode1.InheritanceMaskIndex = 1;
            newChildNode1.SetOutOfOrder();
            formatNode5.PrependChild(newChildNode1);
          }
          parent = newChildNode1;
          break;
        case Format.FormatContainerType.TableExtraContent:
          propContainerInheritanceStopLevel = stackPos;
          Format.FormatNode formatNode6 = parent.NodeType == Format.FormatContainerType.Table || parent.NodeType == Format.FormatContainerType.TableRow || parent.NodeType == Format.FormatContainerType.TableColumnGroup ? this.FindStackParentForExtraContent(stackPos, out propContainerInheritanceStopLevel) : parent;
          if (formatNode6.IsNull)
          {
            Format.FormatNode newChildNode2 = parent.NodeType == Format.FormatContainerType.Table ? parent : this.FindStackAncestor(stackPos, Format.FormatContainerType.Table);
            Format.FormatNode newSiblingNode = newChildNode2.Parent;
            Format.FormatNode newChildNode3 = Format.FormatNode.Null;
            if (newSiblingNode.NodeType != Format.FormatContainerType.TableContainer)
            {
              newSiblingNode = this.Store.AllocateNode(Format.FormatContainerType.TableContainer, newChildNode2.BeginTextPosition);
              newSiblingNode.InheritanceMaskIndex = 1;
              newChildNode2.InsertSiblingAfter(newSiblingNode);
              newChildNode2.RemoveFromParent();
              newSiblingNode.AppendChild(newChildNode2);
              newSiblingNode.SetOnRightEdge();
              newChildNode3 = this.Store.AllocateNode(Format.FormatContainerType.TableExtraContent);
              newChildNode3.InheritanceMaskIndex = 1;
              newChildNode3.SetOutOfOrder();
              if (newChildNode2.OnLeftEdge)
              {
                newSiblingNode.SetOnLeftEdge();
                newSiblingNode.AppendChild(newChildNode3);
              }
              else
                newSiblingNode.PrependChild(newChildNode3);
            }
            else
            {
              foreach (Format.FormatNode formatNode7 in newSiblingNode.Children)
              {
                if (formatNode7.NodeType == Format.FormatContainerType.TableExtraContent)
                  newChildNode3 = formatNode7;
              }
            }
            formatNode6 = newChildNode3;
          }
          parent = formatNode6;
          break;
        case Format.FormatContainerType.TableRow:
          if (parent.NodeType != Format.FormatContainerType.Table)
            parent = this.FindStackAncestor(stackPos, Format.FormatContainerType.Table);
          propContainerInheritanceStopLevel = stackPos;
          break;
        case Format.FormatContainerType.TableCell:
          if (parent.NodeType != Format.FormatContainerType.TableRow)
            parent = this.FindStackAncestor(stackPos, Format.FormatContainerType.TableRow);
          propContainerInheritanceStopLevel = stackPos;
          break;
        default:
          if (parent.NodeType != Format.FormatContainerType.Table && parent.NodeType != Format.FormatContainerType.TableRow && parent.NodeType != Format.FormatContainerType.TableColumnGroup)
          {
            propContainerInheritanceStopLevel = this.DefaultPropContainerInheritanceStopLevel(stackPos);
            break;
          }
          goto case Format.FormatContainerType.TableExtraContent;
      }
      return parent;
    }

    protected override Format.FormatContainerType FixContainerType(Format.FormatContainerType type, Format.StyleBuildHelper styleBuilderWithContainerProperties)
    {
      Format.PropertyValue property1 = styleBuilderWithContainerProperties.GetProperty(Format.PropertyId.Display);
      if (property1.IsEnum)
      {
        switch (property1.Enum)
        {
          case 1:
            if (type == Format.FormatContainerType.Block)
            {
              type = Format.FormatContainerType.Inline;
              break;
            }
            break;
          case 2:
            if (type == Format.FormatContainerType.PropertyContainer || type == Format.FormatContainerType.Inline)
            {
              type = Format.FormatContainerType.Block;
              break;
            }
            break;
        }
      }
      if (type == Format.FormatContainerType.PropertyContainer)
      {
        Format.PropertyValue property2 = styleBuilderWithContainerProperties.GetProperty(Format.PropertyId.UnicodeBiDi);
        if (property2.IsEnum && (int) (byte) property2.Enum != 0)
          type = Format.FormatContainerType.Inline;
      }
      if (type == Format.FormatContainerType.HyperLink && styleBuilderWithContainerProperties.GetProperty(Format.PropertyId.HyperlinkUrl).IsNull)
        type = Format.FormatContainerType.Bookmark;
      return type;
    }

    protected static HtmlDtd.TagDefinition GetTagDefinition(HtmlTagIndex tagIndex)
    {
      if (tagIndex == HtmlTagIndex._NULL)
        return (HtmlDtd.TagDefinition) null;
      return HtmlDtd.tags[(int) tagIndex];
    }

    protected void ProcessStartTagAttributes(HtmlDtd.TagDefinition tagDef)
    {
      if (HtmlConverterData.tagInstructions[(int) this.token.TagIndex].ContainerType != Format.FormatContainerType.Null)
      {
        this.token.Attributes.Rewind();
        foreach (HtmlAttribute attr in this.token.Attributes)
        {
          if (attr.NameIndex == HtmlNameIndex.Style)
            this.ProcessStyleAttribute(tagDef, attr);
          else if (attr.NameIndex == HtmlNameIndex.Id)
          {
            string @string = attr.Value.GetString(60);
            this.FindAndApplyStyle(new HtmlFormatConverter.StyleSelector(this.token.NameIndex, (string) null, @string));
            this.FindAndApplyStyle(new HtmlFormatConverter.StyleSelector(HtmlNameIndex.Unknown, (string) null, @string));
            this.ProcessNonStyleAttribute(attr);
          }
          else if (attr.NameIndex == HtmlNameIndex.Class)
          {
            string @string = attr.Value.GetString(60);
            this.FindAndApplyStyle(new HtmlFormatConverter.StyleSelector(this.token.NameIndex, @string, (string) null));
            this.FindAndApplyStyle(new HtmlFormatConverter.StyleSelector(HtmlNameIndex.Unknown, @string, (string) null));
            if (!this.LastNonEmpty.Node.IsNull && @string.Equals("EmailQuote", StringComparison.OrdinalIgnoreCase))
              this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.QuotingLevelDelta, new Format.PropertyValue(Format.PropertyType.Integer, 1));
          }
          else if (attr.NameIndex != HtmlNameIndex.Unknown)
            this.ProcessNonStyleAttribute(attr);
        }
      }
      if (!this.token.IsTagEnd || this.token.TagIndex != HtmlTagIndex.BR)
        return;
      this.AddLineBreak(1);
    }

    protected void ProcessPreformatedText()
    {
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
                this.AddLineBreak(1);
                continue;
              case RunTextType.Tabulation:
                this.AddTabulation(current.Length);
                continue;
              default:
                this.AddSpace(current.Length);
                continue;
            }
          }
          else if (current.TextType == RunTextType.Nbsp)
          {
            if (this.treatNbspAsBreakable)
              this.AddSpace(current.Length);
            else
              this.AddNbsp(current.Length);
          }
          else
            this.OutputNonspace(current);
        }
      }
    }

    protected void ProcessText()
    {
      Token.RunEnumerator enumerator = this.token.Runs.GetEnumerator();
      while (enumerator.MoveNext())
      {
        TokenRun current = enumerator.Current;
        if (current.IsTextRun)
        {
          if (current.IsAnyWhitespace)
            this.AddSpace(1);
          else if (current.TextType == RunTextType.Nbsp)
          {
            if (this.treatNbspAsBreakable)
              this.AddSpace(current.Length);
            else
              this.AddNbsp(current.Length);
          }
          else
            this.OutputNonspace(current);
        }
      }
    }

    protected int PushElement(HtmlDtd.TagDefinition tagDef, bool emptyScope)
    {
      int num1 = this.BuildStackTop;
      Format.FormatConverterContainer converterContainer = Format.FormatConverterContainer.Null;
      if (HtmlConverterData.tagInstructions[(int) tagDef.TagIndex].ContainerType != Format.FormatContainerType.Null)
      {
        int defaultStyle = HtmlConverterData.tagInstructions[(int) tagDef.TagIndex].DefaultStyle;
        converterContainer = this.OpenContainer(HtmlConverterData.tagInstructions[(int) tagDef.TagIndex].ContainerType, emptyScope, HtmlConverterData.tagInstructions[(int) tagDef.TagIndex].InheritanceMaskIndex, this.GetStyle(defaultStyle), tagDef.TagIndex);
      }
      else if (!emptyScope)
        converterContainer = this.OpenContainer(Format.FormatContainerType.PropertyContainer, false, HtmlConverterData.tagInstructions[(int) tagDef.TagIndex].InheritanceMaskIndex, Format.FormatStyle.Null, tagDef.TagIndex);
      if (!converterContainer.IsNull)
      {
        HtmlTagIndex htmlTagIndex = tagDef.TagIndex;
        if ((uint) htmlTagIndex <= 67U)
        {
          if ((uint) htmlTagIndex <= 31U)
          {
            if (htmlTagIndex != HtmlTagIndex.BlockQuote)
            {
              if (htmlTagIndex != HtmlTagIndex.Comment)
                goto label_22;
            }
            else
              goto label_20;
          }
          else
          {
            switch (htmlTagIndex)
            {
              case HtmlTagIndex.DL:
                goto label_19;
              case HtmlTagIndex.H1:
              case HtmlTagIndex.H2:
              case HtmlTagIndex.H3:
              case HtmlTagIndex.H4:
              case HtmlTagIndex.H5:
              case HtmlTagIndex.H6:
                goto label_20;
              case HtmlTagIndex.Listing:
                goto label_18;
              default:
                goto label_22;
            }
          }
        }
        else if ((uint) htmlTagIndex <= 91U)
        {
          switch (htmlTagIndex)
          {
            case HtmlTagIndex.OL:
              goto label_19;
            case HtmlTagIndex.P:
              goto label_20;
            case HtmlTagIndex.PlainText:
            case HtmlTagIndex.Pre:
              goto label_18;
            case HtmlTagIndex.Script:
              break;
            default:
              goto label_22;
          }
        }
        else
        {
          switch (htmlTagIndex)
          {
            case HtmlTagIndex.Style:
              if (this.cssParserInput == null)
              {
                this.cssParserInput = new ConverterBufferInput(524288, this.progressMonitor);
                this.cssParser = new Css.CssParser((ConverterInput) this.cssParserInput, 1024, false);
              }
              this.insideStyle = true;
              goto label_22;
            case HtmlTagIndex.Title:
              this.insideComment = !this.token.IsEndTag;
              goto label_22;
            case HtmlTagIndex.UL:
              goto label_19;
            case HtmlTagIndex.Xmp:
              goto label_18;
            case HtmlTagIndex.Xml:
              break;
            default:
              goto label_22;
          }
        }
        this.insideComment = true;
        goto label_22;
label_18:
        this.insidePre = true;
        goto label_20;
label_19:
        int num2 = this.insideList;
        ++this.insideList;
        if (num2 != 0)
          goto label_22;
label_20:
        if (!this.LastNode.FirstChild.IsNull || this.LastNode.NodeType != Format.FormatContainerType.Document && this.LastNode.NodeType != Format.FormatContainerType.Fragment && this.LastNode.NodeType != Format.FormatContainerType.TableCell)
        {
          converterContainer.SetProperty(Format.PropertyPrecedence.TagDefault, Format.PropertyId.Margins, new Format.PropertyValue(Format.LengthUnits.Points, 14));
          converterContainer.SetProperty(Format.PropertyPrecedence.TagDefault, Format.PropertyId.BottomMargin, new Format.PropertyValue(Format.LengthUnits.Points, 14));
        }
label_22:
        this.FindAndApplyStyle(new HtmlFormatConverter.StyleSelector(tagDef.NameIndex, (string) null, (string) null));
      }
      return num1;
    }

    protected void PopElement(int stackPos, bool explicitClose)
    {
      Format.FormatNode node = this.Last.Node;
      HtmlTagIndex htmlTagIndex = this.BuildStack[stackPos].TagIndex;
      if ((uint) htmlTagIndex <= 78U)
      {
        if ((uint) htmlTagIndex <= 37U)
        {
          if (htmlTagIndex != HtmlTagIndex.Comment)
          {
            if (htmlTagIndex == HtmlTagIndex.DL)
              goto label_15;
            else
              goto label_16;
          }
        }
        else if (htmlTagIndex != HtmlTagIndex.Listing)
        {
          if (htmlTagIndex == HtmlTagIndex.OL)
            goto label_15;
          else
            goto label_16;
        }
        else
          goto label_14;
      }
      else if ((uint) htmlTagIndex <= 91U)
      {
        switch (htmlTagIndex)
        {
          case HtmlTagIndex.PlainText:
          case HtmlTagIndex.Pre:
            goto label_14;
          case HtmlTagIndex.Script:
            break;
          default:
            goto label_16;
        }
      }
      else
      {
        switch (htmlTagIndex)
        {
          case HtmlTagIndex.Style:
            if (this.insideStyle)
            {
              this.ProcessStylesheet();
              this.insideStyle = false;
              goto label_16;
            }
            else
              goto label_16;
          case HtmlTagIndex.Title:
            this.insideComment = !this.token.IsEndTag;
            goto label_16;
          case HtmlTagIndex.UL:
            goto label_15;
          case HtmlTagIndex.Xmp:
            goto label_14;
          case HtmlTagIndex.Xml:
            break;
          default:
            goto label_16;
        }
      }
      this.insideComment = false;
      goto label_16;
label_14:
      this.insidePre = false;
      goto label_16;
label_15:
      --this.insideList;
label_16:
      if (stackPos == this.BuildStackTop - 1)
        this.CloseContainer();
      else
        this.CloseOverlappingContainer(this.BuildStackTop - 1 - stackPos);
      if (node.IsNull || node.NodeType != Format.FormatContainerType.Table)
        return;
      while (!node.LastChild.IsNull && node.LastChild.NodeType == Format.FormatContainerType.TableRow)
      {
        bool flag = true;
        if (!node.LastChild.GetProperty(Format.PropertyId.Height).IsNull)
          break;
        foreach (Format.FormatNode formatNode in node.LastChild.Children)
        {
          if (!formatNode.FirstChild.IsNull)
          {
            flag = false;
            break;
          }
        }
        if (!flag)
          break;
        node.LastChild.RemoveFromParent();
      }
    }

    private void Initialize()
    {
      this.insideComment = false;
      this.insidePre = false;
      this.insideStyle = false;
      this.insideList = 0;
      this.temporarilyClosedLevels = 0;
      if (this.styleDictionary != null)
        this.styleDictionary.Clear();
      if (this.cssParserInput == null)
        return;
      this.cssParserInput.Reset();
      this.cssParser.Reset();
    }

    private bool FlushOutput()
    {
      if (!this.output.Flush())
        return false;
      this.MustFlush = false;
      return true;
    }

    private Format.FormatNode FindStackAncestor(int stackPosOfNewContainer, Format.FormatContainerType type)
    {
      for (int index = stackPosOfNewContainer - 1; index >= 0; --index)
      {
        if (this.BuildStack[index].Type == type)
          return this.Store.GetNode(this.BuildStack[index].Node);
      }
      return Format.FormatNode.Null;
    }

    private Format.FormatNode FindStackParentForExtraContent(int stackPosOfNewContainer, out int ancestorContainerLevel)
    {
      bool flag = false;
      for (int index = stackPosOfNewContainer - 1; index >= 0; --index)
      {
        if (this.BuildStack[index].Node != 0)
        {
          if (this.BuildStack[index].Type == Format.FormatContainerType.Table)
            flag = true;
          else if (this.BuildStack[index].Type != Format.FormatContainerType.TableRow && this.BuildStack[index].Type != Format.FormatContainerType.TableColumnGroup)
          {
            ancestorContainerLevel = index + 1;
            if (!flag)
              return this.Store.GetNode(this.BuildStack[index].Node);
            return Format.FormatNode.Null;
          }
        }
      }
      ancestorContainerLevel = stackPosOfNewContainer;
      return Format.FormatNode.Null;
    }

    private Format.FormatNode FindStackParentForColumn(int stackPosOfNewContainer)
    {
      for (int index = stackPosOfNewContainer - 1; index >= 0; --index)
      {
        if (this.BuildStack[index].Node != 0)
        {
          if (this.BuildStack[index].Type != Format.FormatContainerType.Table)
          {
            if (this.BuildStack[index].Type == Format.FormatContainerType.TableColumnGroup)
              return this.Store.GetNode(this.BuildStack[index].Node);
          }
          else
            break;
        }
      }
      return Format.FormatNode.Null;
    }

    private bool StartTagHasAttribute(HtmlNameIndex attributeNameIndex)
    {
      foreach (HtmlAttribute htmlAttribute in this.token.Attributes)
      {
        if (htmlAttribute.NameIndex == attributeNameIndex)
          return true;
      }
      return false;
    }

    private void OutputNonspace(TokenRun run)
    {
      if (run.IsLiteral)
        this.AddNonSpaceText(this.literalBuffer, 0, run.ReadLiteral(this.literalBuffer));
      else
        this.AddNonSpaceText(run.RawBuffer, run.RawOffset, run.RawLength);
    }

    private void ProcessStyleAttribute(HtmlDtd.TagDefinition tagDef, HtmlAttribute attr)
    {
      if (attr.IsAttrBegin)
      {
        if (this.cssParserInput == null)
        {
          this.cssParserInput = new ConverterBufferInput(524288, this.progressMonitor);
          this.cssParser = new Css.CssParser((ConverterInput) this.cssParserInput, 1024, false);
        }
        this.cssParser.SetParseMode(Css.CssParseMode.StyleAttribute);
      }
      attr.Value.Rewind();
      attr.Value.WriteTo((ITextSink) this.cssParserInput);
      if (!attr.IsAttrEnd)
        return;
      Css.CssTokenId cssTokenId;
      do
      {
        cssTokenId = this.cssParser.Parse();
        if (Css.CssTokenId.Declarations == cssTokenId && this.cssParser.Token.Properties.ValidCount != 0)
        {
          this.cssParser.Token.Properties.Rewind();
          foreach (Css.CssProperty prop in this.cssParser.Token.Properties)
          {
            if (prop.NameId != Css.CssNameIndex.Unknown)
            {
              if (HtmlConverterData.cssPropertyInstructions[(int) prop.NameId].ParsingMethod != null)
              {
                Format.PropertyValue propertyValue;
                if (!prop.Value.IsEmpty && prop.Value.IsContiguous)
                {
                  propertyValue = HtmlConverterData.cssPropertyInstructions[(int) prop.NameId].ParsingMethod(prop.Value.ContiguousBufferString, (Format.FormatConverter) this);
                }
                else
                {
                  this.scratch.AppendCssPropertyValue(prop, 4096);
                  propertyValue = HtmlConverterData.cssPropertyInstructions[(int) prop.NameId].ParsingMethod(this.scratch.BufferString, (Format.FormatConverter) this);
                  this.scratch.Reset();
                }
                if (!propertyValue.IsNull)
                  this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, HtmlConverterData.cssPropertyInstructions[(int) prop.NameId].PropertyId, propertyValue);
              }
              else if (HtmlConverterData.cssPropertyInstructions[(int) prop.NameId].MultiPropertyParsingMethod != null)
              {
                if (this.parsedProperties == null)
                  this.parsedProperties = new Format.Property[12];
                int parsedPropertiesCount;
                if (!prop.Value.IsEmpty && prop.Value.IsContiguous)
                {
                  HtmlConverterData.cssPropertyInstructions[(int) prop.NameId].MultiPropertyParsingMethod(prop.Value.ContiguousBufferString, (Format.FormatConverter) this, HtmlConverterData.cssPropertyInstructions[(int) prop.NameId].PropertyId, this.parsedProperties, out parsedPropertiesCount);
                }
                else
                {
                  this.scratch.AppendCssPropertyValue(prop, 4096);
                  HtmlConverterData.cssPropertyInstructions[(int) prop.NameId].MultiPropertyParsingMethod(this.scratch.BufferString, (Format.FormatConverter) this, HtmlConverterData.cssPropertyInstructions[(int) prop.NameId].PropertyId, this.parsedProperties, out parsedPropertiesCount);
                  this.scratch.Reset();
                }
                if (parsedPropertiesCount != 0)
                  this.Last.SetProperties(Format.PropertyPrecedence.InlineStyle, this.parsedProperties, parsedPropertiesCount);
              }
            }
          }
        }
      }
      while (Css.CssTokenId.EndOfFile != cssTokenId);
      this.cssParserInput.Reset();
      this.cssParser.Reset();
    }

    private void ProcessNonStyleAttribute(HtmlAttribute attr)
    {
      if (attr.IsAttrBegin)
      {
        if (this.token.TagIndex == HtmlTagIndex.A && attr.NameIndex == HtmlNameIndex.Href)
        {
          this.Last.SetProperty(Format.PropertyPrecedence.TagDefault, Format.PropertyId.Underline, new Format.PropertyValue(true));
          this.Last.SetProperty(Format.PropertyPrecedence.TagDefault, Format.PropertyId.FontColor, new Format.PropertyValue(new Format.RGBT(0U, 0U, (uint) byte.MaxValue)));
        }
        this.attributeParsingMethod = (PropertyValueParsingMethod) null;
        if (HtmlConverterData.tagInstructions[(int) this.token.TagIndex].AttributeInstructions != null)
        {
          for (int index = 0; index < HtmlConverterData.tagInstructions[(int) this.token.TagIndex].AttributeInstructions.Length; ++index)
          {
            if (attr.NameIndex == HtmlConverterData.tagInstructions[(int) this.token.TagIndex].AttributeInstructions[index].AttributeNameId)
            {
              this.attributeParsingMethod = HtmlConverterData.tagInstructions[(int) this.token.TagIndex].AttributeInstructions[index].ParsingMethod;
              this.attributePropertyId = HtmlConverterData.tagInstructions[(int) this.token.TagIndex].AttributeInstructions[index].PropertyId;
              if (attr.IsAttrEnd && !attr.Value.IsEmpty && attr.Value.IsContiguous)
              {
                Format.PropertyValue propertyValue = this.attributeParsingMethod(attr.Value.ContiguousBufferString, (Format.FormatConverter) this);
                if (propertyValue.IsNull)
                  return;
                this.Last.SetProperty(Format.PropertyPrecedence.NonStyle, this.attributePropertyId, propertyValue);
                return;
              }
              break;
            }
          }
        }
        PropertyValueParsingMethod valueParsingMethod = this.attributeParsingMethod;
      }
      if (this.attributeParsingMethod == null)
        return;
      this.scratch.AppendHtmlAttributeValue(attr, 4096);
      if (!attr.IsAttrEnd)
        return;
      Format.PropertyValue propertyValue1 = this.attributeParsingMethod(this.scratch.BufferString, (Format.FormatConverter) this);
      if (!propertyValue1.IsNull)
        this.Last.SetProperty(Format.PropertyPrecedence.NonStyle, this.attributePropertyId, propertyValue1);
      this.scratch.Reset();
    }

    private void ProcessStylesheet()
    {
      this.cssParser.SetParseMode(Css.CssParseMode.StyleTag);
      Css.CssTokenId cssTokenId;
      do
      {
        cssTokenId = this.cssParser.Parse();
        if (Css.CssTokenId.RuleSet == cssTokenId && this.cssParser.Token.Selectors.ValidCount != 0 && this.cssParser.Token.Properties.ValidCount != 0)
        {
          bool flag1 = false;
          bool flag2 = false;
          this.cssParser.Token.Selectors.Rewind();
          foreach (Css.CssSelector cssSelector in this.cssParser.Token.Selectors)
          {
            if (cssSelector.IsSimple)
            {
              HtmlFormatConverter.StyleSelector key = new HtmlFormatConverter.StyleSelector();
              bool flag3 = false;
              if (cssSelector.HasClassFragment)
              {
                if (cssSelector.ClassType == Css.CssSelectorClassType.Regular || cssSelector.ClassType == Css.CssSelectorClassType.Hash)
                {
                  string @string = cssSelector.ClassName.GetString(60);
                  if (cssSelector.ClassType == Css.CssSelectorClassType.Regular)
                  {
                    key = new HtmlFormatConverter.StyleSelector(cssSelector.NameId, @string, (string) null);
                    flag3 = true;
                    flag2 = flag2 || @string.Equals("MsoNormal", StringComparison.OrdinalIgnoreCase);
                  }
                  else
                  {
                    key = new HtmlFormatConverter.StyleSelector(cssSelector.NameId, (string) null, @string);
                    flag3 = true;
                  }
                }
              }
              else if (cssSelector.NameId != HtmlNameIndex.Unknown)
              {
                key = new HtmlFormatConverter.StyleSelector(cssSelector.NameId, (string) null, (string) null);
                flag3 = true;
              }
              if (flag3 && (flag1 || this.styleHandleIndexCount < 128))
              {
                if (!flag1)
                {
                  if (this.styleHandleIndex == null)
                    this.styleHandleIndex = new int[32];
                  else if (this.styleHandleIndexCount == this.styleHandleIndex.Length)
                  {
                    int[] numArray = new int[this.styleHandleIndexCount * 2];
                    Array.Copy((Array) this.styleHandleIndex, 0, (Array) numArray, 0, this.styleHandleIndexCount);
                    this.styleHandleIndex = numArray;
                  }
                  ++this.styleHandleIndexCount;
                  flag1 = true;
                }
                if (this.styleDictionary == null)
                  this.styleDictionary = new Dictionary<HtmlFormatConverter.StyleSelector, int>((IEqualityComparer<HtmlFormatConverter.StyleSelector>) new HtmlFormatConverter.StyleSelectorComparer());
                if (!this.styleDictionary.ContainsKey(key))
                  this.styleDictionary.Add(key, this.styleHandleIndexCount - 1);
                else
                  this.styleDictionary[key] = this.styleHandleIndexCount - 1;
              }
            }
          }
          if (flag1)
          {
            Format.StyleBuilder builder;
            Format.FormatStyle formatStyle = this.RegisterStyle(false, out builder);
            this.cssParser.Token.Properties.Rewind();
            foreach (Css.CssProperty prop in this.cssParser.Token.Properties)
            {
              if (prop.NameId != Css.CssNameIndex.Unknown)
              {
                if (HtmlConverterData.cssPropertyInstructions[(int) prop.NameId].ParsingMethod != null)
                {
                  Format.PropertyValue propertyValue;
                  if (!prop.Value.IsEmpty && prop.Value.IsContiguous)
                  {
                    propertyValue = HtmlConverterData.cssPropertyInstructions[(int) prop.NameId].ParsingMethod(prop.Value.ContiguousBufferString, (Format.FormatConverter) this);
                  }
                  else
                  {
                    this.scratch.AppendCssPropertyValue(prop, 4096);
                    propertyValue = HtmlConverterData.cssPropertyInstructions[(int) prop.NameId].ParsingMethod(this.scratch.BufferString, (Format.FormatConverter) this);
                    this.scratch.Reset();
                  }
                  if (!propertyValue.IsNull)
                    builder.SetProperty(HtmlConverterData.cssPropertyInstructions[(int) prop.NameId].PropertyId, propertyValue);
                }
                else if (HtmlConverterData.cssPropertyInstructions[(int) prop.NameId].MultiPropertyParsingMethod != null)
                {
                  if (this.parsedProperties == null)
                    this.parsedProperties = new Format.Property[12];
                  int parsedPropertiesCount;
                  if (!prop.Value.IsEmpty && prop.Value.IsContiguous)
                  {
                    HtmlConverterData.cssPropertyInstructions[(int) prop.NameId].MultiPropertyParsingMethod(prop.Value.ContiguousBufferString, (Format.FormatConverter) this, HtmlConverterData.cssPropertyInstructions[(int) prop.NameId].PropertyId, this.parsedProperties, out parsedPropertiesCount);
                  }
                  else
                  {
                    this.scratch.AppendCssPropertyValue(prop, 4096);
                    HtmlConverterData.cssPropertyInstructions[(int) prop.NameId].MultiPropertyParsingMethod(this.scratch.BufferString, (Format.FormatConverter) this, HtmlConverterData.cssPropertyInstructions[(int) prop.NameId].PropertyId, this.parsedProperties, out parsedPropertiesCount);
                    this.scratch.Reset();
                  }
                  if (parsedPropertiesCount != 0)
                    builder.SetProperties(this.parsedProperties, parsedPropertiesCount);
                }
              }
            }
            builder.Flush();
            if (formatStyle.IsEmpty)
            {
              formatStyle.Release();
              this.styleHandleIndex[this.styleHandleIndexCount - 1] = 0;
            }
            else
              this.styleHandleIndex[this.styleHandleIndexCount - 1] = formatStyle.Handle;
          }
        }
      }
      while (Css.CssTokenId.EndOfFile != cssTokenId);
      this.cssParserInput.Reset();
      this.cssParser.Reset();
    }

    private void FindAndApplyStyle(HtmlFormatConverter.StyleSelector styleSelector)
    {
      int index;
      if (this.styleDictionary == null || !this.styleDictionary.TryGetValue(styleSelector, out index) || this.styleHandleIndex[index] == 0)
        return;
      this.Last.SetStyleReference(9 - styleSelector.Specificity, this.styleHandleIndex[index]);
    }

    private struct StyleSelector
    {
      public HtmlNameIndex NameId;
      public string Cls;
      public string Id;

      public int Specificity
      {
        get
        {
          return (this.Id == null ? 0 : 4) + (this.Cls == null ? 0 : 2) + (this.NameId == HtmlNameIndex.Unknown ? 0 : 1);
        }
      }

      public StyleSelector(HtmlNameIndex nameId, string cls, string id)
      {
        this.NameId = nameId;
        this.Cls = cls == null || cls.Length == 0 || cls.Equals("*") ? (string) null : cls;
        this.Id = string.IsNullOrEmpty(id) || id.Equals("*") ? (string) null : id;
      }
    }

    private class StyleSelectorComparer : IEqualityComparer<HtmlFormatConverter.StyleSelector>, IComparer<HtmlFormatConverter.StyleSelector>
    {
      public int Compare(HtmlFormatConverter.StyleSelector x, HtmlFormatConverter.StyleSelector y)
      {
        int specificity = x.Specificity;
        if (specificity != y.Specificity)
          return specificity - y.Specificity;
        switch (specificity)
        {
          case 1:
            return (int) (x.NameId - y.NameId);
          case 2:
            return string.Compare(x.Cls, y.Cls, StringComparison.OrdinalIgnoreCase);
          case 3:
            int num1;
            if ((num1 = string.Compare(x.Cls, y.Cls, StringComparison.OrdinalIgnoreCase)) == 0)
              return (int) (x.NameId - y.NameId);
            return num1;
          case 4:
            return string.Compare(x.Id, y.Id, StringComparison.OrdinalIgnoreCase);
          case 5:
            int num2;
            if ((num2 = string.Compare(x.Id, y.Id, StringComparison.OrdinalIgnoreCase)) == 0)
              return (int) (x.NameId - y.NameId);
            return num2;
          case 6:
            int num3;
            if ((num3 = string.Compare(x.Id, y.Id, StringComparison.OrdinalIgnoreCase)) == 0)
              return string.Compare(x.Cls, y.Cls, StringComparison.OrdinalIgnoreCase);
            return num3;
          case 7:
            int num4;
            if ((num4 = string.Compare(x.Id, y.Id, StringComparison.OrdinalIgnoreCase)) != 0)
              return num4;
            int num5;
            if ((num5 = string.Compare(x.Cls, y.Cls, StringComparison.OrdinalIgnoreCase)) == 0)
              return (int) (x.NameId - y.NameId);
            return num5;
          default:
            return 0;
        }
      }

      public bool Equals(HtmlFormatConverter.StyleSelector x, HtmlFormatConverter.StyleSelector y)
      {
        return this.Compare(x, y) == 0;
      }

      public int GetHashCode(HtmlFormatConverter.StyleSelector x)
      {
        int specificity = x.Specificity;
        return (int) (x.NameId ^ ((specificity & 4) == 0 ? HtmlNameIndex._NOTANAME : (HtmlNameIndex) x.Id.GetHashCode()) ^ ((specificity & 2) == 0 ? HtmlNameIndex._NOTANAME : (HtmlNameIndex) x.Cls.GetHashCode()));
      }
    }
  }
}
