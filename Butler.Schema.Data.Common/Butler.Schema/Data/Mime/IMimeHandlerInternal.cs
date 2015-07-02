// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.IMimeHandlerInternal
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  internal interface IMimeHandlerInternal
  {
    void PartStart(bool isInline, string inlineFileName, out PartParseOptionInternal partParseOption, out Stream outerContentWriteStream);

    void HeaderStart(HeaderId headerId, string name, out HeaderParseOptionInternal headerParseOption);

    void Header(Header header);

    void EndOfHeaders(string mediaType, ContentTransferEncoding cte, out PartContentParseOptionInternal partContentParseOption);

    void PartContent(byte[] buffer, int offset, int length);

    void PartEnd();

    void EndOfFile();
  }
}
