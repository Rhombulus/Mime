// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Rtf.RtfBorderKind
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Rtf
{
  internal enum RtfBorderKind : byte
  {
    None = (byte) 0,
    Dot = (byte) 1,
    Dash = (byte) 2,
    Solid = (byte) 3,
    Double = (byte) 4,
    Groove = (byte) 5,
    Ridge = (byte) 6,
    Inset = (byte) 7,
    Outset = (byte) 8,
    Default = (byte) 255,
  }
}
