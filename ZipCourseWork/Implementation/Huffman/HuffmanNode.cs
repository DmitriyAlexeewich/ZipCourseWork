using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.Huffman
{
    public class HuffmanNode
    {
        public int Frequency { get { return _frequency; } }
        public bool[] Path { get { return _path; } }
        public HuffmanNode Parent { get { return _parent; } }
        public bool[] InfoBits { get { return _infoBits; } }

        private char _letter;
        private int _frequency;
        private HuffmanNode _left;
        private HuffmanNode _right;
        private HuffmanNode _parent;
        private bool[] _path;
        private bool[] _infoBits;

        public HuffmanNode() { }

        public HuffmanNode(char letter, int frequency)
        {
            _letter = letter;
            _frequency = frequency;
        }

        public void AddFrequency() => _frequency++;

        public void SetLeft(HuffmanNode left) => SetChildNode(left, true);

        public void SetRight(HuffmanNode right) => SetChildNode(right);

        public bool IsChildRight(HuffmanNode child) => _right == child;

        private void SetChildNode(HuffmanNode node, bool isLeft = false)
        {
            if (isLeft)
                _left = node;
            else
                _right = node;

            _frequency += node.Frequency;

            node.SetParent(this);
        }

        public void SetParent(HuffmanNode parent) => _parent = parent;

        public void CompressInfo()
        {
            _path = GetPath().ToArray();

            var letterBytes = BitConverter.GetBytes(_letter);
            var lengthBytes = BitConverter.GetBytes(_path.Length).RemoveExtraZero();

            var result = new List<bool>();

            result.AddRange(CalculateLength(letterBytes.Length));
            result.AddRange(CalculateLength(lengthBytes.Length));

            result.AddRange(lengthBytes.GetBits());
            result.AddRange(_path);
            result.AddRange(letterBytes.GetBits());

            _infoBits = result.ToArray();
        }

        private bool[] CalculateLength(int bytesLength) => bytesLength switch
        {
            1 => [false, false],
            2 => [false, true],
            3 => [true, false],
            _ => [true, true]
        };

        public List<bool> GetPath()
        {
            if (Parent is null && _left is null && _right is null)
                return new List<bool>() { false };

            var isRight = Parent.IsChildRight(this);

            if (Parent.Parent is null)
                return new List<bool>() { isRight };

            var code = new List<bool>();

            code.AddRange(Parent.GetPath());
            code.Add(isRight);

            return code;
        }

    }
}
