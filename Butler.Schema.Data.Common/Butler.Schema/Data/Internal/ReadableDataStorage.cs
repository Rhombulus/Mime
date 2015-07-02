// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Internal.ReadableDataStorage
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.Internal
{
  internal abstract class ReadableDataStorage : DataStorage
  {
    public abstract long Length { get; }

    public abstract int Read(long position, byte[] buffer, int offset, int count);

    public override Stream OpenReadStream(long start, long end)
    {
      this.ThrowIfDisposed();
      return (Stream) new StreamOnReadableDataStorage(this, start, end);
    }

    public override long CopyContentToStream(long start, long end, Stream destStream, ref byte[] scratchBuffer)
    {
      this.ThrowIfDisposed();
      if (scratchBuffer == null || scratchBuffer.Length < 16384)
        scratchBuffer = new byte[16384];
      long num = 0L;
      long val1 = end == long.MaxValue ? long.MaxValue : end - start;
      while (val1 != 0L)
      {
        int count1 = (int) Math.Min(val1, (long) scratchBuffer.Length);
        int count2 = this.Read(start, scratchBuffer, 0, count1);
        if (count2 != 0)
        {
          start += (long) count2;
          destStream.Write(scratchBuffer, 0, count2);
          num += (long) count2;
          if (val1 != long.MaxValue)
            val1 -= (long) count2;
        }
        else
          break;
      }
      return num;
    }
  }
}
