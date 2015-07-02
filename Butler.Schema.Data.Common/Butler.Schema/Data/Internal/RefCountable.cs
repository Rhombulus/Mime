// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Internal.RefCountable
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Threading;

namespace Butler.Schema.Data.Internal
{
  internal abstract class RefCountable : IDisposable
  {
    private int refCount;
    private bool isDisposed;

    protected bool IsDisposed => this.isDisposed;

      public int RefCount => this.refCount;

      protected RefCountable()
    {
      this.refCount = 1;
    }

    protected void ThrowIfDisposed()
    {
      if (this.isDisposed)
        throw new ObjectDisposedException("RefCountable");
    }

    public void AddRef()
    {
      this.ThrowIfDisposed();
      Interlocked.Increment(ref this.refCount);
    }

    public void Release()
    {
      this.ThrowIfDisposed();
      if (Interlocked.Decrement(ref this.refCount) != 0)
        return;
      this.Dispose();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      this.isDisposed = true;
    }
  }
}
