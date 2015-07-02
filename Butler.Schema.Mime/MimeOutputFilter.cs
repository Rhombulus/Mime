using System.Linq;

namespace Butler.Schema.Mime {

    public abstract class MimeOutputFilter {

        public virtual bool FilterPart(MimePart part, System.IO.Stream stream) {
            return false;
        }

        public virtual bool FilterHeaderList(HeaderList headerList, System.IO.Stream stream) {
            return false;
        }

        public virtual bool FilterHeader(Header header, System.IO.Stream stream) {
            return false;
        }

        public virtual bool FilterPartBody(MimePart part, System.IO.Stream stream) {
            return false;
        }

        public virtual void ClosePart(MimePart part, System.IO.Stream stream) {}

    }

}