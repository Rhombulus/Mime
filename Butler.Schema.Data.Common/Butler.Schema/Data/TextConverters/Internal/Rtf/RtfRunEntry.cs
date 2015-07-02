// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Rtf.RtfRunEntry
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Rtf
{
  internal struct RtfRunEntry
  {
    private const ushort RunKindMask = (ushort) 61440;
    private const ushort SkipBit = (ushort) 1024;
    private const ushort LeadBit = (ushort) 512;
    private const ushort KeywordIdMask = (ushort) 511;
    private ushort bitFields;
    private ushort length;
    private int value;

    public RtfRunKind Kind
    {
      get
      {
        return (RtfRunKind) ((uint) this.bitFields & 61440U);
      }
    }

    public short KeywordId
    {
      get
      {
        return (short) ((int) this.bitFields & 511);
      }
    }

    public bool Skip
    {
      get
      {
        return 0 != ((int) this.bitFields & 1024);
      }
    }

    public bool Lead
    {
      get
      {
        return 0 != ((int) this.bitFields & 512);
      }
    }

    public ushort Length
    {
      get
      {
        return this.length;
      }
    }

    public int Value
    {
      get
      {
        return this.value;
      }
    }

    public bool IsSkiped
    {
      get
      {
        if (this.Kind != RtfRunKind.Ignore)
          return this.Skip;
        return true;
      }
    }

    public bool IsSmall
    {
      get
      {
        switch (this.Kind)
        {
          case RtfRunKind.Escape:
          case RtfRunKind.Zero:
            return true;
          case RtfRunKind.Text:
            return (int) this.length == 1;
          default:
            return false;
        }
      }
    }

    public bool IsUnicode
    {
      get
      {
        return this.Kind == RtfRunKind.Unicode;
      }
    }

    internal void Reset()
    {
      this.bitFields = (ushort) 0;
      this.length = (ushort) 0;
      this.value = 0;
    }

    internal void Initialize(RtfRunKind kind, int length, int value)
    {
      this.bitFields = (ushort) kind;
      this.length = (ushort) length;
      this.value = value;
    }

    internal void Initialize(RtfRunKind kind, int length, int unescaped, bool skip, bool lead)
    {
      ushort num = (ushort) kind;
      if (skip)
        num |= (ushort) 1024;
      if (lead)
        num |= (ushort) 512;
      this.bitFields = num;
      this.length = (ushort) length;
      this.value = unescaped;
    }

    internal void InitializeKeyword(short keywordId, int value, int length, bool skip, bool firstKeyword)
    {
      ushort num = (ushort) (20480U | (uint) (ushort) keywordId);
      if (skip)
        num |= (ushort) 1024;
      if (firstKeyword)
        num |= (ushort) 512;
      this.bitFields = num;
      this.length = (ushort) length;
      this.value = value;
    }
  }
}
