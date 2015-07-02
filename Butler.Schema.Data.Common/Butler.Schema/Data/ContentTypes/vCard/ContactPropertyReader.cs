// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.vCard.ContactPropertyReader
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.ContentTypes.vCard
{
  public struct ContactPropertyReader
  {
    private Internal.ContentLineReader reader;
    private ContactValueSeparators lastSeparator;

    public ContactValueSeparators LastValueSeparator
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
        return ContactCommon.GetPropertyEnum(this.reader.PropertyName);
      }
    }

    public ContactValueType ValueType
    {
      get
      {
        this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
        return (this.reader.ValueType as ContactValueTypeContainer).ValueType;
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

    public ContactParameterReader ParameterReader
    {
      get
      {
        this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
        return new ContactParameterReader(this.reader);
      }
    }

    internal ContactPropertyReader(Internal.ContentLineReader reader)
    {
      this.reader = reader;
      this.lastSeparator = ContactValueSeparators.None;
    }

    public object ReadValue(ContactValueSeparators expectedSeparators)
    {
      return this.ReadValue(new ContactValueSeparators?(expectedSeparators));
    }

    private object ReadValue(ContactValueSeparators? expectedSeparators)
    {
      do
        ;
      while (this.reader.ReadNextParameter());
      switch (this.ValueType)
      {
        case ContactValueType.Binary:
          return (object) this.ReadValueAsBytes();
        case ContactValueType.Boolean:
              return this.ReadValueAsBoolean(expectedSeparators);
        case ContactValueType.Date:
        case ContactValueType.DateTime:
        case ContactValueType.Time:
          return (object) this.ReadValueAsDateTime(this.ValueType, expectedSeparators);
        case ContactValueType.Float:
          return (object) this.ReadValueAsDouble(expectedSeparators);
        case ContactValueType.Integer:
          return (object) this.ReadValueAsInt32(expectedSeparators);
        case ContactValueType.UtcOffset:
          return (object) this.ReadValueAsTimeSpan(expectedSeparators);
        default:
          return (object) this.ReadValueAsString(expectedSeparators);
      }
    }

    public object ReadValue()
    {
      return this.ReadValue(new ContactValueSeparators?());
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

    private string ReadValueAsString(ContactValueSeparators? expectedSeparators)
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
      Internal.ContentLineParser.Separators endSeparator;
      string str = !expectedSeparators.HasValue ? this.reader.ReadPropertyValue(true, Internal.ContentLineParser.Separators.None, true, out endSeparator) : this.reader.ReadPropertyValue(true, (Internal.ContentLineParser.Separators) expectedSeparators.Value, false, out endSeparator);
      this.lastSeparator = (ContactValueSeparators) endSeparator;
      return str;
    }

    public string ReadValueAsString(ContactValueSeparators expectedSeparators)
    {
      return this.ReadValueAsString(new ContactValueSeparators?(expectedSeparators));
    }

    public string ReadValueAsString()
    {
      return this.ReadValueAsString(new ContactValueSeparators?());
    }

    private DateTime ReadValueAsDateTime(ContactValueType type, ContactValueSeparators? expectedSeparators)
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
      string s = this.ReadValueAsString(expectedSeparators).Trim();
      this.CheckType(type);
      if (type == ContactValueType.DateTime)
        return ContactCommon.ParseDateTime(s, this.reader.ComplianceTracker);
      if (type == ContactValueType.Time)
        return ContactCommon.ParseTime(s, this.reader.ComplianceTracker);
      return ContactCommon.ParseDate(s, this.reader.ComplianceTracker);
    }

    public DateTime ReadValueAsDateTime(ContactValueType type, ContactValueSeparators expectedSeparators)
    {
      return this.ReadValueAsDateTime(type, new ContactValueSeparators?(expectedSeparators));
    }

    public DateTime ReadValueAsDateTime(ContactValueType type)
    {
      return this.ReadValueAsDateTime(type, new ContactValueSeparators?());
    }

    private TimeSpan ReadValueAsTimeSpan(ContactValueSeparators? expectedSeparators)
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
      string s = this.ReadValueAsString(expectedSeparators).Trim();
      this.CheckType(ContactValueType.UtcOffset);
      return ContactCommon.ParseUtcOffset(s, this.reader.ComplianceTracker);
    }

    public TimeSpan ReadValueAsTimeSpan(ContactValueSeparators expectedSeparators)
    {
      return this.ReadValueAsTimeSpan(new ContactValueSeparators?(expectedSeparators));
    }

    public TimeSpan ReadValueAsTimeSpan()
    {
      return this.ReadValueAsTimeSpan(new ContactValueSeparators?());
    }

    private bool ReadValueAsBoolean(ContactValueSeparators? expectedSeparators)
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
      string str = this.ReadValueAsString(expectedSeparators).Trim();
      this.CheckType(ContactValueType.Boolean);
      bool result;
      if (!bool.TryParse(str, out result))
        this.reader.ComplianceTracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidValueFormat);
      return result;
    }

    public bool ReadValueAsBoolean(ContactValueSeparators expectedSeparators)
    {
      return this.ReadValueAsBoolean(new ContactValueSeparators?(expectedSeparators));
    }

    public bool ReadValueAsBoolean()
    {
      return this.ReadValueAsBoolean(new ContactValueSeparators?());
    }

    private double ReadValueAsDouble(ContactValueSeparators? expectedSeparators)
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
      string s = this.ReadValueAsString(expectedSeparators).Trim();
      this.CheckType(ContactValueType.Float);
      double result;
      if (!double.TryParse(s, NumberStyles.Float, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
        this.reader.ComplianceTracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidValueFormat);
      return result;
    }

    public double ReadValueAsDouble(ContactValueSeparators expectedSeparators)
    {
      return this.ReadValueAsDouble(new ContactValueSeparators?(expectedSeparators));
    }

    public double ReadValueAsDouble()
    {
      return this.ReadValueAsDouble(new ContactValueSeparators?());
    }

    private int ReadValueAsInt32(ContactValueSeparators? expectedSeparators)
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
      string s = this.ReadValueAsString(expectedSeparators).Trim();
      this.CheckType(ContactValueType.Integer);
      int result;
      if (!int.TryParse(s, NumberStyles.Integer, (IFormatProvider) NumberFormatInfo.InvariantInfo, out result))
        this.reader.ComplianceTracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidValueFormat);
      return result;
    }

    public int ReadValueAsInt32(ContactValueSeparators expectedSeparators)
    {
      return this.ReadValueAsInt32(new ContactValueSeparators?(expectedSeparators));
    }

    public int ReadValueAsInt32()
    {
      return this.ReadValueAsInt32(new ContactValueSeparators?());
    }

    public bool ReadNextValue()
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property | Internal.ContentLineNodeType.DocumentEnd);
      return this.reader.ReadNextPropertyValue();
    }

    public bool ReadNextProperty()
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.ComponentStart | Internal.ContentLineNodeType.ComponentEnd | Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property | Internal.ContentLineNodeType.BeforeComponentStart | Internal.ContentLineNodeType.BeforeComponentEnd | Internal.ContentLineNodeType.DocumentEnd);
      while (this.reader.ReadNextProperty())
      {
        if (this.Name != string.Empty)
          return true;
      }
      return false;
    }

    public void ApplyValueOverrides(Encoding charset, Mime.Encoders.ByteEncoder decoder)
    {
      this.reader.ApplyValueOverrides(charset, decoder);
    }

    public Stream GetValueReadStream()
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property);
      this.lastSeparator = ContactValueSeparators.None;
      return this.reader.GetValueReadStream();
    }

    private void CheckType(ContactValueType type)
    {
      if (this.ValueType == type || this.ValueType == ContactValueType.Text)
        return;
      this.reader.ComplianceTracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidValueFormat);
    }
  }
}
