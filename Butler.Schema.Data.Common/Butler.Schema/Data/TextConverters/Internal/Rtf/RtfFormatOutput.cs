// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Rtf.RtfFormatOutput
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters.Internal.Rtf
{
  internal class RtfFormatOutput : Format.FormatOutput, IRestartable
  {
    private static readonly string[] FontSuffixes = new string[6]
    {
      " CE",
      " Cyr",
      " Greek",
      " Tur",
      " Baltic",
      " UPC"
    };
    private static string[] symbolFonts = new string[37]
    {
      "Symbol",
      "Wingdings",
      "Wingdings 2",
      "Wingdings 3",
      "Webdings",
      "Marlett",
      "Map Symbols",
      "ZapfDingbats",
      "Monotype Sorts",
      "MT Extra",
      "Bookshelf Symbol 1",
      "Bookshelf Symbol 2",
      "Bookshelf Symbol 3",
      "Sign Language",
      "Shapes1",
      "Shapes2",
      "Bullets1",
      "Bullets2",
      "Bullets3",
      "Common Bullets",
      "Geographic Symbols",
      "Carta",
      "MICR",
      "Musical Symbols",
      "Sonata",
      "Almanac MT",
      "Bon Apetit MT",
      "Directions MT",
      "Holidays MT",
      "Keystrokes MT",
      "MS Outlook",
      "Parties MT",
      "Signs MT",
      "Sports Three MT",
      "Sports Two MT",
      "Transport MT",
      "Vacation MT"
    };
    private Dictionary<string, int> fontNameDictionary = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private Dictionary<uint, int> colorDictionary = new Dictionary<uint, int>();
    private RtfFormatOutput.OutputFont[] fonts = new RtfFormatOutput.OutputFont[100];
    private Format.RGBT[] colors = new Format.RGBT[100];
    private bool startedBlock;
    private IResultsFeedback resultFeedback;
    private bool restartable;
    private RtfOutput output;
    private int tableLevel;
    private RtfFormatOutput.ListLevel[] listStack;
    private int listLevel;
    private int textPosition;
    private int fontsTop;
    private int firstFontHandle;
    private int symbolFont;
    private int colorsTop;
    private int firstColorHandle;
    private Encoding preferredEncoding;
    private int delayedBottomMargin;
    private ImageRenderingCallbackInternal imageRenderingCallback;
    private bool htmlRtfOn;
    private bool htmlRtfOnOff;
    private int outOfOrderNesting;
    private uint runningPosition;

    public override bool CanAcceptMoreOutput => this.output.CanAcceptMoreOutput;

      public RtfFormatOutput(Stream destination, bool push, bool restartable, bool testBoundaryConditions, IResultsFeedback resultFeedback, ImageRenderingCallbackInternal imageRenderingCallback, Stream formatTraceStream, Stream formatOutputTraceStream, Encoding preferredEncoding)
      : base(formatOutputTraceStream)
    {
      this.output = new RtfOutput(destination, push, restartable);
      this.resultFeedback = resultFeedback;
      this.restartable = restartable;
      this.imageRenderingCallback = imageRenderingCallback;
      this.preferredEncoding = preferredEncoding;
    }

    public RtfFormatOutput(RtfOutput output, Stream formatTraceStream, Stream formatOutputTraceStream)
      : base(formatOutputTraceStream)
    {
      this.output = output;
    }

    bool IRestartable.CanRestart()
    {
      if (this.restartable)
        return ((IRestartable) this.output).CanRestart();
      return false;
    }

    void IRestartable.Restart()
    {
      ((IRestartable) this.output).Restart();
      this.Restart();
      this.tableLevel = 0;
      this.listLevel = 0;
      this.startedBlock = false;
      this.fontNameDictionary.Clear();
      this.colorDictionary.Clear();
      this.fontsTop = 0;
      this.delayedBottomMargin = 0;
      this.restartable = false;
    }

    void IRestartable.DisableRestart()
    {
      ((IRestartable) this.output).DisableRestart();
      this.restartable = false;
    }

    public override void Initialize(Format.FormatStore store, Format.SourceFormat sourceFormat, string comment)
    {
      base.Initialize(store, sourceFormat, comment);
      store.InitializeCodepageDetector();
    }

    public void OutputColors(int nextColorIndex)
    {
      this.firstColorHandle = nextColorIndex;
      this.AddColor(new Format.PropertyValue(new Format.RGBT(192U, 192U, 192U)));
      this.BuildColorsTable(this.FormatStore.RootNode);
      this.OutputColorsTableEntries();
    }

    public void OutputFonts(int firstAvailableFontHandle)
    {
      int bestWindowsCodePage = this.FormatStore.GetBestWindowsCodePage();
      Encoding encoding;
      if (!Globalization.Charset.TryGetEncoding(bestWindowsCodePage, out encoding))
        encoding = Encoding.UTF8;
      this.output.SetEncoding(encoding);
      int charset = RtfSupport.CharSetFromCodePage((ushort) bestWindowsCodePage);
      this.firstFontHandle = firstAvailableFontHandle;
      int num;
      if (!this.fontNameDictionary.TryGetValue("Symbol", out num))
      {
        this.fonts[this.fontsTop].Name = "Symbol";
        this.fonts[this.fontsTop].SymbolFont = true;
        this.fontNameDictionary.Add(this.fonts[this.fontsTop].Name, this.fontsTop);
        this.symbolFont = this.fontsTop;
        ++this.fontsTop;
      }
      else
        this.symbolFont = num;
      this.BuildFontsTable(this.FormatStore.RootNode);
      this.OutputFontsTableEntries(charset);
    }

    public void WriteText(string buffer)
    {
      this.WriteText(buffer, 0, buffer.Length);
    }

    public void WriteText(string buffer, int offset, int count)
    {
      this.HtmlRtfOffReally();
      this.output.WriteText(buffer, offset, count);
    }

    public void WriteText(char[] buffer, int offset, int count)
    {
      this.HtmlRtfOffReally();
      this.output.WriteText(buffer, offset, count);
    }

    public void WriteEncapsulatedMarkupText(char[] buffer, int offset, int count)
    {
      this.HtmlRtfOffReally();
      this.output.WriteEncapsulatedMarkupText(buffer, offset, count);
    }

    public void WriteDoubleEscapedText(string buffer)
    {
      this.WriteDoubleEscapedText(buffer, 0, buffer.Length);
    }

    public void WriteDoubleEscapedText(string buffer, int offset, int count)
    {
      this.HtmlRtfOffReally();
      this.output.WriteDoubleEscapedText(buffer, offset, count);
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
      int codePage = this.preferredEncoding == null ? this.FormatStore.GetBestWindowsCodePage() : this.FormatStore.GetBestWindowsCodePage(Globalization.CodePageMap.GetCodePage(this.preferredEncoding));
      Encoding encoding;
      if (!Globalization.Charset.TryGetEncoding(codePage, out encoding))
        encoding = Encoding.UTF8;
      this.output.SetEncoding(encoding);
      if (this.resultFeedback != null)
        this.resultFeedback.Set(ConfigParameter.OutputEncoding, (object) encoding);
      int charset = RtfSupport.CharSetFromCodePage((ushort) codePage);
      this.fonts[this.fontsTop].Name = "Times New Roman";
      this.fontNameDictionary.Add(this.fonts[this.fontsTop].Name, this.fontsTop);
      int num = this.fontsTop;
      ++this.fontsTop;
      this.fonts[this.fontsTop].Name = "Symbol";
      this.fonts[this.fontsTop].SymbolFont = true;
      this.fontNameDictionary.Add(this.fonts[this.fontsTop].Name, this.fontsTop);
      this.symbolFont = this.fontsTop;
      ++this.fontsTop;
      this.firstColorHandle = 1;
      this.AddColor(new Format.PropertyValue(new Format.RGBT(192U, 192U, 192U)));
      this.BuildTables(this.CurrentNode);
      this.WriteControlText("{\\rtf1\\ansi", true);
      this.WriteControlText("\\fbidis", true);
      this.WriteKeyword("\\ansicpg", codePage);
      this.WriteKeyword("\\deff", num);
      if (this.SourceFormat == Format.SourceFormat.Text)
        this.WriteControlText("\\deftab720\\fromtext", true);
      else if (this.SourceFormat == Format.SourceFormat.HtmlEncapsulateMarkup)
        this.WriteControlText("\\fromhtml1", true);
      this.WriteControlText("{\\fonttbl", true);
      this.OutputFontsTableEntries(charset);
      this.WriteControlText("}\r\n", false);
      this.output.RtfLineLength = 0;
      this.WriteControlText("{\\colortbl;", false);
      this.OutputColorsTableEntries();
      this.WriteControlText("}\r\n", false);
      this.output.RtfLineLength = 0;
      this.WriteControlText("{\\*\\generator Microsoft Exchange Server;}\r\n", false);
      this.output.RtfLineLength = 0;
      if (this.Comment != null)
      {
        this.WriteControlText("{\\*\\formatConverter ", false);
        this.WriteControlText(this.Comment, false);
        this.WriteControlText(";}\r\n", false);
        this.output.RtfLineLength = 0;
      }
      if (!this.CurrentNode.FirstChild.IsNull && this.CurrentNode.FirstChild == this.CurrentNode.LastChild && (this.CurrentNode.LastChild.NodeType & Format.FormatContainerType.BlockFlag) != Format.FormatContainerType.Null)
      {
        Format.PropertyValue property = this.CurrentNode.FirstChild.GetProperty(Format.PropertyId.BackColor);
        if (property.IsColor)
        {
          this.WriteControlText("{\\*\\background {\\shp{\\*\\shpinst{\\sp{\\sn fillColor}{\\sv ", false);
          this.WriteControlText(RtfSupport.RGB((int) property.Color.Red, (int) property.Color.Green, (int) property.Color.Blue).ToString(), false);
          this.WriteControlText("}}}}}", false);
        }
      }
      this.WriteControlText("\\viewkind5\\viewscale100\r\n", false);
      this.output.RtfLineLength = 0;
      this.HtmlRtfOn();
      this.WriteControlText("{\\*\\bkmkstart BM_BEGIN}", false);
      this.HtmlRtfOff();
      if (this.SourceFormat == Format.SourceFormat.HtmlEncapsulateMarkup)
      {
        this.WriteControlText("{\\*\\htmltag64}", false);
        this.OutputNodeStartEncapsulatedMarkup();
      }
      return true;
    }

    protected override void EndDocument()
    {
      this.OutputNodeEndEncapsulatedMarkup();
      this.WriteControlText("}\r\n", false);
      this.output.RtfLineLength = 0;
      this.output.Flush();
    }

    protected override bool StartTable()
    {
      this.OutputNodeStartEncapsulatedMarkup();
      if (this.startedBlock)
        this.ReallyEndBlock();
      ++this.tableLevel;
      return true;
    }

    protected override void EndTable()
    {
      --this.tableLevel;
      this.OutputNodeEndEncapsulatedMarkup();
    }

    protected override bool StartTableRow()
    {
      this.OutputNodeStartEncapsulatedMarkup();
      if (this.tableLevel == 1)
      {
        this.HtmlRtfOn();
        this.OutputRowProps();
        this.HtmlRtfOff();
      }
      return true;
    }

    protected override void EndTableRow()
    {
      this.HtmlRtfOn();
      this.OutputTableLevel();
      if (this.tableLevel > 1)
      {
        this.WriteControlText("{\\*\\nesttableprops", true);
        this.OutputRowProps();
        this.WriteControlText("\\nestrow}{\\nonesttables\\par}\r\n", false);
        this.textPosition += 2;
        this.output.RtfLineLength = 0;
      }
      else
      {
        this.WriteControlText("\\row\r\n", false);
        this.textPosition += 2;
        this.output.RtfLineLength = 0;
      }
      this.HtmlRtfOff();
      this.OutputNodeEndEncapsulatedMarkup();
    }

    protected override bool StartTableCell()
    {
      this.OutputNodeStartEncapsulatedMarkup();
      this.textPosition += 5;
      return true;
    }

    protected override void EndTableCell()
    {
      this.HtmlRtfOn();
      if (!this.startedBlock)
      {
        this.OutputTableLevel();
        this.OutputBlockProps();
      }
      if (this.tableLevel > 1)
        this.WriteControlText("\\nestcell{\\nonesttables\\tab}", false);
      else
        this.WriteControlText("\\cell", true);
      ++this.textPosition;
      this.startedBlock = false;
      this.HtmlRtfOff();
      this.OutputNodeEndEncapsulatedMarkup();
    }

    protected override bool StartTableCaption()
    {
      if (this.CurrentNode.Parent.NodeType == Format.FormatContainerType.Table)
        --this.tableLevel;
      return this.StartBlockContainer();
    }

    protected override void EndTableCaption()
    {
      this.EndBlockContainer();
      if (this.startedBlock)
        this.ReallyEndBlock();
      if (this.CurrentNode.Parent.NodeType != Format.FormatContainerType.Table)
        return;
      ++this.tableLevel;
    }

    protected override bool StartList()
    {
      this.OutputNodeStartEncapsulatedMarkup();
      if (this.startedBlock)
        this.ReallyEndBlock();
      if (this.listStack == null)
        this.listStack = new RtfFormatOutput.ListLevel[8];
      else if (this.listStack.Length == this.listLevel)
      {
        RtfFormatOutput.ListLevel[] listLevelArray = new RtfFormatOutput.ListLevel[this.listStack.Length * 2];
        Array.Copy((Array) this.listStack, 0, (Array) listLevelArray, 0, this.listLevel);
        this.listStack = listLevelArray;
      }
      if (this.listLevel == -1)
        this.listLevel = 0;
      this.listStack[this.listLevel].Reset();
      Format.PropertyValue effectiveProperty = this.GetEffectiveProperty(Format.PropertyId.ListStyle);
      if (!effectiveProperty.IsNull)
        this.listStack[this.listLevel].ListType = (RtfNumbering) effectiveProperty.Enum;
      effectiveProperty = this.GetEffectiveProperty(Format.PropertyId.ListStart);
      if (!effectiveProperty.IsNull)
        this.listStack[this.listLevel].NextIndex = (short) effectiveProperty.Integer;
      ++this.listLevel;
      return true;
    }

    protected override void EndList()
    {
      --this.listLevel;
      this.OutputNodeEndEncapsulatedMarkup();
    }

    protected override bool StartListItem()
    {
      this.OutputNodeStartEncapsulatedMarkup();
      if (this.startedBlock)
        this.ReallyEndBlock();
      if (this.listLevel == 0)
        this.listLevel = -1;
      return true;
    }

    protected override void EndListItem()
    {
      if (this.CurrentNode.FirstChild.IsNull)
        this.ReallyStartAppropriateBlock();
      if (this.startedBlock)
        this.ReallyEndBlock();
      if (this.listLevel > 0)
        ++this.listStack[this.listLevel - 1].NextIndex;
      else
        this.listLevel = 0;
      this.OutputNodeEndEncapsulatedMarkup();
    }

    protected override bool StartHyperLink()
    {
      this.OutputNodeStartEncapsulatedMarkup();
      this.HtmlRtfOn();
      if (!this.startedBlock)
        this.ReallyStartAppropriateBlock();
      Format.PropertyValue effectiveProperty = this.GetEffectiveProperty(Format.PropertyId.HyperlinkUrl);
      if (!effectiveProperty.IsNull)
      {
        this.WriteControlText("{\\field{\\*\\fldinst HYPERLINK ", false);
        string buffer = this.FormatStore.GetStringValue(effectiveProperty).GetString();
        bool flag = false;
        if ((int) buffer[0] == 35)
        {
          this.WriteControlText("\\\\l ", false);
          buffer = buffer.Length <= 1 ? string.Empty : (char.IsLetter(buffer[1]) ? buffer.Substring(1) : "BM_" + buffer.Substring(1));
          flag = true;
        }
        this.WriteControlText("\"", false);
        if (buffer.Length != 0)
          this.WriteDoubleEscapedText(buffer);
        else if (flag)
          this.WriteControlText("BM_BEGIN", false);
        else
          this.WriteControlText("http://", false);
        this.WriteControlText("\" }{\\fldrslt", true);
      }
      this.HtmlRtfOff();
      return true;
    }

    protected override void EndHyperLink()
    {
      this.HtmlRtfOn();
      Format.PropertyValue effectiveProperty = this.GetEffectiveProperty(Format.PropertyId.HyperlinkUrl);
      if (!effectiveProperty.IsNull)
      {
        this.WriteControlText("}}", false);
        this.textPosition += this.FormatStore.GetStringValue(effectiveProperty).GetString().Length + 2;
      }
      Format.FormatNode nextSibling = this.CurrentNode.NextSibling;
      if (!nextSibling.IsNull && (nextSibling.NodeType == Format.FormatContainerType.Block || nextSibling.NodeType == Format.FormatContainerType.List || (nextSibling.NodeType == Format.FormatContainerType.Table || nextSibling.NodeType == Format.FormatContainerType.HorizontalLine)))
        this.EndBlockContainer();
      this.HtmlRtfOff();
      this.OutputNodeEndEncapsulatedMarkup();
    }

    protected override bool StartBookmark()
    {
      this.OutputNodeStartEncapsulatedMarkup();
      this.HtmlRtfOn();
      Format.PropertyValue effectiveProperty = this.GetEffectiveProperty(Format.PropertyId.BookmarkName);
      if (!effectiveProperty.IsNull)
      {
        string @string = this.FormatStore.GetStringValue(effectiveProperty).GetString();
        if (@string != "BM_BEGIN")
        {
          this.WriteControlText("{\\*\\bkmkstart", true);
          if (!char.IsLetter(@string[0]))
            this.WriteText("BM_");
          this.WriteText(@string);
          this.WriteControlText("}", false);
        }
      }
      this.HtmlRtfOff();
      return true;
    }

    protected override void EndBookmark()
    {
      this.HtmlRtfOn();
      Format.PropertyValue effectiveProperty = this.GetEffectiveProperty(Format.PropertyId.BookmarkName);
      if (!effectiveProperty.IsNull)
      {
        string @string = this.FormatStore.GetStringValue(effectiveProperty).GetString();
        if (@string != "BM_BEGIN")
        {
          this.WriteControlText("{\\*\\bkmkend", true);
          this.WriteText(@string);
          this.WriteControlText("}", false);
        }
      }
      this.HtmlRtfOff();
      this.OutputNodeEndEncapsulatedMarkup();
    }

    protected override void StartEndImage()
    {
      this.OutputNodeStartEncapsulatedMarkup();
      this.HtmlRtfOn();
      if (!this.startedBlock)
        this.ReallyStartAppropriateBlock();
      Format.PropertyValue propertyValue1 = this.GetEffectiveProperty(Format.PropertyId.Width);
      Format.PropertyValue propertyValue2 = this.GetEffectiveProperty(Format.PropertyId.Height);
      Format.PropertyValue effectiveProperty1 = this.GetEffectiveProperty(Format.PropertyId.ImageUrl);
      Format.PropertyValue effectiveProperty2 = this.GetEffectiveProperty(Format.PropertyId.ImageAltText);
      string str = (string) null;
      if (!effectiveProperty1.IsNull)
        str = this.FormatStore.GetStringValue(effectiveProperty1).GetString();
      string buffer = (string) null;
      if (!effectiveProperty2.IsNull)
        buffer = this.FormatStore.GetStringValue(effectiveProperty2).GetString();
      bool flag = false;
      if (this.imageRenderingCallback != null && str != null)
        flag = this.imageRenderingCallback(str, this.textPosition);
      if (flag)
      {
        this.WriteControlText("\\objattph  ", false);
        ++this.textPosition;
      }
      else
      {
        if (propertyValue1.IsNull || !propertyValue1.IsAbsLength && !propertyValue1.IsPixels)
          propertyValue1 = Format.PropertyValue.Null;
        else if (propertyValue1.TwipsInteger == 0)
          propertyValue1.Set(Format.LengthUnits.Pixels, 1f);
        if (propertyValue2.IsNull || !propertyValue2.IsAbsLength && !propertyValue2.IsPixels)
          propertyValue2 = Format.PropertyValue.Null;
        else if (propertyValue2.TwipsInteger == 0)
          propertyValue2.Set(Format.LengthUnits.Pixels, 1f);
        if (str != null)
        {
          this.WriteControlText("{\\field{\\*\\fldinst INCLUDEPICTURE \"", false);
          this.WriteDoubleEscapedText(str);
          this.WriteControlText("\" \\\\d \\\\* MERGEFORMAT}{\\fldrslt", true);
        }
        this.WriteControlText("{\\pict{\\*\\picprop{\\sp{\\sn fillColor}{\\sv 14286846}}{\\sp{\\sn fillOpacity}{\\sv 16384}}{\\sp{\\sn fFilled}{\\sv 1}}", false);
        if (buffer != null)
        {
          this.WriteControlText("{\\sp{\\sn wzDescription}{\\sv ", false);
          this.WriteText(buffer);
          this.WriteControlText("}}", false);
        }
        this.WriteControlText("}", false);
        this.WriteControlText("\\brdrt\\brdrs\\brdrw10", true);
        this.WriteKeyword("\\brdrcf", this.firstColorHandle);
        this.WriteControlText("\\brdrl\\brdrs\\brdrw10", true);
        this.WriteKeyword("\\brdrcf", this.firstColorHandle);
        this.WriteControlText("\\brdrb\\brdrs\\brdrw10", true);
        this.WriteKeyword("\\brdrcf", this.firstColorHandle);
        this.WriteControlText("\\brdrr\\brdrs\\brdrw10", true);
        this.WriteKeyword("\\brdrcf", this.firstColorHandle);
        if (!propertyValue1.IsNull)
          this.WriteKeyword("\\picwgoal", propertyValue1.TwipsInteger);
        if (!propertyValue2.IsNull)
          this.WriteKeyword("\\pichgoal", propertyValue2.TwipsInteger);
        this.WriteControlText("\\wmetafile8 0100090000032100000000000500000000000400000003010800050000000b0200000000050000000c0202000200030000001e00040000002701ffff030000000000}", false);
        if (str != null)
          this.WriteControlText("}}", false);
        if ((propertyValue2.IsNull || propertyValue2.PixelsInteger >= 8) && (propertyValue1.IsNull || propertyValue1.PixelsInteger >= 8) && str != null)
          this.textPosition += str.Length + 2;
      }
      Format.FormatNode nextSibling = this.CurrentNode.NextSibling;
      if (!nextSibling.IsNull && (nextSibling.NodeType == Format.FormatContainerType.Block || nextSibling.NodeType == Format.FormatContainerType.List || (nextSibling.NodeType == Format.FormatContainerType.Table || nextSibling.NodeType == Format.FormatContainerType.HorizontalLine)))
        this.EndBlockContainer();
      this.HtmlRtfOff();
      this.OutputNodeEndEncapsulatedMarkup();
    }

    protected override void StartEndHorizontalLine()
    {
      this.OutputNodeStartEncapsulatedMarkup();
      this.HtmlRtfOn();
      if (this.startedBlock)
      {
        this.WriteControlText("\\par\r\n", false);
        this.textPosition += 2;
        this.output.RtfLineLength = 0;
        this.startedBlock = false;
      }
      this.OutputTableLevel();
      this.WriteControlText("\\plain", true);
      this.WriteControlText("{\\f0\\qc\\qd\\cf1\\ulth\\~ ________________________________ \\~\\par}\r\n", false);
      this.textPosition += 36;
      this.output.RtfLineLength = 0;
      this.HtmlRtfOff();
      this.OutputNodeEndEncapsulatedMarkup();
    }

    protected override void StartEndArea()
    {
    }

    protected override bool StartOption()
    {
      return true;
    }

    protected override bool StartText()
    {
      this.OutputNodeStartEncapsulatedMarkup();
      this.HtmlRtfOn();
      if (!this.startedBlock)
        this.ReallyStartAppropriateBlock();
      this.WriteControlText("{", false);
      this.OutputTextProps();
      this.HtmlRtfOff();
      return true;
    }

    protected override bool ContinueText(uint beginTextPosition, uint endTextPosition)
    {
      this.OutputTextRuns(beginTextPosition, endTextPosition, false);
      return true;
    }

    protected override void EndText()
    {
      this.HtmlRtfOn();
      this.WriteControlText("}", false);
      this.HtmlRtfOff();
      this.OutputNodeEndEncapsulatedMarkup();
    }

    protected override bool StartBlockContainer()
    {
      this.OutputNodeStartEncapsulatedMarkup();
      if (this.startedBlock)
        this.ReallyEndBlock();
      if (this.CurrentNode.FirstChild.IsNull || (this.CurrentNode.FirstChild.NodeType & Format.FormatContainerType.BlockFlag) == Format.FormatContainerType.Null)
        this.ReallyStartAppropriateBlock();
      return true;
    }

    protected override void EndBlockContainer()
    {
      if (this.startedBlock && this.CurrentNode != this.CurrentNode.Parent.LastChild)
        this.ReallyEndBlock();
      this.OutputNodeEndEncapsulatedMarkup();
    }

    protected override void Dispose(bool disposing)
    {
      if (this.output != null && this.output != null)
        ((IDisposable) this.output).Dispose();
      this.fonts = (RtfFormatOutput.OutputFont[]) null;
      this.fontNameDictionary = (Dictionary<string, int>) null;
      this.colors = (Format.RGBT[]) null;
      this.colorDictionary = (Dictionary<uint, int>) null;
      this.output = (RtfOutput) null;
      base.Dispose(disposing);
    }

    private static bool IsTaggedFontName(string name, out int lengthWithoutTag)
    {
      lengthWithoutTag = name.Length;
      for (int index = 0; index < RtfFormatOutput.FontSuffixes.Length; ++index)
      {
        if (RtfFormatOutput.FontSuffixes[index].Length < name.Length && name.EndsWith(RtfFormatOutput.FontSuffixes[index], StringComparison.OrdinalIgnoreCase))
        {
          lengthWithoutTag -= RtfFormatOutput.FontSuffixes[index].Length;
          return true;
        }
      }
      return false;
    }

    private static int ConvertLengthToHalfPoints(Format.PropertyValue pv)
    {
      switch (pv.Type)
      {
        case Format.PropertyType.Percentage:
          return 0;
        case Format.PropertyType.AbsLength:
          return pv.BaseUnits / 8 / 10;
        case Format.PropertyType.RelLength:
          return 0;
        case Format.PropertyType.Pixels:
          return pv.Value / 8 / 10;
        case Format.PropertyType.Ems:
          return pv.Value / 8 / 10;
        case Format.PropertyType.Exs:
          return pv.Value / 8 / 10;
        case Format.PropertyType.HtmlFontUnits:
          return Format.PropertyValue.ConvertHtmlFontUnitsToTwips(pv.HtmlFontUnits) / 10;
        case Format.PropertyType.RelHtmlFontUnits:
          return 0;
        default:
          return 0;
      }
    }

    private void HtmlRtfOn()
    {
      if (this.SourceFormat != Format.SourceFormat.HtmlEncapsulateMarkup)
        return;
      if (!this.htmlRtfOn)
      {
        this.htmlRtfOn = true;
        this.WriteControlText("\\htmlrtf", true);
      }
      else
      {
        if (!this.htmlRtfOnOff)
          return;
        this.htmlRtfOnOff = false;
      }
    }

    private void HtmlRtfOff()
    {
      if (!this.htmlRtfOn)
        return;
      this.htmlRtfOnOff = true;
    }

    private void HtmlRtfOffReally()
    {
      if (!this.htmlRtfOnOff || this.outOfOrderNesting != 0)
        return;
      this.htmlRtfOnOff = false;
      this.htmlRtfOn = false;
      this.WriteControlText("\\htmlrtf0", true);
    }

    private void OutputTextRuns(uint start, uint end, bool encapsulatedMarkup)
    {
      if ((int) start == (int) end)
        return;
      Format.TextRun textRun = this.FormatStore.GetTextRun(start);
      do
      {
        int effectiveLength = textRun.EffectiveLength;
        Format.TextRunType type = textRun.Type;
        char[] buffer;
        int offset;
        int count;
        if ((uint) type <= 16384U)
        {
          if (type != Format.TextRunType.Markup)
          {
            if (type != Format.TextRunType.NonSpace)
            {
              if (type == Format.TextRunType.NbSp)
              {
                this.textPosition += effectiveLength;
                int num = 0;
                do
                {
                  this.WriteText(" ");
                }
                while (++num < effectiveLength);
              }
            }
            else
            {
              this.textPosition += effectiveLength;
              int start1 = 0;
              do
              {
                textRun.GetChunk(start1, out buffer, out offset, out count);
                this.WriteText(buffer, offset, count);
                start1 += count;
              }
              while (start1 < effectiveLength);
            }
          }
          else if (this.outOfOrderNesting == 0)
          {
            if (!encapsulatedMarkup)
              this.WriteControlText("{\\*\\htmltag0 ", false);
            int start1 = 0;
            do
            {
              textRun.GetChunk(start1, out buffer, out offset, out count);
              this.WriteEncapsulatedMarkupText(buffer, offset, count);
              start1 += count;
            }
            while (start1 < effectiveLength);
            if (!encapsulatedMarkup)
              this.WriteControlText("}", false);
          }
        }
        else if (type != Format.TextRunType.Space)
        {
          if (type != Format.TextRunType.Tabulation)
          {
            if (type == Format.TextRunType.NewLine)
            {
              this.textPosition += effectiveLength;
              if (!encapsulatedMarkup)
              {
                this.HtmlRtfOn();
                int num = 0;
                do
                {
                  this.WriteControlText("\\line\r\n", false);
                }
                while (++num < effectiveLength);
                this.output.RtfLineLength = 0;
                this.HtmlRtfOff();
              }
              if (this.SourceFormat == Format.SourceFormat.HtmlEncapsulateMarkup && this.outOfOrderNesting == 0)
              {
                if (!encapsulatedMarkup)
                  this.WriteControlText("{\\*\\htmltag0", true);
                do
                {
                  this.WriteControlText("\\par", true);
                }
                while (0 < --effectiveLength);
                if (!encapsulatedMarkup)
                  this.WriteControlText("}", false);
              }
            }
          }
          else
          {
            this.textPosition += effectiveLength;
            if (encapsulatedMarkup)
            {
              this.HtmlRtfOn();
              int num = 0;
              do
              {
                this.WriteControlText("\\tab", true);
              }
              while (++num < effectiveLength);
              this.HtmlRtfOff();
            }
            if (this.SourceFormat == Format.SourceFormat.HtmlEncapsulateMarkup && this.outOfOrderNesting == 0)
            {
              if (!encapsulatedMarkup)
                this.WriteControlText("{\\*\\htmltag0 ", false);
              do
              {
                this.WriteText("\t");
              }
              while (0 < --effectiveLength);
              if (!encapsulatedMarkup)
                this.WriteControlText("}", false);
            }
          }
        }
        else
        {
          this.textPosition += effectiveLength;
          do
          {
            this.WriteText(" ");
          }
          while (0 < --effectiveLength);
        }
        textRun.MoveNext();
      }
      while (textRun.Position < end);
    }

    private void OutputNodeStartEncapsulatedMarkup()
    {
      if (this.SourceFormat != Format.SourceFormat.HtmlEncapsulateMarkup)
        return;
      if (!this.CurrentNode.IsInOrder)
      {
        ++this.outOfOrderNesting;
      }
      else
      {
        if (this.CurrentNode.IsText || this.outOfOrderNesting != 0)
          return;
        uint beginTextPosition = this.CurrentNode.BeginTextPosition;
        Format.FormatNode formatNode = this.CurrentNode.FirstChild;
        while (!formatNode.IsNull && !formatNode.IsInOrder)
          formatNode = formatNode.NextSibling;
        uint end = formatNode.IsNull ? this.CurrentNode.EndTextPosition : formatNode.BeginTextPosition;
        if ((int) beginTextPosition == (int) end)
          return;
        this.WriteControlText("{\\*\\htmltag0 ", false);
        this.OutputTextRuns(beginTextPosition, end, true);
        this.WriteControlText("}", false);
        this.runningPosition = end;
      }
    }

    private void OutputNodeEndEncapsulatedMarkup()
    {
      if (this.SourceFormat != Format.SourceFormat.HtmlEncapsulateMarkup)
        return;
      if (this.outOfOrderNesting > 0)
      {
        if (this.CurrentNode.IsInOrder)
          return;
        --this.outOfOrderNesting;
      }
      else
      {
        uint start = this.CurrentNode.EndTextPosition;
        Format.FormatNode nextSibling = this.CurrentNode.NextSibling;
        while (!nextSibling.IsNull && !nextSibling.IsInOrder)
          nextSibling = nextSibling.NextSibling;
        uint end = nextSibling.IsNull ? this.CurrentNode.Parent.EndTextPosition : nextSibling.BeginTextPosition;
        if (start >= end || end < this.runningPosition)
          return;
        if (start < this.runningPosition)
          start = this.runningPosition;
        if ((int) start == (int) end)
          return;
        this.WriteControlText("{\\*\\htmltag0 ", false);
        this.OutputTextRuns(start, end, true);
        this.WriteControlText("}", false);
        this.runningPosition = end;
      }
    }

    private void ReallyStartAppropriateBlock()
    {
      this.OutputTableLevel();
      Format.FormatNode currentNode = this.CurrentNode;
      if (currentNode.NodeType != Format.FormatContainerType.Document)
        this.OutputBlockProps();
      if (this.listLevel != 0)
        this.OutputListProperties();
      this.WriteControlText("\\plain", true);
      if (currentNode.GetProperty(Format.PropertyId.FontFace).IsNull)
        this.WriteControlText("\\f0", true);
      this.startedBlock = true;
    }

    private void ReallyEndBlock()
    {
      this.HtmlRtfOn();
      this.WriteControlText("\\par\r\n", false);
      this.textPosition += 2;
      this.output.RtfLineLength = 0;
      this.startedBlock = false;
      this.HtmlRtfOff();
    }

    private void BuildTables(Format.FormatNode node)
    {
      foreach (Format.FormatNode formatNode in node.Subtree)
      {
        this.AddFont(formatNode.GetProperty(Format.PropertyId.FontFace));
        this.AddColor(formatNode.GetProperty(Format.PropertyId.FontColor));
        this.AddColor(formatNode.GetProperty(Format.PropertyId.BackColor));
        this.AddColor(formatNode.GetProperty(Format.PropertyId.BorderColors));
        this.AddColor(formatNode.GetProperty(Format.PropertyId.RightBorderColor));
        this.AddColor(formatNode.GetProperty(Format.PropertyId.BottomBorderColor));
        this.AddColor(formatNode.GetProperty(Format.PropertyId.LeftBorderColor));
      }
    }

    private void BuildFontsTable(Format.FormatNode node)
    {
      foreach (Format.FormatNode formatNode in node.Subtree)
        this.AddFont(formatNode.GetProperty(Format.PropertyId.FontFace));
    }

    private void BuildColorsTable(Format.FormatNode node)
    {
      foreach (Format.FormatNode formatNode in node.Subtree)
      {
        this.AddFont(formatNode.GetProperty(Format.PropertyId.FontFace));
        this.AddColor(formatNode.GetProperty(Format.PropertyId.FontColor));
        this.AddColor(formatNode.GetProperty(Format.PropertyId.BackColor));
        this.AddColor(formatNode.GetProperty(Format.PropertyId.BorderColors));
        this.AddColor(formatNode.GetProperty(Format.PropertyId.RightBorderColor));
        this.AddColor(formatNode.GetProperty(Format.PropertyId.BottomBorderColor));
        this.AddColor(formatNode.GetProperty(Format.PropertyId.LeftBorderColor));
      }
    }

    private void OutputFontsTableEntries(int charset)
    {
      for (int index = 0; index < this.fontsTop; ++index)
      {
        this.WriteKeyword("{\\f", index + this.firstFontHandle);
        this.WriteControlText("\\fswiss", true);
        if (!this.fonts[index].SymbolFont)
          this.WriteKeyword("\\fcharset", charset);
        else
          this.WriteKeyword("\\fcharset", 2);
        int lengthWithoutTag;
        if (RtfFormatOutput.IsTaggedFontName(this.fonts[index].Name, out lengthWithoutTag))
        {
          this.WriteControlText("{\\fname", true);
          this.WriteText(this.fonts[index].Name);
          this.WriteControlText(";}", false);
          this.WriteText(this.fonts[index].Name, 0, lengthWithoutTag);
          this.WriteControlText(";}", false);
        }
        else
        {
          this.WriteText(this.fonts[index].Name);
          this.WriteControlText(";}", false);
        }
      }
    }

    private void OutputColorsTableEntries()
    {
      for (int index = 0; index < this.colorsTop; ++index)
      {
        this.WriteKeyword("\\red", (int) this.colors[index].Red);
        this.WriteKeyword("\\green", (int) this.colors[index].Green);
        this.WriteKeyword("\\blue", (int) this.colors[index].Blue);
        this.WriteControlText(";", false);
      }
    }

    private void AddColor(Format.PropertyValue pv)
    {
      if (pv.IsEnum)
        pv = Html.HtmlSupport.TranslateSystemColor(pv);
      if (!pv.IsColor || (this.colorDictionary.ContainsKey(pv.Color.RawValue) || this.colorsTop >= this.colors.Length))
        return;
      this.colors[this.colorsTop] = pv.Color;
      this.colorDictionary.Add(pv.Color.RawValue, this.colorsTop);
      ++this.colorsTop;
    }

    private int FindColorHandle(Format.PropertyValue pv)
    {
      if (pv.IsEnum)
        pv = Html.HtmlSupport.TranslateSystemColor(pv);
      int num = 0;
      if (pv.IsColor && !pv.Color.IsTransparent && this.colorDictionary.TryGetValue(pv.Color.RawValue, out num))
        return num + this.firstColorHandle;
      return 0;
    }

    private void AddFont(Format.PropertyValue pv)
    {
      if (pv.IsNull || this.fontsTop >= this.fonts.Length)
        return;
      string key = (string) null;
      if (pv.IsString)
        key = this.FormatStore.GetStringValue(pv).GetString();
      else if (pv.IsMultiValue)
      {
        Format.MultiValue multiValue = this.FormatStore.GetMultiValue(pv);
        if (multiValue.Length > 0)
          key = multiValue.GetStringValue(0).GetString();
      }
      if (key == null)
        return;
      int index;
      if (!this.fontNameDictionary.TryGetValue(key, out index))
      {
        index = this.fontsTop;
        this.fonts[index].Name = key;
        this.fontNameDictionary.Add(key, index);
        foreach (string str in RtfFormatOutput.symbolFonts)
        {
          if (key.Equals(str, StringComparison.OrdinalIgnoreCase))
          {
            this.fonts[index].SymbolFont = true;
            break;
          }
        }
        ++this.fontsTop;
      }
      ++this.fonts[index].Count;
    }

    private int FindFontHandle(Format.PropertyValue pv, out bool symbolFont)
    {
      int index = 0;
      symbolFont = false;
      if (!pv.IsNull)
      {
        string key = (string) null;
        if (pv.IsString)
          key = this.FormatStore.GetStringValue(pv).GetString();
        else if (pv.IsMultiValue)
        {
          Format.MultiValue multiValue = this.FormatStore.GetMultiValue(pv);
          if (multiValue.Length > 0)
            key = multiValue.GetStringValue(0).GetString();
        }
        if (key != null && this.fontNameDictionary.TryGetValue(key, out index))
        {
          symbolFont = this.fonts[index].SymbolFont;
          return index + this.firstFontHandle;
        }
      }
      return 0;
    }

    private void OutputTableLevel()
    {
      this.WriteControlText("\\pard", true);
      if (this.tableLevel <= 0)
        return;
      this.WriteControlText("\\intbl", true);
      if (this.tableLevel <= 1)
        return;
      this.WriteKeyword("\\itap", this.tableLevel);
    }

    private void OutputListProperties()
    {
      bool flag1 = false;
      Format.FormatNode formatNode;
      for (formatNode = this.CurrentNode; formatNode.NodeType != Format.FormatContainerType.Root && formatNode.NodeType != Format.FormatContainerType.ListItem; formatNode = formatNode.Parent)
      {
        if ((formatNode.NodeType & Format.FormatContainerType.BlockFlag) != Format.FormatContainerType.Null && !formatNode.PreviousSibling.IsNull)
          flag1 = true;
      }
      if (formatNode.NodeType != Format.FormatContainerType.Root)
      {
        bool flag2 = formatNode == formatNode.Parent.FirstChild;
        bool flag3 = formatNode == formatNode.Parent.LastChild;
        formatNode = formatNode.Parent;
        if (!formatNode.IsNull)
        {
          if (flag2)
          {
            Format.PropertyValue property = formatNode.GetProperty(Format.PropertyId.Margins);
            if (property.IsAbsRelLength)
              this.WriteKeyword("\\sb", property.TwipsInteger);
          }
          if (flag3)
          {
            Format.PropertyValue property = formatNode.GetProperty(Format.PropertyId.BottomMargin);
            if (property.IsAbsRelLength)
              this.WriteKeyword("\\sa", property.TwipsInteger);
          }
        }
      }
      if (this.listLevel == 1 && this.listStack[this.listLevel - 1].ListType == RtfNumbering.Arabic)
      {
        if (flag1)
        {
          this.WriteControlText("\\pnlvlcont", true);
          this.WriteControlText("\\pnindent360", true);
        }
        else
        {
          this.WriteControlText("{\\pntext", true);
          string buffer = this.listStack[this.listLevel - 1].NextIndex.ToString();
          this.WriteText(buffer);
          this.WriteControlText(". ", false);
          if (buffer.Length == 1)
            this.WriteControlText(" ", false);
          this.WriteControlText("}", false);
          this.WriteControlText("{\\*\\pn", true);
          this.WriteControlText("\\pnlvlbody", true);
          this.WriteControlText("\\pndec", true);
          this.WriteKeyword("\\pnstart", (int) this.listStack[this.listLevel - 1].NextIndex);
          this.WriteControlText("\\pnindent360", true);
          this.WriteControlText("\\pnql", true);
          this.WriteControlText("{\\pntxta.}}", false);
        }
      }
      else if (flag1)
      {
        this.WriteControlText("\\pnlvlcont", true);
        this.WriteControlText("\\pnindent240", true);
      }
      else
      {
        this.WriteControlText("{\\pntext", true);
        this.WriteControlText("*   }", false);
        this.WriteControlText("{\\*\\pn", true);
        this.WriteControlText("\\pnlvlblt", true);
        this.WriteKeyword("\\pnf", this.firstFontHandle + this.symbolFont);
        this.WriteControlText("\\pnindent240", true);
        this.WriteControlText("\\pnql", true);
        this.WriteControlText("{\\pntxtb\\'B7}}", false);
      }
    }

    private void OutputTextProps()
    {
      Format.PropertyValue effectiveProperty1 = this.GetEffectiveProperty(Format.PropertyId.FontFace);
      if (!effectiveProperty1.IsNull)
      {
        bool symbolFont;
        int fontHandle = this.FindFontHandle(effectiveProperty1, out symbolFont);
        if (symbolFont)
        {
          this.WriteKeyword("\\loch\\af", fontHandle);
          this.WriteControlText("\\dbch\\af0\\hich", true);
        }
        this.WriteKeyword("\\f", fontHandle);
      }
      Format.PropertyValue effectiveProperty2 = this.GetEffectiveProperty(Format.PropertyId.RightToLeft);
      if (effectiveProperty2.IsBool)
        this.WriteControlText(effectiveProperty2.Bool ? "\\rtlch" : "\\ltrch", true);
      Format.PropertyValue effectiveProperty3 = this.GetEffectiveProperty(Format.PropertyId.Language);
      if (effectiveProperty3.IsInteger)
        this.WriteKeyword("\\lang", effectiveProperty3.Integer);
      Format.PropertyValue effectiveProperty4 = this.GetEffectiveProperty(Format.PropertyId.FontColor);
      if (effectiveProperty4.IsColor)
        this.WriteKeyword("\\cf", this.FindColorHandle(effectiveProperty4));
      Format.PropertyValue distinctProperty = this.GetDistinctProperty(Format.PropertyId.BackColor);
      if (distinctProperty.IsColor)
        this.WriteKeyword("\\highlight", this.FindColorHandle(distinctProperty));
      Format.PropertyValue effectiveProperty5 = this.GetEffectiveProperty(Format.PropertyId.FontSize);
      if (!effectiveProperty5.IsNull)
      {
        int num = RtfFormatOutput.ConvertLengthToHalfPoints(effectiveProperty5);
        if (num != 0)
          this.WriteKeyword("\\fs", num);
      }
      Format.PropertyValue effectiveProperty6 = this.GetEffectiveProperty(Format.PropertyId.FirstFlag);
      if (effectiveProperty6.IsBool)
        this.WriteControlText(effectiveProperty6.Bool ? "\\b" : "\\b0", true);
      Format.PropertyValue effectiveProperty7 = this.GetEffectiveProperty(Format.PropertyId.Italic);
      if (effectiveProperty7.IsBool)
        this.WriteControlText(effectiveProperty7.Bool ? "\\i" : "\\i0", true);
      Format.PropertyValue effectiveProperty8 = this.GetEffectiveProperty(Format.PropertyId.Underline);
      if (effectiveProperty8.IsBool)
        this.WriteControlText(effectiveProperty8.Bool ? "\\ul" : "\\ul0", true);
      Format.PropertyValue effectiveProperty9 = this.GetEffectiveProperty(Format.PropertyId.Subscript);
      if (effectiveProperty9.IsBool)
        this.WriteControlText(effectiveProperty9.Bool ? "\\sub" : "\\sub0", true);
      Format.PropertyValue effectiveProperty10 = this.GetEffectiveProperty(Format.PropertyId.Superscript);
      if (effectiveProperty10.IsBool)
        this.WriteControlText(effectiveProperty10.Bool ? "\\super" : "\\super0", true);
      Format.PropertyValue effectiveProperty11 = this.GetEffectiveProperty(Format.PropertyId.Strikethrough);
      if (!effectiveProperty11.IsBool)
        return;
      this.WriteControlText(effectiveProperty11.Bool ? "\\strike" : "\\strike0", true);
    }

    private void OutputBlockProps()
    {
      int val1 = 0;
      int val2 = this.delayedBottomMargin;
      this.delayedBottomMargin = 0;
      Format.PropertyValue effectiveProperty1 = this.GetEffectiveProperty(Format.PropertyId.BackColor);
      if (effectiveProperty1.IsColor)
        this.WriteKeyword("\\cbpat", this.FindColorHandle(effectiveProperty1));
      Format.PropertyValue effectiveProperty2 = this.GetEffectiveProperty(Format.PropertyId.TextAlignment);
      if (effectiveProperty2.IsEnum)
      {
        switch (effectiveProperty2.Enum)
        {
          case 1:
            this.WriteControlText("\\qc", true);
            break;
          case 3:
            this.WriteControlText("\\ql", true);
            break;
          case 4:
            this.WriteControlText("\\qr", true);
            break;
          case 6:
            this.WriteControlText("\\qj", true);
            break;
        }
      }
      effectiveProperty2 = this.GetEffectiveProperty(Format.PropertyId.Margins);
      if (effectiveProperty2.IsAbsRelLength && (this.CurrentNode.Parent.IsNull || this.CurrentNode.Parent.FirstChild != this.CurrentNode))
        val1 = effectiveProperty2.TwipsInteger;
      effectiveProperty2 = this.GetEffectiveProperty(Format.PropertyId.BottomMargin);
      if (effectiveProperty2.IsAbsRelLength)
        this.delayedBottomMargin = effectiveProperty2.TwipsInteger;
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      if (this.listLevel != 0)
      {
        if (this.listLevel == 1 && this.listStack[this.listLevel - 1].ListType == RtfNumbering.Arabic)
        {
          num1 = this.listLevel * 600;
          num3 = -360;
        }
        else
        {
          num1 = this.listLevel * 600;
          num3 = -240;
        }
      }
      effectiveProperty2 = this.GetEffectiveProperty(Format.PropertyId.LeftMargin);
      if (effectiveProperty2.IsAbsRelLength)
        num1 += effectiveProperty2.TwipsInteger;
      effectiveProperty2 = this.GetEffectiveProperty(Format.PropertyId.RightMargin);
      if (effectiveProperty2.IsAbsRelLength)
        num2 += effectiveProperty2.TwipsInteger;
      effectiveProperty2 = this.GetEffectiveProperty(Format.PropertyId.FirstLineIndent);
      if (effectiveProperty2.IsAbsRelLength)
        num3 = effectiveProperty2.TwipsInteger;
      if (num1 != 0)
        this.WriteKeyword("\\li", num1);
      if (num2 != 0)
        this.WriteKeyword("\\ri", num2);
      if (num3 != 0)
        this.WriteKeyword("\\fi", num3);
      if (val2 == 0 && val1 == 0)
        return;
      int num4 = val1 < 0 == val2 < 0 ? (val1 >= 0 ? Math.Max(val1, val2) : Math.Min(val1, val2)) : val1 + val2;
      if (num4 == 0)
        return;
      this.WriteKeyword("\\sb", num4);
    }

    private void OutputRowProps()
    {
      this.WriteControlText("\\trowd", true);
      this.WriteKeyword("\\irow", this.CurrentNodeIndex);
      this.WriteKeyword("\\irowband", this.CurrentNodeIndex);
      Format.PropertyValue effectiveProperty = this.GetEffectiveProperty(Format.PropertyId.RightToLeft);
      if (effectiveProperty.IsBool && effectiveProperty.Bool)
        this.WriteControlText("\\rtlrow", true);
      effectiveProperty = this.GetEffectiveProperty(Format.PropertyId.BackColor);
      if (effectiveProperty.IsColor)
        this.WriteKeyword("\\trcbpat", this.FindColorHandle(effectiveProperty));
      effectiveProperty = this.GetEffectiveProperty(Format.PropertyId.Width);
      if (effectiveProperty.IsPercentage)
      {
        this.WriteControlText("\\trftsWidth2", true);
        this.WriteKeyword("\\trwWidth", effectiveProperty.Percentage10K / 200);
      }
      effectiveProperty = this.GetEffectiveProperty(Format.PropertyId.Height);
      if (effectiveProperty.IsAbsLength || effectiveProperty.IsPixels)
        this.WriteKeyword("\\trrh", effectiveProperty.TwipsInteger);
      int num1 = 8856;
      Format.FormatNode node = this.CurrentNode.FirstChild;
      if (!node.IsNull)
      {
        int num2 = 0;
        for (; !node.IsNull; node = node.NextSibling)
          ++num2;
        int num3 = num1 / num2;
        int num4 = 0;
        int num5 = 0;
        int num6 = num2;
        node = this.CurrentNode.FirstChild;
        while (!node.IsNull)
        {
          Format.PropertyValue cellWidth;
          Format.PropertyValue cellHeight;
          this.OutputCellProps(node, out cellWidth, out cellHeight);
          if (num2 == 1 && !cellHeight.IsNull && node.FirstChild.IsNull)
          {
            this.WriteKeyword("\\fs", 2);
            if (cellHeight.IsAbsLength || cellHeight.IsPixels)
              this.WriteKeyword("\\trrh", cellHeight.TwipsInteger);
          }
          int num7;
          if (num6 == 1)
          {
            num7 = num1 - num4 > 360 ? num1 - num4 : num3;
          }
          else
          {
            num7 = num3;
            if (!cellWidth.IsNull)
            {
              if (cellWidth.IsPercentage)
                num7 = num1 * (cellWidth.Percentage10K / 100) / 100 / 100;
              else if (cellWidth.IsAbsLength || cellWidth.IsPixels)
                num7 = cellWidth.TwipsInteger;
            }
          }
          num4 += num7;
          this.WriteKeyword("\\cellx", num4);
          node = node.NextSibling;
          ++num5;
          --num6;
        }
      }
      else
      {
        this.WriteKeyword("\\cellx", num1);
        this.WriteControlText("\\cell", true);
      }
    }

    private void OutputCellProps(Format.FormatNode node, out Format.PropertyValue cellWidth, out Format.PropertyValue cellHeight)
    {
      bool flag = false;
      cellWidth = Format.PropertyValue.Null;
      cellHeight = Format.PropertyValue.Null;
      using (Format.NodePropertiesEnumerator propertiesEnumerator = node.PropertiesEnumerator)
      {
        foreach (Format.Property property in propertiesEnumerator)
        {
          switch (property.Id)
          {
            case Format.PropertyId.BlockAlignment:
              switch (property.Value.Enum)
              {
                case 1:
                  this.WriteControlText("\\clvertalc", true);
                  continue;
                case 2:
                  this.WriteControlText("\\clvertalb", true);
                  continue;
                default:
                  continue;
              }
            case Format.PropertyId.BackColor:
              this.WriteKeyword("\\clcbpat", this.FindColorHandle(property.Value));
              flag = true;
              continue;
            case Format.PropertyId.Width:
              cellWidth = property.Value;
              continue;
            case Format.PropertyId.Height:
              cellHeight = property.Value;
              continue;
            case Format.PropertyId.Paddings:
              if (property.Value.IsAbsRelLength)
              {
                this.WriteKeyword("\\clpadl", property.Value.TwipsInteger);
                this.WriteControlText("\\clpadfl3", true);
                continue;
              }
              continue;
            case Format.PropertyId.RightPadding:
              if (property.Value.IsAbsRelLength)
              {
                this.WriteKeyword("\\clpadr", property.Value.TwipsInteger);
                this.WriteControlText("\\clpadfr3", true);
                continue;
              }
              continue;
            case Format.PropertyId.BottomPadding:
              if (property.Value.IsAbsRelLength)
              {
                this.WriteKeyword("\\clpadb", property.Value.TwipsInteger);
                this.WriteControlText("\\clpadfb3", true);
                continue;
              }
              continue;
            case Format.PropertyId.LeftPadding:
              if (property.Value.IsAbsRelLength)
              {
                this.WriteKeyword("\\clpadt", property.Value.TwipsInteger);
                this.WriteControlText("\\clpadft3", true);
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
      if (flag)
        return;
      Format.PropertyValue effectiveProperty = this.GetEffectiveProperty(Format.PropertyId.BackColor);
      if (effectiveProperty.IsNull)
        return;
      this.WriteKeyword("\\clcbpat", this.FindColorHandle(effectiveProperty));
    }

    private void WriteKeyword(string keyword, int value)
    {
      this.HtmlRtfOffReally();
      this.output.WriteKeyword(keyword, value);
    }

    private void WriteControlText(string controlText, bool lastKeyword)
    {
      this.HtmlRtfOffReally();
      this.output.WriteControlText(controlText, lastKeyword);
    }

    private struct ListLevel
    {
      public RtfNumbering ListType;
      public bool Restart;
      public short NextIndex;

      public void Reset()
      {
        this.ListType = RtfNumbering.Bullet;
        this.Restart = false;
        this.NextIndex = (short) 1;
      }
    }

    private struct OutputFont
    {
      public string Name;
      public int Count;
      public bool SymbolFont;
    }
  }
}
