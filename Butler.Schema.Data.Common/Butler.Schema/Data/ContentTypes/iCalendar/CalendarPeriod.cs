// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.iCalendar.CalendarPeriod
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.ContentTypes.iCalendar
{
  public struct CalendarPeriod
  {
    private DateTime start;
    private DateTime end;
    private TimeSpan duration;
    private bool isExplicitPeriod;

    public DateTime Start
    {
      get
      {
        return this.start;
      }
      set
      {
        this.start = value;
      }
    }

    public DateTime End
    {
      get
      {
        return this.end;
      }
      set
      {
        this.end = value;
        this.isExplicitPeriod = true;
      }
    }

    public TimeSpan Duration
    {
      get
      {
        return this.duration;
      }
      set
      {
        this.duration = value;
        this.isExplicitPeriod = false;
      }
    }

    public bool IsExplicit
    {
      get
      {
        return this.isExplicitPeriod;
      }
    }

    public CalendarPeriod(DateTime start, DateTime end)
    {
      this.start = start;
      this.end = end;
      this.duration = start - end;
      this.isExplicitPeriod = true;
    }

    public CalendarPeriod(DateTime start, TimeSpan duration)
    {
      this.start = start;
      this.end = start + duration;
      this.duration = duration;
      this.isExplicitPeriod = false;
    }

    internal CalendarPeriod(string s, Internal.ComplianceTracker tracker)
    {
      int length = s.IndexOf('/');
      if (length <= 0 || s.Length - 1 == length)
      {
        tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidTimeFormat);
        this.start = CalendarCommon.MinDateTime;
        this.end = CalendarCommon.MinDateTime;
        this.duration = TimeSpan.Zero;
        this.isExplicitPeriod = false;
      }
      else
      {
        DateTime dateTime1 = CalendarCommon.ParseDateTime(s.Substring(0, length), tracker);
        switch (s[length + 1])
        {
          case '+':
          case '-':
          case 'P':
            TimeSpan timeSpan = CalendarCommon.ParseDuration(s.Substring(length + 1), tracker);
            this.start = dateTime1;
            this.end = dateTime1 + timeSpan;
            this.duration = timeSpan;
            this.isExplicitPeriod = false;
            break;
          default:
            DateTime dateTime2 = CalendarCommon.ParseDateTime(s.Substring(length + 1), tracker);
            this.start = dateTime1;
            this.end = dateTime2;
            this.duration = dateTime1 - dateTime2;
            this.isExplicitPeriod = true;
            break;
        }
      }
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(CalendarCommon.FormatDateTime(this.start));
      stringBuilder.Append('/');
      if (this.isExplicitPeriod)
        stringBuilder.Append(CalendarCommon.FormatDateTime(this.end));
      else
        stringBuilder.Append(CalendarCommon.FormatDuration(this.duration));
      return stringBuilder.ToString();
    }
  }
}
