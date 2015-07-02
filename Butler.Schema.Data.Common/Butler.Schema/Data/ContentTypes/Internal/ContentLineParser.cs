// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.Internal.ContentLineParser
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.ContentTypes.Internal
{
  internal class ContentLineParser : IDisposable
  {
    internal static readonly ContentLineParser.Tokens[] Dictionary = new ContentLineParser.Tokens[256]
    {
      ContentLineParser.Tokens.CTL,
      ContentLineParser.Tokens.CTL,
      ContentLineParser.Tokens.CTL,
      ContentLineParser.Tokens.CTL,
      ContentLineParser.Tokens.CTL,
      ContentLineParser.Tokens.CTL,
      ContentLineParser.Tokens.CTL,
      ContentLineParser.Tokens.CTL,
      ContentLineParser.Tokens.CTL,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP,
      ContentLineParser.Tokens.CTL,
      ContentLineParser.Tokens.CTL,
      ContentLineParser.Tokens.CTL,
      ContentLineParser.Tokens.CTL,
      ContentLineParser.Tokens.CTL,
      ContentLineParser.Tokens.CTL,
      ~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
      ~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
      ~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
      ~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
      ~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
      ~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
      ~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
      ~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
      ~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
      ~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
      ~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
      ~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
      ~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
      ~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
      ~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
      ~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII),
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar,
      ContentLineParser.Tokens.CTL,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII,
      ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.NonASCII
    };
    private bool lastCharProcessed = true;
    private const char DQuote = '"';
    private const char CR = '\r';
    private const char LF = '\n';
    private const char SemiColon = ';';
    private const char Colon = ':';
    private const char Comma = ',';
    private const char Dash = '-';
    private const char BackSlash = '\\';
    private ContentLineParser.States state;
    private DirectoryReader reader;
    private char lastChar;
    private bool eof;
    private bool isEndOfLine;
    private bool escaped;
    private bool emitLF;
    private ComplianceTracker complianceTracker;
    private bool isDisposed;
    private Encoding currentValueCharsetOverride;
    private Mime.Encoders.ByteEncoder currentValueEncodingOverride;

    public ContentLineParser.States State
    {
      get
      {
        this.CheckDisposed("State::get");
        return this.state;
      }
    }

    public Encoding CurrentCharsetEncoding
    {
      get
      {
        this.CheckDisposed("CurrentEncoding::get");
        return this.reader.CurrentCharsetEncoding;
      }
    }

    public ContentLineParser(Stream stream, Encoding encoding, ComplianceTracker complianceTracker)
    {
      this.reader = new DirectoryReader(stream, encoding, complianceTracker);
      this.complianceTracker = complianceTracker;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public bool ParseElement(char[] buffer, int offset, int size, out int filled, bool parseAsText, ContentLineParser.Separators separators)
    {
      this.CheckDisposed("ParseElement");
      bool flag1 = false;
      bool flag2 = false;
      int num1 = offset;
      int num2 = offset + size;
      filled = 0;
label_102:
      while (!flag1)
      {
        if (!this.eof && this.state != ContentLineParser.States.ValueStart && (this.state != ContentLineParser.States.ValueStartComma && this.state != ContentLineParser.States.ValueStartSemiColon) && (this.state != ContentLineParser.States.Value && this.state != ContentLineParser.States.ValueEnd))
          this.GetCurrentChar();
        if (this.eof)
        {
          if (this.state != ContentLineParser.States.ValueStartComma && this.state != ContentLineParser.States.ValueStartSemiColon && (this.state != ContentLineParser.States.ValueStart && this.state != ContentLineParser.States.Value) && (this.state != ContentLineParser.States.ValueEnd && this.state != ContentLineParser.States.End && this.state != ContentLineParser.States.PropName))
            this.complianceTracker.SetComplianceStatus(ComplianceStatus.StreamTruncated, CtsResources.CalendarStrings.UnexpectedEndOfStream);
          this.state = ContentLineParser.States.End;
          flag2 = true;
          break;
        }
        if (this.state != ContentLineParser.States.ValueStart && this.state != ContentLineParser.States.ValueStartComma && (this.state != ContentLineParser.States.ValueStartSemiColon && this.state != ContentLineParser.States.Value) && this.state != ContentLineParser.States.ValueEnd)
          this.lastCharProcessed = false;
        switch (this.state)
        {
          case ContentLineParser.States.PropName:
            this.currentValueCharsetOverride = (Encoding) null;
            this.currentValueEncodingOverride = (Mime.Encoders.ByteEncoder) null;
            do
            {
              char currentChar = this.GetCurrentChar();
              if (this.eof)
              {
                this.complianceTracker.SetComplianceStatus(ComplianceStatus.StreamTruncated | ComplianceStatus.PropertyTruncated, CtsResources.CalendarStrings.PropertyTruncated);
                goto label_102;
              }
              else if (this.isEndOfLine || (int) currentChar == 13)
              {
                this.complianceTracker.SetComplianceStatus(ComplianceStatus.PropertyTruncated, CtsResources.CalendarStrings.PropertyTruncated);
                this.lastCharProcessed = false;
                this.state = ContentLineParser.States.Value;
                flag2 = true;
                flag1 = true;
                goto label_102;
              }
              else if ((int) currentChar == 58)
              {
                this.state = ContentLineParser.States.ValueStart;
                this.escaped = false;
                flag1 = true;
                flag2 = true;
                goto label_102;
              }
              else if ((int) currentChar == 59)
              {
                this.state = ContentLineParser.States.ParamName;
                flag1 = true;
                flag2 = true;
                goto label_102;
              }
              else
              {
                if ((ContentLineParser.GetToken((int) currentChar) & ContentLineParser.Tokens.Alpha) == ~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII) && (ContentLineParser.GetToken((int) currentChar) & ContentLineParser.Tokens.Digit) == ~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII) && (int) currentChar != 45)
                  this.complianceTracker.SetComplianceStatus(ComplianceStatus.InvalidCharacterInPropertyName, CtsResources.CalendarStrings.InvalidCharacterInPropertyName);
                buffer[offset++] = currentChar;
              }
            }
            while (offset != num2);
            flag1 = true;
            continue;
          case ContentLineParser.States.ParamName:
            do
            {
              char currentChar = this.GetCurrentChar();
              if (!this.eof)
              {
                if (this.isEndOfLine || (int) currentChar == 13)
                {
                  this.complianceTracker.SetComplianceStatus(ComplianceStatus.PropertyTruncated, CtsResources.CalendarStrings.PropertyTruncated);
                  this.lastCharProcessed = false;
                  this.state = ContentLineParser.States.Value;
                  flag2 = true;
                  flag1 = true;
                  goto label_102;
                }
                else if ((int) currentChar == 61)
                {
                  this.state = ContentLineParser.States.ParamValueStart;
                  flag2 = true;
                  flag1 = true;
                  goto label_102;
                }
                else if (this.complianceTracker.Format == FormatType.VCard && ((int) currentChar == 59 || (int) currentChar == 58))
                {
                  this.complianceTracker.SetComplianceStatus(ComplianceStatus.ParameterNameMissing, CtsResources.CalendarStrings.ParameterNameMissing);
                  this.state = ContentLineParser.States.UnnamedParamEnd;
                  this.lastCharProcessed = false;
                  flag1 = true;
                  flag2 = true;
                  goto label_102;
                }
                else
                {
                  if ((ContentLineParser.GetToken((int) currentChar) & ContentLineParser.Tokens.Alpha) == ~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII) && (ContentLineParser.GetToken((int) currentChar) & ContentLineParser.Tokens.Digit) == ~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII) && (int) currentChar != 45)
                    this.complianceTracker.SetComplianceStatus(ComplianceStatus.InvalidCharacterInParameterName, CtsResources.CalendarStrings.InvalidCharacterInParameterName);
                  buffer[offset++] = currentChar;
                }
              }
              else
                goto label_102;
            }
            while (offset != num2);
            flag1 = true;
            continue;
          case ContentLineParser.States.UnnamedParamEnd:
            char currentChar1 = this.GetCurrentChar();
            flag1 = true;
            flag2 = true;
            if ((int) currentChar1 == 58)
            {
              this.state = ContentLineParser.States.ValueStart;
              continue;
            }
            if ((int) currentChar1 == 59)
            {
              this.state = ContentLineParser.States.ParamName;
              continue;
            }
            continue;
          case ContentLineParser.States.ParamValueStart:
            char currentChar2 = this.GetCurrentChar();
            this.lastCharProcessed = false;
            if (this.isEndOfLine || (int) currentChar2 == 13)
            {
              this.complianceTracker.SetComplianceStatus(ComplianceStatus.PropertyTruncated, CtsResources.CalendarStrings.PropertyTruncated);
              this.state = ContentLineParser.States.ParamValueUnquoted;
              flag2 = true;
              flag1 = true;
              continue;
            }
            if ((int) currentChar2 == 34)
            {
              this.lastCharProcessed = true;
              this.state = ContentLineParser.States.ParamValueQuoted;
            }
            else
              this.state = ContentLineParser.States.ParamValueUnquoted;
            flag1 = true;
            flag2 = true;
            continue;
          case ContentLineParser.States.ParamValueUnquoted:
            do
            {
              char currentChar3 = this.GetCurrentChar();
              if (!this.eof)
              {
                if (this.isEndOfLine || (int) currentChar3 == 13)
                {
                  this.complianceTracker.SetComplianceStatus(ComplianceStatus.PropertyTruncated, CtsResources.CalendarStrings.PropertyTruncated);
                  this.lastCharProcessed = false;
                  this.state = ContentLineParser.States.Value;
                  flag2 = true;
                  flag1 = true;
                  goto label_102;
                }
                else if ((int) currentChar3 == 58)
                {
                  this.state = ContentLineParser.States.ValueStart;
                  this.escaped = false;
                  flag1 = true;
                  flag2 = true;
                  goto label_102;
                }
                else if ((int) currentChar3 == 59)
                {
                  this.state = ContentLineParser.States.ParamName;
                  flag1 = true;
                  flag2 = true;
                  goto label_102;
                }
                else if ((separators & ContentLineParser.Separators.Comma) != ContentLineParser.Separators.None && (int) currentChar3 == 44)
                {
                  this.state = ContentLineParser.States.ParamValueStart;
                  flag1 = true;
                  flag2 = true;
                  goto label_102;
                }
                else
                {
                  if ((ContentLineParser.GetToken((int) currentChar3) & ContentLineParser.Tokens.SafeChar) == ~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII))
                    this.complianceTracker.SetComplianceStatus(ComplianceStatus.InvalidCharacterInParameterText, CtsResources.CalendarStrings.InvalidCharacterInParameterText);
                  buffer[offset++] = currentChar3;
                }
              }
              else
                goto label_102;
            }
            while (offset != num2);
            flag1 = true;
            continue;
          case ContentLineParser.States.ParamValueQuoted:
            do
            {
              char currentChar3 = this.GetCurrentChar();
              if (!this.eof)
              {
                if (this.isEndOfLine || (int) currentChar3 == 13)
                {
                  this.complianceTracker.SetComplianceStatus(ComplianceStatus.PropertyTruncated, CtsResources.CalendarStrings.PropertyTruncated);
                  this.lastCharProcessed = false;
                  this.state = ContentLineParser.States.ParamValueQuotedEnd;
                  goto label_102;
                }
                else if ((int) currentChar3 == 34)
                {
                  this.state = ContentLineParser.States.ParamValueQuotedEnd;
                  goto label_102;
                }
                else
                {
                  if ((ContentLineParser.GetToken((int) currentChar3) & ContentLineParser.Tokens.QSafeChar) == ~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII))
                    this.complianceTracker.SetComplianceStatus(ComplianceStatus.InvalidCharacterInQuotedString, CtsResources.CalendarStrings.InvalidCharacterInQuotedString);
                  buffer[offset++] = currentChar3;
                }
              }
              else
                goto label_102;
            }
            while (offset != num2);
            flag1 = true;
            continue;
          case ContentLineParser.States.ParamValueQuotedEnd:
            char currentChar4 = this.GetCurrentChar();
            if (this.isEndOfLine || (int) currentChar4 == 13)
            {
              this.complianceTracker.SetComplianceStatus(ComplianceStatus.PropertyTruncated, CtsResources.CalendarStrings.PropertyTruncated);
              this.lastCharProcessed = false;
              this.state = ContentLineParser.States.Value;
              flag2 = true;
              flag1 = true;
              continue;
            }
            if ((int) currentChar4 == 59)
            {
              this.state = ContentLineParser.States.ParamName;
              flag1 = true;
              flag2 = true;
              continue;
            }
            if ((int) currentChar4 == 58)
            {
              this.state = ContentLineParser.States.ValueStart;
              this.escaped = false;
              flag1 = true;
              flag2 = true;
              continue;
            }
            if ((separators & ContentLineParser.Separators.Comma) != ContentLineParser.Separators.None && (int) currentChar4 == 44)
            {
              this.state = ContentLineParser.States.ParamValueStart;
              flag1 = true;
              flag2 = true;
              continue;
            }
            this.complianceTracker.SetComplianceStatus(ComplianceStatus.InvalidParameterValue, CtsResources.CalendarStrings.InvalidParameterValue);
            this.state = ContentLineParser.States.ParamValueUnquoted;
            continue;
          case ContentLineParser.States.ValueStartComma:
          case ContentLineParser.States.ValueStartSemiColon:
            this.state = ContentLineParser.States.Value;
            int num3 = (int) this.GetCurrentChar();
            flag1 = true;
            flag2 = true;
            continue;
          case ContentLineParser.States.ValueStart:
            this.state = ContentLineParser.States.Value;
            flag1 = true;
            flag2 = true;
            continue;
          case ContentLineParser.States.Value:
            if (this.currentValueCharsetOverride != null)
            {
              this.reader.SwitchCharsetEncoding(this.currentValueCharsetOverride);
              this.currentValueCharsetOverride = (Encoding) null;
            }
            if (this.currentValueEncodingOverride != null)
            {
              this.reader.ApplyValueDecoder(this.currentValueEncodingOverride);
              this.currentValueEncodingOverride = (Mime.Encoders.ByteEncoder) null;
            }
            do
            {
              if (this.emitLF)
              {
                this.emitLF = false;
                buffer[offset++] = '\n';
                if (offset == num2)
                {
                  flag1 = true;
                  goto label_102;
                }
              }
              char ch = this.GetCurrentChar();
              if (this.isEndOfLine || this.eof)
              {
                this.state = ContentLineParser.States.ValueEnd;
                goto label_102;
              }
              else if (parseAsText && (int) ch == 92 && !this.escaped)
                this.escaped = true;
              else if ((separators & ContentLineParser.Separators.Comma) != ContentLineParser.Separators.None && (int) ch == 44 && !this.escaped)
              {
                this.state = ContentLineParser.States.ValueStartComma;
                this.lastCharProcessed = false;
                flag1 = true;
                flag2 = true;
                goto label_102;
              }
              else if ((separators & ContentLineParser.Separators.SemiColon) != ContentLineParser.Separators.None && (int) ch == 59 && !this.escaped)
              {
                this.state = ContentLineParser.States.ValueStartSemiColon;
                this.lastCharProcessed = false;
                flag1 = true;
                flag2 = true;
                goto label_102;
              }
              else
              {
                if ((ContentLineParser.GetToken((int) ch) & ContentLineParser.Tokens.ValueChar) == ~(ContentLineParser.Tokens.CTL | ContentLineParser.Tokens.Alpha | ContentLineParser.Tokens.Digit | ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar | ContentLineParser.Tokens.WSP | ContentLineParser.Tokens.NonASCII))
                  this.complianceTracker.SetComplianceStatus(ComplianceStatus.InvalidCharacterInPropertyValue, CtsResources.CalendarStrings.InvalidCharacterInPropertyValue);
                if (this.escaped)
                {
                  if (110 == (int) ch || 78 == (int) ch)
                  {
                    ch = '\r';
                    this.emitLF = true;
                  }
                  this.escaped = false;
                }
                buffer[offset++] = ch;
              }
            }
            while (offset != num2);
            flag1 = true;
            continue;
          case ContentLineParser.States.ValueEnd:
            this.reader.RestoreCharsetEncoding();
            this.state = ContentLineParser.States.PropName;
            flag1 = true;
            flag2 = true;
            continue;
          default:
            flag1 = true;
            flag2 = true;
            continue;
        }
      }
      filled = offset - num1;
      return !flag2;
    }

    public void ApplyValueOverrides(Encoding charset, Mime.Encoders.ByteEncoder decoder)
    {
      this.CheckDisposed("ApplyValueDecoder");
      if (this.state == ContentLineParser.States.Value)
        throw new InvalidOperationException();
      this.currentValueCharsetOverride = charset;
      this.currentValueEncodingOverride = decoder;
    }

    public Stream GetValueReadStream()
    {
      this.CheckDisposed("GetValueReadStream");
      if (this.state != ContentLineParser.States.Value)
        throw new InvalidOperationException();
      Stream stream = this.reader.GetValueReadStream((DirectoryReader.OnValueEndFunc) (() =>
      {
        if (this.state != ContentLineParser.States.Value)
          throw new InvalidOperationException();
        this.state = ContentLineParser.States.ValueEnd;
        int filled;
        this.ParseElement(new char[0], 0, 0, out filled, true, ContentLineParser.Separators.None);
      }));
      if (this.currentValueEncodingOverride != null)
      {
        stream = (Stream) new Mime.Encoders.EncoderStream(stream, this.currentValueEncodingOverride, Mime.Encoders.EncoderStreamAccess.Read);
        this.currentValueEncodingOverride = (Mime.Encoders.ByteEncoder) null;
      }
      return stream;
    }

    protected virtual void CheckDisposed(string methodName)
    {
      if (this.isDisposed)
        throw new ObjectDisposedException("ContentLineParser", methodName);
    }

    private static ContentLineParser.Tokens GetToken(int ch)
    {
      if (ch > (int) byte.MaxValue)
        return ContentLineParser.Tokens.SafeChar | ContentLineParser.Tokens.QSafeChar | ContentLineParser.Tokens.ValueChar;
      return ContentLineParser.Dictionary[ch];
    }

    private void Dispose(bool disposing)
    {
      if (this.isDisposed)
        return;
      this.isDisposed = true;
      this.InternalDispose(disposing);
    }

    private void InternalDispose(bool disposing)
    {
      if (!disposing || this.reader == null)
        return;
      this.reader.Dispose();
      this.reader = (DirectoryReader) null;
    }

    private char GetCurrentChar()
    {
      if (this.lastCharProcessed)
      {
        if (this.eof)
          throw new InvalidOperationException();
        this.eof = !this.reader.ReadChar(out this.lastChar, out this.isEndOfLine);
      }
      this.lastCharProcessed = true;
      return this.lastChar;
    }

    internal enum States
    {
      PropName,
      ParamName,
      UnnamedParamEnd,
      ParamValueStart,
      ParamValueUnquoted,
      ParamValueQuoted,
      ParamValueQuotedEnd,
      ValueStartComma,
      ValueStartSemiColon,
      ValueStart,
      Value,
      ValueEnd,
      End,
    }

    [Flags]
    internal enum Tokens : byte
    {
      CTL = (byte) 1,
      Alpha = (byte) 2,
      Digit = (byte) 4,
      SafeChar = (byte) 8,
      QSafeChar = (byte) 16,
      ValueChar = (byte) 32,
      WSP = (byte) 64,
      NonASCII = (byte) 128,
    }

    [Flags]
    internal enum Separators
    {
      None = 0,
      Comma = 1,
      SemiColon = 2,
    }
  }
}
