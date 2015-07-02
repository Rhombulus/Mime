// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.TokenBuilder
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Butler.Schema.Data.TextConverters.Internal
{
  internal abstract class TokenBuilder
  {
    protected const byte BuildStateInitialized = (byte) 0;
    protected const byte BuildStateEnded = (byte) 5;
    protected const byte FirstStarted = (byte) 10;
    protected const byte BuildStateText = (byte) 10;
    protected byte state;
    protected Token token;
    protected int maxRuns;
    protected int tailOffset;
    protected bool tokenValid;

    public Token Token
    {
      get
      {
        return this.token;
      }
    }

    public bool IsStarted
    {
      get
      {
        return (int) this.state != 0;
      }
    }

    public bool Valid
    {
      get
      {
        return this.tokenValid;
      }
    }

    public int TotalLength
    {
      get
      {
        return this.tailOffset - this.token.Whole.HeadOffset;
      }
    }

    public TokenBuilder(Token token, char[] buffer, int maxRuns, bool testBoundaryConditions)
    {
      int length = 64;
      if (!testBoundaryConditions)
      {
        this.maxRuns = maxRuns;
      }
      else
      {
        this.maxRuns = 55;
        length = 7;
      }
      this.token = token;
      this.token.Buffer = buffer;
      this.token.RunList = new Token.RunEntry[length];
    }

    public void BufferChanged(char[] newBuffer, int newBase)
    {
      if (newBuffer == this.token.Buffer && newBase == this.token.Whole.HeadOffset)
        return;
      this.token.Buffer = newBuffer;
      if (newBase == this.token.Whole.HeadOffset)
        return;
      this.Rebase(newBase - this.token.Whole.HeadOffset);
    }

    public virtual void Reset()
    {
      if ((int) this.state <= 0)
        return;
      this.token.Reset();
      this.tailOffset = 0;
      this.tokenValid = false;
      this.state = (byte) 0;
    }

    public TokenId MakeEmptyToken(TokenId tokenId)
    {
      this.token.TokenId = tokenId;
      this.state = (byte) 5;
      this.tokenValid = true;
      return tokenId;
    }

    public TokenId MakeEmptyToken(TokenId tokenId, int argument)
    {
      this.token.TokenId = tokenId;
      this.token.Argument = argument;
      this.state = (byte) 5;
      this.tokenValid = true;
      return tokenId;
    }

    public TokenId MakeEmptyToken(TokenId tokenId, Encoding tokenEncoding)
    {
      this.token.TokenId = tokenId;
      this.token.TokenEncoding = tokenEncoding;
      this.state = (byte) 5;
      this.tokenValid = true;
      return tokenId;
    }

    public void StartText(int baseOffset)
    {
      this.token.TokenId = TokenId.Text;
      this.state = (byte) 10;
      this.token.Whole.HeadOffset = baseOffset;
      this.tailOffset = baseOffset;
    }

    public void EndText()
    {
      this.state = (byte) 5;
      this.tokenValid = true;
      this.token.WholePosition.Rewind(this.token.Whole);
      this.AddSentinelRun();
    }

    public void SkipRunIfNecessary(int start, uint skippedRunKind)
    {
      if (start == this.tailOffset)
        return;
      this.AddInvalidRun(start, skippedRunKind);
    }

    public bool PrepareToAddMoreRuns(int numRuns, int start, uint skippedRunKind)
    {
      if (start != this.tailOffset || this.token.Whole.Tail + numRuns >= this.token.RunList.Length)
        return this.SlowPrepareToAddMoreRuns(numRuns, start, skippedRunKind);
      return true;
    }

    public bool SlowPrepareToAddMoreRuns(int numRuns, int start, uint skippedRunKind)
    {
      if (start != this.tailOffset)
        ++numRuns;
      if (this.token.Whole.Tail + numRuns >= this.token.RunList.Length && !this.ExpandRunsArray(numRuns))
        return false;
      if (start != this.tailOffset)
        this.AddInvalidRun(start, skippedRunKind);
      return true;
    }

    public bool PrepareToAddMoreRuns(int numRuns)
    {
      if (this.token.Whole.Tail + numRuns >= this.token.RunList.Length)
        return this.ExpandRunsArray(numRuns);
      return true;
    }

    [Conditional("DEBUG")]
    public void AssertPreparedToAddMoreRuns(int numRuns)
    {
    }

    [Conditional("DEBUG")]
    public void AssertCanAddMoreRuns(int numRuns)
    {
    }

    [Conditional("DEBUG")]
    public void AssertCurrentRunPosition(int position)
    {
    }

    [Conditional("DEBUG")]
    public void DebugPrepareToAddMoreRuns(int numRuns)
    {
    }

    public void AddTextRun(RunTextType textType, int start, int end)
    {
      this.AddRun(RunType.Normal, textType, 67108864U, start, end, 0);
    }

    public void AddLiteralTextRun(RunTextType textType, int start, int end, int literal)
    {
      this.AddRun(RunType.Literal, textType, 67108864U, start, end, literal);
    }

    public void AddSpecialRun(RunKind kind, int startEnd, int value)
    {
      this.AddRun(RunType.Special, RunTextType.Unknown, (uint) kind, this.tailOffset, startEnd, value);
    }

    internal void AddRun(RunType type, RunTextType textType, uint kind, int start, int end, int value)
    {
      this.token.RunList[this.token.Whole.Tail++].Initialize(type, textType, kind, end - start, value);
      this.tailOffset = end;
    }

    internal void AddInvalidRun(int offset, uint kind)
    {
      this.token.RunList[this.token.Whole.Tail++].Initialize(RunType.Invalid, RunTextType.Unknown, kind, offset - this.tailOffset, 0);
      this.tailOffset = offset;
    }

    internal void AddNullRun(uint kind)
    {
      this.token.RunList[this.token.Whole.Tail++].Initialize(RunType.Invalid, RunTextType.Unknown, kind, 0, 0);
    }

    internal void AddSentinelRun()
    {
      this.token.RunList[this.token.Whole.Tail].InitializeSentinel();
    }

    protected virtual void Rebase(int deltaOffset)
    {
      this.token.Whole.HeadOffset += deltaOffset;
      this.token.WholePosition.RunOffset += deltaOffset;
      this.tailOffset += deltaOffset;
    }

    private bool ExpandRunsArray(int numRuns)
    {
      int length = Math.Min(this.maxRuns, Math.Max(this.token.RunList.Length * 2, this.token.Whole.Tail + numRuns + 1));
      if (length - this.token.Whole.Tail < numRuns + 1)
        return false;
      Token.RunEntry[] runEntryArray = new Token.RunEntry[length];
      Array.Copy((Array) this.token.RunList, 0, (Array) runEntryArray, 0, this.token.Whole.Tail);
      this.token.RunList = runEntryArray;
      return true;
    }
  }
}
