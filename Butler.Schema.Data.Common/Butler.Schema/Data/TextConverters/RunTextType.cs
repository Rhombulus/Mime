// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.RunTextType
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  internal enum RunTextType : uint
  {
    Unknown = 0U,
    Space = 134217728U,
    NewLine = 268435456U,
    Tabulation = 402653184U,
    LastWhitespace = 536870912U,
    UnusualWhitespace = 536870912U,
    Nbsp = 671088640U,
    Last = 805306368U,
    LastText = 805306368U,
    NonSpace = 805306368U,
    Mask = 939524096U,
  }
}
