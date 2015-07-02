// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.iCalendar.CalendarValueTypeContainer
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.ContentTypes.iCalendar
{
  internal class CalendarValueTypeContainer : Internal.ValueTypeContainer
  {
    private CalendarValueType valueType;

    public override bool IsTextType
    {
      get
      {
        this.CalculateValueType();
        return this.valueType == CalendarValueType.Text;
      }
    }

    public override bool CanBeMultivalued
    {
      get
      {
        this.CalculateValueType();
        if (this.valueType != CalendarValueType.Recurrence)
          return this.valueType != CalendarValueType.Binary;
        return false;
      }
    }

    public override bool CanBeCompound
    {
      get
      {
        this.CalculateValueType();
        if (this.valueType != CalendarValueType.Recurrence)
          return this.valueType != CalendarValueType.Binary;
        return false;
      }
    }

    public CalendarValueType ValueType
    {
      get
      {
        this.CalculateValueType();
        return this.valueType;
      }
    }

    private void CalculateValueType()
    {
      if (this.isValueTypeInitialized)
        return;
      this.valueType = CalendarValueType.Unknown;
      if (this.valueTypeParameter != null)
      {
        this.valueType = CalendarCommon.GetValueTypeEnum(this.valueTypeParameter);
      }
      else
      {
        PropertyId propertyEnum = CalendarCommon.GetPropertyEnum(this.propertyName);
        if (propertyEnum != PropertyId.Unknown)
          this.valueType = CalendarCommon.GetDefaultValueType(propertyEnum);
      }
      if (this.valueType == CalendarValueType.Unknown)
        this.valueType = CalendarValueType.Text;
      this.isValueTypeInitialized = true;
    }
  }
}
