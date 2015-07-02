using System;
using System.Linq;

namespace Butler.Schema.Data.Mime {

    internal struct MimeStringLength {

        public MimeStringLength(int value) {
            this.InChars = value;
            this.InBytes = value;
        }

        public MimeStringLength(int valueInChars, int valueInBytes) {
            this.InChars = valueInChars;
            this.InBytes = valueInBytes;
        }

        public int InChars { get; private set; }
        public int InBytes { get; private set; }

        public void IncrementBy(int count) {
            this.InChars += count;
            this.InBytes += count;
        }

        public void IncrementBy(int countInChars, int countInBytes) {
            this.InChars += countInChars;
            this.InBytes += countInBytes;
        }

        public void IncrementBy(MimeStringLength count) {
            this.InChars += count.InChars;
            this.InBytes += count.InBytes;
        }

        public void DecrementBy(int count) {
            this.InChars -= count;
            this.InBytes -= count;
        }

        public void DecrementBy(int countInChars, int countInBytes) {
            this.InChars -= countInChars;
            this.InBytes -= countInBytes;
        }

        public void DecrementBy(MimeStringLength count) {
            this.InChars -= count.InChars;
            this.InBytes -= count.InBytes;
        }

        public void SetAs(int value) {
            this.InChars = value;
            this.InBytes = value;
        }

        public void SetAs(int valueInChars, int valueInBytes) {
            this.InChars = valueInChars;
            this.InBytes = valueInBytes;
        }

        public void SetAs(MimeStringLength value) {
            this.InChars = value.InChars;
            this.InBytes = value.InBytes;
        }

        public override string ToString() {
            return string.Format("InChars={0}, InBytes={1}", this.InChars, this.InBytes);
        }

    }

}