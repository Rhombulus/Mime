// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.RecognizeInterestingFontName
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters
{
  internal struct RecognizeInterestingFontName
  {
    private static byte[] charMapToClass = new byte[128]
    {
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 2,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 11,
      (byte) 0,
      (byte) 7,
      (byte) 0,
      (byte) 0,
      (byte) 6,
      (byte) 0,
      (byte) 4,
      (byte) 0,
      (byte) 0,
      (byte) 13,
      (byte) 10,
      (byte) 5,
      (byte) 12,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 8,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 3,
      (byte) 0,
      (byte) 9,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 11,
      (byte) 0,
      (byte) 7,
      (byte) 0,
      (byte) 0,
      (byte) 6,
      (byte) 0,
      (byte) 4,
      (byte) 0,
      (byte) 0,
      (byte) 13,
      (byte) 10,
      (byte) 5,
      (byte) 12,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 8,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 3,
      (byte) 0,
      (byte) 9,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0
    };
    private static sbyte[,] stateTransitionTable = new sbyte[16, 14]
    {
      {
        (sbyte) -1,
        (sbyte) 0,
        (sbyte) -1,
        (sbyte) 3,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) 11,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1
      },
      {
        (sbyte) -1,
        (sbyte) 1,
        (sbyte) 1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1
      },
      {
        (sbyte) -1,
        (sbyte) 2,
        (sbyte) 2,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1
      },
      {
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) 4,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1
      },
      {
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) 5,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1
      },
      {
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) 6,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1
      },
      {
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) 7,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1
      },
      {
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) 8,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1
      },
      {
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) 9,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1
      },
      {
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) 10,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1
      },
      {
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) 2,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1
      },
      {
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) 12,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1
      },
      {
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) 13,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1
      },
      {
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) 14,
        (sbyte) -1,
        (sbyte) -1
      },
      {
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) 15,
        (sbyte) -1
      },
      {
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) -1,
        (sbyte) 1
      }
    };
    private sbyte state;

    public TextMapping TextMapping
    {
      get
      {
        switch (this.state)
        {
          case (sbyte) 1:
            return TextMapping.Symbol;
          case (sbyte) 2:
            return TextMapping.Wingdings;
          default:
            return TextMapping.Unicode;
        }
      }
    }

    public bool IsRejected
    {
      get
      {
        return (int) this.state < 0;
      }
    }

    public void AddCharacter(byte ch)
    {
      if ((int) this.state < 0)
        return;
      this.state = RecognizeInterestingFontName.stateTransitionTable[(int) this.state, (int) ch > (int) sbyte.MaxValue ? 0 : (int) RecognizeInterestingFontName.charMapToClass[(int) ch]];
    }

    public void AddCharacter(char ch)
    {
      if ((int) this.state < 0)
        return;
      this.state = RecognizeInterestingFontName.stateTransitionTable[(int) this.state, (int) ch > (int) sbyte.MaxValue ? 0 : (int) RecognizeInterestingFontName.charMapToClass[(int) ch]];
    }
  }
}
