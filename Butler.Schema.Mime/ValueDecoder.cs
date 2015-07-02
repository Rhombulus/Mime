using System.Linq;

namespace Butler.Schema.Mime {

    internal struct ValueDecoder {

        public ValueDecoder(MimeStringList lines, uint linesMask) {
            iterator = new ValueIterator(lines, linesMask);
            maxCharsetNameLength = System.Math.Max(60, Globalization.Charset.MaxCharsetNameLength) + 10 + 1;
        }

        public bool TryDecodeValue(
            Globalization.Charset defaultCharset, bool enableFallback, bool allowControlCharacters, bool enable2047, bool enableJisDetection, bool enableUtf8Detection, bool enableDbcsDetection, out string charsetName, out string cultureName,
            out EncodingScheme encodingScheme, out string value) {
            charsetName = null;
            cultureName = null;
            encodingScheme = EncodingScheme.None;
            value = null;
            var stringBuilder = Internal.ScratchPad.GetStringBuilder(System.Math.Min(1024, iterator.TotalLength));
            char[] charBuffer = null;
            byte[] byteBuffer = null;
            var currentPosition1 = iterator.CurrentPosition;
            var whitespaceOnly = false;
            var flag1 = false;
            if (defaultCharset != null && (enableJisDetection || enableUtf8Detection || enableDbcsDetection && Globalization.FeInboundCharsetDetector.IsSupportedFarEastCharset(defaultCharset))) {
                defaultCharset = this.DetectValueCharset(defaultCharset, enableJisDetection, enableUtf8Detection, enableDbcsDetection, out encodingScheme);
                flag1 = true;
            }
            System.Text.Decoder decoder1 = null;
            string lastEncodedWordCharsetName = null;
            if (!enable2047)
                iterator.SkipToEof();
            else {
                string lastEncodedWordLanguage = null;
                Globalization.Charset charset1 = null;
                System.Text.Decoder decoder2 = null;
                var flag2 = true;
                string encodedWordCharsetName;
                while (true) {
                    this.ParseRawFragment(ref whitespaceOnly);
                    if (!iterator.Eof) {
                        var currentPosition2 = iterator.CurrentPosition;
                        string encodedWordLanguage;
                        byte bOrQ;
                        ValuePosition encodedWordContentStart;
                        ValuePosition encodedWordContentEnd;
                        if (!this.ParseEncodedWord(lastEncodedWordCharsetName, lastEncodedWordLanguage, ref byteBuffer, out encodedWordCharsetName, out encodedWordLanguage, out bOrQ, out encodedWordContentStart, out encodedWordContentEnd))
                            whitespaceOnly = false;
                        else {
                            if (lastEncodedWordCharsetName == null) {
                                encodingScheme = EncodingScheme.Rfc2047;
                                charsetName = encodedWordCharsetName;
                                cultureName = encodedWordLanguage;
                            }
                            lastEncodedWordCharsetName = encodedWordCharsetName;
                            if (currentPosition1 != currentPosition2 && !whitespaceOnly) {
                                if (!flag2) {
                                    this.FlushDecoder(decoder2, allowControlCharacters, ref byteBuffer, ref charBuffer, stringBuilder);
                                    flag2 = true;
                                }
                                if (decoder1 == null) {
                                    if (defaultCharset == null || !defaultCharset.IsAvailable) {
                                        if (!enableFallback)
                                            break;
                                    } else {
                                        decoder1 = defaultCharset.GetEncoding()
                                                                 .GetDecoder();
                                    }
                                }
                                if (decoder1 != null)
                                    this.ConvertRawFragment(currentPosition1, currentPosition2, decoder1, allowControlCharacters, ref charBuffer, stringBuilder);
                                else
                                    this.ZeroExpandFragment(currentPosition1, currentPosition2, allowControlCharacters, stringBuilder);
                            }
                            Globalization.Charset charset2;
                            if (!Globalization.Charset.TryGetCharset(encodedWordCharsetName, out charset2)) {
                                if (!flag2) {
                                    this.FlushDecoder(decoder2, allowControlCharacters, ref byteBuffer, ref charBuffer, stringBuilder);
                                    flag2 = true;
                                }
                                if (enableFallback)
                                    decoder2 = null;
                                else
                                    goto label_25;
                            } else if (charset2 != charset1) {
                                if (!flag2) {
                                    this.FlushDecoder(decoder2, allowControlCharacters, ref byteBuffer, ref charBuffer, stringBuilder);
                                    flag2 = true;
                                }
                                if (!charset2.IsAvailable) {
                                    if (enableFallback)
                                        decoder2 = null;
                                    else
                                        goto label_32;
                                } else {
                                    decoder2 = charset2.GetEncoding()
                                                       .GetDecoder();
                                    charset1 = charset2;
                                }
                            }
                            if (decoder2 != null) {
                                this.DecodeEncodedWord(bOrQ, decoder2, encodedWordContentStart, encodedWordContentEnd, allowControlCharacters, ref byteBuffer, ref charBuffer, stringBuilder);
                                flag2 = false;
                            } else
                                this.ZeroExpandFragment(currentPosition2, iterator.CurrentPosition, allowControlCharacters, stringBuilder);
                            currentPosition1 = iterator.CurrentPosition;
                            whitespaceOnly = true;
                        }
                    } else
                        goto label_39;
                }
                charsetName = defaultCharset == null ? null : defaultCharset.Name;
                return false;
                label_25:
                charsetName = encodedWordCharsetName;
                return false;
                label_32:
                charsetName = encodedWordCharsetName;
                return false;
                label_39:
                if (!flag2)
                    this.FlushDecoder(decoder2, allowControlCharacters, ref byteBuffer, ref charBuffer, stringBuilder);
            }
            if (currentPosition1 != iterator.CurrentPosition) {
                if (lastEncodedWordCharsetName == null) {
                    charsetName = !flag1 || encodingScheme != EncodingScheme.None || (defaultCharset == null || defaultCharset.IsSevenBit) || defaultCharset.AsciiSupport != Globalization.CodePageAsciiSupport.Complete
                                      ? (defaultCharset == null ? null : defaultCharset.Name)
                                      : Globalization.Charset.ASCII.Name;
                }
                if (decoder1 == null) {
                    if (defaultCharset == null || !defaultCharset.IsAvailable) {
                        if (!enableFallback) {
                            charsetName = defaultCharset == null ? null : defaultCharset.Name;
                            return false;
                        }
                        decoder1 = null;
                    } else {
                        decoder1 = defaultCharset.GetEncoding()
                                                 .GetDecoder();
                    }
                }
                if (decoder1 != null)
                    this.ConvertRawFragment(currentPosition1, iterator.CurrentPosition, decoder1, allowControlCharacters, ref charBuffer, stringBuilder);
                else
                    this.ZeroExpandFragment(currentPosition1, iterator.CurrentPosition, allowControlCharacters, stringBuilder);
            }
            Internal.ScratchPad.ReleaseStringBuilder();
            value = stringBuilder.ToString();
            return true;
        }

