// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Internal.ReadableWritableDataStorage
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Internal
{
  internal abstract class ReadableWritableDataStorage : ReadableDataStorage
  {
    public abstract void Write(long position, byte[] buffer, int offset, int count);

    public abstract void SetLength(long length);

    public virtual StreamOnDataStorage OpenWriteStream(bool append)
    {
      this.ThrowIfDisposed();
      if (append)
        return (StreamOnDataStorage) new AppendStreamOnDataStorage(this);
      return (StreamOnDataStorage) new ReadWriteStreamOnDataStorage(this);
    }
  }
}
