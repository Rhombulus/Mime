// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.ValueIterator
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  internal struct ValueIterator
  {
    private MimeStringList lines;
    private uint linesMask;
    private int currentLine;
    private int currentOffset;
    private int lineStart;
    private int lineEnd;
    private byte[] lineBytes;
    private int endLine;
    private int endOffset;

    public ValuePosition CurrentPosition => new ValuePosition(this.currentLine, this.currentOffset);

      public byte[] Bytes => this.lineBytes;

      public int Offset => this.currentOffset;

      public int Length => this.lineEnd - this.currentOffset;

      public int TotalLength => this.lines.Length;

      public MimeStringList Lines => this.lines;

      public uint LinesMask => this.linesMask;

      public bool Eof
    {
      get
      {
        if (this.currentLine == this.endLine)
          return this.currentOffset == this.lineEnd;
        return false;
      }
    }

    public ValueIterator(MimeStringList lines, uint linesMask)
    {
      this.lines = lines;
      this.linesMask = linesMask;
      this.lineStart = this.lineEnd = this.currentLine = this.currentOffset = 0;
      this.lineBytes = (byte[]) null;
      this.endLine = this.lines.Count;
      this.endOffset = 0;
      for (; this.currentLine != this.endLine; ++this.currentLine)
      {
        MimeString mimeString = this.lines[this.currentLine];
        if (((int) mimeString.Mask & (int) this.linesMask) != 0)
        {
          int count;
          this.lineBytes = mimeString.GetData(out this.lineStart, out count);
          this.lineEnd = this.lineStart + count;
          this.currentOffset = this.lineStart;
          break;
        }
      }
    }

    public ValueIterator(MimeStringList lines, uint linesMask, ValuePosition startPosition, ValuePosition endPosition)
    {
      this.lines = lines;
      this.linesMask = linesMask;
      this.currentLine = startPosition.Line;
      this.currentOffset = startPosition.Offset;
      this.endLine = endPosition.Line;
      this.endOffset = endPosition.Offset;
      if (startPosition != endPosition)
      {
        int count;
        this.lineBytes = this.lines[this.currentLine].GetData(out this.lineStart, out count);
        this.lineEnd = this.currentLine == this.endLine ? this.endOffset : this.lineStart + count;
      }
      else
      {
        this.lineStart = this.lineEnd = this.currentOffset;
        this.lineBytes = (byte[]) null;
      }
    }

    public void Get(int length)
    {
      this.currentOffset += length;
      if (this.currentOffset != this.lineEnd)
        return;
      this.NextLine();
    }

    public int Get()
    {
      if (this.Eof)
        return -1;
      byte num = this.lineBytes[this.currentOffset++];
      if (this.currentOffset == this.lineEnd)
        this.NextLine();
      return (int) num;
    }

    public int Pick()
    {
      if (this.Eof)
        return -1;
      return (int) this.lineBytes[this.currentOffset];
    }

    public void Unget()
    {
      if (this.currentOffset == this.lineStart)
      {
        MimeString mimeString;
        do
        {
          mimeString = this.lines[--this.currentLine];
        }
        while (((int) mimeString.Mask & (int) this.linesMask) == 0);
        int count;
        this.lineBytes = mimeString.GetData(out this.lineStart, out count);
        this.currentOffset = this.lineEnd = this.lineStart + count;
      }
      --this.currentOffset;
    }

    public void SkipToEof()
    {
      while (!this.Eof)
        this.Get(this.Length);
    }

    private void NextLine()
    {
      if (this.Eof)
        return;
      MimeString mimeString;
      do
      {
        ++this.currentLine;
        if (this.currentLine == this.lines.Count)
        {
          this.lineEnd = this.lineStart = this.currentOffset = 0;
          return;
        }
        mimeString = this.lines[this.currentLine];
      }
      while (((int) mimeString.Mask & (int) this.linesMask) == 0);
      int count;
      this.lineBytes = mimeString.GetData(out this.lineStart, out count);
      this.currentOffset = this.lineStart;
      this.lineEnd = this.currentLine == this.endLine ? this.endOffset : this.lineStart + count;
    }
  }
}
