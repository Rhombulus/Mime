// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.Tnef.TnefNameId
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.ContentTypes.Tnef
{
  public struct TnefNameId
  {

      public Guid PropertySetGuid { get; private set; }

      public TnefNameIdKind Kind { get; private set; }

      public string Name { get; private set; }

      public int Id { get; private set; }

      public TnefNameId(Guid propertySetGuid, int id)
    {
      this.PropertySetGuid = propertySetGuid;
      this.Id = id;
      this.Name = (string) null;
      this.Kind = TnefNameIdKind.Id;
    }

    public TnefNameId(Guid propertySetGuid, string name)
    {
      this.PropertySetGuid = propertySetGuid;
      this.Id = 0;
      this.Name = name;
      this.Kind = TnefNameIdKind.Name;
    }

    internal void Set(Guid propertySetGuid, int id)
    {
      this.PropertySetGuid = propertySetGuid;
      this.Id = id;
      this.Name = (string) null;
      this.Kind = TnefNameIdKind.Id;
    }

    internal void Set(Guid propertySetGuid, string name)
    {
      this.PropertySetGuid = propertySetGuid;
      this.Id = 0;
      this.Name = name;
      this.Kind = TnefNameIdKind.Name;
    }
  }
}
