// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Format.MultiValue
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Format
{
  internal struct MultiValue
  {
    internal FormatStore.MultiValueStore MultiValues;
    internal int MultiValueHandle;

    public PropertyValue PropertyValue
    {
      get
      {
        return new PropertyValue(PropertyType.MultiValue, this.MultiValueHandle);
      }
    }

    public int Length
    {
      get
      {
        return this.MultiValues.Plane(this.MultiValueHandle)[this.MultiValues.Index(this.MultiValueHandle)].Values.Length;
      }
    }

    internal int Handle
    {
      get
      {
        return this.MultiValueHandle;
      }
    }

    internal int RefCount
    {
      get
      {
        return this.MultiValues.Plane(this.MultiValueHandle)[this.MultiValues.Index(this.MultiValueHandle)].RefCount;
      }
    }

    public PropertyValue this[int index]
    {
      get
      {
        return this.MultiValues.Plane(this.MultiValueHandle)[this.MultiValues.Index(this.MultiValueHandle)].Values[index];
      }
    }

    internal MultiValue(FormatStore store, int multiValueHandle)
    {
      this.MultiValues = store.MultiValues;
      this.MultiValueHandle = multiValueHandle;
    }

    internal MultiValue(FormatStore.MultiValueStore multiValues, int multiValueHandle)
    {
      this.MultiValues = multiValues;
      this.MultiValueHandle = multiValueHandle;
    }

    public StringValue GetStringValue(int index)
    {
      return this.MultiValues.Store.GetStringValue(this.MultiValues.Plane(this.MultiValueHandle)[this.MultiValues.Index(this.MultiValueHandle)].Values[index]);
    }

    public void AddRef()
    {
      if (this.MultiValues.Plane(this.MultiValueHandle)[this.MultiValues.Index(this.MultiValueHandle)].RefCount == int.MaxValue)
        return;
      ++this.MultiValues.Plane(this.MultiValueHandle)[this.MultiValues.Index(this.MultiValueHandle)].RefCount;
    }

    public void Release()
    {
      if (this.MultiValues.Plane(this.MultiValueHandle)[this.MultiValues.Index(this.MultiValueHandle)].RefCount != int.MaxValue && --this.MultiValues.Plane(this.MultiValueHandle)[this.MultiValues.Index(this.MultiValueHandle)].RefCount == 0)
        this.MultiValues.Free(this.MultiValueHandle);
      this.MultiValueHandle = -1;
    }
  }
}
