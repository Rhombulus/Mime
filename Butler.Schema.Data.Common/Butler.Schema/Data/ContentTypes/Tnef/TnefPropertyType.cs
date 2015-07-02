// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.Tnef.TnefPropertyType
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.ContentTypes.Tnef
{
  public enum TnefPropertyType : short
  {
    Unspecified = (short) 0,
    Null = (short) 1,
    I2 = (short) 2,
    Long = (short) 3,
    R4 = (short) 4,
    Double = (short) 5,
    Currency = (short) 6,
    AppTime = (short) 7,
    Error = (short) 10,
    Boolean = (short) 11,
    Object = (short) 13,
    I8 = (short) 20,
    String8 = (short) 30,
    Unicode = (short) 31,
    SysTime = (short) 64,
    ClassId = (short) 72,
    Binary = (short) 258,
    MultiValued = (short) 4096,
  }
}