        private static bool Is2047Token(byte bT) {
            if (bT < 128) {
                return
                    ~(Schema.Mime.Encoders.ByteEncoder.Tables.CharClasses.WSp | Schema.Mime.Encoders.ByteEncoder.Tables.CharClasses.QPEncode | Schema.Mime.Encoders.ByteEncoder.Tables.CharClasses.QPUnsafe | Schema.Mime.Encoders.ByteEncoder.Tables.CharClasses.QPWSp |
                      Schema.Mime.Encoders.ByteEncoder.Tables.CharClasses.QEncode | Schema.Mime.Encoders.ByteEncoder.Tables.CharClasses.QPhraseUnsafe | Schema.Mime.Encoders.ByteEncoder.Tables.CharClasses.QCommentUnsafe | Schema.Mime.Encoders.ByteEncoder.Tables.CharClasses.Token2047) !=
                    (Schema.Mime.Encoders.ByteEncoder.Tables.CharacterTraits[bT] & Schema.Mime.Encoders.ByteEncoder.Tables.CharClasses.Token2047);
            }
            return false;
        }

        private static void RemoveProhibitedControlCharacters(char[] charBuffer, int offset, int length) {
            for (; length != 0; --length) {
                var ch = charBuffer[offset];
                if (ch < 32)
                    charBuffer[offset] = ValueDecoder.ReplaceProhibitedControlCharacter(ch);
                ++offset;
            }
        }

