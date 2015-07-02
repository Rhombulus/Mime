// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.MimeLimits
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  public class MimeLimits
  {
    private static object configurationLockObject = new object();
    private static MimeLimits unlimitedLimits = new MimeLimits();
      private static volatile MimeLimits defaultLimits;

      public static MimeLimits Default => MimeLimits.GetDefaultMimeDocumentLimits();

      public static MimeLimits Unlimited => MimeLimits.unlimitedLimits;

      public int MaxPartDepth { get; } = int.MaxValue;

      public int MaxEmbeddedDepth { get; } = int.MaxValue;

      public int MaxSize { get; } = int.MaxValue;

      public int MaxHeaderBytes { get; } = int.MaxValue;

      public int MaxParts { get; } = int.MaxValue;

      public int MaxHeaders { get; } = int.MaxValue;

      public int MaxAddressItemsPerHeader { get; } = int.MaxValue;

      public int MaxTextValueBytesPerValue { get; } = int.MaxValue;

      public int MaxParametersPerHeader { get; } = int.MaxValue;

      internal int MaxEncodedWordLength { get; }

      internal MimeLimits(int partDepth, int embeddedDepth, int size, int headerBytes, int parts, int headers, int addressItemsPerHeader, int textValueBytesPerValue, int parametersPerHeader, int encodedWordLength)
    {
      this.MaxPartDepth = partDepth;
      this.MaxEmbeddedDepth = embeddedDepth;
      this.MaxSize = size;
      this.MaxHeaderBytes = headerBytes;
      this.MaxParts = parts;
      this.MaxHeaders = headers;
      this.MaxAddressItemsPerHeader = addressItemsPerHeader;
      this.MaxTextValueBytesPerValue = textValueBytesPerValue;
      this.MaxParametersPerHeader = parametersPerHeader;
      this.MaxEncodedWordLength = encodedWordLength;
    }

    private MimeLimits()
    {
    }

    internal static void RefreshConfiguration()
    {
      MimeLimits.defaultLimits = (MimeLimits) null;
    }

    private static MimeLimits GetDefaultMimeDocumentLimits()
    {
      if (MimeLimits.defaultLimits == null)
      {
        lock (MimeLimits.configurationLockObject)
        {
          if (MimeLimits.defaultLimits == null)
          {
            IList<Internal.CtsConfigurationSetting> local_0 = Internal.ApplicationServices.Provider.GetConfiguration("MimeLimits");
            int local_1 = 10;
            int local_2 = 100;
            int local_3 = int.MaxValue;
            int local_4 = int.MaxValue;
            int local_5 = 10000;
            int local_6 = 100000;
            int local_7 = int.MaxValue;
            int local_8 = 32768;
            int local_9 = int.MaxValue;
            int local_10 = 256;
            foreach (Internal.CtsConfigurationSetting item_0 in (IEnumerable<Internal.CtsConfigurationSetting>) local_0)
            {
              switch (item_0.Name.ToLower())
              {
                case "maximumpartdepth":
                  local_1 = Internal.ApplicationServices.ParseIntegerSetting(item_0, local_1, 5, false);
                  continue;
                case "maximumembeddeddepth":
                  local_2 = Internal.ApplicationServices.ParseIntegerSetting(item_0, local_2, 10, false);
                  continue;
                case "maximumsize":
                  local_3 = Internal.ApplicationServices.ParseIntegerSetting(item_0, local_3, 100, true);
                  continue;
                case "maximumtotalheaderssize":
                  local_4 = Internal.ApplicationServices.ParseIntegerSetting(item_0, local_4, 100, true);
                  continue;
                case "maximumparts":
                  local_5 = Internal.ApplicationServices.ParseIntegerSetting(item_0, local_5, 100, false);
                  continue;
                case "maximumheaders":
                  local_6 = Internal.ApplicationServices.ParseIntegerSetting(item_0, local_6, 100, false);
                  continue;
                case "maximumaddressitemsperheader":
                  local_7 = Internal.ApplicationServices.ParseIntegerSetting(item_0, local_7, 100, false);
                  continue;
                case "maximumparametersperheader":
                  local_9 = Internal.ApplicationServices.ParseIntegerSetting(item_0, local_9, 10, false);
                  continue;
                case "maximumtextvaluesize":
                  local_8 = Internal.ApplicationServices.ParseIntegerSetting(item_0, local_8, 10, true);
                  continue;
                default:
                  continue;
              }
            }
            MimeLimits.defaultLimits = new MimeLimits(local_1, local_2, local_3, local_4, local_5, local_6, local_7, local_8, local_9, local_10);
          }
        }
      }
      return MimeLimits.defaultLimits;
    }
  }
}
