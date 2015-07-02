// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.DecodingFlags
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  [Flags]
  public enum DecodingFlags
  {
    None = 0,
    Rfc2047 = 1,
    Rfc2231 = 2,
    Jis = 4,
    Utf8 = 8,
    Dbcs = 16,
    AllEncodings = 65535,
    FallbackToRaw = 65536,
    AllowControlCharacters = 131072,
  }
}
