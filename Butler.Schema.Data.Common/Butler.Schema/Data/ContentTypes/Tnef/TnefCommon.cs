// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.Tnef.TnefCommon
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.ContentTypes.Tnef
{
  internal static class TnefCommon
  {
    public static readonly byte[] HexDigit = new byte[16]
    {
      (byte) 48,
      (byte) 49,
      (byte) 50,
      (byte) 51,
      (byte) 52,
      (byte) 53,
      (byte) 54,
      (byte) 55,
      (byte) 56,
      (byte) 57,
      (byte) 65,
      (byte) 66,
      (byte) 67,
      (byte) 68,
      (byte) 69,
      (byte) 70
    };
    public static readonly byte[] Padding = new byte[4];
    public static readonly byte[] OidOle1Storage = new byte[11]
    {
      (byte) 42,
      (byte) 134,
      (byte) 72,
      (byte) 134,
      (byte) 247,
      (byte) 20,
      (byte) 3,
      (byte) 10,
      (byte) 3,
      (byte) 1,
      (byte) 1
    };
    public static readonly byte[] OidMacBinary = new byte[9]
    {
      (byte) 42,
      (byte) 134,
      (byte) 72,
      (byte) 134,
      (byte) 247,
      (byte) 20,
      (byte) 3,
      (byte) 11,
      (byte) 1
    };
    public static readonly byte[] MuidOOP = new byte[16]
    {
      (byte) 129,
      (byte) 43,
      (byte) 31,
      (byte) 164,
      (byte) 190,
      (byte) 163,
      (byte) 16,
      (byte) 25,
      (byte) 157,
      (byte) 110,
      (byte) 0,
      (byte) 221,
      (byte) 1,
      (byte) 15,
      (byte) 84,
      (byte) 2
    };
    public const int TnefSignature = 574529400;
    public const int MaxTnefVersion = 65536;
    public const int AttributeHeaderLength = 9;
    public const int CheckSumLength = 2;
    public const int StringNameKind = 1;
    public const int MaxNestingDepth = 100;
    private static readonly byte[] oleGuid;
    public static readonly Guid MessageIID;
    public static readonly string MessageClassLegacyPrefix;
    public static readonly TnefCommon.MessageClassMapping[] MessageClassMappingTable;

    static TnefCommon()
    {
      byte[] numArray = new byte[8];
      numArray[0] = (byte) 192;
      numArray[7] = (byte) 70;
      TnefCommon.oleGuid = numArray;
      TnefCommon.MessageIID = new Guid(131847, (short) 0, (short) 0, TnefCommon.oleGuid);
      TnefCommon.MessageClassLegacyPrefix = "Microsoft Mail v3.0 ";
      TnefCommon.MessageClassMappingTable = new TnefCommon.MessageClassMapping[9]
      {
        new TnefCommon.MessageClassMapping("IPM.Microsoft Mail.Note", "IPM.Note", false, false),
        new TnefCommon.MessageClassMapping("IPM.Microsoft Mail.Note", "IPM", false, false),
        new TnefCommon.MessageClassMapping("IPM.Microsoft Mail.Read Receipt", "Report.IPM.Note.IPNRN", false, false),
        new TnefCommon.MessageClassMapping("IPM.Microsoft Mail.Non-Delivery", "Report.IPM.Note.NDR", false, false),
        new TnefCommon.MessageClassMapping("IPM.Microsoft Schedule.MtgRespP", "IPM.Schedule.Meeting.Resp.Pos", true, true),
        new TnefCommon.MessageClassMapping("IPM.Microsoft Schedule.MtgRespN", "IPM.Schedule.Meeting.Resp.Neg", true, true),
        new TnefCommon.MessageClassMapping("IPM.Microsoft Schedule.MtgRespA", "IPM.Schedule.Meeting.Resp.Tent", true, true),
        new TnefCommon.MessageClassMapping("IPM.Microsoft Schedule.MtgReq", "IPM.Schedule.Meeting.Request", true, false),
        new TnefCommon.MessageClassMapping("IPM.Microsoft Schedule.MtgCncl", "IPM.Schedule.Meeting.Canceled", true, false)
      };
    }

    public static bool BytesEqualToPattern(byte[] buffer, int offset, string pattern)
    {
      int length = pattern.Length;
      for (int index = 0; index < pattern.Length; ++index)
      {
        if ((int) buffer[offset + index] != (int) pattern[index])
          return false;
      }
      return true;
    }

    public static bool BytesEqualToPattern(byte[] buffer, int offset, byte[] pattern)
    {
      for (int index = 0; index < pattern.Length; ++index)
      {
        if ((int) buffer[offset + index] != (int) pattern[index])
          return false;
      }
      return true;
    }

    public static int StrZLength(byte[] buffer, int offset, int maxEndOffset)
    {
      int index = offset;
      while (index < maxEndOffset && (int) buffer[index] != 0)
        ++index;
      return index - offset;
    }

    public static bool IsUnicodeCodepage(int messageCodePage)
    {
      if (messageCodePage != 1200 && messageCodePage != 1201 && (messageCodePage != 12000 && messageCodePage != 12001) && messageCodePage != 65005)
        return messageCodePage == 65006;
      return true;
    }

    public struct MessageClassMapping
    {
      public string LegacyName;
      public string MapiName;
      public bool Splus;
      public bool SplusResponse;

      public MessageClassMapping(string legacyName, string mapiName, bool splus, bool splusResponse)
      {
        this.LegacyName = legacyName;
        this.MapiName = mapiName;
        this.Splus = splus;
        this.SplusResponse = splusResponse;
      }
    }
  }
}
