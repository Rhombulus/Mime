// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Format.FlagProperties
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters.Internal.Format
{
  internal struct FlagProperties
  {
    public static readonly FlagProperties AllUndefined = new FlagProperties(0U);
    public static readonly FlagProperties AllOff = new FlagProperties(0U);
    public static readonly FlagProperties AllOn = new FlagProperties(uint.MaxValue);
    private const uint AllDefinedBits = 2863311530U;
    private const uint AllValueBits = 1431655765U;
    private const uint ValueBit = 1U;
    private const uint DefinedBit = 2U;
    private const uint ValueAndDefinedBits = 3U;
    private uint bits;

    public bool IsClear
    {
      get
      {
        return 0 == (int) this.bits;
      }
    }

    public uint Mask
    {
      get
      {
        return this.bits & 2863311530U | (this.bits & 2863311530U) >> 1;
      }
    }

    public uint Bits
    {
      get
      {
        return this.bits;
      }
    }

    internal int IntegerBag
    {
      get
      {
        return (int) this.bits;
      }
      set
      {
        this.bits = (uint) value;
      }
    }

    internal FlagProperties(uint bits)
    {
      this.bits = bits;
    }

    public static FlagProperties operator &(FlagProperties x, FlagProperties y)
    {
      return new FlagProperties(x.bits & (y.bits & 2863311530U | (y.bits & 2863311530U) >> 1));
    }

    public static FlagProperties operator |(FlagProperties x, FlagProperties y)
    {
      return FlagProperties.Merge(x, y);
    }

    public static FlagProperties operator ^(FlagProperties x, FlagProperties y)
    {
      uint num = (x.bits ^ y.bits) & x.Mask & y.Mask;
      return new FlagProperties(num | num << 1);
    }

    public static FlagProperties operator ~(FlagProperties x)
    {
      return new FlagProperties((uint) ~((int) x.bits & -1431655766 | (int) ((x.bits & 2863311530U) >> 1)));
    }

    public static bool operator ==(FlagProperties x, FlagProperties y)
    {
      return (int) x.bits == (int) y.bits;
    }

    public static bool operator !=(FlagProperties x, FlagProperties y)
    {
      return (int) x.bits != (int) y.bits;
    }

    public static bool IsFlagProperty(PropertyId id)
    {
      if (id >= PropertyId.FirstFlag)
        return id <= PropertyId.MergedCell;
      return false;
    }

    public static FlagProperties Merge(FlagProperties baseFlags, FlagProperties overrideFlags)
    {
      return new FlagProperties(baseFlags.bits & ~((overrideFlags.bits & 2863311530U) >> 1) | overrideFlags.bits);
    }

    public void Set(PropertyId id, bool value)
    {
      int num = (int) (id - (byte) 1) * 2;
      if (value)
      {
        this.bits |= (uint) (3 << num);
      }
      else
      {
        this.bits &= (uint) ~(1 << num);
        this.bits |= (uint) (2 << num);
      }
    }

    public void Remove(PropertyId id)
    {
      this.bits &= (uint) ~(3 << (int) (id - (byte) 1) * 2);
    }

    public void ClearAll()
    {
      this.bits = 0U;
    }

    public bool IsDefined(PropertyId id)
    {
      return 0 != ((int) this.bits & 2 << (int) (id - (byte) 1) * 2);
    }

    public bool IsAnyDefined()
    {
      return (int) this.bits != 0;
    }

    public bool IsOn(PropertyId id)
    {
      return 0 != ((int) this.bits & 1 << (int) (id - (byte) 1) * 2);
    }

    public bool IsDefinedAndOn(PropertyId id)
    {
      return 3 == ((int) (this.bits >> (int) (id - (byte) 1) * 2) & 3);
    }

    public bool IsDefinedAndOff(PropertyId id)
    {
      return 2 == ((int) (this.bits >> (int) (id - (byte) 1) * 2) & 3);
    }

    public PropertyValue GetPropertyValue(PropertyId id)
    {
      int num = (int) (id - (byte) 1) * 2;
      if (((int) this.bits & 2 << num) != 0)
        return new PropertyValue(0 != ((int) this.bits & 1 << num));
      return PropertyValue.Null;
    }

    public void SetPropertyValue(PropertyId id, PropertyValue value)
    {
      if (!value.IsBool)
        return;
      this.Set(id, value.Bool);
    }

    public bool IsSubsetOf(FlagProperties overrideFlags)
    {
      return 0 == ((int) this.bits & -1431655766 & ~((int) overrideFlags.bits & -1431655766));
    }

    public void Merge(FlagProperties overrideFlags)
    {
      this.bits = this.bits & ~((overrideFlags.bits & 2863311530U) >> 1) | overrideFlags.bits;
    }

    public void ReverseMerge(FlagProperties baseFlags)
    {
      this.bits = baseFlags.bits & ~((this.bits & 2863311530U) >> 1) | this.bits;
    }

    public override bool Equals(object obj)
    {
      if (obj is FlagProperties)
        return (int) this.bits == (int) ((FlagProperties) obj).bits;
      return false;
    }

    public override int GetHashCode()
    {
      return (int) this.bits;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(240);
      for (PropertyId id = PropertyId.FirstFlag; id <= PropertyId.MergedCell; ++id)
      {
        if (this.IsDefined(id))
        {
          if (stringBuilder.Length != 0)
            stringBuilder.Append(", ");
          stringBuilder.Append(id.ToString());
          stringBuilder.Append(this.IsOn(id) ? ":on" : ":off");
        }
      }
      return stringBuilder.ToString();
    }
  }
}
