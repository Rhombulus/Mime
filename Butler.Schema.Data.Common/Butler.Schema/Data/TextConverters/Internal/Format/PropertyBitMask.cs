// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Format.PropertyBitMask
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters.Internal.Format
{
  internal struct PropertyBitMask
  {
    public static readonly PropertyBitMask AllOff = new PropertyBitMask(0U, 0U);
    public static readonly PropertyBitMask AllOn = new PropertyBitMask(uint.MaxValue, uint.MaxValue);
    public const PropertyId FirstNonFlag = PropertyId.FontColor;
    internal uint Bits1;
    internal uint Bits2;

    public bool IsClear
    {
      get
      {
        if ((int) this.Bits1 == 0)
          return 0 == (int) this.Bits2;
        return false;
      }
    }

    internal PropertyBitMask(uint bits1, uint bits2)
    {
      this.Bits1 = bits1;
      this.Bits2 = bits2;
    }

    public static PropertyBitMask operator |(PropertyBitMask x, PropertyBitMask y)
    {
      return new PropertyBitMask(x.Bits1 | y.Bits1, x.Bits2 | y.Bits2);
    }

    public static PropertyBitMask operator &(PropertyBitMask x, PropertyBitMask y)
    {
      return new PropertyBitMask(x.Bits1 & y.Bits1, x.Bits2 & y.Bits2);
    }

    public static PropertyBitMask operator ^(PropertyBitMask x, PropertyBitMask y)
    {
      return new PropertyBitMask(x.Bits1 ^ y.Bits1, x.Bits2 ^ y.Bits2);
    }

    public static PropertyBitMask operator ~(PropertyBitMask x)
    {
      return new PropertyBitMask(~x.Bits1, ~x.Bits2);
    }

    public static bool operator ==(PropertyBitMask x, PropertyBitMask y)
    {
      if ((int) x.Bits1 == (int) y.Bits1)
        return (int) x.Bits2 == (int) y.Bits2;
      return false;
    }

    public static bool operator !=(PropertyBitMask x, PropertyBitMask y)
    {
      if ((int) x.Bits1 == (int) y.Bits1)
        return (int) x.Bits2 != (int) y.Bits2;
      return true;
    }

    public void Or(PropertyBitMask newBits)
    {
      this.Bits1 |= newBits.Bits1;
      this.Bits2 |= newBits.Bits2;
    }

    public bool IsSet(PropertyId id)
    {
      return 0 != (id < PropertyId.ListLevel ? (int) this.Bits1 & 1 << (int) (id - (byte) 17) : (int) this.Bits2 & 1 << (int) (id - (byte) 17 - (byte) 32));
    }

    public bool IsNotSet(PropertyId id)
    {
      return 0 == (id < PropertyId.ListLevel ? (int) this.Bits1 & 1 << (int) (id - (byte) 17) : (int) this.Bits2 & 1 << (int) (id - (byte) 17 - (byte) 32));
    }

    public void Set(PropertyId id)
    {
      if (id < PropertyId.ListLevel)
        this.Bits1 |= 1U << (int) (id - (byte) 17);
      else
        this.Bits2 |= 1U << (int) (id - (byte) 17 - (byte) 32);
    }

    public void Clear(PropertyId id)
    {
      if (id < PropertyId.ListLevel)
        this.Bits1 &= (uint) ~(1 << (int) (id - (byte) 17));
      else
        this.Bits2 &= (uint) ~(1 << (int) (id - (byte) 17 - (byte) 32));
    }

    public bool IsSubsetOf(PropertyBitMask overrideFlags)
    {
      if (((int) this.Bits1 & ~(int) overrideFlags.Bits1) == 0)
        return 0 == ((int) this.Bits2 & ~(int) overrideFlags.Bits2);
      return false;
    }

    public void ClearAll()
    {
      this.Bits1 = 0U;
      this.Bits2 = 0U;
    }

    public override bool Equals(object obj)
    {
      if (obj is PropertyBitMask && (int) this.Bits1 == (int) ((PropertyBitMask) obj).Bits1)
        return (int) this.Bits2 == (int) ((PropertyBitMask) obj).Bits2;
      return false;
    }

    public override int GetHashCode()
    {
      return (int) this.Bits1 ^ (int) this.Bits2;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(864);
      for (PropertyId id = PropertyId.FontColor; id < PropertyId.MaxValue; ++id)
      {
        if (this.IsSet(id))
        {
          if (stringBuilder.Length != 0)
            stringBuilder.Append(", ");
          stringBuilder.Append(id.ToString());
        }
      }
      return stringBuilder.ToString();
    }

    public PropertyBitMask.DefinedPropertyIdEnumerator GetEnumerator()
    {
      return new PropertyBitMask.DefinedPropertyIdEnumerator(this);
    }

    internal void Set1(uint bits1)
    {
      this.Bits1 = bits1;
    }

    internal void Set2(uint bits2)
    {
      this.Bits2 = bits2;
    }

    public struct DefinedPropertyIdEnumerator
    {
      internal ulong Bits;
      internal ulong CurrentBit;
      internal PropertyId CurrentId;

      public PropertyId Current => this.CurrentId;

        internal DefinedPropertyIdEnumerator(PropertyBitMask mask)
      {
        this.Bits = (ulong) mask.Bits2 << 32 | (ulong) mask.Bits1;
        this.CurrentBit = 1UL;
        this.CurrentId = (long) this.Bits != 0L ? PropertyId.MergedCell : PropertyId.MaxValue;
      }

      public bool MoveNext()
      {
        while (this.CurrentId != PropertyId.MaxValue)
        {
          if (this.CurrentId != PropertyId.MergedCell)
            this.CurrentBit <<= 1;
          ++this.CurrentId;
          if (this.CurrentId != PropertyId.MaxValue && 0L != ((long) this.Bits & (long) this.CurrentBit))
            return true;
        }
        return false;
      }
    }
  }
}
