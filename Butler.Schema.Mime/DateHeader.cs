using System.Linq;

namespace Butler.Schema.Data.Mime {

    public class DateHeader : Header {

        public DateHeader(string name, System.DateTime dateTime)
            : base(name, Header.GetHeaderId(name, true)) {
            var type = Header.TypeFromHeaderId(this.HeaderId);
            if (this.HeaderId != HeaderId.Unknown && type != typeof (DateHeader))
                throw new System.ArgumentException(Resources.Strings.NameNotValidForThisHeaderType(name, "DateHeader", type.Name));
            this.SetRawValue(null, true);
            parsed = true;
            utcDateTime = dateTime.ToUniversalTime();
            if (dateTime.Kind == System.DateTimeKind.Utc)
                return;
            timeZoneOffset = System.TimeZoneInfo.Local.GetUtcOffset(dateTime);
        }

        public DateHeader(string name, System.DateTime dateTime, System.TimeSpan timeZoneOffset)
            : base(name, Header.GetHeaderId(name, true)) {
            var type = Header.TypeFromHeaderId(this.HeaderId);
            if (this.HeaderId != HeaderId.Unknown && type != typeof (DateHeader))
                throw new System.ArgumentException(Resources.Strings.NameNotValidForThisHeaderType(name, "DateHeader", type.Name));
            this.SetRawValue(null, true);
            parsed = true;
            utcDateTime = dateTime.ToUniversalTime();
            this.timeZoneOffset = timeZoneOffset;
        }

        internal DateHeader(string name, HeaderId headerId)
            : base(name, headerId) {}

        public override sealed string Value {
            get {
                return this.GetRawValue(false);
            }
            set {
                this.SetRawValue(value, true, false);
            }
        }

        public System.DateTime DateTime {
            get {
                if (!parsed)
                    this.Parse();
                if (!(utcDateTime == minDateTime))
                    return utcDateTime.ToLocalTime();
                return minDateTime;
            }
            set {
                this.SetRawValue(null, true);
                parsed = true;
                utcDateTime = value.ToUniversalTime();
                if (value.Kind != System.DateTimeKind.Utc)
                    timeZoneOffset = System.TimeZoneInfo.Local.GetUtcOffset(value);
                else
                    timeZoneOffset = System.TimeSpan.Zero;
            }
        }

        public System.DateTime UtcDateTime {
            get {
                if (!parsed)
                    this.Parse();
                return utcDateTime;
            }
        }

        public System.TimeSpan TimeZoneOffset {
            get {
                if (!parsed)
                    this.Parse();
                return timeZoneOffset;
            }
        }

        internal override byte[] RawValue {
            get {
                if (!parsed)
                    this.Parse();
                var characterCount = 0;
                return DateHeader.FormatValue(utcDateTime, timeZoneOffset, out characterCount) ?? MimeString.EmptyByteArray;
            }
            set {
                base.RawValue = value;
            }
        }

        public override sealed MimeNode Clone() {
            var dateHeader = new DateHeader(this.Name, this.HeaderId);
            this.CopyTo(dateHeader);
            return dateHeader;
        }

        public override sealed void CopyTo(object destination) {
            if (destination == null)
                throw new System.ArgumentNullException(nameof(destination));
            if (destination == this)
                return;
            var dateHeader = destination as DateHeader;
            if (dateHeader == null)
                throw new System.ArgumentException(Resources.Strings.CantCopyToDifferentObjectType);
            base.CopyTo(destination);
            dateHeader.parsed = parsed;
            dateHeader.utcDateTime = utcDateTime;
            dateHeader.timeZoneOffset = timeZoneOffset;
        }

        public override sealed bool IsValueValid(string value) {
            return Internal.ByteString.IsStringArgumentValid(value, false);
        }

