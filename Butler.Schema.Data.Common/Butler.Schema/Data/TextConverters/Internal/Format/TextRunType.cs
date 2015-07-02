// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Format.TextRunType
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Format
{
  internal enum TextRunType : ushort
  {
    Invalid = (ushort) 0,
    Skip = (ushort) 0,
    Markup = (ushort) 4096,
    NonSpace = (ushort) 8192,
    FirstShort = (ushort) 12288,
    InlineObject = (ushort) 12288,
    NbSp = (ushort) 16384,
    Space = (ushort) 20480,
    Tabulation = (ushort) 24576,
    NewLine = (ushort) 28672,
    BlockBoundary = (ushort) 32768,
  }
}
