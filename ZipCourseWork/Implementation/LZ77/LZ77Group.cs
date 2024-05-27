using System.Collections;
using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.LZ77
{
    public class LZ77GroupInfo
    {
        public List<LZ77Letter> FullText { get { return _buffer.Union(_text).ToList(); } }
        public List<LZ77Letter> ExtraText { get { return _extraText ?? Array.Empty<LZ77Letter>().ToList(); } }
        public bool HasRepeats { get; private set; }
        public bool HasText { get { return !_buffer.IsNullOrEmpty() || !_text.IsNullOrEmpty() || !_extraText.IsNullOrEmpty(); } }

        private const int _distance = 31;

        private int _bufferLength;
        private bool[] _bufferLengthInBits;
        private int _bufferDeltaLength;
        private List<LZ77Letter> _buffer;
        private int _deltaDistance = 0;
        private List<LZ77Letter> _text;
        private List<int> _repeatsIndexes = new List<int>();
        private List<LZ77Letter> _extraText;

        public LZ77GroupInfo(int bufferLength)
        {
            _bufferLength = bufferLength;
            _bufferDeltaLength = 0;
            _buffer = new List<LZ77Letter>();
            _bufferLengthInBits = CreateBufferLengthBits(bufferLength);
            _text = new List<LZ77Letter>();
        }

        private bool[] CreateBufferLengthBits(int bufferLength) => bufferLength switch
        {
            2 => [false, false],
            3 => [false, true],
            4 => [true, false],
            5 => [true, true],
            _ => throw new ArgumentOutOfRangeException(nameof(bufferLength), $"Not expected bufferLength value: {bufferLength}")
        };

        public bool TryAdd(byte fByte, byte sByte)
        {
            if (_bufferDeltaLength < _bufferLength)
            {
                _buffer.Add(new LZ77Letter(fByte, sByte));
                _bufferDeltaLength++;
                return true;
            }

            if (_deltaDistance < _distance)
            {
                _text.Add(new LZ77Letter(fByte, sByte));
                _deltaDistance++;
                return true;
            }

            return false;
        }

        public void FillRepeats()
        {
            var buffer = string.Join(",", _buffer.Select(x => x.Letter));
            var text = string.Join(",", _text.Select(x => x.Letter));

            if (!text.Contains(buffer))
                return;

            var index = 0;

            while (index != -1)
            {
                index = text.IndexOf(buffer, index);

                if (index != -1)
                {
                    _repeatsIndexes.Add(index / 8);
                    index += _bufferLength * 8;
                }

                if (index >= text.Length)
                    break;
            }

            if (_repeatsIndexes.IsNullOrEmpty())
                return;

            HasRepeats = true;
        }

        public LZ77CompressedGroupInfo[] Compress()
        {
            var result = new List<LZ77CompressedGroupInfo>();

            _buffer.ForEach(x =>
            {
                x.Bytes.ForEach(k => result.Add(new LZ77CompressedGroupInfo(false, k)));
            });

            for (int i = 0; i < _text.Count; i++)
            {
                if (!_repeatsIndexes.Contains(i))
                {
                    _text[i].Bytes.ForEach(x => result.Add(new LZ77CompressedGroupInfo(false, x)));
                    continue;
                }

                var infoBitsList = new List<bool>();

                infoBitsList.Add(false);
                infoBitsList.AddRange(i.GetBits().Take(5));
                infoBitsList.AddRange(_bufferLengthInBits);

                var infoBits = new BitArray(infoBitsList.ToArray());

                var infoByte = new byte[1];
                infoBits.CopyTo(infoByte, 0);

                _repeatsIndexes.Remove(i);

                var buffer = string.Join(",", _buffer.Select(x => x.Letter));
                result.Add(new LZ77CompressedGroupInfo(true, infoByte[0], i, _bufferLength, buffer));
                i += _bufferLength - 1;

                if (!_repeatsIndexes.IsNullOrEmpty())
                    continue;

                i++;

                if (i >= _text.Count)
                    break;

                _extraText = _text.Skip(i).ToList();
                break;
            }

            return result.ToArray();
        }
    }
}
