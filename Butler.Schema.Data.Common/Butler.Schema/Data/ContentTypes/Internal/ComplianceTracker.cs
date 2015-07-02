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

      public ComplianceStatus Status { get; private set; }

      public ComplianceMode Mode { get; }

      public FormatType Format { get; }

      public ComplianceTracker(FormatType format, ComplianceMode complianceMode)
    {
      this.Format = format;
      this.Mode = complianceMode;
    }

    public void SetComplianceStatus(ComplianceStatus status, string message)
    {
      this.Status |= status;
      if (ComplianceMode.Strict != this.Mode)
        return;
      if (this.Format == FormatType.Calendar)
        throw new iCalendar.InvalidCalendarDataException(message);
      if (FormatType.VCard == this.Format)
        throw new vCard.InvalidContactDataException(message);
    }

    public void Reset()
    {
      this.Status = ComplianceStatus.Compliant;
    }
  }
}