        private static char ReplaceProhibitedControlCharacter(char ch) {
            foreach (int num in ControlCharacters) {
                if (num == ch)
                    return ' ';
            }
            return ch;
        }

        //    private bool ParseEncodedWord(string lastEncodedWordCharsetName, string lastEncodedWordLanguage, ref byte[] byteBuffer, out string encodedWordCharsetName, out string encodedWordLanguage, out byte bOrQ, out ValuePosition encodedWordContentStart, out ValuePosition encodedWordContentEnd)
        //    {
        //      encodedWordCharsetName = (string) null;
        //      encodedWordLanguage = (string) null;
        //      bOrQ = (byte) 0;
        //      // ISSUE: explicit reference operation
        //      // ISSUE: variable of a reference type
        //      ValuePosition& local1 = @encodedWordContentStart;
        //      // ISSUE: explicit reference operation
        //      // ISSUE: variable of a reference type
        //      ValuePosition& local2 = @encodedWordContentEnd;
        //      // ISSUE: explicit reference operation
        //      // ISSUE: variable of a reference type
        //      ValuePosition& local3 = @encodedWordContentEnd;
        //      ValuePosition valuePosition1 = new ValuePosition();
        //      ValuePosition valuePosition2 = valuePosition1;
        //      // ISSUE: explicit reference operation
        //      ^local3 = valuePosition2;
        //      ValuePosition valuePosition3;
        //      ValuePosition valuePosition4 = valuePosition3 = valuePosition1;
        //      // ISSUE: explicit reference operation
        //      ^local2 = valuePosition3;
        //      ValuePosition valuePosition5 = valuePosition4;
        //      // ISSUE: explicit reference operation
        //      ^local1 = valuePosition5;
        //      int num1 = this.iterator.Get();
        //      if (this.iterator.Get() != 63)
        //        return false;
        //      if (byteBuffer == null)
        //        byteBuffer = ScratchPad.GetByteBuffer(Math.Max(this.maxCharsetNameLength + 1, Math.Min(1024, this.iterator.TotalLength)));
        //      int num2 = -1;
        //      int count1;
        //      for (count1 = 0; count1 < this.maxCharsetNameLength + 1; ++count1)
        //      {
        //        num1 = this.iterator.Get();
        //        if (ValueDecoder.Is2047Token((byte) num1))
        //        {
        //          byteBuffer[count1] = (byte) num1;
        //          if (num2 == -1 && num1 == 42)
        //            num2 = count1;
        //        }
        //        else
        //          break;
        //      }
        //      if (count1 == this.maxCharsetNameLength + 1 || num1 != 63 || (count1 == 0 || num2 == 0))
        //        return false;
        //      int num3 = this.iterator.Get();
        //      bOrQ = num3 == 66 || num3 == 98 ? (byte) 66 : (num3 == 81 || num3 == 113 ? (byte) 81 : (byte) 0);
        //      if ((int) bOrQ == 0 || this.iterator.Get() != 63)
        //        return false;
        //      if (num2 != -1)
        //      {
        //        int offset = num2 + 1;
        //        int count2 = count1 - (num2 + 1);
        //        count1 = num2;
        //        if (count2 != 0)
        //        {
        //          if (lastEncodedWordLanguage != null && count2 == lastEncodedWordLanguage.Length)
        //          {
        //            int index = 0;
        //            while (index < count2 && (int) lastEncodedWordLanguage[index] == (int) byteBuffer[offset + index])
        //              ++index;
        //            encodedWordLanguage = index == count2 ? lastEncodedWordLanguage : ByteString.BytesToString(byteBuffer, offset, count2, false);
        //          }
        //          else
        //            encodedWordLanguage = ByteString.BytesToString(byteBuffer, offset, count2, false);
        //        }
        //      }
        //      if (lastEncodedWordCharsetName != null && count1 == lastEncodedWordCharsetName.Length)
        //      {
        //        int index = 0;
        //        while (index < count1 && (int) lastEncodedWordCharsetName[index] == (int) byteBuffer[index])
        //          ++index;
        //        encodedWordCharsetName = index == count1 ? lastEncodedWordCharsetName : ByteString.BytesToString(byteBuffer, 0, count1, false);
        //      }
        //      else
        //        encodedWordCharsetName = ByteString.BytesToString(byteBuffer, 0, count1, false);
        //      encodedWordContentStart = this.iterator.CurrentPosition;
        //      bool flag = false;
        //      while (true)
        //      {
        //        encodedWordContentEnd = this.iterator.CurrentPosition;
        //        int num4 = this.iterator.Get();
        //        if (num4 != -1)
        //        {
        //          if (MimeScan.IsLWSP((byte) num4))
        //          {
        //            flag = true;
        //          }
        //          else
        //          {
        //            if (num4 == 63)
        //            {
        //              switch (this.iterator.Get())
        //              {
        //                case -1:
        //                  goto label_35;
        //                case 61:
        //                  goto label_44;
        //                default:
        //                  this.iterator.Unget();
        //                  if ((int) bOrQ == 81)
        //                    break;
        //                  goto label_37;
        //              }
        //            }
        //            else if (num4 == 61 && flag)
        //            {
        //              switch (this.iterator.Get())
        //              {
        //                case -1:
        //                  goto label_40;
        //                case 63:
        //                  goto label_41;
        //                default:
        //                  this.iterator.Unget();
        //                  break;
        //              }
        //            }
        //            flag = false;
        //          }
        //        }
        //        else
        //          break;
        //      }
        //      return false;
        //label_35:
        //      return false;
        //label_37:
        //      return false;
        //label_40:
        //      return false;
        //label_41:
        //      this.iterator.Unget();
        //      this.iterator.Unget();
        //      return false;
        //label_44:
        //      return true;
        //    }
        private bool ParseEncodedWord(
            string lastEncodedWordCharsetName, string lastEncodedWordLanguage, ref byte[] byteBuffer, out string encodedWordCharsetName, out string encodedWordLanguage, out byte bOrQ, out ValuePosition encodedWordContentStart,
            out ValuePosition encodedWordContentEnd) {
            encodedWordCharsetName = null;
            encodedWordLanguage = null;
            bOrQ = 0;
            var position = new ValuePosition();
            encodedWordContentEnd = position;
            encodedWordContentStart = encodedWordContentEnd = position;
            var num = iterator.Get();
            if (iterator.Get() != 0x3f)
                return false;
            if (byteBuffer == null)
                byteBuffer = Internal.ScratchPad.GetByteBuffer(System.Math.Max(maxCharsetNameLength + 1, System.Math.Min(0x400, iterator.TotalLength)));
            var num3 = -1;
            var index = 0;
            while (index < (maxCharsetNameLength + 1)) {
                num = iterator.Get();
                if (!ValueDecoder.Is2047Token((byte) num))
                    break;
                byteBuffer[index] = (byte) num;
                if ((num3 == -1) && (num == 0x2a))
                    num3 = index;
                index++;
            }
            if (((index == (maxCharsetNameLength + 1)) || (num != 0x3f)) || ((index == 0) || (num3 == 0)))
                return false;
            num = iterator.Get();
            bOrQ = ((num == 0x42) || (num == 0x62)) ? ((byte) 0x42) : (((num == 0x51) || (num == 0x71)) ? ((byte) 0x51) : ((byte) 0));
            if ((bOrQ == 0) || (iterator.Get() != 0x3f))
                return false;
            if (num3 != -1) {
                var offset = num3 + 1;
                var count = index - (num3 + 1);
                index = num3;
                if (count != 0) {
                    if ((lastEncodedWordLanguage == null) || (count != lastEncodedWordLanguage.Length))
                        encodedWordLanguage = Internal.ByteString.BytesToString(byteBuffer, offset, count, false);
                    else {
                        var num6 = 0;
                        while (num6 < count) {
                            if (lastEncodedWordLanguage[num6] != byteBuffer[offset + num6])
                                break;
                            num6++;
                        }
                        if (num6 != count)
                            encodedWordLanguage = Internal.ByteString.BytesToString(byteBuffer, offset, count, false);
                        else
                            encodedWordLanguage = lastEncodedWordLanguage;
                    }
                }
            }
            if ((lastEncodedWordCharsetName == null) || (index != lastEncodedWordCharsetName.Length))
                encodedWordCharsetName = Internal.ByteString.BytesToString(byteBuffer, 0, index, false);
            else {
                var num7 = 0;
                while (num7 < index) {
                    if (lastEncodedWordCharsetName[num7] != byteBuffer[num7])
                        break;
                    num7++;
                }
                if (num7 != index)
                    encodedWordCharsetName = Internal.ByteString.BytesToString(byteBuffer, 0, index, false);
                else
                    encodedWordCharsetName = lastEncodedWordCharsetName;
            }
            encodedWordContentStart = iterator.CurrentPosition;
            var flag = false;
            while (true) {
                encodedWordContentEnd = iterator.CurrentPosition;
                num = iterator.Get();
                if (num == -1)
                    return false;
                if (MimeScan.IsLWSP((byte) num))
                    flag = true;
                else {
                    if (num == 0x3f) {
                        num = iterator.Get();
                        switch (num) {
                            case -1:
                                return false;

                            case 0x3d:
                                return true;
                        }
                        iterator.Unget();
                        if (bOrQ != 0x51)
                            return false;
                    } else if ((num == 0x3d) && flag) {
                        switch (iterator.Get()) {
                            case -1:
                                return false;

                            case 0x3f:
                                iterator.Unget();
                                iterator.Unget();
                                return false;
                        }
                        iterator.Unget();
                    }
                    flag = false;
                }
            }
        }

