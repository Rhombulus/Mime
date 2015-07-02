// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Rtf.RTFData
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Rtf
{
  internal sealed class RTFData
  {
    public static short[] keywordHashTable = new short[299]
    {
      (short) 4,
      (short) 0,
      (short) 6,
      (short) 0,
      (short) 0,
      (short) 8,
      (short) 0,
      (short) 0,
      (short) 10,
      (short) 11,
      (short) 0,
      (short) 0,
      (short) 12,
      (short) 0,
      (short) 0,
      (short) 0,
      (short) 13,
      (short) 0,
      (short) 14,
      (short) 17,
      (short) 20,
      (short) 23,
      (short) 25,
      (short) 26,
      (short) 29,
      (short) 32,
      (short) 34,
      (short) 0,
      (short) 37,
      (short) 38,
      (short) 42,
      (short) 44,
      (short) 0,
      (short) 46,
      (short) 47,
      (short) 49,
      (short) 52,
      (short) 56,
      (short) 57,
      (short) 58,
      (short) 59,
      (short) 60,
      (short) 61,
      (short) 62,
      (short) 63,
      (short) 0,
      (short) 0,
      (short) 0,
      (short) 0,
      (short) 64,
      (short) 67,
      (short) 0,
      (short) 0,
      (short) 68,
      (short) 69,
      (short) 70,
      (short) 0,
      (short) 71,
      (short) 0,
      (short) 0,
      (short) 0,
      (short) 0,
      (short) 0,
      (short) 0,
      (short) 72,
      (short) 0,
      (short) 74,
      (short) 0,
      (short) 76,
      (short) 77,
      (short) 78,
      (short) 79,
      (short) 80,
      (short) 81,
      (short) 82,
      (short) 83,
      (short) 85,
      (short) 0,
      (short) 0,
      (short) 0,
      (short) 86,
      (short) 87,
      (short) 0,
      (short) 0,
      (short) 88,
      (short) 92,
      (short) 93,
      (short) 94,
      (short) 95,
      (short) 0,
      (short) 97,
      (short) 100,
      (short) 102,
      (short) 103,
      (short) 0,
      (short) 105,
      (short) 107,
      (short) 108,
      (short) 109,
      (short) 0,
      (short) 112,
      (short) 0,
      (short) 113,
      (short) 0,
      (short) 114,
      (short) 115,
      (short) 117,
      (short) 0,
      (short) 119,
      (short) 122,
      (short) 123,
      (short) 125,
      (short) 128,
      (short) 0,
      (short) 0,
      (short) 129,
      (short) 130,
      (short) 132,
      (short) 134,
      (short) 137,
      (short) 0,
      (short) 140,
      (short) 141,
      (short) 145,
      (short) 147,
      (short) 149,
      (short) 0,
      (short) 0,
      (short) 0,
      (short) 151,
      (short) 0,
      (short) 152,
      (short) 0,
      (short) 155,
      (short) 0,
      (short) 156,
      (short) 157,
      (short) 158,
      (short) 160,
      (short) 162,
      (short) 163,
      (short) 0,
      (short) 164,
      (short) 165,
      (short) 167,
      (short) 168,
      (short) 169,
      (short) 170,
      (short) 171,
      (short) 174,
      (short) 0,
      (short) 177,
      (short) 178,
      (short) 0,
      (short) 179,
      (short) 180,
      (short) 181,
      (short) 0,
      (short) 182,
      (short) 183,
      (short) 0,
      (short) 184,
      (short) 186,
      (short) 0,
      (short) 187,
      (short) 191,
      (short) 0,
      (short) 0,
      (short) 192,
      (short) 0,
      (short) 194,
      (short) 0,
      (short) 196,
      (short) 199,
      (short) 201,
      (short) 0,
      (short) 204,
      (short) 205,
      (short) 206,
      (short) 0,
      (short) 208,
      (short) 209,
      (short) 210,
      (short) 212,
      (short) 0,
      (short) 213,
      (short) 214,
      (short) 216,
      (short) 218,
      (short) 0,
      (short) 0,
      (short) 220,
      (short) 0,
      (short) 222,
      (short) 226,
      (short) 228,
      (short) 230,
      (short) 232,
      (short) 234,
      (short) 235,
      (short) 0,
      (short) 236,
      (short) 0,
      (short) 0,
      (short) 238,
      (short) 0,
      (short) 240,
      (short) 243,
      (short) 0,
      (short) 0,
      (short) 244,
      (short) 0,
      (short) 247,
      (short) 249,
      (short) 250,
      (short) 251,
      (short) 252,
      (short) 0,
      (short) 253,
      (short) 0,
      (short) 0,
      (short) 0,
      (short) 254,
      (short) 256,
      (short) 0,
      (short) 0,
      (short) 258,
      (short) 259,
      (short) 0,
      (short) 260,
      (short) 263,
      (short) 0,
      (short) 0,
      (short) 0,
      (short) 264,
      (short) 0,
      (short) 265,
      (short) 0,
      (short) 0,
      (short) 0,
      (short) 0,
      (short) 266,
      (short) 268,
      (short) 0,
      (short) 271,
      (short) 272,
      (short) 273,
      (short) 0,
      (short) 0,
      (short) 275,
      (short) 276,
      (short) 0,
      (short) 277,
      (short) 0,
      (short) 280,
      (short) 0,
      (short) 282,
      (short) 0,
      (short) 284,
      (short) 285,
      (short) 286,
      (short) 288,
      (short) 289,
      (short) 290,
      (short) 0,
      (short) 0,
      (short) 291,
      (short) 295,
      (short) 0,
      (short) 296,
      (short) 297,
      (short) 298,
      (short) 300,
      (short) 301,
      (short) 302,
      (short) 0,
      (short) 305,
      (short) 307,
      (short) 309,
      (short) 311,
      (short) 0,
      (short) 312,
      (short) 313,
      (short) 0,
      (short) 314,
      (short) 316,
      (short) 317,
      (short) 319,
      (short) 320,
      (short) 321,
      (short) 324,
      (short) 325,
      (short) 0,
      (short) 326,
      (short) 327,
      (short) 328,
      (short) 329,
      (short) 330,
      (short) 0
    };
    public static RTFData.KeyDef[] keywords = new RTFData.KeyDef[332]
    {
      new RTFData.KeyDef((short) -1, (short) 0, char.MinValue, (short) 0, false, "null"),
      new RTFData.KeyDef((short) -1, (short) 0, char.MinValue, (short) 0, true, "null"),
      new RTFData.KeyDef((short) -1, (short) 0, char.MinValue, (short) 0, false, "null"),
      new RTFData.KeyDef((short) -1, (short) 0, char.MinValue, (short) 0, false, "null"),
      new RTFData.KeyDef((short) 0, (short) 0, char.MinValue, (short) 0, false, "aul"),
      new RTFData.KeyDef((short) 0, (short) 2, char.MinValue, (short) -1, false, "ulw"),
      new RTFData.KeyDef((short) 2, (short) 0, char.MinValue, (short) 0, false, "pichgoal"),
      new RTFData.KeyDef((short) 2, (short) 7, char.MinValue, (short) 0, false, "trbrdrb"),
      new RTFData.KeyDef((short) 5, (short) 0, char.MinValue, (short) 0, false, "leveltext"),
      new RTFData.KeyDef((short) 5, (short) 0, char.MinValue, (short) 0, false, "listlevel"),
      new RTFData.KeyDef((short) 8, (short) 8, char.MinValue, (short) 0, false, "trbrdrh"),
      new RTFData.KeyDef((short) 9, (short) 5, char.MinValue, (short) 0, false, "brdrengrave"),
      new RTFData.KeyDef((short) 12, (short) 4, char.MinValue, (short) 0, false, "trbrdrl"),
      new RTFData.KeyDef((short) 16, (short) 0, char.MinValue, (short) 0, false, "irow"),
      new RTFData.KeyDef((short) 18, (short) 4, char.MinValue, (short) 0, false, "brdrtriple"),
      new RTFData.KeyDef((short) 18, (short) 0, char.MinValue, (short) 0, false, "footer"),
      new RTFData.KeyDef((short) 18, (short) 6, char.MinValue, (short) 0, false, "trbrdrr"),
      new RTFData.KeyDef((short) 19, (short) 0, char.MinValue, (short) -1, false, "caps"),
      new RTFData.KeyDef((short) 19, (short) 4, char.MinValue, (short) 0, true, "fscript"),
      new RTFData.KeyDef((short) 19, (short) 5, char.MinValue, (short) -1, false, "uldash"),
      new RTFData.KeyDef((short) 20, (short) 0, char.MinValue, (short) 0, false, "expndtw"),
      new RTFData.KeyDef((short) 20, (short) 6, char.MinValue, (short) 0, false, "pnucrm"),
      new RTFData.KeyDef((short) 20, (short) 5, char.MinValue, (short) 0, false, "trbrdrt"),
      new RTFData.KeyDef((short) 21, (short) 4, char.MinValue, (short) 0, false, "brdrwavydb"),
      new RTFData.KeyDef((short) 21, (short) 0, char.MinValue, (short) 0, false, "header"),
      new RTFData.KeyDef((short) 22, (short) 9, char.MinValue, (short) 0, false, "trbrdrv"),
      new RTFData.KeyDef((short) 23, (short) 0, char.MinValue, (short) -1, false, "embo"),
      new RTFData.KeyDef((short) 23, (short) 0, char.MinValue, (short) 0, false, "pnindent"),
      new RTFData.KeyDef((short) 23, (short) 0, '\x200D', (short) 0, false, "zwj"),
      new RTFData.KeyDef((short) 24, (short) 0, char.MinValue, (short) 0, false, "field"),
      new RTFData.KeyDef((short) 24, (short) 0, char.MinValue, (short) 0, true, "fnil"),
      new RTFData.KeyDef((short) 24, (short) 0, char.MinValue, (short) -1, false, "link"),
      new RTFData.KeyDef((short) 25, (short) 0, char.MinValue, (short) -1, false, "disabled"),
      new RTFData.KeyDef((short) 25, (short) 0, char.MinValue, (short) 0, false, "footnote"),
      new RTFData.KeyDef((short) 26, (short) 0, char.MinValue, (short) 0, true, "fcharset"),
      new RTFData.KeyDef((short) 26, (short) 0, char.MinValue, (short) 0, true, "mac"),
      new RTFData.KeyDef((short) 26, (short) 4, char.MinValue, (short) 0, false, "pnucltr"),
      new RTFData.KeyDef((short) 28, (short) 0, char.MinValue, (short) 0, true, "fbidis"),
      new RTFData.KeyDef((short) 29, (short) 0, '‘', (short) 0, false, "lquote"),
      new RTFData.KeyDef((short) 29, (short) 0, char.MinValue, (short) 0, false, "macpict"),
      new RTFData.KeyDef((short) 29, (short) 0, char.MinValue, (short) 0, false, "row"),
      new RTFData.KeyDef((short) 29, (short) 1, char.MinValue, (short) 0, false, "rtlrow"),
      new RTFData.KeyDef((short) 30, (short) 0, char.MinValue, (short) 0, true, "fprq"),
      new RTFData.KeyDef((short) 30, (short) 0, char.MinValue, (short) 0, false, "picprop"),
      new RTFData.KeyDef((short) 31, (short) 0, char.MinValue, (short) 0, false, "levelstartat"),
      new RTFData.KeyDef((short) 31, (short) 0, char.MinValue, (short) 0, false, "pich"),
      new RTFData.KeyDef((short) 33, (short) 3, char.MinValue, (short) 0, false, "brdrwavy"),
      new RTFData.KeyDef((short) 34, (short) 0, char.MinValue, (short) 0, true, "bin"),
      new RTFData.KeyDef((short) 34, (short) 0, char.MinValue, (short) 0, false, "line"),
      new RTFData.KeyDef((short) 35, (short) 3, char.MinValue, (short) 0, true, "fmodern"),
      new RTFData.KeyDef((short) 35, (short) 0, char.MinValue, (short) 0, false, "pict"),
      new RTFData.KeyDef((short) 35, (short) 1, char.MinValue, (short) 0, false, "pnlvlblt"),
      new RTFData.KeyDef((short) 36, (short) 2, char.MinValue, (short) 0, false, "brdrdashsm"),
      new RTFData.KeyDef((short) 36, (short) 0, char.MinValue, (short) 0, false, "clcfpat"),
      new RTFData.KeyDef((short) 36, (short) 0, char.MinValue, (short) 0, false, "list"),
      new RTFData.KeyDef((short) 36, (short) 0, char.MinValue, (short) 0, false, "nestrow"),
      new RTFData.KeyDef((short) 37, (short) 0, char.MinValue, (short) 0, false, "brdrbtw"),
      new RTFData.KeyDef((short) 38, (short) 0, char.MinValue, (short) 0, false, "picw"),
      new RTFData.KeyDef((short) 39, (short) 0, char.MinValue, (short) 0, false, "cbpat"),
      new RTFData.KeyDef((short) 40, (short) 0, '\x200E', (short) 0, false, "rtlmark"),
      new RTFData.KeyDef((short) 41, (short) 0, char.MinValue, (short) 0, true, "deflang"),
      new RTFData.KeyDef((short) 42, (short) 12, char.MinValue, (short) -1, false, "ulhwave"),
      new RTFData.KeyDef((short) 43, (short) 0, char.MinValue, (short) 0, false, "pnaiud"),
      new RTFData.KeyDef((short) 44, (short) 13, char.MinValue, (short) -1, false, "ulldash"),
      new RTFData.KeyDef((short) 49, (short) 1, char.MinValue, (short) 0, false, "brdrdashdotstr"),
      new RTFData.KeyDef((short) 49, (short) 0, char.MinValue, (short) 0, false, "nesttableprops"),
      new RTFData.KeyDef((short) 49, (short) 0, char.MinValue, (short) 0, false, "trleft"),
      new RTFData.KeyDef((short) 50, (short) 2, char.MinValue, (short) 0, false, "bghoriz"),
      new RTFData.KeyDef((short) 53, (short) 0, char.MinValue, (short) 0, false, "par"),
      new RTFData.KeyDef((short) 54, (short) 0, char.MinValue, (short) 0, false, "keepn"),
      new RTFData.KeyDef((short) 55, (short) 0, char.MinValue, (short) 0, false, "pnordt"),
      new RTFData.KeyDef((short) 57, (short) 0, char.MinValue, (short) 0, true, "lang"),
      new RTFData.KeyDef((short) 64, (short) 6, char.MinValue, (short) 0, true, "fbidi"),
      new RTFData.KeyDef((short) 64, (short) 0, char.MinValue, (short) 0, false, "lastrow"),
      new RTFData.KeyDef((short) 66, (short) 0, '•', (short) 0, false, "bullet"),
      new RTFData.KeyDef((short) 66, (short) 0, char.MinValue, (short) 0, false, "sectd"),
      new RTFData.KeyDef((short) 68, (short) 1, char.MinValue, (short) -1, false, "ul"),
      new RTFData.KeyDef((short) 69, (short) 3, char.MinValue, (short) 0, false, "pnlcltr"),
      new RTFData.KeyDef((short) 70, (short) 0, char.MinValue, (short) 0, false, "clvmrg"),
      new RTFData.KeyDef((short) 71, (short) 0, char.MinValue, (short) -1, false, "shad"),
      new RTFData.KeyDef((short) 72, (short) 2, char.MinValue, (short) 0, false, "brdrdash"),
      new RTFData.KeyDef((short) 73, (short) 0, char.MinValue, (short) 1, true, "uc"),
      new RTFData.KeyDef((short) 74, (short) 0, char.MinValue, (short) 0, false, "highlight"),
      new RTFData.KeyDef((short) 75, (short) 0, char.MinValue, (short) 0, false, "htmlbase"),
      new RTFData.KeyDef((short) 75, (short) 0, char.MinValue, (short) 0, false, "pncnum"),
      new RTFData.KeyDef((short) 76, (short) 0, char.MinValue, (short) 0, false, "ud"),
      new RTFData.KeyDef((short) 80, (short) 0, char.MinValue, (short) 0, false, "pnstart"),
      new RTFData.KeyDef((short) 81, (short) 0, char.MinValue, (short) 0, true, "adeff"),
      new RTFData.KeyDef((short) 84, (short) 0, char.MinValue, (short) 0, false, "blue"),
      new RTFData.KeyDef((short) 84, (short) 4, char.MinValue, (short) 0, false, "brdrdb"),
      new RTFData.KeyDef((short) 84, (short) 4, char.MinValue, (short) 0, false, "brdrframe"),
      new RTFData.KeyDef((short) 84, (short) 0, char.MinValue, (short) 0, false, "taprtl"),
      new RTFData.KeyDef((short) 85, (short) 0, char.MinValue, (short) 0, false, "comment"),
      new RTFData.KeyDef((short) 86, (short) 1, char.MinValue, (short) 0, true, "froman"),
      new RTFData.KeyDef((short) 87, (short) 5, char.MinValue, (short) 0, true, "fdecor"),
      new RTFData.KeyDef((short) 88, (short) 4, char.MinValue, (short) 0, true, "dbch"),
      new RTFData.KeyDef((short) 88, (short) 1, char.MinValue, (short) 0, false, "up"),
      new RTFData.KeyDef((short) 90, (short) 8, char.MinValue, (short) 0, false, "brdroutset"),
      new RTFData.KeyDef((short) 90, (short) 2, char.MinValue, (short) 0, false, "clvertalb"),
      new RTFData.KeyDef((short) 90, (short) 0, char.MinValue, (short) -1, false, "striked"),
      new RTFData.KeyDef((short) 91, (short) 1, char.MinValue, (short) 0, false, "clvertalc"),
      new RTFData.KeyDef((short) 91, (short) 0, char.MinValue, (short) 1, false, "itap"),
      new RTFData.KeyDef((short) 92, (short) 0, char.MinValue, (short) 0, false, "pnlvl"),
      new RTFData.KeyDef((short) 93, (short) 0, char.MinValue, (short) 0, false, "clftsWidth"),
      new RTFData.KeyDef((short) 93, (short) 0, char.MinValue, (short) 0, false, "upr"),
      new RTFData.KeyDef((short) 95, (short) 0, char.MinValue, (short) 0, false, "nestcell"),
      new RTFData.KeyDef((short) 95, (short) 0, char.MinValue, (short) 0, false, "pnord"),
      new RTFData.KeyDef((short) 96, (short) 0, char.MinValue, (short) -1, false, "protect"),
      new RTFData.KeyDef((short) 97, (short) 0, char.MinValue, (short) 0, true, "pc"),
      new RTFData.KeyDef((short) 98, (short) 0, char.MinValue, (short) -1, true, "b"),
      new RTFData.KeyDef((short) 98, (short) 0, char.MinValue, (short) 0, false, "deftab"),
      new RTFData.KeyDef((short) 98, (short) 3, char.MinValue, (short) 0, false, "qj"),
      new RTFData.KeyDef((short) 100, (short) 0, char.MinValue, (short) 0, false, "ql"),
      new RTFData.KeyDef((short) 102, (short) 0, char.MinValue, (short) -1, true, "f"),
      new RTFData.KeyDef((short) 104, (short) 0, char.MinValue, (short) 0, false, "sp"),
      new RTFData.KeyDef((short) 105, (short) 0, char.MinValue, (short) -1, true, "i"),
      new RTFData.KeyDef((short) 105, (short) 1, char.MinValue, (short) 0, false, "qc"),
      new RTFData.KeyDef((short) 106, (short) 0, char.MinValue, (short) -1, false, "revised"),
      new RTFData.KeyDef((short) 106, (short) 0, char.MinValue, (short) 0, false, "trpaddr"),
      new RTFData.KeyDef((short) 108, (short) 0, char.MinValue, (short) 0, false, "cell"),
      new RTFData.KeyDef((short) 108, (short) 4, char.MinValue, (short) 0, false, "qd"),
      new RTFData.KeyDef((short) 108, (short) 0, char.MinValue, (short) 0, false, "trpaddt"),
      new RTFData.KeyDef((short) 109, (short) 4, char.MinValue, (short) 0, false, "brdrthtnlg"),
      new RTFData.KeyDef((short) 110, (short) 0, char.MinValue, (short) 0, false, "pn"),
      new RTFData.KeyDef((short) 110, (short) 0, char.MinValue, (short) 0, false, "sv"),
      new RTFData.KeyDef((short) 111, (short) 0, char.MinValue, (short) 0, false, "brsp"),
      new RTFData.KeyDef((short) 111, (short) 0, char.MinValue, (short) 0, false, "tab"),
      new RTFData.KeyDef((short) 111, (short) 0, char.MinValue, (short) 0, false, "trgaph"),
      new RTFData.KeyDef((short) 112, (short) 0, char.MinValue, (short) 0, false, "shp"),
      new RTFData.KeyDef((short) 115, (short) 0, char.MinValue, (short) 0, false, "s"),
      new RTFData.KeyDef((short) 116, (short) 0, char.MinValue, (short) 0, false, "sl"),
      new RTFData.KeyDef((short) 116, (short) 0, char.MinValue, (short) 0, false, "trpaddl"),
      new RTFData.KeyDef((short) 117, (short) 4, char.MinValue, (short) 0, false, "brdrthtnmg"),
      new RTFData.KeyDef((short) 117, (short) 0, char.MinValue, (short) 0, true, "u"),
      new RTFData.KeyDef((short) 118, (short) 0, char.MinValue, (short) 0, true, "ltrch"),
      new RTFData.KeyDef((short) 118, (short) 0, char.MinValue, (short) 0, false, "sn"),
      new RTFData.KeyDef((short) 118, (short) 0, char.MinValue, (short) -1, false, "v"),
      new RTFData.KeyDef((short) 119, (short) 3, char.MinValue, (short) 0, true, "hich"),
      new RTFData.KeyDef((short) 119, (short) 0, char.MinValue, (short) 0, false, "ri"),
      new RTFData.KeyDef((short) 119, (short) 0, char.MinValue, (short) 0, false, "sa"),
      new RTFData.KeyDef((short) 121, (short) 5, char.MinValue, (short) 0, false, "qs"),
      new RTFData.KeyDef((short) 122, (short) 2, char.MinValue, (short) 0, false, "qr"),
      new RTFData.KeyDef((short) 122, (short) 0, char.MinValue, (short) 0, false, "sb"),
      new RTFData.KeyDef((short) 122, (short) 0, char.MinValue, (short) 0, false, "trcbpat"),
      new RTFData.KeyDef((short) 122, (short) 0, char.MinValue, (short) 0, false, "trpaddb"),
      new RTFData.KeyDef((short) 123, (short) 0, char.MinValue, (short) 0, false, "cfpat"),
      new RTFData.KeyDef((short) 123, (short) 0, char.MinValue, (short) 0, false, "keep"),
      new RTFData.KeyDef((short) 124, (short) 1, char.MinValue, (short) 0, false, "bgvert"),
      new RTFData.KeyDef((short) 124, (short) 2, char.MinValue, (short) 0, false, "red"),
      new RTFData.KeyDef((short) 125, (short) 0, char.MinValue, (short) 0, true, "deflangfe"),
      new RTFData.KeyDef((short) 125, (short) 11, char.MinValue, (short) -1, false, "ululdbwave"),
      new RTFData.KeyDef((short) 129, (short) 0, char.MinValue, (short) 0, false, "trrh"),
      new RTFData.KeyDef((short) 131, (short) 0, char.MinValue, (short) 0, false, "_hyphen"),
      new RTFData.KeyDef((short) 131, (short) 0, char.MinValue, (short) 1, false, "htmlrtf"),
      new RTFData.KeyDef((short) 131, (short) 0, char.MinValue, (short) 0, false, "picwgoal"),
      new RTFData.KeyDef((short) 133, (short) 7, char.MinValue, (short) -1, false, "uldashdd"),
      new RTFData.KeyDef((short) 135, (short) 4, char.MinValue, (short) 0, false, "brdrtnthsg"),
      new RTFData.KeyDef((short) 136, (short) 0, char.MinValue, (short) 0, false, "objattph"),
      new RTFData.KeyDef((short) 137, (short) 9, char.MinValue, (short) 0, false, "bgdkbdiag"),
      new RTFData.KeyDef((short) 137, (short) 3, char.MinValue, (short) -1, false, "uldb"),
      new RTFData.KeyDef((short) 138, (short) 0, char.MinValue, (short) 0, false, "clmrg"),
      new RTFData.KeyDef((short) 138, (short) 0, char.MinValue, (short) 0, false, "clpadr"),
      new RTFData.KeyDef((short) 139, (short) 0, char.MinValue, (short) -1, false, "outl"),
      new RTFData.KeyDef((short) 140, (short) 0, char.MinValue, (short) 0, false, "clpadt"),
      new RTFData.KeyDef((short) 142, (short) 0, char.MinValue, (short) 0, true, "fcs"),
      new RTFData.KeyDef((short) 143, (short) 0, char.MinValue, (short) 0, true, "ansicpg"),
      new RTFData.KeyDef((short) 143, (short) 0, char.MinValue, (short) 0, false, "shpinst"),
      new RTFData.KeyDef((short) 144, (short) 0, char.MinValue, (short) 0, false, "brdrcf"),
      new RTFData.KeyDef((short) 145, (short) 0, char.MinValue, (short) 0, false, "sect"),
      new RTFData.KeyDef((short) 146, (short) 0, char.MinValue, (short) 0, true, "afs"),
      new RTFData.KeyDef((short) 147, (short) 0, char.MinValue, (short) 0, true, "plain"),
      new RTFData.KeyDef((short) 148, (short) 7, char.MinValue, (short) 0, false, "brdrinset"),
      new RTFData.KeyDef((short) 148, (short) 0, char.MinValue, (short) 0, false, "clpadl"),
      new RTFData.KeyDef((short) 148, (short) 0, char.MinValue, (short) 0, false, "listid"),
      new RTFData.KeyDef((short) 149, (short) 0, char.MinValue, (short) 0, false, "acf"),
      new RTFData.KeyDef((short) 149, (short) 0, char.MinValue, (short) 0, true, "fonttbl"),
      new RTFData.KeyDef((short) 149, (short) 1, char.MinValue, (short) 0, false, "rtlpar"),
      new RTFData.KeyDef((short) 151, (short) 0, char.MinValue, (short) 0, false, "htmltag"),
      new RTFData.KeyDef((short) 152, (short) 15, char.MinValue, (short) -1, false, "ulthdashd"),
      new RTFData.KeyDef((short) 154, (short) 0, char.MinValue, (short) 0, false, "clpadb"),
      new RTFData.KeyDef((short) 155, (short) 0, char.MinValue, (short) -1, false, "scaps"),
      new RTFData.KeyDef((short) 156, (short) 12, char.MinValue, (short) 0, false, "clbrdrr"),
      new RTFData.KeyDef((short) 158, (short) 0, char.MinValue, (short) 0, false, "shading"),
      new RTFData.KeyDef((short) 159, (short) 2, char.MinValue, (short) 0, false, "trqr"),
      new RTFData.KeyDef((short) 161, (short) 0, char.MinValue, (short) 0, false, "pard"),
      new RTFData.KeyDef((short) 161, (short) 0, char.MinValue, (short) 0, true, "pnfs"),
      new RTFData.KeyDef((short) 162, (short) 11, char.MinValue, (short) 0, false, "clbrdrt"),
      new RTFData.KeyDef((short) 164, (short) 4, char.MinValue, (short) 0, false, "bgdkvert"),
      new RTFData.KeyDef((short) 164, (short) 0, char.MinValue, (short) 0, false, "brdrl"),
      new RTFData.KeyDef((short) 164, (short) 0, '\x200F', (short) 0, false, "ltrmark"),
      new RTFData.KeyDef((short) 164, (short) 18, char.MinValue, (short) -1, false, "ulthldash"),
      new RTFData.KeyDef((short) 165, (short) 0, char.MinValue, (short) 0, false, "intbl"),
      new RTFData.KeyDef((short) 168, (short) 12, char.MinValue, (short) 0, false, "bgbdiag"),
      new RTFData.KeyDef((short) 168, (short) 0, char.MinValue, (short) 0, false, "bkmkstart"),
      new RTFData.KeyDef((short) 170, (short) 3, char.MinValue, (short) 0, false, "brdrb"),
      new RTFData.KeyDef((short) 170, (short) 10, char.MinValue, (short) 0, false, "clbrdrl"),
      new RTFData.KeyDef((short) 172, (short) 13, char.MinValue, (short) 0, false, "clbrdrb"),
      new RTFData.KeyDef((short) 172, (short) 0, char.MinValue, (short) 0, false, "pagebb"),
      new RTFData.KeyDef((short) 172, (short) 0, char.MinValue, (short) -1, false, "strike"),
      new RTFData.KeyDef((short) 173, (short) 0, char.MinValue, (short) 0, false, "clvmgf"),
      new RTFData.KeyDef((short) 173, (short) 0, char.MinValue, (short) 0, false, "trowd"),
      new RTFData.KeyDef((short) 174, (short) 0, char.MinValue, (short) 0, false, "info"),
      new RTFData.KeyDef((short) 174, (short) 0, '“', (short) 0, false, "ldblquote"),
      new RTFData.KeyDef((short) 174, (short) 0, char.MinValue, (short) 0, false, "listoverride"),
      new RTFData.KeyDef((short) 176, (short) 1, char.MinValue, (short) 0, false, "trqc"),
      new RTFData.KeyDef((short) 177, (short) 0, char.MinValue, (short) 0, false, "ilvl"),
      new RTFData.KeyDef((short) 178, (short) 0, char.MinValue, (short) 0, false, "li"),
      new RTFData.KeyDef((short) 178, (short) 0, '\x200C', (short) 0, false, "zwnj"),
      new RTFData.KeyDef((short) 180, (short) 0, char.MinValue, (short) 0, false, "listsimple"),
      new RTFData.KeyDef((short) 181, (short) 2, char.MinValue, (short) 0, false, "brdrdashdd"),
      new RTFData.KeyDef((short) 182, (short) 0, char.MinValue, (short) 0, true, "fname"),
      new RTFData.KeyDef((short) 182, (short) 0, char.MinValue, (short) 0, false, "ulnone"),
      new RTFData.KeyDef((short) 183, (short) 0, char.MinValue, (short) 0, false, "bkmkend"),
      new RTFData.KeyDef((short) 185, (short) 0, char.MinValue, (short) 0, false, "clwWidth"),
      new RTFData.KeyDef((short) 186, (short) 0, char.MinValue, (short) 0, true, "adeflang"),
      new RTFData.KeyDef((short) 186, (short) 2, char.MinValue, (short) 0, false, "brdrr"),
      new RTFData.KeyDef((short) 187, (short) 3, char.MinValue, (short) 0, false, "brdrs"),
      new RTFData.KeyDef((short) 187, (short) 0, char.MinValue, (short) 0, false, "pniroha"),
      new RTFData.KeyDef((short) 188, (short) 1, char.MinValue, (short) 0, false, "brdrt"),
      new RTFData.KeyDef((short) 188, (short) 0, char.MinValue, (short) 0, false, "ls"),
      new RTFData.KeyDef((short) 191, (short) 0, char.MinValue, (short) 0, false, "brdrw"),
      new RTFData.KeyDef((short) 191, (short) 0, char.MinValue, (short) 0, false, "irowband"),
      new RTFData.KeyDef((short) 193, (short) 2, char.MinValue, (short) 0, true, "loch"),
      new RTFData.KeyDef((short) 193, (short) 0, char.MinValue, (short) -1, true, "pnf"),
      new RTFData.KeyDef((short) 193, (short) 0, char.MinValue, (short) 0, false, "slmult"),
      new RTFData.KeyDef((short) 193, (short) 14, char.MinValue, (short) -1, false, "ulthdash"),
      new RTFData.KeyDef((short) 194, (short) 7, char.MinValue, (short) 0, false, "bgdkdcross"),
      new RTFData.KeyDef((short) 194, (short) 0, char.MinValue, (short) 0, false, "pntxta"),
      new RTFData.KeyDef((short) 195, (short) 0, char.MinValue, (short) 0, false, "cellx"),
      new RTFData.KeyDef((short) 195, (short) 0, char.MinValue, (short) 0, false, "pnlvlcont"),
      new RTFData.KeyDef((short) 196, (short) 0, char.MinValue, (short) 0, false, "headerl"),
      new RTFData.KeyDef((short) 196, (short) 0, char.MinValue, (short) 0, false, "jclisttab"),
      new RTFData.KeyDef((short) 197, (short) 0, '–', (short) 0, false, "endash"),
      new RTFData.KeyDef((short) 197, (short) 0, char.MinValue, (short) 0, false, "pntxtb"),
      new RTFData.KeyDef((short) 198, (short) 3, char.MinValue, (short) 0, false, "bgfdiag"),
      new RTFData.KeyDef((short) 199, (short) 0, char.MinValue, (short) 0, false, "brdrbar"),
      new RTFData.KeyDef((short) 201, (short) 6, char.MinValue, (short) 0, false, "bgdkfdiag"),
      new RTFData.KeyDef((short) 201, (short) 0, char.MinValue, (short) 0, false, "trwWidth"),
      new RTFData.KeyDef((short) 204, (short) 0, char.MinValue, (short) 0, true, "alang"),
      new RTFData.KeyDef((short) 204, (short) 4, char.MinValue, (short) 0, false, "brdrtnthtnlg"),
      new RTFData.KeyDef((short) 206, (short) 0, char.MinValue, (short) 0, true, "cpg"),
      new RTFData.KeyDef((short) 206, (short) 0, char.MinValue, (short) 0, false, "headerf"),
      new RTFData.KeyDef((short) 206, (short) 0, char.MinValue, (short) 0, false, "ltrrow"),
      new RTFData.KeyDef((short) 207, (short) 4, char.MinValue, (short) 0, false, "brdrtnthtnsg"),
      new RTFData.KeyDef((short) 210, (short) 5, char.MinValue, (short) 0, false, "bgdkhoriz"),
      new RTFData.KeyDef((short) 210, (short) 0, char.MinValue, (short) 0, false, "dropcapt"),
      new RTFData.KeyDef((short) 210, (short) 0, char.MinValue, (short) 0, false, "listtable"),
      new RTFData.KeyDef((short) 212, (short) 4, char.MinValue, (short) 0, false, "brdrtnthtnmg"),
      new RTFData.KeyDef((short) 212, (short) 5, char.MinValue, (short) 0, false, "pnlcrm"),
      new RTFData.KeyDef((short) 213, (short) 0, char.MinValue, (short) 0, false, "pndecd"),
      new RTFData.KeyDef((short) 214, (short) 0, char.MinValue, (short) 0, false, "leveljc"),
      new RTFData.KeyDef((short) 215, (short) 4, char.MinValue, (short) 0, false, "brdrtnthmg"),
      new RTFData.KeyDef((short) 216, (short) 0, char.MinValue, (short) 0, false, "colortbl"),
      new RTFData.KeyDef((short) 218, (short) 0, char.MinValue, (short) 0, false, "headerr"),
      new RTFData.KeyDef((short) 222, (short) 3, char.MinValue, (short) 0, false, "brdrth"),
      new RTFData.KeyDef((short) 222, (short) 0, char.MinValue, (short) 0, false, "levelpicture"),
      new RTFData.KeyDef((short) 223, (short) 4, char.MinValue, (short) 0, false, "brdrtnthlg"),
      new RTFData.KeyDef((short) 223, (short) 0, char.MinValue, (short) 0, false, "listtext"),
      new RTFData.KeyDef((short) 226, (short) 0, char.MinValue, (short) 0, false, "footerr"),
      new RTFData.KeyDef((short) 227, (short) 0, char.MinValue, (short) 0, false, "clmgf"),
      new RTFData.KeyDef((short) 229, (short) 10, char.MinValue, (short) 0, false, "bgdcross"),
      new RTFData.KeyDef((short) 229, (short) 4, char.MinValue, (short) 0, false, "brdrthtnsg"),
      new RTFData.KeyDef((short) 229, (short) 0, char.MinValue, (short) -1, false, "sub"),
      new RTFData.KeyDef((short) 230, (short) 0, '”', (short) 0, false, "rdblquote"),
      new RTFData.KeyDef((short) 234, (short) 0, char.MinValue, (short) 0, false, "nosupersub"),
      new RTFData.KeyDef((short) 236, (short) 0, char.MinValue, (short) 0, true, "fs"),
      new RTFData.KeyDef((short) 241, (short) 7, char.MinValue, (short) 0, true, "ftech"),
      new RTFData.KeyDef((short) 241, (short) 0, char.MinValue, (short) 0, true, "rtf"),
      new RTFData.KeyDef((short) 242, (short) 0, char.MinValue, (short) 0, true, "falt"),
      new RTFData.KeyDef((short) 242, (short) 0, char.MinValue, (short) 0, false, "fldrslt"),
      new RTFData.KeyDef((short) 242, (short) 0, char.MinValue, (short) 0, false, "ltrpar"),
      new RTFData.KeyDef((short) 244, (short) 0, char.MinValue, (short) 0, true, "urtf"),
      new RTFData.KeyDef((short) 245, (short) 0, char.MinValue, (short) 0, false, "pncard"),
      new RTFData.KeyDef((short) 246, (short) 0, char.MinValue, (short) 0, false, "footerf"),
      new RTFData.KeyDef((short) 246, (short) 2, char.MinValue, (short) 0, false, "pndec"),
      new RTFData.KeyDef((short) 249, (short) -1, char.MinValue, (short) 0, false, "dn"),
      new RTFData.KeyDef((short) 250, (short) 2, char.MinValue, (short) 0, false, "brdrdashd"),
      new RTFData.KeyDef((short) 252, (short) 0, char.MinValue, (short) 0, false, "footerl"),
      new RTFData.KeyDef((short) 252, (short) 6, char.MinValue, (short) -1, false, "uldashd"),
      new RTFData.KeyDef((short) 252, (short) 8, char.MinValue, (short) -1, false, "ulwave"),
      new RTFData.KeyDef((short) 254, (short) 0, char.MinValue, (short) 0, true, "deff"),
      new RTFData.KeyDef((short) 254, (short) 0, char.MinValue, (short) 0, true, "langfe"),
      new RTFData.KeyDef((short) 256, (short) 8, char.MinValue, (short) 0, false, "bgdkcross"),
      new RTFData.KeyDef((short) 256, (short) 0, char.MinValue, (short) 0, false, "nonesttables"),
      new RTFData.KeyDef((short) 258, (short) 0, char.MinValue, (short) 0, false, "fi"),
      new RTFData.KeyDef((short) 259, (short) 0, char.MinValue, (short) 0, false, "chcbpat"),
      new RTFData.KeyDef((short) 260, (short) 0, '’', (short) 0, false, "rquote"),
      new RTFData.KeyDef((short) 260, (short) 1, char.MinValue, (short) 0, true, "rtlch"),
      new RTFData.KeyDef((short) 261, (short) 0, char.MinValue, (short) 0, false, "pndbnum"),
      new RTFData.KeyDef((short) 262, (short) 3, char.MinValue, (short) 0, false, "brdrsh"),
      new RTFData.KeyDef((short) 263, (short) 0, char.MinValue, (short) 0, false, "pnaiu"),
      new RTFData.KeyDef((short) 266, (short) 0, char.MinValue, (short) -1, true, "ai"),
      new RTFData.KeyDef((short) 266, (short) 0, char.MinValue, (short) 0, true, "fromhtml"),
      new RTFData.KeyDef((short) 266, (short) 0, char.MinValue, (short) 0, false, "trhdr"),
      new RTFData.KeyDef((short) 266, (short) 10, char.MinValue, (short) -1, false, "ulhair"),
      new RTFData.KeyDef((short) 267, (short) 0, '—', (short) 0, false, "emdash"),
      new RTFData.KeyDef((short) 269, (short) 0, char.MinValue, (short) 0, false, "levelnfc"),
      new RTFData.KeyDef((short) 270, (short) 0, char.MinValue, (short) -1, false, "deleted"),
      new RTFData.KeyDef((short) 271, (short) 0, char.MinValue, (short) 0, false, "dropcapli"),
      new RTFData.KeyDef((short) 271, (short) 9, char.MinValue, (short) -1, false, "ulth"),
      new RTFData.KeyDef((short) 272, (short) 0, char.MinValue, (short) 0, false, "mhtmltag"),
      new RTFData.KeyDef((short) 273, (short) 0, char.MinValue, (short) 0, false, "pmmetafile"),
      new RTFData.KeyDef((short) 274, (short) 0, char.MinValue, (short) -1, false, "impr"),
      new RTFData.KeyDef((short) 274, (short) 0, char.MinValue, (short) 0, true, "pca"),
      new RTFData.KeyDef((short) 274, (short) 0, char.MinValue, (short) 0, false, "pnirohad"),
      new RTFData.KeyDef((short) 276, (short) 0, char.MinValue, (short) 0, false, "cs"),
      new RTFData.KeyDef((short) 276, (short) 0, char.MinValue, (short) 0, false, "fldinst"),
      new RTFData.KeyDef((short) 277, (short) 0, char.MinValue, (short) -1, true, "ab"),
      new RTFData.KeyDef((short) 277, (short) 2, char.MinValue, (short) 0, true, "fswiss"),
      new RTFData.KeyDef((short) 278, (short) 1, char.MinValue, (short) 0, false, "green"),
      new RTFData.KeyDef((short) 278, (short) 17, char.MinValue, (short) -1, false, "ulthd"),
      new RTFData.KeyDef((short) 279, (short) 0, char.MinValue, (short) 0, false, "ulc"),
      new RTFData.KeyDef((short) 281, (short) 0, char.MinValue, (short) -1, true, "af"),
      new RTFData.KeyDef((short) 282, (short) 4, char.MinValue, (short) -1, false, "uld"),
      new RTFData.KeyDef((short) 284, (short) 11, char.MinValue, (short) 0, false, "bgcross"),
      new RTFData.KeyDef((short) 284, (short) 0, char.MinValue, (short) 0, false, "stylesheet"),
      new RTFData.KeyDef((short) 285, (short) 0, char.MinValue, (short) 0, false, "listoverridetable"),
      new RTFData.KeyDef((short) 286, (short) 0, char.MinValue, (short) 0, false, "background"),
      new RTFData.KeyDef((short) 286, (short) 0, char.MinValue, (short) 0, false, "clcbpat"),
      new RTFData.KeyDef((short) 287, (short) 0, char.MinValue, (short) 0, false, "pntext"),
      new RTFData.KeyDef((short) 288, (short) 0, char.MinValue, (short) 0, false, "trftsWidth"),
      new RTFData.KeyDef((short) 289, (short) 3, char.MinValue, (short) 0, false, "brdrhair"),
      new RTFData.KeyDef((short) 289, (short) 0, char.MinValue, (short) 0, false, "pnlvlbody"),
      new RTFData.KeyDef((short) 289, (short) 0, char.MinValue, (short) -1, false, "super"),
      new RTFData.KeyDef((short) 290, (short) 0, char.MinValue, (short) 0, false, "objdata"),
      new RTFData.KeyDef((short) 291, (short) 0, char.MinValue, (short) 0, true, "ansi"),
      new RTFData.KeyDef((short) 293, (short) 16, char.MinValue, (short) -1, false, "ulthdashdd"),
      new RTFData.KeyDef((short) 294, (short) 0, char.MinValue, (short) 0, false, "ulp"),
      new RTFData.KeyDef((short) 295, (short) 1, char.MinValue, (short) 0, false, "brdrdot"),
      new RTFData.KeyDef((short) 296, (short) 0, char.MinValue, (short) 0, true, "fromtext"),
      new RTFData.KeyDef((short) 297, (short) 6, char.MinValue, (short) 0, false, "brdremboss"),
      new RTFData.KeyDef((short) 297, (short) 0, char.MinValue, (short) 0, false, "cf")
    };
    public static int[] allowedCodePages = new int[32]
    {
      28591,
      437,
      708,
      720,
      850,
      852,
      860,
      862,
      863,
      864,
      865,
      866,
      874,
      932,
      936,
      949,
      950,
      1250,
      1251,
      1252,
      1253,
      1254,
      1255,
      1256,
      1257,
      1258,
      1361,
      10001,
      10002,
      10003,
      10008,
      65001
    };
    public const int ID__unknownKeyword = 0;
    public const int ID__ignorableDest = 1;
    public const int ID__formulaChar = 2;
    public const int ID__indexSubentry = 3;
    public const int ID_aul = 4;
    public const int ID_ulw = 5;
    public const int ID_pichgoal = 6;
    public const int ID_trbrdrb = 7;
    public const int ID_leveltext = 8;
    public const int ID_listlevel = 9;
    public const int ID_trbrdrh = 10;
    public const int ID_brdrengrave = 11;
    public const int ID_trbrdrl = 12;
    public const int ID_irow = 13;
    public const int ID_brdrtriple = 14;
    public const int ID_footer = 15;
    public const int ID_trbrdrr = 16;
    public const int ID_caps = 17;
    public const int ID_fscript = 18;
    public const int ID_uldash = 19;
    public const int ID_expndtw = 20;
    public const int ID_pnucrm = 21;
    public const int ID_trbrdrt = 22;
    public const int ID_brdrwavydb = 23;
    public const int ID_header = 24;
    public const int ID_trbrdrv = 25;
    public const int ID_embo = 26;
    public const int ID_pnindent = 27;
    public const int ID_zwj = 28;
    public const int ID_field = 29;
    public const int ID_fnil = 30;
    public const int ID_link = 31;
    public const int ID_disabled = 32;
    public const int ID_footnote = 33;
    public const int ID_fcharset = 34;
    public const int ID_mac = 35;
    public const int ID_pnucltr = 36;
    public const int ID_fbidis = 37;
    public const int ID_lquote = 38;
    public const int ID_macpict = 39;
    public const int ID_row = 40;
    public const int ID_rtlrow = 41;
    public const int ID_fprq = 42;
    public const int ID_picprop = 43;
    public const int ID_levelstartat = 44;
    public const int ID_pich = 45;
    public const int ID_brdrwavy = 46;
    public const int ID_bin = 47;
    public const int ID_line = 48;
    public const int ID_fmodern = 49;
    public const int ID_pict = 50;
    public const int ID_pnlvlblt = 51;
    public const int ID_brdrdashsm = 52;
    public const int ID_clcfpat = 53;
    public const int ID_list = 54;
    public const int ID_nestrow = 55;
    public const int ID_brdrbtw = 56;
    public const int ID_picw = 57;
    public const int ID_cbpat = 58;
    public const int ID_rtlmark = 59;
    public const int ID_deflang = 60;
    public const int ID_ulhwave = 61;
    public const int ID_pnaiud = 62;
    public const int ID_ulldash = 63;
    public const int ID_brdrdashdotstr = 64;
    public const int ID_nesttableprops = 65;
    public const int ID_trleft = 66;
    public const int ID_bghoriz = 67;
    public const int ID_par = 68;
    public const int ID_keepn = 69;
    public const int ID_pnordt = 70;
    public const int ID_lang = 71;
    public const int ID_fbidi = 72;
    public const int ID_lastrow = 73;
    public const int ID_bullet = 74;
    public const int ID_sectd = 75;
    public const int ID_ul = 76;
    public const int ID_pnlcltr = 77;
    public const int ID_clvmrg = 78;
    public const int ID_shad = 79;
    public const int ID_brdrdash = 80;
    public const int ID_uc = 81;
    public const int ID_highlight = 82;
    public const int ID_htmlbase = 83;
    public const int ID_pncnum = 84;
    public const int ID_ud = 85;
    public const int ID_pnstart = 86;
    public const int ID_adeff = 87;
    public const int ID_blue = 88;
    public const int ID_brdrdb = 89;
    public const int ID_brdrframe = 90;
    public const int ID_taprtl = 91;
    public const int ID_comment = 92;
    public const int ID_froman = 93;
    public const int ID_fdecor = 94;
    public const int ID_dbch = 95;
    public const int ID_up = 96;
    public const int ID_brdroutset = 97;
    public const int ID_clvertalb = 98;
    public const int ID_striked = 99;
    public const int ID_clvertalc = 100;
    public const int ID_itap = 101;
    public const int ID_pnlvl = 102;
    public const int ID_clftsWidth = 103;
    public const int ID_upr = 104;
    public const int ID_nestcell = 105;
    public const int ID_pnord = 106;
    public const int ID_protect = 107;
    public const int ID_pc = 108;
    public const int ID_b = 109;
    public const int ID_deftab = 110;
    public const int ID_qj = 111;
    public const int ID_ql = 112;
    public const int ID_f = 113;
    public const int ID_sp = 114;
    public const int ID_i = 115;
    public const int ID_qc = 116;
    public const int ID_revised = 117;
    public const int ID_trpaddr = 118;
    public const int ID_cell = 119;
    public const int ID_qd = 120;
    public const int ID_trpaddt = 121;
    public const int ID_brdrthtnlg = 122;
    public const int ID_pn = 123;
    public const int ID_sv = 124;
    public const int ID_brsp = 125;
    public const int ID_tab = 126;
    public const int ID_trgaph = 127;
    public const int ID_shp = 128;
    public const int ID_s = 129;
    public const int ID_sl = 130;
    public const int ID_trpaddl = 131;
    public const int ID_brdrthtnmg = 132;
    public const int ID_u = 133;
    public const int ID_ltrch = 134;
    public const int ID_sn = 135;
    public const int ID_v = 136;
    public const int ID_hich = 137;
    public const int ID_ri = 138;
    public const int ID_sa = 139;
    public const int ID_qs = 140;
    public const int ID_qr = 141;
    public const int ID_sb = 142;
    public const int ID_trcbpat = 143;
    public const int ID_trpaddb = 144;
    public const int ID_cfpat = 145;
    public const int ID_keep = 146;
    public const int ID_bgvert = 147;
    public const int ID_red = 148;
    public const int ID_deflangfe = 149;
    public const int ID_ululdbwave = 150;
    public const int ID_trrh = 151;
    public const int ID__hyphen = 152;
    public const int ID_htmlrtf = 153;
    public const int ID_picwgoal = 154;
    public const int ID_uldashdd = 155;
    public const int ID_brdrtnthsg = 156;
    public const int ID_objattph = 157;
    public const int ID_bgdkbdiag = 158;
    public const int ID_uldb = 159;
    public const int ID_clmrg = 160;
    public const int ID_clpadr = 161;
    public const int ID_outl = 162;
    public const int ID_clpadt = 163;
    public const int ID_fcs = 164;
    public const int ID_ansicpg = 165;
    public const int ID_shpinst = 166;
    public const int ID_brdrcf = 167;
    public const int ID_sect = 168;
    public const int ID_afs = 169;
    public const int ID_plain = 170;
    public const int ID_brdrinset = 171;
    public const int ID_clpadl = 172;
    public const int ID_listid = 173;
    public const int ID_acf = 174;
    public const int ID_fonttbl = 175;
    public const int ID_rtlpar = 176;
    public const int ID_htmltag = 177;
    public const int ID_ulthdashd = 178;
    public const int ID_clpadb = 179;
    public const int ID_scaps = 180;
    public const int ID_clbrdrr = 181;
    public const int ID_shading = 182;
    public const int ID_trqr = 183;
    public const int ID_pard = 184;
    public const int ID_pnfs = 185;
    public const int ID_clbrdrt = 186;
    public const int ID_bgdkvert = 187;
    public const int ID_brdrl = 188;
    public const int ID_ltrmark = 189;
    public const int ID_ulthldash = 190;
    public const int ID_intbl = 191;
    public const int ID_bgbdiag = 192;
    public const int ID_bkmkstart = 193;
    public const int ID_brdrb = 194;
    public const int ID_clbrdrl = 195;
    public const int ID_clbrdrb = 196;
    public const int ID_pagebb = 197;
    public const int ID_strike = 198;
    public const int ID_clvmgf = 199;
    public const int ID_trowd = 200;
    public const int ID_info = 201;
    public const int ID_ldblquote = 202;
    public const int ID_listoverride = 203;
    public const int ID_trqc = 204;
    public const int ID_ilvl = 205;
    public const int ID_li = 206;
    public const int ID_zwnj = 207;
    public const int ID_listsimple = 208;
    public const int ID_brdrdashdd = 209;
    public const int ID_fname = 210;
    public const int ID_ulnone = 211;
    public const int ID_bkmkend = 212;
    public const int ID_clwWidth = 213;
    public const int ID_adeflang = 214;
    public const int ID_brdrr = 215;
    public const int ID_brdrs = 216;
    public const int ID_pniroha = 217;
    public const int ID_brdrt = 218;
    public const int ID_ls = 219;
    public const int ID_brdrw = 220;
    public const int ID_irowband = 221;
    public const int ID_loch = 222;
    public const int ID_pnf = 223;
    public const int ID_slmult = 224;
    public const int ID_ulthdash = 225;
    public const int ID_bgdkdcross = 226;
    public const int ID_pntxta = 227;
    public const int ID_cellx = 228;
    public const int ID_pnlvlcont = 229;
    public const int ID_headerl = 230;
    public const int ID_jclisttab = 231;
    public const int ID_endash = 232;
    public const int ID_pntxtb = 233;
    public const int ID_bgfdiag = 234;
    public const int ID_brdrbar = 235;
    public const int ID_bgdkfdiag = 236;
    public const int ID_trwWidth = 237;
    public const int ID_alang = 238;
    public const int ID_brdrtnthtnlg = 239;
    public const int ID_cpg = 240;
    public const int ID_headerf = 241;
    public const int ID_ltrrow = 242;
    public const int ID_brdrtnthtnsg = 243;
    public const int ID_bgdkhoriz = 244;
    public const int ID_dropcapt = 245;
    public const int ID_listtable = 246;
    public const int ID_brdrtnthtnmg = 247;
    public const int ID_pnlcrm = 248;
    public const int ID_pndecd = 249;
    public const int ID_leveljc = 250;
    public const int ID_brdrtnthmg = 251;
    public const int ID_colortbl = 252;
    public const int ID_headerr = 253;
    public const int ID_brdrth = 254;
    public const int ID_levelpicture = 255;
    public const int ID_brdrtnthlg = 256;
    public const int ID_listtext = 257;
    public const int ID_footerr = 258;
    public const int ID_clmgf = 259;
    public const int ID_bgdcross = 260;
    public const int ID_brdrthtnsg = 261;
    public const int ID_sub = 262;
    public const int ID_rdblquote = 263;
    public const int ID_nosupersub = 264;
    public const int ID_fs = 265;
    public const int ID_ftech = 266;
    public const int ID_rtf = 267;
    public const int ID_falt = 268;
    public const int ID_fldrslt = 269;
    public const int ID_ltrpar = 270;
    public const int ID_urtf = 271;
    public const int ID_pncard = 272;
    public const int ID_footerf = 273;
    public const int ID_pndec = 274;
    public const int ID_dn = 275;
    public const int ID_brdrdashd = 276;
    public const int ID_footerl = 277;
    public const int ID_uldashd = 278;
    public const int ID_ulwave = 279;
    public const int ID_deff = 280;
    public const int ID_langfe = 281;
    public const int ID_bgdkcross = 282;
    public const int ID_nonesttables = 283;
    public const int ID_fi = 284;
    public const int ID_chcbpat = 285;
    public const int ID_rquote = 286;
    public const int ID_rtlch = 287;
    public const int ID_pndbnum = 288;
    public const int ID_brdrsh = 289;
    public const int ID_pnaiu = 290;
    public const int ID_ai = 291;
    public const int ID_fromhtml = 292;
    public const int ID_trhdr = 293;
    public const int ID_ulhair = 294;
    public const int ID_emdash = 295;
    public const int ID_levelnfc = 296;
    public const int ID_deleted = 297;
    public const int ID_dropcapli = 298;
    public const int ID_ulth = 299;
    public const int ID_mhtmltag = 300;
    public const int ID_pmmetafile = 301;
    public const int ID_impr = 302;
    public const int ID_pca = 303;
    public const int ID_pnirohad = 304;
    public const int ID_cs = 305;
    public const int ID_fldinst = 306;
    public const int ID_ab = 307;
    public const int ID_fswiss = 308;
    public const int ID_green = 309;
    public const int ID_ulthd = 310;
    public const int ID_ulc = 311;
    public const int ID_af = 312;
    public const int ID_uld = 313;
    public const int ID_bgcross = 314;
    public const int ID_stylesheet = 315;
    public const int ID_listoverridetable = 316;
    public const int ID_background = 317;
    public const int ID_clcbpat = 318;
    public const int ID_pntext = 319;
    public const int ID_trftsWidth = 320;
    public const int ID_brdrhair = 321;
    public const int ID_pnlvlbody = 322;
    public const int ID_super = 323;
    public const int ID_objdata = 324;
    public const int ID_ansi = 325;
    public const int ID_ulthdashdd = 326;
    public const int ID_ulp = 327;
    public const int ID_brdrdot = 328;
    public const int ID_fromtext = 329;
    public const int ID_brdremboss = 330;
    public const int ID_cf = 331;

    private RTFData()
    {
    }

    public static short Hash(byte[] chars, int off, int len)
    {
      short num1 = (short) 0;
      while (len != 0)
      {
        byte num2 = chars[off];
        num1 = (short) ((((int) num1 << 3) + ((int) num1 >> 6) ^ (int) num2) % 299);
        --len;
        ++off;
      }
      return num1;
    }

    public static short AddHash(short hash, byte ch)
    {
      return (short) ((((int) hash << 3) + ((int) hash >> 6) ^ (int) ch) % 299);
    }

    public struct KeyDef
    {
      public short hash;
      public short idx;
      public char character;
      public short defaultValue;
      public bool affectsParsing;
      public string name;

      public KeyDef(short hash, short idx, char character, short defaultValue, bool affectsParsing, string name)
      {
        this.hash = hash;
        this.idx = idx;
        this.character = character;
        this.defaultValue = defaultValue;
        this.affectsParsing = affectsParsing;
        this.name = name;
      }
    }
  }
}
