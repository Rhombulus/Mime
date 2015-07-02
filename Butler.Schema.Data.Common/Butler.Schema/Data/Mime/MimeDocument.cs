// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.MimeDocument
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  public class MimeDocument : IDisposable
  {
    private static bool fixMime = true;
    private bool dangerousFixBadMimeBoundary = true;
    private DecodingOptions decodingOptions = DecodingOptions.Default;
    private bool searchMimeTreeCharset = true;
    private bool createValidateStorage = true;
    private int cachedSizeVersion = -1;
    private MimeDocument.MimeDocumentThreadAccessToken accessToken;
    private Internal.DataStorage backingStorage;
    private long backingStorageOffset;
    private Stream backingStorageWriteStream;
    private MimePart root;
    private MimeComplianceMode complianceMode;
    private MimeComplianceStatus complianceStatus;
    private EncodingOptions encodingOptions;
    private Globalization.Charset mimeTreeCharset;
    private MimeReader reader;
    private MimeLimits limits;
    private long contentStart;
    private long headersEnd;
    private ContentTransferEncoding contentTransferEncoding;
    private MimeDocument.ContentPositionEntry[] contentPositionStack;
    private int contentPositionStackTop;
    private MimePart lastPart;
    private long parsedSize;
    private MimeDocument.EndOfHeadersCallback eohCallback;
      private bool stopLoading;
    private bool loadEmbeddedMessages;
    private bool isDisposed;
      private bool parseCompletely;
    private int embeddedMessagePartDepth;
    private long cachedSize;
    private bool expectBinaryContent;

    public MimeDocument.EndOfHeadersCallback EndOfHeaders
    {
      get
      {
        this.ThrowIfDisposed();
        using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
          return this.eohCallback;
      }
      set
      {
        this.ThrowIfDisposed();
        using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
          this.eohCallback = value;
      }
    }

    public MimePart RootPart
    {
      get
      {
        this.ThrowIfDisposed();
        using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
          return this.root;
      }
      set
      {
        this.ThrowIfDisposed();
        using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
        {
          if (value == null)
            throw new ArgumentNullException(nameof(value));
          if (value.Parent != null)
            throw new ArgumentException(CtsResources.Strings.RootPartCantHaveAParent);
          this.ThrowIfReadOnly("MimeDocument.set_RootPart");
          if (this.reader != null)
            throw new InvalidOperationException("Cannot set a new document root part while document loading is not complete");
          this.lastPart = (MimePart) null;
          this.contentStart = 0L;
          this.complianceStatus = MimeComplianceStatus.Compliant;
          this.stopLoading = false;
          if (this.root != null)
            this.root.ParentDocument = (MimeDocument) null;
          this.root = value;
          this.root.ParentDocument = this;
          this.parsedSize = 0L;
          this.IncrementVersion();
        }
      }
    }

    public DecodingOptions HeaderDecodingOptions
    {
      get
      {
        this.ThrowIfDisposed();
        using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
          return this.decodingOptions;
      }
      internal set
      {
        this.ThrowIfDisposed();
        this.ThrowIfReadOnly("MimeDocument.set_HeaderDecodingOptions");
        using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
          this.decodingOptions = value;
      }
    }

    public MimeLimits MimeLimits
    {
      get
      {
        this.ThrowIfDisposed();
        using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
          return this.limits;
      }
    }

    public MimeComplianceMode ComplianceMode
    {
      get
      {
        this.ThrowIfDisposed();
        using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
          return this.complianceMode;
      }
      set
      {
        this.ThrowIfDisposed();
        this.ThrowIfReadOnly("MimeDocument.set_ComplianceMode");
        using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
          this.complianceMode = value;
      }
    }

    public MimeComplianceStatus ComplianceStatus
    {
      get
      {
        this.ThrowIfDisposed();
        using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
          return this.complianceStatus;
      }
    }

    public bool RequiresSMTPUTF8
    {
      get
      {
        this.ThrowIfDisposed();
        using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
          return this.root.RequiresSMTPUTF8;
      }
    }

    public int Version { get; private set; }

      internal static bool FixMimeForTestUseOnly
    {
      get
      {
        return MimeDocument.fixMime;
      }
      set
      {
        MimeDocument.fixMime = value;
      }
    }

    internal ObjectThreadAccessToken AccessToken => (ObjectThreadAccessToken) this.accessToken;

      internal bool IsReadOnly { get; private set; }

      internal DecodingOptions EffectiveHeaderDecodingOptions
    {
      get
      {
        this.ThrowIfDisposed();
        using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
        {
          DecodingOptions decodingOptions = this.decodingOptions;
          Globalization.Charset mimeTreeCharset = this.GetMimeTreeCharset();
          if (mimeTreeCharset != null)
            decodingOptions.Charset = mimeTreeCharset;
          return decodingOptions;
        }
      }
    }

    internal EncodingOptions EncodingOptions
    {
      get
      {
        this.ThrowIfDisposed();
        using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
        {
          if (this.encodingOptions == null)
            this.encodingOptions = new EncodingOptions(this.GetMimeTreeCharset());
          return this.encodingOptions;
        }
      }
    }

    internal bool CreateValidateStorage
    {
      set
      {
        using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
          this.createValidateStorage = value;
      }
    }

    internal long Position
    {
      get
      {
        using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
          return this.reader.StreamOffset;
      }
    }

    internal long ParsedSize
    {
      get
      {
        using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
          return this.parsedSize;
      }
    }

    private bool CreateDomObjects
    {
      get
      {
        using (ThreadAccessGuard.EnterPrivate((ObjectThreadAccessToken) this.accessToken))
          return this.loadEmbeddedMessages || this.lastPart == null || !this.lastPart.IsEmbeddedMessage;
      }
    }

    public MimeDocument()
      : this(DecodingOptions.Default, MimeLimits.Default)
    {
    }

    public MimeDocument(DecodingOptions headerDecodingOptions, MimeLimits mimeLimits)
    {
      if (mimeLimits == null)
        throw new ArgumentNullException(nameof(mimeLimits));
      this.decodingOptions = headerDecodingOptions;
      this.limits = mimeLimits;
      this.accessToken = new MimeDocument.MimeDocumentThreadAccessToken(this);
    }

    public MimePart Load(Stream stream, CachingMode cachingMode)
    {
      this.ThrowIfDisposed();
      this.ThrowIfReadOnly("MimeDocument.Load");
      using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
      {
        if (stream == null)
          throw new ArgumentNullException(nameof(stream));
        if (this.root != null)
          throw new InvalidOperationException(CtsResources.Strings.CannotLoadIntoNonEmptyDocument);
        if (this.reader != null)
          throw new InvalidOperationException("Cannot load document again while previous load is not complete");
        switch (cachingMode)
        {
          case CachingMode.Copy:
            this.InitializePushMode(true);
            bool discard = true;
            try
            {
              byte[] buffer = new byte[4096];
              while (!this.stopLoading)
              {
                int count = stream.Read(buffer, 0, buffer.Length);
                if (count != 0)
                  this.Write(buffer, 0, count);
                else
                  break;
              }
              discard = false;
              break;
            }
            finally
            {
              this.Flush(discard);
            }
          case CachingMode.Source:
          case CachingMode.SourceTakeOwnership:
            if (this.createValidateStorage)
            {
              if (!stream.CanSeek)
                throw new NotSupportedException(CtsResources.Strings.CachingModeSourceButStreamCannotSeek);
              stream.Position = 0L;
              this.backingStorage = (Internal.DataStorage) new Internal.ReadableDataStorageOnStream(stream, cachingMode == CachingMode.SourceTakeOwnership);
            }
            this.reader = new MimeReader(stream, true, this.decodingOptions, this.limits, true, true, this.expectBinaryContent);
            this.reader.DangerousSetFixBadMimeBoundary(this.dangerousFixBadMimeBoundary);
            try
            {
              this.BuildDom((byte[]) null, 0, 0, true);
            }
            finally
            {
              this.reader.DisconnectInputStream();
            }
            this.parsedSize = this.reader.StreamOffset;
            this.reader.Dispose();
            this.reader = (MimeReader) null;
            if (this.backingStorage != null)
            {
              this.backingStorage.Release();
              this.backingStorage = (Internal.DataStorage) null;
              break;
            }
            break;
          default:
            throw new ArgumentException("Invalid Caching Mode value", nameof(cachingMode));
        }
        return this.RootPart;
      }
    }

    public Stream GetLoadStream()
    {
      return this.GetLoadStream(true);
    }

    public MimeDocument Clone()
    {
      this.ThrowIfDisposed();
      using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
      {
        if (this.reader != null)
          throw new NotSupportedException(CtsResources.Strings.DocumentCloneNotSupportedInThisState);
        MimeDocument mimeDocument = (MimeDocument) this.MemberwiseClone();
        if (this.root != null)
        {
          mimeDocument.root = (MimePart) this.root.Clone();
          mimeDocument.root.ParentDocument = mimeDocument;
        }
        mimeDocument.contentPositionStack = (MimeDocument.ContentPositionEntry[]) null;
        mimeDocument.lastPart = (MimePart) null;
        return mimeDocument;
      }
    }

    public long WriteTo(Stream stream)
    {
      this.ThrowIfDisposed();
      using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
      {
        if (Stream.Null != stream || this.cachedSizeVersion != this.Version)
        {
          this.cachedSizeVersion = this.Version;
          this.cachedSize = this.root == null ? 0L : this.root.WriteTo(stream, this.EncodingOptions, (MimeOutputFilter) null);
        }
        return this.cachedSize;
      }
    }

    public long WriteTo(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter)
    {
      this.ThrowIfDisposed();
      using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
      {
        if (this.root == null)
          return 0L;
        if (encodingOptions == null)
          encodingOptions = this.EncodingOptions;
        return this.root.WriteTo(stream, encodingOptions, filter);
      }
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    internal void DangerousSetFixBadMimeBoundary(bool value)
    {
      if (this.reader != null)
        throw new InvalidOperationException("Cannot change FixBadMimeBoundary flag while previous load is not complete");
      this.dangerousFixBadMimeBoundary = value;
    }

    internal Stream GetLoadStream(bool expectBinaryContent)
    {
      this.ThrowIfDisposed();
      this.ThrowIfReadOnly("MimeDocument.GetLoadStream");
      using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
      {
        if (this.root != null)
          throw new InvalidOperationException(CtsResources.Strings.CannotLoadIntoNonEmptyDocument);
        if (this.reader != null)
          throw new InvalidOperationException(CtsResources.Strings.CannotGetLoadStreamMoreThanOnce);
        this.expectBinaryContent = expectBinaryContent;
        this.InitializePushMode(this.expectBinaryContent);
        return (Stream) new MimeDocument.PushStream(this);
      }
    }

    internal void SetReadOnly(bool makeReadOnly)
    {
      this.ThrowIfDisposed();
      using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
      {
        if (makeReadOnly == this.IsReadOnly)
          return;
        if (makeReadOnly)
          this.CompleteParse();
        this.SetReadOnlyInternal(makeReadOnly);
      }
    }

    internal void CompleteParse()
    {
      using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
      {
        this.BuildDomAndCompleteParse(this.RootPart);
        EncodingOptions encodingOptions = this.EncodingOptions;
        this.GetMimeTreeCharset();
      }
    }

    internal void SetReadOnlyInternal(bool makeReadOnly)
    {
      using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
      {
        this.IsReadOnly = makeReadOnly;
        using (MimePart.SubtreeEnumerator enumerator = this.root.Subtree.GetEnumerator(MimePart.SubtreeEnumerationOptions.IncludeEmbeddedMessages, false))
        {
          while (enumerator.MoveNext())
            enumerator.Current.SetReadOnlyInternal(makeReadOnly);
        }
      }
    }

    internal void IncrementVersion()
    {
      using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
        this.Version = int.MaxValue == this.Version ? 1 : this.Version + 1;
    }

    internal void BuildEmbeddedDom(MimePart part)
    {
      this.ThrowIfDisposed();
      this.ThrowIfReadOnly("MimeDocument.BuildEmbeddedDom");
      using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
      {
        if (this.reader != null || this.stopLoading)
          return;
        MimePart mimePart = this.root;
        bool flag = this.loadEmbeddedMessages;
        MimeDocument.EndOfHeadersCallback ofHeadersCallback = this.eohCallback;
        this.eohCallback = (MimeDocument.EndOfHeadersCallback) null;
        this.loadEmbeddedMessages = true;
        this.reader = new MimeReader((Stream) null, true, this.decodingOptions, this.limits, true, true, this.expectBinaryContent);
        this.reader.DangerousSetFixBadMimeBoundary(this.dangerousFixBadMimeBoundary);
        try
        {
          using (MimePart.SubtreeEnumerator enumerator = part.Subtree.GetEnumerator(MimePart.SubtreeEnumerationOptions.IncludeEmbeddedMessages, true))
          {
            while (enumerator.MoveNext())
            {
              MimePart current = enumerator.Current;
              if (current.InternalLastChild == null && current.Storage != null && current.IsEmbeddedMessage)
              {
                this.ParseOnePart(current);
                this.root.ParentDocument = (MimeDocument) null;
                current.InternalInsertAfter((MimeNode) this.root, (MimeNode) null);
              }
            }
          }
        }
        finally
        {
          this.reader.DisconnectInputStream();
          this.reader.Dispose();
          this.reader = (MimeReader) null;
          if (this.backingStorage != null)
          {
            this.backingStorage.Release();
            this.backingStorage = (Internal.DataStorage) null;
          }
          this.backingStorageOffset = 0L;
          this.root = mimePart;
          this.loadEmbeddedMessages = flag;
          this.eohCallback = ofHeadersCallback;
        }
      }
    }

    internal void BuildDomAndCompleteParse(MimePart rootPart)
    {
      using (ThreadAccessGuard.EnterPublic((ObjectThreadAccessToken) this.accessToken))
      {
        if (this.reader != null)
          throw new InvalidOperationException("do not call BuildDomAndCompleteParse() before Load is complete");
        if (this.stopLoading)
          throw new InvalidOperationException("do not call BuildDomAndCompleteParse() after canceling Load");
        if (rootPart.InternalLastChild == null && rootPart.Storage == null)
        {
          this.ParseAllHeaders(rootPart);
        }
        else
        {
          MimePart mimePart1 = this.root;
          bool flag1 = this.loadEmbeddedMessages;
          bool flag2 = this.parseCompletely;
          MimeDocument.EndOfHeadersCallback ofHeadersCallback = this.eohCallback;
          this.eohCallback = (MimeDocument.EndOfHeadersCallback) null;
          this.loadEmbeddedMessages = true;
          this.parseCompletely = true;
          this.reader = new MimeReader((Stream) null, true, this.decodingOptions, this.limits, true, true, this.expectBinaryContent);
          this.reader.DangerousSetFixBadMimeBoundary(this.dangerousFixBadMimeBoundary);
          try
          {
            Stack<MimePart> stack = new Stack<MimePart>(5);
            stack.Push(rootPart);
label_13:
            if (stack.Count <= 0)
              return;
            MimePart part = stack.Pop();
            do
            {
              MimeNode internalLastChild = part.InternalLastChild;
              this.ParseAllHeaders(part);
              MimePart mimePart2 = part.FirstChild as MimePart;
              if (mimePart2 != null)
                stack.Push(mimePart2);
              part = part.NextSibling as MimePart;
            }
            while (part != null);
            goto label_13;
          }
          finally
          {
            this.reader.DisconnectInputStream();
            this.reader.Dispose();
            this.reader = (MimeReader) null;
            if (this.backingStorage != null)
            {
              this.backingStorage.Release();
              this.backingStorage = (Internal.DataStorage) null;
            }
            this.backingStorageOffset = 0L;
            this.root = mimePart1;
            this.loadEmbeddedMessages = flag1;
            this.parseCompletely = flag2;
            this.eohCallback = ofHeadersCallback;
          }
        }
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing && !this.isDisposed)
      {
        if (this.backingStorageWriteStream != null)
        {
          this.backingStorageWriteStream.Dispose();
          this.backingStorageWriteStream = (Stream) null;
        }
        if (this.backingStorage != null)
        {
          this.backingStorage.Release();
          this.backingStorage = (Internal.DataStorage) null;
        }
        if (this.reader != null)
        {
          this.reader.Dispose();
          this.reader = (MimeReader) null;
        }
        if (this.root != null)
        {
          this.root.Dispose();
          this.root = (MimePart) null;
        }
      }
      this.isDisposed = true;
    }

    private static bool IsContentBinary(Stream stream, int bytesToExamine, int thresholdPercentage)
    {
      int offset = 0;
      int num1 = 0;
      byte[] buffer = new byte[bytesToExamine];
      while (offset < buffer.Length)
      {
        int num2 = stream.Read(buffer, offset, buffer.Length - offset);
        if (num2 != 0)
          offset += num2;
        else
          break;
      }
      if (offset < 1)
        return false;
      for (int index = 0; index < offset; ++index)
      {
        if (((int) buffer[index] & 128) != 0)
          ++num1;
      }
      return num1 * 100 / offset >= thresholdPercentage;
    }

    private Globalization.Charset GetMimeTreeCharset()
    {
      this.ThrowIfDisposed();
      using (ThreadAccessGuard.EnterPrivate((ObjectThreadAccessToken) this.accessToken))
      {
        if (this.searchMimeTreeCharset)
        {
          this.searchMimeTreeCharset = false;
          if (this.root != null)
            this.mimeTreeCharset = this.root.FindMimeTreeCharset();
        }
        return this.mimeTreeCharset;
      }
    }

    private void ParseOnePart(MimePart nextPart)
    {
      using (ThreadAccessGuard.EnterPrivate((ObjectThreadAccessToken) this.accessToken))
      {
        this.root = (MimePart) null;
        try
        {
          this.backingStorage = nextPart.Storage;
          this.backingStorage.AddRef();
          this.backingStorageOffset = nextPart.DataStart + nextPart.BodyOffset;
          Stream contentReadStream;
          using (contentReadStream = nextPart.GetRawContentReadStream())
          {
            this.reader.Reset(contentReadStream);
            this.BuildDom((byte[]) null, 0, 0, true, nextPart.IsEmbeddedMessage);
          }
        }
        finally
        {
          this.backingStorage.Release();
          this.backingStorage = (Internal.DataStorage) null;
        }
      }
    }

    private void Flush(bool discard)
    {
      this.ThrowIfDisposed();
      this.ThrowIfReadOnly("MimeDocument.Flush");
      using (ThreadAccessGuard.EnterPrivate((ObjectThreadAccessToken) this.accessToken))
      {
        if (this.reader == null)
          return;
        if (!discard)
        {
          this.backingStorageWriteStream.Flush();
          if (!this.stopLoading)
            this.BuildDom((byte[]) null, 0, 0, true);
        }
        this.parsedSize = this.reader.StreamOffset;
        this.reader.Dispose();
        this.reader = (MimeReader) null;
        this.backingStorageWriteStream.Dispose();
        this.backingStorageWriteStream = (Stream) null;
        this.backingStorage.Release();
        this.backingStorage = (Internal.DataStorage) null;
      }
    }

    private void Write(byte[] buffer, int offset, int count)
    {
      this.ThrowIfDisposed();
      this.ThrowIfReadOnly("MimeDocument.Write");
      using (ThreadAccessGuard.EnterPrivate((ObjectThreadAccessToken) this.accessToken))
      {
        if (this.reader == null)
          throw new InvalidOperationException(CtsResources.Strings.CannotWriteAfterFlush);
        if (count == 0)
          return;
        this.backingStorageWriteStream.Write(buffer, offset, count);
        if (this.stopLoading)
          return;
        this.BuildDom(buffer, offset, count, false);
      }
    }

    private void InitializePushMode(bool expectBinaryContent)
    {
      using (ThreadAccessGuard.EnterPrivate((ObjectThreadAccessToken) this.accessToken))
      {
        Internal.TemporaryDataStorage temporaryDataStorage = new Internal.TemporaryDataStorage();
        this.backingStorage = (Internal.DataStorage) temporaryDataStorage;
        this.backingStorageWriteStream = (Stream) temporaryDataStorage.OpenWriteStream(true);
        this.reader = new MimeReader((Stream) null, true, this.decodingOptions, this.limits, true, true, expectBinaryContent);
        this.reader.DangerousSetFixBadMimeBoundary(this.dangerousFixBadMimeBoundary);
      }
    }

    private void BuildDom(byte[] buffer, int offset, int length, bool eof)
    {
      this.BuildDom(buffer, offset, length, eof, false);
    }

    private void BuildDom(byte[] buffer, int offset, int length, bool eof, bool parseHeaders)
    {
      using (ThreadAccessGuard.EnterPrivate((ObjectThreadAccessToken) this.accessToken))
      {
        while (!this.reader.EndOfFile && (!this.reader.DataExhausted || length != 0 || eof) && !this.stopLoading)
        {
          if (this.reader.DataExhausted)
          {
            int num = this.reader.AddMoreData(buffer, offset, length, eof);
            offset += num;
            length -= num;
          }
          if (this.reader.TryReachNextState())
          {
            MimeComplianceStatus complianceStatus = this.reader.ComplianceStatus;
            switch (this.reader.ReaderState)
            {
              case MimeReaderState.InlineBody:
              case MimeReaderState.End:
              case MimeReaderState.PartBody:
                if (this.reader.ReaderState != MimeReaderState.PartBody)
                {
                  this.complianceStatus |= complianceStatus;
                  if (this.ComplianceMode == MimeComplianceMode.Strict && this.ComplianceStatus != MimeComplianceStatus.Compliant)
                    throw new MimeException(CtsResources.Strings.StrictComplianceViolation);
                  continue;
                }
                continue;
              case MimeReaderState.InlineEnd:
                complianceStatus = this.CompletePart(true, parseHeaders);
                goto case MimeReaderState.InlineBody;
              case MimeReaderState.PartEnd:
                complianceStatus = this.CompletePart(false, parseHeaders);
                goto case MimeReaderState.InlineBody;
              case MimeReaderState.InlineStart:
                this.StartPart(true);
                goto case MimeReaderState.InlineBody;
              case MimeReaderState.EndOfHeaders:
                this.EndPartHeaders();
                goto case MimeReaderState.InlineBody;
              case MimeReaderState.PartStart:
                this.StartPart(false);
                goto case MimeReaderState.InlineBody;
              case MimeReaderState.HeaderStart:
                if (!this.reader.TryCompleteCurrentHeader(this.embeddedMessagePartDepth == 0 || this.CreateDomObjects))
                  goto case MimeReaderState.InlineBody;
                else
                  goto case MimeReaderState.HeaderComplete;
              case MimeReaderState.HeaderComplete:
                if (this.embeddedMessagePartDepth == 0 || this.CreateDomObjects)
                {
                  Header currentHeaderObject = this.reader.CurrentHeaderObject;
                  if (currentHeaderObject != null)
                  {
                    this.lastPart.Headers.InternalAppendChild((MimeNode) currentHeaderObject);
                    goto case MimeReaderState.InlineBody;
                  }
                  else
                    goto case MimeReaderState.InlineBody;
                }
                else
                  goto case MimeReaderState.InlineBody;
              default:
                throw new InvalidOperationException("unexpected reader state");
            }
          }
        }
      }
    }

    private void StartPart(bool inline)
    {
      using (ThreadAccessGuard.EnterPrivate((ObjectThreadAccessToken) this.accessToken))
      {
        if (this.CreateDomObjects)
        {
          MimePart part1 = new MimePart();
          if (this.root == null)
          {
            this.root = part1;
            part1.ParentDocument = this;
          }
          else
          {
            if (inline)
            {
              if (!this.root.IsMultipart)
              {
                MimePart mimePart = this.lastPart == null ? this.root : (MimePart) this.lastPart.FirstChild;
                MimePart part2 = new MimePart();
                Header first1 = mimePart.Headers.FindFirst(HeaderId.ContentType);
                Header first2 = mimePart.Headers.FindFirst(HeaderId.ContentTransferEncoding);
                if (first1 != null)
                {
                  mimePart.Headers.InternalRemoveChild((MimeNode) first1);
                  part2.Headers.InternalAppendChild((MimeNode) first1);
                }
                if (first2 != null)
                {
                  mimePart.Headers.InternalRemoveChild((MimeNode) first2);
                  part2.Headers.InternalAppendChild((MimeNode) first2);
                }
                Header header1 = (Header) new AsciiTextHeader("X-ConvertedToMime", HeaderId.Unknown);
                header1.RawValue = MimeString.ConvertedToMimeUU;
                mimePart.Headers.InternalAppendChild((MimeNode) header1);
                Internal.DataStorage storage = mimePart.Storage;
                part2.SetStorageImpl(storage, mimePart.DataStart + mimePart.BodyOffset, mimePart.DataEnd, 0L, mimePart.BodyCte, mimePart.BodyLineTermination);
                mimePart.SetStorageImpl((Internal.DataStorage) null, 0L, 0L);
                Header header2 = (Header) new ContentTypeHeader("multipart/mixed");
                mimePart.Headers.InternalInsertAfter((MimeNode) header2, (MimeNode) null);
                mimePart.InternalAppendChild((MimeNode) part2);
                this.lastPart = mimePart;
                if (this.eohCallback != null)
                {
                  bool stopLoading;
                  this.eohCallback(part2, out stopLoading);
                  if (stopLoading)
                    this.stopLoading = true;
                }
              }
              string str = this.reader.InlineFileName;
              if (string.IsNullOrEmpty(str))
                str = "unnamed.dat";
              Header header3 = Header.Create(HeaderId.ContentTransferEncoding);
              header3.RawValue = MimeString.Uuencode;
              part1.Headers.InternalAppendChild((MimeNode) header3);
              Header header4 = (Header) new ContentDispositionHeader("attachment");
              MimeParameter mimeParameter1 = new MimeParameter("filename", str);
              header4.InternalAppendChild((MimeNode) mimeParameter1);
              part1.Headers.InternalAppendChild((MimeNode) header4);
              Header header5 = (Header) new ContentTypeHeader("application/octet-stream");
              MimeParameter mimeParameter2 = new MimeParameter("name", str);
              header5.InternalAppendChild((MimeNode) mimeParameter2);
              part1.Headers.InternalAppendChild((MimeNode) header5);
              if (this.eohCallback != null)
              {
                bool stopLoading;
                this.eohCallback(part1, out stopLoading);
                if (stopLoading)
                  this.stopLoading = true;
              }
            }
            this.lastPart.InternalInsertAfter((MimeNode) part1, this.lastPart.InternalLastChild);
          }
          if (this.contentPositionStack == null)
            this.contentPositionStack = new MimeDocument.ContentPositionEntry[4];
          else if (this.contentPositionStack.Length == this.contentPositionStackTop)
          {
            MimeDocument.ContentPositionEntry[] contentPositionEntryArray = new MimeDocument.ContentPositionEntry[this.contentPositionStack.Length * 2];
            Array.Copy((Array) this.contentPositionStack, 0, (Array) contentPositionEntryArray, 0, this.contentPositionStackTop);
            this.contentPositionStack = contentPositionEntryArray;
          }
          this.contentPositionStack[this.contentPositionStackTop++] = new MimeDocument.ContentPositionEntry(this.contentStart, this.headersEnd, this.contentTransferEncoding);
          this.lastPart = part1;
          this.contentStart = this.headersEnd = this.reader.StreamOffset;
          this.contentTransferEncoding = inline ? this.reader.ContentTransferEncoding : ContentTransferEncoding.Unknown;
        }
        else if (this.embeddedMessagePartDepth == 0)
          this.embeddedMessagePartDepth = this.reader.Depth;
        this.complianceStatus |= this.reader.ComplianceStatus;
        this.reader.ResetComplianceStatus();
      }
    }

    private void EndPartHeaders()
    {
      using (ThreadAccessGuard.EnterPrivate((ObjectThreadAccessToken) this.accessToken))
      {
        this.complianceStatus |= this.reader.ComplianceStatus;
        this.reader.ResetComplianceStatus();
        if (this.embeddedMessagePartDepth != 0 && !this.CreateDomObjects)
          return;
        this.headersEnd = this.reader.StreamOffset;
        this.contentTransferEncoding = this.reader.ContentTransferEncoding;
        if (MimeDocument.fixMime)
        {
          if ((this.lastPart == this.root || ((MimePart) this.lastPart.Parent).IsEmbeddedMessage) && this.lastPart.Headers.FindFirst(HeaderId.MimeVersion) == null)
          {
            Header header = Header.Create(HeaderId.MimeVersion);
            header.RawValue = MimeString.Version1;
            this.lastPart.Headers.InternalAppendChild((MimeNode) header);
          }
          if (this.lastPart.Headers.FindFirst(HeaderId.ContentType) == null)
          {
            bool flag = false;
            MimePart mimePart = this.lastPart.Parent as MimePart;
            if (mimePart != null && mimePart.Headers.FindFirst(HeaderId.ContentType).Value == "multipart/digest")
              flag = true;
            this.lastPart.Headers.InternalAppendChild((MimeNode) new ContentTypeHeader(flag ? "message/rfc822" : "text/plain"));
          }
        }
        if (this.parseCompletely)
          this.ParseAllHeaders(this.lastPart);
        if (this.eohCallback == null)
          return;
        bool stopLoading;
        this.eohCallback(this.lastPart, out stopLoading);
        if (!stopLoading)
          return;
        this.stopLoading = true;
      }
    }

    private void ParseAllHeaders(MimePart part)
    {
      using (ThreadAccessGuard.EnterPrivate((ObjectThreadAccessToken) this.accessToken))
      {
        foreach (Header header in part.Headers)
          header.ForceParse();
      }
    }

    private MimeComplianceStatus CompletePart(bool inline, bool parseHeaders)
    {
      using (ThreadAccessGuard.EnterPrivate((ObjectThreadAccessToken) this.accessToken))
      {
        MimeComplianceStatus complianceStatus1 = this.reader.ComplianceStatus;
        if (this.embeddedMessagePartDepth == 0 || this.CreateDomObjects)
        {
          if (this.createValidateStorage)
          {
            if (parseHeaders)
              this.ParseAllHeaders(this.lastPart);
            this.lastPart.SetStorageImpl(this.backingStorage, this.contentStart + this.backingStorageOffset, this.reader.StreamOffset + this.backingStorageOffset, this.headersEnd - this.contentStart, this.contentTransferEncoding, this.reader.LineTerminationState);
            MimeComplianceStatus complianceStatus2 = MimeComplianceStatus.InvalidWrapping | MimeComplianceStatus.BareLinefeedInBody | MimeComplianceStatus.UnexpectedBinaryContent;
            if (MimeDocument.fixMime && (complianceStatus2 & complianceStatus1) != MimeComplianceStatus.Compliant && this.FixPartContent())
              complianceStatus1 &= ~complianceStatus2;
          }
          ContentTypeHeader contentTypeHeader = this.lastPart.Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
          if (contentTypeHeader != null && contentTypeHeader.IsMultipart && !this.lastPart.HasChildren)
            contentTypeHeader.RawValue = MimeString.TextPlain;
          this.lastPart = this.lastPart.Parent as MimePart;
          --this.contentPositionStackTop;
          this.contentStart = this.contentPositionStack[this.contentPositionStackTop].ContentStart;
          this.headersEnd = this.contentPositionStack[this.contentPositionStackTop].HeadersEnd;
          this.contentTransferEncoding = this.contentPositionStack[this.contentPositionStackTop].ContentTransferEncoding;
        }
        if (0 < this.embeddedMessagePartDepth && this.embeddedMessagePartDepth == this.reader.Depth)
          this.embeddedMessagePartDepth = 0;
        this.reader.ResetComplianceStatus();
        return complianceStatus1;
      }
    }

    private bool FixPartContent()
    {
      using (ThreadAccessGuard.EnterPrivate((ObjectThreadAccessToken) this.accessToken))
      {
        ContentTypeHeader contentTypeHeader1 = this.lastPart.Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
        if (contentTypeHeader1.IsMultipart || contentTypeHeader1.MediaType == "message" || this.lastPart.BodyCte == ContentTransferEncoding.Unknown)
          return false;
        MimePart mimePart1 = this.lastPart;
        do
        {
          MimePart mimePart2 = mimePart1.Parent as MimePart;
          if (mimePart2 != null)
          {
            if (mimePart1 == mimePart2.FirstChild)
            {
              ContentTypeHeader contentTypeHeader2 = mimePart2.Headers.FindFirst(HeaderId.ContentType) as ContentTypeHeader;
              if (contentTypeHeader2 != null && contentTypeHeader2.Value == "multipart/signed")
                return false;
            }
            if (mimePart2.Headers.FindFirst("DKIM-Signature") != null)
              return false;
          }
          mimePart1 = mimePart2;
        }
        while (mimePart1 != null);
        Header first = this.lastPart.Headers.FindFirst(HeaderId.ContentTransferEncoding);
        if (first == null)
        {
          Header header = Header.Create(HeaderId.ContentTransferEncoding);
          this.lastPart.Headers.InternalAppendChild((MimeNode) header);
          header.RawValue = MimeString.QuotedPrintable;
          return true;
        }
        ContentTransferEncoding encodingType = MimePart.GetEncodingType(first.FirstRawToken);
        switch (encodingType)
        {
          case ContentTransferEncoding.Unknown:
          case ContentTransferEncoding.SevenBit:
          case ContentTransferEncoding.EightBit:
            first.RawValue = MimeString.QuotedPrintable;
            return true;
          case ContentTransferEncoding.QuotedPrintable:
            this.ForceReencoding(encodingType);
            return true;
          case ContentTransferEncoding.Base64:
            bool flag = false;
            if ((this.reader.ComplianceStatus & MimeComplianceStatus.UnexpectedBinaryContent) != MimeComplianceStatus.Compliant)
            {
              long start = this.lastPart.DataStart + this.lastPart.BodyOffset;
              long num = Math.Min(this.lastPart.DataEnd - start, 1000L);
              long end = start + num;
              if (num > 10L)
              {
                using (Stream stream = this.lastPart.Storage.OpenReadStream(start, end))
                  flag = MimeDocument.IsContentBinary(stream, (int) num, 10);
              }
            }
            if (flag)
              this.RepairBrokenExchangeMime(encodingType);
            else
              this.ForceReencoding(encodingType);
            return true;
          default:
            return false;
        }
      }
    }

    private void RepairBrokenExchangeMime(ContentTransferEncoding encoding)
    {
      MimeDocument.EncodingDataStorage encodingDataStorage = new MimeDocument.EncodingDataStorage(this.lastPart.Storage, this.lastPart.DataStart + this.lastPart.BodyOffset, this.lastPart.DataEnd, encoding);
      this.lastPart.SetStorageImpl((Internal.DataStorage) encodingDataStorage, 0L, long.MaxValue, 0L, encoding, LineTerminationState.CRLF);
      encodingDataStorage.Release();
    }

    private void ForceReencoding(ContentTransferEncoding encoding)
    {
      MimeDocument.DecodingDataStorage decodingDataStorage = new MimeDocument.DecodingDataStorage(this.lastPart.Storage, this.lastPart.DataStart + this.lastPart.BodyOffset, this.lastPart.DataEnd, encoding);
      this.lastPart.SetStorageImpl((Internal.DataStorage) decodingDataStorage, 0L, long.MaxValue, 0L, ContentTransferEncoding.Binary, LineTerminationState.CRLF);
      decodingDataStorage.Release();
    }

    private void ThrowIfDisposed()
    {
      if (this.isDisposed)
        throw new ObjectDisposedException("MimeDocument");
    }

    private void ThrowIfReadOnly(string method)
    {
      if (this.IsReadOnly)
        throw new ReadOnlyMimeException(method);
    }

    public delegate void EndOfHeadersCallback(MimePart part, out bool stopLoading);

    private struct ContentPositionEntry
    {
      public long ContentStart;
      public long HeadersEnd;
      public ContentTransferEncoding ContentTransferEncoding;

      public ContentPositionEntry(long contentStart, long headersEnd, ContentTransferEncoding contentTransferEncoding)
      {
        this.ContentStart = contentStart;
        this.HeadersEnd = headersEnd;
        this.ContentTransferEncoding = contentTransferEncoding;
      }
    }

    private class MimeDocumentThreadAccessToken : ObjectThreadAccessToken
    {
      internal MimeDocumentThreadAccessToken(MimeDocument parent)
      {
      }
    }

    private class PushStream : Stream
    {
      private MimeDocument document;
      private bool badState;

      public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => this.document != null;

        public override long Length
      {
        get
        {
          throw new NotSupportedException();
        }
      }

      public override long Position
      {
        get
        {
          throw new NotSupportedException();
        }
        set
        {
          throw new NotSupportedException();
        }
      }

      public PushStream(MimeDocument document)
      {
        this.document = document;
      }

      public override void Flush()
      {
        if (this.document == null)
          throw new ObjectDisposedException("stream");
        using (ThreadAccessGuard.EnterPublic(this.document.AccessToken))
        {
          if (this.badState)
            return;
          if (this.document.stopLoading)
            throw new InvalidOperationException(CtsResources.Strings.LoadingStopped);
          this.badState = true;
          this.document.Flush(false);
          this.badState = false;
        }
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        throw new NotSupportedException();
      }

      public override long Seek(long offset, SeekOrigin origin)
      {
        throw new NotSupportedException();
      }

      public override void SetLength(long length)
      {
        throw new NotSupportedException();
      }

      public override void Write(byte[] buffer, int offset, int count)
      {
        if (this.document == null)
          throw new ObjectDisposedException("stream");
        using (ThreadAccessGuard.EnterPublic(this.document.AccessToken))
        {
          if (this.badState)
            return;
          if (this.document.stopLoading)
            throw new InvalidOperationException(CtsResources.Strings.LoadingStopped);
          this.badState = true;
          this.document.Write(buffer, offset, count);
          this.badState = false;
        }
      }

      protected override void Dispose(bool disposing)
      {
        if (disposing && this.document != null)
        {
          using (ThreadAccessGuard.EnterPublic(this.document.AccessToken))
          {
            this.document.Flush(this.badState);
            this.document = (MimeDocument) null;
          }
        }
        base.Dispose(disposing);
      }
    }

    private class CodingDataStorage : Internal.DataStorage
    {
      private Internal.DataStorage storage;
      private long start;
      private long end;
      private ContentTransferEncoding cte;
      private bool encode;

      public CodingDataStorage(Internal.DataStorage storage, long start, long end, ContentTransferEncoding cte, bool encode)
      {
        storage.AddRef();
        this.storage = storage;
        this.start = start;
        this.end = end;
        this.cte = cte;
        this.encode = encode;
      }

      public override Stream OpenReadStream(long start, long end)
      {
        this.ThrowIfDisposed();
        start = this.start + start;
        end = end != long.MaxValue ? this.start + end : this.end;
        Encoders.ByteEncoder encoder = this.encode ? MimeDocument.EncodingDataStorage.CreateEncoder(this.cte) : MimePart.CreateDecoder(this.cte);
        if (encoder == null)
          return this.storage.OpenReadStream(start, end);
        return (Stream) new Encoders.EncoderStream(this.storage.OpenReadStream(start, end), encoder, Encoders.EncoderStreamAccess.Read, true);
      }

      protected override void Dispose(bool disposing)
      {
        if (disposing && !this.IsDisposed && this.storage != null)
        {
          this.storage.Release();
          this.storage = (Internal.DataStorage) null;
        }
        base.Dispose(disposing);
      }
    }

    private class EncodingDataStorage : MimeDocument.CodingDataStorage
    {
      public EncodingDataStorage(Internal.DataStorage storage, long start, long end, ContentTransferEncoding cte)
        : base(storage, start, end, cte, true)
      {
      }

      internal static Encoders.ByteEncoder CreateEncoder(ContentTransferEncoding encoding)
      {
        switch (encoding)
        {
          case ContentTransferEncoding.QuotedPrintable:
            return (Encoders.ByteEncoder) new Encoders.QPEncoder();
          case ContentTransferEncoding.Base64:
            return (Encoders.ByteEncoder) new Encoders.Base64Encoder();
          case ContentTransferEncoding.UUEncode:
            return (Encoders.ByteEncoder) new Encoders.UUEncoder();
          default:
            return (Encoders.ByteEncoder) null;
        }
      }
    }

    private class DecodingDataStorage : MimeDocument.CodingDataStorage
    {
      public DecodingDataStorage(Internal.DataStorage storage, long start, long end, ContentTransferEncoding cte)
        : base(storage, start, end, cte, false)
      {
      }
    }
  }
}
