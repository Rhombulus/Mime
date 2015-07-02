// Decompiled with JetBrains decompiler
// Type: Microsoft.Exchange.Data.Mime.DateHeader
// Assembly: Microsoft.Exchange.Data.Common, Version=15.0.1040.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 60AF4FF7-547F-476B-8FAC-6C80D63CB41A
// Assembly location: C:\Users\Thomas\Downloads\Microsoft.Exchange.Data.Common.dll

using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Butler.Schema.Data.Mime
{
  public class DateHeader : Header
  {
    private static readonly DateTime minDateTime = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
    private DateTime utcDateTime = DateHeader.minDateTime;
    private TimeSpan timeZoneOffset = TimeSpan.Zero;
    internal const bool AllowUTF8Value = false;
    private const int Y2KThreshold = 30;
    private bool parsed;

    public override sealed string Value
    {
      get
      {
        return this.GetRawValue(false);
      }
      set
      {
        this.SetRawValue(value, true, false);
      }
    }

    public DateTime DateTime
    {
      get
      {
        if (!this.parsed)
          this.Parse();
        if (!(this.utcDateTime == DateHeader.minDateTime))
          return this.utcDateTime.ToLocalTime();
        return DateHeader.minDateTime;
      }
      set
      {
        this.SetRawValue((byte[]) null, true);
        this.parsed = true;
        this.utcDateTime = value.ToUniversalTime();
        if (value.Kind != DateTimeKind.Utc)
          this.timeZoneOffset = TimeZoneInfo.Local.GetUtcOffset(value);
        else
          this.timeZoneOffset = TimeSpan.Zero;
      }
    }

    public DateTime UtcDateTime
    {
      get
      {
        if (!this.parsed)
          this.Parse();
        return this.utcDateTime;
      }
    }

    public TimeSpan TimeZoneOffset
    {
      get
      {
        if (!this.parsed)
          this.Parse();
        return this.timeZoneOffset;
      }
    }

    internal override byte[] RawValue
    {
      get
      {
        if (!this.parsed)
          this.Parse();
        int characterCount = 0;
        return DateHeader.FormatValue(this.utcDateTime, this.timeZoneOffset, out characterCount) ?? MimeString.EmptyByteArray;
      }
      set
      {
        base.RawValue = value;
      }
    }

    public DateHeader(string name, DateTime dateTime)
      : base(name, Header.GetHeaderId(name, true))
    {
      Type type = Header.TypeFromHeaderId(this.HeaderId);
      if (this.HeaderId != HeaderId.Unknown && type != typeof (DateHeader))
        throw new ArgumentException(CtsResources.Strings.NameNotValidForThisHeaderType(name, "DateHeader", type.Name));
      this.SetRawValue((byte[]) null, true);
      this.parsed = true;
      this.utcDateTime = dateTime.ToUniversalTime();
      if (dateTime.Kind == DateTimeKind.Utc)
        return;
      this.timeZoneOffset = TimeZoneInfo.Local.GetUtcOffset(dateTime);
    }

    public DateHeader(string name, DateTime dateTime, TimeSpan timeZoneOffset)
      : base(name, Header.GetHeaderId(name, true))
    {
      Type type = Header.TypeFromHeaderId(this.HeaderId);
      if (this.HeaderId != HeaderId.Unknown && type != typeof (DateHeader))
        throw new ArgumentException(CtsResources.Strings.NameNotValidForThisHeaderType(name, "DateHeader", type.Name));
      this.SetRawValue((byte[]) null, true);
      this.parsed = true;
      this.utcDateTime = dateTime.ToUniversalTime();
      this.timeZoneOffset = timeZoneOffset;
    }

    internal DateHeader(string name, HeaderId headerId)
      : base(name, headerId)
    {
    }

    public override sealed MimeNode Clone()
    {
      DateHeader dateHeader = new DateHeader(this.Name, this.HeaderId);
      this.CopyTo((object) dateHeader);
      return (MimeNode) dateHeader;
    }

    public override sealed void CopyTo(object destination)
    {
      if (destination == null)
        throw new ArgumentNullException(nameof(destination));
      if (destination == this)
        return;
      DateHeader dateHeader = destination as DateHeader;
      if (dateHeader == null)
        throw new ArgumentException(CtsResources.Strings.CantCopyToDifferentObjectType);
      base.CopyTo(destination);
      dateHeader.parsed = this.parsed;
      dateHeader.utcDateTime = this.utcDateTime;
      dateHeader.timeZoneOffset = this.timeZoneOffset;
    }

    public override sealed bool IsValueValid(string value)
    {
      return Internal.ByteString.IsStringArgumentValid(value, false);
    }

    internal static long WriteDateHeaderValue(Stream stream, DateTime utcDateTime, TimeSpan timeZoneOffset, ref MimeStringLength currentLineLength)
    {
      long num1 = 0L;
      int characterCount = 0;
      byte[] buffer = DateHeader.FormatValue(utcDateTime, timeZoneOffset, out characterCount);
      stream.Write(MimeString.Space, 0, MimeString.Space.Length);
      long num2 = num1 + (long) MimeString.Space.Length;
      currentLineLength.IncrementBy(MimeString.Space.Length);
      long num3;
      if (buffer != null)
      {
        stream.Write(buffer, 0, buffer.Length);
        num3 = num2 + (long) buffer.Length;
        currentLineLength.IncrementBy(characterCount, buffer.Length);
      }
      else
      {
        stream.Write(MimeString.CommentInvalidDate, 0, MimeString.CommentInvalidDate.Length);
        num3 = num2 + (long) MimeString.CommentInvalidDate.Length;
        currentLineLength.IncrementBy(MimeString.CommentInvalidDate.Length);
      }
      return num3 + Header.WriteLineEnd(stream, ref currentLineLength);
    }

    internal static DateTime ParseDateHeaderValue(string value)
    {
      byte[] data = value == null ? MimeString.EmptyByteArray : Internal.ByteString.StringToBytes(value, false);
      DateTime utcDateTime;
      TimeSpan timeZoneOffset;
      DateHeader.ParseValue(new MimeStringList(data, 0, data.Length), out utcDateTime, out timeZoneOffset);
      return utcDateTime;
    }

    internal void SetValue(DateTime value, TimeSpan timeZoneOffset)
    {
      this.SetRawValue((byte[]) null, true);
      this.parsed = true;
      this.utcDateTime = value.ToUniversalTime();
      this.timeZoneOffset = timeZoneOffset;
    }

    internal override void RawValueAboutToChange()
    {
      this.Reset();
    }

    internal override long WriteTo(Stream stream, EncodingOptions encodingOptions, MimeOutputFilter filter, ref MimeStringLength currentLineLength, ref byte[] scratchBuffer)
    {
      long num1 = this.WriteName(stream, ref scratchBuffer);
      currentLineLength.IncrementBy((int) num1);
      if (!this.IsDirty && this.RawLength != 0 && this.IsProtected)
      {
        long num2 = Header.WriteLines(this.Lines, stream);
        long num3 = num1 + num2;
        currentLineLength.SetAs(0);
        return num3;
      }
      if (!this.parsed)
        this.Parse();
      long num4 = num1 + DateHeader.WriteDateHeaderValue(stream, this.utcDateTime, this.timeZoneOffset, ref currentLineLength);
      currentLineLength.SetAs(0);
      return num4;
    }

    internal override MimeNode ValidateNewChild(MimeNode newChild, MimeNode refChild)
    {
      throw new NotSupportedException(CtsResources.Strings.CannotAddChildrenToMimeHeaderDate);
    }

    internal override void ForceParse()
    {
      if (this.parsed)
        return;
      this.Parse();
    }

    private static byte MakeUpper(byte ch)
    {
      return (int) ch < 97 || (int) ch > 122 ? ch : (byte) ((int) ch - 97 + 65);
    }

    private static DateHeader.TimeZoneToken MapZoneToInt(byte[] ptr, int offset, int len)
    {
      switch (DateHeader.MakeUpper(ptr[offset++]))
      {
        case (byte) 67:
          return --len == 0 || 68 != (int) DateHeader.MakeUpper(ptr[offset++]) ? DateHeader.TimeZoneToken.CST : DateHeader.TimeZoneToken.EST;
        case (byte) 69:
          return --len == 0 || 68 != (int) DateHeader.MakeUpper(ptr[offset++]) ? DateHeader.TimeZoneToken.EST : DateHeader.TimeZoneToken.EDT;
        case (byte) 71:
          return DateHeader.TimeZoneToken.GMT;
        case (byte) 74:
          return DateHeader.TimeZoneToken.KST;
        case (byte) 75:
          return DateHeader.TimeZoneToken.KST;
        case (byte) 77:
          return --len == 0 || 68 != (int) DateHeader.MakeUpper(ptr[offset++]) ? DateHeader.TimeZoneToken.MST : DateHeader.TimeZoneToken.CST;
        case (byte) 80:
          return --len == 0 || 68 != (int) DateHeader.MakeUpper(ptr[offset++]) ? DateHeader.TimeZoneToken.PST : DateHeader.TimeZoneToken.MST;
        case (byte) 85:
          return DateHeader.TimeZoneToken.GMT;
        default:
          return DateHeader.TimeZoneToken.GMT;
      }
    }

    private static int MapMonthToInt(byte[] ptr, int offset, int len)
    {
      byte num = DateHeader.MakeUpper(ptr[offset++]);
      if ((uint) num <= 70U)
      {
        switch (num)
        {
          case (byte) 65:
            return --len == 0 || 85 != (int) DateHeader.MakeUpper(ptr[offset++]) ? 4 : 8;
          case (byte) 68:
            return 12;
          case (byte) 70:
            return 2;
        }
      }
      else
      {
        switch (num)
        {
          case (byte) 74:
            if (--len == 0 || 85 != (int) DateHeader.MakeUpper(ptr[offset++]))
              return 1;
            return --len == 0 || 76 != (int) DateHeader.MakeUpper(ptr[offset++]) ? 6 : 7;
          case (byte) 77:
            return --len == 0 || (65 != (int) DateHeader.MakeUpper(ptr[offset++]) || --len == 0) || 89 != (int) DateHeader.MakeUpper(ptr[offset++]) ? 3 : 5;
          case (byte) 78:
            return 11;
          case (byte) 79:
            return 10;
          case (byte) 83:
            return 9;
        }
      }
      return 1;
    }

    private static void ParseValue(MimeStringList list, out DateTime utcDateTime, out TimeSpan timeZoneOffset)
    {
      MimeStringList phrase = new MimeStringList();
      ValueParser valueParser = new ValueParser(list, false);
      DateHeader.ParseStage parseStage = DateHeader.ParseStage.DayOfWeek;
      int[] numArray = new int[8];
      byte num1 = (byte) 32;
      while (parseStage != DateHeader.ParseStage.Count)
      {
        valueParser.ParseCFWS(false, ref phrase, true);
        byte ch = valueParser.ParseGet();
        if ((int) ch != 0)
        {
          if (!MimeScan.IsToken(ch) || (int) ch == 45 || (int) ch == 43)
          {
            num1 = ch;
            valueParser.ParseCFWS(false, ref phrase, true);
          }
          else
          {
            if (MimeScan.IsDigit(ch))
            {
              if (parseStage == DateHeader.ParseStage.DayOfWeek)
                parseStage = DateHeader.ParseStage.DayOfMonth;
              if (parseStage == DateHeader.ParseStage.Second && (int) num1 != 58)
                parseStage = DateHeader.ParseStage.Zone;
              int num2 = 0;
              do
              {
                ++num2;
                numArray[(int) parseStage] *= 10;
                numArray[(int) parseStage] += (int) ch - 48;
                ch = valueParser.ParseGet();
              }
              while ((int) ch != 0 && MimeScan.IsDigit(ch));
              if ((int) ch != 0)
                valueParser.ParseUnget();
              if (parseStage == DateHeader.ParseStage.Year && num2 <= 2)
                numArray[(int) parseStage] += numArray[(int) parseStage] < 30 ? 2000 : 1900;
              if (parseStage == DateHeader.ParseStage.Zone && num2 <= 2)
                numArray[(int) parseStage] *= 100;
              if (parseStage == DateHeader.ParseStage.Zone && (int) num1 == 45)
                numArray[(int) parseStage] = -numArray[(int) parseStage];
              ++parseStage;
            }
            else if (MimeScan.IsAlpha(ch))
            {
              valueParser.ParseUnget();
              MimeString mimeString = valueParser.ParseToken(MimeScan.Token.Alpha);
              if (parseStage == DateHeader.ParseStage.DayOfWeek)
                parseStage = DateHeader.ParseStage.DayOfMonth;
              else if (parseStage == DateHeader.ParseStage.Month)
              {
                numArray[(int) parseStage] = DateHeader.MapMonthToInt(mimeString.Data, mimeString.Offset, mimeString.Length);
                parseStage = DateHeader.ParseStage.Year;
              }
              else if (parseStage >= DateHeader.ParseStage.Second)
              {
                if (mimeString.Length == 2 && 65 == (int) DateHeader.MakeUpper(mimeString[0]) && 77 == (int) DateHeader.MakeUpper(mimeString[1]))
                {
                  if (numArray[4] == 12)
                    numArray[4] = 0;
                  parseStage = DateHeader.ParseStage.Zone;
                }
                else if (mimeString.Length == 2 && 80 == (int) DateHeader.MakeUpper(mimeString[0]) && 77 == (int) DateHeader.MakeUpper(mimeString[1]))
                {
                  if (numArray[4] < 12)
                    numArray[4] += 12;
                  parseStage = DateHeader.ParseStage.Zone;
                }
                else
                {
                  numArray[7] = (int) DateHeader.MapZoneToInt(mimeString.Data, mimeString.Offset, mimeString.Length);
                  parseStage = DateHeader.ParseStage.Count;
                }
              }
            }
            num1 = (byte) 32;
          }
        }
        else
          break;
      }
      if (parseStage > DateHeader.ParseStage.Year)
      {
        int year = numArray[3];
        int month = numArray[2];
        int day = numArray[1];
        int hour = numArray[4];
        int minute = numArray[5];
        int second = numArray[6];
        if (hour == 23 && minute == 59 && second == 60)
          second = 59;
        if (year >= 1900 && year <= 9999 && (month >= 1 && month <= 12) && (day >= 1 && day <= DateTime.DaysInMonth(year, month) && (hour >= 0 && hour < 24)) && (minute >= 0 && minute < 60 && second >= 0))
        {
          if (second < 60)
          {
            try
            {
              utcDateTime = new DateTime(year, month, day, hour, minute, second, 0, DateTimeKind.Utc);
              goto label_46;
            }
            catch (ArgumentException ex)
            {
              utcDateTime = DateHeader.minDateTime;
              goto label_46;
            }
          }
        }
        utcDateTime = DateHeader.minDateTime;
      }
      else
        utcDateTime = DateHeader.minDateTime;
label_46:
      if (parseStage == DateHeader.ParseStage.Count && utcDateTime > DateHeader.minDateTime)
      {
        int hours = numArray[7] / 100;
        int minutes = numArray[7] % 100;
        if (hours > 99 || hours < -99)
        {
          hours = 0;
          minutes = 0;
        }
        if (minutes > 59 || minutes < -59)
          minutes = 0;
        timeZoneOffset = new TimeSpan(hours, minutes, 0);
        if (utcDateTime.Ticks >= timeZoneOffset.Ticks && DateTime.MaxValue.Ticks >= utcDateTime.Ticks - timeZoneOffset.Ticks)
        {
          utcDateTime -= timeZoneOffset;
        }
        else
        {
          utcDateTime = DateHeader.minDateTime;
          timeZoneOffset = TimeSpan.Zero;
        }
      }
      else
        timeZoneOffset = TimeSpan.Zero;
    }

    private static byte[] FormatValue(DateTime utcDateTime, TimeSpan timeZoneOffset, out int characterCount)
    {
      if (DateHeader.minDateTime == utcDateTime)
      {
        characterCount = 0;
        return (byte[]) null;
      }
      string str1 = (utcDateTime + timeZoneOffset).ToString("ddd, d MMM yyyy HH:mm:ss ", (IFormatProvider) CultureInfo.InvariantCulture);
      int bytesOffset = Internal.ByteString.StringToBytesCount(str1, false);
      string str2 = (timeZoneOffset.Hours * 100 + timeZoneOffset.Minutes).ToString("+0000;-0000");
      int num = Internal.ByteString.StringToBytesCount(str2, false);
      byte[] bytes = new byte[bytesOffset + num];
      Internal.ByteString.StringToBytes(str1, bytes, 0, false);
      Internal.ByteString.StringToBytes(str2, bytes, bytesOffset, false);
      characterCount = str1.Length + str2.Length;
      return bytes;
    }

    private void Reset()
    {
      this.parsed = false;
      this.utcDateTime = DateHeader.minDateTime;
      this.timeZoneOffset = TimeSpan.Zero;
    }

    private void Parse()
    {
      this.parsed = true;
      DateHeader.ParseValue(this.Lines, out this.utcDateTime, out this.timeZoneOffset);
    }

    private enum TimeZoneToken
    {
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
      KST = 900,
    }

    private enum ParseStage
    {
      DayOfWeek,
      DayOfMonth,
      Month,
      Year,
      Hour,
      Minute,
      Second,
      Zone,
      Count,
    }
  }
}
