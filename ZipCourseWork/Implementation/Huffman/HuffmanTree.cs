using System.Collections;
using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.Huffman
{
    public class HuffmanTree
    {
        private bool _hasEmptyByte;
        private Dictionary<char, HuffmanNode> _lettersWithFrequency = new Dictionary<char, HuffmanNode>();
        private string _text = "";

        public void SetEmptyBytesCount() => _hasEmptyByte = true;

        public void AddToCompress(char letter)
        {
            _text += letter;

            if (_lettersWithFrequency.ContainsKey(letter))
            {
                _lettersWithFrequency[letter].AddFrequency();
                return;
            }

            _lettersWithFrequency.Add(letter, new HuffmanNode(letter, 1));
        }

        public void Build()
        {
            if (_lettersWithFrequency.IsNullOrEmpty())
                return;

            var letters = _lettersWithFrequency.Values.ToList();

            while (letters.Count > 1)
            {
                var lettersPairWithMinFrequency = letters.OrderBy(x => x.Frequency).ToArray();

                if (lettersPairWithMinFrequency.Length >= 2)
                {
                    var parent = new HuffmanNode();

                    parent.SetLeft(lettersPairWithMinFrequency[0]);
                    parent.SetRight(lettersPairWithMinFrequency[1]);
                    letters.Add(parent);

                    letters.Remove(lettersPairWithMinFrequency[0]);
                    letters.Remove(lettersPairWithMinFrequency[1]);
                }
            }

            if (letters.IsNullOrEmpty())
                return;

            _lettersWithFrequency.Values.ForEach(x => x.CompressInfo());
        }

        public byte[] Compress()
        {
            var bits = new List<bool>();

            if (_lettersWithFrequency.IsNullOrEmpty())
                throw new NullReferenceException(nameof(_lettersWithFrequency));

            _lettersWithFrequency.Values.ForEach(x => bits.AddRange(x.InfoBits));

            foreach (var letter in _text)
                bits.AddRange(_lettersWithFrequency[letter].Path);

            var emptyBitsCount = 8 - bits.Count % 8;

            for (var i = 0; i < emptyBitsCount; i++)
                bits.Add(false);

            bits = CompressLength(emptyBitsCount).Concat(bits).ToList();
            var resultInBits = new BitArray(bits.ToArray());

            var result = new byte[bits.Count / 8];
            resultInBits.CopyTo(result, 0);

            return result;
        }

        private List<bool> CompressLength(int emptyBitsCount)
        {
            var result = new List<bool>();

            switch (emptyBitsCount)
            {
                case 1:
                    result.Add(false);
                    result.Add(false);
                    result.Add(true);
                    break;
                case 2:
                    result.Add(false);
                    result.Add(true);
                    result.Add(false);
                    break;
                case 3:
                    result.Add(false);
                    result.Add(true);
                    result.Add(true);
                    break;
                case 4:
                    result.Add(true);
                    result.Add(false);
                    result.Add(false);
                    break;
                case 5:
                    result.Add(true);
                    result.Add(false);
                    result.Add(true);
                    break;
                case 6:
                    result.Add(true);
                    result.Add(true);
                    result.Add(false);
                    break;
                case 7:
                    result.Add(true);
                    result.Add(true);
                    result.Add(true);
                    break;
                default:
                    result.Add(false);
                    result.Add(false);
                    result.Add(false);
                    break;
            }

            var pathLengthBytes = BitConverter.GetBytes(_lettersWithFrequency.Count).RemoveExtraZero();

            result.AddRange(FourLengthToBits(pathLengthBytes.Length));
            result.Add(_hasEmptyByte);

            result.Add(false);
            result.Add(false);

            result.AddRange(pathLengthBytes.GetBits());

            return result;
        }
        
        private bool[] FourLengthToBits(int length) => length switch
        {
            1 => [false, false],
            2 => [false, true],
            3 => [true, false],
            _ => [true, true]
        };
    }
}
