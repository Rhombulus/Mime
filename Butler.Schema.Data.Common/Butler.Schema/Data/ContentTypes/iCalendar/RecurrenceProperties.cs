// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.iCalendar.RecurrenceProperties
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.ContentTypes.iCalendar
{
  [Flags]
  public enum RecurrenceProperties
  {
    None = 0,
    Frequency = 1,
    UntilDate = 2,
    Count = 4,
    Interval = 8,
    BySecond = 16,
    ByMinute = 32,
    ByHour = 64,
    ByDay = 128,
    ByMonthDay = 256,
    ByYearDay = 512,
    ByWeek = 1024,
    ByMonth = 2048,
    BySetPosition = 4096,
    WeekStart = 8192,
    UntilDateTime = 16384,
  }
}
