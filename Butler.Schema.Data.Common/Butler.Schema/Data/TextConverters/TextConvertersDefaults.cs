// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.TextConvertersDefaults
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  internal static class TextConvertersDefaults
  {
    private const int NormalMinDecodeBytes = 64;
    private const int NormalInitialTokenRuns = 64;
    private const int NormalMaxTokenRuns = 512;
    private const int NormalInitialTokenBufferSize = 1024;
    private const int NormalMaxTokenSize = 4096;
    private const int NormalInitialHtmlAttributes = 8;
    private const int NormalMaxHtmlAttributes = 128;
    private const int NormalMaxHtmlNormalizerNesting = 4096;
    private const int NormalMaxHtmlMetaRestartOffset = 4096;
    private const int BoundaryMinDecodeBytes = 1;
    private const int BoundaryInitialTokenRuns = 7;
    private const int BoundaryMaxTokenRuns = 16;
    private const int BoundaryInitialTokenBufferSize = 32;
    private const int BoundaryMaxTokenSize = 128;
    private const int BoundaryInitialHtmlAttributes = 1;
    private const int BoundaryMaxHtmlAttributes = 5;
    private const int BoundaryMaxHtmlNormalizerNesting = 10;
    private const int BoundaryMaxHtmlMetaRestartOffset = 4096;

    public static int MinDecodeBytes(bool boundaryTest)
    {
      return !boundaryTest ? 64 : 1;
    }

    public static int InitialTokenRuns(bool boundaryTest)
    {
      return !boundaryTest ? 64 : 7;
    }

    public static int MaxTokenRuns(bool boundaryTest)
    {
      return !boundaryTest ? 512 : 16;
    }

    public static int InitialTokenBufferSize(bool boundaryTest)
    {
      return !boundaryTest ? 1024 : 32;
    }

    public static int MaxTokenSize(bool boundaryTest)
    {
      return !boundaryTest ? 4096 : 128;
    }

    public static int InitialHtmlAttributes(bool boundaryTest)
    {
      return !boundaryTest ? 8 : 1;
    }

    public static int MaxHtmlAttributes(bool boundaryTest)
    {
      return !boundaryTest ? 128 : 5;
    }

    public static int MaxHtmlNormalizerNesting(bool boundaryTest)
    {
      return !boundaryTest ? 4096 : 10;
    }

    public static int MaxHtmlMetaRestartOffset(bool boundaryTest)
    {
      return !boundaryTest ? 4096 : 4096;
    }
  }
}
