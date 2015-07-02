// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.Internal.ComplianceTracker
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.ContentTypes.Internal
{
  internal class ComplianceTracker
  {
    private FormatType format;
    private ComplianceMode complianceMode;
    private ComplianceStatus complianceStatus;

    public ComplianceStatus Status
    {
      get
      {
        return this.complianceStatus;
      }
    }

    public ComplianceMode Mode
    {
      get
      {
        return this.complianceMode;
      }
    }

    public FormatType Format
    {
      get
      {
        return this.format;
      }
    }

    public ComplianceTracker(FormatType format, ComplianceMode complianceMode)
    {
      this.format = format;
      this.complianceMode = complianceMode;
    }

    public void SetComplianceStatus(ComplianceStatus status, string message)
    {
      this.complianceStatus |= status;
      if (ComplianceMode.Strict != this.complianceMode)
        return;
      if (this.format == FormatType.Calendar)
        throw new iCalendar.InvalidCalendarDataException(message);
      if (FormatType.VCard == this.format)
        throw new vCard.InvalidContactDataException(message);
    }

    public void Reset()
    {
      this.complianceStatus = ComplianceStatus.Compliant;
    }
  }
}
