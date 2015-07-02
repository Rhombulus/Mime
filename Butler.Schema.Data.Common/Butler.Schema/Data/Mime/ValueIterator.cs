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

      private int currentLine;
      private int lineStart;
    private int lineEnd;
      private int endLine;
    private int endOffset;

    public ValuePosition CurrentPosition => new ValuePosition(this.currentLine, this.Offset);

      public byte[] Bytes { get; private set; }

      public int Offset { get; private set; }

      public int Length => this.lineEnd - this.Offset;

      public int TotalLength => this.Lines.Length;

      public MimeStringList Lines { get; }

      public uint LinesMask { get; }

      public bool Eof
    {
      get
      {
        if (this.currentLine == this.endLine)
          return this.Offset == this.lineEnd;
        return false;
      }
    }

    public ValueIterator(MimeStringList lines, uint linesMask)
    {
      this.Lines = lines;
      this.LinesMask = linesMask;
      this.lineStart = this.lineEnd = this.currentLine = this.Offset = 0;
      this.Bytes = (byte[]) null;
      this.endLine = this.Lines.Count;
      this.endOffset = 0;
      for (; this.currentLine != this.endLine; ++this.currentLine)
      {
        MimeString mimeString = this.Lines[this.currentLine];
        if (((int) mimeString.Mask & (int) this.LinesMask) != 0)
        {
          int count;
          this.Bytes = mimeString.GetData(out this.lineStart, out count);
          this.lineEnd = this.lineStart + count;
          this.Offset = this.lineStart;
          break;
        }
      }
    }

    public ValueIterator(MimeStringList lines, uint linesMask, ValuePosition startPosition, ValuePosition endPosition)
    {
      this.Lines = lines;
      this.LinesMask = linesMask;
      this.currentLine = startPosition.Line;
      this.Offset = startPosition.Offset;
      this.endLine = endPosition.Line;
      this.endOffset = endPosition.Offset;
      if (startPosition != endPosition)
      {
        int count;
        this.Bytes = this.Lines[this.currentLine].GetData(out this.lineStart, out count);
        this.lineEnd = this.currentLine == this.endLine ? this.endOffset : this.lineStart + count;
      }
      else
      {
        this.lineStart = this.lineEnd = this.Offset;
        this.Bytes = (byte[]) null;
      }
    }

    public void Get(int length)
    {
      this.Offset += length;
      if (this.Offset != this.lineEnd)
        return;
      this.NextLine();
    }

    public int Get()
    {
      if (this.Eof)
        return -1;
      byte num = this.Bytes[this.Offset++];
      if (this.Offset == this.lineEnd)
        this.NextLine();
      return (int) num;
    }

    public int Pick()
    {
      if (this.Eof)
        return -1;
      return (int) this.Bytes[this.Offset];
    }

    public void Unget()
    {
      if (this.Offset == this.lineStart)
      {
        MimeString mimeString;
        do
        {
          mimeString = this.Lines[--this.currentLine];
        }
        while (((int) mimeString.Mask & (int) this.LinesMask) == 0);
        int count;
        this.Bytes = mimeString.GetData(out this.lineStart, out count);
        this.Offset = this.lineEnd = this.lineStart + count;
      }
      --this.Offset;
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
        if (this.currentLine == this.Lines.Count)
        {
          this.lineEnd = this.lineStart = this.Offset = 0;
          return;
        }
        mimeString = this.Lines[this.currentLine];
      }
      while (((int) mimeString.Mask & (int) this.LinesMask) == 0);
      int count;
      this.Bytes = mimeString.GetData(out this.lineStart, out count);
      this.Offset = this.lineStart;
      this.lineEnd = this.currentLine == this.endLine ? this.endOffset : this.lineStart + count;
    }
  }
}