        private void DecodeEncodedWord(
            byte bOrQ, System.Text.Decoder decoder, ValuePosition encodedWordContentStart, ValuePosition encodedWordContentEnd, bool allowControlCharacters, ref byte[] byteBuffer, ref char[] charBuffer, System.Text.StringBuilder sb) {
            var valueIterator = new ValueIterator(iterator.Lines, iterator.LinesMask, encodedWordContentStart, encodedWordContentEnd);
            if (charBuffer == null)
                charBuffer = Internal.ScratchPad.GetCharBuffer(System.Math.Min(1024, iterator.TotalLength));
            if (byteBuffer == null)
                byteBuffer = Internal.ScratchPad.GetByteBuffer(System.Math.Max(maxCharsetNameLength + 1, System.Math.Min(1024, iterator.TotalLength)));
            var byteBufferLength = 0;
            if (bOrQ == 66) {
                var num1 = 0;
                var num2 = 0;
                while (!valueIterator.Eof) {
                    var num3 = (byte) (valueIterator.Get() - 32);
                    if (num3 < Schema.Mime.Encoders.ByteEncoder.Tables.Base64ToByte.Length) {
                        var num4 = Schema.Mime.Encoders.ByteEncoder.Tables.Base64ToByte[num3];
                        if (num4 < 64) {
                            num2 = num2 << 6 | num4;
                            ++num1;
                            if (num1 == 4) {
                                var numArray1 = byteBuffer;
                                var index1 = byteBufferLength;
                                var num5 = 1;
                                var num6 = index1 + num5;
                                int num7 = (byte) (num2 >> 16);
                                numArray1[index1] = (byte) num7;
                                var numArray2 = byteBuffer;
                                var index2 = num6;
                                var num8 = 1;
                                var num9 = index2 + num8;
                                int num10 = (byte) (num2 >> 8);
                                numArray2[index2] = (byte) num10;
                                var numArray3 = byteBuffer;
                                var index3 = num9;
                                var num11 = 1;
                                byteBufferLength = index3 + num11;
                                int num12 = (byte) num2;
                                numArray3[index3] = (byte) num12;
                                num1 = 0;
                                if (byteBufferLength + 3 >= byteBuffer.Length) {
                                    this.FlushDecodedBytes(byteBuffer, byteBufferLength, decoder, allowControlCharacters, charBuffer, sb);
                                    byteBufferLength = 0;
                                }
                            }
                        }
                    }
                }
                if (num1 != 0) {
                    if (num1 == 2) {
                        var num3 = num2 << 12;
                        byteBuffer[byteBufferLength++] = (byte) (num3 >> 16);
                    } else if (num1 == 3) {
                        var num3 = num2 << 6;
                        var numArray1 = byteBuffer;
                        var index1 = byteBufferLength;
                        var num4 = 1;
                        var num5 = index1 + num4;
                        int num6 = (byte) (num3 >> 16);
                        numArray1[index1] = (byte) num6;
                        var numArray2 = byteBuffer;
                        var index2 = num5;
                        var num7 = 1;
                        byteBufferLength = index2 + num7;
                        int num8 = (byte) (num3 >> 8);
                        numArray2[index2] = (byte) num8;
                    }
                }
            } else {
                while (!valueIterator.Eof) {
                    var num1 = (byte) valueIterator.Get();
                    switch (num1) {
                        case 61:
                            var index1 = valueIterator.Get();
                            var index2 = valueIterator.Get();
                            int num2 = index1 < 0 ? byte.MaxValue : Schema.Mime.Encoders.ByteEncoder.Tables.NumFromHex[index1];
                            int num3 = index2 < 0 ? byte.MaxValue : Schema.Mime.Encoders.ByteEncoder.Tables.NumFromHex[index2];
                            num1 = num2 == (int) byte.MaxValue || num3 == (int) byte.MaxValue ? (byte) 61 : (byte) (num2 << 4 | num3);
                            break;
                        case 95:
                            num1 = 32;
                            break;
                    }
                    byteBuffer[byteBufferLength++] = num1;
                    if (byteBufferLength >= byteBuffer.Length) {
                        this.FlushDecodedBytes(byteBuffer, byteBufferLength, decoder, allowControlCharacters, charBuffer, sb);
                        byteBufferLength = 0;
                    }
                }
            }
            if (byteBufferLength == 0)
                return;
            this.FlushDecodedBytes(byteBuffer, byteBufferLength, decoder, allowControlCharacters, charBuffer, sb);
        }

