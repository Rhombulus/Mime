// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.Tnef.TnefWriterStreamWrapper
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.ContentTypes.Tnef
{
  internal class TnefWriterStreamWrapper : Stream
  {
    internal TnefWriter Writer;

    public override bool CanRead => false;

      public override bool CanWrite => this.Writer != null;

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

    public TnefWriterStreamWrapper(TnefWriter writer)
    {
      this.Writer = writer;
      this.Writer.Child = (object) this;
    }

    public override void Flush()
    {
      if (this.Writer == null)
        throw new ObjectDisposedException("TnefWriterStreamWrapper");
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException(CtsResources.TnefStrings.StreamDoesNotSupportRead);
    }

    public override void SetLength(long value)
    {
      throw new NotSupportedException(CtsResources.TnefStrings.StreamDoesNotSupportSeek);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (this.Writer == null)
        throw new ObjectDisposedException("TnefWriterStreamWrapper");
      this.Writer.WritePropertyRawValueImpl(buffer, offset, count, true);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotSupportedException(CtsResources.TnefStrings.StreamDoesNotSupportSeek);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.Writer != null)
        this.Writer.Child = (object) null;
      this.Writer = (TnefWriter) null;
      base.Dispose(disposing);
    }
  }
}
