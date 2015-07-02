// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Injection
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  internal abstract class Injection : IDisposable
  {
    protected HeaderFooterFormat injectionFormat;
    protected string injectHead;
    protected string injectTail;
    protected bool headInjected;
    protected bool tailInjected;
    protected bool testBoundaryConditions;
    protected Stream traceStream;

    public HeaderFooterFormat HeaderFooterFormat => this.injectionFormat;

      public bool HaveHead => this.injectHead != null;

      public bool HaveTail => this.injectTail != null;

      public bool HeadDone => this.headInjected;

      public bool TailDone => this.tailInjected;

      public abstract void Inject(bool head, Internal.Text.TextOutput output);

    void IDisposable.Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    public virtual void Reset()
    {
      this.headInjected = false;
      this.tailInjected = false;
    }

    public abstract void CompileForRtf(Internal.Rtf.RtfOutput output);

    public abstract void InjectRtf(bool head, bool immediatelyAfterText);

    public abstract void InjectRtfFonts(int firstAvailableFontHandle);

    public abstract void InjectRtfColors(int nextColorIndex);
  }
}
