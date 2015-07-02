// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Internal.StreamOnReadableDataStorage
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.Internal
{
  internal class StreamOnReadableDataStorage : StreamOnDataStorage, ICloneableStream
  {
    private ReadableDataStorage baseStorage;
    private long start;
    private long end;
    private long position;
    private bool disposed;

    public override DataStorage Storage
    {
      get
      {
        this.ThrowIfDisposed();
        return (DataStorage) this.baseStorage;
      }
    }

    public override long Start
    {
      get
      {
        this.ThrowIfDisposed();
        return this.start;
      }
    }

    public override long End
    {
      get
      {
        this.ThrowIfDisposed();
        return this.end;
      }
    }

    public override bool CanRead => !this.disposed;

      public override bool CanWrite => false;

      public override bool CanSeek => !this.disposed;

      public override long Length
    {
      get
      {
        this.ThrowIfDisposed();
        if (this.end != long.MaxValue)
          return this.end - this.start;
        return this.baseStorage.Length - this.start;
      }
    }

    public override long Position
    {
      get
      {
        this.ThrowIfDisposed();
        return this.position;
      }
      set
      {
        this.ThrowIfDisposed();
        if (value < 0L)
          throw new ArgumentOutOfRangeException(nameof(value), Resources.SharedStrings.CannotSeekBeforeBeginning);
        this.position = value;
      }
    }

    public StreamOnReadableDataStorage(ReadableDataStorage baseStorage, long start, long end)
    {
      if (baseStorage != null)
      {
        baseStorage.AddRef();
        this.baseStorage = baseStorage;
      }
      this.start = start;
      this.end = end;
    }

    private StreamOnReadableDataStorage(ReadableDataStorage baseStorage, long start, long end, long position)
    {
      if (baseStorage != null)
      {
        baseStorage.AddRef();
        this.baseStorage = baseStorage;
      }
      this.start = start;
      this.end = end;
      this.position = position;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      this.ThrowIfDisposed();
      if (buffer == null)
        throw new ArgumentNullException(nameof(buffer));
      if (offset > buffer.Length || offset < 0)
        throw new ArgumentOutOfRangeException(nameof(offset), Resources.SharedStrings.OffsetOutOfRange);
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof(count), Resources.SharedStrings.CountOutOfRange);
      if (count + offset > buffer.Length)
        throw new ArgumentOutOfRangeException(nameof(count), Resources.SharedStrings.CountTooLarge);
      int num1 = 0;
      if ((this.end == long.MaxValue || this.position < this.end - this.start) && count != 0)
      {
        if (this.end != long.MaxValue && (long) count > this.end - this.start - this.position)
          count = (int) (this.end - this.start - this.position);
        int num2;
        do
        {
          num2 = this.baseStorage.Read(this.start + this.position, buffer, offset, count);
          count -= num2;
          offset += num2;
          this.position += (long) num2;
          num1 += num2;
        }
        while (count != 0 && num2 != 0);
      }
      return num1;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException();
    }

    public override void Flush()
    {
      throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
      throw new NotSupportedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      this.ThrowIfDisposed();
      switch (origin)
      {
        case SeekOrigin.Begin:
          if (offset < 0L)
            throw new ArgumentOutOfRangeException(nameof(offset), Resources.SharedStrings.CannotSeekBeforeBeginning);
          this.position = offset;
          return this.position;
        case SeekOrigin.Current:
          offset += this.position;
          goto case 0;
        case SeekOrigin.End:
          offset += this.Length;
          goto case 0;
        default:
          throw new ArgumentException("Invalid Origin enumeration value", nameof(origin));
      }
    }

    public Stream Clone()
    {
      this.ThrowIfDisposed();
      return (Stream) new StreamOnReadableDataStorage(this.baseStorage, this.start, this.end, this.position);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.baseStorage != null)
      {
        this.baseStorage.Release();
        this.baseStorage = (ReadableDataStorage) null;
      }
      this.disposed = true;
      base.Dispose(disposing);
    }

    private void ThrowIfDisposed()
    {
      if (this.disposed)
        throw new ObjectDisposedException("StreamOnReadableDataStorage");
    }
  }
}
