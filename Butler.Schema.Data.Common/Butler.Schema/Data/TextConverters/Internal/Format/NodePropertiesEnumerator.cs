// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Format.NodePropertiesEnumerator
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Format
{
  internal struct NodePropertiesEnumerator : IEnumerable<Property>, IEnumerable, IEnumerator<Property>, IDisposable, IEnumerator
  {
    internal FlagProperties FlagProperties;
    internal Property[] Properties;
    internal int CurrentPropertyIndex;
    internal Property CurrentProperty;

    public Property Current
    {
      get
      {
        return this.CurrentProperty;
      }
    }

    object IEnumerator.Current
    {
      get
      {
        return (object) this.CurrentProperty;
      }
    }

    public NodePropertiesEnumerator(FormatNode node)
    {
      this.FlagProperties = node.FlagProperties;
      this.Properties = node.Properties;
      this.CurrentPropertyIndex = 0;
      this.CurrentProperty = Property.Null;
    }

    public IEnumerator<Property> GetEnumerator()
    {
      return (IEnumerator<Property>) this;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this;
    }

    public bool MoveNext()
    {
      while (this.CurrentPropertyIndex < 16)
      {
        ++this.CurrentPropertyIndex;
        if (this.FlagProperties.IsDefined((PropertyId) this.CurrentPropertyIndex))
        {
          this.CurrentProperty.Set((PropertyId) this.CurrentPropertyIndex, new PropertyValue(this.FlagProperties.IsOn((PropertyId) this.CurrentPropertyIndex)));
          return true;
        }
      }
      if (this.Properties != null && this.CurrentPropertyIndex < this.Properties.Length + 17)
      {
        ++this.CurrentPropertyIndex;
        if (this.CurrentPropertyIndex < this.Properties.Length + 17)
        {
          this.CurrentProperty = this.Properties[this.CurrentPropertyIndex - 17];
          return true;
        }
      }
      this.CurrentProperty = Property.Null;
      return false;
    }

    public void Reset()
    {
      this.CurrentPropertyIndex = 0;
      this.CurrentProperty = Property.Null;
    }

    public void Dispose()
    {
    }
  }
}
