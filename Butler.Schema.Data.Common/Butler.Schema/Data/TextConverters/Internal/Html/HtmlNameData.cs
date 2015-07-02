// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Html.HtmlNameData
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Html
{
  internal static class HtmlNameData
  {
    public const short MAX_NAME = (short) 14;
    public const short MAX_TAG_NAME = (short) 14;
    public const short MAX_ENTITY_NAME = (short) 8;
    public const short NAME_HASH_SIZE = (short) 601;
    public const int NAME_HASH_MODIFIER = 221;
    public const short ENTITY_HASH_SIZE = (short) 705;
    public const int ENTITY_HASH_MODIFIER = 230;
    public static HtmlNameIndex[] nameHashTable;
    public static HtmlEntityIndex[] entityHashTable;
    public static HtmlNameData.NameDef[] Names;
    public static HtmlNameIndex[] TagIndex;
    public static HtmlNameIndex[] attributeIndex;
    public static HtmlNameData.EntityDef[] entities;

    static HtmlNameData()
    {
      HtmlNameIndex[] htmlNameIndexArray = new HtmlNameIndex[601];
      htmlNameIndexArray[1] = HtmlNameIndex.Nofill;
      htmlNameIndexArray[2] = HtmlNameIndex.Comment;
      htmlNameIndexArray[3] = HtmlNameIndex.LI;
      htmlNameIndexArray[4] = HtmlNameIndex.Version;
      htmlNameIndexArray[8] = HtmlNameIndex.CellSpacing;
      htmlNameIndexArray[10] = HtmlNameIndex.Kbd;
      htmlNameIndexArray[14] = HtmlNameIndex.Scheme;
      htmlNameIndexArray[15] = HtmlNameIndex.Multiple;
      htmlNameIndexArray[21] = HtmlNameIndex.Ruby;
      htmlNameIndexArray[23] = HtmlNameIndex.Code;
      htmlNameIndexArray[24] = HtmlNameIndex.NoResize;
      htmlNameIndexArray[25] = HtmlNameIndex.Alt;
      htmlNameIndexArray[27] = HtmlNameIndex.HrefLang;
      htmlNameIndexArray[30] = HtmlNameIndex.FlushRight;
      htmlNameIndexArray[38] = HtmlNameIndex.Accept;
      htmlNameIndexArray[45] = HtmlNameIndex.FrameBorder;
      htmlNameIndexArray[52] = HtmlNameIndex.Shape;
      htmlNameIndexArray[55] = HtmlNameIndex.Param;
      htmlNameIndexArray[56] = HtmlNameIndex.Acronym;
      htmlNameIndexArray[58] = HtmlNameIndex.For;
      htmlNameIndexArray[59] = HtmlNameIndex.Color;
      htmlNameIndexArray[63] = HtmlNameIndex.A;
      htmlNameIndexArray[66] = HtmlNameIndex._Pxml;
      htmlNameIndexArray[68] = HtmlNameIndex.Face;
      htmlNameIndexArray[72] = HtmlNameIndex.RowSpan;
      htmlNameIndexArray[73] = HtmlNameIndex.NoWrap;
      htmlNameIndexArray[74] = HtmlNameIndex.Ins;
      htmlNameIndexArray[85] = HtmlNameIndex.RP;
      htmlNameIndexArray[86] = HtmlNameIndex.Script;
      htmlNameIndexArray[88] = HtmlNameIndex.Char;
      htmlNameIndexArray[100] = HtmlNameIndex.BGColor;
      htmlNameIndexArray[123] = HtmlNameIndex.Style;
      htmlNameIndexArray[126] = HtmlNameIndex.Width;
      htmlNameIndexArray[128] = HtmlNameIndex.Headers;
      htmlNameIndexArray[130] = HtmlNameIndex.Map;
      htmlNameIndexArray[132] = HtmlNameIndex.Data;
      htmlNameIndexArray[135] = HtmlNameIndex.Sub;
      htmlNameIndexArray[136] = HtmlNameIndex.H2;
      htmlNameIndexArray[137] = HtmlNameIndex.Image;
      htmlNameIndexArray[141] = HtmlNameIndex.StandBy;
      htmlNameIndexArray[143] = HtmlNameIndex.Select;
      htmlNameIndexArray[145] = HtmlNameIndex.Profile;
      htmlNameIndexArray[155] = HtmlNameIndex.Button;
      htmlNameIndexArray[162] = HtmlNameIndex.Meta;
      htmlNameIndexArray[166] = HtmlNameIndex.Rules;
      htmlNameIndexArray[167] = HtmlNameIndex.Class;
      htmlNameIndexArray[170] = HtmlNameIndex.Src;
      htmlNameIndexArray[171] = HtmlNameIndex.Legend;
      htmlNameIndexArray[177] = HtmlNameIndex.Scrolling;
      htmlNameIndexArray[180] = HtmlNameIndex.Vlink;
      htmlNameIndexArray[181] = HtmlNameIndex.Del;
      htmlNameIndexArray[187] = HtmlNameIndex.Hspace;
      htmlNameIndexArray[190] = HtmlNameIndex.Charset;
      htmlNameIndexArray[196] = HtmlNameIndex.RT;
      htmlNameIndexArray[198] = HtmlNameIndex.Italic;
      htmlNameIndexArray[201] = HtmlNameIndex.Div;
      htmlNameIndexArray[205] = HtmlNameIndex.Dir;
      htmlNameIndexArray[206] = HtmlNameIndex.TT;
      htmlNameIndexArray[208] = HtmlNameIndex.H6;
      htmlNameIndexArray[209] = HtmlNameIndex.ValueType;
      htmlNameIndexArray[210] = HtmlNameIndex.Declare;
      htmlNameIndexArray[212] = HtmlNameIndex.Size;
      htmlNameIndexArray[214] = HtmlNameIndex.FrameSet;
      htmlNameIndexArray[216] = HtmlNameIndex.ReadOnly;
      htmlNameIndexArray[218] = HtmlNameIndex.Language;
      htmlNameIndexArray[220] = HtmlNameIndex.BlockQuote;
      htmlNameIndexArray[222] = HtmlNameIndex.TopMargin;
      htmlNameIndexArray[223] = HtmlNameIndex.NoEmbed;
      htmlNameIndexArray[224] = HtmlNameIndex.BaseFont;
      htmlNameIndexArray[229] = HtmlNameIndex.NoFrames;
      htmlNameIndexArray[232] = HtmlNameIndex.Border;
      htmlNameIndexArray[233] = HtmlNameIndex.Center;
      htmlNameIndexArray[237] = HtmlNameIndex.Height;
      htmlNameIndexArray[240] = HtmlNameIndex.Underline;
      htmlNameIndexArray[242] = HtmlNameIndex.FlushBoth;
      htmlNameIndexArray[244] = HtmlNameIndex.BGSound;
      htmlNameIndexArray[249] = HtmlNameIndex.Var;
      htmlNameIndexArray[252] = HtmlNameIndex.TD;
      htmlNameIndexArray[253] = HtmlNameIndex.Id;
      htmlNameIndexArray[254] = HtmlNameIndex.Rows;
      htmlNameIndexArray[(int) byte.MaxValue] = HtmlNameIndex.H4;
      htmlNameIndexArray[257] = HtmlNameIndex.Abbr;
      htmlNameIndexArray[258] = HtmlNameIndex.HttpEquiv;
      htmlNameIndexArray[259] = HtmlNameIndex.Span;
      htmlNameIndexArray[268] = HtmlNameIndex.ItemProp;
      htmlNameIndexArray[271] = HtmlNameIndex.Address;
      htmlNameIndexArray[273] = HtmlNameIndex.Applet;
      htmlNameIndexArray[274] = HtmlNameIndex.Rel;
      htmlNameIndexArray[278] = HtmlNameIndex.TextArea;
      htmlNameIndexArray[279] = HtmlNameIndex.Tbody;
      htmlNameIndexArray[283] = HtmlNameIndex.ParaIndent;
      htmlNameIndexArray[286] = HtmlNameIndex.DT;
      htmlNameIndexArray[292] = HtmlNameIndex.NextId;
      htmlNameIndexArray[293] = HtmlNameIndex.Head;
      htmlNameIndexArray[296] = HtmlNameIndex.Rev;
      htmlNameIndexArray[297] = HtmlNameIndex.Small;
      htmlNameIndexArray[299] = HtmlNameIndex.Cite;
      htmlNameIndexArray[305] = HtmlNameIndex.Cols;
      htmlNameIndexArray[306] = HtmlNameIndex.Sup;
      htmlNameIndexArray[314] = HtmlNameIndex.Fixed;
      htmlNameIndexArray[315] = HtmlNameIndex.Prompt;
      htmlNameIndexArray[320] = HtmlNameIndex.Disabled;
      htmlNameIndexArray[322] = HtmlNameIndex.Coords;
      htmlNameIndexArray[323] = HtmlNameIndex.Summary;
      htmlNameIndexArray[324] = HtmlNameIndex.Object;
      htmlNameIndexArray[331] = HtmlNameIndex.Label;
      htmlNameIndexArray[332] = HtmlNameIndex.Content;
      htmlNameIndexArray[333] = HtmlNameIndex.Target;
      htmlNameIndexArray[340] = HtmlNameIndex.EM;
      htmlNameIndexArray[344] = HtmlNameIndex.Clear;
      htmlNameIndexArray[354] = HtmlNameIndex.Scope;
      htmlNameIndexArray[356] = HtmlNameIndex.Compact;
      htmlNameIndexArray[358] = HtmlNameIndex.Blink;
      htmlNameIndexArray[372] = HtmlNameIndex.Selected;
      htmlNameIndexArray[374] = HtmlNameIndex.MaxLength;
      htmlNameIndexArray[380] = HtmlNameIndex.Frame;
      htmlNameIndexArray[381] = HtmlNameIndex.Thead;
      htmlNameIndexArray[383] = HtmlNameIndex.ItemScope;
      htmlNameIndexArray[389] = HtmlNameIndex.TabIndex;
      htmlNameIndexArray[395] = HtmlNameIndex.Marquee;
      htmlNameIndexArray[405] = HtmlNameIndex.Embed;
      htmlNameIndexArray[406] = HtmlNameIndex.TH;
      htmlNameIndexArray[407] = HtmlNameIndex.Caption;
      htmlNameIndexArray[413] = HtmlNameIndex.Value;
      htmlNameIndexArray[420] = HtmlNameIndex.Smaller;
      htmlNameIndexArray[424] = HtmlNameIndex.DateTime;
      htmlNameIndexArray[426] = HtmlNameIndex.ClassId;
      htmlNameIndexArray[432] = HtmlNameIndex.Bold;
      htmlNameIndexArray[433] = HtmlNameIndex.Strike;
      htmlNameIndexArray[447] = HtmlNameIndex.FlushLeft;
      htmlNameIndexArray[448] = HtmlNameIndex.NoShade;
      htmlNameIndexArray[449] = HtmlNameIndex.LeftMargin;
      htmlNameIndexArray[450] = HtmlNameIndex.Title;
      htmlNameIndexArray[452] = HtmlNameIndex.Excerpt;
      htmlNameIndexArray[454] = HtmlNameIndex.CellPadding;
      htmlNameIndexArray[458] = HtmlNameIndex.Dfn;
      htmlNameIndexArray[459] = HtmlNameIndex.CharOff;
      htmlNameIndexArray[461] = HtmlNameIndex.IsIndex;
      htmlNameIndexArray[462] = HtmlNameIndex.Tfoot;
      htmlNameIndexArray[464] = HtmlNameIndex.NoBR;
      htmlNameIndexArray[470] = HtmlNameIndex.Lang;
      htmlNameIndexArray[472] = HtmlNameIndex.OptGroup;
      htmlNameIndexArray[474] = HtmlNameIndex.Option;
      htmlNameIndexArray[476] = HtmlNameIndex.Big;
      htmlNameIndexArray[477] = HtmlNameIndex.Font;
      htmlNameIndexArray[479] = HtmlNameIndex.Type;
      htmlNameIndexArray[482] = HtmlNameIndex.Href;
      htmlNameIndexArray[484] = HtmlNameIndex.Img;
      htmlNameIndexArray[486] = HtmlNameIndex.Vspace;
      htmlNameIndexArray[487] = HtmlNameIndex.H3;
      htmlNameIndexArray[488] = HtmlNameIndex.Meter;
      htmlNameIndexArray[493] = HtmlNameIndex.Align;
      htmlNameIndexArray[497] = HtmlNameIndex.Wbr;
      htmlNameIndexArray[499] = HtmlNameIndex.AccessKey;
      htmlNameIndexArray[502] = HtmlNameIndex.Col;
      htmlNameIndexArray[504] = HtmlNameIndex.Bigger;
      htmlNameIndexArray[506] = HtmlNameIndex.CodeBase;
      htmlNameIndexArray[508] = HtmlNameIndex.Strong;
      htmlNameIndexArray[510] = HtmlNameIndex.BR;
      htmlNameIndexArray[512] = HtmlNameIndex.Archive;
      htmlNameIndexArray[514] = HtmlNameIndex.UL;
      htmlNameIndexArray[516] = HtmlNameIndex.NoScript;
      htmlNameIndexArray[517] = HtmlNameIndex.PlainText;
      htmlNameIndexArray[521] = HtmlNameIndex.Base;
      htmlNameIndexArray[522] = HtmlNameIndex.Defer;
      htmlNameIndexArray[523] = HtmlNameIndex.Body;
      htmlNameIndexArray[524] = HtmlNameIndex.OL;
      htmlNameIndexArray[526] = HtmlNameIndex.H1;
      htmlNameIndexArray[528] = HtmlNameIndex.Valign;
      htmlNameIndexArray[531] = HtmlNameIndex.Media;
      htmlNameIndexArray[532] = HtmlNameIndex.Iframe;
      htmlNameIndexArray[533] = HtmlNameIndex.DL;
      htmlNameIndexArray[535] = HtmlNameIndex.ColSpan;
      htmlNameIndexArray[538] = HtmlNameIndex.Axis;
      htmlNameIndexArray[542] = HtmlNameIndex.MarginHeight;
      htmlNameIndexArray[543] = HtmlNameIndex.Alink;
      htmlNameIndexArray[545] = HtmlNameIndex._Xml_Namespace;
      htmlNameIndexArray[546] = HtmlNameIndex.Method;
      htmlNameIndexArray[549] = HtmlNameIndex.FontFamily;
      htmlNameIndexArray[554] = HtmlNameIndex.FieldSet;
      htmlNameIndexArray[556] = HtmlNameIndex.Pre;
      htmlNameIndexArray[557] = HtmlNameIndex.Table;
      htmlNameIndexArray[560] = HtmlNameIndex.TR;
      htmlNameIndexArray[562] = HtmlNameIndex.Samp;
      htmlNameIndexArray[563] = HtmlNameIndex.Link;
      htmlNameIndexArray[564] = HtmlNameIndex.HR;
      htmlNameIndexArray[566] = HtmlNameIndex.ItemRef;
      htmlNameIndexArray[568] = HtmlNameIndex.Form;
      htmlNameIndexArray[569] = HtmlNameIndex.Input;
      htmlNameIndexArray[570] = HtmlNameIndex.Xml;
      htmlNameIndexArray[572] = HtmlNameIndex.UseMap;
      htmlNameIndexArray[574] = HtmlNameIndex.Xmp;
      htmlNameIndexArray[575] = HtmlNameIndex.CodeType;
      htmlNameIndexArray[580] = HtmlNameIndex.MarginWidth;
      htmlNameIndexArray[584] = HtmlNameIndex.Q;
      htmlNameIndexArray[585] = HtmlNameIndex.DynSrc;
      htmlNameIndexArray[586] = HtmlNameIndex.S;
      htmlNameIndexArray[587] = HtmlNameIndex.P;
      htmlNameIndexArray[588] = HtmlNameIndex.U;
      htmlNameIndexArray[589] = HtmlNameIndex.Action;
      htmlNameIndexArray[590] = HtmlNameIndex.EncType;
      htmlNameIndexArray[591] = HtmlNameIndex.ItemId;
      htmlNameIndexArray[592] = HtmlNameIndex.I;
      htmlNameIndexArray[597] = HtmlNameIndex.B;
      htmlNameIndexArray[598] = HtmlNameIndex.H5;
      htmlNameIndexArray[599] = HtmlNameIndex.Background;
      HtmlNameData.nameHashTable = htmlNameIndexArray;
      HtmlEntityIndex[] htmlEntityIndexArray = new HtmlEntityIndex[705];
      htmlEntityIndexArray[1] = HtmlEntityIndex.omega;
      htmlEntityIndexArray[2] = HtmlEntityIndex.rle;
      htmlEntityIndexArray[5] = HtmlEntityIndex.Oacute;
      htmlEntityIndexArray[8] = HtmlEntityIndex.fnof;
      htmlEntityIndexArray[9] = HtmlEntityIndex.Oslash;
      htmlEntityIndexArray[12] = HtmlEntityIndex.Ntilde;
      htmlEntityIndexArray[14] = HtmlEntityIndex.larr;
      htmlEntityIndexArray[15] = HtmlEntityIndex.psi;
      htmlEntityIndexArray[20] = HtmlEntityIndex.Pi;
      htmlEntityIndexArray[22] = HtmlEntityIndex.micro;
      htmlEntityIndexArray[24] = HtmlEntityIndex.piv;
      htmlEntityIndexArray[26] = HtmlEntityIndex.upsih;
      htmlEntityIndexArray[28] = HtmlEntityIndex.Xi;
      htmlEntityIndexArray[29] = HtmlEntityIndex.aring;
      htmlEntityIndexArray[30] = HtmlEntityIndex.ni;
      htmlEntityIndexArray[32] = HtmlEntityIndex.cap;
      htmlEntityIndexArray[33] = HtmlEntityIndex.iuml;
      htmlEntityIndexArray[34] = HtmlEntityIndex.chi;
      htmlEntityIndexArray[38] = HtmlEntityIndex.frac14;
      htmlEntityIndexArray[40] = HtmlEntityIndex.frac34;
      htmlEntityIndexArray[41] = HtmlEntityIndex.ordm;
      htmlEntityIndexArray[44] = HtmlEntityIndex.and;
      htmlEntityIndexArray[47] = HtmlEntityIndex.brvbar;
      htmlEntityIndexArray[49] = HtmlEntityIndex.zwsp;
      htmlEntityIndexArray[50] = HtmlEntityIndex.forall;
      htmlEntityIndexArray[52] = HtmlEntityIndex.pi;
      htmlEntityIndexArray[53] = HtmlEntityIndex.otimes;
      htmlEntityIndexArray[54] = HtmlEntityIndex.uacute;
      htmlEntityIndexArray[55] = HtmlEntityIndex.ang;
      htmlEntityIndexArray[56] = HtmlEntityIndex.iexcl;
      htmlEntityIndexArray[57] = HtmlEntityIndex.lrm;
      htmlEntityIndexArray[60] = HtmlEntityIndex.xi;
      htmlEntityIndexArray[65] = HtmlEntityIndex.lre;
      htmlEntityIndexArray[66] = HtmlEntityIndex.zwj;
      htmlEntityIndexArray[68] = HtmlEntityIndex.Nu;
      htmlEntityIndexArray[69] = HtmlEntityIndex.Mu;
      htmlEntityIndexArray[71] = HtmlEntityIndex.lro;
      htmlEntityIndexArray[73] = HtmlEntityIndex.COPY;
      htmlEntityIndexArray[74] = HtmlEntityIndex.nsub;
      htmlEntityIndexArray[83] = HtmlEntityIndex.thorn;
      htmlEntityIndexArray[85] = HtmlEntityIndex.sum;
      htmlEntityIndexArray[87] = HtmlEntityIndex.rsquo;
      htmlEntityIndexArray[88] = HtmlEntityIndex.middot;
      htmlEntityIndexArray[97] = HtmlEntityIndex.Ecirc;
      htmlEntityIndexArray[98] = HtmlEntityIndex.thinsp;
      htmlEntityIndexArray[100] = HtmlEntityIndex.times;
      htmlEntityIndexArray[101] = HtmlEntityIndex.mu;
      htmlEntityIndexArray[102] = HtmlEntityIndex.yen;
      htmlEntityIndexArray[107] = HtmlEntityIndex.prod;
      htmlEntityIndexArray[116] = HtmlEntityIndex.dArr;
      htmlEntityIndexArray[118] = HtmlEntityIndex.euml;
      htmlEntityIndexArray[122] = HtmlEntityIndex.Beta;
      htmlEntityIndexArray[123] = HtmlEntityIndex.radic;
      htmlEntityIndexArray[130] = HtmlEntityIndex.hearts;
      htmlEntityIndexArray[131] = HtmlEntityIndex.TRADE;
      htmlEntityIndexArray[134] = HtmlEntityIndex.rsaquo;
      htmlEntityIndexArray[136] = HtmlEntityIndex.Auml;
      htmlEntityIndexArray[137] = HtmlEntityIndex.ugrave;
      htmlEntityIndexArray[142] = HtmlEntityIndex.ccedil;
      htmlEntityIndexArray[143] = HtmlEntityIndex.OElig;
      htmlEntityIndexArray[144] = HtmlEntityIndex.sect;
      htmlEntityIndexArray[146] = HtmlEntityIndex.there4;
      htmlEntityIndexArray[148] = HtmlEntityIndex.REG;
      htmlEntityIndexArray[155] = HtmlEntityIndex.plusmn;
      htmlEntityIndexArray[158] = HtmlEntityIndex.thetasym;
      htmlEntityIndexArray[161] = HtmlEntityIndex.rArr;
      htmlEntityIndexArray[162] = HtmlEntityIndex.iota;
      htmlEntityIndexArray[163] = HtmlEntityIndex.rceil;
      htmlEntityIndexArray[164] = HtmlEntityIndex.empty;
      htmlEntityIndexArray[165] = HtmlEntityIndex.Phi;
      htmlEntityIndexArray[166] = HtmlEntityIndex.Gamma;
      htmlEntityIndexArray[167] = HtmlEntityIndex.ass;
      htmlEntityIndexArray[171] = HtmlEntityIndex.lt;
      htmlEntityIndexArray[177] = HtmlEntityIndex.Zeta;
      htmlEntityIndexArray[179] = HtmlEntityIndex.Ograve;
      htmlEntityIndexArray[187] = HtmlEntityIndex.spades;
      htmlEntityIndexArray[193] = HtmlEntityIndex.zwnj;
      htmlEntityIndexArray[196] = HtmlEntityIndex.delta;
      htmlEntityIndexArray[200] = HtmlEntityIndex.reg;
      htmlEntityIndexArray[203] = HtmlEntityIndex.isin;
      htmlEntityIndexArray[204] = HtmlEntityIndex.Alpha;
      htmlEntityIndexArray[207] = HtmlEntityIndex.Yuml;
      htmlEntityIndexArray[211] = HtmlEntityIndex.cedil;
      htmlEntityIndexArray[218] = HtmlEntityIndex.rfloor;
      htmlEntityIndexArray[220] = HtmlEntityIndex.divide;
      htmlEntityIndexArray[222] = HtmlEntityIndex.Omicron;
      htmlEntityIndexArray[225] = HtmlEntityIndex.ordf;
      htmlEntityIndexArray[227] = HtmlEntityIndex.clubs;
      htmlEntityIndexArray[228] = HtmlEntityIndex.Uuml;
      htmlEntityIndexArray[229] = HtmlEntityIndex.Eta;
      htmlEntityIndexArray[231] = HtmlEntityIndex.Acirc;
      htmlEntityIndexArray[232] = HtmlEntityIndex.Atilde;
      htmlEntityIndexArray[233] = HtmlEntityIndex.Rho;
      htmlEntityIndexArray[234] = HtmlEntityIndex.alefsym;
      htmlEntityIndexArray[240] = HtmlEntityIndex.AElig;
      htmlEntityIndexArray[248] = HtmlEntityIndex.hArr;
      htmlEntityIndexArray[250] = HtmlEntityIndex.oline;
      htmlEntityIndexArray[254] = HtmlEntityIndex.Aacute;
      htmlEntityIndexArray[(int) byte.MaxValue] = HtmlEntityIndex.Ccedil;
      htmlEntityIndexArray[256] = HtmlEntityIndex.theta;
      htmlEntityIndexArray[257] = HtmlEntityIndex.or;
      htmlEntityIndexArray[258] = HtmlEntityIndex.Egrave;
      htmlEntityIndexArray[259] = HtmlEntityIndex.trade;
      htmlEntityIndexArray[260] = HtmlEntityIndex.Int;
      htmlEntityIndexArray[262] = HtmlEntityIndex.sup1;
      htmlEntityIndexArray[263] = HtmlEntityIndex.phi;
      htmlEntityIndexArray[265] = HtmlEntityIndex.cup;
      htmlEntityIndexArray[268] = HtmlEntityIndex.equiv;
      htmlEntityIndexArray[272] = HtmlEntityIndex.sim;
      htmlEntityIndexArray[273] = HtmlEntityIndex.Yacute;
      htmlEntityIndexArray[276] = HtmlEntityIndex.Prime;
      htmlEntityIndexArray[277] = HtmlEntityIndex.uarr;
      htmlEntityIndexArray[280] = HtmlEntityIndex.mdash;
      htmlEntityIndexArray[281] = HtmlEntityIndex.acute;
      htmlEntityIndexArray[285] = HtmlEntityIndex.ETH;
      htmlEntityIndexArray[287] = HtmlEntityIndex.eacute;
      htmlEntityIndexArray[290] = HtmlEntityIndex.weierp;
      htmlEntityIndexArray[292] = HtmlEntityIndex.Kappa;
      htmlEntityIndexArray[294] = HtmlEntityIndex.Theta;
      htmlEntityIndexArray[304] = HtmlEntityIndex.pound;
      htmlEntityIndexArray[307] = HtmlEntityIndex.rarr;
      htmlEntityIndexArray[312] = HtmlEntityIndex.oelig;
      htmlEntityIndexArray[313] = HtmlEntityIndex.sup;
      htmlEntityIndexArray[314] = HtmlEntityIndex.igrave;
      htmlEntityIndexArray[316] = HtmlEntityIndex.cent;
      htmlEntityIndexArray[319] = HtmlEntityIndex.agrave;
      htmlEntityIndexArray[321] = HtmlEntityIndex.dagger;
      htmlEntityIndexArray[328] = HtmlEntityIndex.bull;
      htmlEntityIndexArray[330] = HtmlEntityIndex.Ouml;
      htmlEntityIndexArray[335] = HtmlEntityIndex.Aring;
      htmlEntityIndexArray[338] = HtmlEntityIndex.loz;
      htmlEntityIndexArray[345] = HtmlEntityIndex.lowast;
      htmlEntityIndexArray[346] = HtmlEntityIndex.otilde;
      htmlEntityIndexArray[351] = HtmlEntityIndex.euro;
      htmlEntityIndexArray[352] = HtmlEntityIndex.uml;
      htmlEntityIndexArray[354] = HtmlEntityIndex.sigma;
      htmlEntityIndexArray[373] = HtmlEntityIndex.Epsilon;
      htmlEntityIndexArray[376] = HtmlEntityIndex.lsaquo;
      htmlEntityIndexArray[379] = HtmlEntityIndex.image;
      htmlEntityIndexArray[380] = HtmlEntityIndex.lArr;
      htmlEntityIndexArray[384] = HtmlEntityIndex.Iuml;
      htmlEntityIndexArray[385] = HtmlEntityIndex.Chi;
      htmlEntityIndexArray[390] = HtmlEntityIndex.eta;
      htmlEntityIndexArray[394] = HtmlEntityIndex.harr;
      htmlEntityIndexArray[397] = HtmlEntityIndex.aacute;
      htmlEntityIndexArray[399] = HtmlEntityIndex.nads;
      htmlEntityIndexArray[401] = HtmlEntityIndex.eth;
      htmlEntityIndexArray[402] = HtmlEntityIndex.GT;
      htmlEntityIndexArray[404] = HtmlEntityIndex.Sigma;
      htmlEntityIndexArray[408] = HtmlEntityIndex.oslash;
      htmlEntityIndexArray[409] = HtmlEntityIndex.aelig;
      htmlEntityIndexArray[411] = HtmlEntityIndex.notin;
      htmlEntityIndexArray[415] = HtmlEntityIndex.aafs;
      htmlEntityIndexArray[416] = HtmlEntityIndex.yacute;
      htmlEntityIndexArray[427] = HtmlEntityIndex.laquo;
      htmlEntityIndexArray[431] = HtmlEntityIndex.prime;
      htmlEntityIndexArray[432] = HtmlEntityIndex.Agrave;
      htmlEntityIndexArray[433] = HtmlEntityIndex.lambda;
      htmlEntityIndexArray[436] = HtmlEntityIndex.oplus;
      htmlEntityIndexArray[442] = HtmlEntityIndex.real;
      htmlEntityIndexArray[443] = HtmlEntityIndex.Ugrave;
      htmlEntityIndexArray[448] = HtmlEntityIndex.supe;
      htmlEntityIndexArray[450] = HtmlEntityIndex.para;
      htmlEntityIndexArray[455] = HtmlEntityIndex.darr;
      htmlEntityIndexArray[466] = HtmlEntityIndex.sube;
      htmlEntityIndexArray[468] = HtmlEntityIndex.asymp;
      htmlEntityIndexArray[469] = HtmlEntityIndex.Euml;
      htmlEntityIndexArray[470] = HtmlEntityIndex.ocirc;
      htmlEntityIndexArray[471] = HtmlEntityIndex.Lambda;
      htmlEntityIndexArray[476] = HtmlEntityIndex.beta;
      htmlEntityIndexArray[477] = HtmlEntityIndex.sup3;
      htmlEntityIndexArray[486] = HtmlEntityIndex.infin;
      htmlEntityIndexArray[487] = HtmlEntityIndex.Upsilon;
      htmlEntityIndexArray[488] = HtmlEntityIndex.ucirc;
      htmlEntityIndexArray[490] = HtmlEntityIndex.Delta;
      htmlEntityIndexArray[506] = HtmlEntityIndex.Iacute;
      htmlEntityIndexArray[513] = HtmlEntityIndex.Iota;
      htmlEntityIndexArray[516] = HtmlEntityIndex.tilde;
      htmlEntityIndexArray[517] = HtmlEntityIndex.bdquo;
      htmlEntityIndexArray[520] = HtmlEntityIndex.scaron;
      htmlEntityIndexArray[529] = HtmlEntityIndex.Tau;
      htmlEntityIndexArray[531] = HtmlEntityIndex.zeta;
      htmlEntityIndexArray[533] = HtmlEntityIndex.pdf;
      htmlEntityIndexArray[537] = HtmlEntityIndex.perp;
      htmlEntityIndexArray[546] = HtmlEntityIndex.shy;
      htmlEntityIndexArray[547] = HtmlEntityIndex.icirc;
      htmlEntityIndexArray[561] = HtmlEntityIndex.yuml;
      htmlEntityIndexArray[565] = HtmlEntityIndex.gamma;
      htmlEntityIndexArray[567] = HtmlEntityIndex.sigmaf;
      htmlEntityIndexArray[569] = HtmlEntityIndex.rang;
      htmlEntityIndexArray[572] = HtmlEntityIndex.crarr;
      htmlEntityIndexArray[575] = HtmlEntityIndex.raquo;
      htmlEntityIndexArray[577] = HtmlEntityIndex.minus;
      htmlEntityIndexArray[578] = HtmlEntityIndex.ograve;
      htmlEntityIndexArray[582] = HtmlEntityIndex.uuml;
      htmlEntityIndexArray[583] = HtmlEntityIndex.Ocirc;
      htmlEntityIndexArray[585] = HtmlEntityIndex.nods;
      htmlEntityIndexArray[586] = HtmlEntityIndex.ldquo;
      htmlEntityIndexArray[587] = HtmlEntityIndex.rho;
      htmlEntityIndexArray[588] = HtmlEntityIndex.szlig;
      htmlEntityIndexArray[601] = HtmlEntityIndex.Ucirc;
      htmlEntityIndexArray[603] = HtmlEntityIndex.alpha;
      htmlEntityIndexArray[606] = HtmlEntityIndex.quot;
      htmlEntityIndexArray[615] = HtmlEntityIndex.Dagger;
      htmlEntityIndexArray[616] = HtmlEntityIndex.Uacute;
      htmlEntityIndexArray[620] = HtmlEntityIndex.Igrave;
      htmlEntityIndexArray[622] = HtmlEntityIndex.Psi;
      htmlEntityIndexArray[627] = HtmlEntityIndex.tau;
      htmlEntityIndexArray[628] = HtmlEntityIndex.lsquo;
      htmlEntityIndexArray[629] = HtmlEntityIndex.ge;
      htmlEntityIndexArray[631] = HtmlEntityIndex.atilde;
      htmlEntityIndexArray[632] = HtmlEntityIndex.nabla;
      htmlEntityIndexArray[633] = HtmlEntityIndex.Scaron;
      htmlEntityIndexArray[634] = HtmlEntityIndex.cong;
      htmlEntityIndexArray[635] = HtmlEntityIndex.frasl;
      htmlEntityIndexArray[638] = HtmlEntityIndex.ne;
      htmlEntityIndexArray[639] = HtmlEntityIndex.curren;
      htmlEntityIndexArray[640] = HtmlEntityIndex.le;
      htmlEntityIndexArray[643] = HtmlEntityIndex.uArr;
      htmlEntityIndexArray[648] = HtmlEntityIndex.epsilon;
      htmlEntityIndexArray[649] = HtmlEntityIndex.iacute;
      htmlEntityIndexArray[651] = HtmlEntityIndex.rlm;
      htmlEntityIndexArray[652] = HtmlEntityIndex.Otilde;
      htmlEntityIndexArray[653] = HtmlEntityIndex.sdot;
      htmlEntityIndexArray[654] = HtmlEntityIndex.ndash;
      htmlEntityIndexArray[657] = HtmlEntityIndex.egrave;
      htmlEntityIndexArray[660] = HtmlEntityIndex.Icirc;
      htmlEntityIndexArray[662] = HtmlEntityIndex.part;
      htmlEntityIndexArray[663] = HtmlEntityIndex.diams;
      htmlEntityIndexArray[664] = HtmlEntityIndex.copy;
      htmlEntityIndexArray[667] = HtmlEntityIndex.ntilde;
      htmlEntityIndexArray[670] = HtmlEntityIndex.rdquo;
      htmlEntityIndexArray[671] = HtmlEntityIndex.iquest;
      htmlEntityIndexArray[672] = HtmlEntityIndex.sbquo;
      htmlEntityIndexArray[678] = HtmlEntityIndex.not;
      htmlEntityIndexArray[679] = HtmlEntityIndex.iafs;
      htmlEntityIndexArray[684] = HtmlEntityIndex.ouml;
      htmlEntityIndexArray[689] = HtmlEntityIndex.ecirc;
      htmlEntityIndexArray[691] = HtmlEntityIndex.kappa;
      htmlEntityIndexArray[696] = HtmlEntityIndex.ensp;
      htmlEntityIndexArray[699] = HtmlEntityIndex.prop;
      htmlEntityIndexArray[702] = HtmlEntityIndex.omicron;
      htmlEntityIndexArray[703] = HtmlEntityIndex.hellip;
      HtmlNameData.entityHashTable = htmlEntityIndexArray;
      HtmlNameData.Names = new HtmlNameData.NameDef[232]
      {
        new HtmlNameData.NameDef((short) 0, (string) null, HtmlTagId.Unknown, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 0, (string) null, HtmlTagIndex._COMMENT, false, false, HtmlTagId.Unknown, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 0, (string) null, HtmlTagIndex._CONDITIONAL, false, false, HtmlTagId.Unknown, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 0, (string) null, HtmlTagIndex._BANG, false, false, HtmlTagId.Unknown, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 0, (string) null, HtmlTagIndex._ASP, false, false, HtmlTagId.Unknown, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 0, (string) null, HtmlTagIndex._DTD, false, false, HtmlTagId.Unknown, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 0, (string) null, HtmlTagIndex.Unknown, false, false, HtmlTagId.Unknown, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 1, "nofill", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 2, "comment", HtmlTagIndex.Comment, true, true, HtmlTagId.Comment, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 3, "li", HtmlTagIndex.LI, false, false, HtmlTagId.LI, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 4, "version", HtmlTagId.Unknown, HtmlAttributeId.Version),
        new HtmlNameData.NameDef((short) 8, "cellspacing", HtmlTagId.Unknown, HtmlAttributeId.CellSpacing),
        new HtmlNameData.NameDef((short) 10, "kbd", HtmlTagIndex.Kbd, false, false, HtmlTagId.Kbd, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 14, "scheme", HtmlTagId.Unknown, HtmlAttributeId.Scheme),
        new HtmlNameData.NameDef((short) 15, "multiple", HtmlTagId.Unknown, HtmlAttributeId.Multiple),
        new HtmlNameData.NameDef((short) 21, "ruby", HtmlTagIndex.Ruby, false, false, HtmlTagId.Ruby, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 23, "code", HtmlTagIndex.Code, false, false, HtmlTagId.Code, HtmlAttributeId.Code),
        new HtmlNameData.NameDef((short) 24, "noresize", HtmlTagId.Unknown, HtmlAttributeId.NoResize),
        new HtmlNameData.NameDef((short) 25, "alt", HtmlTagId.Unknown, HtmlAttributeId.Alt),
        new HtmlNameData.NameDef((short) 27, "hreflang", HtmlTagId.Unknown, HtmlAttributeId.HrefLang),
        new HtmlNameData.NameDef((short) 30, "flushright", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 38, "accept", HtmlTagId.Unknown, HtmlAttributeId.Accept),
        new HtmlNameData.NameDef((short) 45, "frameborder", HtmlTagId.Unknown, HtmlAttributeId.FrameBorder),
        new HtmlNameData.NameDef((short) 52, "shape", HtmlTagId.Unknown, HtmlAttributeId.Shape),
        new HtmlNameData.NameDef((short) 55, "param", HtmlTagIndex.Param, false, false, HtmlTagId.Param, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 56, "acronym", HtmlTagIndex.Acronym, false, false, HtmlTagId.Acronym, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 56, "bdo", HtmlTagIndex.Bdo, false, false, HtmlTagId.Bdo, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 58, "for", HtmlTagId.Unknown, HtmlAttributeId.For),
        new HtmlNameData.NameDef((short) 58, "text", HtmlTagId.Unknown, HtmlAttributeId.Text),
        new HtmlNameData.NameDef((short) 59, "color", HtmlTagId.Unknown, HtmlAttributeId.Color),
        new HtmlNameData.NameDef((short) 63, "a", HtmlTagIndex.A, false, false, HtmlTagId.A, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 66, "?pxml", HtmlTagIndex._Pxml, false, false, HtmlTagId.Unknown, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 68, "face", HtmlTagId.Unknown, HtmlAttributeId.Face),
        new HtmlNameData.NameDef((short) 72, "rowspan", HtmlTagId.Unknown, HtmlAttributeId.RowSpan),
        new HtmlNameData.NameDef((short) 73, "nowrap", HtmlTagId.Unknown, HtmlAttributeId.NoWrap),
        new HtmlNameData.NameDef((short) 74, "ins", HtmlTagIndex.Ins, false, false, HtmlTagId.Ins, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 85, "rp", HtmlTagIndex.RP, false, false, HtmlTagId.RP, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 86, "script", HtmlTagIndex.Script, true, true, HtmlTagId.Script, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 88, "char", HtmlTagId.Unknown, HtmlAttributeId.Char),
        new HtmlNameData.NameDef((short) 100, "bgcolor", HtmlTagId.Unknown, HtmlAttributeId.BGColor),
        new HtmlNameData.NameDef((short) 123, "style", HtmlTagIndex.Style, true, true, HtmlTagId.Style, HtmlAttributeId.Style),
        new HtmlNameData.NameDef((short) 126, "width", HtmlTagId.Unknown, HtmlAttributeId.Width),
        new HtmlNameData.NameDef((short) 128, "headers", HtmlTagId.Unknown, HtmlAttributeId.Headers),
        new HtmlNameData.NameDef((short) 130, "map", HtmlTagIndex.Map, false, false, HtmlTagId.Map, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 130, "listing", HtmlTagIndex.Listing, false, false, HtmlTagId.Listing, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 132, "data", HtmlTagIndex.Data, false, false, HtmlTagId.Data, HtmlAttributeId.Data),
        new HtmlNameData.NameDef((short) 135, "sub", HtmlTagIndex.Sub, false, false, HtmlTagId.Sub, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 136, "h2", HtmlTagIndex.H2, false, false, HtmlTagId.H2, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 137, "image", HtmlTagIndex.Image, false, false, HtmlTagId.Image, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 141, "standby", HtmlTagId.Unknown, HtmlAttributeId.StandBy),
        new HtmlNameData.NameDef((short) 143, "select", HtmlTagIndex.Select, false, false, HtmlTagId.Select, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 145, "profile", HtmlTagId.Unknown, HtmlAttributeId.Profile),
        new HtmlNameData.NameDef((short) 145, "nohref", HtmlTagId.Unknown, HtmlAttributeId.NoHref),
        new HtmlNameData.NameDef((short) 155, "button", HtmlTagIndex.Button, false, false, HtmlTagId.Button, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 162, "meta", HtmlTagIndex.Meta, false, false, HtmlTagId.Meta, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 166, "rules", HtmlTagId.Unknown, HtmlAttributeId.Rules),
        new HtmlNameData.NameDef((short) 167, "class", HtmlTagId.Unknown, HtmlAttributeId.Class),
        new HtmlNameData.NameDef((short) 170, "src", HtmlTagId.Unknown, HtmlAttributeId.Src),
        new HtmlNameData.NameDef((short) 171, "legend", HtmlTagIndex.Legend, false, false, HtmlTagId.Legend, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 177, "scrolling", HtmlTagId.Unknown, HtmlAttributeId.Scrolling),
        new HtmlNameData.NameDef((short) 180, "vlink", HtmlTagId.Unknown, HtmlAttributeId.Vlink),
        new HtmlNameData.NameDef((short) 181, "del", HtmlTagIndex.Del, false, false, HtmlTagId.Del, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 187, "hspace", HtmlTagId.Unknown, HtmlAttributeId.Hspace),
        new HtmlNameData.NameDef((short) 190, "charset", HtmlTagId.Unknown, HtmlAttributeId.Charset),
        new HtmlNameData.NameDef((short) 196, "rt", HtmlTagIndex.RT, false, false, HtmlTagId.RT, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 198, "italic", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 201, "div", HtmlTagIndex.Div, false, false, HtmlTagId.Div, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 205, "dir", HtmlTagIndex.Dir, false, false, HtmlTagId.Dir, HtmlAttributeId.Dir),
        new HtmlNameData.NameDef((short) 206, "tt", HtmlTagIndex.TT, false, false, HtmlTagId.TT, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 206, "lowsrc", HtmlTagId.Unknown, HtmlAttributeId.LowSrc),
        new HtmlNameData.NameDef((short) 208, "h6", HtmlTagIndex.H6, false, false, HtmlTagId.H6, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 209, "valuetype", HtmlTagId.Unknown, HtmlAttributeId.ValueType),
        new HtmlNameData.NameDef((short) 210, "declare", HtmlTagId.Unknown, HtmlAttributeId.Declare),
        new HtmlNameData.NameDef((short) 212, "size", HtmlTagId.Unknown, HtmlAttributeId.Size),
        new HtmlNameData.NameDef((short) 214, "frameset", HtmlTagIndex.FrameSet, false, false, HtmlTagId.FrameSet, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 216, "readonly", HtmlTagId.Unknown, HtmlAttributeId.ReadOnly),
        new HtmlNameData.NameDef((short) 218, "language", HtmlTagId.Unknown, HtmlAttributeId.Language),
        new HtmlNameData.NameDef((short) 220, "blockquote", HtmlTagIndex.BlockQuote, false, false, HtmlTagId.BlockQuote, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 220, "area", HtmlTagIndex.Area, false, false, HtmlTagId.Area, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 222, "topmargin", HtmlTagId.Unknown, HtmlAttributeId.TopMargin),
        new HtmlNameData.NameDef((short) 223, "noembed", HtmlTagIndex.NoEmbed, false, false, HtmlTagId.NoEmbed, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 224, "basefont", HtmlTagIndex.BaseFont, false, false, HtmlTagId.BaseFont, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 229, "noframes", HtmlTagIndex.NoFrames, false, false, HtmlTagId.NoFrames, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 232, "border", HtmlTagId.Unknown, HtmlAttributeId.Border),
        new HtmlNameData.NameDef((short) 233, "center", HtmlTagIndex.Center, false, false, HtmlTagId.Center, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 237, "height", HtmlTagId.Unknown, HtmlAttributeId.Height),
        new HtmlNameData.NameDef((short) 240, "underline", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 242, "flushboth", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 244, "bgsound", HtmlTagIndex.BGSound, false, false, HtmlTagId.BGSound, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 249, "var", HtmlTagIndex.Var, false, false, HtmlTagId.Var, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 249, "start", HtmlTagId.Unknown, HtmlAttributeId.Start),
        new HtmlNameData.NameDef((short) 252, "td", HtmlTagIndex.TD, false, false, HtmlTagId.TD, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 253, "id", HtmlTagId.Unknown, HtmlAttributeId.Id),
        new HtmlNameData.NameDef((short) 254, "rows", HtmlTagId.Unknown, HtmlAttributeId.Rows),
        new HtmlNameData.NameDef((short) byte.MaxValue, "h4", HtmlTagIndex.H4, false, false, HtmlTagId.H4, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 257, "abbr", HtmlTagIndex.Abbr, false, false, HtmlTagId.Abbr, HtmlAttributeId.Abbr),
        new HtmlNameData.NameDef((short) 258, "http-equiv", HtmlTagId.Unknown, HtmlAttributeId.HttpEquiv),
        new HtmlNameData.NameDef((short) 259, "span", HtmlTagIndex.Span, false, false, HtmlTagId.Span, HtmlAttributeId.Span),
        new HtmlNameData.NameDef((short) 268, "itemprop", HtmlTagId.Unknown, HtmlAttributeId.ItemProp),
        new HtmlNameData.NameDef((short) 268, "dd", HtmlTagIndex.DD, false, false, HtmlTagId.DD, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 271, "address", HtmlTagIndex.Address, false, false, HtmlTagId.Address, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 273, "applet", HtmlTagIndex.Applet, false, false, HtmlTagId.Applet, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 274, "rel", HtmlTagId.Unknown, HtmlAttributeId.Rel),
        new HtmlNameData.NameDef((short) 278, "textarea", HtmlTagIndex.TextArea, true, false, HtmlTagId.TextArea, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 279, "tbody", HtmlTagIndex.Tbody, false, false, HtmlTagId.Tbody, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 283, "paraindent", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 286, "dt", HtmlTagIndex.DT, false, false, HtmlTagId.DT, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 292, "nextid", HtmlTagIndex.NextId, false, false, HtmlTagId.NextId, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 292, "checked", HtmlTagId.Unknown, HtmlAttributeId.Checked),
        new HtmlNameData.NameDef((short) 293, "head", HtmlTagIndex.Head, false, false, HtmlTagId.Head, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 296, "rev", HtmlTagId.Unknown, HtmlAttributeId.Rev),
        new HtmlNameData.NameDef((short) 297, "small", HtmlTagIndex.Small, false, false, HtmlTagId.Small, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 299, "cite", HtmlTagIndex.Cite, false, false, HtmlTagId.Cite, HtmlAttributeId.Cite),
        new HtmlNameData.NameDef((short) 305, "cols", HtmlTagId.Unknown, HtmlAttributeId.Cols),
        new HtmlNameData.NameDef((short) 305, "longdesc", HtmlTagId.Unknown, HtmlAttributeId.LongDesc),
        new HtmlNameData.NameDef((short) 306, "sup", HtmlTagIndex.Sup, false, false, HtmlTagId.Sup, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 314, "fixed", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 315, "prompt", HtmlTagId.Unknown, HtmlAttributeId.Prompt),
        new HtmlNameData.NameDef((short) 320, "disabled", HtmlTagId.Unknown, HtmlAttributeId.Disabled),
        new HtmlNameData.NameDef((short) 322, "coords", HtmlTagId.Unknown, HtmlAttributeId.Coords),
        new HtmlNameData.NameDef((short) 322, "name", HtmlTagId.Unknown, HtmlAttributeId.Name),
        new HtmlNameData.NameDef((short) 323, "summary", HtmlTagId.Unknown, HtmlAttributeId.Summary),
        new HtmlNameData.NameDef((short) 324, "object", HtmlTagIndex.Object, false, false, HtmlTagId.Object, HtmlAttributeId.Object),
        new HtmlNameData.NameDef((short) 331, "label", HtmlTagIndex.Label, false, false, HtmlTagId.Label, HtmlAttributeId.Label),
        new HtmlNameData.NameDef((short) 332, "content", HtmlTagId.Unknown, HtmlAttributeId.Content),
        new HtmlNameData.NameDef((short) 333, "target", HtmlTagId.Unknown, HtmlAttributeId.Target),
        new HtmlNameData.NameDef((short) 340, "em", HtmlTagIndex.EM, false, false, HtmlTagId.EM, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 344, "clear", HtmlTagId.Unknown, HtmlAttributeId.Clear),
        new HtmlNameData.NameDef((short) 354, "scope", HtmlTagId.Unknown, HtmlAttributeId.Scope),
        new HtmlNameData.NameDef((short) 356, "compact", HtmlTagId.Unknown, HtmlAttributeId.Compact),
        new HtmlNameData.NameDef((short) 358, "blink", HtmlTagIndex.Blink, false, false, HtmlTagId.Blink, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 372, "selected", HtmlTagId.Unknown, HtmlAttributeId.Selected),
        new HtmlNameData.NameDef((short) 374, "maxlength", HtmlTagId.Unknown, HtmlAttributeId.MaxLength),
        new HtmlNameData.NameDef((short) 380, "frame", HtmlTagIndex.Frame, false, false, HtmlTagId.Frame, HtmlAttributeId.Frame),
        new HtmlNameData.NameDef((short) 381, "thead", HtmlTagIndex.Thead, false, false, HtmlTagId.Thead, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 383, "itemscope", HtmlTagId.Unknown, HtmlAttributeId.ItemScope),
        new HtmlNameData.NameDef((short) 389, "tabindex", HtmlTagId.Unknown, HtmlAttributeId.TabIndex),
        new HtmlNameData.NameDef((short) 395, "marquee", HtmlTagIndex.Marquee, false, false, HtmlTagId.Marquee, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 395, "?import", HtmlTagIndex._Import, false, false, HtmlTagId.Unknown, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 405, "embed", HtmlTagIndex.Embed, false, false, HtmlTagId.Embed, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 406, "th", HtmlTagIndex.TH, false, false, HtmlTagId.TH, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 407, "caption", HtmlTagIndex.Caption, false, false, HtmlTagId.Caption, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 413, "value", HtmlTagId.Unknown, HtmlAttributeId.Value),
        new HtmlNameData.NameDef((short) 420, "smaller", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 424, "datetime", HtmlTagId.Unknown, HtmlAttributeId.DateTime),
        new HtmlNameData.NameDef((short) 426, "classid", HtmlTagId.Unknown, HtmlAttributeId.ClassId),
        new HtmlNameData.NameDef((short) 432, "bold", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 433, "strike", HtmlTagIndex.Strike, false, false, HtmlTagId.Strike, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 447, "flushleft", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 448, "noshade", HtmlTagId.Unknown, HtmlAttributeId.NoShade),
        new HtmlNameData.NameDef((short) 449, "leftmargin", HtmlTagId.Unknown, HtmlAttributeId.LeftMargin),
        new HtmlNameData.NameDef((short) 450, "title", HtmlTagIndex.Title, true, false, HtmlTagId.Title, HtmlAttributeId.Title),
        new HtmlNameData.NameDef((short) 452, "excerpt", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 454, "cellpadding", HtmlTagId.Unknown, HtmlAttributeId.CellPadding),
        new HtmlNameData.NameDef((short) 458, "dfn", HtmlTagIndex.Dfn, false, false, HtmlTagId.Dfn, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 459, "charoff", HtmlTagId.Unknown, HtmlAttributeId.CharOff),
        new HtmlNameData.NameDef((short) 461, "isindex", HtmlTagIndex.IsIndex, false, false, HtmlTagId.IsIndex, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 462, "tfoot", HtmlTagIndex.Tfoot, false, false, HtmlTagId.Tfoot, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 464, "nobr", HtmlTagIndex.NoBR, false, false, HtmlTagId.NoBR, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 470, "lang", HtmlTagId.Unknown, HtmlAttributeId.Lang),
        new HtmlNameData.NameDef((short) 472, "optgroup", HtmlTagIndex.OptGroup, false, false, HtmlTagId.OptGroup, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 474, "option", HtmlTagIndex.Option, false, false, HtmlTagId.Option, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 474, "accept-charset", HtmlTagId.Unknown, HtmlAttributeId.AcceptCharset),
        new HtmlNameData.NameDef((short) 476, "big", HtmlTagIndex.Big, false, false, HtmlTagId.Big, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 477, "font", HtmlTagIndex.Font, false, false, HtmlTagId.Font, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 479, "type", HtmlTagId.Unknown, HtmlAttributeId.Type),
        new HtmlNameData.NameDef((short) 482, "href", HtmlTagId.Unknown, HtmlAttributeId.Href),
        new HtmlNameData.NameDef((short) 484, "img", HtmlTagIndex.Img, false, false, HtmlTagId.Img, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 486, "vspace", HtmlTagId.Unknown, HtmlAttributeId.Vspace),
        new HtmlNameData.NameDef((short) 487, "h3", HtmlTagIndex.H3, false, false, HtmlTagId.H3, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 488, "meter", HtmlTagIndex.Meter, false, false, HtmlTagId.Meter, HtmlAttributeId.Meter),
        new HtmlNameData.NameDef((short) 493, "align", HtmlTagId.Unknown, HtmlAttributeId.Align),
        new HtmlNameData.NameDef((short) 497, "wbr", HtmlTagIndex.Wbr, false, false, HtmlTagId.Wbr, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 499, "accesskey", HtmlTagId.Unknown, HtmlAttributeId.AccessKey),
        new HtmlNameData.NameDef((short) 502, "col", HtmlTagIndex.Col, false, false, HtmlTagId.Col, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 504, "bigger", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 504, "menu", HtmlTagIndex.Menu, false, false, HtmlTagId.Menu, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 506, "codebase", HtmlTagId.Unknown, HtmlAttributeId.CodeBase),
        new HtmlNameData.NameDef((short) 508, "strong", HtmlTagIndex.Strong, false, false, HtmlTagId.Strong, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 510, "br", HtmlTagIndex.BR, false, false, HtmlTagId.BR, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 512, "archive", HtmlTagId.Unknown, HtmlAttributeId.Archive),
        new HtmlNameData.NameDef((short) 514, "ul", HtmlTagIndex.UL, false, false, HtmlTagId.UL, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 516, "noscript", HtmlTagIndex.NoScript, false, false, HtmlTagId.NoScript, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 517, "plaintext", HtmlTagIndex.PlainText, true, true, HtmlTagId.PlainText, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 521, "base", HtmlTagIndex.Base, false, false, HtmlTagId.Base, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 521, "ismap", HtmlTagId.Unknown, HtmlAttributeId.IsMap),
        new HtmlNameData.NameDef((short) 522, "defer", HtmlTagId.Unknown, HtmlAttributeId.Defer),
        new HtmlNameData.NameDef((short) 523, "body", HtmlTagIndex.Body, false, false, HtmlTagId.Body, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 524, "ol", HtmlTagIndex.OL, false, false, HtmlTagId.OL, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 526, "h1", HtmlTagIndex.H1, false, false, HtmlTagId.H1, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 528, "valign", HtmlTagId.Unknown, HtmlAttributeId.Valign),
        new HtmlNameData.NameDef((short) 531, "media", HtmlTagId.Unknown, HtmlAttributeId.Media),
        new HtmlNameData.NameDef((short) 532, "iframe", HtmlTagIndex.Iframe, false, false, HtmlTagId.Iframe, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 533, "dl", HtmlTagIndex.DL, false, false, HtmlTagId.DL, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 535, "colspan", HtmlTagId.Unknown, HtmlAttributeId.ColSpan),
        new HtmlNameData.NameDef((short) 538, "axis", HtmlTagId.Unknown, HtmlAttributeId.Axis),
        new HtmlNameData.NameDef((short) 542, "marginheight", HtmlTagId.Unknown, HtmlAttributeId.MarginHeight),
        new HtmlNameData.NameDef((short) 542, "itemtype", HtmlTagId.Unknown, HtmlAttributeId.ItemType),
        new HtmlNameData.NameDef((short) 543, "alink", HtmlTagId.Unknown, HtmlAttributeId.Alink),
        new HtmlNameData.NameDef((short) 545, "?xml:namespace", HtmlTagIndex._Xml_Namespace, false, false, HtmlTagId.Unknown, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 546, "method", HtmlTagId.Unknown, HtmlAttributeId.Method),
        new HtmlNameData.NameDef((short) 549, "fontfamily", HtmlTagId.Unknown, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 554, "fieldset", HtmlTagIndex.FieldSet, false, false, HtmlTagId.FieldSet, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 556, "pre", HtmlTagIndex.Pre, false, false, HtmlTagId.Pre, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 557, "table", HtmlTagIndex.Table, false, false, HtmlTagId.Table, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 560, "tr", HtmlTagIndex.TR, false, false, HtmlTagId.TR, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 562, "samp", HtmlTagIndex.Samp, false, false, HtmlTagId.Samp, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 563, "link", HtmlTagIndex.Link, false, false, HtmlTagId.Link, HtmlAttributeId.Link),
        new HtmlNameData.NameDef((short) 564, "hr", HtmlTagIndex.HR, false, false, HtmlTagId.HR, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 566, "itemref", HtmlTagId.Unknown, HtmlAttributeId.ItemRef),
        new HtmlNameData.NameDef((short) 568, "form", HtmlTagIndex.Form, false, false, HtmlTagId.Form, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 569, "input", HtmlTagIndex.Input, false, false, HtmlTagId.Input, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 570, "xml", HtmlTagIndex.Xml, true, true, HtmlTagId.Xml, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 572, "usemap", HtmlTagId.Unknown, HtmlAttributeId.UseMap),
        new HtmlNameData.NameDef((short) 574, "xmp", HtmlTagIndex.Xmp, true, true, HtmlTagId.Xmp, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 574, "html", HtmlTagIndex.Html, false, false, HtmlTagId.Html, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 575, "codetype", HtmlTagId.Unknown, HtmlAttributeId.CodeType),
        new HtmlNameData.NameDef((short) 580, "marginwidth", HtmlTagId.Unknown, HtmlAttributeId.MarginWidth),
        new HtmlNameData.NameDef((short) 584, "q", HtmlTagIndex.Q, false, false, HtmlTagId.Q, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 585, "dynsrc", HtmlTagId.Unknown, HtmlAttributeId.DynSrc),
        new HtmlNameData.NameDef((short) 585, "colgroup", HtmlTagIndex.ColGroup, false, false, HtmlTagId.ColGroup, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 586, "s", HtmlTagIndex.S, false, false, HtmlTagId.S, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 587, "p", HtmlTagIndex.P, false, false, HtmlTagId.P, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 588, "u", HtmlTagIndex.U, false, false, HtmlTagId.U, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 589, "action", HtmlTagId.Unknown, HtmlAttributeId.Action),
        new HtmlNameData.NameDef((short) 590, "enctype", HtmlTagId.Unknown, HtmlAttributeId.EncType),
        new HtmlNameData.NameDef((short) 591, "itemid", HtmlTagId.Unknown, HtmlAttributeId.ItemId),
        new HtmlNameData.NameDef((short) 592, "i", HtmlTagIndex.I, false, false, HtmlTagId.I, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 597, "b", HtmlTagIndex.B, false, false, HtmlTagId.B, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 598, "h5", HtmlTagIndex.H5, false, false, HtmlTagId.H5, HtmlAttributeId.Unknown),
        new HtmlNameData.NameDef((short) 599, "background", HtmlTagId.Unknown, HtmlAttributeId.Background),
        new HtmlNameData.NameDef((short) 600, (string) null, HtmlTagId.Unknown, HtmlAttributeId.Unknown)
      };
      HtmlNameData.TagIndex = new HtmlNameIndex[111]
      {
        HtmlNameIndex.Unknown,
        HtmlNameIndex.A,
        HtmlNameIndex.Abbr,
        HtmlNameIndex.Acronym,
        HtmlNameIndex.Address,
        HtmlNameIndex.Applet,
        HtmlNameIndex.Area,
        HtmlNameIndex.B,
        HtmlNameIndex.Base,
        HtmlNameIndex.BaseFont,
        HtmlNameIndex.Bdo,
        HtmlNameIndex.BGSound,
        HtmlNameIndex.Big,
        HtmlNameIndex.Blink,
        HtmlNameIndex.BlockQuote,
        HtmlNameIndex.Body,
        HtmlNameIndex.BR,
        HtmlNameIndex.Button,
        HtmlNameIndex.Caption,
        HtmlNameIndex.Center,
        HtmlNameIndex.Cite,
        HtmlNameIndex.Code,
        HtmlNameIndex.Col,
        HtmlNameIndex.ColGroup,
        HtmlNameIndex.Comment,
        HtmlNameIndex.DD,
        HtmlNameIndex.Del,
        HtmlNameIndex.Dfn,
        HtmlNameIndex.Dir,
        HtmlNameIndex.Div,
        HtmlNameIndex.DL,
        HtmlNameIndex.DT,
        HtmlNameIndex.EM,
        HtmlNameIndex.Embed,
        HtmlNameIndex.FieldSet,
        HtmlNameIndex.Font,
        HtmlNameIndex.Form,
        HtmlNameIndex.Frame,
        HtmlNameIndex.FrameSet,
        HtmlNameIndex.H1,
        HtmlNameIndex.H2,
        HtmlNameIndex.H3,
        HtmlNameIndex.H4,
        HtmlNameIndex.H5,
        HtmlNameIndex.H6,
        HtmlNameIndex.Head,
        HtmlNameIndex.HR,
        HtmlNameIndex.Html,
        HtmlNameIndex.I,
        HtmlNameIndex.Iframe,
        HtmlNameIndex.Image,
        HtmlNameIndex.Img,
        HtmlNameIndex.Input,
        HtmlNameIndex.Ins,
        HtmlNameIndex.IsIndex,
        HtmlNameIndex.Kbd,
        HtmlNameIndex.Label,
        HtmlNameIndex.Legend,
        HtmlNameIndex.LI,
        HtmlNameIndex.Link,
        HtmlNameIndex.Listing,
        HtmlNameIndex.Map,
        HtmlNameIndex.Marquee,
        HtmlNameIndex.Menu,
        HtmlNameIndex.Meta,
        HtmlNameIndex.NextId,
        HtmlNameIndex.NoBR,
        HtmlNameIndex.NoEmbed,
        HtmlNameIndex.NoFrames,
        HtmlNameIndex.NoScript,
        HtmlNameIndex.Object,
        HtmlNameIndex.OL,
        HtmlNameIndex.OptGroup,
        HtmlNameIndex.Option,
        HtmlNameIndex.P,
        HtmlNameIndex.Param,
        HtmlNameIndex.PlainText,
        HtmlNameIndex.Pre,
        HtmlNameIndex.Q,
        HtmlNameIndex.RP,
        HtmlNameIndex.RT,
        HtmlNameIndex.Ruby,
        HtmlNameIndex.S,
        HtmlNameIndex.Samp,
        HtmlNameIndex.Script,
        HtmlNameIndex.Select,
        HtmlNameIndex.Small,
        HtmlNameIndex.Span,
        HtmlNameIndex.Strike,
        HtmlNameIndex.Strong,
        HtmlNameIndex.Style,
        HtmlNameIndex.Sub,
        HtmlNameIndex.Sup,
        HtmlNameIndex.Table,
        HtmlNameIndex.Tbody,
        HtmlNameIndex.TD,
        HtmlNameIndex.TextArea,
        HtmlNameIndex.Tfoot,
        HtmlNameIndex.TH,
        HtmlNameIndex.Thead,
        HtmlNameIndex.Title,
        HtmlNameIndex.TR,
        HtmlNameIndex.TT,
        HtmlNameIndex.U,
        HtmlNameIndex.UL,
        HtmlNameIndex.Var,
        HtmlNameIndex.Wbr,
        HtmlNameIndex.Xml,
        HtmlNameIndex.Xmp,
        HtmlNameIndex.Data,
        HtmlNameIndex.Meter
      };
      HtmlNameData.attributeIndex = new HtmlNameIndex[112]
      {
        HtmlNameIndex.Unknown,
        HtmlNameIndex.Abbr,
        HtmlNameIndex.Accept,
        HtmlNameIndex.AcceptCharset,
        HtmlNameIndex.AccessKey,
        HtmlNameIndex.Action,
        HtmlNameIndex.Align,
        HtmlNameIndex.Alink,
        HtmlNameIndex.Alt,
        HtmlNameIndex.Archive,
        HtmlNameIndex.Axis,
        HtmlNameIndex.Background,
        HtmlNameIndex.BGColor,
        HtmlNameIndex.Border,
        HtmlNameIndex.CellPadding,
        HtmlNameIndex.CellSpacing,
        HtmlNameIndex.Char,
        HtmlNameIndex.CharOff,
        HtmlNameIndex.Charset,
        HtmlNameIndex.Checked,
        HtmlNameIndex.Cite,
        HtmlNameIndex.Class,
        HtmlNameIndex.ClassId,
        HtmlNameIndex.Clear,
        HtmlNameIndex.Code,
        HtmlNameIndex.CodeBase,
        HtmlNameIndex.CodeType,
        HtmlNameIndex.Color,
        HtmlNameIndex.Cols,
        HtmlNameIndex.ColSpan,
        HtmlNameIndex.Compact,
        HtmlNameIndex.Content,
        HtmlNameIndex.Coords,
        HtmlNameIndex.Data,
        HtmlNameIndex.DateTime,
        HtmlNameIndex.Declare,
        HtmlNameIndex.Defer,
        HtmlNameIndex.Dir,
        HtmlNameIndex.Disabled,
        HtmlNameIndex.DynSrc,
        HtmlNameIndex.EncType,
        HtmlNameIndex.Face,
        HtmlNameIndex.For,
        HtmlNameIndex.Frame,
        HtmlNameIndex.FrameBorder,
        HtmlNameIndex.Headers,
        HtmlNameIndex.Height,
        HtmlNameIndex.Href,
        HtmlNameIndex.HrefLang,
        HtmlNameIndex.Hspace,
        HtmlNameIndex.HttpEquiv,
        HtmlNameIndex.Id,
        HtmlNameIndex.IsMap,
        HtmlNameIndex.Label,
        HtmlNameIndex.Lang,
        HtmlNameIndex.Language,
        HtmlNameIndex.LeftMargin,
        HtmlNameIndex.Link,
        HtmlNameIndex.LongDesc,
        HtmlNameIndex.LowSrc,
        HtmlNameIndex.MarginHeight,
        HtmlNameIndex.MarginWidth,
        HtmlNameIndex.MaxLength,
        HtmlNameIndex.Media,
        HtmlNameIndex.Method,
        HtmlNameIndex.Multiple,
        HtmlNameIndex.Name,
        HtmlNameIndex.NoHref,
        HtmlNameIndex.NoResize,
        HtmlNameIndex.NoShade,
        HtmlNameIndex.NoWrap,
        HtmlNameIndex.Object,
        HtmlNameIndex.Profile,
        HtmlNameIndex.Prompt,
        HtmlNameIndex.ReadOnly,
        HtmlNameIndex.Rel,
        HtmlNameIndex.Rev,
        HtmlNameIndex.Rows,
        HtmlNameIndex.RowSpan,
        HtmlNameIndex.Rules,
        HtmlNameIndex.Scheme,
        HtmlNameIndex.Scope,
        HtmlNameIndex.Scrolling,
        HtmlNameIndex.Selected,
        HtmlNameIndex.Shape,
        HtmlNameIndex.Size,
        HtmlNameIndex.Span,
        HtmlNameIndex.Src,
        HtmlNameIndex.StandBy,
        HtmlNameIndex.Start,
        HtmlNameIndex.Style,
        HtmlNameIndex.Summary,
        HtmlNameIndex.TabIndex,
        HtmlNameIndex.Target,
        HtmlNameIndex.Text,
        HtmlNameIndex.Title,
        HtmlNameIndex.TopMargin,
        HtmlNameIndex.Type,
        HtmlNameIndex.UseMap,
        HtmlNameIndex.Valign,
        HtmlNameIndex.Value,
        HtmlNameIndex.ValueType,
        HtmlNameIndex.Version,
        HtmlNameIndex.Vlink,
        HtmlNameIndex.Vspace,
        HtmlNameIndex.Width,
        HtmlNameIndex.ItemId,
        HtmlNameIndex.ItemProp,
        HtmlNameIndex.ItemRef,
        HtmlNameIndex.ItemScope,
        HtmlNameIndex.ItemType,
        HtmlNameIndex.Meter
      };
      HtmlNameData.entities = new HtmlNameData.EntityDef[273]
      {
        new HtmlNameData.EntityDef((short) 0, (short) 0, (string) null),
        new HtmlNameData.EntityDef((short) 1, (short) 969, "omega"),
        new HtmlNameData.EntityDef((short) 1, (short) 8195, "emsp"),
        new HtmlNameData.EntityDef((short) 2, (short) 8235, "rle"),
        new HtmlNameData.EntityDef((short) 5, (short) 211, "Oacute"),
        new HtmlNameData.EntityDef((short) 8, (short) 402, "fnof"),
        new HtmlNameData.EntityDef((short) 9, (short) 216, "Oslash"),
        new HtmlNameData.EntityDef((short) 12, (short) 209, "Ntilde"),
        new HtmlNameData.EntityDef((short) 14, (short) 8592, "larr"),
        new HtmlNameData.EntityDef((short) 15, (short) 968, "psi"),
        new HtmlNameData.EntityDef((short) 15, (short) 34, "QUOT"),
        new HtmlNameData.EntityDef((short) 20, (short) 928, "Pi"),
        new HtmlNameData.EntityDef((short) 22, (short) 181, "micro"),
        new HtmlNameData.EntityDef((short) 24, (short) 982, "piv"),
        new HtmlNameData.EntityDef((short) 26, (short) 978, "upsih"),
        new HtmlNameData.EntityDef((short) 28, (short) 926, "Xi"),
        new HtmlNameData.EntityDef((short) 28, (short) 8968, "lceil"),
        new HtmlNameData.EntityDef((short) 29, (short) 229, "aring"),
        new HtmlNameData.EntityDef((short) 30, (short) 8715, "ni"),
        new HtmlNameData.EntityDef((short) 30, (short) 175, "macr"),
        new HtmlNameData.EntityDef((short) 32, (short) 8745, "cap"),
        new HtmlNameData.EntityDef((short) 33, (short) 239, "iuml"),
        new HtmlNameData.EntityDef((short) 34, (short) 967, "chi"),
        new HtmlNameData.EntityDef((short) 38, (short) 188, "frac14"),
        new HtmlNameData.EntityDef((short) 40, (short) 190, "frac34"),
        new HtmlNameData.EntityDef((short) 41, (short) 186, "ordm"),
        new HtmlNameData.EntityDef((short) 41, (short) 160, "nbsp"),
        new HtmlNameData.EntityDef((short) 44, (short) 8743, "and"),
        new HtmlNameData.EntityDef((short) 47, (short) 166, "brvbar"),
        new HtmlNameData.EntityDef((short) 49, (short) 8203, "zwsp"),
        new HtmlNameData.EntityDef((short) 50, (short) 8704, "forall"),
        new HtmlNameData.EntityDef((short) 52, (short) 960, "pi"),
        new HtmlNameData.EntityDef((short) 53, (short) 8855, "otimes"),
        new HtmlNameData.EntityDef((short) 54, (short) 250, "uacute"),
        new HtmlNameData.EntityDef((short) 55, (short) 8736, "ang"),
        new HtmlNameData.EntityDef((short) 56, (short) 161, "iexcl"),
        new HtmlNameData.EntityDef((short) 57, (short) 8206, "lrm"),
        new HtmlNameData.EntityDef((short) 57, (short) 965, "upsilon"),
        new HtmlNameData.EntityDef((short) 60, (short) 958, "xi"),
        new HtmlNameData.EntityDef((short) 65, (short) 8234, "lre"),
        new HtmlNameData.EntityDef((short) 66, (short) 8205, "zwj"),
        new HtmlNameData.EntityDef((short) 68, (short) 925, "Nu"),
        new HtmlNameData.EntityDef((short) 69, (short) 924, "Mu"),
        new HtmlNameData.EntityDef((short) 71, (short) 8237, "lro"),
        new HtmlNameData.EntityDef((short) 73, (short) 169, "COPY"),
        new HtmlNameData.EntityDef((short) 74, (short) 8836, "nsub"),
        new HtmlNameData.EntityDef((short) 74, (short) 8834, "sub"),
        new HtmlNameData.EntityDef((short) 83, (short) 254, "thorn"),
        new HtmlNameData.EntityDef((short) 85, (short) 8721, "sum"),
        new HtmlNameData.EntityDef((short) 87, (short) 8217, "rsquo"),
        new HtmlNameData.EntityDef((short) 88, (short) 183, "middot"),
        new HtmlNameData.EntityDef((short) 97, (short) 202, "Ecirc"),
        new HtmlNameData.EntityDef((short) 97, (short) 8240, "permil"),
        new HtmlNameData.EntityDef((short) 98, (short) 8201, "thinsp"),
        new HtmlNameData.EntityDef((short) 100, (short) 215, "times"),
        new HtmlNameData.EntityDef((short) 100, (short) 957, "nu"),
        new HtmlNameData.EntityDef((short) 101, (short) 956, "mu"),
        new HtmlNameData.EntityDef((short) 102, (short) 165, "yen"),
        new HtmlNameData.EntityDef((short) 107, (short) 8719, "prod"),
        new HtmlNameData.EntityDef((short) 116, (short) 8659, "dArr"),
        new HtmlNameData.EntityDef((short) 118, (short) 235, "euml"),
        new HtmlNameData.EntityDef((short) 118, (short) 226, "acirc"),
        new HtmlNameData.EntityDef((short) 122, (short) 914, "Beta"),
        new HtmlNameData.EntityDef((short) 123, (short) 8730, "radic"),
        new HtmlNameData.EntityDef((short) 123, (short) 189, "frac12"),
        new HtmlNameData.EntityDef((short) 130, (short) 9829, "hearts"),
        new HtmlNameData.EntityDef((short) 131, (short) 8482, "TRADE"),
        new HtmlNameData.EntityDef((short) 134, (short) 8250, "rsaquo"),
        new HtmlNameData.EntityDef((short) 136, (short) 196, "Auml"),
        new HtmlNameData.EntityDef((short) 137, (short) 249, "ugrave"),
        new HtmlNameData.EntityDef((short) 142, (short) 231, "ccedil"),
        new HtmlNameData.EntityDef((short) 143, (short) 338, "OElig"),
        new HtmlNameData.EntityDef((short) 144, (short) 167, "sect"),
        new HtmlNameData.EntityDef((short) 144, (short) 201, "Eacute"),
        new HtmlNameData.EntityDef((short) 146, (short) 8756, "there4"),
        new HtmlNameData.EntityDef((short) 148, (short) 174, "REG"),
        new HtmlNameData.EntityDef((short) 155, (short) 177, "plusmn"),
        new HtmlNameData.EntityDef((short) 155, (short) 9001, "lang"),
        new HtmlNameData.EntityDef((short) 158, (short) 977, "thetasym"),
        new HtmlNameData.EntityDef((short) 161, (short) 8658, "rArr"),
        new HtmlNameData.EntityDef((short) 162, (short) 953, "iota"),
        new HtmlNameData.EntityDef((short) 163, (short) 8969, "rceil"),
        new HtmlNameData.EntityDef((short) 164, (short) 8709, "empty"),
        new HtmlNameData.EntityDef((short) 164, (short) 38, "AMP"),
        new HtmlNameData.EntityDef((short) 164, (short) 62, "gt"),
        new HtmlNameData.EntityDef((short) 165, (short) 934, "Phi"),
        new HtmlNameData.EntityDef((short) 166, (short) 915, "Gamma"),
        new HtmlNameData.EntityDef((short) 167, (short) 8299, "ass"),
        new HtmlNameData.EntityDef((short) 171, (short) 60, "lt"),
        new HtmlNameData.EntityDef((short) 177, (short) 918, "Zeta"),
        new HtmlNameData.EntityDef((short) 179, (short) 210, "Ograve"),
        new HtmlNameData.EntityDef((short) 179, (short) 176, "deg"),
        new HtmlNameData.EntityDef((short) 187, (short) 9824, "spades"),
        new HtmlNameData.EntityDef((short) 193, (short) 8204, "zwnj"),
        new HtmlNameData.EntityDef((short) 193, (short) 8707, "exist"),
        new HtmlNameData.EntityDef((short) 196, (short) 948, "delta"),
        new HtmlNameData.EntityDef((short) 200, (short) 174, "reg"),
        new HtmlNameData.EntityDef((short) 203, (short) 8712, "isin"),
        new HtmlNameData.EntityDef((short) 204, (short) 913, "Alpha"),
        new HtmlNameData.EntityDef((short) 207, (short) 376, "Yuml"),
        new HtmlNameData.EntityDef((short) 211, (short) 184, "cedil"),
        new HtmlNameData.EntityDef((short) 211, (short) 178, "sup2"),
        new HtmlNameData.EntityDef((short) 218, (short) 8971, "rfloor"),
        new HtmlNameData.EntityDef((short) 220, (short) 247, "divide"),
        new HtmlNameData.EntityDef((short) 222, (short) 927, "Omicron"),
        new HtmlNameData.EntityDef((short) 225, (short) 170, "ordf"),
        new HtmlNameData.EntityDef((short) 227, (short) 9827, "clubs"),
        new HtmlNameData.EntityDef((short) 228, (short) 220, "Uuml"),
        new HtmlNameData.EntityDef((short) 229, (short) 919, "Eta"),
        new HtmlNameData.EntityDef((short) 231, (short) 194, "Acirc"),
        new HtmlNameData.EntityDef((short) 232, (short) 195, "Atilde"),
        new HtmlNameData.EntityDef((short) 233, (short) 929, "Rho"),
        new HtmlNameData.EntityDef((short) 234, (short) 8501, "alefsym"),
        new HtmlNameData.EntityDef((short) 240, (short) 198, "AElig"),
        new HtmlNameData.EntityDef((short) 240, (short) 8970, "lfloor"),
        new HtmlNameData.EntityDef((short) 248, (short) 8660, "hArr"),
        new HtmlNameData.EntityDef((short) 250, (short) 8254, "oline"),
        new HtmlNameData.EntityDef((short) 254, (short) 193, "Aacute"),
        new HtmlNameData.EntityDef((short) byte.MaxValue, (short) 199, "Ccedil"),
        new HtmlNameData.EntityDef((short) 256, (short) 952, "theta"),
        new HtmlNameData.EntityDef((short) 257, (short) 8744, "or"),
        new HtmlNameData.EntityDef((short) 258, (short) 200, "Egrave"),
        new HtmlNameData.EntityDef((short) 259, (short) 8482, "trade"),
        new HtmlNameData.EntityDef((short) 260, (short) 8747, "int"),
        new HtmlNameData.EntityDef((short) 262, (short) 185, "sup1"),
        new HtmlNameData.EntityDef((short) 263, (short) 966, "phi"),
        new HtmlNameData.EntityDef((short) 265, (short) 8746, "cup"),
        new HtmlNameData.EntityDef((short) 268, (short) 8801, "equiv"),
        new HtmlNameData.EntityDef((short) 272, (short) 8764, "sim"),
        new HtmlNameData.EntityDef((short) 273, (short) 221, "Yacute"),
        new HtmlNameData.EntityDef((short) 276, (short) 8243, "Prime"),
        new HtmlNameData.EntityDef((short) 277, (short) 8593, "uarr"),
        new HtmlNameData.EntityDef((short) 280, (short) 8212, "mdash"),
        new HtmlNameData.EntityDef((short) 280, (short) 38, "amp"),
        new HtmlNameData.EntityDef((short) 281, (short) 180, "acute"),
        new HtmlNameData.EntityDef((short) 285, (short) 208, "ETH"),
        new HtmlNameData.EntityDef((short) 287, (short) 233, "eacute"),
        new HtmlNameData.EntityDef((short) 290, (short) 8472, "weierp"),
        new HtmlNameData.EntityDef((short) 292, (short) 922, "Kappa"),
        new HtmlNameData.EntityDef((short) 294, (short) 920, "Theta"),
        new HtmlNameData.EntityDef((short) 304, (short) 163, "pound"),
        new HtmlNameData.EntityDef((short) 307, (short) 8594, "rarr"),
        new HtmlNameData.EntityDef((short) 307, (short) 937, "Omega"),
        new HtmlNameData.EntityDef((short) 312, (short) 339, "oelig"),
        new HtmlNameData.EntityDef((short) 313, (short) 8835, "sup"),
        new HtmlNameData.EntityDef((short) 314, (short) 236, "igrave"),
        new HtmlNameData.EntityDef((short) 316, (short) 162, "cent"),
        new HtmlNameData.EntityDef((short) 319, (short) 224, "agrave"),
        new HtmlNameData.EntityDef((short) 321, (short) 8224, "dagger"),
        new HtmlNameData.EntityDef((short) 328, (short) 8226, "bull"),
        new HtmlNameData.EntityDef((short) 330, (short) 214, "Ouml"),
        new HtmlNameData.EntityDef((short) 335, (short) 197, "Aring"),
        new HtmlNameData.EntityDef((short) 338, (short) 9674, "loz"),
        new HtmlNameData.EntityDef((short) 345, (short) 8727, "lowast"),
        new HtmlNameData.EntityDef((short) 346, (short) 245, "otilde"),
        new HtmlNameData.EntityDef((short) 351, (short) 8364, "euro"),
        new HtmlNameData.EntityDef((short) 352, (short) 168, "uml"),
        new HtmlNameData.EntityDef((short) 354, (short) 963, "sigma"),
        new HtmlNameData.EntityDef((short) 373, (short) 917, "Epsilon"),
        new HtmlNameData.EntityDef((short) 376, (short) 8249, "lsaquo"),
        new HtmlNameData.EntityDef((short) 379, (short) 8465, "image"),
        new HtmlNameData.EntityDef((short) 380, (short) 8656, "lArr"),
        new HtmlNameData.EntityDef((short) 384, (short) 207, "Iuml"),
        new HtmlNameData.EntityDef((short) 385, (short) 935, "Chi"),
        new HtmlNameData.EntityDef((short) 390, (short) 951, "eta"),
        new HtmlNameData.EntityDef((short) 394, (short) 8596, "harr"),
        new HtmlNameData.EntityDef((short) 397, (short) 225, "aacute"),
        new HtmlNameData.EntityDef((short) 399, (short) 8302, "nads"),
        new HtmlNameData.EntityDef((short) 401, (short) 240, "eth"),
        new HtmlNameData.EntityDef((short) 402, (short) 62, "GT"),
        new HtmlNameData.EntityDef((short) 404, (short) 931, "Sigma"),
        new HtmlNameData.EntityDef((short) 404, (short) 222, "THORN"),
        new HtmlNameData.EntityDef((short) 408, (short) 248, "oslash"),
        new HtmlNameData.EntityDef((short) 409, (short) 230, "aelig"),
        new HtmlNameData.EntityDef((short) 409, (short) 60, "LT"),
        new HtmlNameData.EntityDef((short) 411, (short) 8713, "notin"),
        new HtmlNameData.EntityDef((short) 415, (short) 8301, "aafs"),
        new HtmlNameData.EntityDef((short) 416, (short) 253, "yacute"),
        new HtmlNameData.EntityDef((short) 427, (short) 171, "laquo"),
        new HtmlNameData.EntityDef((short) 431, (short) 8242, "prime"),
        new HtmlNameData.EntityDef((short) 431, (short) 8298, "iss"),
        new HtmlNameData.EntityDef((short) 432, (short) 192, "Agrave"),
        new HtmlNameData.EntityDef((short) 433, (short) 955, "lambda"),
        new HtmlNameData.EntityDef((short) 436, (short) 8853, "oplus"),
        new HtmlNameData.EntityDef((short) 442, (short) 8476, "real"),
        new HtmlNameData.EntityDef((short) 443, (short) 217, "Ugrave"),
        new HtmlNameData.EntityDef((short) 448, (short) 8839, "supe"),
        new HtmlNameData.EntityDef((short) 450, (short) 182, "para"),
        new HtmlNameData.EntityDef((short) 455, (short) 8595, "darr"),
        new HtmlNameData.EntityDef((short) 466, (short) 8838, "sube"),
        new HtmlNameData.EntityDef((short) 468, (short) 8776, "asymp"),
        new HtmlNameData.EntityDef((short) 469, (short) 203, "Euml"),
        new HtmlNameData.EntityDef((short) 470, (short) 244, "ocirc"),
        new HtmlNameData.EntityDef((short) 471, (short) 923, "Lambda"),
        new HtmlNameData.EntityDef((short) 476, (short) 946, "beta"),
        new HtmlNameData.EntityDef((short) 477, (short) 179, "sup3"),
        new HtmlNameData.EntityDef((short) 486, (short) 8734, "infin"),
        new HtmlNameData.EntityDef((short) 487, (short) 933, "Upsilon"),
        new HtmlNameData.EntityDef((short) 488, (short) 251, "ucirc"),
        new HtmlNameData.EntityDef((short) 490, (short) 916, "Delta"),
        new HtmlNameData.EntityDef((short) 490, (short) 228, "auml"),
        new HtmlNameData.EntityDef((short) 506, (short) 205, "Iacute"),
        new HtmlNameData.EntityDef((short) 506, (short) 710, "circ"),
        new HtmlNameData.EntityDef((short) 513, (short) 921, "Iota"),
        new HtmlNameData.EntityDef((short) 516, (short) 732, "tilde"),
        new HtmlNameData.EntityDef((short) 517, (short) 8222, "bdquo"),
        new HtmlNameData.EntityDef((short) 520, (short) 353, "scaron"),
        new HtmlNameData.EntityDef((short) 529, (short) 932, "Tau"),
        new HtmlNameData.EntityDef((short) 531, (short) 950, "zeta"),
        new HtmlNameData.EntityDef((short) 533, (short) 8236, "pdf"),
        new HtmlNameData.EntityDef((short) 537, (short) 8869, "perp"),
        new HtmlNameData.EntityDef((short) 546, (short) 173, "shy"),
        new HtmlNameData.EntityDef((short) 547, (short) 238, "icirc"),
        new HtmlNameData.EntityDef((short) 561, (short) byte.MaxValue, "yuml"),
        new HtmlNameData.EntityDef((short) 565, (short) 947, "gamma"),
        new HtmlNameData.EntityDef((short) 567, (short) 962, "sigmaf"),
        new HtmlNameData.EntityDef((short) 569, (short) 9002, "rang"),
        new HtmlNameData.EntityDef((short) 572, (short) 8629, "crarr"),
        new HtmlNameData.EntityDef((short) 575, (short) 187, "raquo"),
        new HtmlNameData.EntityDef((short) 577, (short) 8722, "minus"),
        new HtmlNameData.EntityDef((short) 578, (short) 242, "ograve"),
        new HtmlNameData.EntityDef((short) 582, (short) 252, "uuml"),
        new HtmlNameData.EntityDef((short) 583, (short) 212, "Ocirc"),
        new HtmlNameData.EntityDef((short) 585, (short) 8303, "nods"),
        new HtmlNameData.EntityDef((short) 586, (short) 8220, "ldquo"),
        new HtmlNameData.EntityDef((short) 587, (short) 961, "rho"),
        new HtmlNameData.EntityDef((short) 588, (short) 223, "szlig"),
        new HtmlNameData.EntityDef((short) 601, (short) 219, "Ucirc"),
        new HtmlNameData.EntityDef((short) 603, (short) 945, "alpha"),
        new HtmlNameData.EntityDef((short) 606, (short) 34, "quot"),
        new HtmlNameData.EntityDef((short) 615, (short) 8225, "Dagger"),
        new HtmlNameData.EntityDef((short) 616, (short) 218, "Uacute"),
        new HtmlNameData.EntityDef((short) 620, (short) 204, "Igrave"),
        new HtmlNameData.EntityDef((short) 622, (short) 936, "Psi"),
        new HtmlNameData.EntityDef((short) 627, (short) 964, "tau"),
        new HtmlNameData.EntityDef((short) 628, (short) 8216, "lsquo"),
        new HtmlNameData.EntityDef((short) 629, (short) 8805, "ge"),
        new HtmlNameData.EntityDef((short) 631, (short) 227, "atilde"),
        new HtmlNameData.EntityDef((short) 632, (short) 8711, "nabla"),
        new HtmlNameData.EntityDef((short) 633, (short) 352, "Scaron"),
        new HtmlNameData.EntityDef((short) 634, (short) 8773, "cong"),
        new HtmlNameData.EntityDef((short) 635, (short) 8260, "frasl"),
        new HtmlNameData.EntityDef((short) 638, (short) 8800, "ne"),
        new HtmlNameData.EntityDef((short) 639, (short) 164, "curren"),
        new HtmlNameData.EntityDef((short) 640, (short) 8804, "le"),
        new HtmlNameData.EntityDef((short) 643, (short) 8657, "uArr"),
        new HtmlNameData.EntityDef((short) 648, (short) 949, "epsilon"),
        new HtmlNameData.EntityDef((short) 649, (short) 237, "iacute"),
        new HtmlNameData.EntityDef((short) 651, (short) 8207, "rlm"),
        new HtmlNameData.EntityDef((short) 652, (short) 213, "Otilde"),
        new HtmlNameData.EntityDef((short) 653, (short) 8901, "sdot"),
        new HtmlNameData.EntityDef((short) 653, (short) 8238, "rlo"),
        new HtmlNameData.EntityDef((short) 654, (short) 8211, "ndash"),
        new HtmlNameData.EntityDef((short) 657, (short) 232, "egrave"),
        new HtmlNameData.EntityDef((short) 660, (short) 206, "Icirc"),
        new HtmlNameData.EntityDef((short) 660, (short) 243, "oacute"),
        new HtmlNameData.EntityDef((short) 662, (short) 8706, "part"),
        new HtmlNameData.EntityDef((short) 663, (short) 9830, "diams"),
        new HtmlNameData.EntityDef((short) 664, (short) 169, "copy"),
        new HtmlNameData.EntityDef((short) 667, (short) 241, "ntilde"),
        new HtmlNameData.EntityDef((short) 670, (short) 8221, "rdquo"),
        new HtmlNameData.EntityDef((short) 671, (short) 191, "iquest"),
        new HtmlNameData.EntityDef((short) 672, (short) 8218, "sbquo"),
        new HtmlNameData.EntityDef((short) 678, (short) 172, "not"),
        new HtmlNameData.EntityDef((short) 679, (short) 8300, "iafs"),
        new HtmlNameData.EntityDef((short) 684, (short) 246, "ouml"),
        new HtmlNameData.EntityDef((short) 689, (short) 234, "ecirc"),
        new HtmlNameData.EntityDef((short) 691, (short) 954, "kappa"),
        new HtmlNameData.EntityDef((short) 696, (short) 8194, "ensp"),
        new HtmlNameData.EntityDef((short) 699, (short) 8733, "prop"),
        new HtmlNameData.EntityDef((short) 702, (short) 959, "omicron"),
        new HtmlNameData.EntityDef((short) 703, (short) 8230, "hellip"),
        new HtmlNameData.EntityDef((short) 704, (short) 0, (string) null)
      };
    }

    public struct NameDef
    {
      public short Hash;
      public bool LiteralTag;
      public bool LiteralEnt;
      public string Name;
      public HtmlTagIndex TagIndex;
      public HtmlTagId PublicTagId;
      public HtmlAttributeId PublicAttributeId;

      public NameDef(short hash, string name, HtmlTagId publicTagId, HtmlAttributeId publicAttributeId)
      {
        this = new HtmlNameData.NameDef(hash, name, HtmlTagIndex.Unknown, false, false, publicTagId, publicAttributeId);
      }

      public NameDef(short hash, string name, HtmlTagIndex tagIndex, bool literalTag, bool literalEnt, HtmlTagId publicTagId, HtmlAttributeId publicAttributeId)
      {
        this.Hash = hash;
        this.LiteralTag = literalTag;
        this.LiteralEnt = literalEnt;
        this.Name = name;
        this.TagIndex = tagIndex;
        this.PublicTagId = publicTagId;
        this.PublicAttributeId = publicAttributeId;
      }
    }

    public struct EntityDef
    {
      public short Hash;
      public short Value;
      public string Name;

      public EntityDef(short hash, short value, string name)
      {
        this.Hash = hash;
        this.Name = name;
        this.Value = value;
      }
    }
  }
}
