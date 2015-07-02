// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.RtfPreviewStream
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  internal class RtfPreviewStream : Stream
  {
    internal Stream InputRtfStream;
    internal Internal.Rtf.RtfParserBase Parser;
    internal int InternalPosition;
    private RtfEncapsulation rtfEncapsulation;

    public RtfEncapsulation Encapsulation
    {
      get
      {
        return this.rtfEncapsulation;
      }
    }

    public override bool CanRead
    {
      get
      {
        return true;
      }
    }

    public override bool CanSeek
    {
      get
      {
        return false;
      }
    }

    public override bool CanWrite
    {
      get
      {
        return false;
      }
    }

    public override long Length
    {
      get
      {
        throw new NotSupportedException(CtsResources.TextConvertersStrings.SeekUnsupported);
      }
    }

    public override long Position
    {
      get
      {
        throw new NotSupportedException(CtsResources.TextConvertersStrings.SeekUnsupported);
      }
      set
      {
        throw new NotSupportedException(CtsResources.TextConvertersStrings.SeekUnsupported);
      }
    }

    public RtfPreviewStream(Stream inputRtfStream, int inputBufferSize)
    {
      this.InputRtfStream = inputRtfStream;
      this.Parser = new Internal.Rtf.RtfParserBase(inputBufferSize, false, (IReportBytes) null);
      int offset;
      int bufferSpace = this.Parser.GetBufferSpace(false, out offset);
      int length = this.InputRtfStream.Read(this.Parser.ParseBuffer, offset, bufferSpace);
      this.Parser.ReportMoreDataAvailable(length, length == 0);
      int num = 0;
      while (this.Parser.ParseRun())
      {
        switch (this.Parser.RunKind)
        {
          case Internal.Rtf.RtfRunKind.Ignore:
            continue;
          case Internal.Rtf.RtfRunKind.Begin:
            if (num++ != 0)
              return;
            continue;
          case Internal.Rtf.RtfRunKind.Keyword:
            if (num++ > 10)
              return;
            if ((int) this.Parser.KeywordId == 292)
            {
              if (this.Parser.KeywordValue < 1)
                return;
              this.rtfEncapsulation = RtfEncapsulation.Html;
              return;
            }
            if ((int) this.Parser.KeywordId == 329)
            {
              this.rtfEncapsulation = RtfEncapsulation.Text;
              return;
            }
            continue;
          default:
            return;
        }
      }
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotSupportedException(CtsResources.TextConvertersStrings.SeekUnsupported);
    }

    public override void SetLength(long value)
    {
      throw new NotSupportedException(CtsResources.TextConvertersStrings.SeekUnsupported);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException(CtsResources.TextConvertersStrings.WriteUnsupported);
    }

    public override void Flush()
    {
      throw new NotSupportedException(CtsResources.TextConvertersStrings.WriteUnsupported);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (this.InputRtfStream == null)
        throw new ObjectDisposedException("RtfPreviewStream");
      int num1 = 0;
      if (this.InternalPosition != int.MaxValue)
      {
        if (this.Parser != null && this.InternalPosition < this.Parser.ParseEnd)
        {
          int count1 = Math.Min(this.Parser.ParseEnd - this.InternalPosition, count);
          Buffer.BlockCopy((Array) this.Parser.ParseBuffer, 0, (Array) buffer, offset, count1);
          this.InternalPosition += count1;
          count -= count1;
          offset += count1;
          num1 += count1;
          if (this.InternalPosition == this.Parser.ParseEnd)
            this.Parser = (Internal.Rtf.RtfParserBase) null;
        }
        if (count != 0)
        {
          int num2 = this.InputRtfStream.Read(buffer, offset, count);
          num1 += num2;
        }
      }
      return num1;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.InputRtfStream != null)
        this.InputRtfStream.Dispose();
      this.InputRtfStream = (Stream) null;
      this.Parser = (Internal.Rtf.RtfParserBase) null;
      base.Dispose(disposing);
    }
  }
}
