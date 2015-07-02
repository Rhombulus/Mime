using System.Linq;

namespace Butler.Schema.Data.Mime {

    public class HeaderList : MimeNode, System.Collections.Generic.IEnumerable<Header> {

        internal HeaderList(MimeNode parent)
            : base(parent) {}

        private HeaderList() {}

        System.Collections.Generic.IEnumerator<Header> System.Collections.Generic.IEnumerable<Header>.GetEnumerator() {
            return new Enumerator<Header>(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return new Enumerator<Header>(this);
        }

        public static HeaderList ReadFrom(MimeReader reader) {
            if (reader == null)
                throw new System.ArgumentNullException(nameof(reader));
            return reader.ReadHeaderList();
        }

        public Header FindFirst(string name) {
            if (name != null) {
                var headerId = Header.GetHeaderId(name, false);
                if (headerId != HeaderId.Unknown)
                    return this.FindFirst(headerId);
                if (headerMap[(int) headerId] == 0)
                    return null;
                var header = this.FirstChild as Header;
                var count = 0;
                for (; header != null; header = header.NextSibling as Header) {
                    if (header.IsName(name))
                        return header;
                    ++count;
                    this.CheckLoopCount(count);
                }
            }
            return null;
        }

        public Header FindFirst(HeaderId headerId) {
            if (headerId < HeaderId.Unknown || headerId > (HeaderId) MimeData.nameIndex.Length)
                throw new System.ArgumentException(Resources.Strings.InvalidHeaderId, nameof(headerId));
            if (headerMap[(int) headerId] == 0)
                return null;
            var header = this.FirstChild as Header;
            var count = 0;
            for (; header != null; header = header.NextSibling as Header) {
                if (headerId == header.HeaderId)
                    return header;
                ++count;
                this.CheckLoopCount(count);
            }
            headerMap[(int) headerId] = 0;
            return null;
        }

        public Header FindNext(Header refHeader) {
            if (refHeader == null)
                throw new System.ArgumentNullException(nameof(refHeader));
            if (this != refHeader.Parent)
                throw new System.ArgumentException(Resources.Strings.RefHeaderIsNotMyChild);
            var headerId = refHeader.HeaderId;
            if (headerMap[(int) headerId] == 1)
                return null;
            var header = refHeader.NextSibling as Header;
            var count = 0;
            if (headerId == HeaderId.Unknown) {
                var name = refHeader.Name;
                for (; header != null; header = header.NextSibling as Header) {
                    if (header.IsName(name))
                        return header;
                    ++count;
                    this.CheckLoopCount(count);
                }
            } else {
                for (; header != null; header = header.NextSibling as Header) {
                    if (headerId == header.HeaderId)
                        return header;
                    ++count;
                    this.CheckLoopCount(count);
                }
            }
            return null;
        }

        public Header[] FindAll(HeaderId headerId) {
            var first = this.FindFirst(headerId);
            if (first == null)
                return new Header[0];
            int length = headerMap[(int) headerId];
            if (length == byte.MaxValue) {
                var header = first;
                length = 1;
                var count = 0;
                while (true) {
                    do {
                        header = header.NextSibling as Header;
                        if (header != null) {
                            ++count;
                            this.CheckLoopCount(count);
                        } else
                            goto label_7;
                    } while (headerId != header.HeaderId);
                    ++length;
                }
                label_7:
                if (length < byte.MaxValue)
                    headerMap[(int) headerId] = (byte) length;
            }
            var headerArray = new Header[length];
            var header1 = first;
            headerArray[0] = header1;
            var count1 = 0;
            var num = 1;
            while (num < length) {
                header1 = header1.NextSibling as Header;
                ++count1;
                this.CheckLoopCount(count1);
                if (header1 != null) {
                    if (headerId == header1.HeaderId)
                        headerArray[num++] = header1;
                } else
                    break;
            }
            return headerArray;
        }

        public Header[] FindAll(string name) {
            var headerId = Header.GetHeaderId(name, false);
            if (headerId != HeaderId.Unknown)
                return this.FindAll(headerId);
            var first = this.FindFirst(name);
            if (first == null)
                return new Header[0];
            var header1 = first;
            var length = 1;
            var count1 = 0;
            while (true) {
                do {
                    header1 = header1.NextSibling as Header;
                    ++count1;
                    this.CheckLoopCount(count1);
                    if (header1 == null)
                        goto label_8;
                } while (!header1.IsName(name));
                ++length;
            }
            label_8:
            var headerArray = new Header[length];
            var header2 = first;
            headerArray[0] = header2;
            var count2 = 0;
            var num = 1;
            while (num < length) {
                header2 = header2.NextSibling as Header;
                ++count2;
                this.CheckLoopCount(count2);
                if (header2 != null) {
                    if (header2.IsName(name))
                        headerArray[num++] = header2;
                } else
                    break;
            }
            return headerArray;
        }

        public void RemoveAll(HeaderId headerId) {
            var header1 = this.FindFirst(headerId);
            if (header1 == null)
                return;
            if (headerMap[(int) headerId] == 1)
                this.RemoveChild(header1);
            else {
                var count = 0;
                do {
                    var header2 = header1.NextSibling as Header;
                    ++count;
                    this.CheckLoopCount(count);
                    if (header1.HeaderId == headerId)
                        this.RemoveChild(header1);
                    header1 = header2;
                } while (header1 != null);
                headerMap[(int) headerId] = 0;
            }
        }

        public void RemoveAll(string name) {
            if (name == null)
                return;
            var headerId = Header.GetHeaderId(name, false);
            Header next;
            if (headerId != HeaderId.Unknown)
                this.RemoveAll(headerId);
            else {
                for (var refHeader = this.FindFirst(name); refHeader != null; refHeader = next) {
                    next = this.FindNext(refHeader);
                    this.RemoveChild(refHeader);
                }
            }
        }

        public Enumerator<Header> GetEnumerator() {
            return new Enumerator<Header>(this);
        }

        public override sealed MimeNode Clone() {
            var headerList = new HeaderList();
            this.CopyTo(headerList);
            return headerList;
        }

        public override sealed void CopyTo(object destination) {
            if (destination == null)
                throw new System.ArgumentNullException(nameof(destination));
            if (destination == this)
                return;
            if (!(destination is HeaderList))
                throw new System.ArgumentException(Resources.Strings.CantCopyToDifferentObjectType);
            base.CopyTo(destination);
        }

        public void WriteTo(System.IO.Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter) {
            if (stream == null)
                throw new System.ArgumentNullException(nameof(stream));
            if (encodingOptions == null)
                encodingOptions = this.GetDocumentEncodingOptions();
            byte[] scratchBuffer = null;
            var currentLineLength = new MimeStringLength(0);
            this.WriteTo(stream, encodingOptions, filter, ref currentLineLength, ref scratchBuffer);
        }

        internal override long WriteTo(System.IO.Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer) {
            MimePart.CountingWriteStream countingWriteStream1 = null;
            MimePart.CountingWriteStream countingWriteStream2 = null;
            var num1 = 0L;
            var num2 = 0L;
            if (filter != null) {
                countingWriteStream1 = stream as MimePart.CountingWriteStream;
                if (countingWriteStream1 == null) {
                    countingWriteStream2 = new MimePart.CountingWriteStream(stream);
                    countingWriteStream1 = countingWriteStream2;
                    stream = countingWriteStream1;
                }
                num1 = countingWriteStream1.Count;
            }
            for (var mimeNode = this.FirstChild; mimeNode != null; mimeNode = mimeNode.NextSibling) {
                if (filter == null || !filter.FilterHeader(mimeNode as Header, countingWriteStream1))
                    num2 += mimeNode.WriteTo(stream, encodingOptions, filter, ref currentLineLength, ref scratchBuffer);
            }
            if (countingWriteStream1 != null) {
                num2 = countingWriteStream1.Count - num1;
                if (countingWriteStream2 != null)
                    countingWriteStream2.Dispose();
            }
            currentLineLength.SetAs(0);
            return num2;
        }

        internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild) {
            var header = newChild as Header;
            if (header == null)
                throw new System.ArgumentException(Resources.Strings.NewChildNotMimeHeader, nameof(newChild));
            var headerId = header.HeaderId;
            if (Header.IsRestrictedHeader(headerId)) {
                if (headerMap[(int) headerId] != 0) {
                    var first = this.FindFirst(headerId);
                    if (first == refChild)
                        refChild = first.PreviousSibling;
                    this.InternalRemoveChild(first);
                }
                ++headerMap[(int) headerId];
            } else if (headerMap[(int) headerId] != byte.MaxValue)
                ++headerMap[(int) headerId];
            return refChild;
        }

        internal override void ChildRemoved(MimeNode oldChild) {
            var headerId = (oldChild as Header).HeaderId;
            if (headerMap[(int) headerId] == byte.MaxValue)
                return;
            --headerMap[(int) headerId];
        }

        private void CheckLoopCount(int count) {
            if (!loopLimitInitialized) {
                MimeDocument document;
                MimeNode treeRoot;
                this.GetMimeDocumentOrTreeRoot(out document, out treeRoot);
                if (document != null)
                    loopLimit = document.MimeLimits.MaxHeaders;
                loopLimitInitialized = true;
            }
            if (count > loopLimit)
                throw new System.InvalidOperationException(string.Format("Loop detected in headers collection. Loop count: {0}", loopLimit));
        }

        private const string LoopLimitMessage = "Loop detected in headers collection. Loop count: {0}";
        private readonly byte[] headerMap = new byte[MimeData.nameIndex.Length];
        private int loopLimit = MimeLimits.Default.MaxHeaders;
        private bool loopLimitInitialized;

    }

}