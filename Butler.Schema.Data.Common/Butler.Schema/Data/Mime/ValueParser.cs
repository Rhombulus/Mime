// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.ValueParser
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  internal struct ValueParser
  {
    private static readonly MimeString SpaceLine = new MimeString(" ");
    private MimeStringList lines;
    private int nextLine;
    private byte[] bytes;
    private int start;
    private int end;
    private int position;
    private readonly bool allowUTF8;

    private bool Eof => this.nextLine >= this.lines.Count;

      public ValueParser(MimeStringList lines, bool allowUTF8)
    {
      this.lines = lines;
      this.allowUTF8 = allowUTF8;
      this.nextLine = 0;
      this.bytes = (byte[]) null;
      this.start = 0;
      this.end = 0;
      this.position = 0;
      this.ParseNextLine();
    }

    public ValueParser(MimeStringList lines, ValueParser valueParser)
    {
      this.lines = lines;
      this.allowUTF8 = valueParser.allowUTF8;
      this.nextLine = valueParser.nextLine;
      if (this.nextLine > 0 && this.nextLine <= this.lines.Count)
      {
        int count;
        this.bytes = this.lines[this.nextLine - 1].GetData(out this.start, out count);
        this.start = valueParser.start;
        this.position = valueParser.position;
        this.end = valueParser.end;
      }
      else
      {
        this.bytes = (byte[]) null;
        this.start = 0;
        this.end = 0;
        this.position = 0;
      }
    }

    public static int ParseToken(string value, int currentOffset, bool allowUTF8)
    {
      return MimeScan.FindEndOf(MimeScan.Token.Token, value, currentOffset, allowUTF8);
    }

    public static int ParseToken(MimeString str, out int characterCount, bool allowUTF8)
    {
      return MimeScan.FindEndOf(MimeScan.Token.Token, str.Data, str.Offset, str.Length, out characterCount, allowUTF8);
    }

    public bool ParseToDelimiter(bool ignoreNextByte, bool separateWithWhitespace, ref MimeStringList phrase)
    {
      bool flag = false;
      int num = ignoreNextByte ? 1 : 0;
      while (true)
      {
        int characterCount = 0;
        int count = num + MimeScan.FindEndOf(MimeScan.Token.Atom, this.bytes, this.position + num, this.end - this.position - num, out characterCount, this.allowUTF8);
        if (count != 0)
        {
          flag = true;
          if (phrase.Length != 0 && separateWithWhitespace)
          {
            if (this.position == this.start || (int) this.bytes[this.position - 1] != 32)
            {
              phrase.AppendFragment(ValueParser.SpaceLine);
            }
            else
            {
              --this.position;
              ++count;
            }
          }
          separateWithWhitespace = false;
          phrase.AppendFragment(new MimeString(this.bytes, this.position, count));
          this.position += count;
        }
        if (this.position == this.end && this.ParseNextLine())
          num = 0;
        else
          break;
      }
      return flag;
    }

    public byte ParseGet()
    {
      if (this.position == this.end && !this.ParseNextLine())
        return (byte) 0;
      return this.bytes[this.position++];
    }

    public void ParseUnget()
    {
      if (this.position == this.start)
        this.ParseUngetLine();
      --this.position;
    }

    public void ParseQString(bool save, ref MimeStringList phrase, bool handleISO2022)
    {
      bool quotedPair = false;
      if (save)
        phrase.AppendFragment(new MimeString(this.bytes, this.position, 1, 268435456U));
      ++this.position;
      bool singleByte = true;
      do
      {
        int count = MimeScan.ScanQuotedString(this.bytes, this.position, this.end - this.position, handleISO2022, ref quotedPair);
        if (count != 0)
        {
          if (save)
            phrase.AppendFragment(new MimeString(this.bytes, this.position, count));
          this.position += count;
        }
        if (this.position != this.end)
        {
          if ((int) this.bytes[this.position] == 14 || (int) this.bytes[this.position] == 27)
          {
            this.ParseEscapedString(save, ref phrase, out singleByte);
          }
          else
          {
            if (save)
              phrase.AppendFragment(new MimeString(this.bytes, this.position, 1, 268435456U));
            ++this.position;
            if ((int) this.bytes[this.position - 1] == 34)
              return;
            quotedPair = true;
          }
        }
      }
      while (this.ParseNextLine());
      if (!save || !singleByte)
        return;
      phrase.AppendFragment(new MimeString(MimeString.DoubleQuote, 0, MimeString.DoubleQuote.Length, 268435456U));
    }

    public void ParseComment(bool save, bool saveInnerOnly, ref MimeStringList comment, bool handleISO2022)
    {
      int level = 1;
      bool quotedPair = false;
      int num1 = 0;
      if (save && !saveInnerOnly)
        comment.AppendFragment(new MimeString(this.bytes, this.position, 1));
      ++this.position;
      do
      {
        int num2 = MimeScan.ScanComment(this.bytes, this.position, this.end - this.position, handleISO2022, ref level, ref quotedPair);
        if (num2 != 0)
        {
          if (save)
          {
            if (level == 0 && saveInnerOnly)
              num1 = 1;
            comment.AppendFragment(new MimeString(this.bytes, this.position, num2 - num1));
          }
          this.position += num2;
          if (level == 0)
            break;
        }
        if (this.position != this.end && ((int) this.bytes[this.position] == 14 || (int) this.bytes[this.position] == 27))
        {
          bool singleByte;
          this.ParseEscapedString(save, ref comment, out singleByte);
        }
      }
      while (this.ParseNextLine());
    }

    public bool ParseNextLine()
    {
      if (this.nextLine >= this.lines.Count)
        return false;
      int count;
      this.bytes = this.lines[this.nextLine].GetData(out this.start, out count);
      this.position = this.start;
      this.end = this.start + count;
      ++this.nextLine;
      return true;
    }

    public void ParseUngetLine()
    {
      int count;
      this.bytes = this.lines[this.nextLine - 2].GetData(out this.start, out count);
      this.position = this.end = this.start + count;
      --this.nextLine;
    }

    public void ParseWhitespace(bool save, ref MimeStringList phrase)
    {
      do
      {
        int count = MimeScan.SkipLwsp(this.bytes, this.position, this.end - this.position);
        if (save && count != 0)
          phrase.AppendFragment(new MimeString(this.bytes, this.position, count));
        this.position += count;
      }
      while (this.position == this.end && this.ParseNextLine());
    }

    public void ParseCFWS(bool save, ref MimeStringList phrase, bool handleISO2022)
    {
      do
      {
        int count = MimeScan.SkipLwsp(this.bytes, this.position, this.end - this.position);
        if (save && count != 0)
          phrase.AppendFragment(new MimeString(this.bytes, this.position, count));
        this.position += count;
        if (this.position != this.end)
        {
          if ((int) this.bytes[this.position] == 40)
            this.ParseComment(save, false, ref phrase, handleISO2022);
          else
            goto label_6;
        }
      }
      while (this.ParseNextLine());
      goto label_7;
label_6:
      return;
label_7:;
    }

    public void ParseSkipToNextDelimiterByte(byte delimiter)
    {
      MimeStringList mimeStringList = new MimeStringList();
      while (true)
      {
        while (this.position == this.end)
        {
          if (!this.ParseNextLine())
            return;
        }
        byte num = this.bytes[this.position];
        if ((int) num != (int) delimiter)
        {
          if ((int) num == 34)
            this.ParseQString(false, ref mimeStringList, true);
          else if ((int) num == 40)
          {
            this.ParseComment(false, false, ref mimeStringList, true);
          }
          else
          {
            ++this.position;
            this.ParseCFWS(false, ref mimeStringList, true);
            int characterCount = 0;
            this.position += MimeScan.FindEndOf(MimeScan.Token.Atom, this.bytes, this.position, this.end - this.position, out characterCount, this.allowUTF8);
          }
        }
        else
          break;
      }
    }

    public MimeString ParseToken()
    {
      return this.ParseToken(MimeScan.Token.Token);
    }

    public MimeString ParseToken(MimeScan.Token token)
    {
      MimeStringList mimeStringList = new MimeStringList();
      while (this.position != this.end || this.ParseNextLine())
      {
        int characterCount = 0;
        int endOf = MimeScan.FindEndOf(token, this.bytes, this.position, this.end - this.position, out characterCount, this.allowUTF8);
        if (endOf != 0)
        {
          mimeStringList.AppendFragment(new MimeString(this.bytes, this.position, endOf));
          this.position += endOf;
        }
        else
          break;
      }
      if (mimeStringList.Count == 0)
        return new MimeString();
      if (mimeStringList.Count == 1)
        return mimeStringList[0];
      byte[] sz = mimeStringList.GetSz();
      return new MimeString(sz, 0, sz.Length);
    }

    public void ParseParameterValue(ref MimeStringList value, ref bool goodValue, bool handleISO2022)
    {
      MimeStringList mimeStringList = new MimeStringList();
      goodValue = true;
      while (this.position != this.end || this.ParseNextLine())
      {
        byte ch = this.bytes[this.position];
        switch (ch)
        {
          case (byte) 34:
            value.Reset();
            mimeStringList.Reset();
            this.ParseQString(true, ref value, handleISO2022);
            return;
          case (byte) 40:
            this.ParseCFWS(true, ref mimeStringList, handleISO2022);
            continue;
          default:
            if (!MimeScan.IsLWSP(ch))
            {
              if ((int) ch == 59)
                return;
              int offset = this.position;
              do
              {
                int bytesUsed = 1;
                if (!MimeScan.IsToken(ch))
                {
                  if (this.allowUTF8 && (int) ch >= 128)
                  {
                    if (!MimeScan.IsUTF8NonASCII(this.bytes, this.position, this.end, out bytesUsed))
                    {
                      bytesUsed = 1;
                      goodValue = false;
                    }
                  }
                  else
                    goodValue = false;
                }
                this.position += bytesUsed;
                if (this.position != this.end)
                {
                  ch = this.bytes[this.position];
                  switch (ch)
                  {
                    case (byte) 59:
                    case (byte) 40:
                      goto label_17;
                    default:
                      continue;
                  }
                }
                else
                  break;
              }
              while (!MimeScan.IsLWSP(ch));
label_17:
              value.TakeOverAppend(ref mimeStringList);
              value.AppendFragment(new MimeString(this.bytes, offset, this.position - offset));
              continue;
            }
            goto case (byte) 40;
        }
      }
    }

    public void ParseDomainLiteral(bool save, ref MimeStringList domain)
    {
      bool flag = false;
      int offset = this.position;
      ++this.position;
      byte num;
      do
      {
        if (this.position == this.end)
        {
          if (offset != this.position && save)
            domain.AppendFragment(new MimeString(this.bytes, offset, this.position - offset));
          if (!this.ParseNextLine())
          {
            offset = this.position;
            break;
          }
          offset = this.position;
        }
        num = this.bytes[this.position++];
        if (flag)
          flag = false;
        else if ((int) num == 92)
          flag = true;
      }
      while ((int) num != 93);
      if (offset == this.position || !save)
        return;
      domain.AppendFragment(new MimeString(this.bytes, offset, this.position - offset));
    }

    public void ParseToEnd(ref MimeStringList phrase)
    {
      if (this.position != this.end)
      {
        phrase.AppendFragment(new MimeString(this.bytes, this.position, this.end - this.position));
        this.position = this.end;
      }
      while (this.ParseNextLine())
      {
        phrase.AppendFragment(new MimeString(this.bytes, this.start, this.end - this.start));
        this.position = this.end;
      }
    }

    public void ParseAppendLastByte(ref MimeStringList phrase)
    {
      phrase.AppendFragment(new MimeString(this.bytes, this.position - 1, 1));
    }

    public void ParseAppendSpace(ref MimeStringList phrase)
    {
      if (this.position == this.start || (int) this.bytes[this.position - 1] != 32)
        phrase.AppendFragment(ValueParser.SpaceLine);
      else
        phrase.AppendFragment(new MimeString(this.bytes, this.position - 1, 1));
    }

    private void ParseEscapedString(bool save, ref MimeStringList outStr, out bool singleByte)
    {
      bool flag = (int) this.bytes[this.position] == 27;
      if (save)
        outStr.AppendFragment(new MimeString(this.bytes, this.position, 1, 536870912U));
      ++this.position;
      if (flag && !this.ParseEscapeSequence(save, ref outStr))
      {
        singleByte = true;
      }
      else
      {
        singleByte = false;
        do
        {
          int count = MimeScan.ScanJISString(this.bytes, this.position, this.end - this.position, ref singleByte);
          if (save && count != 0)
            outStr.AppendFragment(new MimeString(this.bytes, this.position, count, 536870912U));
          this.position += count;
        }
        while (!singleByte && this.ParseNextLine());
        if (flag || this.position == this.end || (int) this.bytes[this.position] != 15)
          return;
        if (save)
          outStr.AppendFragment(new MimeString(this.bytes, this.position, 1, 536870912U));
        ++this.position;
      }
    }

    private bool ParseEscapeSequence(bool save, ref MimeStringList outStr)
    {
      byte num1 = this.ParseGet();
      byte num2 = this.ParseGet();
      byte num3 = this.ParseGet();
      if ((int) num3 != 0)
        this.ParseUnget();
      if ((int) num2 != 0)
        this.ParseUnget();
      if ((int) num1 != 0)
        this.ParseUnget();
      int num4 = 0;
      bool flag = false;
      switch (num1)
      {
        case (byte) 36:
          if ((int) num2 == 66 || (int) num2 == 65 || (int) num2 == 64)
          {
            num4 = 2;
            flag = true;
            break;
          }
          if ((int) num2 == 40 && ((int) num3 == 67 || (int) num3 == 68))
          {
            num4 = 3;
            flag = true;
            break;
          }
          break;
        case (byte) 40:
          if ((int) num2 == 73)
          {
            flag = true;
            num4 = 2;
            break;
          }
          if ((int) num2 == 66 || (int) num2 == 74 || (int) num2 == 72)
          {
            num4 = 2;
            break;
          }
          break;
        case (byte) 78:
        case (byte) 79:
          if ((int) num2 >= 33)
          {
            num4 = 2;
            if ((int) num3 >= 33)
            {
              num4 = 3;
              break;
            }
            break;
          }
          break;
      }
      while (num4-- != 0)
      {
        int num5 = (int) this.ParseGet();
        if (save)
          outStr.AppendFragment(new MimeString(this.bytes, this.position - 1, 1, 536870912U));
      }
      return flag;
    }
  }
}
