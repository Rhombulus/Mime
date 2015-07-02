// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.iCalendar.CalendarPropertyReader
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.ContentTypes.iCalendar
{
  public struct CalendarPropertyReader
  {
    private Internal.ContentLineReader reader;
    private CalendarValueSeparators lastSeparator;

    internal CalendarValueSeparators LastValueSeparator
    {
      get
      {
        return this.lastSeparator;
      }
    }

    public PropertyId PropertyId
    {
      get
      {
        this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
        return CalendarCommon.GetPropertyEnum(this.reader.PropertyName);
      }
    }

    public CalendarValueType ValueType
    {
      get
      {
        this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
        return (this.reader.ValueType as CalendarValueTypeContainer).ValueType;
      }
    }

    public string Name
    {
      get
      {
        this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
        return this.reader.PropertyName;
      }
    }

    public CalendarParameterReader ParameterReader
    {
      get
      {
        this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
        return new CalendarParameterReader(this.reader);
      }
    }

    internal CalendarPropertyReader(Internal.ContentLineReader reader)
    {
      this.reader = reader;
      this.lastSeparator = CalendarValueSeparators.None;
    }

    internal object ReadValue(CalendarValueSeparators expectedSeparators)
    {
      return this.ReadValue(new CalendarValueSeparators?(expectedSeparators));
    }

    private object ReadValue(CalendarValueSeparators? expectedSeparators)
    {
      do
        ;
      while (this.reader.ReadNextParameter());
      switch (this.ValueType)
      {
        case CalendarValueType.Recurrence:
          return (object) this.ReadValueAsRecurrence(expectedSeparators);
        case CalendarValueType.Time:
          return (object) this.ReadValueAsCalendarTime(expectedSeparators);
        case CalendarValueType.UtcOffset:
          return (object) this.ReadValueAsTimeSpan(expectedSeparators);
        case CalendarValueType.Integer:
          return (object) this.ReadValueAsInt32(expectedSeparators);
        case CalendarValueType.Period:
          return (object) this.ReadValueAsCalendarPeriod(expectedSeparators);
        case CalendarValueType.DateTime:
          return (object) this.ReadValueAsDateTime(expectedSeparators);
        case CalendarValueType.Duration:
          return (object) this.ReadValueAsTimeSpan(expectedSeparators);
        case CalendarValueType.Float:
          return (object) this.ReadValueAsFloat(expectedSeparators);
        case CalendarValueType.Binary:
          return (object) this.ReadValueAsBytes();
        case CalendarValueType.Boolean:
                    return this.ReadValueAsBoolean(expectedSeparators);
                case CalendarValueType.Date:
          return (object) this.ReadValueAsDateTime(expectedSeparators);
        default:
          return (object) this.ReadValueAsString(expectedSeparators);
      }
    }

    public object ReadValue()
    {
      return this.ReadValue(new CalendarValueSeparators?());
    }

    public byte[] ReadValueAsBytes()
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
      using (MemoryStream memoryStream = new MemoryStream(256))
      {
        using (Stream valueReadStream = this.GetValueReadStream())
        {
          byte[] buffer = new byte[256];
          for (int count = valueReadStream.Read(buffer, 0, buffer.Length); count > 0; count = valueReadStream.Read(buffer, 0, buffer.Length))
            memoryStream.Write(buffer, 0, count);
          return memoryStream.ToArray();
        }
      }
    }

    private string ReadValueAsString(CalendarValueSeparators? expectedSeparators)
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
      Internal.ContentLineParser.Separators endSeparator;
      string str = !expectedSeparators.HasValue ? this.reader.ReadPropertyValue(true, Internal.ContentLineParser.Separators.None, true, out endSeparator) : this.reader.ReadPropertyValue(true, (Internal.ContentLineParser.Separators) expectedSeparators.Value, false, out endSeparator);
      this.lastSeparator = (CalendarValueSeparators) endSeparator;
      return str;
    }

    internal string ReadValueAsString(CalendarValueSeparators expectedSeparators)
    {
      return this.ReadValueAsString(new CalendarValueSeparators?(expectedSeparators));
    }

    public string ReadValueAsString()
    {
      return this.ReadValueAsString(new CalendarValueSeparators?());
    }

    private CalendarTime ReadValueAsCalendarTime(CalendarValueSeparators? expectedSeparators)
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
      string s = this.ReadValueAsString(expectedSeparators).Trim();
      this.CheckType(CalendarValueType.Time);
      return new CalendarTime(s, this.reader.ComplianceTracker);
    }

    internal CalendarTime ReadValueAsCalendarTime(CalendarValueSeparators expectedSeparators)
    {
      return this.ReadValueAsCalendarTime(new CalendarValueSeparators?(expectedSeparators));
    }

    public CalendarTime ReadValueAsCalendarTime()
    {
      return this.ReadValueAsCalendarTime(new CalendarValueSeparators?());
    }

    private DateTime ReadValueAsDateTime(CalendarValueSeparators? expectedSeparators)
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
      string s = this.ReadValueAsString(expectedSeparators).Trim();
      if (CalendarValueType.Date == this.ValueType)
        return CalendarCommon.ParseDate(s, this.reader.ComplianceTracker);
      this.CheckType(CalendarValueType.DateTime);
      return CalendarCommon.ParseDateTime(s, this.reader.ComplianceTracker);
    }

    internal DateTime ReadValueAsDateTime(CalendarValueSeparators expectedSeparators)
    {
      return this.ReadValueAsDateTime(new CalendarValueSeparators?(expectedSeparators));
    }

    public DateTime ReadValueAsDateTime()
    {
      return this.ReadValueAsDateTime(new CalendarValueSeparators?());
    }

    private DateTime ReadValueAsDateTime(CalendarValueType valueType, CalendarValueSeparators? expectedSeparators)
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
      string s = this.ReadValueAsString(expectedSeparators).Trim();
      if (CalendarValueType.DateTime == valueType || CalendarValueType.Text == valueType)
      {
        this.CheckType(CalendarValueType.DateTime);
        return CalendarCommon.ParseDateTime(s, this.reader.ComplianceTracker);
      }
      if (CalendarValueType.Date != valueType)
        throw new ArgumentOutOfRangeException("valueType");
      this.CheckType(CalendarValueType.Date);
      return CalendarCommon.ParseDate(s, this.reader.ComplianceTracker);
    }

    internal DateTime ReadValueAsDateTime(CalendarValueType valueType, CalendarValueSeparators expectedSeparators)
    {
      return this.ReadValueAsDateTime(valueType, new CalendarValueSeparators?(expectedSeparators));
    }

    public DateTime ReadValueAsDateTime(CalendarValueType valueType)
    {
      return this.ReadValueAsDateTime(valueType, new CalendarValueSeparators?());
    }

    private TimeSpan ReadValueAsTimeSpan(CalendarValueSeparators? expectedSeparators)
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
      string s = this.ReadValueAsString(expectedSeparators).Trim();
      if (CalendarValueType.UtcOffset == this.ValueType)
        return CalendarCommon.ParseUtcOffset(s, this.reader.ComplianceTracker);
      this.CheckType(CalendarValueType.Duration);
      return CalendarCommon.ParseDuration(s, this.reader.ComplianceTracker);
    }

    internal TimeSpan ReadValueAsTimeSpan(CalendarValueSeparators expectedSeparators)
    {
      return this.ReadValueAsTimeSpan(new CalendarValueSeparators?(expectedSeparators));
    }

    public TimeSpan ReadValueAsTimeSpan()
    {
      return this.ReadValueAsTimeSpan(new CalendarValueSeparators?());
    }

    private TimeSpan ReadValueAsTimeSpan(CalendarValueType valueType, CalendarValueSeparators? expectedSeparators)
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
      string s = this.ReadValueAsString(expectedSeparators).Trim();
      if (CalendarValueType.Duration == valueType)
      {
        this.CheckType(CalendarValueType.Duration);
        return CalendarCommon.ParseDuration(s, this.reader.ComplianceTracker);
      }
      if (CalendarValueType.UtcOffset != valueType)
        throw new ArgumentOutOfRangeException("valueType");
      this.CheckType(CalendarValueType.UtcOffset);
      return CalendarCommon.ParseUtcOffset(s, this.reader.ComplianceTracker);
    }

    internal TimeSpan ReadValueAsTimeSpan(CalendarValueType valueType, CalendarValueSeparators expectedSeparators)
    {
      return this.ReadValueAsTimeSpan(valueType, new CalendarValueSeparators?(expectedSeparators));
    }

    public TimeSpan ReadValueAsTimeSpan(CalendarValueType valueType)
    {
      return this.ReadValueAsTimeSpan(valueType, new CalendarValueSeparators?());
    }

    private bool ReadValueAsBoolean(CalendarValueSeparators? expectedSeparators)
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
      string str = this.ReadValueAsString(expectedSeparators).Trim();
      this.CheckType(CalendarValueType.Boolean);
      bool result;
      if (!bool.TryParse(str, out result))
        this.reader.ComplianceTracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidValueFormat);
      return result;
    }

    internal bool ReadValueAsBoolean(CalendarValueSeparators expectedSeparators)
    {
      return this.ReadValueAsBoolean(new CalendarValueSeparators?(expectedSeparators));
    }

    public bool ReadValueAsBoolean()
    {
      return this.ReadValueAsBoolean(new CalendarValueSeparators?());
    }

    private float ReadValueAsFloat(CalendarValueSeparators? expectedSeparators)
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
      string s = this.ReadValueAsString(expectedSeparators).Trim();
      this.CheckType(CalendarValueType.Float);
      float result;
      if (!float.TryParse(s, NumberStyles.Float, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
        this.reader.ComplianceTracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidValueFormat);
      return result;
    }

    internal float ReadValueAsFloat(CalendarValueSeparators expectedSeparators)
    {
      return this.ReadValueAsFloat(new CalendarValueSeparators?(expectedSeparators));
    }

    public float ReadValueAsFloat()
    {
      return this.ReadValueAsFloat(new CalendarValueSeparators?());
    }

    private double ReadValueAsDouble(CalendarValueSeparators? expectedSeparators)
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
      string s = this.ReadValueAsString(expectedSeparators).Trim();
      this.CheckType(CalendarValueType.Float);
      double result;
      if (!double.TryParse(s, NumberStyles.Float, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
        this.reader.ComplianceTracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidValueFormat);
      return result;
    }

    internal double ReadValueAsDouble(CalendarValueSeparators expectedSeparators)
    {
      return this.ReadValueAsDouble(new CalendarValueSeparators?(expectedSeparators));
    }

    public double ReadValueAsDouble()
    {
      return this.ReadValueAsDouble(new CalendarValueSeparators?());
    }

    private int ReadValueAsInt32(CalendarValueSeparators? expectedSeparators)
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
      string s = this.ReadValueAsString(expectedSeparators).Trim();
      this.CheckType(CalendarValueType.Integer);
      int result;
      if (!int.TryParse(s, NumberStyles.Integer, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
        this.reader.ComplianceTracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidValueFormat);
      return result;
    }

    internal int ReadValueAsInt32(CalendarValueSeparators expectedSeparators)
    {
      return this.ReadValueAsInt32(new CalendarValueSeparators?(expectedSeparators));
    }

    public int ReadValueAsInt32()
    {
      return this.ReadValueAsInt32(new CalendarValueSeparators?());
    }

    private CalendarPeriod ReadValueAsCalendarPeriod(CalendarValueSeparators? expectedSeparators)
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
      string s = this.ReadValueAsString(expectedSeparators).Trim();
      this.CheckType(CalendarValueType.Period);
      return new CalendarPeriod(s, this.reader.ComplianceTracker);
    }

    internal CalendarPeriod ReadValueAsCalendarPeriod(CalendarValueSeparators expectedSeparators)
    {
      return this.ReadValueAsCalendarPeriod(new CalendarValueSeparators?(expectedSeparators));
    }

    public CalendarPeriod ReadValueAsCalendarPeriod()
    {
      return this.ReadValueAsCalendarPeriod(new CalendarValueSeparators?());
    }

    private Recurrence ReadValueAsRecurrence(CalendarValueSeparators? expectedSeparators)
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
      string s = this.ReadValueAsString(expectedSeparators).Trim();
      this.CheckType(CalendarValueType.Recurrence);
      return new Recurrence(s, this.reader.ComplianceTracker);
    }

    internal Recurrence ReadValueAsRecurrence(CalendarValueSeparators expectedSeparators)
    {
      return this.ReadValueAsRecurrence(new CalendarValueSeparators?(expectedSeparators));
    }

    public Recurrence ReadValueAsRecurrence()
    {
      return this.ReadValueAsRecurrence(new CalendarValueSeparators?());
    }

    public bool ReadNextValue()
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property | Internal.ContentLineNodeType.DocumentEnd);
      return this.reader.ReadNextPropertyValue();
    }

    public bool ReadNextProperty()
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.ComponentStart | Internal.ContentLineNodeType.ComponentEnd | Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property | Internal.ContentLineNodeType.BeforeComponentStart | Internal.ContentLineNodeType.BeforeComponentEnd | Internal.ContentLineNodeType.DocumentEnd);
      return this.reader.ReadNextProperty();
    }

    public Stream GetValueReadStream()
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
      this.lastSeparator = CalendarValueSeparators.None;
      return this.reader.GetValueReadStream();
    }

    private void CheckType(CalendarValueType type)
    {
      if (this.ValueType == type || this.ValueType == CalendarValueType.Text)
        return;
      this.reader.ComplianceTracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidValueFormat);
    }
  }
}
