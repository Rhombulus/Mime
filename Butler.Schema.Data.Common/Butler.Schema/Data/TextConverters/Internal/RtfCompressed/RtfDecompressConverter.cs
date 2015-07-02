// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.RtfCompressed.RtfDecompressConverter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.RtfCompressed
{
  internal class RtfDecompressConverter : RtfCompressCommon, IProducerConsumer, IDisposable
  {
    private IResultsFeedback resultFeedback;
    private bool disableFastLoop;
    private bool uncompressed;
    private int expectedCount;
    private uint expectedCrc;
    private bool headerRead;
    private bool logicalEOS;
    private uint crc;
    private int pointerRead;
    private int match;
    private int matchLength;
    private int numFlags;
    private int flags;
    private byte[] headerBuffer;
    private int headerBufferCurrent;

    public RtfDecompressConverter(Stream input, bool push, Stream output, bool disableFastLoop, IResultsFeedback resultFeedback, int inputBufferSize, int outputBufferSize)
      : base(input, push, output, inputBufferSize, outputBufferSize)
    {
      this.resultFeedback = resultFeedback;
      this.disableFastLoop = disableFastLoop;
      this.window = new byte[4130];
      Buffer.BlockCopy((Array) RtfCompressCommon.PreloadData, 0, (Array) this.window, 0, RtfCompressCommon.PreloadData.Length);
      this.windowCurrent = RtfCompressCommon.PreloadData.Length;
      Buffer.BlockCopy((Array) RtfCompressCommon.PreloadData, 0, (Array) this.window, 4096, 34);
    }

    public void Run()
    {
      if (this.endOfFile || this.writeCurrent == this.writeEnd && !this.GetOutputSpace())
        return;
      if (this.readCurrent == this.readEnd)
      {
        if (!this.inputEndOfFile && !this.ReadMoreData())
          return;
        if (this.inputEndOfFile && (this.logicalEOS || this.numFlags == 0 || ((this.flags & 1) == 0 || this.pointerRead < 2)))
        {
          this.endOfFile = true;
          if (this.pullSink != null)
            this.pullSink.ReportEndOfFile();
          else
            this.pushSink.Flush();
          if (this.uncompressed || this.logicalEOS)
            return;
          throw new TextConvertersException("data truncated");
        }
        if (this.logicalEOS && this.readCurrent != this.readEnd)
        {
          this.readCurrent = this.readEnd;
          this.ReportRead();
          return;
        }
        if (!this.headerRead)
        {
          byte[] numArray;
          int startIndex;
          if (this.headerBuffer != null || this.readEnd - this.readCurrent < 16)
          {
            int count = Math.Min(16 - this.headerBufferCurrent, this.readEnd - this.readCurrent);
            if (this.headerBuffer == null)
              this.headerBuffer = new byte[16];
            Buffer.BlockCopy((Array) this.readBuffer, this.readCurrent, (Array) this.headerBuffer, this.headerBufferCurrent, count);
            this.readCurrent += count;
            this.headerBufferCurrent += count;
            if (this.headerBufferCurrent != 16)
            {
              this.ReportRead();
              return;
            }
            numArray = this.headerBuffer;
            startIndex = 0;
          }
          else
          {
            numArray = this.readBuffer;
            startIndex = this.readCurrent;
            this.readCurrent += 16;
          }
          uint num = BitConverter.ToUInt32(numArray, startIndex + 8);
          this.expectedCount = BitConverter.ToInt32(numArray, startIndex) - 12;
          if ((int) num == 1095517517)
            this.uncompressed = true;
          if ((int) num != 1095517517 && (int) num != 1967544908 || (this.expectedCount > int.MaxValue || this.expectedCount < 0))
          {
            this.readCurrent = this.readEnd;
            throw new TextConvertersException("invalid compressed RTF header");
          }
          if (this.resultFeedback != null)
            this.resultFeedback.Set(ConfigParameter.RtfCompressionMode, (object) (RtfCompressionMode) (this.uncompressed ? 1 : 0));
          this.expectedCrc = BitConverter.ToUInt32(numArray, startIndex + 12);
          this.headerRead = true;
        }
      }
      if (this.uncompressed)
      {
        int count = Math.Min(this.readEnd - this.readCurrent, this.writeEnd - this.writeCurrent);
        Buffer.BlockCopy((Array) this.readBuffer, this.readCurrent, (Array) this.writeBuffer, this.writeCurrent, count);
        this.readCurrent += count;
        this.writeCurrent += count;
        this.ReportRead();
        this.FlushOutput();
      }
      else
      {
        if (this.numFlags != 0 && (this.flags & 1) != 0 && this.pointerRead != 0)
        {
          if (this.pointerRead == 1)
          {
            int num = (int) this.readBuffer[this.readCurrent++];
            this.crc = CRC32.ComputeCRC(this.crc, (uint) num);
            this.match = this.match << 8 | num;
            this.matchLength = (this.match & 15) + 2;
            this.match = this.match >> 4;
            ++this.pointerRead;
            --this.expectedCount;
          }
          if (this.match == (this.windowCurrent & 4095))
          {
            for (; this.expectedCount > 0 && this.readCurrent != this.readEnd; --this.expectedCount)
              this.crc = CRC32.ComputeCRC(this.crc, (uint) this.readBuffer[this.readCurrent++]);
            this.readCurrent = this.readEnd;
            if (this.expectedCount <= 0)
            {
              this.logicalEOS = true;
              if ((int) this.crc != (int) this.expectedCrc)
                throw new TextConvertersException("CRC does not match, data corrupted");
              if (this.pullSource != null)
              {
                this.endOfFile = true;
                this.pullSink.ReportEndOfFile();
                return;
              }
              if (this.expectedCount < 0)
                throw new TextConvertersException("expected count in header is smaller than actual, data corrupted");
            }
            else if (this.inputEndOfFile)
            {
              this.endOfFile = true;
              throw new TextConvertersException("data truncated");
            }
          }
          else if (this.ExpandMatchWithOverflow())
          {
            --this.numFlags;
            this.flags >>= 1;
            this.pointerRead = 0;
          }
        }
        if (!this.disableFastLoop && this.readCurrent + 3 <= this.readEnd && this.writeCurrent + 17 <= this.writeEnd)
          this.FastLoop();
        if (this.pointerRead == 0)
          this.SlowLoop();
        this.ReportRead();
        if (this.pushSink != null && this.writeCurrent + 17 <= this.writeEnd && !this.logicalEOS)
          return;
        this.FlushOutput();
      }
    }

    public bool Flush()
    {
      if (!this.endOfFile)
        this.Run();
      return this.endOfFile;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.pullSource != null)
          this.pullSource.Dispose();
        if (this.pushSink != null)
          this.pushSink.Dispose();
      }
      this.pullSource = (Stream) null;
      this.pushSource = (ConverterStream) null;
      this.pullSink = (ConverterStream) null;
      this.pushSink = (Stream) null;
    }

    private void FastLoop()
    {
      int num1 = this.numFlags;
      int num2 = this.flags;
      int num3 = this.readCurrent;
      byte[] numArray1 = this.readBuffer;
      uint seed = this.crc;
      this.readEnd -= 3;
      this.writeEnd -= 17;
      this.expectedCount += num3 - this.readStart;
      do
      {
        if (num1 == 0)
        {
          num2 = (int) numArray1[num3++];
          seed = CRC32.ComputeCRC(seed, (uint) num2);
          num1 = 8;
        }
        if ((num2 & 1) != 0)
        {
          byte[] numArray2 = numArray1;
          int index1 = num3;
          int num4 = 1;
          int num5 = index1 + num4;
          int num6 = (int) numArray2[index1];
          uint crc = CRC32.ComputeCRC(seed, (uint) num6);
          byte[] numArray3 = numArray1;
          int index2 = num5;
          int num7 = 1;
          num3 = index2 + num7;
          int num8 = (int) numArray3[index2];
          seed = CRC32.ComputeCRC(crc, (uint) num8);
          int num9 = num6 << 8 | num8;
          int matchLength = (num9 & 15) + 2;
          int match = num9 >> 4;
          if (match == (this.windowCurrent & 4095))
          {
            this.match = match;
            this.matchLength = matchLength;
            this.pointerRead = 2;
            break;
          }
          this.ExpandMatch(match, matchLength);
        }
        else
        {
          int b = (int) numArray1[num3++];
          seed = CRC32.ComputeCRC(seed, (uint) b);
          this.AddRawByte(b);
        }
        --num1;
        num2 >>= 1;
      }
      while (num3 <= this.readEnd && this.writeCurrent <= this.writeEnd);
      this.crc = seed;
      this.readCurrent = num3;
      this.readEnd += 3;
      this.writeEnd += 17;
      this.expectedCount -= this.readCurrent - this.readStart;
      this.numFlags = num1;
      this.flags = num2;
    }

    private void SlowLoop()
    {
      this.expectedCount += this.readCurrent - this.readStart;
      while (this.readCurrent != this.readEnd && this.writeCurrent != this.writeEnd)
      {
        if (this.numFlags == 0)
        {
          this.flags = (int) this.readBuffer[this.readCurrent++];
          this.crc = CRC32.ComputeCRC(this.crc, (uint) this.flags);
          this.numFlags = 8;
        }
        if (this.readCurrent != this.readEnd)
        {
          if ((this.flags & 1) != 0)
          {
            if (this.readEnd - this.readCurrent >= 2)
            {
              this.match = (int) this.readBuffer[this.readCurrent++];
              this.crc = CRC32.ComputeCRC(this.crc, (uint) this.match);
              int num = (int) this.readBuffer[this.readCurrent++];
              this.crc = CRC32.ComputeCRC(this.crc, (uint) num);
              this.match = this.match << 8 | num;
              this.matchLength = (this.match & 15) + 2;
              this.match = this.match >> 4;
              if (this.match == (this.windowCurrent & 4095))
              {
                this.pointerRead = 2;
                break;
              }
              if (!this.ExpandMatchWithOverflow())
              {
                this.pointerRead = 2;
                break;
              }
            }
            else
            {
              this.match = (int) this.readBuffer[this.readCurrent++];
              this.crc = CRC32.ComputeCRC(this.crc, (uint) this.match);
              this.pointerRead = 1;
              break;
            }
          }
          else
          {
            int b = (int) this.readBuffer[this.readCurrent++];
            this.crc = CRC32.ComputeCRC(this.crc, (uint) b);
            this.AddRawByte(b);
          }
          --this.numFlags;
          this.flags >>= 1;
        }
      }
      this.expectedCount -= this.readCurrent - this.readStart;
    }

    private void ExpandMatch(int match, int matchLength)
    {
      if (match < 17 && this.windowCurrent >= 4096 && match < this.windowCurrent % 4096)
        match += 4096;
      byte[] numArray1 = this.window;
      int num1 = this.windowCurrent;
      byte[] numArray2 = this.writeBuffer;
      int num2 = this.writeCurrent;
      int num3 = matchLength;
      while (num3-- != 0)
      {
        byte num4 = numArray1[match++];
        numArray2[num2++] = num4;
        numArray1[num1++] = num4;
      }
      this.windowCurrent = num1;
      this.writeCurrent = num2;
      if (num1 <= 4096)
        return;
      this.ShadowCopy(matchLength);
    }

    private bool ExpandMatchWithOverflow()
    {
      int num1 = this.match;
      if (num1 < 17 && this.windowCurrent >= 4096 && num1 < this.windowCurrent % 4096)
        num1 += 4096;
      int matchLength = Math.Min(this.matchLength, this.writeEnd - this.writeCurrent);
      this.matchLength -= matchLength;
      int num2 = matchLength;
      while (num2-- != 0)
      {
        byte[] numArray1 = this.writeBuffer;
        int index1 = this.writeCurrent++;
        byte[] numArray2 = this.window;
        int index2 = this.windowCurrent++;
        byte[] numArray3 = this.window;
        int index3 = num1++;
        int num3;
        byte num4 = (byte) (num3 = (int) numArray3[index3]);
        numArray2[index2] = (byte) num3;
        int num5 = (int) num4;
        numArray1[index1] = (byte) num5;
      }
      if (this.windowCurrent > 4096)
        this.ShadowCopy(matchLength);
      if (num1 >= 4096)
        num1 -= 4096;
      this.match = num1;
      return this.matchLength == 0;
    }

    private void ShadowCopy(int matchLength)
    {
      int num1 = Math.Min(matchLength, this.windowCurrent - 4096);
      int num2 = this.windowCurrent - num1;
      int num3 = num2 - 4096;
      while (num1-- != 0)
        this.window[num3++] = this.window[num2++];
      if (num3 < 17)
        return;
      this.windowCurrent = num3;
    }

    private void AddRawByte(int b)
    {
      this.writeBuffer[this.writeCurrent++] = (byte) (int) (this.window[this.windowCurrent++] = (byte) (int) (byte) b);
      if (this.windowCurrent <= 4096)
        return;
      this.window[this.windowCurrent - 1 - 4096] = (byte) b;
      if (this.windowCurrent - 4096 < 17)
        return;
      this.windowCurrent -= 4096;
    }
  }
}
