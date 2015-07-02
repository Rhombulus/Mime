// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.Internal.ValueTypeContainer
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.ContentTypes.Internal
{
  internal abstract class ValueTypeContainer
  {
    protected string valueTypeParameter;
    protected string propertyName;
    protected bool isValueTypeInitialized;

    public abstract bool IsTextType { get; }

    public abstract bool CanBeMultivalued { get; }

    public abstract bool CanBeCompound { get; }

    public bool IsInitialized
    {
      get
      {
        return this.propertyName != null;
      }
    }

    protected ValueTypeContainer()
    {
      this.Reset();
    }

    public void SetValueTypeParameter(string value)
    {
      this.valueTypeParameter = value;
      this.isValueTypeInitialized = false;
    }

    public void SetPropertyName(string value)
    {
      this.propertyName = value;
      this.isValueTypeInitialized = false;
    }

    public virtual void Reset()
    {
      this.valueTypeParameter = (string) null;
      this.propertyName = (string) null;
      this.isValueTypeInitialized = false;
    }
  }
}
