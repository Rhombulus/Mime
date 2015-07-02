// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Internal.CtsConfigurationArgument
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Internal
{
  internal class CtsConfigurationArgument
  {

      public string Name { get; }

      public string Value { get; }

      internal CtsConfigurationArgument(string name, string value)
    {
      this.Name = name;
      this.Value = value;
    }
  }
}
