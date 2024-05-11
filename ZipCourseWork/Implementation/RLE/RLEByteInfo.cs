using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.RLE
{
    public abstract class RLEByteInfo
    {
        protected List<byte> _bytes;
        protected const int _maxCount = 127;

        public abstract bool TryAdd(byte source);

        public abstract List<byte> Compress();

        public RLEByteInfo(byte source)
        {
            _bytes = new List<byte>() { source };
        }
    }

    public class RLESingleBytesInfo : RLEByteInfo
    {
        public RLESingleBytesInfo(byte source) : base(source) { }

        private int _length = 1;

        public override bool TryAdd(byte source)
        {
            if (_bytes[0] != source || _length >= _maxCount)
                return false;

            _length++;
            return true;
        }

        public override List<byte> Compress()
        {
            var result = new List<byte>();

            var infoByte = new List<bool>();

            infoByte.Add(true);
            infoByte.AddRange(_length.GetBits().Take(7));

            result.Add(infoByte.GetByte());
            result.Add(_bytes[0]);

            return result;
        }
    }

    public class RLEMultipleBytesInfo : RLEByteInfo
    {
        public RLEMultipleBytesInfo(byte source) : base(source) { }

        public override bool TryAdd(byte source)
        {
            if (_bytes.Last() == source || _bytes.Count >= _maxCount)
                return false;

            _bytes.Add(source);
            return true;
        }

        public override List<byte> Compress()
        {
            var result = new List<byte>();

            var infoByte = new List<bool>();

            infoByte.Add(false);
            infoByte.AddRange(_bytes.Count.GetBits().Take(7));

            result.Add(infoByte.GetByte());
            _bytes.ForEach(x => result.Add(x));

            return result;
        }
    }
}