        internal static long WriteDateHeaderValue(System.IO.Stream stream, System.DateTime utcDateTime, System.TimeSpan timeZoneOffset, ref MimeStringLength currentLineLength) {
            var num1 = 0L;
            var characterCount = 0;
            var buffer = DateHeader.FormatValue(utcDateTime, timeZoneOffset, out characterCount);
            stream.Write(MimeString.Space, 0, MimeString.Space.Length);
            var num2 = num1 + MimeString.Space.Length;
            currentLineLength.IncrementBy(MimeString.Space.Length);
            long num3;
            if (buffer != null) {
                stream.Write(buffer, 0, buffer.Length);
                num3 = num2 + buffer.Length;
                currentLineLength.IncrementBy(characterCount, buffer.Length);
            } else {
                stream.Write(MimeString.CommentInvalidDate, 0, MimeString.CommentInvalidDate.Length);
                num3 = num2 + MimeString.CommentInvalidDate.Length;
                currentLineLength.IncrementBy(MimeString.CommentInvalidDate.Length);
            }
            return num3 + Header.WriteLineEnd(stream, ref currentLineLength);
        }

        internal static System.DateTime ParseDateHeaderValue(string value) {
            var data = value == null ? MimeString.EmptyByteArray : Internal.ByteString.StringToBytes(value, false);
            System.DateTime utcDateTime;
            System.TimeSpan timeZoneOffset;
            DateHeader.ParseValue(new MimeStringList(data, 0, data.Length), out utcDateTime, out timeZoneOffset);
            return utcDateTime;
        }

        internal void SetValue(System.DateTime value, System.TimeSpan timeZoneOffset) {
            this.SetRawValue(null, true);
            parsed = true;
            utcDateTime = value.ToUniversalTime();
            this.timeZoneOffset = timeZoneOffset;
        }

        internal override void RawValueAboutToChange() {
            this.Reset();
        }

        internal override long WriteTo(System.IO.Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer) {
            var num1 = this.WriteName(stream, ref scratchBuffer);
            currentLineLength.IncrementBy((int) num1);
            if (!this.IsDirty && this.RawLength != 0 && this.IsProtected) {
                var num2 = Header.WriteLines(this.Lines, stream);
                var num3 = num1 + num2;
                currentLineLength.SetAs(0);
                return num3;
            }
            if (!parsed)
                this.Parse();
            var num4 = num1 + DateHeader.WriteDateHeaderValue(stream, utcDateTime, timeZoneOffset, ref currentLineLength);
            currentLineLength.SetAs(0);
            return num4;
        }

        internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild) {
            throw new System.NotSupportedException(Resources.Strings.CannotAddChildrenToMimeHeaderDate);
        }

        internal override void ForceParse() {
            if (parsed)
                return;
            this.Parse();
        }

        private static byte MakeUpper(byte ch) {
            return (int) ch < 97 || (int) ch > 122 ? ch : (byte) (ch - 97 + 65);
        }

        private static TimeZoneToken MapZoneToInt(byte[] ptr, int offset, int len) {
            switch (DateHeader.MakeUpper(ptr[offset++])) {
                case 67:
                    return --len == 0 || 68 != (int) DateHeader.MakeUpper(ptr[offset++]) ? TimeZoneToken.CST : TimeZoneToken.EST;
                case 69:
                    return --len == 0 || 68 != (int) DateHeader.MakeUpper(ptr[offset++]) ? TimeZoneToken.EST : TimeZoneToken.EDT;
                case 71:
                    return TimeZoneToken.GMT;
                case 74:
                    return TimeZoneToken.KST;
                case 75:
                    return TimeZoneToken.KST;
                case 77:
                    return --len == 0 || 68 != (int) DateHeader.MakeUpper(ptr[offset++]) ? TimeZoneToken.MST : TimeZoneToken.CST;
                case 80:
                    return --len == 0 || 68 != (int) DateHeader.MakeUpper(ptr[offset++]) ? TimeZoneToken.PST : TimeZoneToken.MST;
                case 85:
                    return TimeZoneToken.GMT;
                default:
                    return TimeZoneToken.GMT;
            }
        }

