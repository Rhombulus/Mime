// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Format.TextRun
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters.Internal.Format
{
  internal struct TextRun
  {
    public static readonly TextRun Invalid = new TextRun();
    public const int MaxEffectiveLength = 4095;
    private FormatStore.TextStore text;
      private bool isImmutable;

    public uint Position { get; private set; }

      public TextRunType Type => FormatStore.TextStore.TypeFromRunHeader(this.text.Pick(this.Position));

      public int EffectiveLength => FormatStore.TextStore.LengthFromRunHeader(this.text.Pick(this.Position));

      public int Length
    {
      get
      {
        char runHeader = this.text.Pick(this.Position);
        if ((int) runHeader < 12288)
          return FormatStore.TextStore.LengthFromRunHeader(runHeader) + 1;
        return 1;
      }
    }

    public int WordLength
    {
      get
      {
        int num = 0;
        for (TextRun textRun = this; !textRun.IsEnd() && textRun.Type == TextRunType.NonSpace && num < 1024; textRun = textRun.GetNext())
          num += textRun.EffectiveLength;
        return num;
      }
    }

    private bool IsLong => this.Type < TextRunType.FirstShort;

      public char this[int index] => this.text.Plane(this.Position)[this.text.Index(this.Position) + 1 + index];

      internal TextRun(FormatStore.TextStore text, uint position)
    {
      this.isImmutable = false;
      this.text = text;
      this.Position = position;
    }

    public char GetWordChar(int index)
    {
      int effectiveLength1 = this.EffectiveLength;
      if (index < effectiveLength1)
        return this[index];
      TextRun next = this.GetNext();
      index -= effectiveLength1;
      for (; !next.IsEnd(); next = next.GetNext())
      {
        int effectiveLength2 = next.EffectiveLength;
        if (index < effectiveLength2)
          return next[index];
        if (next.Type == TextRunType.NonSpace)
          index -= effectiveLength2;
        else
          break;
      }
      throw new ArgumentOutOfRangeException("index");
    }

    public void MoveNext()
    {
      if (this.isImmutable)
        throw new InvalidOperationException("This run is immutable");
      this.Position += (uint) this.Length;
    }

    public void SkipInvalid()
    {
      if (this.isImmutable)
        throw new InvalidOperationException("This run is immutable");
      while (!this.IsEnd() && this.Type == TextRunType.Invalid)
        this.MoveNext();
    }

    public bool IsEnd()
    {
      return this.Position >= this.text.CurrentPosition;
    }

    public TextRun GetNext()
    {
      return new TextRun(this.text, this.Position + (uint) this.Length);
    }

    public void GetChunk(int start, out char[] buffer, out int offset, out int count)
    {
      buffer = this.text.Plane(this.Position);
      offset = this.text.Index(this.Position) + 1 + start;
      count = this.EffectiveLength - start;
    }

    public int AppendFragment(int start, ref ScratchBuffer scratchBuffer, int maxLength)
    {
      int offset = this.text.Index(this.Position) + 1 + start;
      int length = Math.Min(this.EffectiveLength - start, maxLength);
      if (length != 0)
        scratchBuffer.Append(this.text.Plane(this.Position), offset, length);
      return length;
    }

    public void ConvertToInvalid()
    {
      this.text.ConvertToInvalid(this.Position);
    }

    public void ConvertToInvalid(int count)
    {
      this.text.ConvertToInvalid(this.Position, count);
    }

    public void ConvertShort(TextRunType type, int newEffectiveLength)
    {
      this.text.ConvertShortRun(this.Position, type, newEffectiveLength);
    }

    public override string ToString()
    {
      int wordLength = this.WordLength;
      StringBuilder stringBuilder = new StringBuilder(wordLength);
      for (int index = 0; index < wordLength; ++index)
        stringBuilder.Append(this.GetWordChar(index));
      return stringBuilder.ToString();
    }

    internal void MakeImmutable()
    {
      this.isImmutable = true;
    }
  }
}
