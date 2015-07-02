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
    private Guid propertySetGuid;
    private int id;
    private string name;
    private TnefNameIdKind kind;

    public Guid PropertySetGuid
    {
      get
      {
        return this.propertySetGuid;
      }
    }

    public TnefNameIdKind Kind
    {
      get
      {
        return this.kind;
      }
    }

    public string Name
    {
      get
      {
        return this.name;
      }
    }

    public int Id
    {
      get
      {
        return this.id;
      }
    }

    public TnefNameId(Guid propertySetGuid, int id)
    {
      this.propertySetGuid = propertySetGuid;
      this.id = id;
      this.name = (string) null;
      this.kind = TnefNameIdKind.Id;
    }

    public TnefNameId(Guid propertySetGuid, string name)
    {
      this.propertySetGuid = propertySetGuid;
      this.id = 0;
      this.name = name;
      this.kind = TnefNameIdKind.Name;
    }

    internal void Set(Guid propertySetGuid, int id)
    {
      this.propertySetGuid = propertySetGuid;
      this.id = id;
      this.name = (string) null;
      this.kind = TnefNameIdKind.Id;
    }

    internal void Set(Guid propertySetGuid, string name)
    {
      this.propertySetGuid = propertySetGuid;
      this.id = 0;
      this.name = name;
      this.kind = TnefNameIdKind.Name;
    }
  }
}
