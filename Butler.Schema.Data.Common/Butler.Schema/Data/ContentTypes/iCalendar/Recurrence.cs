// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.iCalendar.Recurrence
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.ContentTypes.iCalendar
{
  public class Recurrence
  {
    private int interval = 1;
    private DayOfWeek workWeekStart = DayOfWeek.Monday;
    private static Dictionary<string, Frequency> frequencyToEnumTable = new Dictionary<string, Frequency>();
    private static Dictionary<string, RecurrenceProperties> recurPropToEnumTable = new Dictionary<string, RecurrenceProperties>();
    private Frequency freq;
    private DateTime untilDate;
    private DateTime untilDateTime;
    private int count;
    private int[] bySecond;
    private int[] byMinute;
    private int[] byHour;
    private Recurrence.ByDay[] byDay;
    private int[] byMonthDay;
    private int[] byYearDay;
    private int[] byWeekNumber;
    private int[] byMonth;
    private int[] bySetPos;
      private Internal.ComplianceTracker tracker;

    public Frequency Frequency
    {
      get
      {
        return this.freq;
      }
      set
      {
        this.AvailableProperties |= RecurrenceProperties.Frequency;
        this.freq = value;
      }
    }

    public DateTime UntilDate
    {
      get
      {
        return this.untilDate;
      }
      set
      {
        this.AvailableProperties &= ~RecurrenceProperties.UntilDateTime;
        this.AvailableProperties |= RecurrenceProperties.UntilDate;
        this.untilDate = value;
      }
    }

    public DateTime UntilDateTime
    {
      get
      {
        return this.untilDateTime;
      }
      set
      {
        this.AvailableProperties &= ~RecurrenceProperties.UntilDate;
        this.AvailableProperties |= RecurrenceProperties.UntilDateTime;
        this.untilDateTime = value;
      }
    }

    public int Count
    {
      get
      {
        return this.count;
      }
      set
      {
        this.AvailableProperties |= RecurrenceProperties.Count;
        this.count = value;
      }
    }

    public int Interval
    {
      get
      {
        return this.interval;
      }
      set
      {
        this.AvailableProperties |= RecurrenceProperties.Interval;
        this.interval = value;
      }
    }

    public int[] BySecond
    {
      get
      {
        return this.bySecond;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException();
        this.AvailableProperties |= RecurrenceProperties.BySecond;
        this.bySecond = value;
      }
    }

    public int[] ByMinute
    {
      get
      {
        return this.byMinute;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException();
        this.AvailableProperties |= RecurrenceProperties.ByMinute;
        this.byMinute = value;
      }
    }

    public int[] ByHour
    {
      get
      {
        return this.byHour;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException();
        this.AvailableProperties |= RecurrenceProperties.ByHour;
        this.byHour = value;
      }
    }

    public Recurrence.ByDay[] ByDayList
    {
      get
      {
        return this.byDay;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException();
        this.AvailableProperties |= RecurrenceProperties.ByDay;
        this.byDay = value;
      }
    }

    public int[] ByMonthDay
    {
      get
      {
        return this.byMonthDay;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException();
        this.AvailableProperties |= RecurrenceProperties.ByMonthDay;
        this.byMonthDay = value;
      }
    }

    public int[] ByYearDay
    {
      get
      {
        return this.byYearDay;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException();
        this.AvailableProperties |= RecurrenceProperties.ByYearDay;
        this.byYearDay = value;
      }
    }

    public int[] ByWeek
    {
      get
      {
        return this.byWeekNumber;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException();
        this.AvailableProperties |= RecurrenceProperties.ByWeek;
        this.byWeekNumber = value;
      }
    }

    public int[] ByMonth
    {
      get
      {
        return this.byMonth;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException();
        this.AvailableProperties |= RecurrenceProperties.ByMonth;
        this.byMonth = value;
      }
    }

    public int[] BySetPosition
    {
      get
      {
        return this.bySetPos;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException();
        this.AvailableProperties |= RecurrenceProperties.BySetPosition;
        this.bySetPos = value;
      }
    }

    public DayOfWeek WorkWeekStart
    {
      get
      {
        return this.workWeekStart;
      }
      set
      {
        this.AvailableProperties |= RecurrenceProperties.WeekStart;
        this.workWeekStart = value;
      }
    }

    public RecurrenceProperties AvailableProperties { get; set; }

      static Recurrence()
    {
      Recurrence.frequencyToEnumTable.Add("SECONDLY", Frequency.Secondly);
      Recurrence.frequencyToEnumTable.Add("MINUTELY", Frequency.Minutely);
      Recurrence.frequencyToEnumTable.Add("HOURLY", Frequency.Hourly);
      Recurrence.frequencyToEnumTable.Add("DAILY", Frequency.Daily);
      Recurrence.frequencyToEnumTable.Add("WEEKLY", Frequency.Weekly);
      Recurrence.frequencyToEnumTable.Add("MONTHLY", Frequency.Monthly);
      Recurrence.frequencyToEnumTable.Add("YEARLY", Frequency.Yearly);
      Recurrence.recurPropToEnumTable.Add("FREQ", RecurrenceProperties.Frequency);
      Recurrence.recurPropToEnumTable.Add("UNTIL", RecurrenceProperties.UntilDate);
      Recurrence.recurPropToEnumTable.Add("COUNT", RecurrenceProperties.Count);
      Recurrence.recurPropToEnumTable.Add("INTERVAL", RecurrenceProperties.Interval);
      Recurrence.recurPropToEnumTable.Add("BYSECOND", RecurrenceProperties.BySecond);
      Recurrence.recurPropToEnumTable.Add("BYMINUTE", RecurrenceProperties.ByMinute);
      Recurrence.recurPropToEnumTable.Add("BYHOUR", RecurrenceProperties.ByHour);
      Recurrence.recurPropToEnumTable.Add("BYDAY", RecurrenceProperties.ByDay);
      Recurrence.recurPropToEnumTable.Add("BYMONTHDAY", RecurrenceProperties.ByMonthDay);
      Recurrence.recurPropToEnumTable.Add("BYYEARDAY", RecurrenceProperties.ByYearDay);
      Recurrence.recurPropToEnumTable.Add("BYWEEKNO", RecurrenceProperties.ByWeek);
      Recurrence.recurPropToEnumTable.Add("BYMONTH", RecurrenceProperties.ByMonth);
      Recurrence.recurPropToEnumTable.Add("BYSETPOS", RecurrenceProperties.BySetPosition);
      Recurrence.recurPropToEnumTable.Add("WKST", RecurrenceProperties.WeekStart);
    }

    public Recurrence()
    {
    }

    public Recurrence(string value)
      : this(value, (Internal.ComplianceTracker) null)
    {
    }

    internal Recurrence(string s, Internal.ComplianceTracker tracker)
    {
      this.tracker = tracker;
      Recurrence.ParserStates parserStates = Recurrence.ParserStates.Name;
      int length = s.Length;
      string s1 = string.Empty;
      List<string> list = new List<string>();
      int num = 0;
      while (num < length)
      {
        switch (parserStates)
        {
          case Recurrence.ParserStates.Name:
            StringBuilder stringBuilder1 = new StringBuilder();
            while (num < length)
            {
              char ch = s[num++];
              if ((int) ch >= Internal.ContentLineParser.Dictionary.Length || (Internal.ContentLineParser.Dictionary[(int) ch] & Internal.ContentLineParser.Tokens.ValueChar) == ~(Internal.ContentLineParser.Tokens.CTL | Internal.ContentLineParser.Tokens.Alpha | Internal.ContentLineParser.Tokens.Digit | Internal.ContentLineParser.Tokens.SafeChar | Internal.ContentLineParser.Tokens.QSafeChar | Internal.ContentLineParser.Tokens.ValueChar | Internal.ContentLineParser.Tokens.WSP | Internal.ContentLineParser.Tokens.NonASCII))
              {
                this.SetComplianceStatus(CtsResources.CalendarStrings.InvalidCharacterInRecurrence);
                return;
              }
              if ((int) ch != 61)
                stringBuilder1.Append(ch);
              else
                break;
            }
            s1 = stringBuilder1.ToString();
            parserStates = Recurrence.ParserStates.Value;
            continue;
          case Recurrence.ParserStates.Value:
            bool flag = false;
            StringBuilder stringBuilder2 = new StringBuilder();
            while (num < length)
            {
              char ch = s[num++];
              if ((int) ch >= Internal.ContentLineParser.Dictionary.Length || (Internal.ContentLineParser.Dictionary[(int) ch] & Internal.ContentLineParser.Tokens.ValueChar) == ~(Internal.ContentLineParser.Tokens.CTL | Internal.ContentLineParser.Tokens.Alpha | Internal.ContentLineParser.Tokens.Digit | Internal.ContentLineParser.Tokens.SafeChar | Internal.ContentLineParser.Tokens.QSafeChar | Internal.ContentLineParser.Tokens.ValueChar | Internal.ContentLineParser.Tokens.WSP | Internal.ContentLineParser.Tokens.NonASCII))
              {
                this.SetComplianceStatus(CtsResources.CalendarStrings.InvalidCharacterInRecurrence);
                return;
              }
              if ((int) ch == 59)
              {
                flag = true;
                parserStates = Recurrence.ParserStates.Name;
                break;
              }
              if ((int) ch == 44)
              {
                flag = false;
                break;
              }
              stringBuilder2.Append(ch);
            }
            list.Add(stringBuilder2.ToString());
            if (flag || num == length)
            {
              int count = list.Count;
              switch (Recurrence.GetRecurProp(s1))
              {
                case RecurrenceProperties.ByMonth:
                  if ((this.AvailableProperties & RecurrenceProperties.ByMonth) != RecurrenceProperties.None)
                  {
                    this.SetComplianceStatus(CtsResources.CalendarStrings.ByMonthOnlyPermittedOnce);
                    return;
                  }
                  this.byMonth = new int[count];
                  for (int index = 0; index < count; ++index)
                  {
                    int result;
                    if (!int.TryParse(list[index], out result))
                      this.SetComplianceStatus(CtsResources.CalendarStrings.InvalidValueFormat);
                    this.byMonth[index] = result;
                    if (result < 0 || result > 12)
                    {
                      this.SetComplianceStatus(CtsResources.CalendarStrings.ByMonthOutOfRange);
                      return;
                    }
                  }
                  this.AvailableProperties |= RecurrenceProperties.ByMonth;
                  break;
                case RecurrenceProperties.BySetPosition:
                  if ((this.AvailableProperties & RecurrenceProperties.BySetPosition) != RecurrenceProperties.None)
                  {
                    this.SetComplianceStatus(CtsResources.CalendarStrings.BySetPosOnlyPermittedOnce);
                    return;
                  }
                  this.bySetPos = new int[count];
                  for (int index = 0; index < count; ++index)
                  {
                    int result;
                    if (!int.TryParse(list[index], out result))
                      this.SetComplianceStatus(CtsResources.CalendarStrings.InvalidValueFormat);
                    this.bySetPos[index] = result;
                    if (result == 0 || result > 366 || result < -366)
                    {
                      this.SetComplianceStatus(CtsResources.CalendarStrings.BySetPosOutOfRange);
                      return;
                    }
                  }
                  this.AvailableProperties |= RecurrenceProperties.BySetPosition;
                  break;
                case RecurrenceProperties.WeekStart:
                  if ((this.AvailableProperties & RecurrenceProperties.WeekStart) != RecurrenceProperties.None)
                  {
                    this.SetComplianceStatus(CtsResources.CalendarStrings.WkStOnlyPermittedOnce);
                    return;
                  }
                  if (count > 1)
                  {
                    this.SetComplianceStatus(CtsResources.CalendarStrings.MultivalueNotPermittedOnWkSt);
                    return;
                  }
                  this.workWeekStart = this.GetDayOfWeek(list[0]);
                  this.AvailableProperties |= RecurrenceProperties.WeekStart;
                  break;
                case RecurrenceProperties.ByMonthDay:
                  if ((this.AvailableProperties & RecurrenceProperties.ByMonthDay) != RecurrenceProperties.None)
                  {
                    this.SetComplianceStatus(CtsResources.CalendarStrings.ByMonthDayOnlyPermittedOnce);
                    return;
                  }
                  this.byMonthDay = new int[count];
                  for (int index = 0; index < count; ++index)
                  {
                    int result;
                    if (!int.TryParse(list[index], out result))
                      this.SetComplianceStatus(CtsResources.CalendarStrings.InvalidValueFormat);
                    this.byMonthDay[index] = result;
                    if (result == 0 || result > 31 || result < -31)
                    {
                      this.SetComplianceStatus(CtsResources.CalendarStrings.ByMonthDayOutOfRange);
                      return;
                    }
                  }
                  this.AvailableProperties |= RecurrenceProperties.ByMonthDay;
                  break;
                case RecurrenceProperties.ByYearDay:
                  if ((this.AvailableProperties & RecurrenceProperties.ByYearDay) != RecurrenceProperties.None)
                  {
                    this.SetComplianceStatus(CtsResources.CalendarStrings.ByYearDayOnlyPermittedOnce);
                    return;
                  }
                  this.byYearDay = new int[count];
                  for (int index = 0; index < count; ++index)
                  {
                    int result;
                    if (!int.TryParse(list[index], out result))
                      this.SetComplianceStatus(CtsResources.CalendarStrings.InvalidValueFormat);
                    this.byYearDay[index] = result;
                    if (result == 0 || result > 366 || result < -366)
                    {
                      this.SetComplianceStatus(CtsResources.CalendarStrings.ByYearDayOutOfRange);
                      return;
                    }
                  }
                  this.AvailableProperties |= RecurrenceProperties.ByYearDay;
                  break;
                case RecurrenceProperties.ByWeek:
                  if ((this.AvailableProperties & RecurrenceProperties.ByWeek) != RecurrenceProperties.None)
                  {
                    this.SetComplianceStatus(CtsResources.CalendarStrings.ByWeekNoOnlyPermittedOnce);
                    return;
                  }
                  this.byWeekNumber = new int[count];
                  for (int index = 0; index < count; ++index)
                  {
                    int result;
                    if (!int.TryParse(list[index], out result))
                      this.SetComplianceStatus(CtsResources.CalendarStrings.InvalidValueFormat);
                    this.byWeekNumber[index] = result;
                    if (result == 0 || result > 53 || result < -53)
                    {
                      this.SetComplianceStatus(CtsResources.CalendarStrings.ByWeekNoOutOfRange);
                      return;
                    }
                  }
                  this.AvailableProperties |= RecurrenceProperties.ByWeek;
                  break;
                case RecurrenceProperties.ByMinute:
                  if ((this.AvailableProperties & RecurrenceProperties.ByMinute) != RecurrenceProperties.None)
                  {
                    this.SetComplianceStatus(CtsResources.CalendarStrings.ByMinuteOnlyPermittedOnce);
                    return;
                  }
                  this.byMinute = new int[count];
                  for (int index = 0; index < count; ++index)
                  {
                    if (!int.TryParse(list[index], out this.byMinute[index]))
                      this.SetComplianceStatus(CtsResources.CalendarStrings.InvalidValueFormat);
                    if (this.byMinute[index] < 0 || this.byMinute[index] > 59)
                    {
                      this.SetComplianceStatus(CtsResources.CalendarStrings.ByMinuteOutOfRange);
                      return;
                    }
                  }
                  this.AvailableProperties |= RecurrenceProperties.ByMinute;
                  break;
                case RecurrenceProperties.ByHour:
                  if ((this.AvailableProperties & RecurrenceProperties.ByHour) != RecurrenceProperties.None)
                  {
                    this.SetComplianceStatus(CtsResources.CalendarStrings.ByHourOnlyPermittedOnce);
                    return;
                  }
                  this.byHour = new int[count];
                  for (int index = 0; index < count; ++index)
                  {
                    if (!int.TryParse(list[index], out this.byHour[index]))
                      this.SetComplianceStatus(CtsResources.CalendarStrings.InvalidValueFormat);
                    if (this.byHour[index] < 0 || this.byHour[index] > 23)
                    {
                      this.SetComplianceStatus(CtsResources.CalendarStrings.ByHourOutOfRange);
                      return;
                    }
                  }
                  this.AvailableProperties |= RecurrenceProperties.ByHour;
                  break;
                case RecurrenceProperties.ByDay:
                  if ((this.AvailableProperties & RecurrenceProperties.ByDay) != RecurrenceProperties.None)
                  {
                    this.SetComplianceStatus(CtsResources.CalendarStrings.ByDayOnlyPermittedOnce);
                    return;
                  }
                  this.byDay = new Recurrence.ByDay[count];
                  for (int index = 0; index < count; ++index)
                  {
                    string str = list[index];
                    if (str.Length != 0)
                    {
                      int startIndex1 = 0;
                      while (startIndex1 < str.Length && (int) str[startIndex1] == 32)
                        ++startIndex1;
                      if (startIndex1 != str.Length)
                      {
                        int startIndex2 = startIndex1 - 1;
                        char ch;
                        do
                        {
                          ch = str[++startIndex2];
                          if ((int) ch >= Internal.ContentLineParser.Dictionary.Length)
                          {
                            this.SetComplianceStatus(CtsResources.CalendarStrings.InvalidValueFormat);
                            break;
                          }
                        }
                        while (((Internal.ContentLineParser.Dictionary[(int) ch] & Internal.ContentLineParser.Tokens.Digit) > ~(Internal.ContentLineParser.Tokens.CTL | Internal.ContentLineParser.Tokens.Alpha | Internal.ContentLineParser.Tokens.Digit | Internal.ContentLineParser.Tokens.SafeChar | Internal.ContentLineParser.Tokens.QSafeChar | Internal.ContentLineParser.Tokens.ValueChar | Internal.ContentLineParser.Tokens.WSP | Internal.ContentLineParser.Tokens.NonASCII) || (int) ch == 43 || (int) ch == 45) && startIndex2 + 1 < str.Length);
                        if (startIndex2 != startIndex1)
                        {
                          int result = 0;
                          if (!int.TryParse(str.Substring(startIndex1, startIndex2 - startIndex1), out result) || result == 0)
                            this.SetComplianceStatus(CtsResources.CalendarStrings.InvalidValueFormat);
                          this.byDay[index].OccurrenceNumber = result;
                        }
                        while ((int) str[startIndex2] == 32 && startIndex2 + 1 < str.Length)
                          ++startIndex2;
                        this.byDay[index].Day = this.GetDayOfWeek(str.Substring(startIndex2, str.Length - startIndex2));
                      }
                    }
                  }
                  this.AvailableProperties |= RecurrenceProperties.ByDay;
                  break;
                case RecurrenceProperties.Frequency:
                  if (count > 1)
                  {
                    this.SetComplianceStatus(CtsResources.CalendarStrings.MultivalueNotPermittedOnFreq);
                    return;
                  }
                  this.freq = Recurrence.GetFrequency(list[0]);
                  if (this.freq == Frequency.Unknown)
                  {
                    this.SetComplianceStatus(CtsResources.CalendarStrings.UnknownFrequencyValue);
                    return;
                  }
                  this.AvailableProperties |= RecurrenceProperties.Frequency;
                  break;
                case RecurrenceProperties.UntilDate:
                  if (count > 1)
                  {
                    this.SetComplianceStatus(CtsResources.CalendarStrings.MultivalueNotPermittedOnUntil);
                    return;
                  }
                  if ((this.AvailableProperties & RecurrenceProperties.UntilDate) != RecurrenceProperties.None || (this.AvailableProperties & RecurrenceProperties.UntilDateTime) != RecurrenceProperties.None)
                  {
                    this.SetComplianceStatus(CtsResources.CalendarStrings.UntilOnlyPermittedOnce);
                    return;
                  }
                  if ((this.AvailableProperties & RecurrenceProperties.Count) != RecurrenceProperties.None)
                  {
                    this.SetComplianceStatus(CtsResources.CalendarStrings.UntilNotPermittedWithCount);
                    return;
                  }
                  if (list[0].Length > 8)
                  {
                    this.untilDateTime = CalendarCommon.ParseDateTime(list[0], tracker);
                    this.AvailableProperties |= RecurrenceProperties.UntilDateTime;
                    break;
                  }
                  this.untilDate = CalendarCommon.ParseDate(list[0], tracker);
                  this.AvailableProperties |= RecurrenceProperties.UntilDate;
                  break;
                case RecurrenceProperties.Count:
                  if ((this.AvailableProperties & RecurrenceProperties.Count) != RecurrenceProperties.None)
                  {
                    this.SetComplianceStatus(CtsResources.CalendarStrings.CountOnlyPermittedOnce);
                    return;
                  }
                  if ((this.AvailableProperties & RecurrenceProperties.UntilDate) != RecurrenceProperties.None || (this.AvailableProperties & RecurrenceProperties.UntilDateTime) != RecurrenceProperties.None)
                  {
                    this.SetComplianceStatus(CtsResources.CalendarStrings.CountNotPermittedWithUntil);
                    return;
                  }
                  if (count > 1)
                  {
                    this.SetComplianceStatus(CtsResources.CalendarStrings.MultivalueNotPermittedOnCount);
                    return;
                  }
                  if (!int.TryParse(list[0], out this.count))
                    this.SetComplianceStatus(CtsResources.CalendarStrings.InvalidValueFormat);
                  this.AvailableProperties |= RecurrenceProperties.Count;
                  break;
                case RecurrenceProperties.Interval:
                  if ((this.AvailableProperties & RecurrenceProperties.Interval) != RecurrenceProperties.None)
                  {
                    this.SetComplianceStatus(CtsResources.CalendarStrings.IntervalOnlyPermittedOnce);
                    return;
                  }
                  if (count > 1)
                  {
                    this.SetComplianceStatus(CtsResources.CalendarStrings.MultivalueNotPermittedOnInterval);
                    return;
                  }
                  if (!int.TryParse(list[0], out this.interval))
                    this.SetComplianceStatus(CtsResources.CalendarStrings.InvalidValueFormat);
                  if (this.interval < 1)
                  {
                    this.SetComplianceStatus(CtsResources.CalendarStrings.IntervalMustBePositive);
                    return;
                  }
                  this.AvailableProperties |= RecurrenceProperties.Interval;
                  break;
                case RecurrenceProperties.BySecond:
                  if ((this.AvailableProperties & RecurrenceProperties.BySecond) != RecurrenceProperties.None)
                  {
                    this.SetComplianceStatus(CtsResources.CalendarStrings.BySecondOnlyPermittedOnce);
                    return;
                  }
                  this.bySecond = new int[count];
                  for (int index = 0; index < count; ++index)
                  {
                    if (!int.TryParse(list[index], out this.bySecond[index]))
                      this.SetComplianceStatus(CtsResources.CalendarStrings.InvalidValueFormat);
                    if (this.bySecond[index] < 0 || this.bySecond[index] > 59)
                    {
                      this.SetComplianceStatus(CtsResources.CalendarStrings.BySecondOutOfRange);
                      return;
                    }
                  }
                  this.AvailableProperties |= RecurrenceProperties.BySecond;
                  break;
                default:
                  this.SetComplianceStatus(CtsResources.CalendarStrings.UnknownRecurrenceProperty);
                  break;
              }
              list = new List<string>();
              continue;
            }
            continue;
          default:
            continue;
        }
      }
    }

    public override string ToString()
    {
      StringBuilder s = new StringBuilder();
      if ((this.AvailableProperties & RecurrenceProperties.Frequency) != RecurrenceProperties.None)
      {
        s.Append("FREQ");
        s.Append('=');
        s.Append(Recurrence.GetFrequencyString(this.freq));
      }
      if ((this.AvailableProperties & RecurrenceProperties.UntilDate) != RecurrenceProperties.None)
      {
        s.Append(";UNTIL");
        s.Append('=');
        s.Append(CalendarCommon.FormatDate(this.untilDate));
      }
      if ((this.AvailableProperties & RecurrenceProperties.UntilDateTime) != RecurrenceProperties.None)
      {
        s.Append(";UNTIL");
        s.Append('=');
        s.Append(CalendarCommon.FormatDateTime(this.untilDateTime));
      }
      if ((this.AvailableProperties & RecurrenceProperties.Count) != RecurrenceProperties.None)
      {
        s.Append(";COUNT");
        s.Append('=');
        s.Append(this.count);
      }
      if ((this.AvailableProperties & RecurrenceProperties.Interval) != RecurrenceProperties.None)
      {
        s.Append(";INTERVAL");
        s.Append('=');
        s.Append(this.interval);
      }
      if ((this.AvailableProperties & RecurrenceProperties.BySecond) != RecurrenceProperties.None)
      {
        s.Append(";BYSECOND");
        s.Append('=');
        this.OutputList(this.bySecond, s);
      }
      if ((this.AvailableProperties & RecurrenceProperties.ByMinute) != RecurrenceProperties.None)
      {
        s.Append(";BYMINUTE");
        s.Append('=');
        this.OutputList(this.byMinute, s);
      }
      if ((this.AvailableProperties & RecurrenceProperties.ByHour) != RecurrenceProperties.None)
      {
        s.Append(";BYHOUR");
        s.Append('=');
        this.OutputList(this.byHour, s);
      }
      if ((this.AvailableProperties & RecurrenceProperties.ByDay) != RecurrenceProperties.None)
      {
        s.Append(";BYDAY");
        s.Append('=');
        int length = this.byDay.Length;
        if (length > 0)
          s.Append((object) this.byDay[0]);
        for (int index = 1; index < length; ++index)
        {
          s.Append(',');
          s.Append(this.byDay[index].ToString());
        }
      }
      if ((this.AvailableProperties & RecurrenceProperties.ByMonthDay) != RecurrenceProperties.None)
      {
        s.Append(";BYMONTHDAY");
        s.Append('=');
        this.OutputList(this.byMonthDay, s);
      }
      if ((this.AvailableProperties & RecurrenceProperties.ByYearDay) != RecurrenceProperties.None)
      {
        s.Append(";BYYEARDAY");
        s.Append('=');
        this.OutputList(this.byYearDay, s);
      }
      if ((this.AvailableProperties & RecurrenceProperties.ByWeek) != RecurrenceProperties.None)
      {
        s.Append(";BYWEEKNO");
        s.Append('=');
        this.OutputList(this.byWeekNumber, s);
      }
      if ((this.AvailableProperties & RecurrenceProperties.ByMonth) != RecurrenceProperties.None)
      {
        s.Append(";BYMONTH");
        s.Append('=');
        this.OutputList(this.byMonth, s);
      }
      if ((this.AvailableProperties & RecurrenceProperties.BySetPosition) != RecurrenceProperties.None)
      {
        s.Append(";BYSETPOS");
        s.Append('=');
        this.OutputList(this.bySetPos, s);
      }
      if ((this.AvailableProperties & RecurrenceProperties.WeekStart) != RecurrenceProperties.None)
      {
        s.Append(";WKST");
        s.Append('=');
        s.Append(Recurrence.GetDayOfWeekString(this.workWeekStart));
      }
      return s.ToString();
    }

    private static Frequency GetFrequency(string s)
    {
      Frequency frequency;
      if (!Recurrence.frequencyToEnumTable.TryGetValue(s.ToUpper(), out frequency))
        return Frequency.Unknown;
      return frequency;
    }

    private static RecurrenceProperties GetRecurProp(string s)
    {
      RecurrenceProperties recurrenceProperties;
      if (!Recurrence.recurPropToEnumTable.TryGetValue(s.ToUpper(), out recurrenceProperties))
        return RecurrenceProperties.None;
      return recurrenceProperties;
    }

    private static string GetDayOfWeekString(DayOfWeek d)
    {
      switch (d)
      {
        case DayOfWeek.Sunday:
          return "SU";
        case DayOfWeek.Monday:
          return "MO";
        case DayOfWeek.Tuesday:
          return "TU";
        case DayOfWeek.Wednesday:
          return "WE";
        case DayOfWeek.Thursday:
          return "TH";
        case DayOfWeek.Friday:
          return "FR";
        case DayOfWeek.Saturday:
          return "SA";
        default:
          return string.Empty;
      }
    }

    private static string GetFrequencyString(Frequency f)
    {
      switch (f)
      {
        case Frequency.Secondly:
          return "SECONDLY";
        case Frequency.Minutely:
          return "MINUTELY";
        case Frequency.Hourly:
          return "HOURLY";
        case Frequency.Daily:
          return "DAILY";
        case Frequency.Weekly:
          return "WEEKLY";
        case Frequency.Monthly:
          return "MONTHLY";
        case Frequency.Yearly:
          return "YEARLY";
        default:
          return string.Empty;
      }
    }

    private DayOfWeek GetDayOfWeek(string s)
    {
      switch (s.ToUpper())
      {
        case "SU":
          return DayOfWeek.Sunday;
        case "MO":
          return DayOfWeek.Monday;
        case "TU":
          return DayOfWeek.Tuesday;
        case "WE":
          return DayOfWeek.Wednesday;
        case "TH":
          return DayOfWeek.Thursday;
        case "FR":
          return DayOfWeek.Friday;
        case "SA":
          return DayOfWeek.Saturday;
        default:
          this.SetComplianceStatus(CtsResources.CalendarStrings.UnknownDayOfWeek);
          return DayOfWeek.Sunday;
      }
    }

    private void OutputList(int[] list, StringBuilder s)
    {
      int length = list.Length;
      if (length > 0)
        s.Append(list[0]);
      for (int index = 1; index < length; ++index)
      {
        s.Append(',');
        s.Append(list[index].ToString());
      }
    }

    private void SetComplianceStatus(string message)
    {
      if (this.tracker == null)
        throw new InvalidCalendarDataException(message);
      this.tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidValueFormat);
    }

    private enum ParserStates
    {
      Name,
      Value,
    }

    public struct ByDay
    {

        public int OccurrenceNumber { get; set; }

        public DayOfWeek Day { get; set; }

        public ByDay(DayOfWeek day, int occurrenceNumber)
      {
        this.Day = day;
        this.OccurrenceNumber = occurrenceNumber;
      }

      public override string ToString()
      {
        string dayOfWeekString = Recurrence.GetDayOfWeekString(this.Day);
        if (this.OccurrenceNumber != 0)
          return this.OccurrenceNumber.ToString() + dayOfWeekString;
        return dayOfWeekString;
      }
    }
  }
}