        private void FlushDecodedBytes(byte[] byteBuffer, int byteBufferLength, System.Text.Decoder decoder, bool allowControlCharacters, char[] charBuffer, System.Text.StringBuilder sb) {
            var byteIndex = 0;
            bool completed;
            do {
                int bytesUsed;
                int charsUsed;
                decoder.Convert(byteBuffer, byteIndex, byteBufferLength, charBuffer, 0, charBuffer.Length, false, out bytesUsed, out charsUsed, out completed);
                if (charsUsed != 0) {
                    if (!allowControlCharacters)
                        ValueDecoder.RemoveProhibitedControlCharacters(charBuffer, 0, charsUsed);
                    sb.Append(charBuffer, 0, charsUsed);
                }
                byteIndex += bytesUsed;
                byteBufferLength -= bytesUsed;
            } while (!completed);
        }

        private void FlushDecoder(System.Text.Decoder decoder, bool allowControlCharacters, ref byte[] byteBuffer, ref char[] charBuffer, System.Text.StringBuilder sb) {
            int bytesUsed;
            int charsUsed;
            bool completed;
            decoder.Convert(byteBuffer, 0, 0, charBuffer, 0, charBuffer.Length, true, out bytesUsed, out charsUsed, out completed);
            if (charsUsed == 0)
                return;
            if (!allowControlCharacters)
                ValueDecoder.RemoveProhibitedControlCharacters(charBuffer, 0, charsUsed);
            sb.Append(charBuffer, 0, charsUsed);
        }

