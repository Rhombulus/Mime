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

      private long position;

    public override DataStorage Storage => (DataStorage) this.ReadableWritableStorage;

      public override long Start => 0L;

      public override long End => this.position;

      public ReadableWritableDataStorage ReadableWritableStorage { get; private set; }

      public override bool CanRead => false;

      public override bool CanWrite => this.ReadableWritableStorage != null;

      public override bool CanSeek => false;

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
      this.ReadableWritableStorage = storage;
      this.position = storage.Length;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (this.ReadableWritableStorage == null)
        throw new ObjectDisposedException("AppendStreamOnDataStorage");
      if (buffer == null)
        throw new ArgumentNullException(nameof(buffer));
      if (offset > buffer.Length || offset < 0)
        throw new ArgumentOutOfRangeException(nameof(offset), CtsResources.SharedStrings.OffsetOutOfRange);
      if (count > buffer.Length || count < 0)
        throw new ArgumentOutOfRangeException(nameof(count), CtsResources.SharedStrings.CountOutOfRange);
      if (count + offset > buffer.Length)
        throw new ArgumentOutOfRangeException(nameof(count), CtsResources.SharedStrings.CountTooLarge);
      this.ReadableWritableStorage.Write(this.position, buffer, offset, count);
      this.position += (long) count;
    }

    public override void SetLength(long value)
    {
      throw new NotSupportedException();
    }

    public override void Flush()
    {
      if (this.ReadableWritableStorage == null)
        throw new ObjectDisposedException("AppendStreamOnDataStorage");
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotSupportedException();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.ReadableWritableStorage != null)
      {
        this.ReadableWritableStorage.Release();
        this.ReadableWritableStorage = (ReadableWritableDataStorage) null;
      }
      base.Dispose(disposing);
    }
  }
}
