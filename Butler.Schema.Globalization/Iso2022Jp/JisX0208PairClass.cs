using System;
using System.Linq;

namespace Butler.Schema.Data.Globalization.Iso2022Jp {

    [Flags]
    internal enum JisX0208PairClass {

        Unrecognized = 1,
        IbmExtension = 2,
        Cp932 = 4

    }

}