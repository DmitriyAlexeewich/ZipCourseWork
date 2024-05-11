namespace ZipCourseWork.Implementation.Huffman
{
    public class ComressResul
    {
        public ComressResultHeader Header { get; set; }
        public byte[] Compressed { get; set; }
    }

    public class ComressResultHeader
    {
        public HuffmanNode Root { get; set; }
        public int EmptyBitsCount { get; set; }
    }

    public class CompressedFileHeader
    {
        public CompressedHuffmanNode Root { get; set; }
        public int EmptyBitsCount { get; set; }
    }

    public class CompressedHuffmanNode
    {
        public int[] Symbol { get; set; }
        public CompressedHuffmanNode Left { get; set; }
        public CompressedHuffmanNode Right { get; set; }
    }
}
