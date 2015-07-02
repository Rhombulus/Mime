// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.ConverterOutput
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  internal abstract class ConverterOutput : ITextSink, IDisposable
  {
    private const int StringBufferMax = 128;
    protected char[] stringBuffer;
    protected IReportBytes reportBytes;
    private IFallback fallback;

    public abstract bool CanAcceptMore { get; }

    bool ITextSink.IsEnough
    {
      get
      {
        return false;
      }
    }

    internal IReportBytes ReportBytes
    {
      get
      {
        return this.reportBytes;
      }
      set
      {
        this.reportBytes = value;
      }
    }

    public ConverterOutput()
    {
      this.stringBuffer = new char[128];
    }

    public abstract void Write(char[] buffer, int offset, int count, IFallback fallback);

    public abstract void Flush();

    public void Write(char[] buffer, int offset, int count)
    {
      this.Write(buffer, offset, count, (IFallback) null);
    }

    public virtual void Write(string text)
    {
      this.Write(text, 0, text.Length, (IFallback) null);
    }

    public void Write(string text, IFallback fallback)
    {
      this.Write(text, 0, text.Length, fallback);
    }

    public void Write(string text, int offset, int count)
    {
      this.Write(text, offset, count, (IFallback) null);
    }

    public void Write(string text, int offset, int count, IFallback fallback)
    {
      if (this.stringBuffer.Length < count)
        this.stringBuffer = new char[count * 2];
      text.CopyTo(offset, this.stringBuffer, 0, count);
      this.Write(this.stringBuffer, 0, count, fallback);
    }

    public void Write(char ch)
    {
      this.Write(ch, (IFallback) null);
    }

    public void Write(char ch, IFallback fallback)
    {
      this.stringBuffer[0] = ch;
      this.Write(this.stringBuffer, 0, 1, fallback);
    }

    public void Write(int ucs32Literal)
    {
      this.Write(ucs32Literal, (IFallback) null);
    }

    public void Write(int ucs32Literal, IFallback fallback)
    {
      int count = 1;
      if (ucs32Literal > (int) ushort.MaxValue)
      {
        if (fallback != null && fallback is HtmlWriter)
        {
          uint num1 = (uint) ucs32Literal;
          int num2 = num1 < 10U ? 1 : (num1 < 100U ? 2 : (num1 < 1000U ? 3 : (num1 < 10000U ? 4 : (num1 < 100000U ? 5 : (num1 < 1000000U ? 6 : 7)))));
          int index = 2 + num2;
          this.stringBuffer[0] = '&';
          this.stringBuffer[1] = '#';
          this.stringBuffer[index] = ';';
          while ((int) num1 != 0)
          {
            uint num3 = num1 % 10U;
            this.stringBuffer[--index] = (char) (num3 + 48U);
            num1 /= 10U;
          }
          this.Write(this.stringBuffer, 0, 3 + num2, (IFallback) null);
          return;
        }
        this.stringBuffer[0] = ParseSupport.HighSurrogateCharFromUcs4(ucs32Literal);
        this.stringBuffer[1] = ParseSupport.LowSurrogateCharFromUcs4(ucs32Literal);
        count = 2;
      }
      else
        this.stringBuffer[0] = (char) ucs32Literal;
      this.Write(this.stringBuffer, 0, count, fallback);
    }

    public ITextSink PrepareSink(IFallback fallback)
    {
      this.fallback = fallback;
      return (ITextSink) this;
    }

    void ITextSink.Write(char[] buffer, int offset, int count)
    {
      this.Write(buffer, offset, count, this.fallback);
    }

    void ITextSink.Write(int ucs32Literal)
    {
      this.Write(ucs32Literal, this.fallback);
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }
  }
}
