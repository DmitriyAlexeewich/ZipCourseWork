using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.Huffman
{
    public class HuffmanTree
    {
        private Dictionary<char, HuffmanNode> _lettersWithFrequency = new Dictionary<char, HuffmanNode>();
        private string _text = "";

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

            var lengthBytes = BitConverter.GetBytes(_lettersWithFrequency.Count).RemoveExtraZero();

            switch(lengthBytes.Length)
            {
                case 1:
                    result.Add(false);
                    result.Add(false);
                    break;
                case 2:
                    result.Add(false);
                    result.Add(true);
                    break;
                case 3:
                    result.Add(true);
                    result.Add(false);
                    break;
                default:
                    result.Add(true);
                    result.Add(true);
                    break;
            }

            result.Add(false);
            result.Add(false);
            result.Add(false);

            result.AddRange(lengthBytes.GetBits());

            return result;
        }
    
    }
}
