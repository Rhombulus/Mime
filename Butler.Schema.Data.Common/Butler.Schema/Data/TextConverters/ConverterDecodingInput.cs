using System;
using System.Linq;

namespace Butler.Schema.Data.TextConverters {

    internal class ConverterDecodingInput : ConverterInput, IReusable {

        public ConverterDecodingInput(
            System.IO.Stream source, bool push, System.Text.Encoding encoding, bool detectEncodingFromByteOrderMark, int maxParseToken, int restartMax, int inputBufferSize, bool testBoundaryConditions, IResultsFeedback resultFeedback,
            IProgressMonitor progressMonitor)
            : base(progressMonitor) {
            this.resultFeedback = resultFeedback;
            this.restartMax = restartMax;
            if (push)
                pushSource = source as ConverterStream;
            else
                pullSource = source;
            this.detectEncodingFromByteOrderMark = detectEncodingFromByteOrderMark;
            minDecodeBytes = testBoundaryConditions ? 1 : 64;
            originalEncoding = encoding;
            this.SetNewEncoding(encoding);
            maxTokenSize = maxParseToken == int.MaxValue ? maxParseToken : (testBoundaryConditions ? maxParseToken : (maxParseToken + 1023)/1024*1024);
            parseBuffer = new char[testBoundaryConditions ? (55) : checked (Math.Min(4096L, unchecked (maxTokenSize + (long) (minDecodeChars + 1))))];
            readBuffer = pushSource != null ? new byte[Math.Max(minDecodeBytes*2, 8)] : new byte[Math.Max(this.CalculateMaxBytes(parseBuffer.Length), inputBufferSize)];
        }

        public System.Text.Encoding Encoding { get; private set; }

        public bool EncodingChanged {
            get {
                return encodingChanged;
            }
            set {
                encodingChanged = false;
            }
        }

        void IReusable.Initialize(object newSourceOrDestination) {
            if (pullSource != null && newSourceOrDestination != null) {
                var stream = newSourceOrDestination as System.IO.Stream;
                if (stream == null || !stream.CanRead)
                    throw new InvalidOperationException("cannot reinitialize this converter - new input should be a readable Stream object");
                pullSource = stream;
            }
            this.Reinitialize();
        }

        private void Reinitialize() {
            parseStart = 0;
            parseEnd = 0;
            rawEndOfFile = false;
            this.SetNewEncoding(originalEncoding);
            encodingChanged = false;
            readFileOffset = 0;
            readCurrent = 0;
            readEnd = 0;
            pushChunkBuffer = null;
            pushChunkStart = 0;
            pushChunkCount = 0;
            pushChunkUsed = 0;
            restartCache?.Reset();
            restarting = false;
            endOfFile = false;
        }

        public override void SetRestartConsumer(IRestartable restartConsumer) {
            if (restartMax == 0 && restartConsumer != null)
                return;
            this.restartConsumer = restartConsumer;
        }

