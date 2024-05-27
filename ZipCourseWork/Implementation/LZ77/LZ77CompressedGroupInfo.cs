using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.LZ77
{
    public class LZ77Letter
    {
        public string Letter { get; }
        public byte[] Bytes { get;}
        public byte FByte { get; }
        public byte SByte { get; }

        public LZ77Letter(byte fByte, byte sByte)
        {
            Letter = $"{ByteToString(fByte)}-{ByteToString(sByte)}";
            Bytes = [fByte, sByte];
            FByte = fByte;
            SByte = sByte;
        }

        private string ByteToString(byte value)
        {
            var intValue = (int)value;
            return intValue.ToString("D3");
        }
    }

    public class LZ77CompressedGroupInfo
    {
        public bool IsCompressed { get; }
        public byte Data { get; }
        public int Index { get; }
        public int Size { get; }
        public string Buffer { get; private set; }

        public LZ77CompressedGroupInfo(bool isCompressed, byte data)
        {
            IsCompressed = isCompressed;
            Data = data;
        }

        public LZ77CompressedGroupInfo(bool isCompressed, byte data, int index, int size, string buffer)
        {
            IsCompressed = isCompressed;
            Data = data;
            Index = index;
            Size = size;
            Buffer = buffer;
        }
    }
}
