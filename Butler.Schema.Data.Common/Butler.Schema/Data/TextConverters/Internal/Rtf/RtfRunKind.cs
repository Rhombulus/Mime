// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Rtf.RtfRunKind
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Rtf
{
  internal enum RtfRunKind : ushort
  {
    Ignore = (ushort) 0,
    EndOfFile = (ushort) 4096,
    Begin = (ushort) 8192,
    End = (ushort) 12288,
    Binary = (ushort) 16384,
    SingleRunLast = (ushort) 16384,
    Keyword = (ushort) 20480,
    Text = (ushort) 28672,
    Escape = (ushort) 32768,
    Unicode = (ushort) 36864,
    Zero = (ushort) 40960,
    Mask = (ushort) 61440,
  }
}
