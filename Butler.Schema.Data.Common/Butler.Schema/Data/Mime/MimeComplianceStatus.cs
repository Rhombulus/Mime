// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.MimeComplianceStatus
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  [Flags]
  public enum MimeComplianceStatus
  {
    Compliant = 0,
    MissingBoundary = 1,
    InvalidBoundary = 2,
    InvalidWrapping = 4,
    BareLinefeedInBody = 8,
    InvalidHeader = 16,
    MissingBodySeparator = 32,
    MissingBoundaryParameter = 64,
    InvalidTransferEncoding = 128,
    InvalidExternalBody = 256,
    BareLinefeedInHeader = 512,
    UnexpectedBinaryContent = 1024,
  }
}