        public override bool ReadMore(ref char[] buffer, ref int start, ref int current, ref int end) {
            if (parseBuffer.Length - parseEnd <= minDecodeChars && !this.EnsureFreeSpace())
                return true;
            var num1 = 0;
            while ((!rawEndOfFile || readEnd - readCurrent != 0 || restarting) && parseBuffer.Length - parseEnd > minDecodeChars) {
                if (readEnd - readCurrent >= (readFileOffset == 0 ? Math.Max(4, minDecodeBytes) : minDecodeBytes) || rawEndOfFile && !restarting)
                    num1 += this.DecodeFromBuffer(readBuffer, ref readCurrent, readEnd, readFileOffset + readCurrent, rawEndOfFile);
                else if (restarting) {
                    byte[] restartChunk;
                    int restartStart;
                    int restartEnd;
                    if (!this.GetRestartChunk(out restartChunk, out restartStart, out restartEnd))
                        restarting = false;
                    else {
                        var num2 = restartStart;
                        num1 += this.DecodeFromBuffer(restartChunk, ref restartStart, restartEnd, readFileOffset, false);
                        readFileOffset += restartStart - num2;
                        this.ReportRestartChunkUsed(restartStart - num2);
                    }
                } else if (pushSource != null) {
                    if (pushChunkCount == 0) {
                        if (!pushSource.GetInputChunk(out pushChunkBuffer, out pushChunkStart, out pushChunkCount, out rawEndOfFile))
                            break;
                    } else if (pushChunkCount - pushChunkUsed == 0) {
                        if (restartConsumer != null)
                            this.BackupForRestart(pushChunkBuffer, pushChunkStart, pushChunkCount, readFileOffset, false);
                        pushSource.ReportRead(pushChunkCount);
                        readFileOffset += pushChunkCount;
                        pushChunkCount = 0;
                        pushChunkUsed = 0;
                        break;
                    }
                    if (pushChunkCount - pushChunkUsed < (readFileOffset == 0 ? Math.Max(4, minDecodeBytes) : minDecodeBytes)) {
                        if (pushChunkCount - pushChunkUsed != 0) {
                            if (readBuffer.Length - readEnd < pushChunkCount - pushChunkUsed) {
                                if (restartConsumer != null)
                                    this.BackupForRestart(readBuffer, 0, readCurrent, readFileOffset, false);
                                Buffer.BlockCopy(readBuffer, readCurrent, readBuffer, 0, readEnd - readCurrent);
                                readFileOffset += readCurrent;
                                readEnd = readEnd - readCurrent;
                                readCurrent = 0;
                            }
                            if (pushChunkUsed != 0) {
                                if (restartConsumer != null)
                                    this.BackupForRestart(pushChunkBuffer, pushChunkStart, pushChunkUsed, readFileOffset + readEnd, false);
                                readFileOffset += pushChunkUsed;
                            }
                            Buffer.BlockCopy(pushChunkBuffer, pushChunkStart + pushChunkUsed, readBuffer, readEnd, pushChunkCount - pushChunkUsed);
                            readEnd += pushChunkCount - pushChunkUsed;
                            pushSource.ReportRead(pushChunkCount);
                            pushChunkCount = 0;
                            pushChunkUsed = 0;
                            if (readEnd - readCurrent < (readFileOffset == 0 ? Math.Max(4, minDecodeBytes) : minDecodeBytes))
                                break;
                        }
                        num1 += this.DecodeFromBuffer(readBuffer, ref readCurrent, readEnd, readFileOffset + readCurrent, rawEndOfFile);
                    } else if (readEnd - readCurrent != 0) {
                        if (readFileOffset == 0 && readCurrent == 0) {
                            var num2 = Math.Max(4, minDecodeBytes) - (readEnd - readCurrent);
                            Buffer.BlockCopy(pushChunkBuffer, pushChunkStart + pushChunkUsed, readBuffer, readEnd, num2);
                            readEnd += num2;
                            pushSource.ReportRead(num2);
                            pushChunkCount -= num2;
                            pushChunkStart += num2;
                        }
                        num1 += this.DecodeFromBuffer(readBuffer, ref readCurrent, readEnd, readFileOffset + readCurrent, false);
                    }
                    if (parseBuffer.Length - parseEnd > minDecodeChars && pushChunkCount - pushChunkUsed != 0 && readEnd - readCurrent == 0) {
                        if (readEnd != 0) {
                            if (restartConsumer != null)
                                this.BackupForRestart(readBuffer, 0, readCurrent, readFileOffset, false);
                            readFileOffset += readCurrent;
                            readEnd = 0;
                            readCurrent = 0;
                        }
                        var start1 = pushChunkStart + pushChunkUsed;
                        num1 += this.DecodeFromBuffer(pushChunkBuffer, ref start1, pushChunkStart + pushChunkCount, readFileOffset + pushChunkUsed, false);
                        pushChunkUsed = start1 - pushChunkStart;
                    }
                } else {
                    if (readBuffer.Length - readEnd < minDecodeBytes) {
                        if (restartConsumer != null)
                            this.BackupForRestart(readBuffer, 0, readCurrent, readFileOffset, false);
                        Buffer.BlockCopy(readBuffer, readCurrent, readBuffer, 0, readEnd - readCurrent);
                        readFileOffset += readCurrent;
                        readEnd = readEnd - readCurrent;
                        readCurrent = 0;
                    }
                    var num2 = pullSource.Read(readBuffer, readEnd, readBuffer.Length - readEnd);
                    if (num2 == 0)
                        rawEndOfFile = true;
                    else {
                        readEnd += num2;
                        progressMonitor?.ReportProgress();
                    }
                    num1 += this.DecodeFromBuffer(readBuffer, ref readCurrent, readEnd, readFileOffset + readCurrent, rawEndOfFile);
                }
            }
            if (rawEndOfFile && readEnd - readCurrent == 0)
                endOfFile = true;
            if (buffer != parseBuffer)
                buffer = parseBuffer;
            if (start != parseStart) {
                current = parseStart + (current - start);
                start = parseStart;
            }
            end = parseEnd;
            if (num1 == 0 && !endOfFile)
                return encodingChanged;
            return true;
        }

        public override void ReportProcessed(int processedSize) {
            parseStart += processedSize;
        }

