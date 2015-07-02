// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.MimeReaderState
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  [Flags]
  internal enum MimeReaderState
  {
    Start = 1,
    PartStart = 2,
    HeaderStart = 4,
    HeaderIncomplete = 8,
    HeaderComplete = 16,
    EndOfHeaders = 32,
    PartPrologue = 64,
    PartBody = 128,
    PartEpilogue = 256,
    PartEnd = 512,
    InlineStart = 1024,
    InlineBody = 2048,
    InlineEnd = 4096,
    InlineJunk = 8192,
    Embedded = 16384,
    EmbeddedEnd = 32768,
    End = 65536,
  }
}
