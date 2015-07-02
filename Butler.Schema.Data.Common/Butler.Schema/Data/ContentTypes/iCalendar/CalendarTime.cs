// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.iCalendar.CalendarTime
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Globalization;
using System.Linq;

namespace Butler.Schema.Data.ContentTypes.iCalendar
{
  public struct CalendarTime
  {
    private const string TimeFormatUtc = "HHmmss\\Z";
    private const string TimeFormat = "HHmmss";

      public TimeSpan Time { get; set; }

      public bool IsUtc { get; set; }

      public CalendarTime(TimeSpan time, bool isUtc)
    {
      this.Time = time;
      this.IsUtc = isUtc;
    }

    internal CalendarTime(string s, Internal.ComplianceTracker tracker)
    {
      this.IsUtc = false;
      if (s.Length != 6 && s.Length != 7)
      {
        tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidTimeStringLength);
        this.Time = TimeSpan.Zero;
      }
      else
      {
        if (s.Length == 7)
        {
          if ((int) s[6] != 90)
          {
            tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.ExpectedZ);
            this.Time = TimeSpan.Zero;
            return;
          }
          this.IsUtc = true;
          s = s.Substring(0, 6);
        }
        DateTime result;
        if (!DateTime.TryParseExact(s, "HHmmss", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
        {
          tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidTimeFormat);
          this.Time = TimeSpan.Zero;
        }
        else
          this.Time = new TimeSpan(result.Hour, result.Minute, result.Second);
      }
    }

    public override string ToString()
    {
      return new DateTime(1, 1, 1, this.Time.Hours, this.Time.Minutes, this.Time.Seconds).ToString(this.IsUtc ? "HHmmss\\Z" : "HHmmss");
    }
  }
}