        public override int RemoveGap(int gapBegin, int gapEnd) {
            parseEnd = gapBegin;
            parseBuffer[gapBegin] = char.MinValue;
            return gapBegin;
        }

        public bool RestartWithNewEncoding(System.Text.Encoding newEncoding) {
            if (this.Encoding == newEncoding) {
                if (restartConsumer != null) {
                    restartConsumer.DisableRestart();
                    restartConsumer = null;
                    if (restartCache != null) {
                        restartCache.Reset();
                        restartCache = null;
                    }
                }
                return false;
            }
            if (restartConsumer == null || !restartConsumer.CanRestart())
                return false;
            restartConsumer.Restart();
            this.SetNewEncoding(newEncoding);
            encodingChanged = true;
            if (readEnd != 0 && readFileOffset != 0) {
                this.BackupForRestart(readBuffer, 0, readEnd, readFileOffset, true);
                readEnd = 0;
                readFileOffset = 0;
            }
            readCurrent = 0;
            pushChunkUsed = 0;
            restartConsumer = null;
            parseStart = parseEnd = 0;
            restarting = restartCache != null && restartCache.Length != 0;
            return true;
        }

        private void SetNewEncoding(System.Text.Encoding newEncoding) {
            this.Encoding = newEncoding;
            decoder = this.Encoding.GetDecoder();
            preamble = this.Encoding.GetPreamble();
            minDecodeChars = this.GetMaxCharCount(minDecodeBytes);
            if (resultFeedback == null)
                return;
            resultFeedback.Set(ConfigParameter.InputEncoding, newEncoding);
        }

        protected override void Dispose(bool disposing) {
            if (disposing && restartCache != null && restartCache is IDisposable)
                ((IDisposable) restartCache).Dispose();
            restartCache = null;
            pullSource = null;
            pushSource = null;
            parseBuffer = null;
            readBuffer = null;
            pushChunkBuffer = null;
            preamble = null;
            restartConsumer = null;
            base.Dispose(disposing);
        }

        private int DecodeFromBuffer(byte[] buffer, ref int start, int end, int fileOffset, bool flush) {
            var num = 0;
            if (fileOffset == 0) {
                if (detectEncodingFromByteOrderMark)
                    this.DetectEncoding(buffer, start, end);
                if (preamble.Length != 0 && end - start >= preamble.Length) {
                    var index = 0;
                    while (index < preamble.Length && preamble[index] == buffer[start + index])
                        ++index;
                    if (index == preamble.Length) {
                        start += preamble.Length;
                        num = preamble.Length;
                        if (restartConsumer != null) {
                            restartConsumer.DisableRestart();
                            restartConsumer = null;
                        }
                    }
                }
                encodingChanged = true;
                preamble = null;
            }
            var byteCount = end - start;
            if (this.GetMaxCharCount(byteCount) >= parseBuffer.Length - parseEnd)
                byteCount = this.CalculateMaxBytes(parseBuffer.Length - parseEnd - 1);
            parseEnd += decoder.GetChars(buffer, start, byteCount, parseBuffer, parseEnd);
            parseBuffer[parseEnd] = char.MinValue;
            start += byteCount;
            return byteCount + num;
        }

        private bool EnsureFreeSpace() {
            if (parseBuffer.Length - (parseEnd - parseStart) < minDecodeChars + 1 || parseStart < minDecodeChars && parseBuffer.Length < maxTokenSize + (long) (minDecodeChars + 1)) {
                if (parseBuffer.Length >= maxTokenSize + (long) (minDecodeChars + 1))
                    return false;
                long num = parseBuffer.Length*2;
                if (num > maxTokenSize + (long) (minDecodeChars + 1))
                    num = maxTokenSize + (long) (minDecodeChars + 1);
                if (num > int.MaxValue)
                    num = int.MaxValue;
                if (num - (parseEnd - parseStart) < minDecodeChars + 1)
                    return false;
                char[] chArray;
                try {
                    chArray = new char[(int) num];
                } catch (OutOfMemoryException ex) {
                    throw new TextConvertersException(CtsResources.TextConvertersStrings.TagTooLong, ex);
                }
                Buffer.BlockCopy(parseBuffer, parseStart*2, chArray, 0, (parseEnd - parseStart + 1)*2);
                parseBuffer = chArray;
                parseEnd = parseEnd - parseStart;
                parseStart = 0;
            } else {
                Buffer.BlockCopy(parseBuffer, parseStart*2, parseBuffer, 0, (parseEnd - parseStart + 1)*2);
                parseEnd = parseEnd - parseStart;
                parseStart = 0;
            }
            return true;
        }

