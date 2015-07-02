// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Common.LocalizedDisplayNameAttribute
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.ComponentModel;
using System.Linq;
using System.Security.Permissions;

namespace Butler.Schema.Data.Common
{
  [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
  [Serializable]
  public class LocalizedDisplayNameAttribute : DisplayNameAttribute, ILocalizedString
  {
    private LocalizedString displayname;

    public override sealed string DisplayName
    {
      [HostProtection(SecurityAction.LinkDemand, SharedState = true)] get
      {
        return (string) this.displayname;
      }
    }

    public LocalizedString LocalizedString
    {
      get
      {
        return this.displayname;
      }
    }

    public LocalizedDisplayNameAttribute()
    {
    }

    public LocalizedDisplayNameAttribute(LocalizedString displayname)
    {
      this.displayname = displayname;
    }
  }
}
