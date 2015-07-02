// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.Tnef.TnefComplianceStatus
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.ContentTypes.Tnef
{
  [Flags]
  public enum TnefComplianceStatus
  {
    Compliant = 0,
    InvalidAttribute = 1,
    InvalidAttributeLevel = 2,
    InvalidAttributeLength = 16,
    StreamTruncated = 32,
    InvalidTnefSignature = 64,
    InvalidTnefVersion = 256,
    InvalidMessageClass = 512,
    InvalidRowCount = 1024,
    InvalidAttributeValue = 2048,
    AttributeOverflow = 4096,
    InvalidAttributeChecksum = 8192,
    InvalidMessageCodepage = 16384,
    UnsupportedPropertyType = 32768,
    InvalidPropertyLength = 65536,
    InvalidDate = 131072,
    NestingTooDeep = 262144,
  }
}
