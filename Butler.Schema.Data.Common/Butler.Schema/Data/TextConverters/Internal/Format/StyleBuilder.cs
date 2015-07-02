// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Format.StyleBuilder
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Format
{
  internal struct StyleBuilder
  {
    private FormatConverter converter;
    private int handle;

    internal StyleBuilder(FormatConverter converter, int handle)
    {
      this.converter = converter;
      this.handle = handle;
    }

    public void SetProperty(PropertyId propertyId, PropertyValue value)
    {
      this.converter.StyleBuildHelper.SetProperty(0, propertyId, value);
    }

    public void SetProperties(Property[] properties, int propertyCount)
    {
      for (int index = 0; index < propertyCount; ++index)
        this.SetProperty(properties[index].Id, properties[index].Value);
    }

    public void SetStringProperty(PropertyId propertyId, StringValue value)
    {
      this.converter.StyleBuildHelper.SetProperty(0, propertyId, value.PropertyValue);
    }

    public void SetStringProperty(PropertyId propertyId, string value)
    {
      StringValue stringValue = this.converter.RegisterStringValue(false, value);
      this.converter.StyleBuildHelper.SetProperty(0, propertyId, stringValue.PropertyValue);
    }

    public void SetStringProperty(PropertyId propertyId, BufferString value)
    {
      StringValue stringValue = this.converter.RegisterStringValue(false, value);
      this.converter.StyleBuildHelper.SetProperty(0, propertyId, stringValue.PropertyValue);
    }

    public void SetStringProperty(PropertyId propertyId, char[] buffer, int offset, int count)
    {
      StringValue stringValue = this.converter.RegisterStringValue(false, new BufferString(buffer, offset, count));
      this.converter.StyleBuildHelper.SetProperty(0, propertyId, stringValue.PropertyValue);
    }

    public void SetMultiValueProperty(PropertyId propertyId, MultiValue value)
    {
      value.AddRef();
      this.converter.StyleBuildHelper.SetProperty(0, propertyId, value.PropertyValue);
    }

    public void SetMultiValueProperty(PropertyId propertyId, out MultiValueBuilder multiValueBuilder)
    {
      MultiValue multiValue = this.converter.RegisterMultiValue(false, out multiValueBuilder);
      this.converter.StyleBuildHelper.SetProperty(0, propertyId, multiValue.PropertyValue);
    }

    public void Flush()
    {
      this.converter.StyleBuildHelper.GetPropertyList(out this.converter.Store.Styles.Plane(this.handle)[this.converter.Store.Styles.Index(this.handle)].PropertyList, out this.converter.Store.Styles.Plane(this.handle)[this.converter.Store.Styles.Index(this.handle)].FlagProperties, out this.converter.Store.Styles.Plane(this.handle)[this.converter.Store.Styles.Index(this.handle)].PropertyMask);
    }
  }
}
