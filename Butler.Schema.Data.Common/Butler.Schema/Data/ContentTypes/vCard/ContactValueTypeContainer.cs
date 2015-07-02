// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.vCard.ContactValueTypeContainer
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.ContentTypes.vCard
{
  internal class ContactValueTypeContainer : Internal.ValueTypeContainer
  {
    private ContactValueType valueType;

    public override bool IsTextType
    {
      get
      {
        this.CalculateValueType();
        if (this.valueType != ContactValueType.Text && this.valueType != ContactValueType.PhoneNumber)
          return this.valueType == ContactValueType.VCard;
        return true;
      }
    }

    public override bool CanBeMultivalued
    {
      get
      {
        this.CalculateValueType();
        if (this.valueType != ContactValueType.Binary)
          return this.valueType != ContactValueType.VCard;
        return false;
      }
    }

    public override bool CanBeCompound
    {
      get
      {
        this.CalculateValueType();
        if (this.valueType != ContactValueType.Binary)
          return this.valueType != ContactValueType.VCard;
        return false;
      }
    }

    public ContactValueType ValueType
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
      this.valueType = ContactValueType.Unknown;
      if (this.valueTypeParameter != null)
      {
        this.valueType = ContactCommon.GetValueTypeEnum(this.valueTypeParameter);
      }
      else
      {
        PropertyId propertyEnum = ContactCommon.GetPropertyEnum(this.propertyName);
        if (propertyEnum != PropertyId.Unknown)
          this.valueType = ContactCommon.GetDefaultValueType(propertyEnum);
      }
      if (this.valueType == ContactValueType.Unknown)
        this.valueType = ContactValueType.Text;
      this.isValueTypeInitialized = true;
    }
  }
}
