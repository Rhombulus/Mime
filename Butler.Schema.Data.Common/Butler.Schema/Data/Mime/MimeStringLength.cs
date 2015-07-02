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
    private int inChars;
    private int inBytes;

    public int InChars
    {
      get
      {
        return this.inChars;
      }
    }

    public int InBytes
    {
      get
      {
        return this.inBytes;
      }
    }

    public MimeStringLength(int value)
    {
      this.inChars = value;
      this.inBytes = value;
    }

    public MimeStringLength(int valueInChars, int valueInBytes)
    {
      this.inChars = valueInChars;
      this.inBytes = valueInBytes;
    }

    public void IncrementBy(int count)
    {
      this.inChars += count;
      this.inBytes += count;
    }

    public void IncrementBy(int countInChars, int countInBytes)
    {
      this.inChars += countInChars;
      this.inBytes += countInBytes;
    }

    public void IncrementBy(MimeStringLength count)
    {
      this.inChars += count.InChars;
      this.inBytes += count.InBytes;
    }

    public void DecrementBy(int count)
    {
      this.inChars -= count;
      this.inBytes -= count;
    }

    public void DecrementBy(int countInChars, int countInBytes)
    {
      this.inChars -= countInChars;
      this.inBytes -= countInBytes;
    }

    public void DecrementBy(MimeStringLength count)
    {
      this.inChars -= count.InChars;
      this.inBytes -= count.InBytes;
    }

    public void SetAs(int value)
    {
      this.inChars = value;
      this.inBytes = value;
    }

    public void SetAs(int valueInChars, int valueInBytes)
    {
      this.inChars = valueInChars;
      this.inBytes = valueInBytes;
    }

    public void SetAs(MimeStringLength value)
    {
      this.inChars = value.InChars;
      this.inBytes = value.InBytes;
    }

    public override string ToString()
    {
      return string.Format("InChars={0}, InBytes={1}", (object) this.inChars, (object) this.inBytes);
    }
  }
}
