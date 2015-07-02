using System.Linq;

namespace Butler.Schema.Data.Mime {

    public struct DecodingResults {

        public string CharsetName { get; internal set; }
        public string CultureName { get; internal set; }
        public EncodingScheme EncodingScheme { get; internal set; }
        public bool DecodingFailed { get; internal set; }

    }

}