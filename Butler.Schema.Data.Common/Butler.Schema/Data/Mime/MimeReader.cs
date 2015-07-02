// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.MimeReader
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  public class MimeReader : IDisposable
  {
    private bool FixBadMimeBoundary = true;
    private MimeReaderState state = MimeReaderState.Start;
    private int partCount = 1;
    private bool eofMeansEndOfFile = true;
    private const int DataBufferSize = 5120;
    private Stream mimeStream;
    private IMimeHandlerInternal handler;
    private bool inferMime;
    private bool parseEmbeddedMessages;
    private DecodingOptions decodingOptions;
    private MimeLimits limits;
    private MimeParser parser;
    private int depth;
    private int cleanupDepth;
    private int embeddedDepth;
    private bool dataExhausted;
    private bool dataEOF;
    private byte[] data;
    private int dataOffset;
    private int dataCount;
    private MimeToken currentToken;
    private HeaderId currentHeaderId;
    private string currentHeaderName;
    private bool createHeader;
    private Header currentHeader;
    private bool currentHeaderEmpty;
    private bool currentHeaderConsumed;
    private bool currentChildConsumed;
    private MimeNode currentChild;
    private MimeNode currentGrandChild;
    private MajorContentType currentPartMajorType;
    private string currentPartContentType;
    private ContentTransferEncoding currentPartContentTransferEncoding;
    private LineTerminationState currentLineTerminationState;
    private MimeString inlineFileName;
    private Encoders.ByteEncoder decoder;
    private bool readRawData;
    private Stream contentStream;
    private bool enableReadingOuterContent;
    private Stream outerContentStream;
    private int outerContentDepth;
    private MimeReader childReader;
    private MimeReader parentReader;
    private int headerCount;
    private int cumulativeHeaderBytes;
    private int currentTextHeaderBytes;
    private bool skipPart;
    private bool skipHeaders;
    private bool skipHeader;
    private HeaderList headerList;

    public MimeLimits MimeLimits
    {
      get
      {
        this.AssertGoodToUse(false, false);
        return this.limits;
      }
    }

    public DecodingOptions HeaderDecodingOptions => this.decodingOptions;

      public MimeComplianceStatus ComplianceStatus
    {
      get
      {
        this.AssertGoodToUse(false, false);
        return this.parser.ComplianceStatus;
      }
    }

    public long StreamOffset
    {
      get
      {
        this.AssertGoodToUse(false, true);
        return (long) this.parser.Position;
      }
    }

    internal int Depth
    {
      get
      {
        this.AssertGoodToUse(false, true);
        return this.depth;
      }
    }

    public int PartDepth
    {
      get
      {
        this.AssertGoodToUse(false, true);
        if (this.depth != 0)
          return this.parser.PartDepth + 1;
        return 0;
      }
    }

    public int EmbeddedDepth
    {
      get
      {
        this.AssertGoodToUse(false, true);
        return this.embeddedDepth;
      }
    }

    internal MimeReaderState ReaderState => this.state;

      internal bool DataExhausted => this.dataExhausted;

      internal bool EndOfFile => this.state == MimeReaderState.End;

      public MimeHeaderReader HeaderReader
    {
      get
      {
        this.AssertGoodToUse(true, true);
        if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderComplete | MimeReaderState.EndOfHeaders | MimeReaderState.InlineStart))
          throw new InvalidOperationException(CtsResources.Strings.OperationNotValidInThisReaderState);
        return new MimeHeaderReader(this);
      }
    }

    internal HeaderId HeaderId
    {
      get
      {
        this.AssertGoodToUse(false, true);
        if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.HeaderStart | MimeReaderState.HeaderIncomplete | MimeReaderState.HeaderComplete))
          throw new InvalidOperationException(CtsResources.Strings.OperationNotValidInThisReaderState);
        return this.currentHeaderId;
      }
    }

    internal string HeaderName
    {
      get
      {
        this.AssertGoodToUse(false, true);
        if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.HeaderStart | MimeReaderState.HeaderIncomplete | MimeReaderState.HeaderComplete))
          throw new InvalidOperationException(CtsResources.Strings.OperationNotValidInThisReaderState);
        return this.currentHeaderName;
      }
    }

    public bool IsMultipart
    {
      get
      {
        this.AssertGoodToUse(false, true);
        if (this.state == MimeReaderState.InlineStart)
          return false;
        if (this.state != MimeReaderState.EndOfHeaders)
          throw new InvalidOperationException(CtsResources.Strings.OperationNotValidInThisReaderState);
        if (this.currentPartMajorType == MajorContentType.Multipart)
          return this.parser.IsMime;
        return false;
      }
    }

    public bool IsEmbeddedMessage
    {
      get
      {
        this.AssertGoodToUse(false, true);
        if (this.state == MimeReaderState.InlineStart)
          return false;
        if (this.state != MimeReaderState.EndOfHeaders)
          throw new InvalidOperationException(CtsResources.Strings.OperationNotValidInThisReaderState);
        if (this.currentPartMajorType == MajorContentType.MessageRfc822)
          return this.parser.IsMime;
        return false;
      }
    }

    public string ContentType
    {
      get
      {
        this.AssertGoodToUse(false, true);
        if (this.state == MimeReaderState.InlineStart)
          return "application/octet-stream";
        if (this.state != MimeReaderState.EndOfHeaders)
          throw new InvalidOperationException(CtsResources.Strings.OperationNotValidInThisReaderState);
        return this.currentPartContentType;
      }
    }

    internal ContentTransferEncoding ContentTransferEncoding
    {
      get
      {
        this.AssertGoodToUse(false, true);
        if (this.state != MimeReaderState.EndOfHeaders && this.state != MimeReaderState.InlineStart)
          throw new InvalidOperationException(CtsResources.Strings.OperationNotValidInThisReaderState);
        return this.currentPartContentTransferEncoding;
      }
    }

    public bool IsInline
    {
      get
      {
        this.AssertGoodToUse(false, true);
        if (MimeReader.StateIsOneOf(this.state, MimeReaderState.Start | MimeReaderState.End))
          throw new InvalidOperationException(CtsResources.Strings.OperationNotValidInThisReaderState);
        return MimeReader.StateIsOneOf(this.state, MimeReaderState.InlineStart | MimeReaderState.InlineBody | MimeReaderState.InlineEnd);
      }
    }

    public string InlineFileName
    {
      get
      {
        this.AssertGoodToUse(false, true);
        if (!this.IsInline)
          throw new InvalidOperationException(CtsResources.Strings.OperationNotValidInThisReaderState);
        return this.inlineFileName.ToString();
      }
    }

    internal LineTerminationState LineTerminationState => this.currentLineTerminationState;

      internal Header CurrentHeaderObject => this.currentHeader;

      internal bool GroupInProgress
    {
      get
      {
        if (this.currentChild != null)
          return this.currentChild is MimeGroup;
        return false;
      }
    }

    public MimeReader(Stream mime)
      : this(mime, true, DecodingOptions.Default, MimeLimits.Unlimited, false, true)
    {
      if (mime == null)
        throw new ArgumentNullException(nameof(mime));
    }

    public MimeReader(Stream mime, bool inferMime, DecodingOptions decodingOptions, MimeLimits mimeLimits)
      : this(mime, inferMime, decodingOptions, mimeLimits, false, true)
    {
      if (mime == null)
        throw new ArgumentNullException(nameof(mime));
    }

    internal MimeReader(Stream mime, bool inferMime, DecodingOptions decodingOptions, MimeLimits mimeLimits, bool parseEmbeddedMessages, bool parseInline)
      : this(mime, inferMime, decodingOptions, mimeLimits, parseEmbeddedMessages, parseInline, true)
    {
    }

    internal MimeReader(Stream mime, bool inferMime, DecodingOptions decodingOptions, MimeLimits mimeLimits, bool parseEmbeddedMessages, bool parseInline, bool expectBinaryContent)
    {
      if (mime != null && !mime.CanRead)
        throw new ArgumentException(CtsResources.Strings.StreamMustAllowRead, nameof(mime));
      this.mimeStream = mime;
      this.parser = new MimeParser(true, parseInline, expectBinaryContent);
      this.data = new byte[5120];
      this.inferMime = inferMime;
      this.decodingOptions = decodingOptions;
      this.limits = mimeLimits;
      this.parseEmbeddedMessages = parseEmbeddedMessages;
    }

    private MimeReader(MimeReader parent)
    {
      this.parentReader = parent;
      this.parentReader.childReader = this;
      this.mimeStream = parent.mimeStream;
      this.parser = parent.parser;
      this.data = parent.data;
      this.dataOffset = parent.dataOffset;
      this.dataCount = parent.dataCount;
      this.dataEOF = parent.dataEOF;
      this.outerContentStream = parent.outerContentStream;
      this.outerContentDepth = -1;
      this.inferMime = parent.inferMime;
      this.limits = parent.limits;
      this.decodingOptions = parent.decodingOptions;
      this.partCount = parent.partCount;
      this.headerCount = parent.headerCount;
      this.cumulativeHeaderBytes = parent.cumulativeHeaderBytes;
      this.embeddedDepth = parent.embeddedDepth + 1;
    }

    internal MimeReader(IMimeHandlerInternal handler, bool inferMime, DecodingOptions decodingOptions, MimeLimits mimeLimits, bool parseInline)
    {
      if (handler == null)
        throw new ArgumentNullException(nameof(handler));
      this.handler = handler;
      this.parser = new MimeParser(true, parseInline, true);
      this.data = new byte[5120];
      this.inferMime = inferMime;
      this.decodingOptions = decodingOptions;
      this.limits = mimeLimits;
      this.parseEmbeddedMessages = true;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.parser == null)
        return;
      if (disposing)
      {
        if (this.childReader != null)
          throw new InvalidOperationException(CtsResources.Strings.EmbeddedMessageReaderNeedsToBeClosedFirst);
        if (this.parentReader == null)
        {
          if (this.mimeStream != null)
            this.mimeStream.Dispose();
        }
        else
        {
          this.parentReader.partCount = this.partCount;
          this.parentReader.headerCount = this.headerCount;
          this.parentReader.cumulativeHeaderBytes = this.cumulativeHeaderBytes;
          this.parentReader.dataOffset = this.dataOffset;
          this.parentReader.dataCount = this.dataCount;
          this.parentReader.dataEOF = this.dataEOF;
          this.parentReader.currentToken = this.currentToken;
          this.parentReader.cleanupDepth = this.depth + this.cleanupDepth;
          this.parentReader.state = MimeReaderState.EmbeddedEnd;
          this.parentReader.childReader = (MimeReader) null;
          this.parentReader = (MimeReader) null;
        }
        if (this.contentStream != null)
          this.contentStream.Dispose();
        if (this.outerContentStream != null)
        {
          int num = this.outerContentDepth;
        }
      }
      this.state = MimeReaderState.End;
      this.mimeStream = (Stream) null;
      this.handler = (IMimeHandlerInternal) null;
      this.contentStream = (Stream) null;
      this.outerContentStream = (Stream) null;
      this.data = (byte[]) null;
      this.parser = (MimeParser) null;
      this.currentHeader = (Header) null;
      this.currentChild = (MimeNode) null;
      this.currentGrandChild = (MimeNode) null;
    }

    public void Close()
    {
      this.Dispose();
    }

    internal void DisconnectInputStream()
    {
      this.mimeStream = (Stream) null;
    }

    public bool ReadNextPart()
    {
      this.AssertGoodToUse(true, true);
      this.TrySkipToNextPartBoundary(false);
      return MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.InlineStart);
    }

    public bool ReadFirstChildPart()
    {
      this.AssertGoodToUse(true, true);
      if (this.state == MimeReaderState.InlineStart)
        return false;
      if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.Start | MimeReaderState.EndOfHeaders))
      {
        if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderComplete))
          throw new InvalidOperationException(CtsResources.Strings.OperationNotValidInThisReaderState);
        do
        {
          this.TryReachNextState();
        }
        while (this.state != MimeReaderState.EndOfHeaders);
      }
      if (this.state == MimeReaderState.EndOfHeaders && !this.IsMultipart && (!this.IsEmbeddedMessage || !this.parseEmbeddedMessages))
        return false;
      this.TrySkipToNextPartBoundary(true);
      return this.state == MimeReaderState.PartStart;
    }

    public bool ReadNextSiblingPart()
    {
      this.AssertGoodToUse(true, true);
      if (this.state == MimeReaderState.End)
        return false;
      if (MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderComplete | MimeReaderState.EndOfHeaders))
      {
        this.createHeader = false;
        while (this.state != MimeReaderState.EndOfHeaders)
          this.TryReachNextState();
        this.parser.SetContentType(MajorContentType.Other, new MimeString());
      }
      if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.Start | MimeReaderState.PartEnd | MimeReaderState.InlineEnd))
      {
        int num = this.depth;
        do
        {
          this.TrySkipToNextPartBoundary(true);
        }
        while (this.depth > num || !MimeReader.StateIsOneOf(this.state, MimeReaderState.PartEnd | MimeReaderState.InlineEnd));
      }
      this.TrySkipToNextPartBoundary(true);
      return MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.InlineStart);
    }

    public void EnableReadingUnparsedHeaders()
    {
      this.enableReadingOuterContent = true;
    }

    public void ReadHeaders()
    {
      this.AssertGoodToUse(true, true);
      if (MimeReader.StateIsOneOf(this.state, MimeReaderState.EndOfHeaders | MimeReaderState.InlineStart))
        return;
      if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderComplete))
        throw new InvalidOperationException(CtsResources.Strings.OperationNotValidInThisReaderState);
      this.createHeader = false;
      do
      {
        this.TryReachNextState();
      }
      while (this.state != MimeReaderState.EndOfHeaders);
    }

    internal bool ReadNextHeader()
    {
      this.AssertGoodToUse(true, true);
      if (MimeReader.StateIsOneOf(this.state, MimeReaderState.EndOfHeaders | MimeReaderState.InlineStart))
        return false;
      if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderComplete))
        throw new InvalidOperationException(CtsResources.Strings.OperationNotValidInThisReaderState);
      do
      {
        this.TryReachNextState();
      }
      while (!MimeReader.StateIsOneOf(this.state, MimeReaderState.HeaderStart | MimeReaderState.EndOfHeaders));
      return this.state == MimeReaderState.HeaderStart;
    }

    internal Header ReadHeaderObject()
    {
      if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.HeaderStart | MimeReaderState.HeaderComplete))
        throw new InvalidOperationException(CtsResources.Strings.OperationNotValidInThisReaderState);
      if (this.state == MimeReaderState.HeaderStart)
        this.TryCompleteCurrentHeader(true);
      return this.currentHeader;
    }

    internal bool TryCompleteCurrentHeader(bool createHeader)
    {
      this.createHeader = this.createHeader || createHeader;
      if (!this.TryReachNextState())
        return false;
      this.currentHeaderConsumed = false;
      this.currentChildConsumed = false;
      this.currentChild = (MimeNode) null;
      this.currentGrandChild = (MimeNode) null;
      return true;
    }

    internal void Reset(Stream stream)
    {
      this.mimeStream = stream;
      this.parser.Reset();
      this.state = MimeReaderState.Start;
      this.depth = 0;
      this.cleanupDepth = 0;
      this.embeddedDepth = 0;
      this.dataExhausted = false;
      this.dataEOF = false;
      this.dataOffset = 0;
      this.dataCount = 0;
      this.currentToken = new MimeToken();
      this.currentHeader = (Header) null;
      this.currentChild = (MimeNode) null;
      this.currentGrandChild = (MimeNode) null;
      this.decoder = (Encoders.ByteEncoder) null;
      this.readRawData = false;
      this.contentStream = (Stream) null;
      this.enableReadingOuterContent = false;
      this.outerContentStream = (Stream) null;
      this.outerContentDepth = 0;
      this.childReader = (MimeReader) null;
      this.parentReader = (MimeReader) null;
      this.skipPart = false;
      this.skipHeaders = false;
      this.skipHeader = false;
      this.partCount = 0;
      this.headerCount = 0;
      this.cumulativeHeaderBytes = 0;
      this.currentTextHeaderBytes = 0;
    }

    internal void DangerousSetFixBadMimeBoundary(bool value)
    {
      this.FixBadMimeBoundary = value;
    }

    internal HeaderList ReadHeaderList()
    {
      this.AssertGoodToUse(true, true);
      if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.InlineStart))
        throw new InvalidOperationException(CtsResources.Strings.OperationNotValidInThisReaderState);
      HeaderList headerList = new HeaderList((MimeNode) null);
      if (this.state == MimeReaderState.InlineStart)
        return headerList;
      do
      {
        this.TryReachNextState();
        if (this.state == MimeReaderState.HeaderStart)
          this.createHeader = true;
        else if (this.state == MimeReaderState.HeaderComplete && this.currentHeader != null)
          headerList.InternalAppendChild((MimeNode) this.currentHeader);
      }
      while (this.state != MimeReaderState.EndOfHeaders);
      return headerList;
    }

    internal bool ReadNextDescendant(bool topLevel)
    {
      this.AssertGoodToUse(true, true);
      if (this.state != MimeReaderState.HeaderComplete)
        throw new InvalidOperationException(CtsResources.Strings.OperationNotValidInThisReaderState);
      if (topLevel)
      {
        if (this.currentHeaderConsumed)
          return false;
        if (this.currentChild == null)
        {
          this.currentChild = this.currentHeader.FirstChild;
        }
        else
        {
          this.currentGrandChild = (MimeNode) null;
          this.currentChild = this.currentChild.NextSibling;
        }
        if (this.currentChild == null)
          this.currentHeaderConsumed = true;
        this.currentChildConsumed = false;
        this.currentGrandChild = (MimeNode) null;
        return this.currentChild != null;
      }
      if (this.currentChild == null)
        throw new InvalidOperationException(CtsResources.Strings.OperationNotValidInThisReaderState);
      if (this.currentChildConsumed)
        return false;
      this.currentGrandChild = this.currentGrandChild != null ? this.currentGrandChild.NextSibling : this.currentChild.FirstChild;
      if (this.currentGrandChild == null)
        this.currentChildConsumed = true;
      return this.currentGrandChild != null;
    }

    internal bool IsCurrentChildValid(bool topLevel)
    {
      if (!topLevel)
        return this.currentGrandChild != null;
      return this.currentChild != null;
    }

    public void CopyOuterContentTo(Stream stream)
    {
      this.AssertGoodToUse(false, true);
      if (stream == null)
        throw new ArgumentNullException(nameof(stream));
      if (!stream.CanWrite)
        throw new ArgumentException(CtsResources.Strings.StreamMustSupportWriting, nameof(stream));
      if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.InlineStart))
        throw new InvalidOperationException(CtsResources.Strings.OperationNotValidInThisReaderState);
      if (this.outerContentStream != null)
        throw new NotSupportedException(CtsResources.Strings.OnlyOneOuterContentPushStreamAllowed);
      this.outerContentStream = stream;
      this.outerContentDepth = this.depth;
    }

    public int ReadRawContent(byte[] buffer, int offset, int count)
    {
      this.AssertGoodToUse(true, true);
      MimeCommon.CheckBufferArguments(buffer, offset, count);
      if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.PartBody | MimeReaderState.InlineBody))
      {
        if (MimeReader.StateIsOneOf(this.state, MimeReaderState.PartEnd | MimeReaderState.InlineEnd))
          return 0;
        this.TryInitializeReadContent(false);
      }
      if (this.contentStream != null)
        throw new NotSupportedException(CtsResources.Strings.CannotReadContentWhileStreamIsActive);
      if (!this.readRawData)
        throw new NotSupportedException(CtsResources.Strings.CannotMixReadRawContentAndReadContent);
      int readCount;
      this.ReadPartData(buffer, offset, count, out readCount);
      return readCount;
    }

    public int ReadContent(byte[] buffer, int offset, int count)
    {
      this.AssertGoodToUse(true, true);
      MimeCommon.CheckBufferArguments(buffer, offset, count);
      if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.PartBody | MimeReaderState.InlineBody))
      {
        if (MimeReader.StateIsOneOf(this.state, MimeReaderState.PartEnd | MimeReaderState.InlineEnd))
          return 0;
        if (!this.TryInitializeReadContent(true))
          throw new MimeException(CtsResources.Strings.CannotDecodeContentStream);
      }
      if (this.contentStream != null)
        throw new NotSupportedException(CtsResources.Strings.CannotReadContentWhileStreamIsActive);
      if (this.readRawData)
        throw new NotSupportedException(CtsResources.Strings.CannotMixReadRawContentAndReadContent);
      int readCount;
      this.ReadPartData(buffer, offset, count, out readCount);
      return readCount;
    }

    public Stream GetContentReadStream()
    {
      Stream result;
      if (!this.TryGetContentReadStream(out result))
        throw new MimeException(CtsResources.Strings.CannotDecodeContentStream);
      return result;
    }

    public bool TryGetContentReadStream(out Stream result)
    {
      this.AssertGoodToUse(true, true);
      if (!this.TryInitializeReadContent(true))
      {
        result = (Stream) null;
        return false;
      }
      this.contentStream = (Stream) new MimeReader.ContentReadStream(this);
      result = this.contentStream;
      return true;
    }

    public Stream GetRawContentReadStream()
    {
      this.AssertGoodToUse(true, true);
      this.TryInitializeReadContent(false);
      this.contentStream = (Stream) new MimeReader.ContentReadStream(this);
      return this.contentStream;
    }

    private bool TryInitializeReadContent(bool decode)
    {
      if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.EndOfHeaders | MimeReaderState.InlineStart))
      {
        if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderComplete))
          throw new InvalidOperationException(CtsResources.Strings.OperationNotValidInThisReaderState);
        if (this.state == MimeReaderState.PartStart && this.enableReadingOuterContent)
        {
          this.parser.SetStreamMode();
          this.state = MimeReaderState.PartBody;
          this.decoder = (Encoders.ByteEncoder) null;
          this.readRawData = !decode;
          return true;
        }
        while (this.TryReachNextState())
        {
          if (this.state == MimeReaderState.EndOfHeaders)
            goto label_8;
        }
        return false;
      }
