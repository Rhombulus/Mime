// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.vCard.ContactReader
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.ContentTypes.vCard
{
  public class ContactReader : IDisposable
  {
    private Internal.ContentLineReader reader;
    private bool isClosed;

    public ContactComplianceMode ComplianceMode
    {
      get
      {
        this.CheckDisposed("ComplianceMode::get");
        return (ContactComplianceMode) this.reader.ComplianceTracker.Mode;
      }
    }

    public ContactComplianceStatus ComplianceStatus
    {
      get
      {
        this.CheckDisposed("ComplianceStatus::get");
        return (ContactComplianceStatus) this.reader.ComplianceTracker.Status;
      }
    }

    public ContactPropertyReader PropertyReader
    {
      get
      {
        this.CheckDisposed("PropertyReader::get");
        this.reader.AssertValidState(~Internal.ContentLineNodeType.DocumentEnd);
        return new ContactPropertyReader(this.reader);
      }
    }

    public ContactReader(Stream stream)
      : this(stream, Encoding.UTF8, ContactComplianceMode.Strict)
    {
    }

    public ContactReader(Stream stream, Encoding encoding, ContactComplianceMode contactComplianceMode)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (!stream.CanRead)
        throw new ArgumentException(CtsResources.CalendarStrings.StreamMustAllowRead, "stream");
      if (encoding == null)
        throw new ArgumentNullException("encoding");
      Internal.ComplianceMode complianceMode = (Internal.ComplianceMode) contactComplianceMode;
      this.reader = new Internal.ContentLineReader(stream, encoding, new Internal.ComplianceTracker(Internal.FormatType.VCard, complianceMode), (Internal.ValueTypeContainer) new ContactValueTypeContainer());
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing && !this.isClosed && this.reader != null)
      {
        this.reader.Dispose();
        this.reader = (Internal.ContentLineReader) null;
      }
      this.isClosed = true;
    }

    protected void CheckDisposed(string methodName)
    {
      if (this.isClosed)
        throw new ObjectDisposedException("ContactReader", methodName);
    }

    public virtual void Close()
    {
      this.Dispose();
    }

    public bool ReadNext()
    {
      this.CheckDisposed("ReadNext");
      if (!this.reader.ReadNextComponent())
        return false;
      if (string.Compare(this.reader.ComponentName, "VCARD", StringComparison.OrdinalIgnoreCase) != 0)
        this.reader.ComplianceTracker.SetComplianceStatus(Internal.ComplianceStatus.ComponentNameMismatch, CtsResources.CalendarStrings.ComponentNameMismatch);
      if (this.reader.Depth > 1)
        this.reader.ComplianceTracker.SetComplianceStatus(Internal.ComplianceStatus.NotAllComponentsClosed, CtsResources.CalendarStrings.NotAllComponentsClosed);
      return true;
    }

    public void ResetComplianceStatus()
    {
      this.CheckDisposed("ResetComplianceStatus");
      this.reader.ComplianceTracker.Reset();
    }
  }
}
