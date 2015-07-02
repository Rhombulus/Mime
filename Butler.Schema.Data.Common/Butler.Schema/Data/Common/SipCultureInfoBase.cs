// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Common.SipCultureInfoBase
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Globalization;
using System.Linq;

namespace Butler.Schema.Data.Common
{
  internal class SipCultureInfoBase : CultureInfo
  {
    protected string description;
    protected string name;
    protected string sipName;
    protected bool useSipName;
    protected string segmentID;
    protected CultureInfo parent;

    public override string Name
    {
      get
      {
        if (!this.useSipName)
          return this.name;
        return this.sipName;
      }
    }

    public override CultureInfo Parent => this.parent;

      public override string EnglishName => this.description;

      internal virtual bool UseSipName
    {
      get
      {
        return this.useSipName;
      }
      set
      {
        this.useSipName = value;
      }
    }

    internal virtual string SipName => this.sipName;

      internal virtual string SipSegmentID => this.segmentID;

      internal SipCultureInfoBase(CultureInfo parent, string segmentID)
      : base(parent.Name)
    {
      this.parent = parent;
      this.segmentID = segmentID;
      this.name = parent.Name;
      this.sipName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}-x-{1}", new object[2]
      {
        parent.IsNeutralCulture ? (object) parent.Name : (object) parent.Parent.Name,
        (object) segmentID
      });
      this.description = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Role-Based Culture ({0})", new object[1]
      {
        (object) this.name
      });
    }
  }
}
