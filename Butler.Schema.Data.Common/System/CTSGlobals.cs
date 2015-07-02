// Decompiled with JetBrains decompiler
// Type: System.CTSGlobals
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System.Text;

namespace System
{
  public static class CTSGlobals
  {
    public static Encoding AsciiEncoding = Encoding.GetEncoding("us-ascii");
    public static Encoding UnicodeEncoding = Encoding.GetEncoding("utf-16");
    public static Encoding Utf8Encoding = Encoding.GetEncoding("utf-8");
    public const int ReadBufferSize = 16384;
  }
}
