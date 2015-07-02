// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.DecodingResults
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  public struct DecodingResults
  {
    private string charsetName;
    private string cultureName;
    private EncodingScheme encodingScheme;
    private bool decodingFailed;

    public string CharsetName
    {
      get
      {
        return this.charsetName;
      }
      internal set
      {
        this.charsetName = value;
      }
    }

    public string CultureName
    {
      get
      {
        return this.cultureName;
      }
      internal set
      {
        this.cultureName = value;
      }
    }

    public EncodingScheme EncodingScheme
    {
      get
      {
        return this.encodingScheme;
      }
      internal set
      {
        this.encodingScheme = value;
      }
    }

    public bool DecodingFailed
    {
      get
      {
        return this.decodingFailed;
      }
      internal set
      {
        this.decodingFailed = value;
      }
    }
  }
}
