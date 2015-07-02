// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Internal.TemporaryDataStorage
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;

namespace Butler.Schema.Data.Internal
{
  internal class TemporaryDataStorage : ReadableWritableDataStorage
  {
    public static int DefaultBufferBlockSize = 8192;
    public static int DefaultBufferMaximumSize = TemporaryDataStorage.DefaultBufferBlockSize * 16;
    public static string DefaultPath = (string) null;
    public static Func<int, byte[]> DefaultAcquireBuffer = (Func<int, byte[]>) null;
    public static Action<byte[]> DefaultReleaseBuffer = (Action<byte[]>) null;
    internal static volatile bool Configured = false;
    private static object configurationLockObject = new object();
    private static readonly FileSystemAccessRule[] DirectoryAccessRules = new FileSystemAccessRule[4]
    {
      new FileSystemAccessRule((IdentityReference) new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, (SecurityIdentifier) null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow),
      new FileSystemAccessRule((IdentityReference) new SecurityIdentifier(WellKnownSidType.LocalSystemSid, (SecurityIdentifier) null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow),
      new FileSystemAccessRule((IdentityReference) new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, (SecurityIdentifier) null), FileSystemRights.Modify | FileSystemRights.DeleteSubdirectoriesAndFiles | FileSystemRights.Synchronize, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow),
      new FileSystemAccessRule((IdentityReference) new SecurityIdentifier(WellKnownSidType.LocalServiceSid, (SecurityIdentifier) null), FileSystemRights.Modify | FileSystemRights.DeleteSubdirectoriesAndFiles | FileSystemRights.Synchronize, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow)
    };
    private long totalLength;
    private long filePosition;
    private Stream fileStream;
    private TemporaryDataStorage.VirtualBuffer buffer;

    public override long Length
    {
      get
      {
        this.ThrowIfDisposed();
        return this.totalLength;
      }
    }

    public TemporaryDataStorage()
      : this(TemporaryDataStorage.DefaultAcquireBuffer, TemporaryDataStorage.DefaultReleaseBuffer)
    {
    }

    public TemporaryDataStorage(Func<int, byte[]> acquireBuffer, Action<byte[]> releaseBuffer)
    {
      TemporaryDataStorage.Configure();
      this.buffer = new TemporaryDataStorage.VirtualBuffer(TemporaryDataStorage.DefaultBufferBlockSize, TemporaryDataStorage.DefaultBufferMaximumSize, acquireBuffer != null ? acquireBuffer : TemporaryDataStorage.DefaultAcquireBuffer, releaseBuffer != null ? releaseBuffer : TemporaryDataStorage.DefaultReleaseBuffer);
    }

    public static void Configure(int defaultMaximumSize, int defaultBlockSize, string defaultPath, Func<int, byte[]> defaultAcquireBuffer, Action<byte[]> defaultReleaseBuffer)
    {
      TemporaryDataStorage.DefaultBufferMaximumSize = defaultMaximumSize;
      TemporaryDataStorage.DefaultBufferBlockSize = defaultBlockSize;
      TemporaryDataStorage.DefaultPath = defaultPath;
      TemporaryDataStorage.DefaultAcquireBuffer = defaultAcquireBuffer;
      TemporaryDataStorage.DefaultReleaseBuffer = defaultReleaseBuffer;
      TemporaryDataStorage.Configured = false;
      TemporaryDataStorage.Configure();
    }

    public override int Read(long position, byte[] buffer, int offset, int count)
    {
      this.ThrowIfDisposed();
      int num = 0;
      if (this.isReadOnly)
      {
        this.readOnlySemaphore.Wait();
        try
        {
          num = this.InternalRead(position, buffer, offset, count);
        }
        finally
        {
          this.readOnlySemaphore.Release();
        }
      }
      else
        num = this.InternalRead(position, buffer, offset, count);
      return num;
    }

    public override void Write(long position, byte[] buffer, int offset, int count)
    {
      this.ThrowIfDisposed();
      if (this.isReadOnly)
        throw new InvalidOperationException("Write to read-only DataStorage");
      if (count == 0)
        return;
      if (position < (long) this.buffer.MaxBytes)
      {
        int num = this.buffer.Write(position, buffer, offset, count);
        offset += num;
        count -= num;
        position += (long) num;
        if (position > this.totalLength)
          this.totalLength = position;
      }
      else if (this.buffer.Length < this.buffer.MaxBytes)
      {
        this.buffer.SetLength((long) this.buffer.MaxBytes);
        this.totalLength = (long) this.buffer.MaxBytes;
      }
      if (count == 0)
        return;
      if (this.fileStream == null)
      {
        this.fileStream = (Stream) TempFileStream.CreateInstance();
        this.filePosition = 0L;
      }
      if (this.filePosition != position - (long) this.buffer.MaxBytes)
        this.fileStream.Position = position - (long) this.buffer.MaxBytes;
      this.fileStream.Write(buffer, offset, count);
      position += (long) count;
      this.filePosition = position - (long) this.buffer.MaxBytes;
      if (position <= this.totalLength)
        return;
      this.totalLength = position;
    }

