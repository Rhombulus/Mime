// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Text.TextTokenBuilder
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters.Internal.Text
{
  internal class TextTokenBuilder : TokenBuilder
  {
    public TextToken Token
    {
      get
      {
        return (TextToken) base.Token;
      }
    }

    public TextTokenBuilder(char[] buffer, int maxRuns, bool testBoundaryConditions)
      : this(new TextToken(), buffer, maxRuns, testBoundaryConditions)
    {
    }

    public TextTokenBuilder(TextToken token, char[] buffer, int maxRuns, bool testBoundaryConditions)
      : base((Token) token, buffer, maxRuns, testBoundaryConditions)
    {
    }

    public TextTokenId MakeEmptyToken(TextTokenId tokenId)
    {
      return (TextTokenId) this.MakeEmptyToken((TokenId) tokenId);
    }

    public TextTokenId MakeEmptyToken(TextTokenId tokenId, int argument)
    {
      return (TextTokenId) this.MakeEmptyToken((TokenId) tokenId, argument);
    }

    public TextTokenId MakeEmptyToken(TextTokenId tokenId, Encoding encoding)
    {
      return (TextTokenId) this.MakeEmptyToken((TokenId) tokenId, encoding);
    }

    public void SkipRunIfNecessary(int start, RunKind skippedRunKind)
    {
      this.SkipRunIfNecessary(start, (uint) skippedRunKind);
    }

    public bool PrepareToAddMoreRuns(int numRuns, int start, RunKind skippedRunKind)
    {
      return this.PrepareToAddMoreRuns(numRuns, start, (uint) skippedRunKind);
    }

    public void AddSpecialRun(TextRunKind kind, int startEnd, int value)
    {
      this.AddRun(RunType.Special, RunTextType.Unknown, (uint) kind, this.tailOffset, startEnd, value);
    }
  }
}
