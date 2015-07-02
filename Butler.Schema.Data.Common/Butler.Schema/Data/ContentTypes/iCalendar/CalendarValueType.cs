// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.iCalendar.CalendarValueType
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.ContentTypes.iCalendar
{
  [Flags]
  public enum CalendarValueType
  {
    Unknown = 1,
    Binary = 2,
    Boolean = 4,
    CalAddress = 8,
    Date = 16,
    DateTime = 32,
    Duration = 64,
    Float = 128,
    Integer = 256,
    Period = 512,
    Recurrence = 1024,
    Text = 2048,
    Time = 4096,
    Uri = 8192,
    UtcOffset = 16384,
  }
}
