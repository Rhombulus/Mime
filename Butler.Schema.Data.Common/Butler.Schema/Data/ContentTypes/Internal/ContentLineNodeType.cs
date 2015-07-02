// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.Internal.ContentLineNodeType
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.ContentTypes.Internal
{
  [Flags]
  internal enum ContentLineNodeType
  {
    DocumentStart = 0,
    ComponentStart = 1,
    ComponentEnd = 2,
    Parameter = 4,
    Property = 8,
    BeforeComponentStart = 16,
    BeforeComponentEnd = 32,
    DocumentEnd = 64,
  }
}
