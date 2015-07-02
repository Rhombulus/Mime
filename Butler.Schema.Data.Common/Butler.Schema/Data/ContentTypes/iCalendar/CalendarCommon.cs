// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.iCalendar.CalendarCommon
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.ContentTypes.iCalendar
{
  internal class CalendarCommon
  {
    internal static readonly DateTime MinDateTime = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
    private static Dictionary<ParameterId, string> parameterStringTable = new Dictionary<ParameterId, string>((IEqualityComparer<ParameterId>) new CalendarCommon.ParameterIdComparer());
    private static Dictionary<ComponentId, string> componentStringTable = new Dictionary<ComponentId, string>((IEqualityComparer<ComponentId>) new CalendarCommon.ComponentIdComparer());
    private static Dictionary<PropertyId, CalendarValueType> defaultValueTypeTable = new Dictionary<PropertyId, CalendarValueType>((IEqualityComparer<PropertyId>) new CalendarCommon.PropertyIdComparer());
    private static string[] propertyStringTable = new string[48]
    {
      null,
      "PRODID",
      "VERSION",
      "CALSCALE",
      "METHOD",
      "ATTACH",
      "CATEGORIES",
      "CLASS",
      "COMMENT",
      "DESCRIPTION",
      "GEO",
      "LOCATION",
      "PERCENT-COMPLETE",
      "PRIORITY",
      "RESOURCES",
      "STATUS",
      "SUMMARY",
      "COMPLETED",
      "DTEND",
      "DUE",
      "DTSTART",
      "DURATION",
      "FREEBUSY",
      "TRANSP",
      "TZID",
      "TZNAME",
      "TZOFFSETFROM",
      "TZOFFSETTO",
      "TZURL",
      "ATTENDEE",
      "CONTACT",
      "ORGANIZER",
      "RECURRENCE-ID",
      "RELATED-TO",
      "URL",
      "UID",
      "EXDATE",
      "EXRULE",
      "RDATE",
      "RRULE",
      "ACTION",
      "REPEAT",
      "TRIGGER",
      "CREATED",
      "DTSTAMP",
      "LAST-MODIFIED",
      "SEQUENCE",
      "REQUEST-STATUS"
    };
    private static Dictionary<string, ComponentId> componentEnumTable = new Dictionary<string, ComponentId>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static Dictionary<string, PropertyId> propertyEnumTable = new Dictionary<string, PropertyId>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static Dictionary<string, ParameterId> parameterEnumTable = new Dictionary<string, ParameterId>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static Dictionary<string, CalendarValueType> valueEnumTable = new Dictionary<string, CalendarValueType>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private const string DateFormat = "yyyyMMdd";
    private const string DateTimeFormat = "yyyyMMdd\\THHmmss";
    private const string DateTimeFormatUtc = "yyyyMMdd\\THHmmss\\Z";

    static CalendarCommon()
    {
      CalendarCommon.parameterStringTable.Add(ParameterId.AlternateRepresentation, "ALTREP");
      CalendarCommon.parameterStringTable.Add(ParameterId.CommonName, "CN");
      CalendarCommon.parameterStringTable.Add(ParameterId.CalendarUserType, "CUTYPE");
      CalendarCommon.parameterStringTable.Add(ParameterId.Delegator, "DELEGATED-FROM");
      CalendarCommon.parameterStringTable.Add(ParameterId.Delegatee, "DELEGATED-TO");
      CalendarCommon.parameterStringTable.Add(ParameterId.Directory, "DIR");
      CalendarCommon.parameterStringTable.Add(ParameterId.Encoding, "ENCODING");
      CalendarCommon.parameterStringTable.Add(ParameterId.FormatType, "FMTTYPE");
      CalendarCommon.parameterStringTable.Add(ParameterId.FreeBusyType, "FBTYPE");
      CalendarCommon.parameterStringTable.Add(ParameterId.Language, "LANGUAGE");
      CalendarCommon.parameterStringTable.Add(ParameterId.Membership, "MEMBER");
      CalendarCommon.parameterStringTable.Add(ParameterId.ParticipationStatus, "PARTSTAT");
      CalendarCommon.parameterStringTable.Add(ParameterId.RecurrenceRange, "RANGE");
      CalendarCommon.parameterStringTable.Add(ParameterId.TriggerRelationship, "RELATED");
      CalendarCommon.parameterStringTable.Add(ParameterId.RelationshipType, "RELTYPE");
      CalendarCommon.parameterStringTable.Add(ParameterId.ParticipationRole, "ROLE");
      CalendarCommon.parameterStringTable.Add(ParameterId.RsvpExpectation, "RSVP");
      CalendarCommon.parameterStringTable.Add(ParameterId.SentBy, "SENT-BY");
      CalendarCommon.parameterStringTable.Add(ParameterId.TimeZoneId, "TZID");
      CalendarCommon.parameterStringTable.Add(ParameterId.ValueType, "VALUE");
      CalendarCommon.componentStringTable.Add(ComponentId.VCalendar, "VCALENDAR");
      CalendarCommon.componentStringTable.Add(ComponentId.VEvent, "VEVENT");
      CalendarCommon.componentStringTable.Add(ComponentId.VTodo, "VTODO");
      CalendarCommon.componentStringTable.Add(ComponentId.VJournal, "VJOURNAL");
      CalendarCommon.componentStringTable.Add(ComponentId.VFreeBusy, "VFREEBUSY");
      CalendarCommon.componentStringTable.Add(ComponentId.VTimeZone, "VTIMEZONE");
      CalendarCommon.componentStringTable.Add(ComponentId.VAlarm, "VALARM");
      CalendarCommon.componentStringTable.Add(ComponentId.Standard, "STANDARD");
      CalendarCommon.componentStringTable.Add(ComponentId.Daylight, "DAYLIGHT");
      CalendarCommon.componentEnumTable.Add("NONE", ComponentId.None);
      CalendarCommon.componentEnumTable.Add("VCALENDAR", ComponentId.VCalendar);
      CalendarCommon.componentEnumTable.Add("VEVENT", ComponentId.VEvent);
      CalendarCommon.componentEnumTable.Add("VTODO", ComponentId.VTodo);
      CalendarCommon.componentEnumTable.Add("VJOURNAL", ComponentId.VJournal);
      CalendarCommon.componentEnumTable.Add("VFREEBUSY", ComponentId.VFreeBusy);
      CalendarCommon.componentEnumTable.Add("VTIMEZONE", ComponentId.VTimeZone);
      CalendarCommon.componentEnumTable.Add("VALARM", ComponentId.VAlarm);
      CalendarCommon.componentEnumTable.Add("STANDARD", ComponentId.Standard);
      CalendarCommon.componentEnumTable.Add("DAYLIGHT", ComponentId.Daylight);
      CalendarCommon.propertyEnumTable.Add("PRODID", PropertyId.ProductId);
      CalendarCommon.propertyEnumTable.Add("VERSION", PropertyId.Version);
      CalendarCommon.propertyEnumTable.Add("CALSCALE", PropertyId.CalendarScale);
      CalendarCommon.propertyEnumTable.Add("METHOD", PropertyId.Method);
      CalendarCommon.propertyEnumTable.Add("ATTACH", PropertyId.Attachment);
      CalendarCommon.propertyEnumTable.Add("CATEGORIES", PropertyId.Categories);
      CalendarCommon.propertyEnumTable.Add("CLASS", PropertyId.Class);
      CalendarCommon.propertyEnumTable.Add("COMMENT", PropertyId.Comment);
      CalendarCommon.propertyEnumTable.Add("DESCRIPTION", PropertyId.Description);
      CalendarCommon.propertyEnumTable.Add("GEO", PropertyId.GeographicPosition);
      CalendarCommon.propertyEnumTable.Add("LOCATION", PropertyId.Location);
      CalendarCommon.propertyEnumTable.Add("PERCENT-COMPLETE", PropertyId.PercentComplete);
      CalendarCommon.propertyEnumTable.Add("PRIORITY", PropertyId.Priority);
      CalendarCommon.propertyEnumTable.Add("RESOURCES", PropertyId.Resources);
      CalendarCommon.propertyEnumTable.Add("STATUS", PropertyId.Status);
      CalendarCommon.propertyEnumTable.Add("SUMMARY", PropertyId.Summary);
      CalendarCommon.propertyEnumTable.Add("COMPLETED", PropertyId.Completed);
      CalendarCommon.propertyEnumTable.Add("DTEND", PropertyId.DateTimeEnd);
      CalendarCommon.propertyEnumTable.Add("DUE", PropertyId.DateTimeDue);
      CalendarCommon.propertyEnumTable.Add("DTSTART", PropertyId.DateTimeStart);
      CalendarCommon.propertyEnumTable.Add("DURATION", PropertyId.Duration);
      CalendarCommon.propertyEnumTable.Add("FREEBUSY", PropertyId.FreeBusy);
      CalendarCommon.propertyEnumTable.Add("TRANSP", PropertyId.Transparency);
      CalendarCommon.propertyEnumTable.Add("TZID", PropertyId.TimeZoneId);
      CalendarCommon.propertyEnumTable.Add("TZNAME", PropertyId.TimeZoneName);
      CalendarCommon.propertyEnumTable.Add("TZOFFSETFROM", PropertyId.TimeZoneOffsetFrom);
      CalendarCommon.propertyEnumTable.Add("TZOFFSETTO", PropertyId.TimeZoneOffsetTo);
      CalendarCommon.propertyEnumTable.Add("TZURL", PropertyId.TimeZoneUrl);
      CalendarCommon.propertyEnumTable.Add("ATTENDEE", PropertyId.Attendee);
      CalendarCommon.propertyEnumTable.Add("CONTACT", PropertyId.Contact);
      CalendarCommon.propertyEnumTable.Add("ORGANIZER", PropertyId.Organizer);
      CalendarCommon.propertyEnumTable.Add("RECURRENCE-ID", PropertyId.RecurrenceId);
      CalendarCommon.propertyEnumTable.Add("RELATED-TO", PropertyId.RelatedTo);
      CalendarCommon.propertyEnumTable.Add("URL", PropertyId.Url);
      CalendarCommon.propertyEnumTable.Add("UID", PropertyId.Uid);
      CalendarCommon.propertyEnumTable.Add("EXDATE", PropertyId.ExceptionDate);
      CalendarCommon.propertyEnumTable.Add("EXRULE", PropertyId.ExceptionRule);
      CalendarCommon.propertyEnumTable.Add("RDATE", PropertyId.RecurrenceDate);
      CalendarCommon.propertyEnumTable.Add("RRULE", PropertyId.RecurrenceRule);
      CalendarCommon.propertyEnumTable.Add("ACTION", PropertyId.Action);
      CalendarCommon.propertyEnumTable.Add("REPEAT", PropertyId.Repeat);
      CalendarCommon.propertyEnumTable.Add("TRIGGER", PropertyId.Trigger);
      CalendarCommon.propertyEnumTable.Add("CREATED", PropertyId.Created);
      CalendarCommon.propertyEnumTable.Add("DTSTAMP", PropertyId.DateTimeStamp);
      CalendarCommon.propertyEnumTable.Add("LAST-MODIFIED", PropertyId.LastModified);
      CalendarCommon.propertyEnumTable.Add("SEQUENCE", PropertyId.Sequence);
      CalendarCommon.propertyEnumTable.Add("REQUEST-STATUS", PropertyId.RequestStatus);
      CalendarCommon.parameterEnumTable.Add("ALTREP", ParameterId.AlternateRepresentation);
      CalendarCommon.parameterEnumTable.Add("CN", ParameterId.CommonName);
      CalendarCommon.parameterEnumTable.Add("CUTYPE", ParameterId.CalendarUserType);
      CalendarCommon.parameterEnumTable.Add("DELEGATED-FROM", ParameterId.Delegator);
      CalendarCommon.parameterEnumTable.Add("DELEGATED-TO", ParameterId.Delegatee);
      CalendarCommon.parameterEnumTable.Add("DIR", ParameterId.Directory);
      CalendarCommon.parameterEnumTable.Add("ENCODING", ParameterId.Encoding);
      CalendarCommon.parameterEnumTable.Add("FMTTYPE", ParameterId.FormatType);
      CalendarCommon.parameterEnumTable.Add("FBTYPE", ParameterId.FreeBusyType);
      CalendarCommon.parameterEnumTable.Add("LANGUAGE", ParameterId.Language);
      CalendarCommon.parameterEnumTable.Add("MEMBER", ParameterId.Membership);
      CalendarCommon.parameterEnumTable.Add("PARTSTAT", ParameterId.ParticipationStatus);
      CalendarCommon.parameterEnumTable.Add("RANGE", ParameterId.RecurrenceRange);
      CalendarCommon.parameterEnumTable.Add("RELATED", ParameterId.TriggerRelationship);
      CalendarCommon.parameterEnumTable.Add("RELTYPE", ParameterId.RelationshipType);
      CalendarCommon.parameterEnumTable.Add("ROLE", ParameterId.ParticipationRole);
      CalendarCommon.parameterEnumTable.Add("RSVP", ParameterId.RsvpExpectation);
      CalendarCommon.parameterEnumTable.Add("SENT-BY", ParameterId.SentBy);
      CalendarCommon.parameterEnumTable.Add("TZID", ParameterId.TimeZoneId);
      CalendarCommon.parameterEnumTable.Add("VALUE", ParameterId.ValueType);
      CalendarCommon.valueEnumTable.Add("BINARY", CalendarValueType.Binary);
      CalendarCommon.valueEnumTable.Add("BOOLEAN", CalendarValueType.Boolean);
      CalendarCommon.valueEnumTable.Add("CAL-ADDRESS", CalendarValueType.CalAddress);
      CalendarCommon.valueEnumTable.Add("DATE", CalendarValueType.Date);
      CalendarCommon.valueEnumTable.Add("DATE-TIME", CalendarValueType.DateTime);
      CalendarCommon.valueEnumTable.Add("DURATION", CalendarValueType.Duration);
      CalendarCommon.valueEnumTable.Add("FLOAT", CalendarValueType.Float);
      CalendarCommon.valueEnumTable.Add("INTEGER", CalendarValueType.Integer);
      CalendarCommon.valueEnumTable.Add("PERIOD", CalendarValueType.Period);
      CalendarCommon.valueEnumTable.Add("RECUR", CalendarValueType.Recurrence);
      CalendarCommon.valueEnumTable.Add("TEXT", CalendarValueType.Text);
      CalendarCommon.valueEnumTable.Add("TIME", CalendarValueType.Time);
      CalendarCommon.valueEnumTable.Add("URI", CalendarValueType.Uri);
      CalendarCommon.valueEnumTable.Add("UTC-OFFSET", CalendarValueType.UtcOffset);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.Attachment, CalendarValueType.Uri);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.PercentComplete, CalendarValueType.Integer);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.Priority, CalendarValueType.Integer);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.Completed, CalendarValueType.DateTime);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.DateTimeEnd, CalendarValueType.DateTime);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.DateTimeStart, CalendarValueType.DateTime);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.DateTimeDue, CalendarValueType.DateTime);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.Duration, CalendarValueType.Duration);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.FreeBusy, CalendarValueType.Period);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.TimeZoneOffsetFrom, CalendarValueType.UtcOffset);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.TimeZoneOffsetTo, CalendarValueType.UtcOffset);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.TimeZoneUrl, CalendarValueType.Uri);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.Attendee, CalendarValueType.CalAddress);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.Organizer, CalendarValueType.CalAddress);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.RecurrenceId, CalendarValueType.DateTime);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.Url, CalendarValueType.Uri);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.ExceptionDate, CalendarValueType.DateTime);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.ExceptionRule, CalendarValueType.Recurrence);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.RecurrenceDate, CalendarValueType.DateTime);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.RecurrenceRule, CalendarValueType.Recurrence);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.Repeat, CalendarValueType.Integer);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.Trigger, CalendarValueType.Duration);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.Created, CalendarValueType.DateTime);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.DateTimeStamp, CalendarValueType.DateTime);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.LastModified, CalendarValueType.DateTime);
      CalendarCommon.defaultValueTypeTable.Add(PropertyId.Sequence, CalendarValueType.Integer);
    }

    public static string GetPropertyString(PropertyId p)
    {
      uint num = (uint) p;
      if (num < 0U || (long) num >= (long) CalendarCommon.propertyStringTable.Length)
        return (string) null;
      return CalendarCommon.propertyStringTable[num];
    }

    public static string GetComponentString(ComponentId c)
    {
      return CalendarCommon.componentStringTable[c];
    }

    public static string GetParameterString(ParameterId p)
    {
      return CalendarCommon.parameterStringTable[p];
    }

    public static PropertyId GetPropertyEnum(string p)
    {
      PropertyId propertyId;
      if (CalendarCommon.propertyEnumTable.TryGetValue(p, out propertyId))
        return propertyId;
      return PropertyId.Unknown;
    }

    public static ComponentId GetComponentEnum(string c)
    {
      ComponentId componentId;
      if (CalendarCommon.componentEnumTable.TryGetValue(c, out componentId))
        return componentId;
      return ComponentId.Unknown;
    }

    public static ParameterId GetParameterEnum(string p)
    {
      ParameterId parameterId;
      if (CalendarCommon.parameterEnumTable.TryGetValue(p, out parameterId))
        return parameterId;
      return ParameterId.Unknown;
    }

    public static CalendarValueType GetValueTypeEnum(string v)
    {
      CalendarValueType calendarValueType;
      if (CalendarCommon.valueEnumTable.TryGetValue(v, out calendarValueType))
        return calendarValueType;
      return CalendarValueType.Unknown;
    }

    public static CalendarValueType GetDefaultValueType(PropertyId p)
    {
      CalendarValueType calendarValueType;
      if (CalendarCommon.defaultValueTypeTable.TryGetValue(p, out calendarValueType))
        return calendarValueType;
      return CalendarValueType.Text;
    }

    public static DateTime ParseDate(string s, Internal.ComplianceTracker tracker)
    {
      DateTime result;
      if (DateTime.TryParseExact(s, "yyyyMMdd", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
        return result;
      tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidDateFormat);
      return CalendarCommon.MinDateTime;
    }

    public static DateTime ParseDateTime(string s, Internal.ComplianceTracker tracker)
    {
      switch (s.Length)
      {
        case 15:
        case 16:
          DateTime dateTime = CalendarCommon.ParseDate(s.Substring(0, 8), tracker);
          if ((int) s[8] != 84)
          {
            tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.ExpectedTAfterDate);
            return CalendarCommon.MinDateTime;
          }
          CalendarTime calendarTime = new CalendarTime(s.Substring(9), tracker);
          return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, calendarTime.Time.Hours, calendarTime.Time.Minutes, calendarTime.Time.Seconds, calendarTime.IsUtc ? DateTimeKind.Utc : DateTimeKind.Unspecified);
        default:
          tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidDateTimeLength);
          return CalendarCommon.MinDateTime;
      }
    }

    public static TimeSpan ParseDuration(string s, Internal.ComplianceTracker tracker)
    {
      int index = 0;
      int length = s.Length;
      CalendarCommon.DurationParseStates durationParseStates = CalendarCommon.DurationParseStates.Start;
      StringBuilder stringBuilder = (StringBuilder) null;
      int result1 = 0;
      int result2 = 0;
      int result3 = 0;
      int result4 = 0;
      int result5 = 0;
      bool negative = false;
      for (; index < length && durationParseStates != CalendarCommon.DurationParseStates.End; ++index)
      {
        char ch = s[index];
        if ((int) ch >= Internal.ContentLineParser.Dictionary.Length)
        {
          tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidCharacter);
          return TimeSpan.Zero;
        }
        switch (durationParseStates)
        {
          case CalendarCommon.DurationParseStates.Start:
            if ((int) ch == 80)
            {
              durationParseStates = CalendarCommon.DurationParseStates.S1;
              break;
            }
            if ((int) ch == 43)
            {
              durationParseStates = CalendarCommon.DurationParseStates.Sign;
              break;
            }
            if ((int) ch == 45)
            {
              negative = true;
              durationParseStates = CalendarCommon.DurationParseStates.Sign;
              break;
            }
            tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidCharacter);
            return TimeSpan.Zero;
          case CalendarCommon.DurationParseStates.Sign:
            if ((int) ch == 80)
            {
              durationParseStates = CalendarCommon.DurationParseStates.S1;
              break;
            }
            tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.ExpectedP);
            return TimeSpan.Zero;
          case CalendarCommon.DurationParseStates.S1:
            if ((Internal.ContentLineParser.Tokens.Digit & Internal.ContentLineParser.Dictionary[(int) ch]) != ~(Internal.ContentLineParser.Tokens.CTL | Internal.ContentLineParser.Tokens.Alpha | Internal.ContentLineParser.Tokens.Digit | Internal.ContentLineParser.Tokens.SafeChar | Internal.ContentLineParser.Tokens.QSafeChar | Internal.ContentLineParser.Tokens.ValueChar | Internal.ContentLineParser.Tokens.WSP | Internal.ContentLineParser.Tokens.NonASCII))
            {
              stringBuilder = new StringBuilder();
              stringBuilder.Append(ch);
              durationParseStates = CalendarCommon.DurationParseStates.S2;
              break;
            }
            if ((int) ch == 84)
            {
              durationParseStates = CalendarCommon.DurationParseStates.T1;
              break;
            }
            tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidCharacter);
            return TimeSpan.Zero;
          case CalendarCommon.DurationParseStates.S2:
            while ((Internal.ContentLineParser.Tokens.Digit & Internal.ContentLineParser.Dictionary[(int) ch]) != ~(Internal.ContentLineParser.Tokens.CTL | Internal.ContentLineParser.Tokens.Alpha | Internal.ContentLineParser.Tokens.Digit | Internal.ContentLineParser.Tokens.SafeChar | Internal.ContentLineParser.Tokens.QSafeChar | Internal.ContentLineParser.Tokens.ValueChar | Internal.ContentLineParser.Tokens.WSP | Internal.ContentLineParser.Tokens.NonASCII))
            {
              stringBuilder.Append(ch);
              if (++index == length)
              {
                tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.ExpectedWOrD);
                return TimeSpan.Zero;
              }
              ch = s[index];
              if ((int) ch >= Internal.ContentLineParser.Dictionary.Length)
              {
                tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidCharacter);
                return TimeSpan.Zero;
              }
            }
            if ((int) ch == 87)
            {
              if (!int.TryParse(stringBuilder.ToString(), out result2))
                tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidValueFormat);
              durationParseStates = CalendarCommon.DurationParseStates.End;
              break;
            }
            if ((int) ch == 68)
            {
              if (!int.TryParse(stringBuilder.ToString(), out result1))
                tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidValueFormat);
              durationParseStates = CalendarCommon.DurationParseStates.D1;
              break;
            }
            tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidCharacter);
            return TimeSpan.Zero;
          case CalendarCommon.DurationParseStates.D1:
            if ((int) ch == 84)
            {
              durationParseStates = CalendarCommon.DurationParseStates.T1;
              break;
            }
            tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.ExpectedT);
            return TimeSpan.Zero;
          case CalendarCommon.DurationParseStates.T1:
            if ((Internal.ContentLineParser.Tokens.Digit & Internal.ContentLineParser.Dictionary[(int) ch]) != ~(Internal.ContentLineParser.Tokens.CTL | Internal.ContentLineParser.Tokens.Alpha | Internal.ContentLineParser.Tokens.Digit | Internal.ContentLineParser.Tokens.SafeChar | Internal.ContentLineParser.Tokens.QSafeChar | Internal.ContentLineParser.Tokens.ValueChar | Internal.ContentLineParser.Tokens.WSP | Internal.ContentLineParser.Tokens.NonASCII))
            {
              stringBuilder = new StringBuilder();
              stringBuilder.Append(ch);
              durationParseStates = CalendarCommon.DurationParseStates.T2;
              break;
            }
            tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidCharacter);
            return TimeSpan.Zero;
          case CalendarCommon.DurationParseStates.T2:
            while ((Internal.ContentLineParser.Tokens.Digit & Internal.ContentLineParser.Dictionary[(int) ch]) != ~(Internal.ContentLineParser.Tokens.CTL | Internal.ContentLineParser.Tokens.Alpha | Internal.ContentLineParser.Tokens.Digit | Internal.ContentLineParser.Tokens.SafeChar | Internal.ContentLineParser.Tokens.QSafeChar | Internal.ContentLineParser.Tokens.ValueChar | Internal.ContentLineParser.Tokens.WSP | Internal.ContentLineParser.Tokens.NonASCII))
            {
              stringBuilder.Append(ch);
              if (++index == length)
              {
                tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.ExpectedHMS);
                return TimeSpan.Zero;
              }
              ch = s[index];
              if ((int) ch >= Internal.ContentLineParser.Dictionary.Length)
              {
                tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidCharacter);
                return TimeSpan.Zero;
              }
            }
            if ((int) ch == 72)
            {
              if (!int.TryParse(stringBuilder.ToString(), out result3))
                tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidValueFormat);
              durationParseStates = CalendarCommon.DurationParseStates.H1;
              break;
            }
            if ((int) ch == 77)
            {
              if (!int.TryParse(stringBuilder.ToString(), out result4))
                tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidValueFormat);
              durationParseStates = CalendarCommon.DurationParseStates.M1;
              break;
            }
            if ((int) ch == 83)
            {
              if (!int.TryParse(stringBuilder.ToString(), out result5))
                tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidValueFormat);
              durationParseStates = CalendarCommon.DurationParseStates.End;
              break;
            }
            tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidCharacter);
            return TimeSpan.Zero;
          case CalendarCommon.DurationParseStates.H1:
            if ((Internal.ContentLineParser.Tokens.Digit & Internal.ContentLineParser.Dictionary[(int) ch]) != ~(Internal.ContentLineParser.Tokens.CTL | Internal.ContentLineParser.Tokens.Alpha | Internal.ContentLineParser.Tokens.Digit | Internal.ContentLineParser.Tokens.SafeChar | Internal.ContentLineParser.Tokens.QSafeChar | Internal.ContentLineParser.Tokens.ValueChar | Internal.ContentLineParser.Tokens.WSP | Internal.ContentLineParser.Tokens.NonASCII))
            {
              stringBuilder = new StringBuilder();
              stringBuilder.Append(ch);
              durationParseStates = CalendarCommon.DurationParseStates.H2;
              break;
            }
            tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidCharacter);
            return TimeSpan.Zero;
          case CalendarCommon.DurationParseStates.H2:
            while ((Internal.ContentLineParser.Tokens.Digit & Internal.ContentLineParser.Dictionary[(int) ch]) != ~(Internal.ContentLineParser.Tokens.CTL | Internal.ContentLineParser.Tokens.Alpha | Internal.ContentLineParser.Tokens.Digit | Internal.ContentLineParser.Tokens.SafeChar | Internal.ContentLineParser.Tokens.QSafeChar | Internal.ContentLineParser.Tokens.ValueChar | Internal.ContentLineParser.Tokens.WSP | Internal.ContentLineParser.Tokens.NonASCII))
            {
              stringBuilder.Append(ch);
              if (++index == length)
              {
                tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.ExpectedM);
                return TimeSpan.Zero;
              }
              ch = s[index];
              if ((int) ch >= Internal.ContentLineParser.Dictionary.Length)
              {
                tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidCharacter);
                return TimeSpan.Zero;
              }
            }
            if ((int) ch == 77)
            {
              if (!int.TryParse(stringBuilder.ToString(), out result4))
                tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidValueFormat);
              durationParseStates = CalendarCommon.DurationParseStates.M1;
              break;
            }
            tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidCharacter);
            return TimeSpan.Zero;
          case CalendarCommon.DurationParseStates.M1:
            if ((Internal.ContentLineParser.Tokens.Digit & Internal.ContentLineParser.Dictionary[(int) ch]) != ~(Internal.ContentLineParser.Tokens.CTL | Internal.ContentLineParser.Tokens.Alpha | Internal.ContentLineParser.Tokens.Digit | Internal.ContentLineParser.Tokens.SafeChar | Internal.ContentLineParser.Tokens.QSafeChar | Internal.ContentLineParser.Tokens.ValueChar | Internal.ContentLineParser.Tokens.WSP | Internal.ContentLineParser.Tokens.NonASCII))
            {
              stringBuilder = new StringBuilder();
              stringBuilder.Append(ch);
              durationParseStates = CalendarCommon.DurationParseStates.M2;
              break;
            }
            tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidCharacter);
            return TimeSpan.Zero;
          case CalendarCommon.DurationParseStates.M2:
            while ((Internal.ContentLineParser.Tokens.Digit & Internal.ContentLineParser.Dictionary[(int) ch]) != ~(Internal.ContentLineParser.Tokens.CTL | Internal.ContentLineParser.Tokens.Alpha | Internal.ContentLineParser.Tokens.Digit | Internal.ContentLineParser.Tokens.SafeChar | Internal.ContentLineParser.Tokens.QSafeChar | Internal.ContentLineParser.Tokens.ValueChar | Internal.ContentLineParser.Tokens.WSP | Internal.ContentLineParser.Tokens.NonASCII))
            {
              stringBuilder.Append(ch);
              if (++index == length)
              {
                tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.ExpectedS);
                return TimeSpan.Zero;
              }
              ch = s[index];
              if ((int) ch >= Internal.ContentLineParser.Dictionary.Length)
              {
                tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidCharacter);
                return TimeSpan.Zero;
              }
            }
            if ((int) ch == 83)
            {
              if (!int.TryParse(stringBuilder.ToString(), out result5))
                tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidValueFormat);
              durationParseStates = CalendarCommon.DurationParseStates.End;
              break;
            }
            tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidCharacter);
            return TimeSpan.Zero;
          default:
            tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidValueFormat);
            return TimeSpan.Zero;
        }
      }
      if (index != length)
      {
        tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.DurationDataNotEndedProperly);
        return TimeSpan.Zero;
      }
      if (result2 != 0)
        result1 += result2 * 7;
      return CalendarCommon.CreateTimeSpan(tracker, result1, result3, result4, result5, negative);
    }

    public static TimeSpan ParseUtcOffset(string s, Internal.ComplianceTracker tracker)
    {
      int result1 = 0;
      int result2 = 0;
      int result3 = 0;
      int length = s.Length;
      switch (length)
      {
        case 5:
        case 7:
          bool negative;
          switch (s[0])
          {
            case '+':
              negative = false;
              break;
            case '-':
              negative = true;
              break;
            default:
              tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.ExpectedPlusMinus);
              return TimeSpan.Zero;
          }
          for (int index = 1; index < length; ++index)
          {
            char ch = s[index];
            if ((int) ch >= Internal.ContentLineParser.Dictionary.Length)
            {
              tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidCharacter);
              return TimeSpan.Zero;
            }
            if ((Internal.ContentLineParser.Tokens.Digit & Internal.ContentLineParser.Dictionary[(int) ch]) == ~(Internal.ContentLineParser.Tokens.CTL | Internal.ContentLineParser.Tokens.Alpha | Internal.ContentLineParser.Tokens.Digit | Internal.ContentLineParser.Tokens.SafeChar | Internal.ContentLineParser.Tokens.QSafeChar | Internal.ContentLineParser.Tokens.ValueChar | Internal.ContentLineParser.Tokens.WSP | Internal.ContentLineParser.Tokens.NonASCII))
            {
              tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidToken);
              return TimeSpan.Zero;
            }
          }
          if (!int.TryParse(s.Substring(1, 2), out result1) || result1 < 0 || result1 > 23)
            tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidValueFormat);
          if (!int.TryParse(s.Substring(3, 2), out result2) || result2 < 0 || result2 > 59)
            tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidValueFormat);
          if (length == 7 && (!int.TryParse(s.Substring(5, 2), out result3) || result3 < 0 || result3 > 59))
            tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidValueFormat);
          if (result1 == 0 && result2 == 0 && (result3 == 0 && negative))
            tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidValueFormat);
          return CalendarCommon.CreateTimeSpan(tracker, 0, result1, result2, result3, negative);
        default:
          tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidUtcOffsetLength);
          return TimeSpan.Zero;
      }
    }

    public static string FormatDate(DateTime s)
    {
      return s.ToString("yyyyMMdd", (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static string FormatDateTime(DateTime s)
    {
      return s.ToString(s.Kind == DateTimeKind.Utc ? "yyyyMMdd\\THHmmss\\Z" : "yyyyMMdd\\THHmmss", (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public static string FormatDuration(TimeSpan ts)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (ts.Ticks < 0L)
      {
        stringBuilder.Append('-');
        ts = ts == TimeSpan.MinValue ? TimeSpan.MaxValue : ts.Negate();
      }
      stringBuilder.Append('P');
      if (ts.Days >= 7 && ts.Hours == 0 && (ts.Minutes == 0 && ts.Seconds == 0) && ts.Days % 7 == 0)
      {
        int num = ts.Days / 7;
        stringBuilder.Append(num.ToString());
        stringBuilder.Append("W");
      }
      else
      {
        if (ts.Days > 0)
        {
          stringBuilder.Append(ts.Days.ToString());
          stringBuilder.Append('D');
        }
        if (ts.Hours != 0 || ts.Minutes != 0 || ts.Seconds != 0)
          stringBuilder.Append('T');
        if (ts.Hours != 0)
        {
          stringBuilder.Append(ts.Hours.ToString());
          stringBuilder.Append('H');
          if (ts.Minutes != 0 || ts.Seconds != 0)
          {
            stringBuilder.Append(ts.Minutes.ToString());
            stringBuilder.Append('M');
            if (ts.Seconds != 0)
            {
              stringBuilder.Append(ts.Seconds.ToString());
              stringBuilder.Append('S');
            }
          }
        }
        else if (ts.Minutes != 0)
        {
          stringBuilder.Append(ts.Minutes.ToString());
          stringBuilder.Append('M');
          if (ts.Seconds != 0)
          {
            stringBuilder.Append(ts.Seconds.ToString());
            stringBuilder.Append('S');
          }
        }
        else if (ts.Seconds != 0)
        {
          stringBuilder.Append(ts.Seconds.ToString());
          stringBuilder.Append('S');
        }
      }
      return stringBuilder.ToString();
    }

    public static string FormatUtcOffset(TimeSpan ts)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (ts.Hours == 0 && ts.Minutes == 0 && ts.Seconds == 0)
        return "+0000";
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
      if (ts.Minutes < 10)
        stringBuilder.Append('0');
      stringBuilder.Append(ts.Minutes.ToString());
      if (ts.Seconds != 0)
      {
        if (ts.Seconds < 10)
          stringBuilder.Append('0');
        stringBuilder.Append(ts.Seconds.ToString());
      }
      return stringBuilder.ToString();
    }

    private static TimeSpan CreateTimeSpan(Internal.ComplianceTracker tracker, int days, int hours, int minutes, int seconds, bool negative)
    {
      TimeSpan timeSpan;
      try
      {
        timeSpan = new TimeSpan(days, hours, minutes, seconds);
      }
      catch (ArgumentOutOfRangeException ex)
      {
        tracker.SetComplianceStatus(Internal.ComplianceStatus.InvalidValueFormat, CtsResources.CalendarStrings.InvalidTimeFormat);
        return TimeSpan.Zero;
      }
      if (negative)
        timeSpan = timeSpan.Negate();
      return timeSpan;
    }

    internal enum DurationParseStates
    {
      Start,
      Sign,
      S1,
      S2,
      D1,
      T1,
      T2,
      H1,
      H2,
      M1,
      M2,
      End,
    }

    private class ComponentIdComparer : IEqualityComparer<ComponentId>
    {
      public bool Equals(ComponentId x, ComponentId y)
      {
        return x == y;
      }

      public int GetHashCode(ComponentId obj)
      {
        return (int) obj;
      }
    }

    private class PropertyIdComparer : IEqualityComparer<PropertyId>
    {
      public bool Equals(PropertyId x, PropertyId y)
      {
        return x == y;
      }

      public int GetHashCode(PropertyId obj)
      {
        return (int) obj;
      }
    }

    private class ParameterIdComparer : IEqualityComparer<ParameterId>
    {
      public bool Equals(ParameterId x, ParameterId y)
      {
        return x == y;
      }

      public int GetHashCode(ParameterId obj)
      {
        return (int) obj;
      }
    }
  }
}
