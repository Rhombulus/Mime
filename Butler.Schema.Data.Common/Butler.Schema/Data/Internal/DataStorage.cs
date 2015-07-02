// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Internal.DataStorage
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Butler.Schema.Data.Internal
{
  internal abstract class DataStorage : RefCountable
  {
    protected bool isReadOnly;
    protected SemaphoreSlim readOnlySemaphore;

    internal bool IsReadOnly => this.isReadOnly;

      public static long CopyStreamToStream(Stream srcStream, Stream destStream, long lengthToCopy, ref byte[] scratchBuffer)
    {
      if (scratchBuffer == null || scratchBuffer.Length < 16384)
        scratchBuffer = new byte[16384];
      long num = 0L;
      while (lengthToCopy != 0L)
      {
        int count1 = (int) Math.Min(lengthToCopy, (long) scratchBuffer.Length);
        int count2 = srcStream.Read(scratchBuffer, 0, count1);
        if (count2 != 0)
        {
          if (destStream != null)
            destStream.Write(scratchBuffer, 0, count2);
          num += (long) count2;
          if (lengthToCopy != long.MaxValue)
            lengthToCopy -= (long) count2;
        }
        else
          break;
      }
      return num;
    }

    public static Stream NewEmptyReadStream()
    {
      return (Stream) new StreamOnReadableDataStorage((ReadableDataStorage) null, 0L, 0L);
    }

    public abstract Stream OpenReadStream(long start, long end);

    public virtual long CopyContentToStream(long start, long end, Stream destStream, ref byte[] scratchBuffer)
    {
      this.ThrowIfDisposed();
      if (destStream == null && end != long.MaxValue)
        return end - start;
      using (Stream srcStream = this.OpenReadStream(start, end))
        return DataStorage.CopyStreamToStream(srcStream, destStream, long.MaxValue, ref scratchBuffer);
    }

    internal virtual void SetReadOnly(bool makeReadOnly)
    {
      this.ThrowIfDisposed();
      if (makeReadOnly == this.isReadOnly)
        return;
      this.readOnlySemaphore = !makeReadOnly ? (SemaphoreSlim) null : new SemaphoreSlim(1);
      this.isReadOnly = makeReadOnly;
    }
  }
}
