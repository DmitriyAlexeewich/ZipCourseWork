using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.RLE
{
    public class RLEByteListInfo
    {
        public byte LastByte { get { return _bytes.Last(); } }
        public bool IsSingle { get { return _bytes.Count == 1 || _bytes[0] == _bytes[1]; } }
        public bool HasAny { get { return !_bytes.IsNullOrEmpty(); } }

        private const int _maxCount = 127;

        private List<byte> _bytes;

        public RLEByteListInfo(byte source)
        {
            _bytes = new List<byte>() { source };
        }

        public RLEByteListInfo()
        {
            _bytes = new List<byte>();
        }

        public bool TryAdd(byte source)
        {
            if (_bytes.Count < 2)
            {
                _bytes.Add(source);
                return true;
            }

            if (IsSingle)
            {
                if (_bytes.Count >= _maxCount)
                    return false;

                if (_bytes[0] != source)
                    return false;

                _bytes.Add(source);
                return true;
            }

            if(_bytes.Count >= _maxCount)
                return false;

            if (_bytes.Last() == source)
                return false;

            _bytes.Add(source);
            return true;
        }

        public List<byte> Compress(bool isRemoveLast = false) => IsSingle ? CompressSingle() : CompressMultiplie(isRemoveLast);

        private List<byte> CompressSingle()
        {
            var result = new List<byte>();

            var infoByte = new List<bool>();

            infoByte.Add(true);
            infoByte.AddRange(_bytes.Count.GetBits().Take(7));

            result.Add(infoByte.GetByte());
            result.Add(_bytes[0]);

            return result;
        }

        private List<byte> CompressMultiplie(bool isRemoveLast)
        {
            if (isRemoveLast)
                _bytes.RemoveAt(_bytes.Count - 1);

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
