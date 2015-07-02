// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.HtmlTagParts
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  internal struct HtmlTagParts
  {
    private Internal.Html.HtmlToken.TagPartMajor major;
    private Internal.Html.HtmlToken.TagPartMinor minor;

    public bool Begin
    {
      get
      {
        return Internal.Html.HtmlToken.TagPartMajor.Begin == (this.major & Internal.Html.HtmlToken.TagPartMajor.Begin);
      }
    }

    public bool End
    {
      get
      {
        return Internal.Html.HtmlToken.TagPartMajor.End == (this.major & Internal.Html.HtmlToken.TagPartMajor.End);
      }
    }

    public bool Complete
    {
      get
      {
        return Internal.Html.HtmlToken.TagPartMajor.Complete == this.major;
      }
    }

    public bool NameBegin
    {
      get
      {
        return Internal.Html.HtmlToken.TagPartMinor.BeginName == (this.minor & Internal.Html.HtmlToken.TagPartMinor.BeginName);
      }
    }

    public bool Name
    {
      get
      {
        return Internal.Html.HtmlToken.TagPartMinor.ContinueName == (this.minor & Internal.Html.HtmlToken.TagPartMinor.ContinueName);
      }
    }

    public bool NameEnd
    {
      get
      {
        return Internal.Html.HtmlToken.TagPartMinor.EndName == (this.minor & Internal.Html.HtmlToken.TagPartMinor.EndName);
      }
    }

    public bool NameComplete
    {
      get
      {
        return Internal.Html.HtmlToken.TagPartMinor.CompleteName == (this.minor & Internal.Html.HtmlToken.TagPartMinor.CompleteName);
      }
    }

    public bool Attributes
    {
      get
      {
        return Internal.Html.HtmlToken.TagPartMinor.Empty != (this.minor & (Internal.Html.HtmlToken.TagPartMinor) 144);
      }
    }

    internal HtmlTagParts(Internal.Html.HtmlToken.TagPartMajor major, Internal.Html.HtmlToken.TagPartMinor minor)
    {
      this.minor = minor;
      this.major = major;
    }

    public override string ToString()
    {
      return this.major.ToString() + " /" + this.minor.ToString() + "/";
    }

    internal void Reset()
    {
      this.minor = Internal.Html.HtmlToken.TagPartMinor.Empty;
      this.major = Internal.Html.HtmlToken.TagPartMajor.None;
    }
  }
}