    public override void SetLength(long length)
    {
      this.ThrowIfDisposed();
      if (this.isReadOnly)
        throw new InvalidOperationException("Write to read-only DataStorage");
      this.totalLength = length;
      if (length <= (long) this.buffer.MaxBytes)
      {
        this.buffer.SetLength(length);
        if (this.fileStream == null)
          return;
        this.fileStream.SetLength(0L);
      }
      else
      {
        this.buffer.SetLength((long) this.buffer.MaxBytes);
        if (this.fileStream == null)
        {
          this.fileStream = (Stream) TempFileStream.CreateInstance();
          this.filePosition = 0L;
        }
        this.fileStream.SetLength(length - (long) this.buffer.MaxBytes);
      }
    }

    internal static void RefreshConfiguration()
    {
      TemporaryDataStorage.Configured = false;
    }

    internal static string GetTempPath()
    {
      TemporaryDataStorage.Configure();
      return TempFileStream.Path;
    }

    protected override void Dispose(bool disposing)
    {
      if (!this.IsDisposed)
      {
        if (disposing)
        {
          if (this.fileStream != null)
            this.fileStream.Dispose();
          this.buffer.Dispose();
        }
        this.fileStream = (Stream) null;
      }
      base.Dispose(disposing);
    }

    private static void Configure()
    {
      if (TemporaryDataStorage.Configured)
        return;
      lock (TemporaryDataStorage.configurationLockObject)
      {
        if (TemporaryDataStorage.Configured)
          return;
        int local_0 = TemporaryDataStorage.DefaultBufferMaximumSize;
        int local_1 = TemporaryDataStorage.DefaultBufferBlockSize;
        string local_2 = TemporaryDataStorage.DefaultPath;
        foreach (CtsConfigurationSetting item_1 in (IEnumerable<CtsConfigurationSetting>) ApplicationServices.Provider.GetConfiguration((string) null))
        {
          if (item_1.Name.Equals("TemporaryStorage", StringComparison.OrdinalIgnoreCase))
          {
            foreach (CtsConfigurationArgument item_0 in (IEnumerable<CtsConfigurationArgument>) item_1.Arguments)
            {
              if (item_0.Name.Equals("Path", StringComparison.OrdinalIgnoreCase))
                local_2 = item_0.Value.Trim();
              else if (item_0.Name.Equals("MaximumBufferSize", StringComparison.OrdinalIgnoreCase))
              {
                if (!int.TryParse(item_0.Value.Trim(), out local_0))
                {
                  ApplicationServices.Provider.LogConfigurationErrorEvent();
                  local_0 = TemporaryDataStorage.DefaultBufferMaximumSize;
                }
                else if (local_0 < 16 || local_0 > 10240)
                {
                  ApplicationServices.Provider.LogConfigurationErrorEvent();
                  local_0 = TemporaryDataStorage.DefaultBufferMaximumSize;
                }
                else
                  local_0 *= 1024;
              }
              else if (item_0.Name.Equals("BufferIncrement", StringComparison.OrdinalIgnoreCase))
              {
                if (!int.TryParse(item_0.Value.Trim(), out local_1))
                {
                  ApplicationServices.Provider.LogConfigurationErrorEvent();
                  local_1 = TemporaryDataStorage.DefaultBufferBlockSize;
                }
                else if (local_1 < 4 || local_1 > 64)
                {
                  ApplicationServices.Provider.LogConfigurationErrorEvent();
                  local_1 = TemporaryDataStorage.DefaultBufferBlockSize;
                }
                else
                  local_1 *= 1024;
              }
              else
                ApplicationServices.Provider.LogConfigurationErrorEvent();
            }
          }
        }
        if (local_0 < local_1 || local_0 % local_1 != 0)
        {
          ApplicationServices.Provider.LogConfigurationErrorEvent();
          local_0 = TemporaryDataStorage.DefaultBufferMaximumSize;
          local_1 = TemporaryDataStorage.DefaultBufferBlockSize;
        }
        TemporaryDataStorage.DefaultBufferMaximumSize = local_0;
        TemporaryDataStorage.DefaultBufferBlockSize = local_1;
        string local_6 = TemporaryDataStorage.GetSystemTempPath();
        if (local_2 != null)
          local_2 = TemporaryDataStorage.ValidatePath(local_2);
        if (local_2 == null)
          local_2 = local_6;
        TempFileStream.SetTemporaryPath(local_2);
        TemporaryDataStorage.Configured = true;
      }
    }

