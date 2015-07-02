// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.RunKind
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  internal enum RunKind : uint
  {
    Invalid = 0U,
    MinorKindMask = 50331648U,
    Text = 67108864U,
    MajorKindMask = 2080374784U,
    StartLexicalUnitFlag = 2147483648U,
    MajorKindMaskWithStartLexicalUnitFlag = 4227858432U,
    KindMask = 4278190080U,
  }
}
