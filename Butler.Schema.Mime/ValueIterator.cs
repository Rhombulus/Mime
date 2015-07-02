using System;
using System.Linq;

namespace Butler.Schema.Data.Mime {

    internal struct ValueIterator {

        public ValueIterator(MimeStringList lines, uint linesMask) {
            this.Lines = lines;
            this.LinesMask = linesMask;
            lineStart = lineEnd = currentLine = this.Offset = 0;
            this.Bytes = null;
            endLine = this.Lines.Count;
            endOffset = 0;
            for (; currentLine != endLine; ++currentLine) {
                var mimeString = this.Lines[currentLine];
                if (((int) mimeString.Mask & (int) this.LinesMask) != 0) {
                    int count;
                    this.Bytes = mimeString.GetData(out lineStart, out count);
                    lineEnd = lineStart + count;
                    this.Offset = lineStart;
                    break;
                }
            }
        }

        public ValueIterator(MimeStringList lines, uint linesMask, ValuePosition startPosition, ValuePosition endPosition) {
            this.Lines = lines;
            this.LinesMask = linesMask;
            currentLine = startPosition.Line;
            this.Offset = startPosition.Offset;
            endLine = endPosition.Line;
            endOffset = endPosition.Offset;
            if (startPosition != endPosition) {
                int count;
                this.Bytes = this.Lines[currentLine].GetData(out lineStart, out count);
                lineEnd = currentLine == endLine ? endOffset : lineStart + count;
            } else {
                lineStart = lineEnd = this.Offset;
                this.Bytes = null;
            }
        }

        public ValuePosition CurrentPosition => new ValuePosition(currentLine, this.Offset);
        public byte[] Bytes { get; private set; }
        public int Offset { get; private set; }
        public int Length => lineEnd - this.Offset;
        public int TotalLength => this.Lines.Length;
        public MimeStringList Lines { get; }
        public uint LinesMask { get; }

        public bool Eof {
            get {
                if (currentLine == endLine)
                    return this.Offset == lineEnd;
                return false;
            }
        }

        public void Get(int length) {
            this.Offset += length;
            if (this.Offset != lineEnd)
                return;
            this.NextLine();
        }

        public int Get() {
            if (this.Eof)
                return -1;
            var num = this.Bytes[this.Offset++];
            if (this.Offset == lineEnd)
                this.NextLine();
            return num;
        }

        public int Pick() {
            if (this.Eof)
                return -1;
            return this.Bytes[this.Offset];
        }

        public void Unget() {
            if (this.Offset == lineStart) {
                MimeString mimeString;
                do {
                    mimeString = this.Lines[--currentLine];
                } while (((int) mimeString.Mask & (int) this.LinesMask) == 0);
                int count;
                this.Bytes = mimeString.GetData(out lineStart, out count);
                this.Offset = lineEnd = lineStart + count;
            }
            --this.Offset;
        }

        public void SkipToEof() {
            while (!this.Eof)
                this.Get(this.Length);
        }

        private void NextLine() {
            if (this.Eof)
                return;
            MimeString mimeString;
            do {
                ++currentLine;
                if (currentLine == this.Lines.Count) {
                    lineEnd = lineStart = this.Offset = 0;
                    return;
                }
                mimeString = this.Lines[currentLine];
            } while (((int) mimeString.Mask & (int) this.LinesMask) == 0);
            int count;
            this.Bytes = mimeString.GetData(out lineStart, out count);
            this.Offset = lineStart;
            lineEnd = currentLine == endLine ? endOffset : lineStart + count;
        }

        private readonly int endLine;
        private readonly int endOffset;
        private int currentLine;
        private int lineEnd;
        private int lineStart;

    }

}