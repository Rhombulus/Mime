// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.MimeStringLength
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  internal struct MimeStringLength
  {

      public int InChars { get; private set; }

      public int InBytes { get; private set; }

      public MimeStringLength(int value)
    {
      this.InChars = value;
      this.InBytes = value;
    }

    public MimeStringLength(int valueInChars, int valueInBytes)
    {
      this.InChars = valueInChars;
      this.InBytes = valueInBytes;
    }

    public void IncrementBy(int count)
    {
      this.InChars += count;
      this.InBytes += count;
    }

    public void IncrementBy(int countInChars, int countInBytes)
    {
      this.InChars += countInChars;
      this.InBytes += countInBytes;
    }

    public void IncrementBy(MimeStringLength count)
    {
      this.InChars += count.InChars;
      this.InBytes += count.InBytes;
    }

    public void DecrementBy(int count)
    {
      this.InChars -= count;
      this.InBytes -= count;
    }

    public void DecrementBy(int countInChars, int countInBytes)
    {
      this.InChars -= countInChars;
      this.InBytes -= countInBytes;
    }

    public void DecrementBy(MimeStringLength count)
    {
      this.InChars -= count.InChars;
      this.InBytes -= count.InBytes;
    }

    public void SetAs(int value)
    {
      this.InChars = value;
      this.InBytes = value;
    }

    public void SetAs(int valueInChars, int valueInBytes)
    {
      this.InChars = valueInChars;
      this.InBytes = valueInBytes;
    }

    public void SetAs(MimeStringLength value)
    {
      this.InChars = value.InChars;
      this.InBytes = value.InBytes;
    }

    public override string ToString()
    {
      return string.Format("InChars={0}, InBytes={1}", (object) this.InChars, (object) this.InBytes);
    }
  }
}