        private static int MapMonthToInt(byte[] ptr, int offset, int len) {
            var num = DateHeader.MakeUpper(ptr[offset++]);
            if (num <= 70U) {
                switch (num) {
                    case 65:
                        return --len == 0 || 85 != (int) DateHeader.MakeUpper(ptr[offset++]) ? 4 : 8;
                    case 68:
                        return 12;
                    case 70:
                        return 2;
                }
            } else {
                switch (num) {
                    case 74:
                        if (--len == 0 || 85 != DateHeader.MakeUpper(ptr[offset++]))
                            return 1;
                        return --len == 0 || 76 != (int) DateHeader.MakeUpper(ptr[offset++]) ? 6 : 7;
                    case 77:
                        return --len == 0 || (65 != (int) DateHeader.MakeUpper(ptr[offset++]) || --len == 0) || 89 != (int) DateHeader.MakeUpper(ptr[offset++]) ? 3 : 5;
                    case 78:
                        return 11;
                    case 79:
                        return 10;
                    case 83:
                        return 9;
                }
            }
            return 1;
        }

        private static void ParseValue(MimeStringList list, out System.DateTime utcDateTime, out System.TimeSpan timeZoneOffset) {
            var phrase = new MimeStringList();
            var valueParser = new ValueParser(list, false);
            var parseStage = ParseStage.DayOfWeek;
            var numArray = new int[8];
            byte num1 = 32;
            while (parseStage != ParseStage.Count) {
                valueParser.ParseCFWS(false, ref phrase, true);
                var ch = valueParser.ParseGet();
                if (ch != 0) {
                    if (!MimeScan.IsToken(ch) || ch == 45 || ch == 43) {
                        num1 = ch;
                        valueParser.ParseCFWS(false, ref phrase, true);
                    } else {
                        if (MimeScan.IsDigit(ch)) {
                            if (parseStage == ParseStage.DayOfWeek)
                                parseStage = ParseStage.DayOfMonth;
                            if (parseStage == ParseStage.Second && num1 != 58)
                                parseStage = ParseStage.Zone;
                            var num2 = 0;
                            do {
                                ++num2;
                                numArray[(int) parseStage] *= 10;
                                numArray[(int) parseStage] += ch - 48;
                                ch = valueParser.ParseGet();
                            } while (ch != 0 && MimeScan.IsDigit(ch));
                            if (ch != 0)
                                valueParser.ParseUnget();
                            if (parseStage == ParseStage.Year && num2 <= 2)
                                numArray[(int) parseStage] += numArray[(int) parseStage] < 30 ? 2000 : 1900;
                            if (parseStage == ParseStage.Zone && num2 <= 2)
                                numArray[(int) parseStage] *= 100;
                            if (parseStage == ParseStage.Zone && num1 == 45)
                                numArray[(int) parseStage] = -numArray[(int) parseStage];
                            ++parseStage;
                        } else if (MimeScan.IsAlpha(ch)) {
                            valueParser.ParseUnget();
                            var mimeString = valueParser.ParseToken(MimeScan.Token.Alpha);
                            if (parseStage == ParseStage.DayOfWeek)
                                parseStage = ParseStage.DayOfMonth;
                            else if (parseStage == ParseStage.Month) {
                                numArray[(int) parseStage] = DateHeader.MapMonthToInt(mimeString.Data, mimeString.Offset, mimeString.Length);
                                parseStage = ParseStage.Year;
                            } else if (parseStage >= ParseStage.Second) {
                                if (mimeString.Length == 2 && 65 == DateHeader.MakeUpper(mimeString[0]) && 77 == DateHeader.MakeUpper(mimeString[1])) {
                                    if (numArray[4] == 12)
                                        numArray[4] = 0;
                                    parseStage = ParseStage.Zone;
                                } else if (mimeString.Length == 2 && 80 == DateHeader.MakeUpper(mimeString[0]) && 77 == DateHeader.MakeUpper(mimeString[1])) {
                                    if (numArray[4] < 12)
                                        numArray[4] += 12;
                                    parseStage = ParseStage.Zone;
                                } else {
                                    numArray[7] = (int) DateHeader.MapZoneToInt(mimeString.Data, mimeString.Offset, mimeString.Length);
                                    parseStage = ParseStage.Count;
                                }
                            }
                        }
                        num1 = 32;
                    }
                } else
                    break;
            }
            if (parseStage > ParseStage.Year) {
                var year = numArray[3];
                var month = numArray[2];
                var day = numArray[1];
                var hour = numArray[4];
                var minute = numArray[5];
                var second = numArray[6];
                if (hour == 23 && minute == 59 && second == 60)
                    second = 59;
                if (year >= 1900 && year <= 9999 && (month >= 1 && month <= 12) && (day >= 1 && day <= System.DateTime.DaysInMonth(year, month) && (hour >= 0 && hour < 24)) && (minute >= 0 && minute < 60 && second >= 0)) {
                    if (second < 60) {
                        try {
                            utcDateTime = new System.DateTime(year, month, day, hour, minute, second, 0, System.DateTimeKind.Utc);
                            goto label_46;
                        } catch (System.ArgumentException ex) {
                            utcDateTime = minDateTime;
                            goto label_46;
                        }
                    }
                }
                utcDateTime = minDateTime;
            } else
                utcDateTime = minDateTime;
            label_46:
            if (parseStage == ParseStage.Count && utcDateTime > minDateTime) {
                var hours = numArray[7]/100;
                var minutes = numArray[7]%100;
                if (hours > 99 || hours < -99) {
                    hours = 0;
                    minutes = 0;
                }
                if (minutes > 59 || minutes < -59)
                    minutes = 0;
                timeZoneOffset = new System.TimeSpan(hours, minutes, 0);
                if (utcDateTime.Ticks >= timeZoneOffset.Ticks && System.DateTime.MaxValue.Ticks >= utcDateTime.Ticks - timeZoneOffset.Ticks)
                    utcDateTime -= timeZoneOffset;
                else {
                    utcDateTime = minDateTime;
                    timeZoneOffset = System.TimeSpan.Zero;
                }
            } else
                timeZoneOffset = System.TimeSpan.Zero;
        }

