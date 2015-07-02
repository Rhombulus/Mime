// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.ScratchBuffer
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  internal struct ScratchBuffer
  {

      public char[] Buffer { get; private set; }

      public int Offset => 0;

      public int Length { get; set; }

      public int Capacity
    {
      get
      {
        if (this.Buffer != null)
          return this.Buffer.Length;
        return 64;
      }
    }

    public BufferString BufferString => new BufferString(this.Buffer, 0, this.Length);

      public char this[int offset]
    {
      get
      {
        return this.Buffer[offset];
      }
      set
      {
        this.Buffer[offset] = value;
      }
    }

    public BufferString SubString(int offset, int count)
    {
      return new BufferString(this.Buffer, offset, count);
    }

    public void Reset()
    {
      this.Length = 0;
    }

    public void Reset(int space)
    {
      this.Length = 0;
      if (this.Buffer != null && this.Buffer.Length >= space)
        return;
      this.Buffer = new char[space];
    }

    public bool AppendRtfTokenText(Internal.Rtf.RtfToken token, int maxSize)
    {
      int num1 = 0;
      int space;
      while ((space = this.GetSpace(maxSize)) != 0)
      {
        Internal.Rtf.RtfToken.TextReader text = token.Text;
        int num2;
        if ((num2 = text.Read(this.Buffer, this.Length, space)) != 0)
        {
          this.Length += num2;
          num1 += num2;
        }
        else
          break;
      }
      return num1 != 0;
    }

    public bool AppendTokenText(Token token, int maxSize)
    {
      int num1 = 0;
      int space;
      while ((space = this.GetSpace(maxSize)) != 0)
      {
        Token.TextReader text = token.Text;
        int num2;
        if ((num2 = text.Read(this.Buffer, this.Length, space)) != 0)
        {
          this.Length += num2;
          num1 += num2;
        }
        else
          break;
      }
      return num1 != 0;
    }

    public bool AppendHtmlAttributeValue(Internal.Html.HtmlAttribute attr, int maxSize)
    {
      int num1 = 0;
      int space;
      while ((space = this.GetSpace(maxSize)) != 0)
      {
        Internal.Html.HtmlToken.AttributeValueTextReader attributeValueTextReader = attr.Value;
        int num2;
        if ((num2 = attributeValueTextReader.Read(this.Buffer, this.Length, space)) != 0)
        {
          this.Length += num2;
          num1 += num2;
        }
        else
          break;
      }
      return num1 != 0;
    }

    public bool AppendCssPropertyValue(Internal.Css.CssProperty prop, int maxSize)
    {
      int num1 = 0;
      int space;
      while ((space = this.GetSpace(maxSize)) != 0)
      {
        Internal.Css.CssToken.PropertyValueTextReader propertyValueTextReader = prop.Value;
        int num2;
        if ((num2 = propertyValueTextReader.Read(this.Buffer, this.Length, space)) != 0)
        {
          this.Length += num2;
          num1 += num2;
        }
        else
          break;
      }
      return num1 != 0;
    }

    public int AppendInt(int value)
    {
      int space = 1;
      bool flag = false;
      if (value < 0)
      {
        flag = true;
        value = -value;
        ++space;
        if (value < 0)
          value = int.MaxValue;
      }
      int num1 = value;
      while (num1 >= 10)
      {
        num1 /= 10;
        ++space;
      }
      this.EnsureSpace(space);
      int num2 = this.Length + space;
      while (value >= 10)
      {
        this.Buffer[--num2] = (char) (value % 10 + 48);
        value /= 10;
      }
      int num3;
      this.Buffer[num3 = num2 - 1] = (char) (value + 48);
      if (flag)
      {
        int num4;
        this.Buffer[num4 = num3 - 1] = '-';
      }
      this.Length += space;
      return space;
    }

    public int AppendFractional(int value, int decimalPoint)
    {
      int num1 = this.AppendInt(value / decimalPoint);
      if (value % decimalPoint != 0)
      {
        if (value < 0)
          value = -value;
        int num2 = (int) (((long) value * 100L + (long) (decimalPoint / 2)) / (long) decimalPoint) % 100;
        if (num2 != 0)
        {
          int num3 = num1 + this.Append('.');
          if (num2 % 10 == 0)
            num2 /= 10;
          num1 = num3 + this.AppendInt(num2);
        }
      }
      return num1;
    }

    public int AppendHex2(uint value)
    {
      this.EnsureSpace(2);
      uint num1 = value >> 4 & 15U;
      this.Buffer[this.Length++] = num1 >= 10U ? (char) ((int) num1 - 10 + 65) : (char) (num1 + 48U);
      uint num2 = value & 15U;
      this.Buffer[this.Length++] = num2 >= 10U ? (char) ((int) num2 - 10 + 65) : (char) (num2 + 48U);
      return 2;
    }

    public int Append(char ch)
    {
      return this.Append(ch, int.MaxValue);
    }

    public int Append(char ch, int maxSize)
    {
      if (this.GetSpace(maxSize) == 0)
        return 0;
      this.Buffer[this.Length++] = ch;
      return 1;
    }

    public int Append(string str)
    {
      return this.Append(str, int.MaxValue);
    }

    public int Append(string str, int maxSize)
    {
      int sourceIndex = 0;
      int count;
      while ((count = Math.Min(this.GetSpace(maxSize), str.Length - sourceIndex)) != 0)
      {
        str.CopyTo(sourceIndex, this.Buffer, this.Length, count);
        this.Length += count;
        sourceIndex += count;
      }
      return sourceIndex;
    }

    public int Append(char[] buffer, int offset, int length)
    {
      return this.Append(buffer, offset, length, int.MaxValue);
    }

    public int Append(char[] buffer, int offset, int length, int maxSize)
    {
      int num1 = 0;
      int num2;
      while ((num2 = Math.Min(this.GetSpace(maxSize), length)) != 0)
      {
        System.Buffer.BlockCopy((Array) buffer, offset * 2, (Array) this.Buffer, this.Length * 2, num2 * 2);
        this.Length += num2;
        offset += num2;
        length -= num2;
        num1 += num2;
      }
      return num1;
    }

    public string ToString(int offset, int count)
    {
      return new string(this.Buffer, offset, count);
    }

    public void DisposeBuffer()
    {
      this.Buffer = (char[]) null;
      this.Length = 0;
    }

    private int GetSpace(int maxSize)
    {
      if (this.Length >= maxSize)
        return 0;
      if (this.Buffer == null)
        this.Buffer = new char[64];
      else if (this.Buffer.Length == this.Length)
      {
        char[] chArray = new char[this.Buffer.Length * 2];
        System.Buffer.BlockCopy((Array) this.Buffer, 0, (Array) chArray, 0, this.Length * 2);
        this.Buffer = chArray;
      }
      return this.Buffer.Length - this.Length;
    }

    private void EnsureSpace(int space)
    {
      if (this.Buffer == null)
      {
        this.Buffer = new char[Math.Max(space, 64)];
      }
      else
      {
        if (this.Buffer.Length - this.Length >= space)
          return;
        char[] chArray = new char[Math.Max(this.Buffer.Length * 2, this.Length + space)];
        System.Buffer.BlockCopy((Array) this.Buffer, 0, (Array) chArray, 0, this.Length * 2);
        this.Buffer = chArray;
      }
    }
  }
}
