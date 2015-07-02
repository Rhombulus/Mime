// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.MimeParser
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  internal class MimeParser
  {
    private static readonly byte[] UUBegin = Internal.ByteString.StringToBytes("begin ", true);
    private static readonly byte[] UUEnd = Internal.ByteString.StringToBytes("end", true);
    private MimeParser.ParseState state;
    private int currentOffset;
    private int lineOffset;
    private bool mime;
    private MimeParser.ParseLevel currentLevel;
    private MimeParser.ParseLevel[] parseStack;
    private int parseStackTop;
    private int position;
    private int lastTokenLength;
    private bool firstHeader;
    private bool parseEmbeddedMessages;
    private bool parseInlineAttachments;
    private int nextBoundaryLevel;
    private bool nextBoundaryEnd;
    private MimeComplianceStatus compliance;
    private bool expectBinaryContent;
    private int headerNameLength;
    private int headerDataOffset;
    private bool headerComplete;
    private ContentTransferEncoding inlineFormat;

    public bool IsEndOfFile
    {
      get
      {
        return this.state == MimeParser.ParseState.EndOfFile;
      }
    }

    public int Position
    {
      get
      {
        return this.position;
      }
    }

    public int Depth
    {
      get
      {
        return this.parseStackTop;
      }
    }

    public int PartDepth
    {
      get
      {
        if (this.parseStackTop != 0)
          return this.parseStack[this.parseStackTop - 1].PartDepth;
        return 0;
      }
    }

    public int HeaderNameLength
    {
      get
      {
        return this.headerNameLength;
      }
    }

    public int HeaderDataOffset
    {
      get
      {
        return this.headerDataOffset;
      }
    }

    public bool IsHeaderComplete
    {
      get
      {
        return this.headerComplete;
      }
    }

    public MimeComplianceStatus ComplianceStatus
    {
      get
      {
        return this.compliance;
      }
      set
      {
        this.compliance = value;
      }
    }

    public bool IsMime
    {
      get
      {
        return this.mime;
      }
    }

    public MajorContentType ContentType
    {
      get
      {
        return this.currentLevel.ContentType;
      }
    }

    public ContentTransferEncoding TransferEncoding
    {
      get
      {
        return this.currentLevel.TransferEncoding;
      }
    }

    public ContentTransferEncoding InlineFormat
    {
      get
      {
        return this.inlineFormat;
      }
    }

    public MimeParser(bool expectBinaryContent)
    {
      this.parseInlineAttachments = true;
      this.expectBinaryContent = expectBinaryContent;
      this.Reset();
    }

    public MimeParser(bool parseEmbeddedMessages, bool parseInlineAttachments, bool expectBinaryContent)
    {
      this.parseEmbeddedMessages = parseEmbeddedMessages;
      this.parseInlineAttachments = parseInlineAttachments;
      this.expectBinaryContent = expectBinaryContent;
      this.Reset();
    }

    public void Reset()
    {
      this.state = MimeParser.ParseState.Headers;
      this.currentOffset = 0;
      this.lineOffset = 0;
      this.mime = false;
      this.currentLevel.Reset(true);
      this.parseStackTop = 0;
      this.position = 0;
      this.lastTokenLength = 0;
      this.firstHeader = true;
      this.nextBoundaryLevel = -1;
      this.nextBoundaryEnd = false;
      this.headerNameLength = 0;
      this.headerDataOffset = 0;
      this.headerComplete = false;
      this.inlineFormat = ContentTransferEncoding.Unknown;
      this.compliance = MimeComplianceStatus.Compliant;
    }

    public void SetMIME()
    {
      if (this.parseStackTop != 0 && this.parseStack[this.parseStackTop - 1].ContentType != MajorContentType.MessageRfc822)
        return;
      if (this.parseStackTop == 0)
        this.mime = true;
      this.currentLevel.IsMime = true;
    }

    public void SetContentType(MajorContentType contentType, MimeString boundaryValue)
    {
      this.currentLevel.SetContentType(contentType, boundaryValue);
      if (contentType == MajorContentType.Multipart)
        return;
      this.nextBoundaryLevel = -1;
    }

    public void SetTransferEncoding(ContentTransferEncoding encoding)
    {
      this.currentLevel.TransferEncoding = encoding;
    }

    public void SetStreamMode()
    {
      this.state = MimeParser.ParseState.Body;
      this.currentLevel.StreamMode = true;
    }

    public void ReportConsumedData(int lengthConsumed)
    {
      this.lastTokenLength -= lengthConsumed;
      this.position += lengthConsumed;
    }

    public MimeToken Parse(byte[] data, int start, int end, bool flush)
    {
      int num1 = start + this.currentOffset;
      int line = start + this.lineOffset;
      switch (this.state)
      {
        case MimeParser.ParseState.Headers:
          bool flag = false;
          int nextNL = Internal.ByteString.IndexOf(data, (byte) 10, num1, end - num1);
          if (nextNL == -1)
            nextNL = end;
          if (nextNL == end)
          {
            if (end - start <= 998 && !flush || !flush && end - start <= 999 && (int) data[end - 1] == 13)
            {
              this.currentOffset = end - start;
              return new MimeToken(MimeTokenId.None, 0);
            }
          }
          else if (nextNL == start || (int) data[nextNL - 1] != 13)
          {
            this.compliance |= MimeComplianceStatus.BareLinefeedInHeader;
            flag = true;
          }
          else
            --nextNL;
          this.headerNameLength = 0;
          this.headerDataOffset = 0;
          int num2;
          if (nextNL - start > 998)
          {
            this.compliance |= MimeComplianceStatus.InvalidWrapping;
            this.currentOffset = nextNL - (start + 998);
            this.lineOffset = line - (start + 998);
            nextNL = start + 998;
            num2 = 0;
          }
          else
          {
            this.currentOffset = 0;
            this.lineOffset = nextNL == end ? line - nextNL : 0;
            num2 = nextNL == end ? 0 : (flag ? 1 : 2);
          }
          if (nextNL == start)
          {
            this.state = MimeParser.ParseState.EndOfHeaders;
            this.lastTokenLength = num2;
            return new MimeToken(MimeTokenId.EndOfHeaders, this.lastTokenLength);
          }
          this.headerComplete = num2 != 0 && nextNL + num2 < end && ((int) data[nextNL + num2] != 32 && (int) data[nextNL + num2] != 9);
          if (!this.firstHeader && (line < start || (int) data[line] == 32 || (int) data[line] == 9))
          {
            this.lastTokenLength = nextNL + num2 - start;
            return new MimeToken(MimeTokenId.HeaderContinuation, this.lastTokenLength);
          }
          this.firstHeader = false;
          int characterCount = 0;
          this.headerNameLength = MimeScan.FindEndOf(MimeScan.Token.Field, data, start, nextNL - start, out characterCount, false);
          if (this.headerNameLength == 0)
          {
            this.compliance |= MimeComplianceStatus.InvalidHeader;
            this.lastTokenLength = nextNL + num2 - start;
            return new MimeToken(MimeTokenId.Header, this.lastTokenLength);
          }
          int offset = start + this.headerNameLength;
          if (offset == nextNL || (int) data[offset] != 58)
          {
            offset += MimeScan.SkipLwsp(data, offset, nextNL - offset);
            if (offset == nextNL || (int) data[offset] != 58)
            {
              this.headerNameLength = 0;
              if (this.mime && (this.parseStackTop > 0 || this.currentLevel.ContentType == MajorContentType.Multipart) && (nextNL - line > 2 && (int) data[line] == 45 && ((int) data[line + 1] == 45 && this.FindBoundary(data, line, nextNL, out this.nextBoundaryLevel, out this.nextBoundaryEnd))))
              {
                this.compliance |= MimeComplianceStatus.MissingBodySeparator;
                if (this.nextBoundaryLevel != this.parseStackTop)
                  this.compliance |= MimeComplianceStatus.MissingBoundary;
                this.lineOffset = 0;
                this.currentOffset = nextNL - start;
                this.state = MimeParser.ParseState.EndOfHeaders;
                return new MimeToken(MimeTokenId.EndOfHeaders, 0);
              }
              this.compliance |= MimeComplianceStatus.InvalidHeader;
              this.lastTokenLength = nextNL + num2 - start;
              return new MimeToken(MimeTokenId.Header, this.lastTokenLength);
            }
          }
          this.headerDataOffset = offset + 1 - start;
          this.lastTokenLength = nextNL + num2 - start;
          return new MimeToken(MimeTokenId.Header, this.lastTokenLength);
        case MimeParser.ParseState.EndOfHeaders:
          this.CheckMimeConstraints();
          if (this.mime && this.parseEmbeddedMessages && (this.currentLevel.ContentType == MajorContentType.MessageRfc822 && !this.currentLevel.StreamMode))
          {
            this.PushLevel(false);
            this.lastTokenLength = 0;
            return new MimeToken(MimeTokenId.EmbeddedStart, 0);
          }
          this.state = MimeParser.ParseState.Body;
          goto case 2;
        case MimeParser.ParseState.Body:
          return this.ParseBody(data, start, end, flush, line, num1);
        default:
          return new MimeToken(MimeTokenId.EndOfFile, 0);
      }
    }

    private static bool IsUUEncodeBegin(byte[] data, int line, int nextNL)
    {
      MimeString mimeString = new MimeString(data, line, nextNL - line);
      if (mimeString.Length < 13 || !mimeString.HasPrefixEq(MimeParser.UUBegin, 0, 6))
        return false;
      int index = 6;
      while (index < 10 && (48 <= (int) mimeString[index] && 55 >= (int) mimeString[index]))
        ++index;
      return index != 6 && 32 == (int) mimeString[index];
    }

    private static bool IsUUEncodeEnd(byte[] data, int line, int nextNL)
    {
      MimeString mimeString = new MimeString(data, line, nextNL - line);
      return mimeString.Length >= 3 && mimeString.HasPrefixEq(MimeParser.UUEnd, 0, 3);
    }

    private MimeToken ParseBody(byte[] data, int start, int end, bool flush, int line, int current)
    {
      int num1 = line <= start ? 0 : line - start;
      int nextNL;
      int sizeNL;
      bool flag;
      while (true)
      {
        if (this.expectBinaryContent)
        {
          nextNL = Internal.ByteString.IndexOf(data, (byte) 10, current, end - current);
        }
        else
        {
          bool containsBinary;
          nextNL = Internal.ByteString.IndexOf(data, (byte) 10, current, end - current, out containsBinary);
          if (containsBinary)
            this.compliance |= MimeComplianceStatus.UnexpectedBinaryContent;
        }
        if (nextNL == -1)
          nextNL = end;
        if (nextNL == end)
        {
          sizeNL = 0;
          if (end - start != 0 && (int) data[end - 1] == 13 && !flush)
          {
            --nextNL;
            --end;
          }
        }
        else if (nextNL == start || (int) data[nextNL - 1] != 13)
        {
          if (this.currentLevel.TransferEncoding != ContentTransferEncoding.Binary)
            this.compliance |= MimeComplianceStatus.BareLinefeedInBody;
          sizeNL = 1;
        }
        else
        {
          --nextNL;
          sizeNL = 2;
        }
        if (nextNL - line > 998 && this.currentLevel.TransferEncoding != ContentTransferEncoding.Binary)
          this.compliance |= MimeComplianceStatus.InvalidWrapping;
        if (this.nextBoundaryLevel == -1)
        {
          if (!this.mime || line < start || nextNL != end && nextNL == line || (nextNL != line && (int) data[line] != 45 || nextNL - line > 998))
          {
            if (!this.parseInlineAttachments || this.currentLevel.IsMime || line < start || nextNL != end && nextNL == line || nextNL != line && (this.inlineFormat == ContentTransferEncoding.Unknown && ((int) data[line] | 32) != 98 || this.inlineFormat == ContentTransferEncoding.UUEncode && ((int) data[line] | 32) != 101) || nextNL - line > 998)
            {
              if (nextNL != end)
              {
                current = nextNL + sizeNL;
                line = current;
                num1 = sizeNL;
                continue;
              }
              break;
            }
            flag = false;
          }
          else
            flag = true;
          if (nextNL != end || flush)
          {
            if (!flag || nextNL - line <= 2 || this.parseStackTop == 0 && this.currentLevel.ContentType != MajorContentType.Multipart || ((int) data[line + 1] != 45 || !this.FindBoundary(data, line, nextNL, out this.nextBoundaryLevel, out this.nextBoundaryEnd)))
            {
              if (!this.parseInlineAttachments || this.currentLevel.IsMime || !this.IsInlineBoundary(data, line, nextNL, end, out this.nextBoundaryLevel, out this.nextBoundaryEnd))
              {
                if (nextNL != end)
                {
                  current = nextNL + sizeNL;
                  line = current;
                  num1 = sizeNL;
                  continue;
                }
                goto label_30;
              }
              else
                flag = false;
            }
            if (this.nextBoundaryLevel == this.parseStackTop && (this.currentLevel.Epilogue || this.nextBoundaryEnd))
            {
              this.compliance |= MimeComplianceStatus.MissingBoundary;
              this.nextBoundaryLevel = -1;
              this.currentLevel.Epilogue = true;
              if (nextNL != end)
              {
                current = nextNL + sizeNL;
                line = current;
                num1 = sizeNL;
              }
              else
                goto label_36;
            }
            else
              goto label_37;
          }
          else
            goto label_26;
        }
        else
          goto label_39;
      }
      int num2 = end;
      goto label_40;
label_26:
      num2 = line < start + num1 || !flag ? line : line - num1;
      goto label_40;
label_30:
      num2 = end;
      goto label_40;
label_36:
      num2 = end;
      goto label_40;
label_37:
      if (line - start > (flag ? num1 : 0))
      {
        num2 = line - (flag ? num1 : 0);
        goto label_40;
      }
label_39:
      return this.ProcessBoundary(start, line, nextNL, sizeNL);
label_40:
      this.lineOffset = line - num2;
      this.currentOffset = nextNL - num2;
      if (num2 != start)
      {
        this.lastTokenLength = num2 - start;
        return new MimeToken(MimeTokenId.PartData, this.lastTokenLength);
      }
      if (!flush)
        return new MimeToken(MimeTokenId.None, 0);
      return this.ProcessEOF();
    }

    private void PushLevel(bool inheritMime)
    {
      if (this.parseStack == null || this.parseStackTop == this.parseStack.Length)
      {
        MimeParser.ParseLevel[] parseLevelArray = new MimeParser.ParseLevel[this.parseStack == null ? 4 : this.parseStack.Length * 2];
        if (this.parseStack != null)
          Array.Copy((Array) this.parseStack, 0, (Array) parseLevelArray, 0, this.parseStackTop);
        for (int index = 0; index < this.parseStackTop; ++index)
          this.parseStack[index] = new MimeParser.ParseLevel();
        this.parseStack = parseLevelArray;
      }
      if (this.currentLevel.ContentType != MajorContentType.MessageRfc822)
        this.currentLevel.PartDepth = this.parseStackTop == 0 ? 1 : this.parseStack[this.parseStackTop - 1].PartDepth + 1;
      this.parseStack[this.parseStackTop++] = this.currentLevel;
      this.currentLevel.Reset(!inheritMime);
      this.state = MimeParser.ParseState.Headers;
      this.firstHeader = true;
    }

    private void CheckMimeConstraints()
    {
      if (!this.mime)
      {
        this.currentLevel.SetContentType(MajorContentType.Other, new MimeString());
        this.currentLevel.TransferEncoding = ContentTransferEncoding.SevenBit;
      }
      else
      {
        if ((this.currentLevel.ContentType == MajorContentType.Multipart || this.currentLevel.ContentType == MajorContentType.MessageRfc822 || this.currentLevel.ContentType == MajorContentType.Message) && this.currentLevel.TransferEncoding > ContentTransferEncoding.Binary)
          this.compliance |= MimeComplianceStatus.InvalidTransferEncoding;
        if (this.parseStackTop == 0 || this.currentLevel.TransferEncoding > ContentTransferEncoding.Binary || this.currentLevel.TransferEncoding <= this.parseStack[this.parseStackTop - 1].TransferEncoding)
          return;
        this.compliance |= MimeComplianceStatus.InvalidTransferEncoding;
      }
    }

    private bool FindBoundary(byte[] data, int line, int nextNL, out int nextBoundaryLevel, out bool nextBoundaryEnd)
    {
      while (nextNL > line && MimeScan.IsLWSP(data[nextNL - 1]))
        --nextNL;
      uint crc = Internal.ByteString.ComputeCrc(data, line, nextNL - line);
      bool term;
      if (this.currentLevel.IsBoundary(data, line, nextNL - line, (long) crc, out term))
      {
        nextBoundaryLevel = this.parseStackTop;
        nextBoundaryEnd = term;
        return true;
      }
      for (int index = this.parseStackTop - 1; index >= 0; --index)
      {
        if (this.parseStack[index].IsBoundary(data, line, nextNL - line, (long) crc, out term))
        {
          nextBoundaryLevel = index;
          nextBoundaryEnd = term;
          return true;
        }
      }
      nextBoundaryLevel = -1;
      nextBoundaryEnd = false;
      return false;
    }

    private bool IsInlineBoundary(byte[] data, int line, int nextNL, int end, out int nextBoundaryLevel, out bool nextBoundaryEnd)
    {
      switch (this.inlineFormat)
      {
        case ContentTransferEncoding.Unknown:
          if (((int) data[line] | 32) == 98 && nextNL - line >= 11 && (nextNL != end && MimeParser.IsUUEncodeBegin(data, line, nextNL)))
          {
            nextBoundaryLevel = -100;
            nextBoundaryEnd = false;
            return true;
          }
          break;
        case ContentTransferEncoding.UUEncode:
          if (((int) data[line] | 32) == 101 && nextNL - line >= 3 && MimeParser.IsUUEncodeEnd(data, line, nextNL))
          {
            nextBoundaryLevel = -100;
            nextBoundaryEnd = true;
            return true;
          }
          break;
      }
      nextBoundaryLevel = -1;
      nextBoundaryEnd = false;
      return false;
    }

    private MimeToken ProcessBoundary(int start, int line, int nextNL, int sizeNL)
    {
      if (this.nextBoundaryLevel < 0)
      {
        this.lineOffset = 0;
        this.currentOffset = 0;
        if (!this.nextBoundaryEnd)
        {
          this.inlineFormat = this.nextBoundaryLevel == -100 ? ContentTransferEncoding.UUEncode : ContentTransferEncoding.BinHex;
          this.nextBoundaryLevel = -1;
          this.lastTokenLength = nextNL + sizeNL - start;
          return new MimeToken(MimeTokenId.InlineStart, this.lastTokenLength);
        }
        this.inlineFormat = ContentTransferEncoding.Unknown;
        this.nextBoundaryLevel = -1;
        this.lastTokenLength = nextNL + sizeNL - start;
        return new MimeToken(MimeTokenId.InlineEnd, this.lastTokenLength);
      }
      if (this.nextBoundaryLevel == this.parseStackTop)
      {
        this.lineOffset = 0;
        this.currentOffset = 0;
        this.nextBoundaryLevel = -1;
        this.PushLevel(true);
        this.lastTokenLength = nextNL + sizeNL - start;
        return new MimeToken(MimeTokenId.NestedStart, this.lastTokenLength);
      }
      if (this.nextBoundaryLevel == this.parseStackTop - 1)
      {
        if (this.currentLevel.ContentType == MajorContentType.Multipart && !this.currentLevel.Epilogue)
          this.compliance |= MimeComplianceStatus.MissingBoundary;
        this.lineOffset = 0;
        this.currentOffset = 0;
        this.nextBoundaryLevel = -1;
        if (this.nextBoundaryEnd)
        {
          this.currentLevel = this.parseStack[--this.parseStackTop];
          this.currentLevel.Epilogue = true;
          this.parseStack[this.parseStackTop].Reset(false);
          this.lastTokenLength = nextNL + sizeNL - start;
          return new MimeToken(MimeTokenId.NestedEnd, this.lastTokenLength);
        }
        this.currentLevel.Reset(false);
        this.state = MimeParser.ParseState.Headers;
        this.firstHeader = true;
        this.lastTokenLength = nextNL + sizeNL - start;
        return new MimeToken(MimeTokenId.NestedNext, this.lastTokenLength);
      }
      this.lineOffset = line - start;
      this.currentOffset = nextNL - start;
      if (this.inlineFormat != ContentTransferEncoding.Unknown)
      {
        this.compliance |= MimeComplianceStatus.MissingBoundary;
        this.inlineFormat = ContentTransferEncoding.Unknown;
        return new MimeToken(MimeTokenId.InlineEnd, 0);
      }
      this.currentLevel = this.parseStack[--this.parseStackTop];
      this.currentLevel.Epilogue = true;
      this.parseStack[this.parseStackTop].Reset(false);
      if (this.currentLevel.ContentType == MajorContentType.MessageRfc822)
        return new MimeToken(MimeTokenId.EmbeddedEnd, 0);
      this.compliance |= MimeComplianceStatus.MissingBoundary;
      return new MimeToken(MimeTokenId.NestedEnd, 0);
    }

    private MimeToken ProcessEOF()
    {
      if (this.inlineFormat != ContentTransferEncoding.Unknown)
      {
        this.compliance |= MimeComplianceStatus.MissingBoundary;
        this.inlineFormat = ContentTransferEncoding.Unknown;
        return new MimeToken(MimeTokenId.InlineEnd, 0);
      }
      if (this.parseStackTop != 0)
      {
        this.currentLevel = this.parseStack[--this.parseStackTop];
        this.currentLevel.Epilogue = true;
        this.parseStack[this.parseStackTop].Reset(false);
        if (this.currentLevel.ContentType == MajorContentType.MessageRfc822)
          return new MimeToken(MimeTokenId.EmbeddedEnd, 0);
        this.compliance |= MimeComplianceStatus.MissingBoundary;
        return new MimeToken(MimeTokenId.NestedEnd, 0);
      }
      this.state = MimeParser.ParseState.EndOfFile;
      this.currentLevel.Reset(true);
      return new MimeToken(MimeTokenId.EndOfFile, 0);
    }

    private enum ParseState
    {
      Headers,
      EndOfHeaders,
      Body,
      EndOfFile,
    }


      private struct ParseLevel {

          private bool mime;
          private bool streamMode;
          private MajorContentType contentType;
          public bool Epilogue;
          private ContentTransferEncoding transferEncoding;
          private MimeString boundaryValue;
          private int partDepth;
          private uint boundaryCrc;
          private uint endBoundaryCrc;

          public MajorContentType ContentType {
              get {
                  return this.contentType;
              }
          }

          public ContentTransferEncoding TransferEncoding {
              get {
                  return this.transferEncoding;
              }
              set {
                  this.transferEncoding = value;
              }
          }

          public int PartDepth {
              get {
                  return this.partDepth;
              }
              set {
                  this.partDepth = value;
              }
          }

          public bool IsMime {
              get {
                  return this.mime;
              }
              set {
                  this.mime = value;
              }
          }

          public bool StreamMode {
              get {
                  return this.streamMode;
              }
              set {
                  this.streamMode = value;
              }
          }

          public void Reset(bool cleanMimeState) {
              this.streamMode = false;
              this.contentType = MajorContentType.Other;
              this.Epilogue = false;
              this.transferEncoding = ContentTransferEncoding.SevenBit;
              if (cleanMimeState)
                  this.mime = false;
              this.boundaryValue = new MimeString();
              this.boundaryCrc = 0U;
              this.endBoundaryCrc = 0U;
              this.partDepth = 0;
          }

          public void SetContentType(MajorContentType contentType, MimeString boundaryValue) {
              if (contentType == MajorContentType.Multipart) {
                  int offset;
                  int count;
                  byte[] data = boundaryValue.GetData(out offset, out count);
                  byte[] numArray = new byte[MimeString.TwoDashes.Length + count + MimeString.TwoDashes.Length];
                  int length = MimeString.TwoDashes.Length;
                  Buffer.BlockCopy((Array) MimeString.TwoDashes, 0, (Array) numArray, 0, length);
                  Buffer.BlockCopy((Array) data, offset, (Array) numArray, length, count);
                  int num1 = length + count;
                  this.boundaryCrc = Internal.ByteString.ComputeCrc(numArray, 0, num1);
                  Buffer.BlockCopy((Array) MimeString.TwoDashes, 0, (Array) numArray, num1, MimeString.TwoDashes.Length);
                  int num2 = num1 + MimeString.TwoDashes.Length;
                  this.endBoundaryCrc = Internal.ByteString.ComputeCrc(numArray, 0, num2);
                  this.boundaryValue = new MimeString(numArray, 0, num2);
              } else {
                  this.boundaryValue = new MimeString();
                  this.boundaryCrc = 0U;
                  this.endBoundaryCrc = 0U;
              }
              this.contentType = contentType;
          }

          //public bool IsBoundary(byte[] bytes, int offset, int length, long crc, out bool term)
          //{
          //  if (crc == (long) this.boundaryCrc && this.boundaryValue.Length - 2 == length)
          //  {
          //    term = false;
          //    return this.boundaryValue.HasPrefixEq(bytes, offset, length);
          //  }
          //  if (crc == (long) this.endBoundaryCrc && this.boundaryValue.Length == length)
          //  {
          //    // ISSUE: explicit reference operation
          //    // ISSUE: cast to a reference type
          //    // ISSUE: explicit reference operation
          //    return (bool) (^(sbyte&) @term = (sbyte) this.boundaryValue.HasPrefixEq(bytes, offset, length));
          //  }
          //  // ISSUE: explicit reference operation
          //  // ISSUE: cast to a reference type
          //  // ISSUE: explicit reference operation
          //  return (bool) (^(sbyte&) @term = (sbyte) false);
          //}
          public bool IsBoundary(byte[] bytes, int offset, int length, long crc, out bool term) {
              bool flag2;
              if ((crc == this.boundaryCrc) && ((this.boundaryValue.Length - 2) == length)) {
                  term = false;
                  return this.boundaryValue.HasPrefixEq(bytes, offset, length);
              }
              if ((crc == this.endBoundaryCrc) && (this.boundaryValue.Length == length)) {
                  bool flag;
                  term = flag = this.boundaryValue.HasPrefixEq(bytes, offset, length);
                  return flag;
              }
              term = flag2 = false;
              return flag2;
          }
      }
  }
}
