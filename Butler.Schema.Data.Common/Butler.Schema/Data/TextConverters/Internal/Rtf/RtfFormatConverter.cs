// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Rtf.RtfFormatConverter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Rtf
{
  internal class RtfFormatConverter : Format.FormatConverter, IProducerConsumer, IDisposable
  {
    private static readonly short TwelvePointsInTwips = (short) 240;
    private static string[] familyGenericName = new string[8]
    {
      null,
      "serif",
      "sans-serif",
      "monospace",
      "cursive",
      "fantasy",
      null,
      null
    };
    private bool beforeContent = true;
    private const short TableDefaultLeftIndent = (short) 108;
    protected short listsCount;
    protected RtfFormatConverter.List[] lists;
    protected short[] listDirectory;
    protected short listDirectoryCount;
    protected short listIdx;
    protected short listLevel;
    private RtfParser parser;
    private Format.FormatOutput output;
    private bool firstKeyword;
    private bool ignorableDestination;
    private RtfFormatConverter.RtfState state;
    private short fontsCount;
    private RtfFont[] fonts;
    private int colorsCount;
    private int[] colors;
    private short containerFontIndex;
    private int containerFontColor;
    private int containerBackColor;
    private short containerFontSize;
    private bool documentContainerStillOpen;
    private short documentFontIndex;
    private short documentFontSize;
    private bool ignoreFieldResult;
    private bool treatNbspAsBreakable;
    private Injection injection;
    private int color;
    private ScratchBuffer scratch;
    private ScratchBuffer scratchAlt;
    private int pictureWidth;
    private int pictureHeight;
    private RtfBorderId borderId;
    private RtfFormatConverter.RtfTable[] tableStack;
    private int currentTableLevel;
    private RtfFormatConverter.RtfRow newRow;
    private RtfFormatConverter.RtfRow newRowTopLevel;
    private RtfFormatConverter.RtfRow newRowNested;
    private int requiredTableLevel;
    private RtfFormatConverter.RtfRow firstFreeRow;
    private RtfFormatConverter.ListState currentListState;

    private bool CanFlush => this.output.CanAcceptMoreOutput;

      public RtfFormatConverter(RtfParser parser, Format.FormatOutput output, Injection injection, bool treatNbspAsBreakable, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum, Stream formatConverterTraceStream)
      : base(formatConverterTraceStream)
    {
      this.treatNbspAsBreakable = treatNbspAsBreakable;
      this.parser = parser;
      this.injection = injection;
      this.output = output;
      if (this.output != null)
        this.output.Initialize(this.Store, Format.SourceFormat.Rtf, "converted from rtf");
      this.Construct();
    }

    public RtfFormatConverter(RtfParser parser, Format.FormatStore formatStore, bool treatNbspAsBreakable, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum, Stream formatConverterTraceStream)
      : base(formatStore, formatConverterTraceStream)
    {
      this.treatNbspAsBreakable = treatNbspAsBreakable;
      this.parser = parser;
      this.Construct();
    }

    void IDisposable.Dispose()
    {
      this.scratch.DisposeBuffer();
      this.scratchAlt.DisposeBuffer();
      if (this.parser != null && this.parser is IDisposable)
        ((IDisposable) this.parser).Dispose();
      this.parser = (RtfParser) null;
      GC.SuppressFinalize((object) this);
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
        RtfTokenId tokenId = this.parser.Parse();
        if (tokenId == RtfTokenId.None)
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

    public void BreakLine()
    {
      if (this.beforeContent)
        this.PrepareToAddContent();
      this.AddLineBreak(1);
    }

    public void OutputTabulation(int count)
    {
      if (this.beforeContent)
        this.PrepareToAddContent();
      this.AddTabulation(count);
    }

    public void OutputSpace(int count)
    {
      this.AddSpace(count);
    }

    public void OutputNbsp(int count)
    {
      this.AddNbsp(count);
    }

    public void OutputNonspace(char[] buffer, int offset, int count, TextMapping textMapping)
    {
      this.AddNonSpaceText(buffer, offset, count);
    }

    private static RtfNumbering ListTypeToNumbering(int listType)
    {
      switch (listType)
      {
        case 0:
          return RtfNumbering.Arabic;
        case 1:
          return RtfNumbering.UcRoman;
        case 2:
          return RtfNumbering.LcRoman;
        case 3:
          return RtfNumbering.UcLetter;
        case 4:
          return RtfNumbering.LcLetter;
        case 23:
          return RtfNumbering.Bullet;
        default:
          return RtfNumbering.Bullet;
      }
    }

    private void Construct()
    {
      this.state = new RtfFormatConverter.RtfState();
      this.state.Initialize();
      this.colors = new int[64];
      this.fonts = new RtfFont[32];
      this.lists = new RtfFormatConverter.List[128];
      this.listDirectory = new short[128];
      this.fonts[0] = new RtfFont("Times New Roman");
      ++this.fontsCount;
      this.lists[0].Levels = new RtfFormatConverter.ListLevel[9];
      this.lists[0].Levels[0].Type = RtfNumbering.Arabic;
      this.lists[0].Levels[0].Start = (short) 1;
      this.lists[0].Levels[1].Type = RtfNumbering.LcLetter;
      this.lists[0].Levels[1].Start = (short) 1;
      this.lists[0].Levels[2].Type = RtfNumbering.LcRoman;
      this.lists[0].Levels[2].Start = (short) 1;
      this.lists[0].Levels[3].Type = RtfNumbering.Arabic;
      this.lists[0].Levels[3].Start = (short) 1;
      this.lists[0].Levels[4].Type = RtfNumbering.LcLetter;
      this.lists[0].Levels[4].Start = (short) 1;
      this.lists[0].Levels[5].Type = RtfNumbering.LcRoman;
      this.lists[0].Levels[5].Start = (short) 1;
      this.lists[0].Levels[6].Type = RtfNumbering.Arabic;
      this.lists[0].Levels[6].Start = (short) 1;
      this.lists[0].Levels[7].Type = RtfNumbering.LcLetter;
      this.lists[0].Levels[7].Start = (short) 1;
      this.lists[0].Levels[8].Type = RtfNumbering.LcRoman;
      this.lists[0].Levels[8].Start = (short) 1;
      this.lists[0].LevelCount = (byte) 9;
      ++this.listsCount;
      this.lists[1].Levels = new RtfFormatConverter.ListLevel[9];
      this.lists[1].Levels[0].Type = RtfNumbering.Bullet;
      this.lists[1].Levels[0].Start = (short) 1;
      this.lists[1].Levels[1].Type = RtfNumbering.Bullet;
      this.lists[1].Levels[1].Start = (short) 1;
      this.lists[1].Levels[2].Type = RtfNumbering.Bullet;
      this.lists[1].Levels[2].Start = (short) 1;
      this.lists[1].Levels[3].Type = RtfNumbering.Bullet;
      this.lists[1].Levels[3].Start = (short) 1;
      this.lists[1].Levels[4].Type = RtfNumbering.Bullet;
      this.lists[1].Levels[4].Start = (short) 1;
      this.lists[1].Levels[5].Type = RtfNumbering.Bullet;
      this.lists[1].Levels[5].Start = (short) 1;
      this.lists[1].Levels[6].Type = RtfNumbering.Bullet;
      this.lists[1].Levels[6].Start = (short) 1;
      this.lists[1].Levels[7].Type = RtfNumbering.Bullet;
      this.lists[1].Levels[7].Start = (short) 1;
      this.lists[1].Levels[8].Type = RtfNumbering.Bullet;
      this.lists[1].Levels[8].Start = (short) 1;
      this.lists[1].LevelCount = (byte) 9;
      ++this.listsCount;
      this.listDirectory[0] = (short) 0;
      ++this.listDirectoryCount;
      this.state.SetPlain();
      this.InitializeDocument();
      this.RegisterFontValue((short) 0);
      this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.FontFace, this.fonts[(int) this.state.FontIndex].Value);
      this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.FontSize, new Format.PropertyValue(Format.LengthUnits.Twips, (int) this.state.FontSize));
      this.currentListState.ListIndex = (short) -1;
      this.documentContainerStillOpen = true;
      if (this.injection == null)
        return;
      int num = this.injection.HaveHead ? 1 : 0;
    }

    private bool FlushOutput()
    {
      if (!this.output.Flush())
        return false;
      this.MustFlush = false;
      return true;
    }

    private void Process(RtfTokenId tokenId)
    {
      switch (tokenId)
      {
        case RtfTokenId.EndOfFile:
          this.ProcessEOF();
          break;
        case RtfTokenId.Begin:
          this.ProcessBeginGroup();
          break;
        case RtfTokenId.End:
          this.ProcessEndGroup();
          break;
        case RtfTokenId.Binary:
          this.ProcessBinary(this.parser.Token);
          break;
        case RtfTokenId.Keywords:
          this.ProcessKeywords(this.parser.Token);
          break;
        case RtfTokenId.Text:
          this.ProcessText(this.parser.Token);
          break;
      }
    }

    private void ProcessBeginGroup()
    {
      if (this.state.SkipLevel != 0)
      {
        ++this.state.Level;
      }
      else
      {
        this.state.Push();
        this.firstKeyword = true;
        this.ignorableDestination = false;
      }
    }

    private void ProcessEndGroup()
    {
      if (this.state.SkipLevel != 0)
      {
        if (this.state.Level != this.state.SkipLevel)
        {
          --this.state.Level;
          return;
        }
        this.state.SkipLevel = 0;
      }
      this.firstKeyword = false;
      if (this.state.Level <= 0)
        return;
      this.EndGroup();
    }

    private void ProcessKeywords(RtfToken token)
    {
      if (this.state.SkipLevel != 0 && this.state.Level >= this.state.SkipLevel)
        return;
      foreach (RtfKeyword rtfKeyword in token.Keywords)
      {
        if (this.firstKeyword)
        {
          if ((int) rtfKeyword.Id == 1)
          {
            this.ignorableDestination = true;
            continue;
          }
          this.firstKeyword = false;
          switch (rtfKeyword.Id)
          {
            case (short) 306:
              if (this.state.Destination == RtfDestination.Field)
              {
                this.state.Destination = RtfDestination.FieldInstruction;
                continue;
              }
              this.state.SkipGroup();
              return;
            case (short) 315:
            case (short) 273:
            case (short) 277:
            case (short) 253:
            case (short) 258:
            case (short) 233:
            case (short) 241:
            case (short) 227:
            case (short) 230:
            case (short) 201:
            case (short) 15:
            case (short) 24:
              this.state.SkipGroup();
              return;
            case (short) 316:
              if (this.state.Destination == RtfDestination.RTF && (int) this.listDirectoryCount == 1)
              {
                this.state.Destination = RtfDestination.ListOverrideTable;
                continue;
              }
              continue;
            case (short) 319:
              this.state.SkipGroup();
              return;
            case (short) 283:
              this.state.SkipGroup();
              return;
            case (short) 301:
            case (short) 92:
            case (short) 39:
              this.state.SkipGroup();
              return;
            case (short) 268:
              if (this.state.Destination == RtfDestination.FontTable && (int) this.state.FontIndex >= 0)
              {
                this.state.Destination = RtfDestination.AltFontName;
                continue;
              }
              continue;
            case (short) 269:
              if (this.state.Destination == RtfDestination.Field)
              {
                this.state.Destination = RtfDestination.FieldResult;
                continue;
              }
              this.state.SkipGroup();
              return;
            case (short) 246:
              if (this.state.Destination == RtfDestination.RTF && (int) this.listsCount == 2)
              {
                this.state.Destination = RtfDestination.ListTable;
                continue;
              }
              continue;
            case (short) 252:
              if (this.state.Destination == RtfDestination.RTF)
              {
                this.state.Destination = RtfDestination.ColorTable;
                continue;
              }
              continue;
            case (short) 257:
              this.state.SkipGroup();
              return;
            case (short) 210:
              if (this.state.Destination == RtfDestination.FontTable && (int) this.state.FontIndex >= 0)
              {
                this.state.Destination = RtfDestination.RealFontName;
                continue;
              }
              continue;
            case (short) 212:
              this.state.SkipGroup();
              return;
            case (short) 193:
              if (this.state.Destination == RtfDestination.RTF || this.state.Destination == RtfDestination.FieldResult && !this.ignoreFieldResult)
              {
                this.state.Destination = RtfDestination.BookmarkName;
                continue;
              }
              this.state.SkipGroup();
              return;
            case (short) 203:
              if (this.state.Destination == RtfDestination.ListOverrideTable)
              {
                if ((int) this.listDirectoryCount < this.lists.Length)
                {
                  this.listDirectory[(int) this.listDirectoryCount] = (short) 0;
                  ++this.listDirectoryCount;
                }
                this.state.Destination = RtfDestination.ListOverrideTableEntry;
                continue;
              }
              continue;
            case (short) 123:
              if (this.state.Destination != RtfDestination.Picture)
              {
                this.state.ListIndex = (short) 0;
                this.state.ListLevel = (byte) 0;
                this.state.Destination = RtfDestination.ParaNumbering;
                break;
              }
              break;
            case (short) 175:
              if (this.state.Destination == RtfDestination.RTF)
              {
                this.state.FontIndex = (short) -1;
                this.state.Destination = RtfDestination.FontTable;
                continue;
              }
              continue;
            case (short) 54:
              if (this.state.Destination == RtfDestination.ListTable && (int) this.listsCount < this.lists.Length)
              {
                this.lists[(int) this.listsCount].Levels = new RtfFormatConverter.ListLevel[9];
                this.listIdx = this.listsCount;
                ++this.listsCount;
                this.state.Destination = RtfDestination.ListTableEntry;
                continue;
              }
              continue;
            case (short) 65:
              this.state.Destination = RtfDestination.NestTableProps;
              continue;
            case (short) 29:
              if (this.state.Destination != RtfDestination.RTF && (this.state.Destination != RtfDestination.FieldResult || this.ignoreFieldResult))
                return;
              this.state.Destination = RtfDestination.Field;
              return;
            case (short) 50:
              if (this.state.Destination == RtfDestination.RTF || this.state.Destination == RtfDestination.FieldResult)
              {
                this.state.Destination = RtfDestination.Picture;
                this.pictureWidth = 0;
                this.pictureHeight = 0;
                continue;
              }
              this.state.SkipGroup();
              return;
            case (short) 8:
              this.state.SkipGroup();
              return;
            case (short) 9:
              if (this.state.Destination == RtfDestination.ListTableEntry)
              {
                if ((int) this.lists[(int) this.listIdx].LevelCount != 9)
                {
                  this.state.Destination = RtfDestination.ListLevelEntry;
                  this.listLevel = (short) this.lists[(int) this.listIdx].LevelCount++;
                  this.lists[(int) this.listIdx].Levels[(int) this.listLevel].Start = (short) 1;
                  break;
                }
                continue;
              }
              break;
            default:
              if (this.ignorableDestination)
              {
                this.state.SkipGroup();
                return;
              }
              break;
          }
        }
        switch (rtfKeyword.Id)
        {
          case (short) 309:
          case (short) 88:
          case (short) 148:
            if (this.state.Destination == RtfDestination.ColorTable)
            {
              this.color &= ~((int) byte.MaxValue << (int) RTFData.keywords[(int) rtfKeyword.Id].idx * 8);
              this.color |= (rtfKeyword.Value & (int) byte.MaxValue) << (int) RTFData.keywords[(int) rtfKeyword.Id].idx * 8;
              continue;
            }
            continue;
          case (short) 310:
          case (short) 313:
          case (short) 326:
          case (short) 294:
          case (short) 299:
          case (short) 4:
          case (short) 5:
          case (short) 19:
          case (short) 61:
          case (short) 63:
          case (short) 76:
          case (short) 150:
          case (short) 155:
          case (short) 159:
          case (short) 178:
          case (short) 190:
          case (short) 225:
          case (short) 278:
          case (short) 279:
            this.state.Underline = 0 != rtfKeyword.Value;
            continue;
          case (short) 318:
            if (rtfKeyword.Value >= 0 && rtfKeyword.Value < this.colorsCount && (this.newRow != null && this.newRow.EnsureEntryForCurrentCell()))
            {
              this.newRow.Cells[(int) this.newRow.CellCount].BackColor = rtfKeyword.Value == 0 ? 16777215 : this.colors[rtfKeyword.Value];
              continue;
            }
            continue;
          case (short) 320:
            if (rtfKeyword.Value >= 0 && rtfKeyword.Value <= 3 && this.newRow != null)
            {
              this.newRow.WidthType = (byte) rtfKeyword.Value;
              continue;
            }
            continue;
          case (short) 321:
          case (short) 328:
          case (short) 330:
          case (short) 289:
          case (short) 11:
          case (short) 14:
          case (short) 23:
          case (short) 46:
          case (short) 52:
          case (short) 64:
          case (short) 80:
          case (short) 89:
          case (short) 90:
          case (short) 97:
          case (short) 122:
          case (short) 132:
          case (short) 156:
          case (short) 171:
          case (short) 209:
          case (short) 216:
          case (short) 239:
          case (short) 243:
          case (short) 247:
          case (short) 251:
          case (short) 254:
          case (short) 256:
          case (short) 261:
          case (short) 276:
            this.SetBorderKind((RtfBorderKind) RTFData.keywords[(int) rtfKeyword.Id].idx);
            continue;
          case (short) 322:
            if (this.state.Destination == RtfDestination.ParaNumbering && (int) this.state.ListIndex == -1)
            {
              this.state.ListIndex = (short) 0;
              this.state.ListLevel = (byte) 0;
              continue;
            }
            continue;
          case (short) 323:
            this.state.Superscript = 0 != rtfKeyword.Value;
            continue;
          case (short) 331:
          case (short) 174:
            if (rtfKeyword.Value >= 0 && rtfKeyword.Value < this.colorsCount)
            {
              this.state.FontColor = this.colors[rtfKeyword.Value];
              if (this.state.FontColor == 16777215)
              {
                this.state.FontColor = 13619151;
                continue;
              }
              continue;
            }
            continue;
          case (short) 293:
            if (this.newRow != null)
            {
              this.newRow.HeaderRow = true;
              continue;
            }
            continue;
          case (short) 296:
            if (this.state.Destination == RtfDestination.ListLevelEntry && 0 <= rtfKeyword.Value && rtfKeyword.Value <= (int) byte.MaxValue)
            {
              this.lists[(int) this.listIdx].Levels[(int) this.listLevel].Type = RtfFormatConverter.ListTypeToNumbering(rtfKeyword.Value);
              continue;
            }
            continue;
          case (short) 6:
            if (this.state.Destination == RtfDestination.Picture)
            {
              this.pictureHeight = rtfKeyword.Value;
              continue;
            }
            continue;
          case (short) 7:
          case (short) 10:
          case (short) 12:
          case (short) 16:
          case (short) 22:
          case (short) 25:
          case (short) 181:
          case (short) 186:
          case (short) 188:
          case (short) 194:
          case (short) 195:
          case (short) 196:
          case (short) 215:
          case (short) 218:
            this.borderId = (RtfBorderId) RTFData.keywords[(int) rtfKeyword.Id].idx;
            continue;
          case (short) 13:
            if (rtfKeyword.Value < 0 || rtfKeyword.Value >= (int) short.MaxValue || this.newRow == null)
              continue;
            continue;
          case (short) 17:
            this.state.Capitalize = 0 != rtfKeyword.Value;
            continue;
          case (short) 21:
          case (short) 36:
          case (short) 77:
          case (short) 248:
          case (short) 274:
            if (this.state.Destination == RtfDestination.ParaNumbering)
            {
              this.state.ListStyle = (RtfNumbering) RTFData.keywords[(int) rtfKeyword.Id].idx;
              continue;
            }
            continue;
          case (short) 40:
            this.EventEndRow(false);
            continue;
          case (short) 41:
          case (short) 91:
            if (this.newRow != null)
            {
              this.newRow.RightToLeft = true;
              continue;
            }
            continue;
          case (short) 44:
            if (this.state.Destination == RtfDestination.ListLevelEntry && 0 < rtfKeyword.Value)
            {
              this.lists[(int) this.listIdx].Levels[(int) this.listLevel].Start = (short) rtfKeyword.Value;
              continue;
            }
            continue;
          case (short) 48:
            if (this.state.Destination == RtfDestination.RTF || this.state.Destination == RtfDestination.FieldResult && !this.ignoreFieldResult)
            {
              this.BreakLine();
              continue;
            }
            continue;
          case (short) 51:
            if (this.state.Destination == RtfDestination.ParaNumbering)
            {
              this.state.ListIndex = (short) 1;
              this.state.ListLevel = (byte) 0;
              this.state.ListStyle = RtfNumbering.Bullet;
              continue;
            }
            continue;
          case (short) 55:
            this.EventEndRow(true);
            continue;
          case (short) 58:
            if (rtfKeyword.Value >= 0 && rtfKeyword.Value < this.colorsCount)
            {
              this.state.ParagraphBackColor = rtfKeyword.Value == 0 ? 16777215 : this.colors[rtfKeyword.Value];
              continue;
            }
            continue;
          case (short) 66:
            if (rtfKeyword.Value >= (int) short.MinValue && rtfKeyword.Value < (int) short.MaxValue && (this.newRow != null && (int) this.newRow.CellCount == 0))
            {
              this.newRow.Left = this.newRow.Right = (short) (rtfKeyword.Value + 108);
              continue;
            }
            continue;
          case (short) 68:
            if (this.state.Destination == RtfDestination.RTF || this.state.Destination == RtfDestination.FieldResult && !this.ignoreFieldResult)
            {
              this.EventEndParagraph();
              continue;
            }
            continue;
          case (short) 73:
            if (this.newRow != null)
            {
              this.newRow.LastRow = true;
              continue;
            }
            continue;
          case (short) 78:
            if (this.newRow != null && this.newRow.EnsureEntryForCurrentCell())
            {
              this.newRow.Cells[(int) this.newRow.CellCount].MergeUp = true;
              continue;
            }
            continue;
          case (short) 82:
          case (short) 285:
            if (rtfKeyword.Value >= 0 && rtfKeyword.Value < this.colorsCount)
            {
              this.state.FontBackColor = rtfKeyword.Value == 0 ? this.state.ParagraphBackColor : this.colors[rtfKeyword.Value];
              continue;
            }
            continue;
          case (short) 86:
            if (this.state.Destination == RtfDestination.ParaNumbering && (int) this.state.ListIndex != -1)
            {
              this.lists[(int) this.state.ListIndex].Levels[(int) this.state.ListLevel].Start = (short) rtfKeyword.Value;
              continue;
            }
            continue;
          case (short) 98:
          case (short) 100:
            if (rtfKeyword.Value >= 0 && rtfKeyword.Value < this.colorsCount && (this.newRow != null && this.newRow.EnsureEntryForCurrentCell()))
            {
              this.newRow.Cells[(int) this.newRow.CellCount].VerticalAlignment = (RtfVertAlignment) RTFData.keywords[(int) rtfKeyword.Id].idx;
              continue;
            }
            continue;
          case (short) 99:
          case (short) 198:
            this.state.Strikethrough = 0 != rtfKeyword.Value;
            continue;
          case (short) 101:
            if (rtfKeyword.Value >= 0 && rtfKeyword.Value < 10)
            {
              this.requiredTableLevel = rtfKeyword.Value;
              continue;
            }
            continue;
          case (short) 102:
            if (this.state.Destination == RtfDestination.ParaNumbering && 1 <= rtfKeyword.Value && (rtfKeyword.Value <= 9 && (int) this.state.ListIndex == -1))
            {
              this.state.ListIndex = (short) 0;
              this.state.ListLevel = (byte) (rtfKeyword.Value - 1);
              continue;
            }
            continue;
          case (short) 103:
            if (rtfKeyword.Value >= 0 && rtfKeyword.Value <= 3 && (this.newRow != null && this.newRow.EnsureEntryForCurrentCell()))
            {
              this.newRow.Cells[(int) this.newRow.CellCount].WidthType = (byte) rtfKeyword.Value;
              continue;
            }
            continue;
          case (short) 105:
            this.EventEndCell(true);
            continue;
          case (short) 111:
          case (short) 112:
          case (short) 116:
          case (short) 120:
          case (short) 140:
          case (short) 141:
            this.state.ParagraphAlignment = (RtfAlignment) RTFData.keywords[(int) rtfKeyword.Id].idx;
            continue;
          case (short) 113:
            if (this.state.Destination == RtfDestination.FontTable || this.state.Destination == RtfDestination.AltFontName || this.state.Destination == RtfDestination.RealFontName)
            {
              if ((int) this.state.FontIndex >= 0 && (int) this.state.FontIndex < (int) this.fontsCount)
              {
                this.fonts[(int) this.state.FontIndex].Name = RtfSupport.StringFontNameFromScratch(this.scratch);
                this.scratch.Reset();
                this.state.FontIndex = (short) -1;
              }
              short num = this.parser.FontIndex((short) rtfKeyword.Value);
              if ((int) num >= 0)
              {
                this.state.FontIndex = num;
                if ((int) num >= (int) this.fontsCount)
                {
                  if ((int) num >= this.fonts.Length)
                  {
                    RtfFont[] rtfFontArray = new RtfFont[Math.Max(this.fonts.Length * 2, (int) num + 1)];
                    Array.Copy((Array) this.fonts, (Array) rtfFontArray, (int) this.fontsCount);
                    this.fonts = rtfFontArray;
                  }
                  this.fontsCount = (short) ((int) num + 1);
                }
                if (this.fonts[(int) num].Value.IsRefCountedHandle)
                {
                  this.ReleasePropertyValue(this.fonts[(int) num].Value);
                  this.fonts[(int) num].Value = Format.PropertyValue.Null;
                  continue;
                }
                continue;
              }
              continue;
            }
            continue;
          case (short) 118:
            if (rtfKeyword.Value >= 0 && rtfKeyword.Value < (int) short.MaxValue && this.newRow != null)
            {
              this.newRow.CellPadding.Right = (short) rtfKeyword.Value;
              continue;
            }
            continue;
          case (short) 119:
            this.EventEndCell(false);
            continue;
          case (short) 121:
            if (rtfKeyword.Value >= 0 && rtfKeyword.Value < (int) short.MaxValue && this.newRow != null)
            {
              this.newRow.CellPadding.Top = (short) rtfKeyword.Value;
              continue;
            }
            continue;
          case (short) 126:
            if (this.state.Destination == RtfDestination.RTF || this.state.Destination == RtfDestination.FieldResult && !this.ignoreFieldResult)
            {
              this.OutputTabulation(1);
              continue;
            }
            continue;
          case (short) 131:
            if (rtfKeyword.Value >= 0 && rtfKeyword.Value < (int) short.MaxValue && this.newRow != null)
            {
              this.newRow.CellPadding.Left = (short) rtfKeyword.Value;
              continue;
            }
            continue;
          case (short) 136:
            this.state.Invisible = rtfKeyword.Value != 0;
            continue;
          case (short) 138:
            this.state.RightIndent = (short) rtfKeyword.Value;
            continue;
          case (short) 139:
            this.state.SpaceAfter = (short) rtfKeyword.Value;
            continue;
          case (short) 142:
            this.state.SpaceBefore = (short) rtfKeyword.Value;
            continue;
          case (short) 143:
            if (rtfKeyword.Value >= 0 && rtfKeyword.Value < this.colorsCount && this.newRow != null)
            {
              this.newRow.BackColor = this.colors[rtfKeyword.Value];
              continue;
            }
            continue;
          case (short) 144:
            if (rtfKeyword.Value >= 0 && rtfKeyword.Value < (int) short.MaxValue && this.newRow != null)
            {
              this.newRow.CellPadding.Bottom = (short) rtfKeyword.Value;
              continue;
            }
            continue;
          case (short) 151:
            if (rtfKeyword.Value >= 0 && rtfKeyword.Value < (int) short.MaxValue && this.newRow != null)
            {
              this.newRow.Height = (short) rtfKeyword.Value;
              continue;
            }
            continue;
          case (short) 154:
            if (this.state.Destination == RtfDestination.Picture)
            {
              this.pictureWidth = rtfKeyword.Value;
              continue;
            }
            continue;
          case (short) 157:
            if (this.beforeContent)
              this.PrepareToAddContent();
            this.OpenContainer(Format.FormatContainerType.Image, true);
            this.Last.SetStringProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.ImageUrl, "objattph://");
            this.documentContainerStillOpen = false;
            continue;
          case (short) 160:
            if (this.newRow != null && this.newRow.EnsureEntryForCurrentCell())
            {
              this.newRow.Cells[(int) this.newRow.CellCount].MergeLeft = true;
              continue;
            }
            continue;
          case (short) 161:
            if (rtfKeyword.Value >= 0 && rtfKeyword.Value < (int) short.MaxValue && (this.newRow != null && this.newRow.EnsureEntryForCurrentCell()))
            {
              this.newRow.Cells[(int) this.newRow.CellCount].Padding.Right = (short) rtfKeyword.Value;
              continue;
            }
            continue;
          case (short) 163:
            if (rtfKeyword.Value >= 0 && rtfKeyword.Value < (int) short.MaxValue && (this.newRow != null && this.newRow.EnsureEntryForCurrentCell()))
            {
              this.newRow.Cells[(int) this.newRow.CellCount].Padding.Top = (short) rtfKeyword.Value;
              continue;
            }
            continue;
          case (short) 167:
            if (rtfKeyword.Value >= 0 && rtfKeyword.Value < this.colorsCount)
            {
              this.SetBorderColor(this.colors[rtfKeyword.Value]);
              continue;
            }
            continue;
          case (short) 170:
            this.state.SetPlain();
            continue;
          case (short) 172:
            if (rtfKeyword.Value >= 0 && rtfKeyword.Value < (int) short.MaxValue && (this.newRow != null && this.newRow.EnsureEntryForCurrentCell()))
            {
              this.newRow.Cells[(int) this.newRow.CellCount].Padding.Left = (short) rtfKeyword.Value;
              continue;
            }
            continue;
          case (short) 173:
            if (this.state.Destination == RtfDestination.ListTableEntry)
            {
              this.lists[(int) this.listIdx].Id = rtfKeyword.Value;
              continue;
            }
            if (this.state.Destination == RtfDestination.ListOverrideTableEntry && (int) this.listsCount != 1)
            {
              for (short index = (short) 1; (int) index < (int) this.listsCount; ++index)
              {
                if (this.lists[(int) index].Id == rtfKeyword.Value)
                {
                  this.listDirectory[(int) this.listDirectoryCount - 1] = index;
                  break;
                }
              }
              continue;
            }
            continue;
          case (short) 176:
            this.state.ParagraphRtl = true;
            continue;
          case (short) 179:
            if (rtfKeyword.Value >= 0 && rtfKeyword.Value < (int) short.MaxValue && (this.newRow != null && this.newRow.EnsureEntryForCurrentCell()))
            {
              this.newRow.Cells[(int) this.newRow.CellCount].Padding.Bottom = (short) rtfKeyword.Value;
              continue;
            }
            continue;
          case (short) 180:
            this.state.SmallCaps = 0 != rtfKeyword.Value;
            continue;
          case (short) 184:
            this.state.SetParagraphDefault();
            this.requiredTableLevel = 0;
            continue;
          case (short) 191:
            if (this.requiredTableLevel == 0)
            {
              this.requiredTableLevel = 1;
              continue;
            }
            continue;
          case (short) 199:
            if (this.newRow == null || !this.newRow.EnsureEntryForCurrentCell())
              continue;
            continue;
          case (short) 200:
            this.InitializeFreshRowProperties();
            continue;
          case (short) 205:
            if ((int) this.state.ListIndex >= 0 && 0 <= rtfKeyword.Value && rtfKeyword.Value < (int) this.lists[(int) this.state.ListIndex].LevelCount)
            {
              this.state.ListLevel = (byte) rtfKeyword.Value;
              this.state.ListStyle = this.lists[(int) this.state.ListIndex].Levels[(int) this.state.ListLevel].Type;
              continue;
            }
            continue;
          case (short) 206:
            this.state.LeftIndent = (short) rtfKeyword.Value;
            continue;
          case (short) 211:
            this.state.Underline = false;
            continue;
          case (short) 213:
            if (rtfKeyword.Value >= 0 && rtfKeyword.Value < (int) short.MaxValue && (this.newRow != null && this.newRow.EnsureEntryForCurrentCell()))
            {
              this.newRow.Cells[(int) this.newRow.CellCount].Width = (short) rtfKeyword.Value;
              continue;
            }
            continue;
          case (short) 219:
            if (this.state.Destination != RtfDestination.ListOverrideTableEntry && rtfKeyword.Value >= 1)
            {
              short num = (short) rtfKeyword.Value;
              if ((int) num < 0 || (int) num >= (int) this.listDirectoryCount)
                num = (short) 0;
              this.state.ListIndex = this.listDirectory[(int) num];
              this.state.ListLevel = (byte) 0;
              this.state.ListStyle = this.lists[(int) this.state.ListIndex].Levels[(int) this.state.ListLevel].Type;
              continue;
            }
            continue;
          case (short) 220:
            if (rtfKeyword.Value >= 0 && rtfKeyword.Value <= 75)
            {
              this.SetBorderWidth((byte) rtfKeyword.Value);
              continue;
            }
            continue;
          case (short) 221:
            if (rtfKeyword.Value < 0 || rtfKeyword.Value >= (int) short.MaxValue || this.newRow == null)
              continue;
            continue;
          case (short) 228:
            if (rtfKeyword.Value >= 0 && rtfKeyword.Value < 32659 && (this.newRow != null && this.newRow.EnsureEntryForCurrentCell()))
            {
              if (rtfKeyword.Value >= (int) this.newRow.Right)
                this.newRow.Right = this.newRow.Cells[(int) this.newRow.CellCount].Cellx = (short) (rtfKeyword.Value + 108);
              else
                this.newRow.Cells[(int) this.newRow.CellCount].Cellx = this.newRow.Right;
              ++this.newRow.CellCount;
              continue;
            }
            continue;
          case (short) 229:
            if (this.state.Destination == RtfDestination.ParaNumbering)
            {
              this.state.ListStyle = RtfNumbering.None;
              this.state.ListIndex = (short) -1;
              continue;
            }
            continue;
          case (short) 237:
            if (rtfKeyword.Value >= 0 && rtfKeyword.Value < (int) short.MaxValue && this.newRow != null)
            {
              this.newRow.Width = (short) rtfKeyword.Value;
              continue;
            }
            continue;
          case (short) 250:
            if (this.state.Destination != RtfDestination.ListLevelEntry || 0 > rtfKeyword.Value || rtfKeyword.Value > 2)
              continue;
            continue;
          case (short) 259:
            if (this.newRow == null || !this.newRow.EnsureEntryForCurrentCell())
              continue;
            continue;
          case (short) 262:
            this.state.Subscript = 0 != rtfKeyword.Value;
            continue;
          case (short) 264:
            this.state.Subscript = false;
            this.state.Superscript = false;
            continue;
          case (short) 284:
            this.state.FirstLineIndent = (short) rtfKeyword.Value;
            continue;
          default:
            continue;
        }
      }
      if ((int) this.parser.CurrentFontIndex < (int) this.fontsCount)
        this.state.FontIndex = this.parser.CurrentFontIndex;
      this.state.FontSize = this.parser.CurrentFontSize;
      this.state.Language = this.parser.CurrentLanguage;
      this.state.Bold = this.parser.CurrentFontBold;
      this.state.Italic = this.parser.CurrentFontItalic;
      this.state.BiDi = this.parser.CurrentRunBiDi;
      this.state.ComplexScript = this.parser.CurrentRunComplexScript;
    }

    private void ProcessText(RtfToken token)
    {
      if (this.state.SkipLevel != 0 && this.state.Level >= this.state.SkipLevel)
        return;
      switch (this.state.Destination)
      {
        case RtfDestination.RTF:
          if (this.state.Invisible)
            break;
          RtfToken.TextEnumerator textElements = token.TextElements;
          if (!textElements.MoveNext())
            break;
          if (this.beforeContent)
            this.PrepareToAddContent();
          if (this.state.TextPropertiesChanged)
          {
            this.OpenTextContainer();
            if ((int) this.state.FontSize != (int) this.containerFontSize)
              this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.FontSize, new Format.PropertyValue(Format.LengthUnits.Twips, (int) this.state.FontSize));
            if (this.state.Bold)
              this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.FirstFlag, new Format.PropertyValue(this.state.Bold));
            if (this.state.Italic)
              this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.Italic, new Format.PropertyValue(this.state.Italic));
            if (this.state.Underline)
              this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.Underline, new Format.PropertyValue(this.state.Underline));
            if (this.state.Subscript)
            {
              this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.Subscript, new Format.PropertyValue(this.state.Subscript));
              this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.FontSize, new Format.PropertyValue(Format.LengthUnits.Twips, (int) this.state.FontSize * 2 / 3));
            }
            if (this.state.Superscript)
            {
              this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.Superscript, new Format.PropertyValue(this.state.Superscript));
              this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.FontSize, new Format.PropertyValue(Format.LengthUnits.Twips, (int) this.state.FontSize * 2 / 3));
            }
            if (this.state.Strikethrough)
              this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.Strikethrough, new Format.PropertyValue(this.state.Strikethrough));
            if (this.state.SmallCaps)
              this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.SmallCaps, new Format.PropertyValue(this.state.SmallCaps));
            if (this.state.Capitalize)
              this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.Capitalize, new Format.PropertyValue(this.state.Capitalize));
            if ((int) this.state.FontIndex != (int) this.containerFontIndex && (int) this.state.FontIndex != -1)
            {
              if (this.fonts[(int) this.state.FontIndex].Value.IsNull)
                this.RegisterFontValue(this.state.FontIndex);
              this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.FontFace, this.fonts[(int) this.state.FontIndex].Value);
            }
            if (this.state.FontColor != this.containerFontColor)
              this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.FontColor, new Format.PropertyValue(new Format.RGBT((uint) this.state.FontColor)));
            if (this.state.FontBackColor != this.containerBackColor)
              this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.BackColor, new Format.PropertyValue(new Format.RGBT((uint) this.state.FontBackColor)));
            this.state.TextPropertiesChanged = false;
          }
          RtfTextElement current;
          if (this.firstKeyword)
          {
            current = textElements.Current;
            if (current.TextType != RunTextType.Space || current.Length != 1)
            {
              if (textElements.MoveNext())
                this.firstKeyword = false;
              textElements.Rewind();
              textElements.MoveNext();
            }
          }
          do
          {
            current = textElements.Current;
            switch (current.TextType)
            {
              case RunTextType.Nbsp:
                if (this.treatNbspAsBreakable)
                {
                  this.OutputSpace(current.Length);
                  break;
                }
                this.OutputNbsp(current.Length);
                break;
              case RunTextType.NonSpace:
                this.OutputNonspace(current.RawBuffer, current.RawOffset, current.RawLength, token.TextMapping);
                break;
              case RunTextType.Space:
              case RunTextType.UnusualWhitespace:
                this.OutputSpace(current.Length);
                break;
            }
          }
          while (textElements.MoveNext());
          break;
        case RtfDestination.FontTable:
          this.firstKeyword = false;
          this.scratch.AppendRtfTokenText(token, 256);
          break;
        case RtfDestination.RealFontName:
        case RtfDestination.AltFontName:
          this.firstKeyword = false;
          break;
        case RtfDestination.ColorTable:
          this.firstKeyword = false;
          this.scratch.Reset();
          while (this.scratch.AppendRtfTokenText(token, this.scratch.Capacity))
          {
            for (int index = 0; index < this.scratch.Length; ++index)
            {
              if ((int) this.scratch[index] == 59)
              {
                if (this.colorsCount == this.colors.Length)
                  return;
                this.colors[this.colorsCount] = this.color;
                ++this.colorsCount;
                this.color = 0;
              }
            }
            this.scratch.Reset();
          }
          break;
        case RtfDestination.FieldResult:
          if (this.ignoreFieldResult)
            break;
          goto case 0;
        case RtfDestination.FieldInstruction:
          this.firstKeyword = false;
          this.scratch.AppendRtfTokenText(token, 4096);
          break;
        case RtfDestination.BookmarkName:
          this.firstKeyword = false;
          this.scratch.AppendRtfTokenText(token, 4096);
          break;
        default:
          this.firstKeyword = false;
          break;
      }
    }

    private void ProcessBinary(RtfToken token)
    {
    }

    private void RegisterFontValue(short fontIndex)
    {
      Format.PropertyValue propertyValue;
      if (this.fonts[(int) fontIndex].Name != null)
      {
        propertyValue = this.RegisterFaceName(false, this.fonts[(int) fontIndex].Name);
      }
      else
      {
        string faceName = RtfFormatConverter.familyGenericName[(int) this.parser.FontFamily(fontIndex)];
        propertyValue = faceName == null ? this.RegisterFaceName(false, "serif") : this.RegisterFaceName(false, faceName);
      }
      this.fonts[(int) fontIndex].Value = propertyValue;
    }

    private void ProcessEOF()
    {
      while (this.LastNonEmpty.Type != Format.FormatContainerType.Document)
        this.CloseContainer();
      if (this.injection != null && this.injection.HaveTail)
      {
        while (this.LastNonEmpty.Type != Format.FormatContainerType.Document)
          this.CloseContainer();
      }
      this.CloseAllContainersAndSetEOF();
    }

    private void EndGroup()
    {
      short num1 = (short) -1;
      byte num2 = (byte) 0;
      RtfNumbering rtfNumbering = RtfNumbering.None;
      int num3 = 0;
      short num4 = (short) 0;
      short num5 = (short) 0;
      RtfDestination destination1 = this.state.Destination;
      if ((uint) destination1 <= 18U)
      {
        switch (destination1)
        {
          case RtfDestination.FontTable:
            if (this.state.ParentDestination != RtfDestination.FontTable)
            {
              if ((int) this.state.FontIndex >= 0)
              {
                this.fonts[(int) this.state.FontIndex].Name = RtfSupport.StringFontNameFromScratch(this.scratch);
                this.scratch.Reset();
                break;
              }
              break;
            }
            if ((int) this.state.FontIndex >= 0)
            {
              this.state.SetParentFontIndex(this.state.FontIndex);
              break;
            }
            break;
          case RtfDestination.RealFontName:
            if (this.state.ParentDestination != RtfDestination.RealFontName && (int) this.state.ParentFontIndex >= 0)
            {
              this.scratchAlt.Reset();
              break;
            }
            break;
          case RtfDestination.AltFontName:
            if (this.state.ParentDestination != RtfDestination.AltFontName && (int) this.state.ParentFontIndex >= 0)
            {
              this.scratchAlt.Reset();
              break;
            }
            break;
          case RtfDestination.Field:
            if (this.state.ParentDestination != RtfDestination.Field)
            {
              this.ignoreFieldResult = false;
              if (this.LastNonEmpty.Type == Format.FormatContainerType.HyperLink && this.state.ParentDestination != RtfDestination.FieldResult)
              {
                this.CloseContainer();
                break;
              }
              break;
            }
            break;
          case RtfDestination.FieldInstruction:
            if (this.state.ParentDestination == RtfDestination.Field)
            {
              bool local;
              BufferString linkUrl;
              if (RtfSupport.IsHyperlinkField(ref this.scratch, out local, out linkUrl))
              {
                if (this.beforeContent)
                  this.PrepareToAddContent();
                else if (this.LastNonEmpty.Type == Format.FormatContainerType.HyperLink)
                  this.CloseContainer();
                linkUrl.TrimWhitespace();
                this.OpenContainer(Format.FormatContainerType.HyperLink, false);
                this.Last.SetStringProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.HyperlinkUrl, linkUrl);
                this.documentContainerStillOpen = false;
              }
              else if (RtfSupport.IsIncludePictureField(ref this.scratch, out linkUrl))
              {
                if (this.beforeContent)
                  this.PrepareToAddContent();
                linkUrl.TrimWhitespace();
                this.OpenContainer(Format.FormatContainerType.Image, true);
                this.Last.SetStringProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.ImageUrl, linkUrl);
                this.documentContainerStillOpen = false;
                this.ignoreFieldResult = true;
              }
              else
              {
                TextMapping textMapping;
                char symbol;
                short points;
                if (RtfSupport.IsSymbolField(ref this.scratch, out textMapping, out symbol, out points))
                {
                  if (this.beforeContent)
                    this.PrepareToAddContent();
                  this.OpenTextContainer();
                  Format.PropertyValue propertyValue1 = new Format.PropertyValue();
                  Format.PropertyValue propertyValue2;
                  switch (textMapping)
                  {
                    case TextMapping.Symbol:
                      propertyValue2 = this.RegisterFaceName(false, "Symbol");
                      break;
                    default:
                      propertyValue2 = this.RegisterFaceName(false, "Wingdings");
                      break;
                  }
                  if (!propertyValue2.IsNull)
                    this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.FontFace, propertyValue2);
                  if ((int) points != 0)
                    this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.FontSize, new Format.PropertyValue(Format.LengthUnits.Points, (int) points));
                  this.scratch.Buffer[0] = symbol;
                  this.OutputNonspace(this.scratch.Buffer, 0, 1, textMapping);
                  this.documentContainerStillOpen = false;
                  this.ignoreFieldResult = true;
                }
              }
              this.scratch.Reset();
              break;
            }
            break;
          case RtfDestination.ParaNumbering:
          case RtfDestination.ParaNumText:
            num1 = this.state.ListIndex;
            num2 = this.state.ListLevel;
            rtfNumbering = this.state.ListStyle;
            num3 = this.state.FontColor;
            num4 = this.state.FontSize;
            num5 = this.state.FontIndex;
            break;
        }
      }
      else if (destination1 != RtfDestination.Picture)
      {
        if (destination1 == RtfDestination.BookmarkName && this.state.ParentDestination != RtfDestination.BookmarkName)
        {
          BufferString bufferString = this.scratch.BufferString;
          bufferString.TrimWhitespace();
          if (bufferString.Length != 0)
          {
            int num6 = this.beforeContent ? 1 : 0;
            this.OpenContainer(Format.FormatContainerType.Bookmark, false);
            this.Last.SetStringProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.BookmarkName, bufferString);
            this.CloseContainer();
            this.documentContainerStillOpen = false;
          }
          this.scratch.Reset();
        }
      }
      else if (this.state.ParentDestination != RtfDestination.Picture)
      {
        if (!this.ignoreFieldResult)
        {
          if (this.beforeContent)
            this.PrepareToAddContent();
          this.OpenContainer(Format.FormatContainerType.Image, true);
          this.Last.SetStringProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.ImageUrl, "rtfimage://");
          this.documentContainerStillOpen = false;
        }
        if (this.Last.Type == Format.FormatContainerType.Image)
        {
          if (this.pictureWidth != 0)
            this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.Width, new Format.PropertyValue(Format.LengthUnits.Twips, this.pictureWidth));
          if (this.pictureHeight != 0)
            this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.Height, new Format.PropertyValue(Format.LengthUnits.Twips, this.pictureHeight));
        }
      }
      RtfDestination destination2 = this.state.Destination;
      this.state.Pop();
      if (destination2 != RtfDestination.ParaNumbering || this.state.Destination == RtfDestination.ParaNumbering)
        return;
      this.state.ListIndex = num1;
      this.state.ListLevel = num2;
      this.state.ListStyle = rtfNumbering;
      this.state.FontColor = num3;
      this.state.FontSize = num4;
      this.state.FontIndex = num5;
    }

    private void EventEndParagraph()
    {
      if (this.beforeContent)
      {
        this.PrepareToAddContent();
        this.AddNbsp(1);
      }
      if (this.LastNonEmpty.Type == Format.FormatContainerType.HyperLink)
        this.CloseContainer();
      if (this.LastNonEmpty.Type == Format.FormatContainerType.Block)
        this.CloseContainer();
      if (this.LastNonEmpty.Type == Format.FormatContainerType.ListItem)
        this.CloseContainer();
      this.beforeContent = true;
    }

    private void EventEndRow(bool nested)
    {
      if (this.beforeContent && (this.requiredTableLevel != this.currentTableLevel || this.LastNonEmpty.Type != Format.FormatContainerType.TableRow || this.LastNonEmpty.Node.LastChild.IsNull))
        this.PrepareToAddContent();
      if (this.currentTableLevel > (nested ? 1 : 0))
      {
        if (!nested && 1 != this.currentTableLevel)
          this.AdjustTableLevel(1);
        else if (-1 != (int) this.currentListState.ListIndex)
        {
          this.CloseList();
        }
        else
        {
          if (this.LastNonEmpty.Type == Format.FormatContainerType.HyperLink)
            this.CloseContainer();
          if (this.LastNonEmpty.Type == Format.FormatContainerType.Block)
            this.CloseContainer();
        }
        if (this.LastNonEmpty.Type == Format.FormatContainerType.Table)
        {
          this.OpenRow();
          this.OpenCell();
        }
        if (this.LastNonEmpty.Type == Format.FormatContainerType.TableCell)
          this.CloseCell();
        this.CloseRow(false);
        this.beforeContent = true;
      }
      else
        this.EventEndParagraph();
    }

    private void EventEndCell(bool nested)
    {
      if (this.beforeContent)
        this.PrepareToAddContent();
      if (this.currentTableLevel > (nested ? 1 : 0))
      {
        if (!nested && 1 != this.currentTableLevel)
          this.AdjustTableLevel(1);
        else if (-1 != (int) this.currentListState.ListIndex)
        {
          this.CloseList();
        }
        else
        {
          if (this.LastNonEmpty.Type == Format.FormatContainerType.HyperLink)
            this.CloseContainer();
          if (this.LastNonEmpty.Type == Format.FormatContainerType.Block)
            this.CloseContainer();
        }
        if (this.LastNonEmpty.Type == Format.FormatContainerType.Table)
          this.OpenRow();
        if (this.LastNonEmpty.Type == Format.FormatContainerType.TableRow)
          this.OpenCell();
        this.CloseCell();
        this.beforeContent = true;
      }
      else
      {
        if (nested)
          return;
        this.AddLineBreak(1);
        this.documentContainerStillOpen = false;
      }
    }

    private void AdjustContainer()
    {
      if (this.requiredTableLevel != this.currentTableLevel)
        this.AdjustTableLevel(this.requiredTableLevel);
      if ((int) this.state.ListIndex != (int) this.currentListState.ListIndex || (int) this.state.ListLevel != (int) this.currentListState.ListLevel || (this.state.ListStyle != this.currentListState.ListStyle || (int) this.state.LeftIndent != (int) this.currentListState.LeftIndent) || ((int) this.state.RightIndent != (int) this.currentListState.RightIndent || (int) this.state.FirstLineIndent != (int) this.currentListState.FirstLineIndent || this.state.ParagraphRtl != this.currentListState.Rtl))
        this.CloseList();
      if ((int) this.state.ListIndex == -1)
        return;
      this.OpenList();
    }

    private void PrepareToAddContent()
    {
      if (this.documentContainerStillOpen)
      {
        short num = this.state.FontIndex;
        if ((int) this.state.FontIndex != -1)
        {
          if (this.fonts[(int) this.state.FontIndex].Name != null && !this.fonts[(int) this.state.FontIndex].Name.StartsWith("Wingdings", StringComparison.OrdinalIgnoreCase) && !this.fonts[(int) this.state.FontIndex].Name.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
          {
            if (this.fonts[(int) this.state.FontIndex].Value.IsNull)
              this.RegisterFontValue(this.state.FontIndex);
            this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.FontFace, this.fonts[(int) this.state.FontIndex].Value);
          }
          else
            num = (short) -1;
        }
        if ((int) this.state.FontSize != (int) RtfFormatConverter.TwelvePointsInTwips)
          this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.FontSize, new Format.PropertyValue(Format.LengthUnits.Twips, (int) this.state.FontSize));
        this.documentFontIndex = num;
        this.documentFontSize = this.state.FontSize;
        this.documentContainerStillOpen = false;
      }
      this.AdjustContainer();
      if (this.LastNonEmpty.Type == Format.FormatContainerType.Document)
        this.OpenContainer(Format.FormatContainerType.Block, false);
      else if (this.LastNonEmpty.Type == Format.FormatContainerType.Table)
      {
        this.OpenRow();
        this.OpenCell();
      }
      else if (this.LastNonEmpty.Type == Format.FormatContainerType.TableRow)
        this.OpenCell();
      else if (this.LastNonEmpty.Type == Format.FormatContainerType.List)
      {
        this.OpenContainer(Format.FormatContainerType.ListItem, false);
        ++this.lists[(int) this.currentListState.ListIndex].Levels[(int) this.currentListState.ListLevel].Start;
      }
      if (this.LastNonEmpty.Type == Format.FormatContainerType.TableCell && !this.LastNonEmpty.Node.FirstChild.IsNull)
        this.OpenContainer(Format.FormatContainerType.Block, false);
      if (this.LastNonEmpty.Type != Format.FormatContainerType.ListItem)
      {
        short num = this.state.FontIndex;
        if ((int) this.state.FontIndex != -1 && (int) this.state.FontIndex != (int) this.documentFontIndex && (this.fonts[(int) this.state.FontIndex].Name != null && !this.fonts[(int) this.state.FontIndex].Name.StartsWith("Wingdings", StringComparison.OrdinalIgnoreCase)) && !this.fonts[(int) this.state.FontIndex].Name.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
        {
          if (this.fonts[(int) this.state.FontIndex].Value.IsNull)
            this.RegisterFontValue(this.state.FontIndex);
          this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.FontFace, this.fonts[(int) this.state.FontIndex].Value);
        }
        else
          num = this.documentFontIndex;
        this.containerFontIndex = num;
        if ((int) this.state.FontSize != (int) this.documentFontSize)
          this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.FontSize, new Format.PropertyValue(Format.LengthUnits.Twips, (int) this.state.FontSize));
        this.containerFontSize = this.state.FontSize;
        if (this.state.FontColor != 0)
          this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.FontColor, new Format.PropertyValue(new Format.RGBT((uint) this.state.FontColor)));
        this.containerFontColor = this.state.FontColor;
        if (this.state.ParagraphBackColor != 16777215)
          this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.BackColor, new Format.PropertyValue(new Format.RGBT((uint) this.state.ParagraphBackColor)));
        this.containerBackColor = this.state.ParagraphBackColor;
      }
      if ((int) this.state.SpaceBefore != 0)
        this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.Margins, new Format.PropertyValue(Format.LengthUnits.Twips, (int) this.state.SpaceBefore));
      if ((int) this.state.SpaceAfter != 0)
        this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.BottomMargin, new Format.PropertyValue(Format.LengthUnits.Twips, (int) this.state.SpaceAfter));
      if (this.LastNonEmpty.Type != Format.FormatContainerType.ListItem)
      {
        if ((int) this.state.LeftIndent != 0)
          this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.LeftPadding, new Format.PropertyValue(Format.LengthUnits.Twips, (int) this.state.LeftIndent));
        if ((int) this.state.RightIndent != 0)
          this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.RightPadding, new Format.PropertyValue(Format.LengthUnits.Twips, (int) this.state.RightIndent));
        if ((int) this.state.FirstLineIndent != 0)
          this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.FirstLineIndent, new Format.PropertyValue(Format.LengthUnits.Twips, (int) this.state.FirstLineIndent));
        if (this.state.ParagraphAlignment != RtfAlignment.Left && this.state.ParagraphAlignment < RtfAlignment.Distributed)
          this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.TextAlignment, new Format.PropertyValue((Enum) (Format.TextAlign) this.state.ParagraphAlignment));
        if (this.state.ParagraphRtl)
          this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.RightToLeft, new Format.PropertyValue(this.state.ParagraphRtl));
      }
      this.state.TextPropertiesChanged = true;
      this.beforeContent = false;
    }

    private void AdjustTableLevel(int requiredTableLevel)
    {
      if (-1 != (int) this.currentListState.ListIndex)
        this.CloseList();
      if (this.LastNonEmpty.Type == Format.FormatContainerType.HyperLink)
        this.CloseContainer();
      if (this.LastNonEmpty.Type == Format.FormatContainerType.Block)
        this.CloseContainer();
      if (this.currentTableLevel < requiredTableLevel)
      {
        if (this.LastNonEmpty.Type == Format.FormatContainerType.Table)
          this.OpenRow();
        if (this.LastNonEmpty.Type == Format.FormatContainerType.TableRow)
          this.OpenCell();
        do
        {
          this.OpenTable();
          this.OpenRow();
          this.OpenCell();
        }
        while (this.currentTableLevel < requiredTableLevel);
        this.documentContainerStillOpen = false;
      }
      else
      {
        do
        {
          if (this.LastNonEmpty.Type == Format.FormatContainerType.TableCell)
            this.CloseCell();
          if (this.LastNonEmpty.Type == Format.FormatContainerType.TableRow)
            this.CloseRow(true);
          this.CloseTable();
        }
        while (this.currentTableLevel < requiredTableLevel);
      }
    }

    private void CloseList()
    {
      if (this.LastNonEmpty.Type == Format.FormatContainerType.HyperLink)
        this.CloseContainer();
      if (this.LastNonEmpty.Type == Format.FormatContainerType.Block)
        this.CloseContainer();
      if (this.LastNonEmpty.Type == Format.FormatContainerType.ListItem)
        this.CloseContainer();
      if (this.LastNonEmpty.Type == Format.FormatContainerType.List)
        this.CloseContainer();
      this.currentListState.ListIndex = (short) -1;
    }

    private void OpenList()
    {
      if (this.LastNonEmpty.Type == Format.FormatContainerType.HyperLink)
        this.CloseContainer();
      if (this.LastNonEmpty.Type == Format.FormatContainerType.Block)
        this.CloseContainer();
      if (this.LastNonEmpty.Type == Format.FormatContainerType.ListItem)
        this.CloseContainer();
      if (this.LastNonEmpty.Type == Format.FormatContainerType.List)
        return;
      if (this.LastNonEmpty.Type == Format.FormatContainerType.Table)
        this.OpenRow();
      if (this.LastNonEmpty.Type == Format.FormatContainerType.TableRow)
        this.OpenCell();
      this.OpenContainer(Format.FormatContainerType.List, false);
      this.currentListState.ListIndex = this.state.ListIndex;
      this.currentListState.ListLevel = this.state.ListLevel;
      this.currentListState.ListStyle = this.state.ListStyle;
      this.currentListState.LeftIndent = this.state.LeftIndent;
      this.currentListState.RightIndent = this.state.RightIndent;
      this.currentListState.FirstLineIndent = this.state.FirstLineIndent;
      this.currentListState.Rtl = this.state.ParagraphRtl;
      this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.Margins, new Format.PropertyValue(Format.LengthUnits.Twips, 0));
      this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.RightMargin, new Format.PropertyValue(Format.LengthUnits.Twips, 0));
      this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.BottomMargin, new Format.PropertyValue(Format.LengthUnits.Twips, 0));
      this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.LeftMargin, new Format.PropertyValue(Format.LengthUnits.Twips, 0));
      if ((int) this.lists[(int) this.currentListState.ListIndex].Levels[(int) this.currentListState.ListLevel].Start != 1)
        this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.ListStart, new Format.PropertyValue(Format.PropertyType.Integer, (int) this.lists[(int) this.currentListState.ListIndex].Levels[(int) this.currentListState.ListLevel].Start));
      if (this.state.ParagraphBackColor != 16777215)
        this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.BackColor, new Format.PropertyValue(new Format.RGBT((uint) this.state.ParagraphBackColor)));
      this.containerBackColor = this.state.ParagraphBackColor;
      if (this.state.ListStyle != RtfNumbering.Bullet)
        this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.ListStyle, new Format.PropertyValue((Enum) (Format.ListStyle) this.state.ListStyle));
      if ((int) this.state.LeftIndent != 0)
        this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.LeftPadding, new Format.PropertyValue(Format.LengthUnits.Twips, (int) this.state.LeftIndent));
      if ((int) this.state.RightIndent != 0)
        this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.RightMargin, new Format.PropertyValue(Format.LengthUnits.Twips, (int) this.state.RightIndent));
      if (this.state.ParagraphAlignment != RtfAlignment.Left && this.state.ParagraphAlignment < RtfAlignment.Distributed)
        this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.TextAlignment, new Format.PropertyValue((Enum) (Format.TextAlign) this.state.ParagraphAlignment));
      if (this.state.ParagraphRtl)
        this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.RightToLeft, new Format.PropertyValue(this.state.ParagraphRtl));
      short num = this.state.FontIndex;
      if ((int) this.state.FontIndex != -1 && (int) this.state.FontIndex != (int) this.documentFontIndex && (this.fonts[(int) this.state.FontIndex].Name != null && !this.fonts[(int) this.state.FontIndex].Name.StartsWith("Wingdings", StringComparison.OrdinalIgnoreCase)) && !this.fonts[(int) this.state.FontIndex].Name.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
      {
        if (this.fonts[(int) this.state.FontIndex].Value.IsNull)
          this.RegisterFontValue(this.state.FontIndex);
        this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.FontFace, this.fonts[(int) this.state.FontIndex].Value);
      }
      else
        num = this.documentFontIndex;
      this.containerFontIndex = num;
      if ((int) this.state.FontSize != (int) this.documentFontSize)
        this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.FontSize, new Format.PropertyValue(Format.LengthUnits.Twips, (int) this.state.FontSize));
      this.containerFontSize = this.state.FontSize;
      if (this.state.FontColor != 0)
        this.Last.SetProperty(Format.PropertyPrecedence.InlineStyle, Format.PropertyId.FontColor, new Format.PropertyValue(new Format.RGBT((uint) this.state.FontColor)));
      this.containerFontColor = this.state.FontColor;
      this.documentContainerStillOpen = false;
    }

    private void InitializeFreshRowProperties()
    {
      if (this.state.Destination == RtfDestination.NestTableProps)
      {
        if (this.newRowNested != null)
        {
          this.newRowNested.NextFree = this.firstFreeRow;
          this.firstFreeRow = this.newRowNested;
        }
      }
      else if (this.newRowTopLevel != null)
      {
        this.newRowTopLevel.NextFree = this.firstFreeRow;
        this.firstFreeRow = this.newRowTopLevel;
      }
      if (this.firstFreeRow != null)
      {
        this.newRow = this.firstFreeRow;
        this.firstFreeRow = this.newRow.NextFree;
        this.newRow.NextFree = (RtfFormatConverter.RtfRow) null;
        this.newRow.Initialize();
      }
      else
        this.newRow = new RtfFormatConverter.RtfRow();
      if (this.state.Destination == RtfDestination.NestTableProps)
        this.newRowNested = this.newRow;
      else
        this.newRowTopLevel = this.newRow;
    }

    private void OpenTable()
    {
      if (this.tableStack == null)
        this.tableStack = new RtfFormatConverter.RtfTable[4];
      else if (this.currentTableLevel == this.tableStack.Length)
      {
        RtfFormatConverter.RtfTable[] rtfTableArray = new RtfFormatConverter.RtfTable[this.currentTableLevel * 2];
        Array.Copy((Array) this.tableStack, 0, (Array) rtfTableArray, 0, this.currentTableLevel);
        this.tableStack = rtfTableArray;
      }
      ++this.currentTableLevel;
      this.tableStack[this.currentTableLevel - 1].Initialize();
      this.tableStack[this.currentTableLevel - 1].TableContainer = this.OpenContainer(Format.FormatContainerType.Table, false);
    }

    private void CloseTable()
    {
      Format.FormatNode node1 = this.tableStack[this.currentTableLevel - 1].TableContainer.Node;
      Format.FormatNode node2 = this.CreateNode(Format.FormatContainerType.TableDefinition);
      node1.PrependChild(node2);
      for (int index = 0; index < (int) this.tableStack[this.currentTableLevel - 1].ColumnCount; ++index)
      {
        Format.FormatNode node3 = this.CreateNode(Format.FormatContainerType.TableColumn);
        node3.SetProperty(Format.PropertyId.Width, new Format.PropertyValue(Format.LengthUnits.Twips, (int) this.tableStack[this.currentTableLevel - 1].Columns[index].Width));
        node2.AppendChild(node3);
      }
      this.CloseContainer();
      this.tableStack[this.currentTableLevel - 1].TableContainer = Format.FormatConverterContainer.Null;
      --this.currentTableLevel;
    }

    private void OpenRow()
    {
      if (this.currentTableLevel == 1 && !this.tableStack[this.currentTableLevel - 1].FirstRow && (this.newRowTopLevel != null && (int) this.newRowTopLevel.CellCount != 0) && ((int) this.newRowTopLevel.CellCount != (int) this.tableStack[this.currentTableLevel - 1].ColumnCount || (int) this.newRowTopLevel.Left != (int) this.tableStack[this.currentTableLevel - 1].Left || (int) this.newRowTopLevel.Right - (int) this.newRowTopLevel.Left != (int) this.tableStack[this.currentTableLevel - 1].Width))
      {
        this.CloseTable();
        this.OpenTable();
      }
      this.tableStack[this.currentTableLevel - 1].RowContainer = this.OpenContainer(Format.FormatContainerType.TableRow, false);
      this.tableStack[this.currentTableLevel - 1].CellIndex = (short) -1;
    }

    private void CloseRow(bool closingTable)
    {
      RtfFormatConverter.RtfRow rtfRow;
      if (this.currentTableLevel == 1)
      {
        rtfRow = this.newRowTopLevel;
      }
      else
      {
        rtfRow = this.newRowNested;
        this.newRowNested = (RtfFormatConverter.RtfRow) null;
      }
      int index1 = this.currentTableLevel - 1;
      bool flag = false;
      if (rtfRow != null)
      {
        flag = rtfRow.LastRow;
        Format.FormatNode node1 = this.tableStack[index1].TableContainer.Node;
        Format.FormatNode node2 = this.tableStack[index1].RowContainer.Node;
        if ((int) rtfRow.Height != 0)
          node2.SetProperty(Format.PropertyId.Height, new Format.PropertyValue(Format.LengthUnits.Twips, (int) rtfRow.Height));
        if (this.tableStack[index1].FirstRow)
        {
          if ((int) rtfRow.Left != 0)
            node1.SetProperty(Format.PropertyId.LeftMargin, new Format.PropertyValue(Format.LengthUnits.Twips, (int) rtfRow.Left));
          node1.SetProperty(Format.PropertyId.Width, new Format.PropertyValue(Format.LengthUnits.Twips, (int) rtfRow.Right - (int) rtfRow.Left));
          short num1 = Math.Max(rtfRow.CellCount, (short) ((int) this.tableStack[index1].CellIndex + 1));
          this.tableStack[index1].RightToLeft = rtfRow.RightToLeft;
          this.tableStack[index1].Left = rtfRow.Left;
          this.tableStack[index1].Width = (short) ((int) rtfRow.Right - (int) rtfRow.Left);
          if ((int) rtfRow.WidthType == 2)
            this.tableStack[index1].WidthPercentage = rtfRow.Width;
          this.tableStack[index1].BackColor = rtfRow.BackColor;
          this.tableStack[index1].BorderKind = rtfRow.BorderKind;
          this.tableStack[index1].BorderWidth = rtfRow.BorderWidth;
          this.tableStack[index1].BorderColor = rtfRow.BorderColor;
          this.tableStack[index1].EnsureColumns((int) num1);
          Format.FormatNode formatNode = node2.FirstChild;
          int index2 = 0;
          int num2 = (int) this.tableStack[index1].Left;
          int index3;
          for (index3 = 0; index3 < (int) rtfRow.CellCount && !formatNode.IsNull; ++index3)
          {
            Format.FormatNode nextSibling = formatNode.NextSibling;
            this.tableStack[index1].Columns[index2].Cellx = rtfRow.Cells[index3].Cellx;
            this.tableStack[index1].Columns[index2].Width = (short) ((int) rtfRow.Cells[index3].Cellx - num2);
            if ((int) rtfRow.Cells[index3].WidthType == 2)
              this.tableStack[index1].Columns[index2].WidthPercentage = rtfRow.Cells[index3].Width;
            if (index3 == 0 || !rtfRow.Cells[index3].MergeLeft)
            {
              this.tableStack[index1].Columns[index2].BackColor = rtfRow.Cells[index3].BackColor;
              this.tableStack[index1].Columns[index2].VerticalMergeCell = formatNode;
              if (rtfRow.Cells[index3].BackColor != 16777215)
                formatNode.SetProperty(Format.PropertyId.BackColor, new Format.PropertyValue(new Format.RGBT((uint) rtfRow.Cells[index3].BackColor)));
            }
            else
            {
              this.tableStack[index1].Columns[index2].BackColor = this.tableStack[index1].Columns[index3 - 1].BackColor;
              Format.FormatNode previousSibling = formatNode.PreviousSibling;
              while (!formatNode.FirstChild.IsNull)
              {
                Format.FormatNode firstChild = formatNode.FirstChild;
                firstChild.RemoveFromParent();
                previousSibling.AppendChild(firstChild);
              }
              formatNode.RemoveFromParent();
              int num3 = 1;
              Format.PropertyValue property = previousSibling.GetProperty(Format.PropertyId.NumColumns);
              if (!property.IsNull)
                num3 = property.Integer;
              previousSibling.SetProperty(Format.PropertyId.NumColumns, new Format.PropertyValue(Format.PropertyType.Integer, num3 + 1));
              this.tableStack[index1].Columns[index2].VerticalMergeCell = Format.FormatNode.Null;
            }
            num2 = (int) rtfRow.Cells[index3].Cellx;
            ++index2;
            formatNode = nextSibling;
          }
          this.tableStack[index1].ColumnCount = (short) index3;
        }
        else if ((int) rtfRow.Left == (int) this.tableStack[index1].Left && (int) rtfRow.Right - (int) rtfRow.Left == (int) this.tableStack[index1].Width)
        {
          Format.FormatNode formatNode1 = node2.FirstChild;
          int index2 = 0;
          for (int index3 = 0; index3 < (int) rtfRow.CellCount && !formatNode1.IsNull && index2 < (int) this.tableStack[index1].ColumnCount; ++index3)
          {
            int index4 = index2;
            while (index4 < (int) this.tableStack[index1].ColumnCount && (int) rtfRow.Cells[index3].Cellx > (int) this.tableStack[index1].Columns[index4].Cellx)
            {
              ++index4;
              if (index4 < (int) this.tableStack[index1].ColumnCount)
                this.tableStack[index1].Columns[index4].VerticalMergeCell = Format.FormatNode.Null;
            }
            if (index4 < (int) this.tableStack[index1].ColumnCount && (int) rtfRow.Cells[index3].Cellx < (int) this.tableStack[index1].Columns[index4].Cellx)
            {
              this.tableStack[index1].InsertColumn(index4, rtfRow.Cells[index3].Cellx);
              for (Format.FormatNode formatNode2 = node1.FirstChild; formatNode2 != node1.LastChild; formatNode2 = formatNode2.NextSibling)
              {
                Format.FormatNode formatNode3 = formatNode2.FirstChild;
                int num1 = 0;
                for (; !formatNode3.IsNull; formatNode3 = formatNode3.NextSibling)
                {
                  int num2 = 1;
                  Format.PropertyValue property = formatNode3.GetProperty(Format.PropertyId.NumColumns);
                  if (!property.IsNull)
                    num2 = property.Integer;
                  if (num1 + num2 <= index4)
                  {
                    num1 += num2;
                  }
                  else
                  {
                    formatNode3.SetProperty(Format.PropertyId.NumColumns, new Format.PropertyValue(Format.PropertyType.Integer, num2 + 1));
                    break;
                  }
                }
              }
            }
            Format.FormatNode nextSibling = formatNode1.NextSibling;
            if (index3 != 0 && rtfRow.Cells[index3].MergeLeft)
            {
              this.tableStack[index1].Columns[index2].VerticalMergeCell = Format.FormatNode.Null;
              Format.FormatNode previousSibling = formatNode1.PreviousSibling;
              while (!formatNode1.FirstChild.IsNull)
              {
                Format.FormatNode firstChild = formatNode1.FirstChild;
                firstChild.RemoveFromParent();
                previousSibling.AppendChild(firstChild);
              }
              formatNode1.RemoveFromParent();
              int num = 1;
              Format.PropertyValue property = previousSibling.GetProperty(Format.PropertyId.NumColumns);
              if (!property.IsNull)
                num = property.Integer;
              previousSibling.SetProperty(Format.PropertyId.NumColumns, new Format.PropertyValue(Format.PropertyType.Integer, num + (index4 - index2 + 1)));
            }
            else
            {
              if (index4 != index2)
                formatNode1.SetProperty(Format.PropertyId.NumColumns, new Format.PropertyValue(Format.PropertyType.Integer, index4 - index2 + 1));
              if (rtfRow.Cells[index3].MergeUp)
              {
                Format.FormatNode formatNode2 = this.tableStack[index1].Columns[index2].VerticalMergeCell;
                if (!formatNode2.IsNull)
                {
                  formatNode1.SetProperty(Format.PropertyId.MergedCell, new Format.PropertyValue(true));
                  int num = 1;
                  Format.PropertyValue property = formatNode2.GetProperty(Format.PropertyId.NumRows);
                  if (!property.IsNull)
                    num = property.Integer;
                  formatNode2.SetProperty(Format.PropertyId.NumRows, new Format.PropertyValue(Format.PropertyType.Integer, num + 1));
                }
              }
              else
              {
                this.tableStack[index1].Columns[index2].VerticalMergeCell = formatNode1;
                if (rtfRow.Cells[index3].BackColor != 16777215)
                  formatNode1.SetProperty(Format.PropertyId.BackColor, new Format.PropertyValue(new Format.RGBT((uint) rtfRow.Cells[index3].BackColor)));
              }
            }
            index2 = index4 + 1;
            formatNode1 = nextSibling;
          }
        }
      }
      this.CloseContainer();
      this.tableStack[index1].RowContainer = Format.FormatConverterContainer.Null;
      this.tableStack[index1].FirstRow = false;
      if (closingTable || !flag)
        return;
      this.CloseTable();
    }

    private void OpenCell()
    {
      ++this.tableStack[this.currentTableLevel - 1].CellIndex;
      this.tableStack[this.currentTableLevel - 1].CellContainer = this.OpenContainer(Format.FormatContainerType.TableCell, false);
    }

    private void CloseCell()
    {
      this.CloseContainer();
      this.tableStack[this.currentTableLevel - 1].CellContainer = Format.FormatConverterContainer.Null;
    }

    private void SetBorderKind(RtfBorderKind kind)
    {
      switch (this.borderId)
      {
        case RtfBorderId.Left:
        case RtfBorderId.Top:
        case RtfBorderId.Right:
        case RtfBorderId.Bottom:
          this.state.SetParagraphBorderKind(this.borderId, kind);
          break;
        case RtfBorderId.RowLeft:
        case RtfBorderId.RowTop:
        case RtfBorderId.RowRight:
        case RtfBorderId.RowBottom:
        case RtfBorderId.RowHorizontal:
        case RtfBorderId.RowVertical:
          if (this.newRow == null)
            break;
          this.newRow.SetBorderKind(this.borderId, kind);
          break;
        case RtfBorderId.CellLeft:
        case RtfBorderId.CellTop:
        case RtfBorderId.CellRight:
        case RtfBorderId.CellBottom:
          if (this.newRow == null || !this.newRow.EnsureEntryForCurrentCell())
            break;
          this.newRow.Cells[(int) this.newRow.CellCount].SetBorderKind(this.borderId, kind);
          break;
      }
    }

    private void SetBorderColor(int color)
    {
      switch (this.borderId)
      {
        case RtfBorderId.Left:
        case RtfBorderId.Top:
        case RtfBorderId.Right:
        case RtfBorderId.Bottom:
          this.state.SetParagraphBorderColor(this.borderId, color);
          break;
        case RtfBorderId.RowLeft:
        case RtfBorderId.RowTop:
        case RtfBorderId.RowRight:
        case RtfBorderId.RowBottom:
        case RtfBorderId.RowHorizontal:
        case RtfBorderId.RowVertical:
          if (this.newRow == null)
            break;
          this.newRow.SetBorderColor(this.borderId, color);
          break;
        case RtfBorderId.CellLeft:
        case RtfBorderId.CellTop:
        case RtfBorderId.CellRight:
        case RtfBorderId.CellBottom:
          if (this.newRow == null || !this.newRow.EnsureEntryForCurrentCell())
            break;
          this.newRow.Cells[(int) this.newRow.CellCount].SetBorderColor(this.borderId, color);
          break;
      }
    }

    private void SetBorderWidth(byte width)
    {
      switch (this.borderId)
      {
        case RtfBorderId.Left:
        case RtfBorderId.Top:
        case RtfBorderId.Right:
        case RtfBorderId.Bottom:
          this.state.SetParagraphBorderWidth(this.borderId, width);
          break;
        case RtfBorderId.RowLeft:
        case RtfBorderId.RowTop:
        case RtfBorderId.RowRight:
        case RtfBorderId.RowBottom:
        case RtfBorderId.RowHorizontal:
        case RtfBorderId.RowVertical:
          if (this.newRow == null)
            break;
          this.newRow.SetBorderWidth(this.borderId, width);
          break;
        case RtfBorderId.CellLeft:
        case RtfBorderId.CellTop:
        case RtfBorderId.CellRight:
        case RtfBorderId.CellBottom:
          if (this.newRow == null || !this.newRow.EnsureEntryForCurrentCell())
            break;
          this.newRow.Cells[(int) this.newRow.CellCount].SetBorderWidth(this.borderId, width);
          break;
      }
    }

    internal struct ListLevel
    {
      public RtfNumbering Type;
      public short Start;
    }

    internal struct List
    {
      public int Id;
      public byte LevelCount;
      public RtfFormatConverter.ListLevel[] Levels;
    }

    internal struct RtfFourSideValue<T>
    {
      public T Left;
      public T Top;
      public T Right;
      public T Bottom;

      public void Initialize(T value)
      {
        this.Left = this.Right = this.Top = this.Bottom = value;
      }
    }

    internal struct RtfSixSideValue<T>
    {
      public T Left;
      public T Top;
      public T Right;
      public T Bottom;
      public T Horizontal;
      public T Vertical;

      public void Initialize(T value)
      {
        this.Left = this.Right = this.Top = this.Bottom = this.Horizontal = this.Vertical = value;
      }
    }

    internal struct RtfRowCell
    {
      public bool MergeLeft;
      public bool MergeUp;
      public short Cellx;
      public short Width;
      public byte WidthType;
      public int BackColor;
      public RtfFormatConverter.RtfFourSideValue<byte> BorderWidth;
      public RtfFormatConverter.RtfFourSideValue<int> BorderColor;
      public RtfFormatConverter.RtfFourSideValue<RtfBorderKind> BorderKind;
      public RtfFormatConverter.RtfFourSideValue<short> Padding;
      public RtfVertAlignment VerticalAlignment;

      public void Initialize()
      {
        this.BackColor = 16777215;
        this.Width = (short) 0;
        this.WidthType = (byte) 0;
        this.Cellx = (short) 0;
        this.VerticalAlignment = RtfVertAlignment.Undefined;
        this.MergeLeft = false;
        this.MergeUp = false;
        this.Padding.Initialize((short) -1);
        this.BorderWidth.Initialize(byte.MaxValue);
        this.BorderColor.Initialize(-1);
        this.BorderKind.Initialize(RtfBorderKind.None);
      }

      public void SetBorderKind(RtfBorderId borderId, RtfBorderKind value)
      {
        switch (borderId)
        {
          case RtfBorderId.CellLeft:
            this.BorderKind.Left = value;
            break;
          case RtfBorderId.CellTop:
            this.BorderKind.Top = value;
            break;
          case RtfBorderId.CellRight:
            this.BorderKind.Right = value;
            break;
          case RtfBorderId.CellBottom:
            this.BorderKind.Bottom = value;
            break;
        }
      }

      public void SetBorderWidth(RtfBorderId borderId, byte value)
      {
        switch (borderId)
        {
          case RtfBorderId.CellLeft:
            this.BorderWidth.Left = value;
            break;
          case RtfBorderId.CellTop:
            this.BorderWidth.Top = value;
            break;
          case RtfBorderId.CellRight:
            this.BorderWidth.Right = value;
            break;
          case RtfBorderId.CellBottom:
            this.BorderWidth.Bottom = value;
            break;
        }
      }

      public void SetBorderColor(RtfBorderId borderId, int value)
      {
        switch (borderId)
        {
          case RtfBorderId.CellLeft:
            this.BorderColor.Left = value;
            break;
          case RtfBorderId.CellTop:
            this.BorderColor.Top = value;
            break;
          case RtfBorderId.CellRight:
            this.BorderColor.Right = value;
            break;
          case RtfBorderId.CellBottom:
            this.BorderColor.Bottom = value;
            break;
        }
      }
    }

    internal struct RtfTable
    {
      public bool FirstRow;
      public bool RightToLeft;
      public short Left;
      public short Width;
      public short WidthPercentage;
      public int BackColor;
      public RtfFormatConverter.RtfSixSideValue<byte> BorderWidth;
      public RtfFormatConverter.RtfSixSideValue<int> BorderColor;
      public RtfFormatConverter.RtfSixSideValue<RtfBorderKind> BorderKind;
      public short ColumnCount;
      public RtfFormatConverter.RtfTableColumn[] Columns;
      public short CellIndex;
      public Format.FormatConverterContainer TableContainer;
      public Format.FormatConverterContainer RowContainer;
      public Format.FormatConverterContainer CellContainer;

      public void SetBorderKind(RtfBorderId borderId, RtfBorderKind value)
      {
        switch (borderId)
        {
          case RtfBorderId.RowLeft:
            this.BorderKind.Left = value;
            break;
          case RtfBorderId.RowTop:
            this.BorderKind.Top = value;
            break;
          case RtfBorderId.RowRight:
            this.BorderKind.Right = value;
            break;
          case RtfBorderId.RowBottom:
            this.BorderKind.Bottom = value;
            break;
          case RtfBorderId.RowHorizontal:
            this.BorderKind.Horizontal = value;
            break;
          case RtfBorderId.RowVertical:
            this.BorderKind.Vertical = value;
            break;
        }
      }

      public void SetBorderWidth(RtfBorderId borderId, byte value)
      {
        switch (borderId)
        {
          case RtfBorderId.RowLeft:
            this.BorderWidth.Left = value;
            break;
          case RtfBorderId.RowTop:
            this.BorderWidth.Top = value;
            break;
          case RtfBorderId.RowRight:
            this.BorderWidth.Right = value;
            break;
          case RtfBorderId.RowBottom:
            this.BorderWidth.Bottom = value;
            break;
          case RtfBorderId.RowHorizontal:
            this.BorderWidth.Horizontal = value;
            break;
          case RtfBorderId.RowVertical:
            this.BorderWidth.Vertical = value;
            break;
        }
      }

      public void SetBorderColor(RtfBorderId borderId, int value)
      {
        switch (borderId)
        {
          case RtfBorderId.RowLeft:
            this.BorderColor.Left = value;
            break;
          case RtfBorderId.RowTop:
            this.BorderColor.Top = value;
            break;
          case RtfBorderId.RowRight:
            this.BorderColor.Right = value;
            break;
          case RtfBorderId.RowBottom:
            this.BorderColor.Bottom = value;
            break;
          case RtfBorderId.RowHorizontal:
            this.BorderColor.Horizontal = value;
            break;
          case RtfBorderId.RowVertical:
            this.BorderColor.Vertical = value;
            break;
        }
      }

      public void Initialize()
      {
        this.FirstRow = true;
        this.RightToLeft = false;
        this.Left = (short) 0;
        this.Width = (short) 0;
        this.WidthPercentage = (short) 0;
        this.BackColor = 16777215;
        this.BorderWidth.Initialize((byte) 0);
        this.BorderColor.Initialize(0);
        this.BorderKind.Initialize(RtfBorderKind.None);
        this.ColumnCount = (short) 0;
        if (this.Columns == null)
          return;
        this.InitializeColumns(0);
      }

      public void EnsureColumns(int count)
      {
        if (this.Columns == null)
        {
          this.Columns = new RtfFormatConverter.RtfTableColumn[Math.Max(count, 4)];
          this.InitializeColumns(0);
        }
        else
        {
          if (this.Columns.Length >= count)
            return;
          RtfFormatConverter.RtfTableColumn[] rtfTableColumnArray = new RtfFormatConverter.RtfTableColumn[Math.Max(count, this.Columns.Length * 2)];
          if ((int) this.ColumnCount != 0)
            Array.Copy((Array) this.Columns, 0, (Array) rtfTableColumnArray, 0, (int) this.ColumnCount);
          this.Columns = rtfTableColumnArray;
          this.InitializeColumns((int) this.ColumnCount);
        }
      }

      public void InsertColumn(int index, short cellx)
      {
        this.EnsureColumns((int) this.ColumnCount + 1);
        Array.Copy((Array) this.Columns, index, (Array) this.Columns, index + 1, (int) this.ColumnCount - index);
        ++this.ColumnCount;
        this.Columns[index].Cellx = cellx;
        short num = this.Columns[index].Width;
        this.Columns[index].Width = (short) ((int) this.Columns[index].Cellx - (index == 0 ? (int) this.Left : (int) this.Columns[index - 1].Cellx));
        this.Columns[index + 1].Width = (short) ((int) this.Columns[index + 1].Cellx - (int) cellx);
        if ((int) this.Columns[index].WidthPercentage == 0)
          return;
        this.Columns[index].WidthPercentage = (short) ((int) this.Columns[index].WidthPercentage * (int) this.Columns[index].Width / (int) num);
        this.Columns[index + 1].WidthPercentage = (short) ((int) this.Columns[index + 1].WidthPercentage * (int) this.Columns[index + 1].Width / (int) num);
      }

      private void InitializeColumns(int startIndex)
      {
        for (int index = startIndex; index < this.Columns.Length; ++index)
          this.Columns[index].Initialize();
      }
    }

    internal struct RtfTableColumn
    {
      public short Cellx;
      public short Width;
      public short WidthPercentage;
      public int BackColor;
      public Format.FormatNode VerticalMergeCell;

      public void Initialize()
      {
        this.Cellx = (short) 0;
        this.Width = (short) 0;
        this.WidthPercentage = (short) 0;
        this.BackColor = 16777215;
        this.VerticalMergeCell = Format.FormatNode.Null;
      }
    }

    internal struct RtfState
    {
      public int Level;
      public int SkipLevel;
      private RtfFormatConverter.RtfState.BasicState basic;
      private RtfFormatConverter.RtfState.BasicState[] basicStack;
      private int basicStackTop;
      private RtfFormatConverter.RtfState.FontProperties font;
      private RtfFormatConverter.RtfState.FontProperties[] fontStack;
      private int fontStackTop;
      private RtfFormatConverter.RtfState.ParagraphProperties paragraph;
      private RtfFormatConverter.RtfState.ParagraphProperties[] paragraphStack;
      private int paragraphStackTop;

        public bool TextPropertiesChanged { get; set; }

        public RtfDestination Destination
      {
        get
        {
          return this.basic.Destination;
        }
        set
        {
          if (this.basic.Destination == value)
            return;
          this.DirtyBasicState();
          this.basic.Destination = value;
        }
      }

      public bool BiDi
      {
        get
        {
          return (this.font.Flags & RtfFormatConverter.RtfState.FontFlags.BiDi) != RtfFormatConverter.RtfState.FontFlags.None;
        }
        set
        {
          if (this.BiDi == value)
            return;
          this.DirtyFontProps();
          this.font.Flags ^= RtfFormatConverter.RtfState.FontFlags.BiDi;
        }
      }

      public bool ComplexScript
      {
        get
        {
          return (this.font.Flags & RtfFormatConverter.RtfState.FontFlags.ComplexScript) != RtfFormatConverter.RtfState.FontFlags.None;
        }
        set
        {
          if (this.ComplexScript == value)
            return;
          this.DirtyFontProps();
          this.font.Flags ^= RtfFormatConverter.RtfState.FontFlags.ComplexScript;
        }
      }

      public bool Bold
      {
        get
        {
          return (this.font.Flags & RtfFormatConverter.RtfState.FontFlags.Bold) != RtfFormatConverter.RtfState.FontFlags.None;
        }
        set
        {
          if (this.Bold == value)
            return;
          this.DirtyFontProps();
          this.font.Flags ^= RtfFormatConverter.RtfState.FontFlags.Bold;
        }
      }

      public bool Italic
      {
        get
        {
          return (this.font.Flags & RtfFormatConverter.RtfState.FontFlags.Italic) != RtfFormatConverter.RtfState.FontFlags.None;
        }
        set
        {
          if (this.Italic == value)
            return;
          this.DirtyFontProps();
          this.font.Flags ^= RtfFormatConverter.RtfState.FontFlags.Italic;
        }
      }

      public bool Underline
      {
        get
        {
          return (this.font.Flags & RtfFormatConverter.RtfState.FontFlags.Underline) != RtfFormatConverter.RtfState.FontFlags.None;
        }
        set
        {
          if (this.Underline == value)
            return;
          this.DirtyFontProps();
          this.font.Flags ^= RtfFormatConverter.RtfState.FontFlags.Underline;
        }
      }

      public bool Subscript
      {
        get
        {
          return (this.font.Flags & RtfFormatConverter.RtfState.FontFlags.Subscript) != RtfFormatConverter.RtfState.FontFlags.None;
        }
        set
        {
          if (this.Subscript == value)
            return;
          this.DirtyFontProps();
          this.font.Flags ^= RtfFormatConverter.RtfState.FontFlags.Subscript;
          if (!value)
            return;
          this.Superscript = false;
        }
      }

      public bool Superscript
      {
        get
        {
          return (this.font.Flags & RtfFormatConverter.RtfState.FontFlags.Superscript) != RtfFormatConverter.RtfState.FontFlags.None;
        }
        set
        {
          if (this.Superscript == value)
            return;
          this.DirtyFontProps();
          this.font.Flags ^= RtfFormatConverter.RtfState.FontFlags.Superscript;
          if (!value)
            return;
          this.Subscript = false;
        }
      }

      public bool Strikethrough
      {
        get
        {
          return (this.font.Flags & RtfFormatConverter.RtfState.FontFlags.Strikethrough) != RtfFormatConverter.RtfState.FontFlags.None;
        }
        set
        {
          if (this.Strikethrough == value)
            return;
          this.DirtyFontProps();
          this.font.Flags ^= RtfFormatConverter.RtfState.FontFlags.Strikethrough;
        }
      }

      public bool SmallCaps
      {
        get
        {
          return (this.font.Flags & RtfFormatConverter.RtfState.FontFlags.SmallCaps) != RtfFormatConverter.RtfState.FontFlags.None;
        }
        set
        {
          if (this.SmallCaps == value)
            return;
          this.DirtyFontProps();
          this.font.Flags ^= RtfFormatConverter.RtfState.FontFlags.SmallCaps;
        }
      }

      public bool Capitalize
      {
        get
        {
          return (this.font.Flags & RtfFormatConverter.RtfState.FontFlags.Capitalize) != RtfFormatConverter.RtfState.FontFlags.None;
        }
        set
        {
          if (this.Capitalize == value)
            return;
          this.DirtyFontProps();
          this.font.Flags ^= RtfFormatConverter.RtfState.FontFlags.Capitalize;
        }
      }

      public bool Invisible
      {
        get
        {
          return (this.font.Flags & RtfFormatConverter.RtfState.FontFlags.Invisible) != RtfFormatConverter.RtfState.FontFlags.None;
        }
        set
        {
          if (this.Invisible == value)
            return;
          this.DirtyFontProps();
          this.font.Flags ^= RtfFormatConverter.RtfState.FontFlags.Invisible;
        }
      }

      public short FontIndex
      {
        get
        {
          return this.font.FontIndex;
        }
        set
        {
          if ((int) this.font.FontIndex == (int) value)
            return;
          this.DirtyFontProps();
          this.font.FontIndex = value;
        }
      }

      public short FontSize
      {
        get
        {
          return this.font.FontSize;
        }
        set
        {
          if ((int) this.font.FontSize == (int) value)
            return;
          this.DirtyFontProps();
          this.font.FontSize = value;
        }
      }

      public int FontColor
      {
        get
        {
          return this.font.FontColor;
        }
        set
        {
          if (this.font.FontColor == value)
            return;
          this.DirtyFontProps();
          this.font.FontColor = value;
        }
      }

      public int FontBackColor
      {
        get
        {
          return this.font.FontBackColor;
        }
        set
        {
          if (this.font.FontBackColor == value)
            return;
          this.DirtyFontProps();
          this.font.FontBackColor = value;
        }
      }

      public short Language
      {
        get
        {
          return this.font.Language;
        }
        set
        {
          if ((int) this.font.Language == (int) value)
            return;
          this.DirtyFontProps();
          this.font.Language = value;
        }
      }

      public bool Preformatted
      {
        get
        {
          return (this.paragraph.Flags & RtfFormatConverter.RtfState.ParagraphFlags.Preformatted) != RtfFormatConverter.RtfState.ParagraphFlags.None;
        }
        set
        {
          if (this.Preformatted == value)
            return;
          this.DirtyParagraphProps();
          this.paragraph.Flags ^= RtfFormatConverter.RtfState.ParagraphFlags.Preformatted;
        }
      }

      public bool ParagraphRtl
      {
        get
        {
          return (this.paragraph.Flags & RtfFormatConverter.RtfState.ParagraphFlags.ParagraphRtl) != RtfFormatConverter.RtfState.ParagraphFlags.None;
        }
        set
        {
          if (this.ParagraphRtl == value)
            return;
          this.DirtyParagraphProps();
          this.paragraph.Flags ^= RtfFormatConverter.RtfState.ParagraphFlags.ParagraphRtl;
        }
      }

      public short FirstLineIndent
      {
        get
        {
          return this.paragraph.FirstLineIndent;
        }
        set
        {
          if ((int) this.paragraph.FirstLineIndent == (int) value)
            return;
          this.DirtyParagraphProps();
          this.paragraph.FirstLineIndent = value;
        }
      }

      public short LeftIndent
      {
        get
        {
          return this.paragraph.LeftIndent;
        }
        set
        {
          if ((int) this.paragraph.LeftIndent == (int) value)
            return;
          this.DirtyParagraphProps();
          this.paragraph.LeftIndent = value;
        }
      }

      public short RightIndent
      {
        get
        {
          return this.paragraph.RightIndent;
        }
        set
        {
          if ((int) this.paragraph.RightIndent == (int) value)
            return;
          this.DirtyParagraphProps();
          this.paragraph.RightIndent = value;
        }
      }

      public short SpaceBefore
      {
        get
        {
          return this.paragraph.SpaceBefore;
        }
        set
        {
          if ((int) this.paragraph.SpaceBefore == (int) value)
            return;
          this.DirtyParagraphProps();
          this.paragraph.SpaceBefore = value;
        }
      }

      public short SpaceAfter
      {
        get
        {
          return this.paragraph.SpaceAfter;
        }
        set
        {
          if ((int) this.paragraph.SpaceAfter == (int) value)
            return;
          this.DirtyParagraphProps();
          this.paragraph.SpaceAfter = value;
        }
      }

      public RtfAlignment ParagraphAlignment
      {
        get
        {
          return this.paragraph.Alignment;
        }
        set
        {
          if (this.paragraph.Alignment == value)
            return;
          this.DirtyParagraphProps();
          this.paragraph.Alignment = value;
        }
      }

      public short ListIndex
      {
        get
        {
          return this.paragraph.ListIndex;
        }
        set
        {
          if ((int) this.paragraph.ListIndex == (int) value)
            return;
          this.DirtyParagraphProps();
          this.paragraph.ListIndex = value;
        }
      }

      public byte ListLevel
      {
        get
        {
          return this.paragraph.ListLevel;
        }
        set
        {
          if ((int) this.paragraph.ListLevel == (int) value)
            return;
          this.DirtyParagraphProps();
          this.paragraph.ListLevel = value;
        }
      }

      public RtfNumbering ListStyle
      {
        get
        {
          return this.paragraph.ListStyle;
        }
        set
        {
          if (this.paragraph.ListStyle == value)
            return;
          this.DirtyParagraphProps();
          this.paragraph.ListStyle = value;
        }
      }

      public int ParagraphBackColor
      {
        get
        {
          return this.paragraph.BackColor;
        }
        set
        {
          if (this.paragraph.BackColor == value)
            return;
          this.DirtyParagraphProps();
          this.paragraph.BackColor = value;
        }
      }

      public byte BorderWidthLeft
      {
        get
        {
          return this.paragraph.BorderWidth.Left;
        }
        set
        {
          if ((int) this.paragraph.BorderWidth.Left == (int) value)
            return;
          this.DirtyParagraphProps();
          this.paragraph.BorderWidth.Left = value;
        }
      }

      public byte BorderWidthRight
      {
        get
        {
          return this.paragraph.BorderWidth.Right;
        }
        set
        {
          if ((int) this.paragraph.BorderWidth.Right == (int) value)
            return;
          this.DirtyParagraphProps();
          this.paragraph.BorderWidth.Right = value;
        }
      }

      public byte BorderWidthTop
      {
        get
        {
          return this.paragraph.BorderWidth.Top;
        }
        set
        {
          if ((int) this.paragraph.BorderWidth.Top == (int) value)
            return;
          this.DirtyParagraphProps();
          this.paragraph.BorderWidth.Top = value;
        }
      }

      public byte BorderWidthBottom
      {
        get
        {
          return this.paragraph.BorderWidth.Bottom;
        }
        set
        {
          if ((int) this.paragraph.BorderWidth.Bottom == (int) value)
            return;
          this.DirtyParagraphProps();
          this.paragraph.BorderWidth.Bottom = value;
        }
      }

      public int BorderColorLeft
      {
        get
        {
          return this.paragraph.BorderColor.Left;
        }
        set
        {
          if (this.paragraph.BorderColor.Left == value)
            return;
          this.DirtyParagraphProps();
          this.paragraph.BorderColor.Left = value;
        }
      }

      public int BorderColorRight
      {
        get
        {
          return this.paragraph.BorderColor.Right;
        }
        set
        {
          if (this.paragraph.BorderColor.Right == value)
            return;
          this.DirtyParagraphProps();
          this.paragraph.BorderColor.Right = value;
        }
      }

      public int BorderColorTop
      {
        get
        {
          return this.paragraph.BorderColor.Top;
        }
        set
        {
          if (this.paragraph.BorderColor.Top == value)
            return;
          this.DirtyParagraphProps();
          this.paragraph.BorderColor.Top = value;
        }
      }

      public int BorderColorBottom
      {
        get
        {
          return this.paragraph.BorderColor.Bottom;
        }
        set
        {
          if (this.paragraph.BorderColor.Bottom == value)
            return;
          this.DirtyParagraphProps();
          this.paragraph.BorderColor.Bottom = value;
        }
      }

      public RtfBorderKind BorderKindLeft
      {
        get
        {
          return this.paragraph.BorderKind.Left;
        }
        set
        {
          if (this.paragraph.BorderKind.Left == value)
            return;
          this.DirtyParagraphProps();
          this.paragraph.BorderKind.Left = value;
        }
      }

      public RtfBorderKind BorderKindRight
      {
        get
        {
          return this.paragraph.BorderKind.Right;
        }
        set
        {
          if (this.paragraph.BorderKind.Right == value)
            return;
          this.DirtyParagraphProps();
          this.paragraph.BorderKind.Right = value;
        }
      }

      public RtfBorderKind BorderKindTop
      {
        get
        {
          return this.paragraph.BorderKind.Top;
        }
        set
        {
          if (this.paragraph.BorderKind.Top == value)
            return;
          this.DirtyParagraphProps();
          this.paragraph.BorderKind.Top = value;
        }
      }

      public RtfBorderKind BorderKindBottom
      {
        get
        {
          return this.paragraph.BorderKind.Bottom;
        }
        set
        {
          if (this.paragraph.BorderKind.Bottom == value)
            return;
          this.DirtyParagraphProps();
          this.paragraph.BorderKind.Bottom = value;
        }
      }

      public RtfDestination ParentDestination
      {
        get
        {
          if ((int) this.basic.Depth <= 1 && this.basicStackTop != 0)
            return this.basicStack[this.basicStackTop - 1].Destination;
          return this.basic.Destination;
        }
      }

      public short ParentFontIndex
      {
        get
        {
          if ((int) this.font.Depth <= 1 && this.fontStackTop != 0)
            return this.fontStack[this.fontStackTop - 1].FontIndex;
          return this.font.FontIndex;
        }
      }

      public void Initialize()
      {
        this.basic.Depth = (short) 1;
        this.font.Depth = (short) 1;
        this.paragraph.Depth = (short) 1;
        this.SetParagraphDefault();
        this.SetPlain();
        this.Destination = RtfDestination.RTF;
      }

      public byte GetParagraphBorderWidth(RtfBorderId borderId)
      {
        switch (borderId)
        {
          case RtfBorderId.Top:
            return this.BorderWidthTop;
          case RtfBorderId.Right:
            return this.BorderWidthRight;
          case RtfBorderId.Bottom:
            return this.BorderWidthBottom;
          default:
            return this.BorderWidthLeft;
        }
      }

      public void SetParagraphBorderWidth(RtfBorderId borderId, byte value)
      {
        switch (borderId)
        {
          case RtfBorderId.Left:
            this.BorderWidthLeft = value;
            break;
          case RtfBorderId.Top:
            this.BorderWidthTop = value;
            break;
          case RtfBorderId.Right:
            this.BorderWidthRight = value;
            break;
          case RtfBorderId.Bottom:
            this.BorderWidthBottom = value;
            break;
        }
      }

      public int GetParagraphBorderColor(RtfBorderId borderId)
      {
        switch (borderId)
        {
          case RtfBorderId.Top:
            return this.BorderColorTop;
          case RtfBorderId.Right:
            return this.BorderColorRight;
          case RtfBorderId.Bottom:
            return this.BorderColorBottom;
          default:
            return this.BorderColorLeft;
        }
      }

      public void SetParagraphBorderColor(RtfBorderId borderId, int value)
      {
        switch (borderId)
        {
          case RtfBorderId.Left:
            this.BorderColorLeft = value;
            break;
          case RtfBorderId.Top:
            this.BorderColorTop = value;
            break;
          case RtfBorderId.Right:
            this.BorderColorRight = value;
            break;
          case RtfBorderId.Bottom:
            this.BorderColorBottom = value;
            break;
        }
      }

      public RtfBorderKind GetParagraphBorderKind(RtfBorderId borderId)
      {
        switch (borderId)
        {
          case RtfBorderId.Top:
            return this.BorderKindTop;
          case RtfBorderId.Right:
            return this.BorderKindRight;
          case RtfBorderId.Bottom:
            return this.BorderKindBottom;
          default:
            return this.BorderKindLeft;
        }
      }

      public void SetParagraphBorderKind(RtfBorderId borderId, RtfBorderKind value)
      {
        switch (borderId)
        {
          case RtfBorderId.Left:
            this.BorderKindLeft = value;
            break;
          case RtfBorderId.Top:
            this.BorderKindTop = value;
            break;
          case RtfBorderId.Right:
            this.BorderKindRight = value;
            break;
          case RtfBorderId.Bottom:
            this.BorderKindBottom = value;
            break;
        }
      }

      public void SetParentFontIndex(short index)
      {
        this.font.FontIndex = index;
        if (this.fontStackTop <= 0)
          return;
        this.fontStack[this.fontStackTop - 1].FontIndex = index;
      }

      public void SetPlain()
      {
        this.DirtyFontProps();
        this.font.Flags = RtfFormatConverter.RtfState.FontFlags.None;
        this.font.FontIndex = (short) 0;
        this.font.FontSize = RtfFormatConverter.TwelvePointsInTwips;
        this.font.Language = (short) 0;
        this.font.FontColor = 0;
        this.font.FontBackColor = 16777215;
      }

      public void SetParagraphDefault()
      {
        this.DirtyParagraphProps();
        this.paragraph.Flags = RtfFormatConverter.RtfState.ParagraphFlags.None;
        this.paragraph.Alignment = RtfAlignment.Left;
        this.paragraph.SpaceBefore = (short) 0;
        this.paragraph.SpaceAfter = (short) 0;
        this.paragraph.LeftIndent = (short) 0;
        this.paragraph.RightIndent = (short) 0;
        this.paragraph.FirstLineIndent = (short) 0;
        this.paragraph.ListIndex = (short) -1;
        this.paragraph.ListLevel = (byte) 0;
        this.paragraph.ListStyle = RtfNumbering.None;
        this.paragraph.BackColor = 16777215;
        this.paragraph.BorderWidth.Initialize((byte) 0);
        this.paragraph.BorderColor.Initialize(-1);
        this.paragraph.BorderKind.Initialize(RtfBorderKind.None);
      }

      public void Push()
      {
        ++this.Level;
        ++this.basic.Depth;
        ++this.font.Depth;
        ++this.paragraph.Depth;
        if ((int) this.basic.Depth == (int) short.MaxValue)
          this.PushBasicState();
        if ((int) this.font.Depth == (int) short.MaxValue)
          this.PushFontProps();
        if ((int) this.paragraph.Depth != (int) short.MaxValue)
          return;
        this.PushParagraphProps();
      }

      public void Pop()
      {
        if (this.Level <= 1)
          return;
        if ((int) --this.basic.Depth == 0)
          this.basic = this.basicStack[--this.basicStackTop];
        if ((int) --this.font.Depth == 0)
        {
          this.font = this.fontStack[--this.fontStackTop];
          this.TextPropertiesChanged = true;
        }
        if ((int) --this.paragraph.Depth == 0)
          this.paragraph = this.paragraphStack[--this.paragraphStackTop];
        --this.Level;
      }

      public void SkipGroup()
      {
        this.SkipLevel = this.Level;
      }

      private void DirtyFontProps()
      {
        this.TextPropertiesChanged = true;
        if ((int) this.font.Depth <= 1)
          return;
        this.PushFontProps();
      }

      private void PushFontProps()
      {
        if (this.fontStackTop >= 4096)
          throw new TextConvertersException(CtsResources.TextConvertersStrings.InputDocumentTooComplex);
        if (this.fontStack == null || this.fontStackTop == this.fontStack.Length)
        {
          RtfFormatConverter.RtfState.FontProperties[] fontPropertiesArray;
          if (this.fontStack != null)
          {
            fontPropertiesArray = new RtfFormatConverter.RtfState.FontProperties[this.fontStackTop * 2];
            Array.Copy((Array) this.fontStack, 0, (Array) fontPropertiesArray, 0, this.fontStackTop);
          }
          else
            fontPropertiesArray = new RtfFormatConverter.RtfState.FontProperties[8];
          this.fontStack = fontPropertiesArray;
        }
        this.fontStack[this.fontStackTop] = this.font;
        --this.fontStack[this.fontStackTop].Depth;
        ++this.fontStackTop;
        this.font.Depth = (short) 1;
      }

      private void DirtyParagraphProps()
      {
        if ((int) this.paragraph.Depth <= 1)
          return;
        this.PushParagraphProps();
      }

      private void PushParagraphProps()
      {
        if (this.paragraphStackTop >= 4096)
          throw new TextConvertersException(CtsResources.TextConvertersStrings.InputDocumentTooComplex);
        if (this.paragraphStack == null || this.paragraphStackTop == this.paragraphStack.Length)
        {
          RtfFormatConverter.RtfState.ParagraphProperties[] paragraphPropertiesArray;
          if (this.paragraphStack != null)
          {
            paragraphPropertiesArray = new RtfFormatConverter.RtfState.ParagraphProperties[this.paragraphStackTop * 2];
            Array.Copy((Array) this.paragraphStack, 0, (Array) paragraphPropertiesArray, 0, this.paragraphStackTop);
          }
          else
            paragraphPropertiesArray = new RtfFormatConverter.RtfState.ParagraphProperties[4];
          this.paragraphStack = paragraphPropertiesArray;
        }
        this.paragraphStack[this.paragraphStackTop] = this.paragraph;
        --this.paragraphStack[this.paragraphStackTop].Depth;
        ++this.paragraphStackTop;
        this.paragraph.Depth = (short) 1;
      }

      private void DirtyBasicState()
      {
        if ((int) this.basic.Depth <= 1)
          return;
        this.PushBasicState();
      }

      private void PushBasicState()
      {
        if (this.basicStackTop >= 4096)
          throw new TextConvertersException(CtsResources.TextConvertersStrings.InputDocumentTooComplex);
        if (this.basicStack == null || this.basicStackTop == this.basicStack.Length)
        {
          RtfFormatConverter.RtfState.BasicState[] basicStateArray;
          if (this.basicStack != null)
          {
            basicStateArray = new RtfFormatConverter.RtfState.BasicState[this.basicStackTop * 2];
            Array.Copy((Array) this.basicStack, 0, (Array) basicStateArray, 0, this.basicStackTop);
          }
          else
            basicStateArray = new RtfFormatConverter.RtfState.BasicState[16];
          this.basicStack = basicStateArray;
        }
        this.basicStack[this.basicStackTop] = this.basic;
        --this.basicStack[this.basicStackTop].Depth;
        ++this.basicStackTop;
        this.basic.Depth = (short) 1;
      }

      internal enum FontFlagsIndex : byte
      {
        BiDi,
        ComplexScript,
        Bold,
        Italic,
        Underline,
        Subscript,
        Superscript,
        Strikethrough,
        SmallCaps,
        Capitalize,
        Invisible,
      }

      [Flags]
      internal enum FontFlags : ushort
      {
        None = (ushort) 0,
        BiDi = (ushort) 1,
        ComplexScript = (ushort) 2,
        Bold = (ushort) 4,
        Italic = (ushort) 8,
        Underline = (ushort) 16,
        Subscript = (ushort) 32,
        Superscript = (ushort) 64,
        Strikethrough = (ushort) 128,
        SmallCaps = (ushort) 256,
        Capitalize = (ushort) 512,
        Invisible = (ushort) 1024,
      }

      internal enum ParagraphFlagsIndex : byte
      {
        ParagraphRtl,
        Preformatted,
      }

      [Flags]
      internal enum ParagraphFlags : byte
      {
        None = (byte) 0,
        ParagraphRtl = (byte) 1,
        Preformatted = (byte) 2,
      }

      internal struct BasicState
      {
        public short Depth;
        public RtfDestination Destination;
      }

      internal struct FontProperties
      {
        public short Depth;
        public RtfFormatConverter.RtfState.FontFlags Flags;
        public short FontIndex;
        public short FontSize;
        public short Language;
        public int FontColor;
        public int FontBackColor;
      }

      internal struct ParagraphProperties
      {
        public short Depth;
        public RtfFormatConverter.RtfState.ParagraphFlags Flags;
        public RtfAlignment Alignment;
        public short FirstLineIndent;
        public short LeftIndent;
        public short RightIndent;
        public short SpaceBefore;
        public short SpaceAfter;
        public RtfFormatConverter.RtfFourSideValue<byte> BorderWidth;
        public RtfFormatConverter.RtfFourSideValue<int> BorderColor;
        public RtfFormatConverter.RtfFourSideValue<RtfBorderKind> BorderKind;
        public int BackColor;
        public short ListIndex;
        public byte ListLevel;
        public RtfNumbering ListStyle;
      }
    }

    private struct ListState
    {
      public short ListIndex;
      public byte ListLevel;
      public RtfNumbering ListStyle;
      public bool Rtl;
      public short LeftIndent;
      public short RightIndent;
      public short FirstLineIndent;
    }

    internal class RtfRow
    {
      public RtfFormatConverter.RtfRow NextFree;
      public short CellCount;
      public RtfFormatConverter.RtfRowCell[] Cells;
      public bool RightToLeft;
      public bool HeaderRow;
      public bool LastRow;
      public short Left;
      public short Right;
      public short Width;
      public byte WidthType;
      public short Height;
      public bool HeightExact;
      public int BackColor;
      public RtfFormatConverter.RtfFourSideValue<short> CellPadding;
      public RtfFormatConverter.RtfSixSideValue<byte> BorderWidth;
      public RtfFormatConverter.RtfSixSideValue<int> BorderColor;
      public RtfFormatConverter.RtfSixSideValue<RtfBorderKind> BorderKind;

      public RtfRow()
      {
        this.BackColor = 16777215;
      }

      public void Initialize()
      {
        this.CellCount = (short) 0;
        if (this.Cells != null)
          this.InitializeCells(0);
        this.RightToLeft = false;
        this.HeaderRow = false;
        this.LastRow = false;
        this.Left = (short) 0;
        this.Right = (short) 0;
        this.Width = (short) 0;
        this.WidthType = (byte) 0;
        this.Height = (short) 0;
        this.HeightExact = false;
        this.BackColor = 16777215;
        this.CellPadding.Initialize((short) 0);
        this.BorderWidth.Initialize((byte) 0);
        this.BorderColor.Initialize(0);
        this.BorderKind.Initialize(RtfBorderKind.None);
      }

      public bool EnsureEntryForCurrentCell()
      {
        if (this.Cells == null)
        {
          this.Cells = new RtfFormatConverter.RtfRowCell[4];
          this.InitializeCells(0);
        }
        else if ((int) this.CellCount == this.Cells.Length)
        {
          if ((int) this.CellCount >= 64)
            return false;
          RtfFormatConverter.RtfRowCell[] rtfRowCellArray = new RtfFormatConverter.RtfRowCell[this.Cells.Length * 2];
          Array.Copy((Array) this.Cells, 0, (Array) rtfRowCellArray, 0, (int) this.CellCount);
          this.Cells = rtfRowCellArray;
          this.InitializeCells((int) this.CellCount);
        }
        return true;
      }

      public void SetBorderKind(RtfBorderId borderId, RtfBorderKind value)
      {
        switch (borderId)
        {
          case RtfBorderId.RowLeft:
            this.BorderKind.Left = value;
            break;
          case RtfBorderId.RowTop:
            this.BorderKind.Top = value;
            break;
          case RtfBorderId.RowRight:
            this.BorderKind.Right = value;
            break;
          case RtfBorderId.RowBottom:
            this.BorderKind.Bottom = value;
            break;
          case RtfBorderId.RowHorizontal:
            this.BorderKind.Horizontal = value;
            break;
          case RtfBorderId.RowVertical:
            this.BorderKind.Vertical = value;
            break;
        }
      }

      public void SetBorderWidth(RtfBorderId borderId, byte value)
      {
        switch (borderId)
        {
          case RtfBorderId.RowLeft:
            this.BorderWidth.Left = value;
            break;
          case RtfBorderId.RowTop:
            this.BorderWidth.Top = value;
            break;
          case RtfBorderId.RowRight:
            this.BorderWidth.Right = value;
            break;
          case RtfBorderId.RowBottom:
            this.BorderWidth.Bottom = value;
            break;
          case RtfBorderId.RowHorizontal:
            this.BorderWidth.Horizontal = value;
            break;
          case RtfBorderId.RowVertical:
            this.BorderWidth.Vertical = value;
            break;
        }
      }

      public void SetBorderColor(RtfBorderId borderId, int value)
      {
        switch (borderId)
        {
          case RtfBorderId.RowLeft:
            this.BorderColor.Left = value;
            break;
          case RtfBorderId.RowTop:
            this.BorderColor.Top = value;
            break;
          case RtfBorderId.RowRight:
            this.BorderColor.Right = value;
            break;
          case RtfBorderId.RowBottom:
            this.BorderColor.Bottom = value;
            break;
          case RtfBorderId.RowHorizontal:
            this.BorderColor.Horizontal = value;
            break;
          case RtfBorderId.RowVertical:
            this.BorderColor.Vertical = value;
            break;
        }
      }

      private void InitializeCells(int startIndex)
      {
        for (int index = startIndex; index < this.Cells.Length; ++index)
          this.Cells[index].Initialize();
      }
    }
  }
}
