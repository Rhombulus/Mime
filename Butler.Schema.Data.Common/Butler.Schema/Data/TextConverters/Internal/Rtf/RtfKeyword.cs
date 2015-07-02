// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Rtf.RtfKeyword
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Diagnostics;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Rtf
{
  internal struct RtfKeyword
  {
    private RtfToken token;

    public byte[] Buffer => this.token.Buffer;

      public int Offset => this.token.CurrentRunOffset;

      public int Length => (int) this.token.RunQueue[this.token.CurrentRun].Length;

      public short Id => this.token.RunQueue[this.token.CurrentRun].KeywordId;

      public int Value => this.token.RunQueue[this.token.CurrentRun].Value;

      public bool Skip => this.token.RunQueue[this.token.CurrentRun].Skip;

      public bool First => this.token.RunQueue[this.token.CurrentRun].Lead;

      internal RtfKeyword(RtfToken token)
    {
      this.token = token;
    }

    [Conditional("DEBUG")]
    private void AssertCurrent()
    {
    }
  }
}
