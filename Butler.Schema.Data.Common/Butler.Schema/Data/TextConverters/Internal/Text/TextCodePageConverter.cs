// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Text.TextCodePageConverter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters.Internal.Text
{
  internal class TextCodePageConverter : IProducerConsumer, IDisposable
  {
    protected ConverterInput input;
    protected bool endOfFile;
    protected bool gotAnyText;
    protected ConverterOutput output;

    public TextCodePageConverter(ConverterInput input, ConverterOutput output)
    {
      this.input = input;
      this.output = output;
    }

    public void Run()
    {
      if (this.endOfFile)
        return;
      char[] buffer = (char[]) null;
      int start = 0;
      int current = 0;
      int end = 0;
      if (!this.input.ReadMore(ref buffer, ref start, ref current, ref end))
        return;
      if (this.input.EndOfFile)
        this.endOfFile = true;
      if (end - start != 0)
      {
        if (!this.gotAnyText)
        {
          if (this.output is ConverterEncodingOutput)
          {
            ConverterEncodingOutput converterEncodingOutput = this.output as ConverterEncodingOutput;
            if (converterEncodingOutput.CodePageSameAsInput)
              converterEncodingOutput.Encoding = !(this.input is ConverterDecodingInput) ? Encoding.UTF8 : (this.input as ConverterDecodingInput).Encoding;
          }
          this.gotAnyText = true;
        }
        this.output.Write(buffer, start, end - start);
        this.input.ReportProcessed(end - start);
      }
      if (!this.endOfFile)
        return;
      this.output.Flush();
    }

    public bool Flush()
    {
      if (!this.endOfFile)
        this.Run();
      return this.endOfFile;
    }

    void IDisposable.Dispose()
    {
      if (this.input != null)
        this.input.Dispose();
      if (this.output != null)
        this.output.Dispose();
      this.input = (ConverterInput) null;
      this.output = (ConverterOutput) null;
      GC.SuppressFinalize((object) this);
    }
  }
}
