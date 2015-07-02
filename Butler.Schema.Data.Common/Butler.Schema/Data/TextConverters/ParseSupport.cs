// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.ParseSupport
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  internal static class ParseSupport
  {
    private static readonly char[] latin1MappingInUnicodeControlArea = new char[32]
    {
      '€',
      '\x0081',
      '‚',
      'ƒ',
      '„',
      '…',
      '†',
      '‡',
      '\x02C6',
      '‰',
      'Š',
      '‹',
      'Œ',
      '\x008D',
      'Ž',
      '\x008F',
      '\x0090',
      '‘',
      '’',
      '“',
      '”',
      '•',
      '–',
      '—',
      '˜',
      '™',
      'š',
      '›',
      'œ',
      '\x009D',
      'ž',
      'Ÿ'
    };
    private static readonly DbcsLeadBits[] dbcsLeadTable = new DbcsLeadBits[128]
    {
      (DbcsLeadBits) 0,
      DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead10008 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead1361 | DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead10001 | DbcsLeadBits.Lead10002 | DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead932 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead10003 | DbcsLeadBits.Lead9XX,
      DbcsLeadBits.Lead9XX,
      (DbcsLeadBits) 0
    };
    private static readonly byte[] charToHexTable = new byte[32]
    {
      byte.MaxValue,
      (byte) 10,
      (byte) 11,
      (byte) 12,
      (byte) 13,
      (byte) 14,
      (byte) 15,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      (byte) 0,
      (byte) 1,
      (byte) 2,
      (byte) 3,
      (byte) 4,
      (byte) 5,
      (byte) 6,
      (byte) 7,
      (byte) 8,
      (byte) 9,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue
    };
    private static readonly byte[] octetBitsCount = new byte[16]
    {
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 2,
      (byte) 1,
      (byte) 2,
      (byte) 2,
      (byte) 3,
      (byte) 1,
      (byte) 2,
      (byte) 2,
      (byte) 3,
      (byte) 2,
      (byte) 3,
      (byte) 3,
      (byte) 4
    };
    private static readonly CharClass[] lowCharClass = new CharClass[256]
    {
      CharClass.RtfInteresting,
      CharClass.Control,
      CharClass.Control,
      CharClass.Control,
      CharClass.Control,
      CharClass.Control,
      CharClass.Control,
      CharClass.Control,
      CharClass.Control,
      CharClass.Whitespace | CharClass.RtfInteresting,
      CharClass.Whitespace | CharClass.RtfInteresting,
      CharClass.Whitespace,
      CharClass.Whitespace,
      CharClass.Whitespace | CharClass.RtfInteresting,
      CharClass.Control,
      CharClass.Control,
      CharClass.Control,
      CharClass.Control,
      CharClass.Control,
      CharClass.Control,
      CharClass.Control,
      CharClass.Control,
      CharClass.Control,
      CharClass.Control,
      CharClass.Control,
      CharClass.Control,
      CharClass.Control,
      CharClass.Control,
      CharClass.Control,
      CharClass.Control,
      CharClass.Control,
      CharClass.Control,
      CharClass.Whitespace,
      CharClass.NotInterestingText,
      CharClass.DoubleQuote,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText | CharClass.HtmlSuffix,
      CharClass.Ampersand,
      CharClass.SingleQuote,
      CharClass.Parentheses,
      CharClass.Parentheses,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.Comma,
      CharClass.NotInterestingText | CharClass.HtmlSuffix,
      CharClass.NotInterestingText,
      CharClass.Solidus,
      CharClass.Numeric,
      CharClass.Numeric,
      CharClass.Numeric,
      CharClass.Numeric,
      CharClass.Numeric,
      CharClass.Numeric,
      CharClass.Numeric,
      CharClass.Numeric,
      CharClass.Numeric,
      CharClass.Numeric,
      CharClass.Colon,
      CharClass.NotInterestingText,
      CharClass.LessThan,
      CharClass.Equals,
      CharClass.GreaterThan | CharClass.HtmlSuffix,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.Alpha | CharClass.AlphaHex,
      CharClass.Alpha | CharClass.AlphaHex,
      CharClass.Alpha | CharClass.AlphaHex,
      CharClass.Alpha | CharClass.AlphaHex,
      CharClass.Alpha | CharClass.AlphaHex,
      CharClass.Alpha | CharClass.AlphaHex,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.SquareBrackets,
      CharClass.Backslash | CharClass.RtfInteresting,
      CharClass.SquareBrackets | CharClass.HtmlSuffix,
      CharClass.Circumflex,
      CharClass.NotInterestingText,
      CharClass.GraveAccent,
      CharClass.Alpha | CharClass.AlphaHex,
      CharClass.Alpha | CharClass.AlphaHex,
      CharClass.Alpha | CharClass.AlphaHex,
      CharClass.Alpha | CharClass.AlphaHex,
      CharClass.Alpha | CharClass.AlphaHex,
      CharClass.Alpha | CharClass.AlphaHex,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.Alpha,
      CharClass.CurlyBrackets | CharClass.RtfInteresting,
      CharClass.VerticalLine,
      CharClass.CurlyBrackets | CharClass.RtfInteresting,
      CharClass.Tilde,
      CharClass.Control,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.Nbsp,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText,
      CharClass.NotInterestingText
    };

    public static int CharToDecimal(char ch)
    {
      return (int) ch - 48;
    }

    public static int CharToHex(char ch)
    {
      return (int) ParseSupport.charToHexTable[(int) ch & 31];
    }

    public static int BitCount(byte v)
    {
      return (int) ParseSupport.octetBitsCount[(int) v & 15] + (int) ParseSupport.octetBitsCount[(int) v >> 4 & 15];
    }

    public static int BitCount(short v)
    {
      return ParseSupport.BitCount((byte) v) + ParseSupport.BitCount((byte) ((uint) v >> 8));
    }

    public static int BitCount(int v)
    {
      return ParseSupport.BitCount((short) v) + ParseSupport.BitCount((short) (v >> 16));
    }

    public static char HighSurrogateCharFromUcs4(int ich)
    {
      return (char) (55296 + (ich - 65536 >> 10));
    }

    public static char LowSurrogateCharFromUcs4(int ich)
    {
      return (char) (56320 + (ich & 1023));
    }

    public static int Ucs4FromSurrogatePair(char low, char high)
    {
      return ((int) low & 1023) + (((int) high & 1023) << 10) + 65536;
    }

    public static bool IsHighSurrogate(char ch)
    {
      if ((int) ch >= 55296)
        return (int) ch < 56320;
      return false;
    }

    public static bool IsLowSurrogate(char ch)
    {
      if ((int) ch >= 56320)
        return (int) ch < 56832;
      return false;
    }

    public static bool IsCharClassOneOf(CharClass charClass, CharClass charClassSet)
    {
      return (charClass & charClassSet) != CharClass.Invalid;
    }

    public static bool InvalidUnicodeCharacter(CharClass charClass)
    {
      return (charClass & CharClass.UniqueMask) == CharClass.Invalid;
    }

    public static bool HtmlTextCharacter(CharClass charClass)
    {
      return (charClass & CharClass.HtmlNonWhitespaceText) != CharClass.Invalid;
    }

    public static bool TextCharacter(CharClass charClass)
    {
      return (charClass & CharClass.NonWhitespaceText) != CharClass.Invalid;
    }

    public static bool TextNonUriCharacter(CharClass charClass)
    {
      return (charClass & CharClass.NonWhitespaceNonUri) != CharClass.Invalid;
    }

    public static bool TextUriCharacter(CharClass charClass)
    {
      return (charClass & CharClass.NonWhitespaceUri) != CharClass.Invalid;
    }

    public static bool NonControlTextCharacter(CharClass charClass)
    {
      return (charClass & CharClass.NonWhitespaceNonControlText) != CharClass.Invalid;
    }

    public static bool ControlCharacter(CharClass charClass)
    {
      return (charClass & CharClass.Control) != CharClass.Invalid;
    }

    public static bool WhitespaceCharacter(CharClass charClass)
    {
      return (charClass & CharClass.Whitespace) != CharClass.Invalid;
    }

    public static bool WhitespaceCharacter(char ch)
    {
      return (ParseSupport.GetCharClass(ch) & CharClass.Whitespace) != CharClass.Invalid;
    }

    public static bool NbspCharacter(CharClass charClass)
    {
      return (charClass & CharClass.Nbsp) != CharClass.Invalid;
    }

    public static bool AlphaCharacter(CharClass charClass)
    {
      return (charClass & CharClass.Alpha) != CharClass.Invalid;
    }

    public static bool AlphaCharacter(char ch)
    {
      return (ParseSupport.GetCharClass(ch) & CharClass.Alpha) != CharClass.Invalid;
    }

    public static bool QuoteCharacter(CharClass charClass)
    {
      return (charClass & CharClass.Quote) != CharClass.Invalid;
    }

    public static bool HtmlTagNamePrefixCharacter(CharClass charClass)
    {
      return (charClass & CharClass.HtmlTagNamePrefix) != CharClass.Invalid;
    }

    public static bool HtmlTagNameCharacter(CharClass charClass)
    {
      return (charClass & CharClass.HtmlTagName) != CharClass.Invalid;
    }

    public static bool HtmlAttrNamePrefixCharacter(CharClass charClass)
    {
      return (charClass & CharClass.HtmlAttrNamePrefix) != CharClass.Invalid;
    }

    public static bool HtmlAttrNameCharacter(CharClass charClass)
    {
      return (charClass & CharClass.HtmlAttrName) != CharClass.Invalid;
    }

    public static bool HtmlAttrValueCharacter(CharClass charClass)
    {
      return (charClass & CharClass.HtmlAttrValue) != CharClass.Invalid;
    }

    public static bool HtmlScanQuoteSensitiveCharacter(CharClass charClass)
    {
      return (charClass & CharClass.HtmlScanQuoteSensitive) != CharClass.Invalid;
    }

    public static bool HtmlSimpleTagNameCharacter(CharClass charClass)
    {
      return (charClass & CharClass.HtmlSimpleTagName) != CharClass.Invalid;
    }

    public static bool HtmlEndTagNameCharacter(CharClass charClass)
    {
      return (charClass & CharClass.HtmlEndTagName) != CharClass.Invalid;
    }

    public static bool HtmlSimpleAttrNameCharacter(CharClass charClass)
    {
      return (charClass & CharClass.HtmlSimpleAttrName) != CharClass.Invalid;
    }

    public static bool HtmlEndAttrNameCharacter(CharClass charClass)
    {
      return (charClass & CharClass.HtmlEndAttrName) != CharClass.Invalid;
    }

    public static bool HtmlSimpleAttrQuotedValueCharacter(CharClass charClass)
    {
      return (charClass & CharClass.HtmlSimpleAttrQuotedValue) != CharClass.Invalid;
    }

    public static bool HtmlSimpleAttrUnquotedValueCharacter(CharClass charClass)
    {
      return (charClass & CharClass.HtmlAttrValue) != CharClass.Invalid;
    }

    public static bool HtmlEndAttrUnquotedValueCharacter(CharClass charClass)
    {
      return (charClass & CharClass.HtmlEndAttrUnquotedValue) != CharClass.Invalid;
    }

    public static bool NumericCharacter(CharClass charClass)
    {
      return (charClass & CharClass.Numeric) != CharClass.Invalid;
    }

    public static bool NumericCharacter(char ch)
    {
      return (ParseSupport.GetCharClass(ch) & CharClass.Numeric) != CharClass.Invalid;
    }

    public static bool HexCharacter(CharClass charClass)
    {
      return (charClass & CharClass.Hex) != CharClass.Invalid;
    }

    public static bool HtmlEntityCharacter(CharClass charClass)
    {
      return (charClass & CharClass.HtmlEntity) != CharClass.Invalid;
    }

    public static bool HtmlSuffixCharacter(CharClass charClass)
    {
      return (charClass & CharClass.HtmlSuffix) != CharClass.Invalid;
    }

    public static bool RtfInterestingCharacter(CharClass charClass)
    {
      return (charClass & CharClass.RtfInteresting) != CharClass.Invalid;
    }

    public static CharClass GetCharClass(byte ch)
    {
      return ParseSupport.lowCharClass[(int) ch];
    }

    public static CharClass GetCharClass(char ch)
    {
      if ((int) ch > (int) byte.MaxValue)
        return ParseSupport.GetHighCharClass(ch);
      return ParseSupport.lowCharClass[(int) ch];
    }

    public static CharClass GetHighCharClass(char ch)
    {
      return (int) ch >= 64976 && (65529 > (int) ch || (int) ch > 65533) && (65008 > (int) ch || (int) ch > 65519) ? CharClass.Invalid : CharClass.NotInterestingText;
    }

    public static bool IsLeadByte(byte bt, DbcsLeadBits codePageMask)
    {
      if (codePageMask != (DbcsLeadBits) 0)
        return ParseSupport.IsLeadByteEx(bt, codePageMask);
      return false;
    }

    private static bool IsLeadByteEx(byte bt, DbcsLeadBits codePageMask)
    {
      if ((int) bt >= 128)
        return (ParseSupport.dbcsLeadTable[(int) bt - 128] & codePageMask) != (DbcsLeadBits) 0;
      return false;
    }

    public static DbcsLeadBits GetCodePageLeadMask(int codePage)
    {
      DbcsLeadBits dbcsLeadBits = (DbcsLeadBits) 0;
      if (codePage >= 1361)
      {
        if (codePage == 1361)
          dbcsLeadBits = DbcsLeadBits.Lead1361;
        else if (codePage == 10001)
          dbcsLeadBits = DbcsLeadBits.Lead10001;
        else if (codePage == 10002)
          dbcsLeadBits = DbcsLeadBits.Lead10002;
        else if (codePage == 10003)
          dbcsLeadBits = DbcsLeadBits.Lead10003;
        else if (codePage == 10008)
          dbcsLeadBits = DbcsLeadBits.Lead10008;
      }
      else if (codePage <= 950)
      {
        if (codePage == 950 || codePage == 949 || codePage == 936)
          dbcsLeadBits = DbcsLeadBits.Lead9XX;
        else if (codePage == 932)
          dbcsLeadBits = DbcsLeadBits.Lead932;
      }
      return dbcsLeadBits;
    }

    public static bool IsUpperCase(char ch)
    {
      return (uint) ch - 65U <= 25U;
    }

    public static bool IsLowerCase(char ch)
    {
      return (uint) ch - 97U <= 25U;
    }

    public static char ToLowerCase(char ch)
    {
      if (!ParseSupport.IsUpperCase(ch))
        return ch;
      return (char) ((uint) ch + 32U);
    }

    public static int Latin1MappingInUnicodeControlArea(int value)
    {
      return (int) ParseSupport.latin1MappingInUnicodeControlArea[value - 128];
    }

    public static bool TwoFarEastNonHanguelChars(char ch1, char ch2)
    {
      if ((int) ch1 < 12288 || (int) ch2 < 12288 || ParseSupport.HanguelRange(ch1))
        return false;
      return !ParseSupport.HanguelRange(ch2);
    }

    public static bool FarEastNonHanguelChar(char ch)
    {
      if ((int) ch >= 12288)
        return !ParseSupport.HanguelRange(ch);
      return false;
    }

    private static bool HanguelRange(char ch)
    {
      if (12592 <= (int) ch && (int) ch <= 12687 || 44032 <= (int) ch && (int) ch <= 55203)
        return true;
      if (65441 <= (int) ch)
        return (int) ch <= 65500;
      return false;
    }

    public static string TrimWhitespace(string value)
    {
      string str = value;
      if (!string.IsNullOrEmpty(value))
      {
        int startIndex = 0;
        int length = value.Length;
        if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[0])))
        {
          startIndex = 1;
          while (startIndex < length && ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[startIndex])))
            ++startIndex;
        }
        if (startIndex != length)
        {
          if (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[length - 1])))
          {
            --length;
            while (ParseSupport.WhitespaceCharacter(ParseSupport.GetCharClass(value[length - 1])))
              --length;
          }
          if (length - startIndex != value.Length)
            str = value.Substring(startIndex, length - startIndex);
        }
        else
          str = string.Empty;
      }
      return str;
    }
  }
}