        private void ParseRawFragment(ref bool whitespaceOnly) {
            while (!iterator.Eof && 61 != iterator.Pick()) {
                var length1 = MimeScan.SkipLwsp(iterator.Bytes, iterator.Offset, iterator.Length);
                if (length1 != 0)
                    iterator.Get(length1);
                if (iterator.Eof || 61 == iterator.Pick())
                    break;
                var length2 = MimeScan.SkipToLwspOrEquals(iterator.Bytes, iterator.Offset, iterator.Length);
                if (length2 != 0) {
                    whitespaceOnly = false;
                    iterator.Get(length2);
                }
            }
        }

        private Globalization.Charset DetectValueCharset(Globalization.Charset defaultCharset, bool enableJisDetection, bool enableUtf8Detection, bool enableDbcsDetection, out EncodingScheme encodingScheme) {
            var valueIterator = new ValueIterator(iterator.Lines, iterator.LinesMask);
            var inboundCharsetDetector = new Globalization.FeInboundCharsetDetector(defaultCharset.CodePage, false, enableJisDetection, enableUtf8Detection, enableDbcsDetection);
            while (!valueIterator.Eof) {
                inboundCharsetDetector.AddBytes(valueIterator.Bytes, valueIterator.Offset, valueIterator.Length, false);
                valueIterator.Get(valueIterator.Length);
            }
            inboundCharsetDetector.AddBytes(null, 0, 0, true);
            var codePageChoice = inboundCharsetDetector.GetCodePageChoice();
            if (codePageChoice != defaultCharset.CodePage)
                defaultCharset = Globalization.Charset.GetCharset(codePageChoice);
            encodingScheme = inboundCharsetDetector.PureAscii
                                 ? (!(defaultCharset.Name == "iso-2022-jp") || inboundCharsetDetector.Iso2022KrLikely ? EncodingScheme.None : EncodingScheme.Jis)
                                 : (inboundCharsetDetector.Iso2022JpLikely || inboundCharsetDetector.Iso2022KrLikely ? EncodingScheme.Jis : EncodingScheme.EightBit);
            return defaultCharset;
        }

