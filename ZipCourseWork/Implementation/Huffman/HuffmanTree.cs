using System.Collections;
using System.Text;
using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.Huffman
{
    public class HuffmanTree
    {
        private HuffmanNode _root = new HuffmanNode();
        private Dictionary<char, HuffmanNode> _lettersWithFrequency = new Dictionary<char, HuffmanNode>();

        public void Build(string source)
        {
            _lettersWithFrequency = new Dictionary<char, HuffmanNode>();

            if (source.IsNullOrEmpty())
                return;

            foreach (char c in source)
            {
                if (_lettersWithFrequency.ContainsKey(c))
                {
                    _lettersWithFrequency[c].AddFrequency();
                    continue;
                }

                _lettersWithFrequency.Add(c, new HuffmanNode(c, 1));
            }

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

            _lettersWithFrequency.Values.ForEach(x => x.CalculatePath());

            _root = letters.First();
        }

        public ComressResul Compress(string source)
        {
            var sourceInBools = new List<bool>();

            foreach(var letter in source)
                sourceInBools.AddRange(_lettersWithFrequency[letter].Path);

            var bitsCount = sourceInBools.Count;
            var emptyBits = new List<bool>();

            while (bitsCount % 8 != 0)
            {
                emptyBits.Add(false);
                bitsCount++;
            }

            sourceInBools.AddRange(emptyBits);

            var sourceInBits = new BitArray(sourceInBools.ToArray());

            var bytes = new byte[bitsCount / 8];

            sourceInBits.CopyTo(bytes, 0);

            var result = new ComressResul()
            {
                Header = new ComressResultHeader()
                {
                    Root = _root,
                    EmptyBitsCount = emptyBits.Count,
                },
                Compressed = bytes
            };

            return result;
        }

        public byte[] Uncompress(byte[] source, CompressedFileHeader compressedFileHeader)
        {
            var bits = new BitArray(source);
            var bitsInBools = new bool[bits.Length];

            bits.CopyTo(bitsInBools, 0);
            bitsInBools = bitsInBools.Take(bitsInBools.Length - compressedFileHeader.EmptyBitsCount).ToArray();

            var result = new List<byte>();
            var currentNode = compressedFileHeader.Root;

            foreach (bool bit in bitsInBools)
            {
                if (bit)
                {
                    if(currentNode.Right != null)
                        currentNode = currentNode.Right;
                }
                else
                {
                    if (currentNode.Left != null)
                        currentNode = currentNode.Left;
                }

                if (currentNode.Left is null && currentNode.Right is null)
                {
                    var bytes = ParseIntToByte(currentNode.Symbol);
                    result.AddRange(bytes);
                    currentNode = compressedFileHeader.Root;
                }
            }

            return result.ToArray();
        }


        private byte[] ParseIntToByte(int[] source)
        {
            var result = new List<byte>();

            foreach (int item in source)
                result.Add((byte)item);

            return result.ToArray();
        }
    }
}
