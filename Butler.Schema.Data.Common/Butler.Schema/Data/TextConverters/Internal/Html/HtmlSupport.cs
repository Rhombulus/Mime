// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Html.HtmlSupport
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Html
{
  internal static class HtmlSupport
  {
    public static readonly byte[] UnsafeAsciiMap = new byte[161]
    {
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 0,
      (byte) 0,
      (byte) 2,
      (byte) 2,
      (byte) 0,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 0,
      (byte) 2,
      (byte) 3,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 3,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 3,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 2,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 2,
      (byte) 2,
      (byte) 3,
      (byte) 2,
      (byte) 3,
      (byte) 2,
      (byte) 2,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 0,
      (byte) 2,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 2,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3,
      (byte) 3
    };
    public static readonly HtmlEntityIndex[] EntityMap = new HtmlEntityIndex[96]
    {
      HtmlEntityIndex.nbsp,
      HtmlEntityIndex.iexcl,
      HtmlEntityIndex.cent,
      HtmlEntityIndex.pound,
      HtmlEntityIndex.curren,
      HtmlEntityIndex.yen,
      HtmlEntityIndex.brvbar,
      HtmlEntityIndex.sect,
      HtmlEntityIndex.uml,
      HtmlEntityIndex.copy,
      HtmlEntityIndex.ordf,
      HtmlEntityIndex.laquo,
      HtmlEntityIndex.not,
      HtmlEntityIndex.shy,
      HtmlEntityIndex.reg,
      HtmlEntityIndex.macr,
      HtmlEntityIndex.deg,
      HtmlEntityIndex.plusmn,
      HtmlEntityIndex.sup2,
      HtmlEntityIndex.sup3,
      HtmlEntityIndex.acute,
      HtmlEntityIndex.micro,
      HtmlEntityIndex.para,
      HtmlEntityIndex.middot,
      HtmlEntityIndex.cedil,
      HtmlEntityIndex.sup1,
      HtmlEntityIndex.ordm,
      HtmlEntityIndex.raquo,
      HtmlEntityIndex.frac14,
      HtmlEntityIndex.frac12,
      HtmlEntityIndex.frac34,
      HtmlEntityIndex.iquest,
      HtmlEntityIndex.Agrave,
      HtmlEntityIndex.Aacute,
      HtmlEntityIndex.Acirc,
      HtmlEntityIndex.Atilde,
      HtmlEntityIndex.Auml,
      HtmlEntityIndex.Aring,
      HtmlEntityIndex.AElig,
      HtmlEntityIndex.Ccedil,
      HtmlEntityIndex.Egrave,
      HtmlEntityIndex.Eacute,
      HtmlEntityIndex.Ecirc,
      HtmlEntityIndex.Euml,
      HtmlEntityIndex.Igrave,
      HtmlEntityIndex.Iacute,
      HtmlEntityIndex.Icirc,
      HtmlEntityIndex.Iuml,
      HtmlEntityIndex.ETH,
      HtmlEntityIndex.Ntilde,
      HtmlEntityIndex.Ograve,
      HtmlEntityIndex.Oacute,
      HtmlEntityIndex.Ocirc,
      HtmlEntityIndex.Otilde,
      HtmlEntityIndex.Ouml,
      HtmlEntityIndex.times,
      HtmlEntityIndex.Oslash,
      HtmlEntityIndex.Ugrave,
      HtmlEntityIndex.Uacute,
      HtmlEntityIndex.Ucirc,
      HtmlEntityIndex.Uuml,
      HtmlEntityIndex.Yacute,
      HtmlEntityIndex.THORN,
      HtmlEntityIndex.szlig,
      HtmlEntityIndex.agrave,
      HtmlEntityIndex.aacute,
      HtmlEntityIndex.acirc,
      HtmlEntityIndex.atilde,
      HtmlEntityIndex.auml,
      HtmlEntityIndex.aring,
      HtmlEntityIndex.aelig,
      HtmlEntityIndex.ccedil,
      HtmlEntityIndex.egrave,
      HtmlEntityIndex.eacute,
      HtmlEntityIndex.ecirc,
      HtmlEntityIndex.euml,
      HtmlEntityIndex.igrave,
      HtmlEntityIndex.iacute,
      HtmlEntityIndex.icirc,
      HtmlEntityIndex.iuml,
      HtmlEntityIndex.eth,
      HtmlEntityIndex.ntilde,
      HtmlEntityIndex.ograve,
      HtmlEntityIndex.oacute,
      HtmlEntityIndex.ocirc,
      HtmlEntityIndex.otilde,
      HtmlEntityIndex.ouml,
      HtmlEntityIndex.divide,
      HtmlEntityIndex.oslash,
      HtmlEntityIndex.ugrave,
      HtmlEntityIndex.uacute,
      HtmlEntityIndex.ucirc,
      HtmlEntityIndex.uuml,
      HtmlEntityIndex.yacute,
      HtmlEntityIndex.thorn,
      HtmlEntityIndex.yuml
    };
    private static HtmlSupport.EnumerationDef[] directionEnumeration = new HtmlSupport.EnumerationDef[2]
    {
      new HtmlSupport.EnumerationDef("rtl", Format.PropertyValue.True),
      new HtmlSupport.EnumerationDef("ltr", Format.PropertyValue.False)
    };
    internal static HtmlSupport.EnumerationDef[] TextAlignmentEnumeration = new HtmlSupport.EnumerationDef[4]
    {
      new HtmlSupport.EnumerationDef("left", new Format.PropertyValue((Enum) Format.TextAlign.Left)),
      new HtmlSupport.EnumerationDef("center", new Format.PropertyValue((Enum) Format.TextAlign.Center)),
      new HtmlSupport.EnumerationDef("right", new Format.PropertyValue((Enum) Format.TextAlign.Right)),
      new HtmlSupport.EnumerationDef("justify", new Format.PropertyValue((Enum) Format.TextAlign.Justify))
    };
    internal static HtmlSupport.EnumerationDef[] HorizontalAlignmentEnumeration = new HtmlSupport.EnumerationDef[3]
    {
      new HtmlSupport.EnumerationDef("left", new Format.PropertyValue((Enum) Format.BlockHorizontalAlign.Left)),
      new HtmlSupport.EnumerationDef("center", new Format.PropertyValue((Enum) Format.BlockHorizontalAlign.Center)),
      new HtmlSupport.EnumerationDef("right", new Format.PropertyValue((Enum) Format.BlockHorizontalAlign.Right))
    };
    private static HtmlSupport.EnumerationDef[] verticalAlignmentEnumeration = new HtmlSupport.EnumerationDef[4]
    {
      new HtmlSupport.EnumerationDef("top", new Format.PropertyValue((Enum) Format.BlockVerticalAlign.Top)),
      new HtmlSupport.EnumerationDef("middle", new Format.PropertyValue((Enum) Format.BlockVerticalAlign.Middle)),
      new HtmlSupport.EnumerationDef("bottom", new Format.PropertyValue((Enum) Format.BlockVerticalAlign.Bottom)),
      new HtmlSupport.EnumerationDef("baseline", new Format.PropertyValue((Enum) Format.BlockVerticalAlign.Middle))
    };
    internal static HtmlSupport.EnumerationDef[] BlockAlignmentEnumeration = new HtmlSupport.EnumerationDef[5]
    {
      new HtmlSupport.EnumerationDef("top", new Format.PropertyValue((Enum) Format.BlockHorizontalAlign.Left)),
      new HtmlSupport.EnumerationDef("middle", new Format.PropertyValue((Enum) Format.BlockHorizontalAlign.Left)),
      new HtmlSupport.EnumerationDef("bottom", new Format.PropertyValue((Enum) Format.BlockHorizontalAlign.Left)),
      new HtmlSupport.EnumerationDef("left", new Format.PropertyValue((Enum) Format.BlockHorizontalAlign.Center)),
      new HtmlSupport.EnumerationDef("right", new Format.PropertyValue((Enum) Format.BlockHorizontalAlign.Right))
    };
    internal static HtmlSupport.EnumerationDef[] BorderStyleEnumeration = new HtmlSupport.EnumerationDef[10]
    {
      new HtmlSupport.EnumerationDef("none", new Format.PropertyValue((Enum) Format.BorderStyle.None)),
      new HtmlSupport.EnumerationDef("hidden", new Format.PropertyValue((Enum) Format.BorderStyle.Hidden)),
      new HtmlSupport.EnumerationDef("dotted", new Format.PropertyValue((Enum) Format.BorderStyle.Dotted)),
      new HtmlSupport.EnumerationDef("dashed", new Format.PropertyValue((Enum) Format.BorderStyle.Dashed)),
      new HtmlSupport.EnumerationDef("solid", new Format.PropertyValue((Enum) Format.BorderStyle.Solid)),
      new HtmlSupport.EnumerationDef("double", new Format.PropertyValue((Enum) Format.BorderStyle.Double)),
      new HtmlSupport.EnumerationDef("groove", new Format.PropertyValue((Enum) Format.BorderStyle.Groove)),
      new HtmlSupport.EnumerationDef("ridge", new Format.PropertyValue((Enum) Format.BorderStyle.Ridge)),
      new HtmlSupport.EnumerationDef("inset", new Format.PropertyValue((Enum) Format.BorderStyle.Inset)),
      new HtmlSupport.EnumerationDef("outset", new Format.PropertyValue((Enum) Format.BorderStyle.Outset))
    };
    private static HtmlSupport.EnumerationDef[] targetEnumeration = new HtmlSupport.EnumerationDef[4]
    {
      new HtmlSupport.EnumerationDef("_self", new Format.PropertyValue((Enum) Format.LinkTarget.Self)),
      new HtmlSupport.EnumerationDef("_top", new Format.PropertyValue((Enum) Format.LinkTarget.Top)),
      new HtmlSupport.EnumerationDef("_blank", new Format.PropertyValue((Enum) Format.LinkTarget.Blank)),
      new HtmlSupport.EnumerationDef("_parent", new Format.PropertyValue((Enum) Format.LinkTarget.Parent))
    };
    private static HtmlSupport.EnumerationDef[] fontWeightEnumeration = new HtmlSupport.EnumerationDef[13]
    {
      new HtmlSupport.EnumerationDef("normal", Format.PropertyValue.False),
      new HtmlSupport.EnumerationDef("bold", Format.PropertyValue.True),
      new HtmlSupport.EnumerationDef("lighter", Format.PropertyValue.False),
      new HtmlSupport.EnumerationDef("bolder", Format.PropertyValue.True),
      new HtmlSupport.EnumerationDef("100", Format.PropertyValue.False),
      new HtmlSupport.EnumerationDef("200", Format.PropertyValue.False),
      new HtmlSupport.EnumerationDef("300", Format.PropertyValue.False),
      new HtmlSupport.EnumerationDef("400", Format.PropertyValue.False),
      new HtmlSupport.EnumerationDef("500", Format.PropertyValue.False),
      new HtmlSupport.EnumerationDef("600", Format.PropertyValue.True),
      new HtmlSupport.EnumerationDef("700", Format.PropertyValue.True),
      new HtmlSupport.EnumerationDef("800", Format.PropertyValue.True),
      new HtmlSupport.EnumerationDef("900", Format.PropertyValue.True)
    };
    private static HtmlSupport.EnumerationDef[] fontSizeEnumeration = new HtmlSupport.EnumerationDef[7]
    {
      new HtmlSupport.EnumerationDef("xx-small", new Format.PropertyValue(Format.LengthUnits.HtmlFontUnits, 1)),
      new HtmlSupport.EnumerationDef("x-small", new Format.PropertyValue(Format.LengthUnits.HtmlFontUnits, 2)),
      new HtmlSupport.EnumerationDef("small", new Format.PropertyValue(Format.LengthUnits.HtmlFontUnits, 2)),
      new HtmlSupport.EnumerationDef("medium", new Format.PropertyValue(Format.LengthUnits.HtmlFontUnits, 3)),
      new HtmlSupport.EnumerationDef("large", new Format.PropertyValue(Format.LengthUnits.HtmlFontUnits, 4)),
      new HtmlSupport.EnumerationDef("x-large", new Format.PropertyValue(Format.LengthUnits.HtmlFontUnits, 5)),
      new HtmlSupport.EnumerationDef("xx-large", new Format.PropertyValue(Format.LengthUnits.HtmlFontUnits, 6))
    };
    private static HtmlSupport.EnumerationDef[] fontStyleEnumeration = new HtmlSupport.EnumerationDef[3]
    {
      new HtmlSupport.EnumerationDef("normal", Format.PropertyValue.False),
      new HtmlSupport.EnumerationDef("italic", Format.PropertyValue.True),
      new HtmlSupport.EnumerationDef("oblique", Format.PropertyValue.True)
    };
    private static HtmlSupport.EnumerationDef[] fontVariantEnumeration = new HtmlSupport.EnumerationDef[2]
    {
      new HtmlSupport.EnumerationDef("normal", Format.PropertyValue.False),
      new HtmlSupport.EnumerationDef("small-caps", Format.PropertyValue.True)
    };
    private static HtmlSupport.EnumerationDef[] tableLayoutEnumeration = new HtmlSupport.EnumerationDef[2]
    {
      new HtmlSupport.EnumerationDef("auto", Format.PropertyValue.False),
      new HtmlSupport.EnumerationDef("fixed", Format.PropertyValue.True)
    };
    private static HtmlSupport.EnumerationDef[] borderCollapseEnumeration = new HtmlSupport.EnumerationDef[2]
    {
      new HtmlSupport.EnumerationDef("collapse", Format.PropertyValue.True),
      new HtmlSupport.EnumerationDef("separate", Format.PropertyValue.False)
    };
    private static HtmlSupport.EnumerationDef[] emptyCellsEnumeration = new HtmlSupport.EnumerationDef[2]
    {
      new HtmlSupport.EnumerationDef("show", Format.PropertyValue.True),
      new HtmlSupport.EnumerationDef("hide", Format.PropertyValue.False)
    };
    private static HtmlSupport.EnumerationDef[] captionSideEnumeration = new HtmlSupport.EnumerationDef[2]
    {
      new HtmlSupport.EnumerationDef("bottom", Format.PropertyValue.True),
      new HtmlSupport.EnumerationDef("top", Format.PropertyValue.False)
    };
    private static HtmlSupport.EnumerationDef[] borderWidthEnumeration = new HtmlSupport.EnumerationDef[3]
    {
      new HtmlSupport.EnumerationDef("thin", new Format.PropertyValue(Format.LengthUnits.Pixels, 2)),
      new HtmlSupport.EnumerationDef("medium", new Format.PropertyValue(Format.LengthUnits.Pixels, 4)),
      new HtmlSupport.EnumerationDef("thick", new Format.PropertyValue(Format.LengthUnits.Pixels, 6))
    };
    private static HtmlSupport.EnumerationDef[] tableFrameEnumeration = new HtmlSupport.EnumerationDef[9]
    {
      new HtmlSupport.EnumerationDef("void", new Format.PropertyValue((Enum) HtmlSupport.TableFrame.Void)),
      new HtmlSupport.EnumerationDef("above", new Format.PropertyValue((Enum) HtmlSupport.TableFrame.Above)),
      new HtmlSupport.EnumerationDef("below", new Format.PropertyValue((Enum) HtmlSupport.TableFrame.Below)),
      new HtmlSupport.EnumerationDef("border", new Format.PropertyValue((Enum) HtmlSupport.TableFrame.Border)),
      new HtmlSupport.EnumerationDef("box", new Format.PropertyValue((Enum) HtmlSupport.TableFrame.Box)),
      new HtmlSupport.EnumerationDef("hsides", new Format.PropertyValue((Enum) HtmlSupport.TableFrame.Hsides)),
      new HtmlSupport.EnumerationDef("lhs", new Format.PropertyValue((Enum) HtmlSupport.TableFrame.Lhs)),
      new HtmlSupport.EnumerationDef("rhs", new Format.PropertyValue((Enum) HtmlSupport.TableFrame.Rhs)),
      new HtmlSupport.EnumerationDef("vsides", new Format.PropertyValue((Enum) HtmlSupport.TableFrame.Vsides))
    };
    private static HtmlSupport.EnumerationDef[] tableRulesEnumeration = new HtmlSupport.EnumerationDef[5]
    {
      new HtmlSupport.EnumerationDef("none", new Format.PropertyValue((Enum) HtmlSupport.TableRules.None)),
      new HtmlSupport.EnumerationDef("groups", new Format.PropertyValue((Enum) HtmlSupport.TableRules.Groups)),
      new HtmlSupport.EnumerationDef("rows", new Format.PropertyValue((Enum) HtmlSupport.TableRules.Rows)),
      new HtmlSupport.EnumerationDef("cells", new Format.PropertyValue((Enum) HtmlSupport.TableRules.Cells)),
      new HtmlSupport.EnumerationDef("all", new Format.PropertyValue((Enum) HtmlSupport.TableRules.All))
    };
    private static HtmlSupport.EnumerationDef[] unicodeBiDiEnumeration = new HtmlSupport.EnumerationDef[3]
    {
      new HtmlSupport.EnumerationDef("normal", new Format.PropertyValue((Enum) Format.UnicodeBiDi.Normal)),
      new HtmlSupport.EnumerationDef("embed", new Format.PropertyValue((Enum) Format.UnicodeBiDi.Embed)),
      new HtmlSupport.EnumerationDef("bidi-override", new Format.PropertyValue((Enum) Format.UnicodeBiDi.Override))
    };
    private static HtmlSupport.EnumerationDef[] displayEnumeration = new HtmlSupport.EnumerationDef[16]
    {
      new HtmlSupport.EnumerationDef("none", new Format.PropertyValue((Enum) Format.Display.None)),
      new HtmlSupport.EnumerationDef("inline", new Format.PropertyValue((Enum) Format.Display.Inline)),
      new HtmlSupport.EnumerationDef("block", new Format.PropertyValue((Enum) Format.Display.Block)),
      new HtmlSupport.EnumerationDef("list-item", new Format.PropertyValue((Enum) Format.Display.ListItem)),
      new HtmlSupport.EnumerationDef("run-in", new Format.PropertyValue((Enum) Format.Display.RunIn)),
      new HtmlSupport.EnumerationDef("inline-block", new Format.PropertyValue((Enum) Format.Display.InlineBlock)),
      new HtmlSupport.EnumerationDef("table", new Format.PropertyValue((Enum) Format.Display.Table)),
      new HtmlSupport.EnumerationDef("inline-table", new Format.PropertyValue((Enum) Format.Display.InlineTable)),
      new HtmlSupport.EnumerationDef("table-row-group", new Format.PropertyValue((Enum) Format.Display.TableRowGroup)),
      new HtmlSupport.EnumerationDef("table-header-group", new Format.PropertyValue((Enum) Format.Display.TableHeaderGroup)),
      new HtmlSupport.EnumerationDef("table-footer-group", new Format.PropertyValue((Enum) Format.Display.TableFooterGroup)),
      new HtmlSupport.EnumerationDef("table-row", new Format.PropertyValue((Enum) Format.Display.TableRow)),
      new HtmlSupport.EnumerationDef("table-column-group", new Format.PropertyValue((Enum) Format.Display.TableColumnGroup)),
      new HtmlSupport.EnumerationDef("table-column", new Format.PropertyValue((Enum) Format.Display.TableColumn)),
      new HtmlSupport.EnumerationDef("table-cell", new Format.PropertyValue((Enum) Format.Display.TableCell)),
      new HtmlSupport.EnumerationDef("table-caption", new Format.PropertyValue((Enum) Format.Display.TableCaption))
    };
    private static HtmlSupport.EnumerationDef[] visibilityEnumeration = new HtmlSupport.EnumerationDef[3]
    {
      new HtmlSupport.EnumerationDef("visible", Format.PropertyValue.True),
      new HtmlSupport.EnumerationDef("hidden", Format.PropertyValue.False),
      new HtmlSupport.EnumerationDef("collapse", Format.PropertyValue.False)
    };
    private static readonly HtmlSupport.EnumerationDef[] colorNames = new HtmlSupport.EnumerationDef[169]
    {
      new HtmlSupport.EnumerationDef("activeborder", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.ActiveBorder)),
      new HtmlSupport.EnumerationDef("activecaption", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.ActiveCaption)),
      new HtmlSupport.EnumerationDef("aliceblue", new Format.PropertyValue(new Format.RGBT(15792383U))),
      new HtmlSupport.EnumerationDef("antiquewhite", new Format.PropertyValue(new Format.RGBT(16444375U))),
      new HtmlSupport.EnumerationDef("appworkspace", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.AppWorkspace)),
      new HtmlSupport.EnumerationDef("aqua", new Format.PropertyValue(new Format.RGBT((uint) ushort.MaxValue))),
      new HtmlSupport.EnumerationDef("aquamarine", new Format.PropertyValue(new Format.RGBT(8388564U))),
      new HtmlSupport.EnumerationDef("azure", new Format.PropertyValue(new Format.RGBT(15794175U))),
      new HtmlSupport.EnumerationDef("background", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.Background)),
      new HtmlSupport.EnumerationDef("beige", new Format.PropertyValue(new Format.RGBT(16119260U))),
      new HtmlSupport.EnumerationDef("bisque", new Format.PropertyValue(new Format.RGBT(16770244U))),
      new HtmlSupport.EnumerationDef("black", new Format.PropertyValue(new Format.RGBT(0U))),
      new HtmlSupport.EnumerationDef("blanchedalmond", new Format.PropertyValue(new Format.RGBT(16772045U))),
      new HtmlSupport.EnumerationDef("blue", new Format.PropertyValue(new Format.RGBT((uint) byte.MaxValue))),
      new HtmlSupport.EnumerationDef("blueviolet", new Format.PropertyValue(new Format.RGBT(9055202U))),
      new HtmlSupport.EnumerationDef("brown", new Format.PropertyValue(new Format.RGBT(10824234U))),
      new HtmlSupport.EnumerationDef("burlywood", new Format.PropertyValue(new Format.RGBT(14596231U))),
      new HtmlSupport.EnumerationDef("buttonface", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.ButtonFace)),
      new HtmlSupport.EnumerationDef("buttonhighlight", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.ButtonHighlight)),
      new HtmlSupport.EnumerationDef("buttonshadow", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.ButtonShadow)),
      new HtmlSupport.EnumerationDef("buttontext", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.ButtonText)),
      new HtmlSupport.EnumerationDef("cadetblue", new Format.PropertyValue(new Format.RGBT(6266528U))),
      new HtmlSupport.EnumerationDef("captiontext", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.CaptionText)),
      new HtmlSupport.EnumerationDef("chartreuse", new Format.PropertyValue(new Format.RGBT(8388352U))),
      new HtmlSupport.EnumerationDef("chocolate", new Format.PropertyValue(new Format.RGBT(13789470U))),
      new HtmlSupport.EnumerationDef("coral", new Format.PropertyValue(new Format.RGBT(16744272U))),
      new HtmlSupport.EnumerationDef("cornflowerblue", new Format.PropertyValue(new Format.RGBT(6591981U))),
      new HtmlSupport.EnumerationDef("cornsilk", new Format.PropertyValue(new Format.RGBT(16775388U))),
      new HtmlSupport.EnumerationDef("crimson", new Format.PropertyValue(new Format.RGBT(14423100U))),
      new HtmlSupport.EnumerationDef("cyan", new Format.PropertyValue(new Format.RGBT((uint) ushort.MaxValue))),
      new HtmlSupport.EnumerationDef("darkblue", new Format.PropertyValue(new Format.RGBT(139U))),
      new HtmlSupport.EnumerationDef("darkcyan", new Format.PropertyValue(new Format.RGBT(35723U))),
      new HtmlSupport.EnumerationDef("darkgoldenrod", new Format.PropertyValue(new Format.RGBT(12092939U))),
      new HtmlSupport.EnumerationDef("darkgray", new Format.PropertyValue(new Format.RGBT(11119017U))),
      new HtmlSupport.EnumerationDef("darkgreen", new Format.PropertyValue(new Format.RGBT(25600U))),
      new HtmlSupport.EnumerationDef("darkkhaki", new Format.PropertyValue(new Format.RGBT(12433259U))),
      new HtmlSupport.EnumerationDef("darkmagenta", new Format.PropertyValue(new Format.RGBT(9109643U))),
      new HtmlSupport.EnumerationDef("darkolivegreen", new Format.PropertyValue(new Format.RGBT(5597999U))),
      new HtmlSupport.EnumerationDef("darkorange", new Format.PropertyValue(new Format.RGBT(16747520U))),
      new HtmlSupport.EnumerationDef("darkorchid", new Format.PropertyValue(new Format.RGBT(10040012U))),
      new HtmlSupport.EnumerationDef("darkred", new Format.PropertyValue(new Format.RGBT(9109504U))),
      new HtmlSupport.EnumerationDef("darksalmon", new Format.PropertyValue(new Format.RGBT(15308410U))),
      new HtmlSupport.EnumerationDef("darkseagreen", new Format.PropertyValue(new Format.RGBT(9419919U))),
      new HtmlSupport.EnumerationDef("darkslateblue", new Format.PropertyValue(new Format.RGBT(4734347U))),
      new HtmlSupport.EnumerationDef("darkslategray", new Format.PropertyValue(new Format.RGBT(3100495U))),
      new HtmlSupport.EnumerationDef("darkturquoise", new Format.PropertyValue(new Format.RGBT(52945U))),
      new HtmlSupport.EnumerationDef("darkviolet", new Format.PropertyValue(new Format.RGBT(9699539U))),
      new HtmlSupport.EnumerationDef("deeppink", new Format.PropertyValue(new Format.RGBT(16716947U))),
      new HtmlSupport.EnumerationDef("deepskyblue", new Format.PropertyValue(new Format.RGBT(49151U))),
      new HtmlSupport.EnumerationDef("dimgray", new Format.PropertyValue(new Format.RGBT(6908265U))),
      new HtmlSupport.EnumerationDef("dodgerblue", new Format.PropertyValue(new Format.RGBT(2003199U))),
      new HtmlSupport.EnumerationDef("firebrick", new Format.PropertyValue(new Format.RGBT(11674146U))),
      new HtmlSupport.EnumerationDef("floralwhite", new Format.PropertyValue(new Format.RGBT(16775920U))),
      new HtmlSupport.EnumerationDef("forestgreen", new Format.PropertyValue(new Format.RGBT(2263842U))),
      new HtmlSupport.EnumerationDef("fuchsia", new Format.PropertyValue(new Format.RGBT(16711935U))),
      new HtmlSupport.EnumerationDef("gainsboro", new Format.PropertyValue(new Format.RGBT(14474460U))),
      new HtmlSupport.EnumerationDef("ghostwhite", new Format.PropertyValue(new Format.RGBT(16316671U))),
      new HtmlSupport.EnumerationDef("gold", new Format.PropertyValue(new Format.RGBT(16766720U))),
      new HtmlSupport.EnumerationDef("goldenrod", new Format.PropertyValue(new Format.RGBT(14329120U))),
      new HtmlSupport.EnumerationDef("gray", new Format.PropertyValue(new Format.RGBT(8421504U))),
      new HtmlSupport.EnumerationDef("graytext", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.GrayText)),
      new HtmlSupport.EnumerationDef("green", new Format.PropertyValue(new Format.RGBT(32768U))),
      new HtmlSupport.EnumerationDef("greenyellow", new Format.PropertyValue(new Format.RGBT(11403055U))),
      new HtmlSupport.EnumerationDef("highlight", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.Highlight)),
      new HtmlSupport.EnumerationDef("highlighttext", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.HighlightText)),
      new HtmlSupport.EnumerationDef("honeydew", new Format.PropertyValue(new Format.RGBT(15794160U))),
      new HtmlSupport.EnumerationDef("hotpink", new Format.PropertyValue(new Format.RGBT(16738740U))),
      new HtmlSupport.EnumerationDef("inactiveborder", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.InactiveBorder)),
      new HtmlSupport.EnumerationDef("inactivecaption", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.InactiveCaption)),
      new HtmlSupport.EnumerationDef("inactivecaptiontext", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.InactiveCaptionText)),
      new HtmlSupport.EnumerationDef("indianred", new Format.PropertyValue(new Format.RGBT(13458524U))),
      new HtmlSupport.EnumerationDef("indigo", new Format.PropertyValue(new Format.RGBT(4915330U))),
      new HtmlSupport.EnumerationDef("infobackground", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.InfoBackground)),
      new HtmlSupport.EnumerationDef("infotext", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.InfoText)),
      new HtmlSupport.EnumerationDef("ivory", new Format.PropertyValue(new Format.RGBT(16777200U))),
      new HtmlSupport.EnumerationDef("khaki", new Format.PropertyValue(new Format.RGBT(15787660U))),
      new HtmlSupport.EnumerationDef("lavender", new Format.PropertyValue(new Format.RGBT(15132410U))),
      new HtmlSupport.EnumerationDef("lavenderblush", new Format.PropertyValue(new Format.RGBT(16773365U))),
      new HtmlSupport.EnumerationDef("lawngreen", new Format.PropertyValue(new Format.RGBT(8190976U))),
      new HtmlSupport.EnumerationDef("lemonchiffon", new Format.PropertyValue(new Format.RGBT(16775885U))),
      new HtmlSupport.EnumerationDef("lightblue", new Format.PropertyValue(new Format.RGBT(11393254U))),
      new HtmlSupport.EnumerationDef("lightcoral", new Format.PropertyValue(new Format.RGBT(15761536U))),
      new HtmlSupport.EnumerationDef("lightcyan", new Format.PropertyValue(new Format.RGBT(14745599U))),
      new HtmlSupport.EnumerationDef("lightgoldenrodyellow", new Format.PropertyValue(new Format.RGBT(16448210U))),
      new HtmlSupport.EnumerationDef("lightgreen", new Format.PropertyValue(new Format.RGBT(9498256U))),
      new HtmlSupport.EnumerationDef("lightgrey", new Format.PropertyValue(new Format.RGBT(13882323U))),
      new HtmlSupport.EnumerationDef("lightpink", new Format.PropertyValue(new Format.RGBT(16758465U))),
      new HtmlSupport.EnumerationDef("lightsalmon", new Format.PropertyValue(new Format.RGBT(16752762U))),
      new HtmlSupport.EnumerationDef("lightseagreen", new Format.PropertyValue(new Format.RGBT(2142890U))),
      new HtmlSupport.EnumerationDef("lightskyblue", new Format.PropertyValue(new Format.RGBT(8900346U))),
      new HtmlSupport.EnumerationDef("lightslategray", new Format.PropertyValue(new Format.RGBT(7833753U))),
      new HtmlSupport.EnumerationDef("lightsteelblue", new Format.PropertyValue(new Format.RGBT(11584734U))),
      new HtmlSupport.EnumerationDef("lightyellow", new Format.PropertyValue(new Format.RGBT(16777184U))),
      new HtmlSupport.EnumerationDef("lime", new Format.PropertyValue(new Format.RGBT(65280U))),
      new HtmlSupport.EnumerationDef("limegreen", new Format.PropertyValue(new Format.RGBT(3329330U))),
      new HtmlSupport.EnumerationDef("linen", new Format.PropertyValue(new Format.RGBT(16445670U))),
      new HtmlSupport.EnumerationDef("magenta", new Format.PropertyValue(new Format.RGBT(16711935U))),
      new HtmlSupport.EnumerationDef("maroon", new Format.PropertyValue(new Format.RGBT(8388608U))),
      new HtmlSupport.EnumerationDef("mediumaquamarine", new Format.PropertyValue(new Format.RGBT(6737322U))),
      new HtmlSupport.EnumerationDef("mediumblue", new Format.PropertyValue(new Format.RGBT(205U))),
      new HtmlSupport.EnumerationDef("mediumorchid", new Format.PropertyValue(new Format.RGBT(12211667U))),
      new HtmlSupport.EnumerationDef("mediumpurple", new Format.PropertyValue(new Format.RGBT(9662683U))),
      new HtmlSupport.EnumerationDef("mediumseagreen", new Format.PropertyValue(new Format.RGBT(3978097U))),
      new HtmlSupport.EnumerationDef("mediumslateblue", new Format.PropertyValue(new Format.RGBT(8087790U))),
      new HtmlSupport.EnumerationDef("mediumspringgreen", new Format.PropertyValue(new Format.RGBT(64154U))),
      new HtmlSupport.EnumerationDef("mediumturquoise", new Format.PropertyValue(new Format.RGBT(4772300U))),
      new HtmlSupport.EnumerationDef("mediumvioletred", new Format.PropertyValue(new Format.RGBT(13047173U))),
      new HtmlSupport.EnumerationDef("menu", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.Menu)),
      new HtmlSupport.EnumerationDef("menutext", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.MenuText)),
      new HtmlSupport.EnumerationDef("midnightblue", new Format.PropertyValue(new Format.RGBT(1644912U))),
      new HtmlSupport.EnumerationDef("mintcream", new Format.PropertyValue(new Format.RGBT(16121850U))),
      new HtmlSupport.EnumerationDef("mistyrose", new Format.PropertyValue(new Format.RGBT(16770273U))),
      new HtmlSupport.EnumerationDef("moccasin", new Format.PropertyValue(new Format.RGBT(16770229U))),
      new HtmlSupport.EnumerationDef("navajowhite", new Format.PropertyValue(new Format.RGBT(16768685U))),
      new HtmlSupport.EnumerationDef("navy", new Format.PropertyValue(new Format.RGBT(128U))),
      new HtmlSupport.EnumerationDef("oldlace", new Format.PropertyValue(new Format.RGBT(16643558U))),
      new HtmlSupport.EnumerationDef("olive", new Format.PropertyValue(new Format.RGBT(8421376U))),
      new HtmlSupport.EnumerationDef("olivedrab", new Format.PropertyValue(new Format.RGBT(7048739U))),
      new HtmlSupport.EnumerationDef("orange", new Format.PropertyValue(new Format.RGBT(16753920U))),
      new HtmlSupport.EnumerationDef("orangered", new Format.PropertyValue(new Format.RGBT(16729344U))),
      new HtmlSupport.EnumerationDef("orchid", new Format.PropertyValue(new Format.RGBT(14315734U))),
      new HtmlSupport.EnumerationDef("palegoldenrod", new Format.PropertyValue(new Format.RGBT(15657130U))),
      new HtmlSupport.EnumerationDef("palegreen", new Format.PropertyValue(new Format.RGBT(10025880U))),
      new HtmlSupport.EnumerationDef("paleturquoise", new Format.PropertyValue(new Format.RGBT(11529966U))),
      new HtmlSupport.EnumerationDef("palevioletred", new Format.PropertyValue(new Format.RGBT(14381203U))),
      new HtmlSupport.EnumerationDef("papayawhip", new Format.PropertyValue(new Format.RGBT(16773077U))),
      new HtmlSupport.EnumerationDef("peachpuff", new Format.PropertyValue(new Format.RGBT(16767673U))),
      new HtmlSupport.EnumerationDef("peru", new Format.PropertyValue(new Format.RGBT(13468991U))),
      new HtmlSupport.EnumerationDef("pink", new Format.PropertyValue(new Format.RGBT(16761035U))),
      new HtmlSupport.EnumerationDef("plum", new Format.PropertyValue(new Format.RGBT(14524637U))),
      new HtmlSupport.EnumerationDef("powderblue", new Format.PropertyValue(new Format.RGBT(11591910U))),
      new HtmlSupport.EnumerationDef("purple", new Format.PropertyValue(new Format.RGBT(8388736U))),
      new HtmlSupport.EnumerationDef("red", new Format.PropertyValue(new Format.RGBT(16711680U))),
      new HtmlSupport.EnumerationDef("rosybrown", new Format.PropertyValue(new Format.RGBT(12357519U))),
      new HtmlSupport.EnumerationDef("royalblue", new Format.PropertyValue(new Format.RGBT(4286945U))),
      new HtmlSupport.EnumerationDef("saddlebrown", new Format.PropertyValue(new Format.RGBT(9127187U))),
      new HtmlSupport.EnumerationDef("salmon", new Format.PropertyValue(new Format.RGBT(16416882U))),
      new HtmlSupport.EnumerationDef("sandybrown", new Format.PropertyValue(new Format.RGBT(16032864U))),
      new HtmlSupport.EnumerationDef("scrollbar", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.Scrollbar)),
      new HtmlSupport.EnumerationDef("seagreen", new Format.PropertyValue(new Format.RGBT(3050327U))),
      new HtmlSupport.EnumerationDef("seashell", new Format.PropertyValue(new Format.RGBT(16774638U))),
      new HtmlSupport.EnumerationDef("sienna", new Format.PropertyValue(new Format.RGBT(10506797U))),
      new HtmlSupport.EnumerationDef("silver", new Format.PropertyValue(new Format.RGBT(12632256U))),
      new HtmlSupport.EnumerationDef("skyblue", new Format.PropertyValue(new Format.RGBT(8900331U))),
      new HtmlSupport.EnumerationDef("slateblue", new Format.PropertyValue(new Format.RGBT(6970061U))),
      new HtmlSupport.EnumerationDef("slategray", new Format.PropertyValue(new Format.RGBT(7372944U))),
      new HtmlSupport.EnumerationDef("snow", new Format.PropertyValue(new Format.RGBT(16775930U))),
      new HtmlSupport.EnumerationDef("springgreen", new Format.PropertyValue(new Format.RGBT(65407U))),
      new HtmlSupport.EnumerationDef("steelblue", new Format.PropertyValue(new Format.RGBT(4620980U))),
      new HtmlSupport.EnumerationDef("tan", new Format.PropertyValue(new Format.RGBT(13808780U))),
      new HtmlSupport.EnumerationDef("teal", new Format.PropertyValue(new Format.RGBT(32896U))),
      new HtmlSupport.EnumerationDef("thistle", new Format.PropertyValue(new Format.RGBT(14204888U))),
      new HtmlSupport.EnumerationDef("threeddarkshadow", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.ThreeDDarkShadow)),
      new HtmlSupport.EnumerationDef("threedface", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.ButtonFace)),
      new HtmlSupport.EnumerationDef("threedhighlight", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.ButtonHighlight)),
      new HtmlSupport.EnumerationDef("threedlightshadow", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.ThreeDLightShadow)),
      new HtmlSupport.EnumerationDef("threedshadow", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.ButtonShadow)),
      new HtmlSupport.EnumerationDef("tomato", new Format.PropertyValue(new Format.RGBT(16737095U))),
      new HtmlSupport.EnumerationDef("transparent", new Format.PropertyValue(new Format.RGBT(0U, 0U, 0U, 7U))),
      new HtmlSupport.EnumerationDef("turquoise", new Format.PropertyValue(new Format.RGBT(4251856U))),
      new HtmlSupport.EnumerationDef("violet", new Format.PropertyValue(new Format.RGBT(15631086U))),
      new HtmlSupport.EnumerationDef("wheat", new Format.PropertyValue(new Format.RGBT(16113331U))),
      new HtmlSupport.EnumerationDef("white", new Format.PropertyValue(new Format.RGBT(16777215U))),
      new HtmlSupport.EnumerationDef("whitesmoke", new Format.PropertyValue(new Format.RGBT(16119285U))),
      new HtmlSupport.EnumerationDef("window", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.Window)),
      new HtmlSupport.EnumerationDef("windowframe", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.WindowFrame)),
      new HtmlSupport.EnumerationDef("windowtext", new Format.PropertyValue((Enum) HtmlSupport.SystemColors.WindowText)),
      new HtmlSupport.EnumerationDef("yellow", new Format.PropertyValue(new Format.RGBT(16776960U))),
      new HtmlSupport.EnumerationDef("yellowgreen", new Format.PropertyValue(new Format.RGBT(10145074U)))
    };
    private static Dictionary<Format.PropertyValue, string> colorToNameDictionary = HtmlSupport.BuildColorToNameDictionary();
    private static HtmlSupport.EnumerationDef[] cssTextDecorationEnumeration = new HtmlSupport.EnumerationDef[5]
    {
      new HtmlSupport.EnumerationDef("underline", new Format.PropertyValue((Enum) HtmlSupport.TextDecoration.Underline)),
      new HtmlSupport.EnumerationDef("overline", new Format.PropertyValue((Enum) HtmlSupport.TextDecoration.Overline)),
      new HtmlSupport.EnumerationDef("line-through", new Format.PropertyValue((Enum) HtmlSupport.TextDecoration.LineThrough)),
      new HtmlSupport.EnumerationDef("blink", new Format.PropertyValue((Enum) HtmlSupport.TextDecoration.Blink)),
      new HtmlSupport.EnumerationDef("none", new Format.PropertyValue((Enum) HtmlSupport.TextDecoration.None))
    };
    private static HtmlSupport.EnumerationDef[] cssTextTransformEnumeration = new HtmlSupport.EnumerationDef[4]
    {
      new HtmlSupport.EnumerationDef("capitalize", new Format.PropertyValue((Enum) HtmlSupport.TextTransform.Capitalize)),
      new HtmlSupport.EnumerationDef("uppercase", new Format.PropertyValue((Enum) HtmlSupport.TextTransform.Uppercase)),
      new HtmlSupport.EnumerationDef("lowercase", new Format.PropertyValue((Enum) HtmlSupport.TextTransform.Lowercase)),
      new HtmlSupport.EnumerationDef("none", new Format.PropertyValue((Enum) HtmlSupport.TextTransform.None))
    };
    private static HtmlSupport.EnumerationDef[] cssVerticalAlignmentEnumeration = new HtmlSupport.EnumerationDef[8]
    {
      new HtmlSupport.EnumerationDef("baseline", new Format.PropertyValue((Enum) Format.Align.BaseLine)),
      new HtmlSupport.EnumerationDef("sub", new Format.PropertyValue((Enum) Format.Align.Sub)),
      new HtmlSupport.EnumerationDef("super", new Format.PropertyValue((Enum) Format.Align.Super)),
      new HtmlSupport.EnumerationDef("top", new Format.PropertyValue((Enum) Format.Align.Top)),
      new HtmlSupport.EnumerationDef("text-top", new Format.PropertyValue((Enum) Format.Align.TextTop)),
      new HtmlSupport.EnumerationDef("middle", new Format.PropertyValue((Enum) Format.Align.Middle)),
      new HtmlSupport.EnumerationDef("bottom", new Format.PropertyValue((Enum) Format.Align.Bottom)),
      new HtmlSupport.EnumerationDef("text-bottom", new Format.PropertyValue((Enum) Format.Align.TextBottom))
    };
    private static HtmlSupport.EnumerationDef[] cssWhiteSpaceEnumeration = new HtmlSupport.EnumerationDef[5]
    {
      new HtmlSupport.EnumerationDef("normal", new Format.PropertyValue((Enum) HtmlSupport.CssWhiteSpace.Normal)),
      new HtmlSupport.EnumerationDef("pre", new Format.PropertyValue((Enum) HtmlSupport.CssWhiteSpace.Pre)),
      new HtmlSupport.EnumerationDef("nowrap", new Format.PropertyValue((Enum) HtmlSupport.CssWhiteSpace.Nowrap)),
      new HtmlSupport.EnumerationDef("pre-wrap", new Format.PropertyValue((Enum) HtmlSupport.CssWhiteSpace.PreWrap)),
      new HtmlSupport.EnumerationDef("pre-line", new Format.PropertyValue((Enum) HtmlSupport.CssWhiteSpace.PreLine))
    };
    public const int HtmlNestingLimit = 4096;
    public const int MaxAttributeSize = 4096;
    public const int MaxCssPropertySize = 4096;
    public const int MaxNumberOfNonInlineStyles = 128;

    public static Format.PropertyValue ParseNumber(BufferString value, HtmlSupport.NumberParseFlags parseFlags)
    {
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      ulong num1 = 0UL;
      int num2 = 0;
      int num3 = 0;
      bool flag4 = false;
      int index1 = 0;
      int length = value.Length;
      while (index1 < length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[index1])))
        ++index1;
      if (index1 == length)
        return Format.PropertyValue.Null;
      if (index1 < length && ((int) value[index1] == 45 || (int) value[index1] == 43))
      {
        flag2 = true;
        flag3 = (int) value[index1] == 45;
        ++index1;
      }
      for (; index1 < length && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(value[index1])); ++index1)
      {
        flag1 = true;
        if (num1 < 1844674407370955152UL)
          num1 = num1 * 10UL + (ulong) ((uint) value[index1] - 48U);
        else
          ++num2;
      }
      if (index1 < length && (int) value[index1] == 46)
      {
        flag4 = true;
        for (++index1; index1 < length && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(value[index1])); ++index1)
        {
          flag1 = true;
          if (num1 < 1844674407370955152UL)
          {
            num1 = num1 * 10UL + (ulong) ((uint) value[index1] - 48U);
            --num2;
          }
        }
        if (num2 >= 0 && (parseFlags & HtmlSupport.NumberParseFlags.Strict) != (HtmlSupport.NumberParseFlags) 0)
          return Format.PropertyValue.Null;
      }
      if (!flag1)
        return Format.PropertyValue.Null;
      while (index1 < length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[index1])))
        ++index1;
      if (index1 < length && ((int) value[index1] | 32) == 101 && index1 + 1 < length && ((int) value[index1 + 1] == 45 || (int) value[index1 + 1] == 43 || ParseSupport.NumericCharacter(ParseSupport.GetCharClass(value[index1 + 1]))))
      {
        flag4 = true;
        ++index1;
        bool flag5 = false;
        if ((int) value[index1] == 45 || (int) value[index1] == 43)
        {
          flag5 = (int) value[index1] == 45;
          ++index1;
        }
        while (index1 < length && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(value[index1])))
          num3 = num3 * 10 + ((int) value[index1++] - 48);
        if (flag5)
          num3 = -num3;
        while (index1 < length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[index1])))
          ++index1;
      }
      uint num4 = flag4 ? 10000U : 1U;
      uint num5 = 1U;
      Format.PropertyType type = flag4 ? Format.PropertyType.Fractional : Format.PropertyType.Integer;
      bool flag6 = false;
      int num6 = 0;
      if (index1 + 1 < length)
      {
        if (((int) value[index1] | 32) == 112)
        {
          if (((int) value[index1 + 1] | 32) == 99)
          {
            num4 = 1920U;
            num5 = 1U;
            flag6 = true;
            type = Format.PropertyType.AbsLength;
            num6 = 2;
          }
          else if (((int) value[index1 + 1] | 32) == 116)
          {
            num4 = 160U;
            num5 = 1U;
            flag6 = true;
            type = Format.PropertyType.AbsLength;
            num6 = 2;
          }
          else if (((int) value[index1 + 1] | 32) == 120)
          {
            num4 = 11520U;
            num5 = 120U;
            type = Format.PropertyType.Pixels;
            flag6 = true;
            num6 = 2;
          }
        }
        else if (((int) value[index1] | 32) == 101)
        {
          if (((int) value[index1 + 1] | 32) == 109)
          {
            num4 = 160U;
            num5 = 1U;
            type = Format.PropertyType.Ems;
            flag6 = true;
            num6 = 2;
          }
          else if (((int) value[index1 + 1] | 32) == 120)
          {
            num4 = 160U;
            num5 = 1U;
            type = Format.PropertyType.Exs;
            flag6 = true;
            num6 = 2;
          }
        }
        else if (((int) value[index1] | 32) == 105)
        {
          if (((int) value[index1 + 1] | 32) == 110)
          {
            num4 = 11520U;
            num5 = 1U;
            flag6 = true;
            type = Format.PropertyType.AbsLength;
            num6 = 2;
          }
        }
        else if (((int) value[index1] | 32) == 99)
        {
          if (((int) value[index1 + 1] | 32) == 109)
          {
            num4 = 1152000U;
            num5 = 254U;
            flag6 = true;
            type = Format.PropertyType.AbsLength;
            num6 = 2;
          }
        }
        else if (((int) value[index1] | 32) == 109 && ((int) value[index1 + 1] | 32) == 109)
        {
          num4 = 115200U;
          num5 = 254U;
          flag6 = true;
          type = Format.PropertyType.AbsLength;
          num6 = 2;
        }
      }
      bool flag7;
      if (!flag6 && index1 < length)
      {
        if ((int) value[index1] == 37)
        {
          num4 = 10000U;
          num5 = 1U;
          type = Format.PropertyType.Percentage;
          flag7 = true;
          num6 = 1;
        }
        else if ((int) value[index1] == 42)
        {
          num4 = 1U;
          num5 = 1U;
          type = Format.PropertyType.Multiple;
          flag7 = true;
          num6 = 1;
        }
      }
      int index2 = index1 + num6;
      if (index2 < length)
      {
        while (index2 < length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[index2])))
          ++index2;
        if (index2 < length && (parseFlags & (HtmlSupport.NumberParseFlags.StyleSheetProperty | HtmlSupport.NumberParseFlags.Strict)) != (HtmlSupport.NumberParseFlags) 0)
          return Format.PropertyValue.Null;
      }
      if ((long) num1 != 0L)
      {
        int num7 = num2 + num3;
        if (num7 > 0)
        {
          if (num7 > 20)
          {
            num7 = 0;
            num1 = ulong.MaxValue;
          }
          else
          {
            for (; num7 != 0; --num7)
            {
              if (num1 > 1844674407370955161UL)
              {
                num7 = 0;
                num1 = ulong.MaxValue;
                break;
              }
              num1 *= 10UL;
            }
          }
        }
        else if (num7 < -10)
        {
          if (num7 < -21)
          {
            num7 = 0;
            num1 = 0UL;
          }
          else
          {
            for (; num7 != -10; ++num7)
              num1 /= 10UL;
          }
        }
        num1 = num1 * (ulong) num4 / (ulong) num5;
        for (; num7 != 0; ++num7)
          num1 /= 10UL;
        if (num1 > 67108863UL)
          num1 = 67108863UL;
      }
      int num8 = (int) num1;
      if (flag3)
        num8 = -num8;
      if (type == Format.PropertyType.Integer)
      {
        if ((parseFlags & HtmlSupport.NumberParseFlags.Integer) == (HtmlSupport.NumberParseFlags) 0)
        {
          if ((parseFlags & HtmlSupport.NumberParseFlags.HtmlFontUnits) != (HtmlSupport.NumberParseFlags) 0)
          {
            if (flag2)
            {
              if (num8 < -7)
                num8 = -7;
              else if (num8 > 7)
                num8 = 7;
              type = Format.PropertyType.RelHtmlFontUnits;
            }
            else
            {
              if (num8 < 1)
                num8 = 1;
              else if (num8 > 7)
                num8 = 7;
              type = Format.PropertyType.HtmlFontUnits;
            }
          }
          else if ((parseFlags & HtmlSupport.NumberParseFlags.AbsoluteLength) != (HtmlSupport.NumberParseFlags) 0)
          {
            ulong num7 = num1 * 11520UL / 120UL;
            if (num7 > 67108863UL)
              num7 = 67108863UL;
            num8 = (int) num7;
            if (flag3)
              num8 = -num8;
            type = Format.PropertyType.Pixels;
          }
          else
          {
            if ((parseFlags & HtmlSupport.NumberParseFlags.Float) == (HtmlSupport.NumberParseFlags) 0)
              return Format.PropertyValue.Null;
            ulong num7 = num1 * 10000UL;
            if (num7 > 67108863UL)
              num7 = 67108863UL;
            num8 = (int) num7;
            if (flag3)
              num8 = -num8;
            type = Format.PropertyType.Fractional;
          }
        }
      }
      else if (type == Format.PropertyType.Fractional)
      {
        if ((parseFlags & HtmlSupport.NumberParseFlags.Float) == (HtmlSupport.NumberParseFlags) 0)
        {
          if ((parseFlags & HtmlSupport.NumberParseFlags.AbsoluteLength) == (HtmlSupport.NumberParseFlags) 0)
            return Format.PropertyValue.Null;
          ulong num7 = num1 * 11520UL / 120UL / 10000UL;
          if (num7 > 67108863UL)
            num7 = 67108863UL;
          num8 = (int) num7;
          if (flag3)
            num8 = -num8;
          type = Format.PropertyType.Pixels;
        }
      }
      else if (type == Format.PropertyType.AbsLength || type == Format.PropertyType.Pixels)
      {
        if ((parseFlags & HtmlSupport.NumberParseFlags.AbsoluteLength) == (HtmlSupport.NumberParseFlags) 0)
          return Format.PropertyValue.Null;
      }
      else if (type == Format.PropertyType.Ems || type == Format.PropertyType.Exs)
      {
        if ((parseFlags & HtmlSupport.NumberParseFlags.EmExLength) == (HtmlSupport.NumberParseFlags) 0)
          return Format.PropertyValue.Null;
      }
      else if (type == Format.PropertyType.Percentage)
      {
        if ((parseFlags & HtmlSupport.NumberParseFlags.Percentage) == (HtmlSupport.NumberParseFlags) 0)
          return Format.PropertyValue.Null;
      }
      else if (type == Format.PropertyType.Multiple && (parseFlags & HtmlSupport.NumberParseFlags.Multiple) == (HtmlSupport.NumberParseFlags) 0)
        return Format.PropertyValue.Null;
      if (num8 < 0 && (parseFlags & HtmlSupport.NumberParseFlags.NonNegative) != (HtmlSupport.NumberParseFlags) 0 && type != Format.PropertyType.RelHtmlFontUnits)
        return Format.PropertyValue.Null;
      return new Format.PropertyValue(type, num8);
    }

    public static BufferString FormatPixelOrPercentageLength(ref ScratchBuffer scratchBuffer, Format.PropertyValue value)
    {
      scratchBuffer.Reset();
      HtmlSupport.AppendNumber(ref scratchBuffer, value, HtmlSupport.NumberParseFlags.Integer | HtmlSupport.NumberParseFlags.Percentage);
      return scratchBuffer.BufferString;
    }

    public static BufferString FormatPixelLength(ref ScratchBuffer scratchBuffer, Format.PropertyValue value)
    {
      scratchBuffer.Reset();
      HtmlSupport.AppendNumber(ref scratchBuffer, value, HtmlSupport.NumberParseFlags.Integer);
      return scratchBuffer.BufferString;
    }

    public static BufferString FormatLength(ref ScratchBuffer scratchBuffer, Format.PropertyValue value)
    {
      scratchBuffer.Reset();
      HtmlSupport.AppendNumber(ref scratchBuffer, value, HtmlSupport.NumberParseFlags.Length);
      return scratchBuffer.BufferString;
    }

    public static BufferString FormatFontSize(ref ScratchBuffer scratchBuffer, Format.PropertyValue value)
    {
      scratchBuffer.Reset();
      HtmlSupport.AppendNumber(ref scratchBuffer, value, HtmlSupport.NumberParseFlags.HtmlFontUnits);
      return scratchBuffer.BufferString;
    }

    public static void AppendPixelOrPercentageLength(ref ScratchBuffer scratchBuffer, Format.PropertyValue value)
    {
      HtmlSupport.AppendNumber(ref scratchBuffer, value, HtmlSupport.NumberParseFlags.Integer | HtmlSupport.NumberParseFlags.Percentage);
    }

    public static void AppendPixelLength(ref ScratchBuffer scratchBuffer, Format.PropertyValue value)
    {
      HtmlSupport.AppendNumber(ref scratchBuffer, value, HtmlSupport.NumberParseFlags.Integer);
    }

    public static void AppendLength(ref ScratchBuffer scratchBuffer, Format.PropertyValue value)
    {
      HtmlSupport.AppendNumber(ref scratchBuffer, value, HtmlSupport.NumberParseFlags.Length);
    }

    public static void AppendFontSize(ref ScratchBuffer scratchBuffer, Format.PropertyValue value)
    {
      HtmlSupport.AppendNumber(ref scratchBuffer, value, HtmlSupport.NumberParseFlags.HtmlFontUnits);
    }

    public static void AppendCssFontSize(ref ScratchBuffer scratchBuffer, Format.PropertyValue value)
    {
      HtmlSupport.AppendNumber(ref scratchBuffer, value, HtmlSupport.NumberParseFlags.FontSize);
    }

    private static void AppendNumber(ref ScratchBuffer scratchBuffer, Format.PropertyValue value, HtmlSupport.NumberParseFlags formatFlags)
    {
      if (value.IsPercentage)
      {
        if ((formatFlags & HtmlSupport.NumberParseFlags.Percentage) == (HtmlSupport.NumberParseFlags) 0)
          return;
        scratchBuffer.AppendFractional(value.Percentage10K, 10000);
        scratchBuffer.Append('%');
      }
      else if (value.IsAbsRelLength)
      {
        if ((formatFlags & HtmlSupport.NumberParseFlags.Integer) != (HtmlSupport.NumberParseFlags) 0)
          scratchBuffer.AppendInt(value.PixelsInteger);
        else if ((formatFlags & HtmlSupport.NumberParseFlags.AbsoluteLength) != (HtmlSupport.NumberParseFlags) 0)
        {
          if (value.IsPixels)
          {
            int pixelsInteger96 = value.PixelsInteger96;
            scratchBuffer.AppendFractional(pixelsInteger96, 96);
            if (pixelsInteger96 == 0)
              return;
            scratchBuffer.Append("px");
          }
          else
          {
            int pointsInteger160 = value.PointsInteger160;
            scratchBuffer.AppendFractional(pointsInteger160, 160);
            if (pointsInteger160 == 0)
              return;
            scratchBuffer.Append("pt");
          }
        }
        else
        {
          if ((formatFlags & HtmlSupport.NumberParseFlags.HtmlFontUnits) == (HtmlSupport.NumberParseFlags) 0)
            return;
          scratchBuffer.AppendInt(Format.PropertyValue.ConvertTwipsToHtmlFontUnits(value.TwipsInteger));
        }
      }
      else if (value.IsEms)
      {
        if ((formatFlags & HtmlSupport.NumberParseFlags.EmExLength) == (HtmlSupport.NumberParseFlags) 0)
          return;
        scratchBuffer.AppendFractional(value.EmsInteger160, 160);
        scratchBuffer.Append("em");
      }
      else if (value.IsExs)
      {
        if ((formatFlags & HtmlSupport.NumberParseFlags.EmExLength) == (HtmlSupport.NumberParseFlags) 0)
          return;
        scratchBuffer.AppendFractional(value.ExsInteger160, 160);
        scratchBuffer.Append("ex");
      }
      else if (value.IsHtmlFontUnits)
      {
        if ((formatFlags & HtmlSupport.NumberParseFlags.HtmlFontUnits) == (HtmlSupport.NumberParseFlags) 0)
          return;
        scratchBuffer.AppendInt(value.HtmlFontUnits);
      }
      else
      {
        if (!value.IsRelativeHtmlFontUnits || (formatFlags & HtmlSupport.NumberParseFlags.HtmlFontUnits) == (HtmlSupport.NumberParseFlags) 0)
          return;
        if (value.RelativeHtmlFontUnits > 0)
        {
          scratchBuffer.Append("+");
          scratchBuffer.AppendInt(value.RelativeHtmlFontUnits);
        }
        else
        {
          if (value.RelativeHtmlFontUnits >= 0)
            return;
          scratchBuffer.AppendInt(value.RelativeHtmlFontUnits);
        }
      }
    }

    public static Format.PropertyValue ParseEnum(BufferString value, HtmlSupport.EnumerationDef[] enumerationDefs)
    {
      value.TrimWhitespace();
      if (value.Length == 0)
        return Format.PropertyValue.Null;
      for (int index = 0; index < enumerationDefs.Length; ++index)
      {
        if (value.EqualsToLowerCaseStringIgnoreCase(enumerationDefs[index].Name))
          return enumerationDefs[index].Value;
      }
      return Format.PropertyValue.Null;
    }

    public static string GetEnumString(Format.PropertyValue value, HtmlSupport.EnumerationDef[] enumerationDefs)
    {
      for (int index = 0; index < enumerationDefs.Length; ++index)
      {
        if ((int) value.RawValue == (int) enumerationDefs[index].Value.RawValue)
          return enumerationDefs[index].Name;
      }
      return (string) null;
    }

    public static Format.PropertyValue ParseBooleanAttribute(BufferString value, Format.FormatConverter formatConverter)
    {
      return Format.PropertyValue.True;
    }

    internal static Format.PropertyValue ParseDirection(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseEnum(value, HtmlSupport.directionEnumeration);
    }

    internal static Format.PropertyValue ParseTextAlignment(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseEnum(value, HtmlSupport.TextAlignmentEnumeration);
    }

    internal static string GetTextAlignmentString(Format.PropertyValue value)
    {
      return HtmlSupport.GetEnumString(value, HtmlSupport.TextAlignmentEnumeration);
    }

    internal static Format.PropertyValue ParseHorizontalAlignment(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseEnum(value, HtmlSupport.HorizontalAlignmentEnumeration);
    }

    internal static string GetHorizontalAlignmentString(Format.PropertyValue value)
    {
      return HtmlSupport.GetEnumString(value, HtmlSupport.HorizontalAlignmentEnumeration);
    }

    internal static Format.PropertyValue ParseVerticalAlignment(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseEnum(value, HtmlSupport.verticalAlignmentEnumeration);
    }

    internal static string GetVerticalAlignmentString(Format.PropertyValue value)
    {
      return HtmlSupport.GetEnumString(value, HtmlSupport.verticalAlignmentEnumeration);
    }

    internal static Format.PropertyValue ParseBlockAlignment(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseEnum(value, HtmlSupport.BlockAlignmentEnumeration);
    }

    internal static string GetBlockAlignmentString(Format.PropertyValue value)
    {
      return HtmlSupport.GetEnumString(value, HtmlSupport.BlockAlignmentEnumeration);
    }

    internal static Format.PropertyValue ParseBorderStyle(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseEnum(value, HtmlSupport.BorderStyleEnumeration);
    }

    internal static string GetBorderStyleString(Format.PropertyValue value)
    {
      return HtmlSupport.GetEnumString(value, HtmlSupport.BorderStyleEnumeration);
    }

    internal static Format.PropertyValue ParseTarget(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseEnum(value, HtmlSupport.targetEnumeration);
    }

    internal static string GetTargetString(Format.PropertyValue value)
    {
      return HtmlSupport.GetEnumString(value, HtmlSupport.targetEnumeration);
    }

    internal static Format.PropertyValue ParseFontWeight(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseEnum(value, HtmlSupport.fontWeightEnumeration);
    }

    internal static Format.PropertyValue ParseCssFontSize(BufferString value, Format.FormatConverter formatConverter)
    {
      Format.PropertyValue propertyValue = HtmlSupport.ParseEnum(value, HtmlSupport.fontSizeEnumeration);
      if (propertyValue.IsNull)
        propertyValue = HtmlSupport.ParseFontSize(value, formatConverter);
      return propertyValue;
    }

    internal static Format.PropertyValue ParseFontStyle(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseEnum(value, HtmlSupport.fontStyleEnumeration);
    }

    internal static Format.PropertyValue ParseFontVariant(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseEnum(value, HtmlSupport.fontVariantEnumeration);
    }

    internal static Format.PropertyValue ParseTableLayout(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseEnum(value, HtmlSupport.tableLayoutEnumeration);
    }

    internal static Format.PropertyValue ParseBorderCollapse(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseEnum(value, HtmlSupport.borderCollapseEnumeration);
    }

    internal static Format.PropertyValue ParseEmptyCells(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseEnum(value, HtmlSupport.emptyCellsEnumeration);
    }

    internal static Format.PropertyValue ParseCaptionSide(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseEnum(value, HtmlSupport.captionSideEnumeration);
    }

    internal static Format.PropertyValue ParseBorderWidth(BufferString value, Format.FormatConverter formatConverter)
    {
      Format.PropertyValue propertyValue = HtmlSupport.ParseEnum(value, HtmlSupport.borderWidthEnumeration);
      if (propertyValue.IsNull)
        propertyValue = HtmlSupport.ParseNonNegativeLength(value, formatConverter);
      return propertyValue;
    }

    internal static Format.PropertyValue ParseTableFrame(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseEnum(value, HtmlSupport.tableFrameEnumeration);
    }

    internal static string GetTableFrameString(Format.PropertyValue value)
    {
      return HtmlSupport.GetEnumString(value, HtmlSupport.tableFrameEnumeration);
    }

    internal static Format.PropertyValue ParseTableRules(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseEnum(value, HtmlSupport.tableRulesEnumeration);
    }

    internal static string GetTableRulesString(Format.PropertyValue value)
    {
      return HtmlSupport.GetEnumString(value, HtmlSupport.tableRulesEnumeration);
    }

    internal static Format.PropertyValue ParseUnicodeBiDi(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseEnum(value, HtmlSupport.unicodeBiDiEnumeration);
    }

    internal static string GetUnicodeBiDiString(Format.PropertyValue value)
    {
      return HtmlSupport.GetEnumString(value, HtmlSupport.unicodeBiDiEnumeration);
    }

    internal static Format.PropertyValue ParseDisplay(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseEnum(value, HtmlSupport.displayEnumeration);
    }

    internal static string GetDisplayString(Format.PropertyValue value)
    {
      return HtmlSupport.GetEnumString(value, HtmlSupport.displayEnumeration);
    }

    internal static Format.PropertyValue ParseVisibility(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseEnum(value, HtmlSupport.visibilityEnumeration);
    }

    internal static Format.PropertyValue ParseLanguage(BufferString value, Format.FormatConverter formatConverter)
    {
      value.TrimWhitespace();
      Globalization.Culture culture;
      if (value.Length == 0 || !Globalization.Culture.TryGetCulture(value.ToString(), out culture) || culture.LCID == 0)
        return Format.PropertyValue.Null;
      return new Format.PropertyValue(Format.PropertyType.Integer, culture.LCID);
    }

    public static Format.PropertyValue ParseColor(BufferString value, bool enriched, bool css)
    {
      int offset = 0;
      if (value.Length == 0 || (int) value[0] != 35 || enriched)
      {
        bool flag1 = false;
        bool flag2 = false;
        for (int index = 0; index < value.Length; ++index)
        {
          if (!ParseSupport.AlphaCharacter(ParseSupport.GetCharClass(value[index])))
          {
            flag1 = true;
            break;
          }
          if (!flag2 && !ParseSupport.HexCharacter(ParseSupport.GetCharClass(value[index])))
            flag2 = true;
        }
        if (!flag1 && flag2)
        {
          Format.PropertyValue propertyValue = HtmlSupport.ParseNamedColor(value);
          if (!propertyValue.IsNull)
            return propertyValue;
        }
        if (!enriched)
        {
          Format.PropertyValue propertyValue = HtmlSupport.ParseRgbColor(value);
          if (!propertyValue.IsNull)
            return propertyValue;
        }
      }
      else
        ++offset;
      if (value.Length > 0)
      {
        if (enriched)
        {
          Format.RGBT rgbt;
          if (HtmlSupport.ParseHexColorEnriched(value, offset, out rgbt))
            return new Format.PropertyValue(rgbt);
        }
        else
        {
          Format.RGBT rgbt;
          if (HtmlSupport.ParseHexColor(value, offset, css, out rgbt))
            return new Format.PropertyValue(rgbt);
        }
      }
      return Format.PropertyValue.Null;
    }

    private static Dictionary<Format.PropertyValue, string> BuildColorToNameDictionary()
    {
      Dictionary<Format.PropertyValue, string> dictionary = new Dictionary<Format.PropertyValue, string>();
      foreach (HtmlSupport.EnumerationDef enumerationDef in HtmlSupport.colorNames)
      {
        if (!dictionary.ContainsKey(enumerationDef.Value))
          dictionary.Add(enumerationDef.Value, enumerationDef.Name);
      }
      return dictionary;
    }

    private static Format.PropertyValue ParseNamedColor(BufferString value)
    {
      int num1 = 0;
      int num2 = HtmlSupport.colorNames.Length - 1;
      while (num1 <= num2)
      {
        int index = num1 + (num2 - num1 >> 1);
        int num3 = BufferString.CompareLowerCaseStringToBufferStringIgnoreCase(HtmlSupport.colorNames[index].Name, value);
        if (num3 == 0)
          return HtmlSupport.colorNames[index].Value;
        if (num3 < 0)
          num1 = index + 1;
        else
          num2 = index - 1;
      }
      return Format.PropertyValue.Null;
    }

    internal static Format.PropertyValue TranslateSystemColor(Format.PropertyValue value)
    {
      switch (value.Enum)
      {
        case 0:
          return new Format.PropertyValue(new Format.RGBT(16777215U));
        case 1:
          return new Format.PropertyValue(new Format.RGBT(16777215U));
        case 2:
          return new Format.PropertyValue(new Format.RGBT(16777215U));
        case 3:
          return new Format.PropertyValue(new Format.RGBT(16777215U));
        case 4:
          return new Format.PropertyValue(new Format.RGBT(16777215U));
        case 5:
          return new Format.PropertyValue(new Format.RGBT(16777215U));
        case 6:
          return new Format.PropertyValue(new Format.RGBT(0U));
        case 7:
          return new Format.PropertyValue(new Format.RGBT(0U));
        case 8:
          return new Format.PropertyValue(new Format.RGBT(0U));
        case 9:
          return new Format.PropertyValue(new Format.RGBT(0U));
        case 10:
          return new Format.PropertyValue(new Format.RGBT(0U));
        case 11:
          return new Format.PropertyValue(new Format.RGBT(0U));
        case 12:
          return new Format.PropertyValue(new Format.RGBT(16777215U));
        case 13:
          return new Format.PropertyValue(new Format.RGBT(16777215U));
        case 14:
          return new Format.PropertyValue(new Format.RGBT(0U));
        case 15:
          return new Format.PropertyValue(new Format.RGBT(16777215U));
        case 16:
          return new Format.PropertyValue(new Format.RGBT(16777215U));
        case 17:
          return new Format.PropertyValue(new Format.RGBT(0U));
        case 18:
          return new Format.PropertyValue(new Format.RGBT(0U));
        case 19:
          return new Format.PropertyValue(new Format.RGBT(0U));
        case 20:
          return new Format.PropertyValue(new Format.RGBT(16777215U));
        case 21:
          return new Format.PropertyValue(new Format.RGBT(16777215U));
        case 22:
          return new Format.PropertyValue(new Format.RGBT(16777215U));
        case 23:
          return new Format.PropertyValue(new Format.RGBT(0U));
        case 24:
          return new Format.PropertyValue(new Format.RGBT(16777215U));
        default:
          return Format.PropertyValue.Null;
      }
    }

    private static Format.PropertyValue ParseRgbColor(BufferString value)
    {
      if (value.Length <= 4 || !value.StartsWithLowerCaseStringIgnoreCase("rgb("))
        return Format.PropertyValue.Null;
      int offset = 4;
      uint result1;
      uint result2;
      uint result3;
      if (!HtmlSupport.ParseRgbParam(value, ref offset, out result1) || !HtmlSupport.ParseRgbParam(value, ref offset, out result2) || (!HtmlSupport.ParseRgbParam(value, ref offset, out result3) || offset != value.Length - 1) || (int) value[offset] != 41)
        return Format.PropertyValue.Null;
      return new Format.PropertyValue(new Format.RGBT(result1, result2, result3));
    }

    private static bool ParseRgbParam(BufferString str, ref int offset, out uint result)
    {
      uint num1 = 0U;
      uint num2 = 1U;
      bool flag = false;
      while (offset < str.Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(str[offset])))
        ++offset;
      if (offset < str.Length && (int) str[offset] == 45)
      {
        flag = true;
        ++offset;
      }
      if (offset == str.Length || !ParseSupport.NumericCharacter(ParseSupport.GetCharClass(str[offset])))
      {
        result = 0U;
        return false;
      }
