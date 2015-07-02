// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.RtfCompressed.RtfCompressCommon
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.RtfCompressed
{
  internal class RtfCompressCommon
  {
    protected static readonly byte[] PreloadData = CTSGlobals.AsciiEncoding.GetBytes("{\\rtf1\\ansi\\mac\\deff0\\deftab720{\\fonttbl;}{\\f0\\fnil \\froman \\fswiss \\fmodern \\fscript \\fdecor MS Sans SerifSymbolArialTimes New RomanCourier{\\colortbl\\red0\\green0\\blue0\r\n\\par \\pard\\plain\\f0\\fs20\\b\\i\\u\\tab\\tx");
    protected const uint MagicCompressed = 1967544908U;
    protected const uint MagicUncompressed = 1095517517U;
    protected const int WindowSize = 4096;
    protected const int LookAheadMost = 17;
    protected const int MaxExpansion = 17;
    protected const int BreakEven = 1;
    protected bool endOfFile;
    protected ConverterStream pushSource;
    protected Stream pullSource;
    protected bool inputEndOfFile;
    protected Stream pushSink;
    protected ConverterStream pullSink;
    protected int readFileOffset;
    protected int writeFileOffset;
    protected byte[] readBuffer;
    protected int readStart;
    protected int readCurrent;
    protected int readEnd;
    protected byte[] writeBuffer;
    protected int writeStart;
    protected int writeCurrent;
    protected int writeEnd;
    protected byte[] window;
    protected int windowCurrent;

    protected RtfCompressCommon(Stream input, bool push, Stream output, int inputBufferSize, int outputBufferSize)
    {
      if (push)
      {
        this.pushSource = input as ConverterStream;
        this.pushSink = output;
        this.writeBuffer = new byte[outputBufferSize];
        this.writeEnd = this.writeBuffer.Length;
        this.writeCurrent = this.writeStart;
      }
      else
      {
        this.pullSource = input;
        this.pullSink = output as ConverterStream;
        this.readBuffer = new byte[inputBufferSize];
        this.readEnd = this.readBuffer.Length;
        this.readCurrent = this.readEnd;
      }
    }

    protected static void ToBytes(uint value, byte[] buffer, int offset)
    {
      buffer[offset++] = (byte) (value & (uint) byte.MaxValue);
      buffer[offset++] = (byte) ((value & 65280U) >> 8);
      buffer[offset++] = (byte) ((value & 16711680U) >> 16);
      buffer[offset++] = (byte) ((value & 4278190080U) >> 24);
    }

    protected bool ReadMoreData()
    {
      if (this.pushSource != null)
      {
        int chunkCount;
        if (!this.pushSource.GetInputChunk(out this.readBuffer, out this.readStart, out chunkCount, out this.inputEndOfFile))
        {
          this.readEnd = this.readCurrent;
          return false;
        }
        this.readCurrent = this.readStart;
        this.readEnd = this.readCurrent + chunkCount;
      }
      else
      {
        this.readStart = 0;
        this.readCurrent = this.readStart;
        this.readEnd = this.pullSource.Read(this.readBuffer, 0, this.readBuffer.Length);
        if (this.readEnd == 0)
          this.inputEndOfFile = true;
      }
      return true;
    }

    protected void ReportRead()
    {
      this.readFileOffset += this.readCurrent - this.readStart;
      if (this.pushSource != null)
      {
        this.pushSource.ReportRead(this.readCurrent - this.readStart);
        this.readStart = this.readCurrent;
      }
      else
        this.readStart = this.readCurrent;
    }

    protected bool GetOutputSpace()
    {
      if (this.pullSink != null)
      {
        int outputCount;
        this.pullSink.GetOutputBuffer(out this.writeBuffer, out this.writeCurrent, out outputCount);
        this.writeStart = this.writeCurrent;
        this.writeEnd = this.writeCurrent + outputCount;
        if (outputCount == 0)
          return false;
      }
      else
        this.writeCurrent = 0;
      return true;
    }

    protected void FlushOutput()
    {
      this.writeFileOffset += this.writeCurrent - this.writeStart;
      if (this.pullSink != null)
      {
        this.pullSink.ReportOutput(this.writeCurrent - this.writeStart);
        this.writeStart = this.writeCurrent;
      }
      else
      {
        this.pushSink.Write(this.writeBuffer, this.writeStart, this.writeCurrent - this.writeStart);
        this.writeCurrent = this.writeEnd;
      }
    }
  }
}
