// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.DbcsLeadBits
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  [Flags]
  internal enum DbcsLeadBits : byte
  {
    Lead1361 = (byte) 1,
    Lead10001 = (byte) 2,
    Lead10002 = (byte) 4,
    Lead10003 = (byte) 8,
    Lead10008 = (byte) 16,
    Lead932 = (byte) 32,
    Lead9XX = (byte) 64,
  }
}
