// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.RtfCompressed.RtfCompressConverter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.RtfCompressed
{
  internal class RtfCompressConverter : RtfCompressCommon, IProducerConsumer, IDisposable
  {
    private bool uncompressed;
    private Stream bufferStream;
    private bool flushing;
    private bool directOutput;
    private int flushOffset;
    private long initialStreamPosition;
    private int outputDataSize;
    private int inputDataSize;
    private uint crc;
    private byte[] header;
    private byte[] codes;
    private int codesCurrent;
    private int codesEnd;
    private bool flushCodes;
    private bool flushedTail;
    private int match;
    private int matchLength;
    private int lookAhead;
    private int readLength;
    private int numFlags;
    private int flags;
    private RtfCompressConverter.Node[] nodes;

    public RtfCompressConverter(Stream input, bool push, Stream output, RtfCompressionMode mode, int inputBufferSize, int outputBufferSize)
      : base(input, push, output, inputBufferSize, outputBufferSize)
    {
      if (mode == RtfCompressionMode.Compressed)
      {
        this.uncompressed = false;
        this.window = new byte[4113];
        this.nodes = new RtfCompressConverter.Node[4097];
        int index1 = this.nodes.Length - 1;
        for (int index2 = 0; index2 <= index1; ++index2)
          this.nodes[index2].Parent = (short) -1;
        this.nodes[index1].LeftChild = (short) -1;
        this.nodes[index1].RightChild = (short) -1;
        this.codes = new byte[32];
        Array.Copy((Array) RtfCompressCommon.PreloadData, 0, (Array) this.window, 0, RtfCompressCommon.PreloadData.Length);
        this.windowCurrent = RtfCompressCommon.PreloadData.Length - 17;
        Buffer.BlockCopy((Array) this.window, 0, (Array) this.window, 4096, 17);
        for (int node = 0; node <= this.windowCurrent; ++node)
          this.AddNode(node, out this.match, out this.matchLength);
        this.readLength = 17;
        this.lookAhead = 17;
        this.bufferStream = (Stream) null;
        if (this.pushSink != null && this.pushSink.CanSeek)
        {
          this.initialStreamPosition = this.pushSink.Position;
          this.pushSink.Position += 16L;
          this.directOutput = true;
        }
        else
        {
          this.bufferStream = Data.Internal.ApplicationServices.Provider.CreateTemporaryStorage();
          this.directOutput = false;
          if (this.writeBuffer != null)
            return;
          this.writeBuffer = new byte[4096];
          this.writeEnd = this.writeBuffer.Length;
          this.writeCurrent = this.writeStart;
        }
      }
      else
      {
        if (mode != RtfCompressionMode.Uncompressed)
          throw new ArgumentOutOfRangeException("CompressionMode");
        this.uncompressed = true;
        if (this.pushSink != null && this.pushSink.CanSeek)
        {
          this.initialStreamPosition = this.pushSink.Position;
          this.pushSink.Position += 16L;
          this.directOutput = true;
        }
        else if (this.pullSource != null && this.pullSource.CanSeek)
          this.initialStreamPosition = this.pullSource.Position;
        else
          this.bufferStream = Data.Internal.ApplicationServices.Provider.CreateTemporaryStorage();
      }
    }

    public void Run()
    {
      if (this.endOfFile)
        return;
      if (this.flushing)
      {
        if (this.writeCurrent == this.writeEnd && !this.GetOutputSpace())
          return;
        this.CopyBufferedData();
        this.FlushOutput();
        if (!this.inputEndOfFile)
          return;
        if (this.pullSink != null)
          this.pullSink.ReportEndOfFile();
        else
          this.pushSink.Flush();
        this.endOfFile = true;
      }
      else
      {
        if (this.readCurrent == this.readEnd)
        {
          if (!this.inputEndOfFile && !this.ReadMoreData())
            return;
          if (this.inputEndOfFile)
          {
            if (!this.uncompressed)
            {
              if (this.writeCurrent == this.writeEnd && (this.directOutput ? (!this.GetOutputSpace() ? 1 : 0) : (!this.GetTempOutputSpace() ? 1 : 0)) != 0)
                return;
              bool flag = this.Compress(true);
              if (flag || this.writeEnd == this.writeCurrent)
              {
                if (this.directOutput)
                  this.FlushOutput();
                else
                  this.FlushTempOutput();
              }
              if (!flag)
                return;
              this.inputDataSize = this.readFileOffset;
              this.outputDataSize = this.writeFileOffset;
            }
            this.PrepareHeader();
            if (this.directOutput)
            {
              this.RewriteHeader();
              this.endOfFile = true;
              if (this.pullSink != null)
              {
                this.pullSink.ReportEndOfFile();
                return;
              }
              this.pushSink.Flush();
              return;
            }
            this.flushing = true;
            this.directOutput = true;
            this.inputEndOfFile = false;
            this.flushOffset = 0;
            this.writeFileOffset = 0;
            return;
          }
        }
        if (this.uncompressed)
        {
          this.CopyInputChunkTo(this.directOutput ? this.pushSink : this.bufferStream);
        }
        else
        {
          if (this.writeCurrent == this.writeEnd && (this.directOutput ? (!this.GetOutputSpace() ? 1 : 0) : (!this.GetTempOutputSpace() ? 1 : 0)) != 0)
            return;
          this.Compress(false);
          if (this.writeEnd - this.writeCurrent == 0)
          {
            if (this.directOutput)
              this.FlushOutput();
            else
              this.FlushTempOutput();
          }
        }
        this.ReportRead();
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

    private bool Compress(bool flush)
    {
      if (this.flushCodes && !this.FlushCodes())
        return false;
      if (this.readCurrent == this.readEnd && (!flush || this.flushedTail))
        return this.flushedTail;
      if (this.lookAhead != 0)
      {
        do
        {
          while (this.readLength-- > 0)
          {
            if (this.readCurrent == this.readEnd)
            {
              if (!flush)
              {
                ++this.readLength;
                return false;
              }
              --this.lookAhead;
            }
            else
            {
              this.DeleteNode(this.windowCurrent + 17);
              this.window[this.windowCurrent + 17] = (byte) (int) this.readBuffer[this.readCurrent++];
            }
            if (++this.windowCurrent == 4096)
            {
              this.windowCurrent = 0;
              Buffer.BlockCopy((Array) this.window, 4096, (Array) this.window, 0, 17);
            }
            if (this.lookAhead > 0)
              this.AddNode(this.windowCurrent, out this.match, out this.matchLength);
          }
          if (this.lookAhead != 0)
          {
            if (this.matchLength > this.lookAhead)
              this.matchLength = this.lookAhead;
            this.flags >>= 1;
            if (this.matchLength <= 1)
            {
              this.codes[this.codesEnd++] = this.window[this.windowCurrent];
              this.readLength = 1;
            }
            else
            {
              this.flags |= 128;
              int num = this.match << 4 | this.matchLength - 2;
              this.codes[this.codesEnd++] = (byte) ((num & 65280) >> 8);
              this.codes[this.codesEnd++] = (byte) (num & (int) byte.MaxValue);
              this.readLength = this.matchLength;
            }
            if (++this.numFlags == 8)
            {
              this.FlushCodes(this.flags);
              this.flags = 0;
              this.numFlags = 0;
            }
          }
          else
            break;
        }
        while (this.writeEnd != this.writeCurrent);
        if (!flush || this.writeEnd == this.writeCurrent)
          return false;
      }
      this.flags >>= 1;
      this.flags |= 128;
      if (++this.numFlags < 8)
        this.flags >>= 8 - this.numFlags;
      int num1 = this.windowCurrent << 4;
      this.codes[this.codesEnd++] = (byte) ((num1 & 65280) >> 8);
      this.codes[this.codesEnd++] = (byte) (num1 & (int) byte.MaxValue);
      this.flushedTail = true;
      return this.FlushCodes(this.flags);
    }

    private bool FlushCodes(int flags)
    {
      this.writeBuffer[this.writeCurrent++] = (byte) (int) (byte) flags;
      this.codesCurrent = 0;
      this.flushCodes = true;
      this.crc = CRC32.ComputeCRC(this.crc, (uint) flags);
      return this.FlushCodes();
    }

    private bool FlushCodes()
    {
      while (this.writeCurrent != this.writeEnd)
      {
        byte num = this.codes[this.codesCurrent++];
        this.writeBuffer[this.writeCurrent++] = (byte) (int) num;
        this.crc = CRC32.ComputeCRC(this.crc, (uint) num);
        if (this.codesCurrent == this.codesEnd)
        {
          this.flushCodes = false;
          this.codesEnd = 0;
          return true;
        }
      }
      return false;
    }

    private void DeleteNode(int node)
    {
      node &= 4095;
      int index1 = (int) this.nodes[node].Parent;
      if (index1 < 0)
        return;
      if ((int) this.nodes[node].LeftChild < 0)
      {
        int index2 = (int) this.nodes[node].RightChild;
        if ((int) this.nodes[index1].LeftChild == node)
          this.nodes[index1].LeftChild = (short) index2;
        else
          this.nodes[index1].RightChild = (short) index2;
        if (index2 >= 0)
          this.nodes[index2].Parent = (short) index1;
      }
      else if ((int) this.nodes[node].RightChild < 0)
      {
        int index2 = (int) this.nodes[node].LeftChild;
        if ((int) this.nodes[index1].LeftChild == node)
          this.nodes[index1].LeftChild = (short) index2;
        else
          this.nodes[index1].RightChild = (short) index2;
        this.nodes[index2].Parent = (short) index1;
      }
      else
      {
        int index2 = (int) this.nodes[node].RightChild;
        if ((int) this.nodes[index2].LeftChild >= 0)
        {
          do
          {
            index2 = (int) this.nodes[index2].LeftChild;
          }
          while ((int) this.nodes[index2].LeftChild >= 0);
          this.nodes[(int) this.nodes[index2].Parent].LeftChild = this.nodes[index2].RightChild;
          if ((int) this.nodes[index2].RightChild >= 0)
            this.nodes[(int) this.nodes[index2].RightChild].Parent = this.nodes[index2].Parent;
          this.nodes[index2].RightChild = this.nodes[node].RightChild;
          this.nodes[index2].LeftChild = this.nodes[node].LeftChild;
          if ((int) this.nodes[index2].RightChild >= 0)
            this.nodes[(int) this.nodes[index2].RightChild].Parent = (short) index2;
          if ((int) this.nodes[index2].LeftChild >= 0)
            this.nodes[(int) this.nodes[index2].LeftChild].Parent = (short) index2;
        }
        else
        {
          this.nodes[index2].LeftChild = this.nodes[node].LeftChild;
          this.nodes[(int) this.nodes[index2].LeftChild].Parent = (short) index2;
        }
        if ((int) this.nodes[index1].LeftChild == node)
          this.nodes[index1].LeftChild = (short) index2;
        else
          this.nodes[index1].RightChild = (short) index2;
        this.nodes[index2].Parent = (short) index1;
      }
      this.nodes[node].Parent = (short) -1;
    }

    private void AddNode(int node, out int match, out int matchLength)
    {
      matchLength = 0;
      match = -1;
      int index1 = 4096;
      int index2 = (int) this.nodes[index1].LeftChild;
      bool flag = true;
      for (; index2 >= 0; index2 = flag ? (int) this.nodes[index2].LeftChild : (int) this.nodes[index2].RightChild)
      {
        int num1 = 0;
        int num2 = 17;
        int num3 = node;
        int num4 = index2;
        while (num2-- > 0)
        {
          num1 = (int) this.window[num3++] - (int) this.window[num4++];
          if (num1 != 0)
            break;
        }
        int num5 = 17 - (num2 + 1);
        if (num5 > matchLength)
        {
          matchLength = num5;
          match = index2;
          if (matchLength == 17)
          {
            this.nodes[node] = this.nodes[index2];
            this.nodes[index2].Parent = (short) -1;
            if ((int) this.nodes[node].LeftChild >= 0)
              this.nodes[(int) this.nodes[node].LeftChild].Parent = (short) node;
            if ((int) this.nodes[node].RightChild >= 0)
              this.nodes[(int) this.nodes[node].RightChild].Parent = (short) node;
            if (flag)
            {
              this.nodes[index1].LeftChild = (short) node;
              return;
            }
            this.nodes[index1].RightChild = (short) node;
            return;
          }
        }
        index1 = index2;
        flag = num1 < 0;
      }
      this.nodes[node].LeftChild = (short) -1;
      this.nodes[node].RightChild = (short) -1;
      this.nodes[node].Parent = (short) index1;
      if (flag)
        this.nodes[index1].LeftChild = (short) node;
      else
        this.nodes[index1].RightChild = (short) node;
    }

    private void PrepareHeader()
    {
      this.header = new byte[16];
      RtfCompressCommon.ToBytes((uint) (this.outputDataSize + 12), this.header, 0);
      RtfCompressCommon.ToBytes((uint) this.inputDataSize, this.header, 4);
      RtfCompressCommon.ToBytes(this.uncompressed ? 1095517517U : 1967544908U, this.header, 8);
      RtfCompressCommon.ToBytes(this.crc, this.header, 12);
    }

    private void RewriteHeader()
    {
      long position = this.pushSink.Position;
      this.pushSink.Position = this.initialStreamPosition;
      this.pushSink.Write(this.header, 0, 16);
      this.pushSink.Position = position;
    }

    private void CopyInputChunkTo(Stream destinationStream)
    {
      int count = this.readEnd - this.readCurrent;
      this.outputDataSize += count;
      this.inputDataSize += count;
      if (destinationStream != null)
        destinationStream.Write(this.readBuffer, this.readCurrent, count);
      this.readCurrent = this.readEnd;
    }

    private void CopyBufferedData()
    {
      if (this.flushOffset < 16)
      {
        int count = Math.Min(16 - this.flushOffset, this.writeEnd - this.writeCurrent);
        Buffer.BlockCopy((Array) this.header, this.flushOffset, (Array) this.writeBuffer, this.writeCurrent, count);
        this.writeCurrent += count;
        this.flushOffset += count;
        if (this.flushOffset < 16)
          return;
        if (this.bufferStream == null)
          this.pullSource.Position = this.initialStreamPosition;
        else
          this.bufferStream.Position = 0L;
      }
      if (this.writeEnd == this.writeCurrent)
        return;
      int num = this.bufferStream == null ? this.pullSource.Read(this.writeBuffer, this.writeCurrent, this.writeEnd - this.writeCurrent) : this.bufferStream.Read(this.writeBuffer, this.writeCurrent, this.writeEnd - this.writeCurrent);
      if (num == 0)
      {
        this.inputEndOfFile = true;
      }
      else
      {
        this.writeCurrent += num;
        this.flushOffset += num;
      }
    }

    private bool GetTempOutputSpace()
    {
      this.writeCurrent = 0;
      return true;
    }

    private void FlushTempOutput()
    {
      this.writeFileOffset += this.writeCurrent - this.writeStart;
      this.bufferStream.Write(this.writeBuffer, this.writeStart, this.writeCurrent - this.writeStart);
      this.writeCurrent = this.writeEnd;
    }

    private struct Node
    {
      public short Parent;
      public short LeftChild;
      public short RightChild;
    }
  }
}
