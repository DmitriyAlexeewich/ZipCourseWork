using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.MTF
{
    public class MTFHeaderLength
    {
        public int Length { get { return _headerLength; } }

        private int _length;
        private List<byte> _bytes = new List<byte>();
        private int _headerLength;
        private bool _isConfigured;

        public MTFHeaderLength(int length)
        {
            _length = length;
        }

        public bool TryAdd(byte source)
        {
            if (_bytes.Count < _length)
            {
                _bytes.Add(source);

                if (_bytes.Count < _length)
                    return true;

                _headerLength = _bytes.GetInt32();
                _isConfigured = true;
                return true;
            }

            return false;
        }
    }

    public class MTFHeaderLetters
    {
        public List<MTFCompressedLetter> Letters { get { return _letters; } }

        private const int _letterLength = 2;

        private MTFHeaderLength _headerLength;
        private List<MTFCompressedLetter> _letters = new List<MTFCompressedLetter>();

        public MTFHeaderLetters(int headerLengthBytesLength)
        {
            _headerLength = new MTFHeaderLength(headerLengthBytesLength);
        }

        public bool TryAdd(byte source)
        {
            if (_headerLength.TryAdd(source))
                return true;

            if (!_letters.Any())
            {
                _letters.Add(new MTFCompressedLetter(_letterLength, source));
                return true;
            }

            if (_letters.Count == _headerLength.Length && _letters.Last().IsConfigured)
                return false;

            var lastLetter = _letters.Last();

            if (lastLetter.TryAdd(source))
                return true;

            if (_letters.Count == _headerLength.Length && _letters.Last().IsConfigured)
                return false;

            _letters.Add(new MTFCompressedLetter(_letterLength, source));
            return true;
        }

        public void MoveToFront(int index)
        {
            var toFront = _letters[index];

            for (int j = index; j > 0; j--)
                _letters[j] = _letters[j - 1];

            _letters[0] = toFront;
        }
    }

    public class MTFCompressedLetter
    {
        public List<byte> Letter { get { return _bytes; } }
        public bool IsConfigured { get { return _bytes.Count == _length; } }

        private int _length;
        private List<byte> _bytes = new List<byte>();

        public MTFCompressedLetter(int length, byte source)
        {
            _length = length;
            _bytes.Add(source);
        }

        public bool TryAdd(byte source)
        {
            if (_bytes.Count < _length)
            {
                _bytes.Add(source);

                return true;
            }

            return false;
        }
    }

    public class MTFLettersCodes
    {
        public List<int> Codes { get { return _codes; } }

        private int _codeLength;
        private List<int> _codes = new List<int>();
        private MTFCompressedLetter _deltaLetter;

        public MTFLettersCodes(int codeLength)
        {
            _codeLength = codeLength;
        }

        public void Add(byte source)
        {
            if (_deltaLetter is null)
            {
                _deltaLetter = new MTFCompressedLetter(_codeLength, source);
                return;
            }

            if (_deltaLetter.TryAdd(source))
                return;

            var code = _deltaLetter.Letter.GetInt32();
            _codes.Add(code);

            _deltaLetter = new MTFCompressedLetter(_codeLength, source);
        }

        public void ConfigLast()
        {
            var bytes = _deltaLetter.Letter.Take(_codeLength).ToArray();

            if (bytes.IsNullOrEmpty())
                return;

            var code = bytes.GetInt32();
            _codes.Add(code);
        }
    }
}
