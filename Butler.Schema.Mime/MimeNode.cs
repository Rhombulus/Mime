using System.Linq;

namespace Butler.Schema.Data.Mime {

    public abstract class MimeNode : System.Collections.Generic.IEnumerable<MimeNode> {

        internal MimeNode(MimeNode parent) {
            this.Parent = parent;
        }

        internal MimeNode() {}
        public bool HasChildren => null != this.FirstChild;
        public MimeNode Parent { get; private set; }

        public MimeNode FirstChild {
            get {
                if (this.InternalLastChild == null)
                    return this.ParseNextChild();
                return this.InternalLastChild.nextSibling;
            }
        }

        public MimeNode LastChild {
            get {
                do
                    ; while (this.ParseNextChild() != null);
                return this.InternalLastChild;
            }
        }

        public MimeNode NextSibling {
            get {
                if (this.Parent == null)
                    return null;
                if (this != this.Parent.InternalLastChild)
                    return nextSibling;
                return this.Parent.ParseNextChild();
            }
        }

        public MimeNode PreviousSibling {
            get {
                if (this.Parent == null || this.Parent.InternalLastChild.nextSibling == this)
                    return null;
                var mimeNode = this.Parent.InternalLastChild.nextSibling;
                var count = 0;
                for (; mimeNode.nextSibling != this; mimeNode = mimeNode.nextSibling) {
                    ++count;
                    this.CheckLoopCount(count);
                }
                return mimeNode;
            }
        }

        internal MimeNode InternalLastChild { get; private set; }

        internal MimeNode InternalNextSibling {
            get {
                if (this.Parent == null || this == this.Parent.InternalLastChild)
                    return null;
                return nextSibling;
            }
        }

        protected bool IsReadOnly {
            get {
                MimeNode treeRoot;
                var parentDocument = MimeNode.GetParentDocument(this, out treeRoot);
                if (parentDocument != null)
                    return parentDocument.IsReadOnly;
                return false;
            }
        }

