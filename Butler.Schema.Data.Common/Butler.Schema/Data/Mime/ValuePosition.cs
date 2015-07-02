// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.ValuePosition
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  internal struct ValuePosition
  {
    public int Line;
    public int Offset;

    public ValuePosition(int line, int offset)
    {
      this.Line = line;
      this.Offset = offset;
    }

    public static bool operator ==(ValuePosition pos1, ValuePosition pos2)
    {
      if (pos1.Line == pos2.Line)
        return pos1.Offset == pos2.Offset;
      return false;
    }

    public static bool operator !=(ValuePosition pos1, ValuePosition pos2)
    {
      return !(pos1 == pos2);
    }

    public override bool Equals(object rhs)
    {
      if (!(rhs is ValuePosition))
        return false;
      ValuePosition valuePosition = (ValuePosition) rhs;
      if (this.Line == valuePosition.Line)
        return this.Offset == valuePosition.Offset;
      return false;
    }

    public override int GetHashCode()
    {
      return this.Line * 1000 + this.Offset;
    }
  }
}
