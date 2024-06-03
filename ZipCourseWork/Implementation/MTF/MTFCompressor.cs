using System.Linq;
using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.MTF
{
    public class MTFCompressor
    {
        private bool _hasEmptyByte = false;
        private Dictionary<string, MTFLetter> _alphabet = new Dictionary<string, MTFLetter>();
        private MTFLetter[] _letters;
        private List<string> _source = new List<string>();

        public void SetHasEmptyByte() => _hasEmptyByte = true;

        public void Add(MTFLetter letter)
        {
            if(!_alphabet.ContainsKey(letter.Letter))
                _alphabet.Add(letter.Letter, letter);

            _source.Add(letter.Letter);
        }

        public byte[] Compress()
        {
            var result = new List<int>();
            SetDefaultSymbolsByAlphabet();

            foreach (var item in _source)
            {
                for (var i = 0; i < _letters.Length; i++)
                {
                    if (_letters[i].Letter == item)
                    {
                        result.Add(i);
                        MoveToFront(i);
                        break;
                    }
                }
            }

            var headerLengthBytes = BitConverter.GetBytes(_alphabet.Values.Count).RemoveExtraZero();
            var headerLengthBits = LengthToBits(headerLengthBytes.Length);

            var maxResultLength = BitConverter.GetBytes(result.Max()).RemoveExtraZero().Length;
            var maxResultLengthBits = LengthToBits(maxResultLength);

            var infoBits = new bool[8];
            infoBits[0] = _hasEmptyByte;
            infoBits[1] = headerLengthBits[0];
            infoBits[2] = headerLengthBits[1];
            infoBits[3] = maxResultLengthBits[0];
            infoBits[4] = maxResultLengthBits[1];

            var bytes = new List<byte>() { infoBits.GetByte() };
            bytes.AddRange(headerLengthBytes);

            _alphabet.Values.ForEach(x => bytes.AddRange(x.Bytes));
            result.ForEach(x =>
            {
                var letterBytes = BitConverter.GetBytes(x).Take(maxResultLength);
                bytes.AddRange(letterBytes);
            });

            return bytes.ToArray();
        }

        private void SetDefaultSymbolsByAlphabet() => _letters = _alphabet.Values.ToArray();

        private void MoveToFront(int charIndex)
        {
            var toFront = _letters[charIndex];

            for (int j = charIndex; j > 0; j--)
                _letters[j] = _letters[j - 1];

            _letters[0] = toFront;
        }

        private bool[] LengthToBits(int length) => length switch
        {
            1 => [false, false],
            2 => [false, true],
            3 => [true, false],
            4 => [true, true],
            _ => throw new NotSupportedException(nameof(length))
        };
    }
}
