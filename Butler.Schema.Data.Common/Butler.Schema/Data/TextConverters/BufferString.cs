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

      public char[] Buffer { get; private set; }

      public int Offset { get; private set; }

      public int Length { get; private set; }

      public bool IsEmpty => this.Length == 0;

      public char this[int index] => this.Buffer[this.Offset + index];

      public BufferString(char[] buffer, int offset, int count)
    {
      this.Buffer = buffer;
      this.Offset = offset;
      this.Length = count;
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
      this.Buffer = buffer;
      this.Offset = offset;
      this.Length = count;
    }

    public BufferString SubString(int offset, int count)
    {
      return new BufferString(this.Buffer, this.Offset + offset, count);
    }

    public void Trim(int offset, int count)
    {
      this.Offset += offset;
      this.Length = count;
    }

    public void TrimWhitespace()
    {
      for (; this.Length != 0 && ParseSupport.WhitespaceCharacter(this.Buffer[this.Offset]); --this.Length)
        ++this.Offset;
      if (this.Length == 0)
        return;
      int num = this.Offset + this.Length - 1;
      while (ParseSupport.WhitespaceCharacter(this.Buffer[num--]))
        --this.Length;
    }

    public bool EqualsToString(string rightPart)
    {
      if (this.Length != rightPart.Length)
        return false;
      for (int index = 0; index < rightPart.Length; ++index)
      {
        if ((int) this.Buffer[this.Offset + index] != (int) rightPart[index])
          return false;
      }
      return true;
    }

    public bool EqualsToLowerCaseStringIgnoreCase(string rightPart)
    {
      if (this.Length != rightPart.Length)
        return false;
      for (int index = 0; index < rightPart.Length; ++index)
      {
        if ((int) ParseSupport.ToLowerCase(this.Buffer[this.Offset + index]) != (int) rightPart[index])
          return false;
      }
      return true;
    }

    public bool StartsWithLowerCaseStringIgnoreCase(string rightPart)
    {
      if (this.Length < rightPart.Length)
        return false;
      for (int index = 0; index < rightPart.Length; ++index)
      {
        if ((int) ParseSupport.ToLowerCase(this.Buffer[this.Offset + index]) != (int) rightPart[index])
          return false;
      }
      return true;
    }

    public bool StartsWithString(string rightPart)
    {
      if (this.Length < rightPart.Length)
        return false;
      for (int index = 0; index < rightPart.Length; ++index)
      {
        if ((int) this.Buffer[this.Offset + index] != (int) rightPart[index])
          return false;
      }
      return true;
    }

    public bool EndsWithLowerCaseStringIgnoreCase(string rightPart)
    {
      if (this.Length < rightPart.Length)
        return false;
      int num = this.Offset + this.Length - rightPart.Length;
      for (int index = 0; index < rightPart.Length; ++index)
      {
        if ((int) ParseSupport.ToLowerCase(this.Buffer[num + index]) != (int) rightPart[index])
          return false;
      }
      return true;
    }

    public bool EndsWithString(string rightPart)
    {
      if (this.Length < rightPart.Length)
        return false;
      int num = this.Offset + this.Length - rightPart.Length;
      for (int index = 0; index < rightPart.Length; ++index)
      {
        if ((int) this.Buffer[num + index] != (int) rightPart[index])
          return false;
      }
      return true;
    }

    public override string ToString()
    {
      if (this.Buffer == null)
        return (string) null;
      if (this.Length != 0)
        return new string(this.Buffer, this.Offset, this.Length);
      return string.Empty;
    }

    [Conditional("DEBUG")]
    private static void AssertStringIsLowerCase(string rightPart)
    {
    }
  }
}
