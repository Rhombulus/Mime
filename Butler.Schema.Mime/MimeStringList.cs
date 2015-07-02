using System.Linq;

namespace Butler.Schema.Data.Mime {

    internal struct MimeStringList {

        public MimeStringList(byte[] data) {
            first = new MimeString(data, 0, data.Length);
            overflow = null;
        }

        public MimeStringList(byte[] data, int offset, int count) {
            first = new MimeString(data, offset, count);
            overflow = null;
        }

        public MimeStringList(MimeString str) {
            first = str;
            overflow = null;
        }

        public int Count {
            get {
                if (overflow != null)
                    return overflow[0].HeaderCount;
                return first.Data == null ? 0 : 1;
            }
        }

        public int Length {
            get {
                if (overflow == null)
                    return first.Length;
                return overflow[0].HeaderTotalLength;
            }
        }

        public MimeString this[int index] {
            get {
                if (index == 0)
                    return first;
                if (index < 4096)
                    return overflow[index].Str;
                return overflow[4096 + index/4096 - 1].Secondary[index%4096];
            }
            set {
                if (overflow != null)
                    overflow[0].HeaderTotalLength += value.Length - this[index].Length;
                if (index == 0)
                    first = value;
                else if (index < 4096)
                    overflow[index].Str = value;
                else
                    overflow[4096 + index/4096 - 1].Secondary[index%4096] = value;
            }
        }

        public void Append(MimeString value) {
            if (value.Length == 0)
                return;
            var count = this.Count;
            if (count == 0) {
                first = value;
                if (overflow == null)
                    return;
                ++overflow[0].HeaderCount;
                overflow[0].HeaderTotalLength += value.Length;
            } else if (count < 4096) {
                if (overflow == null) {
                    overflow = new ListEntry[8];
                    overflow[0].HeaderCount = 1;
                    overflow[0].HeaderTotalLength = first.Length;
                } else if (count == overflow.Length) {
                    var length = count*2;
                    if (length >= 4096)
                        length = 4128;
                    var listEntryArray = new ListEntry[length];
                    System.Array.Copy(overflow, 0, listEntryArray, 0, overflow.Length);
                    overflow = listEntryArray;
                }
                overflow[count].Str = value;
                ++overflow[0].HeaderCount;
                overflow[0].HeaderTotalLength += value.Length;
            } else {
                var index1 = 4096 + count/4096 - 1;
                var index2 = count%4096;
                if (index1 >= overflow.Length)
                    throw new MimeException("MIME is too complex (header value is too long)");
                if (overflow[index1].Secondary == null)
                    overflow[index1].Secondary = new MimeString[4096];
                overflow[index1].Secondary[index2] = value;
                ++overflow[0].HeaderCount;
                overflow[0].HeaderTotalLength += value.Length;
            }
        }

        public void TakeOver(ref MimeStringList list) {
            this.TakeOver(ref list, 4026531840U);
        }

        public void TakeOver(ref MimeStringList list, uint mask) {
            if ((int) mask == -268435456) {
                first = list.first;
                overflow = list.overflow;
                list.first = new MimeString();
                list.overflow = null;
            } else {
                this.Reset();
                this.TakeOverAppend(ref list, mask);
            }
        }

        public void TakeOverAppend(ref MimeStringList list) {
            this.TakeOverAppend(ref list, 4026531840U);
        }

        public void TakeOverAppend(ref MimeStringList list, uint mask) {
            if (this.Count == 0 && (int) mask == -268435456)
                this.TakeOver(ref list, mask);
            else {
                for (var index = 0; index < list.Count; ++index) {
                    var refLine = list[index];
                    if ((int) mask == -268435456 || ((int) refLine.Mask & (int) mask) != 0)
                        this.AppendFragment(refLine);
                }
                list.Reset();
            }
        }

        public void AppendFragment(MimeString refLine) {
            var count = this.Count;
            if (count != 0) {
                var index = count - 1;
                if (index == 0) {
                    if (first.MergeIfAdjacent(refLine)) {
                        if (overflow == null)
                            return;
                        overflow[0].HeaderTotalLength += refLine.Length;
                        return;
                    }
                } else if (index < 4096) {
                    if (overflow[index].StrMergeIfAdjacent(refLine)) {
                        overflow[0].HeaderTotalLength += refLine.Length;
                        return;
                    }
                } else if (overflow[4096 + index/4096 - 1].Secondary[index%4096].MergeIfAdjacent(refLine)) {
                    overflow[0].HeaderTotalLength += refLine.Length;
                    return;
                }
            }
            this.Append(refLine);
        }

