// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.HeaderList
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  public class HeaderList : MimeNode, IEnumerable<Header>
  {
    private int loopLimit = MimeLimits.Default.MaxHeaders;
    private byte[] headerMap = new byte[MimeData.nameIndex.Length];
    private const string LoopLimitMessage = "Loop detected in headers collection. Loop count: {0}";
    private bool loopLimitInitialized;

    internal HeaderList(MimeNode parent)
      : base(parent)
    {
    }

    private HeaderList()
    {
    }

    public static HeaderList ReadFrom(MimeReader reader)
    {
      if (reader == null)
        throw new ArgumentNullException(nameof(reader));
      return reader.ReadHeaderList();
    }

    public Header FindFirst(string name)
    {
      if (name != null)
      {
        HeaderId headerId = Header.GetHeaderId(name, false);
        if (headerId != HeaderId.Unknown)
          return this.FindFirst(headerId);
        if ((int) this.headerMap[(int) headerId] == 0)
          return (Header) null;
        Header header = this.FirstChild as Header;
        int count = 0;
        for (; header != null; header = header.NextSibling as Header)
        {
          if (header.IsName(name))
            return header;
          ++count;
          this.CheckLoopCount(count);
        }
      }
      return (Header) null;
    }

    public Header FindFirst(HeaderId headerId)
    {
      if (headerId < HeaderId.Unknown || headerId > (HeaderId) MimeData.nameIndex.Length)
        throw new ArgumentException(CtsResources.Strings.InvalidHeaderId, nameof(headerId));
      if ((int) this.headerMap[(int) headerId] == 0)
        return (Header) null;
      Header header = this.FirstChild as Header;
      int count = 0;
      for (; header != null; header = header.NextSibling as Header)
      {
        if (headerId == header.HeaderId)
          return header;
        ++count;
        this.CheckLoopCount(count);
      }
      this.headerMap[(int) headerId] = (byte) 0;
      return (Header) null;
    }

    public Header FindNext(Header refHeader)
    {
      if (refHeader == null)
        throw new ArgumentNullException(nameof(refHeader));
      if (this != refHeader.Parent)
        throw new ArgumentException(CtsResources.Strings.RefHeaderIsNotMyChild);
      HeaderId headerId = refHeader.HeaderId;
      if ((int) this.headerMap[(int) headerId] == 1)
        return (Header) null;
      Header header = refHeader.NextSibling as Header;
      int count = 0;
      if (headerId == HeaderId.Unknown)
      {
        string name = refHeader.Name;
        for (; header != null; header = header.NextSibling as Header)
        {
          if (header.IsName(name))
            return header;
          ++count;
          this.CheckLoopCount(count);
        }
      }
      else
      {
        for (; header != null; header = header.NextSibling as Header)
        {
          if (headerId == header.HeaderId)
            return header;
          ++count;
          this.CheckLoopCount(count);
        }
      }
      return (Header) null;
    }

    public Header[] FindAll(HeaderId headerId)
    {
      Header first = this.FindFirst(headerId);
      if (first == null)
        return new Header[0];
      int length = (int) this.headerMap[(int) headerId];
      if (length == (int) byte.MaxValue)
      {
        Header header = first;
        length = 1;
        int count = 0;
        while (true)
        {
          do
          {
            header = header.NextSibling as Header;
            if (header != null)
            {
              ++count;
              this.CheckLoopCount(count);
            }
            else
              goto label_7;
          }
          while (headerId != header.HeaderId);
          ++length;
        }
label_7:
        if (length < (int) byte.MaxValue)
          this.headerMap[(int) headerId] = (byte) length;
      }
      Header[] headerArray = new Header[length];
      Header header1 = first;
      headerArray[0] = header1;
      int count1 = 0;
      int num = 1;
      while (num < length)
      {
        header1 = header1.NextSibling as Header;
        ++count1;
        this.CheckLoopCount(count1);
        if (header1 != null)
        {
          if (headerId == header1.HeaderId)
            headerArray[num++] = header1;
        }
        else
          break;
      }
      return headerArray;
    }

    public Header[] FindAll(string name)
    {
      HeaderId headerId = Header.GetHeaderId(name, false);
      if (headerId != HeaderId.Unknown)
        return this.FindAll(headerId);
      Header first = this.FindFirst(name);
      if (first == null)
        return new Header[0];
      Header header1 = first;
      int length = 1;
      int count1 = 0;
      while (true)
      {
        do
        {
          header1 = header1.NextSibling as Header;
          ++count1;
          this.CheckLoopCount(count1);
          if (header1 == null)
            goto label_8;
        }
        while (!header1.IsName(name));
        ++length;
      }
label_8:
      Header[] headerArray = new Header[length];
      Header header2 = first;
      headerArray[0] = header2;
      int count2 = 0;
      int num = 1;
      while (num < length)
      {
        header2 = header2.NextSibling as Header;
        ++count2;
        this.CheckLoopCount(count2);
        if (header2 != null)
        {
          if (header2.IsName(name))
            headerArray[num++] = header2;
        }
        else
          break;
      }
      return headerArray;
    }

    public void RemoveAll(HeaderId headerId)
    {
      Header header1 = this.FindFirst(headerId);
      if (header1 == null)
        return;
      if ((int) this.headerMap[(int) headerId] == 1)
      {
        this.RemoveChild((MimeNode) header1);
      }
      else
      {
        int count = 0;
        do
        {
          Header header2 = header1.NextSibling as Header;
          ++count;
          this.CheckLoopCount(count);
          if (header1.HeaderId == headerId)
            this.RemoveChild((MimeNode) header1);
          header1 = header2;
        }
        while (header1 != null);
        this.headerMap[(int) headerId] = (byte) 0;
      }
    }

    public void RemoveAll(string name)
    {
      if (name == null)
        return;
      HeaderId headerId = Header.GetHeaderId(name, false);
      Header next;
      if (headerId != HeaderId.Unknown)
      {
        this.RemoveAll(headerId);
      }
      else
      {
        for (Header refHeader = this.FindFirst(name); refHeader != null; refHeader = next)
        {
          next = this.FindNext(refHeader);
          this.RemoveChild((MimeNode) refHeader);
        }
      }
    }

    public MimeNode.Enumerator<Header> GetEnumerator()
    {
      return new MimeNode.Enumerator<Header>((MimeNode) this);
    }

    IEnumerator<Header> IEnumerable<Header>.GetEnumerator()
    {
      return (IEnumerator<Header>) new MimeNode.Enumerator<Header>((MimeNode) this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) new MimeNode.Enumerator<Header>((MimeNode) this);
    }

    public override sealed MimeNode Clone()
    {
      HeaderList headerList = new HeaderList();
      this.CopyTo((object) headerList);
      return (MimeNode) headerList;
    }

    public override sealed void CopyTo(object destination)
    {
      if (destination == null)
        throw new ArgumentNullException(nameof(destination));
      if (destination == this)
        return;
      if (!(destination is HeaderList))
        throw new ArgumentException(CtsResources.Strings.CantCopyToDifferentObjectType);
      base.CopyTo(destination);
    }

    public void WriteTo(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof(stream));
      if (encodingOptions == null)
        encodingOptions = this.GetDocumentEncodingOptions();
      byte[] scratchBuffer = (byte[]) null;
      MimeStringLength currentLineLength = new MimeStringLength(0);
      this.WriteTo(stream, encodingOptions, filter, ref currentLineLength, ref scratchBuffer);
    }

    internal override long WriteTo(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer)
    {
      MimePart.CountingWriteStream countingWriteStream1 = (MimePart.CountingWriteStream) null;
      MimePart.CountingWriteStream countingWriteStream2 = (MimePart.CountingWriteStream) null;
      long num1 = 0L;
      long num2 = 0L;
      if (filter != null)
      {
        countingWriteStream1 = stream as MimePart.CountingWriteStream;
        if (countingWriteStream1 == null)
        {
          countingWriteStream2 = new MimePart.CountingWriteStream(stream);
          countingWriteStream1 = countingWriteStream2;
          stream = (Stream) countingWriteStream1;
        }
        num1 = countingWriteStream1.Count;
      }
      for (MimeNode mimeNode = this.FirstChild; mimeNode != null; mimeNode = mimeNode.NextSibling)
      {
        if (filter == null || !filter.FilterHeader(mimeNode as Header, (Stream) countingWriteStream1))
          num2 += mimeNode.WriteTo(stream, encodingOptions, filter, ref currentLineLength, ref scratchBuffer);
      }
      if (countingWriteStream1 != null)
      {
        num2 = countingWriteStream1.Count - num1;
        if (countingWriteStream2 != null)
          countingWriteStream2.Dispose();
      }
      currentLineLength.SetAs(0);
      return num2;
    }

    internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild)
    {
      Header header = newChild as Header;
      if (header == null)
        throw new ArgumentException(CtsResources.Strings.NewChildNotMimeHeader, nameof(newChild));
      HeaderId headerId = header.HeaderId;
      if (Header.IsRestrictedHeader(headerId))
      {
        if ((int) this.headerMap[(int) headerId] != 0)
        {
          Header first = this.FindFirst(headerId);
          if (first == refChild)
            refChild = first.PreviousSibling;
          this.InternalRemoveChild((MimeNode) first);
        }
        ++this.headerMap[(int) headerId];
      }
      else if ((int) this.headerMap[(int) headerId] != (int) byte.MaxValue)
        ++this.headerMap[(int) headerId];
      return refChild;
    }

    internal override void ChildRemoved(MimeNode oldChild)
    {
      HeaderId headerId = (oldChild as Header).HeaderId;
      if ((int) this.headerMap[(int) headerId] == (int) byte.MaxValue)
        return;
      --this.headerMap[(int) headerId];
    }

    private void CheckLoopCount(int count)
    {
      if (!this.loopLimitInitialized)
      {
        MimeDocument document;
        MimeNode treeRoot;
        this.GetMimeDocumentOrTreeRoot(out document, out treeRoot);
        if (document != null)
          this.loopLimit = document.MimeLimits.MaxHeaders;
        this.loopLimitInitialized = true;
      }
      if (count > this.loopLimit)
        throw new InvalidOperationException(string.Format("Loop detected in headers collection. Loop count: {0}", (object) this.loopLimit));
    }
  }
}