        System.Collections.Generic.IEnumerator<MimeNode> System.Collections.Generic.IEnumerable<MimeNode>.GetEnumerator() {
            return new Enumerator<MimeNode>(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return new Enumerator<MimeNode>(this);
        }

        public MimeNode InsertBefore(MimeNode newChild, MimeNode refChild) {
            this.ThrowIfReadOnly("MimeNode.InsertBefore");
            if (refChild == null)
                refChild = this.LastChild;
            else if (this.InternalLastChild != null && refChild == this.InternalLastChild.nextSibling)
                refChild = null;
            else {
                refChild = refChild.PreviousSibling;
                if (refChild == null)
                    throw new System.ArgumentException(Resources.Strings.RefChildIsNotMyChild, nameof(refChild));
            }
            return this.InsertAfter(newChild, refChild);
        }

        public MimeNode InsertAfter(MimeNode newChild, MimeNode refChild) {
            this.ThrowIfReadOnly("MimeNode.InsertAfter");
            newChild = this.InternalInsertAfter(newChild, refChild);
            this.SetDirty();
            return newChild;
        }

        public MimeNode AppendChild(MimeNode newChild) {
            this.ThrowIfReadOnly("MimeNode.AppendChild");
            return this.InsertAfter(newChild, this.LastChild);
        }

        public MimeNode PrependChild(MimeNode newChild) {
            this.ThrowIfReadOnly("MimeNode.PrependChild");
            return this.InsertAfter(newChild, null);
        }

        public MimeNode RemoveChild(MimeNode oldChild) {
            this.ThrowIfReadOnly("MimeNode.RemoveChild");
            oldChild = this.InternalRemoveChild(oldChild);
            this.SetDirty();
            return oldChild;
        }

        public MimeNode ReplaceChild(MimeNode newChild, MimeNode oldChild) {
            this.ThrowIfReadOnly("MimeNode.ReplaceChild");
            if (oldChild == null)
                throw new System.ArgumentNullException(nameof(oldChild));
            if (this != oldChild.Parent)
                throw new System.ArgumentException(Resources.Strings.OldChildIsNotMyChild, nameof(oldChild));
            if (newChild == oldChild)
                return oldChild;
            var mimeNode = this.InsertAfter(newChild, oldChild);
            if (this == oldChild.Parent)
                this.RemoveChild(oldChild);
            return mimeNode;
        }

        public void RemoveAll() {
            this.ThrowIfReadOnly("MimeNode.RemoveAll");
            this.InternalRemoveAll();
            this.SetDirty();
        }

        internal virtual void RemoveAllUnparsed() {}

        public void RemoveFromParent() {
            this.ThrowIfReadOnly("MimeNode.RemoveFromParent");
            if (this.Parent == null)
                return;
            this.Parent.RemoveChild(this);
        }

        public long WriteTo(System.IO.Stream stream) {
            return this.WriteTo(stream, null);
        }

        public long WriteTo(System.IO.Stream stream, EncodingOptions encodingOptions) {
            if (stream == null)
                throw new System.ArgumentNullException(nameof(stream));
            if (encodingOptions == null)
                encodingOptions = this.GetDocumentEncodingOptions();
            byte[] scratchBuffer = null;
            var currentLineLength = new MimeStringLength(0);
            return this.WriteTo(stream, encodingOptions, null, ref currentLineLength, ref scratchBuffer);
        }

        public void WriteTo(MimeWriter writer) {
            if (writer == null)
                throw new System.ArgumentNullException(nameof(writer));
            writer.WriteMimeNode(this);
        }

        public Enumerator<MimeNode> GetEnumerator() {
            return new Enumerator<MimeNode>(this);
        }

        public virtual MimeNode Clone() {
            throw new System.NotSupportedException(
                Resources.Strings.ThisNodeDoesNotSupportCloning(
                    this.GetType()
                        .ToString()));
        }

        public virtual void CopyTo(object destination) {
            if (destination == null)
                throw new System.ArgumentNullException(nameof(destination));
            var mimeNode1 = destination as MimeNode;
            if (mimeNode1 == null)
                throw new System.ArgumentException(Resources.Strings.CantCopyToDifferentObjectType);
            mimeNode1.RemoveAll();
            if (this.InternalLastChild != null) {
                for (var mimeNode2 = this.InternalLastChild.nextSibling; mimeNode2 != null; mimeNode2 = mimeNode2.InternalNextSibling) {
                    mimeNode1.InternalAppendChild(mimeNode2.Clone());
                }
            }
            mimeNode1.SetDirty();
        }

        internal MimeNode InternalInsertAfter(MimeNode newChild, MimeNode refChild) {
            this.ThrowIfReadOnly("MimeNode.InternalInsertAfter");
            if (newChild == null)
                throw new System.ArgumentNullException(nameof(newChild));
            if (refChild != null) {
                if (refChild == newChild)
                    return refChild;
                if (this != refChild.Parent)
                    throw new System.ArgumentException(Resources.Strings.RefChildIsNotMyChild, nameof(refChild));
            }
            if (newChild.Parent != null)
                throw new System.ArgumentException(Resources.Strings.NewChildCannotHaveDifferentParent);
            refChild = this.ValidateNewChild(newChild, refChild);
            newChild.Parent = this;
            if (refChild == null) {
                if (this.InternalLastChild == null) {
                    newChild.nextSibling = newChild;
                    this.InternalLastChild = newChild;
                } else {
                    newChild.nextSibling = this.InternalLastChild.nextSibling;
                    this.InternalLastChild.nextSibling = newChild;
                }
            } else {
                newChild.nextSibling = refChild.nextSibling;
                refChild.nextSibling = newChild;
                if (refChild == this.InternalLastChild)
                    this.InternalLastChild = newChild;
            }
            return newChild;
        }

        internal MimeNode InternalAppendChild(MimeNode newChild) {
            this.ThrowIfReadOnly("MimeNode.InternalAppendChild");
            return this.InternalInsertAfter(newChild, this.LastChild);
        }

        internal MimeNode InternalRemoveChild(MimeNode oldChild) {
            this.ThrowIfReadOnly("MimeNode.InternalRemoveChild");
            if (oldChild == null)
                throw new System.ArgumentNullException(nameof(oldChild));
            if (this != oldChild.Parent)
                throw new System.ArgumentException(Resources.Strings.OldChildIsNotMyChild, nameof(oldChild));
            if (oldChild == this.InternalLastChild.nextSibling) {
                if (oldChild == this.InternalLastChild)
                    this.InternalLastChild = null;
                else
                    this.InternalLastChild.nextSibling = oldChild.nextSibling;
            } else {
                var previousSibling = oldChild.PreviousSibling;
                previousSibling.nextSibling = oldChild.nextSibling;
                if (oldChild == this.InternalLastChild)
                    this.InternalLastChild = previousSibling;
            }
            oldChild.Parent = null;
            oldChild.nextSibling = null;
            this.ChildRemoved(oldChild);
            return oldChild;
        }

        internal void InternalRemoveAll() {
            this.ThrowIfReadOnly("MimeNode.InternalRemoveAll");
            while (this.InternalLastChild != null) {
                var oldChild = this.InternalLastChild.nextSibling;
                if (oldChild == this.InternalLastChild)
                    this.InternalLastChild = null;
                else
                    this.InternalLastChild.nextSibling = oldChild.nextSibling;
                oldChild.nextSibling = null;
                oldChild.Parent = null;
                this.ChildRemoved(oldChild);
            }
            this.RemoveAllUnparsed();
        }

        internal void InternalDetachParent() {
            this.Parent = null;
        }

        internal virtual MimeNode ParseNextChild() {
            return null;
        }

        internal virtual void CheckChildrenLimit(int countLimit, int bytesLimit) {}

        internal virtual MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild) {
            return refChild;
        }

