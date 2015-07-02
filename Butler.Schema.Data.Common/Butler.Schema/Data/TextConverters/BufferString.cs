// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.BufferString
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Diagnostics;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  internal struct BufferString
  {
    public static readonly BufferString Null = new BufferString();
    private char[] buffer;
    private int offset;
    private int count;

    public char[] Buffer
    {
      get
      {
        return this.buffer;
      }
    }

    public int Offset
    {
      get
      {
        return this.offset;
      }
    }

    public int Length
    {
      get
      {
        return this.count;
      }
    }

    public bool IsEmpty
    {
      get
      {
        return this.count == 0;
      }
    }

    public char this[int index]
    {
      get
      {
        return this.buffer[this.offset + index];
      }
    }

    public BufferString(char[] buffer, int offset, int count)
    {
      this.buffer = buffer;
      this.offset = offset;
      this.count = count;
    }

    public static int CompareLowerCaseStringToBufferStringIgnoreCase(string left, BufferString right)
    {
      int num1 = Math.Min(left.Length, right.Length);
      for (int index = 0; index < num1; ++index)
      {
        int num2 = (int) left[index] - (int) ParseSupport.ToLowerCase(right[index]);
        if (num2 != 0)
          return num2;
      }
      return left.Length - right.Length;
    }

    public void Set(char[] buffer, int offset, int count)
    {
      this.buffer = buffer;
      this.offset = offset;
      this.count = count;
    }

    public BufferString SubString(int offset, int count)
    {
      return new BufferString(this.buffer, this.offset + offset, count);
    }

    public void Trim(int offset, int count)
    {
      this.offset += offset;
      this.count = count;
    }

    public void TrimWhitespace()
    {
      for (; this.count != 0 && ParseSupport.WhitespaceCharacter(this.buffer[this.offset]); --this.count)
        ++this.offset;
      if (this.count == 0)
        return;
      int num = this.offset + this.count - 1;
      while (ParseSupport.WhitespaceCharacter(this.buffer[num--]))
        --this.count;
    }

    public bool EqualsToString(string rightPart)
    {
      if (this.count != rightPart.Length)
        return false;
      for (int index = 0; index < rightPart.Length; ++index)
      {
        if ((int) this.buffer[this.offset + index] != (int) rightPart[index])
          return false;
      }
      return true;
    }

    public bool EqualsToLowerCaseStringIgnoreCase(string rightPart)
    {
      if (this.count != rightPart.Length)
        return false;
      for (int index = 0; index < rightPart.Length; ++index)
      {
        if ((int) ParseSupport.ToLowerCase(this.buffer[this.offset + index]) != (int) rightPart[index])
          return false;
      }
      return true;
    }

    public bool StartsWithLowerCaseStringIgnoreCase(string rightPart)
    {
      if (this.count < rightPart.Length)
        return false;
      for (int index = 0; index < rightPart.Length; ++index)
      {
        if ((int) ParseSupport.ToLowerCase(this.buffer[this.offset + index]) != (int) rightPart[index])
          return false;
      }
      return true;
    }

    public bool StartsWithString(string rightPart)
    {
      if (this.count < rightPart.Length)
        return false;
      for (int index = 0; index < rightPart.Length; ++index)
      {
        if ((int) this.buffer[this.offset + index] != (int) rightPart[index])
          return false;
      }
      return true;
    }

    public bool EndsWithLowerCaseStringIgnoreCase(string rightPart)
    {
      if (this.count < rightPart.Length)
        return false;
      int num = this.offset + this.count - rightPart.Length;
      for (int index = 0; index < rightPart.Length; ++index)
      {
        if ((int) ParseSupport.ToLowerCase(this.buffer[num + index]) != (int) rightPart[index])
          return false;
      }
      return true;
    }

    public bool EndsWithString(string rightPart)
    {
      if (this.count < rightPart.Length)
        return false;
      int num = this.offset + this.count - rightPart.Length;
      for (int index = 0; index < rightPart.Length; ++index)
      {
        if ((int) this.buffer[num + index] != (int) rightPart[index])
          return false;
      }
      return true;
    }

    public override string ToString()
    {
      if (this.buffer == null)
        return (string) null;
      if (this.count != 0)
        return new string(this.buffer, this.offset, this.count);
      return string.Empty;
    }

    [Conditional("DEBUG")]
    private static void AssertStringIsLowerCase(string rightPart)
    {
    }
  }
}
