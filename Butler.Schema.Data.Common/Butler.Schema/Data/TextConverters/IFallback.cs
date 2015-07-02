// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.IFallback
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  internal interface IFallback
  {
    byte[] GetUnsafeAsciiMap(out byte unsafeAsciiMask);

    bool HasUnsafeUnicode();

    bool TreatNonAsciiAsUnsafe(string charset);

    bool IsUnsafeUnicode(char ch, bool isFirstChar);

    bool FallBackChar(char ch, char[] outputBuffer, ref int outputBufferCount, int lineBufferEnd);
  }
}
