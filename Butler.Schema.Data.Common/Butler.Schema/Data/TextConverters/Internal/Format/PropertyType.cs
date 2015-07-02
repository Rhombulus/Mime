// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Format.PropertyType
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Format
{
  internal enum PropertyType : byte
  {
    Null = (byte) 0,
    Calculated = (byte) 1,
    Bool = (byte) 2,
    String = (byte) 3,
    MultiValue = (byte) 4,
    Enum = (byte) 5,
    Color = (byte) 6,
    Integer = (byte) 7,
    Fractional = (byte) 8,
    Percentage = (byte) 9,
    AbsLength = (byte) 10,
    FirstLength = (byte) 10,
    RelLength = (byte) 11,
    Pixels = (byte) 12,
    Ems = (byte) 13,
    Exs = (byte) 14,
    HtmlFontUnits = (byte) 15,
    RelHtmlFontUnits = (byte) 16,
    LastLength = (byte) 17,
    Multiple = (byte) 17,
    Milliseconds = (byte) 18,
    kHz = (byte) 19,
    Degrees = (byte) 20,
  }
}
