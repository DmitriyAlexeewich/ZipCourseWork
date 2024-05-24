using System.Collections;
using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.LZ77
{
    public class LZ77GroupInfo
    {
        public string FullText { get { return _buffer + _text; } }
        public string ExtraText { get { return _extraText ?? string.Empty; } }
        public bool HasRepeats { get; private set; }
        public bool HasExtraText { get { return !_extraText.IsNullOrEmpty(); } }

        private const int _distance = 31;

        private int _bufferLength;
        private bool[] _bufferLengthInBits;
        private int _bufferDeltaLength;
        private string _buffer;
        private int _deltaDistance = 0;
        private string _text;
        private List<int> _repeatsIndexes = new List<int>();
        private string _extraText;

        public LZ77GroupInfo(int bufferLength)
        {
            _bufferLength = bufferLength;
            _bufferDeltaLength = 0;
            _buffer = string.Empty;
            _bufferLengthInBits = CreateBufferLengthBits(bufferLength);
        }

        private bool[] CreateBufferLengthBits(int bufferLength) => bufferLength switch
        {
            2 => [false, false],
            3 => [false, true],
            4 => [true, false],
            5 => [true, true],
            _ => throw new ArgumentOutOfRangeException(nameof(bufferLength), $"Not expected bufferLength value: {bufferLength}")
        };

        public bool TryAdd(char letter)
        {
            if (_bufferDeltaLength < _bufferLength)
            {
                _buffer += letter;
                _bufferDeltaLength++;
                return true;
            }

            if (_deltaDistance < _distance)
            {
                _text += letter;
                _deltaDistance++;
                return true;
            }

            return false;
        }

        public void FillRepeats()
        {
            if (!_text.Contains(_buffer))
                return;

            var index = 0;

            while (index != -1)
            {
                index = _text.IndexOf(_buffer, index);

                if (index != -1)
                {
                    _repeatsIndexes.Add(index);
                    index++;
                }

                if (index >= _text.Length)
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
                var bytes = BitConverter.GetBytes(x);

                bytes.ForEach(k => result.Add(new LZ77CompressedGroupInfo(false, k)));
            });

            for (int i = 0; i < _text.Length; i++)
            {
                if (!_repeatsIndexes.Contains(i))
                {
                    var notRepeatedBytes = BitConverter.GetBytes(_text[i]);

                    notRepeatedBytes.ForEach(x => result.Add(new LZ77CompressedGroupInfo(false, x)));
                    continue;
                }

                var infoBitsList = new List<bool>();

                infoBitsList.Add(false);
                infoBitsList.AddRange(i.GetBits().Take(5));
                infoBitsList.AddRange(_bufferLengthInBits);

                var infoBits = new BitArray(infoBitsList.ToArray());

                var infoByte = new byte[1];
                infoBits.CopyTo(infoByte, 0);

                if (infoBits[1] == true && infoBits[0] == false && infoBits[2] == false && infoBits[3] == false && infoBits[4] == false && infoBits[5] == false && infoBits[6] == false && infoBits[7] == false)
                {
                    var t = 0;
                    t++;
                }

                _repeatsIndexes.Remove(i);

                result.Add(new LZ77CompressedGroupInfo(true, infoByte[0]));
                i += _bufferLength - 1;

                if (!_repeatsIndexes.IsNullOrEmpty())
                    continue;

                i++;

                if (i >= _text.Length)
                    break;

                _extraText = _text.Substring(i);
                break;
            }

            return result.ToArray();
        }
    }
}