        private int GetMaxCharCount(int byteCount) {
            if (string.Compare(this.Encoding.WebName, "utf-8", StringComparison.OrdinalIgnoreCase) == 0)
                return byteCount + 1;
            if (string.Compare(this.Encoding.WebName, "GB18030", StringComparison.OrdinalIgnoreCase) == 0)
                return byteCount + 3;
            return this.Encoding.GetMaxCharCount(byteCount);
        }

        private int CalculateMaxBytes(int charCount) {
            if (charCount == this.GetMaxCharCount(charCount))
                return charCount;
            if (charCount == this.GetMaxCharCount(charCount - 1))
                return charCount - 1;
            if (charCount == this.GetMaxCharCount(charCount - 3))
                return charCount - 3;
            var byteCount1 = charCount - 4;
            var maxCharCount = this.GetMaxCharCount(byteCount1);
            var byteCount2 = (int) (byteCount1*(double) charCount/maxCharCount);
            while (this.GetMaxCharCount(byteCount2) < charCount)
                ++byteCount2;
            do {
                --byteCount2;
            } while (this.GetMaxCharCount(byteCount2) > charCount);
            return byteCount2;
        }

        private void DetectEncoding(byte[] buffer, int start, int end) {
            if (end - start < 2)
                return;
            System.Text.Encoding encoding = null;
            if (buffer[start] == 254 && buffer[start + 1] == byte.MaxValue)
                encoding = System.Text.Encoding.BigEndianUnicode;
            else if (buffer[start] == byte.MaxValue && buffer[start + 1] == 254)
                encoding = end - start < 4 || (int) buffer[start + 2] != 0 || (int) buffer[start + 3] != 0 ? System.Text.Encoding.Unicode : System.Text.Encoding.GetEncoding("utf-32");
            else if (end - start >= 3 && buffer[start] == 239 && (buffer[start + 1] == 187 && buffer[start + 2] == 191))
                encoding = System.Text.Encoding.UTF8;
            else if (end - start >= 4 && buffer[start] == 0 && (buffer[start + 1] == 0 && buffer[start + 2] == 254) && buffer[start + 3] == byte.MaxValue)
                encoding = System.Text.Encoding.GetEncoding("utf-32BE");
            if (encoding == null)
                return;
            this.Encoding = encoding;
            decoder = this.Encoding.GetDecoder();
            preamble = this.Encoding.GetPreamble();
            minDecodeChars = this.GetMaxCharCount(minDecodeBytes);
            if (restartConsumer == null)
                return;
            restartConsumer.DisableRestart();
            restartConsumer = null;
        }

        private void BackupForRestart(byte[] buffer, int offset, int count, int fileOffset, bool force) {
            if (!force && fileOffset > restartMax) {
                restartConsumer.DisableRestart();
                restartConsumer = null;
                preamble = null;
            } else {
                if (restartCache == null)
                    restartCache = new ByteCache();
                byte[] buffer1;
                int offset1;
                restartCache.GetBuffer(count, out buffer1, out offset1);
                Buffer.BlockCopy(buffer, offset, buffer1, offset1, count);
                restartCache.Commit(count);
            }
        }

        private bool GetRestartChunk(out byte[] restartChunk, out int restartStart, out int restartEnd) {
            if (restartCache.Length == 0) {
                restartChunk = null;
                restartStart = 0;
                restartEnd = 0;
                return false;
            }
            int outputCount;
            restartCache.GetData(out restartChunk, out restartStart, out outputCount);
            restartEnd = restartStart + outputCount;
            return true;
        }

        private void ReportRestartChunkUsed(int count) {
            restartCache.ReportRead(count);
        }

        private readonly bool detectEncodingFromByteOrderMark;
        private readonly int minDecodeBytes;
        private readonly System.Text.Encoding originalEncoding;
        private readonly int restartMax;
        private readonly IResultsFeedback resultFeedback;
        private System.Text.Decoder decoder;
        private bool encodingChanged;
        private int minDecodeChars;
        private char[] parseBuffer;
        private int parseEnd;
        private int parseStart;
        private byte[] preamble;
        private System.IO.Stream pullSource;
        private byte[] pushChunkBuffer;
        private int pushChunkCount;
        private int pushChunkStart;
        private int pushChunkUsed;
        private ConverterStream pushSource;
        private bool rawEndOfFile;
        private byte[] readBuffer;
        private int readCurrent;
        private int readEnd;
        private int readFileOffset;
        private ByteCache restartCache;
        private IRestartable restartConsumer;
        private bool restarting;

    }

}