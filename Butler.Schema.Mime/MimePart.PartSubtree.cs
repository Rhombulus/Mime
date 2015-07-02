using System.Linq;

namespace Butler.Schema.Data.Mime {

    public partial class MimePart {

        public struct PartSubtree : System.Collections.Generic.IEnumerable<MimePart> {

            internal PartSubtree(MimePart part) {
                _part = part;
            }

            System.Collections.Generic.IEnumerator<MimePart> System.Collections.Generic.IEnumerable<MimePart>.GetEnumerator() {
                return new SubtreeEnumerator(_part, SubtreeEnumerationOptions.None, true);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
                return new SubtreeEnumerator(_part, SubtreeEnumerationOptions.None, true);
            }

            public SubtreeEnumerator GetEnumerator() {
                return new SubtreeEnumerator(_part, SubtreeEnumerationOptions.None, true);
            }

            public SubtreeEnumerator GetEnumerator(SubtreeEnumerationOptions options) {
                return new SubtreeEnumerator(_part, options, true);
            }

            internal SubtreeEnumerator GetEnumerator(SubtreeEnumerationOptions options, bool includeUnparsed) {
                return new SubtreeEnumerator(_part, options, includeUnparsed);
            }

            private readonly MimePart _part;

        }

    }

}