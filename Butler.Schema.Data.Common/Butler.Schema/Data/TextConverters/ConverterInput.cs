// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.ConverterInput
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  internal abstract class ConverterInput : IDisposable
  {
    protected bool endOfFile;
    protected int maxTokenSize;
    protected IProgressMonitor progressMonitor;

    public bool EndOfFile
    {
      get
      {
        return this.endOfFile;
      }
    }

    public int MaxTokenSize
    {
      get
      {
        return this.maxTokenSize;
      }
    }

    protected ConverterInput(IProgressMonitor progressMonitor)
    {
      this.progressMonitor = progressMonitor;
    }

    public virtual void SetRestartConsumer(IRestartable restartConsumer)
    {
    }

    public abstract bool ReadMore(ref char[] buffer, ref int start, ref int current, ref int end);

    public abstract void ReportProcessed(int processedSize);

    public abstract int RemoveGap(int gapBegin, int gapEnd);

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
