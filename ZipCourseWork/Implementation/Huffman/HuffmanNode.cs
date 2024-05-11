using System.Text;
using System.Text.Json.Serialization;
using ZipCourseWork.Implementation.Helpers;

namespace ZipCourseWork.Implementation.Huffman
{
    public class HuffmanNode
    {
        public List<int> Symbol { get; private set; }
        public HuffmanNode Left { get; private set; }
        public HuffmanNode Right { get; private set; }

        [JsonIgnore]
        public int Frequency { get; private set; }
        [JsonIgnore]
        public HuffmanNode Parent { get; set; }
        [JsonIgnore]
        public List<bool> Path { get; private set; }

        public HuffmanNode() { }

        public HuffmanNode(char symbol, int frequency)
        {
            var bytes = symbol.GetBytes();

            Symbol = bytes.Select(x => (int)x).ToList();
            Frequency = frequency;
        }

        public void AddFrequency() => Frequency++;

        public void SetLeft(HuffmanNode left) => SetChildNode(left, true);

        public void SetRight(HuffmanNode right) => SetChildNode(right);

        private void SetChildNode(HuffmanNode node, bool isLeft = false)
        {
            if (isLeft)
                Left = node;
            else
                Right = node;

            Frequency += node.Frequency;

            node.Parent = this;
        }

        public void CalculatePath()
        {
            Path = GetPath();
        }

        public List<bool> GetPath()
        {
            if (Parent is null && Left is null && Right is null)
                return new List<bool> { false };

            var isRight = Parent.Right == this;

            if (Parent.Parent is null)
                return new List<bool> { isRight };

            var code = new List<bool>();

            code.AddRange(Parent.GetPath());
            code.Add(isRight);

            return code;
        }

    }
}
