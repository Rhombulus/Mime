// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Format.MultiValueBuilder
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Format
{
  internal struct MultiValueBuilder
  {
    private FormatConverter converter;
    private int handle;

    public int Count => this.converter.MultiValueBuildHelper.Count;

      public PropertyValue this[int i] => this.converter.MultiValueBuildHelper[i];

      internal MultiValueBuilder(FormatConverter converter, int handle)
    {
      this.converter = converter;
      this.handle = handle;
    }

    public void AddValue(PropertyValue value)
    {
      this.converter.MultiValueBuildHelper.AddValue(value);
    }

    public void AddStringValue(StringValue value)
    {
      this.converter.MultiValueBuildHelper.AddValue(value.PropertyValue);
    }

    public void AddStringValue(string value)
    {
      this.converter.MultiValueBuildHelper.AddValue(this.converter.RegisterStringValue(false, value).PropertyValue);
    }

    public void AddStringValue(char[] buffer, int offset, int count)
    {
      this.converter.MultiValueBuildHelper.AddValue(this.converter.RegisterStringValue(false, new BufferString(buffer, offset, count)).PropertyValue);
    }

    public void Flush()
    {
      this.converter.Store.MultiValues.Plane(this.handle)[this.converter.Store.MultiValues.Index(this.handle)].Values = this.converter.MultiValueBuildHelper.GetValues();
    }

    public void Cancel()
    {
      this.converter.MultiValueBuildHelper.Cancel();
    }
  }
}
