// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.iCalendar.CalendarParameterReader
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.ContentTypes.iCalendar
{
  public struct CalendarParameterReader
  {
    private Internal.ContentLineReader reader;

    public ParameterId ParameterId
    {
      get
      {
        this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter);
        return CalendarCommon.GetParameterEnum(this.reader.ParameterName);
      }
    }

    public string Name
    {
      get
      {
        this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter);
        return this.reader.ParameterName;
      }
    }

    internal CalendarParameterReader(Internal.ContentLineReader reader)
    {
      this.reader = reader;
    }

    public string ReadValue()
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter);
      return this.reader.ReadParameterValue(true);
    }

    public bool ReadNextValue()
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.DocumentEnd);
      return this.reader.ReadNextParameterValue();
    }

    public bool ReadNextParameter()
    {
      this.reader.AssertValidState(Internal.ContentLineNodeType.Parameter | Internal.ContentLineNodeType.Property | Internal.ContentLineNodeType.DocumentEnd);
      return this.reader.ReadNextParameter();
    }
  }
}
