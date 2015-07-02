using System.Linq;

namespace Butler.Schema.Mime {

    public partial class MimePart {

        public struct SubtreeEnumerator : System.Collections.Generic.IEnumerator<MimePart> {

            internal SubtreeEnumerator(MimePart part, SubtreeEnumerationOptions options, bool includeUnparsed) {
                _options = options;
                _includeUnparsed = includeUnparsed;
                _root = part;
                this.Current = null;
                _currentDisposition = 0;
                _nextChild = part;
                this.Depth = -1;
            }

            public bool FirstVisit => 0 != (_currentDisposition & EnumeratorDisposition.Begin);
            public bool LastVisit => 0 != (_currentDisposition & EnumeratorDisposition.End);
            public int Depth { get; private set; }
            public MimePart Current { get; private set; }
            object System.Collections.IEnumerator.Current => this.Current;

            public bool MoveNext() {
                using (ThreadAccessGuard.EnterPublic(_root.AccessToken)) {
                    if (_nextChild != null) {
                        ++this.Depth;
                        this.Current = _nextChild;
                        if (_options.HasFlag(SubtreeEnumerationOptions.IncludeEmbeddedMessages)) {
                            if (_includeUnparsed)
                                _nextChild = (MimePart) this.Current.FirstChild;
                            else {
                                if (this.Current.InternalLastChild != null)
                                    _nextChild = (MimePart) this.Current.FirstChild;
                                else
                                    _nextChild = null;
                            }
                        } else {
                            if (this.Current.IsMultipart) {
                                if (_includeUnparsed)
                                    _nextChild = (MimePart) this.Current.FirstChild;
                                else {
                                    if (this.Current.InternalLastChild != null)
                                        _nextChild = (MimePart) this.Current.FirstChild;
                                    else
                                        _nextChild = null;
                                }
                            } else
                                _nextChild = null;
                        }
                        _currentDisposition = EnumeratorDisposition.Begin | (_nextChild == null ? EnumeratorDisposition.End : 0);
                        return true;
                    }
                    if (this.Depth < 0)
                        return false;
                    do {
                        --this.Depth;
                        if (this.Depth < 0) {
                            this.Current = null;
                            _nextChild = null;
                            _currentDisposition = 0;
                            return false;
                        }
                        if (_includeUnparsed)
                            _nextChild = (MimePart) this.Current.NextSibling;
                        else
                            _nextChild = (MimePart) this.Current.InternalNextSibling;
                        this.Current = (MimePart) this.Current.Parent;
                        _currentDisposition = _nextChild == null ? EnumeratorDisposition.End : 0;
                    } while ( !_options.HasFlag(SubtreeEnumerationOptions.RevisitParent) && _nextChild == null);
                    return _options.HasFlag(SubtreeEnumerationOptions.RevisitParent) || this.MoveNext();
                }
            }

            void System.Collections.IEnumerator.Reset() {
                this.Current = null;
                _currentDisposition = 0;
                _nextChild = _root;
                this.Depth = -1;
            }

            public void Dispose() {
                ((System.Collections.IEnumerator) this).Reset();
                System.GC.SuppressFinalize(this);
            }

            public void SkipChildren() {
                if (_nextChild == null)
                    return;
                _nextChild = null;
                _currentDisposition |= EnumeratorDisposition.End;
            }

            private readonly bool _includeUnparsed;
            private readonly SubtreeEnumerationOptions _options;
            private readonly MimePart _root;
            private EnumeratorDisposition _currentDisposition;
            private MimePart _nextChild;


            [System.Flags]
            private enum EnumeratorDisposition : byte {

                None = 0,
                Begin = 1,
                End = 2

            }

        }

    }

}