        public int GetLength(uint mask) {
            if ((int) mask == -268435456)
                return this.Length;
            var num = 0;
            for (var index = 0; index < this.Count; ++index) {
                var mimeString = this[index];
                if (((int) mimeString.Mask & (int) mask) != 0)
                    num += mimeString.Length;
            }
            return num;
        }

        public byte[] GetSz() {
            var count = this.Count;
            if (count <= 1)
                return first.GetSz();
            var destination = new byte[this.Length];
            var destinationIndex = 0;
            for (var index = 0; index < count; ++index) {
                destinationIndex += this[index].CopyTo(destination, destinationIndex);
            }
            return destination;
        }

        public byte[] GetSz(uint mask) {
            if ((int) mask == -268435456)
                return this.GetSz();
            var count = this.Count;
            switch (count) {
                case 0:
                    return null;
                case 1:
                    if (((int) first.Mask & (int) mask) == 0)
                        return MimeString.EmptyByteArray;
                    return first.GetSz();
                default:
                    var destination = new byte[this.GetLength(mask)];
                    var destinationIndex = 0;
                    for (var index = 0; index < count; ++index) {
                        var mimeString = this[index];
                        if (((int) mimeString.Mask & (int) mask) != 0)
                            destinationIndex += mimeString.CopyTo(destination, destinationIndex);
                    }
                    return destination;
            }
        }

        public void WriteTo(System.IO.Stream stream) {
            for (var index = 0; index < this.Count; ++index) {
                this[index].WriteTo(stream);
            }
        }

        public override string ToString() {
            var count = this.Count;
            if (count <= 1)
                return first.ToString();
            var stringBuilder = Internal.ScratchPad.GetStringBuilder(this.Length);
            for (var index = 0; index < count; ++index) {
                var mimeString = this[index];
                var str = Internal.ByteString.BytesToString(mimeString.Data, mimeString.Offset, mimeString.Length, true);
                stringBuilder.Append(str);
            }
            Internal.ScratchPad.ReleaseStringBuilder();
            return stringBuilder.ToString();
        }

        public MimeStringList Clone() {
            var mimeStringList = new MimeStringList();
            mimeStringList.first = first;
            if (overflow != null && overflow[0].HeaderCount > 1) {
                mimeStringList.overflow = new ListEntry[overflow.Length];
                var length1 = System.Math.Min(this.Count, 4096);
                System.Array.Copy(overflow, 0, mimeStringList.overflow, 0, length1);
                if (this.Count > 4096) {
                    var index = 4096;
                    var num = 4096;
                    while (num < this.Count) {
                        mimeStringList.overflow[index].Secondary = new MimeString[4096];
                        var length2 = System.Math.Min(this.Count - num, 4096);
                        System.Array.Copy(overflow[index].Secondary, 0, mimeStringList.overflow[index].Secondary, 0, length2);
                        num += 4096;
                        ++index;
                    }
                }
            }
            return mimeStringList;
        }

        public void Reset() {
            first = new MimeString();
            if (overflow == null)
                return;
            overflow[0].Reset();
        }

        public const uint MaskAny = 4026531840U;
        public const uint MaskRawOnly = 268435456U;
        public const uint MaskJis = 536870912U;
        public static readonly MimeStringList Empty = new MimeStringList();
        private MimeString first;
        private ListEntry[] overflow;


        private struct ListEntry {

            public int HeaderCount { get; set; }
            public int HeaderTotalLength { get; set; }

            public MimeString Str {
                get {
                    return new MimeString((byte[]) obj, this.HeaderCount, (uint) this.HeaderTotalLength);
                }
                set {
                    obj = value.Data;
                    this.HeaderCount = value.Offset;
                    this.HeaderTotalLength = (int) value.LengthAndMask;
                }
            }

            public MimeString[] Secondary {
                get {
                    return (MimeString[]) obj;
                }
                set {
                    obj = value;
                    this.HeaderCount = 0;
                    this.HeaderTotalLength = 0;
                }
            }

            public void Reset() {
                obj = null;
                this.HeaderCount = 0;
                this.HeaderTotalLength = 0;
            }

            public bool StrMergeIfAdjacent(MimeString refLine) {
                var str = this.Str;
                if (!str.MergeIfAdjacent(refLine))
                    return false;
                this.Str = str;
                return true;
            }

            private object obj;

        }

    }

}