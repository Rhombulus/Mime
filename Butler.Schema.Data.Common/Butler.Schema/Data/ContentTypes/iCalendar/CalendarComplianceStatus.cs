// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.iCalendar.CalendarComplianceStatus
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.ContentTypes.iCalendar
{
  [Flags]
  public enum CalendarComplianceStatus
  {
    Compliant = 0,
    StreamTruncated = 1,
    PropertyTruncated = 2,
    InvalidCharacterInPropertyName = 4,
    InvalidCharacterInParameterName = 8,
    InvalidCharacterInParameterText = 16,
    InvalidCharacterInQuotedString = 32,
    InvalidCharacterInPropertyValue = 64,
    NotAllComponentsClosed = 128,
    ParametersOnComponentTag = 256,
    EndTagWithoutBegin = 512,
    ComponentNameMismatch = 1024,
    InvalidValueFormat = 2048,
    EmptyParameterName = 4096,
    EmptyPropertyName = 8192,
    EmptyComponentName = 16384,
    PropertyOutsideOfComponent = 32768,
    InvalidParameterValue = 65536,
    ParameterNameMissing = 131072,
  }
}
