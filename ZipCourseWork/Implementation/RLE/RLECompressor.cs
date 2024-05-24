using System.Text.Json;
using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.RLE
{
    public class RLECompressor
    {
        private List<byte> _bytes = new List<byte>();
        private RLEByteListInfo _bytesInfo;

        public void AddToCompress(byte source)
        {
            if (_bytesInfo is null)
            {
                _bytesInfo = new RLEByteListInfo(source);
                return;
            }

            if (_bytesInfo.TryAdd(source))
                return;

            if (!_bytesInfo.IsSingle && _bytesInfo.LastByte == source)
            {
                _bytes.AddRange(_bytesInfo.Compress(true));
                _bytesInfo = new RLEByteListInfo(source);
                _bytesInfo.TryAdd(source);
                return;
            }

            _bytes.AddRange(_bytesInfo.Compress());
            _bytesInfo = new RLEByteListInfo(source);
        }

        public byte[] Compress()
        {
            if(_bytesInfo != null && _bytesInfo.HasAny)
                _bytes.AddRange(_bytesInfo.Compress());

            return _bytes.ToArray();
        }

        public void AddToUncompress(byte source) => _bytes.Add(source);

        public byte[] UnCompress()
        {
            if (_bytes.IsNullOrEmpty())
                throw new NullReferenceException(nameof(_bytes));

            var result = new List<byte>();

            while (_bytes.Count > 0)
            {
                var infoBits = _bytes[0].GetBits();

                var isSingle = infoBits[0];
                var count = CalculateCount(infoBits, isSingle);
                _bytes.RemoveAt(0);

                for (var i = 0; i < count; i++)
                {
                    if (isSingle)
                    {
                        result.Add(_bytes[0]);
                        continue;
                    }

                    result.Add(_bytes[0]);
                    _bytes.RemoveAt(0);
                }

                if (isSingle)
                    _bytes.RemoveAt(0);
            }

            return result.ToArray();
        }

        private int CalculateCount(bool[] infoBits, bool isSingle)
        {
            var bits = new List<bool>();

            for (var i = 1; i < 8; i++)
                bits.Add(infoBits[i]);

            bits.Add(false);

            int count = bits.GetByte();

            return count;
        }
    }
}
