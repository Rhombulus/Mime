// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Format.StringValue
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Format
{
  internal struct StringValue
  {
    internal FormatStore.StringValueStore Strings;
    internal int StringHandle;

    public PropertyValue PropertyValue
    {
      get
      {
        return new PropertyValue(PropertyType.String, this.StringHandle);
      }
    }

    public int Length
    {
      get
      {
        return this.Strings.Plane(this.StringHandle)[this.Strings.Index(this.StringHandle)].Str.Length;
      }
    }

    public int RefCount
    {
      get
      {
        return this.Strings.Plane(this.StringHandle)[this.Strings.Index(this.StringHandle)].RefCount;
      }
    }

    internal int Handle
    {
      get
      {
        return this.StringHandle;
      }
    }

    internal StringValue(FormatStore store, int stringHandle)
    {
      this.Strings = store.Strings;
      this.StringHandle = stringHandle;
    }

    internal StringValue(FormatStore.StringValueStore strings, int stringHandle)
    {
      this.Strings = strings;
      this.StringHandle = stringHandle;
    }

    public string GetString()
    {
      return this.Strings.Plane(this.StringHandle)[this.Strings.Index(this.StringHandle)].Str;
    }

    public void CopyTo(int sourceOffset, char[] buffer, int offset, int count)
    {
      this.Strings.Plane(this.StringHandle)[this.Strings.Index(this.StringHandle)].Str.CopyTo(sourceOffset, buffer, offset, count);
    }

    public void AddRef()
    {
      if (this.Strings.Plane(this.StringHandle)[this.Strings.Index(this.StringHandle)].RefCount == int.MaxValue)
        return;
      ++this.Strings.Plane(this.StringHandle)[this.Strings.Index(this.StringHandle)].RefCount;
    }

    public void Release()
    {
      if (this.Strings.Plane(this.StringHandle)[this.Strings.Index(this.StringHandle)].RefCount != int.MaxValue && --this.Strings.Plane(this.StringHandle)[this.Strings.Index(this.StringHandle)].RefCount == 0)
        this.Strings.Free(this.StringHandle);
      this.StringHandle = -1;
    }

    internal void SetString(string str)
    {
      this.Strings.Plane(this.StringHandle)[this.Strings.Index(this.StringHandle)].Str = str;
    }
  }
}
