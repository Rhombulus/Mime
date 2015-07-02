// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Internal.TempFileStream
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace Butler.Schema.Data.Internal
{
  internal class TempFileStream : FileStream
  {
    private static int nextId = Environment.TickCount ^ Process.GetCurrentProcess().Id;
    private static string tempPath;
    private string filePath;

    internal static string Path => TempFileStream.GetTempPath();

      public string FilePath => this.filePath;

      private TempFileStream(SafeFileHandle handle)
      : base(handle, FileAccess.ReadWrite)
    {
    }

    public static TempFileStream CreateInstance()
    {
      return TempFileStream.CreateInstance("Cts");
    }

    public static TempFileStream CreateInstance(string prefix)
    {
      return TempFileStream.CreateInstance(prefix, true);
    }

    public static TempFileStream CreateInstance(string prefix, bool deleteOnClose)
    {
      NativeMethods.SecurityAttributes securityAttributes = new NativeMethods.SecurityAttributes(false);
      string path = TempFileStream.Path;
      new FileIOPermission(FileIOPermissionAccess.Write, path).Demand();
      int error = 0;
      int num1 = 10;
      string str;
      SafeFileHandle file;
      do
      {
        uint num2 = (uint) Interlocked.Increment(ref TempFileStream.nextId);
        str = System.IO.Path.Combine(path, prefix + num2.ToString("X5") + ".tmp");
        uint num3 = deleteOnClose ? 67108864U : 0U;
        file = NativeMethods.CreateFile(str, 1180063U, 0U, ref securityAttributes, 1U, (uint) (256 | (int) num3 | 8192), IntPtr.Zero);
        --num1;
        if (file.IsInvalid)
        {
          error = Marshal.GetLastWin32Error();
          if (error == 80)
            ++num1;
          using (Process currentProcess = Process.GetCurrentProcess())
            Interlocked.Add(ref TempFileStream.nextId, currentProcess.Id);
        }
        else
          num1 = 0;
      }
      while (num1 > 0);
      if (file.IsInvalid)
      {
        string fileFailed = CtsResources.SharedStrings.CreateFileFailed(str);
        throw new IOException(fileFailed, (Exception) new Win32Exception(error, fileFailed));
      }
      return new TempFileStream(file)
      {
        filePath = str
      };
    }

    internal static void SetTemporaryPath(string path)
    {
      TempFileStream.tempPath = path;
    }

    private static string GetTempPath()
    {
      if (TempFileStream.tempPath == null)
        TempFileStream.tempPath = System.IO.Path.GetTempPath();
      return TempFileStream.tempPath;
    }

    protected override void Dispose(bool disposing)
    {
      try
      {
        base.Dispose(disposing);
      }
      catch (IOException ex)
      {
      }
    }
  }
}
