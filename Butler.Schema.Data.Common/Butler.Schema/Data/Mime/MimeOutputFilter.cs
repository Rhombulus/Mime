// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.MimeOutputFilter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  public abstract class MimeOutputFilter
  {
    public virtual bool FilterPart(MimePart part, Stream stream)
    {
      return false;
    }

    public virtual bool FilterHeaderList(HeaderList headerList, Stream stream)
    {
      return false;
    }

    public virtual bool FilterHeader(Header header, Stream stream)
    {
      return false;
    }

    public virtual bool FilterPartBody(MimePart part, Stream stream)
    {
      return false;
    }

    public virtual void ClosePart(MimePart part, Stream stream)
    {
    }
  }
}
