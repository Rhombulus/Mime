// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Format.MultiValueBuildHelper
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Format
{
  internal struct MultiValueBuildHelper
  {
    internal FormatStore Store;
    internal PropertyValue[] Values;
    internal int ValuesCount;

    public int Count
    {
      get
      {
        return this.ValuesCount;
      }
    }

    public PropertyValue this[int i]
    {
      get
      {
        return this.Values[i];
      }
    }

    internal MultiValueBuildHelper(FormatStore store)
    {
      this.Store = store;
      this.Values = (PropertyValue[]) null;
      this.ValuesCount = 0;
    }

    public void AddValue(PropertyValue value)
    {
      if (this.Values == null)
        this.Values = new PropertyValue[4];
      else if (this.ValuesCount == this.Values.Length)
      {
        if (this.ValuesCount == 32)
          throw new TextConvertersException(CtsResources.TextConvertersStrings.InputDocumentTooComplex);
        PropertyValue[] propertyValueArray = new PropertyValue[this.ValuesCount * 2];
        Array.Copy((Array) this.Values, 0, (Array) propertyValueArray, 0, this.ValuesCount);
        this.Values = propertyValueArray;
      }
      this.Values[this.ValuesCount++] = value;
    }

    public PropertyValue[] GetValues()
    {
      if (this.ValuesCount == 0)
        return (PropertyValue[]) null;
      PropertyValue[] propertyValueArray = new PropertyValue[this.ValuesCount];
      Array.Copy((Array) this.Values, 0, (Array) propertyValueArray, 0, this.ValuesCount);
      this.ValuesCount = 0;
      return propertyValueArray;
    }

    public void Cancel()
    {
      for (int index = 0; index < this.ValuesCount; ++index)
      {
        if (this.Values[index].IsRefCountedHandle)
          this.Store.ReleaseValue(this.Values[index]);
      }
      this.ValuesCount = 0;
    }
  }
}
