using System;
using System.Collections.Generic;
using System.Linq;

namespace Butler.Schema.Data.Common {

    public sealed class ExchangeResourceManager : System.Resources.ResourceManager {

        private ExchangeResourceManager(string baseName, System.Reflection.Assembly assembly)
            : base(baseName, assembly) {
            resourceReleaseStopwatch.Start();
        }

        public override string BaseName => base.BaseName;

        public string AssemblyName => MainAssembly.GetName()
                                                  .FullName;

        public static ExchangeResourceManager GetResourceManager(string baseName, System.Reflection.Assembly assembly) {
            if (null == assembly)
                throw new ArgumentNullException(nameof(assembly));
            var key = baseName + assembly.GetName()
                                         .Name;
            lock (resourceManagers) {
                ExchangeResourceManager local_1 = null;
                if (!resourceManagers.TryGetValue(key, out local_1)) {
                    local_1 = new ExchangeResourceManager(baseName, assembly);
                    resourceManagers.Add(key, local_1);
                }
                return local_1;
            }
        }

        public override string GetString(string name) {
            return this.GetString(name, System.Globalization.CultureInfo.CurrentUICulture);
        }

        public override string GetString(string name, System.Globalization.CultureInfo culture) {
            var culture1 = culture ?? System.Globalization.CultureInfo.CurrentUICulture;
            var sipCultureInfoBase = culture1 as SipCultureInfoBase;
            string stringInternal;
            if (sipCultureInfoBase != null) {
                var useSipName = sipCultureInfoBase.UseSipName;
                try {
                    sipCultureInfoBase.UseSipName = true;
                    stringInternal = this.GetStringInternal(name, sipCultureInfoBase);
                } finally {
                    sipCultureInfoBase.UseSipName = useSipName;
                }
            } else
                stringInternal = this.GetStringInternal(name, culture1);
            return stringInternal;
        }

        private string GetStringInternal(string name, System.Globalization.CultureInfo culture) {
            string str = null;
            try {
                readerWriterLock.EnterReadLock();
                str = base.GetString(name, culture);
            } finally {
                readerWriterLock.ExitReadLock();
            }
            if (str == null) {
                try {
                    readerWriterLock.EnterWriteLock();
                    if (resourceReleaseStopwatch.Elapsed > resourceReleaseInterval) {
                        this.ReleaseAllResources();
                        resourceReleaseStopwatch.Restart();
                    }
                    str = base.GetString(name, culture);
                } finally {
                    readerWriterLock.ExitWriteLock();
                }
            }
            return str;
        }

        private static readonly Dictionary<string, ExchangeResourceManager> resourceManagers = new Dictionary<string, ExchangeResourceManager>();
        private readonly System.Threading.ReaderWriterLockSlim readerWriterLock = new System.Threading.ReaderWriterLockSlim();
        private readonly TimeSpan resourceReleaseInterval = TimeSpan.FromMinutes(1.0);
        private readonly System.Diagnostics.Stopwatch resourceReleaseStopwatch = new System.Diagnostics.Stopwatch();

    }

}