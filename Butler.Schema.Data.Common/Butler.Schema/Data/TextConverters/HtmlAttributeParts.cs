// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.HtmlAttributeParts
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  internal struct HtmlAttributeParts
  {
    private Internal.Html.HtmlToken.AttrPartMajor major;
    private Internal.Html.HtmlToken.AttrPartMinor minor;

    public bool Begin => Internal.Html.HtmlToken.AttrPartMajor.Begin == (this.major & Internal.Html.HtmlToken.AttrPartMajor.Begin);

      public bool End => Internal.Html.HtmlToken.AttrPartMajor.End == (this.major & Internal.Html.HtmlToken.AttrPartMajor.End);

      public bool Complete => Internal.Html.HtmlToken.AttrPartMajor.Complete == this.major;

      public bool NameBegin => Internal.Html.HtmlToken.AttrPartMinor.BeginName == (this.minor & Internal.Html.HtmlToken.AttrPartMinor.BeginName);

      public bool Name => Internal.Html.HtmlToken.AttrPartMinor.ContinueName == (this.minor & Internal.Html.HtmlToken.AttrPartMinor.ContinueName);

      public bool NameEnd => Internal.Html.HtmlToken.AttrPartMinor.EndName == (this.minor & Internal.Html.HtmlToken.AttrPartMinor.EndName);

      public bool NameComplete => Internal.Html.HtmlToken.AttrPartMinor.CompleteName == (this.minor & Internal.Html.HtmlToken.AttrPartMinor.CompleteName);

      public bool ValueBegin => Internal.Html.HtmlToken.AttrPartMinor.BeginValue == (this.minor & Internal.Html.HtmlToken.AttrPartMinor.BeginValue);

      public bool Value => Internal.Html.HtmlToken.AttrPartMinor.ContinueValue == (this.minor & Internal.Html.HtmlToken.AttrPartMinor.ContinueValue);

      public bool ValueEnd => Internal.Html.HtmlToken.AttrPartMinor.EndValue == (this.minor & Internal.Html.HtmlToken.AttrPartMinor.EndValue);

      public bool ValueComplete => Internal.Html.HtmlToken.AttrPartMinor.CompleteValue == (this.minor & Internal.Html.HtmlToken.AttrPartMinor.CompleteValue);

      internal HtmlAttributeParts(Internal.Html.HtmlToken.AttrPartMajor major, Internal.Html.HtmlToken.AttrPartMinor minor)
    {
      this.minor = minor;
      this.major = major;
    }

    public override string ToString()
    {
      return this.major.ToString() + " /" + this.minor.ToString() + "/";
    }
  }
}
