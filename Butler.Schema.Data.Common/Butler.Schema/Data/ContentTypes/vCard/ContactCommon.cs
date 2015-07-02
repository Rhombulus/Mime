// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.vCard.ContactCommon
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.ContentTypes.vCard
{
  internal class ContactCommon
  {
    private static readonly DateTime MinDateTime = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
    private static string[] propertyStringTable = new string[32]
    {
      null,
      "PROFILE",
      "NAME",
      "SOURCE",
      "FN",
      "N",
      "NICKNAME",
      "PHOTO",
      "BDAY",
      "ADR",
      "LABEL",
      "TEL",
      "EMAIL",
      "MAILER",
      "TZ",
      "GEO",
      "TITLE",
      "ROLE",
      "LOGO",
      "AGENT",
      "ORG",
      "CATEGORIES",
      "NOTE",
      "PRODID",
      "REV",
      "SORT-STRING",
      "SOUND",
      "UID",
      "URL",
      "VERSION",
      "CLASS",
      "KEY"
    };
    private static string[] parameterStringTable = new string[6]
    {
      null,
      "TYPE",
      "LANGUAGE",
      "ENCODING",
      "VALUE",
      "CHARSET"
    };
    private static string[] typeStringTable = new string[13]
    {
      null,
      "BINARY",
      "BOOLEAN",
      "DATE",
      "DATE-TIME",
      "FLOAT",
      "INTEGER",
      "TEXT",
      "TIME",
      "URI",
      "UTC-OFFSET",
      "VCARD",
      "PHONE-NUMBER"
    };
    private static ContactValueType[] defaultTypeTable = new ContactValueType[32]
    {
      ContactValueType.Text,
      ContactValueType.Text,
      ContactValueType.Text,
      ContactValueType.Uri,
      ContactValueType.Text,
      ContactValueType.Text,
      ContactValueType.Text,
      ContactValueType.Binary,
      ContactValueType.Date,
      ContactValueType.Text,
      ContactValueType.Text,
      ContactValueType.PhoneNumber,
      ContactValueType.Text,
      ContactValueType.Text,
      ContactValueType.UtcOffset,
      ContactValueType.Float,
      ContactValueType.Text,
      ContactValueType.Text,
      ContactValueType.Binary,
      ContactValueType.VCard,
      ContactValueType.Text,
      ContactValueType.Text,
      ContactValueType.Text,
      ContactValueType.Text,
      ContactValueType.DateTime,
      ContactValueType.Text,
      ContactValueType.Binary,
      ContactValueType.Text,
      ContactValueType.Uri,
      ContactValueType.Text,
      ContactValueType.Text,
      ContactValueType.Binary
    };
    private static Dictionary<string, PropertyId> propertyEnumTable = new Dictionary<string, PropertyId>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static Dictionary<string, ParameterId> parameterEnumTable = new Dictionary<string, ParameterId>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static Dictionary<string, ContactValueType> valueEnumTable = new Dictionary<string, ContactValueType>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private const string DateFormat = "yyyyMMdd";
    private const string DateFormatDash = "yyyy-MM-dd";
    private const string TimeSeparator = "\\T";
    private const string TimeFormat = "HHmmss";
    private const string TimeFormatColon = "HH:mm:ss";
    private const string TimeMsFormat = "\\,fff";
    private const string TimeOffset = "HHmm";
    private const string TimeOffsetColon = "HH:mm";
    private const string UtcSuffix = "\\Z";
    private const string TimeFormatColonMillisecUtc = "HH:mm:ss\\,fff\\Z";
    private const string TimeFormatColonMillisec = "HH:mm:ss\\,fff";
    private const string TimeFormatColonUtc = "HH:mm:ss\\Z";
    private const string DateTimeFormat = "yyyy-MM-dd\\THH:mm:ss";
    private const string DateTimeFormatMillisec = "yyyy-MM-dd\\THH:mm:ss\\,fff";
    private const string DateTimeFormatMillisecUtc = "yyyy-MM-dd\\THH:mm:ss\\,fff\\Z";
    private const string DateTimeFormatUtc = "yyyy-MM-dd\\THH:mm:ss\\Z";
    private const DateTimeStyles VCardDateTimeStyle = DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal;

    static ContactCommon()
    {
      for (int index = 0; index < ContactCommon.propertyStringTable.Length; ++index)
      {
        if (ContactCommon.propertyStringTable[index] != null)
          ContactCommon.propertyEnumTable.Add(ContactCommon.propertyStringTable[index], (PropertyId) index);
      }
      for (int index = 0; index < ContactCommon.parameterStringTable.Length; ++index)
      {
        if (ContactCommon.parameterStringTable[index] != null)
          ContactCommon.parameterEnumTable.Add(ContactCommon.parameterStringTable[index], (ParameterId) index);
      }
      for (int index = 0; index < ContactCommon.typeStringTable.Length; ++index)
      {
        if (ContactCommon.typeStringTable[index] != null)
          ContactCommon.valueEnumTable.Add(ContactCommon.typeStringTable[index], (ContactValueType) index);
      }
    }

    public static string GetPropertyString(PropertyId p)
    {
      uint num = (uint) p;
      if (num < 0U || (long) num >= (long) ContactCommon.propertyStringTable.Length)
        throw new ArgumentOutOfRangeException();
      return ContactCommon.propertyStringTable[num];
    }

    public static string GetValueTypeString(ContactValueType p)
    {
      uint num = (uint) p;
      if (num < 0U || (long) num >= (long) ContactCommon.typeStringTable.Length)
        throw new ArgumentOutOfRangeException();
      return ContactCommon.typeStringTable[num];
    }

    public static string GetParameterString(ParameterId p)
    {
      uint num = (uint) p;
      if (num < 0U || (long) num >= (long) ContactCommon.parameterStringTable.Length)
        throw new ArgumentOutOfRangeException();
      return ContactCommon.parameterStringTable[num];
    }

    public static PropertyId GetPropertyEnum(string p)
    {
      PropertyId propertyId;
      if (ContactCommon.propertyEnumTable.TryGetValue(p, out propertyId))
        return propertyId;
      return PropertyId.Unknown;
    }

    public static ContactValueType GetValueTypeEnum(string c)
    {
      ContactValueType contactValueType;
      if (ContactCommon.valueEnumTable.TryGetValue(c, out contactValueType))
        return contactValueType;
      return ContactValueType.Unknown;
    }

    public static ParameterId GetParameterEnum(string p)
    {
      ParameterId parameterId;
      if (p != null && ContactCommon.parameterEnumTable.TryGetValue(p, out parameterId))
        return parameterId;
      return ParameterId.Unknown;
    }

    public static ContactValueType GetDefaultValueType(PropertyId p)
    {
      uint num = (uint) p;
      if (num < 0U || (long) num >= (long) ContactCommon.defaultTypeTable.Length)
        throw new ArgumentOutOfRangeException();
      return ContactCommon.defaultTypeTable[num];
    }

    public static DateTime ParseDate(string s, Internal.ComplianceTracker tracker)
    {
      DateTime result;
      if (!DateTime.TryParseExact(s, "yyyyMMdd", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result) && !DateTime.TryParseExact(s, "yyyy-MM-dd", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result))
        return ContactCommon.ParseDateTime(s, tracker);
      return result;
    }

    public static DateTime ParseTime(string s, Internal.ComplianceTracker tracker)
    {
      int length = s.Length;
      if (length < 6)
      {
        tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidDateTimeLength);
        return ContactCommon.MinDateTime;
      }
      string format = "HHmmss";
      int formatLength = 6;
      if ((int) s[2] == 58)
      {
        format = "HH:mm:ss";
        formatLength = 8;
      }
      if (length >= formatLength)
        return ContactCommon.InternalParseDateTime(s, length, format, formatLength, tracker);
      tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidDateTimeLength);
      return ContactCommon.MinDateTime;
    }

    public static DateTime ParseDateTime(string s, Internal.ComplianceTracker tracker)
    {
      int length = s.Length;
      if (length < 15)
      {
        tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidDateTimeLength);
        return ContactCommon.MinDateTime;
      }
      string str = "yyyyMMdd";
      int num = 8;
      if ((int) s[4] == 45)
      {
        str = "yyyy-MM-dd";
        num = 10;
      }
      string format;
      int formatLength;
      if ((int) s[num + 3] == 58)
      {
        format = str + "\\THH:mm:ss";
        formatLength = num + 9;
      }
      else
      {
        format = str + "\\THHmmss";
        formatLength = num + 7;
      }
      if (length >= formatLength)
        return ContactCommon.InternalParseDateTime(s, length, format, formatLength, tracker);
      tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidDateTimeLength);
      return ContactCommon.MinDateTime;
    }

    public static TimeSpan ParseUtcOffset(string s, Internal.ComplianceTracker tracker)
    {
      switch (s.Length)
      {
        case 5:
        case 6:
          bool flag = false;
          if ((int) s[0] == 45)
            flag = true;
          else if ((int) s[0] != 43)
          {
            tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.ExpectedPlusMinus);
            return TimeSpan.Zero;
          }
          DateTime result;
          if (!DateTime.TryParseExact(s.Substring(1), "HH:mm", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out result) && !DateTime.TryParseExact(s.Substring(1), "HHmm", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
          {
            tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidValueFormat);
            return TimeSpan.Zero;
          }
          TimeSpan timeSpan = new TimeSpan(result.Hour, result.Minute, 0);
          if (flag)
            return timeSpan.Negate();
          return timeSpan;
        default:
          tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidUtcOffsetLength);
          return TimeSpan.Zero;
      }
    }

    public static string FormatDate(DateTime s)
    {
      return s.ToString("yyyy-MM-dd", (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
    }

    public static string FormatDateTime(DateTime s)
    {
      string format = ContactCommon.RetrieveDateTimeFormatString(s.Millisecond != 0, s.Kind == DateTimeKind.Utc);
      return s.ToString(format, (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
    }

    public static string FormatTime(DateTime s)
    {
      string format = ContactCommon.RetrieveTimeFormatString(s.Millisecond != 0, s.Kind == DateTimeKind.Utc);
      return s.ToString(format, (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
    }

    public static string FormatUtcOffset(TimeSpan ts)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
        return "+00:00";
      if (ts.Ticks > 0L)
      {
        stringBuilder.Append('+');
      }
      else
      {
        stringBuilder.Append('-');
        ts = ts == TimeSpan.MinValue ? TimeSpan.MaxValue : ts.Negate();
      }
      if (ts.Hours < 10)
        stringBuilder.Append('0');
      stringBuilder.Append(ts.Hours.ToString());
      stringBuilder.Append(':');
      if (ts.Minutes < 10)
        stringBuilder.Append('0');
      stringBuilder.Append(ts.Minutes.ToString());
      return stringBuilder.ToString();
    }

    private static DateTime InternalParseDateTime(string s, int length, string format, int formatLength, Internal.ComplianceTracker tracker)
    {
      string s1 = string.Empty;
      string s2 = string.Empty;
      if (length > formatLength)
      {
        if ((int) s[formatLength] == 44)
        {
          int startIndex = formatLength + 1;
          while (startIndex < length && char.IsDigit(s[startIndex]))
            ++startIndex;
          if (startIndex == formatLength + 1)
          {
            tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidDateFormat);
            return ContactCommon.MinDateTime;
          }
          s2 = s.Substring(formatLength + 1, startIndex - (formatLength + 1));
          if (startIndex < length)
            s1 = s.Substring(startIndex);
        }
        else
          s1 = s.Substring(formatLength);
        s = s.Substring(0, formatLength);
      }
      DateTime result1;
      if (!DateTime.TryParseExact(s, format, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result1))
      {
        tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidDateFormat);
        return ContactCommon.MinDateTime;
      }
      if (!string.IsNullOrEmpty(s2))
      {
        if (s2.Length > 3)
          s2 = s2.Substring(0, 3);
        int result2 = 0;
        if (!int.TryParse(s2, out result2))
        {
          tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidDateFormat);
          return ContactCommon.MinDateTime;
        }
        for (int length1 = s2.Length; length1 < 3; ++length1)
          result2 *= 10;
        result1 += new TimeSpan(0, 0, 0, 0, result2);
      }
      if (!string.IsNullOrEmpty(s1) && s1 != "Z")
        result1 += ContactCommon.ParseUtcOffset(s1, tracker);
      return result1;
    }

    private static string RetrieveDateTimeFormatString(bool hasNonZeroMillisecond, bool isUtc)
    {
      if (hasNonZeroMillisecond)
        return !isUtc ? "yyyy-MM-dd\\THH:mm:ss\\,fff" : "yyyy-MM-dd\\THH:mm:ss\\,fff\\Z";
      return !isUtc ? "yyyy-MM-dd\\THH:mm:ss" : "yyyy-MM-dd\\THH:mm:ss\\Z";
    }

    private static string RetrieveTimeFormatString(bool hasNonZeroMillisecond, bool isUtc)
    {
      if (hasNonZeroMillisecond)
        return !isUtc ? "HH:mm:ss\\,fff" : "HH:mm:ss\\,fff\\Z";
      return !isUtc ? "HH:mm:ss" : "HH:mm:ss\\Z";
    }
  }
}
