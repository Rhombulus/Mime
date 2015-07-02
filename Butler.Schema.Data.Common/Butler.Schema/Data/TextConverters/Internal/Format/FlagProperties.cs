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

      public bool IsClear => 0 == (int) this.Bits;

      public uint Mask => this.Bits & 2863311530U | (this.Bits & 2863311530U) >> 1;

      public uint Bits { get; private set; }

      internal int IntegerBag
    {
      get
      {
        return (int) this.Bits;
      }
      set
      {
        this.Bits = (uint) value;
      }
    }

    internal FlagProperties(uint bits)
    {
      this.Bits = bits;
    }

    public static FlagProperties operator &(FlagProperties x, FlagProperties y)
    {
      return new FlagProperties(x.Bits & (y.Bits & 2863311530U | (y.Bits & 2863311530U) >> 1));
    }

    public static FlagProperties operator |(FlagProperties x, FlagProperties y)
    {
      return FlagProperties.Merge(x, y);
    }

    public static FlagProperties operator ^(FlagProperties x, FlagProperties y)
    {
      uint num = (x.Bits ^ y.Bits) & x.Mask & y.Mask;
      return new FlagProperties(num | num << 1);
    }

    public static FlagProperties operator ~(FlagProperties x)
    {
      return new FlagProperties((uint) ~((int) x.Bits & -1431655766 | (int) ((x.Bits & 2863311530U) >> 1)));
    }

    public static bool operator ==(FlagProperties x, FlagProperties y)
    {
      return (int) x.Bits == (int) y.Bits;
    }

    public static bool operator !=(FlagProperties x, FlagProperties y)
    {
      return (int) x.Bits != (int) y.Bits;
    }

    public static bool IsFlagProperty(PropertyId id)
    {
      if (id >= PropertyId.FirstFlag)
        return id <= PropertyId.MergedCell;
      return false;
    }

    public static FlagProperties Merge(FlagProperties baseFlags, FlagProperties overrideFlags)
    {
      return new FlagProperties(baseFlags.Bits & ~((overrideFlags.Bits & 2863311530U) >> 1) | overrideFlags.Bits);
    }

    public void Set(PropertyId id, bool value)
    {
      int num = (int) (id - (byte) 1) * 2;
      if (value)
      {
        this.Bits |= (uint) (3 << num);
      }
      else
      {
        this.Bits &= (uint) ~(1 << num);
        this.Bits |= (uint) (2 << num);
      }
    }

    public void Remove(PropertyId id)
    {
      this.Bits &= (uint) ~(3 << (int) (id - (byte) 1) * 2);
    }

    public void ClearAll()
    {
      this.Bits = 0U;
    }

    public bool IsDefined(PropertyId id)
    {
      return 0 != ((int) this.Bits & 2 << (int) (id - (byte) 1) * 2);
    }

    public bool IsAnyDefined()
    {
      return (int) this.Bits != 0;
    }

    public bool IsOn(PropertyId id)
    {
      return 0 != ((int) this.Bits & 1 << (int) (id - (byte) 1) * 2);
    }

    public bool IsDefinedAndOn(PropertyId id)
    {
      return 3 == ((int) (this.Bits >> (int) (id - (byte) 1) * 2) & 3);
    }

    public bool IsDefinedAndOff(PropertyId id)
    {
      return 2 == ((int) (this.Bits >> (int) (id - (byte) 1) * 2) & 3);
    }

    public PropertyValue GetPropertyValue(PropertyId id)
    {
      int num = (int) (id - (byte) 1) * 2;
      if (((int) this.Bits & 2 << num) != 0)
        return new PropertyValue(0 != ((int) this.Bits & 1 << num));
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
      return 0 == ((int) this.Bits & -1431655766 & ~((int) overrideFlags.Bits & -1431655766));
    }

    public void Merge(FlagProperties overrideFlags)
    {
      this.Bits = this.Bits & ~((overrideFlags.Bits & 2863311530U) >> 1) | overrideFlags.Bits;
    }

    public void ReverseMerge(FlagProperties baseFlags)
    {
      this.Bits = baseFlags.Bits & ~((this.Bits & 2863311530U) >> 1) | this.Bits;
    }

    public override bool Equals(object obj)
    {
      if (obj is FlagProperties)
        return (int) this.Bits == (int) ((FlagProperties) obj).Bits;
      return false;
    }

    public override int GetHashCode()
    {
      return (int) this.Bits;
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
