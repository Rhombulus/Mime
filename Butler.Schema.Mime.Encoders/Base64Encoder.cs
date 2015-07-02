using System;
using System.Linq;

namespace Butler.Schema.Data.Mime.Encoders {

    public class Base64Encoder : ByteEncoder {

        public Base64Encoder()
            : this(76) {}

        public Base64Encoder(int lineLength) {
            this.LineLength = lineLength;
        }

        public int LineLength {
            get {
                return _lineMaximum;
            }
            set {
                if (value != 0 && value != 76)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _lineMaximum = value - value%4;
            }
        }

        public override sealed void Convert(byte[] input, int inputIndex, int inputSize, byte[] output, int outputIndex, int outputSize, bool flush, out int inputUsed, out int outputUsed, out bool completed) {
            if (inputSize != 0) {
                if (input == null)
                    throw new ArgumentNullException(nameof(input));
                if (inputIndex < 0 || inputIndex >= input.Length)
                    throw new ArgumentOutOfRangeException(nameof(inputIndex));
                if (inputSize < 0 || inputSize > input.Length - inputIndex)
                    throw new ArgumentOutOfRangeException(nameof(inputSize));
            }
            if (output == null)
                throw new ArgumentNullException(nameof(output));
            if (outputIndex < 0 || outputIndex >= output.Length)
                throw new ArgumentOutOfRangeException(nameof(outputIndex));
            if (outputSize < 1 || outputSize > output.Length - outputIndex)
                throw new ArgumentOutOfRangeException(nameof(outputSize));
            inputUsed = inputIndex;
            outputUsed = outputIndex;
            if (_encodedSize != 0) {
                var num = Math.Min(_encodedSize, outputSize);
                if ((num & 4) != 0) {
                    output[outputIndex++] = _encoded[_encodedIndex++];
                    output[outputIndex++] = _encoded[_encodedIndex++];
                    output[outputIndex++] = _encoded[_encodedIndex++];
                    output[outputIndex++] = _encoded[_encodedIndex++];
                }
                if ((num & 2) != 0) {
                    output[outputIndex++] = _encoded[_encodedIndex++];
                    output[outputIndex++] = _encoded[_encodedIndex++];
                }
                if ((num & 1) != 0)
                    output[outputIndex++] = _encoded[_encodedIndex++];
                outputSize -= num;
                _encodedSize -= num;
                if (_encodedSize != 0) {
                    inputUsed = 0;
                    outputUsed = num;
                    completed = false;
                    return;
                }
                _encodedIndex = 0;
            }
            var num1 = inputIndex + inputSize;
            while (6 <= outputSize && 3 <= _decodedSize + (num1 - inputIndex)) {
                for (; 3 != _decodedSize; ++_decodedSize) {
                    _decoded = _decoded << 8 | input[inputIndex++];
                }
                output[outputIndex + 3] = Tables.ByteToBase64[(_decoded & 63U)];
                _decoded >>= 6;
                output[outputIndex + 2] = Tables.ByteToBase64[(_decoded & 63U)];
                _decoded >>= 6;
                output[outputIndex + 1] = Tables.ByteToBase64[(_decoded & 63U)];
                _decoded >>= 6;
                output[outputIndex] = Tables.ByteToBase64[(_decoded & 63U)];
                outputIndex += 4;
                outputSize -= 4;
                _decodedSize = 0;
                if (_lineMaximum != 0) {
                    _lineOffset += 4;
                    if (_lineOffset == _lineMaximum) {
                        output[outputIndex++] = 13;
                        output[outputIndex++] = 10;
                        outputSize -= 2;
                        _lineOffset = 0;
                    }
                }
            }
            if (outputSize != 0) {
                for (; num1 != inputIndex && 3 != _decodedSize; ++_decodedSize) {
                    _decoded = _decoded << 8 | input[inputIndex++];
                }
                if (3 == _decodedSize || flush && (_decodedSize != 0 || _lineMaximum != 0 && _lineOffset != 0)) {
                    _encodedSize = 0;
                    switch (_decodedSize) {
                        case 0:
                            _encodedSize = 0;
                            break;
                        case 1:
                            _encoded[3] = 61;
                            _encoded[2] = 61;
                            _decoded <<= 4;
                            _encoded[1] = Tables.ByteToBase64[(_decoded & 63U)];
                            _decoded >>= 6;
                            _encoded[0] = Tables.ByteToBase64[(_decoded & 63U)];
                            _encodedSize = 4;
                            break;
                        case 2:
                            _encoded[3] = 61;
                            _decoded <<= 2;
                            _encoded[2] = Tables.ByteToBase64[(_decoded & 63U)];
                            _decoded >>= 6;
                            _encoded[1] = Tables.ByteToBase64[(_decoded & 63U)];
                            _decoded >>= 6;
                            _encoded[0] = Tables.ByteToBase64[(_decoded & 63U)];
                            _encodedSize = 4;
                            break;
                        case 3:
                            _encoded[3] = Tables.ByteToBase64[(_decoded & 63U)];
                            _decoded >>= 6;
                            _encoded[2] = Tables.ByteToBase64[(_decoded & 63U)];
                            _decoded >>= 6;
                            _encoded[1] = Tables.ByteToBase64[(_decoded & 63U)];
                            _decoded >>= 6;
                            _encoded[0] = Tables.ByteToBase64[(_decoded & 63U)];
                            _encodedSize = 4;
                            break;
                    }
                    _decodedSize = 0;
                    _lineOffset += _encodedSize;
                    if (_lineMaximum != 0 && (_lineOffset == _lineMaximum || flush && num1 == inputIndex)) {
                        _encoded[_encodedSize++] = 13;
                        _encoded[_encodedSize++] = 10;
                        _lineOffset = 0;
                    }
                    if (_encodedSize != 0) {
                        var num2 = Math.Min(_encodedSize, outputSize);
                        if ((num2 & 4) != 0) {
                            output[outputIndex++] = _encoded[_encodedIndex++];
                            output[outputIndex++] = _encoded[_encodedIndex++];
                            output[outputIndex++] = _encoded[_encodedIndex++];
                            output[outputIndex++] = _encoded[_encodedIndex++];
                        }
                        if ((num2 & 2) != 0) {
                            output[outputIndex++] = _encoded[_encodedIndex++];
                            output[outputIndex++] = _encoded[_encodedIndex++];
                        }
                        if ((num2 & 1) != 0)
                            output[outputIndex++] = _encoded[_encodedIndex++];
                        _encodedSize -= num2;
                        if (_encodedSize == 0)
                            _encodedIndex = 0;
                    }
                }
            }
            outputUsed = outputIndex - outputUsed;
            inputUsed = inputIndex - inputUsed;
            completed = num1 == inputIndex && _encodedSize == 0 && (!flush || 0 == _decodedSize);
        }

        public override sealed int GetMaxByteCount(int dataCount) {
            var byteCount = (dataCount + 3)/3*4;
            if (_lineMaximum == 0)
                return byteCount;
            return byteCount + 2*(byteCount + _lineMaximum)/_lineMaximum;
        }

        public override sealed void Reset() {
            _decodedSize = 0;
            _encodedSize = 0;
            _encodedIndex = 0;
            _lineOffset = 0;
        }

        public override sealed ByteEncoder Clone() {
            var base64Encoder = this.MemberwiseClone() as Base64Encoder;
            base64Encoder._encoded = _encoded.Clone() as byte[];
            return base64Encoder;
        }

        private uint _decoded;
        private int _decodedSize;
        private byte[] _encoded = new byte[6];
        private int _encodedIndex;
        private int _encodedSize;
        private int _lineMaximum;
        private int _lineOffset;

    }

}