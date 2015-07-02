// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Internal.ReadWriteStreamOnDataStorage
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.Internal
{
  internal class ReadWriteStreamOnDataStorage : StreamOnDataStorage, ICloneableStream
  {
    private ReadableWritableDataStorage storage;
    private long position;

    public override DataStorage Storage
    {
      get
      {
        if (this.storage == null)
          throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
        return (DataStorage) this.storage;
      }
    }

    public override long Start => 0L;

      public override long End => long.MaxValue;

      public override bool CanRead => this.storage != null;

      public override bool CanWrite => this.storage != null;

      public override bool CanSeek => this.storage != null;

      public override long Length
    {
      get
      {
        if (this.storage == null)
          throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
        return this.storage.Length;
      }
    }

    public override long Position
    {
      get
      {
        if (this.storage == null)
          throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
        return this.position;
      }
      set
      {
        if (this.storage == null)
          throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
        if (value < 0L)
          throw new ArgumentOutOfRangeException(nameof(value), Resources.SharedStrings.CannotSeekBeforeBeginning);
        this.position = value;
      }
    }

    internal ReadWriteStreamOnDataStorage(ReadableWritableDataStorage storage)
    {
      storage.AddRef();
      this.storage = storage;
    }

    private ReadWriteStreamOnDataStorage(ReadableWritableDataStorage storage, long position)
    {
      storage.AddRef();
      this.storage = storage;
      this.position = position;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (this.storage == null)
        throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
      if (buffer == null)
        throw new ArgumentNullException(nameof(buffer));
      if (offset > buffer.Length || offset < 0)
        throw new ArgumentOutOfRangeException(nameof(offset), Resources.SharedStrings.OffsetOutOfRange);
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof(count), Resources.SharedStrings.CountOutOfRange);
      if (count + offset > buffer.Length)
        throw new ArgumentOutOfRangeException(nameof(count), Resources.SharedStrings.CountTooLarge);
      int num = this.storage.Read(this.position, buffer, offset, count);
      this.position += (long) num;
      return num;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (this.storage == null)
        throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
      if (buffer == null)
        throw new ArgumentNullException(nameof(buffer));
      if (offset > buffer.Length || offset < 0)
        throw new ArgumentOutOfRangeException(nameof(offset), Resources.SharedStrings.OffsetOutOfRange);
      if (count > buffer.Length || count < 0)
        throw new ArgumentOutOfRangeException(nameof(count), Resources.SharedStrings.CountOutOfRange);
      if (count + offset > buffer.Length)
        throw new ArgumentOutOfRangeException(nameof(count), Resources.SharedStrings.CountTooLarge);
      this.storage.Write(this.position, buffer, offset, count);
      this.position += (long) count;
    }

    public override void SetLength(long value)
    {
      if (this.storage == null)
        throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
      if (value < 0L)
        throw new ArgumentOutOfRangeException(nameof(value), Resources.SharedStrings.CannotSetNegativelength);
      this.storage.SetLength(value);
    }

    public override void Flush()
    {
      if (this.storage == null)
        throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      if (this.storage == null)
        throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
      switch (origin)
      {
        case SeekOrigin.Begin:
          this.position = offset;
          break;
        case SeekOrigin.Current:
          offset = this.position + offset;
          break;
        case SeekOrigin.End:
          offset = this.storage.Length + offset;
          break;
        default:
          throw new ArgumentException("Invalid Origin enumeration value", nameof(origin));
      }
      if (offset < 0L)
        throw new ArgumentOutOfRangeException(nameof(offset), Resources.SharedStrings.CannotSeekBeforeBeginning);
      this.position = offset;
      return this.position;
    }

    Stream ICloneableStream.Clone()
    {
      if (this.storage == null)
        throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
      return (Stream) new ReadWriteStreamOnDataStorage(this.storage, this.position);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.storage != null)
      {
        this.storage.Release();
        this.storage = (ReadableWritableDataStorage) null;
      }
      base.Dispose(disposing);
    }
  }
}
