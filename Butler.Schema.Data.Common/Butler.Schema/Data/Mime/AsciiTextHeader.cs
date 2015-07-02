// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.AsciiTextHeader
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  public class AsciiTextHeader : Header
  {
    internal const bool AllowUTF8Value = true;

    public override sealed string Value
    {
      get
      {
        return this.GetRawValue(true);
      }
      set
      {
        this.SetRawValue(value, true, true);
      }
    }

    public override sealed bool RequiresSMTPUTF8 => !MimeString.IsPureASCII(this.Lines);

      public AsciiTextHeader(string name, string value)
      : this(name, Header.GetHeaderId(name, true))
    {
      this.Value = value;
    }

    internal AsciiTextHeader(string name, HeaderId headerId)
      : base(name, headerId)
    {
    }

    public override sealed MimeNode Clone()
    {
      AsciiTextHeader asciiTextHeader = new AsciiTextHeader(this.Name, this.HeaderId);
      this.CopyTo((object) asciiTextHeader);
      return (MimeNode) asciiTextHeader;
    }

    public override sealed void CopyTo(object destination)
    {
      if (destination == null)
        throw new ArgumentNullException(nameof(destination));
      if (destination == this)
        return;
      if (!(destination is AsciiTextHeader))
        base.CopyTo(destination);
      else
        base.CopyTo(destination);
    }

    public override sealed bool IsValueValid(string value)
    {
      return true;
    }

    internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild)
    {
      throw new NotSupportedException(CtsResources.Strings.AddingChildrenToAsciiTextHeader);
    }
  }
}