        private static byte[] FormatValue(System.DateTime utcDateTime, System.TimeSpan timeZoneOffset, out int characterCount) {
            if (minDateTime == utcDateTime) {
                characterCount = 0;
                return null;
            }
            var str1 = (utcDateTime + timeZoneOffset).ToString("ddd, d MMM yyyy HH:mm:ss ", System.Globalization.CultureInfo.InvariantCulture);
            var bytesOffset = Internal.ByteString.StringToBytesCount(str1, false);
            var str2 = (timeZoneOffset.Hours*100 + timeZoneOffset.Minutes).ToString("+0000;-0000");
            var num = Internal.ByteString.StringToBytesCount(str2, false);
            var bytes = new byte[bytesOffset + num];
            Internal.ByteString.StringToBytes(str1, bytes, 0, false);
            Internal.ByteString.StringToBytes(str2, bytes, bytesOffset, false);
            characterCount = str1.Length + str2.Length;
            return bytes;
        }

        private void Reset() {
            parsed = false;
            utcDateTime = minDateTime;
            timeZoneOffset = System.TimeSpan.Zero;
        }

        private void Parse() {
            parsed = true;
            DateHeader.ParseValue(this.Lines, out utcDateTime, out timeZoneOffset);
        }

        internal const bool AllowUTF8Value = false;
        private const int Y2KThreshold = 30;
        private static readonly System.DateTime minDateTime = System.DateTime.SpecifyKind(System.DateTime.MinValue, System.DateTimeKind.Utc);
        private bool parsed;
        private System.TimeSpan timeZoneOffset = System.TimeSpan.Zero;
        private System.DateTime utcDateTime = minDateTime;


        private enum TimeZoneToken {

            PST = -800,
            MST = -700,
            PDT = -700,
            CST = -600,
            MDT = -600,
            CDT = -500,
            EST = -500,
            EDT = -400,
            GMT = 0,
            JST = 900,
            KST = 900

        }


        private enum ParseStage {

            DayOfWeek,
            DayOfMonth,
            Month,
            Year,
            Hour,
            Minute,
            Second,
            Zone,
            Count

        }

    }

}