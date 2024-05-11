using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.RLE
{
    public abstract class RLEByteInfo
    {
        protected List<char> _letters;

        public abstract bool TryAdd(char lettter);

        public abstract List<byte> Compress();

        public RLEByteInfo(char lettter)
        {
            _letters = new List<char>() { lettter };
        }
    }

    public class RLESingleBytesInfo : RLEByteInfo
    {
        public RLESingleBytesInfo(char lettter) : base(lettter) { }

        private int _length = 1;
        private const int _maxCount = 129;

        public override bool TryAdd(char lettter)
        {
            if (_letters[0] != lettter || _length >= _maxCount)
                return false;

            _length++;
            return true;
        }

        public override List<byte> Compress()
        {
            var result = new List<byte>();

            var infoByte = new List<bool>();

            infoByte.Add(true);
            infoByte.AddRange(_maxCount.GetBits().Take(7));

            result.Add(infoByte.GetByte());

            result.AddRange(BitConverter.GetBytes(_letters[0]));

            return result;
        }
    }

    public class RLEMultipleBytesInfo : RLEByteInfo
    {
        private const int _maxCount = 128;

        public RLEMultipleBytesInfo(char lettter) : base(lettter) { }

        public override bool TryAdd(char lettter)
        {
            if (_letters.Last() == lettter || _letters.Count >= _maxCount)
                return false;

            _letters.Add(lettter);
            return true;
        }

        public override List<byte> Compress()
        {
            var result = new List<byte>();

            var infoByte = new List<bool>();

            infoByte.Add(false);
            infoByte.AddRange(_maxCount.GetBits().Take(7));

            _letters.ForEach(x =>
            {
                result.AddRange(BitConverter.GetBytes(x));
            });

            return result;
        }
    }
}