    private int InternalRead(long position, byte[] buffer, int offset, int count)
    {
      int num1 = 0;
      if (position < (long) this.buffer.MaxBytes)
      {
        num1 = this.buffer.Read(position, buffer, offset, count);
        offset += num1;
        count -= num1;
        position += (long) num1;
      }
      if (count != 0 && position >= (long) this.buffer.MaxBytes && this.fileStream != null)
      {
        if (this.filePosition != position - (long) this.buffer.MaxBytes)
          this.fileStream.Position = position - (long) this.buffer.MaxBytes;
        int num2 = this.fileStream.Read(buffer, offset, count);
        this.filePosition = position - (long) this.buffer.MaxBytes + (long) num2;
        num1 += num2;
      }
      return num1;
    }

    private static DirectorySecurity GetDirectorySecurity()
    {
      DirectorySecurity directorySecurity = new DirectorySecurity();
      directorySecurity.SetAccessRuleProtection(true, false);
      using (WindowsIdentity current = WindowsIdentity.GetCurrent())
      {
        directorySecurity.SetOwner((IdentityReference) current.User);
        for (int index = 0; index < TemporaryDataStorage.DirectoryAccessRules.Length; ++index)
          directorySecurity.AddAccessRule(TemporaryDataStorage.DirectoryAccessRules[index]);
        if (!current.User.IsWellKnown(WellKnownSidType.LocalSystemSid) && !current.User.IsWellKnown(WellKnownSidType.NetworkServiceSid) && !current.User.IsWellKnown(WellKnownSidType.LocalServiceSid))
          directorySecurity.AddAccessRule(new FileSystemAccessRule((IdentityReference) current.User, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
        return directorySecurity;
      }
    }

    private static string ValidatePath(string path)
    {
      try
      {
        if (Path.IsPathRooted(path))
        {
          if (!Directory.Exists(path))
            Directory.CreateDirectory(path, TemporaryDataStorage.GetDirectorySecurity());
          new FileIOPermission(FileIOPermissionAccess.Write, path).Demand();
        }
        else
          path = (string) null;
      }
      catch (PathTooLongException ex)
      {
        path = (string) null;
      }
      catch (DirectoryNotFoundException ex)
      {
        path = (string) null;
      }
      catch (IOException ex)
      {
        path = (string) null;
      }
      catch (UnauthorizedAccessException ex)
      {
        path = (string) null;
      }
      catch (ArgumentException ex)
      {
        path = (string) null;
      }
      catch (NotSupportedException ex)
      {
        path = (string) null;
      }
      return path;
    }

    private static string GetSystemTempPath()
    {
      return Path.GetTempPath();
    }

    private struct VirtualBuffer : IDisposable
    {
      private int maximumSize;
      private int blockSize;
      private long length;
      private byte[] firstBlock;
      private byte[][] followingBlocks;
      private Func<int, byte[]> acquireBuffer;
      private Action<byte[]> releaseBuffer;

      public int MaxBytes => this.maximumSize;

        public int BlockSize => this.blockSize;

        public int Length => (int) this.length;

        public VirtualBuffer(int blockSize, int maximumSize)
      {
        this = new TemporaryDataStorage.VirtualBuffer(blockSize, maximumSize, (Func<int, byte[]>) null, (Action<byte[]>) null);
      }

      public VirtualBuffer(int blockSize, int maximumSize, Func<int, byte[]> acquireBuffer, Action<byte[]> releaseBuffer)
      {
        if (acquireBuffer != null && releaseBuffer == null || acquireBuffer == null && releaseBuffer != null)
          throw new ArgumentException("acquireBuffer and releaseBuffer should be both null or non-null");
        this.blockSize = blockSize;
        this.maximumSize = maximumSize;
        this.length = 0L;
        this.firstBlock = (byte[]) null;
        this.followingBlocks = (byte[][]) null;
        this.acquireBuffer = acquireBuffer;
        this.releaseBuffer = releaseBuffer;
      }

      public int Read(long position, byte[] buffer, int offset, int count)
      {
        if (position >= this.length)
          return 0;
        int num = 0;
        if (position < (long) this.BlockSize)
        {
          int count1 = (int) Math.Min((long) this.BlockSize - position, this.length - position);
          if (count1 > count)
            count1 = count;
          Buffer.BlockCopy((Array) this.firstBlock, (int) position, (Array) buffer, offset, count1);
          offset += count1;
          count -= count1;
          position += (long) count1;
          num += count1;
        }
        while (count != 0 && position < this.length)
        {
          int index = (int) ((position - (long) this.BlockSize) / (long) this.BlockSize);
          int srcOffset = (int) ((position - (long) this.BlockSize) % (long) this.BlockSize);
          int count1 = (int) Math.Min((long) (this.BlockSize - srcOffset), this.length - position);
          if (count1 > count)
            count1 = count;
          Buffer.BlockCopy((Array) this.followingBlocks[index], srcOffset, (Array) buffer, offset, count1);
          offset += count1;
          count -= count1;
          position += (long) count1;
          num += count1;
        }
        return num;
      }

      public int Write(long position, byte[] buffer, int offset, int count)
      {
        if (position > this.length)
          this.SetLength(position);
        if (position >= (long) this.MaxBytes)
          return 0;
        int num = 0;
        if (position < (long) this.BlockSize)
        {
          if (this.firstBlock == null)
            this.firstBlock = this.GetBlock();
          int count1 = (int) Math.Min((long) this.BlockSize - position, (long) count);
          Buffer.BlockCopy((Array) buffer, offset, (Array) this.firstBlock, (int) position, count1);
          offset += count1;
          count -= count1;
          position += (long) count1;
          num += count1;
        }
        while (count != 0 && position < (long) this.MaxBytes)
        {
          if (this.followingBlocks == null)
            this.followingBlocks = new byte[(this.MaxBytes - this.BlockSize) / this.BlockSize][];
          int index = (int) ((position - (long) this.BlockSize) / (long) this.BlockSize);
          int dstOffset = (int) ((position - (long) this.BlockSize) % (long) this.BlockSize);
          if (this.followingBlocks[index] == null)
            this.followingBlocks[index] = this.GetBlock();
          int count1 = Math.Min(this.BlockSize - dstOffset, count);
          Buffer.BlockCopy((Array) buffer, offset, (Array) this.followingBlocks[index], dstOffset, count1);
          offset += count1;
          count -= count1;
          position += (long) count1;
          num += count1;
        }
        if (position > this.length)
          this.length = position;
        return num;
      }

      public void SetLength(long length)
      {
        if (this.length < length)
        {
          if (this.length < (long) this.BlockSize)
          {
            int length1 = (int) Math.Min((long) this.BlockSize - this.length, length - this.length);
            if (this.firstBlock == null)
              this.firstBlock = this.GetBlock();
            else
              Array.Clear((Array) this.firstBlock, (int) this.length, length1);
            this.length += (long) length1;
          }
          while (this.length < length && this.length < (long) this.MaxBytes)
          {
            if (this.followingBlocks == null)
              this.followingBlocks = new byte[(this.MaxBytes - this.BlockSize) / this.BlockSize][];
            int index1 = (int) ((this.length - (long) this.BlockSize) / (long) this.BlockSize);
            int index2 = (int) ((this.length - (long) this.BlockSize) % (long) this.BlockSize);
            int length1 = (int) Math.Min((long) (this.BlockSize - index2), length - this.length);
            if (this.followingBlocks[index1] == null)
              this.followingBlocks[index1] = this.GetBlock();
            else
              Array.Clear((Array) this.followingBlocks[index1], index2, length1);
            this.length += (long) length1;
          }
        }
        else
          this.length = length;
      }

      public void Dispose()
      {
        if (this.releaseBuffer != null)
        {
          if (this.firstBlock != null)
          {
            this.releaseBuffer(this.firstBlock);
            this.firstBlock = (byte[]) null;
          }
          if (this.followingBlocks != null)
          {
            foreach (byte[] numArray in this.followingBlocks)
            {
              if (numArray != null)
                this.releaseBuffer(numArray);
            }
            this.followingBlocks = (byte[][]) null;
          }
          this.releaseBuffer = (Action<byte[]>) null;
        }
        GC.SuppressFinalize((object) this);
      }

      private byte[] GetBlock()
      {
        if (this.acquireBuffer == null)
          return new byte[this.BlockSize];
        return this.acquireBuffer(this.BlockSize);
      }
    }
  }
}