        private void ZeroExpandFragment(ValuePosition start, ValuePosition end, bool allowControlCharacters, System.Text.StringBuilder sb) {
            var valueIterator = new ValueIterator(iterator.Lines, iterator.LinesMask, start, end);
            while (!valueIterator.Eof) {
                var num = (byte) valueIterator.Get();
                if (!allowControlCharacters && num < 32)
                    num = (byte) ValueDecoder.ReplaceProhibitedControlCharacter((char) num);
                sb.Append((char) num);
            }
        }

        private void ConvertRawFragment(ValuePosition start, ValuePosition end, System.Text.Decoder decoder, bool allowControlCharacters, ref char[] charBuffer, System.Text.StringBuilder sb) {
            var valueIterator = new ValueIterator(iterator.Lines, iterator.LinesMask, start, end);
            if (valueIterator.Eof)
                return;
            if (charBuffer == null)
                charBuffer = Internal.ScratchPad.GetCharBuffer(System.Math.Min(1024, iterator.TotalLength));
            int bytesUsed;
            int charsUsed;
            bool completed;
            do {
                decoder.Convert(valueIterator.Bytes, valueIterator.Offset, valueIterator.Length, charBuffer, 0, charBuffer.Length, false, out bytesUsed, out charsUsed, out completed);
                if (charsUsed != 0) {
                    if (!allowControlCharacters)
                        ValueDecoder.RemoveProhibitedControlCharacters(charBuffer, 0, charsUsed);
                    sb.Append(charBuffer, 0, charsUsed);
                }
                valueIterator.Get(bytesUsed);
            } while (!completed || !valueIterator.Eof);
            decoder.Convert(MimeString.EmptyByteArray, 0, 0, charBuffer, 0, charBuffer.Length, true, out bytesUsed, out charsUsed, out completed);
            if (charsUsed == 0)
                return;
            if (!allowControlCharacters)
                ValueDecoder.RemoveProhibitedControlCharacters(charBuffer, 0, charsUsed);
            sb.Append(charBuffer, 0, charsUsed);
        }

        private const int MaxCharsetNameLength = 60;
        private const int MaxLanguageNameLength = 10;

        private static readonly char[] ControlCharacters = new char[5] {
            char.MinValue,
            '\r',
            '\n',
            '\f',
            '\v'
        };

        private readonly int maxCharsetNameLength;
        private ValueIterator iterator;

    }

}