label_10:
      while (offset < str.Length && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(str[offset])))
      {
        num1 = num1 * 10U + ((uint) str[offset] - 48U);
        ++offset;
        if (num1 > (uint) byte.MaxValue)
        {
          while (true)
          {
            if (offset < str.Length && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(str[offset])))
              ++offset;
            else
              goto label_10;
          }
        }
      }
      if (offset < str.Length && (int) str[offset] == 46)
      {
        ++offset;
label_16:
        while (offset < str.Length && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(str[offset])))
        {
          num1 = num1 * 10U + ((uint) str[offset] - 48U);
          num2 *= 10U;
          ++offset;
          if (num1 > 421075U)
          {
            while (true)
            {
              if (offset < str.Length && ParseSupport.NumericCharacter(ParseSupport.GetCharClass(str[offset])))
                ++offset;
              else
                goto label_16;
            }
          }
        }
      }
      if (offset < str.Length && (int) str[offset] == 37)
      {
        result = num1 / num2 < 100U ? num1 * (uint) byte.MaxValue / (num2 * 100U) : (uint) byte.MaxValue;
        ++offset;
      }
      else
        result = num1 / num2 <= (uint) byte.MaxValue ? num1 / num2 : (uint) byte.MaxValue;
      while (offset < str.Length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(str[offset])))
        ++offset;
      if (offset < str.Length && (int) str[offset] == 44)
        ++offset;
      if (flag)
        result = 0U;
      return true;
    }

    private static bool ParseHexColor(BufferString str, int offset, bool css, out Format.RGBT rgbt)
    {
      int num1 = str.Length - offset;
      if (css && num1 != 3 && (num1 != 6 && num1 != 9))
      {
        rgbt = new Format.RGBT();
        return false;
      }
      int vlen = (str.Length - offset + 3 - 1) / 3;
      uint max = 0U;
      uint result1;
      uint result2;
      uint result3;
      if (!HtmlSupport.ParseHexColorPart(vlen, str, ref offset, ref max, css, out result1) || !HtmlSupport.ParseHexColorPart(vlen, str, ref offset, ref max, css, out result2) || !HtmlSupport.ParseHexColorPart(vlen, str, ref offset, ref max, css, out result3))
      {
        rgbt = new Format.RGBT();
        return false;
      }
      int num2 = 0;
      while (max > (uint) byte.MaxValue)
      {
        max >>= 4;
        ++num2;
      }
      if (num2 > 0)
      {
        result1 >>= num2 * 4;
        result2 >>= num2 * 4;
        result3 >>= num2 * 4;
      }
      if (css && vlen == 1)
      {
        result1 += result1 << 4;
        result2 += result2 << 4;
        result3 += result3 << 4;
      }
      rgbt = new Format.RGBT(result1, result2, result3);
      return true;
    }

    private static bool ParseHexColorPart(int vlen, BufferString str, ref int offset, ref uint max, bool css, out uint result)
    {
      result = 0U;
      for (int index = 0; index < vlen; ++index)
      {
        int num;
        if (offset >= str.Length)
        {
          if (css)
            return false;
          num = 0;
        }
        else if (ParseSupport.HexCharacter(ParseSupport.GetCharClass(str[offset])))
        {
          num = ParseSupport.CharToHex(str[offset]);
        }
        else
        {
          if (css)
            return false;
          num = 0;
        }
        result = (uint) (((int) result << 4) + num);
        ++offset;
      }
      if (result > max)
        max = result;
      return true;
    }

    private static bool ParseHexColorEnriched(BufferString str, int offset, out Format.RGBT rgbt)
    {
      uint result1;
      uint result2;
      uint result3;
      if (!HtmlSupport.ParseHexColorPartEnriched(str, ref offset, out result1) || !HtmlSupport.ParseHexColorPartEnriched(str, ref offset, out result2) || !HtmlSupport.ParseHexColorPartEnriched(str, ref offset, out result3))
      {
        rgbt = new Format.RGBT();
        return false;
      }
      uint red = result1 >> 8;
      uint green = result2 >> 8;
      uint blue = result3 >> 8;
      rgbt = new Format.RGBT(red, green, blue);
      return true;
    }

    private static bool ParseHexColorPartEnriched(BufferString str, ref int offset, out uint result)
    {
      result = 0U;
      for (int index = 0; index < 4; ++index)
      {
        int num;
        if (offset >= str.Length)
        {
          num = 0;
        }
        else
        {
          num = !ParseSupport.HexCharacter(ParseSupport.GetCharClass(str[offset])) ? 0 : ParseSupport.CharToHex(str[offset]);
          ++offset;
        }
        result = (uint) (((int) result << 4) + num);
      }
      if (offset < str.Length && (int) str[offset] == 44)
        ++offset;
      return true;
    }

    internal static Format.PropertyValue ParseStringProperty(BufferString value, Format.FormatConverter formatConverter)
    {
      value.TrimWhitespace();
      if (value.Length == 0)
        return Format.PropertyValue.Null;
      return formatConverter.RegisterStringValue(false, value.ToString(), 0, value.Length).PropertyValue;
    }

    internal static Format.PropertyValue ParseFontFace(BufferString value, Format.FormatConverter formatConverter)
    {
      int offset1 = 0;
      int length = value.Length;
      while (offset1 < length && ((int) value[offset1] == 44 || ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[offset1]))))
        ++offset1;
      if (offset1 == length)
        return Format.PropertyValue.Null;
      char ch1 = ',';
      if ((int) value[offset1] == 39 || (int) value[offset1] == 34)
      {
        ch1 = value[offset1];
        ++offset1;
      }
      int index1 = offset1;
      int num1 = offset1;
      while (index1 < length && (int) value[index1] != (int) ch1)
      {
        ++index1;
        ++num1;
      }
      while (offset1 < index1 && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[index1 - 1])))
        --index1;
      Format.PropertyValue pv = formatConverter.RegisterFaceName(false, value.SubString(offset1, index1 - offset1));
      if (num1 < length)
        ++num1;
      int offset2 = num1;
      while (offset2 < length && ((int) value[offset2] == 44 || ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[offset2]))))
        ++offset2;
      if (offset2 == length)
        return pv;
      Format.MultiValueBuilder builder;
      Format.MultiValue multiValue = formatConverter.RegisterMultiValue(false, out builder);
      if (!pv.IsNull)
        builder.AddValue(pv);
      do
      {
        char ch2 = ',';
        if ((int) value[offset2] == 39 || (int) value[offset2] == 34)
        {
          ch2 = value[offset2];
          ++offset2;
        }
        int index2 = offset2;
        int num2 = offset2;
        while (index2 < length && (int) value[index2] != (int) ch2)
        {
          ++index2;
          ++num2;
        }
        int num3 = index2;
        while (offset2 < index2 && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[index2 - 1])))
          --index2;
        pv = formatConverter.RegisterFaceName(false, value.SubString(offset2, index2 - offset2));
        if (!pv.IsNull)
          builder.AddValue(pv);
        if (num3 < length)
          ++num3;
        offset2 = num3;
        while (offset2 < length && ((int) value[offset2] == 44 || ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[offset2]))))
          ++offset2;
      }
      while (offset2 < length);
      if (builder.Count == 0)
      {
        builder.Cancel();
        multiValue.Release();
        return Format.PropertyValue.Null;
      }
      if (builder.Count == 1)
      {
        pv = builder[0];
        if (pv.IsString)
          formatConverter.GetStringValue(pv).AddRef();
        builder.Cancel();
        multiValue.Release();
        return pv;
      }
      builder.Flush();
      return multiValue.PropertyValue;
    }

    internal static Format.PropertyValue ParseColor(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseColor(value, false, false);
    }

    internal static Format.PropertyValue ParseColorCss(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseColor(value, false, true);
    }

    public static BufferString FormatColor(ref ScratchBuffer scratchBuffer, Format.PropertyValue value)
    {
      scratchBuffer.Reset();
      HtmlSupport.AppendColor(ref scratchBuffer, value);
      return scratchBuffer.BufferString;
    }

    public static void AppendColor(ref ScratchBuffer scratchBuffer, Format.PropertyValue value)
    {
      if (!value.IsColor && !value.IsEnum)
        return;
      string str;
      if (HtmlSupport.colorToNameDictionary.TryGetValue(value, out str))
      {
        scratchBuffer.Append(str);
      }
      else
      {
        scratchBuffer.Append("#");
        scratchBuffer.AppendHex2(value.Color.Red);
        scratchBuffer.AppendHex2(value.Color.Green);
        scratchBuffer.AppendHex2(value.Color.Blue);
      }
    }

    internal static Format.PropertyValue ParseFontSize(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseNumber(value, HtmlSupport.NumberParseFlags.FontSize);
    }

    internal static Format.PropertyValue ParseInteger(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseNumber(value, HtmlSupport.NumberParseFlags.Integer);
    }

    internal static Format.PropertyValue ParseNonNegativeInteger(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseNumber(value, HtmlSupport.NumberParseFlags.Integer | HtmlSupport.NumberParseFlags.NonNegative);
    }

    internal static Format.PropertyValue ParseLength(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseNumber(value, HtmlSupport.NumberParseFlags.Length);
    }

    internal static Format.PropertyValue ParseNonNegativeLength(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseNumber(value, HtmlSupport.NumberParseFlags.NonNegativeLength);
    }

    internal static Format.PropertyValue ParseUrl(BufferString value, Format.FormatConverter formatConverter)
    {
      return HtmlSupport.ParseStringProperty(value, formatConverter);
    }

    public static void ScanSkipWhitespace(ref BufferString value)
    {
      int length = value.Length;
      int offset;
      for (offset = value.Offset; length != 0 && ParseSupport.WhitespaceCharacter(value.Buffer[offset]); --length)
        ++offset;
      value.Trim(offset - value.Offset, length);
    }

    public static void ScanRevertLastToken(ref BufferString value, BufferString token)
    {
      value.Set(value.Buffer, token.Offset, value.Length + token.Length);
    }

    public static BufferString ScanNextNonWhitespaceToken(ref BufferString value)
    {
      int length = value.Length;
      int offset;
      for (offset = value.Offset; length != 0 && !ParseSupport.WhitespaceCharacter(value.Buffer[offset]); --length)
        ++offset;
      BufferString bufferString = new BufferString(value.Buffer, value.Offset, offset - value.Offset);
      value.Trim(offset - value.Offset, length);
      return bufferString;
    }

    public static BufferString ScanNextParenthesizedToken(ref BufferString value)
    {
      int length = value.Length;
      int offset = value.Offset;
      char ch1 = '"';
      bool flag = false;
      for (int index = 0; length != 0 && (index != 0 || !ParseSupport.WhitespaceCharacter(value.Buffer[offset])); --length)
      {
        char ch2 = value.Buffer[offset];
        if (!flag)
        {
          if ((int) ch2 == 39 || (int) ch2 == 34)
          {
            flag = true;
            ch1 = ch2;
          }
          else if (index != 0 && (int) ch2 == 41)
            --index;
          else if ((int) ch2 == 40)
            ++index;
        }
        else if ((int) ch2 == (int) ch1)
          flag = false;
        ++offset;
      }
      BufferString bufferString = new BufferString(value.Buffer, value.Offset, offset - value.Offset);
      value.Trim(offset - value.Offset, length);
      return bufferString;
    }

    public static BufferString ScanNextSize(ref BufferString value)
    {
      int length = value.Length;
      int index = value.Offset;
      if (length != 0)
      {
        switch (value.Buffer[index])
        {
          case '-':
          case '+':
            ++index;
            --length;
            break;
        }
        char ch1;
        if (length == 0 || !ParseSupport.NumericCharacter(ch1 = value.Buffer[index]) && (int) ch1 != 46)
        {
          char ch2;
          for (; length != 0 && (ParseSupport.AlphaCharacter(ch2 = value.Buffer[index]) || (int) ch2 == 45); --length)
            ++index;
        }
        else
        {
          char ch2;
          for (; length != 0 && (ParseSupport.NumericCharacter(ch2 = value.Buffer[index]) || (int) ch2 == 46); --length)
            ++index;
          int num = index;
          for (; length != 0 && ParseSupport.WhitespaceCharacter(value.Buffer[index]); --length)
            ++index;
          char ch3;
          if (length >= 2 && ((int) (ch3 = ParseSupport.ToLowerCase(value.Buffer[index])) == 105 && (int) ParseSupport.ToLowerCase(value.Buffer[index + 1]) == 110 || (int) ch3 == 99 && (int) ParseSupport.ToLowerCase(value.Buffer[index + 1]) == 109 || (int) ch3 == 109 && (int) ParseSupport.ToLowerCase(value.Buffer[index + 1]) == 109 || (int) ch3 == 101 && ((int) ParseSupport.ToLowerCase(value.Buffer[index + 1]) == 109 || (int) ParseSupport.ToLowerCase(value.Buffer[index + 1]) == 120) || (int) ch3 == 112 && ((int) ParseSupport.ToLowerCase(value.Buffer[index + 1]) == 116 || (int) ParseSupport.ToLowerCase(value.Buffer[index + 1]) == 99 || (int) ParseSupport.ToLowerCase(value.Buffer[index + 1]) == 120)))
          {
            index += 2;
            length -= 2;
          }
          else if (length != 0 && (int) value.Buffer[index] == 37)
          {
            ++index;
            --length;
          }
          else
          {
            length += index - num;
            index = num;
          }
        }
      }
      BufferString bufferString = new BufferString(value.Buffer, value.Offset, index - value.Offset);
      value.Trim(index - value.Offset, length);
      return bufferString;
    }

    internal static void ParseCompositeFourSidesValue(BufferString value, Format.FormatConverter formatConverter, Format.PropertyId groupPropertyId, Format.Property[] outputProperties, out int parsedPropertiesCount, PropertyValueParsingMethod valueParsingMethod, bool measurements)
    {
      BufferString bufferString1 = BufferString.Null;
      HtmlSupport.ScanSkipWhitespace(ref value);
      int index = 0;
      if (!value.IsEmpty)
      {
        BufferString bufferString2 = measurements ? HtmlSupport.ScanNextSize(ref value) : HtmlSupport.ScanNextParenthesizedToken(ref value);
        outputProperties[index].Value = valueParsingMethod(bufferString2, formatConverter);
        if (!outputProperties[index].Value.IsNull)
        {
          ++index;
          HtmlSupport.ScanSkipWhitespace(ref value);
          if (!value.IsEmpty)
          {
            BufferString bufferString3 = measurements ? HtmlSupport.ScanNextSize(ref value) : HtmlSupport.ScanNextParenthesizedToken(ref value);
            outputProperties[index].Value = valueParsingMethod(bufferString3, formatConverter);
            if (!outputProperties[index].Value.IsNull)
            {
              ++index;
              HtmlSupport.ScanSkipWhitespace(ref value);
              if (!value.IsEmpty)
              {
                BufferString bufferString4 = measurements ? HtmlSupport.ScanNextSize(ref value) : HtmlSupport.ScanNextParenthesizedToken(ref value);
                outputProperties[index].Value = valueParsingMethod(bufferString4, formatConverter);
                if (!outputProperties[index].Value.IsNull)
                {
                  ++index;
                  HtmlSupport.ScanSkipWhitespace(ref value);
                  if (!value.IsEmpty)
                  {
                    BufferString bufferString5 = measurements ? HtmlSupport.ScanNextSize(ref value) : HtmlSupport.ScanNextParenthesizedToken(ref value);
                    outputProperties[index].Value = valueParsingMethod(bufferString5, formatConverter);
                    if (!outputProperties[index].Value.IsNull)
                      ++index;
                  }
                }
              }
            }
          }
        }
      }
      if (index == 1)
      {
        outputProperties[0].Id = groupPropertyId;
        outputProperties[1].Set(groupPropertyId + (byte) 1, outputProperties[0].Value);
        outputProperties[2].Set(groupPropertyId + (byte) 2, outputProperties[0].Value);
        outputProperties[3].Set(groupPropertyId + (byte) 3, outputProperties[0].Value);
        parsedPropertiesCount = 4;
      }
      else if (index == 2)
      {
        outputProperties[0].Id = groupPropertyId;
        outputProperties[1].Id = groupPropertyId + (byte) 1;
        outputProperties[2].Set(groupPropertyId + (byte) 2, outputProperties[0].Value);
        outputProperties[3].Set(groupPropertyId + (byte) 3, outputProperties[1].Value);
        parsedPropertiesCount = 4;
      }
      else if (index == 3)
      {
        outputProperties[0].Id = groupPropertyId;
        outputProperties[1].Id = groupPropertyId + (byte) 1;
        outputProperties[2].Id = groupPropertyId + (byte) 2;
        outputProperties[3].Set(groupPropertyId + (byte) 3, outputProperties[1].Value);
        parsedPropertiesCount = 4;
      }
      else if (index == 4)
      {
        outputProperties[0].Id = groupPropertyId;
        outputProperties[1].Id = groupPropertyId + (byte) 1;
        outputProperties[2].Id = groupPropertyId + (byte) 2;
        outputProperties[3].Id = groupPropertyId + (byte) 3;
        parsedPropertiesCount = 4;
      }
      else
        parsedPropertiesCount = 0;
    }

    internal static void ParseCompositeLength(BufferString value, Format.FormatConverter formatConverter, Format.PropertyId groupPropertyId, Format.Property[] outputProperties, out int parsedPropertiesCount)
    {
      HtmlSupport.ParseCompositeFourSidesValue(value, formatConverter, groupPropertyId, outputProperties, out parsedPropertiesCount, HtmlConverterData.PropertyValueParsingMethods.ParseLength, true);
    }

    internal static void ParseCompositeNonNegativeLength(BufferString value, Format.FormatConverter formatConverter, Format.PropertyId groupPropertyId, Format.Property[] outputProperties, out int parsedPropertiesCount)
    {
      HtmlSupport.ParseCompositeFourSidesValue(value, formatConverter, groupPropertyId, outputProperties, out parsedPropertiesCount, HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength, true);
    }

    internal static void ParseCompositeColor(BufferString value, Format.FormatConverter formatConverter, Format.PropertyId groupPropertyId, Format.Property[] outputProperties, out int parsedPropertiesCount)
    {
      HtmlSupport.ParseCompositeFourSidesValue(value, formatConverter, groupPropertyId, outputProperties, out parsedPropertiesCount, HtmlConverterData.PropertyValueParsingMethods.ParseColorCss, false);
    }

    internal static void ParseCompositeBorderWidth(BufferString value, Format.FormatConverter formatConverter, Format.PropertyId groupPropertyId, Format.Property[] outputProperties, out int parsedPropertiesCount)
    {
      HtmlSupport.ParseCompositeFourSidesValue(value, formatConverter, groupPropertyId, outputProperties, out parsedPropertiesCount, HtmlConverterData.PropertyValueParsingMethods.ParseBorderWidth, true);
    }

    internal static void ParseCompositeBorderStyle(BufferString value, Format.FormatConverter formatConverter, Format.PropertyId groupPropertyId, Format.Property[] outputProperties, out int parsedPropertiesCount)
    {
      HtmlSupport.ParseCompositeFourSidesValue(value, formatConverter, groupPropertyId, outputProperties, out parsedPropertiesCount, HtmlConverterData.PropertyValueParsingMethods.ParseBorderStyle, false);
    }

    internal static void ParseCompoundBorderSpacing(BufferString value, Format.FormatConverter formatConverter, Format.PropertyId groupPropertyId, Format.Property[] outputProperties, out int parsedPropertiesCount)
    {
      BufferString bufferString1 = BufferString.Null;
      HtmlSupport.ScanSkipWhitespace(ref value);
      int index = 0;
      if (!value.IsEmpty)
      {
        BufferString bufferString2 = HtmlSupport.ScanNextSize(ref value);
        outputProperties[index].Value = HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength(bufferString2, formatConverter);
        if (!outputProperties[index].Value.IsNull)
        {
          ++index;
          HtmlSupport.ScanSkipWhitespace(ref value);
          if (!value.IsEmpty)
          {
            BufferString bufferString3 = HtmlSupport.ScanNextSize(ref value);
            outputProperties[index].Value = HtmlConverterData.PropertyValueParsingMethods.ParseNonNegativeLength(bufferString3, formatConverter);
            if (!outputProperties[index].Value.IsNull)
              ++index;
          }
        }
      }
      if (index == 1)
      {
        outputProperties[0].Id = groupPropertyId;
        outputProperties[1].Set(groupPropertyId + (byte) 1, outputProperties[0].Value);
        parsedPropertiesCount = 2;
      }
      else if (index == 2)
      {
        outputProperties[0].Id = groupPropertyId;
        outputProperties[1].Id = groupPropertyId + (byte) 1;
        parsedPropertiesCount = 2;
      }
      else
        parsedPropertiesCount = 0;
    }

    internal static void ParseCompositeBorder(BufferString value, Format.FormatConverter formatConverter, Format.PropertyId groupPropertyId, Format.Property[] outputProperties, out int parsedPropertiesCount)
    {
      BufferString bufferString1 = BufferString.Null;
      HtmlSupport.ScanSkipWhitespace(ref value);
      int num = 0;
      int index1 = -1;
      int index2 = -1;
      int index3 = -1;
      while (!value.IsEmpty)
      {
        BufferString bufferString2 = HtmlSupport.ScanNextParenthesizedToken(ref value);
        Format.PropertyValue propertyValue = HtmlSupport.ParseBorderWidth(bufferString2, formatConverter);
        if (propertyValue.IsNull)
        {
          propertyValue = HtmlSupport.ParseBorderStyle(bufferString2, formatConverter);
          if (propertyValue.IsNull)
          {
            propertyValue = HtmlSupport.ParseColorCss(bufferString2, formatConverter);
            if (!propertyValue.IsNull)
            {
              if (index3 == -1)
              {
                index3 = num;
                outputProperties[num++].Set(groupPropertyId + (byte) 8, propertyValue);
              }
              else
                outputProperties[index3].Set(groupPropertyId + (byte) 8, propertyValue);
            }
            else
              break;
          }
          else if (index2 == -1)
          {
            index2 = num;
            outputProperties[num++].Set(groupPropertyId + (byte) 4, propertyValue);
          }
          else
            outputProperties[index2].Set(groupPropertyId + (byte) 4, propertyValue);
        }
        else if (index1 == -1)
        {
          index1 = num;
          outputProperties[num++].Set(groupPropertyId, propertyValue);
        }
        else
          outputProperties[index1].Set(groupPropertyId, propertyValue);
        HtmlSupport.ScanSkipWhitespace(ref value);
      }
      parsedPropertiesCount = num;
    }

    internal static void ParseCompositeAllBorders(BufferString value, Format.FormatConverter formatConverter, Format.PropertyId groupPropertyId, Format.Property[] outputProperties, out int parsedPropertiesCount)
    {
      HtmlSupport.ParseCompositeBorder(value, formatConverter, Format.PropertyId.BorderWidths, outputProperties, out parsedPropertiesCount);
      for (int index1 = 0; index1 < parsedPropertiesCount; ++index1)
      {
        for (int index2 = 1; index2 < 4; ++index2)
          outputProperties[parsedPropertiesCount * index2 + index1].Set(outputProperties[index1].Id + (byte) index2, outputProperties[index1].Value);
      }
      parsedPropertiesCount += parsedPropertiesCount * 3;
    }

    internal static void ParseCompositeBackground(BufferString value, Format.FormatConverter formatConverter, Format.PropertyId groupPropertyId, Format.Property[] outputProperties, out int parsedPropertiesCount)
    {
      BufferString bufferString1 = BufferString.Null;
      HtmlSupport.ScanSkipWhitespace(ref value);
      int index = 0;
      if (!value.IsEmpty)
      {
        BufferString bufferString2 = HtmlSupport.ScanNextNonWhitespaceToken(ref value);
        outputProperties[index].Set(Format.PropertyId.BackColor, HtmlSupport.ParseColorCss(bufferString2, formatConverter));
        if (!outputProperties[index].Value.IsNull)
          ++index;
        HtmlSupport.ScanSkipWhitespace(ref value);
      }
      parsedPropertiesCount = index;
    }

    internal static void ParseCssTextDecoration(BufferString value, Format.FormatConverter formatConverter, Format.PropertyId groupPropertyId, Format.Property[] outputProperties, out int parsedPropertiesCount)
    {
      BufferString bufferString = BufferString.Null;
      HtmlSupport.ScanSkipWhitespace(ref value);
      int num1 = 0;
      Format.PropertyValue propertyValue = HtmlSupport.ParseEnum(HtmlSupport.ScanNextNonWhitespaceToken(ref value), HtmlSupport.cssTextDecorationEnumeration);
      if (!propertyValue.IsNull)
      {
        switch (propertyValue.Enum)
        {
          case 0:
          case 2:
          case 4:
            Format.Property[] propertyArray1 = outputProperties;
            int index1 = num1;
            int num2 = 1;
            int num3 = index1 + num2;
            propertyArray1[index1].Set(Format.PropertyId.Underline, Format.PropertyValue.False);
            Format.Property[] propertyArray2 = outputProperties;
            int index2 = num3;
            int num4 = 1;
            num1 = index2 + num4;
            propertyArray2[index2].Set(Format.PropertyId.Strikethrough, Format.PropertyValue.False);
            break;
          case 1:
            Format.Property[] propertyArray3 = outputProperties;
            int index3 = num1;
            int num5 = 1;
            int num6 = index3 + num5;
            propertyArray3[index3].Set(Format.PropertyId.Underline, Format.PropertyValue.True);
            Format.Property[] propertyArray4 = outputProperties;
            int index4 = num6;
            int num7 = 1;
            num1 = index4 + num7;
            propertyArray4[index4].Set(Format.PropertyId.Strikethrough, Format.PropertyValue.False);
            break;
          case 3:
            Format.Property[] propertyArray5 = outputProperties;
            int index5 = num1;
            int num8 = 1;
            int num9 = index5 + num8;
            propertyArray5[index5].Set(Format.PropertyId.Underline, Format.PropertyValue.False);
            Format.Property[] propertyArray6 = outputProperties;
            int index6 = num9;
            int num10 = 1;
            num1 = index6 + num10;
            propertyArray6[index6].Set(Format.PropertyId.Strikethrough, Format.PropertyValue.True);
            break;
        }
      }
      parsedPropertiesCount = num1;
    }

    internal static void ParseCssTextTransform(BufferString value, Format.FormatConverter formatConverter, Format.PropertyId groupPropertyId, Format.Property[] outputProperties, out int parsedPropertiesCount)
    {
      BufferString bufferString = BufferString.Null;
      HtmlSupport.ScanSkipWhitespace(ref value);
      int num = 0;
      Format.PropertyValue propertyValue = HtmlSupport.ParseEnum(HtmlSupport.ScanNextNonWhitespaceToken(ref value), HtmlSupport.cssTextTransformEnumeration);
      if (!propertyValue.IsNull)
      {
        switch (propertyValue.Enum)
        {
          case 0:
            outputProperties[num++].Set(Format.PropertyId.Capitalize, Format.PropertyValue.True);
            break;
          case 1:
          case 2:
          case 3:
            outputProperties[num++].Set(Format.PropertyId.Capitalize, Format.PropertyValue.False);
            break;
        }
      }
      parsedPropertiesCount = num;
    }

    internal static void ParseCssVerticalAlignment(BufferString value, Format.FormatConverter formatConverter, Format.PropertyId groupPropertyId, Format.Property[] outputProperties, out int parsedPropertiesCount)
    {
      BufferString bufferString = BufferString.Null;
      HtmlSupport.ScanSkipWhitespace(ref value);
      int num1 = 0;
      Format.PropertyValue propertyValue = HtmlSupport.ParseEnum(HtmlSupport.ScanNextNonWhitespaceToken(ref value), HtmlSupport.cssVerticalAlignmentEnumeration);
      if (!propertyValue.IsNull)
      {
        switch (propertyValue.Enum)
        {
          case 0:
          case 1:
          case 2:
          case 5:
            outputProperties[num1++].Set(Format.PropertyId.BlockAlignment, propertyValue);
            break;
          case 7:
            Format.Property[] propertyArray1 = outputProperties;
            int index1 = num1;
            int num2 = 1;
            int num3 = index1 + num2;
            propertyArray1[index1].Set(Format.PropertyId.Subscript, Format.PropertyValue.True);
            Format.Property[] propertyArray2 = outputProperties;
            int index2 = num3;
            int num4 = 1;
            num1 = index2 + num4;
            propertyArray2[index2].Set(Format.PropertyId.Superscript, Format.PropertyValue.False);
            break;
          case 8:
            Format.Property[] propertyArray3 = outputProperties;
            int index3 = num1;
            int num5 = 1;
            int num6 = index3 + num5;
            propertyArray3[index3].Set(Format.PropertyId.Superscript, Format.PropertyValue.True);
            Format.Property[] propertyArray4 = outputProperties;
            int index4 = num6;
            int num7 = 1;
            num1 = index4 + num7;
            propertyArray4[index4].Set(Format.PropertyId.Subscript, Format.PropertyValue.False);
            break;
        }
      }
      parsedPropertiesCount = num1;
    }

    internal static void ParseCssWhiteSpace(BufferString value, Format.FormatConverter formatConverter, Format.PropertyId groupPropertyId, Format.Property[] outputProperties, out int parsedPropertiesCount)
    {
      BufferString bufferString = BufferString.Null;
      HtmlSupport.ScanSkipWhitespace(ref value);
      int num = 0;
      Format.PropertyValue propertyValue = HtmlSupport.ParseEnum(HtmlSupport.ScanNextNonWhitespaceToken(ref value), HtmlSupport.cssWhiteSpaceEnumeration);
      if (!propertyValue.IsNull)
      {
        switch (propertyValue.Enum)
        {
          case 1:
          case 2:
          case 3:
          case 4:
            outputProperties[num++].Set(Format.PropertyId.Preformatted, Format.PropertyValue.True);
            break;
        }
      }
      parsedPropertiesCount = num;
    }

    internal static void ParseCompositeFont(BufferString value, Format.FormatConverter formatConverter, Format.PropertyId groupPropertyId, Format.Property[] outputProperties, out int parsedPropertiesCount)
    {
      BufferString token = BufferString.Null;
      HtmlSupport.ScanSkipWhitespace(ref value);
      int index1 = 0;
      int index2 = -1;
      int index3 = -1;
      int index4 = -1;
      while (!value.IsEmpty)
      {
        token = HtmlSupport.ScanNextNonWhitespaceToken(ref value);
        Format.PropertyValue propertyValue = HtmlSupport.ParseFontWeight(token, formatConverter);
        if (propertyValue.IsNull)
        {
          propertyValue = HtmlSupport.ParseFontStyle(token, formatConverter);
          if (propertyValue.IsNull)
          {
            propertyValue = HtmlSupport.ParseFontVariant(token, formatConverter);
            if (!propertyValue.IsNull)
            {
              if (index3 == -1)
              {
                index3 = index1;
                outputProperties[index1++].Set(Format.PropertyId.SmallCaps, propertyValue);
              }
              else
                outputProperties[index3].Set(Format.PropertyId.SmallCaps, propertyValue);
            }
            else
              break;
          }
          else if (index4 == -1)
          {
            index4 = index1;
            outputProperties[index1++].Set(Format.PropertyId.Italic, propertyValue);
          }
          else
            outputProperties[index4].Set(Format.PropertyId.Italic, propertyValue);
        }
        else if (index2 == -1)
        {
          index2 = index1;
          outputProperties[index1++].Set(Format.PropertyId.FirstFlag, propertyValue);
        }
        else
          outputProperties[index2].Set(Format.PropertyId.FirstFlag, propertyValue);
        token = BufferString.Null;
        HtmlSupport.ScanSkipWhitespace(ref value);
      }
      if (!token.IsEmpty)
      {
        HtmlSupport.ScanRevertLastToken(ref value, token);
        BufferString bufferString = HtmlSupport.ScanNextSize(ref value);
        outputProperties[index1].Set(Format.PropertyId.FontSize, HtmlSupport.ParseCssFontSize(bufferString, formatConverter));
        if (!outputProperties[index1].Value.IsNull)
          ++index1;
      }
      HtmlSupport.ScanSkipWhitespace(ref value);
      if (!value.IsEmpty && (int) value[0] == 47)
      {
        value.Trim(1, value.Length - 1);
        HtmlSupport.ScanSkipWhitespace(ref value);
        HtmlSupport.ParseNonNegativeLength(HtmlSupport.ScanNextSize(ref value), formatConverter);
        HtmlSupport.ScanSkipWhitespace(ref value);
      }
      if (!value.IsEmpty)
      {
        outputProperties[index1].Set(Format.PropertyId.FontSize, HtmlSupport.ParseFontFace(value, formatConverter));
        if (!outputProperties[index1].Value.IsNull)
          ++index1;
      }
      parsedPropertiesCount = index1;
    }

    [Flags]
    public enum NumberParseFlags
    {
      Integer = 1,
      Float = 2,
      AbsoluteLength = 4,
      EmExLength = 8,
      Percentage = 16,
      Multiple = 32,
      HtmlFontUnits = 64,
      NonNegative = 8192,
      StyleSheetProperty = 16384,
      Strict = 32768,
      Length = Percentage | EmExLength | AbsoluteLength,
      NonNegativeLength = Length | NonNegative,
      FontSize = NonNegativeLength | HtmlFontUnits,
    }

    public struct EnumerationDef
    {
      public string Name;
      public Format.PropertyValue Value;

      public EnumerationDef(string name, Format.PropertyValue value)
      {
        this.Name = name;
        this.Value = value;
      }
    }

    internal enum TableFrame
    {
      Void,
      Above,
      Below,
      Border,
      Box,
      Hsides,
      Lhs,
      Rhs,
      Vsides,
    }

    internal enum TableRules
    {
      None,
      Groups,
      Rows,
      Cells,
      All,
    }

    internal enum SystemColors
    {
      Scrollbar = 0,
      Background = 1,
      ActiveCaption = 2,
      InactiveCaption = 3,
      Menu = 4,
      Window = 5,
      WindowFrame = 6,
      MenuText = 7,
      WindowText = 8,
      CaptionText = 9,
      ActiveBorder = 10,
      InactiveBorder = 11,
      AppWorkspace = 12,
      Highlight = 13,
      HighlightText = 14,
      ButtonFace = 15,
      ThreeDFace = 15,
      ButtonShadow = 16,
      ThreeDShadow = 16,
      GrayText = 17,
      ButtonText = 18,
      InactiveCaptionText = 19,
      ButtonHighlight = 20,
      ThreeDHighlight = 20,
      ThreeDDarkShadow = 21,
      ThreeDLightShadow = 22,
      InfoText = 23,
      InfoBackground = 24,
    }

    private enum TextDecoration
    {
      None,
      Underline,
      Overline,
      LineThrough,
      Blink,
    }

    private enum TextTransform
    {
      Capitalize,
      Uppercase,
      Lowercase,
      None,
    }

    private enum CssWhiteSpace
    {
      Normal,
      Pre,
      Nowrap,
      PreWrap,
      PreLine,
    }
  }
}
