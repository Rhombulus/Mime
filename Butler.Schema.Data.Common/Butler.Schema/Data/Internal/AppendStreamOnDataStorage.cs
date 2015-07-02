// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Internal.AppendStreamOnDataStorage
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.Internal
{
  internal class AppendStreamOnDataStorage : StreamOnDataStorage
  {
    private ReadableWritableDataStorage storage;
    private long position;

    public override DataStorage Storage
    {
      get
      {
        return (DataStorage) this.storage;
      }
    }

    public override long Start
    {
      get
      {
        return 0L;
      }
    }

    public override long End
    {
      get
      {
        return this.position;
      }
    }

    public ReadableWritableDataStorage ReadableWritableStorage
    {
      get
      {
        return this.storage;
      }
    }

    public override bool CanRead
    {
      get
      {
        return false;
      }
    }

    public override bool CanWrite
    {
      get
      {
        return this.storage != null;
      }
    }

    public override bool CanSeek
    {
      get
      {
        return false;
      }
    }

    public override long Length
    {
      get
      {
        throw new NotSupportedException();
      }
    }

    public override long Position
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
      }
    }

    public AppendStreamOnDataStorage(ReadableWritableDataStorage storage)
    {
      storage.AddRef();
      this.storage = storage;
      this.position = storage.Length;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (this.storage == null)
        throw new ObjectDisposedException("AppendStreamOnDataStorage");
      if (buffer == null)
        throw new ArgumentNullException(nameof(buffer));
      if (offset > buffer.Length || offset < 0)
        throw new ArgumentOutOfRangeException(nameof(offset), CtsResources.SharedStrings.OffsetOutOfRange);
      if (count > buffer.Length || count < 0)
        throw new ArgumentOutOfRangeException(nameof(count), CtsResources.SharedStrings.CountOutOfRange);
      if (count + offset > buffer.Length)
        throw new ArgumentOutOfRangeException(nameof(count), CtsResources.SharedStrings.CountTooLarge);
      this.storage.Write(this.position, buffer, offset, count);
      this.position += (long) count;
    }

    public override void SetLength(long value)
    {
      throw new NotSupportedException();
    }

    public override void Flush()
    {
      if (this.storage == null)
        throw new ObjectDisposedException("AppendStreamOnDataStorage");
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotSupportedException();
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
