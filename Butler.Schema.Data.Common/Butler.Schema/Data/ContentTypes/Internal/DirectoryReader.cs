// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.Internal.DirectoryReader
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.ContentTypes.Internal
{
  internal class DirectoryReader : IDisposable
  {
    private bool swallowUTFByteOrderMark = true;
    private char? lastChar = new char?();
    private const int BufferSize = 256;
    private const char ByteOrderMark1 = '\xFFFE';
    private const char ByteOrderMark2 = '\xFEFF';
    private UnfoldingStream inputStream;
    private Stream decoderStream;
    private Encoding outerCharsetEncoding;
    private Encoding currentCharsetEncoding;
    private Encoder currentCharsetEncoder;
    private Decoder currentCharsetDecoder;
    private byte[] dataBytes;
    private char[] dataChars;
    private int bottomByte;
    private int topByte;
    private int idxByte;
    private int topChar;
    private int idxChar;
    private bool isDisposed;
    private bool isDecoding;
    private ComplianceTracker complianceTracker;

    public Encoding CurrentCharsetEncoding
    {
      get
      {
        this.CheckDisposed("CurrentEncoding::get");
        return this.currentCharsetEncoding;
      }
    }

    public DirectoryReader(Stream inputStream, Encoding outerCharsetEncoding, ComplianceTracker complianceTracker)
    {
      this.inputStream = new UnfoldingStream(inputStream);
      this.outerCharsetEncoding = outerCharsetEncoding;
      this.currentCharsetEncoding = outerCharsetEncoding;
      this.currentCharsetDecoder = outerCharsetEncoding.GetDecoder();
      this.currentCharsetEncoder = outerCharsetEncoding.GetEncoder();
      this.dataBytes = new byte[256];
      this.dataChars = new char[256];
      this.complianceTracker = complianceTracker;
      this.SetFallback();
    }

    public bool ReadChar(out char result, out bool newLine)
    {
      this.CheckDisposed("ReadChar");
      result = '?';
      newLine = false;
      char? nullable = this.lastChar;
      if ((nullable.HasValue ? new int?((int) nullable.GetValueOrDefault()) : new int?()).HasValue)
      {
        result = this.lastChar.Value;
        this.lastChar = new char?();
      }
      else if (!this.ReadChar(out result))
        return false;
      if (!this.isDecoding && (int) result == 13)
      {
        if (this.ReadChar(out result))
        {
          if ((int) result == 10)
          {
            newLine = true;
            return true;
          }
          this.lastChar = new char?(result);
        }
        result = '\r';
      }
      return true;
    }

    public void SwitchCharsetEncoding(Encoding newCharsetEncoding)
    {
      this.CheckDisposed("SwitchEncoding");
      char? nullable = this.lastChar;
      if ((nullable.HasValue ? new int?((int) nullable.GetValueOrDefault()) : new int?()).HasValue)
        throw new InvalidOperationException();
      if (newCharsetEncoding.WebName == this.CurrentCharsetEncoding.WebName)
        return;
      this.bottomByte += this.currentCharsetEncoder.GetByteCount(this.dataChars, 0, this.idxChar, true);
      if (this.bottomByte > this.topByte)
      {
        this.complianceTracker.SetComplianceStatus(ComplianceStatus.InvalidCharacterInPropertyValue, CtsResources.CalendarStrings.InvalidCharacterInPropertyValue);
        this.bottomByte = this.topByte;
      }
      this.idxByte = this.bottomByte;
      this.idxChar = 0;
      this.topChar = 0;
      this.currentCharsetEncoding = newCharsetEncoding;
      this.currentCharsetEncoder = newCharsetEncoding.GetEncoder();
      this.currentCharsetDecoder = newCharsetEncoding.GetDecoder();
    }

    public void RestoreCharsetEncoding()
    {
      this.CheckDisposed("RestoreEncoding");
      this.SwitchCharsetEncoding(this.outerCharsetEncoding);
    }

    public void ApplyValueDecoder(Mime.Encoders.ByteEncoder decoder)
    {
      this.CheckDisposed("ApplyValueDecoder");
      char? nullable = this.lastChar;
      if ((nullable.HasValue ? new int?((int) nullable.GetValueOrDefault()) : new int?()).HasValue)
        throw new InvalidOperationException();
      if (this.decoderStream != null)
        throw new InvalidOperationException();
      this.decoderStream = (Stream) new Mime.Encoders.EncoderStream(this.GetValueReadStream((DirectoryReader.OnValueEndFunc) null), decoder, Mime.Encoders.EncoderStreamAccess.Read);
    }

    public Stream GetValueReadStream(DirectoryReader.OnValueEndFunc callback)
    {
      this.CheckDisposed("GetValueReadStream");
      char? nullable = this.lastChar;
      if ((nullable.HasValue ? new int?((int) nullable.GetValueOrDefault()) : new int?()).HasValue)
        throw new InvalidOperationException();
      this.bottomByte += this.currentCharsetEncoder.GetByteCount(this.dataChars, 0, this.idxChar, true);
      if (this.bottomByte > this.topByte)
      {
        this.complianceTracker.SetComplianceStatus(ComplianceStatus.InvalidCharacterInPropertyValue, CtsResources.CalendarStrings.InvalidCharacterInPropertyValue);
        this.bottomByte = this.topByte;
      }
      this.idxByte = this.bottomByte;
      this.idxChar = 0;
      this.topChar = 0;
      this.inputStream.Rewind(this.topByte - this.idxByte);
      this.topByte = 0;
      this.idxByte = 0;
      this.bottomByte = 0;
      return (Stream) new DirectoryReader.AdapterStream(this.inputStream, callback);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void CheckDisposed(string methodName)
    {
      if (this.isDisposed)
        throw new ObjectDisposedException("DirectoryReader", methodName);
    }

    private bool ReadChar(out char result)
    {
      result = '?';
      if (this.idxChar >= this.topChar)
      {
        this.idxChar = 0;
        this.topChar = 0;
        int bytesUsed = 0;
        int charsUsed = 0;
        bool completed = false;
        if (this.idxByte < this.topByte)
        {
          this.currentCharsetDecoder.Convert(this.dataBytes, this.idxByte, this.topByte - this.idxByte, this.dataChars, 0, this.dataChars.Length, false, out bytesUsed, out charsUsed, out completed);
          this.topChar = charsUsed;
          this.idxByte += bytesUsed;
        }
        while (this.topChar == 0)
        {
          for (int index = 0; index < this.topByte - this.idxByte; ++index)
            this.dataBytes[index] = this.dataBytes[this.idxByte + index];
          this.topByte = this.topByte - this.idxByte;
          this.bottomByte = 0;
          this.idxByte = 0;
          int num = this.ReadInputStream(this.dataBytes, this.topByte, this.dataBytes.Length - this.topByte);
          this.topByte += num;
          this.currentCharsetDecoder.Convert(this.dataBytes, 0, this.topByte, this.dataChars, 0, this.dataChars.Length, num == 0, out bytesUsed, out charsUsed, out completed);
          this.topChar = charsUsed;
          this.idxByte += bytesUsed;
          if (num == 0 && this.topChar == 0)
            return false;
        }
      }
      result = this.dataChars[this.idxChar++];
      if (this.swallowUTFByteOrderMark && ((int) result == 65534 || (int) result == 65279))
      {
        this.swallowUTFByteOrderMark = false;
        return this.ReadChar(out result);
      }
      this.swallowUTFByteOrderMark = false;
      return true;
    }

    private int ReadInputStream(byte[] buffer, int offset, int count)
    {
      this.isDecoding = false;
      if (this.decoderStream == null)
        return this.inputStream.Read(buffer, offset, count);
      int num = this.decoderStream.Read(buffer, offset, count);
      if (num > 0)
      {
        this.isDecoding = true;
        return num;
      }
      this.decoderStream.Dispose();
      this.decoderStream = (Stream) null;
      buffer[offset] = (byte) 13;
      buffer[offset + 1] = (byte) 10;
      return 2;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.isDisposed)
        return;
      if (disposing)
      {
        if (this.decoderStream != null)
        {
          this.decoderStream.Dispose();
          this.decoderStream = (Stream) null;
        }
        if (this.inputStream != null)
        {
          this.inputStream.Dispose();
          this.inputStream = (UnfoldingStream) null;
        }
      }
      this.isDisposed = true;
    }

    private void SetFallback()
    {
      this.currentCharsetDecoder.Fallback = (DecoderFallback) new DecoderReplacementFallback("?");
      this.currentCharsetEncoder.Fallback = (EncoderFallback) new EncoderReplacementFallback("?");
    }

    public delegate void OnValueEndFunc();

    private class AdapterStream : Stream
    {
      private byte[] tempBuffer = new byte[256];
      private int endIdx = -1;
      private bool isClosed;
      private UnfoldingStream inputStream;
      private int idx1;
      private int idx2;
      private DirectoryReader.OnValueEndFunc callback;
      private int position;

      public override bool CanRead
      {
        get
        {
          this.CheckDisposed("CanRead:get");
          return true;
        }
      }

      public override bool CanWrite
      {
        get
        {
          this.CheckDisposed("CanWrite:get");
          return false;
        }
      }

      public override bool CanSeek
      {
        get
        {
          this.CheckDisposed("CanSeek:get");
          return false;
        }
      }

      public override long Length
      {
        get
        {
          this.CheckDisposed("Length:Get");
          throw new NotSupportedException();
        }
      }

      public override long Position
      {
        get
        {
          this.CheckDisposed("Position:get");
          return (long) this.position;
        }
        set
        {
          this.CheckDisposed("Position:set");
          throw new NotSupportedException();
        }
      }

      public AdapterStream(UnfoldingStream inputStream, DirectoryReader.OnValueEndFunc callback)
      {
        this.inputStream = inputStream;
        this.callback = callback;
      }

      protected override void Dispose(bool disposing)
      {
        if (disposing && !this.isClosed)
        {
          byte[] buffer = new byte[1024];
          while (this.Read(buffer, 0, buffer.Length) > 0)
            ;
        }
        this.isClosed = true;
        base.Dispose(disposing);
      }

      public override void Write(byte[] buffer, int offset, int count)
      {
        this.CheckDisposed("Write");
        throw new NotSupportedException();
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        int num = this.InternalRead(buffer, offset, count);
        if (num == 0 && this.callback != null)
        {
          this.callback();
          this.callback = (DirectoryReader.OnValueEndFunc) null;
        }
        return num;
      }

      public override void SetLength(long value)
      {
        this.CheckDisposed("SetLength");
        throw new NotSupportedException();
      }

      public override long Seek(long offset, SeekOrigin origin)
      {
        this.CheckDisposed("Seek");
        throw new NotSupportedException();
      }

      public override void Flush()
      {
        this.CheckDisposed("Flush");
        throw new NotSupportedException();
      }

      private int InternalRead(byte[] buffer, int offset, int count)
      {
        this.CheckDisposed("Read");
        if (this.endIdx >= 0)
        {
          int length = Math.Min(count, this.endIdx - this.idx1);
          Array.Copy((Array) this.tempBuffer, this.idx1, (Array) buffer, offset, length);
          this.idx1 += length;
          this.position += length;
          return length;
        }
        if (this.idx1 != 0)
        {
          for (int index = 0; index < this.idx2 - this.idx1; ++index)
            this.tempBuffer[index] = this.tempBuffer[this.idx1 + index];
          this.idx2 -= this.idx1;
          this.idx1 = 0;
        }
        int num = this.inputStream.Read(this.tempBuffer, this.idx2, this.tempBuffer.Length - this.idx2);
        this.idx2 += num;
        if (num == 0)
        {
          this.endIdx = this.idx2 < 2 || (int) this.tempBuffer[this.idx2 - 2] != 13 || (int) this.tempBuffer[this.idx2 - 1] != 10 ? this.idx2 : this.idx2 - 2;
          return this.InternalRead(buffer, offset, count);
        }
        while (this.idx1 < count && this.idx1 < this.idx2 - 1)
        {
          if ((int) this.tempBuffer[this.idx1] == 13 && (int) this.tempBuffer[this.idx1 + 1] == 10)
          {
            this.endIdx = this.idx1;
            this.inputStream.Rewind(this.idx2 - (this.idx1 + 2));
            break;
          }
          buffer[this.idx1] = this.tempBuffer[this.idx1];
          ++this.idx1;
          ++this.position;
        }
        if (this.idx1 != 0)
          return this.idx1;
        return this.InternalRead(buffer, offset, count);
      }

      private void CheckDisposed(string methodName)
      {
        if (this.isClosed)
          throw new ObjectDisposedException("AdapterStream", methodName);
      }
    }
  }
}
