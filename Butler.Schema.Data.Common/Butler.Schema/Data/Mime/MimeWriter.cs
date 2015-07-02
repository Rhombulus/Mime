// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.MimeWriter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  public class MimeWriter : IDisposable
  {
    private static readonly byte[] terminateBoundarySuffix = new byte[4]
    {
      (byte) 45,
      (byte) 45,
      (byte) 13,
      (byte) 10
    };
    private static readonly byte[] endBoundarySuffix = MimeString.CrLf;
    private static readonly byte[] boundaryPrefix = new byte[2]
    {
      (byte) 45,
      (byte) 45
    };
    private ContentTransferEncoding contentTransferEncoding = ContentTransferEncoding.SevenBit;
    private bool forceMime = true;
    private MimeWriteState state;
    private Stream data;
    private Header lastHeader;
    private MimeWriter.WriterContentStream partContent;
    private Stream encodedPartContent;
    private MimeWriter.PartData[] partStack;
    private int partDepth;
    private int embeddedDepth;
    private MimeWriter.QueuedWrite[] writeQueue;
    private int writeCount;
    private EncodingOptions encodingOptions;
    private int bytesWritten;
    private bool headerValueWritten;
    private bool contentWritten;
    private MimeWriter.PartData currentPart;
    private MimeWriter.QueuedWrite currentWrite;
    private bool foundMimeVersion;
    private MimeWriter.WriterQueueStream shimStream;
    private byte[] scratchBuffer;
    private LineTerminationState lineTermination;

    public int PartDepth
    {
      get
      {
        this.AssertOpen();
        return this.partDepth;
      }
    }

    public int EmbeddedDepth
    {
      get
      {
        this.AssertOpen();
        return this.embeddedDepth;
      }
    }

    public int StreamOffset
    {
      get
      {
        this.AssertOpen();
        return this.bytesWritten;
      }
    }

    public EncodingOptions EncodingOptions
    {
      get
      {
        this.AssertOpen();
        return this.encodingOptions;
      }
    }

    public string PartBoundary
    {
      get
      {
        this.AssertOpen();
        string str = (string) null;
        switch (this.state)
        {
          case MimeWriteState.StartPart:
          case MimeWriteState.Headers:
            if (this.currentPart.IsMultipart)
            {
              str = Internal.ByteString.BytesToString(this.currentPart.Boundary, false);
              break;
            }
            break;
        }
        return str;
      }
    }

    public MimeWriter(Stream data)
      : this(data, true, MimeCommon.DefaultEncodingOptions)
    {
    }

    public MimeWriter(Stream data, bool forceMime, EncodingOptions encodingOptions)
    {
      if (data == null)
        throw new ArgumentNullException(nameof(data));
      if (!data.CanWrite)
        throw new ArgumentException("Stream must support writing", nameof(data));
      this.forceMime = forceMime;
      this.data = data;
      this.encodingOptions = encodingOptions;
      this.shimStream = new MimeWriter.WriterQueueStream(this);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.data == null)
        return;
      try
      {
        if (!disposing)
          return;
        while (this.partDepth != 0)
          this.EndPart();
        this.FlushWriteQueue();
        if (this.lineTermination == LineTerminationState.CRLF)
          return;
        if (this.lineTermination == LineTerminationState.CR)
          this.data.Write(MimeString.CrLf, 1, 1);
        else
          this.data.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
        this.lineTermination = LineTerminationState.CRLF;
      }
      finally
      {
        if (disposing)
        {
          if (this.encodedPartContent != null)
            this.encodedPartContent.Dispose();
          if (this.partContent != null)
            this.partContent.Dispose();
          this.data.Dispose();
        }
        this.state = MimeWriteState.Complete;
        this.encodedPartContent = (Stream) null;
        this.partContent = (MimeWriter.WriterContentStream) null;
        this.data = (Stream) null;
      }
    }

    public virtual void Close()
    {
      this.Dispose();
    }

    public void WritePart(MimeReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException(nameof(reader));
      this.AssertOpen();
      if (!MimeReader.StateIsOneOf(reader.ReaderState, MimeReaderState.PartStart | MimeReaderState.InlineStart))
        throw new InvalidOperationException(CtsResources.Strings.OperationNotValidInThisReaderState);
      this.StartPart();
      MimeHeaderReader headerReader = reader.HeaderReader;
      while (headerReader.ReadNextHeader())
        this.WriteHeader(headerReader);
      this.WriteContent(reader);
      this.EndPart();
    }

    public void WriteHeader(MimeHeaderReader reader)
    {
      this.AssertOpen();
      Header.ReadFrom(reader).WriteTo(this);
    }

    public void WriteAddress(MimeAddressReader reader)
    {
      this.AssertOpen();
      if (reader.IsGroup)
      {
        this.StartGroup(reader.DisplayName);
        MimeAddressReader groupRecipientReader = reader.GroupRecipientReader;
        while (groupRecipientReader.ReadNextAddress())
        {
          string displayName = groupRecipientReader.DisplayName;
          string email = groupRecipientReader.Email;
          if (displayName == null || email == null)
            throw new ExchangeDataException(CtsResources.Strings.AddressReaderIsNotPositionedOnAddress);
          this.WriteRecipient(displayName, email);
        }
        this.EndGroup();
      }
      else
        this.WriteRecipient(reader.DisplayName, reader.Email);
    }

    public void WriteParameter(MimeParameterReader reader)
    {
      this.AssertOpen();
      this.WriteParameter(reader.Name, reader.Value);
    }

    public void WriteContent(MimeReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException(nameof(reader));
      this.AssertOpen();
      if (this.contentWritten)
        throw new InvalidOperationException(CtsResources.Strings.ContentAlreadyWritten);
      using (Stream contentReadStream = reader.GetRawContentReadStream())
      {
        if (contentReadStream == null)
          return;
        using (Stream contentWriteStream = this.GetRawContentWriteStream())
          Internal.DataStorage.CopyStreamToStream(contentReadStream, contentWriteStream, long.MaxValue, ref this.scratchBuffer);
      }
    }

    public void StartHeader(string name)
    {
      this.AssertOpen();
      this.WriteHeader(Header.Create(name));
    }

    public void StartHeader(HeaderId headerId)
    {
      this.AssertOpen();
      this.WriteHeader(Header.Create(headerId));
    }

    public void WriteHeaderValue(string value)
    {
      this.AssertOpen();
      if (this.headerValueWritten)
        throw new InvalidOperationException(CtsResources.Strings.CannotWriteHeaderValueMoreThanOnce);
      if (this.lastHeader == null)
        throw new InvalidOperationException(CtsResources.Strings.CannotWriteHeaderValueHere);
      this.headerValueWritten = true;
      if (value == null)
        return;
      if (!(this.lastHeader is TextHeader))
        this.lastHeader.RawValue = Internal.ByteString.StringToBytes(value, this.encodingOptions.AllowUTF8);
      else
        this.lastHeader.Value = value;
    }

    public void WriteHeaderValue(DateTime value)
    {
      this.AssertOpen();
      if (this.headerValueWritten)
        throw new InvalidOperationException(CtsResources.Strings.CannotWriteHeaderValueMoreThanOnce);
      if (this.lastHeader == null)
        throw new InvalidOperationException(CtsResources.Strings.CannotWriteHeaderValueHere);
      this.headerValueWritten = true;
      TimeSpan timeZoneOffset = TimeSpan.Zero;
      DateTime utcDateTime = value.ToUniversalTime();
      if (value.Kind != DateTimeKind.Utc)
        timeZoneOffset = TimeZoneInfo.Local.GetUtcOffset(value);
      Header.WriteName((Stream) this.shimStream, this.lastHeader.Name, ref this.scratchBuffer);
      MimeStringLength currentLineLength = new MimeStringLength(0);
      DateHeader.WriteDateHeaderValue((Stream) this.shimStream, utcDateTime, timeZoneOffset, ref currentLineLength);
      this.lastHeader = (Header) null;
    }

    public void WriteContent(byte[] buffer, int offset, int count)
    {
      MimeCommon.CheckBufferArguments(buffer, offset, count);
      this.AssertOpen();
      if (this.contentWritten)
        throw new InvalidOperationException(CtsResources.Strings.ContentAlreadyWritten);
      if (this.encodedPartContent != null)
      {
        this.encodedPartContent.Write(buffer, offset, count);
      }
      else
      {
        using (Stream contentWriteStream = this.GetContentWriteStream())
          contentWriteStream.Write(buffer, offset, count);
      }
    }

    public void WriteContent(Stream sourceStream)
    {
      if (sourceStream == null)
        throw new ArgumentNullException("stream");
      this.AssertOpen();
      if (this.contentWritten)
        throw new InvalidOperationException(CtsResources.Strings.ContentAlreadyWritten);
      Stream stream1 = this.encodedPartContent;
      Stream stream2 = (Stream) null;
      try
      {
        if (stream1 == null)
        {
          stream2 = this.GetContentWriteStream();
          stream1 = stream2;
        }
        byte[] buffer = new byte[4096];
        int count;
        while (0 < (count = sourceStream.Read(buffer, 0, 4096)))
          stream1.Write(buffer, 0, count);
      }
      finally
      {
        if (stream2 != null)
          stream2.Dispose();
      }
    }

    public void WriteRawContent(byte[] buffer, int offset, int count)
    {
      MimeCommon.CheckBufferArguments(buffer, offset, count);
      this.AssertOpen();
      if (this.contentWritten)
        throw new InvalidOperationException(CtsResources.Strings.ContentAlreadyWritten);
      ((Stream) this.partContent ?? this.GetRawContentWriteStream()).Write(buffer, offset, count);
    }

    public void WriteRawContent(Stream sourceStream)
    {
      if (sourceStream == null)
        throw new ArgumentNullException("stream");
      this.AssertOpen();
      if (this.contentWritten)
        throw new InvalidOperationException(CtsResources.Strings.ContentAlreadyWritten);
      Stream stream = (Stream) this.partContent ?? this.GetRawContentWriteStream();
      byte[] buffer = new byte[4096];
      int count;
      while (0 < (count = sourceStream.Read(buffer, 0, 4096)))
        stream.Write(buffer, 0, count);
    }

    public void EndContent()
    {
      this.AssertOpen();
      if (this.encodedPartContent == null)
        return;
      this.encodedPartContent.Dispose();
      this.encodedPartContent = (Stream) null;
      this.contentWritten = true;
      if (this.partContent == null)
        return;
      this.partContent.Dispose();
      this.partContent = (MimeWriter.WriterContentStream) null;
    }

    public void Flush()
    {
      this.AssertOpen();
      if (this.state == MimeWriteState.Initial)
        return;
      this.FlushHeader();
      this.EndContent();
      this.FlushWriteQueue();
    }

    internal void WriteMimeNode(MimeNode node)
    {
      if (node == null)
        throw new ArgumentNullException(nameof(node));
      Header header1 = node as Header;
      if (header1 != null)
      {
        this.WriteHeader(header1);
        this.FlushHeader();
      }
      else
      {
        MimePart mimePart = node as MimePart;
        if (mimePart != null)
        {
          this.StartPart();
          mimePart.WriteTo((Stream) this.shimStream, this.encodingOptions);
          this.EndPart();
        }
        else
        {
          HeaderList headerList = node as HeaderList;
          if (headerList != null)
          {
            foreach (Header header2 in headerList)
              this.WriteHeader(header1);
            this.FlushHeader();
          }
          else
          {
            node = node.Clone();
            MimeRecipient recipient = node as MimeRecipient;
            if (recipient != null)
            {
              this.WriteRecipient(recipient);
            }
            else
            {
              MimeParameter parameter = node as MimeParameter;
              if (parameter != null)
              {
                this.WriteParameter(parameter);
              }
              else
              {
                MimeGroup group = node as MimeGroup;
                if (group == null)
                  return;
                this.StartGroup(group);
                this.EndGroup();
              }
            }
          }
        }
      }
    }

    public void WriteHeader(string name, string value)
    {
      this.AssertOpen();
      this.StartHeader(name);
      this.WriteHeaderValue(value);
      this.FlushHeader();
    }

    public void WriteHeader(HeaderId headerId, string value)
    {
      this.AssertOpen();
      this.StartHeader(headerId);
      this.WriteHeaderValue(value);
      this.FlushHeader();
    }

    public void WriteParameter(string name, string value)
    {
      if (name == null)
        throw new ArgumentNullException(nameof(name));
      this.AssertOpen();
      this.WriteParameter(new MimeParameter(name, value));
    }

    public void StartGroup(string name)
    {
      if (name == null)
        throw new ArgumentNullException(nameof(name));
      this.AssertOpen();
      this.StartGroup(new MimeGroup(name));
    }

    public void EndGroup()
    {
      this.AssertOpen();
      if (this.state != MimeWriteState.GroupRecipients)
        throw new InvalidOperationException(CtsResources.Strings.CannotWriteGroupEndHere);
      this.state = MimeWriteState.Recipients;
    }

    public void WriteRecipient(string displayName, string address)
    {
      if (address == null)
        throw new ArgumentNullException(nameof(address));
      this.AssertOpen();
      this.WriteRecipient(new MimeRecipient(displayName, address));
    }

    public MimeWriter GetEmbeddedMessageWriter()
    {
      this.AssertOpen();
      if (this.contentWritten)
        throw new InvalidOperationException(CtsResources.Strings.ContentAlreadyWritten);
      switch (this.state)
      {
        case MimeWriteState.Initial:
        case MimeWriteState.Complete:
        case MimeWriteState.PartContent:
        case MimeWriteState.EndPart:
          throw new InvalidOperationException(CtsResources.Strings.CannotWritePartContentNow);
        default:
          return new MimeWriter(this.GetRawContentWriteStream())
          {
            embeddedDepth = this.embeddedDepth + 1
          };
      }
    }

    public Stream GetRawContentWriteStream()
    {
      this.AssertOpen();
      if (this.contentWritten)
        throw new InvalidOperationException(CtsResources.Strings.ContentAlreadyWritten);
      switch (this.state)
      {
        case MimeWriteState.Initial:
        case MimeWriteState.Complete:
        case MimeWriteState.EndPart:
          throw new InvalidOperationException(CtsResources.Strings.CannotWritePartContentNow);
        case MimeWriteState.StartPart:
        case MimeWriteState.Headers:
        case MimeWriteState.Parameters:
        case MimeWriteState.Recipients:
        case MimeWriteState.GroupRecipients:
          this.FlushHeader();
          if (!this.foundMimeVersion)
          {
            if (this.forceMime && this.partDepth == 1)
              this.WriteMimeVersion();
            else
              this.currentPart.IsMultipart = false;
          }
          if (MimeWriteState.StartPart != this.state)
          {
            this.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
            break;
          }
          break;
        case MimeWriteState.PartContent:
          return (Stream) this.partContent;
      }
      if (this.currentPart.IsMultipart)
        throw new InvalidOperationException(CtsResources.Strings.MultipartCannotContainContent);
      this.state = MimeWriteState.PartContent;
      this.partContent = new MimeWriter.WriterContentStream(this);
      return (Stream) this.partContent;
    }

    public Stream GetContentWriteStream()
    {
      this.AssertOpen();
      if (this.contentWritten)
        throw new InvalidOperationException(CtsResources.Strings.ContentAlreadyWritten);
      if (this.partContent != null)
        throw new InvalidOperationException(CtsResources.Strings.PartContentIsBeingWritten);
      Stream contentWriteStream = this.GetRawContentWriteStream();
      if (this.contentTransferEncoding == ContentTransferEncoding.SevenBit || this.contentTransferEncoding == ContentTransferEncoding.EightBit || this.contentTransferEncoding == ContentTransferEncoding.Binary)
        return contentWriteStream;
      if (this.contentTransferEncoding == ContentTransferEncoding.BinHex)
        throw new NotSupportedException(CtsResources.Strings.BinHexNotSupportedForThisMethod);
      Encoders.ByteEncoder encoder = MimePart.CreateEncoder((Stream) null, this.contentTransferEncoding);
      if (encoder == null)
        throw new NotSupportedException(CtsResources.Strings.UnrecognizedTransferEncodingUsed);
      this.encodedPartContent = (Stream) new Encoders.EncoderStream(contentWriteStream, encoder, Encoders.EncoderStreamAccess.Write);
      return (Stream) new Internal.SuppressCloseStream(this.encodedPartContent);
    }

    public void StartPart()
    {
      this.AssertOpen();
      switch (this.state)
      {
        case MimeWriteState.Complete:
        case MimeWriteState.PartContent:
          throw new InvalidOperationException(CtsResources.Strings.CannotStartPartHere);
        default:
          if (this.partDepth != 0)
          {
            this.FlushHeader();
            if (!this.currentPart.IsMultipart)
              throw new InvalidOperationException(CtsResources.Strings.NonMultiPartPartsCannotHaveChildren);
            if (!this.foundMimeVersion && this.forceMime && this.partDepth == 1)
              this.WriteMimeVersion();
            this.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
            this.WriteBoundary(this.currentPart.Boundary, false);
          }
          MimeWriter.PartData part = new MimeWriter.PartData();
          this.PushPart(ref part);
          this.state = MimeWriteState.StartPart;
          this.contentWritten = false;
          this.contentTransferEncoding = ContentTransferEncoding.SevenBit;
          break;
      }
    }

    public void EndPart()
    {
      this.AssertOpen();
      switch (this.state)
      {
        case MimeWriteState.Initial:
        case MimeWriteState.Complete:
          throw new InvalidOperationException(CtsResources.Strings.CannotEndPartHere);
        case MimeWriteState.StartPart:
        case MimeWriteState.Headers:
        case MimeWriteState.Parameters:
        case MimeWriteState.Recipients:
        case MimeWriteState.GroupRecipients:
          this.FlushHeader();
          if (!this.foundMimeVersion)
          {
            if (this.forceMime && this.partDepth == 1)
              this.WriteMimeVersion();
            else
              this.currentPart.IsMultipart = false;
          }
          this.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
          break;
        case MimeWriteState.PartContent:
          if (this.encodedPartContent != null)
          {
            this.encodedPartContent.Dispose();
            this.encodedPartContent = (Stream) null;
          }
          if (this.partContent != null)
          {
            this.partContent.Dispose();
            this.partContent = (MimeWriter.WriterContentStream) null;
          }
          this.contentWritten = true;
          break;
      }
      this.state = MimeWriteState.EndPart;
      if (this.currentPart.IsMultipart)
      {
        this.Write(MimeString.CrLf, 0, MimeString.CrLf.Length);
        this.WriteBoundary(this.currentPart.Boundary, true);
      }
      this.PopPart();
      if (this.partDepth != 0)
        return;
      this.state = MimeWriteState.Complete;
    }

    internal void Write(byte[] data, int offset, int count)
    {
      if (0 >= count)
        return;
      this.QueueWrite(data, offset, count);
    }

    internal void QueueWrite(byte[] data, int offset, int count)
    {
      this.bytesWritten += count;
      this.lineTermination = MimeCommon.AdvanceLineTerminationState(this.lineTermination, data, offset, count);
      if ((this.writeCount == 1 ? this.currentWrite.Length - this.currentWrite.Count : (this.writeCount == 0 ? MimeWriter.QueuedWrite.QueuedWriteSize : 0)) >= count)
      {
        if (this.writeCount == 0)
        {
          MimeWriter.QueuedWrite write = new MimeWriter.QueuedWrite();
          this.PushWrite(ref write);
        }
        this.currentWrite.Append(data, offset, count);
      }
      else
      {
        MimeWriter.QueuedWrite write = new MimeWriter.QueuedWrite();
        if (count < MimeWriter.QueuedWrite.QueuedWriteSize && this.writeCount > 0)
          write = this.currentWrite;
        this.FlushWriteQueue();
        if (count < MimeWriter.QueuedWrite.QueuedWriteSize && write.Length > 0)
        {
          write.Reset();
          write.Append(data, offset, count);
          this.PushWrite(ref write);
        }
        else
          this.data.Write(data, offset, count);
      }
    }

    private void WriteHeader(Header header)
    {
      switch (this.state)
      {
        case MimeWriteState.Initial:
        case MimeWriteState.Complete:
        case MimeWriteState.PartContent:
        case MimeWriteState.EndPart:
          throw new InvalidOperationException(CtsResources.Strings.CannotWriteHeadersHere);
        default:
          this.state = MimeWriteState.Headers;
          this.FlushHeader();
          this.lastHeader = header;
          break;
      }
    }

    private void WriteParameter(MimeParameter parameter)
    {
      if (this.lastHeader == null || !(this.lastHeader is ComplexHeader))
        throw new InvalidOperationException(CtsResources.Strings.CannotWriteParametersOnThisHeader);
      switch (this.state)
      {
        case MimeWriteState.Complete:
        case MimeWriteState.StartPart:
        case MimeWriteState.Recipients:
        case MimeWriteState.PartContent:
        case MimeWriteState.EndPart:
          throw new InvalidOperationException(CtsResources.Strings.CannotWriteParametersHere);
        default:
          this.state = MimeWriteState.Parameters;
          ContentTypeHeader contentTypeHeader = this.lastHeader as ContentTypeHeader;
          if (contentTypeHeader != null && contentTypeHeader.IsMultipart && parameter.Name == "boundary")
          {
            string str = parameter.Value;
            if (str.Length == 0)
              throw new ArgumentException(CtsResources.Strings.CannotWriteEmptyOrNullBoundary);
            this.currentPart.Boundary = Internal.ByteString.StringToBytes(str, false);
          }
          this.lastHeader.InternalAppendChild((MimeNode) parameter);
          break;
      }
    }

    private void WriteRecipient(MimeRecipient recipient)
    {
      if (this.lastHeader == null || !(this.lastHeader is AddressHeader))
        throw new InvalidOperationException(CtsResources.Strings.CannotWriteRecipientsHere);
      MimeNode mimeNode;
      switch (this.state)
      {
        case MimeWriteState.Complete:
        case MimeWriteState.StartPart:
        case MimeWriteState.PartContent:
        case MimeWriteState.EndPart:
          throw new InvalidOperationException(CtsResources.Strings.CannotWriteRecipientsHere);
        case MimeWriteState.GroupRecipients:
          mimeNode = this.lastHeader.LastChild;
          break;
        default:
          this.state = MimeWriteState.Recipients;
          mimeNode = (MimeNode) this.lastHeader;
          break;
      }
      mimeNode.InternalAppendChild((MimeNode) recipient);
    }

    private void StartGroup(MimeGroup group)
    {
      switch (this.state)
      {
        case MimeWriteState.Complete:
        case MimeWriteState.StartPart:
        case MimeWriteState.PartContent:
        case MimeWriteState.EndPart:
          throw new InvalidOperationException(CtsResources.Strings.CannotWriteGroupStartHere);
        default:
          if (this.lastHeader == null || !(this.lastHeader is AddressHeader))
            throw new InvalidOperationException(CtsResources.Strings.CannotWriteGroupStartHere);
          this.state = MimeWriteState.GroupRecipients;
          this.lastHeader.InternalAppendChild((MimeNode) group);
          break;
      }
    }

    private void FlushHeader()
    {
      this.headerValueWritten = false;
      if (this.lastHeader == null)
        return;
      if (this.lastHeader.HeaderId == HeaderId.MimeVersion && this.partDepth == 1)
        this.foundMimeVersion = true;
      else if (this.lastHeader.HeaderId == HeaderId.ContentTransferEncoding)
      {
        string data = this.lastHeader.Value;
        if (data != null)
          this.contentTransferEncoding = MimePart.GetEncodingType(new MimeString(data));
      }
      else if (this.lastHeader.HeaderId == HeaderId.ContentType)
      {
        ContentTypeHeader contentTypeHeader = this.lastHeader as ContentTypeHeader;
        if (contentTypeHeader.IsMultipart)
        {
          this.currentPart.IsMultipart = true;
          this.currentPart.Boundary = contentTypeHeader["boundary"].RawValue;
        }
        else
          this.currentPart.IsMultipart = false;
        this.currentPart.HasContentType = true;
      }
      this.lastHeader.WriteTo((Stream) this.shimStream, this.encodingOptions);
      this.lastHeader = (Header) null;
    }

    private void WriteMimeVersion()
    {
      this.foundMimeVersion = true;
      this.QueueWrite(MimeString.MimeVersion, 0, MimeString.MimeVersion.Length);
      this.state = MimeWriteState.Headers;
    }

    private void FlushWriteQueue()
    {
      if (this.writeCount != 0)
        this.writeQueue[this.writeCount - 1] = this.currentWrite;
      for (int index = 0; index < this.writeCount; ++index)
      {
        this.data.Write(this.writeQueue[index].Data, this.writeQueue[index].Offset, this.writeQueue[index].Count);
        this.writeQueue[index] = new MimeWriter.QueuedWrite();
      }
      this.writeCount = 0;
    }

    private void WriteBoundary(byte[] boundary, bool final)
    {
      byte[] data = !final ? MimeWriter.endBoundarySuffix : MimeWriter.terminateBoundarySuffix;
      this.Write(MimeWriter.boundaryPrefix, 0, MimeWriter.boundaryPrefix.Length);
      this.Write(boundary, 0, boundary.Length);
      this.Write(data, 0, data.Length);
    }

    private void PushPart(ref MimeWriter.PartData part)
    {
      if (this.partStack == null)
      {
        this.partStack = new MimeWriter.PartData[8];
        this.partDepth = 0;
      }
      else if (this.partStack.Length == this.partDepth)
      {
        MimeWriter.PartData[] partDataArray = new MimeWriter.PartData[this.partStack.Length * 2];
        Array.Copy((Array) this.partStack, 0, (Array) partDataArray, 0, this.partStack.Length);
        for (int index = 0; index < this.partDepth; ++index)
          this.partStack[index] = new MimeWriter.PartData();
        this.partStack = partDataArray;
      }
      if (this.partDepth != 0)
        this.partStack[this.partDepth - 1] = this.currentPart;
      this.partStack[this.partDepth++] = part;
      this.currentPart = part;
    }

    private void PopPart()
    {
      --this.partDepth;
      this.partStack[this.partDepth] = new MimeWriter.PartData();
      this.currentPart = this.partStack[this.partDepth > 0 ? this.partDepth - 1 : 0];
    }

    private void AssertOpen()
    {
      if (this.data == null)
        throw new ObjectDisposedException("MimeWriter");
    }

    private void PushWrite(ref MimeWriter.QueuedWrite write)
    {
      if (this.writeQueue == null)
      {
        this.writeQueue = new MimeWriter.QueuedWrite[16];
        this.writeCount = 0;
      }
      else if (this.writeQueue.Length == this.writeCount)
      {
        MimeWriter.QueuedWrite[] queuedWriteArray = new MimeWriter.QueuedWrite[this.writeQueue.Length * 2];
        Array.Copy((Array) this.writeQueue, 0, (Array) queuedWriteArray, 0, this.writeQueue.Length);
        for (int index = 0; index < this.writeQueue.Length; ++index)
          this.writeQueue[index] = new MimeWriter.QueuedWrite();
        this.writeQueue = queuedWriteArray;
      }
      if (this.writeCount != 0)
        this.writeQueue[this.writeCount - 1] = this.currentWrite;
      this.writeQueue[this.writeCount++] = write;
      this.currentWrite = write;
    }

    private struct PartData
    {

        public bool IsMultipart { get; set; }

        public bool HasContentType { get; set; }

        public byte[] Boundary { get; set; }

    }

    private struct QueuedWrite
    {
      public static int QueuedWriteSize = 4096;

        public int Length => this.Data.Length;

        public byte[] Data { get; private set; }

        public int Offset { get; private set; }

        public int Count { get; private set; }

        public bool Full
      {
        get
        {
          if (this.Data != null)
            return this.Count == this.Data.Length;
          return false;
        }
      }

      public void Reset()
      {
        this.Count = 0;
        this.Offset = 0;
      }

      public int Append(byte[] buffer, int offset, int count)
      {
        if (this.Full)
          return 0;
        if (this.Data == null)
          this.Data = new byte[MimeWriter.QueuedWrite.QueuedWriteSize];
        int count1 = Math.Min(count, this.Data.Length - this.Count);
        Buffer.BlockCopy((Array) buffer, offset, (Array) this.Data, this.Count, count1);
        this.Count += count1;
        return count1;
      }
    }

    internal class WriterQueueStream : Stream
    {
      private MimeWriter writer;

      public override bool CanRead => false;

        public override bool CanWrite => true;

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

      public WriterQueueStream(MimeWriter writer)
      {
        this.writer = writer;
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        throw new NotSupportedException();
      }

      public override void Write(byte[] buffer, int offset, int count)
      {
        if (count <= 0)
          return;
        this.writer.QueueWrite(buffer, offset, count);
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

    private class WriterContentStream : Stream
    {
      private MimeWriter writer;

      public override bool CanRead => false;

        public override bool CanWrite => true;

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

      public WriterContentStream(MimeWriter writer)
      {
        this.writer = writer;
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        throw new NotSupportedException();
      }

      public override void Write(byte[] buffer, int offset, int count)
      {
        MimeCommon.CheckBufferArguments(buffer, offset, count);
        if (this.writer.contentWritten)
          throw new InvalidOperationException(CtsResources.Strings.ContentAlreadyWritten);
        this.writer.Write(buffer, offset, count);
      }

      public override long Seek(long offset, SeekOrigin origin)
      {
        throw new NotSupportedException();
      }

      public override void Flush()
      {
        if (this.writer.contentWritten)
          throw new InvalidOperationException(CtsResources.Strings.ContentAlreadyWritten);
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
