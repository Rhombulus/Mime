// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.iCalendar.ComponentId
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.ContentTypes.iCalendar
{
  [Flags]
  public enum ComponentId
  {
    None = 0,
    Unknown = 1,
    VCalendar = 2,
    VEvent = 4,
    VTodo = 8,
    VJournal = 16,
    VFreeBusy = 32,
    VTimeZone = 64,
    VAlarm = 128,
    Standard = 256,
    Daylight = 512,
  }
}
