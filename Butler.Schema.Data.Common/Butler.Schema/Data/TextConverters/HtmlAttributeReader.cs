// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.HtmlAttributeReader
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  public struct HtmlAttributeReader
  {
    private HtmlReader reader;

    public HtmlAttributeId Id => this.reader.AttributeReader_GetCurrentAttributeId();

      public bool NameIsLong => this.reader.AttributeReader_CurrentAttributeNameIsLong();

      public bool HasValue => this.reader.AttributeReader_CurrentAttributeHasValue();

      public bool ValueIsLong => this.reader.AttributeReader_CurrentAttributeValueIsLong();

      internal HtmlAttributeReader(HtmlReader reader)
    {
      this.reader = reader;
    }

    public bool ReadNext()
    {
      return this.reader.AttributeReader_ReadNextAttribute();
    }

    public string ReadName()
    {
      return this.reader.AttributeReader_ReadCurrentAttributeName();
    }

    public int ReadName(char[] buffer, int offset, int count)
    {
      return this.reader.AttributeReader_ReadCurrentAttributeName(buffer, offset, count);
    }

    internal void WriteNameTo(ITextSink sink)
    {
      this.reader.AttributeReader_WriteCurrentAttributeNameTo(sink);
    }

    public string ReadValue()
    {
      return this.reader.AttributeReader_ReadCurrentAttributeValue();
    }

    public int ReadValue(char[] buffer, int offset, int count)
    {
      return this.reader.AttributeReader_ReadCurrentAttributeValue(buffer, offset, count);
    }

    internal void WriteValueTo(ITextSink sink)
    {
      this.reader.AttributeReader_WriteCurrentAttributeValueTo(sink);
    }
  }
}
