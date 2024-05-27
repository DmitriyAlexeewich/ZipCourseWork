using System.Collections;
using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.LZ77
{
    public class LZ77Uncompressor
    {
        private readonly Range _distanceRange = new Range(1, 6);
        private readonly Range _bufferRange = new Range(6, 8);

        private List<byte> _result = new List<byte>();
        private List<byte> _deltaGroup = new List<byte>();

        public void Add(byte source)
        {
            if (_deltaGroup.Count < 9)
            {
                _deltaGroup.Add(source);
                return;
            }

            UncompressDeltaGroup();

            _deltaGroup = new List<byte>() { source };
        }

        private void UncompressDeltaGroup()
        {
            var info = _deltaGroup[0].GetBits();

            for (var i = 1; i < _deltaGroup.Count; i++)
            {
                if (!info[i - 1])
                {
                    _result.Add(_deltaGroup[i]);
                    continue;
                }

                var bits = _deltaGroup[i].GetBits();

                var distance = (ParseDistance(bits.Take(_distanceRange).ToArray()) * 2);
                var bufferLength = ParseBuffer(bits.Take(_bufferRange).ToArray()) * 2;
                var startIndex = _result.Count - distance - bufferLength;

                var bufferBytes = new List<byte>();

                for (var j = startIndex; bufferBytes.Count < bufferLength; j++)
                    bufferBytes.Add(_result[j]);

                _result.AddRange(bufferBytes);
            }
        }

        private int ParseDistance(bool[] source)
        {
            var distanceBits = new BitArray(source);

            var distanceBytes = new byte[1];
            distanceBits.CopyTo(distanceBytes, 0);

            return distanceBytes[0];
        }
        
        private int ParseBuffer(bool[] source)
        {
            if (source[0])
            {
                if (source[1])
                    return 5;
                else
                    return 4;
            }
            else
            {
                if (source[1])
                    return 3;
                else
                    return 2;
            }
        }

        public byte[] Uncompress()
        {
            if (!_deltaGroup.IsNullOrEmpty())
                UncompressDeltaGroup();

            return _result.ToArray();
        }
    }
}
