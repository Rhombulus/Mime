using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Butler.Schema.Data.TextConverters
{

    [StructLayout(LayoutKind.Sequential)]
    internal struct HashCode
    {
        private int hash1;
        private int hash2;
        private int offset;
        public HashCode(bool ignore)
        {
            this.offset = 0;
            this.hash1 = this.hash2 = 0x1505;
        }

        public static int CalculateEmptyHash()
        {
            return 0x162a16fe;
        }

        public static unsafe int Calculate(string obj)
        {
            int num = 0x1505;
            int num2 = num;
            fixed (char* str = (obj))
            {
                char* chPtr2 = str;
                for (int i = obj.Length; i > 0; i -= 2)
                {
                    num = ((num << 5) + num) ^ chPtr2[0];
                    if (i < 2)
                    {
                        goto Label_004E;
                    }
                    num2 = ((num2 << 5) + num2) ^ chPtr2[1];
                    chPtr2 += 2;
                }
            }
            Label_004E:;
            return (num + (num2 * 0x5d588b65));
        }

        public static unsafe int Calculate(BufferString obj)
        {
            int num = 0x1505;
            int num2 = num;
            fixed (char* chRef = obj.Buffer)
            {
                char* chPtr = chRef + obj.Offset;
                for (int i = obj.Length; i > 0; i -= 2)
                {
                    num = ((num << 5) + num) ^ chPtr[0];
                    if (i == 1)
                    {
                        goto Label_006B;
                    }
                    num2 = ((num2 << 5) + num2) ^ chPtr[1];
                    chPtr += 2;
                }
            }
            Label_006B:;
            return (num + (num2 * 0x5d588b65));
        }

        public static unsafe int CalculateLowerCase(string obj)
        {
            int num = 0x1505;
            int num2 = num;
            fixed (char* str = (obj))
            {
                char* chPtr2 = str;
                for (int i = obj.Length; i > 0; i -= 2)
                {
                    num = ((num << 5) + num) ^ ParseSupport.ToLowerCase(chPtr2[0]);
                    if (i == 1)
                    {
                        goto Label_0058;
                    }
                    num2 = ((num2 << 5) + num2) ^ ParseSupport.ToLowerCase(chPtr2[1]);
                    chPtr2 += 2;
                }
            }
            Label_0058:;
            return (num + (num2 * 0x5d588b65));
        }

        public static unsafe int CalculateLowerCase(BufferString obj)
        {
            int num = 0x1505;
            int num2 = num;
            fixed (char* chRef = obj.Buffer)
            {
                char* chPtr = chRef + obj.Offset;
                for (int i = obj.Length; i > 0; i -= 2)
                {
                    num = ((num << 5) + num) ^ ParseSupport.ToLowerCase(chPtr[0]);
                    if (i == 1)
                    {
                        goto Label_0075;
                    }
                    num2 = ((num2 << 5) + num2) ^ ParseSupport.ToLowerCase(chPtr[1]);
                    chPtr += 2;
                }
            }
            Label_0075:;
            return (num + (num2 * 0x5d588b65));
        }

        public static unsafe int Calculate(char[] buffer, int offset, int length)
        {
            int num = 0x1505;
            int num2 = num;
            HashCode.CheckArgs(buffer, offset, length);
            fixed (char* chRef = buffer)
            {
                char* chPtr = chRef + offset;
                while (length > 0)
                {
                    num = ((num << 5) + num) ^ chPtr[0];
                    if (length == 1)
                    {
                        goto Label_005B;
                    }
                    num2 = ((num2 << 5) + num2) ^ chPtr[1];
                    chPtr += 2;
                    length -= 2;
                }
            }
            Label_005B:;
            return (num + (num2 * 0x5d588b65));
        }

        public static unsafe int CalculateLowerCase(char[] buffer, int offset, int length)
        {
            int num = 0x1505;
            int num2 = num;
            HashCode.CheckArgs(buffer, offset, length);
            fixed (char* chRef = buffer)
            {
                char* chPtr = chRef + offset;
                while (length > 0)
                {
                    num = ((num << 5) + num) ^ ParseSupport.ToLowerCase(chPtr[0]);
                    if (length == 1)
                    {
                        goto Label_0065;
                    }
                    num2 = ((num2 << 5) + num2) ^ ParseSupport.ToLowerCase(chPtr[1]);
                    chPtr += 2;
                    length -= 2;
                }
            }
            Label_0065:;
            return (num + (num2 * 0x5d588b65));
        }

        public void Initialize()
        {
            this.offset = 0;
            this.hash1 = this.hash2 = 0x1505;
        }

        public unsafe void Advance(char* s, int len)
        {
            if ((this.offset & 1) != 0)
            {
                this.hash2 = ((this.hash2 << 5) + this.hash2) ^ s[0];
                s++;
                len--;
                this.offset++;
            }
            this.offset += len;
            while (len > 0)
            {
                this.hash1 = ((this.hash1 << 5) + this.hash1) ^ s[0];
                if (len == 1)
                {
                    return;
                }
                this.hash2 = ((this.hash2 << 5) + this.hash2) ^ s[1];
                s += 2;
                len -= 2;
            }
        }

        public unsafe void AdvanceLowerCase(char* s, int len)
        {
            if ((this.offset & 1) != 0)
            {
                this.hash2 = ((this.hash2 << 5) + this.hash2) ^ ParseSupport.ToLowerCase(s[0]);
                s++;
                len--;
                this.offset++;
            }
            this.offset += len;
            while (len > 0)
            {
                this.hash1 = ((this.hash1 << 5) + this.hash1) ^ ParseSupport.ToLowerCase(s[0]);
                if (len == 1)
                {
                    return;
                }
                this.hash2 = ((this.hash2 << 5) + this.hash2) ^ ParseSupport.ToLowerCase(s[1]);
                s += 2;
                len -= 2;
            }
        }

        public void Advance(int ucs32)
        {
            if (ucs32 >= 0x10000)
            {
                char ch = ParseSupport.LowSurrogateCharFromUcs4(ucs32);
                char ch2 = ParseSupport.LowSurrogateCharFromUcs4(ucs32);
                if (((this.offset += 2) & 1) == 0)
                {
                    this.hash1 = ((this.hash1 << 5) + this.hash1) ^ ch;
                    this.hash2 = ((this.hash2 << 5) + this.hash2) ^ ch2;
                }
                else
                {
                    this.hash2 = ((this.hash2 << 5) + this.hash2) ^ ch;
                    this.hash1 = ((this.hash1 << 5) + this.hash1) ^ ch2;
                }
            }
            else if ((this.offset++ & 1) == 0)
            {
                this.hash1 = ((this.hash1 << 5) + this.hash1) ^ ucs32;
            }
            else
            {
                this.hash2 = ((this.hash2 << 5) + this.hash2) ^ ucs32;
            }
        }

        public void AdvanceLowerCase(int ucs32)
        {
            if (ucs32 >= 0x10000)
            {
                char ch = ParseSupport.LowSurrogateCharFromUcs4(ucs32);
                char ch2 = ParseSupport.LowSurrogateCharFromUcs4(ucs32);
                if (((this.offset += 2) & 1) == 0)
                {
                    this.hash1 = ((this.hash1 << 5) + this.hash1) ^ ch;
                    this.hash2 = ((this.hash2 << 5) + this.hash2) ^ ch2;
                }
                else
                {
                    this.hash2 = ((this.hash2 << 5) + this.hash2) ^ ch;
                    this.hash1 = ((this.hash1 << 5) + this.hash1) ^ ch2;
                }
            }
            else
            {
                this.AdvanceLowerCase((char)ucs32);
            }
        }

        public void Advance(char c)
        {
            if ((this.offset++ & 1) == 0)
            {
                this.hash1 = ((this.hash1 << 5) + this.hash1) ^ c;
            }
            else
            {
                this.hash2 = ((this.hash2 << 5) + this.hash2) ^ c;
            }
        }

        public int AdvanceAndFinalizeHash(char c)
        {
            if ((this.offset++ & 1) == 0)
            {
                this.hash1 = ((this.hash1 << 5) + this.hash1) ^ c;
            }
            else
            {
                this.hash2 = ((this.hash2 << 5) + this.hash2) ^ c;
            }
            return (this.hash1 + (this.hash2 * 0x5d588b65));
        }

        public void AdvanceLowerCase(char c)
        {
            if ((this.offset++ & 1) == 0)
            {
                this.hash1 = ((this.hash1 << 5) + this.hash1) ^ ParseSupport.ToLowerCase(c);
            }
            else
            {
                this.hash2 = ((this.hash2 << 5) + this.hash2) ^ ParseSupport.ToLowerCase(c);
            }
        }

        public unsafe void Advance(BufferString obj)
        {
            fixed (char* chRef = obj.Buffer)
            {
                this.Advance(chRef + obj.Offset, obj.Length);
            }
        }

        public unsafe void AdvanceLowerCase(BufferString obj)
        {
            fixed (char* chRef = obj.Buffer)
            {
                this.AdvanceLowerCase(chRef + obj.Offset, obj.Length);
            }
        }

        public unsafe void Advance(char[] buffer, int offset, int length)
        {
            HashCode.CheckArgs(buffer, offset, length);
            fixed (char* chRef = buffer)
            {
                this.Advance(chRef + offset, length);
            }
        }

        public unsafe void AdvanceLowerCase(char[] buffer, int offset, int length)
        {
            HashCode.CheckArgs(buffer, offset, length);
            fixed (char* chRef = buffer)
            {
                this.AdvanceLowerCase(chRef + offset, length);
            }
        }

        private static void CheckArgs(char[] buffer, int offset, int length)
        {
            int num = buffer.Length;
            if ((offset < 0) || (offset > num))
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length");
            }
            if (((offset + length) < offset) || ((offset + length) > num))
            {
                throw new ArgumentOutOfRangeException("offset + length");
            }
        }

        public int FinalizeHash()
        {
            return (this.hash1 + (this.hash2 * 0x5d588b65));
        }
    }
}

