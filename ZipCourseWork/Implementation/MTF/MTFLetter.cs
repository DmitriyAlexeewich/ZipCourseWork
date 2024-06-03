namespace ZipCourseWork.Implementation.MTF
{
    public class MTFLetter
    {
        public string Letter { get; }
        public byte[] Bytes { get; }

        public MTFLetter(byte fByte, byte sByte)
        {
            Letter = $"{ByteToString(fByte)}-{ByteToString(sByte)}";
            Bytes = [fByte, sByte];
        }

        private string ByteToString(byte value)
        {
            var intValue = (int)value;
            return intValue.ToString("D3");
        }
    }
}
