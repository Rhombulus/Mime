// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.iCalendar.ParameterId
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.ContentTypes.iCalendar
{
  [Flags]
  public enum ParameterId
  {
    Unknown = 1,
    AlternateRepresentation = 2,
    CommonName = 4,
    CalendarUserType = 8,
    Delegator = 16,
    Delegatee = 32,
    Directory = 64,
    Encoding = 128,
    FormatType = 256,
    FreeBusyType = 512,
    Language = 1024,
    Membership = 2048,
    ParticipationStatus = 4096,
    RecurrenceRange = 8192,
    TriggerRelationship = 16384,
    RelationshipType = 32768,
    ParticipationRole = 65536,
    RsvpExpectation = 131072,
    SentBy = 262144,
    TimeZoneId = 524288,
    ValueType = 1048576,
  }
}
