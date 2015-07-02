// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Css.CssData
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Css
{
  internal static class CssData
  {
    public const short MAX_NAME = (short) 26;
    public const short MAX_TAG_NAME = (short) 26;
    public const short NAME_HASH_SIZE = (short) 329;
    public const int NAME_HASH_MODIFIER = 2;
    public static CssNameIndex[] nameHashTable;
    public static CssData.NameDef[] names;
    public static CssNameIndex[] nameIndex;
    public static CssData.FilterActionEntry[] filterInstructions;

    static CssData()
    {
      CssNameIndex[] cssNameIndexArray = new CssNameIndex[329];
      cssNameIndexArray[0] = CssNameIndex.ScrollbarArrowColor;
      cssNameIndexArray[2] = CssNameIndex.WhiteSpace;
      cssNameIndexArray[4] = CssNameIndex.LineBreak;
      cssNameIndexArray[5] = CssNameIndex.Orphans;
      cssNameIndexArray[10] = CssNameIndex.WritingMode;
      cssNameIndexArray[12] = CssNameIndex.Scrollbar3dLightColor;
      cssNameIndexArray[14] = CssNameIndex.TextAutospace;
      cssNameIndexArray[15] = CssNameIndex.VerticalAlign;
      cssNameIndexArray[18] = CssNameIndex.BorderRight;
      cssNameIndexArray[21] = CssNameIndex.Bottom;
      cssNameIndexArray[22] = CssNameIndex.LineHeight;
      cssNameIndexArray[25] = CssNameIndex.BorderBottom;
      cssNameIndexArray[32] = CssNameIndex.ScrollbarBaseColor;
      cssNameIndexArray[33] = CssNameIndex.MinWidth;
      cssNameIndexArray[34] = CssNameIndex.BackgroundColor;
      cssNameIndexArray[36] = CssNameIndex.BorderTopStyle;
      cssNameIndexArray[38] = CssNameIndex.EmptyCells;
      cssNameIndexArray[40] = CssNameIndex.ListStyleType;
      cssNameIndexArray[45] = CssNameIndex.TextAlign;
      cssNameIndexArray[47] = CssNameIndex.FontWeight;
      cssNameIndexArray[49] = CssNameIndex.OutlineWidth;
      cssNameIndexArray[50] = CssNameIndex.CaptionSide;
      cssNameIndexArray[51] = CssNameIndex.ScrollbarShadowColor;
      cssNameIndexArray[55] = CssNameIndex.Clip;
      cssNameIndexArray[57] = CssNameIndex.MarginLeft;
      cssNameIndexArray[58] = CssNameIndex.BorderTopWidth;
      cssNameIndexArray[61] = CssNameIndex.Azimuth;
      cssNameIndexArray[62] = CssNameIndex.Float;
      cssNameIndexArray[66] = CssNameIndex.LayoutFlow;
      cssNameIndexArray[67] = CssNameIndex.MinHeight;
      cssNameIndexArray[68] = CssNameIndex.Content;
      cssNameIndexArray[70] = CssNameIndex.Padding;
      cssNameIndexArray[71] = CssNameIndex.BorderBottomWidth;
      cssNameIndexArray[74] = CssNameIndex.Visibility;
      cssNameIndexArray[76] = CssNameIndex.Overflow;
      cssNameIndexArray[77] = CssNameIndex.BorderLeftColor;
      cssNameIndexArray[80] = CssNameIndex.Pitch;
      cssNameIndexArray[81] = CssNameIndex.Pause;
      cssNameIndexArray[89] = CssNameIndex.OverflowY;
      cssNameIndexArray[93] = CssNameIndex.ScrollbarHighlightColor;
      cssNameIndexArray[95] = CssNameIndex.Height;
      cssNameIndexArray[97] = CssNameIndex.WordWrap;
      cssNameIndexArray[104] = CssNameIndex.Top;
      cssNameIndexArray[105] = CssNameIndex.ListStyle;
      cssNameIndexArray[107] = CssNameIndex.Margin;
      cssNameIndexArray[109] = CssNameIndex.TextKashidaSpace;
      cssNameIndexArray[110] = CssNameIndex.VoiceFamily;
      cssNameIndexArray[111] = CssNameIndex.CueBefore;
      cssNameIndexArray[112] = CssNameIndex.Clear;
      cssNameIndexArray[116] = CssNameIndex.TextOverflow;
      cssNameIndexArray[125] = CssNameIndex.BorderBottomStyle;
      cssNameIndexArray[128] = CssNameIndex.BorderColor;
      cssNameIndexArray[129] = CssNameIndex.TextDecoration;
      cssNameIndexArray[130] = CssNameIndex.Display;
      cssNameIndexArray[136] = CssNameIndex.CounterReset;
      cssNameIndexArray[137] = CssNameIndex.MarginBottom;
      cssNameIndexArray[138] = CssNameIndex.BorderStyle;
      cssNameIndexArray[142] = CssNameIndex.LayoutGrid;
      cssNameIndexArray[143] = CssNameIndex.Quotes;
      cssNameIndexArray[147] = CssNameIndex.Accelerator;
      cssNameIndexArray[148] = CssNameIndex.Border;
      cssNameIndexArray[151] = CssNameIndex.Zoom;
      cssNameIndexArray[154] = CssNameIndex.OutlineStyle;
      cssNameIndexArray[156] = CssNameIndex.Width;
      cssNameIndexArray[158] = CssNameIndex.Color;
      cssNameIndexArray[163] = CssNameIndex.PageBreakInside;
      cssNameIndexArray[165] = CssNameIndex.PitchRange;
      cssNameIndexArray[166] = CssNameIndex.BorderCollapse;
      cssNameIndexArray[167] = CssNameIndex.Cue;
      cssNameIndexArray[169] = CssNameIndex.Left;
      cssNameIndexArray[170] = CssNameIndex.LayoutGridMode;
      cssNameIndexArray[173] = CssNameIndex.SpeakPunctuation;
      cssNameIndexArray[174] = CssNameIndex.LayoutGridLine;
      cssNameIndexArray[179] = CssNameIndex.BorderSpacing;
      cssNameIndexArray[181] = CssNameIndex.TextTransform;
      cssNameIndexArray[185] = CssNameIndex.BorderRightWidth;
      cssNameIndexArray[186] = CssNameIndex.PageBreakBefore;
      cssNameIndexArray[187] = CssNameIndex.TextIndent;
      cssNameIndexArray[188] = CssNameIndex.LayoutGridChar;
      cssNameIndexArray[189] = CssNameIndex.SpeechRate;
      cssNameIndexArray[190] = CssNameIndex.PauseBefore;
      cssNameIndexArray[192] = CssNameIndex.ScrollbarFaceColor;
      cssNameIndexArray[196] = CssNameIndex.PlayDuring;
      cssNameIndexArray[199] = CssNameIndex.WordBreak;
      cssNameIndexArray[200] = CssNameIndex.BorderBottomColor;
      cssNameIndexArray[208] = CssNameIndex.MarginRight;
      cssNameIndexArray[211] = CssNameIndex.SpeakNumeral;
      cssNameIndexArray[216] = CssNameIndex.TextJustify;
      cssNameIndexArray[217] = CssNameIndex.PaddingRight;
      cssNameIndexArray[218] = CssNameIndex.BorderRightStyle;
      cssNameIndexArray[221] = CssNameIndex.CounterIncrement;
      cssNameIndexArray[227] = CssNameIndex.TextUnderlinePosition;
      cssNameIndexArray[233] = CssNameIndex.WordSpacing;
      cssNameIndexArray[236] = CssNameIndex.Background;
      cssNameIndexArray[238] = CssNameIndex.OverflowX;
      cssNameIndexArray[239] = CssNameIndex.BorderWidth;
      cssNameIndexArray[245] = CssNameIndex.ZIndex;
      cssNameIndexArray[252] = CssNameIndex.MaxWidth;
      cssNameIndexArray[257] = CssNameIndex.ScrollbarDarkshadowColor;
      cssNameIndexArray[261] = CssNameIndex.CueAfter;
      cssNameIndexArray[269] = CssNameIndex.SpeakHeader;
      cssNameIndexArray[271] = CssNameIndex.Direction;
      cssNameIndexArray[272] = CssNameIndex.FontVariant;
      cssNameIndexArray[274] = CssNameIndex.Richness;
      cssNameIndexArray[281] = CssNameIndex.Font;
      cssNameIndexArray[285] = CssNameIndex.Outline;
      cssNameIndexArray[289] = CssNameIndex.BorderRightColor;
      cssNameIndexArray[291] = CssNameIndex.FontStyle;
      cssNameIndexArray[292] = CssNameIndex.MarginTop;
      cssNameIndexArray[295] = CssNameIndex.BorderLeft;
      cssNameIndexArray[298] = CssNameIndex.ListStylePosition;
      cssNameIndexArray[300] = CssNameIndex.BorderLeftWidth;
      cssNameIndexArray[305] = CssNameIndex.PaddingBottom;
      cssNameIndexArray[307] = CssNameIndex.LayoutGridType;
      cssNameIndexArray[308] = CssNameIndex.PageBreakAfter;
      cssNameIndexArray[311] = CssNameIndex.FontSize;
      cssNameIndexArray[313] = CssNameIndex.Position;
      cssNameIndexArray[314] = CssNameIndex.BorderLeftStyle;
      cssNameIndexArray[315] = CssNameIndex.PaddingLeft;
      cssNameIndexArray[318] = CssNameIndex.Right;
      cssNameIndexArray[319] = CssNameIndex.PauseAfter;
      cssNameIndexArray[320] = CssNameIndex.MaxHeight;
      cssNameIndexArray[323] = CssNameIndex.LetterSpacing;
      cssNameIndexArray[328] = CssNameIndex.BorderTop;
      CssData.nameHashTable = cssNameIndexArray;
      CssData.names = new CssData.NameDef[137]
      {
        new CssData.NameDef((short) 0, (string) null, CssNameIndex.Unknown),
        new CssData.NameDef((short) 0, "scrollbar-arrow-color", CssNameIndex.ScrollbarArrowColor),
        new CssData.NameDef((short) 2, "white-space", CssNameIndex.WhiteSpace),
        new CssData.NameDef((short) 4, "line-break", CssNameIndex.LineBreak),
        new CssData.NameDef((short) 5, "orphans", CssNameIndex.Orphans),
        new CssData.NameDef((short) 10, "writing-mode", CssNameIndex.WritingMode),
        new CssData.NameDef((short) 12, "scrollbar-3dlight-color", CssNameIndex.Scrollbar3dLightColor),
        new CssData.NameDef((short) 14, "text-autospace", CssNameIndex.TextAutospace),
        new CssData.NameDef((short) 15, "vertical-align", CssNameIndex.VerticalAlign),
        new CssData.NameDef((short) 18, "border-right", CssNameIndex.BorderRight),
        new CssData.NameDef((short) 21, "bottom", CssNameIndex.Bottom),
        new CssData.NameDef((short) 21, "font-family", CssNameIndex.FontFamily),
        new CssData.NameDef((short) 22, "line-height", CssNameIndex.LineHeight),
        new CssData.NameDef((short) 25, "border-bottom", CssNameIndex.BorderBottom),
        new CssData.NameDef((short) 32, "scrollbar-base-color", CssNameIndex.ScrollbarBaseColor),
        new CssData.NameDef((short) 33, "min-width", CssNameIndex.MinWidth),
        new CssData.NameDef((short) 34, "background-color", CssNameIndex.BackgroundColor),
        new CssData.NameDef((short) 36, "border-top-style", CssNameIndex.BorderTopStyle),
        new CssData.NameDef((short) 38, "empty-cells", CssNameIndex.EmptyCells),
        new CssData.NameDef((short) 40, "list-style-type", CssNameIndex.ListStyleType),
        new CssData.NameDef((short) 45, "text-align", CssNameIndex.TextAlign),
        new CssData.NameDef((short) 47, "font-weight", CssNameIndex.FontWeight),
        new CssData.NameDef((short) 49, "outline-width", CssNameIndex.OutlineWidth),
        new CssData.NameDef((short) 50, "caption-side", CssNameIndex.CaptionSide),
        new CssData.NameDef((short) 51, "scrollbar-shadow-color", CssNameIndex.ScrollbarShadowColor),
        new CssData.NameDef((short) 55, "clip", CssNameIndex.Clip),
        new CssData.NameDef((short) 55, "volume", CssNameIndex.Volume),
        new CssData.NameDef((short) 57, "margin-left", CssNameIndex.MarginLeft),
        new CssData.NameDef((short) 58, "border-top-width", CssNameIndex.BorderTopWidth),
        new CssData.NameDef((short) 61, "azimuth", CssNameIndex.Azimuth),
        new CssData.NameDef((short) 61, "unicode-bidi", CssNameIndex.UnicodeBidi),
        new CssData.NameDef((short) 62, "float", CssNameIndex.Float),
        new CssData.NameDef((short) 66, "layout-flow", CssNameIndex.LayoutFlow),
        new CssData.NameDef((short) 67, "min-height", CssNameIndex.MinHeight),
        new CssData.NameDef((short) 68, "content", CssNameIndex.Content),
        new CssData.NameDef((short) 70, "padding", CssNameIndex.Padding),
        new CssData.NameDef((short) 71, "border-bottom-width", CssNameIndex.BorderBottomWidth),
        new CssData.NameDef((short) 74, "visibility", CssNameIndex.Visibility),
        new CssData.NameDef((short) 76, "overflow", CssNameIndex.Overflow),
        new CssData.NameDef((short) 76, "table-layout", CssNameIndex.TableLayout),
        new CssData.NameDef((short) 77, "border-left-color", CssNameIndex.BorderLeftColor),
        new CssData.NameDef((short) 80, "pitch", CssNameIndex.Pitch),
        new CssData.NameDef((short) 81, "pause", CssNameIndex.Pause),
        new CssData.NameDef((short) 89, "overflow-y", CssNameIndex.OverflowY),
        new CssData.NameDef((short) 93, "scrollbar-highlight-color", CssNameIndex.ScrollbarHighlightColor),
        new CssData.NameDef((short) 95, "height", CssNameIndex.Height),
        new CssData.NameDef((short) 97, "word-wrap", CssNameIndex.WordWrap),
        new CssData.NameDef((short) 104, "top", CssNameIndex.Top),
        new CssData.NameDef((short) 105, "list-style", CssNameIndex.ListStyle),
        new CssData.NameDef((short) 107, "margin", CssNameIndex.Margin),
        new CssData.NameDef((short) 109, "text-kashida-space", CssNameIndex.TextKashidaSpace),
        new CssData.NameDef((short) 110, "voice-family", CssNameIndex.VoiceFamily),
        new CssData.NameDef((short) 111, "cue-before", CssNameIndex.CueBefore),
        new CssData.NameDef((short) 112, "clear", CssNameIndex.Clear),
        new CssData.NameDef((short) 116, "text-overflow", CssNameIndex.TextOverflow),
        new CssData.NameDef((short) 125, "border-bottom-style", CssNameIndex.BorderBottomStyle),
        new CssData.NameDef((short) 128, "border-color", CssNameIndex.BorderColor),
        new CssData.NameDef((short) 129, "text-decoration", CssNameIndex.TextDecoration),
        new CssData.NameDef((short) 130, "display", CssNameIndex.Display),
        new CssData.NameDef((short) 136, "counter-reset", CssNameIndex.CounterReset),
        new CssData.NameDef((short) 137, "margin-bottom", CssNameIndex.MarginBottom),
        new CssData.NameDef((short) 138, "border-style", CssNameIndex.BorderStyle),
        new CssData.NameDef((short) 142, "layout-grid", CssNameIndex.LayoutGrid),
        new CssData.NameDef((short) 143, "quotes", CssNameIndex.Quotes),
        new CssData.NameDef((short) 147, "accelerator", CssNameIndex.Accelerator),
        new CssData.NameDef((short) 148, "border", CssNameIndex.Border),
        new CssData.NameDef((short) 151, "zoom", CssNameIndex.Zoom),
        new CssData.NameDef((short) 154, "outline-style", CssNameIndex.OutlineStyle),
        new CssData.NameDef((short) 156, "width", CssNameIndex.Width),
        new CssData.NameDef((short) 158, "color", CssNameIndex.Color),
        new CssData.NameDef((short) 163, "page-break-inside", CssNameIndex.PageBreakInside),
        new CssData.NameDef((short) 165, "pitch-range", CssNameIndex.PitchRange),
        new CssData.NameDef((short) 166, "border-collapse", CssNameIndex.BorderCollapse),
        new CssData.NameDef((short) 166, "speak", CssNameIndex.Speak),
        new CssData.NameDef((short) 167, "cue", CssNameIndex.Cue),
        new CssData.NameDef((short) 169, "left", CssNameIndex.Left),
        new CssData.NameDef((short) 170, "layout-grid-mode", CssNameIndex.LayoutGridMode),
        new CssData.NameDef((short) 173, "speak-punctuation", CssNameIndex.SpeakPunctuation),
        new CssData.NameDef((short) 174, "layout-grid-line", CssNameIndex.LayoutGridLine),
        new CssData.NameDef((short) 179, "border-spacing", CssNameIndex.BorderSpacing),
        new CssData.NameDef((short) 181, "text-transform", CssNameIndex.TextTransform),
        new CssData.NameDef((short) 185, "border-right-width", CssNameIndex.BorderRightWidth),
        new CssData.NameDef((short) 186, "page-break-before", CssNameIndex.PageBreakBefore),
        new CssData.NameDef((short) 187, "text-indent", CssNameIndex.TextIndent),
        new CssData.NameDef((short) 188, "layout-grid-char", CssNameIndex.LayoutGridChar),
        new CssData.NameDef((short) 189, "speech-rate", CssNameIndex.SpeechRate),
        new CssData.NameDef((short) 190, "pause-before", CssNameIndex.PauseBefore),
        new CssData.NameDef((short) 192, "scrollbar-face-color", CssNameIndex.ScrollbarFaceColor),
        new CssData.NameDef((short) 196, "play-during", CssNameIndex.PlayDuring),
        new CssData.NameDef((short) 199, "word-break", CssNameIndex.WordBreak),
        new CssData.NameDef((short) 200, "border-bottom-color", CssNameIndex.BorderBottomColor),
        new CssData.NameDef((short) 208, "margin-right", CssNameIndex.MarginRight),
        new CssData.NameDef((short) 211, "speak-numeral", CssNameIndex.SpeakNumeral),
        new CssData.NameDef((short) 216, "text-justify", CssNameIndex.TextJustify),
        new CssData.NameDef((short) 217, "padding-right", CssNameIndex.PaddingRight),
        new CssData.NameDef((short) 218, "border-right-style", CssNameIndex.BorderRightStyle),
        new CssData.NameDef((short) 221, "counter-increment", CssNameIndex.CounterIncrement),
        new CssData.NameDef((short) 227, "text-underline-position", CssNameIndex.TextUnderlinePosition),
        new CssData.NameDef((short) 233, "word-spacing", CssNameIndex.WordSpacing),
        new CssData.NameDef((short) 236, "background", CssNameIndex.Background),
        new CssData.NameDef((short) 238, "overflow-x", CssNameIndex.OverflowX),
        new CssData.NameDef((short) 239, "border-width", CssNameIndex.BorderWidth),
        new CssData.NameDef((short) 239, "widows", CssNameIndex.Widows),
        new CssData.NameDef((short) 245, "z-index", CssNameIndex.ZIndex),
        new CssData.NameDef((short) 245, "border-top-color", CssNameIndex.BorderTopColor),
        new CssData.NameDef((short) 252, "max-width", CssNameIndex.MaxWidth),
        new CssData.NameDef((short) 257, "scrollbar-darkshadow-color", CssNameIndex.ScrollbarDarkshadowColor),
        new CssData.NameDef((short) 261, "cue-after", CssNameIndex.CueAfter),
        new CssData.NameDef((short) 269, "speak-header", CssNameIndex.SpeakHeader),
        new CssData.NameDef((short) 271, "direction", CssNameIndex.Direction),
        new CssData.NameDef((short) 272, "font-variant", CssNameIndex.FontVariant),
        new CssData.NameDef((short) 274, "richness", CssNameIndex.Richness),
        new CssData.NameDef((short) 274, "stress", CssNameIndex.Stress),
        new CssData.NameDef((short) 281, "font", CssNameIndex.Font),
        new CssData.NameDef((short) 281, "elevation", CssNameIndex.Elevation),
        new CssData.NameDef((short) 285, "outline", CssNameIndex.Outline),
        new CssData.NameDef((short) 289, "border-right-color", CssNameIndex.BorderRightColor),
        new CssData.NameDef((short) 291, "font-style", CssNameIndex.FontStyle),
        new CssData.NameDef((short) 292, "margin-top", CssNameIndex.MarginTop),
        new CssData.NameDef((short) 295, "border-left", CssNameIndex.BorderLeft),
        new CssData.NameDef((short) 298, "list-style-position", CssNameIndex.ListStylePosition),
        new CssData.NameDef((short) 298, "outline-color", CssNameIndex.OutlineColor),
        new CssData.NameDef((short) 300, "border-left-width", CssNameIndex.BorderLeftWidth),
        new CssData.NameDef((short) 305, "padding-bottom", CssNameIndex.PaddingBottom),
        new CssData.NameDef((short) 307, "layout-grid-type", CssNameIndex.LayoutGridType),
        new CssData.NameDef((short) 308, "page-break-after", CssNameIndex.PageBreakAfter),
        new CssData.NameDef((short) 311, "font-size", CssNameIndex.FontSize),
        new CssData.NameDef((short) 313, "position", CssNameIndex.Position),
        new CssData.NameDef((short) 314, "border-left-style", CssNameIndex.BorderLeftStyle),
        new CssData.NameDef((short) 314, "padding-top", CssNameIndex.PaddingTop),
        new CssData.NameDef((short) 315, "padding-left", CssNameIndex.PaddingLeft),
        new CssData.NameDef((short) 318, "right", CssNameIndex.Right),
        new CssData.NameDef((short) 319, "pause-after", CssNameIndex.PauseAfter),
        new CssData.NameDef((short) 320, "max-height", CssNameIndex.MaxHeight),
        new CssData.NameDef((short) 323, "letter-spacing", CssNameIndex.LetterSpacing),
        new CssData.NameDef((short) 328, "border-top", CssNameIndex.BorderTop),
        new CssData.NameDef((short) 329, (string) null, CssNameIndex.Unknown)
      };
      CssData.nameIndex = CssData.InitializeNameIndex();
      CssData.filterInstructions = new CssData.FilterActionEntry[137]
      {
        new CssData.FilterActionEntry(CssData.FilterAction.Drop),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.CheckContent),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.CheckContent),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Keep),
        new CssData.FilterActionEntry(CssData.FilterAction.Drop)
      };
    }

    private static CssNameIndex[] InitializeNameIndex()
    {
      CssNameIndex[] cssNameIndexArray = new CssNameIndex[136];
      for (int index = 0; index < CssData.names.Length; ++index)
      {
        if (CssData.names[index].PublicNameId != CssNameIndex.Unknown)
          cssNameIndexArray[(int) CssData.names[index].PublicNameId] = (CssNameIndex) index;
      }
      cssNameIndexArray[0] = CssNameIndex.Unknown;
      return cssNameIndexArray;
    }

    public struct NameDef
    {
      public short Hash;
      public string Name;
      public CssNameIndex PublicNameId;

      public NameDef(short hash, string name, CssNameIndex publicNameId)
      {
        this.Hash = hash;
        this.Name = name;
        this.PublicNameId = publicNameId;
      }
    }

    public enum FilterAction : byte
    {
      Unknown,
      Drop,
      Keep,
      CheckContent,
    }

    public struct FilterActionEntry
    {
      public CssData.FilterAction propertyAction;

      public FilterActionEntry(CssData.FilterAction propertyAction)
      {
        this.propertyAction = propertyAction;
      }
    }
  }
}
