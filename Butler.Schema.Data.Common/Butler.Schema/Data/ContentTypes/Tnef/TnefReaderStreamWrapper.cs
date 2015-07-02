// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.Tnef.TnefReaderStreamWrapper
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.ContentTypes.Tnef
{
  internal class TnefReaderStreamWrapper : Stream
  {
    internal TnefReader Reader;

    public override bool CanRead => this.Reader != null;

      public override bool CanWrite => false;

      public override bool CanSeek => false;

      public override long Length
    {
      get
      {
        throw new NotSupportedException(CtsResources.TnefStrings.StreamDoesNotSupportSeek);
      }
    }

    public override long Position
    {
      get
      {
        throw new NotSupportedException(CtsResources.TnefStrings.StreamDoesNotSupportSeek);
      }
      set
      {
        throw new NotSupportedException(CtsResources.TnefStrings.StreamDoesNotSupportSeek);
      }
    }

    public TnefReaderStreamWrapper(TnefReader reader)
    {
      this.Reader = reader;
      this.Reader.Child = (object) this;
    }

    public override void Flush()
    {
      throw new NotSupportedException(CtsResources.TnefStrings.StreamDoesNotSupportWrite);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (this.Reader == null)
        throw new ObjectDisposedException("TnefReaderStreamWrapper");
      return this.Reader.ReadPropertyRawValue(buffer, offset, count, true);
    }

    public override void SetLength(long value)
    {
      throw new NotSupportedException(CtsResources.TnefStrings.StreamDoesNotSupportSeek);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException(CtsResources.TnefStrings.StreamDoesNotSupportWrite);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotSupportedException(CtsResources.TnefStrings.StreamDoesNotSupportSeek);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.Reader != null)
        this.Reader.Child = (object) null;
      this.Reader = (TnefReader) null;
      base.Dispose(disposing);
    }
  }
}
