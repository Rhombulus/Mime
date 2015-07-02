// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Common.ExchangeResourceManager
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace Butler.Schema.Data.Common
{
  public sealed class ExchangeResourceManager : ResourceManager
  {
    private static Dictionary<string, ExchangeResourceManager> resourceManagers = new Dictionary<string, ExchangeResourceManager>();
    private readonly TimeSpan resourceReleaseInterval = TimeSpan.FromMinutes(1.0);
    private Stopwatch resourceReleaseStopwatch = new Stopwatch();
    private ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();

    public override string BaseName
    {
      get
      {
        return base.BaseName;
      }
    }

    public string AssemblyName
    {
      get
      {
        return this.MainAssembly.GetName().FullName;
      }
    }

    private ExchangeResourceManager(string baseName, Assembly assembly)
      : base(baseName, assembly)
    {
      this.resourceReleaseStopwatch.Start();
    }

    public static ExchangeResourceManager GetResourceManager(string baseName, Assembly assembly)
    {
      if ((Assembly) null == assembly)
        throw new ArgumentNullException(nameof(assembly));
      string key = baseName + assembly.GetName().Name;
      lock (ExchangeResourceManager.resourceManagers)
      {
        ExchangeResourceManager local_1 = (ExchangeResourceManager) null;
        if (!ExchangeResourceManager.resourceManagers.TryGetValue(key, out local_1))
        {
          local_1 = new ExchangeResourceManager(baseName, assembly);
          ExchangeResourceManager.resourceManagers.Add(key, local_1);
        }
        return local_1;
      }
    }

    public override string GetString(string name)
    {
      return this.GetString(name, CultureInfo.CurrentUICulture);
    }

    public override string GetString(string name, CultureInfo culture)
    {
      CultureInfo culture1 = culture ?? CultureInfo.CurrentUICulture;
      SipCultureInfoBase sipCultureInfoBase = culture1 as SipCultureInfoBase;
      string stringInternal;
      if (sipCultureInfoBase != null)
      {
        bool useSipName = sipCultureInfoBase.UseSipName;
        try
        {
          sipCultureInfoBase.UseSipName = true;
          stringInternal = this.GetStringInternal(name, (CultureInfo) sipCultureInfoBase);
        }
        finally
        {
          sipCultureInfoBase.UseSipName = useSipName;
        }
      }
      else
        stringInternal = this.GetStringInternal(name, culture1);
      return stringInternal;
    }

    private string GetStringInternal(string name, CultureInfo culture)
    {
      string str = (string) null;
      try
      {
        this.readerWriterLock.EnterReadLock();
        str = base.GetString(name, culture);
      }
      finally
      {
        this.readerWriterLock.ExitReadLock();
      }
      if (str == null)
      {
        try
        {
          this.readerWriterLock.EnterWriteLock();
          if (this.resourceReleaseStopwatch.Elapsed > this.resourceReleaseInterval)
          {
            this.ReleaseAllResources();
            this.resourceReleaseStopwatch.Restart();
          }
          str = base.GetString(name, culture);
        }
        finally
        {
          this.readerWriterLock.ExitWriteLock();
        }
      }
      return str;
    }
  }
}
