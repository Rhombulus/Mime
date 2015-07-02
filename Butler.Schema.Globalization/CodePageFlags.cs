namespace Butler.Schema.Globalization {

    [System.Flags]
    internal enum CodePageFlags : byte {

        None = 0,
        Detectable = (byte) 1,
        SevenBit = (byte) 2

    }

}