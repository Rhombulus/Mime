// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Globalization.Iso2022Jp.EscapeState
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Globalization.Iso2022Jp
{
  internal enum EscapeState
  {
    Begin,
    Esc_1,
    Esc_Dollar_2,
    Esc_OpenParen_2,
    Esc_Ampersand_2,
    Esc_K_2,
    Esc_Dollar_OpenParen_3,
    Esc_Dollar_CloseParen_3,
    Esc_Ampersand_At_3,
    Esc_Ampersand_At_Esc_4,
    Esc_Ampersand_At_Esc_Dollar_5,
    Esc_Esc_Reset,
    Esc_SISO_Reset,
  }
}
