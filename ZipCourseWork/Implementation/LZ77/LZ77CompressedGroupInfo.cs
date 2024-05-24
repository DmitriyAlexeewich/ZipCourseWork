namespace ZipCourseWork.Implementation.LZ77
{
    public class LZ77CompressedGroupInfo
    {
        public bool IsCompressed { get; }
        public byte Data { get; }

        public LZ77CompressedGroupInfo(bool isCompressed, byte data)
        {
            IsCompressed = isCompressed;
            Data = data;
        }
    }
}
