// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.TextConverters.Internal.Rtf.RtfToTextConverter
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.TextConverters.Internal.Rtf
{
  internal class RtfToTextConverter : IProducerConsumer, IDisposable
  {
    private bool treatNbspAsBreakable;
    private RtfParser parser;
    private bool endOfFile;
    private Text.TextOutput output;
    private bool firstKeyword;
    private bool ignorableDestination;
    private RtfToTextConverter.RtfState state;
    private bool hyperLinkActive;
    private bool skipFieldResult;
    private bool includePictureField;
    private bool descriptionProperty;
    private string imageUrl;
    private string imageAltText;
    private int pictureWidth;
    private int pictureHeight;
    private int lineIndent;
    private Injection injection;
    private ScratchBuffer scratch;
    private UrlCompareSink urlCompareSink;
    private bool started;

    public RtfToTextConverter(RtfParser parser, Text.TextOutput output, Injection injection, bool treatNbspAsBreakable, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum)
    {
      this.treatNbspAsBreakable = treatNbspAsBreakable;
      this.output = output;
      this.parser = parser;
      this.injection = injection;
      this.state = new RtfToTextConverter.RtfState(128);
    }

    public void Run()
    {
      if (this.endOfFile)
        return;
      this.Process(this.parser.Parse());
    }

    public bool Flush()
    {
      if (!this.endOfFile)
        this.Run();
      return this.endOfFile;
    }

    void IDisposable.Dispose()
    {
      if (this.parser != null && this.parser is IDisposable)
        ((IDisposable) this.parser).Dispose();
      if (this.output != null && this.output != null)
        ((IDisposable) this.output).Dispose();
      this.parser = (RtfParser) null;
      this.output = (Text.TextOutput) null;
      GC.SuppressFinalize((object) this);
    }

    private void Process(RtfTokenId tokenId)
    {
      if (!this.started)
      {
        this.output.OpenDocument();
        if (this.injection != null && this.injection.HaveHead)
          this.injection.Inject(true, this.output);
        this.started = true;
      }
      switch (tokenId)
      {
        case RtfTokenId.EndOfFile:
          this.ProcessEOF();
          break;
        case RtfTokenId.Begin:
          this.ProcessBeginGroup();
          break;
        case RtfTokenId.End:
          this.ProcessEndGroup();
          break;
        case RtfTokenId.Binary:
          this.ProcessBinary(this.parser.Token);
          break;
        case RtfTokenId.Keywords:
          this.ProcessKeywords(this.parser.Token);
          break;
        case RtfTokenId.Text:
          this.ProcessText(this.parser.Token);
          break;
      }
    }

    private void ProcessBeginGroup()
    {
      if (this.state.SkipLevel != 0)
      {
        ++this.state.Level;
      }
      else
      {
        this.state.Push();
        this.firstKeyword = true;
        this.ignorableDestination = false;
      }
    }

    private void ProcessEndGroup()
    {
      if (this.state.SkipLevel != 0)
      {
        if (this.state.Level != this.state.SkipLevel)
        {
          --this.state.Level;
          return;
        }
        this.state.SkipLevel = 0;
      }
      this.firstKeyword = false;
      if (this.state.Level <= 0)
        return;
      this.EndGroup();
      this.state.Pop();
    }

    private void ProcessKeywords(RtfToken token)
    {
      if (this.state.SkipLevel != 0 && this.state.Level >= this.state.SkipLevel)
        return;
      foreach (RtfKeyword rtfKeyword in token.Keywords)
      {
        if (this.firstKeyword)
        {
          if ((int) rtfKeyword.Id == 1)
          {
            this.ignorableDestination = true;
            continue;
          }
          this.firstKeyword = false;
          switch (rtfKeyword.Id)
          {
            case (short) 306:
              if (this.state.Destination == RtfDestination.Field)
              {
                this.state.Destination = RtfDestination.FieldInstruction;
                continue;
              }
              this.state.SkipGroup();
              return;
            case (short) 315:
            case (short) 201:
            case (short) 246:
            case (short) 15:
            case (short) 24:
              this.state.SkipGroup();
              return;
            case (short) 252:
              if (this.state.Destination == RtfDestination.RTF)
              {
                this.state.SkipGroup();
                return;
              }
              continue;
            case (short) 269:
              if (this.state.Destination == RtfDestination.Field && !this.skipFieldResult)
              {
                this.state.Destination = RtfDestination.FieldResult;
                continue;
              }
              this.skipFieldResult = false;
              this.state.SkipGroup();
              return;
            case (short) 175:
              if (this.state.Destination == RtfDestination.RTF)
              {
                this.state.SkipGroup();
                return;
              }
              continue;
            case (short) 124:
              if (this.state.Destination == RtfDestination.PictureProperties)
              {
                this.state.Destination = RtfDestination.ShapeValue;
                continue;
              }
              continue;
            case (short) 135:
              if (this.state.Destination == RtfDestination.PictureProperties)
              {
                this.state.Destination = RtfDestination.ShapeName;
                continue;
              }
              continue;
            case (short) 43:
              if (this.state.Destination == RtfDestination.Picture && this.includePictureField)
              {
                this.state.Destination = RtfDestination.PictureProperties;
                continue;
              }
              this.state.SkipGroup();
              return;
            case (short) 50:
              if (this.state.Destination == RtfDestination.RTF || this.state.Destination == RtfDestination.FieldResult)
              {
                this.state.Destination = RtfDestination.Picture;
                this.pictureWidth = 0;
                this.pictureHeight = 0;
                break;
              }
              this.state.SkipGroup();
              return;
            case (short) 29:
              if (this.state.Destination != RtfDestination.RTF && this.state.Destination != RtfDestination.FieldResult)
                return;
              this.state.Destination = RtfDestination.Field;
              return;
            default:
              if (this.ignorableDestination)
              {
                this.state.SkipGroup();
                return;
              }
              break;
          }
        }
        switch (rtfKeyword.Id)
        {
          case (short) 284:
            this.lineIndent += rtfKeyword.Value;
            continue;
          case (short) 184:
            this.lineIndent = 0;
            continue;
          case (short) 206:
            this.lineIndent = rtfKeyword.Value;
            continue;
          case (short) 154:
            if (this.state.Destination == RtfDestination.Picture)
            {
              this.pictureWidth = rtfKeyword.Value;
              continue;
            }
            continue;
          case (short) 136:
            this.state.IsInvisible = rtfKeyword.Value != 0;
            continue;
          case (short) 142:
            if (rtfKeyword.Value >= 75 && this.output.LineEmpty && this.output.RenderingPosition() != 0)
            {
              this.output.CloseParagraph();
              continue;
            }
            continue;
          case (short) 119:
          case (short) 126:
            this.output.OutputTabulation(1);
            continue;
          case (short) 71:
            if (rtfKeyword.Value <= 0 || rtfKeyword.Value > (int) short.MaxValue)
              continue;
            continue;
          case (short) 48:
            this.output.OutputNewLine();
            continue;
          case (short) 68:
          case (short) 40:
            if (this.hyperLinkActive)
            {
              if (!this.urlCompareSink.IsMatch)
                this.output.CloseAnchor();
              else
                this.output.CancelAnchor();
              this.hyperLinkActive = false;
              this.urlCompareSink.Reset();
              goto case (short) 48;
            }
            else
              goto case (short) 48;
          case (short) 6:
            if (this.state.Destination == RtfDestination.Picture)
            {
              this.pictureHeight = rtfKeyword.Value;
              continue;
            }
            continue;
          default:
            continue;
        }
      }
    }

    private void ProcessText(RtfToken token)
    {
      if (this.state.SkipLevel != 0 && this.state.Level >= this.state.SkipLevel)
        return;
      switch (this.state.Destination)
      {
        case RtfDestination.RTF:
          if (this.state.IsInvisible)
            break;
          if (this.output.LineEmpty && this.lineIndent >= 120 && this.lineIndent < 7200)
            this.output.OutputSpace(this.lineIndent / 120);
          token.TextElements.OutputTextElements(this.output, this.treatNbspAsBreakable);
          break;
        case RtfDestination.Field:
        case RtfDestination.Picture:
        case RtfDestination.PictureProperties:
          this.firstKeyword = false;
          break;
        case RtfDestination.FieldResult:
          if (this.hyperLinkActive && this.urlCompareSink.IsActive)
          {
            token.Text.WriteTo((ITextSink) this.urlCompareSink);
            token.Text.Rewind();
            goto case RtfDestination.RTF;
          }
          else
            goto case RtfDestination.RTF;
        case RtfDestination.FieldInstruction:
          this.firstKeyword = false;
          this.scratch.AppendRtfTokenText(token, 4096);
          break;
        case RtfDestination.ShapeName:
          this.firstKeyword = false;
          this.scratch.AppendRtfTokenText(token, 128);
          break;
        case RtfDestination.ShapeValue:
          this.firstKeyword = false;
          if (!this.descriptionProperty)
            break;
          this.scratch.AppendRtfTokenText(token, 4096);
          break;
        default:
          this.firstKeyword = false;
          break;
      }
    }

    private void ProcessBinary(RtfToken token)
    {
    }

    private void ProcessEOF()
    {
      this.output.CloseParagraph();
      if (this.injection != null && this.injection.HaveTail)
        this.injection.Inject(false, this.output);
      this.output.Flush();
      this.endOfFile = true;
    }

    private void EndGroup()
    {
      switch (this.state.Destination)
      {
        case RtfDestination.Field:
          if (this.state.ParentDestination == RtfDestination.Field)
            break;
          this.skipFieldResult = false;
          if (this.includePictureField)
          {
            this.output.OutputImage(this.imageUrl, this.imageAltText, new Format.PropertyValue(Format.LengthUnits.Twips, this.pictureWidth).PixelsInteger, new Format.PropertyValue(Format.LengthUnits.Twips, this.pictureHeight).PixelsInteger);
            this.includePictureField = false;
            this.imageUrl = (string) null;
            this.imageAltText = (string) null;
            break;
          }
          if (!this.hyperLinkActive)
            break;
          if (!this.urlCompareSink.IsMatch)
            this.output.CloseAnchor();
          else
            this.output.CancelAnchor();
          this.hyperLinkActive = false;
          break;
        case RtfDestination.FieldInstruction:
          if (this.state.ParentDestination != RtfDestination.Field)
            break;
          bool local;
          BufferString linkUrl;
          if (RtfSupport.IsHyperlinkField(ref this.scratch, out local, out linkUrl))
          {
            if (!local)
            {
              if (this.hyperLinkActive)
              {
                if (!this.urlCompareSink.IsMatch)
                  this.output.CloseAnchor();
                else
                  this.output.CancelAnchor();
                this.hyperLinkActive = false;
                this.urlCompareSink.Reset();
              }
              linkUrl.TrimWhitespace();
              if (linkUrl.Length != 0)
              {
                string str = linkUrl.ToString();
                this.output.OpenAnchor(str);
                this.hyperLinkActive = true;
                if (this.urlCompareSink == null)
                  this.urlCompareSink = new UrlCompareSink();
                this.urlCompareSink.Initialize(str);
              }
            }
          }
          else if (RtfSupport.IsIncludePictureField(ref this.scratch, out linkUrl))
          {
            linkUrl.TrimWhitespace();
            if (linkUrl.Length != 0)
            {
              this.includePictureField = true;
              linkUrl.TrimWhitespace();
              if (linkUrl.Length != 0)
                this.imageUrl = linkUrl.ToString();
              this.pictureWidth = 0;
              this.pictureHeight = 0;
            }
          }
          else
          {
            TextMapping textMapping;
            char symbol;
            short points;
            if (RtfSupport.IsSymbolField(ref this.scratch, out textMapping, out symbol, out points))
            {
              if (this.output.LineEmpty && this.lineIndent >= 120 && this.lineIndent < 7200)
                this.output.OutputSpace(this.lineIndent / 120);
              this.output.OutputNonspace((int) symbol, textMapping);
              this.skipFieldResult = true;
            }
          }
          this.scratch.Reset();
          break;
        case RtfDestination.Picture:
          if (this.state.ParentDestination == RtfDestination.Picture || this.includePictureField)
            break;
          this.output.OutputImage((string) null, (string) null, new Format.PropertyValue(Format.LengthUnits.Twips, this.pictureWidth).PixelsInteger, new Format.PropertyValue(Format.LengthUnits.Twips, this.pictureHeight).PixelsInteger);
          break;
        case RtfDestination.ShapeName:
          if (this.state.ParentDestination == RtfDestination.ShapeName)
            break;
          BufferString bufferString1 = this.scratch.BufferString;
          bufferString1.TrimWhitespace();
          this.descriptionProperty = bufferString1.EqualsToLowerCaseStringIgnoreCase("wzdescription");
          this.scratch.Reset();
          break;
        case RtfDestination.ShapeValue:
          if (this.state.ParentDestination == RtfDestination.ShapeValue || !this.descriptionProperty)
            break;
          BufferString bufferString2 = this.scratch.BufferString;
          bufferString2.TrimWhitespace();
          if (bufferString2.Length != 0)
            this.imageAltText = bufferString2.ToString();
          this.scratch.Reset();
          break;
      }
    }

    internal struct RtfState
    {
      public int Level;
      public int SkipLevel;
      public RtfToTextConverter.RtfState.State Current;
      private RtfToTextConverter.RtfState.State[] stack;
      private int stackTop;

      public RtfDestination Destination
      {
        get
        {
          return this.Current.Dest;
        }
        set
        {
          if (value == this.Current.Dest)
            return;
          this.SetDirty();
          this.Current.Dest = value;
        }
      }

      public RtfDestination ParentDestination
      {
        get
        {
          if ((int) this.Current.LevelsDeep <= 1 && this.Level != 0)
            return this.stack[this.stackTop - 1].Dest;
          return this.Current.Dest;
        }
      }

      public bool IsInvisible
      {
        get
        {
          return this.Current.Invisible;
        }
        set
        {
          if (value == this.Current.Invisible)
            return;
          this.SetDirty();
          this.Current.Invisible = value;
        }
      }

      public RtfState(int initialStackSize)
      {
        this.Level = 0;
        this.SkipLevel = 0;
        this.Current = new RtfToTextConverter.RtfState.State();
        this.stack = new RtfToTextConverter.RtfState.State[initialStackSize];
        this.stackTop = 0;
        this.Current.LevelsDeep = (short) 1;
        this.Current.Dest = RtfDestination.RTF;
      }

      public void Push()
      {
        ++this.Level;
        ++this.Current.LevelsDeep;
        if ((int) this.Current.LevelsDeep != (int) short.MaxValue)
          return;
        this.PushReally();
      }

      public void SetDirty()
      {
        if ((int) this.Current.LevelsDeep <= 1)
          return;
        this.PushReally();
      }

      public void Pop()
      {
        if (this.Level <= 1)
          return;
        --this.Current.LevelsDeep;
        if ((int) this.Current.LevelsDeep == 0)
          this.Current = this.stack[--this.stackTop];
        --this.Level;
      }

      public void SkipGroup()
      {
        this.SkipLevel = this.Level;
      }

      private void PushReally()
      {
        if (this.Level >= 4096)
          throw new TextConvertersException(CtsResources.TextConvertersStrings.InputDocumentTooComplex);
        if (this.stackTop == this.stack.Length)
        {
          RtfToTextConverter.RtfState.State[] stateArray = new RtfToTextConverter.RtfState.State[this.stackTop * 2];
          Array.Copy((Array) this.stack, 0, (Array) stateArray, 0, this.stackTop);
          this.stack = stateArray;
        }
        this.stack[this.stackTop] = this.Current;
        --this.stack[this.stackTop].LevelsDeep;
        ++this.stackTop;
        this.Current.LevelsDeep = (short) 1;
      }

      public struct State
      {
        public short LevelsDeep;
        public RtfDestination Dest;
        public bool Invisible;
      }
    }
  }
}
