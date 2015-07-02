// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.ContentTypes.Internal.ContentLineWriter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.ContentTypes.Internal
{
  internal class ContentLineWriter : IDisposable
  {
    private ContentLineWriteState state = ContentLineWriteState.Start;
    private const string FoldingTagString = "\r\n ";
    private ContentLineWriter.FoldingTextWriter foldingTextWriter;
    private bool emptyParamName;

    public ContentLineWriteState State => this.state;

      public ContentLineWriter(Stream s, Encoding encoding)
    {
      this.foldingTextWriter = new ContentLineWriter.FoldingTextWriter(s, encoding, "\r\n ");
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing && this.foldingTextWriter != null)
      {
        this.Flush();
        this.foldingTextWriter.Dispose();
        this.foldingTextWriter = (ContentLineWriter.FoldingTextWriter) null;
      }
      this.state = ContentLineWriteState.Closed;
    }

    public void Flush()
    {
      this.foldingTextWriter.Flush();
    }

    public void WriteProperty(string property, string data)
    {
      this.AssertValidState(ContentLineWriteState.Start | ContentLineWriteState.PropertyEnd);
      this.WriteToStream(property + ":" + data + "\r\n");
      this.state = ContentLineWriteState.PropertyEnd;
    }

    public void StartProperty(string property)
    {
      this.AssertValidState(ContentLineWriteState.Start | ContentLineWriteState.PropertyEnd);
      this.WriteToStream(property);
      this.state = ContentLineWriteState.Property;
    }

    public void EndProperty()
    {
      this.AssertValidState(ContentLineWriteState.PropertyValue);
      this.WriteToStream("\r\n");
      this.state = ContentLineWriteState.PropertyEnd;
    }

    public void StartParameter(string parameter)
    {
      this.AssertValidState(ContentLineWriteState.Property | ContentLineWriteState.ParameterEnd);
      this.WriteToStream(";");
      if (parameter != null)
        this.WriteToStream(parameter);
      this.emptyParamName = parameter == null;
      this.state = ContentLineWriteState.Parameter;
    }

    public void EndParameter()
    {
      this.AssertValidState(ContentLineWriteState.Parameter | ContentLineWriteState.ParameterValue);
      this.state = ContentLineWriteState.ParameterEnd;
    }

    public void WriteNextValue(ContentLineParser.Separators separator)
    {
      string data;
      if (separator == ContentLineParser.Separators.Comma)
      {
        data = ",";
      }
      else
      {
        if (separator != ContentLineParser.Separators.SemiColon)
          throw new ArgumentException();
        data = ";";
      }
      switch (this.state)
      {
        case ContentLineWriteState.PropertyValue:
        case ContentLineWriteState.ParameterValue:
          this.WriteToStream(data);
          break;
        default:
          throw new InvalidOperationException(CtsResources.CalendarStrings.InvalidState);
      }
    }

    public void WriteStartValue()
    {
      switch (this.state)
      {
        case ContentLineWriteState.ParameterEnd:
        case ContentLineWriteState.Property:
          this.WriteToStream(":");
          this.state = ContentLineWriteState.PropertyValue;
          break;
        case ContentLineWriteState.Parameter:
          if (!this.emptyParamName)
            this.WriteToStream("=");
          this.state = ContentLineWriteState.ParameterValue;
          break;
        default:
          throw new InvalidOperationException(CtsResources.CalendarStrings.InvalidState);
      }
    }

    public void WriteChars(char[] data, int offset, int size)
    {
      this.AssertValidState(ContentLineWriteState.PropertyValue | ContentLineWriteState.ParameterValue);
      this.foldingTextWriter.Write(data, offset, size);
    }

    internal void WriteToStream(byte[] data)
    {
      this.AssertValidState(ContentLineWriteState.PropertyValue);
      this.foldingTextWriter.Write(data, 0, data.Length);
    }

    internal void WriteToStream(byte[] data, int offset, int length)
    {
      this.AssertValidState(ContentLineWriteState.PropertyValue);
      this.foldingTextWriter.Write(data, offset, length);
    }

    internal void WriteToStream(byte data)
    {
      this.AssertValidState(ContentLineWriteState.PropertyValue | ContentLineWriteState.ParameterValue);
      this.foldingTextWriter.WriteByte(data);
    }

    internal void WriteToStream(string data)
    {
      this.foldingTextWriter.Write(data);
    }

    private void AssertValidState(ContentLineWriteState state)
    {
      if ((state & this.state) == (ContentLineWriteState) 0)
        throw new InvalidOperationException(CtsResources.CalendarStrings.InvalidStateForOperation);
    }

    private class FoldingTextWriter : TextWriter
    {
      private byte[] byteBuffer = new byte[10];
      private char[] charCheckerArray = new char[1];
      private const char CR = '\r';
      private const char LF = '\n';
      private const int MaxTextLength = 75;
      private Stream baseStream;
      private int linePosition;
      private byte[] foldingBytes;
      private Encoding encoding;
      private Encoder encoder;
      private Decoder decoder;
      private ContentLineWriter.FoldingTextWriter.States state;

      public override Encoding Encoding => this.encoding;

        public FoldingTextWriter(Stream s, Encoding encoding, string foldingString)
      {
        this.baseStream = s;
        this.encoding = encoding;
        this.encoder = this.encoding.GetEncoder();
        this.decoder = this.encoding.GetDecoder();
        this.foldingBytes = CTSGlobals.AsciiEncoding.GetBytes(foldingString);
      }

      private FoldingTextWriter()
      {
      }

      protected override void Dispose(bool disposing)
      {
        if (disposing && this.baseStream != null)
        {
          this.baseStream.Dispose();
          this.baseStream = (Stream) null;
        }
        base.Dispose(disposing);
      }

      public override void Flush()
      {
        this.baseStream.Flush();
      }

      public void Write(byte[] buffer, int offset, int count)
      {
        char[] chArray = new char[this.decoder.GetCharCount(buffer, offset, count, false)];
        this.decoder.GetChars(buffer, offset, count, chArray, 0);
        this.Write(chArray, 0, chArray.Length, buffer, count);
      }

      public override void Write(string data)
      {
        byte[] bytes = this.encoding.GetBytes(data);
        char[] chArray = new char[this.decoder.GetCharCount(bytes, 0, bytes.Length, false)];
        this.decoder.GetChars(bytes, 0, bytes.Length, chArray, 0);
        this.Write(chArray, 0, chArray.Length, bytes, bytes.Length);
      }

      public override void Write(char[] buffer, int offset, int count)
      {
        this.Write(buffer, offset, count, (byte[]) null, -1);
      }

      public void Write(char[] charBuffer, int charOffset, int charCount, byte[] buffer, int byteCount)
      {
        bool flag1 = false;
        int num1 = 1;
        int offset1 = 0;
        int index = 0;
        bool flush = false;
        if (buffer == null)
        {
          buffer = new byte[this.encoder.GetByteCount(charBuffer, charOffset, charCount, false)];
          this.encoder.GetBytes(charBuffer, charOffset, charCount, buffer, 0, false);
        }
        if (byteCount == -1)
          byteCount = buffer.Length;
        while (byteCount > 0 && index < charBuffer.Length)
        {
          switch (this.state)
          {
            case ContentLineWriter.FoldingTextWriter.States.Normal:
              if (flag1 || this.linePosition == 75)
              {
                if (flag1)
                  flag1 = false;
                if ((int) charBuffer[index] == 13)
                {
                  ++index;
                  this.state = ContentLineWriter.FoldingTextWriter.States.CR;
                  this.baseStream.WriteByte(buffer[offset1++]);
                  --byteCount;
                  continue;
                }
                this.baseStream.Write(this.foldingBytes, 0, this.foldingBytes.Length);
                this.linePosition = 1;
              }
              int num2 = Math.Min(75 - this.linePosition, byteCount);
              int offset2 = offset1;
              int codePage = Globalization.CodePageMap.GetCodePage(this.encoding);
              while (offset1 - offset2 < num2)
              {
                bool flag2 = false;
                if (index != charBuffer.Length)
                {
                  switch (codePage)
                  {
                    case 1200:
                      if (this.linePosition + offset1 - offset2 > 71 && char.IsHighSurrogate(charBuffer[index]))
                      {
                        flag1 = true;
                        break;
                      }
                      num1 = 2;
                      break;
                    case 65000:
                      if (!flush)
                      {
                        if (index == charBuffer.Length - 1)
                          flush = true;
                        num1 = this.WriteCharIntoBytes(charBuffer[index], flush);
                        break;
                      }
                      break;
                    case 65001:
                      if ((int) charBuffer[index] < 128)
                      {
                        num1 = 1;
                        break;
                      }
                      if ((int) charBuffer[index] < 2048)
                      {
                        num1 = 2;
                        break;
                      }
                      if (char.IsHighSurrogate(charBuffer[index]))
                      {
                        if (index < charBuffer.Length - 1 && char.IsLowSurrogate(charBuffer[index + 1]))
                        {
                          flag2 = true;
                          num1 = (int) buffer[offset1] >= 248 ? ((int) buffer[offset1] >= 252 ? 6 : 5) : 4;
                          break;
                        }
                        break;
                      }
                      num1 = !ContentLineWriter.FoldingTextWriter.IsInvalidUTF8Byte(charBuffer[index], buffer, offset1) ? 3 : 1;
                      break;
                    default:
                      num1 = this.WriteCharIntoBytes(charBuffer[index], false);
                      break;
                  }
                  if (!flag1)
                  {
                    if (this.linePosition + (offset1 - offset2) + num1 > 75)
                    {
                      flag1 = true;
                      break;
                    }
                    offset1 += num1;
                    if (flag2)
                      ++index;
                    if ((int) charBuffer[index++] == 13)
                    {
                      this.state = ContentLineWriter.FoldingTextWriter.States.CR;
                      break;
                    }
                  }
                  else
                    break;
                }
                else
                  break;
              }
              int count = offset1 - offset2;
              this.baseStream.Write(buffer, offset2, count);
              this.linePosition += count;
              byteCount -= count;
              continue;
            case ContentLineWriter.FoldingTextWriter.States.CR:
              this.baseStream.WriteByte(buffer[offset1]);
              if ((int) charBuffer[index++] == 10)
              {
                this.linePosition = 0;
                this.state = ContentLineWriter.FoldingTextWriter.States.Normal;
              }
              else
              {
                if (this.linePosition == 75 || flag1)
                {
                  if (flag1)
                    flag1 = false;
                  this.baseStream.Write(this.foldingBytes, 0, this.foldingBytes.Length);
                  this.linePosition = 1;
                }
                else
                  ++this.linePosition;
                if (index < charBuffer.Length && (int) charBuffer[index] != 13)
                  this.state = ContentLineWriter.FoldingTextWriter.States.Normal;
              }
              ++offset1;
              --byteCount;
              continue;
            default:
              continue;
          }
        }
      }

      public void WriteByte(byte byteToWrite)
      {
        this.baseStream.WriteByte(byteToWrite);
      }

      public override void Write(char charToWrite)
      {
        throw new NotSupportedException();
      }

      private static bool IsInvalidUTF8Byte(char inputChar, byte[] buffer, int offset)
      {
        return (int) inputChar == 65533 && (offset >= buffer.Length - 2 || (int) buffer[offset] != 239 || ((int) buffer[offset + 1] != 191 || (int) buffer[offset + 2] != 189));
      }

      private int WriteCharIntoBytes(char ch, bool flush)
      {
        this.charCheckerArray[0] = ch;
        return this.encoder.GetBytes(this.charCheckerArray, 0, 1, this.byteBuffer, 0, flush);
      }

      private enum States
      {
        Normal,
        CR,
      }
    }
  }
}
