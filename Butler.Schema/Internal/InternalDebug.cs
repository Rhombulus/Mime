// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Internal.InternalDebug
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Diagnostics;
using System.Linq;

namespace Butler.Schema.Data.Internal
{
  internal static class InternalDebug
  {
    private static bool useSystemDiagnostics;

    internal static bool UseSystemDiagnostics
    {
      get
      {
        return InternalDebug.useSystemDiagnostics;
      }
      set
      {
        InternalDebug.useSystemDiagnostics = value;
      }
    }

    [Conditional("DEBUG")]
    public static void Trace(long traceType, string format, params object[] traceObjects)
    {
    }

    [Conditional("DEBUG")]
    public static void Assert(bool condition, string formatString)
    {
    }

    [Conditional("DEBUG")]
    public static void Assert(bool condition)
    {
    }

    internal class DebugAssertionViolationException : Exception
    {
      public DebugAssertionViolationException()
      {
      }

      public DebugAssertionViolationException(string message)
        : base(message)
      {
      }
    }
  }
}
