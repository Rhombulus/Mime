// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Globalization.Iso2022Jp.EscapeSequence
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Globalization.Iso2022Jp
{
  internal enum EscapeSequence
  {
    None,
    NotRecognized,
    Incomplete,
    JisX0208_1978,
    JisX0208_1983,
    JisX0201K_1976,
    JisX0201_1976,
    JisX0212_1990,
    JisX0208_Nec,
    Iso646Irv,
    ShiftIn,
    ShiftOut,
    JisX0208_1990,
    Cns11643_1992_1,
    Kcs5601_1987,
    Unknown_1,
    EucKsc,
    Gb2312_1980,
    NECKanjiIn,
  }
}