        internal abstract long WriteTo(System.IO.Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer);
        internal virtual void ChildRemoved(MimeNode oldChild) {}

        internal virtual void SetDirty() {
            this.ThrowIfReadOnly("MimeNode.SetDirty");
            if (this.Parent == null)
                return;
            this.Parent.SetDirty();
        }

        internal EncodingOptions GetDocumentEncodingOptions() {
            MimeDocument document;
            MimeNode treeRoot;
            this.GetMimeDocumentOrTreeRoot(out document, out treeRoot);
            if (document != null)
                return document.EncodingOptions;
            var mimePart = treeRoot as MimePart;
            return new EncodingOptions(mimePart == null ? null : mimePart.FindMimeTreeCharset());
        }

        internal DecodingOptions GetHeaderDecodingOptions() {
            MimeDocument document;
            MimeNode treeRoot;
            this.GetMimeDocumentOrTreeRoot(out document, out treeRoot);
            DecodingOptions decodingOptions;
            if (document != null) {
                decodingOptions = document.EffectiveHeaderDecodingOptions;
                if (decodingOptions.Charset == null)
                    decodingOptions.Charset = DecodingOptions.DefaultCharset;
            } else {
                decodingOptions = DecodingOptions.Default;
                decodingOptions.Charset = treeRoot.GetDefaultHeaderDecodingCharset(null, treeRoot);
            }
            return decodingOptions;
        }

        internal Globalization.Charset GetDefaultHeaderDecodingCharset(MimeDocument document, MimeNode treeRoot) {
            if (treeRoot == null)
                this.GetMimeDocumentOrTreeRoot(out document, out treeRoot);
            Globalization.Charset charset;
            if (document != null)
                charset = document.EffectiveHeaderDecodingOptions.Charset;
            else {
                var mimePart = treeRoot as MimePart;
                charset = mimePart == null ? null : mimePart.FindMimeTreeCharset();
            }
            if (charset == null)
                charset = DecodingOptions.DefaultCharset;
            return charset;
        }

        internal MimeNode GetTreeRoot() {
            var mimeNode = this;
            while (mimeNode.Parent != null)
                mimeNode = mimeNode.Parent;
            return mimeNode;
        }

        internal void GetMimeDocumentOrTreeRoot(out MimeDocument document, out MimeNode treeRoot) {
            document = MimeNode.GetParentDocument(this, out treeRoot);
        }

        protected static MimeDocument GetParentDocument(MimeNode node, out MimeNode treeRoot) {
            treeRoot = node.GetTreeRoot();
            var mimePart = treeRoot as MimePart;
            if (mimePart != null)
                return mimePart.ParentDocument;
            return null;
        }

        protected void ThrowIfReadOnly(string method) {
            if (this.IsReadOnly)
                throw new ReadOnlyMimeException(method);
        }

        private void CheckLoopCount(int count) {
            if (!loopLimitInitialized) {
                var mimeLimits = MimeLimits.Default;
                MimeDocument document;
                MimeNode treeRoot;
                this.GetMimeDocumentOrTreeRoot(out document, out treeRoot);
                if (document != null)
                    mimeLimits = document.MimeLimits;
                loopLimit = !(this is Header) ? (!(this is AddressItem) ? (!(this is MimeParameter) ? mimeLimits.MaxParts : mimeLimits.MaxParametersPerHeader) : mimeLimits.MaxAddressItemsPerHeader) : mimeLimits.MaxHeaders;
                loopLimitInitialized = true;
            }
            if (count > loopLimit)
                throw new System.InvalidOperationException(string.Format("Loop detected in sibling collection. Loop count: {0}", loopLimit));
        }

        private const string LoopLimitMessage = "Loop detected in sibling collection. Loop count: {0}";
        private int loopLimit;
        private bool loopLimitInitialized;
        private MimeNode nextSibling;


        public struct Enumerator<T> : System.Collections.Generic.IEnumerator<T>, System.IDisposable, System.Collections.IEnumerator where T : MimeNode {

            internal Enumerator(MimeNode node) {
                this.node = node;
                current = default(T);
                next = this.node.FirstChild as T;
            }

            object System.Collections.IEnumerator.Current {
                get {
                    if (current == null)
                        throw new System.InvalidOperationException((object) next == null ? Resources.Strings.ErrorAfterLast : Resources.Strings.ErrorBeforeFirst);
                    return current;
                }
            }

            public T Current {
                get {
                    if (current == null)
                        throw new System.InvalidOperationException((object) next == null ? Resources.Strings.ErrorAfterLast : Resources.Strings.ErrorBeforeFirst);
                    return current;
                }
            }

            public bool MoveNext() {
                current = next;
                if (current == null)
                    return false;
                next = current.NextSibling as T;
                return true;
            }

            public void Reset() {
                current = default(T);
                next = node.FirstChild as T;
            }

            public void Dispose() {
                this.Reset();
            }

            private readonly MimeNode node;
            private T current;
            private T next;

        }

    }

}