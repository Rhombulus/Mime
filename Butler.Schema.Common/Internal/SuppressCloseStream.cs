// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Internal.SuppressCloseStream
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.Internal
{
  internal sealed class SuppressCloseStream : Stream, ICloneableStream
  {
    private Stream sourceStream;

    public override bool CanRead
    {
      get
      {
        if (this.sourceStream != null)
          return this.sourceStream.CanRead;
        return false;
      }
    }

    public override bool CanWrite
    {
      get
      {
        if (this.sourceStream != null)
          return this.sourceStream.CanWrite;
        return false;
      }
    }

    public override bool CanSeek
    {
      get
      {
        if (this.sourceStream != null)
          return this.sourceStream.CanSeek;
        return false;
      }
    }

    public override long Length
    {
      get
      {
        this.AssertOpen();
        return this.sourceStream.Length;
      }
    }

    public override long Position
    {
      get
      {
        this.AssertOpen();
        return this.sourceStream.Position;
      }
      set
      {
        this.AssertOpen();
        this.sourceStream.Position = value;
      }
    }

    public SuppressCloseStream(Stream sourceStream)
    {
      if (sourceStream == null)
        throw new ArgumentNullException(nameof(sourceStream));
      this.sourceStream = sourceStream;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      this.AssertOpen();
      return this.sourceStream.Read(buffer, offset, count);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      this.AssertOpen();
      this.sourceStream.Write(buffer, offset, count);
    }

    public override void Flush()
    {
      this.AssertOpen();
      this.sourceStream.Flush();
    }

    public override void SetLength(long value)
    {
      this.AssertOpen();
      this.sourceStream.SetLength(value);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      this.AssertOpen();
      return this.sourceStream.Seek(offset, origin);
    }

    public Stream Clone()
    {
      this.AssertOpen();
      if (this.CanWrite)
        throw new NotSupportedException();
      ICloneableStream cloneableStream = this.sourceStream as ICloneableStream;
      if (cloneableStream == null)
      {
        if (!this.sourceStream.CanSeek)
          throw new NotSupportedException();
        this.sourceStream = (Stream) new AutoPositionReadOnlyStream(this.sourceStream, false);
        cloneableStream = this.sourceStream as ICloneableStream;
      }
      return (Stream) new SuppressCloseStream(cloneableStream.Clone());
    }

    private void AssertOpen()
    {
      if (this.sourceStream == null)
        throw new ObjectDisposedException("SuppressCloseStream");
    }

    protected override void Dispose(bool disposing)
    {
      this.sourceStream = (Stream) null;
      base.Dispose(disposing);
    }
  }
}
