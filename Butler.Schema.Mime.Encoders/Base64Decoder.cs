using System.Linq;

namespace Butler.Schema.Mime.Encoders {

    public class Base64Decoder : ByteEncoder {

        public Base64Decoder() {
            _decoded = new byte[3];
            _encoded = new byte[4];
        }

        public override sealed void Convert(byte[] input, int inputIndex, int inputSize, byte[] output, int outputIndex, int outputSize, bool flush, out int inputUsed, out int outputUsed, out bool completed) {
            if (inputSize != 0) {
                if (input == null)
                    throw new System.ArgumentNullException(nameof(input));
                if (inputIndex < 0 || inputIndex >= input.Length)
                    throw new System.ArgumentOutOfRangeException(nameof(inputIndex));
                if (inputSize < 0 || inputSize > input.Length - inputIndex)
                    throw new System.ArgumentOutOfRangeException(nameof(inputSize));
            }
            if (output == null)
                throw new System.ArgumentNullException(nameof(output));
            if (outputIndex < 0 || outputIndex >= output.Length)
                throw new System.ArgumentOutOfRangeException(nameof(outputIndex));
            if (outputSize < 1 || outputSize > output.Length - outputIndex)
                throw new System.ArgumentOutOfRangeException(nameof(outputSize));
            inputUsed = inputIndex;
            outputUsed = outputIndex;
            var num1 = inputIndex + inputSize;
            if (_decodedSize != 0) {
                var num2 = System.Math.Min(outputSize, _decodedSize);
                if ((num2 & 2) != 0) {
                    output[outputIndex++] = _decoded[_decodedIndex++];
                    output[outputIndex++] = _decoded[_decodedIndex++];
                }
                if ((num2 & 1) != 0)
                    output[outputIndex++] = _decoded[_decodedIndex++];
                outputSize -= num2;
                _decodedSize -= num2;
                if (_decodedSize == 0)
                    _decodedIndex = 0;
            }
            while (_decodedSize == 0 && (inputIndex != num1 || flush && _encodedSize != 0)) {
                while (inputIndex != num1 && 4 != _encodedSize) {
                    var bT = input[inputIndex++];
                    if (bT != 61 && !ByteEncoder.IsWhiteSpace(bT)) {
                        var num2 = (byte) (bT - 32U);
                        if (num2 < Tables.Base64ToByte.Length) {
                            var num3 = Tables.Base64ToByte[num2];
                            if (num3 < 64)
                                _encoded[_encodedSize++] = num3;
                        }
                    }
                }
                if (4 == _encodedSize && 3 <= outputSize) {
                    output[outputIndex] = (byte) (_encoded[0] << 2 | _encoded[1] >> 4);
                    output[outputIndex + 1] = (byte) (_encoded[1] << 4 | _encoded[2] >> 2);
                    output[outputIndex + 2] = (byte) ((uint) _encoded[2] << 6 | _encoded[3]);
                    outputSize -= 3;
                    outputIndex += 3;
                    _encodedSize = 0;
                } else if (4 == _encodedSize || flush && num1 == inputIndex) {
                    if (2 > _encodedSize) {
                        _encodedSize = 0;
                        break;
                    }
                    if (_encodedSize > 1)
                        _decoded[_decodedSize++] = (byte) (_encoded[0] << 2 | _encoded[1] >> 4);
                    if (_encodedSize > 2)
                        _decoded[_decodedSize++] = (byte) (_encoded[1] << 4 | _encoded[2] >> 2);
                    if (_encodedSize > 3)
                        _decoded[_decodedSize++] = (byte) ((uint) _encoded[2] << 6 | _encoded[3]);
                    _encodedSize = 0;
                    var num2 = System.Math.Min(outputSize, _decodedSize);
                    if ((num2 & 2) != 0) {
                        output[outputIndex++] = _decoded[_decodedIndex++];
                        output[outputIndex++] = _decoded[_decodedIndex++];
                    }
                    if ((num2 & 1) != 0)
                        output[outputIndex++] = _decoded[_decodedIndex++];
                    outputSize -= num2;
                    _decodedSize -= num2;
                    if (_decodedSize == 0)
                        _decodedIndex = 0;
                } else
                    break;
            }
            outputUsed = outputIndex - outputUsed;
            inputUsed = inputIndex - inputUsed;
            completed = num1 == inputIndex && _decodedSize == 0 && (!flush || 0 == _encodedSize);
        }

        public override sealed int GetMaxByteCount(int dataCount) {
            return (dataCount + 4)/4*3;
        }

        public override sealed void Reset() {
            _decodedSize = 0;
            _decodedIndex = 0;
            _encodedSize = 0;
        }

        public override sealed ByteEncoder Clone() {
            var base64Decoder = this.MemberwiseClone() as Base64Decoder;
            base64Decoder._decoded = _decoded.Clone() as byte[];
            base64Decoder._encoded = _encoded.Clone() as byte[];
            return base64Decoder;
        }

        private byte[] _decoded;
        private int _decodedIndex;
        private int _decodedSize;
        private byte[] _encoded;
        private int _encodedSize;

    }

}