label_8:
      MimeReaderState mimeReaderState1 = this.state;
      MimeReaderState mimeReaderState2;
      if (this.state == MimeReaderState.EndOfHeaders)
      {
        this.parser.SetContentType(MajorContentType.Other, new MimeString());
        mimeReaderState2 = MimeReaderState.PartBody;
      }
      else
        mimeReaderState2 = MimeReaderState.InlineBody;
      if (decode)
      {
        ContentTransferEncoding transferEncoding = this.parser.TransferEncoding;
        switch (transferEncoding)
        {
          case ContentTransferEncoding.SevenBit:
          case ContentTransferEncoding.EightBit:
          case ContentTransferEncoding.Binary:
            this.decoder = (Encoders.ByteEncoder) null;
            break;
          default:
            this.decoder = MimePart.CreateDecoder(transferEncoding);
            if (this.decoder == null)
              return false;
            break;
        }
        this.readRawData = false;
      }
      else
      {
        this.decoder = (Encoders.ByteEncoder) null;
        this.readRawData = true;
      }
      this.state = mimeReaderState2;
      return true;
    }

    public MimeReader GetEmbeddedMessageReader()
    {
      this.AssertGoodToUse(true, true);
      if (this.state != MimeReaderState.EndOfHeaders)
      {
        if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderComplete))
          throw new InvalidOperationException(CtsResources.Strings.OperationNotValidInThisReaderState);
        do
        {
          this.TryReachNextState();
        }
        while (this.state != MimeReaderState.EndOfHeaders);
      }
      if (this.currentPartMajorType != MajorContentType.MessageRfc822 || !this.parser.IsMime)
        throw new InvalidOperationException(CtsResources.Strings.CurrentPartIsNotEmbeddedMessage);
      this.parser.SetContentType(MajorContentType.MessageRfc822, new MimeString());
      this.TryReachNextState();
      this.childReader = new MimeReader(this);
      return this.childReader;
    }

    public void ResetComplianceStatus()
    {
      this.parser.ComplianceStatus = MimeComplianceStatus.Compliant;
    }

    private int TrimEndOfLine(int offset, int length)
    {
      if (length >= 1 && (int) this.data[offset + length - 1] == 10)
      {
        --length;
        while (length >= 1 && (int) this.data[offset + length - 1] == 13)
          --length;
      }
      return length;
    }

    private void ParseAndCheckSize()
    {
      this.currentToken = this.parser.Parse(this.data, this.dataOffset, this.dataOffset + this.dataCount, this.dataEOF);
      if (this.parser.Position > this.MimeLimits.MaxSize)
        throw new MimeException(CtsResources.Strings.InputStreamTooLong(this.parser.Position, this.MimeLimits.MaxSize));
    }

    private void CheckHeaderBytesLimits()
    {
      this.cumulativeHeaderBytes += (int) this.currentToken.Length;
      if (this.cumulativeHeaderBytes > this.MimeLimits.MaxHeaderBytes)
        throw new MimeException(CtsResources.Strings.TooManyHeaderBytes(this.cumulativeHeaderBytes, this.MimeLimits.MaxHeaderBytes));
      if (this.currentToken.Id == MimeTokenId.Header)
        this.currentTextHeaderBytes = 0;
      this.currentTextHeaderBytes += (int) this.currentToken.Length;
      Type type = Header.TypeFromHeaderId(this.currentHeaderId);
      if ((type == typeof (TextHeader) || type == typeof (AsciiTextHeader)) && this.currentTextHeaderBytes > this.MimeLimits.MaxTextValueBytesPerValue)
        throw new MimeException(CtsResources.Strings.TooManyTextValueBytes(this.currentTextHeaderBytes, this.MimeLimits.MaxTextValueBytesPerValue));
    }

    private void CheckPartsLimit()
    {
      if (++this.partCount > this.MimeLimits.MaxParts)
        throw new MimeException(CtsResources.Strings.TooManyParts(this.partCount, this.MimeLimits.MaxParts));
      if (this.PartDepth > this.MimeLimits.MaxPartDepth)
        throw new MimeException(CtsResources.Strings.PartNestingTooDeep(this.PartDepth, this.MimeLimits.MaxPartDepth));
      if (this.embeddedDepth > this.MimeLimits.MaxEmbeddedDepth)
        throw new MimeException(CtsResources.Strings.EmbeddedNestingTooDeep(this.embeddedDepth, this.MimeLimits.MaxEmbeddedDepth));
    }

    internal string ReadRecipientEmail(bool topLevel)
    {
      string str = (string) null;
      MimeRecipient mimeRecipient;
      if (topLevel)
      {
        mimeRecipient = this.currentChild as MimeRecipient;
        if (mimeRecipient == null)
          throw new NotSupportedException(CtsResources.Strings.CurrentAddressIsGroupAndCannotHaveEmail);
      }
      else
        mimeRecipient = this.currentGrandChild as MimeRecipient;
      if (mimeRecipient != null)
        str = mimeRecipient.Email;
      return str;
    }

    internal bool TryReadDisplayName(bool topLevel, DecodingOptions decodingOptions, out DecodingResults decodingResults, out string displayName)
    {
      return (!topLevel ? this.currentGrandChild as AddressItem : this.currentChild as AddressItem).TryGetDisplayName(decodingOptions, out decodingResults, out displayName);
    }

    internal string ReadParameterName()
    {
      return (this.currentChild as MimeParameter).Name;
    }

    internal bool TryReadParameterValue(DecodingOptions decodingOptions, out DecodingResults decodingResults, out string value)
    {
      return (this.currentChild as MimeParameter).TryGetValue(decodingOptions, out decodingResults, out value);
    }

    internal int AddMoreData(byte[] buffer, int offset, int length, bool endOfFile)
    {
      this.CompactDataBuffer();
      int count;
      if (length != 0)
      {
        count = Math.Min(length, this.data.Length - (this.dataOffset + this.dataCount) - 2);
        Buffer.BlockCopy((Array) buffer, offset, (Array) this.data, this.dataOffset + this.dataCount, count);
        length -= count;
        this.dataCount += count;
      }
      else
        count = 0;
      this.dataEOF = length == 0 && endOfFile;
      return count;
    }

    private bool ReadMoreData()
    {
      this.CompactDataBuffer();
      int offset = this.dataOffset + this.dataCount;
      int num = this.mimeStream.Read(this.data, offset, this.data.Length - offset - 2);
      if (num == 0)
      {
        if (!this.eofMeansEndOfFile)
          return false;
        this.dataEOF = true;
        return true;
      }
      this.dataCount += num;
      return true;
    }

    private void CompactDataBuffer()
    {
      if (this.dataCount == 0)
      {
        this.dataOffset = 0;
      }
      else
      {
        if (this.data.Length - this.dataOffset + this.dataCount >= this.data.Length / 2)
          return;
        Buffer.BlockCopy((Array) this.data, this.dataOffset, (Array) this.data, 0, this.dataCount);
        this.dataOffset = 0;
      }
    }

    internal void Write(byte[] buffer, int offset, int length)
    {
      if (this.dataEOF)
        throw new InvalidOperationException(CtsResources.Strings.CannotWriteAfterFlush);
      while (true)
      {
        while (this.currentToken.Id == MimeTokenId.None && this.state != MimeReaderState.Start)
        {
          this.ParseAndCheckSize();
          if (this.currentToken.Id == MimeTokenId.None)
          {
            if (length == 0)
              return;
            int num = this.AddMoreData(buffer, offset, length, false);
            offset += num;
            length -= num;
          }
          else
            break;
        }
        this.HandleTokenInPushMode();
      }
    }

    internal void Flush()
    {
      if (this.dataEOF)
        return;
      this.dataEOF = true;
      do
      {
        if (this.currentToken.Id == MimeTokenId.None && this.state != MimeReaderState.Start)
          this.ParseAndCheckSize();
        this.HandleTokenInPushMode();
      }
      while (this.state != MimeReaderState.End);
    }

    private void HandleTokenInPushMode()
    {
      if (MimeReader.StateIsOneOf(this.state, MimeReaderState.PartBody | MimeReaderState.InlineBody) && !this.skipPart && (this.currentToken.Id == MimeTokenId.PartData || this.state == MimeReaderState.InlineBody && (this.currentToken.Id == MimeTokenId.InlineStart || this.currentToken.Id == MimeTokenId.InlineEnd)))
        this.handler.PartContent(this.data, this.dataOffset, (int) this.currentToken.Length);
      if (!this.RunStateMachine())
        return;
      switch (this.state)
      {
        case MimeReaderState.InlineEnd:
        case MimeReaderState.PartEnd:
          this.handler.PartEnd();
          break;
        case MimeReaderState.End:
          this.handler.EndOfFile();
          break;
        case MimeReaderState.InlineStart:
        case MimeReaderState.PartStart:
          this.skipPart = false;
          this.skipHeaders = false;
          PartParseOptionInternal partParseOption;
          Stream outerContentWriteStream;
          this.handler.PartStart(this.state == MimeReaderState.InlineStart, this.state == MimeReaderState.InlineStart ? this.InlineFileName : (string) null, out partParseOption, out outerContentWriteStream);
          if (outerContentWriteStream != null)
          {
            if (this.outerContentStream != null)
              throw new NotSupportedException(CtsResources.Strings.MimeHandlerErrorMoreThanOneOuterContentPushStream);
            this.outerContentStream = outerContentWriteStream;
            this.outerContentDepth = this.depth;
          }
          if (partParseOption == PartParseOptionInternal.Skip)
          {
            this.skipPart = true;
            this.parser.SetStreamMode();
            this.state = this.state == MimeReaderState.InlineStart ? MimeReaderState.InlineBody : MimeReaderState.PartBody;
          }
          else if (partParseOption == PartParseOptionInternal.ParseSkipHeaders)
            this.skipHeaders = true;
          else if (partParseOption == PartParseOptionInternal.ParseRawOuterContent)
          {
            this.parser.SetStreamMode();
            this.state = this.state == MimeReaderState.InlineStart ? MimeReaderState.InlineBody : MimeReaderState.PartBody;
          }
          if (this.skipPart || this.state != MimeReaderState.InlineStart)
            break;
          goto case MimeReaderState.EndOfHeaders;
        case MimeReaderState.EndOfHeaders:
          PartContentParseOptionInternal partContentParseOption;
          this.handler.EndOfHeaders(this.parser.IsMime ? this.currentPartContentType : (this.state == MimeReaderState.InlineStart ? "application/octet-stream" : "text/plain"), this.parser.TransferEncoding, out partContentParseOption);
          if (partContentParseOption == PartContentParseOptionInternal.Skip)
          {
            if (this.state != MimeReaderState.InlineStart)
              this.parser.SetContentType(MajorContentType.Other, new MimeString());
            this.state = this.state == MimeReaderState.InlineStart ? MimeReaderState.InlineBody : MimeReaderState.PartBody;
            this.skipPart = true;
            break;
          }
          if (partContentParseOption == PartContentParseOptionInternal.ParseRawContent)
          {
            if (this.state != MimeReaderState.InlineStart)
              this.parser.SetContentType(MajorContentType.Other, new MimeString());
            this.state = this.state == MimeReaderState.InlineStart ? MimeReaderState.InlineBody : MimeReaderState.PartBody;
            break;
          }
          if (partContentParseOption == PartContentParseOptionInternal.ParseEmbeddedMessage)
          {
            if (this.currentPartMajorType == MajorContentType.MessageRfc822)
              break;
            throw new NotSupportedException(CtsResources.Strings.MimeHandlerErrorNotEmbeddedMessage);
          }
          if (this.currentPartMajorType == MajorContentType.MessageRfc822)
            this.parser.SetContentType(MajorContentType.Other, new MimeString());
          if (this.currentPartMajorType == MajorContentType.Multipart && this.parser.IsMime)
            break;
          this.state = this.state == MimeReaderState.InlineStart ? MimeReaderState.InlineBody : MimeReaderState.PartBody;
          break;
        case MimeReaderState.HeaderStart:
          if (!this.skipHeaders)
          {
            HeaderParseOptionInternal headerParseOption;
            this.handler.HeaderStart(this.currentHeaderId, this.currentHeaderName, out headerParseOption);
            this.skipHeader = headerParseOption == HeaderParseOptionInternal.Skip;
            if (this.skipHeader)
              break;
            this.createHeader = true;
            break;
          }
          this.skipHeader = true;
          break;
        case MimeReaderState.HeaderComplete:
          if (this.currentHeader == null || this.skipHeader)
            break;
          this.handler.Header(this.currentHeader);
          break;
      }
    }

    private bool TrySkipToNextPartBoundary(bool stopAtPartEnd)
    {
      while (this.state != MimeReaderState.End)
      {
        if (!this.TryReachNextState())
          return false;
        if (MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.InlineStart) || stopAtPartEnd && MimeReader.StateIsOneOf(this.state, MimeReaderState.PartEnd | MimeReaderState.InlineEnd))
          break;
      }
      return true;
    }

    internal bool TryReachNextState()
    {
      while (this.state != MimeReaderState.End)
      {
        if (this.currentToken.Id == MimeTokenId.None)
        {
          if (this.state == MimeReaderState.Start)
          {
            this.RunStateMachine();
            break;
          }
          this.ParseAndCheckSize();
          if (this.currentToken.Id == MimeTokenId.None)
          {
            if (this.mimeStream == null || !this.ReadMoreData())
            {
              this.dataExhausted = true;
              return false;
            }
            continue;
          }
        }
        if (this.RunStateMachine())
          break;
      }
      this.dataExhausted = false;
      return true;
    }

    private bool RunStateMachine()
    {
      switch (this.state)
      {
        case MimeReaderState.InlineJunk:
          if (this.currentToken.Id == MimeTokenId.PartData)
          {
            this.ConsumeCurrentToken();
            break;
          }
          if (this.currentToken.Id == MimeTokenId.InlineStart)
          {
            ++this.depth;
            this.StartPart();
            this.ParseInlineFileName();
            this.state = MimeReaderState.InlineStart;
            return true;
          }
          if (this.currentToken.Id == MimeTokenId.EmbeddedEnd && this.parseEmbeddedMessages)
          {
            this.ConsumeCurrentToken();
            this.EndPart();
            this.state = MimeReaderState.PartEnd;
            return true;
          }
          this.state = MimeReaderState.End;
          return true;
        case MimeReaderState.EmbeddedEnd:
          if (this.currentToken.Id == MimeTokenId.EmbeddedEnd)
          {
            if (this.cleanupDepth == 0)
            {
              this.ConsumeCurrentToken();
              this.EndPart();
              this.state = MimeReaderState.PartEnd;
              return true;
            }
            --this.cleanupDepth;
          }
          else if (this.currentToken.Id == MimeTokenId.InlineStart || this.currentToken.Id == MimeTokenId.NestedStart)
            ++this.cleanupDepth;
          else if (this.currentToken.Id == MimeTokenId.InlineEnd || this.currentToken.Id == MimeTokenId.NestedEnd)
            --this.cleanupDepth;
          this.ConsumeCurrentToken();
          break;
        case MimeReaderState.InlineBody:
          if (this.currentToken.Id == MimeTokenId.InlineEnd)
          {
            this.ConsumeCurrentToken();
            this.EndPart();
            this.state = MimeReaderState.InlineEnd;
            return true;
          }
          this.ConsumeCurrentToken();
          break;
        case MimeReaderState.InlineEnd:
          --this.depth;
          this.state = MimeReaderState.InlineJunk;
          goto case MimeReaderState.InlineJunk;
        case MimeReaderState.PartEpilogue:
          if (this.currentToken.Id == MimeTokenId.PartData)
          {
            this.ConsumeCurrentToken();
            break;
          }
          this.EndPart();
          this.state = MimeReaderState.PartEnd;
          return true;
        case MimeReaderState.PartEnd:
          if (this.currentToken.Id == MimeTokenId.NestedNext)
          {
            this.StartPart();
            this.ConsumeCurrentToken();
            this.state = MimeReaderState.PartStart;
            return true;
          }
          if (this.currentToken.Id == MimeTokenId.NestedEnd)
          {
            this.ConsumeCurrentToken();
            --this.depth;
            this.state = MimeReaderState.PartEpilogue;
            break;
          }
          if (this.currentToken.Id == MimeTokenId.InlineStart)
          {
            this.StartPart();
            this.ParseInlineFileName();
            this.state = MimeReaderState.InlineStart;
            return true;
          }
          if (this.currentToken.Id == MimeTokenId.EmbeddedEnd && this.parseEmbeddedMessages)
          {
            this.ConsumeCurrentToken();
            --this.depth;
            --this.embeddedDepth;
            this.EndPart();
            return true;
          }
          --this.depth;
          this.state = MimeReaderState.End;
          return true;
        case MimeReaderState.InlineStart:
          this.state = MimeReaderState.InlineBody;
          return true;
        case MimeReaderState.EndOfHeaders:
          if (this.currentToken.Id == MimeTokenId.EmbeddedStart)
          {
            this.ConsumeCurrentToken();
            if (this.parseEmbeddedMessages)
            {
              ++this.depth;
              ++this.embeddedDepth;
              this.StartPart();
              this.state = MimeReaderState.PartStart;
              return true;
            }
            this.state = MimeReaderState.Embedded;
            return true;
          }
          if (this.currentToken.Id == MimeTokenId.NestedStart)
          {
            ++this.depth;
            this.StartPart();
            this.ConsumeCurrentToken();
            this.state = MimeReaderState.PartStart;
            return true;
          }
          if (this.currentToken.Id == MimeTokenId.PartData)
          {
            if (this.parser.ContentType == MajorContentType.Multipart)
            {
              this.state = MimeReaderState.PartPrologue;
              goto case MimeReaderState.PartPrologue;
            }
            else
            {
              this.state = MimeReaderState.PartBody;
              return true;
            }
          }
          else
          {
            this.EndPart();
            this.state = MimeReaderState.PartEnd;
            return true;
          }
        case MimeReaderState.PartPrologue:
          if (this.currentToken.Id == MimeTokenId.NestedStart)
          {
            ++this.depth;
            this.StartPart();
            this.ConsumeCurrentToken();
            this.state = MimeReaderState.PartStart;
            return true;
          }
          if (this.currentToken.Id == MimeTokenId.PartData)
          {
            this.ConsumeCurrentToken();
            break;
          }
          this.EndPart();
          this.state = MimeReaderState.PartEnd;
          return true;
        case MimeReaderState.PartBody:
          if (this.currentToken.Id == MimeTokenId.PartData)
          {
            this.ConsumeCurrentToken();
            break;
          }
          this.EndPart();
          this.state = MimeReaderState.PartEnd;
          return true;
        case MimeReaderState.Start:
          ++this.depth;
          this.StartPart();
          this.state = MimeReaderState.PartStart;
          return true;
        case MimeReaderState.PartStart:
        case MimeReaderState.HeaderComplete:
          if (this.currentToken.Id == MimeTokenId.Header)
          {
            this.StartHeader();
            this.state = MimeReaderState.HeaderStart;
            return true;
          }
          this.ConsumeCurrentToken();
          this.state = MimeReaderState.EndOfHeaders;
          return true;
        case MimeReaderState.HeaderStart:
          this.CreateHeader();
          this.ContinueHeader();
          if (this.parser.IsHeaderComplete)
          {
            this.EndHeader();
            this.ConsumeCurrentToken();
            this.state = MimeReaderState.HeaderComplete;
            return true;
          }
          this.ConsumeCurrentToken();
          this.state = MimeReaderState.HeaderIncomplete;
          break;
        case MimeReaderState.HeaderIncomplete:
          if (this.currentToken.Id == MimeTokenId.HeaderContinuation)
          {
            this.ContinueHeader();
            this.ConsumeCurrentToken();
            if (this.parser.IsHeaderComplete)
            {
              this.EndHeader();
              this.state = MimeReaderState.HeaderComplete;
              return true;
            }
            break;
          }
          this.EndHeader();
          this.state = MimeReaderState.HeaderComplete;
          return true;
        default:
          throw new InvalidOperationException();
      }
      return false;
    }

    private void ConsumeCurrentToken()
    {
      if ((int) this.currentToken.Length != 0)
      {
        if (this.outerContentStream != null)
          this.outerContentStream.Write(this.data, this.dataOffset, (int) this.currentToken.Length);
        this.currentLineTerminationState = MimeCommon.AdvanceLineTerminationState(this.currentLineTerminationState, this.data, this.dataOffset, (int) this.currentToken.Length);
        this.parser.ReportConsumedData((int) this.currentToken.Length);
        this.dataOffset += (int) this.currentToken.Length;
        this.dataCount -= (int) this.currentToken.Length;
        this.currentToken.Length = (short) 0;
      }
      this.currentToken.Id = MimeTokenId.None;
    }

    private void StartPart()
    {
      this.CheckPartsLimit();
      this.currentPartMajorType = MajorContentType.Other;
      this.currentPartContentType = (string) null;
      this.currentPartContentTransferEncoding = ContentTransferEncoding.SevenBit;
      this.enableReadingOuterContent = false;
      this.currentHeader = (Header) null;
      this.createHeader = false;
      this.inlineFileName = new MimeString();
      this.decoder = (Encoders.ByteEncoder) null;
      this.contentStream = (Stream) null;
    }

    private void EndPart()
    {
      if (this.outerContentStream == null || this.depth != this.outerContentDepth)
        return;
      this.outerContentStream.Flush();
      this.outerContentStream = (Stream) null;
    }

    private void ParseInlineFileName()
    {
      this.currentPartContentType = "application/octet-stream";
      this.currentPartContentTransferEncoding = this.parser.InlineFormat;
      if (this.parser.InlineFormat == ContentTransferEncoding.UUEncode)
      {
        int offset1 = this.dataOffset + 6;
        while (offset1 < this.dataOffset + (int) this.currentToken.Length && ((int) this.data[offset1] >= 48 && (int) this.data[offset1] <= 55))
          ++offset1;
        int offset2 = offset1 + MimeScan.SkipLwsp(this.data, offset1, this.dataOffset + (int) this.currentToken.Length - offset1);
        int count = this.TrimEndOfLine(offset2, this.dataOffset + (int) this.currentToken.Length - offset2);
        this.inlineFileName = new MimeString(this.data, offset2, count);
      }
      else
        this.inlineFileName = new MimeString();
    }

    private void StartHeader()
    {
      if (++this.headerCount > this.MimeLimits.MaxHeaders)
        throw new MimeException(CtsResources.Strings.TooManyHeaders(this.headerCount, this.MimeLimits.MaxHeaders));
      this.currentHeaderId = this.parser.HeaderNameLength == 0 ? HeaderId.Unknown : Header.GetHeaderId(this.data, this.dataOffset, this.parser.HeaderNameLength);
      this.currentHeaderName = this.parser.HeaderNameLength == 0 ? (string) null : Internal.ByteString.BytesToString(this.data, this.dataOffset, this.parser.HeaderNameLength, false);
      this.currentHeader = (Header) null;
      bool flag = false;
      if (this.currentHeaderId == HeaderId.ContentType || this.currentHeaderId == HeaderId.ContentTransferEncoding || this.currentHeaderId == HeaderId.MimeVersion)
        flag = true;
      else if (this.MimeLimits.MaxAddressItemsPerHeader < int.MaxValue || this.MimeLimits.MaxParametersPerHeader < int.MaxValue || this.MimeLimits.MaxTextValueBytesPerValue < int.MaxValue)
      {
        Type type = Header.TypeFromHeaderId(this.currentHeaderId);
        if (type == typeof (AddressHeader) && (this.MimeLimits.MaxParametersPerHeader < int.MaxValue || this.MimeLimits.MaxAddressItemsPerHeader < int.MaxValue) || (type == typeof (ContentTypeHeader) || type == typeof (ContentDispositionHeader)) && (this.MimeLimits.MaxParametersPerHeader < int.MaxValue || this.MimeLimits.MaxAddressItemsPerHeader < int.MaxValue))
          flag = true;
      }
      this.createHeader = flag;
    }

    private void CreateHeader()
    {
      this.currentHeader = !this.createHeader || this.parser.HeaderNameLength == 0 ? (Header) null : (this.currentHeaderId == HeaderId.Unknown ? Header.CreateGeneralHeader(this.currentHeaderName) : Header.Create(this.currentHeaderName, this.currentHeaderId));
      this.currentHeaderEmpty = true;
    }

    private void ContinueHeader()
    {
      this.CheckHeaderBytesLimits();
      if (this.currentHeader == null)
        return;
      int offset = this.dataOffset + this.parser.HeaderDataOffset;
      int length = (int) this.currentToken.Length - this.parser.HeaderDataOffset;
      int num1 = this.TrimEndOfLine(offset, length);
      if (this.currentHeaderEmpty)
      {
        int num2 = MimeScan.SkipLwsp(this.data, offset, num1);
        offset += num2;
        num1 -= num2;
      }
      if (num1 == 0)
        return;
      this.currentHeaderEmpty = false;
      this.currentHeader.AppendLine(MimeString.CopyData(this.data, offset, num1), false);
    }

    private void EndHeader()
    {
      if (this.currentHeader == null)
        return;
      if (this.currentHeader is ComplexHeader)
      {
        if (this.MimeLimits.MaxParametersPerHeader < int.MaxValue || this.MimeLimits.MaxTextValueBytesPerValue < int.MaxValue)
          this.currentHeader.CheckChildrenLimit(this.MimeLimits.MaxParametersPerHeader, this.MimeLimits.MaxTextValueBytesPerValue);
      }
      else if (this.currentHeader is AddressHeader && (this.MimeLimits.MaxAddressItemsPerHeader < int.MaxValue || this.MimeLimits.MaxTextValueBytesPerValue < int.MaxValue))
        this.currentHeader.CheckChildrenLimit(this.MimeLimits.MaxAddressItemsPerHeader, this.MimeLimits.MaxTextValueBytesPerValue);
      if (this.currentHeader.HeaderId == HeaderId.MimeVersion && this.PartDepth == 1)
        this.parser.SetMIME();
      else if (this.currentHeader.HeaderId == HeaderId.ContentTransferEncoding)
      {
        if (this.inferMime && this.PartDepth == 1)
          this.parser.SetMIME();
        ContentTransferEncoding encodingType = MimePart.GetEncodingType(this.currentHeader.FirstRawToken);
        this.parser.SetTransferEncoding(encodingType);
        this.currentPartContentTransferEncoding = encodingType;
      }
      else
      {
        if (this.currentHeader.HeaderId != HeaderId.ContentType)
          return;
        MajorContentType contentType = MajorContentType.Other;
        string str = (string) null;
        MimeString boundaryValue = new MimeString();
        ContentTypeHeader contentTypeHeader = this.currentHeader as ContentTypeHeader;
        if (contentTypeHeader != null)
        {
          if (contentTypeHeader.IsMultipart)
          {
            MimeParameter mimeParameter = contentTypeHeader["boundary"];
            if (mimeParameter != null)
            {
              byte[] rawValue = mimeParameter.RawValue;
              int count = 0;
              if (rawValue != null && (count = rawValue.Length) != 0)
              {
                while (count != 0 && MimeScan.IsLWSP(rawValue[count - 1]))
                  --count;
                if (count != 0 && count <= 994)
                {
                  if (this.FixBadMimeBoundary)
                  {
                    int index = 0;
                    if (count == rawValue.Length && count <= 70)
                    {
                      while (index < count && MimeScan.IsBChar(rawValue[index]))
                        ++index;
                    }
                    if (index != count)
                    {
                      this.parser.ComplianceStatus |= MimeComplianceStatus.InvalidBoundary;
                      mimeParameter.RawValue = ContentTypeHeader.CreateBoundary();
                    }
                  }
                  contentType = MajorContentType.Multipart;
                  boundaryValue = new MimeString(rawValue, 0, count);
                }
              }
              if (rawValue == null || count == 0 || count > 994)
              {
                this.parser.ComplianceStatus |= MimeComplianceStatus.InvalidBoundary;
                contentTypeHeader.Value = "text/plain";
              }
            }
            else
            {
              this.parser.ComplianceStatus |= MimeComplianceStatus.MissingBoundaryParameter;
              contentTypeHeader.Value = "text/plain";
            }
          }
          else if (contentTypeHeader.IsEmbeddedMessage)
            contentType = MajorContentType.MessageRfc822;
          else if (contentTypeHeader.IsAnyMessage)
            contentType = MajorContentType.Message;
          str = contentTypeHeader.Value;
        }
        if (this.inferMime && this.PartDepth == 1)
          this.parser.SetMIME();
        this.currentPartMajorType = contentType;
        this.currentPartContentType = str;
        if (contentType == MajorContentType.Multipart || this.parseEmbeddedMessages)
          this.parser.SetContentType(contentType, boundaryValue);
        else
          this.parser.SetContentType(MajorContentType.Other, new MimeString());
      }
    }

    internal bool ReadPartData(byte[] buffer, int offset, int count, out int readCount)
    {
      readCount = 0;
      this.dataExhausted = false;
      bool completed = true;
      while (count != 0)
      {
        if (this.currentToken.Id == MimeTokenId.None)
        {
          this.ParseAndCheckSize();
          if (this.currentToken.Id == MimeTokenId.None)
          {
            if (this.mimeStream == null || !this.ReadMoreData())
            {
              this.dataExhausted = true;
              return false;
            }
            continue;
          }
        }
        int inputUsed;
        int outputUsed;
        if (this.currentToken.Id != MimeTokenId.PartData && this.currentToken.Id != MimeTokenId.InlineStart && this.currentToken.Id != MimeTokenId.InlineEnd || this.state == MimeReaderState.PartBody && this.currentToken.Id == MimeTokenId.InlineStart)
        {
          if (this.decoder != null)
          {
            this.decoder.Convert(this.data, 0, 0, buffer, offset, count, true, out inputUsed, out outputUsed, out completed);
            count -= outputUsed;
            offset += outputUsed;
            readCount += outputUsed;
            if (!completed)
              break;
          }
          this.EndPart();
          this.state = MimeReaderState.PartEnd;
          break;
        }
        if (this.decoder != null)
        {
          this.decoder.Convert(this.data, this.dataOffset, (int) this.currentToken.Length, buffer, offset, count, this.currentToken.Id == MimeTokenId.InlineEnd, out inputUsed, out outputUsed, out completed);
          count -= outputUsed;
          offset += outputUsed;
          readCount += outputUsed;
        }
        else
        {
          inputUsed = outputUsed = Math.Min(count, (int) this.currentToken.Length);
          if (outputUsed != 0)
          {
            if (buffer != null)
            {
              Buffer.BlockCopy((Array) this.data, this.dataOffset, (Array) buffer, offset, outputUsed);
              count -= outputUsed;
              offset += outputUsed;
            }
            readCount += outputUsed;
          }
        }
        if (inputUsed != 0)
        {
          if (this.outerContentStream != null)
            this.outerContentStream.Write(this.data, this.dataOffset, inputUsed);
          this.currentLineTerminationState = MimeCommon.AdvanceLineTerminationState(this.currentLineTerminationState, this.data, this.dataOffset, inputUsed);
          this.parser.ReportConsumedData(inputUsed);
          this.dataOffset += inputUsed;
          this.dataCount -= inputUsed;
          this.currentToken.Length -= (short) inputUsed;
        }
        if ((int) this.currentToken.Length == 0)
        {
          if (this.currentToken.Id == MimeTokenId.InlineEnd)
          {
            if (completed)
            {
              this.EndPart();
              this.currentToken.Id = MimeTokenId.None;
              this.state = MimeReaderState.InlineEnd;
              break;
            }
            break;
          }
          this.currentToken.Id = MimeTokenId.None;
        }
        else
          break;
      }
      return true;
    }

    internal static bool StateIsOneOf(MimeReaderState state, MimeReaderState set)
    {
      return (state & set) != (MimeReaderState) 0;
    }

    internal void AssertGoodToUse(bool pullModeOnly, bool noEmbeddedReader)
    {
      if (this.parser == null)
        throw new ObjectDisposedException("MimeReader");
      if (pullModeOnly && this.mimeStream == null)
        throw new ObjectDisposedException("MimeReader");
      if (noEmbeddedReader && this.childReader != null)
        throw new InvalidOperationException(CtsResources.Strings.EmbeddedMessageReaderNeedsToBeClosedFirst);
    }

    internal void SetEofMeansEndOfFile(bool eofMeansEndOfFile)
    {
      this.eofMeansEndOfFile = eofMeansEndOfFile;
    }

    internal bool TryReadNextPart()
    {
      this.AssertGoodToUse(false, true);
      if (!this.TrySkipToNextPartBoundary(false))
        return false;
      return MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.InlineStart);
    }

    internal bool TryReadFirstChildPart()
    {
      this.AssertGoodToUse(false, true);
      if (this.state == MimeReaderState.InlineStart)
        return false;
      if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.Start | MimeReaderState.EndOfHeaders | MimeReaderState.PartPrologue))
      {
        if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderIncomplete | MimeReaderState.HeaderComplete))
          throw new InvalidOperationException(CtsResources.Strings.OperationNotValidInThisReaderState);
        while (this.TryReachNextState())
        {
          if (this.state == MimeReaderState.EndOfHeaders)
            goto label_8;
        }
        return false;
      }
