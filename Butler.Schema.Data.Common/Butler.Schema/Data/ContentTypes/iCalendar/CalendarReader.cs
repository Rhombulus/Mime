// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.iCalendar.CalendarReader
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.ContentTypes.iCalendar
{
  public class CalendarReader : IDisposable
  {
    private Internal.ContentLineReader reader;
    private bool isClosed;

    public ComponentId ComponentId
    {
      get
      {
        this.CheckDisposed("ComponentId::get");
        return CalendarCommon.GetComponentEnum(this.ComponentName);
      }
    }

    public string ComponentName
    {
      get
      {
        this.CheckDisposed("ComponentName::get");
        this.reader.AssertValidState(~Internal.ContentLineNodeType.DocumentEnd);
        return this.reader.ComponentName;
      }
    }

    public CalendarComplianceMode ComplianceMode
    {
      get
      {
        this.CheckDisposed("ComplianceMode::get");
        return (CalendarComplianceMode) this.reader.ComplianceTracker.Mode;
      }
    }

    public CalendarComplianceStatus ComplianceStatus
    {
      get
      {
        this.CheckDisposed("ComplianceStatus::get");
        return (CalendarComplianceStatus) this.reader.ComplianceTracker.Status;
      }
    }

    public CalendarPropertyReader PropertyReader
    {
      get
      {
        this.CheckDisposed("PropertyReader::get");
        this.reader.AssertValidState(~Internal.ContentLineNodeType.DocumentEnd);
        return new CalendarPropertyReader(this.reader);
      }
    }

    public int Depth
    {
      get
      {
        this.CheckDisposed("Depth::get");
        return this.reader.Depth;
      }
    }

    public CalendarReader(Stream stream)
      : this(stream, "utf-8", CalendarComplianceMode.Strict)
    {
    }

    public CalendarReader(Stream stream, string encodingName, CalendarComplianceMode calendarComplianceMode)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");
      if (!stream.CanRead)
        throw new ArgumentException(CtsResources.CalendarStrings.StreamMustAllowRead, "stream");
      if (encodingName == null)
        throw new ArgumentNullException("encodingName");
      Internal.ComplianceMode complianceMode = (Internal.ComplianceMode) calendarComplianceMode;
      this.reader = new Internal.ContentLineReader(stream, Globalization.Charset.GetEncoding(encodingName), new Internal.ComplianceTracker(Internal.FormatType.Calendar, complianceMode), (Internal.ValueTypeContainer) new CalendarValueTypeContainer());
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
        throw new ObjectDisposedException("CalendarReader", methodName);
    }

    public virtual void Close()
    {
      this.Dispose();
    }

    public bool ReadNextComponent()
    {
      this.CheckDisposed("ReadNextComponent");
      return this.reader.ReadNextComponent();
    }

    public bool ReadFirstChildComponent()
    {
      this.CheckDisposed("ReadFirstChildComponent");
      this.reader.AssertValidState(~Internal.ContentLineNodeType.DocumentStart);
      return this.reader.ReadFirstChildComponent();
    }

    public bool ReadNextSiblingComponent()
    {
      this.CheckDisposed("ReadNextSiblingComponent");
      this.reader.AssertValidState(~Internal.ContentLineNodeType.DocumentStart);
      return this.reader.ReadNextSiblingComponent();
    }

    public void ResetComplianceStatus()
    {
      this.CheckDisposed("ResetComplianceStatus");
      this.reader.ComplianceTracker.Reset();
    }
  }
}
