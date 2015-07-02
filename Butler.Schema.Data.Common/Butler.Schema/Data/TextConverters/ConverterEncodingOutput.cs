// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.ConverterEncodingOutput
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters
{
  internal class ConverterEncodingOutput : ConverterOutput, IByteSource, IRestartable, IReusable
  {
    private ByteCache cache = new ByteCache();
    private Globalization.CodePageMap codePageMap = new Globalization.CodePageMap();
    private bool isFirstChar = true;
    private const int LineSpaceThreshold = 256;
    private const int SpaceThreshold = 32;
    protected IResultsFeedback resultFeedback;
    private Stream pushSink;
    private ConverterStream pullSink;
    private bool endOfFile;
    private bool restartablePushSink;
    private long restartPosition;
    private bool encodingSameAsInput;
    private bool restartable;
    private bool canRestart;
    private bool lineModeEncoding;
    private int minCharsEncode;
    private char[] lineBuffer;
    private int lineBufferCount;
    private Encoding originalEncoding;
    private Encoding encoding;
    private Encoder encoder;
    private bool encodingCompleteUnicode;

    public Encoding Encoding
    {
      get
      {
        return this.encoding;
      }
      set
      {
        if (this.encoding == value)
          return;
        this.ChangeEncoding(value);
        if (this.resultFeedback == null)
          return;
        this.resultFeedback.Set(ConfigParameter.OutputEncoding, (object) this.encoding);
      }
    }

    public bool CodePageSameAsInput
    {
      get
      {
        return this.encodingSameAsInput;
      }
    }

    public override bool CanAcceptMore
    {
      get
      {
        if (!this.canRestart && this.pullSink != null)
          return this.cache.Length == 0;
        return true;
      }
    }

    public ConverterEncodingOutput(Stream destination, bool push, bool restartable, Encoding encoding, bool encodingSameAsInput, bool testBoundaryConditions, IResultsFeedback resultFeedback)
    {
      this.resultFeedback = resultFeedback;
      if (!push)
      {
        this.pullSink = destination as ConverterStream;
        this.pullSink.SetSource((IByteSource) this);
      }
      else
      {
        this.pushSink = destination;
        if (restartable && destination.CanSeek && destination.Position == destination.Length)
        {
          this.restartablePushSink = true;
          this.restartPosition = destination.Position;
        }
      }
      this.restartable = this.canRestart = restartable;
      this.lineBuffer = new char[4096];
      this.minCharsEncode = testBoundaryConditions ? 1 : 256;
      this.encodingSameAsInput = encodingSameAsInput;
      this.originalEncoding = encoding;
      this.ChangeEncoding(encoding);
      if (this.resultFeedback == null)
        return;
      this.resultFeedback.Set(ConfigParameter.OutputEncoding, (object) this.encoding);
    }

    private void Reinitialize()
    {
      this.endOfFile = false;
      this.lineBufferCount = 0;
      this.isFirstChar = true;
      this.cache.Reset();
      this.encoding = (Encoding) null;
      this.ChangeEncoding(this.originalEncoding);
      this.canRestart = this.restartable;
    }

    bool IRestartable.CanRestart()
    {
      return this.canRestart;
    }

    void IRestartable.Restart()
    {
      if (this.pullSink == null && this.restartablePushSink)
      {
        this.pushSink.Position = this.restartPosition;
        this.pushSink.SetLength(this.restartPosition);
      }
      this.Reinitialize();
      this.canRestart = false;
    }

    void IRestartable.DisableRestart()
    {
      this.canRestart = false;
      this.FlushCached();
    }

    void IReusable.Initialize(object newSourceOrDestination)
    {
      this.restartablePushSink = false;
      if (this.pushSink != null && newSourceOrDestination != null)
      {
        Stream stream = newSourceOrDestination as Stream;
        if (stream == null || !stream.CanWrite)
          throw new InvalidOperationException("cannot reinitialize this converter - new output should be a writable Stream object");
        this.pushSink = stream;
        if (this.restartable && stream.CanSeek && stream.Position == stream.Length)
        {
          this.restartablePushSink = true;
          this.restartPosition = stream.Position;
        }
      }
      this.Reinitialize();
    }

    public override void Write(char[] buffer, int offset, int count, IFallback fallback)
    {
      if (fallback == null && !this.lineModeEncoding && this.lineBufferCount + count <= this.lineBuffer.Length - this.minCharsEncode)
      {
        if (count == 1)
        {
          this.lineBuffer[this.lineBufferCount++] = buffer[offset];
          return;
        }
        if (count < 16)
        {
          if ((count & 8) != 0)
          {
            this.lineBuffer[this.lineBufferCount] = buffer[offset];
            this.lineBuffer[this.lineBufferCount + 1] = buffer[offset + 1];
            this.lineBuffer[this.lineBufferCount + 2] = buffer[offset + 2];
            this.lineBuffer[this.lineBufferCount + 3] = buffer[offset + 3];
            this.lineBuffer[this.lineBufferCount + 4] = buffer[offset + 4];
            this.lineBuffer[this.lineBufferCount + 5] = buffer[offset + 5];
            this.lineBuffer[this.lineBufferCount + 6] = buffer[offset + 6];
            this.lineBuffer[this.lineBufferCount + 7] = buffer[offset + 7];
            this.lineBufferCount += 8;
            offset += 8;
          }
          if ((count & 4) != 0)
          {
            this.lineBuffer[this.lineBufferCount] = buffer[offset];
            this.lineBuffer[this.lineBufferCount + 1] = buffer[offset + 1];
            this.lineBuffer[this.lineBufferCount + 2] = buffer[offset + 2];
            this.lineBuffer[this.lineBufferCount + 3] = buffer[offset + 3];
            this.lineBufferCount += 4;
            offset += 4;
          }
          if ((count & 2) != 0)
          {
            this.lineBuffer[this.lineBufferCount] = buffer[offset];
            this.lineBuffer[this.lineBufferCount + 1] = buffer[offset + 1];
            this.lineBufferCount += 2;
            offset += 2;
          }
          if ((count & 1) == 0)
            return;
          this.lineBuffer[this.lineBufferCount++] = buffer[offset];
          return;
        }
      }
      this.WriteComplete(buffer, offset, count, fallback);
    }

    public void WriteComplete(char[] buffer, int offset, int count, IFallback fallback)
    {
      int count1 = 0;
      if (fallback != null || this.lineModeEncoding)
      {
        byte unsafeAsciiMask = (byte) 0;
        byte[] numArray = (byte[]) null;
        uint num = 0U;
        bool flag1 = false;
        bool flag2 = false;
        if (fallback != null)
        {
          numArray = fallback.GetUnsafeAsciiMap(out unsafeAsciiMask);
          if (numArray != null)
            num = (uint) numArray.Length;
          flag1 = fallback.HasUnsafeUnicode();
          flag2 = fallback.TreatNonAsciiAsUnsafe(this.encoding.WebName);
        }
        while (count != 0)
        {
          while (count != 0 && this.lineBufferCount != this.lineBuffer.Length)
          {
            char ch = buffer[offset];
            if (fallback != null && ((uint) ch < num && ((int) numArray[(int) ch] & (int) unsafeAsciiMask) != 0 || !this.encodingCompleteUnicode && ((int) ch >= (int) sbyte.MaxValue || (int) ch < 32) && this.codePageMap.IsUnsafeExtendedCharacter(ch) || flag1 && (int) ch >= (int) sbyte.MaxValue && (flag2 || fallback.IsUnsafeUnicode(ch, this.isFirstChar))))
            {
              if (fallback.FallBackChar(ch, this.lineBuffer, ref this.lineBufferCount, this.lineBuffer.Length))
                this.isFirstChar = false;
              else
                break;
            }
            else
            {
              this.lineBuffer[this.lineBufferCount++] = ch;
              this.isFirstChar = false;
              if (this.lineModeEncoding)
              {
                if ((int) ch == 10 || (int) ch == 13)
                  count1 = this.lineBufferCount;
                else if (count1 > this.lineBuffer.Length - 256)
                {
                  --count;
                  ++offset;
                  break;
                }
              }
            }
            --count;
            ++offset;
          }
          if (this.lineModeEncoding && (count1 > this.lineBuffer.Length - 256 || this.lineBufferCount > this.lineBuffer.Length - 32 && count1 != 0))
          {
            this.EncodeBuffer(this.lineBuffer, 0, count1, false);
            this.lineBufferCount -= count1;
            if (this.lineBufferCount != 0)
              Buffer.BlockCopy((Array) this.lineBuffer, count1 * 2, (Array) this.lineBuffer, 0, this.lineBufferCount * 2);
          }
          else if (this.lineBufferCount > this.lineBuffer.Length - Math.Max(this.minCharsEncode, 32))
          {
            this.EncodeBuffer(this.lineBuffer, 0, this.lineBufferCount, false);
            this.lineBufferCount = 0;
          }
          count1 = 0;
        }
      }
      else if (count > this.minCharsEncode)
      {
        if (this.lineBufferCount != 0)
        {
          this.EncodeBuffer(this.lineBuffer, 0, this.lineBufferCount, false);
          this.lineBufferCount = 0;
        }
        this.EncodeBuffer(buffer, offset, count, false);
      }
      else
      {
        Buffer.BlockCopy((Array) buffer, offset * 2, (Array) this.lineBuffer, this.lineBufferCount * 2, count * 2);
        this.lineBufferCount += count;
        if (this.lineBufferCount <= this.lineBuffer.Length - this.minCharsEncode)
          return;
        this.EncodeBuffer(this.lineBuffer, 0, this.lineBufferCount, false);
        this.lineBufferCount = 0;
      }
    }

    public override void Write(string text)
    {
      if (text.Length == 0)
        return;
      if (this.lineModeEncoding || this.lineBufferCount + text.Length > this.lineBuffer.Length - this.minCharsEncode)
        this.Write(text, 0, text.Length);
      else if (text.Length <= 4)
      {
        int length = text.Length;
        this.lineBuffer[this.lineBufferCount++] = text[0];
        int num1;
        if ((num1 = length - 1) == 0)
          return;
        this.lineBuffer[this.lineBufferCount++] = text[1];
        int num2;
        if ((num2 = num1 - 1) == 0)
          return;
        this.lineBuffer[this.lineBufferCount++] = text[2];
        int num3;
        if ((num3 = num2 - 1) == 0)
          return;
        this.lineBuffer[this.lineBufferCount++] = text[3];
      }
      else
      {
        text.CopyTo(0, this.lineBuffer, this.lineBufferCount, text.Length);
        this.lineBufferCount += text.Length;
      }
    }

    public override void Flush()
    {
      if (this.endOfFile)
        return;
      this.canRestart = false;
      this.FlushCached();
      this.EncodeBuffer(this.lineBuffer, 0, this.lineBufferCount, true);
      this.lineBufferCount = 0;
      if (this.pullSink == null)
        this.pushSink.Flush();
      else if (this.cache.Length == 0)
        this.pullSink.ReportEndOfFile();
      this.endOfFile = true;
    }

    bool IByteSource.GetOutputChunk(out byte[] chunkBuffer, out int chunkOffset, out int chunkLength)
    {
      if (this.cache.Length == 0 || this.canRestart)
      {
        chunkBuffer = (byte[]) null;
        chunkOffset = 0;
        chunkLength = 0;
        return false;
      }
      this.cache.GetData(out chunkBuffer, out chunkOffset, out chunkLength);
      return true;
    }

    void IByteSource.ReportOutput(int readCount)
    {
      this.cache.ReportRead(readCount);
      if (this.cache.Length != 0 || !this.endOfFile)
        return;
      this.pullSink.ReportEndOfFile();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.cache != null && this.cache is IDisposable)
        ((IDisposable) this.cache).Dispose();
      this.cache = (ByteCache) null;
      this.pushSink = (Stream) null;
      this.pullSink = (ConverterStream) null;
      this.lineBuffer = (char[]) null;
      this.encoding = (Encoding) null;
      this.encoder = (Encoder) null;
      this.codePageMap = (Globalization.CodePageMap) null;
      base.Dispose(disposing);
    }

    private void EncodeBuffer(char[] buffer, int offset, int count, bool flush)
    {
      int maxByteCount = this.encoding.GetMaxByteCount(count);
      byte[] outputBuffer = (byte[]) null;
      int outputOffset = 0;
      int outputCount1 = 0;
      bool flag = true;
      byte[] numArray;
      int num;
      if (this.canRestart || this.pullSink == null || this.cache.Length != 0)
      {
        this.cache.GetBuffer(maxByteCount, out numArray, out num);
      }
      else
      {
        this.pullSink.GetOutputBuffer(out outputBuffer, out outputOffset, out outputCount1);
        if (outputCount1 >= maxByteCount)
        {
          numArray = outputBuffer;
          num = outputOffset;
          flag = false;
        }
        else
          this.cache.GetBuffer(maxByteCount, out numArray, out num);
      }
      int bytes = this.encoder.GetBytes(buffer, offset, count, numArray, num, flush);
      if (this.reportBytes != null)
        this.reportBytes.ReportBytesWritten(bytes);
      if (flag)
      {
        this.cache.Commit(bytes);
        if (this.pullSink == null)
        {
          if (this.canRestart && !this.restartablePushSink)
            return;
          while (this.cache.Length != 0)
          {
            int outputCount2;
            this.cache.GetData(out numArray, out num, out outputCount2);
            this.pushSink.Write(numArray, num, outputCount2);
            this.cache.ReportRead(outputCount2);
          }
        }
        else
        {
          if (this.canRestart)
            return;
          this.pullSink.ReportOutput(this.cache.Read(outputBuffer, outputOffset, outputCount1));
        }
      }
      else
        this.pullSink.ReportOutput(bytes);
    }

    internal void ChangeEncoding(Encoding newEncoding)
    {
      if (this.encoding != null)
      {
        this.EncodeBuffer(this.lineBuffer, 0, this.lineBufferCount, true);
        this.lineBufferCount = 0;
      }
      this.encoding = newEncoding;
      this.encoder = newEncoding.GetEncoder();
      int codePage = Globalization.CodePageMap.GetCodePage(newEncoding);
      switch (codePage)
      {
        case 1200:
        case 1201:
        case 12000:
        case 12001:
        case 65000:
        case 65001:
        case 65005:
        case 65006:
        case 54936:
          this.lineModeEncoding = false;
          this.encodingCompleteUnicode = true;
          this.codePageMap.ChoseCodePage(1200);
          break;
        default:
          this.encodingCompleteUnicode = false;
          this.codePageMap.ChoseCodePage(codePage);
          if (codePage != 50220 && codePage != 50221 && (codePage != 50222 && codePage != 50225) && (codePage != 50227 && codePage != 50229 && codePage != 52936))
            break;
          this.lineModeEncoding = true;
          break;
      }
    }

    private bool FlushCached()
    {
      if (this.canRestart || this.cache.Length == 0)
        return false;
      if (this.pullSink == null)
      {
        while (this.cache.Length != 0)
        {
          byte[] outputBuffer;
          int outputOffset;
          int outputCount;
          this.cache.GetData(out outputBuffer, out outputOffset, out outputCount);
          this.pushSink.Write(outputBuffer, outputOffset, outputCount);
          this.cache.ReportRead(outputCount);
        }
      }
      else
      {
        byte[] outputBuffer;
        int outputOffset;
        int outputCount;
        this.pullSink.GetOutputBuffer(out outputBuffer, out outputOffset, out outputCount);
        this.pullSink.ReportOutput(this.cache.Read(outputBuffer, outputOffset, outputCount));
      }
      return true;
    }
  }
}