label_8:
      if (this.state == MimeReaderState.EndOfHeaders && !this.IsMultipart && (!this.IsEmbeddedMessage || !this.parseEmbeddedMessages) || !this.TrySkipToNextPartBoundary(true))
        return false;
      return this.state == MimeReaderState.PartStart;
    }

    internal bool TryReadNextSiblingPart()
    {
      this.AssertGoodToUse(false, true);
      if (this.state == MimeReaderState.End)
        return false;
      if (MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.HeaderStart | MimeReaderState.HeaderIncomplete | MimeReaderState.HeaderComplete | MimeReaderState.EndOfHeaders))
      {
        this.createHeader = false;
        while (this.state != MimeReaderState.EndOfHeaders)
        {
          if (!this.TryReachNextState())
            return false;
        }
        this.parser.SetContentType(MajorContentType.Other, new MimeString());
      }
      if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.Start | MimeReaderState.PartEnd | MimeReaderState.InlineEnd))
      {
        int num = this.depth;
        while (this.TrySkipToNextPartBoundary(true))
        {
          if (this.depth <= num && MimeReader.StateIsOneOf(this.state, MimeReaderState.PartEnd | MimeReaderState.InlineEnd))
            goto label_13;
        }
        return false;
      }
label_13:
      if (!this.TrySkipToNextPartBoundary(true))
        return false;
      return MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.InlineStart);
    }

    internal HeaderList TryReadHeaderList()
    {
      this.AssertGoodToUse(false, true);
      if (!MimeReader.StateIsOneOf(this.state, MimeReaderState.PartStart | MimeReaderState.InlineStart) && (!MimeReader.StateIsOneOf(this.state, MimeReaderState.HeaderStart | MimeReaderState.HeaderIncomplete | MimeReaderState.HeaderComplete) || this.headerList == null))
        throw new InvalidOperationException(CtsResources.Strings.OperationNotValidInThisReaderState);
      if (this.state == MimeReaderState.InlineStart)
        return new HeaderList((MimeNode) null);
      HeaderList headerList;
      if (this.headerList == null)
      {
        headerList = new HeaderList((MimeNode) null);
      }
      else
      {
        headerList = this.headerList;
        this.headerList = (HeaderList) null;
      }
      while (this.TryReachNextState())
      {
        if (this.state == MimeReaderState.HeaderStart)
          this.createHeader = true;
        else if (this.state == MimeReaderState.HeaderComplete && this.currentHeader != null)
          headerList.InternalAppendChild((MimeNode) this.currentHeader);
        if (this.state == MimeReaderState.EndOfHeaders)
          return headerList;
      }
      this.headerList = headerList;
      return (HeaderList) null;
    }

    internal Stream TryGetRawContentReadStream()
    {
      this.AssertGoodToUse(false, true);
      if (!this.TryInitializeReadContent(false))
        return (Stream) null;
      this.contentStream = (Stream) new MimeReader.ContentReadStream(this);
      return this.contentStream;
    }

    private class ContentReadStream : Stream
    {
      private MimeReader reader;

      public override bool CanRead => this.reader != null;

        public override bool CanWrite => false;

        public override bool CanSeek => false;

        public override long Length
      {
        get
        {
          throw new NotSupportedException();
        }
      }

      public override long Position
      {
        get
        {
          throw new NotSupportedException();
        }
        set
        {
          throw new NotSupportedException();
        }
      }

      public ContentReadStream(MimeReader reader)
      {
        this.reader = reader;
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        MimeCommon.CheckBufferArguments(buffer, offset, count);
        if (this.reader.contentStream != this)
          throw new NotSupportedException(CtsResources.Strings.StreamNoLongerValid);
        if (!MimeReader.StateIsOneOf(this.reader.state, MimeReaderState.PartBody | MimeReaderState.InlineBody))
        {
          if (MimeReader.StateIsOneOf(this.reader.state, MimeReaderState.PartEnd | MimeReaderState.InlineEnd))
            return 0;
          throw new NotSupportedException(CtsResources.Strings.StreamNoLongerValid);
        }
        int readCount;
        this.reader.ReadPartData(buffer, offset, count, out readCount);
        return readCount;
      }

      public override void Write(byte[] buffer, int offset, int count)
      {
        throw new NotSupportedException();
      }

      public override long Seek(long offset, SeekOrigin origin)
      {
        throw new NotSupportedException();
      }

      public override void Flush()
      {
        throw new NotSupportedException();
      }

      public override void SetLength(long value)
      {
        throw new NotSupportedException();
      }

      protected override void Dispose(bool disposing)
      {
        base.Dispose(disposing);
      }
    }
  }
}
