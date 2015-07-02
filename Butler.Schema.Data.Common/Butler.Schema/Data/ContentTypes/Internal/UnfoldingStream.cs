// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.Internal.UnfoldingStream
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.ContentTypes.Internal
{
  internal class UnfoldingStream : Stream
  {
    private Stream inputStream;
    private byte[] unprocessedBuffer;
    private Stack<int> foldingIndices;
    private int bufferIdx;
    private int bufferCount;
    private bool endOfInputStream;
    private bool isClosed;

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
        throw new NotSupportedException();
      }
      set
      {
        this.CheckDisposed("Position:set");
        throw new NotSupportedException();
      }
    }

    public UnfoldingStream(Stream inputStream)
    {
      this.inputStream = inputStream;
      this.unprocessedBuffer = new byte[1024];
      this.foldingIndices = new Stack<int>();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && !this.isClosed && this.inputStream != null)
      {
        this.inputStream.Dispose();
        this.inputStream = (Stream) null;
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
      if (!this.endOfInputStream && this.bufferIdx >= this.bufferCount - 3)
      {
        if (this.bufferIdx != 0)
        {
          for (int index = 0; index < this.bufferCount - this.bufferIdx; ++index)
            this.unprocessedBuffer[index] = this.unprocessedBuffer[this.bufferIdx + index];
        }
        this.bufferCount -= this.bufferIdx;
        this.bufferIdx = 0;
        this.foldingIndices.Clear();
        int num = this.inputStream.Read(this.unprocessedBuffer, this.bufferCount, this.unprocessedBuffer.Length - this.bufferCount);
        if (num == 0)
          this.endOfInputStream = true;
        this.bufferCount += num;
      }
      int num1 = 0;
      int num2 = this.bufferCount - this.bufferIdx;
      while (count > 0 && num2 > 0 && (num2 >= 3 || this.endOfInputStream))
      {
        if (num2 >= 3 && (int) this.unprocessedBuffer[this.bufferIdx] == 13 && (int) this.unprocessedBuffer[this.bufferIdx + 1] == 10 && ((int) this.unprocessedBuffer[this.bufferIdx + 2] == 32 || (int) this.unprocessedBuffer[this.bufferIdx + 2] == 9))
        {
          this.foldingIndices.Push(this.bufferIdx);
          this.bufferIdx += 3;
          num2 -= 3;
        }
        else
        {
          buffer[offset++] = this.unprocessedBuffer[this.bufferIdx++];
          --count;
          --num2;
          ++num1;
        }
      }
      if (num1 == 0 && !this.endOfInputStream)
        return this.Read(buffer, offset, count);
      return num1;
    }

    public void Rewind(int countBytes)
    {
      this.bufferIdx -= countBytes;
      while (this.foldingIndices.Count > 0 && this.foldingIndices.Peek() > this.bufferIdx)
      {
        this.foldingIndices.Pop();
        this.bufferIdx -= 3;
      }
      if (this.bufferIdx < 0)
        throw new InvalidOperationException();
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

    private void CheckDisposed(string methodName)
    {
      if (this.isClosed)
        throw new ObjectDisposedException("UnfoldingStream", methodName);
    }
  }
}
